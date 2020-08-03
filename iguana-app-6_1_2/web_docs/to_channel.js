/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

//inserts the ChannelId into the correct location of the 
//option list, in sorted order
function TOCHaddToSelect( Select, ChannelId )
{
   ChannelName = DestChannelIdToName[ChannelId];
   NewOption = new Option(ChannelName,ChannelId)
   InsertIndex = 0;
   while(InsertIndex < Select.options.length &&
         FRMCHnaturalCompare(Select.options[InsertIndex].text, NewOption.text) < 0)
   {
      InsertIndex++;
   }
   Select.options.add(NewOption,InsertIndex);
   Select.style.display = '';
   document.getElementById('AddDestChannelEmpty').style.display = 'none';
}

// Removes an existing row from the channels table
function TOCHhideChannelRow(ChannelId)
{
   DivName = "to_channel_"+ChannelId;
   DivToHide = document.getElementById(DivName);

   //hide the channel
   DivToHide.style.display = 'none';
   
   //disable the channel value
   Input = document.getElementById("use_to_channel_"+ChannelId)
   Input.value = 'false';
}

function TOCHremoveChannel(ChannelId)
{
   TOCHhideChannelRow(ChannelId);
   //add the Channel back to the select list
   var Select = document.getElementById("DstSelectDestChannel");
   TOCHaddToSelect(Select,ChannelId);
}

function TOCHshowChannelRow(ChannelId)
{
   DivName = "to_channel_"+ChannelId;
   DivToShow = document.getElementById(DivName);

   //show the channel
   DivToShow.style.display = '';
   
   //change the channel mutability
   Input = document.getElementById("use_to_channel_"+ChannelId)
   Input.value = 'true';
}

function TOCHinitialize(CurrentChannelName)
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
      
   var Select = document.getElementById("DstSelectDestChannel");
   //initial Select
   for (ChannelKey in DestChannelIdToName)
   {
      Found = 0;
      for (InitialIndex = 0; InitialIndex < InitialDestinationChannels.length; InitialIndex++)
      {
         if (InitialDestinationChannels[InitialIndex] == ChannelKey)
         {
            Found = 1;
            break;
         }
      }
      if (Found == 0)
      {
         TOCHaddToSelect(Select, ChannelKey );
         //hide the row with this channel
         TOCHhideChannelRow(ChannelKey);
      }
      else
      {
         //show the row
         TOCHshowChannelRow(ChannelKey);
      }
   }

   //set up display link for all rows
   for (ChannelKey in DestChannelIdToName)
   {
      LogViewLink = document.getElementById("log_view_to_channel_"+ChannelKey);
      if (LogViewLink)
      {
         LogViewLink.href = TOCHqueuedLogBrowserLink(DestChannelIdToName[ChannelKey], CurrentChannelName);
      }
   }

   // Make the drop-down menu for adding a Channel a client-side trigger
   Select.onchange = function() 
   {
      if (Select.selectedIndex < 1) return;

      var ChannelId = Select.options[Select.selectedIndex].value;

      TOCHshowChannelRow(ChannelId);

      //remove the element for that channel for the select list
      Select.removeChild( Select.options[Select.selectedIndex] ) 
      if (Select.options.length == 1)  
      {
        Select.style.display = 'none';
        document.getElementById('AddDestChannelEmpty').style.display = '';
      }   
   }
}

function TOCHfetchDestUpdates(ChannelName, ChannelGuid) {
   function htmlEscape(Text)
   {
      return String(Text).replace(/&/g, '&amp;')
                 .replace(/</g, '&lt;')
                 .replace(/>/g, '&gt;')
                 .replace(/"/g, '&quot;');
   }

   function updateQueueInfo(DestId, Date, Time, Queued)
   {
      document.getElementById('queue_date_' + DestId).innerHTML = htmlEscape(Date);
      document.getElementById('queue_time_' + DestId).innerHTML = htmlEscape(Time);
      document.getElementById('queue_remaining_' + DestId).innerHTML = htmlEscape(Queued);
   } 

   function updateDest(Dest)
   {
      try
      {
         var DestRow = document.getElementById('to_channel_'+Dest.Id);
         if (DestRow)
         {
            var Remaining = '--';
            if (Dest.TotalSent <= Dest.TotalReceived)
            {
               Remaining = '' + (Dest.TotalReceived - Dest.TotalSent);
            }
            updateQueueInfo(Dest.Id, Dest.Date, Dest.Time, Remaining);
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
            'SourceChannel'    : encodeURIComponent(ChannelName),
            'SourceChannelGuid': ChannelGuid,
            'AutomaticRequest' : 1
         },
         success: function(Info, Status, RequestObject) {
               try {
                  if( Info.LoggedIn !== undefined && !Info.LoggedIn ) {
                     MiniLogin.show('Your Iguana Session has Expired.', fetchInfo);
                  } else if( Info.ErrorDescription ) {
                     return; // Channel has been deleted. Just be quiet.
                  } else {
                     var DestChannelIdsToClear = {}
                     for(ChannelKey in DestChannelIdToName) {
                        DestChannelIdsToClear[ChannelKey] = 1;
                     }
                     var Destinations = Info.Channel.Destination;
                     for(var DestIndex in Destinations) {
                        var Dest = Destinations[DestIndex];
                        if (updateDest(Dest)) {
                           DestChannelIdsToClear[Dest.Id] = null;
                        }
                     }

                     for (ChannelKey in DestChannelIdsToClear) {
                        if (DestChannelIdsToClear[ChannelKey]) {
                           updateQueueInfo(ChannelKey, '----/--/--', '--:--', '--');
                        }
                     }
                     setTimeout(fetchInfo, 2000);
                  }
               } catch(e) {
                  // Fail silently.
               }
         },
         error: function(RequestObject, Status, Error) {
            MiniLogin.show('Iguana is not Responding', fetchInfo);
         }
      });
   }
   fetchInfo();
}

function TOCHqueuedLogBrowserLink(ChannelName, SourceChannelName)
{
  return "/logs.html?" +
        'Source=' + encodeURIComponent(ChannelName) + 
        '&Type=messages' + 
        '&DequeueSourceName=' + encodeURIComponent(SourceChannelName) +
        '&JumpToQueueEnd=true';  
}
