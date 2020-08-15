<?cs # vim: set syntax=html :?>
   <style type="text/css">
      .log_config_error{
      text-align: left;
      color: #FF0000;
      }
      .subtle
      {
         text-align: left;
         color: #808080;
      }
   </style>

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt; <a href="/settings#Page=log_settings/view"> Log Settings</a> &gt; Edit Settings
         </td>
      </tr>

      <tr>
         <td id="dashboard_body">
            <center>
	    <font color="red"><div id="ajax_error_label"></div></font>
	    <?cs if:SettingsUpdated == "true" ?>
                  <p>
		               <font color="green"><b>Log settings have been changed.</b></font>
		            </p>
               <?cs /if ?>


               <?cs if:ErrorMessage && ErrorMessage != "No changes to logging settings"?>
                  <p><font color="red"><b><?cs var:html_escape(ErrorMessage) ?></b></font></p>
               <?cs /if ?>

               <form name="log_purge_settings" id="log_purge_settings" method="post" action="/log_purge_settings/edit">
		            <input type="hidden" name="action" value="">
                  <table class="configuration">
                     <tr>
                        <th colspan=2>Log Settings</th>
                     </tr>
                     <!-- for future release, this is where to edit the log directory
                     but right now just show a readonly log directory and a link on how to change it -->

                     <tr>
                        <td class="left_column" valign="top">Log Directory</td>
                        <td style="width:400px">
                           <table class="inner">
                              <tr><td class="inner_left" valign="top">
                                 <?cs var:html_escape(LogDirectory) ?>
            			<?cs # DO_NOT_RELEASE: Make this an external link. (MANUAL_TODO) ?>
                              <br><br><div class="subtle">To alter, see <a href="<?cs var:help_link('iguana4_changing_log_directory') ?>" target="_blank">changing the log directory</a>.</div></td></tr>
                           </table>
                        </td>
                     </tr>

                     <tr id="MaximumLogAgeRow">
                        <td class="left_column">Maximum Log Age</td>
                        <td>
                           <input class="number_field" type="text" name="LogPurgeMaxAge" id="LogPurgeMaxAge" value="<?cs var:html_escape(LogPurgeMaxAge) ?>"><span class="subtle"> (days, minimum 1)</span>
                           <span id="MaximumLogAgeErrorContainer" class="validation_error_message_container">
                           <script defer type="text/javascript">
                              VALregisterIntegerValidationFunction('LogPurgeMaxAge', 'MaximumLogAgeRow', 'MaximumLogAgeErrorContainer', null, null, 1);
                           </script>
                        </td>
		               </tr>

                     <tr>
                        <td class="left_column">Log Purge Time</td>
                        <td style="width:400px">
                           <table class="inner">
                              <tr><td class="inner_left">
                                 <select id="LogPurgeTime" name="LogPurgeTime" >
                                    <?cs def:add_time(name,value) ?>
                                       <option value="<?cs var:html_escape(value) ?>" <?cs if: LogPurgeTime == html_escape(value) ?> selected=="selected"<?cs /if?> > <?cs var:html_escape(value)?></option>
                                    <?cs /def?>
                                    <?cs each: time_interval = Time ?>
                                       <?cs call:add_time(time_interval.Interval_0,time_interval.Interval_0) ?>
                                       <?cs call:add_time(time_interval.Interval_1,time_interval.Interval_1) ?>
                                    <?cs /each?>
                                 </select>
                                 <select name="Meridien">
                                    <option value="am" <?cs if: LogPurgeTimeMeridien == "0" ?> selected=="selected"<?cs /if?> >am</option>
                                    <option value="pm" <?cs if: LogPurgeTimeMeridien == "1" ?> selected=="selected"<?cs /if?> >pm</option>
                                 </select>
                              </td></tr>
                              <tr><td><span class="subtle">For best results, purge at a time when the server is not busy, such as early in the morning.</span></td></tr>
                           </table>
                        </td>
		               </tr>
                     <tr>
                        <td class="left_column" valign="top">Synchronous Writes</td>
                        <td style="width:400px">
                           <input name="LogSynchronousWrites" type="checkbox" <?cs if:LogSynchronousWrites=='1' ?>checked<?cs /if ?> >
	    <a id="LogSynchronousWrites_Icon" class="helpIcon" tabindex="101"
               rel="<ul>
                    <li>When enabled, changes made to the logs are synchronized to stable storage between each processed message. This is safer but significantly slower.
                    <li>When disabled, changes made to logs are written to the OS but not synchronized, therefore relying on the OS to safely ensure the data reaches stable storage. This is significantly faster, but incurs some risk of data loss if the OS crashes or a power outage occurs.
                    </ul>
		    <p>For more information on synchronous writes and how they affect performance, see
            	<a href=&quot;<?cs var:help_link('log_performance_synchronous_writes') ?>&quot; target=&quot;_blank&quot;>Synchronous Writes
		and Logging Performance</a>.</p>"
               title="Synchronous Writes" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                        </td>
                     </tr>
                     <?cs if:LogCanAudit=='1' ?>
                     <tr>
                        <td class="left_column" valign="top">Audit Logging</td>
                        <td style="width:400px">
                           <input name="LogEnableAudit" type="checkbox" <?cs if:LogEnableAudit=='1' ?>checked<?cs /if ?> >   
	    <a id="LogEnableAudit_Icon" class="helpIcon" tabindex="101"
               rel="<ul>
                    <li>When enabled, audit logging will log user activity involving the logs (viewing, browsing, and exporting messages). This will increase log volume but allows granular reporting about who has seen which information.
                    <li>Changing this setting (like all configuration changes) creates an entry in the Iguana history.
                    </ul>"
               title="Audit Logging" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>

                        </td>
                     </tr>
                     <?cs /if ?>
		  <?cs if:!FtsInAllFiles ?>
		  <tr>
                     <td class="left_column" valign="top">Background Text Indexing:</td>
                     <td style="width:400px">
                           <input name="BackgroundTextIndexing" type="checkbox" <?cs if:BackgroundTextIndexing=='1' ?>checked<?cs /if ?> >
			   	    <a id="BackgroundTextIndexing_icon" class="helpIcon" tabindex="101"
               rel="<p>Iguana 4.5 introduced a full text index for fast word searches of the logs. Log files from previous versions of Iguana are updated in the background to have this full text index. </p>
                    <p>This background task can be disabled temporarily in the event that channel activity is being hindered. Disabling background text indexing is only applicable until Iguana is restarted.</p>"
               title="Background Text Indexing" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
	            </td>
		  </tr>
		  <?cs /if ?>
                                       </table>
               </form>
               <?cs if:CurrentUserCanAdmin ?>
		            <div id="buttons">
		                  <a class="action-button blue" id="SaveChanges">Save changes</a>
		            </div>
               <?cs /if ?>
               <p/><p/>

            </center>

		   </td>
	   </tr>
   </table>
   <div id="helpTooltipDiv" class="helpTooltip">
      <b id="helpTooltipTitle"></b>
      <em id="helpTooltipBody"></em>
      <input type="hidden" name="helpTooltipId" id="helpTooltipId" value="0">
   </div>

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
	 On this page, you can edit the logging configuration information.
	 </p>
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


function onLoad() {
<?cs if:ErrorMessage == "No changes to logging settings" ?>
   location.href = '/settings#Page=log_settings/view?SettingsChanged=false';
<?cs /if ?>

   <?cs if:SettingsUpdated ?>
   location.href = '/settings#Page=log_settings/view?SettingsChanged=true';
<?cs /if ?>
   HLPpopUpinitialize();
}

$(document).ready(function() {
   $("form#log_purge_settings").submit({uri: ifware.SettingsScreen.page(), form_id: "log_purge_settings"}, SettingsHelpers.submitForm);
   $("a#SaveChanges").click(function(event) {
      event.preventDefault();
      document.log_purge_settings.action.value = "change";
      $(this).blur();
      $("form#log_purge_settings").submit();
   });
   onLoad();
});
</script>

