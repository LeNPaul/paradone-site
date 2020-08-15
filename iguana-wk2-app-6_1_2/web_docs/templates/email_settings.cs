<?cs # vim: set syntax=html :?>
<script type="text/javascript">

function onTabClick() {
   if (VALvalidateFields()) {
      VALfieldValidationFunctions = [];
      return true;
   } else {
      return false;
   }
}

</script>

<?cs include:"email_notification_macros.cs" ?>

<table id="iguana">
   <tr>
      <td id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; Email Notification
      </td>
   </tr>
   <tr>
      <td id="dashboard_body">
      <center>

      <?cs if:EmailNotSetup ?>
      <!-- EMAIL NOT SETUP -->

         <?cs call:email_not_setup_page_contents() ?>


      <?cs elif:EmailDisabled ?>
      <!-- EMAIL DISABLED -->

         <?cs call:email_disabled_page_contents("Settings") ?>

      <?cs else ?>

      <div class="tabs_and_contents">
         <?cs call:email_navigation_tabs("Settings", "return onTabClick();") ?>
         <br />

         <!-- Firefox supports the onchange event for forms, so this makes use of it -->
         <form name="email_settings" id="email_settings" method="post">
            <table class="configuration">
               <tr>
                  <th colspan="2">Email Notification Settings</th>
               </tr>
               <tr>
                  <td class="left_column">Outgoing Mail Server</td>
                  <td>
                     <input size="40" type="text" id="OutgoingMailServerAddress" name="outgoing_mail_server_address" value="<?cs var:OutgoingMailServerAddress ?>">
                  </td>
               </tr>
               <tr id="MailServerPortRow">
                  <td class="left_column">Mail Server Port<br><span style="font-weight:normal;">(default 25)</span></td>
                  <td>
                     <input class="text" type="text" name="outgoing_mail_server_port" id="OutgoingMailServerPort" onkeyup="expandFields(['OutgoingMailServerPort']);" value="<?cs var:html_escape(OutgoingMailServerPort) ?>">
                     <div id="OutgoingMailServerPort_preview_div" style="display:none;">
                       <div id="OutgoingMailServerPort_preview" class="path_preview" val="<?cs var:html_escape(environment_expand(OutgoingMailServerPort)) ?>"></div>
                     </div>
                     <span id="MailServerPortErrorMessageContainer" class="validation_error_message_container"></span>
                  </td>
               </tr>

               <tr>
                  <td class="left_column" style="vertical-align: top;">
                     <span style="position: relative; top: 4px;"> Authentication </span>
                  </td>
                  <td>
                     <input type="checkbox"
                            class="no_style"
                            name="use_email_authentication"
                            id="UseEmailAuthenticationCheckbox"
                            style="vertical-align: top;"
                            <?cs if:UseEmailAuthentication == "1" ?>
                              checked="checked"
                            <?cs /if ?>
                     />
                     <span id="EmailAuthenticationCaption" style="position: relative; top: 2px; display:block;">
                        Use the following details:
                     </span>

                     <div id="EmailAuthenticationFields" style="vertical-align: middle; display:block;">
                        <table style="vertical-align: middle;">
                           <tr>
                              <td>Username:</td>
                              <td><input size="20" type="text" id="EmailUserName" name="email_user_name" value="<?cs var:EmailUserName ?>"></td>
                           </tr>
                           <tr>
                              <td>Password:</td>
                              <td><input size="20" type="password" id="EmailPassword" name="email_password" value="<?cs var:EmailPassword ?>"></td>
                           </tr>
                        </table>
                     </div>

                  </td>
               </tr>
               <tr>
                  <td class="left_column">Sender Email Address</td>
                  <td>
                     <input id="sender_input_field" size="40" type="text" name="sender_email_address" value="<?cs var:SenderEmailAddress ?>">
                     <div id="sender_preview_div" style="display:none;">
                        <div id="sender_preview" class="path_preview" val="<?cs var:html_escape(environment_expand(SenderEmailAddress)) ?>"></div>
                     </div>
                  </td>
               </tr>
               <tr>
                  <td class="left_column">Iguana Host Name for Email Links</td>
                  <td>
                     <input id ="hostname_input_field" size="40" type="text" name="email_link_domain_name" value="<?cs var:EmailLinkDomainName ?>">
                     <a id="HostName_Icon" class="helpIcon" tabindex="100" title="More Information" target="_blank" href="#" onclick="return false;"
                        rel="This host name is the base of the URL that is generated for an email notification message. The current host name is the recommended best choice.<br><br>If a port is included as part of the host name (as in myhost:80), this port is included in the URL. Otherwise, the Iguana web port is used.">
                        <img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" />
                     </a>
                     <div id="hostname_preview_div" style="display:none;">
                        <div id="hostname_preview" class="path_preview" val="<?cs var:html_escape(environment_expand(EmailLinkDomainName)) ?>"></div>
                     </div>
                     <br />
                     <font size="1">
                        <a id="PopulateDomainName">Current host name (recommended)</a>
                     </font>
                  </td>
               </tr>
            </table>

            <?cs if:CurrentUserCanAdmin ?>
            <div id="buttons">
               <input type="hidden" name="Action" class="hidden_submit" value="Apply Changes"/>
               <a id="ApplyChanges" class="action-button blue">Save Changes</a>
               <a class="action-button blue" href="/settings#Page=email_status"><span>Cancel</span></a>
            </div>
            <?cs /if ?>
         </form>
      </div> <!-- /#tabs_and_contents -->
      <?cs /if ?>
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
            <p> From this page, you can configure the email notification server settings for this Iguana server.  </p>
            <?cs call:link_to_users_and_groups() ?>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
               <li>
               <a href="<?cs var:help_link('iguana4_configuring_email_server') ?>" target="_blank">Configuring the Email Server Settings</a>
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
   HLPpopUpinitialize();
   function toggleEmailAuthRow() {
      SettingsHelpers.toggleFormRow("UseEmailAuthenticationCheckbox", ["#EmailAuthenticationFields input", "#EmailAuthenticationCaption input"], ["#EmailAuthenticationFields", "#EmailAuthenticationCaption"]);
   }

   toggleEmailAuthRow();

   $("#UseEmailAuthenticationCheckbox").click(toggleEmailAuthRow);
   function doUpdates() {
      var Input = $("#sender_input_field").get(0);
      var Preview = $("#sender_preview").get(0);
      var PreviewDiv = $("#sender_preview_div").get(0);
      updatePath('EmailSender', Input, Preview, PreviewDiv, true);
      updatePath('HostName', Input, Preview, PreviewDiv, true);
      expandFields(['OutgoingMailServerPort']);
   }

   doUpdates();

   $("#OutgoingMailServerPort").keyup(function() {
      doUpdates();
      VALclearError("MailServerPortRow", "MailServerPortErrorMessageContainer");
   });

   VALregisterIntegerValidationFunction('OutgoingMailServerPort', 'MailServerPortRow', 'MailServerPortErrorMessageContainer', null, null, 1, 65535, true);

   function checkSubmit(event) {
      if ( $("#OutgoingMailServerAddress").val() == "") {
         alert("Please enter a value for the mail server address.");
         $("#OutgoingMailServerAddress").focus();
         return false;
      }
      if ($("#sender_input_field").val() == "") {
         alert("Please enter a value for the sender's email address.");
         $("#sender_input_field").focus();
         return false;
      }
      if ($("#UseEmailAuthenticationCheckbox").prop("checked") && $("#EmailUserName").val() == "") {
         alert("Please enter a value for the user name, or uncheck the authentication checkbox.");
         $("#EmailUserName").focus();
         return false;
      }
      if ($("#UseEmailAuthenticationCheckbox").prop("checked") && $("#EmailPassword").val() == "") {
         alert("Please enter a value for the password, or uncheck the authentication checkbox.");
         $("#EmailPassword").focus();
         return false;
      }

      SettingsHelpers.submitForm(event);
   }

   $("#PopulateDomainName").click(function() {
      var ServerIpAddress = '<?cs var:ServerIpAddress ?>';
      var DomainName = document.domain;
      if ((DomainName == 'localhost' || DomainName == '127.0.0.1') && ServerIpAddress != '') {
         DomainName = ServerIpAddress;
      }
      $("#hostname_input_field").val(DomainName);
   });

   
   $("form#email_settings").submit({uri: "/email_settings/validation", form_id: "email_settings"}, checkSubmit);

   $("a#ApplyChanges").click(function(event) {
      event.preventDefault();
      $(this).blur();
      if (VALvalidateFields()) {
         $("form#email_settings").submit();
      }
   });

});
</script>
