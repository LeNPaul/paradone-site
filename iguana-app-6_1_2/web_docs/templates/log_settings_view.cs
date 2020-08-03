<?cs # vim: set syntax=html :?>
<style type="text/css">
.subtle
{
   text-align: left;
   color: #808080;
}
</style>

<table id="iguana">
   <tr>
      <td id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; Log Settings
      </td>
   </tr>

   <!-- Log settings table -->
   <tr>
      <td>
      <center>
         <p class="error_message ajax_error" style="display: none"></p>
         <?cs if:SettingsChanged ?>
         <p class="status_message">Log settings have been changed.</p>
         <?cs /if ?>
       </center>
      </td>
   </tr>
   <tr>
      <td id="log_settings">
         <center>
            <form name="log_settings_view" method="post">
               <input type="hidden" name="action" value="">
               <table class="configuration">
               <tr>
                  <th colspan="2">Log Settings</th>
               </tr>
               <tr>
                  <td class="left_column">Log Directory</td>
                  <td class="inner_left">
                     <table class="inner">
                        <tr><td class="inner_left" ><?cs var:html_escape(LogDirectory) ?> </td></tr>
                     </table>
                  </td>
               </tr>
               <tr>
                  <td class="left_column">Maximum Log Age</td>
                  <td>
                     <?cs var:html_escape(LogPurgeMaxAge) ?> day<?cs if:LogPurgeMaxAge != 1 ?>s<?cs /if ?>
                     <?cs if: LogPurgeMaxAge > RecommendedMaxMaxAge ?>
                        <span class="subtle">
                           (keeping logs longer than <?cs var:#RecommendedMaxMaxAge ?> days has some <a href="<?cs var:help_link('log_performance_considerations') ?>" target="_blank">additional overhead</a>)</span>
                     <?cs /if ?>
                  </td>
               </tr>
               <tr>
                  <td class="left_column">Log Purge Time</td>
                  <td class="inner_left">
                     <table class="inner">
                        <tr>
                           <td class="inner_left" ><?cs var:html_escape(LogPurgeHour) ?>:<?cs var:html_escape(LogPurgeMinute) ?><?cs if: LogPurgeTimeMeridien == "1" ?> pm <?cs else?> am <?cs /if?></td>
                           <td rowspan=2 align="center">
                           <?cs if CurrentUserCanAdmin ?>
                              <div id="btnPurgeNow"><!-- filled by javascript --></div>
                           <?cs /if ?>
                           </td>
                        </tr>
                        <tr>
                           <td><span class="subtle">Iguana will purge logs every day at this scheduled time.</span></td>
                        </tr>
                     </table>
                  </td>
               </tr>
               <tr>
                  <td class="left_column" valign="top">Synchronous Writes</td>
                  <td style="width:400px"><?cs if:LogSynchronousWrites=='1' ?>Yes<?cs else ?>No<?cs /if ?></td>
               </tr>
               <?cs if:LogCanAudit=='1' ?>
               <tr>
                  <td class="left_column" valign="top">Audit Logging</td>
                  <td style="width:400px"><?cs if:LogEnableAudit=='1' ?>Enabled<?cs else ?>Disabled<?cs /if ?></td>
               </tr>
               <?cs /if ?>
               <?cs if:!FtsInAllFiles ?>
               <tr>
                  <td class="left_column" valign="top"> Background Text Indexing:</td>
                  <td style="width:400px"><?cs if:BackgroundTextIndexing=='1' ?>Yes<?cs else ?>No<?cs /if ?></td>
               </tr>
               <?cs /if ?>
               </table>
            </form>
            <!-- Change log settings button, should only be accessible if UserCanAdmin -->
            <div id="buttons">
               <?cs if:CurrentUserCanAdmin ?>
                  <a class="action-button blue" href="/settings#Page=log_purge_settings/edit">Edit</a>
               <?cs /if ?>
            </div>
         </center>
      </td>
   </tr>
</table>

<div id="side_panel">
   <table id="side_table">
      <tr>
         <th id="side_header"> Page Help </th>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Overview</h4>
            <p> This page displays the logging configuration information.  </p>
            <?cs if:CurrentUserCanAdmin ?>
               <p>Clicking on <b>Purge Now</b> will purge the logs with the current log purge settings.</p>
            <?cs /if ?>
         </td>
      </tr>
      <tr>
         <td id="side_body">
         <h4 class="side_title">Log Usage</h4>
         <p>
            For detailed information of current log usage and trends, see
            <?cs # DO_NOT_RELEASE: Make this an external link. (MANUAL_TODO) ?>
            <a href="log_usage_statistics">Logs&nbsp;&gt;&nbsp;Log&nbsp;Usage</a>.</p>
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
</div>

<script type="text/javascript">

function setPurgeButton(PurgeInProgress) {
   <?cs if:CurrentUserCanAdmin ?>

   PurgeEnabledHtml = '<a class="action-button-small blue" id="LogPurgeTrigger" style="text-align: center;" href="/settings#Page=log_purge_confirm">Purge Now</a>';

   PurgeDisabledHtml = '<span class="config_warning">Another log purge is currently in progress.</span>';
   NewHtml = (PurgeInProgress ? PurgeDisabledHtml : PurgeEnabledHtml);

   if (NewHtml == PurgeEnabledHtml && $('.tool_tip_exists').length == 1) {
      TOOLtooltipClose();
   }

   $('#btnPurgeNow').html(NewHtml);

   <?cs /if ?>
}

function onLoad() {
   setPurgeButton(<?cs if:LogPurgeInProgress=='1' ?>true<?cs else ?>false<?cs /if ?>);
   checkLogPurge();
}

function checkLogPurge() {
   console.log("checkin' fur the purge");
   $.ajax({
      url: '/log_purge_progress',
      data: {
        AutomaticRequest: 1
      },
      success: function(Response) {
         setPurgeButton(Response.LogPurgeInProgress);
         if (window.location.hash.indexOf("log_settings/view") != -1) {
            // Only run this on the view log purge page. It was being called
            // not stop on all settings pages after you came here once.
            setTimeout(checkLogPurge, 1000);
         }
      },
      error: function(RequestObject, Status, ErrorString) {
         console.log("Error");
         console.log(RequestObject);
         console.log(Status);
         console.log(ErrorString);
         $('.ajax_error_label').show().html(ErrorString);
      }
   });
}

$(document).ready(function() {
   onLoad();
});
</script>
