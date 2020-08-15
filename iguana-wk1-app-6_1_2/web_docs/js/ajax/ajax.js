/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// AJAX goodies.

// To prevent Internet Explorer memory leaks, we will keep request objects
// which have been used on a stack, so that they can be reused.
var AJAXusedRequestObjectStack = new Array();
var arrayRequest = new Array();

// To prevent odd ajax errors/mini login pops, we want to clear outstanding ajax calls 
// when the page is unloaded (or in this case, beforeunload).

var userAgent = navigator.userAgent.toLowerCase()
if (userAgent.indexOf("msie") != -1) {
   /* IE specific stuff */
   window.attachEvent("onbeforeunload", AJAXbeforeUnload);
}
else if (document.implementation && document.implementation.createDocument) {
   /* Mozilla specific stuff (onbeforeunload is in v1.7+ only) */
   window.addEventListener("beforeunload", AJAXbeforeUnload, false);
}

function AJAXbeforeUnload() {
   // Clears the active request
   for (countArray = 0; countArray < arrayRequest.length; countArray++) {
      if (arrayRequest[countArray]) {
         arrayRequest[countArray].onreadystatechange = function () {
         };
         arrayRequest[countArray].abort();
      }
   }

   // I think the above abort should be enough, but if we need to abort all the cached request objects
   // we could enable the code below which should do the trick. Art-Dec.29th/20008
   //
   // Actually, the above code now handles multiple outstanding request.  The code below should be deleted
   // after the the changes for #10121 have been verified.  Art-Jan.6th/2009

   // for (x=0;x<AJAXusedRequestObjectStack.length;x++ )
   // {
   //    var unloadRequest = AJAXusedRequestObjectStack.pop();
   //    unloadRequest.onreadystatechange = function() {};
   //    unloadRequest.abort();      
   // }   
}

function AJAXnewHttpRequest() {
   if (AJAXusedRequestObjectStack.length == 0) {
      if (window.XMLHttpRequest)  // Firefox, Safari, etc.
      {
         return new XMLHttpRequest();
      }
      else // Internet Explorer (with ActiveX enabled).
      {
         return new ActiveXObject('Microsoft.XMLHTTP');
      }
   }
   else {
      return AJAXusedRequestObjectStack.pop();
   }
}

function AJAXremoveFromArrayRequest(Request) {
   if (Request) {
      var ArrayIndex = 0;
      while (ArrayIndex < arrayRequest.length) {
         if (arrayRequest[ArrayIndex] == Request) {
            arrayRequest.splice(ArrayIndex, 1);
         }
         else {
            ArrayIndex++;
         }
      }
   }
}

function AJAXhandleResponse(Request, OnSuccess, OnError) {
   try {
      if (Request.readyState == 4) {
         try {
            // We tag login issues with 403, so we have to let them through.
            if (Request.status == 200 || Request.status == 403) {
               OnSuccess(Request.responseText, Request.getResponseHeader('Content-Type'), Request);
            }
            else if (OnError) {
               var ErrorMessage = 'The response from the Iguana server could not be read.';

               if ((Request.status) && (Request.status != 0)) {
                  ErrorMessage += '<br />Status: ' + Request.status;

                  if ((Request.statusText) && (Request.statusText != '')) {
                     ErrorMessage += ' - ' + Request.statusText;
                  }
               }

               OnError(ErrorMessage, Request);
            }
         }
         catch (e) {
            // Don't care.
         }

         Request.onreadystatechange = function () {
         };
         AJAXusedRequestObjectStack.push(Request);
         AJAXremoveFromArrayRequest(Request);
      }
   }
   catch (Error) {
      if (OnError && Request) {
         try {
            var Description = Error.description || Error;
            OnError('The response from the Iguana server could not be read.<br />' + Description, Request);
         }
         catch (e) {
            // Don't care.
         }

         Request.onreadystatechange = function () {
         };
         AJAXusedRequestObjectStack.push(Request);
         AJAXremoveFromArrayRequest(Request);
      }
   }
}

/* #10511 - We don't use GET any more due to previous issues. */
function AJAXpost(Location, Parameters, OnSuccess, OnError) {
   try {
      var NewRequest = AJAXnewHttpRequest();
      arrayRequest[arrayRequest.length] = NewRequest;
      NewRequest.open('POST', Location, true);
      NewRequest.onreadystatechange = function () {
         AJAXhandleResponse(NewRequest, OnSuccess, OnError);
      };
      NewRequest.send(Parameters);
   }
   catch (Error) {
      if (OnError) {
         OnError('AJAX is not supported by the browser.');
      }
   }
}

/* #10511 - POST only now. */
function AJAXsynchronousPost(Location, Parameters, OnSuccess, OnError) {
   try {
      var NewRequest = AJAXnewHttpRequest();
      arrayRequest[arrayRequest.length] = NewRequest;
      NewRequest.open('POST', Location, false);
      NewRequest.send(Parameters);
      AJAXhandleResponse(NewRequest, OnSuccess, OnError);
   }
   catch (Error) {
      if (OnError) {
         OnError('AJAX is not supported by the browser.');
      }
   }
}

/* 
 * #14327 - AJAXpostWithId() is a special AJAX request which will only allow one
 * request at a time with the same ID.  If there already exists a pending
 * request with the ID RequestId, the existing request will be aborted.
 * Any pending request with the specified ID may also be aborted explicitly
 * with AJAXabortRequestById();
 */

var AJAXrequestsWithIds = {};

function AJAXabortRequest(Request) {
   Request.onreadystatechange = function () {
   };
   Request.abort();
   AJAXusedRequestObjectStack.push(Request);
   AJAXremoveFromArrayRequest(Request);
}

function AJAXclearRequestId(RequestId) {
   if (AJAXrequestsWithIds[RequestId]) {
      delete AJAXrequestsWithIds[RequestId];
   }
}

function AJAXabortRequestById(RequestId) {
   if (AJAXrequestsWithIds[RequestId]) {
      AJAXabortRequest(AJAXrequestsWithIds[RequestId]);
      AJAXclearRequestId(RequestId);
   }
}

function AJAXpostWithId(Location, Parameters, RequestId, OnSuccess, OnError) {
   try {
      AJAXabortRequestById(RequestId);

      var NewRequest = AJAXnewHttpRequest();
      arrayRequest[arrayRequest.length] = NewRequest;
      NewRequest.open('POST', Location, true);
      NewRequest.onreadystatechange = function () {
         // Make sure to do this before AJAXhandleResponse, which could call
         // the OnSuccess handler (which could call AJAXpostWithId again).
         if (NewRequest.readyState == 4) {
            AJAXclearRequestId(RequestId);
         }

         AJAXhandleResponse(NewRequest, OnSuccess, OnError);
      };
      NewRequest.send(Parameters);

      AJAXrequestsWithIds[RequestId] = NewRequest;
   }
   catch (Error) {
      AJAXclearRequestId(RequestId);
      if (OnError) {
         OnError('AJAX is not supported by the browser.');
      }
   }
}

/* End AJAXpostWithId() and helper functions */

if (!window.JSON) {
   // Have a older browser which doesn't support native JSON parsing so we create it - see #15028
   window.JSON = {};
   window.JSON.parse = function (Data) {
      return eval('(' + Data + ')');
   }
}

