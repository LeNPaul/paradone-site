/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

//an object that takes a list
//of jquery selectors, and sequentially assigns the tab order
//and wraps around the tab order as well.
function TABmaker(Selectors) {
   if (!Selectors) { return; }
   //We set this high so we don't mess around with other tab indexes in the document 
   //TODO - kinda dirty
   var TabIndex = 1000;
   for (i in Selectors) {
      $(Selectors[i]).attr('tabindex', TabIndex++);
   }
   if (Selectors.length) {
      //make wraparound for last, back to first
      $(Selectors[Selectors.length - 1]).keydown(function(e) {
         if (e.which == 9) { //tab
            e.preventDefault();
            $(Selectors[0]).focus();
         }
      });
      $(Selectors[0]).focus(); //select first item
   }
}


function IGNMPRexportTablesExecute(InVmdFile, InMapperGuid) {
   //create the dialog
   var ExportId = 'IGNexportTableDialog';
   if ($('#' + ExportId).length) {
      $('#' + ExportId).dialog('destroy');
      $('#' + ExportId).remove();
   }
   function makeId(Name) {
      return ExportId + '_' + Name;
   }

   $(document).find('body').append(
       '<div id="' + ExportId + '">\
       <form>\
       <table>\
       <tr>\
        <td><label for="' + makeId('exportTableDbApi') + '">Database API:</label></td><td><select name="exportTableDbApi" id="' + makeId('exportTableDbApi') + '"/></select></td> \
		<td><label for="' + makeId('exportTableDb') + '">Database:</label></td><td><input id="' + makeId('exportTableDb') + '" name="exportTableDb" type="text"/></td> \
        <td rowspan="2"><button type="button" class="previewExportTableButton">Preview Create Statements</button></td> \
		<td rowspan="2" id="' + makeId("previewExportTableStatusIndicator") + '"></td> \
       </tr>\
       <tr>\
          <td><label for="' + makeId('exportTableDbUsername') + '">Username:</label></td><td><input name="exportTableDbUsername" id="' + makeId('exportTableDbUsername') + '" type="text"/></td> \
          <td><label for="' + makeId('exportTableDbPassword') + '">Password:</label></td><td><input id="' + makeId('exportTableDbPassword') + '" name="exportTableDbPassword" type="password" autocomplete="off" /></td>\
       </tr>\
       </table>\
       <div>\
          <div style="padding-top: 5px;">Preview Results/SQL Statements:\
             <span class="tableDropWarning" style="display:none; color: #FF0000">WARNING: Tables will be dropped.</span>\
          <div>\
          <div class="tableExportSqlEditor" style="background-color: #FFFFFF; border: 1px solid #000000; width: 750px; height: 150px;"></div>\
          <div style="padding-top: 5px;">Execute Results:</div>\
          <div class="tableExportSqlResult" style="background-color: #FFFFFF; padding-left: 3px; border: 1px solid #000000; overflow-x: auto; width: 750px; height: 150px;"></div></>\
       </div>\
       </form>\
       </div>'
       );

   var ExportDialog = $('#' + ExportId);
   var ExecuteInProgress = false;
   var Editor;
   function makeExportParams() {
      return {
         DbName: $('input[name=exportTableDb]', ExportDialog).val(),
         DbApi: $('select[name=exportTableDbApi] option:selected', ExportDialog).val(),
         DbUser: $('input[name=exportTableDbUsername]', ExportDialog).val(),
         DbPassword: $('input[name=exportTableDbPassword]', ExportDialog).val(),
         MapperGuid: InMapperGuid,
         VmdFile: InVmdFile
      };
   }
   ExportDialog.dialog({
      bgiframe: true,
      width: 780,
      height: 530,
      title: 'Export Tables: ' + InVmdFile,
      modal: true,
      autoOpen: false,
      resizable: false,
      buttons: {
         'Close': function() {
            $(this).dialog('close');
         },
         'Execute Statements': function() {
            if (!ExecuteInProgress) {
               ExecuteInProgress = true;
               var ExecuteParams = makeExportParams();
               ExecuteParams.ExecuteSql = Editor.getValue();
               $('.tableExportSqlResult', ExportDialog).html('Executing ...');
               Ajax.post('/export_mapper_vmd_tables_to_db',
                   ExecuteParams,
                   function(Response, TextStatus, RequestObj) {
                      ExecuteInProgress = false;
                      var ResultString = (Response.ExportComplete ? 'Executed successfully.' : Response.Error);
                      $('.tableExportSqlResult', ExportDialog).html('<pre>' + ResultString + '</pre>');
                   }
               );
            }
         }
      }
   });

   function makeDialog(Editor) {
      //Kinda hackish way to override the styling that was done for the main editor
      $(Editor.getWrapperElement(), ExportDialog).css('position', 'static').css('left', '0px').css('right', '0px');
      var InProgress = false;
      var previewButton = $('.previewExportTableButton', ExportDialog);
      var previewIndicator = $("#" + makeId("previewExportTableStatusIndicator"));
      var GoButton = $(".ui-dialog-buttonpane button:contains('Execute Statements')");
      GoButton.attr("disabled", true).addClass("ui-state-disabled");

      previewButton.click(function() {
         if (!InProgress) {
            InProgress = true;
            // Disable the preview and execute buttons and display a GIF for the status indicator.
            GoButton.attr("disabled", true).addClass("ui-state-disabled");
            previewButton.attr("disabled", "disabled");
            previewIndicator.html('<img src="/js/mapper/images/spinner.gif" />');

            Ajax.post('/export_mapper_vmd_tables_to_db',
                makeExportParams(),
                function(Response, TextStatus, RequestObj) {
                   InProgress = false;
                   // Re-enable the preview and execute buttons and remove the status indicator.
                   // The only valid value for the disabled attribute is "disabled",
                   // so to remove it we must remove the attribute entirely.
                   GoButton.attr("disabled", false).removeClass("ui-state-disabled");
                   previewButton.removeAttr("disabled");
                   previewIndicator.html("");

                   var ResultText = (Response.Error ? Response.Error : Response.ExportPreview);
                   Editor.setValue(ResultText);
                   Editor.clearHistory();
                   $('.tableDropWarning', ExportDialog).get(0).style.display = (Response.HaveDropCommands ? '' : 'none');
                },
                'json');
         }
      }
      );
      TABmaker(['#' + makeId('exportTableDbApi'),
         '#' + makeId('exportTableDb'),
         '#' + makeId('exportTableDbUsername'),
         '#' + makeId('exportTableDbPassword'),
         '#' + ExportId + ' .previewExportTableButton',
         '#' + ExportId + '~* button:last',
         '#' + ExportId + '~* button:first'
      ]);
   }//makeDialog()

   //update api dropdown, when it finishes, open the dialog
   Ajax.post('/db_api_list', '',
       function(Response, TextStatus, RequestObject) {
          ExportDialog.dialog('open');
          //the editor needs to be initialized before we do anything else
          Editor = new CodeMirror($('.tableExportSqlEditor', ExportDialog).get(0), {
             mode: 'text/x-plsql',
             theme: 'ifware',
             indentUnit: 3,
             tabMode: 'indent',
             lineNumbers: true,
             height: '150px',
             width: '740px'
          });
          if (Response) {
             var ApiSelect = $('select[name=exportTableDbApi]', ExportDialog);
             for (DbIndex in Response.DbApi) {
                var DbApi = Response.DbApi[DbIndex];
                var NewOpt = $('<option></option');
                NewOpt.val(DbApi).html(DbApi);
                ApiSelect.append(NewOpt);
             }
          }
          makeDialog(Editor);
       }
   , 'json');
}
