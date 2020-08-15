/** 
 * Copyright (c) 2010-2015 iNTERFACEWARE Inc.  All rights reserved.
 */

var SCMreplaceSampleStr = 'REPLACE_SAMPLE';
var SCMappendSampleStr = 'APPEND_SAMPLE';
var SCM_BUMP_NOTE = "Will stop channel"; // ALL CAPS is as close as we get to const

var SCM_POPUP_PARAMS = {
   modal     : true,
   minWidth  : 450,
   resizable : false,
   close     : function() {
      $(this).remove();
   }
};

if (!Array.prototype.indexOf) {
   Array.prototype.indexOf = function (obj, start) {
      for (var i = (start || 0), j = this.length; i < j; i++) {
         if (this[i] === obj) {
            return i;
         }
      }
      return -1;
   }
}

function SCMcommitDlgGetConflictingFiles(Editor, ProjectData) {
   SCMcheckConflicts(Editor, ProjectData, SCMcommitDlgUI);
}

function handleProjectImportError(RequestObject, Status, ErrorString){
   var ErrorMessage = RequestObject.responseJSON.error
                      ? RequestObject.responseJSON.error.description
                      : RequestObject.status + " - " + RequestObject.statusText;
   $(document).find("body").append($("<div id='scResponseDlg'><div class='sc_error'></div><br/></div>"));
   var ResponsePopup = $("#scResponseDlg");
   var DlgButtons = {};
   DlgButtons['Close'] = function () {
      $(this).dialog('close');
   }
   // TODO: properly dimension the dialog based on the parent
   ResponsePopup.dialog({
      bgiframe: true,
      width:    400,
      title:    "Error",
      modal:    true,
      autoOpen: false,
      buttons:  DlgButtons
   });
   ResponsePopup.find('.sc_error').text(ErrorMessage);
   ResponsePopup.find('.sc_error').show();
   ResponsePopup.dialog("open");
   return true;
}

function SCMclearDependencies(Editor) {
   var Params = {
      'translator_guid' : Editor.ifware.ChannelGuid
   };
   $.ajax({
      url    : "/sc/clear_dependencies",
      method : "POST",
      data   : Params,
      success: function(Response){
         SCMresponseDlgUI(Response, false);
      },
      error  : handleProjectImportError
   });
}

function SCMresponseDlgUI(Response, ShowInfo, Callback) {
   if ($('#scResponseDlg').length) {
      $('#scResponseDlg').remove();
   }
   if (typeof Callback == 'function') Callback();
   return false;
}

function SCMgetMainFileLabel(ProjectData) {
   for (var i = 0; i < ProjectData.length; ++i) {
      if (ProjectData[i].Type == "Main") {
         return (ProjectData[i].Label);
      }
   }
   return "";
}

// Returns labels, paths, or anything else that's in ProjectData
function SCMgetProjectList(ProjectData, KeyName) {
   var List = new Array();
   for (var i = 0; i < ProjectData.length; ++i) {
      List.push(ProjectData[i][KeyName]);
   }
   return List;
}

function SCMdialogWaiting(DialogSelector, BoolWaiting) {
   if (BoolWaiting) {
      $(DialogSelector).dialog("disable");
      $(DialogSelector).siblings('.ui-dialog-buttonpane').prepend('<img class="scSpinner" src="/js/mapper/images/spinner.gif" />');
   } else {
      $(DialogSelector).dialog("enable");
      $(DialogSelector).siblings('.ui-dialog-buttonpane').children('.scSpinner').remove();
   }
}

function SCMdialogConfirm(ConfirmText, Callback) {
   if ($('#fossilConfirmDlg').length) {
      $('#fossilConfirmDlg').remove();
   }
   $(document).find("body").append($("<div id='fossilConfirmDlg'>" + ConfirmText + "</div>"));

   var DlgButtons = {};
   DlgButtons['No'] = function () {
      $(this).dialog('close');
   }

   DlgButtons['Yes'] = function () {
      Callback();
      $(this).dialog('close');
   }

   // TODO: properly dimension the dialog based on the parent
   var PopupDialog = $("#fossilConfirmDlg");

   PopupDialog.dialog({
      bgiframe:   true,
      title:      "Confirmation Request",
      width:      500,
      modal:      true,
      autoOpen:   false,
      buttons:    DlgButtons
   });
   PopupDialog.dialog("open");
}

function SCMrevertConfirmText() {
   return "Revert operation might overwrite your changes. Are you sure you want to revert?";
}

function SCMactivateUploadedZip(Editor, FileName) {
   var Params = {
      'translator_guid'   : Editor.ifware.ChannelGuid, 
      'uploaded_filename' : FileName, 
      'data_import_type'  : Editor.ifware.DataImportType
   };

   function importFromZipSuccess(Response){
      if (Editor.ifware.DataImportType == SCMreplaceSampleStr) {
         Editor.ifware.setCountOfSample(Response.CountOfSampleData);
         Editor.ifware.setSampleIndex(1);
         NISchangeSampleData(Editor, 1);
      } else if (Editor.ifware.DataImportType == SCMappendSampleStr) {
         Editor.ifware.setCountOfSample(Editor.ifware.countOfSample() + Response.CountOfSampleData);
      }
      NISupdateSampleDataControl(Editor);
      NISreloadCurrentFile(Editor);
   }

   $.ajax({
      url    : "/sc/import_from_zip",
      method : "POST",
      data   : Params,
      error  : handleProjectImportError,
      success: importFromZipSuccess
   });
}

function SCMuploadFile(Dialog, Form, fileInput, Editor, ProjectManager) {
   ProjectManager.handleProjectUpload(Form, Dialog, fileInput);
}

function SCMenableImportBTN(Dialog, enable) {
   Dialog.dialog('widget').find('button:contains("Import")').button(enable);
}

function SCMimportProjectAndData(Editor, ProjectManager) {
   console.log("SCMimportProjectAndData");
   console.log(ProjectManager);
   var DialogId = 'importProjectAndData';
   $('#' + DialogId).remove();
   $(document).find("body")
      .append('<div id="' + DialogId + '">' +
      '<form id="importForm">' +
      '<table>' +
      '<tr><td valign="top"><input type="radio" name="import_type" value="EmptyProject" id="' + DialogId + '_importTypeId" class="ImportBlank"></td><td ><label for="' + DialogId + '_importTypeId">Use a blank project</label></td>' +
      '<tr><td valign="top"><input type="radio" id="' + DialogId + '_projectList" name="import_type" value="ImportProject" class = "ImportFromProject"></td>' +
      '    <td><label for="' + DialogId + '_projectList">Import from existing projects</label>' +
      '        <br><select name="projectList"></select>' +
      (Editor.ifware.UsesParameter ?
         '        <br><input id="' + DialogId + '_importSampleData" type="checkbox" name="importSampleData" checked="checked">' +
            '        <label for="' + DialogId + '_importSampleData">Import sample data</label>' : '') +
      '<tr><td valign="top"><input type="radio" name="import_type" value="ImportZippedProject" id="' + DialogId + '_importZipFile' + '" class = "ImportFromZip"></td>' +
      '    <td><label for="' + DialogId + '_importZipFile">Import project from zip file</label>' +
      '<br><input type="file" id="ImportZipFileBrowse" name="NewFile" value = ""/>\
      <input type="hidden" name="translator_guid" value="' + Editor.ifware.ChannelGuid + '" />\  ' +
      (Editor.ifware.UsesParameter ?
         '<br><input id = "' + DialogId + '_includeSampleData" type = "checkbox" name = "includeSampleData" checked="checked">' +
            '  <label for="' + DialogId + '_includeSampleData">Import sample data</label>' +
            // Only include the option to append or replace if the project already has sample data.
            (Editor.ifware.countOfSample() > 0 ? '   <select name="addSampleTypeList">' +
               '<option value = "APPEND_SAMPLE">Append</option>' + '<option value = "REPLACE_SAMPLE">Replace</option>' +
               '</select>' : '') : '') +
      '</td></tr></table>' +
      '</form>' +
      '<div class="importError" style="color: red"></div>' +
      '</div>');

   var ImportDialog = $('#' + DialogId);

   function onToggleType() {
      if ($('#ImportZipFileBrowse', ImportDialog).val() == "" && $('input[name=import_type]:checked', ImportDialog).val() == "ImportZippedProject") {
         SCMenableImportBTN(ImportDialog, 'disable');
      }

      else {
         SCMenableImportBTN(ImportDialog, 'enable');
      }
   }

   // Select the blank project button by default if the project is new.
   if (NISmainFileSelectedAndEmpty(Editor)) {
      $('input[class = "ImportBlank"]').prop('checked', true);
   }
   else {
      $('input[class = "ImportFromProject"]').prop('checked', true);
   }

   $('input[name=import_type]', ImportDialog).click(function () {
      onToggleType()
   });
   $('input[name = NewFile]', ImportDialog).change(function () {
      SCMenableImportBTN(ImportDialog, 'enable');
   });

   $('select[name = projectList]', ImportDialog).mousedown(function () {
      $('input[class = "ImportFromProject"]').prop('checked', true);
      $(".importError").html(""); //clear error if existing.
      onToggleType()
   });
   $('input[name = importSampleData]', ImportDialog).click(function () {
      $('input[class = "ImportFromProject"]').prop('checked', true);
      onToggleType()
   });
   $('input[name = NewFile]', ImportDialog).click(function () {
      $('input[class = "ImportFromZip"]').prop('checked', true);
      onToggleType()
   });
   $('input[name = includeSampleData]', ImportDialog).click(function () {
      $('input[class = "ImportFromZip"]').prop('checked', true);
      onToggleType()
   });
   $('select[name = addSampleTypeList]', ImportDialog).mousedown(function () {
      $('input[class = "ImportFromZip"]').prop('checked', true);
      onToggleType()
   });

   var DlgButtons = {};
   DlgButtons['Import'] = function importButtonCallback() {
      if ('ImportProject' == $('input[name=import_type]:checked', this).val()) {
         Editor.ifware.CurrentSourceHash = null;
         var SourceGuid = $('select[name=projectList] option:selected', this).val();
         if (!SourceGuid) $(this).dialog('close');
         SCMdialogWaiting('#' + DialogId, true);
         var Params = {'translator_guid':Editor.ifware.ChannelGuid, 'source_guid':SourceGuid};
         if ($('input[name=importSampleData]:checked', this).length && Editor.ifware.UsesParameter) {
            Params.import_sample_data = 'true';
         }

         function importFromProjectSuccess(Response){
            SCMdialogWaiting('#' + DialogId, false);
            Editor.ifware.setCountOfSample(Editor.ifware.countOfSample() + Response.import_count);
            NISupdateSampleDataControl(Editor);
            NISreloadCurrentFile(Editor);
            $(ImportDialog).dialog('close');
         }

         function importFromProjectError(RequestObject, Status, ErrorString){
            SCMdialogWaiting('#' + DialogId, false);
            var ErrorMessage = RequestObject.responseJSON.error
                   ? RequestObject.responseJSON.error.description
                   : RequestObject.status + " - " + RequestObject.statusText;
            $('.importError', ImportDialog).html(ErrorMessage);
         }

         $.ajax({
            url    : "/sc/import_from_project",
            method : "POST",
            data   : Params,
            error  : importFromProjectError,
            success: importFromProjectSuccess
         });    
      }
      else if ('ImportZippedProject' == $('input[name=import_type]:checked', this).val()) {
         Editor.ifware.CurrentSourceHash = null;
         var Form = ImportDialog.find('form');
         var fileInput = Form.find('input[type="file"]');

         if ($('input[name=includeSampleData]:checked').length && Editor.ifware.UsesParameter) {
            if ($('select[name=addSampleTypeList] option:selected', this).val() == 'APPEND_SAMPLE' ||
               Editor.ifware.countOfSample() == 0) { // default to append if the project doesn't have any sample data
               Editor.ifware.DataImportType = SCMappendSampleStr;
            }
            else {
               Editor.ifware.DataImportType = SCMreplaceSampleStr;
            }
         }
         else {
            Editor.ifware.DataImportType = "";
         }

         SCMuploadFile(ImportDialog, Form, fileInput, Editor, ProjectManager);

      }
      else {
         var MainCode = '';
         var ProductText = MPRapp ? ' from '+MPRapp.type : '';
         if (Editor.ifware.UsesParameter) {
            var MainCode = '-- The main function is the first function called' + ProductText + '.\n\
-- The Data argument will contain the message to be processed.\n\
function main(Data)\n\
end';
         } else {
            var MainCode = '-- The main function is the first function called' + ProductText + '.\n\
function main()\n\
end';
         }
         NISsetScriptName(Editor, Editor.ifware.MainLuaFile);
         Editor.setValue(MainCode); //this should trigger an annotation update
         Editor.clearHistory();
         SCMclearDependencies(Editor);
         $(ImportDialog).dialog('close');
      }
   }
   DlgButtons['Cancel'] = function () {
      $(this).dialog('close');
   }

   ImportDialog.dialog({
      bgiframe:   true,
      title:      Editor.ifware.UsesParameter ? 'Import Project/Sample Data' : 'Import Project',
      resizable:  false,
      modal:      true,
      autoOpen:   false,
      buttons:    DlgButtons,
      close: function (event, ui) {
         Editor.focus();
      }
   });

   onToggleType();

   //only open the dialog when the response comes back, in case mini login is to be displayed.
   ImportDialog.dialog("open");
   SCMdialogWaiting('#' + DialogId, true);

   function handleProjectsFileInfoError(RequestObject, Status, ErrorString){
         ImportDialog.dialog("close");
         SCMdialogWaiting('#' + DialogId, false);
         handleProjectImportError(RequestObject, Status, ErrorString);
   }

   function handleProjectsFileInfoSuccess(Response){
         SCMdialogWaiting('#' + DialogId, false);
         var ProjNames = [];
         var ProjByName = {};
         Response.Projects.sort(function (Left, Right) {
            var LeftName = (Left.Name || Left.Guid).toLowerCase();
            var RightName = (Right.Name || Right.Guid).toLowerCase();
            if (LeftName < RightName) return -1;
            else if (LeftName > RightName) return 1;
            else return 0;
         });
         for (ProjIndex in Response.Projects) {
            var Proj = Response.Projects[ProjIndex];
            var NewOpt = $('<option></option>');
            NewOpt.val(Proj.Guid);
            NewOpt.html(Proj.Name || Proj.Guid);
            $('select[name=projectList]', ImportDialog).append(NewOpt);
         }
         var DialogWidth = $('#' + DialogId + ' table:first').width();
         ImportDialog.dialog('option', 'width', 26 + DialogWidth); //26 replicates padding width from the standard dialog
         ImportDialog.dialog('option', 'position', 'center'); //need to re-center
         SCMdialogWaiting('#' + DialogId, false);
   }

   var Params = { 'translator_guid' : Editor.ifware.ChannelGuid };
   $.ajax({
      url    : "/sc/get_all_projects_file_info",
      method : "POST",
      data   : Params,
      error  : handleProjectsFileInfoError,
      success: handleProjectsFileInfoSuccess
   });
}

function SCMincludeSampleConfirm(ChannelGuid) {
   $('#IncludeSampleConfirmDlg').remove();

   $(document).find("body")
      .append('<div id="IncludeSampleConfirmDlg" align="center">Include Sample Messages? </div>');

   var DlgButtons = {};

   DlgButtons['Yes'] = function () {
      SCMexportRequest(ChannelGuid, true);
      $('#IncludeSampleConfirmDlg').dialog('close');
   }

   DlgButtons['No'] = function () {
      SCMexportRequest(ChannelGuid, false);
      $('#IncludeSampleConfirmDlg').dialog('close');
   }

   $("#IncludeSampleConfirmDlg").dialog({
      bgiframe:   true,
      width:      300,
      height:     130,
      title:      'Confirm',
      modal:      true,
      autoOpen:   true,
      buttons:    DlgButtons
   });
}

function SCMexportRequest(ChannelGuid, IncludeSampleData) {
   window.open('/sc/export_to_zip?translator_guid=' + ChannelGuid + '&sample_data=' + IncludeSampleData);
   this.focus();
}


////////////////////////////

function SCMpluralLines(Count) {
   return (Count == 1) ? 'line' : 'lines';
}

function SCMfindLabel(Label, List) {
   for (var i = 0; i < List.length; i++) {
      if (List[i] == Label) {
         return true;
      }
   }
   return false;
}

function SCMdisplayError(ErrorMessage, ExitMethod){
   $('#scResponseDlg').remove();
   if (! ErrorMessage) { return false; }
   var Message = "<p> We're sorry, but an unexpected error occurred: </p>\
                    <p>" + ErrorMessage + "</p>\ ";
   $("body").append($("<div id='scResponseDlg'>\
                       <div class='sc_error'></div><br/>\
                       <div class='sc_output'></div><div class='sc_info'></div></div>\
                    "));
   var ResponsePopup = $("#scResponseDlg");
   var DlgButtons = {};
   DlgButtons['Close'] = function() {
      $(this).dialog('close');
   }
   ResponsePopup.dialog({
      bgiframe:true,
      width:600,
      title:"Error",
      modal:true,
      autoOpen:false,
      buttons:DlgButtons,
      close: function(event, ui) {
         ExitMethod();
      }
   });
   ResponsePopup.find('.sc_error').html(Message);
   ResponsePopup.find('.sc_error').show();
   ResponsePopup.dialog("open");
   return true;
}

// This is an older function that will detect any errors that
// may come down in a 200 response. 
// Most newer SC handlers will now send down a non-200 if there is an error.
function SCMcatchError(Response, ExitMethod) {
   if (! Response.ErrorMessage) { return false; }
   console.log("Caught error in response.");
   return SCMdisplayError(Response.ErrorMessage, ExitMethod);
}

function SCMupdateLegend(Diffs, Target) {
   var Lines = $(".toolbar_lines");
   Lines.html('');
   if (! Diffs.summary) {
      return;
   }
   var NewLegend = '<span class="label">Lines:</span>'
                 + '<span class="lines_info">Added: <span class="lines_added">'     + Diffs.summary.added   + '</span></span>'
                 + '<span class="lines_info">Deleted: <span class="lines_deleted">' + Diffs.summary.deleted + '</span></span>'
                 + '<span class="lines_info">Changed: <span class="lines_changed">' + Diffs.summary.changed + '</span></span>';
   Lines.html(NewLegend);
}

function SCMdrawDiffs(Diffs, Format) {
   if (Format == "condensed") {
      $("#SCMdiff").html(Diffs.condensed);
      $("#SCMdiff").addClass("changesOnly");
      return;
   }

   $("#SCMdiff").removeClass("changesOnly");
   var DiffClass = (Format == "vertical")
                   ? ' vrt'
                   : ''; 
   if(!Diffs.full){
      console.log("No diffs!");
      return;
   }
   
   var AllTheDiffs = $('<div class="diffTank' 
                      + DiffClass + '" id="diffFromTank"><div class="changeLabel">From</div><div class="diffCling">'
                      + Diffs.full.from
                      + '</div></div><div class="diffDivider'
                      + DiffClass + '" /><div class="diffTank'
                      + DiffClass + '" id="diffToTank"><div class="changeLabel">To</div><div class="diffCling">'
                      + Diffs.full.to + '</div></div>');
   
   $("#SCMdiff").html(AllTheDiffs);
   var $Both = $(".diffCling");

   var SyncDiffs = function(event) {
      var $this     = $(this);
      var $OtherOne = $Both.not(this);
      
      if ($this.attr('scrolling') == 'false' || $this.attr('scrolling') == undefined){
         $OtherOne.attr('scrolling','true');
         $OtherOne.scrollTop($this.scrollTop());
         $OtherOne.scrollLeft($this.scrollLeft());
      }
      
      $this.attr('scrolling','false');
   }

   $Both.on("scroll",SyncDiffs);
}

/////////
function SCMlights() {
   return ['red', 'yellow', 'green', 'gray']; // This matches the enum in DBDchannelStatus.h
}

function SCMinitialDiffs() {
   return {"condensed":'', "full": {"from":'', "to": ''}, "status": '', "summary": {"added": 0, "deleted": 0, "changed": 0}};
}

function SCMcompare(A, B, FieldName) {
   if (A[FieldName] > B[FieldName]) { return 1;  }
   if (A[FieldName] < B[FieldName]) { return -1; }
   return 0;
}

function SCMclip(Text, Length) {
   if (Text.length <= Length) { return Text; }
   return Text.substr(0, Length) + "...";
}

function SCMchannelConfigNameClip(Text, Length) {
   var RegEx  = /^\Channels\//;
   var RegEx2 = /-[A-F0-9]{32}\/ChannelConfiguration.xml$/;
   if (RegEx.test(Text)) {
      return SCMclip(Text.replace(RegEx, "").replace(RegEx2, ""), Length);
   }
   return SCMtranslatorNameClip(Text, Length);
}

function SCMtranslatorNameClip(Text, Length) {
   var RegEx  = /-(Ack|To|From|Filter|FromHTTP)-[A-Za-z0-9]{14}\//;
   var RegEx2 = /[A-F0-9]{32}\//;
   if (RegEx.test(Text)) {
      Text = Text.replace(RegEx, "-$1.../");
   } else if (RegEx2.test(Text)) {
      Text = Text.replace(RegEx2, ".../");
   }
   return Text.length <= Length
          ? Text
          : "..." + Text.substring(Text.length - Length);
}

function SCMsizeBoxes(FileCount, ProjectCount) {
   if (! ProjectCount) { return [100, 0]; }
   if (FileCount < 10)  { return [25, 74]; }
   var FileSize = FileCount / (FileCount + ProjectCount) * 100;
   if (FileSize > 75) { FileSize = 75; }
   return [FileSize, 99 - FileSize];
}

function SCMfileClasses(OneProject, FileCollisionMap) {
   if (! OneProject.shared_files) { return ''; }
   var Classes = ' ' + Object.keys(OneProject.shared_files).map(function(OneFile) {
      return FileCollisionMap[OneFile]["Index"];
   }).join(' ');
   if (OneProject.contains_new_file) {
      Classes += " NEW";
   }
   return Classes;
}

function SCMdecorate(CurrentIndex, FileScroll) {
console.log(CurrentIndex, FileScroll);
   $(".c").removeClass("c");
   $("#" + CurrentIndex).addClass("c");
   $(".h").removeClass("h");
   $("div.project." + CurrentIndex).addClass("h");
   $("div.project").on({
      "mouseenter": function(event) {
         $("span.file." + $(this).attr("id")).addClass("h");
      },
      "mouseleave": function(event) {
         $("span.file." + $(this).attr("id")).removeClass("h");
      }
   });
   $(".SCMstageLabel").each(function() {
      var Ht = $(this).outerHeight();
      $(this).next().css("top", Ht + "px");
   });
   $("#SCMstageFilesCollection").scrollTop(FileScroll);
}

function SCMfixView(ReviewPanelAndContents, ReviewTank, Toolbar, ReviewContents){
   console.log("SCMfixView");
   var InnerContainer = $("#SCMcommitList");
   var Scroller = $("#SCMcommitList .scroller");
   var Fixed = "#SCMcommitList .fixed";
   Scroller.height(InnerContainer.height() - $(Fixed).outerHeight() + "px");
   Scroller.css("top", $(Fixed).outerHeight() + "px");
   $(Fixed + " table").width($("#SCMcommitList .scroller table").width() + "px"); 

   var Parent = ReviewPanelAndContents.parent();
   var Cap = $(".EDT_toolbar_wrapper");
   var MasterHeight = Parent.height();
    
   if (Parent.is("#HistoryBody")) {
      MasterHeight = Parent.outerHeight();
   }
   if (Cap && Cap.length == 1) {
      MasterHeight -= Cap.outerHeight();
   }
   
   ReviewPanelAndContents.height(MasterHeight - InnerContainer.outerHeight() - $("#SCMreviewControls").outerHeight() + "px");   
   ReviewContents.height(ReviewTank.height() - Toolbar.outerHeight() + "px");
}

function SCMfixDiffView(Parent) {
   console.log("SCMfixDiffView");
   var ReviewPanelAndContents = $("#SCMreviewPanelAndDiff");
   var ReviewTank             = $("#SCMdiffTank");
   var Toolbar                = $(".diff_toolbar");
   var ReviewContents         = $("#SCMdiff");
   SCMfixView(ReviewPanelAndContents, ReviewTank, Toolbar, ReviewContents);
}

function SCMfixPreviewView() {
   console.log("SCMfixPreviewView");
   var ReviewPanelAndContents = $("#HistoryBody #SCMreviewPanelAndPreview");
   var ReviewTank             = $("#HistoryBody #SCMpreviewTank");
   var Toolbar                = $("#HistoryBody .preview_toolbar");
   var ReviewContents         = $("#HistoryBody #SCMpreview");
   SCMfixView(ReviewPanelAndContents, ReviewTank, Toolbar, ReviewContents);
}

function SCMpullStatus(StatusMap, NoteText) {
   $.ajax({
      url: '/dashboard_data', 
      method: 'GET',
      cache: false,
      data: {
         include_remote_servers: true
      }, 
      success: function(Response) {
         SCMbumpStatus(Response, StatusMap, NoteText);
      }
   });
}

function SCMbumpStatus(Response, StatusMap, NoteText) {
   var LIGHTS = SCMlights();
   var Channels = Response.Channels
   for (var i = 0; i < Channels.length; i++) {
      var Cguid = Channels[i].Channel.Guid;
      var Light = LIGHTS[Channels[i].Channel.Light];
      if (Light == 'green') {
         StatusMap[Cguid] = true;
      } else {
         StatusMap[Cguid] = false;
      }
      var Ticks = $("input.bumpUpdate[data-cguid=" + Cguid + "]");
      SCMupdateBumpNote(Ticks, Cguid, StatusMap, NoteText);
      $("#BumpStatus_" + Cguid).html('<img src="/images/button-dot' + Light + 'v4.gif" alt="' + Light + '" />');
   }
}

function SCMupdateBumpNote(Ticks, Cguid, StatusMap, Text) {
   var Note = $("td#BumpNote_" + Cguid);
   Ticks.each(function() {
      if ($(this).prop("checked") && StatusMap[Cguid]) {
         Note.html(Text);
         return false;
      } else {
         Note.html("");
      }
   });
}

// Used for channel bump check boxes after committing as well as import/export/restore file 
// collison check boxes.
function SCMbumpTickCheck(BumpTicks, BumpAllTick, NoteText, DataAttr, StatusMap) {
   BumpTicks.on("change", function(event) {
      console.log("tick change, updating gui.");
      var AllThem = true;
      event.stopPropagation();
      var OneTick = $(this);
      var Val = OneTick.attr("data-" + DataAttr);
      console.log(Val);
      
      if (StatusMap) {
         console.log(StatusMap);
         console.log(StatusMap[Val]);
      }

      BumpTicks.each(function() {
         if (! $(this).prop("checked")) {
            AllThem = false;
            BumpAllTick.prop("checked", false);
         }
      });

      if (AllThem) {
         BumpAllTick.prop("checked", true);
      }

      var Ticks = $("input.bumpUpdate[data-" + DataAttr + "='" + Val + "']");
      if (StatusMap) {
         SCMpullStatus(StatusMap, NoteText);
         SCMupdateBumpNote(Ticks, Val, StatusMap, NoteText);
      }
   });

   BumpAllTick.on("change", function(event) {
      if ($(this).prop("checked")) {
         BumpTicks.prop("checked", true);
      } else {
         BumpTicks.prop("checked", false);
      }
      BumpTicks.trigger("change");
   });
}

function SCMcommitKeys(Index, Max, Callback) {
   console.log("Index: " + Index);
   console.log("Max: " + Max);
   SCMdisableMouse();
   if (Max == 0) { return; }
   var StarterIndex = Index;
   var Current = $("tr.commitRow.current");

   function watchKeys(event) {
      console.log("Keydown Event triggered!!");
      console.log("KeyCode : " + event.keyCode);
      if(event.ctrlKey || event.metaKey || event.shiftKey){
         console.log("ctrl/command/shift key was pressed or is being held!");
         return;
      }
      event.preventDefault();
      SCMdisableMouse();
      var Changed = false
      if (event.keyCode == 38 && Index < Max) {     // going up
         console.log("up");
         Index++;
         Changed = true;
      } else if(event.keyCode == 40 && Index > 0) { //going down
         console.log("down");
         Index--;
         Changed = true;
      }
      if (Changed) {
         console.log("new index: " + Index);
         Callback(Index, Current);
      }
      return false;
   }

   $("body").off("keydown.differ");
   $("body").on("keydown.differ", watchKeys);
   $("body").focus();
}

function SCMdisableKeys(){
   $("body").off("keydown.differ");
   $("body").focus();
}

function SCMscrollCommitList() {
   var Current = $('tr.commitRow.current');
   var Pos = Current.position();
   if (Pos.top < 0) {
      Current[0].scrollIntoView(true);
   }
   if (Pos.top + Current.outerHeight() > $('.scroller').height()) {
      Current[0].scrollIntoView(false);
   }
}

function SCMdisableMouse() {
   $(".commitTable").removeClass("mouseworthy");
}

function SCMreEnableMouse() {
   $(".scroller .commitTable").addClass("mouseworthy");
}

function SCMlinkText(Text) {
   var RegEx = /(https?:\/\/\S*)/;
   return Text.replace(RegEx, function(Match) {
      return '<a href="' + Match + '" target="_blank">' + Match + '</a>';
   });
}

function SCMclearActiveRequest(CurrentRequest){
   if(CurrentRequest && 
      Object.keys(CurrentRequest).length && 
      CurrentRequest.hasOwnProperty("abort"))
   {
      CurrentRequest.abort();
      CurrentRequest = {};
   }
}

function SCMcompareFunc(a, b){
   if (a[0] < b[0]) { 
      return -1;
   } 
   return 1;
}



// BEGIN : Collision approval dialog building functions for import/export/restore

function SCMbuildChannelCollisionTable(IncludeStatus, TaskKeyTitleCase, ChannelCollisionMap){
   var ChannelCollisionTable = 
                   '<div id="collision_list"><div class="fixed">\
                   <table class="CollisionTable fixedHead"><thead><tr><th class="CollisionLabel">Channel</th>';
   if (IncludeStatus) {
      ChannelCollisionTable += '<th class="tick status">Status</th>';
   }
   ChannelCollisionTable +=   '<th class="action">Action&nbsp;<br><span class="select-style">\
                                 <select id="bulk_action" class="dropdown">\
                                   <option class="dropdown_opt" value="none">No Action</option>\
                                   <option class="dropdown_opt" value="update_everything">Full Update</option>\
                                   <option class="dropdown_opt" value="update_translators_only">Update Translators Only</option>\
                                   <option class="dropdown_opt" value="create_new_channel">'+ TaskKeyTitleCase +' As New Channel</option>\
                                 </select></span>\
                              </th>\
                              <th class="newnamecol">New channel name</th>\
                              <th class="notecol">Notes</th>\
                    </tr></thead></table></div>\
                    <div class="scroller"><table class="CollisionTable"><tbody>';

   ChannelCollisionTable += SCMbuildChannelCollisionRows(IncludeStatus, TaskKeyTitleCase, ChannelCollisionMap);
   ChannelCollisionTable += '</tbody></table></div></div>'
   return ChannelCollisionTable;
}

function SCMbuildChannelCollisionRows(IncludeStatus, TaskKeyTitleCase, ChannelCollisionMap){
   var ChannelNames = Object.keys(ChannelCollisionMap).sort(SCMcompareFunc);
   var Rows = "";
   for (var i = 0; i < ChannelNames.length; i++) {
      var ChannelGuid = ChannelCollisionMap[ ChannelNames[i] ]["local_guid"];
      Rows += '<tr class="CollisionChannel">' +
               '<td class="CollisionLabel">' + ChannelNames[i] + '</td>';
      if (IncludeStatus) {
         Rows += '<td class="tick status" id="ChannelStatus_' + ChannelGuid + '" /></td>';
      }
      Rows += '<td class="action">' +
                  '<span class="select-style"><select class="dropdown" data-cguid="' + ChannelGuid + '" >' +
                     '<option class="dropdown_opt" value="none">No Action</option>' +
                     '<option class="dropdown_opt" value="update_everything">Full Update</option>' +
                     '<option class="dropdown_opt" value="update_translators_only">Update Translators Only</option>' +
                     '<option class="dropdown_opt" value="create_new_channel">'+ TaskKeyTitleCase +' As New Channel</option>' +
                  '</select></span>' +
               '</td>' +
               '<td class="newnamecol" id="NewName_' + ChannelGuid + '"></td>' +
               '<td class="notecol" id="Note_' + ChannelGuid + '"></td>' +
              '</tr>';
   }
   return Rows;
}

function SCMbuildFileCollisionTable(TaskKeyTitleCase, FileCollisionMap){
   var FileCollisionTable = '<table id="bump_file_list" class="bumpTable">' +
                              '<thead><tr>' +
                                 '<th class="CollisionLabel">Shared file</th>' +
                                 '<th class="tick control"><input type="checkbox" name="BumpAll" id="BumpAll" />Replace with ' + TaskKeyTitleCase + '</th>' +
                                 '<th class="notecol" />' +
                              '</tr></thead>' +
                              '<tbody>' +
                                  SCMbuildFileCollisionRows(TaskKeyTitleCase, FileCollisionMap) +
                              '</tbody></table>';
   return FileCollisionTable;
}

function SCMbuildFileCollisionRows(TaskKeyTitleCase, FileCollisionMap){
   console.log("SCMbuildFileCollisionRows");
   console.log(FileCollisionMap);
   var Files = Object.keys(FileCollisionMap);
   console.log(Files);
   var Rows = "";
   for (var i = 0; i < Files.length; i++) {
      Rows += '<tr class="BumpFile"><td>' + Files[i] + '</td>'
            + '<td class="tick"><input type="checkbox" class="bumpUpdate fileBump" data-filepath="'+ Files[i] + '"></td>';
      if (i == 0) {
         Rows += '<td id="diffLink" rowspan="' + Files.length + '"><span id="showCollidingFileDiffView" class="clickable_link">See what will change</span></td>';
      }
      Rows += '</tr>';
   }
   return Rows;
}

function SCMcheckChannelLights(StatusMap, NoteText){
   $.ajax({
      url: '/dashboard_data',
      method: 'GET',
      data: {
         include_remote_servers: true
      },
      success: function(Response) {
         SCMupdateLightStatus(Response, StatusMap, NoteText);
      }
   });
}

function SCMupdateCollisionRestartNote(Ticks, Cguid, StatusMap, Text){
   var Note = $("table.CollisionTable td#Note_" + Cguid);
   Ticks.each(function() {
      if ($(this).prop("checked") && StatusMap[Cguid]) {
         Note.html(Text);
         return false;
      } 
   else{
         Note.html("");
      }
   });
}

function SCMupdateLightStatus(Response, StatusMap, NoteText){
   var LIGHTS = SCMlights();
   var Channels = Response.Channels
   for (var i = 0; i < Channels.length; i++) {
      var Cguid = Channels[i].Channel.Guid;
      var Light = LIGHTS[Channels[i].Channel.Light];
      if (Light == 'green') {
         StatusMap[Cguid] = true;
      } else {
         StatusMap[Cguid] = false;
      }
      var Ticks = $("input.bumpUpdate[data-cguid=" + Cguid + "]");
      SCMupdateCollisionRestartNote(Ticks, Cguid, StatusMap, NoteText);
      $("#ChannelStatus_" + Cguid).html('<img src="/images/button-dot' + Light + 'v4.gif" alt="' + Light + '" />');
   }
}

function SCMupdateCollidingFileApprovals(Checkboxes, FileCollisionMap){
   console.log("Updating file collision confirmation map.");
   Checkboxes.each(function() {
      FileCollisionMap[$(this).attr("data-filepath")].overwrite = $(this).prop("checked");
      console.log(FileCollisionMap[$(this).attr("data-filepath")]);
   });
}

function SCMremoveInvalidChannelSelectOptions(Page, ChannelSelectBoxes){
   Page.NameOnlyCollisionsPresent = false;
   
   //Need to remove the 2 update options in the select boxes for channels that 
   //only match on name, and don't match on component types. Not allowed to update them.
   console.log("Removing invalid options from the select boxes.");
   for(var ChannelName in Page.ChannelCollisionMap){
     var Channel = Page.ChannelCollisionMap[ChannelName];
     var ChannelGuid = Channel["local_guid"];
     var NameAndComponentsMatch = Channel.hasOwnProperty("update_everything") || Channel.hasOwnProperty("update_translators_only");
     if( ! NameAndComponentsMatch ){
       Page.NameOnlyCollisionsPresent = true;
       var ChannelSelectBoxUpdateAll   = "select.dropdown[data-cguid='" + ChannelGuid + "'] option[value='update_everything']";
       var ChannelSelectBoxUpdateTrans = "select.dropdown[data-cguid='" + ChannelGuid + "'] option[value='update_translators_only']";
       $(ChannelSelectBoxUpdateAll).remove();
       $(ChannelSelectBoxUpdateTrans).remove();
     }
   }
}

function SCMcheckForNewChannelNameColumnVisibility(SelectBoxes){
   var IsHidden = $("table.CollisionTable th.newnamecol").css("display") == "none"; 
   var ShouldBeVisible = SelectBoxes.length;
   $(SelectBoxes).each(function(){
      var SelectedValue = $(this).val();
      var LocalGuid = $(this).attr("data-cguid");
      var NewNameCell ="#NewName_" + LocalGuid;
      if( SelectedValue != "create_new_channel" ){
         //Don't need a new name text box.
         ShouldBeVisible = ShouldBeVisible  - 1;
         $(NewNameCell).children("input").remove();
      }
      else if( ! $(NewNameCell + " input").length ){
         //Needs a new name text box
         $(NewNameCell).append("<input type='text'></input>");
      }
   });
   if( ShouldBeVisible && IsHidden ){
      $("table.CollisionTable .newnamecol").css("display", "table-cell");
      $("table.CollisionTable .notecol").css("width", "25%");
   }
   else if( ! ShouldBeVisible && ! IsHidden ){
      $("table.CollisionTable .newnamecol").css("display", "none");
      $("table.CollisionTable .notecol").css("width", "50%");
   }
}

function SCMresetChannelCollisonChoices(ChannelName, ChannelCollisionMap){
   var Channel = ChannelCollisionMap[ChannelName];
   Channel["create_new_channel"] = false;
   if( Channel.hasOwnProperty("update_everything") ){
     Channel["update_everything"] = false;
   }
   if( Channel.hasOwnProperty("update_translators_only") ){
     Channel["update_translators_only"] = false;
   }
   Channel["new_channel_name"] = "";
}

function SCMupdateChannelApprovals(SelectBoxes, ChannelCollisionMap, LocalGuidsToNameMap){
   SelectBoxes.each(function() {
      var LocalGuid = $(this).attr("data-cguid");
      var SelectedValue = $(this).val();
      var ChannelName = LocalGuidsToNameMap[LocalGuid];
      SCMresetChannelCollisonChoices(ChannelName, ChannelCollisionMap);
      if(SelectedValue != "none"){
         ChannelCollisionMap[ChannelName][SelectedValue] = true;
         if(SelectedValue == "create_new_channel"){
            var NewName = $("#NewName_" + LocalGuid + " input").val();
            ChannelCollisionMap[ChannelName]["new_channel_name"] = NewName;
         }
      }
   });
}

function SCMupdateChannelNotes(SelectBoxes, Cguid, StatusMap, Text){
   var Note = $("#collision_list td.notecol#Note_" + Cguid);
   SelectBoxes.each(function() {
      var CurrentSelectedValue = $(this).val();
      var UpdateIsSelected = CurrentSelectedValue == "update_everything" || 
                                  CurrentSelectedValue == "update_translators_only";
      if(UpdateIsSelected && StatusMap[Cguid]){
         Note.html(Text);
         return false;
      } 
      else {
         Note.html("");
      }
   });
}

function SCMdisplayUpdateWarningMessageIfNeeded(TaskKey, NameOnlyCollisionsPresent){
   if( ! NameOnlyCollisionsPresent || $("#collision_list #channel_update_warning_messge").length != 0){
      return;
   }
   var UpdateMessage = "<p id='channel_update_warning_messge'>" +
                         "Note: <br> Some of the colliding channels match existing channels in name only and don't have the same component types.<br> " + 
                         "Because of this you will not be able to " + TaskKey + " and update them. " +
                         "You will only be able to " + TaskKey + " them as new channels.</p>";
   $("#collision_list").append(UpdateMessage);
}

function SCMcheckIfContinueButtonNeedsDisabling(SelectBoxes, CheckBoxes){
   var NumberOfChannelConfirmations = SelectBoxes.length;
   var NumberOfChannelNoActions     = 0;

   var NumberOfFileConfirmations = CheckBoxes.length;
   var NumberOfEmptyFileConfirmations = 0;
   
   $(SelectBoxes).each(function() {
      var SelectedValue = $(this).val();
      if (SelectedValue == "none") {
         NumberOfChannelNoActions++;
      }
   });

   $(CheckBoxes).each(function() {
      var Checked = $(this).prop("checked");
      if (!Checked)  {
         NumberOfEmptyFileConfirmations++;
      }
   });
   
   //Usually on the first collision confirmation dialog. 
   var ContinueButton = $("#confirmation-button-continue");
   if (!ContinueButton.length) {
      //But may be on the colliding shared file view dialog.
      ContinueButton = $("#SCMgo");
   }

   if (NumberOfChannelConfirmations > 0 && NumberOfChannelConfirmations === NumberOfChannelNoActions && 
      NumberOfFileConfirmations > 0  && NumberOfFileConfirmations == NumberOfEmptyFileConfirmations) {
      console.log("All channels set to no action and no file confirmations specified! Disabling continue button.");
      ContinueButton.button("disable");
      $("#CompleteControls").hide();
      $("#SCMreviewControls .review-actions").css("padding", "20px 20px");
   }
   else if (NumberOfChannelConfirmations > 0 && NumberOfChannelConfirmations === NumberOfChannelNoActions && 
            NumberOfFileConfirmations === 0) {
      console.log("All channels set to no action and no file confirmations are present.");
      ContinueButton.button("disable");
      $("#CompleteControls").hide();
      $("#SCMreviewControls .review-actions").css("padding", "20px 20px");
   }
   else {
      ContinueButton.button("enable");
      $("#SCMreviewControls .review-actions").css("padding", "10px 20px");
      $("#CompleteControls").show();
   }
}

function SCMsetSelectBoxChangeEvents(Page, SelectBoxes, BulkActionSelectBox, DataAttr, TaskKey){

   SelectBoxes.on("change", function(event) {
      console.log("Select Box Changed");
      var AllThem = true;
      event.stopPropagation();
      var SelectBox = $(this);
      var SelectedValue = SelectBox.val();
      var SelectBoxIdentifier = SelectBox.attr("data-" + DataAttr);
      console.log("Current channel guid: " + SelectBoxIdentifier);
      console.log("SelectedValue :" + SelectedValue);
      console.log(Page.StatusMap);
      SCMcheckChannelLights(Page.StatusMap, SCM_BUMP_NOTE);
      SCMupdateChannelNotes(SelectBox, SelectBoxIdentifier, Page.StatusMap, SCM_BUMP_NOTE);
      SCMupdateChannelApprovals(SelectBoxes, Page.ChannelCollisionMap, Page.LocalGuidsToNameMap);
      SCMcheckForNewChannelNameColumnVisibility(SelectBoxes);
      console.log(Page.ChannelCollisionMap);
   });

   BulkActionSelectBox.on("change", function(event) {
      console.log("Bulk Select Box Changed");
      var SelectedValue = $(this).val();
      console.log("Bulk Selection - " + SelectedValue);
      $(SelectBoxes).each(function(index) {
         var ChannelOption = $(this).children("option[value="+ SelectedValue +"]");
         
         //Check to see if the option is available and select it.
         if (ChannelOption.length) {
            $(this).val(SelectedValue);
            $(this).trigger("change");
         }

         if(SelectedValue == "update_everything" || SelectedValue == "update_translators_only"){
            console.log("Possibly displaying warning message...");
            SCMdisplayUpdateWarningMessageIfNeeded(TaskKey, Page.NameOnlyCollisionsPresent);
         }
      });
   });
}

// END : Collision approval dialog functions for import/export/restore


// BEGIN : Validation of collision input.

function SCMclearNewChannelNameWarnings(SelectBoxes){
   for(var i = 0; i < SelectBoxes.length; i++){
      var CurrentBox = SelectBoxes[i];
      var LocalGuid = $(CurrentBox).attr("data-cguid");
      $("#NewName_" + LocalGuid + " input").css("border", "none");
   }
   $("#collision_list .NewChannelNameWarning").remove();
}

function SCMcheckForExistingName(IxportableChannelList, NameToCheck, LocalGuid){
   console.log("checkForExistingName -- " + NameToCheck + " " + LocalGuid);
   for(var i in IxportableChannelList){
      var ExistingChannelName = IxportableChannelList[i]["name"];
      if(NameToCheck === ExistingChannelName){
         console.log("Channel name " + NameToCheck + " exists already!");
         var WarningBoxSelector = '#collision_list .ExistingName-' + LocalGuid;
         if( ! $(WarningBoxSelector).length ){
            var ExistingNameMsg = '<p class="NewChannelNameWarning ExistingName-' + LocalGuid + '">Channel name "' + 
                  NameToCheck + '" exists already! Choose another name.</p>';
            $("#collision_list").prepend(ExistingNameMsg);
            $(WarningBoxSelector).css("color", "red");
         }
         $("#NewName_" + LocalGuid + " input").css("border", "1px solid red");
         return true;
      }
   }
   return false;
}

function SCMemptyNameFound(LocalGuid){
   console.log("Empty name found!");
   if( ! $("#collision_list .EmptyName").length ){
      var NoEmptyMessage = '<p class="NewChannelNameWarning EmptyName">New channel names cannot be empty!</p>';
      $("#collision_list").prepend(NoEmptyMessage);
      $("#collision_list .EmptyName").css("color", "red");
   }
   $("#NewName_" + LocalGuid + " input").css("border", "1px solid red");
}

function SCMcheckForDuplicateNewName(CurrentNewNames, NewName, LocalGuid){
   if(!NewName || NewName == "") return;
   if( CurrentNewNames.hasOwnProperty(NewName) ){
      $("#NewName_" + LocalGuid + " input").css("border", "1px solid red");
      $("#NewName_" + CurrentNewNames[NewName] + " input").css("border", "1px solid red");
      var MessageContents = 'Channel name "' + NewName + '" Can\'t be added more than once!';
      var Selector = '#collision_list .DuplicateNewName';
      if( ( ! $(Selector).length) ||  ( $(Selector).length && $(Selector).text() !== MessageContents)){
         var DuplicateNameMsg = '<p class="NewChannelNameWarning DuplicateNewName">' + MessageContents + '</p>';
         $("#collision_list").prepend(DuplicateNameMsg);
         $(Selector).css("color", "red");   
      }
      return true;
   }
   else{
      CurrentNewNames[NewName] = LocalGuid;
   }
   return false;
}

function SCMvalidateAndStoreNewChannelNames(Page, SelectBoxes, TaskKey){
   console.log("validateAndStoreNewChannelNames");
   var ValidationFailed = false;
   var CurrentNewNames = {};
   for(var i = 0; i < SelectBoxes.length; i++){
      var CurrentBox = SelectBoxes[i];
      var LocalGuid = $(CurrentBox).attr("data-cguid");
      var SelectedValue = $(CurrentBox).val();
      var ChannelName = Page.LocalGuidsToNameMap[LocalGuid];
      if(SelectedValue == "create_new_channel"){
         var NewName = $("#NewName_" + LocalGuid + " input").val();
         Page.ChannelCollisionMap[ChannelName]["new_channel_name"] = NewName;
         if(NewName == "" || ! NewName){
            SCMemptyNameFound(LocalGuid);
            ValidationFailed = true;
         }
         else if(TaskKey == "export"){
            //This won't work on import or restore. (don't have the local channel list)
            ValidationFailed = SCMcheckForExistingName(Page.IxportableChannelList, NewName, LocalGuid) || ValidationFailed;
         }
         ValidationFailed = SCMcheckForDuplicateNewName(CurrentNewNames, NewName, LocalGuid) || ValidationFailed;
      }
   }
   return ValidationFailed;
}

// END : Validation of collision input.



// BEGIN : Diff view for colliding shared files
function SCMbuildCollidingFileDiffView(
   Page, 
   TaskKeyTitleCase, 
   showAffectedProjectsFunction, 
   AddContinueControlFunction, 
   CloseDialogFunction)
{
   console.log("SCMbuildCollidingFileDiffView");
   $("#SCMreview").remove();
   var TopPanel = "";
   var CollidingChannelsExist = Object.keys(Page.ChannelCollisionMap).length > 0;
   if(CollidingChannelsExist){
      console.log("CollidingChannelsExist!");
      TopPanel = '<div id="SCMcommitList"></div>';
   }
   var Screen = $('<div id="SCMreview"></div>');
   Screen.append(TopPanel
               + '<div id="SCMpanelAndDiff">'
               + '<div id="SCMpanel"></div>'
               + '<div id="SCMdiffAndControls">'
               + '<div id="SCMdiffTank">'
               + '<div class="diff_toolbar">'
               + '<div class="toolbar_filename"></div>'
               + '<div class="toolbar_lines"></div>'
               + '<div class="toolbar_display"><span class="label">Display:</span><div class="select-style" style="float: right;">'
               + '<select id="SCMdiffViewSelect">'
               + '<option value="vertical">Side by Side</option>'
               + '<option value="horizontal">Horizontal</option>'
               + '<option value="condensed">Changes Only</option>'
               + '</select></div>'
               + '<!--/toolbar_display--></div>'
               + '<!--/diff_toolbar--></div>'
               + '<div id="SCMdiff"></div>'
               + '<!--/SCMdiffTank--></div>'
               + '<!--/SCMdiffAndControls--></div>'
               + '<!--/SCMpanelAndDiff--></div>'
               + '<div id="SCMreviewControls">'
               + '<div class="review-actions"><span id="CompleteControls"><a id="SCMgo" class="action-button green btn-large">Complete '
               + TaskKeyTitleCase
               + '</a> <span style="margin: 0 10px; color: #888888;">or</span> </span><a id="SCMcancel" style="color: #666666; font-size: 14px;">Cancel</a></div>'
               + '<!--/SCMreviewControls--></div>'
                );
   
   $("body").append(Screen);
   
   showAffectedProjectsFunction();

   if(CollidingChannelsExist){
      $("#collision_list").appendTo( $("#SCMreview #SCMcommitList") );
      $("#SCMreview #SCMcommitList .scroller").css("top", "45px");
   }
   else{
      //fill void of no top panel.
      $("#SCMreview #SCMpanelAndDiff").css("top", "0"); 
      $("#SCMreview #SCMdiff").css("height", "100%");
   }

   $("#bump_file_list").remove();
   $("#ApprovalDlg"   ).remove();
   var DialogWidth  = $(window).width() - 30;
   var DialogHeight = $(window).height() - 30;
   
   Screen.css("overflow", "hidden");
   Screen.dialog({
      bgiframe  : true,
      width     : DialogWidth,
      height    : DialogHeight,
      title     :'Confirm overwrites',
      modal     : true,
      autoOpen  : false,
      resizable : false,
      close     : CloseDialogFunction
   });

   Screen.dialog("open");

   addControlsToDiffView(Page, AddContinueControlFunction, CloseDialogFunction);
}

function SCMfixCollidingSharedFileView() {
   console.log("SCMfixCollidingSharedFileView"); 
   var Container = $("#SCMreview #SCMcommitList");
   var Scroller = $("#SCMreview .scroller");
   Scroller.height(Container.height() - $("#SCMreview .fixed").outerHeight() + "px");
   Scroller.css("top", $("#SCMreview .fixed").outerHeight() - 1 + "px");
   $("#SCMreview .fixed table").width($("#SCMreview .scroller table").width() + "px");
}

function SCMattachDisableActionsToDiffView() {
   var ChannelSelectBoxes = $("#collision_list .CollisionChannel select.dropdown");
   var CollidingFileCheckBoxes =  $("#SCMpanel .tree input.commitFile");
   
   ChannelSelectBoxes.on("change", function() {
      console.log("Select box change!");
      SCMcheckIfContinueButtonNeedsDisabling(ChannelSelectBoxes, CollidingFileCheckBoxes);
   });

   CollidingFileCheckBoxes.on("change", function() {
      console.log("Check box change!");
      SCMcheckIfContinueButtonNeedsDisabling(ChannelSelectBoxes, CollidingFileCheckBoxes);
   });

   // Run it on initial view load.
   SCMcheckIfContinueButtonNeedsDisabling(ChannelSelectBoxes, CollidingFileCheckBoxes);
}

function addControlsToDiffView(Page, AddContinueControlFunction, CloseDialogFunction) {
   $("#SCMgo").button();
   
   $("#SCMreview #SCMdiffViewSelect").change(function(event) {
      Page.DiffFormat = $(this).val();
      SCMdoFileDiff(Page);
   });
   $("#SCMreview #SCMcancel").click(function(event) {
      $("#SCMreview").dialog("close");
      CloseDialogFunction();
   });
   $("#SCMreview #SCMpanelAndDiff").resizable({
      maxHeight: $("#SCMreview").height() - 75,
      minHeight: 10,
      handles: "n"
   });
   $("#SCMreview #SCMpanelAndDiff").on("resize", function(event, ui) {
      $("#SCMreview #SCMcommitList").css("height", ui.position.top + "px");
      SCMfixCollidingSharedFileView();
   });
   $("#SCMreview #SCMpanel").resizable({
      maxWidth: 500,
      minWidth: 60,
      handles: "e"
   });
   $("#SCMreview #SCMpanel").on("resize", function(event, ui) {
      event.stopPropagation();
      $("#SCMreview #SCMdiffAndControls").css("left", ui.size.width + "px");
   });

   AddContinueControlFunction();

   //Adjust butttons at bottom.
   $("#SCMreview #SCMreviewControls").css("padding", "0");
   $(".review-actions").css("padding", "10px 20px");
}

function SCMloadCollidingFileDiffViewData(Page, TaskKey) {
   var FileIndex = 0;
   var TheFileRows = '';
   for (var FilePath in Page.FileCollisionMap) {
      if (! Page.FileCollisionMap.hasOwnProperty(FilePath)) { continue; }
      // Only build the index once.
      //if (! Page.isInitialized) {
         Page.IndexMap["File" + FileIndex] = FilePath;
         Page.FileCollisionMap[FilePath]["index"] = FileIndex;
      //}
      if (FileIndex == 0) {
         SCMsetCurrentFile(Page, FilePath);
      }
      TheFileRows += SCMbuildFileRow(Page, FilePath, FileIndex);
      FileIndex++;
   }

   var Projects = Page.CollationMap.projects;
   var ProjectCount = Projects
                      ? Object.keys(Projects).length
                      : 0;
   var ProjectGuids = Object.keys(Projects);
   if (ProjectCount) {
      ProjectGuids.sort(function(A, B) {
         return SCMcompare(Projects[A], Projects[B], "project_name");
      });
   }

   var TheProjectRows = '';
   for (var i = 0; i < ProjectGuids.length; i++) {
      TheProjectRows += SCMbuildProjectRow(Page, Projects, ProjectGuids[i]);
   }

   var FileCount = Object.keys(Page.FileCollisionMap).length;
   var BoxSizes = SCMsizeBoxes(FileCount, ProjectCount);
   SCMrefreshFileBox(Page, TaskKey, FileIndex, TheFileRows, BoxSizes[0]);
   SCMrefreshProjectBox(TaskKey, TheProjectRows, BoxSizes[1]);  
}

function SCMprojectClasses(OneFile) {
   if (! OneFile.other_projects) { return ''; }
   return ' ' + OneFile.other_projects.map(function(ProjectGuid) {
      return 'Prj' + ProjectGuid;
   }).join(' ');
}

function SCMbuildFileRow(Page, CurrentPath, CurrentIndex) {
   var TickBox = '';
   TickBox = '<input class="commitFile" data-filepath="' + CurrentPath
           + '" id="commitFile' + CurrentIndex + '" type="checkbox"'
           + (Page.FileCollisionMap[CurrentPath]["overwrite"] ? ' checked="checked"' : '')
           + ' />';
   return '<li>'
          + TickBox
          + '<label id="File' + CurrentIndex + '" class="file select '
          + SCMprojectClasses(Page.CollationMap.files[CurrentPath])
          + (CurrentPath == Page.CurrentFile
            ? ' c'
            : '')
          + '">'
          + CurrentPath + '</label></li>';
}

function SCMbuildProjectRow(Page, Projects, ProjectGuid) {
   return '<div id="Prj' + ProjectGuid + '" class="project'
          + SCMfileClasses(Projects[ProjectGuid], Page.FileCollisionMap)
          + '" data-projectguid="' + ProjectGuid + '">'
          + '<strong>(' + Object.keys(Projects[ProjectGuid]['shared_files']).length + ')</strong> '
          + Projects[ProjectGuid]['project_name']
          + '<span class="project-files" id="PrjFil' + ProjectGuid + '" /></div>';
}

function SCMbuildProjectFileRow(ProjectFiles, Path, ProjectGuid) {
   return '<span class="project-file '
          + ProjectFiles[Path]['status']
          + ' " data-projectguid="' + ProjectGuid + '">'
          + Path
          + '</span>';
}

function SCMsetCurrentFile(Page, FilePath) {
   console.log(FilePath);
   Page.CurrentFile   = FilePath;
   Page.CurrentIndex  = "File" + Page.FileCollisionMap[FilePath]["index"];
   $(".toolbar_filename").html(FilePath);
}

function SCMdoFileDiff(Page) {
   console.log(Page);
   var Diff = Page.Diffs[Page.CurrentFile];

   SCMupdateLegend(Diff, $("#SCMreviewControls"));
   if (Diff["status"] && ! Diff["summary"]) {
      $("#SCMdiff").html('<div class="noDiff">(<span>' + Diff["status"] + '</span>)</div>');
      return;
   }
   SCMdrawDiffs(Diff, Page.DiffFormat);
}

function SCMrefreshFileBox(Page, TaskKey, CurrentIndex, Rows, Height) {
   var All = true;
   for (var OnePath in Page.FileCollisionMap) {
      if (! Page.FileCollisionMap[OnePath]["overwrite"]) {
         All = false;
         break;
      }
   }

   var FileBox = 
   $('<ul class="tree">\
      <li><label><input type="checkbox" id="commitAll" name="commitAll"'
      + (All ? ' checked="checked"' : '')
      + '/>Shared files in this ' + TaskKey + ':</label></li>'
      + Rows
      + '</ul>\
   ');

   var CollidingFileCheckBoxes = FileBox.find(".commitFile");
   var CollidingFileCheckBoxAll = FileBox.find("#commitAll");
   
   SCMbumpTickCheck(CollidingFileCheckBoxes, CollidingFileCheckBoxAll, "", "filepath");

   CollidingFileCheckBoxes.on("change", function(event) {
      SCMupdateCollidingFileApprovals($(this), Page.FileCollisionMap);
   });

   FileBox.find(".select").click(function(event) {
      console.log("Click on file");
      FileScroll = $("#SCMstageFilesCollection").scrollTop();
      SCMsetCurrentFile(Page, Page.IndexMap[$(this).attr("id")]);
      SCMdecorate(CurrentIndex, FileScroll);
      SCMdoFileDiff(Page);
   });

   $("#SCMreview #SCMpanel ul.tree").remove();
   $("#SCMreview #SCMpanel").prepend(FileBox);
}

function SCMrefreshProjectBox(TaskKey, Rows, Height) {
   if(TaskKey === 'export'){
      //Don't want to show the local collated projects for export repo file conflicts (won't make sense)
      $("div.projects_affected").remove();
      return;
   }
   if (! Rows) {
      $("div.projects_affected").remove();
      return;
   }
   var ProjectBox = $('<div class="projects_affected"><div class="label">Projects affected:</div><ul>' + Rows + '</ul></div>');
   $("#SCMreview div.projects_affected").remove();
   $("#SCMreview #SCMpanel").append(ProjectBox);
}
// END : Diff view for colliding shared files

