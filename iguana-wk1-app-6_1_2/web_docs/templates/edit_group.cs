<link rel="stylesheet" type="text/css" href="<?cs var:skin("transition.css") ?>" />
<style type="text/css">
a.button {
   float:left;
}
</style>
   <div id="iguana">
      <div id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; <a href="/settings#Page=roles">Roles</a> &gt; <?cs if:Role.Name ?>Edit<?cs else ?>Add<?cs /if ?> Role
      </div>

      <div id="dashboard_body">
         <center>
            <form name="role_data" id="role_data">
               <div class="workarea" style="width: 640px;">

                  <input id="current_user_can_admin" type="hidden" value="<?cs var:CurrentUserCanAdmin ?>" />
                  <input id="current_user" type="hidden" value="<?cs var:html_escape(CurrentUser) ?>" />

                  <h1>
                     <?cs if:Role.Name ?>
                     Edit Role
                     <?cs else ?>
                     Add Role
                     <?cs /if ?>
                  </h1>

                  <div class="border_green"></div>

                  <div id="result" class="<?cs if:ErrorMessage ?>error<?cs elif:StatusMessage ?>success<?cs else?>indeterminate<?cs /if ?>">

                     <div class="result_buttons_system">
                        <a id="result_close" ><img src="images/close_button.gif"/></a>
                     </div>

                     <div id="result_title" class="<?cs if:ErrorMessage ?>error<?cs elif:StatusMessage ?>success<?cs /if ?>">

                        <?cs if:ErrorMessage ?>

                        Error
                        <?cs set:Content=ErrorMessage ?>

                        <?cs else ?>
<?cs include:"browse_macro.cs" ?>
                        Success
                        <?cs set:Content=StatusMessage ?>

                        <?cs /if ?>

                     </div>

                     <div id="result_content">
                        <?cs var:html_escape(Content) ?>
                     </div>
                  </div>

                  <div id="group_info" >
                     <div class="detail_header">
                        <h2>
                        Role Details
                        </h2>
                     </div>
                     <fieldset>
                        <ul>
                           <li <?cs if:!Role.Name ?>class="generic_control"<?cs /if ?>>
                              <div class="label_text">
                              Name
                              </div>

                              <?cs if:Role.Name ?>

                              <div class="value_text"><?cs var:html_escape(Role.Name) ?>
                              </div>

                              <input type="hidden" id="role" name="role" value="<?cs var:html_escape(Role.Name) ?>" />
                              <input type="hidden" id="action" name="action" value="editRole" />

                              <?cs else ?>

                              <div class="field_text">
                                 <input type="text" class="text" id="role" name="role" maxlength="30" />
                              </div>
                              <input type="hidden" id="action" name="action" value="createRole" />

                              <?cs /if ?>

                           </li>
                           <li class="textarea_control">
                              <div class="label_text">
                              Description
                              </div>
                              <div class="field_text">
                                 <textarea class="group_description" id="description" name="description"><?cs var:html_escape(Role.Description) ?></textarea>
                              </div>
                           </li>
                        </ul>
                     </fieldset>
                  </div>

                  <?cs if:CurrentUserCanAdmin ?>

                  <div id="permissions_border" class="border_grey" ></div>
                  <div id="permissions" >
                     <div class="detail_header">
                     <h2>
                     Channel Groups
                     </h2>
                     </div>

                     <div id="permissions_nonadmin">
                        <fieldset>
                           <ul>
                              <li class="permissions_control">

                                 <?cs if:Role.Name == "Administrators" ?>
                                 <span>The Administrators role has full access to all features in Iguana.  This cannot be modified.</span>
                                 <?cs else ?>

                                 <div id="role_channel_group_list" class="list" >

                                    <?cs if:ChannelGroupCount == 0 ?>
                                       <span>There are no configured channel groups.</span>
                                    <?cs else ?>

                                    <input id="channel_group_count" type="hidden" value="<?cs var:#ChannelGroupCount ?>" />

                                    <div class="permission_row_select_all">

                                       <div class="channel-group-dropdown" >
                                          <div id="group_drop_down_cell" <?cs if:ViewableChannelGroupCount == ChannelGroupCount ?>class="hidden_not_place_holder"<?cs /if ?>>
                                             <select id="group_drop_down">
                                                <option value="0">Add a channel group</option>
                                                <?cs each:item = ChannelGroup ?>
                                                   <?cs if:!item.Viewable ?>
                                                      <option value="<?cs var:item.ID ?>"><?cs var:item.Name ?></option>
                                                   <?cs /if ?>
                                                <?cs /each ?>
                                             </select>
                                          </div>
                                       </div>

                                       <div id="select_all" <?cs if:!ViewableChannelGroupCount ?>class="hidden_not_place_holder"<?cs /if ?> >
                                          <div id="select_all_reconfigure" class="reconfigurable" >
                                             <a id="reconfigure_all"><em> All</em></a> | <a id="reconfigure_none"><em> None</em></a>
                                          </div>
                                          <div id="select_all_startstop" class="startstopenabled label" >
                                             <a id="startstop_all"><em> All</em></a> | <a id="startstop_none"><em> None</em></a>
                                          </div>
                                          <div id="select_all_export" class="exportenabled label" >
                                             <a id="exportlogs_all"><em> All</em></a> | <a id="exportlogs_none"><em> None</em></a>
                                          </div>
                                          <div id="select_all_view_logs" class="view_logs_enabled label" >
                                             <a id="view_logs_all"><em> All</em></a> | <a id="view_logs_none"><em> None</em></a>
                                          </div>

                                          <div id="select_all_translator_ide" class="translator_ide_enabled label" >
                                             <a id="translator_ide_all"><em> All</em></a> | <a id="translator_ide_none"><em> None</em></a>
                                          </div>
                                       </div>
                                    </div>
                                    <ul>

                                       <?cs each:item = ChannelGroup ?>

                                       <li id="channel_group_<?cs var:item.ID ?>" class="channel_group_row dark_row <?cs if:!item.Viewable ?>hidden_not_place_holder<?cs /if ?>">
                                          <div class="permission_row" >
                                             <input class="group_id" type="hidden" value="<?cs var:item.ID ?>" />
                                             <div class="viewable">
                                                <a class="label channel_group_toggle">
                                                   <input class="group_id" type="hidden" value="<?cs var:item.ID ?>" />
                                                   <img id="channel_group_details_<?cs var:item.ID ?>_toggle" src="/images/arrow-contracted.gif" />
                                                   <span id="channel_group_name_<?cs var:item.ID ?>" ><?cs var:html_escape(item.Name) ?></span>
                                                </a>

                                                <a class="channel_group_remove">
                                                   <input class="group_id" type="hidden" value="<?cs var:item.ID ?>" />
                                                   <input class="group_name" type="hidden" value="<?cs var:html_escape(item.Name) ?>" />
                                                   [remove]
                                                </a>

                                             </div>
                                             <div class="reconfigurable label_checkbox" id="channel_group_reconfigure_<?cs var:item.ID ?>_cell" >
                                                <input type="checkbox" id="channel_group_reconfigure_<?cs var:item.ID ?>" name="channel_group_reconfigure_<?cs var:item.ID ?>" <?cs if:item.Reconfigurable ?>checked<?cs /if ?> class="checkbox" />
                                                <label for="channel_group_reconfigure_<?cs var:item.ID ?>">Edit</label>
                                             </div>
                                             <div class="startstopenabled label_checkbox" id="channel_group_startstop_<?cs var:item.ID ?>_cell" >
                                                <input type="checkbox" id="channel_group_startstop_<?cs var:item.ID ?>" name="channel_group_startstop_<?cs var:item.ID ?>" <?cs if:item.StartStopEnabled ?>checked<?cs /if ?> class="checkbox" />
                                                <label for="channel_group_startstop_<?cs var:item.ID ?>">Start/Stop</label>
                                             </div>
                                             <div class="exportenabled label_checkbox" id="channel_group_export_<?cs var:item.ID ?>_cell" >
                                                <input type="checkbox" id="channel_group_export_<?cs var:item.ID ?>" name="channel_group_export_<?cs var:item.ID ?>" <?cs if:item.ExportLogEnabled ?>checked<?cs /if ?> class="checkbox" />
                                                <label for="channel_group_export_<?cs var:item.ID ?>">Export Logs</label>
                                             </div>

                                             <?cs # View Logs is enabled by default for new roles. ?>
                                             <div class="view_logs_enabled label_checkbox" id="channel_group_view_logs_<?cs var:item.ID ?>_cell" >
                                                <input type="checkbox" id="channel_group_view_logs_<?cs var:item.ID ?>" name="channel_group_view_logs_<?cs var:item.ID ?>" <?cs if:item.ViewLogsEnabled || !Role.Name ?>checked<?cs /if ?> class="checkbox" />
                                                <label for="channel_group_view_logs_<?cs var:item.ID ?>">View Logs</label>
                                             </div>
                                             <div class="translator_ide_enabled label_checkbox" id="channel_group_translator_ide_<?cs var:item.ID ?>_cell" >
                                                <input type="checkbox" id="channel_group_translator_ide_<?cs var:item.ID ?>" name="channel_group_translator_ide_<?cs var:item.ID ?>" <?cs if:item.TranslatorIDEEnabled ?>checked<?cs /if ?> class="checkbox" />
                                                <label for="channel_group_translator_ide_<?cs var:item.ID ?>">Translator IDE</label>
                                             </div>
                                          </div>
                                          <div id="channel_group_details_<?cs var:item.ID ?>" class="permissions_row_details hidden_not_place_holder" >
                                             <input class="group_id" type="hidden" value="<?cs var:item.ID ?>" />
                                             <div style="padding-left:10px;" >

                                                <?cs if:item.ChannelCount == #0 ?>

                                                There are no channels in this group.

                                                <?cs else ?>

                                                <ul>

                                                   <?cs each:item_channel = item.Channels ?>

                                                   <li>
                                                      <?cs var:html_escape(item_channel.Name) ?>
                                                   </li>

                                                   <?cs /each ?>

                                                </ul>

                                                <?cs /if ?>

                                             </div>
                                          </div>

                                       </li>

                                       <?cs /each ?>

                                    </ul>

                                    <?cs /if ?>

                                 </div>

                                 <?cs /if ?>

                              </li>
                           </ul>

                           <?cs each:item = ChannelGroup ?>
                           <input id="<?cs var:item.ID ?>" name="<?cs var:item.ID ?>" class="channel_group_view_state" type="hidden" value="<?cs if:!item.Viewable ?>invisible<?cs else ?>visible<?cs /if ?>" />
                           <?cs /each ?>

                           <div id="viewable_channel_group_none_blurb" class="effective_permissions_blurb">
                           There are no viewable channel groups for this role.
                           </div>
                        </fieldset>
                     </div>
                  </div>

                  <?cs /if ?>

                  <div class="border_grey" ></div>
                  <div id="buttons">

                           <?cs if:Role.Name ?>

                           <input type="hidden" id="apply_changes" name="apply_changes" value="Apply Changes">
                           <a class="action-button blue" id="SaveChanges">Save Changes</a>

                           <?cs else ?>

                           <input type="hidden" id="apply_changes" name="apply_changes" value="Add Role">
                           <a class="action-button blue" id="SaveChanges">Add Role</a>
                           <?cs /if ?>

                              <a class="action-button blue" href="/settings#Page=roles">Cancel</a>
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

         <?cs if:Role.Name ?>

         <p>
         On this page, you can change the permissions for the role.
         </p>

         <?cs else ?>

         <p>
         Use this page to add a new role to this Iguana server.
         </p>

         <?cs /if ?>

	 <h4 class="side_title">Related Settings</h4>

         <a href="/settings#Page=email_status">Email Notification</a><br/>
         <a href="/settings#Page=channel/group">Channel Groups</a>

      </div>

      <div class="side_item">

         <h4 class="side_title">Help Links</h4>

         <ul class="help_link_icon">

            <?cs if:Role.Name ?>

            <li>
               <a href="<?cs var:help_link('iguana4_assigning_user_to_group') ?>" target="_blank">Adding a User to a Role</a>
            </li>
            <li>
               <a href="<?cs var:help_link('iguana4_user_group_permissions') ?>" target="_blank">Editing Role Permissions</a>
            </li>

            <?cs else ?>

            <li>
               <a href="<?cs var:help_link('iguana4_creating_user_group') ?>" target="_blank">Creating a Role</a>
            </li>

            <?cs /if ?>

         </ul>

      </div>

   </div>

<script type="text/javascript" src="<?cs var:iguana_version_js("/templates/edit_group.js") ?>"></script>
<script type="text/javascript">
$(document).ready(function() {
   var ChannelGroupNames = new Array(<?cs var:#ChannelGroupCount ?>);
   <?cs set:count = #0 ?>
   <?cs each: group = ChannelGroup ?>
   ChannelGroupNames[<?cs var:count ?>] = "<?cs var:js_escape(group.Name) ?>";
   <?cs set:count = count + #1 ?>
   <?cs /each ?>

   $("form#role_data").submit({uri: '/roles', form_id: "role_data"}, SettingsHelpers.submitForm);
   $("a#SaveChanges").click(function(event) {
      event.preventDefault();
      if (! update_role()) { return false; }
      $("form#role_data").submit();
   });

});

</script>

