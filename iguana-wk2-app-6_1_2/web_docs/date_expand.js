/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

   //Only define this once per scope. Associative array holding last polled value.
if (PEXLastInput == undefined)
{  
   var PEXLastInput={};
}

function updateDate(Key, InputElement, PreviewSpan, PreviewDiv, ErrorDiv)
{        
   var UnexpandedText = encodeURIComponent(InputElement.value);
   var PerformAjax = true;
      
   if(PEXLastInput[Key] != undefined && PEXLastInput[Key] == UnexpandedText)
   {
      PerformAjax = false;
   }
    
   if (UnexpandedText == "")
   {
      PerformAjax = false;
      PreviewDiv.style.display = "none";
   }
      
   if (PerformAjax == true)
   {
         
      PEXLastInput[Key] = UnexpandedText;
        
      var AJAXCommandName;
      var AJAXVariableName;
      var PreviewPrefix;

      AJAXCommandName = 'date_expand';
      AJAXVariableName = 'RawText=';
      PreviewPrefix = 'Preview: "';
    
      $.ajax({
         url: AJAXCommandName,
         data: AJAXVariableName + UnexpandedText,
         success: function(data) {         
            try {
               var Response = data;

               var PreviewText;
            
               if(Response) {
                  PreviewText = Response.expandedText;
               }

               PreviewText = PreviewText.replace(/;/g, '&#59;');
               PreviewText = PreviewText.replace(/&(?!#59;)/g, '&amp;');
               PreviewText = PreviewText.replace(/</g, '&lt;');
               PreviewText = PreviewText.replace(/>/g, '&gt;');
               PreviewText = PreviewText.replace(/"/g, '&quot;');
    
               if (!Response || (PreviewText == undefined) || (PreviewText == unescape(UnexpandedText))) {
                  PreviewDiv.style.display =  "none";
               } else {
                  PreviewSpan.innerHTML = PreviewPrefix + PreviewText +'"';
                  PreviewDiv.style.display =  "";
               }
            } catch(err) {
               //exception means that the response is not valid JSON.
               PreviewDiv.style.display =  "none";        
            }
         }
      });
   }
}

function expandDate(Text, Preview)
{ 
   UnexpandedText = encodeURIComponent(Text);
                    
   var AJAXCommandName;
   var AJAXVariableName;
   var PreviewPrefix;

   AJAXCommandName = 'date_expand';
   AJAXVariableName = 'RawText=';
   PreviewPrefix = 'Preview: "';

   $.ajax({
      url: AJAXCommandName,
      data: AJAXVariableName + UnexpandedText, 
      success: function(data) {         
         try {
            var Response = data;
            var PreviewText = "";
            
            if (Response) {
               PreviewText = Response.expandedText;
            }

            PreviewText = PreviewText.replace(/;/g, '&#59;');
            PreviewText = PreviewText.replace(/&(?!#59;)/g, '&amp;');
            PreviewText = PreviewText.replace(/</g, '&lt;');
            PreviewText = PreviewText.replace(/>/g, '&gt;');
            PreviewText = PreviewText.replace(/"/g, '&quot;');
       
            if (Response && (PreviewText != undefined) && (PreviewText != unescape(Text))) {
               Preview.innerHTML =  PreviewPrefix + PreviewText +'"';
            } else {
               Preview.innerHTML = PreviewPrefix + Text + '"';
            }
         } catch(err) { 
            Preview.innerHTML = PreviewPrefix + Text + '"';   
         }
      },
      error: function() {}
   });
}



