<?cs include:"email_notification_macros.cs" ?>

<script type="text/javascript">

var UpdateIntervalId;
var ShowDetails = false;
var MailerComplete = false;
var RecipientAddress;

function replaceNewlines(data)
{
   return ((data.replace('\r\n', '<br />')).replace('\r', '<br />')).replace('\n', '<br />');
}

function updateMailerStatus(Response) {
   if (MailerComplete) { return; }
   var SendingHeader = document.getElementById('sending_mail_header');
   var SendingParagraph = document.getElementById('sending_mail_paragraph');
   var SendingProgress = document.getElementById('mailer_progress_bar');

   // Update the conversation
   updateDetailedMailerStatus(Response);
   var PercentageComplete = Response.mailer_percentage_complete || 0;
   SendingProgress.style.width = (PercentageComplete*2) + 'px';

   if (PercentageComplete == 100) {
      // No need to do any more updates
      clearInterval(UpdateIntervalId);

      MailerComplete = true;

      document.getElementById('email_settings_link').style.display = 'inline';

      if (Response.mailer_error_message) {
         SendingHeader.innerHTML = '<span style="color: red;">' + Response.mailer_error_message + '</span>';
         SendingParagraph.innerHTML = '';
         document.getElementById('progress_bar_section').style.display = 'none';
         document.getElementById('try_again_link').style.display = 'inline';
         document.getElementById('troubleshooting_link').style.display = 'block';
         $("#try_again_link").click(function() {
            window.location.reload(); 
         });
      } else {
         SendingHeader.innerHTML = '<span style="color: green;">Validation email message has been sent to ' + RecipientAddress + '</span>';
         SendingParagraph.innerHTML = 'Validation will not be complete until a link in the email message is clicked.<br>' +
                                    'If the recipient has not received a validation email message from Iguana, please<br>' +
                                    'confirm that the specified Recipient Email Address is correct.'
         SendingProgress.src = "/<?cs var:skin("images/progress_bar.gif") ?>";
      }
   }
}

function mailerStatusUpdateError() {
   var SendingHeader = document.getElementById('sending_mail_header');
   var DetailsPanel = document.getElementById('email_details_panel');
   SendingHeader.innerHTML = '<span style="color: red;">Loading error</span>';
   DetailsPanel.innerHTML = '<span style="color: red;">Loading error</span>';
}

function mailerStatusRequest() {
   $.ajax({
      url    : '/email_settings/validation/status',
      success: updateMailerStatus,
      error  : mailerStatusUpdateError
   });
}

function updateDetailedMailerStatus(Response) {
   var DetailsPanel = document.getElementById('email_details_panel');
   var LogEntriesHtml = Response.body;
   DetailsPanel.innerHTML = LogEntriesHtml.replace(/&lt;br&gt;/g, '<br />');
}

function beginSendValidationEmail() {
   RecipientAddress = document.getElementById('selectRecipient').value;
   UpdateIntervalId = setInterval('mailerStatusRequest();', 500);

   document.getElementById('divSelectRecipient').style.display = 'none';
   document.getElementById('divSendingEmail').style.display = 'inline';

   document.getElementById('sending_mail_header').innerHTML = 'Sending validation email message to ' + RecipientAddress + '...';
console.log(RecipientAddress);
   $.ajax({
      url    : '/email_settings/validation/begin_send',
      data   : { Recipient: RecipientAddress },
      success: updateMailerStatus,
      error  : mailerStatusUpdateError
   });

   return false; // so the href of the <a> element isn't used
}

function showOrHideDetails()
{
   var DetailsButton = document.getElementById('show_or_hide_details_button');
   var DetailsPanel = document.getElementById('email_details_panel');

   if (ShowDetails)
   {
      ShowDetails = false;
      DetailsButton.innerHTML = '[Show SMTP log...]';
      DetailsPanel.style.display = "none";
   }
   else
   {
      ShowDetails = true;
      DetailsButton.innerHTML = '[Hide SMTP log...]';
      DetailsPanel.style.display = "block";

      if (!MailerComplete)
      {
         // Do an update request now
         mailerStatusRequest();
      }
   }
}
</script>


   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt; <a href="/settings#Page=email_status">Email Notification</a> &gt; Send Validation Email
         </td>
      </tr>
      <tr>
         <td id="dashboard_body">
            <center>

            <?cs if:EmailNotSetup ?>

               <?cs call:email_not_setup_page_contents() ?>

            <?cs elif:EmailDisabled ?>

               <?cs call:email_disabled_page_contents("Status") ?>

            <?cs else ?>

               <div class="tabs_and_contents">

                  <?cs call:email_navigation_tabs("Status", "return true;") ?>

                  <?cs if:AtLeastOneEmailAddress ?>
                  <div id="divSelectRecipient" style="display: inline;">
                     <br />

                     <table class="configuration">
                        <tr>
                           <th colspan="2">Email Settings Validation</th>
                        </tr>
                        <tr>
                           <td>Recipient:</td>
                           <td>
                              <select id="selectRecipient">
                                 <?cs each:possibleRecipient = PossibleRecipients ?>
                                    <?cs if:possibleRecipient.EmailAddress ?>
                                       <?cs set:atLeastOnePossibleRecipient = "true" ?>
                                       <option value="<?cs var:html_escape(possibleRecipient.EmailAddress) ?>"
                                          <?cs if:CurrentUser == possibleRecipient.Name ?>selected<?cs /if ?> >
                                          <?cs var:html_escape(possibleRecipient.Name) ?> &lt;<?cs var:html_escape(possibleRecipient.EmailAddress) ?>&gt;
                                       </option>
                                    <?cs /if ?>
                                 <?cs /each ?>
                              </select>
                              <br />
                              <font color="green" size=1>To edit users, go to <a href="/settings#Page=users">Settings &gt; Roles & Users</a></font>
                           </td>
                        </tr>
                        <tr>
                           <td colspan="2">
                              <center><table><tr><td>
                                 <a class="action-button blue" href="#" onclick="return beginSendValidationEmail();"><span>Send Validation Email</span></a>
                              </td></tr></table></center>
                           </td>
                        </tr>
                     </table>

                  </div>

                  <?cs else ?>

                     <p>
                        To send a validation email message, at least one user with an email address must exist.<br />
                        To edit users, go to <a href="/role_control.html">Settings &gt; Roles & Users</a>.<br />
                     </p>
                     <table><tr><td>
                     <a class="action-button blue" href="/settings#Page=email_status"><span>Back to Email Notification</span></a>

                  <?cs /if ?>

                  <div id="divSendingEmail" style="display: none;">
                     <br />

                     <b id="sending_mail_header">Sending validation email message ...</b>
                     <br />
                     <p id="troubleshooting_link" style="display:none;">
                        Need help <a href="<?cs var:help_link('smtp_troubleshooting') ?>">troubleshooting your SMTP server connection</a>?
                     </p>
                     <div id="progress_bar_section">
                        <br />
                        <div class="progress_bar_container">
                           <img id="mailer_progress_bar" class="progress_bar" src="/<?cs var:skin("images/pulsing_progress_bar.gif") ?>" />
                        </div>
                     </div>
                     <a id="show_or_hide_details_button" href="javascript:showOrHideDetails();">[Show SMTP log...]</a><br />
                     <p id="sending_mail_paragraph">Please wait while Iguana sends the validation email message.</p>
                     <div id="email_details_panel" style="display: none;"></div>

                  </div>

                  <table><tr><td>
                     <a id="try_again_link" class="action-button blue" style="display: none;">
                        <span>Try Again</span>
                     </a>
                     </td><td>
                     <a id="email_settings_link" class="action-button blue" style="display: none;" href="/settings#Page=email_status">
                        <span>Back to Email Notification</span>
                     </a>
                  </td></tr></table>

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
               This page is displayed when Iguana is sending a validation email message.
            </p>
            <?cs call:link_to_users_and_groups() ?>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
               <li>
                  <a href="<?cs var:help_link('iguana4_validating_email_notification') ?>" target="_blank">Validating Email Notification</a>
               </li>
            </ul>
         </td>
      </tr>
   </table>


