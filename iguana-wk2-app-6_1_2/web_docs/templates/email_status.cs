<?cs # vim: set syntax=html :?>
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

      <?cs call:email_not_setup_page_contents() ?>

   <?cs else ?>

      <div class="tabs_and_contents">

         <?cs call:email_navigation_tabs("Status", "return true;") ?>

         <?cs if:ActionResult ?>
         <p>
            <font color="green"><b><?cs var:ActionResult ?></b></font>
         </p>
         <?cs elif:EmailSettingsValidationFailure ?>
            <p>
               <b><font color="red">Validation was not successful</font></b>
            </p>
            <p>
               <?cs var:EmailSettingsValidationFailure ?>
               <a href="/settings#Page=email_settings/validation/send">Send another validation email message</a>.
            </p>
         <?cs /if ?>

         <?cs if:!EmailSettingsValidated && !SuppressValidationWarning && !EmailSettingsValidationFailure && !EmailDisabled ?>
            <?cs call:email_settings_not_validated_paragraph() ?>
         <?cs else ?>
            <p />
         <?cs /if ?>

         <table class="configuration">
            <tr>
               <th colspan="2">Email Notification Status</th>
            </tr>
            <tr>
               <td class="left_column">Status</td>
               <td>
                  <?cs if:EmailDisabled ?>
                     <img style="position: relative; top: 2px;" src="<?cs var:skin("../images/button-dotgrayv4.gif") ?>" />
                     Disabled
                  <?cs elif:EmailSetup && !EmailSettingsValidated ?>
                     <img style="position: relative; top: 2px;" src="<?cs var:skin("../images/button-dotyellowv4.gif") ?>" />
                     Enabled, not Validated
                  <?cs else ?>
                     <img style="position: relative; top: 2px;" src="<?cs var:skin("../images/button-dotgreenv4.gif") ?>" />
                     Enabled and Validated
                  <?cs /if ?>
               </td>
            </tr>

            <tr>
               <td class="left_column">Last Email Server Settings Change</td>
               <td>
                  <?cs if:EmailNotSetup || !LastEmailSettingsChange ?>
                     - -
                  <?cs else ?>
                     <?cs var:LastEmailSettingsChange ?>
                  <?cs /if ?>
               </td>
            </tr>
            <tr>
               <td class="left_column">Last Email Server Settings Validation</td>
               <td>
                  <?cs if:!LastEmailSettingsValidation ?>
                     - -
                  <?cs else ?>
                     <?cs if:EmailSettingsValidated || SuppressValidationWarning || EmailDisabled?>
                        <?cs var:LastEmailSettingsValidation ?>
                     <?cs else ?>
                        <span style="color: red; font-weight: bold;"><?cs var:LastEmailSettingsValidation ?></span>
                     <?cs /if ?>
                  <?cs /if ?>
               </td>
            </tr>

         </table>


         <div id="buttons">
            <?cs if:EmailDisabled ?>
               <?cs if:CurrentUserCanAdmin ?>
                  <a class="action-button blue" href="/settings#Page=email_status?Action=enable_email_notification">Enable Email Notification</a>
               <?cs /if ?>
            <?cs else ?>
               <?cs if:CurrentUserCanAdmin ?>
                  <a class="action-button blue" href="/settings#Page=email_status?Action=disable_email_notification">Disable Email Notification</a>
               <?cs /if ?>
            <?cs /if ?>

            <?cs if:!EmailDisabled ?>
               <?cs if:CurrentUserCanAdmin ?>
                  <a class="action-button blue" href="/settings#Page=email_settings/validation">Validate Email Server Settings</a>
               <?cs /if ?>
            <?cs /if ?>
         </div>
      </div>

   <?cs /if ?>

   </center>
   </td>
</tr>
</table>

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
            <?cs if:EmailDisabled ?>
               <p>
                  When email notification is enabled, this page displays the email notification status for Iguana.
               </p>
            <?cs else ?>
               <p>
                  This page displays the email notification status for Iguana.
               </p>
            <?cs /if ?>
            <?cs call:link_to_users_and_groups() ?>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
               <li>
                  <a href="<?cs var:help_link('iguana4_config_email') ?>" target="_blank">Email Notification</a>
               </li>
            </ul>
         </td>
      </tr>
   </table>
</div>
