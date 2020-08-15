<?cs # vim: set syntax=html :?>

<link rel="stylesheet" type="text/css" href="<?cs var:skin("transition.css") ?>" />
<style type="text/css">
a.button {
   float:left;
}
</style>

<div id="iguana">

   <div id="cookie_crumb">
      <a href="/settings">Settings</a> &gt; <a href="/settings#Page=channel/group">Channel Groups</a> &gt; <?cs if:Group.Name ?>Edit<?cs else ?>Add<?cs /if ?> Channel Group
   </div>

   <div id="dashboard_body">
      <center>
      <form id="channel_group_data" name="channel_group_data" method="post">
         <div class="workarea">
            <h1> <?cs if:Group.Name ?> Edit Channel Group <?cs else ?> Add Channel Group  <?cs /if ?> </h1>
            <div class="border_green"></div>

            <div id="result" class="<?cs if:ErrorMessage ?>error<?cs elif:StatusMessage ?>success<?cs else?>indeterminate<?cs /if ?>">
               <div class="result_buttons_system">
                  <a id="result_close" ><img src="images/close_button.gif"/></a>
               </div>

               <div id="result_title" class="<?cs if:ErrorMessage ?>error<?cs elif:StatusMessage ?>success<?cs /if ?>">
               <?cs if:ErrorMessage ?>
                  Error
                  <?cs set:Content=ErrorMessage ?>
               <?cs elif:StatusMessage ?>
                  Success
                  <?cs set:Content=StatusMessage ?>
               <?cs /if ?>
               </div>

               <div id="result_content">
                  <?cs var:html_escape(Content) ?>
               </div>
            </div> <!-- /#result -->

            <div id="group_info" >
               <h2>General Information</h2>
               <fieldset>
                  <ul>
                     <li class="generic_control">
                        <div class="label_text">
                           <span>Group Name</span>
                        </div>
                        <div class="field_text">
                        <?cs if:Group.Name != "All Channels" ?>
                           <input type="text" class="text" id="group" name="group" value="<?cs var:html_escape(Group.Name) ?>" maxlength="30" />
                        <?cs else ?>
                           <?cs var:html_escape(Group.Name) ?>
                           <input type="hidden" id="group" name="group" value="<?cs var:html_escape(Group.Name) ?>" />
                        <?cs /if ?>
                        </div>

                        <?cs if:Group.Name ?>
                           <input type="hidden" id="old_group_name" name="old_group_name" value="<?cs var:html_escape(Group.Name) ?>" />
                           <input type="hidden" id="action" name="action" value="editGroup" />
                        <?cs else ?>
                           <input type="hidden" id="old_group_name" name="old_group_name" value="" />
                           <input type="hidden" id="action" name="action" value="createGroup" />
                        <?cs /if ?>
                     </li>
                     <li class="textarea_control">
                        <div class="label_text">
                           <span>Description</span>
                        </div>

                        <div class="field_textarea">
                           <textarea class="group_description" id="description" name="description"><?cs var:html_escape(Group.Description) ?></textarea>
                        </div>
                     </li>
                  </ul>
               </fieldset>
            </div> <!-- /#group_info -->

            <div class="border_grey"></div>

            <div id="membership" <?cs if:Group.Name == "All Channels" ?>style="height:50px;"<?cs /if ?> >
               <h2>Membership</h2>
               <fieldset>
                  <?cs if:Group.Name != "All Channels" ?>
                  <ul>
                     <li class="membership_control">
                        <div class="excluded_membership_set">
                           <h3> Excluded </h3>
                           <div class="membership_set">
                              <div id="channels_left" name="channels_left" class="custom_multi_select">
                              <?cs each:item = Channels ?>
                                 <?cs if:!item.InGroup ?>
                                    <div id="<?cs var:item.ID ?>" class="MSCunselected excluded_member" value="<?cs var:html_escape(item.Name) ?>">
                                       <input class="channel_id" type="hidden" value="<?cs var:item.ID ?>" />
                                       <?cs var:html_escape(item.Name) ?>
                                    </div>
                                 <?cs /if ?>
                              <?cs /each ?>
                              </div>
                              <div class="membership_count">
                                 <span id="element_count_left"></span>
                              </div>
                           </div>
                        </div>

                        <div class="membership_buttons">
                           <div style="height: 28px;">
                              <a style="" id="member_right_arrow" class="action-button-small blue">&nbsp;&nbsp;&gt;&nbsp;&nbsp;</a>
                           </div>
                           <div style="height: 28px;">
                              <a style=""  id="member_left_arrow"  class="action-button-small blue">&nbsp;&nbsp;&lt;&nbsp;&nbsp;</a>
                           </div>
                           <div style="height: 28px;">
                              <a style="" id="member_dbl_right_arrow" class="action-button-small blue">&nbsp;&gt;&gt;&nbsp;</a>
                           </div>
                           <div style="height: 28px;">
                              <a style="" id="member_dbl_left_arrow" class="action-button-small blue">&nbsp;&lt;&lt;&nbsp;</a>
                           </div>
                        </div>

                        <div class="included_membership_set">
                           <h3> Included </h3>
                           <div class="membership_set">
                              <div id="channels_right" name="channels_right" class="custom_multi_select" >
                              <?cs each:item = Channels ?>
                                 <?cs if:item.InGroup ?>
                                    <div id="<?cs var:item.ID ?>"
                                         class="MSCunselected included_member"
                                         value="<?cs var:html_escape(item.Name) ?>"
                                    >
                                       <input class="channel_id" type="hidden" value="<?cs var:item.ID ?>" />
                                       <?cs var:html_escape(item.Name) ?>
                                    </div>
                                 <?cs /if ?>
                              <?cs /each ?>
                              </div>
                              <div class="membership_count">
                                 <span id="element_count_right"></span>
                              </div>
                           </div>
                           <input type="hidden" id="included_channels" name="included_channels" />
                        </div>

                        <div class="filter_field_left">
                           <div class="filter_field">
                              <a class="filter_search" style="cursor:default;">
                                 <img class="filter" src="/images/icon_search.gif" alt="" />
                              </a>
                              <input id="filter_left" />
                              <a id="filter_clear_left" class="filter_clear hidden_place_holder">
                                 <img class="filter" id="clear_filter_icon_left" name="clear_filter_icon_left"
                                      src="/<?cs var:skin("images/ex14.gif") ?>"/>
                              </a>
                           </div>
                           <a id="group_text_query_icon_left" class="helpIcon" tabindex="100" title="More Information" target="_blank" href
                                 rel="<?cs include:"search_tip_channels.cs" ?>
                                 <a href=&quot;<?cs var:help_link('iguana4_channel_groups_searching') ?>&quot; target=&quot;_blank&quot;>Learn More About Specifying Search Criteria In A Channel Group</a></br>">
                              <img class="filter" src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" />
                           </a>

                        </div>

                        <div class="filter_field_right">

                           <div class="filter_field">

                              <a class="filter_search" style="cursor:default;">
                                 <img class="filter" src="/images/icon_search.gif" alt=""/>
                              </a>

                              <input id="filter_right" />

                              <a id="filter_clear_right" class="filter_clear hidden_place_holder">
                                 <img class="filter" id="clear_filter_icon_right" name="clear_filter_icon_right" alt="" src="/<?cs var:skin("images/ex14.gif") ?>"/>
                              </a>

                           </div>

                           <a id="group_text_query_icon_right" class="helpIcon" tabindex="100" title="More Information" target="_blank" href="#"
                           rel="<?cs include:"search_tip_channels.cs" ?>
                           <a href=&quot;<?cs var:help_link('iguana4_channel_groups_searching') ?>&quot; target=&quot;_blank&quot;>Learn More About Specifying Search Criteria In A Channel Group</a></br>">
                              <img class="filter" src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" />
                           </a>
                        </div>
                     </li>
                  </ul>
                  <?cs else ?>
                     This channel group is configured automatically.  It includes all channels.  This cannot be altered.
                  <?cs /if ?>
               </fieldset>
            </div>

            <div class="border_grey" ></div>

            <div id="buttons">
            <?cs if:Group.Name ?>
            <input type="hidden" name="apply_changes" value="Apply Changes" />
            <a class="action-button blue" id="SaveChanges">Save Changes</a>
            <?cs else ?>
            <input type="hidden" name="apply_changes" value="Add Group" />
            <a class="action-button blue" id="SaveChanges">Add Group</a>
            <?cs /if ?>
            <a class="action-button blue" href="/settings#Page=channel/group">Cancel</a>
            </div>

         </div> <!-- /#work_area -->

      </form>

      <!-- This needed to calculate the width of the user names in the membership control. -->
      <?cs each:item = Channels ?>
      <span id="span_<?cs var:item.ID ?>" class="hidden_place_holder">
         <?cs var:html_escape(item.Name) ?>
      </span>
      <?cs /each ?>
      </center>
   </div> <!-- /#dashboard_body -->
</div> <!-- /#iguana -->

<div id="side_panel">
   <div id="side_table">
      <div id="side_header"> Page Help </div>
      <div id="side_body">
         <h4 class="side_title">Overview</h4>
         <p>
            <?cs if:Group.Name ?>
            On this page you can make changes to a channel group.
            <?cs else ?>
            On this page, you can add channels to a channel group.
            <?cs /if ?>
         </p>
         <h4 class="side_title">Related Settings</h4>
         <ul>
            <li> <a href="/settings#Page=roles">Roles & Users</a> </li>
            <li> <a href="/settings#Page=email_status">Email Notification</a> </li>
         </ul>
      </div>
      <div class="side_item">
         <h4 class="side_title">Help Links</h4>
         <ul class="help_link_icon">
            <li>
               <a href="<?cs var:help_link('iguana4_channel_groups_adding_deleting') ?>" target="_blank">Adding or Deleting Channels in a Group</a>
            </li>
            <li>
               <a href="<?cs var:help_link('iguana4_channel_groups_searching') ?>" target="_blank">Searching in a Channel Group</a>
            </li>
         </ul>
      </div>
   </div>
</div>

<div id="helpTooltipDiv" class="helpTooltip">
   <b id="helpTooltipTitle"></b>
   <em id="helpTooltipBody"></em>
   <input type="hidden" name="helpTooltipId" id="helpTooltipId" value="0">
</div>

<script type="text/javascript" src="<?cs var:iguana_version_js("/templates/edit_channel_group.js") ?>"></script>

<script type="text/javascript">
$(document).ready(function() {
   $("form#channel_group_data").submit({uri: '/channel/group', form_id: "channel_group_data"}, SettingsHelpers.submitForm);
   $("a#SaveChanges").click(function(event) {
      event.preventDefault();
      if (! update_group()) { return false; }
      $("form#channel_group_data").submit();
   });
});
</script>


