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
   var ChannelsFloatingHeader = document.getElementById("thChannels");
   var PortsFloatingHeader = document.getElementById("thPorts");
   var RemoteSystemsFloatingHeader = document.getElementById("thRemoteSystems");
   var RunningStatusFloatingHeader = document.getElementById("thRunningStatus");

   var ChannelsHeader = document.getElementById("Hidden_Channels");
   var PortsHeader = document.getElementById("Hidden_Ports");
   var RemoteSystemsHeader = document.getElementById("Hidden_RemoteSystems");
   var RunningStatusHeader = document.getElementById("Hidden_RunningStatus");
    
   ChannelsFloatingHeader.style.width = (ChannelsHeader.clientWidth - parseInt(WINgetStyle(ChannelsFloatingHeader, "paddingLeft")) - parseInt(WINgetStyle(ChannelsFloatingHeader, "paddingRight"))) + "px"; 
   PortsFloatingHeader.style.width = (PortsHeader.clientWidth - parseInt(WINgetStyle(PortsFloatingHeader, "paddingLeft")) - parseInt(WINgetStyle(PortsFloatingHeader, "paddingRight"))) + "px";
   RemoteSystemsFloatingHeader.style.width = (RemoteSystemsHeader.clientWidth - parseInt(WINgetStyle(RemoteSystemsFloatingHeader, "paddingLeft")) - parseInt(WINgetStyle(RemoteSystemsFloatingHeader, "paddingRight"))) + "px";
   RunningStatusFloatingHeader.style.width = (RunningStatusHeader.clientWidth - parseInt(WINgetStyle(RunningStatusFloatingHeader, "paddingLeft")) - parseInt(WINgetStyle(RunningStatusFloatingHeader, "paddingRight"))) + "px";

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

   <?cs if:NumberOfConsumers > 0 ?>    
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

   if(1 <= <?cs var:NumberOfConsumers ?>)
   {
      var LastRow = document.getElementById("PortList").rows[<?cs var:NumberOfConsumers ?>];
      
      for(var ColumnIndex = 0; ColumnIndex < 4; ++ColumnIndex)
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
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Dashboard &gt; Outbound LLP Ports</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>

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
            <a href="/dashboard.html">Dashboard</a> &gt; Outbound LLP Ports
    </td>
    </tr>
      <tr>
      <td id="otbound_llp_body">
      <?cs if:NumberOfConsumers==0 ?>
         <span id="spnNoConsumersWarning">
            <p style="text-align: center;">
               There are no channels configured to use the <a href="<?cs var:help_link('iguana4_llp_client') ?>" target="_blank">LLP Client</a> component.
            </p>
         </span>
      <?cs else ?>
        <div id="divTableContainer" style="visibility:hidden;"> 
        <div id="divPortListHeadings">
            <table id="PortListHeadings" class="configuration" >
               <tr>
                         <?cs def:smart_heading(type, content) ?>
                           <th
                              id="th<?cs var:type ?>"
                              <?cs if:type == "Channels" ?>
                                        onclick= "document.location='outbound_llp_ports.html?sort=<?cs var:type ?>&order=<?cs var:SortOrder ?>&image=<?cs var:Channels ?>'"
                                    <?cs elif:type == "Ports" ?>
                                        onclick= "document.location='outbound_llp_ports.html?sort=<?cs var:type ?>&order=<?cs var:SortOrder ?>&image=<?cs var:Ports ?>'" 
                                    <?cs elif:type == "RemoteSystems" ?>
                                        onclick= "document.location='outbound_llp_ports.html?sort=<?cs var:type ?>&order=<?cs var:SortOrder ?>&image=<?cs var:RemoteSystems ?>'"  
                                    <?cs elif:type == "RunningStatus" ?>
                                        onclick= "document.location='outbound_llp_ports.html?sort=<?cs var:type ?>&order=<?cs var:SortOrder ?>&image=<?cs var:RunningStatus ?>'"  
                              <?cs /if ?>
                              onmouseover="this.className = 'sortable_hover';"
                              onmouseout="this.className = 'sortable';"                           >
                              <nobr>
                                 <img src="/images/sort_spacer.gif" alt="" />
                                 <?cs var:content ?> 
                                 <?cs if:type == "Channels" ?>
                                          <img src="/<?cs var:skin("images/sort_" + Channels + ".gif") ?>">
                                          (<?cs var:NumberOfConsumers ?>) 
                                     <?cs elif:type == "Ports" ?>
                                          <img src="/<?cs var:skin("images/sort_" + Ports + ".gif") ?>">   
                                     <?cs elif:type == "RemoteSystems" ?>
                                          <img src="/<?cs var:skin("images/sort_" + RemoteSystems + ".gif") ?>">   
                                     <?cs elif:type == "RunningStatus" ?>
                                          <img src="/<?cs var:skin("images/sort_" + RunningStatus + ".gif") ?>">
                                          (<?cs var:NumberOfRunningConsumers ?>)                                          
                                 <?cs /if ?>
                               </nobr>
                           </th>
                         <?cs /def ?>
                  <?cs call:smart_heading('Channels', 'Channel name') ?>
                  <?cs call:smart_heading('Ports', 'LLP Client Port') ?>
                  <?cs call:smart_heading('RemoteSystems', 'Remote System') ?>
                  <?cs call:smart_heading('RunningStatus', 'Running') ?>
              </tr>
            </table>
         </div>
         <div id="divPortList">   
            <table id="PortList" class="configuration" >
               <tr id="PortListHead">
                         <?cs def:hidden_heading(type, content) ?>
                           <th id="Hidden_<?cs var:type ?>" >
                              <nobr>
                                 <img src="/images/sort_spacer.gif" alt="" />
                                 <?cs var:content ?> 
                                 <?cs if:type == "Channels" ?>
                                          <img src="/<?cs var:skin("images/sort_" + Channels + ".gif") ?>">
                                          (<?cs var:NumberOfConsumers ?>) 
                                     <?cs elif:type == "Ports" ?>
                                          <img src="/<?cs var:skin("images/sort_" + Ports + ".gif") ?>">   
                                     <?cs elif:type == "RemoteSystems" ?>
                                          <img src="/<?cs var:skin("images/sort_" + RemoteSystems + ".gif") ?>">   
                                     <?cs elif:type == "RunningStatus" ?>
                                          <img src="/<?cs var:skin("images/sort_" + RunningStatus + ".gif") ?>">
                                          (<?cs var:NumberOfRunningConsumers ?>)                                          
                                 <?cs /if ?>
                               </nobr>
                           </th>
                         <?cs /def ?>
                  <?cs call:hidden_heading('Channels', 'Channel name') ?>
                  <?cs call:hidden_heading('Ports', 'LLP Client Port') ?>
                  <?cs call:hidden_heading('RemoteSystems', 'Remote System') ?>
                  <?cs call:hidden_heading('RunningStatus', 'Running') ?>
              </tr> 
                <?cs each:Consumer = Consumers ?>
					  <tr>
                                                  <td id="tdChannels.<?cs name:Consumer ?>" >

                                                  <a href="/channel#Channel=<?cs var:url_escape(Consumer.Name) ?>">
                                                  <?cs var:html_escape(Consumer.Name) ?>
                                                  </a>      
						  </td>
						  <td class="fixed_width" style="text-align: center;" 
                                                  id="tdConsumerPort.<?cs name:Consumer ?>" >
						  <b><?cs var:html_escape(Consumer.ConsumerPort)?></b>
						  <td class="fixed_width" style="text-align: center;"
                                                  id="tdAddress.<?cs name:Consumer ?>" >
						  <?cs var:html_escape(Consumer.Address)?>
						  </td>
						  <td class="fixed_width" style="text-align: center;border-right:none;"
                                                  id="tdStatus.<?cs name:Consumer ?>" >
                                                        <?cs if:Consumer.Status == '1' ?>
							    <b style="color: green;">Running<br></b>
							<?cs elif:Consumer.Status == '2' ?>	
							    <span style="color: blue;">Attempting connection<br></span>
							<?cs elif:Consumer.Status == '3' ?>	
							    <span style="color: red;">Not connected<br></span>
							<?cs elif:Consumer.Status == '0' ?>	
							    <span style="color: grey;">Off<br></span>
							<?cs /if ?>
						  </td>
					  </tr>
                <?cs /each ?>
            </table>    
         </div>
         </div>
      <?cs /if ?>
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
         This page displays a list of channels whose destination component is LLP Client. For each channel, the outbound LLP port, remote system, and connection status are displayed.
	 </p>
	 <p>
	 To update the channel status, click your browser's Refresh button.
	 </p>
        <p>For information on how to select ports, see
        <a href="<?cs var:help_link('iguana4_port_management_tips') ?>" target="_blank">Port Management Tips</a>. 
        </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
	                <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_channels_transmitting') ?>" target="_blank">Displaying the Outbound LLP Ports</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
</div>
</body>
</html>
