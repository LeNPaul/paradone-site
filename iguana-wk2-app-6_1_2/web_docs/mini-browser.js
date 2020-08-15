/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

var MiniBrowser = function()
{
   var s_Query, s_Source, s_Date, s_Time;
   
   function create(m_Config)
   {
      var m_Ajax = window.Ajax || window.MiniLogin;

      function importMessage(mb, MessageId, fromHttpRequestLog) {
         console.log('importMessage');
         var pb = $('#mb-progress');
         if (pb.length > 0) {
            pb.remove();
         }
         
         $('body').append('\
            <div id="mb-progress" style="display:none">\
               <img src="/js/mapper/images/spinner.gif">\
               Importing message from logs...\
            </div>');
         pb = $('#mb-progress').dialog({
            'autoOpen': false,
            'minHeight': 50,
            'resizable': false,
            'title': 'Importing'
         });
         pb.dialog('open');
         console.log("MessageId = ", MessageId);
         m_Ajax.getJSON(fromHttpRequestLog? '/http_log_entries?' : '/export_to_mapper?', {
               'Action': 'export',
               'TargetGuid': m_Config.target,
               'Export.Plan': MessageId
            },
            function(Result) {
               console.log(Result);
               console.log(m_Config);
               if(Result.Success) {
                  pb.dialog('close');
                  m_Config.onSuccess(Result.ImportStart+1, Result.SizeOfImport);
               }
               else {
                  pb.html(Result.Error);
                  pb.dialog({
                     "buttons": {
                        "Close": function() { $(this).dialog("close"); }
                     },
                     "title": "Import Failed"
                  });
               }
            }
         );
      }

      function addSources(mb, Sources) {
         var Selector = $('#mb-source', mb);
         var Selected = Selector.val();
         Selector.html('<option selected></option>');
         $('option', Selector).text(Selected);
         $('<option>--</option>').attr('value', Selected).appendTo(Selector);
         $.map(Sources, function(Source) {
            if(Source.Name != Selected) {
               Selector.append('<option></option>');
               $('option:last', Selector).text(Source.Name);
            }
         });
      }

      function fillEntries(mb, Result, Query, fromHttpRequestLog) {
         if (!fromHttpRequestLog && Query.BeforePosition === undefined) {
            addSources(mb, Result.Sources);
         }
         var Limit = Math.max(0, 5 - $('div.mb-entry', mb).length);
         var Loading = $('#mb-loading', mb);
         var Row = $('<div class="mb-entry"></div>');
         $.map(Result.Entries.slice(0,Limit), function(Entry) {
            Row.clone()
               .html(Entry.Preview)
               .click(function() {
                  try { m_Config.onStart(); }
                  catch(Ignored) { }
                  importMessage(mb, Entry.MessageId, fromHttpRequestLog);
               })
               .insertBefore(Loading);
         });
         if(Result.Complete || Result.Entries.length >= Limit) {
            Loading.hide();
            if(!$('div.mb-entry', mb).length) {
               $('#mb-status', mb).text('No matching entries.').show();
            }
         } else {
            $('#mb-progress').html(Result.Progress);
            Query.BeforePosition = Result.Continue.Position;
            Query.Date           = Result.Continue.Date;
            search(mb, Query);
         }
      }

      function withTimespec(Action) {
         var d = $('#mb-date').val().replace(/\s/g, '');
         var t = $('#mb-time').val().replace(/\s/g, '');
         if(t && !d) {
            var now = new Date;
            d = [now.getFullYear(), now.getMonth()+1, now.getDate()].join('/');
         } else if(d && !t) {
            t = '23:59:59';
         }
         if(d && !d.match(/^\d+\/\d+\/\d+$/)) {
            $('#mb-status').text('Invalid date (use YYYY/MM/DD).').show();
         } else if(t && !t.match(/^\d+:\d+(:\d+)?$/)) {
            $('#mb-status').text('Invalid time (use HH:MM or HH:MM:SS).').show();
         } else {
            Action(d ? [d,t].join(' ') : '');
         }
      }

      var m_LastSearch;
      function search(mb, Query, fromHttpRequestLog) {
         if(m_LastSearch) {
            m_Ajax.abort(m_LastSearch);
         }
         m_LastSearch = m_Ajax.getJSON(fromHttpRequestLog?'/http_log_entries':'/log_entries', Query,
            function(Result) {
               m_LastSearch = undefined;
               fillEntries(mb, Result, Query, fromHttpRequestLog);
            }
         );
      }

      function loadEntries(mb, fromHttpRequestLog) {
         console.log('loadEntries');
         $('div.mb-entry', mb).remove();
         $('#mb-status', mb).hide();
         withTimespec(function(Before) {
            $('#mb-progress', mb).text('');
            $('#mb-loading', mb).show();
            search(mb, {
               'Output': 'json',
               'Filter': $('#mb-query', mb).val(),
               'Source': $('#mb-source', mb).val(),
               'Before': Before,
               'Type': 'messages',
               'TargetGuid': m_Config.target
            }, fromHttpRequestLog);
         });
      }

      m_Config.parent.append('\
         <div id="mini-browser">\
            <div id="mb-title" style="display:none">\
               Select a message from the logs to add as sample data.\
            </div>\
            <span id="mb-options" style="display:none">\
               <input id="mb-query" title="Search Text">\
               <select id="mb-source">\
                  <option selected></option>\
               </select>\
               <input id="mb-date" size="10" title="Select Date">\
               <input id="mb-time" size="6" title="Time">\
            </span>\
            <div id="mb-divider"></div>\
            <button id="mb-search">Add From Logs</button>\
            <button id="mb-addfromhttp">Add From HTTP Request Log</button>\
            <div id="mb-status" style="display:none"></div>\
            <div id="mb-loading" style="display:none">\
               <img src="/js/mapper/images/spinner.gif">\
               Searching... <span id="mb-progress"></span>\
            </div>\
         </div>');
         
      if (!m_Config.https) {
         $('#mb-addfromhttp').remove();
      }
         
      $('#mb-query')
         .val(s_Query || '')
         .change(function() { s_Query = $(this).val(); })
         .watermark()
         .add('#mb-date,#mb-time').keydown(function(e) {
            if(e.keyCode == 13) { $('#mb-search, #mb-addfromhttp').filter (":visible").click(); }
            else if(e.keyCode == 27) { m_Config.onCancel(); }
         });
      $('#mb-source option').text(s_Source || m_Config.source);
      $('#mb-source').change(function() {
         s_Source = $(this).val();
      });
      $('#mb-date')
         .val(s_Date || '')
         .change(function() { s_Date = $(this).val(); })
         .watermark()
         .datepicker({
            'dateFormat': 'yy/mm/dd',
            'changeMonth': true,
            'changeYear': true,
            'maxDate': '+0d',
            'beforeShow': function() {
               // Needed because we recreate the browser each time.
               $('.ui-datepicker').css('z-index', 950);
            },
            'onSelect': function(NewDate) {
               s_Date = NewDate;
               $(this).watermark();  // Avoid overlap bug.
            }
         })
      $('#mb-time')
         .val(s_Time || '')
         .change(function() { s_Time = $(this).val(); })
         .watermark();
      $('#mb-search').click(function() {
         try { m_Config.onOpen(); }
         catch(Ignored) { }
         $('#mb-divider, #mb-addfromhttp').hide();
         $('#mb-title, #mb-options').show();
         $(this).text('Search');
         loadEntries($('#mini-browser'));
      });
      $('#mb-addfromhttp').click(function() {
         try { m_Config.onOpen(); }
         catch (Ignored) { }
         $('#mb-divider, #mb-search, #mb-source').hide();
         $('#mb-date, #mb-time').parent().hide();
         $('#mb-title, #mb-options').show();
         $(this).text('Search');
         loadEntries($('#mini-browser'), true);
      });
   }

   return {
      'create': create
   }
}();
