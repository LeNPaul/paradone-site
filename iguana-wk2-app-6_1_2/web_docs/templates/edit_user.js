/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Javascript routines for edit_user.cs page

function alternateEffectivePermissionRowColours() {
   $("li.effective_channel_permissions_row:visible").filter(":even").removeClass("dark_row").addClass("light_row");
   $("li.effective_channel_permissions_row:visible").filter(":odd").removeClass("light_row").addClass("dark_row");   
}

function update_user(){  
   if("" == $("#user").val()) {
      showErrorMessage("Users require a name.");

      document.user_data.user.focus();
      return false;
   } else  {
      if("createUser" == $("#action").val()) {
         var Result = validateUserName($("#user").val());

         if(!Result) {
            return false;
         }
      }
   }  

   if($("#old_password").is(":visible")) {
      var Result = validateUserPassword($("#user").val(), $("#old_password").val());

      if(!Result) {
         return false;
      }      
   }

   var password  = $("#password").val();
   var verify = $("#verify_password").val();
   
   var IsNewPasswordRequired = true;

   if("" != $("#user").val()) { 
      if("none" == $("#password_row").css("display")) {
         IsNewPasswordRequired = false;
      }   
   }
  
   if(MembershipTotal < 1) {
      showErrorMessage("Users must belong to at least one role.");
      return false;         
   } 

   if(IsNewPasswordRequired) {
      if(password == verify) {
         if("" == password) {
            showVerifySubmitMessage("Empty Password Confirmation", "Are you sure you want an empty password?", document.user_data);
            return false;      
         }
      } else {
         showErrorMessage("The passwords do not match.");
         return false;
      }
   }
   return true;
}

// Validates the selected user name.  That is, it ensures that the name is unique.
function validateUserName(Name)
{
   var EncodedName = encodeURIComponent(Name);

   var AJAXCommandName = "validate_user_name";
   var AJAXVariableName = "Name=";

   var Result = true;

   $.ajax({url: AJAXCommandName, data: AJAXVariableName + EncodedName,
      async: false, 
      success: 
      function(data) 
      {              
         try
         {
            var Response = data;
 
            if(Response) 
            {
               Result = Response.Result;

               if(!Result)
               {
                  showErrorMessage(Response.ErrorMessage)
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
         MiniLogin.show('Iguana is not Responding', function() { validateUserName(Name); });
      }
   });

   return Result;
}

// Validates the selected user password.
function validateUserPassword(Name, Password)
{
   var EncodedName = encodeURIComponent(Name);
   var EncodedPassword = encodeURIComponent(Password);

   var AJAXCommandName = "validate_user_password";
   var AJAXVariableName = "Name=";
   var AJAXVariablePassword = "Password=";
   var Result = true;

   $.ajax({url: AJAXCommandName, data: AJAXVariableName + EncodedName + "&" + AJAXVariablePassword + EncodedPassword,
      async: false, 
      success: 
      function(data) 
      {              
         try
         {
            var Response = data;
 
            if(Response) 
            {
               Result = Response.Result;

               if(!Result)
               {
                  showErrorMessage("Invalid current password.")
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
         MiniLogin.show('Iguana is not Responding', function() { validateUserPassword(Name, Pwd); });
      }
   });

   return Result;
}

// Retrieves the user's accumulated effective permissions.
function retrieveUserEffectivePermissions()
{ 
   $("div.effective_permission_button").css("visibility", "hidden");

   $("#effective_permissions_admin_blurb").hide();
   $("#effective_permissions_none_blurb").hide();

   $("input.is_included_role[value=true]").each( function(index){ retrieveEffectivePermissions($(this).attr("id")); } );  

   if("none" != $("#effective_permissions_admin_blurb").css("display")){
      // Nothing to do since we're admin. 
   }
   else if(!$("div.effective_permission_button").filter( function(index){ return "visible" == $(this).css("visibility"); } ).length)
   {
      $("#effective_permissions_list").hide();
      if(0 == parseInt($("#channel_count").val()))
      {
         $("#effective_permissions_none_blurb").html("There are no channels configured.").show();
      }
      else
      {
        $("#effective_permissions_none_blurb").html("This user cannot access any channels or view any channel logs.").show();
      }
   }
   else
   {
      // Hide rows that have no permissions.
      $("div.view_effective_permission_button").each
      ( 
         function(button_index)
         {
            if("hidden" == $(this).css("visibility"))
            { 
               $(this).parents().filter
               ( 
                  function(parent_index)
                  { 
                     return $(this).hasClass("effective_channel_permissions_row"); 
                  } 
               ).hide();
            } 
         } 
      );
   }

   alternateEffectivePermissionRowColours();
}

// Retrieves effective permissions for the given role.  Helper function for retrieveUserEffectivePermissions.
function retrieveEffectivePermissions(RoleID)
{
   var EncodedRoleID = encodeURIComponent(RoleID);

   var AJAXCommandName = "add_effective_permissions";

   var AJAXVariableRoleID = "RoleID=";

   var Result = true;

   $.ajax({url: AJAXCommandName, data: AJAXVariableRoleID + EncodedRoleID,
      async: false, 
      success: 
      function(data) 
      {                        
         try
         {
            var Response = data;
 
            if(Response) 
            {
               if(Response.CanAdmin && "none" == $("#effective_permissions_admin_blurb").css("display"))
               {
                  $("#effective_permissions_admin_blurb").show();
                  $("#effective_permissions_none_blurb").hide();
                  $("#effective_permissions_list").hide();
               }
               else
               {
                  if("none" == $("#effective_permissions_admin_blurb").css("display"))
                  {
                     ProcessEffectivePermissionsResponse(Response);
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
         MiniLogin.show('Iguana is not Responding', function() { retrieveEffectivePermissions(RoleID); });
      }
   });
}

// Extracts user permissions from the Response object.
function ProcessEffectivePermissionsResponse(Response)
{
   var NumberOfChannels = Response.ChannelCount; 
   for(var ChannelIndex = 0; ChannelIndex < NumberOfChannels; ++ChannelIndex)
   {
      var ChannelID = Response["Channel_" + ChannelIndex + "_ID"];

      if(Response["Channel_" + ChannelIndex + "_Viewable"])
      {
         $("#effective_permissions_list").show();
         $("#channel_permissions_row_" + ChannelID).show();
         $("#channel_permissions_row_view_" + ChannelID).css("visibility", "visible");
      }

      if(Response["Channel_" + ChannelIndex + "_StartStopEnabled"])
      {
         $("#channel_permissions_row_startstop_" + ChannelID).css("visibility", "visible");
      }

      if(Response["Channel_" + ChannelIndex + "_Reconfigurable"])
      {
         $("#channel_permissions_row_reconfigure_" + ChannelID).css("visibility", "visible");
      }

      if(Response["Channel_" + ChannelIndex + "_ExportLogsEnabled"])
      {
         $("#channel_permissions_row_export_" + ChannelID).css("visibility", "visible");
      }
      
      if(Response["Channel_" + ChannelIndex + "_ViewLogsEnabled"])
      {
         $("#channel_permissions_row_view_logs_" + ChannelID).css("visibility", "visible");
      }
      
      if(Response["Channel_" + ChannelIndex + "_TranslatorIDEEnabled"])
      {
         $("#channel_permissions_row_translator_ide_" + ChannelID).css("visibility", "visible");
      }
   }
}

function onChangePassword()
{
   $("#change_password_row").hide();

   $("#old_password_row").show();
   $("#password_row").show();
   $("#verify_row").show();

   $("#old_password").val("");
   $("#password").val("");
   $("#verify_password").val("");

   $("#change_password").val("yes");
}

// Processes selection made in the roles drop down.
function onDropdownChange(Dropdown)
{	
   if(!Dropdown || "0" == Dropdown.val())
   {
      return;
   }
 
   $("#role_row_" + Dropdown.val()).show();  
 
   $("#" + Dropdown.val()).val("true"); 

   Dropdown.children().filter("[value=" + Dropdown.val() + "]").remove();
   MembershipTotal++;
   Dropdown.val("0");

   if(Dropdown.children().length < 2)   
   {
      $("#role_drop_down_row").hide();
   } 

   retrieveUserEffectivePermissions();
}

// Processes clicks on roles remove buttons.
function onRemoveRole(ID, Name)
{
   var Dropdown = $("#role_drop_down");

   $("#" + ID).val("false"); 
   $("#" + ID + "_txt").val(""); 
   $("#role_row_" + ID).hide();

   if(!Dropdown.children().filter("[value=" + Name + "]").length)
   {
      Dropdown.append("<option value='" + ID + "'>" + Name + "</option>");
   }

   MembershipTotal--;
   $("#role_drop_down_row").show();

   retrieveUserEffectivePermissions();
}

// Changes the row color on a mouseover.
function onRowEntered(Row)
{
   if(Row.hasClass("dark_row")){
      Row.removeClass("dark_row").addClass("dark_row_highlighted");
   }
   else {
      Row.removeClass("light_row").addClass("light_row_highlighted");
   }
}

// Changes the row color on a mouseexit.
function onRowExited(Row)
{
   if(Row.hasClass("dark_row_highlighted")){
      Row.removeClass("dark_row_highlighted").addClass("dark_row");
   }
   else {
      Row.removeClass("light_row_highlighted").addClass("light_row");
   }
}

var RawEffectivePermissionsDetails = "<div id='effective_permissions_details_content'>%CONTENT%</div>";

// Retrieves the source of the inputted permission.
function showEffectivePermissionsDetails(PermissionButton, ID, PermissionType)
{
   var EncodedPermissionType = encodeURIComponent(PermissionType);

   var AJAXCommandName = "effective_permissions_details";
   var AJAXVariableID = "ID=";
   var AJAXVariablePermissionType = "PermissionType=";

   var PermissionsDetails = "";

   $.ajax({url: AJAXCommandName, data: AJAXVariableID + ID + "&" + AJAXVariablePermissionType + EncodedPermissionType,
      async: false, 
      success: 
      function(data) 
      {                            
         try
         {
            var Response = data;
 
            if(Response) 
            {
               PermissionsDetails = "This permission is derived from the following sources:<br><br><table class='permission_details'><tr><th class='role_header'>Role</th><th class='channel_group_header'>Channel Group</th></tr>";

               var IncludedRoles = $("input.is_included_role[value=true]");

               var NumberOfRoles = Response.RoleCount; 
               for(var RoleIndex = 0; RoleIndex < NumberOfRoles; ++RoleIndex)
               {
                  IncludedRoles.each
                  (
                     function(index)
                     {
                        if($(this)[0].id == Response["Role_" + RoleIndex + "_ID"])
                        {
                           var NumberOfChannelGroups = Response["Role_" + RoleIndex + "_ChannelGroupCount"];

                           if(0 < NumberOfChannelGroups)
                           {
                              // Process member role.
                              PermissionsDetails += "<tr><td class='role_cell'>";
                              PermissionsDetails += Response["Role_" + RoleIndex + "_Name"];
                              PermissionsDetails += "</td><td class='channel_group_cell'>"; 

                              for(var ChannelGroupIndex = 0; ChannelGroupIndex < NumberOfChannelGroups; ++ChannelGroupIndex)
                              {
                                 PermissionsDetails += 0 != ChannelGroupIndex ? ", ": "";
                                 PermissionsDetails += Response["Role_" + RoleIndex + "_ChannelGroup_" + ChannelGroupIndex];
                              }

                              PermissionsDetails += "</td></tr>";
                           }
                        }
                     }
                  ); 
               }

               PermissionsDetails += "</table>";

               PermissionsDetails = RawEffectivePermissionsDetails.replace(/%CONTENT%/g, PermissionsDetails);   
            }
         }
         catch(err)
         {                             
         }        
      },

      error:
      function()
      {       
         MiniLogin.show('Iguana is not Responding', function() { validateUserName(Name); });
      }
   });

   TOOLtooltipLink(PermissionsDetails, null, PermissionButton[0]);      
}

$(document).ready(function() 
{
   // Password form initialization.
   if("yes" == $("#change_password").val())
   {
      onChangePassword();
   }

   $("#show_password_form").bind("click", function(){ onChangePassword(); } );


   $("#password").val("");

   // Role membership initialization.    
   $("#role_drop_down").bind("change", function(){ onDropdownChange($(this)); } );

   var ExcludedRoleIDList = $("li.role_membership_row > input.is_included_role[value=false]");
 
   for(var ExcludedRoleIDListIndex = 0; ExcludedRoleIDListIndex < ExcludedRoleIDList.length; ++ExcludedRoleIDListIndex)
   {
      var ID = ExcludedRoleIDList[ExcludedRoleIDListIndex].id;
      $("li.role_membership_row[id$=" + ID + "]").hide();
   }

   var DropdownOptions = $("#role_drop_down option");
	
   if(DropdownOptions.length < 2)
   {
      $("#role_drop_down_row").hide();
   }

   for(var RemoveRoleButtonIndex = 0; RemoveRoleButtonIndex < $("a.remove_role_button").length; ++RemoveRoleButtonIndex)
   {
      $("a.remove_role_button")[RemoveRoleButtonIndex].href = "javascript:onRemoveRole('" + $(".role_id", this)[RemoveRoleButtonIndex].value + "','" + $(".role_truncated_name", this)[RemoveRoleButtonIndex].value + "');";
   }

   $("div.remove_role_disabled").bind("mouseover", function(){ TOOLtooltipLink('You do not have the necessary permissions to remove this user from the role.', null, this); } )
                                .bind("mouseout", function(){ TOOLtooltipClose(); } )
                                .bind("mouseup", function(){ TOOLtooltipClose(); } );

   $("div.remove_admin_role_disabled").bind("mouseover", function(){ TOOLtooltipLink('You do not have the necessary permissions to remove this user from the Administrators role.', null, this); } )
                                      .bind("mouseout", function(){ TOOLtooltipClose(); } )
                                      .bind("mouseup", function(){ TOOLtooltipClose(); } );

   $("div.remove_not_allowed_disabled").bind("mouseover", function(){ TOOLtooltipLink('You are not allowed to remove this user from the Administrators role.', null, this); } )
                                .bind("mouseout", function(){ TOOLtooltipClose(); } )
                                .bind("mouseup", function(){ TOOLtooltipClose(); } );


   // Effective permissions initialization.
   retrieveUserEffectivePermissions();

   $("li.effective_channel_permissions_row").bind("mouseover", function(){ onRowEntered($(this)); } )
                                            .bind("mouseout", function(){ onRowExited($(this)); } );

   alternateEffectivePermissionRowColours();

   $("div.effective_permission_button").bind("mouseover", function(){ showEffectivePermissionsDetails($(this), $(".channel_id", this).val(), $(".permission_type", this).val() ); } )
                                       .bind("mouseout", function(){ TOOLtooltipClose(); } )
                                       .bind("mouseup", function(){ TOOLtooltipClose(); } ); 

   $("#save").attr("href", "javascript:document.getElementById('apply_changes').click();");
});

// Stuff for disabling user accounts.
$(document).ready(function() {
   var checkbox = $('#enabled');
   var div = $('#disable-confirm');
   div.dialog({
      autoOpen: false,
      modal: true,
      buttons: {
         'Cancel': function() {
            checkbox.click();
            $(this).dialog('close');
         },
         'Disable Account': function() {
            $(this).dialog('close');
         }
      }
   });
   checkbox.change(function() {
      if($(this).val() != 'on')
         div.dialog('open');
   });
});
