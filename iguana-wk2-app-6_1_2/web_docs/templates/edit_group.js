/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Javascript routines for edit_group.cs page

function update_role()
{ 
   if("" == $("#role").val())
   {
      showErrorMessage("Roles require a name.");

      document.role_data.role.focus();
      return false;
   }
   else
   {
      if("createRole" == $("#action").val())
      {
         var Result = validateRoleName($("#role").val());

         if(!Result)
         {
            return false;
         }
      }
   } 

   return true;
}

// Selects or deselects all permission of a particular type (i.e. view, reconfigure, startstop) 
// for a given accessor type (i.e. user or role).
function onToggleAllPermissionBoxes(ResourceType, PermissionType, State)
{
   // Toggle the checkboxes within the selected column. 
   for (var i = 0; i < document.role_data.elements.length; i++) 
   {
      var e = document.role_data.elements[i];

      if((e.name != undefined || e.name != null) && 
          (e.name.match("^" + ResourceType + "_" + PermissionType) == ResourceType + "_" + PermissionType) &&
          (e.type == "checkbox") && 
          (e.className == "checkbox")) 
      {
         e.checked = State;
      }
   }

   // If we're dealing with a global select of the view column, then we will have to adjust 
   // the other two columns.
   if("view" == PermissionType)
   { 
      for (var i = 0; i < document.role_data.elements.length; i++) 
      {
         var e = document.role_data.elements[i];

         if((e.name != undefined || e.name != null) && 
          (e.name.match("^" + ResourceType + "_view") == ResourceType + "_view")) 
         {
            var e_reconfigure = document.getElementById(e.id.replace("view", "reconfigure"));
            var e_startstop = document.getElementById(e.id.replace("view", "startstop"));

            if(!e.checked)
            {
               e_reconfigure.checked = false;
               e_reconfigure.className = "checkbox_disabled";
               e_reconfigure.onclick = function(){ return false; };

               e_startstop.checked = false;
               e_startstop.className = "checkbox_disabled";
               e_startstop.onclick = function(){ return false; };
            }
            else
            {
               if(e.className == "checkbox")
               {
                  e_reconfigure.className = "checkbox";
                  e_reconfigure.onclick = function(){ return true; };

                  e_startstop.className = "checkbox";
                  e_startstop.onclick = function(){ return true; };
               }
            }
         }
      }
   }
}

function validateRoleName(Name)
{
   var EncodedName = encodeURIComponent(Name);

   var AJAXCommandName = "validate_user_group_name";
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
                  showErrorMessage(Response.ErrorMessage);
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
         MiniLogin.show('Iguana is not Responding', function() { validateRoleName(Name) });
      }
   });

   return Result;
}

function stripChannelGroupRows()
{
   $("li.channel_group_row:visible").each( function(index)
                                           { 
                                              if(index % 2)
                                              { 
                                                 $(this).removeClass("dark_row").addClass("light_row"); 
                                              }
                                              else
                                              { 
                                                 $(this).removeClass("light_row").addClass("dark_row");
                                              } 
                                           } );
}

// Processes selections made in the viewable channel groups drop down.
function onDropdownChange(Dropdown)
{
   if(!Dropdown || Dropdown.value == "0")
   {
      return;
   }
 
   $("#channel_group_" + Dropdown.value).show();  
   $("#" + Dropdown.value).val("visible");  

   $("#select_all").show();

   for (i = Dropdown.length - 1; i >= 1; i--) 
   {
      if (Dropdown.options[i].selected) 
      {
         Dropdown.remove(i);
         break;
      }
   }      
   
   if(Dropdown.length < 2)
   {
      $("#group_drop_down_cell").hide();
   } 

   Dropdown.value = "0";

   stripChannelGroupRows();

   $("#viewable_channel_group_none_blurb").hide();
}

// Processes clicks on roles remove buttons.
function onRemoveGroup(ID, Name)
{
   var DropDown = $("#group_drop_down")[0];

   $("#channel_group_" + ID).hide();
   $("#" + ID).val("invisible"); 

   var Option = document.createElement('option');
   Option.text = Name;
   Option.value = ID;
   
   var InsertIndex = 1;

   if(DropDown.options.length > 1)
   {
      for (i = 0; i < ChannelGroupNames.length; ++i) 
      {
         if(ChannelGroupNames[i] == Name)
         {
            break;
         }

         if (DropDown.options[InsertIndex].text == ChannelGroupNames[i])
         {
            ++InsertIndex;
         }
      }
   }      

   try {
     DropDown.options.add(Option, InsertIndex); // standards compliant; doesn't work in IE
   }
   catch(ex) {
     DropDown.options.add(Option, DropDown.selectedIndex); // IE only
   }

   if(DropDown.options.length == parseInt($("#channel_group_count").val()) + 1)
   {
      $("#select_all").hide();
   }

   $("#group_drop_down_cell").show();

   stripChannelGroupRows();

   if(!$(".channel_group_view_state[value=visible]").length)
   {
      $("#viewable_channel_group_none_blurb").show();
   }
   else
   {
      $("#viewable_channel_group_none_blurb").hide();
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
   $("li.channel_group_row").removeClass("dark_row_highlighted").addClass("dark_row");
   stripChannelGroupRows();
}

$(document).ready(function() 
{
   $("#group_drop_down").bind("change", function(){ onDropdownChange($(this)[0]); } );

   if("Administrators" == $("#role").val() && "editRole" == $("#action").val())
   {
      $("#group_drop_down_cell").hide();
   }

   $("a.channel_group_toggle").bind("click", function(){ onListViewToggle("channel_group_details_" + $(".group_id", this).val()); } );

   $("a.channel_group_remove").bind("click", function(){ onRemoveGroup($(".group_id", this).val(), $(".group_name", this).val()); } );

   $("#reconfigure_all").attr("href", "javascript:onToggleAllPermissionBoxes('channel_group', 'reconfigure', true);");
   $("#startstop_all").attr("href", "javascript:onToggleAllPermissionBoxes('channel_group', 'startstop', true);");
   $("#exportlogs_all").attr("href", "javascript:onToggleAllPermissionBoxes('channel_group', 'export', true);");
   $("#view_logs_all").attr("href", "javascript:onToggleAllPermissionBoxes('channel_group', 'view_logs', true);");
   $("#translator_ide_all").attr("href", "javascript:onToggleAllPermissionBoxes('channel_group', 'translator_ide', true);");

   $("#reconfigure_none").attr("href", "javascript:onToggleAllPermissionBoxes('channel_group', 'reconfigure', false);");
   $("#startstop_none").attr("href", "javascript:onToggleAllPermissionBoxes('channel_group', 'startstop', false);");
   $("#exportlogs_none").attr("href", "javascript:onToggleAllPermissionBoxes('channel_group', 'export', false);");
   $("#view_logs_none").attr("href", "javascript:onToggleAllPermissionBoxes('channel_group', 'view_logs', false);");
   $("#translator_ide_none").attr("href", "javascript:onToggleAllPermissionBoxes('channel_group', 'translator_ide', false);");

   $("li.channel_group_row").bind("mouseover", function(){ onRowEntered($(this)); } )
                   .bind("mouseout", function(){ onRowExited(); } );

   stripChannelGroupRows();

   if(!$(".channel_group_view_state[value=visible]").length && !("editRole" == $("#action").val() && "Administrators" == $("#role").val()))
   {
      $("#viewable_channel_group_none_blurb").show();
   }
   else
   {
      $("#viewable_channel_group_none_blurb").hide();
   }

   //$("#save").attr("href", "javascript:document.getElementById('apply_changes').click();");
});



