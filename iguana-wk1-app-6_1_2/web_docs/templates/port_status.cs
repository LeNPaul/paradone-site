<?cs include:"doctype.cs" ?>

<script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
<script type="text/javascript">

// Denotes the containing div and list table respectively.
var VisibleArea = null;
var VisibleTable = null;
var VisibleHeaderArea = null;
var VisibleHeaderTable = null;

// Function that calculates the variable table size from the size of the containing window.
function onResize(Area, Table, HeaderArea, HeaderTable)
{  
   VisibleArea = Area;
   VisibleTable = Table;
   VisibleHeaderArea = HeaderArea;
   VisibleHeaderTable = HeaderTable;

   // New height will a constant top offset and a bottom offset equal to the left offset.
   var NewHeight = WINgetWindowHeight() - WINwindowOffsetTop(VisibleArea) - WINwindowOffsetLeft(VisibleArea);

   if (NewHeight < 100)
   {
      NewHeight = 100;
   }

   VisibleArea.style.height = (NewHeight - 5) + "px";

   // New width will be measure from a constant left offset and an absolute 
   // right offset determined by whether or not a vertical scroll bar is present. 
   var NewWidth = (WINgetWindowWidth() - WINwindowOffsetLeft(VisibleArea));

   if (NewWidth < 500)
   {
      NewWidth = 500;
   }

   if(VisibleArea.scrollHeight > VisibleArea.clientHeight)
   {
      NewWidth = NewWidth - 310;
      VisibleArea.style.width = NewWidth + 18 + "px";   
   }
   else
   {
      NewWidth = NewWidth - 295;
      VisibleArea.style.width = NewWidth + 2 + "px";   
   } 

   // Content table width is smaller by the width of a scroll bar.
   VisibleTable.style.width = NewWidth + "px";

   // Header width should equal the content table width.
   VisibleHeaderTable.style.width = VisibleTable.offsetWidth + "px";

   // Now determine the column widths.
   var PortsFloatingHeader = document.getElementById("thPorts");
   var ChannelsFloatingHeader = document.getElementById("thChannels");
   var StatusFloatingHeader = document.getElementById("thStatus");

   var ChannelsHeader = document.getElementById("Hidden_Channels");
   var PortsHeader = document.getElementById("Hidden_Ports");
   var StatusHeader = document.getElementById("Hidden_Status");
    
   ChannelsFloatingHeader.style.width = (ChannelsHeader.clientWidth - parseInt(WINgetStyle(ChannelsFloatingHeader, "paddingLeft")) - parseInt(WINgetStyle(ChannelsFloatingHeader, "paddingRight"))) + "px"; 
   PortsFloatingHeader.style.width = (PortsHeader.clientWidth - parseInt(WINgetStyle(PortsFloatingHeader, "paddingLeft")) - parseInt(WINgetStyle(PortsFloatingHeader, "paddingRight"))) + "px";
   StatusFloatingHeader.style.width = (StatusHeader.clientWidth - parseInt(WINgetStyle(StatusFloatingHeader, "paddingLeft")) - parseInt(WINgetStyle(StatusFloatingHeader, "paddingRight"))) + "px";

   // Set the table heading width; chop off extra headings (only needed for Firefox).
   VisibleHeaderArea.style.width = VisibleArea.clientWidth + "px";

   document.getElementById("divTableContainer").style.visibility = "visible";
}

// Page load handler.  Uses ClearSilver to populate the variable table. 
function onLoad()
{
   var ListArea  = document.getElementById("divPortList");
   var ListTable = document.getElementById("PortList");
   var ListHeaderArea = document.getElementById("divPortListHeadings");
   var ListHeaderTable = document.getElementById("PortListHeadings");

   if(navigator.appName == "Microsoft Internet Explorer")
   {
      var IEVersion = getIEVersion();

      if(IEVersion < 8.0 )
      {
         ListTable.style.borderCollapse = "collapse";     
         ListHeaderTable.style.borderCollapse = "collapse"; 
      }
   }

   if (ListArea)
   { 
      ListArea.style.height = WINgetWindowHeight();
   }

   <?cs if:NumberOfPorts > 0 ?>    

   // Make sure the header and list have the same width to start off.
   ListHeaderArea.style.width = ListArea.clientWidth  + "px";

   // Resize to fit the window.    
   setTimeout(function() { onResize(ListArea, ListTable, ListHeaderArea, ListHeaderTable); }, 150);

   var OnResizeTimer;
   window.onresize = function() {
      clearTimeout(OnResizeTimer);
      OnResizeTimer = setTimeout(function() { onResize(VisibleArea, VisibleTable, VisibleHeaderArea, VisibleHeaderTable); }, 150);
   };
   window.onscroll = window.onresize;    

   if(1 <= <?cs var:NumberOfPorts ?>)
   {
      var LastRow = document.getElementById("PortList").rows[<?cs var:NumberOfPorts ?>];
      
      for(var ColumnIndex = 0; ColumnIndex < 3; ++ColumnIndex)
      {
         LastRow.cells[ColumnIndex].style.borderBottom = "none";
      } 
   }

   if (ListArea)
   {
      ListArea.onscroll = adjustHeaderOnScroll; 
   }

   <?cs /if ?>                                
}

function getIEVersion()
{
   var Result = -1; 

   var UserAgent = navigator.userAgent;
   var RE = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");

   if (RE.exec(UserAgent) != null)
   {
      Result = parseFloat( RegExp.$1 );
   }
  
   return Result;
}

function adjustHeaderOnScroll()
{   
   VisibleHeaderTable.style.left = - VisibleArea.scrollLeft + "px";
}

</script>

<html>
<head>
<title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Dashboard &gt; Ports</title>
<?cs include:"browser_compatibility.cs" ?>
<?cs include:"styles.cs" ?>
   
<style  type="text/css">

   table.configuration
   {
      width: 100%;
      border-collapse:separate;
     
   }

   table.configuration tr
   {
      height: 30px;
   }

   table.configuration td
   {
      padding-top: 10px;
      padding-bottom: 10px;
      height: 10px;
   }
   
   .fixed_width
   {
      width: 150px;
   }

   .warning
   {
      text-align: left;
      color: #FF0000;
   }
   
   #divPortList
   {
      overflow-y: auto;
      overflow-x: auto;
      text-align: center; 
      font-weight: bold;
      font-size: 10px;
      letter-spacing: 0.05em;
      color: #FFFFFF;
      width:100%;
   }
     
   #PortList
   {  
      width:100%;
   }

   #divPortListHeadings
   {
      position: absolute;
      background-color: #FFFFFF;
      text-align: center; 
      overflow: hidden;
      width:100%;
   }

   #PortListHeadings
   {
      position: relative;
      overflow: hidden;
      width: 100%;
   }
   	  	    	  	  
   #PortListHead
   {
      visibility: hidden;
   }
</style>

</head>

<body class="tableft" onload="javascript:onLoad();">

<?cs set:Navigation.CurrentTab = "Dashboard" ?>
<?cs include:"header.cs" ?>


<div id="main">

<table id="iguana">
   <tr>
      <td id="cookie_crumb">
         <a href="/dashboard.html">Dashboard</a> &gt; Ports
      </td>
   </tr>

   <tr>
      <td id="dashboard_body">
         <div id="divTableContainer" style="text-align: left;">

         <div id="divPortListHeadings">
            <table id="PortListHeadings" class="configuration" >         
               <tr>
                  <th id="thPorts">Port</th>
                  <th id="thChannels">Channel(s) or Service(s)</th>
                  <th id="thStatus">Listening (<?cs var:html_escape(CountOfListeningPort) ?>)</th>
               </tr>
            </table>
         </div>

         <div id="divPortList">   
            <table id="PortList" class="configuration" >
               <tr id="PortListHead">
                  <th id="Hidden_Ports">Port</th>
                  <th id="Hidden_Channels">Channel(s) or Service(s)</th>
                  <th id="Hidden_Status">Listening (<?cs var:html_escape(CountOfListeningPort) ?>)</th>
               </tr>                        
               <?cs each:Port = Ports ?>
               <tr>
                  
                  <td class="fixed_width" style="text-align: center;">
                     <b><?cs name:Port ?></b>
                  </td>
                     
                  <td id="tdChannels.<?cs name:Port ?>">
                     <table>
                     <?cs each:ChannelOrService = Port.ChannelsOrServices ?>
                        <tr><td class="channel_or_service_column">
                           <?cs if:ChannelOrService.IsChannel ?>
                           <?cs if:ChannelOrService.IsForbidden ?>
                           You do not have view permission for a channel that is using this port.
                           <?cs else ?>
                           <a href="/channel#Channel=<?cs var:url_escape(ChannelOrService.Name) ?>">
                           <?cs var:html_escape(ChannelOrService.Name) ?>
                           </a>
                           <?cs /if ?>
                           <?cs if:!ChannelOrService.IsForbidden ?>
                              <?cs if:ChannelOrService.IsRunning ?> (Running)<?cs /if ?>
                              <?cs /if ?>
                           <?cs elif:ChannelOrService.IsWebServer ?>
                              <a href="/settings#Page=web_settings/view"><?cs var:html_escape(ChannelOrService.Name) ?></a>
                           <?cs elif:ChannelOrService.IsRpcServer ?>
                              <a href="/settings#Page=rpc_settings/view"><?cs var:html_escape(ChannelOrService.Name) ?></a>
                           <?cs elif:ChannelOrService.IsHttpsChannelServer ?>
                              <a href="/settings#Page=https_channel_settings"><?cs var:html_escape(ChannelOrService.Name) ?></a>
                           <?cs else ?>
                              <?cs var:html_escape(ChannelOrService.Name) ?>
                           <?cs /if ?>
                        </td></tr>
                     <?cs /each ?>
                     </table>
                  </td>
                     
                  <td class="fixed_width" style="text-align: center;border-right:none;">
                     <?cs if:Port.IsListening ?>
                        <b style="color: green;">Listening</b>
                     <?cs elif:Port.InUseByOtherApplication ?>
                        <b style="color: red;">In use by other application</b>
                     <?cs elif:Port.InUseByWebServer ?>
                        <b style="color: red;">In use by Iguana Web Server</b>
                     <?cs else ?>
                        <span style="color: gray;">Off</span>
                     <?cs /if ?>
                  </td>
                     
               </tr>
               <?cs /each ?>
                  
            </table>
            
         </div>
         
         </div>
         
      </td>
   </tr>
</table>

</div>
<div id="side_panel">
   <table id="side_table">
      <tr>
         <th id="side_header">
            Page Help
         </th>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Overview</h4>
	     <p>
	 This page shows the list of TCP/IP ports used by Iguana. The ports are listed in numeric order from lowest to highest.
	 </p> 
         </td>
      </tr>
   </table>
</div>
</body>

</html>
