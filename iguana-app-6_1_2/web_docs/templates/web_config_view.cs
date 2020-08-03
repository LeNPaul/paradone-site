<?cs # vim: set syntax=html :?>

<?cs def:showError(Value) ?>
<?cs if:Value ?>
   <br><span style="color:red;" id="<?cs name:Value ?>"><?cs var:html_escape(Value) ?></span>
<?cs else ?>
   <span id="<?cs name:Value ?>"></span>
<?cs /if ?>
<?cs /def ?>

<table id="iguana">
   <tr>
      <td id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; Web Server
      </td>
   </tr>
   <tr>
      <td id="dashboard_body">
         <center>
         <?cs if:ErrorMessage && !RestartLink ?>
            <span id="ErrorMessage"><h3 style="color:red;"><?cs var:html_escape(ErrorMessage) ?></h3></span>
         <?cs else ?>
            <span id="ErrorMessage"></span>
         <?cs /if ?>
         <?cs if:StatusMessage  ?>
            <span id="StatusMessage"><h3 style="color:green;"><?cs var:html_escape(StatusMessage) ?></h3></span>
         <?cs else ?>
           <span id="StatusMessage"></span>
         <?cs /if ?>

         <?cs if:RestartLink ?>
            <p>
               <span id="divRefreshStatus"></span>
               <a href="<?cs var:RestartLink ?>">Continue</a>
            </p>
         <?cs /if ?>

         <table class="configuration" border="1" id="test.webserver">
            <tr class="header">
               <th colspan="2">Web Server Settings</th>
            </tr>
            <tr>
               <td class="left_column">Server Name</td>
               <td>
               <?cs if: !ServerLabelEdit ?>
                  <i>Unnamed</i>
               <?cs /if ?>
                  <span id="test.ServerLabel"><?cs var:html_escape(ServerLabelEdit) ?></span>
               <?cs if:ServerLabelEdit != environment_expand(ServerLabelEdit) ?>
                  <?cs if:ServerLabelEdit != "" ?>
                     <span style="letter-spacing:-2px;">&ndash;&gt;&nbsp;</span>
                     <?cs var:html_escape(environment_expand(ServerLabelEdit)) ?>
                  <?cs /if ?>
               <?cs /if ?>
               </td>
            </tr>
            <tr>
            <td class="left_column">
               Allowable Login Failures
            <td>

            <?cs var:#LoginFailureLimit ?>
            <?cs if:#LoginFailurePeriod == 1 ?> each minute
            <?cs else ?> every <?cs var:#LoginFailurePeriod ?> minutes
            <?cs /if ?>
            </tr>
            <tr>
               <td class="left_column">Session Timeout<br><span style="font-weight:normal;">(in minutes)</span></td>
               <td>
                  <span id="test.SessionTimeout"><?cs var:html_escape(SessionTimeout) ?></span>
                  <?cs call:showError(SessionTimeout_ErrorString) ?>
               </td>
            </tr>
            <tr>
               <td class="left_column">Web Server Port</td>
               <td>
                  <span id="test.WebPort"><?cs var:html_escape(WebPort) ?>
                  <?cs if:html_escape(WebPort) != html_escape(environment_expand(WebPort)) ?>
                     <span style="letter-spacing:-2px;">&ndash;&gt;&nbsp;</span>
                     <?cs var:html_escape(environment_expand(WebPort)) ?>
                  <?cs /if ?>
                  </span>
                  <?cs call:showError(WebPort_ErrorString) ?>
               </td>
            </tr>
            <tr>
               <td class="left_column">Use HTTPS</td>
               <td>
                  <?cs if:UsingHttps=="true" ?>
                     Yes
                  <?cs else ?>
                     No
                  <?cs /if ?>

                  <?cs if:RestartLink ?>
                     <?cs # not showing startup errors while restarting - it''s confusing, ?>
                  <?cs else ?>

                     <?cs if:HttpsState ?>
                        <br>Startup Status: <?cs var:html_escape(HttpsState) ?>
                     <?cs /if ?>

                     <?cs if:HttpsErrorMessage != "" ?><br />
                        <font color="red" id="test.HttpsErrorMessage"><?cs var:html_escape(HttpsErrorMessage) ?></font>
                     <?cs /if ?>

                     <?cs call:showError(UsingHttps_ErrorString) ?>
                  <?cs /if ?>
               </td>
            </tr>

            <?cs if:UsingHttps=="true" ?>
            <tr>
               <td class="left_column">Certificate File</td>
               <td>
                  <span id="test.CertificateFile"><?cs var:html_escape(CertificateFile) ?></span>
                  <?cs call:showError(CertificateFile_ErrorString) ?>
                  <?cs if:CertificateFile_Link ?>
                     <br>
                     <a href="<?cs var:html_escape(CertificateFile_Link) ?>">view certificate</a>
                  <?cs /if ?>
               </td>
            </tr>

            <tr>
               <td class="left_column">Private Key File</td>
               <td>
                  <span id="test.PrivateKeyFile"><?cs var:html_escape(PrivateKeyFile) ?></span>
                  <?cs call:showError(PrivateKeyFile_ErrorString) ?>
               </td>
            </tr>

            <?cs if:LoadCheck_ErrorString ?>
               <?cs
                  # this row only appears when there''s a problem,
                  # so that we don''t have to explain what it is about otherwise
               ?>
               <tr id="RowLoadCheck">
                  <td class="left_column">Load check</td>
                  <td>
                     <span style="color:red;">FAILED</span>
                     <?cs call:showError(LoadCheck_ErrorString) ?>
                  </td>
               </tr>
               <?cs /if ?>
            <?cs /if ?>
         </table>
         <p>
            <form method="post" id="RestartForm" name="RestartForm">
               <input type="hidden" name="Restart" value="yep">
            </form>
            <?cs if:!RestartLink ?>
               <table id="buttons">
                  <tr>
                     <td>
                     <?cs if:CanAdmin ?>
                        <a class="action-button blue" id="edit_button" href="#Page=web_settings/edit">Edit</a>
                     <?cs /if ?>
                     </td>
                     <td>
                     <?cs if:CanAdmin ?>
                        <a class="action-button blue" id="RestartButton" href="/restart">Restart Web Server</a>
                     <?cs /if ?>
                     </td>
                  </tr>
               </table>
            <?cs /if ?>
         </p>

         <!-- end display area -->
         </center>
      </td>
   </tr>
</table>

<div id="side_panel">
   <table id="side_table">
      <tr>
         <th id="side_header">Page Help</th>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Overview</h4>
	         <p>On this page, you can view the web interface configuration for Iguana.</p>
         <?cs if:CanAdmin ?>
	         <p>Click <b>Change Settings</b> to change the settings on this page, or <b>Restart Web Server</b> to restart the Iguana server.</p>
         <?cs else ?>
            <p>You must be logged in as an administrator to edit the settings on this page.</p>
         <?cs /if ?>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_config_webserver') ?>" target="_blank">Web Server Configuration</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
</div>

<script type="text/javascript">

$(document).ready(function() {
<?cs if:RestartLink ?>

   SettingsHelpers.submitRestartForm();
   var CountDown = 5;
   setTimeout(function() {
      SettingsHelpers.onTimer('<?cs var:RestartLink ?>', CountDown);
   }, 1000);

<?cs /if ?>

   var EditButton = $("#edit_button");
   var RestartButton = $("#RestartButton");

<?cs if:CanAdmin ?>

   EditButton.click(function(event) {
      SettingsHelpers.routeTo(this.hash);
   });

   RestartButton.click(function(event) {
      event.preventDefault();
      SettingsHelpers.submitRestartForm();
   });

<?cs else ?>

   EditButton.on("mouseover", function() {
      TOOLtooltipLink('You do not have the necessary permissions to edit these settings.', null, EditButton.get(0));
   });
   EditButton.on("mouseout mouseup", function() {
      TOOLtipClose();
   });
   RestartButton.on("mouseover", function() {
      TOOLtooltipLink('You do not have the necessary permissions to restart the web server.', null, RestartButton.get(0));
   });
   RestartButton.on("mouseout mouseup", function() {
      TOOLtipClose();
   });

<?cs /if ?>

});

</script>

