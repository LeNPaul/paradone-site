/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

var BrowserDetails = navigator.userAgent;   
var WrapChar = "";
if(BrowserDetails.indexOf("MSIE 6") != -1 || BrowserDetails.indexOf("MSIE 5") != -1)
{
   WrapChar = "<wbr />";
}
else
{
   WrapChar = "&#8203;";
} 

function doCommit(FileName)
{
   var Comment = document.getElementById("comment").value;

   $("#commit_popup").dialog("option", "buttons", {} );
   
   $("#commit_comment").css("display", "none");
   $("#commit_status").css("display", "");

   $("#commit_status").html(Result);
      
   var AJAXCommandName = "commit_configuration";

   var AJAXFileNameVariable = "FileName=" + FileName;
   var AJAXCommentVariable = "Comment=" + Comment;

   AJAXpost(AJAXCommandName, AJAXCommentVariable + "&" + AJAXFileNameVariable, 
      function(data)
      {
         setTimeout("checkForCommitResult()", 500);             
      },
   
      function(data) {}
   );   
}

// AJAX function that checks for commit errors for the last commit operations.
function checkForCommitResult()
{  
   var AJAXCommandName = "check_configuration_commit";
                 	 
   AJAXpost(AJAXCommandName, "", 
      function(data)
      { 
         var Response = eval(data);
  
         var Results = Response.OperationOutput;

         // Filter the output so it doesn't show the "?" lines.
         Results = Results.replace(/^\?[/\\.]*\r?\n|\r?\n\?[/\\.]*\r?\n/, "");

         // Format it for display in a div.
         Results = Results.replace(/\r\n/g, "<br>");

         if("" != Results)
         {
            var ResultsPerOperation = Results.split("###");

            if(0 < ResultsPerOperation.length)
            {
               for(var i = 0; i < ResultsPerOperation.length; ++i)
               {
                  var StdResults = ResultsPerOperation[i].split("@@@");
               
                  if(2 == StdResults.length)
                  { 
                     if("" != StdResults[0])
                     {
                        document.getElementById("result").innerHTML = document.getElementById("result").innerHTML + StdResults[0] + "<br>";
                     }
    
                     if("" != StdResults[1])
                     {
                        document.getElementById("result").innerHTML = document.getElementById("result").innerHTML + "<br><div style='color:#FF0000'>" + StdResults[1] + "</div><br>";
                     }
                  }
                  else
                  {
                     document.getElementById("result").innerHTML = document.getElementById("result").innerHTML + StdResults + "<br>";
                  }
               }
            }
            else
            {
               document.getElementById("result").innerHTML = document.getElementById("result").innerHTML + ResultsPerOperation + "<br>";
            }

            document.getElementById("result").innerHTML = document.getElementById("result").innerHTML + "<br>";
         }
      
         var IsDetermined = Response.IsDetermined;
            
         if(IsDetermined)
         {      
            $("#commit_popup").dialog("option", "buttons", { "Close": function() {    $("#commit_status").css("display", "none"); $("#commit_popup").dialog("close"); location.reload(true); } } );
         }
         else
         {
            setTimeout("checkForCommitResult()", 500); 
         }
      },
      
      function(data) {}
   );
}

var Comment = "<textarea id='comment' style='text-align:left;width:472px;height:300px;overflow:auto;padding:2px;'></textarea>";
var Result = "<div id='result' style='border:1px solid #A0A0A0;background-color:white;text-align:left;width:472px;height:300px;overflow:auto;padding:2px;'></div>"

function showCommitPopup(FileName, User, Date, Extension)
{
   $("#commit_comment").css("display", ""); 
   $("#commit_comment").html("<div>" + Comment + "</div>");   
   $("#comment").val("\n\n\nChanged By: " + User + "\n\nDate: " + Date);
   
   var DateTime = Date.replace(/\//g, "_").replace(/:/g, "_").replace(/ /g, "__");

   $("#commit_popup").dialog({
	 bgiframe: true,
	 width: 500,
         modal: true,
         buttons: {
            "Commit": function() { doCommit(FileName + "." + DateTime + "." + User + "." + Extension); },  
            "Close": function() { $("#commit_popup").dialog("close"); }
         }
      });

   $("#commit_popup").dialog("open");
}
      
document.write("<div id='commit_popup' style='text-align:center;width:450px;overflow:hidden;' title='Commit'><div id='commit_comment' style='text-align:center;display:none;'></div><div id='commit_status' style='text-align:center;display:none;'></div></div>");
