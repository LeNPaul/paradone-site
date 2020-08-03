<?cs include:"doctype.cs" ?>
<html>  <?cs # vim: set syntax=html :?>
<head>

   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Logs</title>
   <?cs include:"browser_compatibility.cs" ?>
   <script type="text/javascript"><!--
   UniqueInstanceId = '<?cs var:UniqueInstanceId ?>';
   --></script>
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("log.css") ?>" />
   
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css") ?>">

   <link rel="stylesheet" type="text/css" href="<?cs var:skin("jquery.treeview.css") ?>" />
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/treeview/treeview.css") ?>" />
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/mapper/node_treeview.css") ?>" />
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/mapper/node_types.css") ?>" />
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/help_popup/help_popup.css") ?>" />
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("syntax_highlight.css") ?>" />
   <!-- Do not add JavaScript files here; add them to jsz/makefile instead. -->
   <?cs include:"browser_js_files.cs" ?>
<script type="text/javascript">
<!--
$(document).ready(function() {
   MiniLogin.init('<?cs var:js_escape(CurrentUser) ?>',
                  '<?cs var:js_escape(DefaultPassword) ?>');
});

<?cs include:"log_browser_js.cs" ?>

function onWindowResize()
{
   resize(VisibleArea);
   DescriptionAreaControl.resizeFrames();
   if (visible('resubmitDialog'))
   {
      LOGCSresizeResubmitDialog();
   }
}

var CurrentTableArea = 'parsedTableArea';
var LogArea, LogTable;
var ResizeTimeoutId = null;
function onLoad()
{
   initLogBrowser();

   setSelectType(); 

   LogTable = document.getElementById('logTable');
   LogArea  = document.getElementById('logArea');
   resize(LogArea);
   LogArea.onscroll = onScrollLogArea;
   
   window.onresize = function()
   {
      // Some browsers (like Firefox) don't always call the onresize event at the right time.
      // So we will make sure to run the adjustments when the user's mouse stops moving.
      clearTimeout(ResizeTimeoutId);
      ResizeTimeoutId = setTimeout('onWindowResize();', 150);
   }

   var OnScrollTimeout;
   window.onscroll = function() {
      clearTimeout(OnScrollTimeout);
      OnScrollTimeout = setTimeout(window.onresize, 150);
   }

   if (navigator.userAgent != null && navigator.userAgent.indexOf('Firefox') !=-1)
   {
      document.onkeypress = onKeyDown;
   }
   else
   {
      document.onkeydown  = onKeyDown;
   }
   
   onSourceChange();
   LOGCSinitialIncludeSourceLogs = false;
   LOGCSinitialDequeueSourceNames = '';

   // Keeps "Today" and "Yesterday" labels up to date.  Only call this once
   // before any entries are added; it will periodically call itself.
   //
   updateDates();

   checkLogUsageBar();

   setupTimeRangeField('After' , '<?cs var:html_escape(After) ?>');
   setupTimeRangeField('Before', '<?cs var:html_escape(Before)?>');
   $('.ui-datepicker').draggable();
   var ShowTimeRange = !(document.getElementById('AfterInput' ).value == '' &&
                         document.getElementById('BeforeInput').value == '')
   document.getElementById('ShowTimeRange_local').checked = ShowTimeRange;
   document.getElementById('TimeRangeTable').style.display = (ShowTimeRange ? '' : 'none');

   // Prep the fancy clear-buttons.
   //
   var ClearProps = {
      'Filter' : { 'EmptyValue' : ''},
      'AfterDateTime_local' : { 'EmptyValue' : InputTimeFormatText, 'OnClickClear' : ClearDateRange['After'] },
      'BeforeDateTime_local' : { 'EmptyValue' : InputTimeFormatText, 'OnClickClear' : ClearDateRange['Before'] }
   }
   for(var FieldName in ClearProps)
   {
      initClearButton(FieldName, ClearProps[FieldName]);
   }

   <?cs if: !Position ?>
      //
      // This is a normal search of the log, with no particular entry to focus on.
      //
      setFinished('forward', true);
      showMore('reverse');
      <?cs if:OnLoad ?>
         eval('<?cs var:js_escape(OnLoad) ?>');
      <?cs /if ?>
   <?cs else ?>
      //
      // We want to focus on a specific entry, so we do something similar to when
      // we display a related entry search:  We load from that position and highlight it.
      //
      RelatedEntry = {
         Position: '<?cs var:js_escape(Position) ?>',
         Date:     '<?cs var:js_escape(Date) ?>'
      };
      setHighlightEntry(RelatedEntry.Date.replace(/\//g,'') + '-' + RelatedEntry.Position);
      setSearchState('forward', { Position: RelatedEntry.Position - 1, Date: RelatedEntry.Date });
      setFinished('forward', false, 'Searching...');
      setFinished('reverse', true);  // Delay the reverse search.
      showMore('forward');

      // This part waits for the target entry to load, then it expands the view for that
      // entry.  Once the view is expanded, we start the reverse search so that when the
      // user closes the expanded view, they won't have to wait for more entries to load.
      //
      var Flasher = function() {
         if( LogEntries.length > 0 )
         {
            var LastEntry = LogEntries[LogEntries.length - 1];

            setFinished('reverse', false);
            showMore('reverse');

            <?cs if:OnLoad ?>
               eval('<?cs var:js_escape(OnLoad) ?>');
            <?cs else ?>
               showEntry(LastEntry, '', true);
            <?cs /if ?>
         }
         else
         {
            setTimeout(Flasher, 100);
         }
      };
      setTimeout(Flasher, 100);
   <?cs /if ?>
   requestDequeueInfo();
   LOGdoHeartBeat();

   $("#alert-dialog").dialog(
		{
			bgiframe: true,
			autoOpen: false,
         		width:375,
			modal: true,
			buttons:{
				'Close': function() {
					$(this).dialog('close');
				}
			}
		});

   var linkDialogOptions = {
			bgiframe: true,
			autoOpen: false,
			height: 215,
			width:585,
			modal: true,
			buttons: {				
				'Close': function() {
					$(this).dialog('close');
				}
			}
		};

   $("#bookmark-dialog").dialog(linkDialogOptions);
   $("#show-link-dialog").dialog(linkDialogOptions);
   
   LogArea.focus();

   LOGstartFtsUpdatePoll();
}

window.onbeforeunload = function(){
   TOOLtooltipClose();
}
   
//-->
</script>

</head>

<body class="tableft" onLoad="onLoad()">

<?cs set:Navigation.CurrentTab = "Logs" ?>
<?cs include:"header.cs" ?>

<div id="main" style="margin-bottom:0">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <table style="width:100%; border-collapse:collapse"><tr>
               <td style="padding:0; width:100%;">
                  Logs
               </td>
               <td align="right" style="width:0; padding:0 0 0 12px;">
                  <span id="newMessageCount" style="cursor:pointer; white-space:nowrap"
                     onclick="showLog(); LogArea.scrollTop = 0; clearNewMessageCount();">
                  </span>
               </td>
               <td id="disableRemoteWindows" align="right" style="width:0; padding:0 0 0 12px; display:none;">
                  <a href="#" class="button"
                        onmouseover=" TOOLtooltipLink('There may be windows in other sessions viewing the same log entries as this window.<br><br>These windows can be disabled by clicking this button.', null, this);" 
                        onmouseout="TOOLtooltipClose();" 
                        onmouseup="TOOLtooltipClose();"
                        onclick="javascript:LOGdisableRemoteWindows(); this.blur(); return false;">
                     <span><img src='images/icon_disable_window.gif'/>&nbsp;Disable Remote Windows</span>
                  </a>
               </td>
               <td id="oldestAndNewestButtons" align="right" style="width:0; padding:3px 0 0 12px;">
                  <div style="white-space:nowrap;">
                     <img class="nav-button" src="/images/log_end.gif"
                          onmouseover="javascript:this.src='/images/log_end_hover.gif'; TOOLtooltipLink('Display the oldest entry.', undefined, this, { Left: -12 });"
                          onmouseout ="javascript:this.src='/images/log_end.gif'; TOOLtooltipClose();"
                          onmousedown="javascript:this.src='/images/log_end_active.gif';"
                          onmouseup  ="javascript:this.src='/images/log_end_hover.gif';"
                          onclick    ="javascript:LOGdisplayOldestEntry();"
                     />
                     <img class="nav-button" src="/images/log_home.gif"
                          onmouseover="javascript:this.src='/images/log_home_hover.gif'; TOOLtooltipLink('Display the newest entry.', undefined, this, { Left: -12 });"
                          onmouseout ="javascript:this.src='/images/log_home.gif';       TOOLtooltipClose();"
                          onmousedown="javascript:this.src='/images/log_home_active.gif';"
                          onmouseup  ="javascript:this.src='/images/log_home_hover.gif';"
                          onclick    ="javascript:LOGdisplayNewestEntry();"
                     />
                  </div>
               </td>
            </tr></table>
         </td>
      </tr>

      <tr>
         <td>
      
         <?cs if:Service.PanicErrorString ?>
            <span id="panicFailure">
         <?cs else ?>
            <span id="panicFailure" style="display:none">
         <?cs /if ?>
            <div class="error_message" style="margin-bottom:5px">
               <div class="error_heading">
                  FATAL ERROR
               </div>
               <div class="error_text">
                  <span id="panicFailureText"><?cs var:Service.PanicErrorString ?></span>
                  <br />All channels have been stopped.
                  Please address this issue and restart Iguana.
               </div>
            </div>
         </span>

         <?cs if: !Service.PanicErrorString && WarnDiskSpace ?>
            <span id="lowDiskSpaceMessage">
         <?cs else ?>
            <span id="lowDiskSpaceMessage" style="display:none">
         <?cs /if ?>
            <div class="warning_message" style="margin-bottom: 5px;">
               <div class="warning_heading">
                  WARNING
               </div>
               <div class="warning_text">
                  The log partition is running out of space: <?cs var:html_escape(DiskSpaceRemaining) ?> remain.
                  <?cs if: AverageUsage != 0 ?>
                     At the current rate of <?cs var:html_escape(AverageUsage) ?>/day (estimated), the
                     partition is likely to be exhausted before Iguana purges old logs.
                  <?cs /if ?>
                  Free some space on the partition, or adjust
                  the <a href="/settings#Page=log_settings/view" >maximum&nbsp;log&nbsp;age</a>. 
                  See <a href="/log_usage_statistics" >Logs&nbsp;&gt;&nbsp;Log Usage</a> for log usage statistics.
               </div>
            </div>
         </span>
      
         <?cs include:"log_entry_view.cs" ?>

         <div id="workingProgress" style="width:100%; height:100%; display:none">
            <table style="width:100%; height:100%"><tr><td align="center" valign="center">
               <div id="workingProgressDiv"></div>
               <div style="padding-top:15px">
                  <img src="/<?cs var:skin('images/resubmit-spinner.gif') ?>">
               </div>
            </td></tr></table>
         </div>
         <div id="logView">
            <div id="logArea" style="width:100%; overflow-x:hidden; overflow-y:scroll" onmousedown="javascript:this.focus();" tabindex=1>
               <div style="text-align:center; width:100%;height:20px; background-color:yellow; display:none;" id="deleteNotify">
                     <div id="deleteStatus" style="font-weight:bold; padding:3px 0 0 0; width:100%;">
                        <!-- set by ajax -->
                     </div>
               </div>
               <div id="noResults" style="position:relative; width:100%; height:100%; display:none">
	          <div id="noResultsContainer"
		       style="position: absolute; 
		              top:50%; 
			      left: 50%; 
			      height: 300px;
			      margin-top: -150px; 
			      width: 400px;
			      margin-left: -200px">
                  <div class="noResultsString">No matching entries.</div>
		  <div class="searchTermsHelp" id="no_results_search_terms_help">
		        <p>Tips for text searching:</p>
			<ul>
			<li>Search for whole words, e.g., "RADIOLOGY" instead of "RADIO".</li>
			<li>Words are separated by any form of punctuation; to find "JOHN'S", search for "JOHN".</li>
			<li>If searching for substrings (e.g., "RICK" in "FREDERICK") start the search with an exclamation mark (!), e.g., "!RICK". This can be a significantly slower search.</li>
			<li>Regular expressions can also be used when starting the search with an exclamation mark.</li>
			</ul>
		  </div>
		  </div>
               </div>
               <table id="logTable" style="width:100%; table-layout:fixed">
                  <colgroup>
                     <col align="center" style="width:13em">
                     <col>
                  </colgroup>

                  <tr id="forwardProgress" style="background-color:yellow; display:none">
                     <td align="center" colspan="2">
                        <div id="forwardStatus" style="width:100%;">
                           Searching...
                        </div>
                     </td>
                  </tr>

                  <?cs # Entries are AJAXed into here. ?>

                  <tr id="reverseProgress" style="background-color:yellow">
                     <td align="center" colspan="2">
                        <div id="reverseStatus" style="width:100%;">
                           Searching...
                        </div>
                     </td>
                  </tr>
               </table>
            </div>
         </div>
      </td></tr>
   </table>
   <table style="width:100%; margin-top: 0px; margin-bottom: 0px;">
      <tr>
         <td style="width: 65px; padding:0px 0 0 0; cursor:pointer"
                                                onclick="location.href='/log_usage_statistics';"
                                                onmouseover=" TOOLtooltipLink(getLogUsageStatistics(), null, document.getElementById('ToolTipRelocate'));" 
                                                onmouseout="TOOLtooltipClose();" 
                                                onmouseup="TOOLtooltipClose();" align=right>
            <a href="/log_usage_statistics" class="subtle">Log Usage:</a>
         </td>
         <td style="width: 100px;" align=left><img id="ToolTipRelocate" src="images/sort_spacer.gif" style="width:0px;height:0px;position:absolute;left:185px;">
            <div id="ToolTippedCell" class="percent_used_container" style="cursor:pointer" onclick="location.href='/log_usage_statistics'"
                                                onmouseover=" TOOLtooltipLink(getLogUsageStatistics(), null, document.getElementById('ToolTipRelocate'));" 
                                                onmouseout="TOOLtooltipClose();" 
                                                onmouseup="TOOLtooltipClose();">            
            <img id="iguana_usage_bar" class="percent_used" src="/<?cs var:skin("images/percent_bar_green.gif") ?>"/><img id="other_usage_bar" class="percent_used" src="/<?cs var:skin("images/percent_used.gif") ?>"/ ></div>
         </td>
	 <td><div id="log_usage_status_panel" class="subtle"></div>
         </td>
      </tr>
   </table>
</div>

<!-- START SIDE PANEL -->
<div id="side_panel">
   <form method="post" action="/log_browse" name="FilterForm" id="FilterForm"> 
      <input type="hidden" name="DebugMode" value="<?cs var:DebugMode ?>" >
      <input type="hidden" name="RefMsgId"  value="<?cs var:html_escape(RefMsgId) ?>">
      <iframe id="contextWarningFrame" src="/empty.html" style="display:none; position:absolute; width:100%; opacity:0; filter:alpha(opacity=0)"></iframe>
      <div id="contextWarning" style="position:absolute; width:102%; display:none">
         <table style="width:100%; height:100%"><tr><td align="center" valign="center">
            <div>Context View</div>
            <table style="opacity:100; filter:alpha(opacity=100)"><tr><td>
               <a class="button" onClick="hideRelated()"><span>Close</span></a>
            </td></tr></table>
         </td></tr></table>
      </div>
      <table id="side_table" style="z-index:10">
         <tr onClick="showSearch()" style="cursor:pointer"><th id="side_header">
            <div id="searchBarOpen" class="toggle">[-]</div>
            <div id="searchBarClosed" class="toggle" style="display:none">[+]</div>
            Search Criteria
         </th></tr>
         <tr><td class="side_body"><div id="searchBar" class="side_body_div">
            <div id="searchMismatch" style="display:none">
               <img src="/<?cs var:skin('images/icon_warning.gif') ?>" />
               This query does not match the entry shown.
            </div>

            <h4 class="side_title">Text Query</h4>
            <div>
               <table style="width:auto; border-collapse:collapse"><tr>
                  <td style="padding-left:0px;">
                     <table style="padding:0; background:white; border:1px solid #DDDDDD; border-collapse:collapse"><tr>
                        <td style="width:12px; padding:0 2px 0 2px">
                           <img src="/<?cs var:skin("images/icon_search.gif") ?>" style="margin-top:1px" />
                        </td>
                        <td style="padding:0">
                           <input type="text" name="Filter" id="filterInput" value="<?cs var:html_escape(Filter) ?>" style="width:135px; border-style:none; outline:none;" onChange="hideDeleteNotify();"/>
                        </td>
                        <td style="width:12px; padding:0 2px 0 2px">
                           <img id="clearFilter" src="<?cs var:skin("images/ex14.gif") ?>" style="margin-top:1px; cursor:pointer; visibility:hidden" />
                        </td>
                     </tr></table>
                  </td>
                  <td style="padding:0">
                     <a id="textQueryHelpIcon" class="helpIcon" tabindex="100"
                        rel="<?cs include:"search_tip_logs.cs" ?>
                             <a href=&quot;<?cs var:help_link('iguana4_searching_the_logs') ?>&quot; target=&quot;_blank&quot;>Learn More About Searching the Logs</a>
                             <br><a href=&quot;<?cs var:help_link('using_regex_metacharacters') ?>&quot; target=&quot;_blank&quot;>Learn More About Regular Expressions</a></br>"
                        title="Text Query Syntax" href="#" onclick="initializeHelp(this,event);">
                        <img src="/<?cs var:skin("images/help_icon.gif") ?>" style="margin-top:1px" border="0" />
                     </a>
                  </td>
               </tr></table>
               <div id="queryError" style="display:none">
                  <img src="/<?cs var:skin('images/icon_warning.gif') ?>" />
                  <span id="queryErrorText"></span>
               </div>
            </div>

            <h4 class="side_title">Channel</h4>
            <select name="Source" id="SourceInput" style="width:100%" onChange="onSourceChange(); hideDeleteNotify(); refreshSearch()" class="hidden">
               <?cs def:add_source(name,value) ?>
                  <?cs if: value == Source ?>
                     <option value="<?cs var:html_escape(value) ?>" selected><?cs var:html_escape(name) ?></option>
                  <?cs else ?>
                     <option value="<?cs var:html_escape(value) ?>"><?cs var:html_escape(name) ?></option>
                  <?cs /if ?>
               <?cs /def ?>
               <?cs call:add_source('[All Entries]',     SourceOptions.AllEntriesOption) ?>
               <?cs call:add_source('[Service Entries]', SourceOptions.ServiceEntriesOption) ?>
               <?cs each: channel = SourceOptions.Channels ?>
                  <?cs call:add_source(channel.Name, channel.Name) ?>
               <?cs /each ?>
            </select>

            <div id="includeSourceLogsDiv" style="display:none; padding:0 0 0 0;">
            <table style="width:auto; border-collapse:collapse">
               <tr><td style="padding:0;">
               <h4 class="side_title">Source Channels</h4>
               </td><td style="padding:0 2 0 0; height: 5px; text-align: center;">
                     <a id="textQueryHelpIcon" class="helpIcon" tabindex="100"
                        rel="The selected channel has a 'From Channel' component. <p>Select the source channels here to include messages from those channels. <p>'Go to Source Position' may be clicked only if one source channel is selected."
                        title="Source Channels" href="#" onclick="initializeHelp(this,event);">
                        <img src="/<?cs var:skin("images/help_icon.gif") ?>" style="margin-top:1px" border="0" />
                     </a>
               </td></tr>
           </table>
               <div id="DequeueSourceNameDiv" style="width:100%; padding:0 0 0 0"><!-- filled by js --></div>
            </div>

            <div id="JumpToQueueEndDiv" style="display:none;" >
              <a href="#" onClick="jumpToQueueEnd(); return false;">Go to Source Position</a>
            </div>

            <table style="width:100%; border-collapse:collapse">
               <tr style="padding: 0 0 0 0;">
               <td class="side_title" style="padding: 0 0 0 0; width: 80px;">
                  <h4 class="side_title">Time Range</h4>
               </td>
               <td style="padding: 6px 0 0 0; text-align: left;">
                  <input type="checkbox" name="ShowTimeRange_local" id="ShowTimeRange_local" class="no_style" onchange="showTimeRange()" onclick="showTimeRange()" onkeyup="showTimeRange()">
               </td>
	       <td style="padding: 6px 0 0 0; text-align:right">
                   <a id="dateQueryHelpIcon" class="helpIcon" tabindex="100"
                        rel="This limits messages to the given time range. Example date/times: 
			<ul>
			<li>-1 h (1 hour ago)</li>
			<li>-1 min (1 minute ago)</li>
			<li>now</li>
			<li>yesterday</li>
			<li>6/4/2005 22:30:45</li>
			</ul>
			"
                        title="Time Range" href="#" onclick="initializeHelp(this,event);">
                        <img src="/<?cs var:skin("images/help_icon.gif") ?>" style="margin-top:1px" border="0" />
                   </a>
                </td>
            </tr>
            </table>
            <table class="timeRange" style="display: none;" id="TimeRangeTable">
               <tr>
                  <td class='timeLabel' valign="top">from</td>
                  <td style="padding: 0 0 5px 0">
                  <table class="timeRangeField">
		     <tr>
                     <td class="left_cell"><input type='text' class='DateTimeField' name='AfterDateTime_local' id='AfterDateTime_local'/></td>
                     <td class="mid_cell"><img id="clearAfterDateTime_local" src="<?cs var:skin("images/ex14.gif") ?>" style="margin-top:1px; cursor:pointer; visibility:hidden" /></td>
		     <td class="right_cell"><img style="float:right" id="calendarImgAfter" src="<?cs var:skin("images/calendar.gif") ?>"></td>
                     </tr>
                  </table>
		  <div class='DateTimePreview' style="display: none" id='AfterDateTimePreview'></div>
		  </td>
                  <td><input type='hidden' name='After' id='AfterInput' value=''/></td>
               </tr>
               <tr>
                  <td class='timeLabel' valign="top">to</td>
                  <td>
                  <table class="timeRangeField">
		     <tr>
                     <td class="left_cell"><input type='text' class='DateTimeField' name='BeforeDateTime_local' id='BeforeDateTime_local'/></td>
                     <td class="mid_cell"><img id="clearBeforeDateTime_local" src="<?cs var:skin("images/ex14.gif") ?>" style="margin-top:1px; cursor:pointer; visibility:hidden" /></td>
		     <td class="right_cell"><img style="float:right" id="calendarImgBefore" src="<?cs var:skin("images/calendar.gif") ?>"></td>
                     </tr>
                  </table>
		  <div class='DateTimePreview' style="display: none" id='BeforeDateTimePreview'></div>
		  </td>
                  <td style='padding:0px;'><input type='hidden' name='Before' id='BeforeInput' value=''/></td>
               </tr>
            </table>

            <h4 class="side_title">Type</h4>
            <select MULTIPLE size=6 name="Type" id="Type" onChange="hideDeleteNotify();refreshSearch();"  class="hidden" style="width:195px;">
               <?cs def:add_type(name,value,selected) ?>
                  <?cs if:(value == TypeFilter) || selected ?>
                     <option value="<?cs var:html_escape(value) ?>" selected><?cs var:html_escape(name) ?></option>
                  <?cs else ?>
                     <option value="<?cs var:html_escape(value) ?>"><?cs var:html_escape(name) ?></option>
                  <?cs /if ?>
               <?cs /def ?>
               <?cs call:add_type('All (except Audit)', '',0) ?>
               <?cs call:add_type('Messages', 'messages',0) ?>
               <?cs call:add_type('ACK Messages', 'ack_messages',0) ?>
               <?cs if:TypeFilter=='errors'?> <!-- special case for older uris -->
                  <?cs set:select_all_errors=1 ?>
               <?cs /if ?>
               <?cs call:add_type('Errors - Marked', 'errors_marked',select_all_errors) ?>
               <?cs call:add_type('Errors - Unmarked', 'errors_unmarked',select_all_errors) ?>
               <?cs call:add_type('Warnings', 'warnings',0) ?>
               <?cs call:add_type('Successes', 'successes',0) ?>
               <?cs call:add_type('Informational', 'info',0) ?>
               <?cs call:add_type('Debug', 'debug',0) ?>
               <?cs call:add_type('Audit', 'audit',0) ?>
               <?cs if:DebugMode == "true" ?>
                  <?cs call:add_type('Channel Pos.', 'channel_position',0) ?>
                  <?cs call:add_type('Queue Meta', 'queue_meta',0) ?>
                  <?cs call:add_type('File Consumer Idx.', 'file_consumer_index',0) ?>
                  <?cs call:add_type('File Producer Pos.', 'file_producer_position',0) ?>
                  <?cs call:add_type('Delete Message', 'delete_message',0) ?>
                  <?cs call:add_type('Ack Meta Message', 'ack_meta_message',0) ?>
               <?cs /if ?>
            </select>

            <p><select name="Deleted" id="Deleted" onChange="hideDeleteNotify(); refreshSearch();" style="width:195px;">
               <option value="false" <?cs if:Deleted=='false' ?>selected<?cs /if ?> >Hide Deleted</option>
               <option value="both" <?cs if:Deleted=='both' || Deleted=='' ?>selected<?cs /if ?> >Show Deleted</option>
               <option value="true" <?cs if:Deleted=='true' ?>selected<?cs /if ?> >Show Only Deleted</option>
            </select></p>
            
            <table id="saveButtonView" style="width:100%">
               <tr><td align="right" style="padding:10px 0 0 0">
                  
                     <a class="action-button blue" style="float:right;" onClick="saveSearchLink('search');">Bookmark Search</a>
                  
               </td></tr>
            </table>
            </h4>

         </div></td></tr>
         <tr onClick="hideDeleteNotify(); showExport()" style="cursor:pointer"><th class="side_header">
            <div id="exportBarOpen" class="toggle" style="display:none">[-]</div>
            <div id="exportBarClosed" class="toggle">[+]</div>
            Export
         </th></tr>
         <tr><td class="side_body"><div id="exportBar" style="display:none" class="side_body_div">

            <?cs def:add_format(value,text) ?>
               <input type="radio" class="no_style" id="Export.Format" name="Export.Format" value="<?cs var:value ?>"
                      <?cs if: Export.Format == value ?>checked<?cs /if ?>>
               <span onClick="this.previousSibling.previousSibling.click();">
                  <?cs var:text ?>
               </span><br />
            <?cs /def ?>

            <h4 class="side_title">Format</h4>
            <?cs call:add_format('Plain',     'One entry per line.') ?>
            <?cs call:add_format('Annotated', 'An annotated text file.') ?>
            <?cs call:add_format('CSV',       'A CSV file.') ?>

            <h4 class="side_title">Options</h4>
            <input type="hidden"   name="Export.Plan" value="" />
            <input type="checkbox"  class="no_style" name="Export.Related" id="Export.Related"
               <?cs if: Export.Related ?>checked<?cs /if ?>>
            <span onClick="this.previousSibling.previousSibling.click();">
               Related messages.
            </span>
            <br />

            <table id="exportProgressView" style="width:100%; margin:10px 0 0 0; display:none">
               <tr><td id="exportProgressArea" style="padding:0; border:1px solid #757671">
                  <img id="exportProgress" class="progress_bar" style="width:0px"
                       src="/<?cs var:skin('images/pulsing_progress_bar.gif') ?>" />
               </td></tr>
            </table>
            <table id="exportButtonView" style="width:100%">
               <tr><td align="right" style="padding:10px 0 0 0">
                  <a class="action-button blue" style="float:right" onClick="hideDeleteNotify(); exportSearch()">Export</a>
                  <a id="export-to-mapper" class="action-button blue" style="float:right">Export to Translator</a><br>
                  <a class="action-button blue" style="float:right;" onClick="saveSearchLink('export');">Bookmark Export</a>
               </td></tr>
            </table>
         </div></td></tr>
      </table>

      <table id="side_table" style="z-index:10">
         <tr><th id="side_header">
            Help Links
         </th></tr>
         <tr><td id="side_body" style="padding-top:15px">
            <ul class="help_link_icon" style="margin:0">
               <li>
               <a href="<?cs var:help_link('iguana4_viewing_logs') ?>" target="_blank">Viewing the Logs</a>
               </li>
               <li>
               <a href="<?cs var:help_link('iguana4_exporting_the_logs') ?>" target="_blank">Exporting the Logs</a>
               </li>
            </ul>
         </td></tr>
      </table>

   </form>
</div>
<!-- END SIDE PANEL -->

<?cs call:defineHelpTooltipDiv() ?>

<div id="repo-dialog" style="display:none" title="Reposition Channel"></div>

<div id="bookmark-dialog" title="Bookmark This URL" style="display:none;">
   <div id="bookmark-text"></div>
	<fieldset>
		<label for="bookmarkLink"><b>URL:</b></label>
		<input type="text" name="bookmarkLink" id="bookmarkLink" class="text ui-widget-content ui-corner-all" />		
	</fieldset>
</div>

<div id="show-link-dialog" title="Link To This Message" style="display:none;">
   <div id="show-link-dialog-text"></div>
	<fieldset>
		<label for="messageLink"><b>URL:</b></label>
		<input type="text" name="messageLink" id="messageLink" class="text ui-widget-content ui-corner-all" />		
	</fieldset>
</div>


<div id="alert-dialog" title="Alert" style="display:none;">
   <div id="alert-dialog-text"></div>
</div>

<iframe id="resubmitFrame" src="/empty.html" style="display:none;">
</iframe>

<div id="resubmitDialog" style="display:none;">

   <div id="resubmitTopControls" style="width:100%; float:left;">
      <div style="float:left;">
         Resubmit To Channel:
         <select id="Destination" name="Destination" onchange="onPreviewDestinationChanged(this);">
            <?cs call:add_channel('[Choose Channel]', '')?>
            <?cs each: channel = SourceOptions.Channels ?>
               <?cs call:add_channel(channel.Name, channel.Name) ?>
            <?cs /each ?>
         </select>
      </div>
      <div style="padding-top: 10px; float: left;" id="resubmit_dest_help_note"></div>
      <div style="float:right;" class="resubmit_toolbar_heading">
         <a href="#" onClick="javascript:hideResubmit(); return false;">
            <img src="<?cs var:skin('images/close_button.gif') ?>" style="border:none;"/>
         </a>
      </div>
   </div>
   
   <div id="resubmitEditControls" class="resubmit_toolbar_heading" style="width:100%; float:left; padding-left:<?cs var:#ResubmitDialogPadding ?>px;">
      <a id="toggleResubmitEditLink" href="#" onclick="javascript:toggleResubmitEdit(); return false;">[+] Edit</a>
   </div>

   <div id="resubmitEdit" style="width:100%; float:left; padding-left:<?cs var:#ResubmitDialogPadding ?>px;">
      <textarea id="messageEditor" wrap="off"
         onkeyup="javascript:onResubmitMessageEditorChange();">
      </textarea>
   </div>
   
   <div id="resubmitPreviewControls" style="width:100%; float:left; padding-bottom:2px;">
      <div style="float:left; padding-top:5px; padding-left:<?cs var:#ResubmitDialogPadding ?>px;" class="resubmit_toolbar_heading">
         <a id="toggleResubmitPreviewLink" href="#" onclick="javascript:toggleResubmitPreview(); return false;">[+] Preview</a>
      </div>
      <div id="resubmitPreviewProgress" style="float:left; padding-left:10px;">
         <img src="<?cs var:skin('images/spinner.gif') ?>"/>
      </div>
      <div id="previewWrapTextControl" style="padding:0 <?cs var:#ResubmitDialogPadding ?>px 0 0; float:right;">
         <img id="previewWrapText" class="button"
            <?cs call:imageTip(wrap_tip) ?>
            src="<?cs var:skin('images/log-browser-nowrap.gif') ?>" />
      </div>
      <div id="previewWrapTextControlGrey" style="padding:0 <?cs var:#ResubmitDialogPadding ?>px 0 0; float:right; display:none;">
         <img id="previewWrapTextGrey" class="button"
            <?cs call:imageTip(wrap_tip_grey) ?>
            src="<?cs var:skin('images/log-browser-nowrap-grey.gif') ?>" />
      </div>
      <div id="entryPreviewViewControlContainer" style="float:right; padding-right:3px;">
         <div class="dropdownButtonWrapper">
            <img id="logPreviewSelectDropdownButton" class="ViewDropdownButton"
               src="<?cs var:skin('images/log_views/dropdown-text.gif') ?>"
               <?cs call:imageTip(preiewView_tip) ?>
               onclick="javascript:toggleDropdown('logPreviewSelectDropdown', 'logPreviewSelectDropdownButton', CurrentPreviewMode);">
            </img>
         </div>
         <div id="logPreviewSelectDropdown" style="display:none; right:<?cs var:#ResubmitDialogPadding + 33 ?>px;"
            onclick="javascript:hideDropdown('logPreviewSelectDropdown', 'logPreviewSelectDropdownButton', CurrentPreviewMode);">
            <a class="logViewDropdownOption Hex" onclick="showHexPreview(false);" <?cs call:imageTip(hexPreview_tip) ?>>
               <img /><span>Hex-dump</span>
            </a>
            <a class="logViewDropdownOption Text" onclick="showTextPreview(false);" <?cs call:imageTip(textPreview_tip) ?>>
               <img /><span>Plain-text</span>
            </a>
            <div class="logViewDropdownDivider messageOption" onclick="cancelBubble(event);"></div>
            <a class="logViewDropdownOption SegmentMessage messageOption" onclick="showParsedPreview('SegmentMessage', 'Preserve');" <?cs call:imageTip(parsedPreviewSgm_tip) ?>>
               <img /><span>Segment View</span>
            </a>
            <a class="logViewDropdownOption SegmentGrammar parseOption" onclick="showParsedPreview('SegmentGrammar', 'Preserve');" <?cs call:imageTip(parsedPreviewSgG_tip) ?>>
               <img /><span>Segment Grammar View</span>
            </a>
            <div class="logViewDropdownDivider parseOption" onclick="cancelBubble(event);"></div>
            <a class="logViewDropdownOption Table parseOption" onclick="showParsedPreview('Table', 'Preserve');" <?cs call:imageTip(parsedPreviewTbl_tip) ?>>
               <img /><span>Table View (Graphical)</span>
            </a>
            <a class="logViewDropdownOption TableText parseOption" onclick="showParsedPreview('TableText', 'Preserve');" <?cs call:imageTip(parsedPreview_tip) ?>>
               <img /><span>Table View (Plain-text)</span>
            </a>
            <a class="logViewDropdownOption SQL parseOption" onclick="showParsedPreview('SQL', 'Preserve');" <?cs call:imageTip(parsedPreviewSQL_tip) ?>>
               <img /><span>SQL View</span>
            </a>
            <div class="logViewDropdownDivider pluginOption" onclick="cancelBubble(event);"></div>
            <a class="logViewDropdownOption PluginOutput pluginOption" onclick="showPluginOutputPreview(false);" <?cs call:imageTip(pluginPreview_tip) ?>>
               <img /><span>Plugin Output</span>
            </a>
            <div class="logViewDropdownDivider mapperOption" onclick="cancelBubble(event);"></div>
            <a class="logViewDropdownOption MapperOutput mapperOption" onclick="LOGshowMapperOutputPreview(false);" <?cs call:imageTip(mapperPreview_tip) ?>>
               <img /><span>Translator Output</span>
            </a>
         </div>
      </div>
      <div class="preview_mode_text" style="float:right;">
         Preview Mode:
      </div>        
   </div>

   <div id="resubmitPreview" style="float:left; margin-left:<?cs var:#ResubmitDialogPadding ?>px;">
      <div class="entryViewPleaseWaitBar">Processing...</div>
   </div>
   
   <div id="resubmitComplete" style="float:left;">
      <div>
         <center>
            <table style="border-collapse:collapse"><tr><td style="padding:0">
               <a class="action-button-small blue" style="margin-top: 23px" onClick="javascript:resubmit(showResubmit);">Resubmit</a>
            </td><td>
               <a class="action-button-small blue" style="margin-top: 20px" onClick="javascript:hideResubmit();">Cancel</a>
            </td></tr></table>
         </center>
      </div>
   </div>
</div>
<div id="export-to-mapper-error" style="display:none"> </div>
<div id="export-to-mapper-setup" style="display:none">
   <p>
   Select destination:<br><select id="mapper-targets"></select>
   <p>
   <div id="to-mapper-progress">
      <i>Exporting... <img src="/js/mapper/images/spinner.gif"></i>
   </div>
   <div id="to-mapper-finished" style="font-weight:bold"></div>
   <div id="to-mapper-redirect" style="padding-top:5px; display:none">
      View message(s) in
      <a href="/mapper/?Page=OpenEditor&MapperGuid=&Index=">Translator Editor</a>
   </div>
</div>
</body>
</html>
