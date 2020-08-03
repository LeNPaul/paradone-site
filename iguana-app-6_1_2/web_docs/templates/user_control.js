/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Javascript routines for user_control.cs page

// Removes selected user.
function remove_user(UserName)
{
   document.remove_user.user.value = UserName;

   if(UserName == $("#current_user").val()){
      showVerifySubmitMessageJson("Removal Confirmation", "Are you sure you want to remove the user '" + UserName + "'?", UserName);
   }
   else{
      showVerifySubmitMessage("Removal Confirmation", "Are you sure you want to remove the user '" + UserName + "'?", document.remove_user);
   }
}

// Changes the row color on a mouseover.
function onRowEntered(Row)
{
   Row.removeClass("dark_row").removeClass("light_row").addClass("dark_row_highlighted");
}

// Changes the row color on a mouseexit.
function onRowExited()
{
   $("li.user_row").removeClass("dark_row_highlighted").addClass("dark_row");
   $("li.user_row:nth-child(even)").removeClass("dark_row").addClass("light_row");
}

$(document).ready(function() 
{
   if("true" != $("#current_user_can_admin").val())
   {
      $("#disabled_add_user_button").bind("mouseover", function(){ TOOLtooltipLink('You do not have the necessary permissions to add new users.', null, this); } )
                                    .bind("mouseout", function(){ TOOLtooltipClose(); } )
                                    .bind("mouseup", function(){ TOOLtooltipClose(); } );

      $("div.remove_user_disabled").bind("mouseover", function(){ TOOLtooltipLink('You do not have the necessary permissions to remove this user.', null, this); } )
                                   .bind("mouseout", function(){ TOOLtooltipClose(); } )
                                   .bind("mouseup", function(){ TOOLtooltipClose(); } );


      $("div.edit_user_disabled").bind("mouseover", function(){ TOOLtooltipLink('You do not have the necessary permissions to edit this user.', null, this); } )
                                    .bind("mouseout", function(){ TOOLtooltipClose(); } )
                                    .bind("mouseup", function(){ TOOLtooltipClose(); } );
   }

   $("li.user_row").bind("mouseover", function(){ onRowEntered($(this)); } )
                   .bind("mouseout", function(){ onRowExited(); } );

   $("li.user_row:nth-child(even)").removeClass("dark_row").addClass("light_row");

   $("a.user_remove").bind("click", function(){ remove_user($(".user_name", this).val()); } );
});

