/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// This might be useful elsewhere.  It is essentially the same
// as DWBnaturalCompareStrings() (in DWButils.cpp), as it compares
// two strings ignoring case, and recognizes and compares
// embedded natural numbers (e.g., 'junk1', 'junk2', 'junk10').
//
function FRMCHnaturalCompare(iLeft, iRight)
{
   var Left  =  iLeft.toLowerCase();
   var Right = iRight.toLowerCase();

   while( Left && Right )
   {
      if( /^\d/.test(Left) && /^\d/.test(Right) )
      {
          LeftNumber = 1* Left.match(/^\d+/);  Left  =  Left.replace(/^\d+/,'');
         RightNumber = 1*Right.match(/^\d+/);  Right = Right.replace(/^\d+/,'');

         if( LeftNumber != RightNumber )
         {
            return LeftNumber - RightNumber;
         }
      }
      else
      {
          LeftCharacter =  Left.charCodeAt(0);  Left  =  Left.substr(1);
         RightCharacter = Right.charCodeAt(0);  Right = Right.substr(1);

         if( LeftCharacter != RightCharacter )
         {
            return LeftCharacter - RightCharacter;
         }
      }
   }

   return Left < Right ? -1 : (Left == Right ? 0 : +1);
}

//inserts the ChannelId into the correct location of the 
//option list, in sorted order
function FRMCHaddToSelect( Select, ChannelId )
{
   ChannelName = SourceChannelIdToName[ChannelId];
   NewOption = new Option(ChannelName,ChannelId)

   InsertIndex = 0;
   while(InsertIndex < Select.options.length &&
         FRMCHnaturalCompare(Select.options[InsertIndex].text, NewOption.text) < 0)
   {
      InsertIndex++;
   }
   Select.options.add(NewOption,InsertIndex);
   Select.style.display = '';
   document.getElementById('AddSourceChannelEmpty').style.display = 'none';
}

// Removes an existing row from the channels table
function FRMCHhideChannelRow(ChannelId)
{
   DivName = "from_channel_"+ChannelId;
   DivToHide = document.getElementById(DivName);

   //hide the channel
   DivToHide.style.display = 'none';
   
   //disable the channel value
   Input = document.getElementById("use_channel_"+ChannelId)
   Input.value = 'false';
}

function FRMCHremoveChannel(ChannelId)
{
   FRMCHhideChannelRow(ChannelId);
   //add the Channel back to the select list
   var Select = document.getElementById("SrcSelectSourceChannel");
   FRMCHaddToSelect(Select,ChannelId);
}

function FRMCHshowChannelRow(ChannelId)
{
   DivName = "from_channel_"+ChannelId;
   DivToShow = document.getElementById(DivName);

   //show the channel
   DivToShow.style.display = '';
   
   //change the channel mutability
   Input = document.getElementById("use_channel_"+ChannelId)
   Input.value = 'true';
}

function FRMCHinitialize(CurrentChannelName)
{
   // Some browsers don't support dynamic element creation
   // so we throw.
   try 
   {
      var Test = document.createElement("input");
      Test.type = "button";
      if (Test.type != "button") throw Error();
   } 
   catch (e) 
   {
      return;
   }
   
   
   var Select = document.getElementById("SrcSelectSourceChannel");
   //initial Select
   for (ChannelKey in SourceChannelIdToName)
   {
      Found = 0;
      for (InitialIndex = 0; InitialIndex < InitialSourceChannels.length; InitialIndex++)
      {
         if (InitialSourceChannels[InitialIndex] == ChannelKey)
         {
            Found = 1;
            break;
         }
      }
      if (Found == 0)
      {
         FRMCHaddToSelect(Select, ChannelKey );
         //hide the row with this channel
         FRMCHhideChannelRow(ChannelKey);
      }
      else
      {
         //show the row
         FRMCHshowChannelRow(ChannelKey);
      }
   }

   //set up display link for all rows
   for (ChannelKey in SourceChannelIdToName)
   {
      LogViewLink = document.getElementById("log_view_from_channel_"+ChannelKey);
      if (LogViewLink)
      {
         LogViewLink.href = FRMCHqueuedLogBrowserLink(CurrentChannelName, SourceChannelIdToName[ChannelKey]);
      }
   }

   // Make the drop-down menu for adding a Channel a client-side trigger
   Select.onchange = function() 
   {
      if (Select.selectedIndex < 1) return;

      var ChannelId = Select.options[Select.selectedIndex].value;

      FRMCHshowChannelRow(ChannelId);

      //remove the element for that channel for the select list
      Select.removeChild( Select.options[Select.selectedIndex] ) 
      if (Select.options.length == 1)  
      {
	Select.style.display = 'none';
        document.getElementById('AddSourceChannelEmpty').style.display = '';
      }   
   }
}

function FRMCHfetchSourceUpdates(ChannelName, ChannelGuid)
{
   function htmlEscape(Text)
   {
      return String(Text).replace(/&/g, '&amp;')
                 .replace(/</g, '&lt;')
                 .replace(/>/g, '&gt;')
                 .replace(/"/g, '&quot;');
   }

   function updateQueueInfo(SourceId, Date, Time, Queued)
   {
      document.getElementById('queue_date_' + SourceId).innerHTML = htmlEscape(Date);
      document.getElementById('queue_time_' + SourceId).innerHTML = htmlEscape(Time);
      document.getElementById('queue_remaining_' + SourceId).innerHTML = htmlEscape(Queued);
   } 

   function updateSource(Source)
   {
      try
      {
         var SourceRow = document.getElementById('from_channel_'+Source.Id);
         if (SourceRow)
         {
            var Remaining = '--';
            if (Source.TotalSent <= Source.TotalReceived)
            {
               Remaining = '' + (Source.TotalReceived - Source.TotalSent);
            }
            updateQueueInfo(Source.Id, Source.Date, Source.Time, Remaining);
            return true;
         }
      }
      catch(e)
      {
         window.location.search = '?Channel=' + ChannelName
                                + '&editTab=' + CurrentTab.id;
      }
      return false;
   }

   function fetchInfo() {
      $.ajax({
         url   : '/queue_progress',
         data  : {
            'Channel'    : encodeURIComponent(ChannelName),
            'ChannelGuid': ChannelGuid,
            'AutomaticRequest' : 1
         },
         success: function(Info, Status, RequestObject) {
               try {
                  if( Info.LoggedIn !== undefined && !Info.LoggedIn ) {
                     MiniLogin.show('Your Iguana Session has Expired.', fetchInfo);
                  } else if( Info.ErrorDescription ) {
                     return; // Channel has been deleted. Just be quiet.
                  } else {
                     var ChannelIdsToClear = {}
                     for(ChannelKey in SourceChannelIdToName) {
                        ChannelIdsToClear[ChannelKey] = 1;
                     }
                     {
                        var Sources = Info.Channel.Source;
                        for(var SourceIndex in Sources) {
                           var Source = Sources[SourceIndex];
                           if (updateSource(Source)) {
                              ChannelIdsToClear[Source.Id] = null;
                           }
                        }
                     }
                     for (ChannelKey in ChannelIdsToClear) {
                        if (ChannelIdsToClear[ChannelKey]) {
                           updateQueueInfo(ChannelKey, '----/--/--', '--:--', '--');
                        }
                     }
                     setTimeout(fetchInfo, 2000);
                  }
               } catch(e) {
                  console.log(e);
                  // Fail silently.
               }
         },
         error: function(RequestObject, Status, Error) {
            MiniLogin.show('Iguana is not responding', fetchInfo);
         }
      });
   }
   fetchInfo();
}

function FRMCHqueuedLogBrowserLink(ChannelName, SourceChannelName)
{
  return "/logs.html?" +
        'Source=' + encodeURIComponent(ChannelName) + 
        '&Type=messages' + 
        '&DequeueSourceName=' + encodeURIComponent(SourceChannelName) +
        '&JumpToQueueEnd=true';  
}
