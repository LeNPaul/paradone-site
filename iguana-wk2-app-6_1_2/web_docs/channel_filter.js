/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

//Only define this once per scope. Associative array holding last polled value.
if (CFLastInput == undefined)
{  
   var CFLastInput={};
}

function filterChannels(Key, ChannelText, FilterText, SelectElement,SelectCounter)
{
   filter(Key, ChannelText, FilterText, SelectElement, SelectCounter, "channel_filter");
}

// Not currently used.
function filterChannelGroups(Key, ChannelText, FilterText, SelectElement,SelectCounter)
{
   filter(Key, ChannelText, FilterText, SelectElement, SelectCounter, "channel_group_filter");
}

function filter(Key, ChannelText, FilterText, SelectElement, SelectCounter, AjaxName)
{
   var PerformAjax = true;
      
   if(CFLastInput[Key] == undefined) {
      CFLastInput[Key] = {};
   }

   var Options = SelectElement.getElementsByTagName("DIV");
 
   if (PerformAjax == true) {        
      CFLastInput[Key].channel = ChannelText;
      CFLastInput[Key].filter = FilterText;
      $.ajax({
         url: AjaxName, 
         data: "DelimitedList=" + ChannelText + "&Regex=" + FilterText,
         success: function(Response, ContentType){
            try { 
               if(Response) {
                  Response.Matched = Response.Matched || {};
                  Response.Unmatched = Response.Unmatched || {};
                  var VisibleCount = 0;
                  
                  for ( var OptionsIndex = Options.length -1; OptionsIndex >= 0 ; --OptionsIndex ) {
                     if (Response.Matched[Options[OptionsIndex].id] != undefined ) {
                        ++VisibleCount;
                        Options[OptionsIndex].style.display = '';          
                        Options[OptionsIndex].innerHTML = Response.Matched[Options[OptionsIndex].id];
                     } else if(Response.Unmatched[Options[OptionsIndex].id] != undefined ) {
                        Options[OptionsIndex].style.display = 'none';
                        //TODO: change this from .selected.
                        Options[OptionsIndex].selected = false;
                        Options[OptionsIndex].innerHTML = (Response.Unmatched[Options[OptionsIndex].id]);
                     } else {
                        SelectElement.removeChild(Options[OptionsIndex]);
                     }
                  }
                  
                  if (VisibleCount == Options.length) {
                     SelectCounter.innerHTML = "<b>All</b> channels displayed";
                  } else {
                     SelectCounter.innerHTML = "<b>" + VisibleCount + "</b> of <b>" + Options.length + "</b> channels displayed";
                  }
               } else {
                  //Result is empty. Hide everything.
                  for ( var OptionsIndex = Options.length -1; OptionsIndex >= 0 ; --OptionsIndex ) {
                     Options[OptionsIndex].style.display = 'none';
                     Options[OptionsIndex].selected = false;         
                  }
                  SelectCounter.innerHTML = "<b>0</b> of <b>" + Options.length + "</b> channels displayed";
                }
            } catch(err) {
               console.log('Error: ' + err);
            }
         },
         error: function(Error) {
            if(MiniLogin) {
               console.log('Error: ' + Error);
            }
         }
      });
   }
}

