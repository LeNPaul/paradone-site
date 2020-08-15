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

               <?cs call:email_disabled_page_contents("Status") ?>

            <?cs else ?>
            <!-- EMAIL SETUP -->

               <div class="tabs_and_contents">

                  <?cs call:email_navigation_tabs("Status", "return true;") ?>

                  <?cs if:ActionResult ?>
                  <p>
                     <b style="color: green"><?cs var:ActionResult ?></b>
                  </p>
                  <?cs elif:EmailSettingsValidated ?>
                  <p>
                     <b style="color: green;">Email settings are validated.</b>
                  </p>
                  <?cs /if ?>

                  <?cs if:EmailSettingsValidated ?>
                  <p>
                     Iguana's email settings have been configured and validated.
                  </p>
                  <?cs else ?>
                  <p>
                     <b style="color: red;">Warning: email notification has not been validated.</b>
                  </p>
                  <p>
                     To ensure that email notification is working properly, click <b>Send Validation Email</b> to send a validation email message.
                  </p>
                  <?cs /if ?>

                  <div id="buttons">
                     <a class="action-button blue" href="/settings#Page=email_settings/validation/send"><span>Send Validation Email</span></a>
                     <?cs if:!EmailSettingsValidated ?>
                        <a class="action-button blue" href="/settings#Page=email_status?Action=ignore_validation_warning"><span>Ignore Warning</span></a>
                     <?cs else ?>
                        <a class="action-button blue" href="/settings#Page=email_status"><span>Back to Email Notification</span></a>
                     <?cs /if ?>
                  </div>

               </div>

            <?cs /if ?>

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
               From this page, you can send a validation email message to confirm that your email notification settings are correct.
            </p>
            <?cs call:link_to_users_and_groups() ?>
         </td>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
               <li>
               <a href="<?cs var:help_link('iguana4_validating_email_notification') ?>" target="_blank">Validating Email Notification</a>
               </li>
            </ul>
         </td>
      </tr>
   </table>

