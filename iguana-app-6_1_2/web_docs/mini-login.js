/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

/*
   ***
   *** See templates/mini-login.cs for installation and dependancies.
   ***


   MiniLogin.show(Message, OnLogin)

   Displays a modal login prompt with the given Message.  On successful
   login, OnLogin will be called.  Users may decline, "Cancel", and be
   redirected to the main login page or dashboard (if allowed).

   
   MiniLogin.loggedIn()

   Returns true if Iguana is reachable and the user is currently logged
   in; false otherwise.  If false is returned, the user will be
   asynchronously prompted to login as per usual.


   = Inherited AJAX Operations (see /js/ajax/ajaximpl.js) =

   MiniLogin.ajax(Config)

   Creates a jQuery.ajax() session with Config, but inserts an error
   handler that opens the mini-login when something goes awry (not-modified
   conditions are ignored).  If you want to handle some errors (including
   not-modified), you can define Config.error yourself.  If your handler
   returns a true value, the mini-login error-handler will assume the
   condition has been handled: the mini-login will not be shown.
   

   MiniLogin.get       (Url, [Data], [OnSuccess], [DataType])
   MiniLogin.getJSON   (Url, [Data], [OnSuccess])
   MiniLogin.getScript (Url, [Data], [OnSuccess])
   MiniLogin.post      (Url, [Data], [OnSuccess], [DataType])

   Shorthand methods to MiniLogin.ajax(), analogous to the shorthand methods
   provided by jQuery.


   MiniLogin.load(Target, Url, [Data], [Callback])

   Like jQuery's Target.load(Url, Data, Callback), except that errors are
   handled with the mini-login as with MiniLogin.ajax().  If Callback is
   used, it can return a true value to avoid having the mini-login appear.
*/

var MiniLogin = function() {
   var m_CurrentUser;
   var m_DefaultPassword;
   var m_LoginRetry;
   var m_OnLogin;
   var m_Ready;

   function login() {
      clearTimeout(m_LoginRetry);
      jQuery.ajax({
         'url': '/login_check',
         'method': 'POST',
         'data': {
            'Username': $('#login-user').val(),
            'Password': $('#login-pass').val()
         },
         'dataType': 'json',
         'cache': false,
         'success': function(Result) {
            if(!Result) {
               $('#login-message').text('Login failed: Iguana is not responding.');
            } else if(!Result.LoginOkay) {
               $('#login-message').text('Login failed: '
                  + Result.ErrorText || 'Please try again.');
            } else {
               if (Result.UnlockRequired) {
                  window.location = "/log_locker_settings.html";
                  return;
               }
               $('#login-popup').dialog('close');
               if(m_OnLogin) {
                  var OnLogin = m_OnLogin;
                  m_OnLogin = null;
                  OnLogin();
               }
            }
         },
         'error': function(Request, Status, Error) {
            if(Request.status == 403 || Status == 'parsererror') {  // Not JSON.
               $('#login-message').text('Iguana is restarting, please wait...');
               m_LoginRetry = setTimeout(login, 1000);
            } else {
               $('#login-message').text('Login failed: Iguana is not responding.');
            }
         }
      });
   }

   function showPopup(Message, OnLogin) {
      if(!m_Ready) return;

      if(m_OnLogin) {
         var OldOnLogin = m_OnLogin;
         m_OnLogin = function() {
            try { OldOnLogin(); }
            catch(Error) { }
            OnLogin();
         };
      } else {
         var Popup = $('#login-popup');
         if(!Popup.length) Popup = createPopup();
         m_OnLogin = function() {
            OnLogin();
         };
         var PasswordField = $('#login-pass').val(m_DefaultPassword);
         $('#login-message').text(Message);
         Popup.dialog('open');
         PasswordField.focus();
      }
   }

   function createPopup() {
      $('body').append('\
         <div id="login-popup" style="display:none">\
            <table width="100%">\
            <tr>\
               <td colspan="2" id="login-message" align="center">\
            <tr>\
               <td align="right">Username:\
               <td><input type="text" id="login-user" readonly class="full_length">\
            <tr>\
               <td align="right">Password:\
               <td><input id="login-pass" type="password" class="full_length">\
            </table>\
         </div>\
      ');
      $('#login-user').val(m_CurrentUser);
      $('#login-pass').keydown(function(Event) {
         if(Event.keyCode == 13) {
            Event.preventDefault();
            login();
         }
      });
      return $('#login-popup').dialog({
         'autoOpen': false,
         'closeOnEscape': false,
         'modal': true,
         'open': function(){$(".ui-dialog-titlebar-close").hide()},
         'resizable': false,
         'title': 'Iguana Login',
         'buttons': {
            'Login & Retry': login,
            'Cancel': function() {
               window.location = '/';
            }
         }
      });
   }

   function loggedIn() {
      var Result = Ajax.ajax({
         'url': '/login_check',
         'method': 'POST',
         'dataType': 'json',
         'cache': false,
         'async': false
      });
      try {
         if($.parseJSON(Result.responseText).LoginOkay) return true;
      } catch(Ignored) { }
      showPopup('Your session has expired.', $.noop);
      return false;
   };

   $(document).ready(function() {
      // We had some trouble with the mini-login popping up when switching
      // between pages.  Originally we used to delay five seconds before
      // displaying the login, but this might be a better solution if we
      // get the unload event before any AJAX requests fail.
      $(window).unload(function() { m_Ready = false; });
      m_Ready = true;
   });

   function why(Status, Request) {
      if(Request.status == 403 || Status == 'parsererror')
         return 'Your session has expired.';
      else if(Status != 'notmodified')
         return 'Iguana is not responding.';
   }

   function onAjaxError(Status, RetryThunk, Request) {
      showPopup(why(Status, Request), RetryThunk);
   }

   // We extend AjaxImpl with the regular mini-login stuff.
   var Ajax = AjaxImpl(onAjaxError);
   Ajax.show = showPopup;
   Ajax.loggedIn = loggedIn;
   Ajax.init = function(User, Pass) {
      m_CurrentUser     = User;
      m_DefaultPassword = Pass;
   };
   return Ajax;
}();
