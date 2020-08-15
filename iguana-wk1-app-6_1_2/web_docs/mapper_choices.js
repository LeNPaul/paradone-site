/** 
 * Copyright (c) 2015 iNTERFACEWARE Inc.  All rights reserved.
 */

function DMPRloadHistory(TranslatorGuid, MapperType, Incumbent) {
  if (! TranslatorGuid || TranslatorGuid == '00000000000000000000000000000000') { return ;}
  var ChoiceBox =  $("#" + MapperType + "CommitChoices");
  var Loader = $('<div id="SCMloadWheelContainer"><span id="SCMloadWheel" class="SCMloading"></span></div></div>');
  ChoiceBox.prepend(Loader);
  $.ajax({
    url: '/sc/translator_history',
    cache: false,
    data: {'translator_guid': TranslatorGuid},
    success: function(Response, TextStatus, RequestObject) {
       DMPRdisplayCommits(Response, TranslatorGuid, MapperType, Incumbent);
    },
    error: function(RequestObject, Status, ErrorString){
      var ErrorMessage = RequestObject.responseJSON.error
                       ? RequestObject.responseJSON.error.description
                       : RequestObject.status + " - " + RequestObject.statusText;
      var Message = "Error : " + ErrorMessage;
      alert(Message);
    }
  });
}   
function DMPRdisplayCommits(History, TranslatorGuid, MapperType, Incumbent) {
   if ( ! History) { return; }
   var NoteMap = {};
   var T = $('<table class="commitTable commitChoices"><thead><tr><th /><th>Time</th><th>Note</th><th>User</th><th>Commit ID</th></tr></thead><tbody></tbody></table>');
   for (var i = History.length - 1; i >= 0 ; i--) {

      var ThisCommit = History[i];
      NoteMap[ThisCommit.commit_id] = ThisCommit.commit_comment;
      var ThisDate = new Date(ThisCommit.commit_timestamp * 1000);
      var DisplayDate = ThisDate.toLocaleDateString() + ' ' + ThisDate.toLocaleTimeString();
      if (ThisCommit.commit_id == Incumbent) {
         $("#" + MapperType + "CommitComment").val(ThisCommit.commit_comment);
      }
      T.find("tbody").append('<tr><td><input type="radio" name="' + MapperType 
                                        + 'CommitId" value="' + ThisCommit.commit_id +'"'
                                        + (ThisCommit.commit_id == Incumbent ? ' checked="checked"' : '' )
                                        + ' /></td><td class="date">' 
                                   + DisplayDate + '</td><td class="note">'
                                   + ThisCommit.commit_comment   + '</td><td class="user">'
                                   + ThisCommit.commit_user      + '</td><td class="commit_id">'
                                   + ThisCommit.commit_id + '</td></tr>');
   }
   T.find("tbody tr").click(function(Event) {
      var Radio = $(this).find("input:radio");
      Radio.attr("checked", "checked");
      $("#" + MapperType + "CommitComment").val(NoteMap[Radio.val()]);
   });

   var ChoiceBox =  $("#" + MapperType + "CommitChoices");
   ChoiceBox.find("#SCMloadWheelContainer").remove();
   ChoiceBox.html(T);
   ChoiceBox.css("max-height", (0.50 * $("body").height()).toString() + "px");
}

