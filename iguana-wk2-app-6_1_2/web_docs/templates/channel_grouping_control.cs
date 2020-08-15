<?cs # vim: set syntax=html :?>
<link rel="stylesheet" type="text/css" href="<?cs var:iguana_version_js("/transition.css") ?>" />
<style type="text/css">
div.warning_message {
   height:35px;
   width:310px;
}

div.warning_text {
   position:relative;
   top:10px;
}
</style>

   <div id="iguana">
      <div id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; Channel Groups
      </div>

      <div id="dashboard_body">

         <center>

            <?cs if:!CurrentUserCanAdmin ?>
                  <p style="text-align: center">Only administrators can view and edit channel groups.</p>
            <?cs else ?>

            <form name="remove_channel_group" id="remove_channel_group">
               <input type="hidden" name="action" value="removeGroup" />
               <input type="hidden" name="group" />
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

            <div class="border_green"></div>

            <div class="access_control">

               <div class="h1">

                  <div class="h1_link" >
                     <span class="h1">Channel Groups</span>
                  </div>

                  <div class="h1_button" >

                     <?cs if:CurrentUserCanAdmin ?>

                     <table style="border:0px;padding:0px;margin:0px;"><tr><td style="border:0px;padding:0px;margin:0px;">

                        <a class="action-button blue" id="AddChannelGroup" href="/settings#Page=channel/group/add" onclick="this.blur();">Add Channel Group</a>

                     </td></tr></table>

                     <?cs /if ?>

                  </div>

               </div>

               <div id="channel_group_list" class="list">

                  <?cs if:ChannelGroupCount == #0 ?>
                  There are no channel groups to view.
                  <?cs else ?>

                  <ul>

                     <?cs set:Count = #0 ?>

                     <?cs each:item = ChannelGroups ?>

                     <li class="channel_group_list_row dark_row">

                        <div id="channel_group_<?cs var:item.ID ?>" class="list_row">

                           <input type="hidden" id="channel_group_index_<?cs var:item.ID ?>" value="<?cs var:#Count ?>" />

                           <div class="list_row_name_cell label_text">
                              <a class="channel_group_toggle label">
                                 <input class="group_id" type="hidden" value="<?cs var:item.ID ?>" />
                                 <input class="group_name" type="hidden" value="<?cs var:html_escape(item.Name) ?>" />
                                 <img id="channel_group_list_details_<?cs var:item.ID ?>_toggle" src="/images/arrow-contracted.gif" />
                                 <span id="channel_group_name_<?cs var:item.ID ?>" ><?cs var:html_escape(item.Name) ?></span>
                              </a>
                           </div>

                           <div class="link_left_channel_group">

                              <?cs if:CurrentUserCanAdmin ?>

                                 <a class="action-button-small blue EditChannelGroup" href="/settings#Page=channel/group/edit?group=<?cs var:url_escape(item.Name) ?>" >Edit</a>

                              <?cs /if ?>

                           </div>

                           <div class="link_left_channel_group">

                              <?cs if:CurrentUserCanAdmin && item.Name != "All Channels" ?>

                                 <a class="action-button-small blue channel_group_remove">
                                    <input class="channel_group_name" type="hidden" value="<?cs var:html_escape(item.Name) ?>" />Remove</a>

                              <?cs /if ?>

                           </div>

                           <div id="channel_group_list_details_<?cs var:item.ID ?>" class="list_row_details hidden_not_place_holder">

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
                     <?cs set:Count = Count + #1 ?>
                     <?cs /each ?>
                 </ul>
                 <?cs /if ?>
              </div>
            </div>
            <?cs /if ?>
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
         <p>
         From this page, you can create and manage channel groups. These groups are useful for quickly setting up <a href="/settings#Page=email_settings/rules/view">email notification rules</a>.
         </p>
         <?cs else ?>
         <p>
         From this page, you can create and manage channel groups.
         </p>
         <p>
         You must have administrator permissions to edit channel groups.
         </p>
         <?cs /if ?>

         <h4 class="side_title">Related Settings</h4>
         <ul>
            <li>
            <a href="/settings#Page=roles">Roles & Users</a>
            </li>
            <li>
            <a href="/settings#Page=email_status">Email Notification</a>
            </li>
         </ul>
      </div>
      <div class="side_item">
         <h4 class="side_title">Help Links</h4>
         <ul class="help_link_icon">
         <li>
         <a href="<?cs var:help_link('iguana4_channel_groups') ?>" target="_blank">Channel Groups</a>
         </li>
         </ul>
      </div>

    </div>
<script type="text/javascript" src="/templates/channel_group_control.js"></script>
<script type="text/javascript">
$(document).ready(function() {
   $("form#remove_channel_group").submit({uri: "/channel/group", form_id: "remove_channel_group"}, SettingsHelpers.submitForm);
   var HomeHash = '#Page=channel/group';
   $("#AddChannelGroup,.EditChannelGroup").click(function(event) {
      if (document.location.hash != HomeHash) {
         window.location.reload();
         return;
      }
      return true;
   });
});
</script>


