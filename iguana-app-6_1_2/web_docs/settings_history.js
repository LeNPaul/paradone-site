/** 
 * Copyright (c) 2015 iNTERFACEWARE Inc.  All rights reserved.
 */

function SCMsettingsReview() {
   ifware.SettingsReviewer = new SCMsettingsReviewer();
   ifware.SettingsReviewer.init();
}

/**
 * @constructor
 */
var SCMsettingsReviewer = function(Params) {
   var m_Diffs               = SCMinitialDiffs();
   var m_DiffFormat          = "vertical";
   var m_Commits             = [];
   var m_CurrentFile         = "IguanaConfiguration.xml";
   var m_CurrentCommitIndex  = 0;
   var m_Files               = [];
   var m_Mode                = "changes";
   var m_Popup               = "";
   var m_Me                  = this;
   var m_DetailedView        = false;

   var POPUP_PARAMS = {
      modal: true,
      width: 400,
      title: "Revert to previous settings",
      resizable: false,
      close: function() {
         $(this).remove();
      }
   };

   // BEGIN: Methods that make HTTP API calls
   function review() {
      var Screen = 
        '<div id="SCMcommitList"></div>\
         <div id="SCMreviewPanelAndDiff">\
         <div id="SCMpanel"></div>\
         <div id="SCMdiffAndControls">\
         <div id="SCMdiffTank">\
         <div class="diff_toolbar">\
            <div class="toolbar_filename"></div>\
            <div class="toolbar_lines"></div>\
            <div class="toolbar_display"><div class="select-style" style="float: right;">\
               <select id="SCMdiffViewSelect">\
               <option value="vertical">Side by Side</option>\
               <option value="horizontal">Horizontal</option>\
               <option value="condensed">Changes Only</option>\
               </select>\
               <select id="SCMcomparisonSelect">\
               <option value="changes">What changed</option>\
               <option value="current">Preview revert</option>\
               </select></div>\
            </div>\
          <!--/diff_toolbar--></div>\
         <div id="SCMdiff"></div>\
         <!--/SCMdiffTank--></div>\
         <!--/SCMdiffAndControls--></div>\
         <!--/SCMreviewPanelAndDiff--></div>\
         <div id="SCMreviewControls">\
         <div class="review-actions"><a id="SCMrevert" style="display: inline" class="action-button green btn-large">Revert</a> <span style="margin: 0 10px; color: #888888;">or</span> <a id="SCMcancel" style="color: #666666; font-size: 14px;">Cancel</a></div>\
         <!--/SCMreviewControls--></div>\
      ';
      $("#HistoryBody").html(Screen);
      SCMfixDiffView();
      $(window).resize(function() {
         $("#SCMreviewPanelAndDiff").resizable("option", "maxHeight", $("body").height() - 250);
         $("#SCMpanel").resizable("option", "maxWidth", $("body").width() / 3);
         SCMfixDiffView();
      });

      $("#SCMreviewPanelAndDiff").resizable({
         maxWidth: "100%",
         minWidth: "100%",
         minHeight: 300,
         maxHeight:  $("body").height() - 350,
         handles: "n"
      });
      $("#SCMreviewPanelAndDiff").on("resize", function(event, ui) {
         var CommitList = $("#SCMcommitList");
         CommitList.css("height", (ui.position.top) + "px");
         SCMfixDiffView();
      });
      var Panel = $("#SCMpanel");
      Panel.resizable({
         maxHeight: "100%",
         minHeight: "100%",
         minWidth : 180,
         maxWidth : $("body").width() / 3,
         handles: "e"
      });
      Panel.on("resize", function(event, ui) {
         event.stopPropagation();
         $("#SCMdiffAndControls").css("left", ui.size.width + "px");
      });
      if( !$("#view_mode").length ){
         addDetailedViewCheckbox();
      }   
      getHistory();
   }

   function getHistory(){
      var Params = {
         "detailed" : m_DetailedView
      } 
      var CommitList = $("#SCMcommitList");
      CommitList.html("").addClass("SCMloading");
      $.ajax({
         url :     '/sc/config_history',
         cache: false,
         data : Params,
         success: function(Response) {
            CommitList.html("").removeClass("SCMloading");
            m_Commits = Response;
            console.log(m_Commits);
            m_CurrentCommitIndex = m_Commits.length - 1;
            buildCommitTable(CommitList);
            checkRevertable(); 
            addControls();
            pullDetails();

         },
         error : function(RequestObject, Status, ErrorString){
            SCerror(RequestObject, Status, ErrorString);
         }
      });
   }

   function diff() {
      if(m_Files.length === 0){
         $("#SCMdiff").html('<div class="noDiff" style="font-weight:bold">No files changed in this commit.</div>');
         m_Diffs = {};
         $(".toolbar_filename").html("");
         $(".toolbar_lines").html("");
         return;
      }
      $(".toolbar_filename").html(SCMchannelConfigNameClip(m_CurrentFile, 30));
      var DiffParams = {
         'file_path': m_CurrentFile
      };
      if (m_Mode == "changes") {
         DiffParams.commit_from = parentId() || "first";
         DiffParams.commit_to   = commitId();
      } else {
         DiffParams.commit_from = "null";
         DiffParams.commit_to   = commitId(); 
      }
      console.log(DiffParams);
      $.ajax({
         url    : '/sc/diff',
         data   : DiffParams,
         success: function(Response) {
            m_Diffs = Response.diff;
            console.log(m_Diffs);
            SCMupdateLegend(m_Diffs, $("#SCMreviewControls"));
            if (m_Diffs["status"] && ! m_Diffs["summary"]) {
               $("#SCMdiff").html('<div class="noDiff">(<span>' + m_Diffs["status"] + '</span>)</div>');
               return;
            }
            SCMdrawDiffs(m_Diffs, m_DiffFormat);
            updateStatus();
         },
         error  : function(RequestObject, Status, ErrorString){
            SCerror(RequestObject, Status, ErrorString);
         }
      }); 
   }

   function pullDetails() {
      $.ajax({
         url:  '/sc/files_from_commit_simple',
         data: { 'commit_id': commitId() },
         success: function(Response) {
            m_Files = Response.files;
            resetCurrentFile();
            refreshFileBox();
         },
         error  : function(RequestObject, Status, ErrorString){
            SCerror(RequestObject, Status, ErrorString);
         }
      });
   }

   function revert(Vars) {
      var RevertParams = Vars || {};
      RevertParams.sc_task          = 'iguana_revert';
      RevertParams.revert_to_commit = commitId();
      m_Popup.dialog("option", "buttons", {
         Cancel  : function() {
            $(this).dialog("close");
            cancel();
         }
      });
      m_Popup.html("<p>Reverting...<img src='jqueryFileTree/images/spinner.gif'> </p>");
      console.log(RevertParams);
      $.ajax({
         url    : '/revert',
         data   : RevertParams,
         success: function(Response) {
            revertSuccess(Response);
         },
         error  : function(RequestObject, Status, ErrorString){
            revertError(RequestObject, Status, ErrorString, RevertParams);
         }
      });
   }
   // END: Methods that make HTTP API calls

   // BEGIN: Utilities
   function seekApproval(Confirmations) {
      var ServiceList = Object.keys(Confirmations).map(function(One) {
         return "<strong>" + One + " server</strong>";
      }).join(", ");
      m_Popup.html("This revert operation will restart the following services:<br><br>" + ServiceList);
      m_Popup.dialog("option", "buttons", {
         "Approve and continue": function() {
            for (var Conf in Confirmations) {
               if (! Confirmations.hasOwnProperty(Conf)) { continue; }
               Confirmations[Conf] = true;
            }
            var Approvals = {"server_restart_confirmations": JSON.stringify(Confirmations)};
            $(".ui-dialog-buttonpane button").button("disable");
            revert(Approvals);
         },
         "Cancel": function() {
            m_Popup.dialog("close");
         }
      });
   }

   function revertSuccess(Response){
      console.log(Response);
      if (! Response.success && Response.server_restart_confirmations) {
         seekApproval(Response.server_restart_confirmations);
         return;
      }
      //In case web server port changed.
      var DashboardUrl = Response.dashboard_url || "/dashboard.html";
      console.log(DashboardUrl);
      m_Popup.html("<p>Revert Success!</p>");
      m_Popup.dialog("option", "buttons", {
         "Return to dashboard": function() {
            window.location = DashboardUrl;
         },
         "Close": function() {
            m_Popup.dialog("close");
            review();
         }
      });
   }

   function revertError(RequestObject, Status, ErrorString, RevertRequestParams){
      console.log(RequestObject);
      console.log(Status);
      console.log(ErrorString);
      console.log(RevertRequestParams);
      var ErrorMessage = RequestObject.responseJSON.error
                         ? RequestObject.responseJSON.error.description
                         : RequestObject.status + " - " + RequestObject.statusText;
      var Message = "<p> We're sorry, but the request could not be completed.\
                     The following error occurred: </p>\
                    <p>" + ErrorMessage + "</p>\ ";
      var PopupButtons = {
         "Close" : function() {
            m_Popup.dialog("close");
         } 
      }
      if( ! isPopupInitalized() ){
         initPopup();
      }
      m_Popup.html(Message);
      m_Popup.dialog("option", "title", "Revert Error");
      m_Popup.dialog("option", "buttons", PopupButtons);
   }

   function SCerror(RequestObject, Status, ErrorString) {
      if (RequestObject.status && RequestObject.status == 403) {
         return; // MiniLogin will handle it.
      }

      ErrorMessage = RequestObject.responseJSON.error
                   ? RequestObject.responseJSON.error.description
                   : RequestObject.status + " - " + RequestObject.statusText;

      var Message = "<p> We're sorry, but an unexpected error occurred: </p>\
                    <p>" + ErrorMessage + "</p>\ ";
      if( ! isPopupInitalized() ){
         initPopup();
      }
      m_Popup.html(Message);
      m_Popup.dialog("option", "title", "Error");
      m_Popup.dialog("option", "close", function() {
         window.location = "/dashboard.html";
      });
      m_Popup.dialog("option", "buttons", {
         "Return to dashboard": function() {  m_Popup.dialog("close");  }
      });
   }

   function commitId() {
      return m_Commits[m_CurrentCommitIndex].commit_id;
   }
   
   function currentCommit() {
      return m_Commits[m_CurrentCommitIndex];
   }

   function parentId() {
      return m_Commits[m_CurrentCommitIndex].commit_parent_id;
   }
   function resetCurrentFile() {
      for (var i = 0; i < m_Files.length; i++) {
         if (m_CurrentFile == m_Files[i]) {
            return;
         }
      }
      m_CurrentFile = m_Files[0];
      
   }

   function removePopup() {
      $("#history-popup").remove();
   }

   function isPopupInitalized(){
      return $("#history-popup").length > 0;
   }

   function initPopup(Filler) {
      removePopup();
      $("body").append("<div id='history-popup'><img src='jqueryFileTree/images/spinner.gif' style='display: none;'></div>");
      m_Popup = $("#history-popup");
      m_Popup.prepend(Filler);
      m_Popup.dialog(POPUP_PARAMS);
      $(window).resize();
   }
   // END: Utilities

   // BEGIN: Methods that handle drawing and styling
   function addDetailedViewCheckbox(){
      var DetailedViewUI = '<span style="position: absolute; right: 25px; top: 6px">' +
                              '<span class="select-style">' +
                                 '<select id="view_mode" class="dropdown">' +
                                    '<option class="dropdown_opt" value="normal">Normal View</option>' +
                                    '<option class="dropdown_opt" value="detailed">Detailed View</option>' +
                                 '</select>' +
                              '</span>' +
                           '</span>';
      $("#cookie_crumb").append(DetailedViewUI);

      $("#view_mode").change( function(){
         m_DetailedView = $(this).val() === "detailed";
         console.log("m_DetailedView is " + m_DetailedView);
         review();
      });
   }

   function checkRevertable() {
      if (m_CurrentCommitIndex == m_Commits.length - 1) {
         $("a#SCMrevert").hide();
         $(".review-actions span").hide();
      } else {
         $("a#SCMrevert").show();
         $(".review-actions span").show();
      }
   }

   function handleKeys() {
      SCMcommitKeys(m_CurrentCommitIndex, m_Commits.length - 1, function(Index) {
         var Increase = (Index > m_CurrentCommitIndex)
                        ? true
                        : false;
         m_CurrentCommitIndex = Index;
         $('tr.commitRow').removeClass("current");
         $('tr.commitRow[data-index="' + Index + '"]').addClass("current");
         console.log($('tr.commitRow.current').position());
         SCMscrollCommitList();
         checkRevertable();
         pullDetails();
      });
   }

   function buildCommitTable(Container) {
      var TheTable = '<div class="fixed">\
                      <table id="commit_list" class="commitTable fixedHead"><thead><tr>\
                      <th class="note">Note</th><th class="commit_id">Commit ID</th>\
                      <th class="user">User</th><th class="date">Date</th></tr></thead></table>\
                      </div>\
                      <div class="scroller"><table class="commitTable"><tbody>';
      for (var i = m_Commits.length - 1; i > -1; i--) {
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
      handleKeys();
      $("tr.commitRow").click(function() {
         var ClickedRow = $(this);
         $("tr.commitRow.current").removeClass("current");
         m_CurrentCommitIndex = ClickedRow.attr("data-index");
         ClickedRow.addClass("current");
         handleKeys();
         checkRevertable();
         pullDetails();
      });
      SCMfixDiffView();
   }

   function buildCommitRow(Index) {
      var OneCommit = m_Commits[Index];
      var ThisDate = new Date(OneCommit.commit_timestamp * 1000);
      var Current = (OneCommit.commit_id == commitId() ? ' current' : '');
      var DisplayDate = ThisDate.toLocaleDateString() + ' ' + ThisDate.toLocaleTimeString();
      return '<tr data-index="' + Index 
           + '" class="commitRow' + Current 
           + '"><td class="note">' + PAGhtmlEscape(OneCommit.commit_comment)
           + '</td><td class="commit_id">' + OneCommit.commit_id
           + '</td><td class="user">' + OneCommit.commit_user 
           + '</td><td class="date">' + DisplayDate 
           + '</td></tr>';
   }

   function refreshFileBox() {
      var FileBox = 
      $('<ul class="tree">\
         <li><label>Changed files:</label></li>'
         + buildFileRows()
         + '</ul>\
      ');
      $("ul.tree").remove();
      var Panel = $("#SCMpanel");
      Panel.append(FileBox);
      if(m_Files.length){
         $("label.file.c")[0].scrollIntoView();
         FileBox.find(".select").click(function(event) {
            m_CurrentFile = m_Files[$(this).attr("data-index")];
            pullDetails();
         });
      }
      Panel.scrollLeft(0);
      Panel.scrollTop(0);
      diff();
    }

   function buildFileRows() {
      var TheRows = '';
      for (var i = 0; i < m_Files.length; i++) {
         var ThisFile = m_Files[i];
         console.log(ThisFile);
         var Classes = (ThisFile == m_CurrentFile ? ' c' : '');
         TheRows += '<li><label class="file' + Classes + '"><span class="select" data-index="' + i + '">' + m_Files[i] + '</label></li>';
      } 
      return TheRows;
   }

   // function loadMoreCommits(){
   //    var CommitList = $("#SCMcommitList");
   //    var LoadSpan = "<span id='PaginateLoader' class='SCMloading' style='width: 100%'></span>";
   //    CommitList.append(LoadSpan);
   //    var LastCommit = 
   //    $.ajax({
   //       url : '/sc/config_history',
   //       data: {
   //          "limit" : m_PaginateSize
   //       },
   //       success: function(Response) {
   //          NewCommits = Response;
   //          m_Commits.push(NewCommits);
   //          m_CurrentCommitIndex = m_Commits.length - 1;
   //          buildCommitTable(CommitList);
   //          if( NewCommits.length < m_PaginateSize ){
   //             m_AllCommitsLoaded = true;
   //          }
   //       },
   //       error : function(RequestObject, Status, ErrorString){
   //          SCerror(RequestObject, Status, ErrorString);
   //       }
   //    });
   // }

   function addControls() {
      $("#SCMdiffViewSelect").change(function(event) {
         m_DiffFormat = $(this).val();
         SCMdrawDiffs(m_Diffs, m_DiffFormat);
      });
      $("#SCMcomparisonSelect").change(function(event) {
         m_Mode = $(this).val();
         diff();
      });
      $("#SCMcancel").click(function(event) {
         cancel();
      });
      $("#SCMrevert").click(function(event) {
         var Message = "Are you sure you want to revert to previous settings?" +
         "<br><br>All channels will be stopped, some channels may be added or removed, " +
         "uncommitted translator changes will be lost, any queued messages will be cleared, " +
         "and the service may need to be restarted."
         initPopup(Message);
         m_Popup.dialog("option", "buttons", {
            Continue: function() {
               revert();
            },
            Cancel  : function() {
               $(this).dialog("close");
            }
         });
      });

      // TODO - Add scroll action here for pagination
      // loadMoreCommits();
   }

   function updateStatus() {
      var Current = currentCommit();
      var StatusSpot = $("#SCMcommitInfo");
      var ModeText = (m_Mode == "changes"
                      ? 'Showing what changed.'
                      : 'Showing differences with current version.');
      var Details = '<span>' + Current.commit_id + '</span>. (' + ModeText + ')';
      StatusSpot.html(Details);

   }
   // END: Methods that handle drawing and styling

   function cancel() {
      document.location = '/settings';
   }

   // BEGIN: Public API
   this.init = function() {
      review();
   }
   this.exit = function() {
      cancel();
   }
   this.files = function() {
      return m_Files;
   }
   this.file = function() {
      return m_CurrentFile;
   }
   // END: Public API
}
