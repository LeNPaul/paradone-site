/** @license
 * Copyright (c) 2010-2016 iNTERFACEWARE Inc.  All rights reserved.
 */


var ChannelRestoreManager = function() {

   // Private vars
   var m_Me   = this;
   var m_Page = {
      Files               : {},
      Popup               : '',
      Commits             : [],
      CurrentFile         : "",
      CurrentRequest      : {},
      CurrentCommitIndex  : 0,
      
      IndexMap            : {},
      StatusMap           : {},
      CollationMap        : {},
      FileCollisionMap    : {},
      ChannelCollisionMap : {},
      
      DiffFormat          : "vertical",
      Diffs               : {},

      StatusInterval      : 0,
      LocalGuidsToNameMap : {},
      NameOnlyCollisionsPresent : false
   }



   // BEGIN : General helpers
   function showCommitsLoadingWheel(){
      $("#HistoryBody #SCMcommitList").html("").addClass("SCMloading");
   }

   function hideCommitsLoadingWheel(){
      $("#HistoryBody #SCMcommitList").removeClass("SCMloading");
   }

   function showFilesLoadingWheel(){
      $("#HistoryBody #SCMpanel ul.tree").remove();
      $("#HistoryBody #SCMpanel").addClass("SCMloading");
   }

   function hideFilesLoadingWheel(){
      $("#HistoryBody #SCMpanel").removeClass("SCMloading");
   }

   function showFileReviewLoadingWheel(){
      $("#HistoryBody .preview_toolbar .toolbar_filename").html(""); // clear title
      $("#HistoryBody #SCMpreview").html("").addClass("SCMloading");
   }

   function hideFileReviewLoadingWheel(){
      $("#HistoryBody #SCMpreview").removeClass("SCMloading");
   }

   function showRestoreButton() {
      $("#SCMrestoreButtonContainer").show();  
   }

   function hideRestoreButton() {
      $("#SCMrestoreButtonContainer").hide();
   }

   function cancel() {
      document.location = '/settings';
   }

   function currentCommitId() {
      return m_Page.Commits[m_Page.CurrentCommitIndex].commit_id;
   }

   function currentCommitIsBulkDeletion(){
      // Channel delete commit comments are automatically done and are hardcoded on
      // the backend. This functionality will break if they are ever changed. 
      var CurrentCommitComment = m_Page.Commits[m_Page.CurrentCommitIndex].commit_comment;
      var Match = CurrentCommitComment.match(/Bulk channel deletion performed by user \"(.+)\"./);  
      return Match && Match.length > 0;
   }

   function currentCommitName() {
      // Will only work if it is a non-delete commit that is currently selected.
      return getChannelNameFromDeleteCommitComment( m_Page.Commits[m_Page.CurrentCommitIndex].commit_comment );
   }

   function getChannelNameFromDeleteCommitComment(Comment){
      // These delete channel commit comments are automatically done and are hardcoded on
      // the backend. This functionality will break if they are ever changed. 
      var Match = Comment.match(/Channel \"(.+)\" deleted./);
      var ChannelName = '';
      if(Match){
         return PAGhtmlEscape(Match[1]); //2nd index is capture match.
      }
      Match = Comment.match(/Channel "(.*)" removed by user ".+" using Channel API./);
      if(Match){
         return PAGhtmlEscape(Match[1]);
      }
      return Comment;// Default
   }

   function resetCurrentFile() {
      var Files = Object.keys(m_Page.Files);
      for (var i = 0; i < Files.length; i++) {
         if (m_Page.CurrentFile == m_Page.Files[i]) {
            return;
         }
      }
      m_Page.CurrentFile = Files[0];
   }

   function numberOfKeys(Obj){
      return Object.keys(Obj).length;
   }

   function buildLocalGuidToNameMap(){
      for(var ChannelName in m_Page.ChannelCollisionMap){
         var LocalGuid = m_Page.ChannelCollisionMap[ChannelName]["local_guid"];
         m_Page.LocalGuidsToNameMap[LocalGuid] = ChannelName;
      }
   }

   function enableKeysAndRefresh() {
      console.log("enableKeysAndRefresh");
      SCMcommitKeys(m_Page.CurrentCommitIndex, m_Page.Commits.length - 1, function(Index) {
         var Increase = (Index > m_Page.CurrentCommitIndex)
                        ? true
                        : false;
         m_Page.CurrentCommitIndex = Index;
         $('tr.commitRow').removeClass("current");
         $('tr.commitRow[data-index="' + Index + '"]').addClass("current");
         console.log($('tr.commitRow.current').position());
         SCMscrollCommitList();
         getDeletedChannelDetails();
      });
   }

   function disableKeys(){
      console.log("disableKeys");
      SCMdisableKeys();
   }
   // END : General helpers



   // BEGIN : Dialog handlers
   function popupBaseContent() {
      var Filling = $("<p />");
      return Filling;
   }

   function popupCustomContent(Content){
      var Base = "<p>" + Content + "</p>";
      return Base;
   }

   function isPopupInitalized(){
      return $("#restore-popup").length > 0;
   }

   function removePopup() {
      $("#restore-popup").remove();
   }

   function removeWarning(){
      $("#HistoryBody #SCMcommitList p.error").remove();
   }

   function displayWarning(Message) {
      console.log("displayWarning");
      removeWarning();
      $("#HistoryBody #SCMcommitList").prepend("<br><div style='text-align: center'><span><p class='error'>" + Message + "</p></span></div>");
   }

   function initPopup(Content) {
      removePopup();
      $("body").append("<div id='restore-popup'><img src='jqueryFileTree/images/spinner.gif' style='display: none;'></div>");
      m_Page.Popup = $("#restore-popup");
      m_Page.Popup.prepend(Content);
      m_Page.Popup.dialog(SCM_POPUP_PARAMS);
      // Fix for #22776 - we trigger a resize event on the window so the dialog will be selectable in IE 10.
      $(window).resize();
   }

   function finalizePopupDisplay(Content, ErrorOccurred) {
      m_Page.Popup = $(m_Page.Popup);
      if( ! isPopupInitalized() ){  initPopup(popupBaseContent());  }
      m_Page.Popup.html(Content);
      if(ErrorOccurred){
         m_Page.Popup.append("<p> If you are having trouble resolving this error, please\
               <a href=mailto:support@interfaceware.com?Subject=Iguana%20Channel%20Restoration%20Failure> email us</a>. </p>");
         m_Page.Popup.dialog("option", "title", "Error");
      }
      m_Page.Popup.dialog("option", "buttons", {
         Close: function() {
            $(this).dialog("close");
         }
      });
      if( ! m_Page.Popup.dialog("isOpen") ){  m_Page.Popup.dialog("open");  }
   }

   function approvalDialogClosed() {
      console.log("Cleaning up.");
      if (m_Page.StatusInterval != 0) {
         clearInterval(m_Page.StatusInterval);
         m_Page.StatusInterval = 0;
      }
      m_Page.ChannelCollisionMap = {};
      m_Page.FileCollisionMap    = {};
      m_Me.showDeletedChannels();
   }

   function openRestoringSpinnerDialog(){
      var Content = "<p>Restoring Channel...<img src='jqueryFileTree/images/spinner.gif'> </p>";
      initPopup(Content);
      m_Page.Popup.dialog("option", "buttons", {});
      m_Page.Popup.dialog("option", "title", "Restore Channel");
   }
   // END : Dialog handlers



   // BEGIN : DOM creation functions
   function buildScreen() {
      var Screen = 
        '<div id="SCMcommitList"></div>\
         <div id="SCMreviewPanelAndPreview">\
            <div id="SCMpanel"></div>\
            <div id="SCMpreviewAndControls">\
               <div id="SCMpreviewTank">\
                  <div class="preview_toolbar">\
                     <div class="toolbar_filename"></div>\
                  </div>\
                  <div id="SCMpreview"></div>\
               </div>\
            </div>\
         </div>\
         <div id="SCMreviewControls">\
            <div class="review-actions">\
               <span id="SCMrestoreButtonContainer">\
                  <a id="SCMrestore" style="display: inline" class="action-button green btn-large">Restore</a>\
                  <span style="margin: 0 10px; color: #888888;">or</span>\
               </span>\
               <a id="SCMcancel" style="color: #666666; font-size: 14px;">Cancel</a></div>\
         </div>\
      ';

      $("#HistoryBody").html(Screen);

      SCMfixPreviewView();

      // $(window).resize(function() {
      //    $("#HistoryBody #SCMreviewPanelAndPreview").resizable("option", "maxHeight", $("body").height() - 250);
      //    $("#HistoryBody #SCMpanel").resizable("option", "maxWidth", $("body").width() / 3);
      //    SCMfixPreviewView();
      // });

      $("#SCMreviewPanelAndPreview").resizable({
         maxWidth: "100%",
         minWidth: "100%",
         minHeight: 300,
         maxHeight:  $("body").height() - 350,
         handles: "n"
      });

      // $("#HistoryBody #SCMreviewPanelAndPreview").on("resize", function(event, ui) {
      //    var CommitList = $("#HistoryBody #SCMcommitList");
      //    CommitList.css("height", (ui.position.top) + "px");
      //    SCMfixPreviewView();
      // });

      var Panel = $("#HistoryBody #SCMpanel");

      Panel.resizable({
         maxHeight: "100%",
         minHeight: "100%",
         minWidth : 180,
         maxWidth : $("body").width() / 3,
         handles: "e"
      });

      Panel.on("resize", function(event, ui) {
         event.stopPropagation();
         $("#HistoryBody #SCMpreviewAndControls").css("left", ui.size.width + "px");
      });
   }

   function displayCurrentFile(){
      console.log("Loading contents of file: " + m_Page.CurrentFile);
      var HtmlFileContents = m_Page.Files[m_Page.CurrentFile];
      $("#SCMpreviewTank .toolbar_filename").html( SCMchannelConfigNameClip(m_Page.CurrentFile) );
      $("#SCMpreviewTank #SCMpreview").html(HtmlFileContents); //Don't escape, wan't to render as html.
      $("#SCMpreviewTank #SCMpreview").css("overflow", "auto");
      $("#SCMpreviewTank #SCMpreview .previewTable").css("border", "none");
   }

   function displayInvalidRestoreCommitWarning() {
      $("#SCMpreviewTank .toolbar_filename").html("Invalid Restore Commit.");
      var Message = "<div id='invalidRestoreCommitMessage'>" +
                        "<p>There was an error loading the configuration files in the selected restore commit.</p>" +
                        "<p>Unfortunately it will not be possible to restore the channel(s) from this deletion.</p>" +
                     "</div>";
      $("#SCMpreviewTank #SCMpreview").html(Message);
   }

   function buildChannelFileRows() {
      var TheRows = '';
      for(var File in m_Page.Files){
         console.log(File);
         var Classes = (File == m_Page.CurrentFile ? ' c' : '');
         TheRows += '<li><label class="file' + Classes + '"><span class="select">' + File + '</label></li>';
      }
      return TheRows;
   }

   function buildChannelNameRows() {
      var ChannelNameRows = '';
      for(var i = 0; i < m_Page.Channel_Names.length; i++){
         var ChannelName = m_Page.Channel_Names[i];
         console.log(ChannelName);
         ChannelNameRows += '<li><label class="file"><span class="select">' + ChannelName + '</label></li>';
      }
      return ChannelNameRows;
   }

   function buildChannelFilesList() {
      var Panel = $("#SCMpanel");
      $("ul.tree").remove();

      if (m_Page.Channel_Names && m_Page.Channel_Names.length) {
         var ChannelLabel = "Channel";
         if (m_Page.Channel_Names.length > 1) {
            ChannelLabel += "s"
         }
         var ChannelNamesBox = 
            $('<ul id="restoreChannelNames" class="tree">\
               <li><label>' + ChannelLabel + ':</label></li>'
               + buildChannelNameRows()
               + '</ul>\
            ');
         Panel.append(ChannelNamesBox);   
      }

      var FileBox = 
         $('<ul class="tree">\
            <li><label>Files:</label></li>'
            + buildChannelFileRows()
            + '</ul>\
         ');

      Panel.append(FileBox);
      var CurrentSelectedFile = $("label.file.c")[0];
      CurrentSelectedFile.scrollIntoView();
      
      FileBox.find("label.file").click(function(event) {
         m_Page.CurrentFile = $(this).text();
         $("label.file").removeClass("c"); //Reset selected element.
         $(this).addClass("c");
         displayCurrentFile();
      });

      Panel.scrollLeft(0);
      Panel.scrollTop(0);
   }

   function buildBulkDeleteConfirmationMessage() {
      if( ! m_Page.Channel_Names || m_Page.Channel_Names.length === 0){
         // There must have been an error retrieving the channel names.
         return "<p>Restore bulk deletion channels?</p>"
      }
      var ConfirmationMessage = "<p>The following bulk deletion channels will be restored:</p>";
      ConfirmationMessage += "<ul>";
      for(var i = 0; i < m_Page.Channel_Names.length; i++){
         ConfirmationMessage += "<li>" + m_Page.Channel_Names[i] + "</li>";
      }
      ConfirmationMessage += "</ul>";
      return ConfirmationMessage;
   }

   function addDeletedChannelTableControls() {
      $("#SCMcancel").click(function(event) {
         disableKeys();
         cancel();
      });
      $("#SCMrestore").click(function(event) {
         var ConfirmationMessage;
         if( currentCommitIsBulkDeletion() ){
            ConfirmationMessage = buildBulkDeleteConfirmationMessage();
         }
         else{
            ConfirmationMessage = "<p>Restore channel \"" + currentCommitName() + "\"?</p>";
         }

         initPopup(popupCustomContent(ConfirmationMessage));

         m_Page.Popup.dialog("option", "buttons", {
            Continue: function() {
               restoreChannel();
            },
            Cancel  : function() {
               $(this).dialog("close");
               m_Me.showDeletedChannels();
            }
         });
      });
   }

   function buildCommitRow(Index) {
      var OneCommit = m_Page.Commits[Index];
      var ThisDate = new Date(OneCommit.commit_timestamp * 1000);
      var Current = (OneCommit.commit_id == currentCommitId() ? ' current' : '');
      var DisplayDate = ThisDate.toLocaleDateString() + ' ' + ThisDate.toLocaleTimeString();
      return '<tr data-index="' + Index 
           + '" class="commitRow' + Current
           + '"><td class="note">' + OneCommit.commit_comment
           + '</td><td class="commit_id">' + OneCommit.commit_id
           + '</td><td class="user">' + OneCommit.commit_user 
           + '</td><td class="date">' + DisplayDate 
           + '</td></tr>';
   }

   function buildDeletedChannelCommitTable(Container) {
      var TheTable = '<div class="fixed">\
                      <table id="commit_list" class="commitTable fixedHead"><thead><tr>\
                      <th class="note">Channel Deletion Commits</th>\
                      <th class="commit_id">Commit ID</th>\
                      <th class="user">Removed By</th>\
                      <th class="date">Date Removed</th></tr></thead></table>\
                      </div>\
                      <div class="scroller"><table class="commitTable"><tbody>';
      for (var i = m_Page.Commits.length - 1; i > -1; i--) {
         TheTable += buildCommitRow(i);
      }

      TheTable += '</tbody></table></div>'
      Container.html(TheTable);

      function stopKeys() {
         SCMreEnableMouse();
      }

      $("body").click(stopKeys);
      $(document).mousemove(function(event) {
         if(window.lastX !== event.clientX || window.lastY !== event.clientY){
            stopKeys();
         }   
         window.lastX = event.clientX;
         window.lastY = event.clientY;
      });

      enableKeysAndRefresh();

      $("tr.commitRow").click(function() {
         var ClickedRow = $(this);
         $("tr.commitRow.current").removeClass("current");
         m_Page.CurrentCommitIndex = ClickedRow.attr("data-index");
         ClickedRow.addClass("current");
         enableKeysAndRefresh();
         getDeletedChannelDetails();
      });
      
      SCMfixPreviewView();
   }

   function buildCollisionConfirmationHeader() {
      var NumOfChannelCollisions = numberOfKeys(m_Page.ChannelCollisionMap);
      var NumOfFileCollisions = numberOfKeys(m_Page.FileCollisionMap);
      var isBulkRestore = currentCommitIsBulkDeletion();
      var Header = "<p>";

      if (NumOfFileCollisions == 1 && NumOfChannelCollisions == 1) {
         Header = "The channel you are trying to restore below has the same name as a channel that already exists. " +
                  "It also uses a different version of a shared file that already exists on this instance. ";
      }
      else if(NumOfFileCollisions == 1 && NumOfChannelCollisions > 1) {
         Header = "The channels you are trying to restore below have the same names as channels that already exist. " +
                  "They also use a different version of a shared file that already exists on this instance. ";  
      }
      else if(NumOfFileCollisions > 1 && NumOfChannelCollisions == 1) {
         Header = "The channel you are trying to restore below has the same name as a channel that already exists. " +
                  "It also uses a different versions of shared files that exist already on this instance. ";  
      }
      else if (NumOfFileCollisions > 1 && NumOfChannelCollisions > 1) {
         Header = "The channels you are trying to restore below have the same names as channels that already exist. " +
                  "They also use different versions of shared files that exist already on this instance. ";  
      }
      else if(NumOfFileCollisions == 1 && isBulkRestore) {
         Header = "The channels you are trying to restore use a different version of a shared file that already exists on this instance.";
      }
      else if(NumOfFileCollisions == 1) {
         Header = "The channel you are trying to restore uses a different version of a shared file that already exists on this instance.";
      }
      else if(NumOfFileCollisions > 1 && isBulkRestore) {
         Header = "The channels you are trying to restore use different versions of shared files that exist already on this instance.";
      }
      else if(NumOfFileCollisions > 1) {
         Header = "The channel you are trying to restore uses different versions of shared files that exist already on this instance.";
      }
      else if(NumOfChannelCollisions == 1) {
         Header = "The channel you are trying to restore below has the same name as a channel that already exists.";
      }
      else if (NumOfChannelCollisions > 1) {
         Header = "The channels you are trying to restore below have the same names as already existing channels.";
      }

      Header += "</p><p>Please make a decision on how to proceed.</p>";
      return Header;
   }

   // END : DOM Creation functions ----------------------------




   // BEGIN : Collison confirmation utilities
   function validateChannelCollisionConfirmations(){
      if (Object.keys(m_Page.ChannelCollisionMap).length > 0) {
         var ChannelSelectBoxes = $("#collision_list .CollisionChannel select.dropdown");
         SCMclearNewChannelNameWarnings(ChannelSelectBoxes);
         var ValidationFailed = SCMvalidateAndStoreNewChannelNames(m_Page, ChannelSelectBoxes, "restore"); 
         if(ValidationFailed){
            return true;
         }
      }
      return false;
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

   function displayChannelRestoreCollisionConfirmations(Response) {
      console.log(Response);
      m_Page.ChannelCollisionMap = Response['restore_summary'].channel_collision_confirmations     || {};
      m_Page.FileCollisionMap    = Response['restore_summary'].overwrite_shared_file_confirmations || {};
      buildLocalGuidToNameMap();
      removeFileDiffsFromConfirmationMap();

      $("#ApprovalDlg").remove();
      $("body").append('<div id="ApprovalDlg"><form id="ApprovalForm" /></div>');
      var ApprovalDlg = $("#ApprovalDlg");
      var DlgButtons = [
         {
            id: "confirmation-button-continue",
            text: "Continue",
            click: function() {
               var InputErrors = validateChannelCollisionConfirmations();
               if( ! InputErrors ){
                  restoreChannel();
               }
            }
         },
         {
            id: "confirmation-button-cancel",
            text: 'Cancel Restore',
            click: function() {
               enableKeysAndRefresh();
               $(this).dialog('close');
            }
         }
      ];

      var Header = buildCollisionConfirmationHeader();
      
      var ChannelCollisionTable = '';
      if (Object.keys(m_Page.ChannelCollisionMap).length > 0) {
            ChannelCollisionTable += SCMbuildChannelCollisionTable(true, "Restore", m_Page.ChannelCollisionMap);
      }

      var FileCollisionTable = '';
      if (Object.keys(m_Page.FileCollisionMap).length > 0) {
         console.log(m_Page.FileCollisionMap);
         FileCollisionTable = SCMbuildFileCollisionTable("Restore", m_Page.FileCollisionMap);
      }

      ApprovalDlg.find("#ApprovalForm").html(Header + ChannelCollisionTable + FileCollisionTable);

      var DialogWidth = $(window).width() * 0.85;
      var DialogMaxHeight = $(window).height() * 0.9;

      ApprovalDlg.dialog({
         bgiframe:  true,
         width:     DialogWidth,
         maxHeight: DialogMaxHeight,
         title:     'Channel Collison Confirmations',
         modal:     true,
         autoOpen:  false,
         buttons:   DlgButtons,
         resizable: true,
         close:     approvalDialogClosed
      });
      
      var ChannelSelectBoxes = $("#collision_list .CollisionChannel select.dropdown");
      var CollidingFileCheckBoxes = $("#bump_file_list input.bumpUpdate");
      
      SCMbumpTickCheck(CollidingFileCheckBoxes, $("#bump_file_list input#BumpAll"), "", "filepath");
      
      CollidingFileCheckBoxes.on("change", function(event) {
         SCMupdateCollidingFileApprovals($(this), m_Page.FileCollisionMap);
         SCMcheckIfContinueButtonNeedsDisabling(ChannelSelectBoxes, CollidingFileCheckBoxes);
      });
      
      SCMremoveInvalidChannelSelectOptions(m_Page, ChannelSelectBoxes);
      SCMdisplayUpdateWarningMessageIfNeeded("restore", m_Page.NameOnlyCollisionsPresent);
      SCMsetSelectBoxChangeEvents(m_Page, ChannelSelectBoxes, $("#collision_list select#bulk_action"), "cguid", "restore");

      function continueButtonNeedsDisablingEvent() {
         SCMcheckIfContinueButtonNeedsDisabling(ChannelSelectBoxes, CollidingFileCheckBoxes);
      }

      ChannelSelectBoxes.on("change", continueButtonNeedsDisablingEvent);
      continueButtonNeedsDisablingEvent();
      
      $("#diffLink").click(function() {
         ChannelSelectBoxes.off("change", continueButtonNeedsDisablingEvent); // Need to re-attach since file boxes will be recreated.
         SCMbuildCollidingFileDiffView(m_Page, "Restore", getCollatedProjects, addContinueButtonAction, closeCollisionDialog);
      });


      m_Page.Popup.dialog("close");
      ApprovalDlg.dialog("open");

      if (document.location.pathname == '/settings') {
         SCMpullStatus(m_Page.StatusMap, SCM_BUMP_NOTE);
         m_Page.StatusInterval = setInterval( function() {
            SCMcheckChannelLights(m_Page.StatusMap, SCM_BUMP_NOTE);
         }, 1000);
      }
   }

   function addContinueButtonAction(){
      $("#SCMgo").click(function(event) {
         var InputErrors = validateChannelCollisionConfirmations();
         if( ! InputErrors ){
            restoreChannel();
         }
      });
   }

   function closeCollisionDialog() {
      console.log("Closing dialog. Resetting collision data...");
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
      m_Me.showDeletedChannels();
      
   }
   // END : Channel/Shared File collison confirmation utilities



   // BEGIN : AJAX callbacks
   function handleDeletedChannelListError(RequestObject, Status, ErrorString, ReqVars) {
      console.log("handleDeletedChannelListError");
      console.log(RequestObject);
      console.log(Status);
      console.log(ErrorString);
      console.log(ReqVars);
      if(Status == "abort"){ return; } //if we abort it isn't an error.
      m_Page.Popup.dialog("close");
      var ErrorMessage = RequestObject.responseJSON && RequestObject.responseJSON.error 
                      ? RequestObject.responseJSON.error.description
                      : RequestObject.status + " - " + RequestObject.statusText;
      var Message = "<p> We're sorry, but the request could not be completed. The following error occurred: </p>" + 
                    "<p>" + ErrorMessage + "</p>";
      hideCommitsLoadingWheel();
      finalizePopupDisplay(Message, true);
   }

   function buildRestoreSuccessMessage(RestoredChannels){
      if( ! RestoredChannels || RestoredChannels.length === 0 ){
         return "<p>The channel(s) has been successfully restored!</p><p><a href='/'>View the channel(s)</a></p>";
      }
      else if(RestoredChannels.length === 1){
         return "<p>The channel \"" + RestoredChannels[0] + "\" has been successfully restored!</p><p><a href='/'>View the channel</a></p>";
      }
      else{
         var SuccessMessage = "<p>The following channel(s) have been successfully restored: </p><ul>";
         for(var i = 0; i < RestoredChannels.length; i++){
            SuccessMessage += "<li>" + RestoredChannels[i] + "</li>";
         }
         SuccessMessage += "</ul><p><a href='/'>View the channel(s)</a></p>";
         return SuccessMessage;
      }
   }

   function handleChannelRestoreSuccess(Response){
      console.log("Restore success!");
      console.log(Response);
      
      if (Response['success']) {
         var SuccessMessage = buildRestoreSuccessMessage( Response["result_channels"] );
         enableKeysAndRefresh();
         finalizePopupDisplay(SuccessMessage);
         $("#ApprovalDlg").remove(); //Might be open.
         approvalDialogClosed();
         closeCollisionDialog();
         return;
      }

      disableKeys();
      displayChannelRestoreCollisionConfirmations(Response);
   }

   function handleDeletedChannelCommitListSuccess(Response){
      var List = $("#HistoryBody #SCMcommitList");
      List.html("");
      console.log(Response);
      m_Page.Commits = Response;
      console.log(m_Page.Commits);
      m_Page.CurrentCommitIndex = m_Page.Commits.length - 1;
      hideCommitsLoadingWheel();
      
      if( ! m_Page.Commits.length ){
         displayWarning("There are no deleted channels under source control that can be restored at this time.");
         return;
      }

      buildDeletedChannelCommitTable(List);
      addDeletedChannelTableControls();
      getDeletedChannelDetails();
   }

   function handleDeletedChannelDetailsSuccess(Response){
      console.log(Response);
      m_Page.Files         = Response.files;
      m_Page.Channel_Names = Response.channels;
      hideFilesLoadingWheel();
      resetCurrentFile();
      
      if (m_Page.Channel_Names.length !== 0) {
         buildChannelFilesList();
         displayCurrentFile();   
      }
      else {
         console.log("Error loading channel name(s) from deleted files. Don't allow restore for this commit.");
         displayInvalidRestoreCommitWarning();
         hideRestoreButton();
      }

      hideFileReviewLoadingWheel();
   }

   function getCollatedProjectsSuccess(Response){
      console.log(Response);
      m_Page.CollationMap = Response;
      SCMloadCollidingFileDiffViewData(m_Page, "restore");
      SCMattachDisableActionsToDiffView();
      SCMdoFileDiff(m_Page);
      SCMfixCollidingSharedFileView();
   }
   // END : AJAX callbacks

   function restoreEscapedHtmlString(Text) {
      var RestoredString = Text.replace(/&amp;/g, "&")
                  .replace(/&lt;/g, "<")
                  .replace(/&gt;/g, ">")
                  .replace(/&quot;/g, "\"");
      return RestoredString;
   }

   // Begin : AJAX calls 
   function restoreChannel(){
      removePopup();
      openRestoringSpinnerDialog();
      SCMclearActiveRequest(m_Page.CurrentRequest);
      var isBulkDeleteCommit = currentCommitIsBulkDeletion();

      var ReqParams = {
         'files'                 : Object.keys(m_Page.Files),
         'delete_commit_id'      : currentCommitId(),
         'bulk_deletion_restore' : isBulkDeleteCommit
      }

      if( ! isBulkDeleteCommit ){
         //If single channel, add the name.
         ReqParams.channel_name = restoreEscapedHtmlString( currentCommitName() )
      }

      if( Object.keys(m_Page.ChannelCollisionMap).length ){
         ReqParams["channel_collision_confirmations"] = JSON.stringify(m_Page.ChannelCollisionMap);
      }

      if( Object.keys(m_Page.FileCollisionMap).length ){
         ReqParams["overwrite_shared_file_confirmations"] = JSON.stringify(m_Page.FileCollisionMap);
      }

      m_Page.CurrentRequest = $.ajax({
         url    : "/restore_selected_channel",
         method : "POST",
         data   : ReqParams,
         traditional : true, // doesn't append [] to "files" key.
         success : handleChannelRestoreSuccess,
         error  : handleDeletedChannelListError
      });
   }

   function getDeletedChannelDetails() {
      showRestoreButton();
      showFilesLoadingWheel();
      showFileReviewLoadingWheel();
      SCMclearActiveRequest(m_Page.CurrentRequest);
      m_Page.CurrentRequest = $.ajax({
         url:  '/sc/deleted_channel_files',
         data: { 'delete_commit_id': currentCommitId() },
         method : "GET",
         success: handleDeletedChannelDetailsSuccess,
         error  : handleDeletedChannelListError
      });
   }

   function getDeletedChannelCommitList(){
      showCommitsLoadingWheel();
      SCMclearActiveRequest(m_Page.CurrentRequest);
      m_Page.CurrentRequest = $.ajax({
         url    : "/sc/view_deleted_channels",
         method : "GET",
         data   : {},
         success: handleDeletedChannelCommitListSuccess,
         error  : handleDeletedChannelListError
      });
   }

   function getCollatedProjects() {
      var ErrorHandler = function(RequestObject, Status, ErrorString){
         m_Page.Popup.dialog("close");
         handleDeletedChannelListError(RequestObject, Status, ErrorString, Params);
      }
      $.ajax({
         url: '/sc/collate',
         method: 'POST',
         data: {
            file_paths: JSON.stringify(m_Page.FileCollisionMap)
         },
         success: getCollatedProjectsSuccess,
         error  : ErrorHandler
      });
   }
   //END : AJAX calls



   // Public API
   this.showDeletedChannels = function(){
      buildScreen();
      getDeletedChannelCommitList();
   }
}

function setupChannelRestoreManager(){
   console.log("Initalizing Restore Manager.");
   ifware = window.ifware || {};
   ifware.ChannelRestoreManager = new ChannelRestoreManager();
}

setupChannelRestoreManager();