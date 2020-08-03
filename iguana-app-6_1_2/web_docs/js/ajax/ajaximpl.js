/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

/*
 *** Requires jQuery. ***

 = Rational =

 This is a semi-thin wrapper on jQuery's AJAX API.  It provides a simple
 way to attach a global error handler.  jQuery provides similar function-
 ality, however jQuery's API does not facilitate the retrying of failed
 AJAX requests.  jQuery also currently treats network errors as successes,
 making things a bit strange in "success" callbacks.

 This API does not deal with errors itself, but sends them to the handler
 it is created with.  Other more concrete interfaces, such as the mini-
 login, can provide feedback and decide if AJAX operations should be
 retried (no bookkeeping required).


 = Interface =

 AjaxImpl(  m_OnError(Status,RetryThunk,Request),
 [m_OnSuccess(Data,Status,Request)] )

 Create an AJAX handler.  m_OnError() must be specified to make this more
 interesting that just using jQuery's AJAX API directly.  In particular
 this interface considers network errors to be, well, errors.

 When a request fails, m_OnError() is called with the action Status (null
 on network errors), a RetryThunk that can be called to retry the request,
 and the original raw Request (a XMLHttpRequest object).  No special
 action or bookeeping is needed to use RetryThunk, just call it and the
 rest is magic.  For details on the values of Status and Request, see the
 jQuery documentation.

 When a request succeeds, m_OnSuccess() is called, if provided.  This
 gives you the opportunity to turn a perfectly good success into a
 complete failure.  Return a non-empty string to change the Status of the
 AJAX call, and have the global error handler m_OnError() invoked instead
 of the local success callback (passed to .ajax(), for instance).  Local
 error callbacks are never called with modified events.

 The resulting object provides the following methods.


 .ajax(Config)

 Creates a jQuery.ajax() session with Config, but inserts an error
 handler that calls m_OnError() when something goes awry (not-modified
 conditions are ignored).  If you want to handle some errors (including
 not-modified), you can define Config.error yourself.  If your handler
 returns a true value, the m_OnError() will not be called.


 .abort(Request)

 Abort a pending AJAX Request as returned by .ajax(), .get(), etc.
 Unlike calling Request.abort() directly, this can be used to avoid
 triggering error handlers.


 .get       (Url, [Data], [OnSuccess], [DataType])
 .getJSON   (Url, [Data], [OnSuccess])
 .getScript (Url, [Data], [OnSuccess])
 .post      (Url, [Data], [OnSuccess], [DataType])

 Shorthand methods to AjaxImpl.ajax(), analogous to the shorthand methods
 provided by jQuery.


 .load(Target, Url, [Data], [Callback])

 Like jQuery's Target.load(Url, Data, Callback), except that errors are
 handled with m_OnError() as with AjaxImpl.ajax().  If Callback is used,
 it can return a true value to avoid passing errors to m_OnError.
 */

function AjaxImpl(m_OnError, m_OnSuccess) {
   if (!m_OnError)   m_OnError = function () {
   };
   if (!m_OnSuccess) m_OnSuccess = function () {
   };

   function abortAjax(Request) {
      if (Request) try {
         Request.onreadystatechange = function () {
         };
         Request.abort();
      } catch (Ignored) {
      }
   }

   function doAjax(Config) {
      var OnError = Config.error;
      Config.error = function (Request, Status, Error) {
         if (OnError) {
            try {
               if (OnError(Request, Status, Error)) return;
            } catch (Ignored) {}
         }
         
         if (Status === 'abort') {
            return;
         }

         if (Status != 'notmodified') {
            m_OnError(Status, function(){
               jQuery.ajax(Config);
            }, Request);
         }
      };
      
      var OnSuccess = Config.success;
      Config.success = function (Data, Status, Request) {
         if (!Request.status) {  // Network error.
            m_OnError(null, function () {
               jQuery.ajax(Config);
            }, Request);
         } else {
         
            var Failure;
            try { Failure = m_OnSuccess(Data, Status, Request);}
            catch (Ignored) {}
            
            if (Failure) {
               m_OnError(Failure, function () {
                  jQuery.ajax(Config);
               }, Request);
            } else if (OnSuccess) {
               OnSuccess(Data, Status, Request);
            }
         }
      };
      
      var Result = jQuery.ajax(Config);
      return Result;
   }

   // This code is untested.  :-[
   function doLoad(Target, Url, Data, OnComplete) {
      if (typeof(Data) == 'function') {
         OnComplete = Data;
         Data = undefined;
      }
      return Target.load(Url, Data, function (Response, Status, Request) {
         if (OnComplete) {
            try {
               if (OnComplete(Response, Status, Request)) return;
            }
            catch (Ignored) {
            }
         }
         var Failure = !Request.status || Status != 'success' && Status != 'notmodified';
         if (!Failure) {
            try {
               Failure = m_OnSuccess(Response, Status, Request) || null;
            }
            catch (Ignored) {
               Failure = null;
            }
         } else if (Request.status) {
            Failure = Status;  // Not a network error.
         }
         if (Failure || !Request.status) {
            m_OnError(Failure, function () {
               doLoad(Target, Url, Data, OnComplete);
            }, Request);
         }
      });
   }

   // For the AJAX shortcuts, like jQuery, there are optional arguments
   // that may be omitted in various combinations.  We abstract that away
   // here.  Notice that you cannot specify the DataType without either
   // the Data or Callback values; if only two arguments are present, they
   // will be assumed to be the Url and the Data.
   //
   function fixArgs(Lambda) {
      return function (Url, Data, Callback, DataType) {
         if (typeof(Data) == 'function')
            return Lambda(Url, undefined, Data, Callback);
         else if (typeof(Callback) != 'function')
            return Lambda(Url, Data, undefined, Callback);
         else
            return Lambda(Url, Data, Callback, DataType);
      }
   }

   function ajaxWithType(Type) {
      function LambdaForFixArgsToCall(Url, Data, Success, DataType) {
         var Result = doAjax({ 
            'url':      Url,
            'data':     Data,
            'type':     Type,
            'success':  Success,
            'dataType': DataType 
         });
         return Result;
      }
      var ResultAfterFixArgs = fixArgs(LambdaForFixArgsToCall);
      return ResultAfterFixArgs;
   }

   function ajaxWithDataType(DataType) {
      return fixArgs(function (Url, Data, Success) {
         return doAjax({ 'url':Url, 'data':Data,
            'success':Success, 'dataType':DataType });
      });
   }

   return {
      'ajax':  doAjax,
      'load':  doLoad,
      'abort': abortAjax,

      // AJAX shortcuts like those provided by jQuery.
      'get':       ajaxWithType(), // 'GET' is the default.
      'getJSON':   ajaxWithDataType('json'),
      'getScript': ajaxWithDataType('script'),
      'post':      ajaxWithType('POST')
   };
}
