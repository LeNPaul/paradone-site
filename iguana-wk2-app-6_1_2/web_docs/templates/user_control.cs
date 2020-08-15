<?cs # vim: set syntax=html :?>
<link rel="stylesheet" type="text/css" href="<?cs var:skin("transition.css") ?>" />

<div id="confirm_remove_user_from_role" style="display:none;width:450px;overflow:hidden;" title="Remove User">
</div>

<div id="add_user_to_role" style="display:none;width:450px;overflow:hidden;" title="Add User">
</div>

<input id="current_user_can_admin" type="hidden" value="<?cs var:CurrentUserCanAdmin ?>" />
<input id="current_user" type="hidden" value="<?cs var:html_escape(CurrentUser) ?>" />

   <div id="iguana">

      <div id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; Users
      </div>

      <div id="dashboard_body">

         <center>

               <form id="remove_user" name="remove_user">
                  <input type="hidden" name="action" value="removeUser" />
                  <input type="hidden" name="user" />
               </form>

               <div id="result" class="<?cs if:ErrorMessage ?>error<?cs elif:StatusMessage ?>success<?cs else?>indeterminate<?cs /if ?>">

                  <div class="result_buttons_system">
                     <a id="result_close" ><img src="images/close_button.gif"/></a>
                  </div>

                  <div id="result_title" class="<?cs if:ErrorMessage ?>error<?cs elif:StatusMessage ?>success<?cs /if ?>">

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

                  <div id="users">

                     <div class="border_green"></div>

                     <div class="h1">

                        <div class="h1_link" >
                           <span class="h1">Users</span>
                        </div>

                        <div class="h1_button" >

                              <?cs if:CurrentUserCanAdmin ?>

                              <a class="action-button blue" id="AddUser" href="/settings#Page=users/add">Add User</a>

                              <?cs /if ?>

                        </div>

                     </div>

                     <div id="user_list" class="list">

                        <ul>

                           <?cs set:UserCount = #0 ?>

                           <?cs each:item = Users ?>

                           <li class="user_row dark_row">

                              <div id="user_<?cs var:item.ID ?>" class="list_row" >



                                 <div class="list_row_name_cell user_name_text">
                                    <span><?cs var:html_escape(item.Name) ?></span>
                                    <?cs if:CurrentUserCanAdmin || item.Name == CurrentUser ?>
                                       <a style="float:right" class="action-button-small blue"
                                          href="/settings#Page=users/edit?user=<?cs var:url_escape(item.Name) ?>"
                                       >Edit</a>
                                    <?cs /if ?>
                                    <?cs if:CurrentUserCanAdmin ?>
                                       <a style="float:right;<?cs if:item.Name == "admin" ?>visibility:hidden;"<?cs /if ?>"
                                          class="action-button-small blue user_remove"
                                       >
                                          <input class="user_name" type="hidden" value="<?cs var:html_escape(item.Name) ?>" />
                                          Remove
                                       </a>
                                    <?cs /if ?>
                                 </div>

                                 <?cs if:item.Disabled ?>
                                    <strong style="color:red">(account disabled)</strong>
                                 <?cs /if ?>

                                 <div id="user_list_details_<?cs var:item.ID ?>" class="list_row_details">

                                    <?cs set:RoleCount = #0 ?>
                                    <?cs each:memberOf = item.Roles ?>

                                    <a class="EditUser" <?cs if:CurrentUserCanAdmin ?>href="/settings#Page=roles/edit?role=<?cs var:url_escape(memberOf.Name) ?>"<?cs /if ?> class="recipient">
                                       <span><?cs var:html_escape(memberOf.TruncatedName)?></span>
                                    </a>

                                    <?cs set:RoleCount = RoleCount + #1 ?>

                                    <?cs /each ?>

                                    <input id="user_role_count_<?cs var:item.ID ?>" type="hidden" value="<?cs var:#RoleCount ?>" />

                                 </div>

                              </div>

                           </li>

                           <?cs set:UserCount = UserCount + #1 ?>

                           <?cs /each ?>

                        </ul>

                        <?cs if:UserCount == #0 ?>
                        There are no users to view.
                        <?cs /if ?>

                     </div>

                  </div>

               </div>

            </div>

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

         <?cs if:CurrentUserCanAdmin ?>
            <p>Roles are used to control who has access and configuration rights to Iguana.
            Users belong to roles and can have email addresses for
            <a href="/settings#Page=email_settings/rules/view">email notification rules</a>.
            <?cs if:CurrentUser != 'admin' ?>
               <p>Only the "admin" user can modify the Administrators role, or remove
               administrator users.
            <?cs /if ?>
         <?cs else ?>
            <p>From this page, you can edit your own user information.
            <p>You must have administrator permissions to view and edit other users and roles.
         <?cs /if ?>

         <h4 class="side_title">Related Settings</h4>

         <a href="/settings#Page=email_status">Email Notification</a><br/>
         <a href="/settings#Page=channel/group">Channel Groups</a>

      </div>

      <div class="side_item">

         <h4 class="side_title">Help Links</h4>

         <ul class="help_link_icon">

            <li>
            <a href="<?cs var:help_link('iguana4_config_users_groups') ?>" target="_blank">Roles & Users</a>
            </li>

         </ul>

      </div>

    </div>


<script type="text/javascript" src="<?cs var:iguana_version_js("/templates/user_control.js") ?>"></script>
<script type="text/javascript">
$(document).ready(function() {
   $("form#remove_user").submit({uri: "/users", form_id: "remove_user"}, SettingsHelpers.submitForm);
   var HomeHash = '#Page=users';
   $("#AddUser,.EditUser").click(function(event) {
      if (document.location.hash != HomeHash) {
         window.location.reload();
         return;
      }
   return true;
   });

});
</script>

