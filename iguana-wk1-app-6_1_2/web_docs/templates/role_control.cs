<link rel="stylesheet" type="text/css" href="<?cs var:skin("transition.css") ?>" />

<input id="current_user_can_admin" type="hidden" value="<?cs var:CurrentUserCanAdmin ?>" />
<input id="current_user" type="hidden" value="<?cs var:html_escape(CurrentUser) ?>" />
<div id="iguana">
<div id="cookie_crumb">
   <a href="/settings">Settings</a> &gt; Roles
</div>
<div id="dashboard_body">
<center>
   <form id="remove_user" name="remove_user">
      <input type="hidden" name="action" value="removeUser" />
      <input type="hidden" name="user" />
   </form>
   <form name="remove_role" id="remove_role">
      <input type="hidden" name="action" value="removeRole" />
      <input type="hidden" name="role" />
   </form>

   <div id="confirm_remove_user_from_role" title="Remove User" class="hidden_not_place_holder">
      <div id="confirm_content" style="text-align:left;margin-bottom:20px;overflow:hidden;margin-top:20px;margin-bottom:50px;">
      </div>
   </div>

   <div id="add_user_to_role" title="Add User" class="hidden_not_place_holder">
      <div id="choose_add_user_path">
         <div id="add_new_or_existing_user_text" style="display:none;text-align:left;margin-bottom:20px;overflow:hidden;margin-top:20px;margin-bottom:50px;">
            Do you want to add an existing user to the role or do you want to create a new user?
         </div>

         <div id="add_new_user_text" style="display:none;text-align:left;margin-bottom:20px;overflow:hidden;margin-top:20px;margin-bottom:50px;">
            All existing users are in this role.  Do you want to create a new user?
         </div>

         <div id="buttons">
            <a id="create_user_button" class="action-button green">Create a New User</a>
            <a id="select_user_button" class="action-button green">Add an Existing User</a>
            <a id="cancel" class="action-button green" href="javascript:closeConfirmDialog('add_user_to_role');">Cancel</a>
         </div>
      </div>

      <div id="select_existing_user" class="hidden_not_place_holder">
         <div id="select_existing_user_content" style="text-align:left;margin-bottom:20px;overflow:hidden;margin-top:20px;margin-bottom:30px;">
            <div id="select_existing_user_content_todo" style="margin-bottom:20px;">
               Please select a user from the following dropdown.
            </div>
            <center>
               <select id="user_to_add_list">
                  <option value="0">Select a User</option>
               </select>
            </center>
         </div>
         <div id="buttons">
            <a id="add_user_button" class="action-button green" style="float:right;margin-left:5px;">Add User</a>
            <a id="cancel" class="action-button green" href="javascript:closeConfirmDialog('add_user_to_role');" style="float:right;">Cancel</a>
         </div>
      </div>
   </div>

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

   <div class="access_control">
      <div id="roles">
         <div class="h1">
            <div class="h1_link" > <span class="h1">Roles</span> </div>
            <div class="h1_button" >
               <?cs if:CurrentUserCanAdmin ?>
                  <a class="action-button blue" id="AddRole" href="/settings#Page=roles/add">Add Role</a>
               <?cs /if ?>
            </div>
         </div>

         <input type="hidden" id="role_count" value="<?cs var:#RoleCount ?>" />
         <input type="hidden" id="user_count" value="<?cs var:#UserCount ?>" />

         <div id="role_list" class="list">
            <ul>
               <?cs set:RoleCount = #0 ?>
                  <?cs each:item = Roles ?>
                  <li class="role_row dark_row">
                     <input class="role_id" type="hidden" value="<?cs var:item.ID ?>" />
                     <div id="role_<?cs var:item.ID ?>" class="list_row" >
                        <input type="hidden" id="role_index_<?cs var:item.ID ?>" value="<?cs var:#RoleCount ?>" />
                        <!-- TODO Jon M Can I remove these hidden placeholders when fixing HTML? -->
                        <div class="link_left hidden_place_holder">
                           <a class="button button_small" >
                              <span></span>
                           </a>
                        </div>

                        <div id="role_name_cell_<?cs var:item.ID ?>" class="list_row_name_cell role_name_text" >
                           <a class="role_details_toggle label">
                              <img id="role_list_details_<?cs var:item.ID ?>_toggle" src="/images/arrow-expanded.gif" />
                              <span id="role_name_<?cs var:item.ID ?>"><?cs var:html_escape(item.Name) ?></span>
                              <input class="role_id" type="hidden" value="<?cs var:item.ID ?>" />
                              <input class="role_name" type="hidden" value="<?cs var:html_escape(item.Name) ?>" />
                           </a>
                           <span class="role-actions">
                              <?cs if:CurrentUserCanAdmin && item.Name != "Administrators" ?>
                                 <a class="action-button-small blue role_remove" >
                                    <input class="role_id" type="hidden" value="<?cs var:item.ID ?>" />
                                    <input class="role_name" type="hidden" value="<?cs var:html_escape(item.Name) ?>" />
                                    Remove
                                 </a>
                              <?cs /if ?>
                              <?cs if:CurrentUserCanAdmin ?>
                                 <a class="action-button-small blue" href="/settings#Page=roles/edit?role=<?cs var:url_escape(item.Name) ?>" >Edit</a>
                              <?cs /if ?>
                              <?cs if:CurrentUserCanAdmin ?>
                                 <a class="role_add_user action-button-small blue">
                                    <input class="role_id" type="hidden" value="<?cs var:item.ID ?>" />
                                    <input class="role_name" type="hidden" value="<?cs var:html_escape(item.Name) ?>" />
                                    Add User
                                 </a>
                              <?cs /if ?>
                           </span>
                        </div>

                        <?cs set:RoleUserCount = #0 ?>
                        <div id="role_list_details_<?cs var:item.ID ?>" class="list_row_details" >
                           <div>
                              <ul>
                                 <?cs each:item_user = item.Users ?>
                                    <li id="role_list_details_<?cs var:item.ID ?>_user_<?cs var:item_user.ID ?>" class="<?cs if:!item_user.InRole ?>hidden_not_place_holder<?cs /if ?>" >
                                       <?cs var:html_escape(item_user.Name) ?>
                                       <?cs if:item_user.Disabled ?>
                                          <span style="color:red">(disabled)</span>
                                       <?cs /if ?>

                                       <?cs if:CurrentUserCanAdmin && !(item.Name == "Administrators" && item_user.Name == "admin") ?>
                                          <a class="role_remove_user">
                                             <input class="user_id" type="hidden" value="<?cs var:item_user.ID ?>" />
                                             <input class="role_name" type="hidden" value="<?cs var:html_escape(item.Name) ?>" />
                                             <input class="user_name" type="hidden" value="<?cs var:html_escape(item_user.Name) ?>" />
                                             <input id="user_role_count_<?cs var:item_user.ID?>" type="hidden" value="<?cs var:#item_user.RoleCount ?>" />[Remove]</a>
                                       <?cs /if ?>
                                    </li>

                                    <?cs if:item_user.InRole ?>
                                       <?cs set:RoleUserCount = RoleUserCount + #1 ?>
                                    <?cs /if ?>
                                 <?cs /each ?>
                              </ul>
                           </div>

                           <div id="no_user_in_role_<?cs var:item.ID ?>" class="<?cs if:RoleUserCount != #0 ?>hidden_not_place_holder<?cs /if ?>" >
                              <span style="margin-left: 12px">There are no users in this role.</span>
                           </div>
                        </div>
                        <input id="role_user_count_<?cs var:item.ID ?>" type="hidden" value="<?cs var:#RoleUserCount ?>" />
                     </div> <!-- /.list_row -->
                  </li> <!-- /.role_row -->

                  <?cs set:RoleCount = RoleCount + #1 ?>
               <?cs /each ?>
            </ul> <!-- /.role_row -->

            <?cs if:RoleCount == #0 ?>
               <p>There are no roles to view.</p>
            <?cs /if ?>
         </div> <!-- /#role_list -->
      </div> <!-- /#roles -->
   </center>

   </div> <!-- /#access_conrol -->
</div> <!-- /#dashboard_body -->
</div> <!-- /#iguana -->

<div id="side_panel">
   <div id="side_table">
      <div id="side_header"> Page Help </div>
      <div id="side_body">
         <h4 class="side_title">Overview</h4>
         <?cs if:CurrentUserCanAdmin ?>
            <p>Roles are used to control who has access and configuration rights to Iguana.
                  Users belong to roles and can have email addresses for
                  <a href="/settings#Page=email_settings/rules/view">email notification rules</a>.</p>
            <?cs if:CurrentUser != 'admin' ?>
               <p>Only the "admin" user can modify the Administrators role, or remove
               administrator users.</p>
            <?cs /if ?>
         <?cs else ?>
            <p>From this page, you can edit your own user information.</p>
            <p>You must have administrator permissions to view and edit other users and roles.</p>
         <?cs /if ?>
         <h4 class="side_title">Related Settings</h4>
         <a href="/settings#Page=email_status">Email Notification</a><br/>
         <a href="/settings#Page=channel/group">Channel Groups</a>
      </div>

      <div class="side_item">
         <h4 class="side_title">Help Links</h4>
         <ul class="help_link_icon">
            <li> <a href="<?cs var:help_link('iguana4_config_users_groups') ?>" target="_blank">Roles & Users</a> </li>
         </ul>
      </div>
   </div>
</div>

<script type="text/javascript" src="<?cs var:iguana_version_js("/templates/role_control.js") ?>"></script>
<script type="text/javascript">
$(document).ready(function() {
   $("form#remove_role").submit({uri: '/roles', form_id: "remove_role"}, SettingsHelpers.submitForm);
   $("form#remove_user").submit({uri: '/roles', form_id: "remove_user"}, SettingsHelpers.submitForm);
});
</script>

