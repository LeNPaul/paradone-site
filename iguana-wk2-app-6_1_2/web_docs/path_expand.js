/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

   //Only define this once per scope. Associative array holding last polled value.
if (PEXLastInput == undefined) {
   var PEXLastInput={};
}

function updatePath(Key, InputElement, PreviewSpan, PreviewDiv, EnvironmentOnly) {
   if (!InputElement) { return; }
   var UnexpandedPath = encodeURIComponent(InputElement.value);
   var PerformAjax = true;

   if(PEXLastInput[Key] != undefined && PEXLastInput[Key] == UnexpandedPath) {
      PerformAjax = false;
   }

   if (UnexpandedPath == "") {
      PerformAjax = false;
      PreviewDiv.style.display = "none";
   }

   if (PerformAjax == true) {
      PEXLastInput[Key] = UnexpandedPath;

      var AJAXCommandName;
      var AJAXVariableName;
      var PreviewPrefix;

      if (EnvironmentOnly) {
         AJAXCommandName = 'environment_expand';
         AJAXVariableName = 'RawText=';
         PreviewPrefix = 'Preview: "';
      } else {
         AJAXCommandName = 'path_expand';
         AJAXVariableName = 'RawPath=';
         PreviewPrefix = 'Absolute Path: "';
      }

      AJAXpost(AJAXCommandName, AJAXVariableName + UnexpandedPath, function(data) {
         try {
            var Response = JSON.parse(data);
            var PreviewText;
            if (Response) {
               if(EnvironmentOnly) {
                  PreviewText = Response.expandedText;
               } else {
                  PreviewText = Response.expandedPath;
               }
            }

            PreviewText = PreviewText.replace(/;/g, '&#59;');
            PreviewText = PreviewText.replace(/&(?!#59;)/g, '&amp;');
            PreviewText = PreviewText.replace(/</g, '&lt;');
            PreviewText = PreviewText.replace(/>/g, '&gt;');
            PreviewText = PreviewText.replace(/"/g, '&quot;');

            if (!Response || (PreviewText == undefined) || (PreviewText == unescape(UnexpandedPath))){
               PreviewDiv.style.display =  "none";
            } else {
               PreviewSpan.innerHTML = PreviewPrefix + PreviewText +'"';
               PreviewDiv.style.display =  "";
            }
         } catch(err) {
            //exception means that the response is not valid JSON.
            PreviewDiv.style.display =  "none";
         }
      });
   }
}

function renderExpansion(FieldId, Expansion, Original, $PreviewSpan, $PreviewDiv) {
   if (! Expansion) {
      $PreviewSpan.html("");
      $PreviewDiv.hide();
      return;
   }

   Expansion = Expansion.replace(/;/g, '&#59;');
   Expansion = Expansion.replace(/&(?!#59;)/g, '&amp;');
   Expansion = Expansion.replace(/</g, '&lt;');
   Expansion = Expansion.replace(/>/g, '&gt;');
   Expansion = Expansion.replace(/"/g, '&quot;');

   if (! Expansion || Expansion === unescape(Original) ) {
      $PreviewSpan.html("");
      $PreviewDiv.hide();
      return;
   }

   Expansion = Expansion.trim();
   $PreviewSpan.html("Preview: " + Expansion);
   $PreviewDiv.show();
}

// Slight consolidation
function expandFields(FieldIds) {
   var Count = FieldIds.length;;
   for (var i = 0; i < Count; i++) {
      var Single = FieldIds[i];
      (function(OneId) {
         var $InputField = $("#" + OneId);
         if ($InputField.length == 0) {
            return;
         }
         var $PreviewSpan = $("#" + OneId + "_preview");
         var $PreviewDiv = $("#" + OneId + "_preview_div");
         var Unexpanded = encodeURIComponent($InputField.val());

         $.ajax({
            method:  "POST",
            url:     "environment_expand",
            data:    "RawText=" + Unexpanded,
            success: function(Response){
               console.log(Response);
               renderExpansion(OneId, Response.expandedText, Unexpanded, $PreviewSpan, $PreviewDiv);
            },
            error: function() {
               $PreviewDiv.hide();
            }
         });
      })(Single);
   }
}


