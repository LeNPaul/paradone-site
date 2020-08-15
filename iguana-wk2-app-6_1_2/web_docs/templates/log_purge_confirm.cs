   <style type="text/css">
      #divLogFilesList
      {
         overflow-y: auto;
         overflow-x: hidden;
      }

       #divLogFilesListLegacy
      {
         overflow-y: auto;
         overflow-x: hidden;
      }

   </style>

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt; <a href="/settings#Page=log_settings/view">Log Settings</a> &gt; Purge Logs
         </td>
      </tr>

      <!-- Log settings table -->
      <tr><td></td></tr>
      <?cs if: FilesToDelete || ChannelsToPurge ?>
         <tr>
            <td>
               <center>
               <h2>Purge Logs</h2>
               </center>
            </td>
         </tr>
         <tr>
            <td id="purgeFiles" style="padding:0px;margin:0px;">
               <center>
            <!-- scrolling stuff -->
            <div id="divWraper">
               <div id="divFloatingHeadings" class="floating_headers" style="display:none;">
                        <b>Iguana will attempt to purge the log files for the following dates from <?cs var:html_escape(LogDirectory) ?>:</b>
                        <br><br>

                  <table id="tableHeadings" class="configuration">
                        <th id="thLogDateHeading" width="145">Date</th>
                        <th id="thLogSizeHeading" width="145">Size</th>
                     </tr>
                  </table>
               </div>
               <div id="divLogFilesList" style="display:none;">

                  <input type="hidden" name="action" value="">
                  <table id="tableData" class="configuration">
                     <?cs def:add_row(name,value) ?>
                        <tr><td><?cs var:html_escape(name)?></td><td><?cs var:html_escape(value)?></td></tr>
                     <?cs /def ?>
                     <?cs each: channel = File ?>
                        <?cs call:add_row(channel.Date, channel.Size) ?>
                     <?cs /each ?>
                  </table>

               </div>
               <div id="divFloatingHeadingsLegacy" class="floating_headers" style="padding-top:20px;display:none;">
               <b>Iguana will attempt to purge the legacy logging database tables for the following channels:</b>
                        <br><br>
                  <table id="tableHeadingsLegacy" class="configuration">
                     <tr>
                        <th id="thLogDateHeadingLegacy" width="145">Date</th>
                        <th id="thLogChannelHeadingLegacy" width="145">Channel</th>
                     </tr>
                  </table>
               </div>
               <div id="divLogFilesListLegacy" style="display:none;">

                  <table id="tableDataLegacy" class="configuration">
                     <?cs def:add_rowLegacy(name,value) ?>
                        <tr>
                           <td>Before <?cs var:html_escape(value)?></td>
                           <td><a href="/channel#Channel=<?cs var:html_escape(name)?>"><?cs var:html_escape(name)?></a></td>
                        </tr>
                     <?cs /def ?>
                     <?cs each: channel = LegacyInfo ?>
                        <?cs call:add_rowLegacy(channel.ChannelName, channel.PurgeTime) ?>
                     <?cs /each ?>
                  </table>
               </div>
      </div>
                  <!-- Change log settings button, should only be accessible if UserCanAdmin -->
                  <?cs if:CurrentUserCanAdmin ?>
		  <div id="buttons">
                   <a class="action-button blue" href="javascript:confirmPurge()">Purge Now</a>
                   <a class="action-button blue" href="javascript:cancelPurge();">Cancel</a>
                  <?cs /if ?>

                     <p/><p/>
               </center>
		      </td>
	      </tr>
         <?cs else ?>
         <tr>
            <td>
               <center>
                  <p>
                     <font color="green"><b>Under the current <?cs var:html_escape(MaxLogPurgeDays) ?> day setting for maximum log age there are no log files to be deleted.</b></font>
                  </p>
                     <div id="buttons">
                        <a class="action-button blue" href="/settings#Page=log_settings/view">Back</a>
                     </div>
               </center>
            </td>
         </tr>

         <?cs /if?>
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
	 On this page, the table to the left shows the log files that Iguana will attempt to purge now.
	 </p>
   <?cs if:CurrentUserCanAdmin ?>
	<p>Click <b>Purge Now</b> to confirm purging of these log files, or <b>Cancel</b> to return to the previous page.</p>
   <?cs else ?>
      <p>You must be logged in as an administrator to view this page.</p>
   <?cs /if ?>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_config_logging') ?>" target="_blank">Logging</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
<script type="text/javascript">
var resizeTimeoutId;
var FilesToDelete   = <?cs if: FilesToDelete   ?>true<?cs else ?>false<?cs /if ?>;
var ChannelsToPurge = <?cs if: ChannelsToPurge ?>true<?cs else ?>false<?cs /if ?>;

function onResize() {
   // Some browsers (like firefox) don't always call onResize() at the right time.
   // So we will make sure to run the adjustments the user's mouse stops moving.
   clearTimeout(resizeTimeoutId);
   resizeTimeoutId = setTimeout("adjustTableDimensions();", 150);
}

function getTotalPadding(Element) {
   return parseInt(WINgetStyle(Element, 'paddingLeft')) + parseInt(WINgetStyle(Element, 'paddingRight'));
}

function adjustTableDimensions() {
   var DivWraper = document.getElementById('divWraper');
   // Table height - fit size to screen
   // 95 is height from top to the bottom of [Dashboard] [Logs] tabs

   var NewHeight = (WINgetWindowHeight() - WINwindowOffsetTop(DivWraper)) - 95;
   if (NewHeight < 300) {
      NewHeight = 300;
   }
   DivWraper.style.height = NewHeight + 'px';
   TableCount = 1;
   if (FilesToDelete && ChannelsToPurge) {
      TableCount = 2;
   }
   if (FilesToDelete) {
      resizeElements('divLogFilesList', 'tableData', 'tableHeadings', 'thLogDateHeading', 'divFloatingHeadings', NewHeight, TableCount)
   }
   if (ChannelsToPurge) {
      resizeElements('divLogFilesListLegacy', 'tableDataLegacy', 'tableHeadingsLegacy', 'thLogDateHeading', 'divFloatingHeadingsLegacy', NewHeight, TableCount)
   }
}

function resizeElements(LogListDivName, TableDataName, TableHeadingsName, HeadingColName, FloatingHeadingsName, NewHeight, TableCount) {
   var LogListDiv =        document.getElementById(LogListDivName);
   var TableData =         document.getElementById(TableDataName);
   var TableHeadings =     document.getElementById(TableHeadingsName);
   var HeadingCol =        document.getElementById(HeadingColName);
   var FloatingHeadings =  document.getElementById(FloatingHeadingsName);
   LogListDiv.style.display = '';
   FloatingHeadings.style.display = '';
   CalculatedHeight = NewHeight / TableCount - 80; //(available space / number of tables - 80px for the headings/spacing)
   if (CalculatedHeight < 60) {
      CalculatedHeight = 60;
   }

   LogListDiv.style.height = CalculatedHeight + 'px';
   if (CalculatedHeight > TableData.offsetHeight) {
      LogListDiv.style.height = TableData.offsetHeight  + "px";
   }
   // Table width - set explicitly to adjust for scrollbars in Internet Explorer
   TableData.style.width = '0px'; // This will set LogListDiv's width to what it should be, without being influenced by TableData's width
   TableData.style.width = LogListDiv.clientWidth + 'px';

   // Column widths - match the floating heading's columns with the data table's
   LogListDiv.style.width = TableHeadings.offsetWidth + 'px';

   // Get first row/col now always trivial
   var Row = TableData.rows[0].cells;
   var DateCol = Row[0];
   var SizeCol = Row[1];
   TableData.style.width = LogListDiv.clientWidth + 'px';
   DateCol.style.width = 145 + ( getTotalPadding(HeadingCol) - getTotalPadding(DateCol) ) + 'px';
}

function confirmPurge() {
   location.href="/settings#Page=log_purge_status?action=purge_now";
}

function cancelPurge() {
   location.href="/settings#Page=log_settings/view";
}

function initialize() {
   if (FilesToDelete || ChannelsToPurge) {
      adjustTableDimensions();
      window.onresize = onResize;
      onResize();
   }
}

$(document).ready(function() {
   initialize();
});
</script>

