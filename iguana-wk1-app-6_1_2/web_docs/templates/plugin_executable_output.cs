<?cs include:"doctype.cs" ?>

<html> <?cs # vim: set syntax=html :?>
<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Dashboard &gt; Channel <?cs var:html_escape(Channel.Name) ?> &gt; <?cs var:html_escape(Component) ?>: <?cs var:html_escape(ComponentType) ?> &gt; Executable output</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>

   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css") ?>">
   
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>

   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-1.11.2.min.js"></script>
   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.min.js"></script>
   
   <script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("tooltip.js") ?>"></script>
   <?cs include:"mini-login.cs" ?>

<script type="text/javascript">

var UpdateInterval = 500;

var InitialDataTree = JSON.parse( '<?cs var:js_escape( JsonData ) ?>' );
function adjustPluginExecutableOutputPanelDimensions()
{
   var PluginExecutableOutputPanel = document.getElementById('plugin_executable_output_panel');
   var SidePanel = document.getElementById('side_panel');

   var NewHeight = WINgetWindowHeight() - WINwindowOffsetTop(PluginExecutableOutputPanel) - 40;
   if (	NewHeight < 100 )
   {
      NewHeight = 100;
   }

   var NewWidth = WINwindowOffsetLeft(SidePanel) - 70;
   if ( NewWidth < 200 )
   {
      NewWidth = 200;
   }

   PluginExecutableOutputPanel.style.height = NewHeight + 'px';
   PluginExecutableOutputPanel.style.width = NewWidth + 'px';
}

var resizeTimeoutId;

function onResize()
{
   // Some browsers (like firefox) don't always call onResize() at the right time.
   // So we will make sure to run the adjustments the user's mouse stops moving.
   clearTimeout(resizeTimeoutId);
   resizeTimeoutId = setTimeout("adjustPluginExecutableOutputPanelDimensions();", 150);
}

function displayPluginExecutableStatus( ExecutableStatus, ExecutableOutput )
{
   document.getElementById('plugin_executable_status_state').innerHTML = ExecutableStatus.State;
   document.getElementById('plugin_executable_status_start_time').innerHTML = ExecutableStatus.StartTime;
   document.getElementById('plugin_executable_status_finish_time').innerHTML = ExecutableStatus.FinishTime;
   document.getElementById('plugin_executable_status_exit_code').innerHTML = ExecutableStatus.ExitCode;
   document.getElementById('plugin_executable_output_last_write_time').innerHTML = ExecutableOutput.LastWriteTime;
}

var pluginExecutableOutputBuffer = '';
var pluginExecutableOutputPosition = 0;
var pluginExecutableOutputStartTime = '';

function combinePluginExecutableOutput( ExecutableOutput )
{
   // Incremental updates from Iguana's plugin executable output ring buffer:
   //
   // We want the output shown on the page to match the actual contents
   // of the ring buffer in Iguana, to avoid bogging down up the user's browser with
   // unbounded data, and to ensure that roughly the same dataset will be displayed
   // if the user navigates away from the page and then returns.
   //
   // The above means that not only are we appending to the tail of our local buffer,
   // we are also trimming off of the head.  However, also note that the buffer 
   // cannot be displayed directly, because it needs to have newlines converted to 
   // <br>'s.  On the other hand, we cannot simply use the converted buffer as our
   // local cache, because its length will be inconsistent with the buffer data
   // returned by Iguana (e.g. length( '<br/>' ) != length( '\r\n' )).

   var curEndPosition = pluginExecutableOutputPosition;
   var curStartPosition = pluginExecutableOutputPosition - pluginExecutableOutputBuffer.length;
   var newEndPosition = ExecutableOutput.Position;
   var newStartPosition = ExecutableOutput.Position - ExecutableOutput.Data.length;

   if ( curEndPosition < curStartPosition || curStartPosition < 0 )
   {
      throw "Invalid pluginExecutableOutput state: Position = " + 
	      pluginExecutableOutputPosition + ", Buffer.length = " + pluginExecutableOutputBuffer.length;
   }

   if ( newEndPosition < newStartPosition || newStartPosition < 0 )
   {
      throw "Invalid ExecutableOutput data: Position = " + 
	      ExecutableOutput.Position + ", Data.length = " + ExecutableOutput.Data.length; 
   }

   if ( ExecutableOutput.BufferSize < 0 )
   {
      throw "Invalid ExecutableOutput data: BufferSize = " + BufferSize;
   }

   if ( newEndPosition <= curEndPosition )
   {
      // Nothing new to display
      return;
   }
 
   if ( newStartPosition == curEndPosition ) 
   {
      // Received perfectly abutted dataset ... just append
      pluginExecutableOutputBuffer += ExecutableOutput.Data;
   }
   else if ( newStartPosition < curEndPosition )
   {
      // Received overlapping dataset ... combine the non-overlapping sections
      // This can happen if overlapping AJAX requests are sent (e.g. Actions overlapping Updates)
      pluginExecutableOutputBuffer += ExecutableOutput.Data.substr( curEndPosition - newStartPosition );
   }
   else // newStartPosition > curEndPosition
   {
      // Received disjoint dataset ...  meaning some data was lost ... discard old completely to catch up
      // This can happen if plugin is outputting faster than we're refreshing
      pluginExecutableOutputBuffer = ExecutableOutput.Data;
   }

   if ( pluginExecutableOutputBuffer.length > ExecutableOutput.BufferSize )
   {
      // Trim to match output buffer size in Iguana
      pluginExecutableOutputBuffer = pluginExecutableOutputBuffer.substr( pluginExecutableOutputBuffer.length - ExecutableOutput.BufferSize );
   }

   pluginExecutableOutputPosition = newEndPosition;
}

function escapePluginExecutableOutputBuffer(UnescapedBuffer)
{
   var EscapedBuffer = UnescapedBuffer;

   EscapedBuffer = EscapedBuffer.replace(/;/g, '&#59;')
   EscapedBuffer = EscapedBuffer.replace(/&(?!#59;)/g, '&amp;');
   EscapedBuffer = EscapedBuffer.replace(/</g, '&lt;');
   EscapedBuffer = EscapedBuffer.replace(/>/g, '&gt;');
   EscapedBuffer = EscapedBuffer.replace(/"/g, '&quot;');
   EscapedBuffer = EscapedBuffer.replace(/\r\n/g, '\n');

   return EscapedBuffer;
}

function displayPluginExecutableOutput( ExecutableStatus, ExecutableOutput )
{
   var OutputPanel  = document.getElementById('plugin_executable_output_panel');

   if ( ExecutableStatus.StartTime != pluginExecutableOutputStartTime )
   {
      // New run of the plugin, discard old data
      pluginExecutableOutputBuffer = ExecutableOutput.Data;
      pluginExecutableOutputPosition = ExecutableOutput.Position;
      pluginExecutableOutputStartTime = ExecutableStatus.StartTime;

      OutputPanel.innerHTML = '<pre>' + escapePluginExecutableOutputBuffer(pluginExecutableOutputBuffer) + '</pre>';
   }
   else if ( ExecutableOutput.Data.length > 0 )
   {
      combinePluginExecutableOutput( ExecutableOutput );
      OutputPanel.innerHTML = '<pre>' + escapePluginExecutableOutputBuffer(pluginExecutableOutputBuffer) + '</pre>';
   }
}

function getStatusImageSource( DataTree )
{
   if ( DataTree.Channel.GreenLight )
   {
      return '/<?cs var:skin("images/button-dotgreenv4.gif") ?>';
   }
   else if ( DataTree.Channel.YellowLight )
   {
      return '/<?cs var:skin("images/button-dotyellowv4.gif") ?>';
   }
   else if ( DataTree.Channel.RedLight )
   {
      return '/<?cs var:skin("images/button-dotredv4.gif") ?>';
   }
   else if ( DataTree.Channel.OffLight )
   {
      return '/<?cs var:skin("images/button-dotgrayv4.gif") ?>';
   }
   else
   {
      return '';
   }
}

function displayChannelStatus( DataTree ){
   var HrefChannelStartStop = document.getElementById('hrefChannelStartStop'); 
   if ( HrefChannelStartStop ) {
      if ( DataTree.Channel.IsRunning ) {
	 HrefChannelStartStop.href = "javascript:stopChannel();";
         HrefChannelStartStop.innerHtml = "Stop Channel";

      } else {
	 HrefChannelStartStop.href = "javascript:startChannel();";
         HrefChannelStartStop.href = "Start Channel";
      }
   }

   if ( DataTree.Channel.CountOfError ) {
      document.getElementById('hrefCountofError').innerHTML = DataTree.Channel.CountOfError;
      document.getElementById("hrefCountofError").className = 'error';
      $("#CountOfErrorDiv").css({ backgroundColor:"#FFCDCB", border:"#FFA19C 1px solid", padding:"1px 5px 1px 5px", marginLeft:"-6px" });
   } else {
      document.getElementById('hrefCountofError').innerHTML = '0';
      document.getElementById("hrefCountofError").className = '';
      $("#CountOfErrorDiv").css({ backgroundColor:"", border:"0px", padding:"0px 5px 0px 0px", marginLeft:"0px" });
   }

   if ( DataTree.Channel.LastActivityTimeStamp ) {
      document.getElementById('hrefLastActivity').innerHTML = DataTree.Channel.LastActivityTimeStamp;
   } else {
      document.getElementById('hrefLastActivity').innerHTML = 'N/A';
   }
 
   var StatusImg = document.getElementById('statusImg'); 
   StatusImg.src = getStatusImageSource( DataTree );
   
   statusTooltipText = DataTree.Channel.LiveStatus;
   if (statusTooltipOn == 1) {
      TOOLtooltipRefresh(statusTooltipText, document.getElementById('statusImg'));
   }

   sourceTooltipText = DataTree.Channel.SourceTooltip;
   if (sourceTooltipOn == 1) {
      TOOLtooltipRefresh(sourceTooltipText);
   }

   filterTooltipText = DataTree.Channel.FilterTooltip;
   if (filterTooltipOn == 1) {
      TOOLtooltipRefresh(filterTooltipText);
   }

   destinationTooltipText = DataTree.Channel.DestinationTooltip;
   if (destinationTooltipOn == 1) {
      TOOLtooltipRefresh(destinationTooltipText);
   }
}

// Setting the LiveStatus tooltip for the jollyrancher icon.
var statusTooltipText;
var statusTooltipOn = 0;
function statusTooltip()
{
   var componentStatusImg = document.getElementById('statusImg');
   componentStatusImg.onmouseover = function()
   {
      TOOLtooltipLink(statusTooltipText, function() { statusTooltipOn = 1; }, this);
   };

   componentStatusImg.onmouseout = function()
   {
      TOOLtooltipClose();  
      statusTooltipOn = 0;
   };

   componentStatusImg.onmouseup = function()
   {
      TOOLtooltipClose();
      statusTooltipOn = 0;
   };
}

// Setting the Source tooltip for the component icon.
var sourceTooltipText;
var sourceTooltipOn = 0;
function sourceTooltip()
{
   var componentSourceImg = document.getElementById('sourceImg');
   componentSourceImg.onmouseover = function()
   {
      TOOLtooltipLink
      (
         sourceTooltipText, 
         function() 
         { 
            sourceTooltipOn = 1; 
            componentSourceImg.src = "/images/icon_<?cs var:html_escape(Channel.Source.ShortName) ?>_hover.gif"
         }, 
         this
       );
   };

   componentSourceImg.onmouseout = function()
   {
      TOOLtooltipClose();  
      sourceTooltipOn = 0;
      componentSourceImg.src = "/images/icon_<?cs var:html_escape(Channel.Source.ShortName) ?>.gif"
   };

   componentSourceImg.onmouseup = function()
   {
      TOOLtooltipClose();
      sourceTooltipOn = 0;
      componentSourceImg.src = "/images/icon_<?cs var:html_escape(Channel.Source.ShortName) ?>.gif"
   };
}

// Setting the Filter tooltip for the component icon.
var filterTooltipText;
var filterTooltipOn = 0;
function filterTooltip()
{
   var componentFilterImg = document.getElementById('filterImg');
   componentFilterImg.onmouseover = function()
   {
      TOOLtooltipLink
      (
          filterTooltipText, 
          function() 
          { 
             filterTooltipOn = 1; 
             <?cs if:Channel.UseMessageFilter ?>
                componentFilterImg.src = "/images/arrow_filter_hover.gif"
             <?cs else ?>
                componentFilterImg.src ="/images/arrow_hover.gif"
             <?cs /if ?>         
          }, 
          this
       );
   };

   componentFilterImg.onmouseout = function()
   {
      TOOLtooltipClose();  
      filterTooltipOn = 0;
      <?cs if:Channel.UseMessageFilter ?>
         componentFilterImg.src = "/images/arrow_filter.gif"
      <?cs else ?>
         componentFilterImg.src ="/images/arrow.gif"
      <?cs /if ?>
   };

   componentFilterImg.onmouseup = function()
   {
      TOOLtooltipClose();
      filterTooltipOn = 0;
      <?cs if:Channel.UseMessageFilter ?>
         componentFilterImg.src = "/images/arrow_filter.gif"
      <?cs else ?>
         componentFilterImg.src ="/images/arrow.gif"
      <?cs /if ?>
   };
}

// Setting the Destination tooltip for the component icon.
var destinationTooltipText;
var destinationTooltipOn = 0;
function destinationTooltip()
{
   var componentDestinationImg = document.getElementById('destinationImg');
   componentDestinationImg.onmouseover = function()
   {
      TOOLtooltipLink
      (
          destinationTooltipText, 
          function() 
          { 
             destinationTooltipOn = 1; 
             componentDestinationImg.src = "/images/icon_<?cs var:html_escape(Channel.Destination.ShortName) ?>_hover.gif"
          }, 
          this
       );
   };

   componentDestinationImg.onmouseout = function()
   {
      TOOLtooltipClose();  
      destinationTooltipOn = 0;
      componentDestinationImg.src = "/images/icon_<?cs var:html_escape(Channel.Destination.ShortName) ?>.gif"
   };

   componentDestinationImg.onmouseup = function()
   {
      TOOLtooltipClose();
      destinationTooltipOn = 0;
      componentDestinationImg.src = "/images/icon_<?cs var:html_escape(Channel.Destination.ShortName) ?>.gif"
   };
}

function displayPluginExecutableOutputUpdate( DataTree )
{
   displayPluginExecutableStatus( DataTree.Executable.Status, DataTree.Executable.Output );
   displayPluginExecutableOutput( DataTree.Executable.Status, DataTree.Executable.Output );
   displayChannelStatus( DataTree );
}

function displayPluginExecutableOutputUpdateError( Error )
{
   MiniLogin.show('Iguana is not responding.', requestPluginExecutableOutputUpdate );
}

function requestPluginExecutableOutputUpdate( Action )
{
   var Params = 'Channel=<?cs var:javascript_escape(url_escape(Channel.Name))?>&Component=<?cs var:javascript_escape(url_escape(Component))?>';
   Params += '&OutputStartTime=' + pluginExecutableOutputStartTime;
   Params += '&OutputPosition=' + pluginExecutableOutputPosition;
   if ( Action )
   {
      Params += '&Action=' + Action;
   }

   AJAXpost('plugin_executable_output_update', Params, function(Content, ContentType) {
      if (ContentType.match('application/json')) {
         var DataTree = JSON.parse(Content);
         displayPluginExecutableOutputUpdate(DataTree);
         
         if (!Action) {
            setTimeout("requestPluginExecutableOutputUpdate()", UpdateInterval);
         }
      } else {
         displayPluginExecutableOutputUpdateError('Invalid content type.');
      }
      
   }, displayPluginExecutableOutputUpdateError);
}

function startChannel()
{
   requestPluginExecutableOutputUpdate( 'Start' );
}

function stopChannel()
{
   requestPluginExecutableOutputUpdate( 'Stop' );
}

function clearCountOfError()
{
   requestPluginExecutableOutputUpdate( 'ClearCountOfError' );
}

function doOnLoaded(){
   console.log("About to do onloaded.");
   adjustPluginExecutableOutputPanelDimensions();
   window.onresize = onResize;

   statusTooltip();
   sourceTooltip();
   filterTooltip();
   destinationTooltip();

   displayPluginExecutableOutputUpdate( InitialDataTree );

   requestPluginExecutableOutputUpdate();
}

$(document).ready(function() {
   doOnLoaded();
});
</script>
</head>

<body class="tableft">

<?cs set:Navigation.CurrentTab = "Dashboard" ?>
<?cs include:"header.cs" ?>

<div id="main">
   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/">Dashboard</a> &gt; <a href="/channel#Channel=<?cs var:url_escape(Channel.Name) ?>">Channel <?cs var:html_escape(Channel.Name) ?></a> &gt; <a href="/channel#Channel=<?cs var:url_escape(Channel.Name) ?>&editTab=<?cs var:url_escape(Channel.editTab) ?>"><?cs var:html_escape(Component) ?>: <?cs var:html_escape(ComponentType) ?> </a> &gt; Executable output
         </td>
      </tr>
      <tr>
	 <td id="dashboard_body">
	    <center>
               <div id="plugin_executable_output_panel"></div>
	    </center>
	 </td>
      </tr>
   </table>
</div>

<div id="side_panel">

   <table id="side_table">
      <tr>
         <th id="side_header">
            Control Panel
         </th>
      </tr>

         <tr>
            <td id="side_body" style="padding-bottom:5px;">
               <table id="tableControlPanel" style="width: 100%; margin:0px;padding:0px;">            
                  <tr>
                     <td style="padding:15px 0px 0px 0px;">

                  <div style="margin-bottom:15px; text-align: center;">
                           <?cs if:CurrentUserCanAdmin ?>
                               <a class="action-button blue" href="javascript:startChannel();" id="hrefStartButton">Start Channel</a>
                           <?cs else ?>
                               <span class="config_warning">You do not have the necessary permissions to start this channel.</span>
                           <?cs /if ?>
                  </div>

                        <div style="margin:0px 0px 0px 53px;">
                           <img src="/images/icon_<?cs var:html_escape(Channel.Source.ShortName) ?>.gif" id="sourceImg">
                           <?cs if:Channel.UseMessageFilter ?>
                              <img src="/images/arrow_filter.gif" id="filterImg">
                           <?cs else ?>
                              <img src="/images/arrow.gif" id="filterImg">
                           <?cs /if ?>        
                           <img src="/images/icon_<?cs var:html_escape(Channel.Destination.ShortName) ?>.gif" id="destinationImg">
                        </div>
                     </td>
                  </tr>
               </table>
            </td>
         </tr>

         <tr>
            <td id="side_body">   
               <div class="textrow" style="height:40px;">
                  <h4 class="side_title" style="float:left;">Status</h4>
                  <p style="float:right;margin-top:15px;">
                     <img src="" id="statusImg">
                  </p>
               </div>
               <div class="textrow" style="padding-right:5px;">
                  <p class="alignleft">Last Activity:</p>
                  <p class="alignright"><span id="spnLastActivityTimeStamp"><a href="/log_browse?Source=<?cs var:url_escape(Channel.Name)?>" id="hrefLastActivity">N/A</a></span></p>
               </div>
               <div class="textrow" id="CountOfErrorDiv" style="padding-right:5px;">
                  <p class="alignleft">Errors [<a href="javascript:clearCountOfError();">clear</a>]:</p>
                  <p class="alignright"><span id="spnCountOfError"><a id="hrefCountofError" href="/log_browse?Source=<?cs var:url_escape(Channel.Name)?>&Type=errors">0</a></span></p>
               </div>
            </td>
         </tr>

      <tr>
	 <td id="side_body">

	    <h4 class="side_title"> 
	    	Plugin Executable   
	    </h4> 

            <div class="textrow" style="padding-right:5px;">
	       <p class="alignleft">
		  State:
	       </p>
               <p class="alignright">
		  <span id="plugin_executable_status_state">
		  </span>
	       </p>
            </div>

            <div class="textrow" style="padding-right:5px;">
	       <p class="alignleft">
		  Started:
	       </p>
               <p class="alignright">
		  <span id="plugin_executable_status_start_time">
		  </span>
	       </p>
            </div>

            <div class="textrow" style="padding-right:5px;">
	       <p class="alignleft">
		  Last Output:
	       </p>
               <p class="alignright">
		  <span id="plugin_executable_output_last_write_time">
		  </span>
	       </p>
            </div>

            <div class="textrow" style="padding-right:5px;">
	       <p class="alignleft">
		  Finished:
	       </p>
               <p class="alignright">
		  <span id="plugin_executable_status_finish_time">
		  </span>
	       </p>
            </div>

            <div class="textrow" style="padding-right:5px;">
	       <p class="alignleft">
		  Exit code:
	       </p>
               <p class="alignright">
		  <span id="plugin_executable_status_exit_code">
		  </span>
	       </p>
            </div>
	 </td>
      </tr>
   </table>

   <table id="side_table">
      <tr>
         <th id="side_header">
            Page Help
         </th>
      </tr>
      <tr>
         <td id="side_body">
            <p>
           This page shows the command-line output from a plugin application.  Note that only the most recent <?cs var:MaxBufferSize / 1024 ?>kb of command-line output are displayed.
	    </p>
         </td>
      </tr>
      <tr>
         <td id="side_body">
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_plugin_standard_output') ?>" target="_blank">Monitoring Plugin Output</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
</div>

</body>
</html>
