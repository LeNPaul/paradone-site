<?cs include:"doctype.cs" ?>  <?cs # vim: set syntax=html :?>

<html>

<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Logs &gt; Log Usage</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-1.11.2.min.js"></script>
   
   <style type="text/css">
      #divLogFilesList
      {
         overflow-y: auto;
         overflow-x: hidden;
      }

   </style>
   
   <script type="text/javascript">
   function startFtsUpdatePoll() {
      ftsUpdateError = function(error) {
         $('#fts_update_panel').html('');
      }
      
      ftsUpdateComplete = function(Data){
         var Restart = true;
         Status = JSON.parse(Data);
         if (Status) {
            if (Status.status == 'Indexing') {
               var StatusStr = Status.current_file 
                             + ' ('  + Status.percent 
                             + '%, ' + Status.bps      + ')<br>'
                             + Status.eta              + ' left.<br>';

               StatusStr     += (Status.count_of_unindexed > 1 ? Status.count_of_unindexed-1 + ' file(s) remaining.' : '');

               $('#fts_update_panel').html(StatusStr);
               $('#fts_update_row').show();
            } else if (Status.status == 'Complete') {
               $('#fts_update_panel').html('');
               $('#fts_update_row').hide();       
               Restart = false;       
            } else {
               $('#fts_update_panel').html(Status.status);
               $('#fts_update_row').show();
            }
         } else {
            ftsUpdateError();
         }
         
         if (Restart) {
            setTimeout('startFtsUpdatePoll();', 1000);
         }
      }
      AJAXpost('fts_update_status', "", ftsUpdateComplete, ftsUpdateError);
   }

   var resizeTimeoutId; 
   function onResize(){
      // Some browsers (like firefox) don't always call onResize() at the right time.
      // So we will make sure to run the adjustments the user's mouse stops moving.
      clearTimeout(resizeTimeoutId);
      resizeTimeoutId = setTimeout("adjustTableDimensions();", 150);
   }
   
   //helper function to get the padding of an element, which is non-trivial
   //
   function getTotalPadding(Element)
   {
      return parseInt(WINgetStyle(Element, 'paddingLeft')) + parseInt(WINgetStyle(Element, 'paddingRight'));
   }
 
   function adjustTableDimensions()
   {
      var LogListDiv = document.getElementById('divLogFilesList');
      var TableData = document.getElementById('tableData');
      var FloatingHeadings = document.getElementById('divFloatingHeadings');
      var TableHeadings = document.getElementById('tableHeadings');
   
   
      // Table height - fit size to screen
      var NewHeight = (WINgetWindowHeight() - WINwindowOffsetTop(LogListDiv)) - 95;
      if (NewHeight < 75)
      {
         NewHeight = 75;
      }

      // Can't have new height more than the height of the table though + 10 for padding
      //
      var MaxHeight = TableData.clientHeight + 10;
      if (NewHeight > MaxHeight)
      {
         NewHeight = MaxHeight;
      }
      LogListDiv.style.height = NewHeight + 'px';
      
      // Table width - set explicitly to adjust for scrollbars in Internet Explorer
      TableData.style.width = '0px'; 
      TableData.style.width = LogListDiv.clientWidth + 'px';
      
      // Column widths - match the floating heading's columns with the data table's
      var LogDateHeader = document.getElementById('thLogDateHeading');
      var SizeHeader = document.getElementById('thSizeHeading');
       
      LogListDiv.style.width = TableHeadings.offsetWidth + 'px';
      
      // Get first row/col
      //
      var Row = TableData.rows[0].cells;
      var DateCol = Row[0];
      var SizeCol = Row[1];
      
      // Sometimes the width of the Div gets readjusted and gets bigger
      //
      TableData.style.width = LogListDiv.clientWidth + 'px';
      DateCol.style.width = (LogDateHeader.clientWidth  - getTotalPadding(LogDateHeader) )+ 'px';  // MAJIK # - for now, represents the padding,
      
   }
  
   function initialize()
   {
      adjustTableDimensions();
      window.onresize = onResize;
      onResize();
      startFtsUpdatePoll();
   }
   </script>

</head>

<body class="tableft" onLoad="initialize();">

<?cs set:Navigation.CurrentTab = "Logs" ?>
<?cs include:"header.cs" ?>


<div id="main">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="logs.html">Logs</a> &gt; Log Usage 
         </td>
      </tr>
      
      <tr>
         <td id="dashboard_body">
            <center>
      <!-- Log usage statistics table -->
            
      <div>
         <div id="divFloatingHeadings" class="floating_headers"  > 
            <table id="tableHeadings" class="configuration" >
               <tr>
                  <th colspan=2>Log Usage</th>
               </tr>
               <tr>
                  <td colspan="2">

               <center>
                  <table class="log_statistics">
                 <tr>
                         <td class="right"><b>Log Directory:</b></td><td> <?cs var:html_escape(LogDirectory) ?></td>
                 </tr>
                 <tr>  
                   <td class="right"><b>Log File Space Usage:</b></td><td> <?cs var:html_escape(IguanaUsage) ?></td>
                 </tr>
                       <?cs if: DiskSpaceRemaining != null ?>
                 <tr>     
                   <td class="right"><b>Space Remaining on Partition:</b></td><td><?cs var:html_escape(DiskSpaceRemaining) ?></td>
                 </tr>    
                 <tr>
                   <td class="right"><b>Partition Capacity:</b></td><td><?cs var:html_escape(PartitionCapacity) ?></td>
                 </tr>
                       <?cs /if ?>
                 <tr>
                   <td class="right"><b>Time Until Next Log Purge:</b></td><td><?cs var:html_escape(TimeToLogPurge) ?></td>
                 </tr>
                 <tr>
                   <td class="right"><b>Maximum Log Age:</b></td><td><?cs var:MaxLogAge ?> day<?cs if:MaxLogAge != 1 ?>s<?cs /if ?></td>
                 </tr>
                 <tr id="fts_update_row" style="display : none">
         <td class="right" valign="top"><b>Background Text Search Indexing:</b></td>
              <td><div id="fts_update_panel">loading...</div></td>
                 </tr>
                 <tr>
                   <td colspan="2" style="text-align:center">
                     <?cs if: LogDailyUsage ?>
                        <br>
                        You are currently using <b><?cs var:html_escape(LogDailyUsage) ?>/day</b> for logs (estimated).
                        <?cs if: LogTimeRemaining ?>
                           <br>
                           At this rate, the remaining disk space will last <b><?cs var:html_escape(LogTimeRemaining) ?></b>.
                        <?cs /if?>
                     <?cs /if?>
                       <br><br>Other log files (not listed below) are using <b><?cs var:OtherFilesSize ?></b>.
                  </td>
                 </tr>
                  </table>
               </center>
               
            </td></tr>

               <p/><p/> 
      <!-- Log details -->
               <tr class="sub_header"><td id="thLogDateHeading"><b>LOG DATE</b><br>(YYYY/MM/DD)</td><td id="thSizeHeading"><b>SIZE</b></td></tr>
            </table>
         </div>
         
         <div id="divLogFilesList" >
            <center>
               <table id="tableData" class="configuration" >   
                  <?cs def:add_row(value,date) ?>
                     <tr><td><center><?cs var:html_escape(date)?></center></td><td><center><?cs var: html_escape(value)?></center></td></tr>
                  <?cs /def ?>
                  <?cs each: channel = File ?>
                     <?cs call:add_row(channel.Size, channel.Date) ?>
                  <?cs /each ?>

               </table>
            </center>
         </div>
         
      </div>
            </center>
               
               <p/><p/>
            
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
        
               <p>This page provides a breakdown of the physical disc space used by the logs and gives an estimate of how much space remains for additional log files.</p>
         </td>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Log Settings</h4>
       <p>
            To edit log settings, go to
            <?cs # DO_NOT_RELEASE: Make this an external link. (MANUAL_TODO) ?>
            <a href="/settings#Page=log_settings/view">Settings&nbsp;&gt;&nbsp;Logs</a>.</p>
         </td>
      </tr>

      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
               <li>
               <a href="<?cs var:help_link('iguana4_viewing_log_statistics') ?>" target="_blank">Viewing Log Usage</a>
               </li>
            </ul>
         </td>
      </tr>
   </table>
</div>
</body>

</html>

