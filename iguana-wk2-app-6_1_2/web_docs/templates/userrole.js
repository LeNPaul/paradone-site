/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Common routines for user/role permissions
// Displays the error div.
function showErrorMessage(ErrorMessage)
{
   document.getElementById("result").className = "error";
   document.getElementById("result_title").innerHTML = "Error";
   document.getElementById("result_title").className = "result_error";

   $("result_content").text(ErrorMessage);

   $("#result").slideDown("fast");
} 

// Displays the success div.
function showSuccessMessage(SuccessMessage)
{
   document.getElementById("result").className = "success";
   document.getElementById("result_title").innerHTML = "Success";
   document.getElementById("result_title").className = "result_success";

   $("result_content").text(SuccessMessage);

   $("#result").slideDown("fast");
} 

// Displays the generic message div.
function showGenericMessage(Title, Message)
{
   document.getElementById("result").className = "success";
   document.getElementById("result_title").innerHTML = Title;
   document.getElementById("result_title").className = "result_success";

   $("result_content").text(Message);

   $("#result").slideDown("fast");
} 

// Closes the result div.
function onHideResult()
{
   $("#result").slideUp("fast");
}

// Toggles the reconfigure and startstop checkboxes.
function onViewableToggle(Type, ID)
{
   if(document.getElementById(Type + "_view_" + ID).checked)
   {
      document.getElementById(Type + "_reconfigure_" + ID).className = "checkbox";
      document.getElementById(Type + "_startstop_" + ID).className = "checkbox";

      document.getElementById(Type + "_reconfigure_" + ID).onclick = function(){ return true; };
      document.getElementById(Type + "_startstop_" + ID).onclick = function(){ return true; };
   }  
   else
   {
      document.getElementById(Type + "_reconfigure_" + ID).checked = false;
      document.getElementById(Type + "_startstop_" + ID).checked = false;

      document.getElementById(Type + "_reconfigure_" + ID).className = "checkbox_disabled";
      document.getElementById(Type + "_startstop_" + ID).className = "checkbox_disabled";

      document.getElementById(Type + "_reconfigure_" + ID).onclick = function(){ return false; };
      document.getElementById(Type + "_startstop_" + ID).onclick = function(){ return false; };
   } 
}

// Opens or closes associated divs.
function onListViewToggle(View)
{
   var List = document.getElementById(View);
   var ListToggleImg = document.getElementById(View + "_toggle");

   if("none" == List.style.display)
   {
      $("#" + View).slideDown("fast");
      ListToggleImg.src = "/images/arrow-expanded.gif";
   }
   else
   {
      $("#" + View).slideUp("fast");
      ListToggleImg.src = "/images/arrow-contracted.gif";
   }
}

