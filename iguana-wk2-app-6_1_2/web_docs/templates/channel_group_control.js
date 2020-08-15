/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Javascript routines for channel_grouping_control.cs page

// Removes selected role. 
function remove_channel_group(GroupName)
{
   console.log(GroupName);
   document.remove_channel_group.group.value = GroupName;
   showVerifySubmitMessage("Removal Confirmation", "Are you sure you want to remove the channel group '" + GroupName + "'?", document.remove_channel_group);
}

// Changes the row color on a mouseover.
function onRowEntered(Row)
{
   Row.removeClass("dark_row").removeClass("light_row").addClass("dark_row_highlighted");
}

// Changes the row color on a mouseexit.
function onRowExited()
{
   $("li.channel_group_list_row").removeClass("dark_row_highlighted").addClass("dark_row");
   $("li.channel_group_list_row:nth-child(even)").removeClass("dark_row").addClass("light_row");
}

var RawGroupTooltipDescriptionText = "%DESCRIPTION%<br><br>";
var RawGroupTooltipMemberCountText = "Contains <strong>%MEMBERCOUNT%</strong> of <strong>%CHANNELCOUNT%</strong> channels.";

$(document).ready(function() 
{
   if("true" != $("#current_user_can_admin").val())
   {
      $("#disabled_add_channel_group_button").bind("mouseover", function(){ TOOLtooltipLink('You do not have the necessary permissions to add new channel groups.', null, this); } )
                                             .bind("mouseout", function(){ TOOLtooltipClose(); } )
                                             .bind("mouseup", function(){ TOOLtooltipClose(); } );
   }

   $("li.channel_group_list_row").bind("mouseover", function(){ onRowEntered($(this)); } )
                            .bind("mouseout", function(){ onRowExited(); } );

   $("li.channel_group_list_row:nth-child(even)").removeClass("dark_row").addClass("light_row");

   $("a.channel_group_toggle").bind("click", function(){ onListViewToggle("channel_group_list_details_" + $(".group_id", this).val()); } );
 
   $("a.channel_group_remove").bind("click", function(){
      var GroupName = $(this).find(".channel_group_name").val();
      remove_channel_group(GroupName);
   });

});
