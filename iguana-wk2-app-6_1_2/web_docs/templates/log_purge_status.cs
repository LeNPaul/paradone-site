   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt; <a href="/settings#Page=log_settings/view">Log Settings</a> &gt; Purge Logs Results
         </td>
      </tr>

      <!-- Log settings table -->
      <tr>
         <td>
            <center>
               <font color="red"><div id="ajax_error_label"></div></font>
               <h2>Purge Logs</h2>
               <img align="center" id="purge_logs_spinner" src="images/spinner.gif">
               <table width="100%">
                  <tr>
                     <td id="warning_cell" style="text-align:left""></td>
                  </tr>
                  <tr>
                     <td align="center">
                        <div id="div_initial_message_reference"></div>
                        <div id="textarea_initial_message" style="font-family:monospace; display:none;text-align:left; width:100%; height:75px; background:#EDF0F4;padding:5px;"></div>
                     </td>
                  </tr>
               </table>
                  <table>
                     <tr>
                        <td><div id="btnOkay"><!--filled by js--></div></a>
                        </td>
                     </tr>
                  </table>
                  <br>

            </center>
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
     On this page, you will see the result of the log purge request.
     </p>
    <p>Click <b>Close</b> to return to the Log Purge Settings page.</p>
         </td>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Log Usage Statistics</h4>
        <p>
            For detailed information of current log usage and trends, see
            <?cs # DO_NOT_RELEASE: Make this an external link. (MANUAL_TODO) ?>
            <a href="log_usage_statistics">Log Usage Statistics</a>.</p>
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
$(document).ready(function() {
   var LogPurgeProgress;
   function showTitle() {
      if (LogPurgeProgress) {
         document.getElementById('purge_logs_spinner').style.display = (LogPurgeProgress.LogPurgeInProgress ? '' : 'none' );
      }
   }

   function setOkayButton() {
      ButtonLbl = 'Close';
      OkEnabledHtml = '<a class="action-button blue" style="text-align: center;" href="/settings#Page=log_settings/view"><span style="text-align: center;">'+ButtonLbl+'</span></a>';
      OkDisabledHtml = '<div class="action-button grey disabled" style="float:right"\
                           onMouseOver="TOOLtooltipLink(\'Log purge is currently in progress.\', null, this);"\
                           onMouseOut="TOOLtooltipClose();"\
                           onmouseup="TOOLtooltipClose();" >\
                           <span>'+ButtonLbl+'</span></div>';
      NewHtml = ( !LogPurgeProgress || LogPurgeProgress.LogPurgeInProgress ? OkDisabledHtml : OkEnabledHtml);
      if (NewHtml == OkEnabledHtml) {
         TOOLtooltipClose();
      }
      document.getElementById('btnOkay').innerHTML = NewHtml;
   }

   function onResize() {
      var textDiv = document.getElementById('textarea_initial_message');
      var offsetTop = WINwindowOffsetTop(textDiv);

      //  Initial 'textarea_initial_message' has no offset, thus we use offset from div_initial_message_reference
      if (offsetTop == 0) {
         offsetTop = WINwindowOffsetTop(document.getElementById('div_initial_message_reference')) + 15;
      }

      var NewHeight = (WINgetWindowHeight() - offsetTop) - 95;
      if (NewHeight < 95) {
         NewHeight = 95;
      }
      textDiv.style.height = NewHeight + 'px';
      textDiv.style.overflow="auto";
   }

   function onLoad() {
      setOkayButton();
      checkLogPurge();
      window.onresize = onResize;
      onResize();
   }

   function showMessage() {
console.log("Showing message");
      if (LogPurgeProgress && !LogPurgeProgress.LogPurgeInProgress) {
         if (LogPurgeProgress.ErrorExists) {
            WarningCell = document.getElementById('warning_cell');
            WarningCell.innerHTML = LogPurgeProgress.WarningMessage;
            WarningCell.style.display = '';
            WarningCell.style.color = '#FF0000';
            WarningCell.style.fontWeight = 'bold';
         }
         InitialMessageArea = document.getElementById('textarea_initial_message');
         InitialMessageArea.innerHTML = '<pre style="white-space:pre-wrap;">' + LogPurgeProgress.InitialMessage + '</pre>';
         InitialMessageArea.style.display = '';
      }
   }

   function checkLogPurge() {
      $.ajax({
         url    :  '/log_purge_progress',
         data   : { 'AutomaticRequest':1 },
         success: function(Response) {
console.log(Response);
            LogPurgeProgress = Response;
            showTitle();
            setOkayButton();
            showMessage();
            if (LogPurgeProgress.LogPurgeInProgress) {
               setTimeout(checkLogPurge, 1000);
            }
         },
         error  : function(RequestObject, Status, ErrorString) {
            console.log("Error");
            console.log(RequestObject);
            console.log(Status);
            console.log(ErrorString);
            document.getElementById('ajax_error_label').innerHTML = ErrorString;
         }
      });
   }
   onLoad();
});
</script>

