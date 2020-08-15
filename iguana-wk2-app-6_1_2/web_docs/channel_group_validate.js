/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

function sourceExists(Type, Name, Callback)
{
   var EncodedName = encodeURIComponent(Name);
   
   if (Type == "Channel" || Type == "Group") {
      $.ajax({
         url: "channel_group_exists",
         async: false,
         data: "Type=" + Type + "&Name=" + EncodedName,
         success: function(Response, contentType){
            try {
               if (Response) {
                  Callback(Response.Results.DoesExist, Type, Name);
               }
            } catch(Error) {
               console.log('Error: ' + Error);
            }
         },
         error: function(Error) {
            console.log('Error: ' + Error);
         }
      });
   } else {
      Callback(true, Type, Name);
   }
}

