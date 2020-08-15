/** @license
 * Copyright (c) 2010-2015 iNTERFACEWARE Inc.  All rights reserved.
 */

function initIfwareSettings() {
   var OriginalCI = clearInterval;
   clearInterval = function(ID) {
      console.log("Clearing interval " + ID);
      OriginalCI(ID);
   }

   $("#main").html("");
   ifware = window.ifware || {};
   ifware.Settings = ifware.Settings || {};
   ifware.SettingsScreen = new SettingsScreen_Class(document.location.hash);
   ifware.SettingsScreen.init();
}

/**
 * @constructor
 */
// Adding the _Class just to make sure we avoid confusion/clashing. It was
// conflicting with the name of the object after refactoring WebConfig to be
// general SettingsScreen helpers.
var SettingsScreen_Class = function(Hash) {
   console.log("Constructing");
   var m_Location  = Hash;
   var m_DefaultPage = "/settings_menu";
   var m_Page        = m_DefaultPage;
   var m_Me          = this;

   function load() {
      console.log(m_Page);
      $.ajax({
         dataType: "json",
         url:      m_Page,
         success:  function(Response, TextStatus, RequestObject) {
            m_Me.display(Response, TextStatus, RequestObject);
            window.setTimeout(function() {
            }, 100);
         },
         error:    m_Me.handleError
      });
   }

   function buildTitle(Response) {
      var ServerLabel = Response.server_label
                        ? ' (' + Response.server_label + ')'
                        : '';
      var Title = Response.title 
                  ? ' &gt; ' + Response.title
                  : '';
      return "Iguana" + ServerLabel + Title;
   }

   function displayError(RequestObject) {
      m_Me.display(RequestObject.responseJSON);
      var ErrorMessage = RequestObject.responseJSON.error
                       ? RequestObject.responseJSON.error.description
                       : RequestObject.status + " - " + RequestObject.statusText;
      $('#scResponseDlg').remove();
      if (RequestObject.responseJSON && RequestObject.responseJSON.error.page) {
         // The handler has printed an on-screen error message, so no popup and no redirect
         return;
      }
      $("body").append($(  "<div id='scResponseDlg'>"
                        +  "  <div class='sc_error'></div><br/>"
                        +  "  <div class='sc_output'></div>"
                        +  "  <div class='sc_info'></div>"
                        +  "</div>"));

      var ResponsePopup = $("#scResponseDlg");
      var DlgButtons = {};

      DlgButtons['Close'] = function() {
         $(this).dialog('close');
      }

      ResponsePopup.dialog({
         bgiframe: true,
         width: 600,
         title: "Error",
         modal: true,
         autoOpen: false,
         buttons: DlgButtons,
         close: function(event, ui) {
            document.location = "/";
         }
      });

      ResponsePopup.find('.sc_error').text(ErrorMessage);
      ResponsePopup.find('.sc_error').show();
      ResponsePopup.dialog("open");
   }

   function route() {
      console.log("Routing");
      if (! m_Location) {
         if (document.location.pathname == "/channel") {
            m_Page = '/channel/add';
         } else {
            m_Page = m_DefaultPage;
         }
         load();
         return;
      }
      var Parts = m_Location.split('#');
      console.log(Parts);
      if (Parts.length < 2) {
         m_Page = m_DefaultPage;
         load();
         return;
      }
      if (document.location.pathname == "/channel") {
         console.log(Parts);
         Parts.shift();
         m_Page = "/channel/control?" + Parts.join(encodeURIComponent('#'));
         console.log(m_Page);
         load();
         return;
      } 
      var QueryString = Parts[1].split('=');
      var Start = QueryString.shift();
      if (Start == "Page") {
         m_Page = "/" + QueryString.join('=');
      } else {
         m_Page = m_DefaultPage;
      }
      load();
    }

   function buildParams(Qstring) {
      var Parts = Qstring.split('=');
      var Params = {};
      for (var i = 0; i < Parts.length; i = i + 2) {
         Params[Parts[i]] = Parts[i+1]; 
      }
      return Params;
   }

   var m_ChangingHash = 0;

   function cutUrl(Url) {
      var Cut = {};
      var Parts = Url.split("#");
      Cut.Base = Parts[0];
      var Queries = [];
      if (Parts[1]) {
         Queries = Parts[1].split("&");
      }
      if (Queries.length > 0) {
         Cut.Queries = {};
         var Count = Queries.length;
         Cut.Qcount = Count;
         for (i = 0; i < Count; i++) {
            var Kv = Queries[i].split("=");
            Cut.Queries[Kv[0]] = Kv[1];
         }
      }
      return Cut;
   }

   function checkForHashChange(event) {
      console.log(event);
      console.log(m_ChangingHash);
      m_Location = document.location.hash;
      if (event && event.oldURL && event.newURL) {      
         var Old = cutUrl(event.oldURL);
         var New = cutUrl(event.newURL);
         if (Old.Queries && New.Queries && Old.Queries.Channel && New.Queries.Channel) {
            if (Old.Queries.Channel != New.Queries.Channel) {
               m_ChangingHash = 0;
            }
         }
      } 
      if (m_ChangingHash > 0) {
         m_ChangingHash--;
         return;
      }
      route();
   }

   // BEGIN: Public API
   this.init = function() {
      $("#fileBrowser ul.jQueryFileTree li a").click(function(event) {
         event.stopPropagation();
      });
      route();
      window.onhashchange = checkForHashChange;
   }
   this.clearHashTally = function() {
      m_ChangingHash = 0;
   }
   this.trimUrl = function() {
      if (document.location.pathname != '/channel') {
         return;
      }
      var RegEx = /&Tab=newChannel$/;
      if (document.location.hash.match(RegEx)) {
         document.location.hash = document.location.hash.replace(RegEx, '&Tab=channel');
         checkForHashChange();
         return;
      }
      m_ChangingHash = 2;
      document.location.hash = document.location.hash.replace(/&Tab=.*$/, '');
   }
   this.display = function(Response, TextStatus, RequestObject) {
      console.log("Running display");
      if ( ! Response) {
         console.log("This shouldn't happen, yet it does. Try to find out why.");
      }
      if (Response.location && Response.location != document.location.pathname + document.location.hash) {
         document.location = Response.location;
         m_Page = Response.location;
      }

      if (Response.body) {
         $("#main").html(Response.body);
         if (Response.title) {
            $("title").html(buildTitle(Response));
         }
      }
      window.setTimeout(function() {
         m_Me.trimUrl();
      }, 10);

   }
   this.handleError = function(RequestObject, Status, ErrorString) {
      console.log("Handling error");
      console.log(RequestObject);
      switch(RequestObject.status) {
         case 401:
            document.location = "/login.html?RedirectLocation=/settings?Page=" + m_Page.substring(1);
            break;
         case 400: 
            displayError(RequestObject);
            break;
         case 402:
         case 403:
         case 404:
            document.location = "/settings";
            break;
         case 500:
            displayError(RequestObject);
            break;
      }
   }
   this.cancel = function(Target) {
      VALfieldValidationFunctions = [];
      if (Target) {
         m_Page = Target;
      }
      load();
   }
   this.page = function() {
      return m_Page;
   }
   // END: Public API
}


