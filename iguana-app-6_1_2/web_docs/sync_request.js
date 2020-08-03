/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

$(document).ready(function() {
   $('[id^="Pass-"]').watermark('Same as Local');
   $('#all').click(function() {
      var Checked = $(this).prop('checked');
      $('[id^="Use-"]').prop('checked', Checked);
   });
   $('#sync').click(function() {
      if($('[id^="Use-"]:checked').size()) {
         $('#pass').val('');
         $('#confirm').dialog('open');
      } else {
         $('#result')
            .text('You must select at least one remote Iguana.')
            .dialog('open');
      }
   });
   $('[id^="fail-"] > a').click(function() {
      $('#result')
         .text($(this).data('Error') || 'Unknown error.')
         .dialog('open');
   });
   $('#result').dialog({
      autoOpen: false,
      modal: true,
      title: 'DataSync Result',
      buttons: {
         'Close': function() {
            $(this).dialog('close');
         },
      }
   });
   $('#confirm').dialog({
      autoOpen: false,
      modal: true,
      title: 'DataSync Confirmation',
      buttons: {
         'Cancel': function() {
            $(this).dialog('close');
         },
         'Apply': function() {
            sendRequest();
            $(this).dialog('close');
         }, 
      }
   });

   var RowsByUrl = {};
   $('[id^="Use-"]').each(function() {
      RowsByUrl[$(this).val()] = $(this).attr('id').match(/-(.*)/)[1];
   });

   function updateStatus(JobInfo, StatusList) {
      var JobList = [];
      for(i in StatusList) {
         var Item = StatusList[i], FadeTo;
         var Row = JobInfo[Item.JobId].Row;
         var Error = undefined;
         if(Item.Error || Item.Status == 'failed') {
            FadeTo = '#fail-' + Row;
            Error = Item.Error || "Unknown failure.";
         } else if(Item.Status == 'running') {
            FadeTo = '#busy-' + Row;
            JobList.push(Item.JobId);
         } else if(Item.Status == 'finished') {
            if(Item.Result.Code != 'success') {
               FadeTo = '#fail-' + Row;
               Error = Item.Result.Text;
            } else {
               FadeTo = '#okay-' + Row;
               var Server = JobInfo[Item.JobId].Link;
               var UndoLink = Server + Item.Result.UndoLink;
               $(FadeTo+' > a').attr('href',
                  '/remote_frame.html?TargetSrc=' + encodeURIComponent(UndoLink)
                  + '&TargetServerLabel=' + Server.match(/\/\/(.*)/)[1]);
            }
         } else { // 'unknown'
            FadeTo = '#fail-' + Row;
            Error = "The status of this operation was lost.  "
               + "Check the log of the remote machine.";
         }
         if(Error) {
            $('#fail-'+Row+' > a').data('Error', Error);
         }
         if($(FadeTo + ':hidden').get()) {
            $('#idle-'+Row+', #busy-'+Row+', #okay-'+Row+', #fail-'+Row).hide();
            $(FadeTo).show();
         }
      }
      if(JobList.length) {
         setTimeout(function() { getStatus(JobInfo, JobList); }, 500);
      }
   }

   function getStatus(JobInfo, JobList) {
      var Args = { 'Action': 'get_status', 'Jobs': JobList.join(',') };
      $.ajax({ 
         'url': '/dataSyncBroadcast', 
         'data': Args,
         'dataType': 'json',
         'cache': false,
         'success': function(Result) {
            if(Result.ErrorDescription) {
               MiniLogin.show(Result.ErrorDescription, function() {
                  getStatus(JobInfo, JobList);
               });
            } else if(Result.Error) {
               $('#result').text(Result.Error).dialog('open');
            } else {
               updateStatus(JobInfo, Result.Status);
            }
         },
         'error': function(Request, Status, Error) {
            MiniLogin.show('Iguana is not responding.', function() {
               getStatus(JobInfo, JobList);
            });
         },
      });
   }

   function sendRequest() {
      var Args = { 'Action': 'send', 'Password': $('#pass').val() };
      $('[id^="Use-"]:checked').each(function() {
         Args[$(this).attr('id')] = $(this).val();
      });
      $('[id^="Pass-"]').filter(function() {
         return this.value != '';
      }).each(function() {
         Args[$(this).attr('id')] = $(this).val();
      });
      $.ajax({ 
         'url': '/dataSyncBroadcast', 
         'data': Args,
         'dataType': 'json', 
         'cache': false,
         'method': "POST",
         'success': function(Result) {
            if(Result.ErrorDescription) {
               MiniLogin.show(Result.ErrorDescription, sendRequest);
            } else if(Result.Error) {
               $('#result').text(Result.Error).dialog('open');
            } else {
               var JobInfo = {};
               for(i in Result.Status) {
                  var Item = Result.Status[i];
                  JobInfo[Item.JobId] = {
                     'Link': Item.Link,
                     'Row': RowsByUrl[Item.Link],
                  }
               }
               updateStatus(JobInfo, Result.Status);
            }
         },
         'error': function(Request, Status, Error) {
            MiniLogin.show('Iguana is not responding.', sendRequest);
         },
      });
   }

   var Query = $('#query').val();
   $('#apply').click(function() {
      var Link = '/dataSync?' + Query + '&Password=' + encodeURIComponent($('#pass').val());
      $(this).html('<span>Busy...</span>').attr('disabled', true);
      $.getJSON(Link, function(Result) {
         $('#result')
            .text(Result.Text)
            .dialog('option', 'buttons', {
               'Close' : function() {
                  if(Result.Code != 'success') {
                     $('#pass').val('');
                     $('#apply').html('<span>Apply</span>').attr('disabled', false);
                  } else {
                     location.replace('/settings#Page=roles');
                  }
                  $(this).dialog('close');
               }})
            .dialog('open');
      });
   });
});
