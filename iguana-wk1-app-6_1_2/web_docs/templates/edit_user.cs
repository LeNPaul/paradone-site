<link rel="stylesheet" type="text/css" href="<?cs var:skin("transition.css") ?>" />
<style type="text/css">
   a.button {
      float:left;
   }
</style>
<script type="text/javascript">
var RoleNames = [
   <?cs each: role = Roles ?>
      <?cs if: role.Name != 'Administrators' || CurrentUser == 'admin' ?>
         "<?cs var:js_escape(role.Name) ?>",
      <?cs /if ?>
   <?cs /each ?>
    'popme'
];
RoleNames.pop();
var CurrentUser = "<?cs var:js_escape(CurrentUser) ?>";
var CurrentUserCanAdmin = <?cs var:#CurrentUserCanAdmin ?>;
var MembershipTotal = <?cs var:#MembershipTotal ?>;
</script>

   <div id="iguana">
      <div id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; <a href="/settings#Page=users">Users</a> &gt; <?cs if:User.Name ?>Edit<?cs else ?>Add<?cs /if ?> User
      </div>

      <div id="dashboard_body">
         <center>
            <form name="user_data" id="user_data"">

               <div class="workarea">
                  <h1>
                  <?cs if:User.Name ?>
                  Edit User
                  <?cs else ?>
                  Add User
                  <?cs /if ?>
                  </h1>

                  <div class="border_green"></div>

                  <div id="result" class="<?cs if:ErrorMessage ?>error<?cs elif:StatusMessage ?>success<?cs else?>indeterminate<?cs /if ?>">

                     <div class="result_buttons_system">
                        <a id="result_close" ><img src="images/close_button.gif"/></a>
                     </div>

                     <div id="result_title" class="<?cs if:ErrorMessage ?>error<?cs elif:StatusMessage ?>success<?cs else?>indeterminate<?cs /if ?>">
                        <?cs if:ErrorMessage ?>
                        Error
                        <?cs set:Content=ErrorMessage ?>
                        <?cs else ?>
                        Success
                        <?cs set:Content=StatusMessage ?>
                        <?cs /if ?>
                     </div>

                     <div id="result_content">
                        <?cs var:html_escape(Content) ?>
                     </div>

                  </div>

                  <div id="user_info" >

                     <div class="detail_header">
                     <?cs if:User.Name != 'admin' ?>
                        <div style="float:right"><nobr>
                           <label for="enabled"><strong>Enabled</strong></label>
                           <input id="enabled" type="checkbox" name="enabled"
                              <?cs if:!User.Disabled ?>checked<?cs /if ?>
                              <?cs if:User.Name && (CurrentUser == User.Name || !CurrentUserCanAdmin ) ?>disabled<?cs /if ?>>
                           &nbsp;&nbsp;&nbsp;&nbsp;
                        </nobr></div>
                        <div id="disable-confirm" title="Disable Account" style="display:none">
                           Disabled users cannot log in, and are not sent emails or SMS messages.
                           Are you sure you want to disable this account?
                        </div>
                     <?cs /if ?>

                        <h2 style="display:inline">
                        User Information
                        </h2>
                     </div>

                     <fieldset>
                        <ul>
                           <li>
                              <div class="label_text">Username</div>

                              <?cs if:User.Name ?>
                              <div class="value_text">
                                 <?cs var:html_escape(User.Name) ?>
                              </div>
                              <input type="hidden" id="user" name="user" value="<?cs var:html_escape(User.Name) ?>" />
                              <input type="hidden" id="action" name="action" value="editUser" />
                              <?cs else ?>
                              <div class="field_text">
                                 <input class="text" type="text" id="user" name="user" maxlength="30" />
                              </div>
                              <input type="hidden" id="action" name="action" value="createUser" />
                              <?cs /if ?>

                           </li>

                           <li class="generic_control">
                              <div class="label_text">Email Address</div>
                              <div class="field_text">
                                  <input class="text" type="text" id="email" name="email" value="<?cs var:html_escape(User.EmailAddress) ?>" />
                              </div>
                           </li>

                           <li class="generic_control">
                              <div class="label_text">SMS Address</div>
                              <div class="field_text">
                                 <input class="text" type="text" id="sms" name="sms" value="<?cs var:html_escape(User.SmsAddress) ?>" />
                              </div>
                           </li>

                           <?cs if:CurrentUser == User.Name || User.Name && User.CanAdmin && CurrentUser != "admin" ?>
                           <li id="old_password_row" class="generic_control hidden_not_place_holder">
                              <div class="label_text">Current Password</div>
                              <div class="field_text">
                                 <input class="text" id="old_password" name="old_password" type="password" value="" />
                              </div>
                           </li>
                           <?cs /if ?>

                           <li id="password_row" class="generic_control <?cs if:User.Name ?>hidden_not_place_holder<?cs /if ?>">
                              <div class="label_text"><?cs if:User.Name ?>New <?cs /if ?>Password</div>
                              <div class="field_text">
                                 <input class="text" id="password" name="password" type="password" value="" />
                              </div>
                           </li>

                           <li id="verify_row" class="generic_control <?cs if:User.Name ?>hidden_not_place_holder<?cs /if ?>">
                              <div class="label_text">Confirm <?cs if:User.Name ?>New <?cs /if ?>Password</div>
                              <div class="field_text">
                                 <input class="text" id="verify_password" name="verify_password" type="password" value="" />
                              </div>
                           </li>

                           <?cs if:User.Name ?>
                           <li class="generic_control" id="change_password_row">
                              <div>
                                 <a id="show_password_form" class="label_link">[Change Password]</a>
                              </div>
                              <input id="change_password" name="change_password" type="hidden" value=<?cs if:ChangePassword ?>"yes"<?cs else ?>"no"<?cs /if ?> />
                           </li>
                           <?cs /if ?>

                        </ul>
                     </fieldset>
                  </div>

                  <div class="border_grey" ></div>

                  <div id="roles_membership">
                     <div class="detail_header">
                     <h2>
                     Role Membership
                     </h2>
                     </div>

                     <fieldset>
                        <ul>

                           <?cs each:item = Roles ?>
                           <li id="role_row_<?cs var:item.ID ?>" class="generic_control role_membership_row">
                              <input id="<?cs var:item.ID ?>" name="is_included_role_<?cs var:item.ID ?>" class="is_included_role" type="hidden" value="<?cs if:item.IsUserInRole ?>true<?cs else ?>false<?cs /if ?>" />
                              <a <?cs if:CurrentUserCanAdmin ?>href="/settings#Page=roles/edit?role=<?cs var:url_escape(item.Name) ?>"<?cs /if ?> class="recipient">
                                 <span id="role_row_name_<?cs var:item.ID ?>"><?cs var:html_escape(item.TruncatedName)?></span>
                              </a>

                                 <?cs # Only administrators can remove roles, and only admin can unmake administrators except themself. ?>

                                 <?cs if:CurrentUserCanAdmin ?>
                                 <a class="action-button-small blue remove_role_button">
                                    <input class="role_id" type="hidden" value="<?cs var:item.ID ?>" />
                                    <input class="role_truncated_name" type="hidden" value="<?cs var:html_escape(item.TruncatedName) ?>" />
                                    <input class="role_name" type="hidden" value="<?cs var:html_escape(item.Name) ?>" />
                                    Remove
                                 </a>
                                 <?cs /if ?>

                           </li>

                           <?cs /each ?>

                           <?cs if:CurrentUserCanAdmin ?>
                           <li id="role_drop_down_row">
                              <select id="role_drop_down">
                                 <option value="0">Add Role</option>
                                 <?cs each:item = Roles ?>
                                    <?cs if:!item.IsUserInRole ?>
                                 <option value="<?cs var:item.ID?>"><?cs var:html_escape(item.TruncatedName)?></option>
                                    <?cs /if ?>
                                 <?cs /each ?>
                              </select>
                           </li>
                           <?cs /if ?>

                        </ul>

                     </fieldset>
                  </div>

                  <div class="border_grey"></div>

                  <div id="buttons">

                           <?cs if:User.Name ?>
                           <input id="apply_changes" type="hidden" value="Apply Changes">
                           <a class="action-button blue" id="SaveChanges">Save Changes</a>

                           <?cs else ?>

                           <input id="apply_changes" type="hidden" value="Add User">
                           <a class="action-button blue" id="SaveChanges">Add User</a>
                           <?cs /if ?>

                              <a class="action-button blue" href="/settings#Page=users">Cancel</a>
                  </div>

                  <div id="effective_permissions_border" class="border_grey" ></div>

                  <div id="effective_permissions">
                     <h2>
                     Effective Permissions
                     </h2>
                     <fieldset>
                        <div id="effective_permissions_list" style="display:none;">
                           <ul>

                              <?cs set:PermissionsRowCount = #0 ?>

                              <?cs each:item = Channels ?>

                              <li id="channel_permissions_row_<?cs var:item.ID ?>" class="label_text effective_channel_permissions_row dark_row">

                                 <div id="channel_permissions_row_name_<?cs var:item.ID ?>" class="label_permissions">
                                    <?cs var:html_escape(item.Name)?>
                                 </div>

                                 <div id="permissions_set">

                                    <div class="view_effective_permission_button effective_permission_button" id="channel_permissions_row_view_<?cs var:item.ID ?>" >
                                       <input class="channel_id" type="hidden" value="<?cs var:item.ID ?>" />
                                       <input class="permission_type" type="hidden" value="view" />
                                       <span>View</span>
                                    </div>

                                    <div class="edit_effective_permission_button effective_permission_button" id="channel_permissions_row_reconfigure_<?cs var:item.ID ?>" >
                                       <input class="channel_id" type="hidden" value="<?cs var:item.ID ?>" />
                                       <input class="permission_type" type="hidden" value="reconfigure" />
                                       <span>Edit</span>
                                    </div>

                                    <div class="startstop_effective_permission_button effective_permission_button" id="channel_permissions_row_startstop_<?cs var:item.ID ?>" >
                                       <input class="channel_id" type="hidden" value="<?cs var:item.ID ?>" />
                                       <input class="permission_type" type="hidden" value="startstop" />
                                       <span>Start/Stop</span>
                                    </div>

                                    <div class="export_logs_effective_permission_button effective_permission_button" id="channel_permissions_row_export_<?cs var:item.ID ?>" >
                                       <input class="channel_id" type="hidden" value="<?cs var:item.ID ?>" />
                                       <input class="permission_type" type="hidden" value="exportlogs" />
                                       <span>Export Logs</span>
                                    </div>

                                    <div class="view_logs_effective_permission_button effective_permission_button" id="channel_permissions_row_view_logs_<?cs var:item.ID ?>" >
                                       <input class="channel_id" type="hidden" value="<?cs var:item.ID ?>" />
                                       <input class="permission_type" type="hidden" value="view_logs" />
                                       <span>View Logs</span>
                                    </div>

                                    <div class="translator_ide_effective_permission_button effective_permission_button" id="channel_permissions_row_translator_ide_<?cs var:item.ID ?>" >
                                       <input class="channel_id" type="hidden" value="<?cs var:item.ID ?>" />
                                       <input class="permission_type" type="hidden" value="translator_ide" />
                                       <span>Translator IDE</span>
                                    </div>

                                 </div>

                              </li>

                              <?cs set:PermissionsRowCount = PermissionsRowCount + #1 ?>

                              <?cs /each ?>

                              <input id="channel_count" type="hidden" value="<?cs var:#PermissionsRowCount ?>" />

                           </ul>

                        </div>

                        <div id="effective_permissions_none_blurb" class="effective_permissions_blurb">
                        </div>

                        <div id="effective_permissions_admin_blurb" class="effective_permissions_blurb">
                           This user has full access to everything.
                        </div>

                     </fieldset>

                  </div>

               </div>

            </form>

         </center>

      </div>

   </div>

</div>

<div id="side_panel">
   <div id="side_table">

      <div id="side_header">
      Page Help
      </div>

      <div id="side_body">
         <h4 class="side_title">Overview</h4>
         <?cs if:User.Name == CurrentUser ?>
            <p>From this page, you can change your password or the
            email address Iguana uses to send you email notifications.
            <?cs if:!CurrentUserCanAdmin ?>
               <p>Only administrators can change role membership.
            <?cs /if ?>
         <?cs elif:User.Name ?>
            <p>From this page, you can change the user's password or the
            email address Iguana uses to send them email notifications.
            <?cs if:!CurrentUserCanAdmin ?>
               <p>You must be logged in as an administrator to change role membership.
            <?cs /if ?>
         <?cs else ?>
            <p>Use this page to create a new Iguana user.
            <p>Once you have created the user, you will be returned to the
            <a href="/settings#Page=users">user control screen</a>.
         <?cs /if ?>

         <p>Administrators can enable/disable non-administrator accounts.  The admin
         user can enable/disable any account.

         <h4 class="side_title">Related Settings</h4>
         <a href="/settings#Page=email_status">Email Notification</a>
      </div>

      <div class="side_item">
         <h4 class="side_title">Help Links</h4>
         <ul class="help_link_icon">
             <li>
             <?cs if:User.Name ?>
             <a href="<?cs var:help_link('iguana4_config_password') ?>" target="_blank">Change Password</a>
             <?cs else ?>
             <a href="<?cs var:help_link('iguana4_creating_user') ?>" target="_blank">Creating a User</a>
             <?cs /if ?>
             </li>
          </ul>
      </div>
   </div>

<script type="text/javascript" src="<?cs var:iguana_version_js("/templates/edit_user.js") ?>"></script>
<script type="text/javascript">
$(document).ready(function() {
   $("form#user_data").submit({uri: '/users', form_id: "user_data"}, SettingsHelpers.submitForm);
   $("a#SaveChanges").click(function(event) {
      event.preventDefault();
      if (! update_user()) { return false; }
      $("form#user_data").submit();
   });
});
</script>

