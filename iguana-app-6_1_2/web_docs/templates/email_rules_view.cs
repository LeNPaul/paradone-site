<?cs # vim: set syntax=html :?>
<?cs include:"email_notification_macros.cs" ?>

<script type="text/javascript">

$(function() {
   $(".confirm-remove-email-rule").on("click", function(e) {
      var DeleteRule = confirm('Remove this email notification rule?');
      var RuleGuid = $(e.target).data('rule-guid');
 
      if (DeleteRule) {
         location.hash = "#Page=email_settings/rules/view?Action=remove_rule&RuleKey=" + RuleGuid;
      }
   });
});

var Tooltips = {};
<?cs each:possibleRecipient = PossibleRecipients ?>
Tooltips['<?cs var:js_escape(possibleRecipient.Name) ?>'] = "<?cs var:js_escape(possibleRecipient.Tooltip) ?>";
<?cs /each ?>
<?cs each: r = BadRecipients ?>
   Tooltips['<?cs var:js_escape(r.Name) ?>'] = '<?cs var:js_escape(r.Tooltip) ?>';
<?cs /each ?>

var CurrentTab = null;
var CurrentTabContents = null;

function getTooltip(RecipientName)
{
   if (Tooltips[RecipientName] == undefined)
   {
      return "User or role does not exist.";
   }
   else
   {
      return Tooltips[RecipientName];
   }
}

function showTooltip(aRecipientObject, RecipientName)
{
   TOOLtooltipLink(getTooltip(RecipientName), null, aRecipientObject);
}
</script>

<?cs def:rule_summary(rule) ?>
   <?cs if:rule.RuleType == "Standard" ?>
      <?cs if:rule.SourceIsAllEntries ?><b>Any source</b>
      <?cs elif:rule.SourceIsService ?><b>Iguana service</b>
      <?cs elif:rule.SourceIsUnknown ?>
         <b style="color: red;">Unknown source <cs var:rule.SourceType ?></b>
      <?cs elif:rule.SourceType == "Channel" ?>
         <b>Channel</b> '<a href="/channel?#Channel=<?cs var:url_escape(rule.SetSource) ?>"><?cs var:html_escape(rule.SetSource) ?></a>'
      <?cs elif:rule.SourceType == "Group" ?>
         Any channel in <b>Group</b> '<a href="/settings#Page=channel/group/edit?group=<?cs var:url_escape(rule.SetSource) ?>"><?cs var:html_escape(rule.SetSource) ?></a>'
      <?cs /if ?>
      logs
      <?cs if:rule.LogTypeIsAllTypes ?><b>any</b> message
      <?cs else ?><?cs var:rule.LogTypeHtml ?>
      <?cs /if ?>
      <?cs if:rule.TextQuery != "" ?>
         matching text query:
         <b><?cs var:html_escape(rule.TextQuery) ?></b>
      <?cs /if ?>
   <?cs elif:rule.RuleType == "Channel Inactivity" ?>
      <?cs if:rule.SourceIsUnknown ?>
         <b style="color: red;">Unknown source</b>
      <?cs else ?>
         <?cs if:rule.InactivitySourceType == "Channel" ?>
            <b>Channel</b> '<a href="/channel#Channel=<?cs var:url_escape(rule.SetSource) ?>"><?cs var:html_escape(rule.SetSource) ?></a>'
         <?cs elif:rule.InactivitySourceType == "Group" ?>
             Any channel in <b>Group</b> '<a href="/settings#Page=channel/group/edit?group=<?cs var:url_escape(rule.SetSource) ?>"><?cs var:html_escape(rule.SetSource) ?></a>'
         <?cs /if ?>
      <?cs /if ?>
      is inactive for <b><?cs var:rule.InactivityInterval ?> <?cs var:rule.UserFriendlyInactivityIntervalUnits ?></b>
   <?cs else ?>
      Unknown rule type
   <?cs /if ?>
<?cs /def ?>

<?cs # Yes, it's a little hard to read, but it's written this way to avoid whitespace ?>
<?cs def:rule_test_url(rule)
   ?>/log_browse?Source=<?cs
   if:rule.SourceIsAllEntries ?><?cs
      elif:rule.SourceIsService ?>+Iguana<?cs
      else ?><?cs var:url_escape(rule.SetSource) ?><?cs
   /if ?>&Type=<?cs
   if:!rule.LogTypeIsAllTypes ?><?cs var:rule.LogTypeGetValue ?><?cs
   /if ?><?cs
   if:rule.TextQuery != ""
      ?>&Filter=<?cs var:url_escape(rule.TextQuery) ?><?cs
   /if ?><?cs
/def ?>

<?cs def:recipient_summary(rule) ?>
   <?cs if:rule.CountOfRecipient == 0 ?>
      <b style="color: red;">None</b>
   <?cs else ?>
      <?cs each:recipient = rule.Recipients ?>
         <a <?cs if:recipient.IsUser ?>
               href="/settings#Page=users/edit?user=<?cs var:url_escape(recipient.Name) ?>"
            <?cs else ?>
               href="/settings#Page=roles/edit?role=<?cs var:url_escape(recipient.Name) ?>"
            <?cs /if ?>
            <?cs if:!CurrentUserCanAdmin && CurrentUser != recipient.Name ?>onclick="return false;"<?cs /if ?>
            class="recipient<?cs if:!CurrentUserCanAdmin && CurrentUser != recipient.Name ?> recipient_no_link<?cs /if
                          ?><?cs if:!recipient.Available ?> recipient_broken<?cs /if ?>"
            onmouseover="showTooltip(this, '<?cs var:js_escape(recipient.Name) ?>');"
            onmouseout="TOOLtooltipClose();">
         <span><?cs var:html_escape(recipient.NameToDisplay) ?></span></a>
      <?cs /each ?>
   <?cs /if ?>
<?cs /def ?>

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

               <?cs elif:EmailDisabled ?>

                  <?cs call:email_disabled_page_contents("Rules") ?>

               <?cs else ?>

                  <div class="tabs_and_contents">

                     <?cs call:email_navigation_tabs("Rules", "return true;") ?>

                     <?cs if:ActionResult ?>
                     <p>
                        <font color="green"><b><?cs var:ActionResult ?></b></font>
                     </p>
                     <?cs /if ?>
                     <?cs if:ErrorResult ?>
                     <p>
                        <font color="red"><b><?cs var:html_escape(ErrorResult) ?></b></font>
                     </p>
                     <?cs /if ?>

                     <?cs if:!EmailSettingsValidated && !SuppressValidationWarning ?>
                        <?cs call:email_settings_not_validated_paragraph() ?>
                     <?cs else ?>
                        <p />
                     <?cs /if ?>

                     <table class="configuration">
                        <?cs if:CountOfEmailNotificationRule == 0 ?>
                           <tr>
                              <td colspan="4" style="color: gray; text-align: center;">&lt;None&gt;</td>
                           </tr>
                        <?cs else ?>
                           <tr>
                              <th>Rule</th>
                              <th>Summary</th>
                              <th>Action</th>
                           </tr>

                           <?cs each:rule = EmailNotificationRules ?>
                           <tr>
                              <td class="left_column"><?cs var:name(rule)+1 ?></td>
                              <td>
                                 <table>
                                    <tr>
                                       <td>Trigger:</td>
                                       <td>
                                          <?cs call:rule_summary(rule) ?>
                                          <?cs if:rule.RuleType == "Standard" ?>
                                             <br />
					     <?cs if: rule.SourceType != "Group" && !rule.SourceIsUnknown ?>
                                                <a style="text-decoration:none;" href="<?cs call:rule_test_url(rule) ?>" target="_blank">[Find matching log entries]</a>
					     <?cs /if ?>
                                          <?cs /if ?>
                                       </td>
                                    </tr>
                                    <?cs if:rule.UseMaximumEmailsPerHour ?>
                                       <tr>
                                          <td>Limit:</td>
                                          <td><b><?cs var:rule.MaximumEmailsPerHour ?></b> per hour
                                       </tr>
                                    <?cs /if ?>
                                    <tr class="recipient_row">
                                       <td>Recipients:</td>
                                       <td><?cs call:recipient_summary(rule) ?></td>
                                    </tr>
                                 </table>
                              </td>

                                 <td>
                                    <table>
                                       <tr><td>
                                          <?cs if:CurrentUserCanAdmin ?>
                                          <a class="action-button blue" href="/settings#Page=email_settings/rules/edit?RuleKey=<?cs var:rule.Guid ?>">Edit</a>
                                          <?cs else ?>
                                          <span class="config_warning">You do not have the necessary permissions to edit a rule.</span>
                                          <?cs /if ?>

                                       </td></tr>
                                       <tr><td>
                                          <?cs if:CurrentUserCanAdmin ?>
                                          <a class="confirm-remove-email-rule action-button blue" data-rule-guid="<?cs var:rule.Guid ?>">Remove</a>
                                          <?cs else ?>
                                          <span class="config_warning">You do not have the necessary permissions to remove a rule.</span>
                                          <?cs /if ?>
                                       </td></tr>
                                    </table>
                                 </td>

                           </tr>
                           <?cs /each ?>

                        <?cs /if ?>
                     </table>

                        <table><tr><td>
                           <?cs if:CurrentUserCanAdmin ?>
                           <a class="action-button blue" href="/settings#Page=email_settings/rules/edit?Action=addRule">Add a Rule</a>
                           <?cs else ?>
                              <a class="action-button disabled grey" onMouseOver="TOOLtooltipLink('You do not have the necessary permissions to enable email notification.', null, this);" onMouseOut="TOOLtooltipClose();" onmouseup="TOOLtooltipClose();">Add a Rule</a>                           
                           <?cs /if ?>
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
            <?cs if:EmailDisabled ?>
               <p>
                  When email notification is enabled, this page displays the email notification rules for Iguana.
               </p>
            <?cs else ?>
               <p>
                  This page displays the email notification rules for Iguana.
               </p>
               <?cs if:CurrentUserCanAdmin ?>
                  <?cs if:EmailSetup ?>
                     <p>Click <b>Add a Rule</b> to add an email notification rule.</p>
                  <?cs /if ?>
               <?cs else ?>
                  <p>You must have administrator permissions to add an email notification rule.</p>
               <?cs /if ?>
            <?cs /if ?>
            <?cs call:link_to_users_and_groups() ?>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
               <li>
                  <a href="<?cs var:help_link('iguana4_email_notification_rule_examples') ?>" target="_blank">Email Notification Rule Examples</a>
               </li>
            </ul>
         </td>
      </tr>
   </table>

