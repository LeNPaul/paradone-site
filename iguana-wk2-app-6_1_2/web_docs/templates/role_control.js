/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Removes selected role. 
function remove_role(RoleName, RoleID)
{
   if(0 != $("#role_user_count_" + RoleID).val())
   {
      showErrorMessage("Role '" + RoleName + "' is not empty.  You must first empty the role of any users before you can delete the role.");
   }
   else
   {
      document.remove_role.role.value = RoleName;
 
      showVerifySubmitMessage("Removal Confirmation", "Are you sure you want to remove the role '" + RoleName + "'?", document.remove_role);
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
   $("li.role_row").removeClass("dark_row_highlighted").addClass("dark_row");
   $("li.role_row:nth-child(even)").removeClass("dark_row").addClass("light_row");
}

var RawRoleTooltipDescriptionText = "%DESCRIPTION%<br><br>";
var RawRoleTooltipMemberCountText = "Contains <strong>%MEMBERCOUNT%</strong> of <strong>%USERCOUNT%</strong> users.";

// Adds the inputted user from inputted role.
function addUser(RoleName)
{
   var EncodedRoleName = encodeURIComponent(RoleName);

   $("#add_user_to_role").dialog("close");

   var EncodedUserName = encodeURIComponent($("#user_to_add_list").val());

   var AJAXCommandName = "/roles/add_user";
   var AJAXVariableUserName = "UserName=";
   var AJAXVariableRoleName = "RoleName=";

   $.ajax({url: AJAXCommandName, data: AJAXVariableUserName + EncodedUserName + "&" + AJAXVariableRoleName + EncodedRoleName,
      async: false, 
      success: 
      function(data) 
      {
         try
         {
            var Response = data;
 
            if(Response) 
            {
               if(Response.IsError)
               {
                  showErrorMessage(Response.ErrorMessage);
               }  
               else
               {
                  showSuccessMessage(Response.StatusMessage);

                  $("#role_user_count_" + Response.RoleID).val(parseInt($("#role_user_count_" + Response.RoleID).val()) + 1);
                  $("#role_list_details_" + Response.RoleID + "_user_" + Response.UserID).show();
                  $("#user_role_count_" + Response.UserID).val(parseInt($("#user_role_count_" + Response.UserID).val()) + 1);
          
                  // Since this change will affect the role's tooltip, we need to update that.
                  var RoleTooltipHandlerText = RawRoleTooltipDescriptionText.replace(/%DESCRIPTION%/g, Response.Description);
                  RoleTooltipHandlerText += RawRoleTooltipMemberCountText.replace(/%MEMBERCOUNT%/g, Response.MemberCount).replace(/%USERCOUNT%/g, Response.UserCount);

                  $("#role_name_cell_" + Response.RoleID)
                     .unbind("mouseover")
                     .bind("mouseover", function() {
                        TOOLtooltipLink(RoleTooltipHandlerText, null,
                           $("#role_name_" + Response.RoleID)[0]);
                     });

                  $("#no_user_in_role_" + Response.RoleID).hide();
               }
            }
         }
         catch(err)
         {                             
         }        
      },

      error:
      function()
      {
         MiniLogin.show('Iguana is not Responding', function() { removeUser(RoleName, UserName); });
      }
   });
}

// Removes the inputted user from inputted role.
function removeUser(UserName, RoleName)
{
   var EncodedUserName = encodeURIComponent(UserName);
   var EncodedRoleName = encodeURIComponent(RoleName);

   $("#confirm_remove_user_from_role").dialog("close");

   var AJAXCommandName = "/roles/remove_user";
   var AJAXVariableUserName = "UserName=";
   var AJAXVariableRoleName = "RoleName=";

   $.ajax({url: AJAXCommandName, data: AJAXVariableUserName + EncodedUserName + "&" + AJAXVariableRoleName + EncodedRoleName,
      async: false, 
      success: 
      function(data) 
      {         
         try
         {
            var Response = data;
 
            if(Response) 
            {
               if(Response.IsError)
               {
                  showErrorMessage(Response.ErrorMessage);
               }  
               else
               {
                  showSuccessMessage(Response.StatusMessage);

                  $("#role_user_count_" + Response.RoleID).val(Math.max(0, parseInt($("#role_user_count_" + Response.RoleID).val()) - 1));
                  $("#role_list_details_" + Response.RoleID + "_user_" + Response.UserID).hide();
                  $("#user_role_count_" + Response.UserID).val(Math.max(0, parseInt($("#user_role_count_" + Response.UserID).val()) - 1));

                  // Since this change will affect the role's tooltip, we need to update that.
                  var RoleTooltipHandlerText = RawRoleTooltipDescriptionText.replace(/%DESCRIPTION%/g, Response.Description);
                  RoleTooltipHandlerText += RawRoleTooltipMemberCountText.replace(/%MEMBERCOUNT%/g, Response.MemberCount).replace(/%USERCOUNT%/g, Response.UserCount);

                  $("#role_name_cell_" + Response.RoleID)
                     .unbind("mouseover")
                     .bind("mouseover", function() {
                        TOOLtooltipLink(RoleTooltipHandlerText, null,
                           $("#role_name_" + Response.RoleID)[0]);
                     });

                  // Make note of the fact if there are no users in the role.
                  if(0 == Response.MemberCount)
                  {
                     $("#no_user_in_role_" + Response.RoleID).show();
                  }

                  if(RoleName == "Administrators" && $("#current_user").val() == UserName){
		     window.location.reload();
                  }
               }
            }
         }
         catch(err)
         {                             
         }        
      },

      error:
      function()
      {
         MiniLogin.show('Iguana is not Responding', function() { removeUser(RoleName, UserName); });
      }
   });
}

// Deletes users.
function deleteUser(UserName)
{
   var EncodedUserName = encodeURIComponent(UserName);

   $("#confirm_remove_user_from_role").dialog("close");

   var AJAXCommandName = "delete_user";
   var AJAXVariableUserName = "UserName=";

   $.ajax({url: AJAXCommandName, data: AJAXVariableUserName + EncodedUserName,
      async: false, 
      success: 
      function(data) 
      {                        
         try
         {
            var Response = data;
 
            if(Response) 
            {
               // Construct the status message
               var StatusMessage = "User " + UserName + " was successfully deleted."; 
  
               // Refresh the page as the user can be in any number of roles.
               window.parent.document.location = "/settings#Page=roles?status_message=" + encodeURIComponent(StatusMessage);
            }
         }
         catch(err)
         {                             
         }        
      },

      error:
      function()
      {      
         MiniLogin.show('Iguana is not Responding', function() { deleteUser(UserName); });
      }
   });
}

// Shows a dialog allowing the selection of a user to include into the inputted role.
function selectUser(RoleName)
{
   var EncodedRoleName = encodeURIComponent(RoleName);

   $("#add_user_button")
      .unbind("click")
      .bind("click", function(){ addUser(RoleName); } );

   var AJAXCommandName = "retrieve_excluded_user_list";
   var AJAXVariableRoleName = "RoleName=";

   $.ajax({url: AJAXCommandName, data: AJAXVariableRoleName + EncodedRoleName,
      async: false, 
      success: 
      function(data) 
      {                     
         try
         {
            var Response = data;
 
            if(Response) 
            {
               var ExcludedUserCount = Response.ExcludedUserCount;

               var DropDown = $("#user_to_add_list")[0];
               DropDown.options.length = 1;
  
               for(var ExcludedUserListIndex = 0; ExcludedUserListIndex < ExcludedUserCount; ++ExcludedUserListIndex)
               { 
                  var ExcludedUserTruncatedName = eval("Response.ExcludedUserTruncatedName" + ExcludedUserListIndex);                     
                  var ExcludedUserName = eval("Response.ExcludedUserName" + ExcludedUserListIndex);
                  var ExcludedUserID = eval("Response.ExcludedUserID" + ExcludedUserListIndex);

                  var Option = document.createElement('option');
                  Option.text = ExcludedUserTruncatedName;
                  Option.value = ExcludedUserName;
   
                  try {
                     DropDown.options.add(Option, DropDown.options.length); // standards compliant; doesn't work in IE
                  }
                  catch(ex) {
                     DropDown.options.add(Option, $("#user_to_add_list")[0].selectedIndex); // IE only
                  }
              }
 
              $("#choose_add_user_path").hide();
              $("#select_existing_user").show();
            }
         }
         catch(err)
         {                             
         }        
      },

      error:
      function()
      {           
         MiniLogin.show('Iguana is not Responding', function() {
            selectUser(RoleName);
         });
      }
   });
}

// Shows jQuery dialog to confirm the user removal from the role.
function onAddUserToRole(RoleName, RoleID)
{
   var EncodedRoleName = encodeURIComponent(RoleName);

   $("#select_existing_user").hide();
   $("#choose_add_user_path").show();

   if($("#role_user_count_" + RoleID).val() == $("#user_count").val()) {
      $("#select_user_button").hide();
      $('#add_new_or_existing_user_text').hide();
      $('#add_new_user_text').show();
   } else {
      $("#select_user_button").show()
         .unbind("click")
         .bind("click", function() {
            selectUser(RoleName);
         });
      $('#add_new_or_existing_user_text').show();
      $('#add_new_user_text').hide();
   }

   var Dlg = $("#add_user_to_role");
   Dlg.dialog({
      bgiframe: true,
      width: 500,
      modal: true
   });
   Dlg.dialog("open");
   var NewBut = $("#create_user_button");
   NewBut.click(function(event) {
      Dlg.dialog("close");
      document.location.hash = "#Page=users/add";
   });
}

// Shows jQuery dialog to confirm the user removal from the role.
function onRemoveUserFromRole(UserName, RoleName, UserID) {
   var Dlg = $("#confirm_remove_user_from_role");
   var Buttons = {
      "Delete user": function() {
         removeUserFromSystem(UserName);
         Dlg.dialog("close");
         Dlg.dialog("destroy");
      },
      "Remove from role": function() {
         removeUser(UserName, RoleName);
         Dlg.dialog("close");
         Dlg.dialog("destroy");
      },
      "Cancel": function() {
         Dlg.dialog("close");
         Dlg.dialog("destroy");
      }
   };
   if("1" == $("#user_role_count_" + UserID).val()) {
      delete Buttons["Remove from role"];
      $("#confirm_content").html("This user is a member of this role only.  Do you want to delete the user entirely?");
   } else if("admin" == UserName || $("#current_user").val() == UserName) {
      delete Buttons["Delete user"];
      $("#confirm_content").html("Do you want to remove the user from the role?");
   } else {
      $("#confirm_content").html("Do you want to remove the user from the role or delete them entirely?");    
   }
   Dlg.dialog({
	 bgiframe: true,
	 width: 500,
         modal: true,
         buttons: Buttons
      });

   $("#confirm_remove_user_from_role").dialog("open");
}

// Closes the user removal confirmation dialog.
function closeConfirmDialog(DialogId)
{
   $("#" + DialogId).dialog("close");
}

$(document).ready(function() 
{
   if ("true" != $("#current_user_can_admin").val()) {
      $("div.button_disable").bind("mouseover", function() {
         TOOLtooltipLink($(".tooltip_message", this).val(), null, this);
      }).bind("mouseout", function(){
         TOOLtooltipClose();
      }).bind("click", function() {
         TOOLtooltipClose(); 
      });

      $("span.disabled_link").bind("mouseover", function() {
         TOOLtooltipLink( $(".tooltip_message", this).val(), null, this);
      }).bind("mouseout", function() {
         TOOLtooltipClose();
      }).bind("click", function() {
         TOOLtooltipClose();
      });
   }

   $("#result_close").bind("click", function(){
      onHideResult();
   });

   $("li.role_row").bind("mouseover", function() {
      onRowEntered($(this));
   }).bind("mouseout", function() {
      onRowExited();
   });

   $("li.role_row:nth-child(even)").removeClass("dark_row").addClass("light_row");

   $("a.role_details_toggle").bind("click", function() {
      onListViewToggle("role_list_details_" + $(".role_id", this).val());
   });
   
   $("a.role_remove").bind("click", function() {
      remove_role($(".role_name", this).val(), $(".role_id", this).val());
   });
   
   $("a.role_add_user").bind("click", function() {
      onAddUserToRole($(".role_name", this).val(), $(".role_id", this).val());
   });
   
   $("a.role_remove_user").bind("click", function(){
      onRemoveUserFromRole( $(".user_name", this).val(), $(".role_name", this).val(), $(".user_id", this).val() );
   });

});
