<?cs # vim: set syntax=html :?>

<?cs include:"browse_macro.cs" ?>

<?cs def:showError(Value) ?>
 <div style="clear:both;">
<?cs if:Value ?>
  <span style="color:red;" id="<?cs name:Value ?>"><?cs var:html_escape(Value) ?></span>
<?cs else ?>
  <span id="<?cs name:Value ?>"></span>
<?cs /if ?>
 </div>
<?cs /def ?>

<table id="iguana">
   <tr>
      <td id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; <a href="/settings#Page=web_settings/view">Web Server</a> &gt; Edit
      </td>
   </tr>

   <tr>
      <td id="dashboard_body">
      <center>

      <?cs if:ErrorMessage ?>
         <h3><font color="red" id="ErrorMessage"><?cs var:html_escape(ErrorMessage) ?></font></h3>
      <?cs else ?>
         <span id="ErrorMessage"></span>
      <?cs /if ?>

      <?cs if:StatusMessage ?>
         <h3><font color="green" id="StatusMessage"><?cs var:html_escape(StatusMessage) ?></font></h3>
      <?cs else ?>
         <span id="StatusMessage"></span>
      <?cs /if ?>

      <?cs if:RestartLink ?>
         <span id="divRefreshStatus"></span> <a href="<?cs var:RestartLink ?>">Continue</a></p>
      <?cs /if ?>

      <div  class="sc_spinner_container">
      <form name="web_settings" id="web_settings" method="post">
         <table class="configuration" border="1" id="webserver">
            <tr class="header">
               <th colspan="2">Web Server Settings</th>
            </tr>

            <tr>
               <td class="left_column">Server Name</td>
               <td><input type="text" size="60" name="ServerLabel" id="ServerLabel" value="<?cs var:html_escape(ServerLabelEdit) ?>"/>
                  <div id="Servername_preview_div" style="display:none;">
                     <div id="Servername_preview" class="path_preview" val="<?cs var:html_escape(environment_expand(ServerLabelEdit)) ?>"></div>
                  </div><!--/#Servername_preview_div-->
               </td>
            </tr>

            <tr>
               <td class="left_column">Allowable Login Failures</td>
               <td>
                  <input type="text" class="number_field" name="LoginFailureLimit" size="3" value="<?cs var:#LoginFailureLimit ?>">
                     every
                  <input type="text" class="number_field" name="LoginFailurePeriod" size="3" value="<?cs var:#LoginFailurePeriod ?>">
                     minutes
                  <a class="helpIcon" tabindex="100" rel="When trying to log in, users who mistype their passwords too many times in a given time period will be forced to wait a while before trying again." title="More Information" href>
                     <img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0">
                  </a>
               </td>
            </tr>

            <tr id="SessionTimeoutRow">
               <td class="left_column">Session Timeout in Minutes</td>
               <td>
                  <input class="number_field" type="text" name="SessionTimeout" id="SessionTimeout" value="<?cs var:html_escape(SessionTimeout) ?>">
                     How long a user can be inactive before being logged out.
                  <span id="SessionTimeoutErrorMessageContainer" class="validation_error_message_container"></span>
               </td>
            </tr>

            <tr id="WebPortRow">
               <td class="left_column">Web Server Port</td>
               <td>
                  <input class="text" type="text" name="WebPort" id="WebPort" value="<?cs var:html_escape(WebPort) ?>">
                  <div id="WebPort_preview_div" style="display:none;">
                     <div id="WebPort_preview" class="path_preview" val="<?cs var:html_escape(environment_expand(WebPort)) ?>"></div>
                  </div>
                  <span id="WebPortErrorMessageContainer" class="validation_error_message_container"></span>
               </td>
            </tr>

            <tr>
               <td class="left_column">Use HTTPS</td>
               <td width="500px">
                  <input type="checkbox" class="no_style" name="UsingHttps" id="UsingHttps" <?cs if:UsingHttps=="true" ?>checked="checked"<?cs /if ?>>
               </td>
            </tr>

            <tr id="RowCertificateFile" class="HTTPSrow">
               <td class="left_column"><span id="LabelCertificateFile">Certificate File</span></td>
               <td>
                  <span id="NaCertificateFile" style="display:none;">N/A</span>
                     <?cs call:browse_input('CertificateFile', CertificateFile) ?>
                     <?cs call:showError(CertificateFile_ErrorString) ?>
               </td>
            </tr>

            <tr id="RowPrivateKeyFile" class="HTTPSrow">
               <td class="left_column"><span id="LabelPrivateKeyFile">Private Key File</span></td>
               <td>
                  <span id="NaPrivateKeyFile" style="display:none;">N/A</span>
                  <?cs call:browse_input('PrivateKeyFile',PrivateKeyFile) ?>
                  <?cs call:showError(PrivateKeyFile_ErrorString) ?></td>
            </tr>

            <?cs if:LoadCheck_ErrorString ?>
            <tr id="RowLoadCheck">
               <td class="left_column">Load check</td>
               <td>
                  <span style="color:red;">FAILED</span>
                  <?cs call:showError(LoadCheck_ErrorString) ?>
               </td>
            </tr>
            <?cs /if ?>
         </table>
         <input type=hidden name="ApplyChanges" value="yes" />
         <table id="buttons">
            <tr>
               <td>
               <!--
               Hidden submit button is need to submit form with multiple fields on <enter>.
               IE ignores display:none buttons, so it must be shrunk and positioned off screen
               -->
                  <input type="submit" name="Action" value="save" style="width: 0px; height: 0px; position: absolute; left: -50px; top: -50px;" />
                  <a id="ApplyChanges" class="action-button blue">Save Changes and Restart Web Server</a>
               </td>
               <td>
                  <a class="action-button blue" id="RestoreDefaults">Reset to Defaults</a>
               </td>
            </tr>
         </table>
      </form>
      </div> <!-- /.spinner_container -->
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
            <p>On this page, you can edit the web server settings for Iguana.</p>
            <p>Click <b>Save Changes and Restart Web Server</b> to update these settings, or
            <b>Set Defaults</b> to use their default values.</p>
            <p>For information on how to create a certificate file, see <a href="<?cs var:help_link('iguana4_certificate_files') ?>" target="_blank">Creating Certificate Files</a>.</p>
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

<div id="helpTooltipDiv" class="helpTooltip">
   <b id="helpTooltipTitle"></b>
   <em id="helpTooltipBody"></em>
   <input type="hidden" name="helpTooltipId" id="helpTooltipId" value="0">
</div>

<script type="text/javascript">

$(document).ready(function() {
   setupFileBrowser();
   HLPpopUpinitialize();
   var ServerLabelText;

   function toggleUsingHttpsRows() {
      SettingsHelpers.toggleFormRow("UsingHttps", ["#CertificateFile", "#PrivateKeyFile"], [".HTTPSrow"]);
   }

   toggleUsingHttpsRows();

   $("#UsingHttps").click(toggleUsingHttpsRows);

   $("form#web_settings").submit({
      uri: ifware.SettingsScreen.page(),
      form_id: "web_settings",
      success_handler: function updateServerName() {
         $(".version_info").text(ServerLabelText);
      }
   }, SettingsHelpers.submitForm);

   $("#WebPort").keyup(function() {
      expandFields(['WebPort']);
      VALclearError("WebPortRow", "WebPortErrorMessageContainer");
   });

   VALregisterIntegerValidationFunction('WebPort', 'WebPortRow', 'WebPortErrorMessageContainer', null, null, 1, 65535, true);

   $("a#ApplyChanges").click(function(event) {
      event.preventDefault();
      ServerLabelText = $("#ServerLabel").val();
      if (VALvalidateFields()) {
         $("form#web_settings").submit(); 
      }
   });

   $("#RestoreDefaults").click(function(event) {
      event.preventDefault();
      SettingsHelpers.setDefaults(ifware.SettingsScreen.page());
   });

<?cs if:RestartLink ?>
   var CountDown = 5;
   setTimeout(function() {
      SettingsHelpers.onTimer('<?cs var:RestartLink ?>', CountDown);
   }, 1000);
<?cs /if ?>

<?cs if:SessionTimeout_ErrorString ?>
   VALdisplayError('SessionTimeout', 'SessionTimeoutRow', 'SessionTimeoutErrorMessageContainer', null, '<?cs var:javascript_escape(SessionTimeout_ErrorString) ?>');
<?cs /if ?>

<?cs if:WebPort_ErrorString ?>
   VALdisplayError('WebPort', 'WebPortRow', 'WebPortErrorMessageContainer', null, '<?cs var:javascript_escape(WebPort_ErrorString) ?>');
<?cs /if ?>
});
</script>

