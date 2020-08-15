/** @license
 * Copyright (c) 2010-2016 iNTERFACEWARE Inc.  All rights reserved.
 */


/**
 * @constructor
 */
var ImportManager = function() {

   //Private Members
   var m_Me = this;
   var s_DemoNoReminderKey = "DemoNoReminder";

   var m_Page = {
      HasPrompted     : false,
      UserInitiated   : false,
      DiffFormat      : "vertical",
      Popup           : '',
      IndexMap        : {},
      Diffs           : {},
      StatusInterval  : 0,
      Task            : '',
      Initialized     : false, /* Not used? */
      
      Cred            : {},
      CurrentRepo     : '',
      CurrentStatus   : '', /* Not used? */
      CommitMessage   : null,
      CurrentRemote   : {},
      IsIfwareRemote  : false,
      CurrentRequest  : {},

      CurrentChannelList       : [],
      AllVersionedChannelsList : [],
      CurrentChannelVersion    : "",
      
      StatusMap           : {},
      CollationMap        : {},
      CurrentFile         : '',
      CurrentIndex        : 0,
      IxportableList      : [],
      FileCollisionMap    : {},
      LocalGuidsToNameMap : {},
      ChannelCollisionMap : {},
      NameOnlyCollisionsPresent : false
   }



   // BEGIN: HTTP API calls
   function attemptImportOrExport() {
      m_Page.Popup = $(m_Page.Popup);
      console.log("Attempting remote action...");
      console.log();
      var Params = {"sc_task": taskKey() + '_channels'};

      if (m_Page.Cred.auth_user) {
         Params.auth_user     = m_Page.Cred.auth_user;
         Params.auth_password = m_Page.Cred.auth_password;
      }

      var CollisonConfirmationsPresent = Object.keys(m_Page.ChannelCollisionMap).length > 0;
      console.log(m_Page.ChannelCollisionMap);
      if(CollisonConfirmationsPresent){
         var ChannelSelectBoxes = $("#collision_list .CollisionChannel select.dropdown");
         SCMclearNewChannelNameWarnings(ChannelSelectBoxes);
         var ValidationFailed = SCMvalidateAndStoreNewChannelNames(m_Page, ChannelSelectBoxes, taskKey()); 
         if(ValidationFailed){
            return false;
         }
         Params.channel_collision_confirmations = JSON.stringify(m_Page.ChannelCollisionMap);
      }
      if (Object.keys(m_Page.FileCollisionMap).length > 0) {
         Params.overwrite_shared_file_confirmations = JSON.stringify(m_Page.FileCollisionMap);
      }
      if(! m_Page.CommitMessage){
         console.log("Commit message is null!");
         initCommitMessagePopup();
         return;
      }
      Params.commit_message = m_Page.CommitMessage;
      console.log(Params);
      console.log(m_Page.Task);
      console.log(m_Page.Popup);
      var Title = '';
      var PopupMessage = '';
      switch (m_Page.Task) {
         case '/import_channels':
            Title = "Import Channels";
            Params.remote_name         = m_Page.CurrentRepo;
            if (m_Page.CurrentRemote.url) {
               Params.remote_location  = m_Page.CurrentRemote.url;
            }
            if (m_Page.IsIfwareRemote) {
               Params.is_ifware_remote = true;
            }
            Params.channels_version = m_Page.CurrentChannelVersion;
            Params.channel_import_list = JSON.stringify(buildIxportList());
            PopupMessage = "Importing Channels...";
            break;
         case '/export_channels':
            Title = "Export Channels";
            Params.remote_name         = m_Page.CurrentRepo;
            Params.channel_export_list = JSON.stringify(buildIxportList());
            PopupMessage = "Exporting Channels...";
            break;
         default:
            m_Page.Popup.dialog("close");
            return true;
      }
      var ErrorHandler = function(RequestObject, Status, ErrorString){
         m_Page.Popup.dialog("close");
         handleImportExportError(RequestObject, Status, ErrorString, Params);
      }
      m_Page.Popup.dialog(SCM_POPUP_PARAMS);
      m_Page.Popup.dialog("option", "buttons", {});
      m_Page.Popup.dialog("option", "title", Title);
      m_Page.Popup.html("<p>"+ PopupMessage + " <img src='jqueryFileTree/images/spinner.gif'> </p>");
      if (! m_Page.Popup.dialog("isOpen")) {
         m_Page.Popup.dialog("open");
      }
      console.log("Params : ");
      console.log(Params);
      removeWarning();
      $.ajax({
         url    : m_Page.Task,
         method : "POST",
         data   : Params,
         success: handleResponse,
         error  : ErrorHandler
      });
      return true;
   }

   function getCollatedProjects() {
      //Only need to collate with other projects for importing.
      var ErrorHandler = function(RequestObject, Status, ErrorString){
         m_Page.Popup.dialog("close");
         handleImportExportError(RequestObject, Status, ErrorString, Params);
      }
      $.ajax({
         url: '/sc/collate',
         method: 'POST',
         data: {
            file_paths: JSON.stringify(m_Page.FileCollisionMap)
         },
         success: function(Response) {
            console.log(Response);
            m_Page.CollationMap = Response;
            SCMloadCollidingFileDiffViewData(m_Page, taskKey());
            SCMattachDisableActionsToDiffView();
            SCMdoFileDiff(m_Page);
            SCMfixDiffView();
            SCMfixCollidingSharedFileView();
         },
         error: ErrorHandler
      });
   }

   function showRemoteChannels(Vars) {
      console.log(Vars);
      removeChannelVersionSelector();
      removeNoChannelSelectionWarning();
      $("#import_export_list").html("");
      
      var PostData = {};
      if (Vars) {
         PostData = Vars;
      } 
      else if (m_Page.CurrentRemote.url) {
         PostData = {"remote_name": m_Page.CurrentRemote.name, "remote_location": m_Page.CurrentRemote.url};
      } 
      else {
         PostData = {"remote_name": m_Page.CurrentRepo};
      }

      if (m_Page.IsIfwareRemote) {
         console.log("The selected repo is one of iNTERFACEWARE's own github ones.");
         PostData.is_ifware_remote = true;
      }
      else if (m_Page.CurrentRepo) {
         console.log("This is a user defined repo.");
      }
      else {
         console.log("No repo selected...");
      }

      console.log(PostData);
      if(PostData["remote_name"] == ""){
        return; //empty placeholder selected on dropdown.
      }

      repoWait();
      
      if (m_Page.Cred.auth_user) {
         PostData.auth_user     = m_Page.Cred.auth_user;
         PostData.auth_password = m_Page.Cred.auth_password;
      }
      
      var ErrorHandler = function(RequestObject, Status, ErrorString){
         handleChannelListError(RequestObject, Status, ErrorString, PostData);
      }

      m_Page.CurrentRequest = $.ajax({
         url    : '/sc/view_remote_channels',
         method : "POST",
         data   : PostData,
         success: function(Response) {
            repoEndWait();
            prepareImport(Response, PostData);
         },
         error  : ErrorHandler
      });
   }

   function showLocalChannels(Vars) {
      $("#import_export_list").html("");
      repoWait();
      changeRepo();
      $("#RepoSelector").on("change", changeRepo);
      $.ajax({
         url    : '/sc/view_local_channels',
         method : "GET",
         data   : { "for_export" : true },
         success: prepareExport,
         error  : handleChannelListError
      });
   }
   // END: HTTP API calls


   // BEGIN: Dedicated export view
   function prepareExport(Response) {
      console.log(Response);
      repoEndWait();
      m_Page.CurrentChannelList = alphabetize(Response.channel_list || [], "name");
      if( ! m_Page.CurrentChannelList.length ){
          $("#RepoSelect").remove();
          displayWarning('There are no valid channels present in Iguana to export.');
          return;
      }
      $("#import_export_list").html( buildIxportableTable(true) );
      $("tr.IxportChannel").click(function() {
         toggleTick($(this));
      });
      $("tr.IxportChannel td a, .sampleDataChannel").click(function(event) {
         event.stopPropagation();
      });
      updateIxportChoices($(".ixportChannel"), true);
      $('<a class="action-button blue" id="ExportTrigger">Export selected channels</a>').appendTo($("#RepoSelect"));

      var Ticks = $(".ixportChannel");
      SCMbumpTickCheck(Ticks, $("#ixportAll"), "", "index");
      Ticks.on("change", function(event) {
         updateIxportChoices($(this), true);
      });
      Ticks.click(function(event) {
         event.stopPropagation();
      });

      var SdTicks = $(".sampleDataChannel");
      SCMbumpTickCheck(SdTicks, $("#sampleDataAll"), "", "index");
      SdTicks.on("change", function(event) {
         updateIxportChoices(Ticks, true);
      });
      $("a#ExportTrigger").click(function() {
         if ($("input:checked").length > 0) {
            m_Page.Task = "/export_channels";
            removeNoChannelSelectionWarning();
            initPopup(popupContentIxport);
            attemptImportOrExport();
         } else {
            displayNoChannelSelectionWarning("Please select at least one channel for export.");
         }
      });
   }

   function toggleTick(TableRow) {
      var Tick = TableRow.find(".ixportChannel");
      if (Tick.prop("checked")) {
         Tick.prop("checked", false);
      } else {
         Tick.prop("checked", true);
      }
      Tick.change();
   }

   function displayNoChannelSelectionWarning(Message) {
      $("#ErrorMessage").html("<p class='error' style='margin: 20px 0px; width: 100%; text-align: center'>" + Message + "</p").show();
   }

   function removeNoChannelSelectionWarning() {
      $("#ErrorMessage").html("").hide();
   }

   function changeRepo() {
      m_Page.CurrentRepo = $("#RepoSelector").val();
   }

   function loginForImportExport(ReqVars) {
      initPopup(popupContentAuth);

      var UserField = $("#repo-username");
      var PassField = $("#repo-password");

      if (ReqVars && (ReqVars["auth_user"] || ReqVars["auth_password"]) ) {
         UserField.val(ReqVars["auth_user"] );
         PassField.val(ReqVars["auth_password"] );
         $("#AuthNotice").html("The username or password you provided was incorrect.").addClass("error");
      }

      m_Page.Popup.dialog(SCM_POPUP_PARAMS);
      m_Page.Popup.dialog("option", "title", "Enter Repository Credentials");

      function doLogin() {
         m_Page.Cred.auth_user     = UserField.val();
         m_Page.Cred.auth_password = PassField.val();
         attemptImportOrExport();
      }

      PassField.on("change keyup keydown", function(event) {
         var code = event.keyCode || event.which;
         if (code == 13) { // enter key
            doLogin();
         }
      });

      var DlgButtons = {
         'Continue': doLogin,
         'Cancel': function() {
            UserField.val("");
            PassField.val("");
            m_Page.Cred = {};
            m_Page.CommitMessage = null;
            $(this).dialog('close');
         }
      };

      m_Page.Popup.dialog("option", "buttons", DlgButtons);
      $("#repo-username,#repo-password").css("width", "95%");

      if (! m_Page.Popup.dialog("isOpen")) {
         m_Page.Popup.dialog("open");
      }
   }


   function sshUsernameForImportExport(ReqVars) {
      initPopup(popupContentSshUsername);
      var UserField = $("#repo-username");

      if (ReqVars && ReqVars["auth_user"]) {
         UserField.val(ReqVars["auth_user"] );
         $("#AuthNotice").html("The SSH username you provided was incorrect.").addClass("error");
      }

      m_Page.Popup.dialog(SCM_POPUP_PARAMS);
      m_Page.Popup.dialog("option", "title", "Enter SSH Username");

      function attemptWithUsername() {
         m_Page.Cred.auth_user = UserField.val();
         attemptImportOrExport();
      }

      UserField.on("change keyup keydown", function(event) {
         var code = event.keyCode || event.which;
         // enter key
         if (code == 13) { attemptWithUsername(); }
      });

      var DlgButtons = {
         'Continue': attemptWithUsername,
         'Cancel': function() {
            UserField.val("");
            m_Page.Cred = {};
            m_Page.CommitMessage = null;
            $(this).dialog('close');
         }
      };

      m_Page.Popup.dialog("option", "buttons", DlgButtons);
      $("#repo-username").css("width", "95%");

      if (! m_Page.Popup.dialog("isOpen")) {
         m_Page.Popup.dialog("open");
      }
   }
   // END: Dedicated export view


   // BEGIN: Dedicated import view
   function renderImportTable() {
      console.log("Rendering Import table...");
      $("#import_export_list").empty();
      $("#import_export_list").html(buildIxportableTable(false));
      
      $('<a class="action-button blue" id="ImportTrigger">Import selected channels</a>').appendTo($("#ixport_list"));
      $("tr.IxportChannel").click(function() {
         toggleTick($(this));
      });
      $("tr.IxportChannel td a, .sampleDataChannel").click(function(event) {
         event.stopPropagation();
      });

      var Ticks = $(".ixportChannel");
      SCMbumpTickCheck(Ticks, $("#ixportAll"), "", "index");

      Ticks.on("change", function(event) {
         updateIxportChoices($(this));
      });
      Ticks.click(function(event) {
         event.stopPropagation();
      });

      $("a#ImportTrigger").click(function() {
         var AnyChannelsSelected = $(".ixportTable tbody input:checked").length > 0;
         if (AnyChannelsSelected) {
            m_Page.Task = "/import_channels";
            removeNoChannelSelectionWarning();
            initPopup(popupContentIxport);
            attemptImportOrExport();
         } else {
            displayNoChannelSelectionWarning("Please select at least one channel for import.");
         }
      });
   }

   function getChannelVersionKey(KeyNumber) {
      var Count = 1;
      for (Key in m_Page.AllVersionedChannelsList) {
         if (Count == KeyNumber) {
            return Key;
         }
         Count++;
      }
      return "";
   }

   function getChannelVersionCount() {
      var Count = 0;
      for (Key in m_Page.AllVersionedChannelsList) {
         Count++;
      }
      return Count;
   }

   function getSortedChannelVersionKeys() {
      var Keys = [];
      for (Key in m_Page.AllVersionedChannelsList) {
         Keys.push(Key);
      }
      Keys.sort();
      return Keys;
   }

   function removeChannelVersionSelector() {
      $("#ChannelVersionSelect").remove();
   }

   function getChannelVersionKeyDisplayName(ChannelVersionKey) {
      var VersionParts = ChannelVersionKey.split("_");
      console.log(VersionParts);
      var VersionDisplayName = "v";
      for (var i = 0; i < VersionParts.length; i++) {
         VersionDisplayName += VersionParts[i];
         if (i != VersionParts.length - 1) {
            VersionDisplayName += ".";
         }
      }
      return VersionDisplayName;
   }

   function addChannelVersionSelectorEvents() {
      $(".ChannelVersionRadio").click(function() {
         var SelectedVersion = $(this).attr("value");
         console.log("Clicked channel version: " + SelectedVersion);
         m_Page.CurrentChannelList = alphabetize( m_Page.AllVersionedChannelsList[SelectedVersion] || [], "name");
         m_Page.CurrentChannelVersion = SelectedVersion;
         renderImportTable();
      });
   }

   function buildChannelVersionSelectors() {
      var Selectors = "";
      for (var i = 0; i < m_SortedChannelVersionKeys.length; i++) {
         var CurrentVersionKey = m_SortedChannelVersionKeys[i];
         var VersionDisplayName = getChannelVersionKeyDisplayName(CurrentVersionKey);
         console.log("Display Name: " + VersionDisplayName);
         Selectors += "<span class='ChannelVersionSelector'>" +
                           "<input type='radio' class='ChannelVersionRadio' name='channelVersionSelector'" +
                           " value='" + CurrentVersionKey + "'>" + VersionDisplayName +
                      "</span>";
      }

      var SelectorRow = "<div id='ChannelVersionSelect'>" +
                           "<span id='ChannelVersionSelectors'>" +
                              Selectors +
                           "</span>" +
                        "</div>";
      
      $(SelectorRow).insertAfter("#ErrorMessage");
      
      // Need to make sure the lastest version is already selected.
      var LatestVersion = m_SortedChannelVersionKeys[ m_SortedChannelVersionKeys.length - 1 ];
      $( "input[value='"+ LatestVersion +"']" ).prop("checked", true);
      
      addChannelVersionSelectorEvents();
   }

   function handleMultipleChannelVersions() {
      m_SortedChannelVersionKeys = getSortedChannelVersionKeys();
      console.log(m_SortedChannelVersionKeys);

      var LatestVersion = m_SortedChannelVersionKeys[ m_SortedChannelVersionKeys.length - 1 ];
      m_Page.CurrentChannelList = alphabetize( m_Page.AllVersionedChannelsList[LatestVersion] || [], "name");
      m_Page.CurrentChannelVersion = LatestVersion;
      buildChannelVersionSelectors();
   }

   function setupCurrentChannelList() {
      var NumberOfVersions = getChannelVersionCount();
      console.log("Number of Channel Versions recieved: " + NumberOfVersions);
      var DisplayEmptyWarning = false;

      if (NumberOfVersions == 0) {
         console.log("No versions found!");
         DisplayEmptyWarning = true;
      }
      else if (NumberOfVersions == 1) {
         console.log("One version found!");
         var VersionKey = getChannelVersionKey(1);
         console.log(Key);
         m_Page.CurrentChannelList = alphabetize( m_Page.AllVersionedChannelsList[VersionKey] || [], "name");
         m_Page.CurrentChannelVersion = VersionKey;
         if (!m_Page.CurrentChannelList.length) {
            DisplayEmptyWarning = true;
         }
      }
      else {
         console.log("Multiple versions found!");
         handleMultipleChannelVersions();
      }

      if (DisplayEmptyWarning) {
         console.log("No channels present on remote!");
         displayWarning("There are no Iguana channels present in this remote repository.");
         return false;
      }
      return true;
   }

   function prepareImport(Response, ReqVars) {
      removePopup();
      removeChannelVersionSelector();
      m_Page.AllVersionedChannelsList = Response.channel_list;;
      console.log(m_Page.AllVersionedChannelsList);      
      
      var Success = setupCurrentChannelList();
      if (!Success) { 
         return; 
      }
      renderImportTable();
   }

   function loginForChannelList(ReqVars) {
      initPopup(popupContentAuth);
      var UserField = $("#repo-username");
      var PassField = $("#repo-password");
      if (ReqVars["auth_user"] || ReqVars["auth_password"]) {
         UserField.val(ReqVars["auth_user"] );
         PassField.val(ReqVars["auth_password"] );
         $("#AuthNotice").html("The username or password you provided was incorrect.").addClass("error");
      }
      m_Page.Popup.dialog(SCM_POPUP_PARAMS);
      m_Page.Popup.dialog("option", "title", "Enter Repository Credentials");
      m_Page.Popup.on("dialogclose", function(event) {
         displayNoChannelSelectionWarning("This repository requires a username and a password.");
      });
      function go() {
         m_Page.Cred.auth_user     = UserField.val();
         m_Page.Cred.auth_password = PassField.val();
         m_Page.Popup.dialog("close");
         showRemoteChannels(ReqVars);
      }
      PassField.on("change keyup keydown", function(event) {
         var code = event.keyCode || event.which;
         if (code == 13) {
            go();
         }
      });
      var DlgButtons = {
         'Continue': go,
         'Cancel': function() {
            $(this).dialog('close');
         }
      };
      m_Page.Popup.dialog("option", "buttons", DlgButtons);
      $("#repo-username,#repo-password").css("width", "95%");
      if (! m_Page.Popup.dialog("isOpen")) {
         m_Page.Popup.dialog("open");
      }
   }

   function sshUsernameForChannelList(ReqVars) {
      initPopup(popupContentSshUsername);
      var UserField = $("#repo-username");
      
      if (ReqVars["auth_user"]) {
         UserField.val(ReqVars["auth_user"] );
         $("#AuthNotice").html("The username you provided was incorrect.").addClass("error");
      }
      
      m_Page.Popup.dialog(SCM_POPUP_PARAMS);
      m_Page.Popup.dialog("option", "title", "Enter SSH Username");
      
      m_Page.Popup.on("dialogclose", function(event) {
         displayNoChannelSelectionWarning("This repository requires the username associated with your SSH key.");
      });
      
      function attemptWithUsername() {
         m_Page.Cred.auth_user = UserField.val();
         m_Page.Popup.dialog("close");
         showRemoteChannels(ReqVars);
      }

      UserField.on("change keyup keydown", function(event) {
         var code = event.keyCode || event.which;
         if (code == 13) {  attemptWithUsername();  }
      });

      var DlgButtons = {
         'Continue': attemptWithUsername,
         'Cancel': function() {  $(this).dialog('close');  }
      };

      m_Page.Popup.dialog("option", "buttons", DlgButtons);
      $("#repo-username").css("width", "95%");
      
      if (! m_Page.Popup.dialog("isOpen")) {
         m_Page.Popup.dialog("open");
      }
   }
   // END: Dedicated import view

   // BEGIN: Shared import/export tables
   function buildIxportableTable(AreWeExporting) {
      var ActionName = AreWeExporting ? 'Export' : 'Import';
      var TheTable = '<div id="ixport_list">'
                   + '  <table class="ixportTable">'
                   + '     <thead>'
                   + '     <tr>'
                   + '        <th class="ixportLabel">Channel</th>'
                   + '        <th class="notecol">Description</th>'
                   + '        <th class="tick control"><span>' + ActionName + '</span><br><input type="checkbox" name="ixportAll" id="ixportAll"></th>';

         if (AreWeExporting) {
            TheTable += '     <th class="tick control"><span>Include&nbsp;Sample&nbsp;Data</span><br><input type="checkbox" name="sampleDataAll" id="sampleDataAll"></th>';
         }

         TheTable += '     </tr>'
                   + '     </thead>'
                   + '     <tbody>'

                   + buildIxportableRows(AreWeExporting)

                   + '     </tbody>'
                   + '  </table>'
                   + '</div>';
      return TheTable;
   }

   function buildIxportableRows(AreWeExporting) {
      var Rows = "", Checked;
      for (var i = 0; i < m_Page.CurrentChannelList.length; i++) {
         Checked = (m_Page.CurrentChannelList[i].guid in ifware.ExportGuids) ? "checked" : "";
         Rows += '<tr class="IxportChannel">'
              +  '  <td class="ixportLabel">' + m_Page.CurrentChannelList[i].name + '</td>'
              +  '  <td class="ixportNote notecol">' + SCMlinkText(m_Page.CurrentChannelList[i].description) + '</td>'
              +  '  <td class="tick control"><input type="checkbox" ' + Checked + ' class="ixportChannel" data-index="' + i + '"></td>';

         if (AreWeExporting) {
            Rows += '<td class="tick control"><input type="checkbox" ' + Checked + ' class="sampleDataChannel" data-index="' + i + '"></td>';
         }

         Rows += '</tr>';
      }
      return Rows;
   }
   // END: Shared import/export tables



   // BEGIN: Popup utilities
   function popupContentAuth() {
      var Filling = $('<p id="AuthNotice">This repository requires authentication.</p>\
              <label for="repo-username">Username</label><br>\
              <input type="text" id="repo-username" name="repo-username" /><br>\
              <label for="repo-password">Password</label><br>\
              <input id="repo-password" name="repo-username" type="password" /><br>');
      return Filling;
   }

   function popupContentSshUsername() {
      var Filling = $('<p id="AuthNotice">This repository requires the username associated with your SSH key.</p>\
              <label for="repo-username">Username</label><br>\
              <input type="text" id="repo-username" name="repo-username" /><br>');
      return Filling;
   }

   function popupContentIxport() {
      var Filling = $("<p />");
      return Filling;
   }

   function popupContentDemo(UserInitiated) {
      var Filling = $("<p><strong>Welcome to Iguana!</strong> To help you get started, we've created a short tour to introduce you to the product. Are you ready to begin?</p>\
              <p><button id='demo-start'> Yes, I'd like to see the tour </button></p>\
              <p><button id='demo-decline'> No thanks, I'm ready to start working </button></p>");
      Filling.find("#demo-start").button().click(function(event) {
         m_Page.Popup.dialog("close");
         m_Me.takeTour(event);
      });
      Filling.find("#demo-decline").button().click(function(event) {
         if (UserInitiated) {
            m_Page.Popup.dialog("close");
         } else {
            ifware.storage.set(s_DemoNoReminderKey, "true", 365);
            finalizeDisplay("\
               <p> No problem! You can access the tour at any time, should you change your mind.\
               Simply open <a href='/settings'> Settings</a> and click &quot;Take the Tour&quot;. </p>\
            ");
         }
      });
      return Filling;
   }

   function removePopup() {
      $("#ixport-popup").remove();
   }

   function validateCommitMessage(){
      if(m_Page.CommitMessage){  return true; }
      $("#user_commit_message input").css("border", "1px solid red");
      $("#user_commit_message_error").remove();
      var Error = "<div id='user_commit_message_error' style='color:red'><p>You must enter a valid commit message to continue!</p></div>"
      m_Page.Popup.append(Error);
   }

   function initCommitMessagePopup(){
      console.log("initCommitMessagePopup");
      removePopup();
      $("body").append("<div id='ixport-popup'></div>");
      m_Page.Popup = $("#ixport-popup");

      function okOrEnter() {
          m_Page.CommitMessage = $("#user_commit_message input").val();
         var IsValid = validateCommitMessage();
         if( ! IsValid ){  return; }
         attemptImportOrExport();
      }

      var CommitMessageButtons = {
         'OK': okOrEnter, 
         'Close': function() {
            $(this).dialog("close");
         }
      };

      var Params = {
         modal: true,
         minWidth: 450,
         resizable: false,
         buttons : CommitMessageButtons,
         title : "Enter a Commit Message",
         close: function() {
           m_Page.CommitMessage = null;
           $(this).remove();
         }
      };

      m_Page.Popup.dialog(Params);
      var Content = "<p>Enter a commit message for the " + taskKey() + "ed" + " channel data.</p><br>" +
                     "<div id='user_commit_message'><input type=text></input></div>";

      m_Page.Popup.append(Content);
      m_Page.Popup.dialog("open");
      console.log("Opening the popup");
      console.log(m_Page.Popup);
      $("#user_commit_message input").on("change keyup keydown", function(event) {
         var code = event.keyCode || event.which;
         if (code == 13) { // enter key
            okOrEnter();
         }
      });
   }

   function initPopup(FillerFunc, UserInitiated) {
      removePopup();
      $("body").append("<div id='ixport-popup'><img src='jqueryFileTree/images/spinner.gif' style='display: none;'></div>");
      m_Page.Popup = $("#ixport-popup");
      m_Page.Popup.prepend(FillerFunc(UserInitiated));
      m_Page.Popup.dialog(SCM_POPUP_PARAMS);
      // Fix for #22776 - we trigger a resize event on the window so the dialog will be selectable in IE 10.
      $(window).resize();
   }

   function isPopupInitalized(){
      return $("#ixport-popup").length > 0;
   }

   function finalizeDisplay(Content, ErrorOccurred) {
      m_Page.Popup = $(m_Page.Popup);
      if( ! isPopupInitalized() ){  initPopup(popupContentIxport);  }
      m_Page.Popup.html(Content);
      if(ErrorOccurred){
         m_Page.Popup.append("<p> If you are having trouble resolving this error, please\
               <a href=mailto:support@interfaceware.com?Subject=Iguana%20Channel%20Demo%20Failed> email us</a>. </p>");
         m_Page.Popup.dialog("option", "title", "Error");
      }
      m_Page.Popup.dialog("option", "buttons", {
         Close: function() {
            $(this).dialog("close");
         }
      });
      if( ! m_Page.Popup.dialog("isOpen") ){  m_Page.Popup.dialog("open");  }
   }
   // END: Popup utilities



   // BEGIN: Business logic
   function buildLocalGuidToNameMap(){
      for(var ChannelName in m_Page.ChannelCollisionMap){
         var LocalGuid = m_Page.ChannelCollisionMap[ChannelName]["local_guid"];
         m_Page.LocalGuidsToNameMap[LocalGuid] = ChannelName;
      }
   }

   var VersionLetterGenerator = function(){
      // Version letter generator for handling shared file version conflicts 
      // on export.
      var LETTER_A = 65;
      var LETTER_Z = 90;
      var m_CurrentValue = null;
      var m_RepeatCount = null;

      function doLetterRepetition(StringValue){
         var RepeatedLetters = '';
         for(var i = 0; i < m_RepeatCount; i++){
            RepeatedLetters += StringValue;
         }
         return RepeatedLetters;
      }

      this.getNextLetter = function(){
         console.log("m_CurrentValue: " + this.m_CurrentValue + "  " + "m_RepeatCount: " + this.m_RepeatCount);
         if(m_CurrentValue === null || m_RepeatCount === null){
            console.log("Initalizing...");
            m_CurrentValue = LETTER_A;
            m_RepeatCount = 1;
         }
         if(m_CurrentValue > LETTER_Z){
            console.log("End of alphabet. Increasing repeat count...");
            m_CurrentValue = LETTER_A;
            m_RepeatCount++;
         }
         var StringValue = String.fromCharCode(m_CurrentValue);
         var RepeatedStringValue = doLetterRepetition(StringValue);
         m_CurrentValue++;
         return RepeatedStringValue;
      }

      this.reset = function(){
         m_CurrentValue = null;
         m_RepeatCount = null;
      }
   }

   function exportSharedFileVersionConflicts(ExportSummary){
      $("#ApprovalDlg").remove();
      $("body").append('<div id="ApprovalDlg"><form id="ApprovalForm" /></div>');
      var DialogBox = $("#ApprovalDlg");
      var Content = ""
      var VersionConflictMsg = "<p>Some of the translators in the channels you are attempting to export are set to run from " +
                               "commits with different versions of the following shared file(s):<br></p>";
      Content += VersionConflictMsg;
      console.log(Content);
      
      var Conflicts = '';
      var VersionLetters = new VersionLetterGenerator();
      console.log(Version);

      for(var SharedFile in ExportSummary.conflicting_files){
         VersionLetters.reset();
         Conflicts += "<span class='conflictedSharedFileHeader'>" + SharedFile + "</span>";

         for(var Version in ExportSummary.conflicting_files[SharedFile]["versions"]){
            Conflicts += "<div class='conflictedSharedFileVersionContainer'>" +
                           "<span class='conflictedSharedFileLetter'>"+ VersionLetters.getNextLetter() + "</span>" +
                           "<span class='conflictedSharedFileVersionBox'>";
            for(var Channel in ExportSummary.conflicting_files[SharedFile]["versions"][Version]["channels"]){
               var ComponentType = ExportSummary.conflicting_files[SharedFile]["versions"][Version]["channels"][Channel].component_type;
               Conflicts += "<div>" + Channel + "<span style='font-weight: bold'> >> </span> " + ComponentType + "</div>";
            } 
            Conflicts +=   "</span>";
            Conflicts += "</div>";
         }

      }

      Content += Conflicts;
      Content += "<p>You must correct this if you want to export this group of channels at the same time.</p>";
      Content += "<p>" +
                     "Note: <br> The translator(s) in each grouping are set to run from the same version of the corresponding shared file, with each " +
                     "grouping representing a different version of that file.<br>" +
                     "In order to correct this you must set all translators to run from a commit containing the same version of the conflicting shared file." +
                 "</p>";
      DialogBox.find("#ApprovalForm").html(Content);
      var DialogWidth = $(window).width() * 0.85;
      var DialogMaxHeight = $(window).height() * 0.9;
      var VersionConflictButtons = {
         'OK': function() {
            $(this).dialog('close');
         }
      };
      DialogBox.dialog({
         bgiframe:  true,
         width:     DialogWidth,
         maxHeight: DialogMaxHeight,
         title:     "Shared File Version Conflicts",
         modal:     true,
         autoOpen:  false,
         buttons:   VersionConflictButtons,
         resizable: true,
         close:     closeApprover
      });
      m_Page.Popup.dialog("close");
      DialogBox.dialog("open");
   }

   function removeFileDiffsFromConfirmationMap(){
      // There is no need to send the file diffs back to the server
      // with the file confirmations.
      for (var Path in m_Page.FileCollisionMap) {
         if (! m_Page.FileCollisionMap.hasOwnProperty(Path)) { continue; }
         m_Page.Diffs[Path] = m_Page.FileCollisionMap[Path].diff;
         delete m_Page.FileCollisionMap[Path].diff;
      }
   }


   function buildCollisionConfirmationHeader() {
      var NumOfChannelCollisions = Object.keys(m_Page.ChannelCollisionMap).length;
      var NumOfFileCollisions = Object.keys(m_Page.FileCollisionMap).length;
      var TaskKey = taskKey();
      var Header = "<p>";

      if (NumOfFileCollisions == 1 && NumOfChannelCollisions == 1) {
         Header = "The channel you are trying to " + TaskKey + " below has the same name as a channel that already exists. " +
                  "It also uses a different version of a shared file that already exists on this " + (TaskKey == 'import' ? 'instance.' : 'repository.');
      }
      else if(NumOfFileCollisions == 1 && NumOfChannelCollisions > 1) {
         Header = "The channels you are trying to " + TaskKey + " below have the same names as channels that already exist. " +
                  "They also use a different version of a shared file that already exists on this " + (TaskKey == 'import' ? 'instance.' : 'repository.');
      }
      else if(NumOfFileCollisions > 1 && NumOfChannelCollisions == 1) {
         Header = "The channel you are trying to " + TaskKey + " below has the same name as a channel that already exists. " +
                  "It also uses a different versions of shared files that exist already on this " + (TaskKey == 'import' ? 'instance.' : 'repository.');
      }
      else if (NumOfFileCollisions > 1 && NumOfChannelCollisions > 1) {
         Header = "The channels you are trying to " + TaskKey + " below have the same names as channels that already exist. " +
                  "They also use different versions of shared files that exist already on this " + (TaskKey == 'import' ? 'instance.' : 'repository.'); 
      }
      else if(NumOfFileCollisions == 1) {
         Header = "The channel you are trying to " + TaskKey + " uses a different version of a shared file that already exists on this " + 
                  (TaskKey == 'import' ? 'instance.' : 'repository.');
      }
      else if(NumOfFileCollisions > 1) {
         Header = "The channel you are trying to " + TaskKey + " uses different versions of shared files that exist already on this " + 
                  (TaskKey == 'import' ? 'instance.' : 'repository.');
      }
      else if(NumOfChannelCollisions == 1) {
         Header = "The channel you are trying to " + TaskKey + " below has the same name as a channel that already exists on this " + 
                  (TaskKey == 'import' ? 'instance.' : 'repository.');
      }
      else if (NumOfChannelCollisions > 1) {
         Header = "The channels you are trying to " + TaskKey + " below have the same names as channels that already exist on this " + 
                  (TaskKey == 'import' ? 'instance.' : 'repository.');
      }

      Header += "</p><p>Please make a decision on how to proceed.</p>";
      return Header;
   }

   function displayChannelCollisionConfirmations(Response){
      console.log(Response);
      var Key = taskKey();
      m_Page.ChannelCollisionMap = Response[Key + '_summary'].channel_collision_confirmations     || {};
      buildLocalGuidToNameMap();
      m_Page.FileCollisionMap    = Response[Key + '_summary'].overwrite_shared_file_confirmations || {};
      
      removeFileDiffsFromConfirmationMap(); // TODO : Abstract this out (dupped in restore.)

      $("#ApprovalDlg").remove();
      $("body").append('<div id="ApprovalDlg"><form id="ApprovalForm" /></div>');
      var Asker = $("#ApprovalDlg");
      var DlgButtons = [
         {
            id: "confirmation-button-continue",
            text: "Continue",
            click: function() {
               var NoInputErrors = attemptImportOrExport();
               if(NoInputErrors){   
                  $(this).dialog('close');   
               }
            }
         },
         {
            id: "confirmation-button-cancel",
            text: 'Cancel ' + Key,
            click: function() {
               $(this).dialog('close');
            }
         }
      ];

      var HelpText = "";
      
      /*
      HelpText += "For channels where the name and all component types are the same you can:\n update translator files, update the channel configuration, update both, " +
                "or " + Key + " it as a new channel with a different name.\n\n";
      HelpText += "For channels where the names are the same, but the coponent types are different you can:\n" + Key + " it as a new channel with a different name.\n\n";
      
      var HelpIcon = '<a id="group_text_query_icon_left" class="helpIcon" tabindex="100" title="More Information" target="_blank" href="" rel='+ HelpText +'>' +
                        '<img class="filter" src="/images/help_icon.gif" border="0"></a>';
      */

      var Header = buildCollisionConfirmationHeader();
      
      var TitleCaseTaskKey = taskKeyTitleCase();
      var CTbl = '';
      if (Object.keys(m_Page.ChannelCollisionMap).length > 0) {
            CTbl += SCMbuildChannelCollisionTable(Key == 'import', TitleCaseTaskKey, m_Page.ChannelCollisionMap);
      }

      var FTbl = '';
      if (Object.keys(m_Page.FileCollisionMap).length > 0) {
         console.log("m_Page.FileCollisionMap is:");
         console.log(m_Page.FileCollisionMap);
         FTbl = SCMbuildFileCollisionTable(TitleCaseTaskKey, m_Page.FileCollisionMap);
      }

      Asker.find("#ApprovalForm").html(Header + CTbl + FTbl);
      var DialogWidth = $(window).width() * 0.85;
      var DialogMaxHeight = $(window).height() * 0.9;

      Asker.dialog({
         bgiframe:  true,
         width:     DialogWidth,
         maxHeight: DialogMaxHeight,
         title:     'Channel Collison Confirmations',
         modal:     true,
         autoOpen:  false,
         buttons:   DlgButtons,
         resizable: true,
         close:     closeApprover
      });

      var ChannelSelectBoxes = $("#collision_list .CollisionChannel select.dropdown");
      var CollidingFileCheckBoxes = $("#bump_file_list input.bumpUpdate");

      SCMbumpTickCheck(CollidingFileCheckBoxes, $("#bump_file_list input#BumpAll"), "", "filepath");
      
      CollidingFileCheckBoxes.on("change", function(event) {
         SCMupdateCollidingFileApprovals($(this), m_Page.FileCollisionMap);
         SCMcheckIfContinueButtonNeedsDisabling(ChannelSelectBoxes, CollidingFileCheckBoxes);
      });
      
      SCMremoveInvalidChannelSelectOptions(m_Page, ChannelSelectBoxes);
      SCMdisplayUpdateWarningMessageIfNeeded(taskKey(), m_Page.NameOnlyCollisionsPresent);
      
      SCMsetSelectBoxChangeEvents(m_Page, ChannelSelectBoxes, $("#collision_list select#bulk_action"), "cguid", taskKey());

      function continueButtonNeedsDisablingEvent() {
         SCMcheckIfContinueButtonNeedsDisabling(ChannelSelectBoxes, CollidingFileCheckBoxes);
      }

      ChannelSelectBoxes.on("change", continueButtonNeedsDisablingEvent);
      continueButtonNeedsDisablingEvent();

      $("#diffLink").click(function() {
         ChannelSelectBoxes.off("change", continueButtonNeedsDisablingEvent); // Need to re-attach since file boxes will be recreated.
         SCMbuildCollidingFileDiffView(m_Page, taskKeyTitleCase(), getCollatedProjects, addCollidingFileDiffViewContinueAction, closeApprover);
      });

      m_Page.Popup.dialog("close");
      Asker.dialog("open");

      if (document.location.pathname == '/settings' && Key == 'import') {
         SCMpullStatus(m_Page.StatusMap, SCM_BUMP_NOTE);
         m_Page.StatusInterval = setInterval( function() {
            SCMcheckChannelLights(m_Page.StatusMap, SCM_BUMP_NOTE);
         }, 1000);
      }
   }

   function addCollidingFileDiffViewContinueAction(){
      $("#SCMgo").click(function(event) {
         var NoInputErrors = attemptImportOrExport();
         if(NoInputErrors){
            $("#SCMreview").dialog("close");
         }
      });
   }


   function handleChannelListError(RequestObject, Status, ErrorString, ReqVars) {
      console.log("handleChannelListError");
      console.log(RequestObject);
      console.log(Status);
      console.log(ErrorString);
      console.log(ReqVars);
      if(Status == "abort"){ return; } //if we abort it isn't an error.
      repoEndWait();
      var ErrorMessage = RequestObject.responseJSON.error
                         ? RequestObject.responseJSON.error.description
                         : RequestObject.status + " - " + RequestObject.statusText;

      switch(ErrorMessage){
         case 'Unsupported URL protocol':
            ErrorMessage = "Invalid URL/Path. Please check the URL/path to the repository and make sure the protocol and address/path are correct.";
            break;
         case 'SSH username required.':
            sshUsernameForChannelList(ReqVars);
            return;
         case 'Authentication failed, make sure your credentials are valid and try again.':
         case 'Authentication is required, please provide a valid username and password to continue.':
         case 'Authentication Error.':
            loginForChannelList(ReqVars);
            return;
      }
      var Message = "<p> We're sorry, but the request could not be completed. The following error occurred: </p>"
                  + "<p>" + ErrorMessage + "</p>";

      finalizeDisplay(Message, true);
   }

   function handleImportExportError(RequestObject, Status, ErrorString, ReqVars){
      console.log("handleImportExportError");
      console.log(RequestObject);
      console.log(Status);
      console.log(ErrorString);
      console.log(ReqVars);
      if(Status == "abort"){ return; } //if we abort it isn't an error.
      var ErrorMessage = RequestObject.responseJSON.error
                         ? RequestObject.responseJSON.error.description
                         : RequestObject.status + " - " + RequestObject.statusText;
      console.log(ErrorMessage);
      switch (ErrorMessage) {
         case 'SSH authentication is required, please provide a valid private key path and (if required) passphrase to continue.':
            ErrorMessage = "This repository requires an SSH key.";
            break;
         case 'SSH username required.':
            sshUsernameForImportExport(ReqVars);
            return;
         case 'Authentication failed, make sure your credentials are valid and try again.':
         case 'Authentication is required, please provide a valid username and password to continue.':
         case 'Authentication Error.':
            loginForImportExport(ReqVars);
            return;
         case "Must select at least one channel.":
            displayNoChannelSelectionWarning("Please select at least one channel for " + taskKey() + ".");
            return;
         case 'Unsupported URL protocol':
            ErrorMessage = "Unsupported URL protocol. Please check the URL/path to the repository.";
            break;
         case "Your account does not have administrative privileges.":
            if (location.pathname == "/dashboard.html") {
               ErrorMessage = "You must be logged in as administrator to import the demo channels.";
            }
            break;
      }

      var Message = "<p> We're sorry, but the request could not be completed. The following error occurred: </p>"
                  + "<p>" + ErrorMessage + "</p>";

      finalizeDisplay(Message, true);
   }

   function handleResponse(Response) {
      console.log(Response);
      var Key = taskKey();
      if (Response['success']) {
         var Congrats = "<p>Success! The channels have " + Key + "ed correctly.</p>";
         if (Key == 'import') {
            Congrats += "<p><a href='/'>View the channels.</a></p>";
         }
         m_Page.CommitMessage = null;
         finalizeDisplay(Congrats);
         return;
      }
      if(Key == "export" && Response['export_summary'].shared_file_version_conflicts == true){
         console.log("Not allowed to export channels that are running from multiple versions of the same shared file!");
         exportSharedFileVersionConflicts(Response['export_summary']);
         return;
      }
      displayChannelCollisionConfirmations(Response);
   }
   // END: Business logic


   // BEGIN: Utilities
   function updateIxportChoices(Ticks, AreWeExporting) {
      Ticks.each(function() {
         var iString = $(this).attr("data-index");
         var i = parseInt(iString);
         var On = $(this).prop("checked");
         m_Page.CurrentChannelList[i].selected = On;

         if (On) {
            $(this).parents("tr.IxportChannel").addClass("selected");
         } 
         else {
            $(this).parents("tr.IxportChannel").removeClass("selected");
         }
         
         if (!AreWeExporting) { 
            return; 
         }
         
         var SdTick = $(".sampleDataChannel[data-index='" + iString + "']");
         if (On) {
            SdTick.prop("disabled", false);
            m_Page.CurrentChannelList[i].sample_data = SdTick.prop("checked");
         } 
         else {
            SdTick.prop("disabled", true);
            m_Page.CurrentChannelList[i].sample_data = false;
         }
      });
   }

   function buildIxportList() {
      console.log("building da list.");
      var Selected = [];
      for (var i = 0; i < m_Page.CurrentChannelList.length; i++) {
         if (m_Page.CurrentChannelList[i].selected) {
            Selected.push(m_Page.CurrentChannelList[i]);
         }
      }
      return Selected;
   }

   function taskKey() {
      return m_Page.Task.substring(1,7);
   }

   function taskKeyTitleCase(){
      var TaskKey = taskKey();
      return TaskKey.charAt(0).toUpperCase() + TaskKey.substr(1).toLowerCase();
   }

   function closeApprover(Page) {
      console.log("Closing approver");
      $("#SCMreview").remove();
      if (m_Page.StatusInterval != 0) {
         clearInterval(m_Page.StatusInterval);
         m_Page.StatusInterval = 0;
      }
      m_Page.ChannelCollisionMap = {};
      m_Page.FileCollisionMap    = {};
      m_Page.CollationMap        = {};
      m_Page.StatusMap           = {};
      m_Page.IndexMap            = {};
      m_Page.LocalGuidsToNameMap = {};
   }

   function alphabetize(List, KeyName) {
      return List.map(function(OneItem) {
         return [OneItem[KeyName].toLocaleLowerCase(), OneItem];
      }).sort(SCMcompareFunc).map(unDecorate);
   }

   function unDecorate(LittleMap) {
      return LittleMap[1];
   }

   function repoWait() {
      $("#SCMloadWheelContainer").show();
   }

   function repoEndWait() {
       $("#SCMloadWheelContainer").hide();
   }

   function removeWarning(){
      $("#import_export_list p.error").remove();
   }

   function displayWarning(Message) {
      console.log("displayWarning");
      removeWarning();
      $("#import_export_list").prepend("<br><div style='text-align: center'><span><p class='error'>" + Message + "</p></span></div>");
   }

   function isOldInstall(DashboardData) {
      var Servers = DashboardData.Servers;
      if (Servers) {
         if (Servers.length > 1) {
            return true;
         }
         var LocalData = Servers[0].DashboardData;
         if (LocalData && LocalData.TotalCountOfChannel > 0) {
            return true;
         }
      }
      return false;
   }

   // END: Utilities


   // BEGIN: Public API
   this.statusMap = function() {
      return m_Page.StatusMap;
   }

   this.openDemoPopup = function(UserInitiated) {
      m_Page.UserInitiated = UserInitiated;
      initPopup(popupContentDemo, UserInitiated);
   }

   this.checkForDemo = function(DashboardData) {
      if (m_Page.HasPrompted || isOldInstall(DashboardData) || ifware.storage.get(s_DemoNoReminderKey)) {
         return;
      }
      m_Page.HasPrompted = true;
      m_Me.openDemoPopup();
   }

   this.takeTour = function(event) {
      var NewWin = window.open('http://training.interfaceware.com/iguana-overview/', '_blank');
      ifware.storage.set(s_DemoNoReminderKey, "true", 365);
      NewWin.focus();
   }

   this.show = function(Name, Remote) {
      SCMclearActiveRequest(m_Page.CurrentRequest);
      m_Page.CurrentRepo = Name;
      m_Page.CurrentRemote = Remote || {};
      m_Page.IsIfwareRemote = false;
      
      if (Remote) {
         // If Remote is passed in, we know that it is an offical iNTERFACEWARE repo.
         m_Page.IsIfwareRemote = true;
      }

      showRemoteChannels();
   }

   this.showLocal = function() {
      showLocalChannels();
   }
   //END: Public API
}

function setupImportManager() {
   ifware = window.ifware || {};
   ifware.ImportManager = new ImportManager();
}

setupImportManager();

