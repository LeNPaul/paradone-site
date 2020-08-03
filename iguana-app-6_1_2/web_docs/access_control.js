/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Opens or closes associated divs.
function onListViewToggle(View)
{
   var List = $("#" + View);
   var ListToggleImg = $("#" + View + "_toggle");

   if("none" == List.css("display"))
   {
      List.slideDown("fast");
      ListToggleImg.attr("src", "/images/arrow-expanded.gif");
   }
   else
   {
      List.slideUp("fast");
      ListToggleImg.attr("src", "/images/arrow-contracted.gif");
   }
}

// Calculates the height of the description field.
function onDescriptionChange(Field, Container)
{
   // Check if the content can fit in the current size.  If not, expand it.
   if(Field.offsetHeight < Field.scrollHeight)
   {
      if(Field.offsetWidth < Field.scrollWidth)
      {
         Field.style.overflow = "auto";
         Field.style.height = Field.scrollHeight + 32 + "px";
      }
      else
      {
         Field.style.height = Field.scrollHeight + "px";
      }
   }
}

function CalculatePopupPosition(WindowPos, MousePos, WindowSize, PopUpSize)
{
   var Position = MousePos;
   if ((Position + PopUpSize) > (WindowSize))
   {
      Position = Math.max(10, (WindowPos + WindowSize - (PopUpSize + 10) ) );
   }

   return Position + WindowPos;
}

// Displays the error div.
function showErrorMessage(ErrorMessage)
{
   $("#result_title").html("Error")
                     .removeClass().addClass("error");

   $("#result_content").text(ErrorMessage);

   $("#result").removeClass().addClass("error")
               .slideDown("fast");
}

// Displays the success div.
function showSuccessMessage(SuccessMessage)
{

   $("#result_title").html("Success")
                     .removeClass().addClass("success");

   $("#result_content").text(SuccessMessage);

   $("#result").removeClass().addClass("success")
               .slideDown("fast");
}

function showVerifySubmitMessageWithCallback(Title, Message, Callback) {
   $("#result_title").html("");
   $("#result_content").html("");
   $("#result").attr("title", Title);
   $("#result").removeClass("success");
   $("#result").removeClass("error");


   $("#result_title").hide();
   $("div.result_buttons_system").hide();

   $("#result_content").text(Message);

   $("#result").dialog({
      bgiframe: true,
      width: 500,
      modal: true,
      buttons: {
         "No": function() {
            $("#result").dialog("close");
            $("div.result_buttons_system").show();
            $("#result_title").show();
            $("#result").removeAttr("title").addClass("indeterminate");
            $("#result").dialog("destroy");

            Callback(false);
         },
         "Yes": function() {
            $("#result").dialog("close");
            $("div.result_buttons_system").show();
            $("#result_title").show();
            $("#result").removeAttr("title");
            $("#result").dialog("destroy");

            Callback(true);
         }
      }
   });

   $("#result").dialog("open");
}


// Displays a message to verify the attempted submission.
function showVerifySubmitMessage(Title, Message, Form)
{
   $("#result_title").html("");
   $("#result_content").html("");
   $("#result").attr("title", Title);
   $("#result").removeClass("success");
   $("#result").removeClass("error");


   $("#result_title").hide();
   $("div.result_buttons_system").hide();

   $("#result_content").text(Message);

   $("#result").dialog({
	 bgiframe: true,
	 width: 500,
         modal: true,
         buttons: {
            "No": function()
                  {
                     $("#result").dialog("close");
                     $("div.result_buttons_system").show();
                     $("#result_title").show();
                     $("#result").removeAttr("title");
                     $("#result").dialog("destroy");
                  },
            "Yes": function()
                   {
                     $("#result").dialog("close");
                     $("div.result_buttons_system").show();
                     $("#result_title").show();
                     $("#result").removeAttr("title");
                     $("#result").dialog("destroy");

                     $(Form).submit();
                   }
         }
      });

    $("#result").dialog("open");
}

function removeUserFromSystem(UserName)
{
   var EncodedUserName = encodeURIComponent(UserName);

   $.ajax({
      url: "/users",
      data: "action=removeUser&user=" + EncodedUserName,
      async: false,
      success: function(data) {
         try {
            var Response = data;

            if(Response) {
               // Construct the status message
               var StatusMessage = "User " + UserName + " was successfully deleted.";

               // Refresh the page as the user can be in any number of roles.
               window.parent.document.location = "/settings#Page=users?status_message=" + encodeURIComponent(StatusMessage);
            }
         }
         catch (err) { }
      },
      error: function() {
         MiniLogin.show('Iguana is not Responding', function() { removeUserFromSystem(UserName); });
      }
   });

}

// Displays a message to verify the attempted submission.
function showVerifySubmitMessageJson(Title, Message, UserName)
{
   $("#result").attr("title", Title);
   $("#result").removeClass("success");
   $("#result").removeClass("error");

   $("#result_title").hide();
   $("div.result_buttons_system").hide();

   $("#result_content").text(Message);

   $("#result").dialog({
	 bgiframe: true,
	 width: 500,
         modal: true,
         buttons: {
            "No": function()
                  {
                     $("#result").dialog("close");
                     $("div.result_buttons_system").show();
                     $("#result_title").show();
                     $("#result").removeAttr("title");
                  },
            "Yes": function()
                   {
                     $("#result").dialog("close");
                     $("div.result_buttons_system").show();
                     $("#result_title").show();
                     $("#result").removeAttr("title");

                     removeUserFromSystem(UserName);
                   }
        }
    });


    $("#result").dialog("open");
}

// Displays a message to verify the attempted submission.
function showRejectSubmitMessage(Title, Message)
{
   $("#result").attr("title", Title);
   $("#result").removeClass("success");
   $("#result").removeClass("error");

   $("#result_title").hide();
   $("div.result_buttons_system").hide();

   $("#result_content").text(Message);

   $("#result").dialog({
	 bgiframe: true,
	 width: 500,
         modal: true,
         buttons: {
           "Close": function()
                  {
                     $("#result").dialog("close");
                     $("div.result_buttons_system").show();
                     $("#result_title").show();
                     $("#result").removeAttr("title");
                  }
         }
      });

   $("#result").dialog("open");
}

// Closes the result div.
function onHideResult()
{
   $("#result").slideUp("fast");
}

$(document).ready(function()
{
   $(document).on("click", "#result_close", onHideResult);
});

