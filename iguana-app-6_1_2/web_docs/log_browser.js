/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

/////////////////////////////////////////////////
// Javscript functions for the log browser.      //
// Make sure to include log_browser_js.cs in the //
// cs template if using this file.               //
///////////////////////////////////////////////////

var InputTimeFormatText = "Enter a date or time";

//
// Parsed Table View Stuff
//

function raiseAlert(Message)
{
   $('#alert-dialog').dialog('open');
   $('#alert-dialog-text').text(Message);
}

function initializeHelp(me,e)
{
   if ($("#helpTooltipDiv").is(":hidden") || document.getElementById('helpTooltipId').value != me.id)
   {
      document.getElementById('helpTooltipTitle').innerHTML = me.title;
      document.getElementById('helpTooltipBody').innerHTML = me.rel;
      document.getElementById('helpTooltipId').value = me.id;

      // Get inner width of browser window
      var windowWidth = 0;
      if( typeof( window.innerWidth ) == 'number' ) // Non-IE
      {
         windowWidth = window.innerWidth;
      }
      else if( document.documentElement
               && ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) //IE 6+ in 'standards compliant mode'
      {
         windowWidth = document.documentElement.clientWidth;
      }
      else if( document.body && ( document.body.clientWidth || document.body.clientHeight ) ) // IE4 compatible
      {
         windowWidth = document.body.clientWidth;
      }

      // Get mouse position
      var mouseX = 0;
      var mouseY = 0;
      if (e.pageX || e.pageY) // This doesn't work in IE6
      {
         mouseX = e.pageX;
         mouseY = e.pageY;
      }
      else if (e.clientX || e.clientY) // This works in IE6
      {
         mouseX = e.clientX + document.body.scrollLeft;
         mouseY = e.clientY + document.body.scrollTop;
      }

      // Position help modal box
      var posX = mouseX + 50;
      var posY = mouseY - 50;
      var boxWidth = parseInt($("#helpTooltipDiv").css("width"));
      if (posX + boxWidth > windowWidth)
      {
         posX = mouseX - boxWidth - 75;
      }
      $("#helpTooltipDiv").css("left", posX);
      $("#helpTooltipDiv").css("top", posY);

      $("#helpTooltipDiv").fadeIn("slow");
   }
   else
   {
      clearTool();
   }
}

function clearTool()
{
   $("#helpTooltipDiv").fadeOut("slow");
}

//
// General Stuff
//

function elementIdToMessageId(Id)
{
   if (Id == null || Id.indexOf('entry_row_') != 0) return null;
   return Id.substring(10);
}

function sanitizedMessage(Message)
{
   return Message.replace(/\r\n|\r|\n/g, '\r');
}

function htmlEscape(Value)
{
   return Value.replace(/&/g, '&amp;')
               .replace(/>/g, '&gt;')
               .replace(/</g, '&lt;')
               .replace(/"/g, '&quot;');
}

function getLogUsageStatistics()
{
   var ResponseText;
   AJAXsynchronousPost('get_usage_statistics', '',
      function(Data){
         ResponseText =  Data;
      },
      function(){
         ResponseText = "Error retrieving statistics";
      });
   return ResponseText;
}

var DelayedFunctions = {};
function delay(Func, Period)
{
   var OldTimeout = DelayedFunctions[Func];
   if( OldTimeout )
   {
      clearTimeout(OldTimeout);
   }

   var Action = function() { DelayedFunctions[Func] = undefined; Func(); };
   DelayedFunctions[Func] = setTimeout(Action, Period);
}

function setSelectType() {
   var SelectType = LOGCStypeFilter;

   if (SelectType == null) {
      SelectType = '';
   }
   if (SelectType != '') {
      var SelectTypeArray = SelectType.split(',');
      var Element = document.getElementById('Type');
      Element.options[0].selected = false;
      for (var optionsIndex=1; optionsIndex < Element.options.length; optionsIndex++)  {
         // jQuery.inArray() is returning undefined instead of -1 in IE8.
         var Index = jQuery.inArray(Element.options[optionsIndex].value, SelectTypeArray);
         if ( (typeof(Index) != 'undefined') && (Index > -1) ) {
            Element.options[optionsIndex].selected = true;
         }
      }
   }
}

function saveSearchLink(type)
{

   var BookmarkLink = document.getElementById('bookmarkLink');
   if (type == "search")
   {
      var Link = document.URL.replace(/(log_browse|logs.html).*/,'') + 'logs.html?' + getFormQuery(document.getElementById('FilterForm'), '', {});
      var Description = 'You can use the following URL to return to and repeat this search at any time.';
       var Title = 'Bookmark Search';
   }
   else
   {
      var Link = document.URL.replace(/(log_browse|logs.html).*/,'') + 'export_tqu_log?' + getFormQuery(document.getElementById('FilterForm'), '', {});
      var Description = 'You can use the following URL to return to and repeat this export at any time.'
      var Title = 'Bookmark Export';
   }

   document.getElementById('bookmark-text').innerHTML = Description;
   document.getElementById('bookmark-dialog').title = Title;
   BookmarkLink.value = Link;
   $('#bookmark-dialog').dialog('open');
   BookmarkLink.focus();
   BookmarkLink.select();


}


function getFormQuery(Form, Delimiter, Replacements)
{
   var Query = '';
   for(var ElementIndex=0; ElementIndex < Form.elements.length; ++ElementIndex)
   {
      var Element = Form.elements[ElementIndex];

      if( (Element.type == 'checkbox'|| Element.value) && Replacements[Element.name] === undefined )
      {
         if ( Element.id == 'Type'  || Element.id == 'DequeueSourceName')
         {
            var optionsDelimiter = '';
            Replacements[Element.name] = '';
            for (var optionsIndex=0; optionsIndex < Element.options.length; optionsIndex++)
            {
               if (Element.options[optionsIndex].selected)
               {
                  // encoding the value here because I don't want the ',' encoded
                  Replacements[Element.name] += optionsDelimiter + encodeURIComponent(Element.options[optionsIndex].value);
                  if (Element.id == 'Type')
                  {
                     optionsDelimiter = ',';
                  }
                  else
                  {
                     optionsDelimiter = '%0B'; //use this unprintable character since names may have commas
                  }
               }
            }
         }
         else
         {
            //checkboxes are special...this is for cross browser consistency, we can't use
            //value, have to use checked
            if (Element.type == 'checkbox')
            {
               if (Element.checked)
               {
                  Replacements[Element.name] = 'true';
               }
            }
            // Same thing for radio buttons.  Like checkboxes, but we need the value of the checked one.
            else if(Element.type == 'radio')
            {
               if (Element.checked)
               {
                  Replacements[Element.name] = encodeURIComponent(Element.value);
               }
            }
            else
            {
               // encoding the value here because I don't want the ',' encoded
               Replacements[Element.name] = encodeURIComponent(Element.value);
            }
         }
      }
   }

   for(var Key in Replacements)
   {
      if( Replacements[Key] !== null && Replacements[Key] != encodeURIComponent(InputTimeFormatText) && Key.indexOf("_local") == -1)
      {
         Query += Delimiter + encodeURIComponent(Key) + '=' + Replacements[Key];
         Delimiter = '&';
      }
   }
   return Query;
}

Array.prototype.indexOf = function(Element)
{
   for(var Index in this)
   {
      if( this[Index] == Element )
      {
         return Index;
      }
   }

   return undefined;
};

//
// Exporting
//

function exportSearch()
{
   if( document.getElementsByName('Export.Related')[0].checked )
   {
      exportRelated();
   }
   else
   {
      doExport();
   }
}

function exportRelated(OnReady)
{
   if(!OnReady) OnReady = doExport;

   var ButtonView   = document.getElementById('exportButtonView');
   var ProgressView = document.getElementById('exportProgressView');
   var ProgressArea = document.getElementById('exportProgressArea');
   var ProgressBar  = document.getElementById('exportProgress');

   ButtonView.style.display = 'none';
   ProgressBar.style.width = '0px';
   ProgressView.style.display = '';

   function hideProgress()
   {
      ProgressView.style.display = 'none';
      ButtonView.style.display = '';
   }

   function watchRelatedExport(PlanId, OnFinished)
   {
      AJAXpost('check_tqu_plan', 'Export.Plan=' + encodeURIComponent(PlanId),
         function(Data, ContentType) {
            if( ContentType.match('text/plain') )
            {
               if( /^OK RUNNING (\d+\.\d+)/.test(Data) )
               {
                  var Progress = Data.match(/^OK RUNNING (\d+\.\d+)/)[1];

                  ProgressBar.style.width =
                     Math.floor(ProgressArea.clientWidth * Progress) + 'px';

                  setTimeout(function() { watchRelatedExport(PlanId, OnFinished); }, 100);
               }
               else if( /^OK READY/.test(Data) )
               {
                  OnReady(OnFinished, function() {
                     watchRelatedExport(PlanId, OnFinished);
                  });
               }
               else if( /^OK EMPTY/.test(Data) )
               {
                  OnFinished();
                  raiseAlert('There are no messages related to these entries.');
               }
               else if( /^OK LOST/.test(Data) )
               {
                  OnFinished();
                  raiseAlert('Export failed.  Please try again.');
               }
               else
               {
                  OnFinished();
                  raiseAlert(Data.replace(/^ERROR\s*/, ''));
               }
            }
            else
            {
               showTimeOutMessage(function() { watchRelatedExport(PlanId, OnFinished); });
            }
         },
         function(Error) {
            showDisconnectMessage(Error, function() { watchRelatedExport(PlanId, OnFinished); });
         }
      );
   }

   AJAXpost('plan_tqu_export', getFormQuery(document.getElementById('FilterForm'), '', {}),
      function(Data, ContentType) {
         if( ContentType.match('text/plain') )
         {
            if( /^OK RUNNING (-?\d+)/.test(Data) )
            {
               var PlanId = Data.match(/^OK RUNNING (-?\d+)/)[1];
               document.getElementsByName('Export.Plan')[0].value = PlanId;

               watchRelatedExport(PlanId, function() {
                  hideProgress();
                  document.getElementsByName('Export.Plan')[0].value = '';
               });
            }
            else
            {
               hideProgress();
               raiseAlert(Data.replace(/^ERROR\s*/, ''));
            }
         }
         else
         {
            showTimeOutMessage(function() { exportRelated(OnReady); });
         }
      },
      function(Error) {
         showDisconnectMessage(Error, function() { exportRelated(OnReady); });
      }
   );
}

function doExport(OnFinished, OnRetry) {
   OnRetry = OnRetry || function() {
      doExport(OnFinished);
   };

   AJAXpost('/login_check', '',
      function(Data, ContentType) {
         if(!ContentType.match('application/json'))
         {
            showTimeOutMessage(OnRetry);
            return;
         }
         var Response = {};
         try { Response = $.parseJSON(Data); } catch(Error) {}
         if(Response.LoginOkay)
         {
            // Adding a majik number to the export url because IE6 doesn't like no-cache with SSL
            var TheForm = $("#FilterForm");
            fixField('DequeueSourceName', TheForm);
            fixField('Type', TheForm);
            document.FilterForm.action="/export_tqu_log" + '?Magic=' + encodeURIComponent(Math.random());
            document.FilterForm.submit();
            unFixField('Type', TheForm);
            unFixField('DequeueSourceName', TheForm);
            document.FilterForm.action="/log_browse";

            if( OnFinished )
            {
               OnFinished();
            }
         }
         else
         {
            showTimeOutMessage(OnRetry, Response.ErrorText);
         }
      },
      function(Error) {
         showDisconnectMessage(Error, OnRetry);
      }
   );
}

// These take values from multiple select fields and concatenate them into single strings, in much the same way getFormQuery does.
// Here, because we actually submit the form, we have to catch and edit it on the fly. Yuck.
function fixField(FieldId, Form) {
   var MultiSelect = $("select#" + FieldId);
   if (! MultiSelect.length) {
      return;
   }
   var Vals = MultiSelect.val() || [''];
   var StringVal = Vals.join(decodeURIComponent('%0B'));
   var FieldName = MultiSelect.attr('name');
   MultiSelect.attr('name', 'TemporarilyNotInteresting' + FieldName);
   var NewField = $('<input type="hidden" id="TemporarilyInteresting' + FieldName + '" name="' + FieldName + '" value="' + StringVal + '" />');
   Form.append(NewField);
}

function unFixField(FieldId, Form) {
   var MultiSelect = $("select#" + FieldId);
   if (! MultiSelect.length) {
      return;
   }
   var StringField = $("input#TemporarilyInteresting" + FieldId);
   MultiSelect.attr('name', StringField.attr('name'));
   StringField.remove();
}

//
// Highlighting
//

var HighlightEntry = { Position:null, Date:null, MessageId:null };
var FirstMessageId = null;

function rowHoverIn(Row)
{
   var JRow = $('#'+Row.id);
   if      (JRow.hasClass('highlight')) JRow.addClass('highlightHover');
   else if (JRow.hasClass('second'   )) JRow.addClass('secondHover'   );
   else if (JRow.hasClass('first'    )) JRow.addClass('firstHover'    );
}

function rowHoverOut(Row)
{
   var JRow = $('#'+Row.id);
   JRow.removeClass('highlightHover');
   JRow.removeClass('secondHover'   );
   JRow.removeClass('firstHover'    );
}

// Send a quick update to the server letting it know which row we've selected.
//
var CurrentEntryUpdateTimeoutId = null;
function sendCurrentEntryUpdate(MessageId)
{
   clearTimeout(CurrentEntryUpdateTimeoutId);

   // We use a small timeout to prevent sending too many requests to the server
   // (if quickly scrolling through entries), which can cause race conditions.
   CurrentEntryUpdateTimeoutId = setTimeout(function()
   {
      var OnRetry = function(){ sendCurrentEntryUpdate(MessageId); };

      AJAXsynchronousPost('/update_current_entry', 'LogBrowserGuid=' + LOGCSlogBrowserGuid + '&MessageId=' + MessageId,
         function(Data, ContentType) {
            if( ContentType.match('application/json') && JSON.parse(Data) ) {
               // Response could contain an error message, but there's not much
               // we can do.
            } else {
               showTimeOutMessage(OnRetry);
            }
         },
         function(Error) {
            showDisconnectMessage(Error, OnRetry);
         }
      );
   }, 100);
}

function setHighlightEntry(MessageId)
{
   if (MessageId == null) return null;

   if (!LOGCSisChildWindow)
   {
      sendCurrentEntryUpdate(MessageId);
   }

   var Parts = MessageId.split('-');
   if (HighlightEntry.MessageId != null && HighlightEntry.MessageId != MessageId)
   {
      var Row = document.getElementById('entry_row_'+HighlightEntry.MessageId);
      if (Row != null)
      {
         var JRow = $('#'+Row.id);
         if (JRow.hasClass('highlightHover'))
         {
            if      (JRow.hasClass('second')) JRow.addClass('secondHover');
            else if (JRow.hasClass('first' )) JRow.addClass('firstHover' );
         }
         JRow.removeClass('highlightHover');
         JRow.removeClass('highlight'     );
      }
   }
   HighlightEntry.MessageId = MessageId;
   HighlightEntry.Position  = Parts[1];
   HighlightEntry.Date      = Parts[0].replace(/(\d{4})(\d{2})(\d{2})/, '$1/$2/$3');
   var Row = document.getElementById('entry_row_'+HighlightEntry.MessageId);
   if (Row != null)
   {
      var JRow = $('#'+Row.id);
      JRow.addClass('highlight');
      if (JRow.hasClass('firstHover') || JRow.hasClass('secondHover')) JRow.addClass('highlightHover');
      JRow.removeClass('secondHover');
      JRow.removeClass('firstHover' );
   }
   return Row;
}

function getHighlightedRow()
{
   var Row = document.getElementById('entry_row_'+HighlightEntry.MessageId);
   if (Row != null || LogTable.rows.length <= 2) return Row;


   Row = document.getElementById('entry_row_'+FirstMessageId);
   if (Row == null) Row = LogTable.rows[1];
   setHighlightEntry(elementIdToMessageId(Row.id));
   scrollTo(Row);
   return null;
}

function scrollTo(Row)
{
   if (Row == null) return;
   var RowBeg = Row.offsetTop - 2;
   var RowEnd = Row.offsetTop + Row.offsetHeight + 2;
   var ScrBeg = LogArea.scrollTop;
   var ScrEnd = LogArea.scrollTop + LogArea.offsetHeight;
   var LimBeg = 0;
   var LimEnd = LogArea.scrollHeight - LogArea.offsetHeight;
   if (RowEnd > ScrEnd) LogArea.scrollTop = Math.max(LimBeg, Math.min(LimEnd, RowEnd-LogArea.offsetHeight));
   if (RowBeg < ScrBeg) LogArea.scrollTop = Math.max(LimBeg, Math.min(LimEnd, RowBeg));
}

//
// Round-tripping and Error Handling
//

function getThisUrl(iExtraVars)
{
   var ExtraVars = iExtraVars ? iExtraVars : {};

   if( RelatedEntry && document.getElementById('contextWarning').style.display != 'none' )
   {
      ExtraVars['Position'] = RelatedEntry.Position;
      ExtraVars['Date']     = RelatedEntry.Date;
      ExtraVars['RefMsgId'] = null;
      ExtraVars['OnLoad']   = 'showRelated();' + ExtraVars['OnLoad'];
   }
   else if( CurrentEntry )
   {
      ExtraVars['Position'] = CurrentEntry.Position;
      ExtraVars['Date']     = CurrentEntry.Date;
   }

   return 'log_browse?' + getFormQuery(document.getElementById('FilterForm'), '', ExtraVars);
}

function showTimeOutMessage(Retry, Message)
{
   MiniLogin.show(Message || 'Your Iguana Session has Expired', Retry);
}

function showDisconnectMessage(Details, Retry)
{
   MiniLogin.show('Iguana is not Responding', Retry);
}

var ShownMissingQueueMetaWarning = false;
function warnMissingQueueMeta()
{
   if( !ShownMissingQueueMetaWarning )
   {
      ShownMissingQueueMetaWarning = true;
      raiseAlert('Some log bookeeping data is missing.\n' +
            'Until Iguana is restarted, some entries will have unknown sources.');
   }
}

//
// View Switching
//

function show(Id)
{
   document.getElementById(Id).style.display = '';
}

function hide(Id)
{
   var Element = document.getElementById(Id);
   if (Element)
   {
      Element.style.display = 'none';
   }
}

function visible(Id)
{
   return document.getElementById(Id).style.display != 'none';
}

function getViewParams(Entry)
{
   return 'Position=' + encodeURIComponent(Entry.Position)
           + '&Date=' + encodeURIComponent(Entry.Date);
}

function getRelatedMessageParams(Entry)
{
   var Parts = Entry.RefMsgId.split('-');

   return getViewParams({
      Position: Parts[1],
      Date:     Parts[0].replace(/(\d{4})(\d{2})(\d{2})/, '$1/$2/$3')
   });
}

function showRelatedButton()
{
   var RelatedLink = document.getElementById('entryRelated');

   if (RelatedLink)
   {
      if( CurrentEntry.RefMsgId == '0-0' )
      {
         RelatedLink.style.display = 'none';
      }
      else
      {
         RelatedLink.style.display = '';
      }
   }
}

function showResubmitButton()
{
   var ResubmitLink = document.getElementById('entryResubmit');

   if (ResubmitLink)
   {
      if( CurrentEntry.IsResubmittable ||
          CurrentEntry.Type == 'Error' && CurrentEntry.RefMsgId != '0-0' )
      {
         ResubmitLink.style.display = '';
      }
      else
      {
         ResubmitLink.style.display = 'none';
      }
   }
}

function toggleDropdown(DropdownId, DropdownButtonId, Mode)
{
   var Dropdown = $('#' + DropdownId);
   Dropdown.stop(true, true);

   var DropMenuDown = !visible(DropdownId);

   document.getElementById(DropdownButtonId).src = LOGCSdropdownButtonImgSrc(Mode, DropMenuDown);

   if (DropMenuDown)
   {
      Dropdown.slideDown('fast');
   }
   else
   {
      Dropdown.slideUp('fast');
   }
}

function hideDropdown(DropdownId, DropdownButtonId, Mode)
{
   var Dropdown = $('#' + DropdownId);
   Dropdown.stop(true, true);
   Dropdown.css('display', 'none');

   document.getElementById(DropdownButtonId).src = LOGCSdropdownButtonImgSrc(Mode, false);
}

function updateEntryViewControlImage(Mode, CanParse, CanPreviewPlugin, CanPreviewMapper, DropdownButtonId, DropdownMenuId)
{
   document.getElementById(DropdownButtonId).src = LOGCSdropdownButtonImgSrc(Mode, false);
   document.getElementById(DropdownMenuId).style.display = 'none';

   $('#' + DropdownMenuId + ' *').each(function()
   {
      var Option = $(this);
      var Disabled = false;
      if (Option.hasClass('parseOption'))
      {
         this.style.display = CanParse ? '' : 'none';
         Disabled = CanParse && CurrentEntry.Deleted;
      }
      else if (Option.hasClass('messageOption'))
      {
         this.style.display = CurrentEntry.IsResubmittable ? '' : 'none';
         Disabled = CurrentEntry.IsResubmittable && CurrentEntry.Deleted;
      }
      else if (Option.hasClass('pluginOption'))
      {
         this.style.display = CanPreviewPlugin ? '' : 'none';
         Disabled = CanPreviewPlugin && CurrentEntry.Deleted;
      }
      else if (Option.hasClass('mapperOption'))
      {
         this.style.display = CanPreviewMapper ? '' : 'none';
         Disabled = CanPreviewMapper && CurrentEntry.Deleted;
      }

      Disabled ? Option.addClass('disabled') : Option.removeClass('disabled');

      Option.children('img').attr('src', LOGCSdropdownOptionImgSrc(Option, Option.hasClass(Mode), Disabled));
   });
}

function updateEntryViewControl()
{
   updateEntryViewControlImage
   (
      CurrentViewMode,
      CurrentEntry.CanParse,
      false,
      false,
      'logViewSelectDropdownButton',
      'logViewSelectDropdown'
   );
}

var ParsableDestinationChannels;
function showPreviewParseButton()
{
   var DestinationList = document.getElementById('Destination');

   updateEntryViewControlImage
   (
      CurrentPreviewMode,
      sourceHasParsers(DestinationList.value),
      sourceHasPluginDestination(DestinationList.value),
      LOGsourceHasMapperDestination(DestinationList.value),
      'logPreviewSelectDropdownButton',
      'logPreviewSelectDropdown'
   );
}

function hideViews()
{
   hide('entryView');
   hide('logView');
   hide('oldestAndNewestButtons');
}

function resizeInnerViewContents()
{
   // Resize the contents of whatever view is showing.
   SGVonTargetResize();
   TBVonTargetResize();
}

function initResubmitToggles()
{
   var EditLink = document.getElementById('toggleResubmitEditLink');
   var PreviewLink = document.getElementById('toggleResubmitPreviewLink');

   show('resubmitEdit');
   show('resubmitPreview');

   EditLink.innerHTML = '[+] Edit';
   PreviewLink.innerHTML = '[+] Preview';
}

var ResubmitToggleAnimationSpeed = 250;

function toggleResubmitEdit()
{
   var EditLink = document.getElementById('toggleResubmitEditLink');
   var PreviewLink = document.getElementById('toggleResubmitPreviewLink');

   if (visible('resubmitPreview'))
   {
      // Make edit take up entire dialog
      show('resubmitEdit');
      EditLink.innerHTML = '[-] Edit';
      PreviewLink.innerHTML = '[+] Preview';
      hide('resubmitPreview');
      hide('entryPreviewViewControlContainer');
      $('#messageEditor').animate({'height':LOGCSgetAvailableResubmitDialogHeight(true, false)}, ResubmitToggleAnimationSpeed);
   }
   else
   {
      // Make the edit and preview share the dialog
      EditLink.innerHTML = '[+] Edit';
      PreviewLink.innerHTML = '[+] Preview';
      var SharedHeight = LOGCSgetAvailableResubmitDialogHeight(true, true);
      $('#messageEditor').animate({'height':SharedHeight}, ResubmitToggleAnimationSpeed, function()
      {
         $('#resubmitPreview').css('height', SharedHeight);
         $('#resubmitPreview').fadeIn('normal');
         $('#entryPreviewViewControlContainer').fadeIn('normal');
         resizeInnerViewContents();
      });
   }
}

function toggleResubmitPreview(SkipAnimation)
{
   var EditLink = document.getElementById('toggleResubmitEditLink');
   var PreviewLink = document.getElementById('toggleResubmitPreviewLink');

   if (visible('resubmitEdit'))
   {
      // Make preview take up entire dialog
      $('#resubmitPreview').css('height', LOGCSgetAvailableResubmitDialogHeight(false, true));
      PreviewLink.innerHTML = '[-] Preview';
      EditLink.innerHTML = '[+] Edit';
      hide('resubmitPreview');

      // This animation cannot be skipped.
      $('#messageEditor').animate({'height':0}, ResubmitToggleAnimationSpeed, function()
      {
         hide('resubmitEdit');
         $('#resubmitPreview').fadeIn('normal');
         $('#entryPreviewViewControlContainer').fadeIn('normal');
         resizeInnerViewContents();
      });
   }
   else
   {
      // Make the edit and preview share the dialog
      PreviewLink.innerHTML = '[+] Preview';
      EditLink.innerHTML = '[+] Edit';
      var SharedHeight = LOGCSgetAvailableResubmitDialogHeight(true, true);
      show('resubmitEdit');
      if (!SkipAnimation)
      {
         hide('resubmitPreview');
      }

      var onResizeDone = function()
      {
         $('#resubmitPreview').css('height', SharedHeight + 'px');
         if (!SkipAnimation)
         {
            $('#resubmitPreview').fadeIn('normal');
         }
         resizeInnerViewContents();
      }

      if (SkipAnimation)
      {
         $('#messageEditor').css('height', SharedHeight + 'px');
         onResizeDone();
      }
      else
      {
         $('#messageEditor').animate({'height':SharedHeight}, onResizeDone);
      }
   }
}

function highlightResubmitEditorText(Start, Length)
{
   var MessageEditor = document.getElementById('messageEditor');

   if (!visible('resubmitEdit'))
   {
      // First we will have to make sure the message editor is visible.
      toggleResubmitPreview(true);
   }

   if (Start < MessageEditor.value.length)
   {
      if (MessageEditor.setSelectionRange) // most browsers
      {
         MessageEditor.setSelectionRange(Start, Start + Length);
         MessageEditor.focus();
      }
      else if (MessageEditor.createTextRange) // IE
      {
         var Range = MessageEditor.createTextRange();
         Range.move('character', Start);
         Range.moveEnd('character', Length);
         Range.select();
         Range.scrollIntoView();
      }
   }
}

function closeResubmitDialog()
{
   hide('resubmitDialog');
   hide('resubmitFrame');
   $('resubmitPreview').empty();
}

function showCurrentEntry()
{
   if (CurrentViewMode == 'Text')
   {
      showText();
   }
   else if (CurrentViewMode == 'Hex')
   {
      showHex();
   }
   else
   {
      showParsed(CurrentViewMode, 'Preserve');
   }
}

function hideResubmit()
{
   closeResubmitDialog();
   showCurrentEntry();
}

function showResubmit()
{
   if (LOGCScurrentUserCanAdmin)
   {
      if (CurrentEntry.Deleted)
      {
         raiseAlert("This message has been deleted. Undelete it before resubmitting.");
         return;
      }

      // If resubmit preview area exists, clear it out.
      var ResubmitPreview = document.getElementById('resubmitPreview');
      if (ResubmitPreview)
      {
         ResubmitPreview.innerHTML = ''
      }

      initResubmitToggles();
      LOGCSresizeResubmitDialog();

      //default drop-down to current channel
      document.getElementById('Destination').value = CurrentEntry.Channel;

      setupResubmitHelpNote(document.getElementById('Destination'));

      CurrentPreviewMode = '';
      hide('logPreviewSelectDropdownButton');
      hide('logPreviewSelectDropdown');

      var Target = document.getElementById('messageEditor');
      Target.value = 'Loading...';
      ResubmitReady = false;

      var Params;
      if (CurrentEntry.Type == 'Error')
      {
         Params = getRelatedMessageParams(CurrentEntry);
      }
      else
      {
         Params = getViewParams(CurrentEntry);
      }

      var FullLocation = 'log_view_entry';
      var FullParams = Params + '&Raw=1';
      AJAXpostWithId(FullLocation, FullParams, 'LogBrowserViewEntry',
         function(Data, ContentType) {
            if( ContentType.match('application/json') )
            {
               LastEntryData = JSON.parse(Data);
               LastEntryData.Location = FullLocation;
               LastEntryData.Params = FullParams;
               Target.value = LastEntryData.Message;
               ResubmitReady = true;

               showResubmitPreview();

               var OriginalValue = Target.value;
               document.getElementById('revertButton').onclick = function() {
                  if( Target.value != OriginalValue &&
                      confirm('Are you sure you want to undo all your changes?') )
                  {
                     Target.value = OriginalValue;
                  }
               };
            }
            else
            {
               showTimeOutMessage(function() { showResubmit(Whence); });
            }
         },
         function(Error) {
            showDisconnectMessage(Error, function() { showResubmit(Whence); });
         }
      );
   }
}

function showPreview(Refresh)
{
   if (CurrentPreviewMode == 'Hex')
   {
      showHexPreview(Refresh);
   }
   else if (CurrentPreviewMode == 'PluginOutput')
   {
      showPluginOutputPreview(Refresh);
   }
   else if (CurrentPreviewMode == 'MapperOutput')
   {
      LOGshowMapperOutputPreview(Refresh);
   }
   else if (CurrentPreviewMode != 'Text')
   {
      showParsedPreview(CurrentPreviewMode, 'Preserve', Refresh);
   }
   else
   {
      showTextPreview(Refresh);
   }
}

function setResubmitPreviewProgressVisible(IsVisible)
{
   $('#resubmitPreviewProgress').css('visibility', IsVisible ? '' : 'hidden');
}

var ResubmitPreviewRequestInProgress = false; // true if there is a request in progress.
var ResubmitPreviewRequestSchedule = false; // true if we attempted to do another request, but there was already one in progress.

function onResubmitMessageEditorChange()
{
   setResubmitPreviewProgressVisible(true);
   if (!ResubmitPreviewRequestInProgress)
   {
      ResubmitPreviewRequestSchedule = false;
      showPreview(true);
      ResubmitPreviewRequestInProgress = true;
   }
   else
   {
      ResubmitPreviewRequestSchedule = true;
   }
}

function onResubmitPreviewComplete()
{
   ResubmitPreviewRequestInProgress = false;
   if (ResubmitPreviewRequestSchedule)
   {
      onResubmitMessageEditorChange();
   }
   else
   {
      setResubmitPreviewProgressVisible(false);
   }
}

function showDefaultResubmitPreviewMode(DestinationChannel)
{
   if(sourceHasParsers(DestinationChannel))
   {
      showParsedPreview('SQL', 'Preserve', false);
   }
   else if (sourceHasPluginDestination(DestinationChannel))
   {
      showPluginOutputPreview(false);
   }
   else if (LOGsourceHasMapperDestination(DestinationChannel))
   {
      LOGshowMapperOutputPreview(false);
   }
   else
   {
      showParsedPreview('SegmentMessage', 'Preserve', false);
   }
}

function showResubmitPreview()
{
   if (!LOGCScurrentUserCanAdmin)
   {
      raiseAlert('Only administrators can resubmit or forward messages.');
   }
   else
   {
      show('logPreviewSelectDropdownButton');
      var Destination = document.getElementById('Destination');
      if (CurrentPreviewMode == '')
      {
         showDefaultResubmitPreviewMode(Destination.value);
      }
      else
      {
         showPreview(false);
      }
   }
}

function setupResubmitHelpNote(DestinationSelectEl)
{
   var ResubmitHelpNote = '';
   if (DestinationSelectEl.selectedIndex != -1)
   {
      //show help note if needed
      var ChannelInfo = LOGCSchannelInfo[DestinationSelectEl.value];
      if (ChannelInfo && ChannelInfo.DestType == 'To Database')
      {
         ResubmitHelpNote = 'If the VMD file';
         if (ChannelInfo.FilterAfterLogging && ChannelInfo.HasFilter)
         {
            ResubmitHelpNote += '(s) for the filter or';
         }
         ResubmitHelpNote += ' for this database channel has been edited, the channel must be restarted before resubmitting the message.';
      }
      else if (ChannelInfo && ChannelInfo.FilterAfterLogging && ChannelInfo.HasFilter)
      {
         ResubmitHelpNote = 'If the VMD file for the filter has been edited, the channel must be restarted before resubmitting the message.';
      }
   }
   $('#resubmit_dest_help_note').html( ResubmitHelpNote );
}

function onPreviewDestinationChanged(Element)
{
   if (Element.selectedIndex != -1)
   {
      var Destination = document.getElementById('Destination');
      setupResubmitHelpNote(Element);
      if (CurrentPreviewMode == 'Text')
      {
         showTextPreview(false);
      }
      if (CurrentPreviewMode == 'Hex')
      {
         showHexPreview(false);
      }
      else if (CurrentPreviewMode == 'SegmentMessage')
      {
         showParsedPreview('SegmentMessage', 'Preserve', false);
      }
      else if (CurrentPreviewMode == 'PluginOutput')
      {
         if (sourceHasPluginDestination(Element.value))
         {
            showPluginOutputPreview(false);
         }
         else
         {
            showDefaultResubmitPreviewMode(Element.value);
         }
      }
      else if (CurrentPreviewMode == 'MapperOutput')
      {
         if (LOGsourceHasMapperDestination(Element.value))
         {
            LOGshowMapperOutputPreview(false);
         }
         else
         {
            showDefaultResubmitPreviewMode(Element.value);
         }
      }
      else // CurrentPreviewMode == <one of the parsed preview modes>
      {
         if (sourceHasParsers(Element.value))
         {
            showParsedPreview(CurrentPreviewMode, 'Preserve', false);
         }
         else
         {
            showDefaultResubmitPreviewMode(Element.value);
         }
      }
   }
}

var RelatedEntry;
function showRelated()
{
   setHighlightEntry(CurrentEntry.MessageId);
   RelatedEntry = CurrentEntry;

   document.getElementById('contextWarningFrame').style.height =
      document.getElementById('contextWarning').style.height =
      document.getElementById('side_table').offsetHeight + 'px';

   document.getElementsByName('RefMsgId')[0].value = CurrentEntry.RefMsgId;
   document.getElementById('contextWarningFrame').style.display = 'block';
   document.getElementById('contextWarning').style.display = 'block';

   showLog();
   clearSearch();
   setFinished('forward', false, 'Searching...');
   showMore('forward');
}

function hideRelated()
{
   document.getElementsByName('RefMsgId')[0].value = '';
   document.getElementById('contextWarning').style.display = 'none';
   document.getElementById('contextWarningFrame').style.display = 'none';

   showLog();
   clearSearch();
   setSearchState('forward', { Position: RelatedEntry.Position - 1, Date: RelatedEntry.Date });
   setSearchState('reverse', { Position: RelatedEntry.Position,     Date: RelatedEntry.Date });
   setFinished('forward', false, 'Searching...');
   setFinished('reverse', false);
   showMore('forward');
}

var ResubmitReady = false;
function resubmit(BackupPlan)
{
   if(!ResubmitReady)
   {
      // Say nothing, but don't let it resubmit "loading..." either.
      return;
   }

   if (CurrentEntry.Deleted)
   {
      raiseAlert("This message has been deleted. Undelete it before resubmitting.");
      return;
   }

   var Message     = document.getElementById('messageEditor').value.replace(/\r\n|\r|\n/g, '\r');
   var Destination = document.getElementById('Destination').value;

   if( !Destination )
   {
      raiseAlert('You must specify a destination.');
      return;
   }

   var RefMsgId = CurrentEntry.RefMsgId;
   if( RefMsgId === '0-0' )
   {
      RefMsgId = CurrentEntry.Date.replace(/\//g,'') + '-' + CurrentEntry.Position;
   }

   closeResubmitDialog();
   hideViews();

   var ProgressArea = document.getElementById('workingProgress');
   $('#workingProgressDiv').text('Resubmitting...');
   ProgressArea.style.display = '';
   resize(ProgressArea);

   AJAXpost('resubmit_message', 'Message=' + encodeURIComponent(Message)
                            + '&RefMsgId=' + encodeURIComponent(RefMsgId)
                           + '&RequestId=' + encodeURIComponent(Math.random())
                         + '&Destination=' + encodeURIComponent(Destination),
      function(Response) {
         if( !/^OK /.test(Response) )
         {
            raiseAlert(Response);
            ProgressArea.style.display = 'none';
            BackupPlan();
         }
         else
         {
            if( /^OK \d{8}-\d+/.test(Response) )
            {
               var MessageId = Response.replace(/^OK (\d{8}-\d+).*/, '$1');
               CurrentEntry = {
                  Date:     MessageId.replace(/^(\d{4})(\d{2})(\d{2})-.*/, '$1/$2/$3'),
                  Position: MessageId.replace(/.*-(\d+)$/, '$1'),
                  RefMsgId: MessageId
               };
               setHighlightEntry(MessageId);
            }
            ProgressArea.style.display = 'none';
            showRelated();
         }
      },
      function(Error) {
         ProgressArea.style.display = 'none';
         showDisconnectMessage(Error, BackupPlan);
      }
   );

   // TODO: Indicate that something's happening; disable the resubmit button.
}

function showLog()
{
   // Cancel any pending requests to display an entry, since it will just be
   // wasted computation when they finish.
   AJAXabortRequestById('LogBrowserViewEntry');

   hideViews();
   show('logView');
   show('oldestAndNewestButtons');
   resize(LogArea);

   CurrentEntry = null;

   document.getElementById('searchMismatch').style.display = 'none';

   startPollingForward();  // Restart polling.
   onScrollLogArea();      // Restart search.

   scrollTo(getHighlightedRow()); // Fix for Safari
}

function setWrapText(Wrap, IsPreview)
{
   var WrapIcon = document.getElementById(IsPreview ? 'previewWrapText' : 'wrapText');

   WrapIcon.src = WrapIcon.src.replace(/(no)?wrap/, Wrap ? 'wrap' : 'nowrap');
   WrapIcon.onclick = function() { setWrapText(!Wrap, IsPreview); doWrapText(IsPreview); }
}

function clearEntryView(PleaseWaitText)
{
   var Target = document.getElementById('entryArea');

   //ensure the entry view is showing
   hideViews();
   show('entryView');
   Target.innerHTML = '<div class="entryViewPleaseWaitBar">' + PleaseWaitText + '</div>';
}

// Call before updateCurrentEntryView, which resizes based on whether or not the
// reparse warning is showing.
function updateCurrentEntryReparseWarning(WarningText)
{
   var ReparseWarningDiv = document.getElementById('reparseWarning');
   ReparseWarningDiv.style.display = (WarningText != '') ? '' : 'none';
   ReparseWarningDiv.innerHTML = WarningText;
}

function updateCurrentEntryView(Direction)
{
   if (CurrentEntry)
   {
      showRelatedButton();
      showResubmitButton();
      $('#entryToMapper').toggle(CurrentEntry.IsResubmittable);

      //description, important that this is first because it sets up elements for subsequent calls
      if (Direction == 'next')
      {
         TOOLtooltipClose();
         DescriptionAreaControl.previous(CurrentEntry.Description);
      }
      else if (Direction == 'previous')
      {
         TOOLtooltipClose();
         DescriptionAreaControl.next(CurrentEntry.Description);
      }
      else
      {
         DescriptionAreaControl.setContent(CurrentEntry.Description);
      }
      DescriptionAreaControl.resizeFrames();

      //description date
      $('.clsExpandedEntryDate',$('#entryDescription') ).html( formatDate(CurrentEntry.Date, CurrentEntry.FormattedDate) );

      //marked
      LOGCSshowMarkButton();
      var jDescriptionIconCells = $('.clsDescriptionIconCell', $('#entryDescription') );
      if (CurrentEntry.Type == 'Error')
      {
         var MarkedIconSrc = (CurrentEntry.Acknowledged ? MsgFailureAckIconSrc : MsgFailureIconSrc );
         $('.log_type_icon', jDescriptionIconCells ).css( 'background-image', 'url(\'' + MarkedIconSrc + '\')' );
         $('#type-icon-'+CurrentEntry.MessageId).css( 'background-image', 'url(\'' + MarkedIconSrc + '\')' );
      }

      //resubmit img
      var ResubmitImage = document.getElementById('entryResubmit');
      if (ResubmitImage)
      {
         ResubmitImage.src = (CurrentEntry.Deleted ? ResubmitIconGreySrc : ResubmitIconSrc);
      }

      //view control dropdown menu
      updateEntryViewControl();

      //"text wrap" button
      var IsViewWrappable = CurrentViewMode == 'Text' || CurrentViewMode == 'SQL';
      document.getElementById('entryWrapTextControl').style.display = (IsViewWrappable ? '' : 'none');
      document.getElementById('entryWrapTextControlGrey').style.display = (IsViewWrappable ? 'none' : '');

      var Target = document.getElementById('entryArea');

      //deleted
      if (document.getElementById('entryDelete') && document.getElementById('entryUndelete'))
      {
         var toggleFunc1 = (CurrentEntry.Deleted ? hide : show);
         var toggleFunc2 = (CurrentEntry.Deleted ? show : hide);

         toggleFunc1('entryDelete');
         toggleFunc2('entryUndelete');
      }

      Target.className = ( CurrentEntry.Deleted ? 'log_entry_area_deleted' : 'log_entry_area' );

      //set the entry area's size
      resize(Target);

      updateCurrentEntryDequeuePosition();

      var len = $('.delete_overlay', jDescriptionIconCells).length;
      $('.delete_overlay', jDescriptionIconCells).css('display',(CurrentEntry.Deleted ? '' : 'none'));
      $('#delete-overlay-type-icon-'+CurrentEntry.MessageId).css('display',(CurrentEntry.Deleted ? '' : 'none'));

      $('#first-cell-'+CurrentEntry.MessageId).each( function(){ this.className = (CurrentEntry.Deleted ? 'preview_deleted' : 'preview') } );
      $('#second-cell-'+CurrentEntry.MessageId).each( function(){ this.className = (CurrentEntry.Deleted ? 'preview_deleted' : 'preview') } );
   }
}

function updateCurrentEntryPreviewView()
{
   showPreviewParseButton();

   //"text wrap" button
   var IsViewWrappable = CurrentPreviewMode == 'Text' || CurrentPreviewMode == 'SQL' || CurrentPreviewMode == 'MapperOutput';
   document.getElementById('previewWrapTextControl').style.display = (IsViewWrappable ? '' : 'none');
   document.getElementById('previewWrapTextControlGrey').style.display = (IsViewWrappable ? 'none' : '');
}

function doWrapText(IsPreview)
{
   var WrapIcon = document.getElementById(IsPreview ? 'previewWrapText' : 'wrapText');

   // Wrap/unwrap text view
   var RootContainer = IsPreview ? $('#resubmitDialog') : $('#entryArea');
   RootContainer.find('#textViewContainer').css('white-space', (/nowrap/.test(WrapIcon.src) ? 'nowrap' : 'normal'));

   // Wrap/unwrap SQL view
   //For whatever reason, IE7 doesn't understand the white-space shorthand
   //for this specific container. We just specify the white space and text-wrap separately
   //as per http://www.w3.org/TR/css3-text/#white-space. See #15041.
   //
   //But...that doesn't work in firefox, so we set the white-space anyway...which works
   //everywhere, I guess setting the text-wrap and word-wrap before hand somehow fixes things.
   RootContainer.find('#ParsedViewContainer')
         .css('white-space-collapse','preserve')
         .css('text-wrap', /nowrap/.test(WrapIcon.src) ? 'none' : 'normal')
         .css('word-wrap', /nowrap/.test(WrapIcon.src) ? 'normal' : 'break-word')
         .css('white-space',/nowrap/.test(WrapIcon.src) ? 'nowrap' : 'normal')
}

var CurrentEntry = null;
var CurrentViewMode = 'Text';
var CurrentPreviewMode = '';

function showEntry(Entry, Direction, IsDefaultView)
{
   setHighlightEntry(Entry.MessageId);
   CurrentEntry = Entry;
   if (Iterator)
   {
      Iterator.enable();
   }

   if (IsDefaultView || CurrentViewMode == 'Text')
   {
      showText(Direction)
   }
   else if (CurrentViewMode == 'Hex')
   {
      showHex(Direction);
   }
   else if ((CurrentViewMode == 'SegmentMessage' && CurrentEntry.IsResubmittable) || CurrentEntry.CanParse)
   {
      showParsed(CurrentViewMode, 'Preserve', false, Direction);
   }
   else
   {
      showText(Direction);
   }
}

function displayEntryPleaseWaitBar(Message)
{
   var PleaseWaitDiv = $('#PleaseWaitDiv');
   PleaseWaitDiv.css('width', PleaseWaitDiv.parent().innerWidth()-18);
   PleaseWaitDiv.html(Message ? Message : 'Please Wait...');
   PleaseWaitDiv.css('opacity', 0.75);
   PleaseWaitDiv.css('visibility', '');
}

function hideEntryPleaseWaitBar()
{
   $('#PleaseWaitDiv').fadeTo('fast', 0.0, function()
   {
      $('#PleaseWaitDiv').css('visibility', 'hidden');
   });
}

function updateLocationHashIfUsed()
{
   if (window.location.hash != '')
   {
      window.location.hash = '#' + CurrentViewMode;
   }
}

function loadingMoreLinesDiv(EntryData)
{
   if (!LastEntryData || !LastEntryData.StartLineOffset || !LastEntryData.CountOfLines || !LastEntryData.TotalCountOfLines ||
       (LastEntryData.StartLineOffset + LastEntryData.CountOfLines >= LastEntryData.TotalCountOfLines))
   {
      return '';
   }
   else
   {
      return '<div id="loadingMoreLinesDiv"><img src="/images/spinner-yellow_bg.gif" /> <span>Loading more...</span></div>';
   }
}

function showText(Direction)
{
   CurrentViewMode = 'Text';
   updateLocationHashIfUsed();

   setWrapText(!CurrentEntry.IsResubmittable &&
               CurrentEntry.Type != 'Ack Msg',
               false);

   var Target = document.getElementById('entryArea');

   if (Direction)
   {
      displayEntryPleaseWaitBar();
   }
   else
   {
      clearEntryView('Loading...');
   }
   updateCurrentEntryReparseWarning('')
   updateCurrentEntryView(Direction);

   var FullParams = getViewParams(CurrentEntry);
   var FilterForm = document.getElementById('FilterForm');
   if (FilterForm)
   {
      FullParams += getFormQuery(FilterForm, '&', {});
   }

   var FullLocation = 'log_view_entry';
   AJAXpostWithId(FullLocation, FullParams, 'LogBrowserViewEntry',
      function(Data, ContentType) {
         hideEntryPleaseWaitBar();
         if( ContentType.match('application/json') )
         {
            LastEntryData = JSON.parse(Data);
            LastEntryData.Location = FullLocation;
            LastEntryData.Params = FullParams;
            //var Whitespace   = /nowrap/.test(document.getElementById('wrapText').src) ? 'nowrap' : 'normal';
            var Style        = 'style="font-family:monospace;"';
            var NewEntryText = '<div id="textViewContainer" ' + Style + '>' + LastEntryData.Message + '</div>' + loadingMoreLinesDiv(LastEntryData);
            Target.innerHTML = NewEntryText;
            doWrapText(false);
         }
         else
         {
            showTimeOutMessage(showText);
         }
      },
      function(Error) { showDisconnectMessage(Error, showText); }
   );
}

var TextPreviewLoader;
function showTextPreview(Refresh, PluginOutput)
{
   if (PluginOutput)
   {
      CurrentPreviewMode = 'PluginOutput';
   }
   else
   {
      CurrentPreviewMode = 'Text';
   }

   if (!Refresh)
   {
      setWrapText(false, true);
   }

   var Target = document.getElementById('resubmitPreview');

   if (!Refresh)
   {
      Target.innerHTML = '<div class="entryViewPleaseWaitBar">Processing...</div>';
   }

   var Message = sanitizedMessage(document.getElementById('messageEditor').value);
   var Destination = document.getElementById('Destination');
   var ChannelName = Destination.value;

   setResubmitPreviewProgressVisible(true);
   updateCurrentEntryPreviewView();

   function loadTextPreview()
   {
      if(TextPreviewLoader)
      {
         clearTimeout(TextPreviewLoader);
         TextPreviewLoader = null;
      }

      AJAXpostWithId('log_view_preview',
         '&Message='+encodeURIComponent(Message) + '&Channel='+encodeURIComponent(ChannelName) + (PluginOutput ? '&PluginOutput=true' : ''),
         'LogBrowserViewEntry',
         function(Data, ContentType)
         {
            if( ContentType.match('text/plain') )
            {
               if(Data.match(/^RETRY /) && visible('entryPreview'))
               {
                  TextPreviewLoader = setTimeout(loadTextPreview, 500);
               }

               Data = Data.replace(/^(RETRY|OK|FAIL) /, '');
               Target.innerHTML = '<div id="textViewContainer" style="font-family:monospace; white-space:nowrap">' + Data + '</div>';
               doWrapText(true);
               onResubmitPreviewComplete();
            }
            else
            {
               showTimeOutMessage(showTextPreview);
            }
         },
         function(Error) { showDisconnectMessage(Error, showTextPreview); }
      );
   }

   loadTextPreview();
}

function showPluginOutputPreview(Refresh)
{
   showTextPreview(Refresh, true);
}

function showHex(Direction)
{
   CurrentViewMode = 'Hex';
   updateLocationHashIfUsed();

   var Target = document.getElementById('entryArea');

   if (Direction)
   {
      displayEntryPleaseWaitBar();
   }
   else
   {
      clearEntryView('Loading...');
   }
   updateCurrentEntryReparseWarning('')
   updateCurrentEntryView(Direction);

   var FullLocation = 'log_view_entry';
   var FullParams = getViewParams(CurrentEntry) + '&Hex=1';
   AJAXpostWithId(FullLocation, FullParams, 'LogBrowserViewEntry',
      function(Data, ContentType)
      {
         hideEntryPleaseWaitBar();
         if( ContentType.match('application/json') )
         {
            LastEntryData = JSON.parse(Data);
            LastEntryData.Location = FullLocation;
            LastEntryData.Params = FullParams;
            Target.innerHTML = '<pre><div id="textViewContainer">' + LastEntryData.Message  + '</div></pre>' + loadingMoreLinesDiv(LastEntryData);
         }
         else
         {
            showTimeOutMessage(showHex);
         }
      },
      function(Error) { showDisconnectMessage(Error, showHex); }
   );
}

var HexPreviewLoader;
function showHexPreview(Refresh)
{
   CurrentPreviewMode = 'Hex';

   var Target = document.getElementById('resubmitPreview');

   if (!Refresh)
   {
      Target.innerHTML = '<div class="entryViewPleaseWaitBar">Processing...</div>';
   }

   var Message = sanitizedMessage(document.getElementById('messageEditor').value);
   var Destination = document.getElementById('Destination');
   var ChannelName = Destination.value;

   setResubmitPreviewProgressVisible(true);
   updateCurrentEntryPreviewView();

   function loadHexPreview()
   {
      if(HexPreviewLoader)
      {
         clearTimeout(HexPreviewLoader);
         HexPreviewLoader = null;
      }

      AJAXpostWithId('log_view_preview',
         '&Message='+encodeURIComponent(Message) + '&Channel='+encodeURIComponent(ChannelName) + '&Hex=1',
         'LogBrowserViewEntry',
         function(Data, ContentType)
         {
            if( ContentType.match('text/plain') )
            {
               if(Data.match(/^RETRY /) && visible('entryPreview'))
               {
                  HexPreviewLoader = setTimeout(loadHexPreview, 500);
               }

               Data = Data.replace(/^(RETRY|OK|FAIL) /, '');
               Target.innerHTML = '<pre>' + Data + '</pre>';

               onResubmitPreviewComplete();
            }
            else
            {
               showTimeOutMessage(showHexPreview);
            }
         },
         function(Error) { showDisconnectMessage(Error, showHexPreview); }
      );
   }

   loadHexPreview();
}

function LOGmapperIoItemSelected(IoItem, ItemNode){
   var jNode = $(ItemNode);
   $('#LOGmapperIoSelect .LOGmapperIoItem.c').removeClass('c');
   jNode.addClass('c');

   LOGmapperDisplayIoItem(IoItem);
}

function LOGmapperDisplayIoItem(IoItem, Refresh){
   var IoView = $('#LOGmapperIoView');
   if (IoItem.textPreview){
      IoView.css('background-color', '');
      IoView.html('<div id="textViewContainer" style="font-family:monospace; white-space:nowrap">' + IoItem.textPreview + '</div>');
      doWrapText(true);
   } else if (IoItem.treePreview){
      IoView.css('background-color', 'white');
      var Destination = document.getElementById('Destination');
      var ChannelName = Destination.value;
      var Treeview = IoView.html('<ul class="ifwareTreeview"></ul>').children();
      NTRinitTreeview(IoItem.treePreview.Root, Treeview.get(0), IoItem.Id, '/log_view_mapper_preview', {'Channel':ChannelName});
   } else { // shouldn't happen
      IoView.empty();
   }
}

// See /js/mapper/node_treeview.js for details
NTRonErrorResponse = function(Response){
   if (Response.ErrorMessage && Response.RefreshRequired){
      showPreview(true);
   }
}

function LOGshowMapperOutputPreview(Refresh)
{
   CurrentPreviewMode = 'MapperOutput';

   var Target = $('#resubmitPreview');

   var SelectedIndex = 0;
   if (Refresh){
      SelectedIndex = Target.find('#LOGinnerMapperIoSelect .LOGmapperIoItem.c').closest('li').prevAll().length;
   } else {
      Target.innerHTML = '<div class="entryViewPleaseWaitBar">Processing...</div>';
      setWrapText(false, true);
   }

   var Message = sanitizedMessage(document.getElementById('messageEditor').value);
   var Destination = document.getElementById('Destination');
   var ChannelName = Destination.value;

   setResubmitPreviewProgressVisible(true);
   updateCurrentEntryPreviewView();

   var ExistingTreeview = Target.find('div#LOGmapperIoView ul.ifwareTreeview');
   var TreeviewId = ExistingTreeview.data('Id');
   var ExpansionTree = TreeviewId + ':' + (ExistingTreeview.size() ? NTRgetExpansionTree(ExistingTreeview) : '');

   AJAXpostWithId('/log_view_mapper_preview',
      '&Message='+encodeURIComponent(Message) + '&Channel='+encodeURIComponent(ChannelName) +
      '&ExpansionTree='+encodeURIComponent(ExpansionTree) + '&LogBrowserGuid='+encodeURIComponent(LOGCSlogBrowserGuid),
      'LogBrowserViewEntry',
      function(Data, ContentType){
         if (ContentType.match('application/json')){
            ResponseData = eval('(' + Data + ')');
            if (ResponseData.ErrorMessage){
               Target.html('<div style="padding:4px; white-space:pre;">' + htmlEscape(ResponseData.ErrorMessage) + '</div>');
            } else {
               var IoSelect = Target.find('#LOGinnerMapperIoSelect');
               var IoView = Target.find('#LOGmapperIoView');
               if (IoSelect.length == 0 || IoView.length == 0){
                  Target.html('<div id="LOGmapperIoSelect">\
                                  <div class="LOGmapperIoHeader">Output</div>\
                                  <div id="LOGinnerMapperIoSelect" />\
                               </div>\
                               <div id="LOGmapperIoView" />');
                  IoSelect = Target.find('#LOGinnerMapperIoSelect');
                  IoView = Target.find('#LOGmapperIoView');
               }
               var OutputTree = document.createElement('ul');
               OutputTree.className = 'ifwareTreeview';
               if (ResponseData.length){
                  if (SelectedIndex >= ResponseData.length){
                     SelectedIndex = 0;
                  }
                  for (var i = 0; i < ResponseData.length; i++){
                     var OutputItem = ResponseData[i];
                     var ItemClass = 'LOGmapperIoItem LOGmapperIoType_' + OutputItem.type;
                     if (i == SelectedIndex){
                        ItemClass += ' c';
                        LOGmapperDisplayIoItem(OutputItem, Refresh);
                     }
                     OutputTree.appendChild(TRVcreateNode(OutputItem, '', ItemClass, false, false, OutputItem.name, null, LOGmapperIoItemSelected));
                  }
               } else {
                  OutputTree.innerHTML = '<span style="padding-left:10px;">The translator will produce no output.</span>';
               }
               IoSelect.empty().append(OutputTree);
            }
            onResubmitPreviewComplete();
         } else {
            showTimeOutMessage(LOGshowMapperOutputPreview);
         }
      },
      function(Error) { showDisconnectMessage(Error, LOGshowMapperOutputPreview); }
   );
}

var ParsersBySource;
function getParsersForSource(Source)
{
   if(!ParsersBySource)
   {
      ParsersBySource = LOGCSinitParsersBySource();
   }
   if (ParsersBySource[Source])
   {
      return ParsersBySource[Source];
   }
   else
   {
      return [{Guid:'HL7', Name:'HL7 Standard Library'}];
   }
}

function sourceHasParsers(Source)
{
   // Parser list will always contain the 'HL7 Standard Library' option - we want to know if has any other parsers (channels).
   return getParsersForSource(Source).length > 1;
}

var IsToPluginBySource;
function sourceHasPluginDestination(Source)
{
   if (!IsToPluginBySource)
   {
      IsToPluginBySource = LOGCSinitIsToPluginBySource();
   }

   return IsToPluginBySource[Source];
}

var LOGisToMapperBySource;
function LOGsourceHasMapperDestination(Source){
   if (!LOGisToMapperBySource){
      LOGisToMapperBySource = LOGCSinitIsToMapperBySource();
   }

   return LOGisToMapperBySource[Source];
}

// Direction should be 'next', 'previous', or 'refresh'.
// If direction is set, some views will have their contents updated, rather than
// the entire content regenerated.
// A ParserGuid of 'Preserve' indicates that we should use the last used parser, if possible.
// A ParserGuid of 'HL7' indicates that we should use the standard HL7 library.
//
function showParsed(ParseType, ParserGuid, IsPreview, Direction)
{
   if (CurrentEntry.Deleted)
   {
      raiseAlert("This message has been deleted. Undelete it before doing a parse.");
      showText(Direction);
      return;
   }

   if (!IsPreview || !Direction)
   {
      setWrapText(false, IsPreview);
   }

   // Remember the currently selected parsing channel, if applicable.
   // Remember it before we potentially clear out the target's contents.
   var OldParsingChannel = null;
   var OldParserSelectBox = document.getElementById('ParserSelectBox');
   if (OldParserSelectBox)
   {
      OldParsingChannel = OldParserSelectBox.options[OldParserSelectBox.selectedIndex].value;
   }

   var Parsers, Target;
   if(IsPreview)
   {
      CurrentPreviewMode = ParseType;

      var Destination = document.getElementById('Destination');

      // Clear out the entry area, to prevent id conflicts
      document.getElementById('entryArea').innerHTML = '';

      var ResubmitPreview = document.getElementById('resubmitPreview');
      if (!Direction && ResubmitPreview)
      {
         ResubmitPreview.innerHTML = '<div class="entryViewPleaseWaitBar">Processing...</div>';
      }

      setResubmitPreviewProgressVisible(true);
      updateCurrentEntryPreviewView();

      CurrentTableArea = 'resubmitPreview';
      Parsers = getParsersForSource(Destination.value);
      Target = document.getElementById('resubmitPreview');
   }
   else // not preview
   {
      CurrentViewMode = ParseType;
      updateLocationHashIfUsed();

      CurrentTableArea = 'entryArea';

      // Clear out the preview area, to prevent id conflicts
      var ResubmitPreview = document.getElementById('resubmitPreview');
      if (ResubmitPreview)
      {
         ResubmitPreview.innerHTML = '';
      }

      if (Direction)
      {
         // Do this before updateCurrentEntryView() so that the "Please Wait" appears
         // immediately (before the log entry description begins to "slide").
         displayEntryPleaseWaitBar();
      }
      else
      {
         clearEntryView('Processing...');
      }

      Parsers = getParsersForSource(CurrentEntry.Channel);
      Target = document.getElementById('entryArea');

      resize(Target);
   }

   // Last option ('HL7 Standard Library') should only be available for the 'Segment View'.
   var CountOfParser = (ParseType == 'SegmentMessage') ? Parsers.length : Parsers.length-1;
   if (CountOfParser < 1)
   {
      raiseAlert("No available parsers for this message.");
      showText(Direction);
      return;
   }

   // If ParserGuid is not specified, in order of precedence, we'll use:
   //  1) The parser used previously (if a parsed view was previously showing, and if the same parser is in the list),
   //  2) The selected channel in the search criteria (if it's in the list), or
   //  3) The first parser in the list.
   if (ParserGuid == 'Preserve')
   {
      ParserGuid = Parsers[0].Guid;

      var SourceInput = document.getElementById('SourceInput');
      if (SourceInput)
      {
          var SelectedChannelName = SourceInput.options[SourceInput.selectedIndex].value;
          for (var CurrentParserIndex = 1; CurrentParserIndex < CountOfParser; ++CurrentParserIndex)
          {
             if (Parsers[CurrentParserIndex].Name == SelectedChannelName)
             {
                ParserGuid = Parsers[CurrentParserIndex].Guid;
             }
          }
      }

      for (var CurrentParserIndex = 0; CurrentParserIndex < CountOfParser; ++CurrentParserIndex)
      {
         if (Parsers[CurrentParserIndex].Guid == OldParsingChannel)
         {
            ParserGuid = Parsers[CurrentParserIndex].Guid;
         }
      }
   }

   // After we've figured out which parser we're using, we can update the view.
   if (!IsPreview)
   {
      updateCurrentEntryReparseWarning
      (
         ParserGuid == 'HL7' ?
            'Using library VMD file: ' + LOGCSstandardHl7LibraryVmdPath :
            'If you change or edit the channel\'s VMD file, the results shown above will be generated from the new file.'
      );
      updateCurrentEntryView(Direction);
   }

   function createParserSelect()
   {
      if(CountOfParser > 1)
      {
         var strSelectBox = '<select id="ParserSelectBox" style="width:125px;" '
                          + 'onChange="showParsed(\'' + ParseType + '\', this.value, ' + IsPreview + ', \'refresh\')">';
         for(var i=0; i < CountOfParser; i++)
         {
            strSelectBox = strSelectBox + '<option value="' + Parsers[i].Guid + '"';
            if (ParserGuid == Parsers[i].Guid)
            {
               strSelectBox = strSelectBox + " selected";
            }
            strSelectBox = strSelectBox + ">" + Parsers[i].Name + "</option>";
         }
         strSelectBox = strSelectBox + '</select> <a id="parseSelectHelpIcon" class="helpIcon" tabindex="100" rel="When multiple channels read messages from the same source channel, you can use this select box to determine which channel you would like to use to parse the message." title="More Information" href="#" onclick="initializeHelp(this,event);"><img src="' + LOGCShelpIconLocation + '" style="margin-top:1px" border="0" /></a>';
         document.getElementById('ParseSelect').innerHTML = strSelectBox;
      }
      else
      {
         document.getElementById('ParseSelect').innerHTML = '';
      }
   }

   function onParsedLoaded()
   {
      createParserSelect();
      if (IsPreview)
      {
         onResubmitPreviewComplete();
      }
      else
      {
         hideEntryPleaseWaitBar();
      }
   }

   // At this point, ParserGuid should be either a valid channel guid, or 'HL7'.
   var Request = (ParserGuid == 'HL7' ? 'StandardLibParse=true' : 'ChannelGuid=' + encodeURIComponent(ParserGuid))
               + '&MessageId=' + encodeURIComponent(CurrentEntry.MessageId)
               + '&ParseType=' + encodeURIComponent(ParseType);

   if(IsPreview)
   {
      var Message = sanitizedMessage(document.getElementById('messageEditor').value);
      Request += '&Message=' + encodeURIComponent(Message) + '&Preview=1';
   }

   if (ParseType == 'SegmentGrammar' || ParseType == 'SegmentMessage')
   {
      Request += '&SegmentParse=true';
      var errorOriginDisplayFunction = null;
      if (IsPreview)
      {
         errorOriginDisplayFunction = highlightResubmitEditorText;
      }
      else
      {
         var FilterInput = document.getElementById('filterInput');
         if (FilterInput)
         {
            Request += '&Filter=' + FilterInput.value;
         }
      }
      SGVrenderSegmentView(Target, ParseType, (IsPreview && Direction), 'log_parse_entry', Request, Direction, onParsedLoaded, errorOriginDisplayFunction);
      return;
   }
   else if (ParseType == 'Table')
   {
      TBVrenderTableView(Target, 'log_parse_entry', Request, Direction, onParsedLoaded);
      return;
   }
   else // Other parse types
   {
      renderOtherParsedView(Target, IsPreview, 'log_parse_entry', Request, onParsedLoaded);
   }
}

function renderOtherParsedView(Target, IsPreview, RequestLocation, RequestParameters, onParsedLoaded)
{
   AJAXpostWithId
   (
      RequestLocation,
      RequestParameters,
      'LogBrowserViewEntry',
      function(Data, ContentType)
      {
         if( ContentType.match('application/json') )
         {
            var InnerHtml = '<div id="ParseSelect" style="padding:4px;"></div>';

            var JsonResponseObject;
            try
            {
               JsonResponseObject = JSON.parse(Data);
               if (JsonResponseObject.ErrorMessage)
               {
                  throw JsonResponseObject.ErrorMessage;
               }
               else
               {
                  LastEntryData = JsonResponseObject;
                  LastEntryData.Location = RequestLocation;
                  LastEntryData.Params = RequestParameters;

                  // Text will be HTML-escaped by the server.
                  InnerHtml += '<pre id="ParsedViewContainer">' + JsonResponseObject.Message + '</pre>' + loadingMoreLinesDiv(LastEntryData);
               }
            }
            catch(e)
            {
               InnerHtml += '<pre>' + (e.message ? e.message : e) + '</pre>';
            }

            Target.innerHTML = InnerHtml;
            doWrapText(IsPreview);
            onParsedLoaded();
         }
         else
         {
            showTimeOutMessage(function()
            {
               renderOtherParsedView(Target, IsPreview, RequestLocation, RequestParameters, onParsedLoaded);
            });
         }
      },
      function (Error)
      {
         showDisconnectMessage(Error, function()
         {
            renderOtherParsedView(Target, IsPreview, RequestLocation, RequestParameters, onParsedLoaded);
         });
      }
   );
}

function showParsedPreview(ParseType, ParserGuid, Refresh)
{
   showParsed(ParseType, ParserGuid, true, (Refresh ? 'refresh' : ''));
}

function returnToMessageEditor()
{
   hideViews();
   show('submitView');
}

function downloadMessage()
{
   if (CurrentEntry)
   {
      var DownloadUrl = '/log_download_entry?' + getViewParams(CurrentEntry);
      location.href = DownloadUrl;
   }
}

function showLink()
{
   var Link = document.URL.replace(/(log_browse|logs.html).*/, CurrentEntry.Link);

   var ExtraInfo = '';
   if( window.clipboardData && clipboardData.setData )  // IE6 and maybe Opera, not Firefox/Safari.
   {
      clipboardData.setData('Text', Link);
      ExtraInfo = '\n\nIt has been copied to your clipboard.';
   }

   $('#show-link-dialog-text').html('The following link will bring you back to this message:\n\n' + ExtraInfo);
   $('#show-link-dialog').dialog('open');
   $('#messageLink').each( function(){
                             this.value = Link;
                             this.focus();
                             this.select() });
}

function setDeleteValueForEntry(MsgId, DelValue)
{
   for (var LogIndex = 0; LogIndex < LogEntries.length; LogIndex++)
   {
      //search the log entries list for the msg id and set the value
      var LogEntry = LogEntries[LogIndex];
      if (LogEntry && LogEntry.MessageId == MsgId)
      {
         LogEntry.Deleted = DelValue;
         updateCurrentEntryView();
         break;
      }
   }
}

function deleteEntry()
{
   if (!LOGCScurrentUserCanAdmin)
   {
      return;
   }

   if (CurrentEntry)
   {
      //flip immediately, and after posting, move to the next item
      CurrentEntry.Deleted = true;
      var MsgId = CurrentEntry.MessageId;
      updateCurrentEntryView();

      AJAXpost('log_delete_entry', 'MessageId=' + encodeURIComponent(CurrentEntry.MessageId) + '&DeleteValue=true',
         function(Data, ContentType)
         {
            if( !(ContentType.match('text/plain') && Data == 'OK') )  // It's actually HTML.
            {
               //passing in the id prevents us from undoing the wrong
               //entry
               setDeleteValueForEntry(MsgId,false);
               if (Data == 'FAILED_LOCK')
               {
                  raiseAlert("A message could not be deleted because it is in use by a channel. Try stopping the channel and try again.");
               }
            }
         },
         function(Error) { undoDeleteEntryPost(MsgId); }
      );

      //move previous, stay where we are and refresh the view
      if (!Iterator.prev())
      {
         showText('previous');
      }
   }
}

function undeleteEntry()
{
   if (!LOGCScurrentUserCanAdmin)
   {
      return;
   }

   if (CurrentEntry)
   {
      //Flip immediately before posting, so user sees it immediately
      CurrentEntry.Deleted = false;
      var MsgId = CurrentEntry.MessageId;
      updateCurrentEntryView();

      AJAXpost('log_delete_entry', 'MessageId=' + encodeURIComponent(CurrentEntry.MessageId) + '&DeleteValue=false',
         function(Data, ContentType)
         {
            if( !(ContentType.match('text/plain') && Data == 'OK') )  // It's actually HTML.
            {
               setDeleteValueForEntry(MsgId, true);
               if (Data == 'FAILED_LOCK')
               {
                  raiseAlert("A message could not be undeleted because it is in use by a channel. Try stopping the channel and try again.");
               }
            }
         },
         function(Error) { undoUndeleteEntryPost(MsgId); }
      );
   }
}

function undoMarkEntryPost(MsgId, OrigValue)
{
   if (CurrentEntry && CurrentEntry.MessageId == MsgId)
   {
      CurrentEntry.Acknowledged = OrigValue;
      updateCurrentEntryView();
   }
}

function markEntry()
{
   if (CurrentEntry)
   {
      if (CurrentEntry.Deleted)
      {
         raiseAlert("This message has been deleted. To mark/unmark it, undelete it first.");
         return;
      }

      //flip the CurrentEntry value now and change the button.
      //the ajax response will update it again anyway once it comes back
      var OrigAckValue = CurrentEntry.Acknowledged;
      var MsgId = CurrentEntry.MessageId;
      CurrentEntry.Acknowledged = !OrigAckValue;
      updateCurrentEntryView();

      AJAXpost('log_mark_entry', 'MessageId=' + encodeURIComponent(CurrentEntry.MessageId)
            + '&Action=' + (CurrentEntry.Acknowledged ? 'mark' : 'unmark'),
         function(Data, ContentType)
         {
            if(!ContentType.match('application/json'))
            {
               undoMarkEntryPost(MsgId, OrigAckValue);
            }
            else
            {
               var Result = JSON.parse(Data);
               if(Result.ErrorDescription)
               {
                  raiseAlert(Result.ErrorDescription);
                  undoMarkEntryPost(MsgId, OrigAckValue);
               }
            }
         },
         function(Error) { undoMarkEntryPost(MsgId, OrigAckValue); }
      );
   }
}


//
// Pop-out windows
//

function LOGpopoutCurrentEntry()
{
   var MessageId = CurrentEntry.RefMsgId;

   var EntryViewDiv = $('#entryView');
   var PopoutHeight = EntryViewDiv.outerHeight();
   var PopoutWidth  = EntryViewDiv.outerWidth();

   var NewWindow = window.open
   (
      '/log_view_entry.html?LogBrowserGuid=' + LOGCSlogBrowserGuid + '&Key=' + LOGCSremoteValidationKey + '#' + CurrentViewMode,
      '', 'location=1,height=' + PopoutHeight + ',width=' + PopoutWidth + ',resizable=1'
   );
}

function LOGdisableRemoteWindows()
{
   AJAXpost
   (
      'log_disable_remote_windows',
      'LogBrowserGuid=' + LOGCSlogBrowserGuid,
      function(Data, ContentType)
      {
         if (ContentType.match('application/json'))
         {
            var ResponseData = JSON.parse(Data);
            if (ResponseData && ResponseData.NewRemoteValidationKey)
            {
               LOGCSremoteValidationKey = ResponseData.NewRemoteValidationKey;
            }
            else
            {
               var ErrorMessage = 'Remote windows could not be disabled.';
               if (ResponseData.ErrorMessage)
               {
                  ErrorMessage += '\nReason:\n' + ResponseData.ErrorMessage;
               }
               alert(ErrorMessage);
            }

            hide('disableRemoteWindows');
         }
         else
         {
            showTimeOutMessage(LOGdisableRemoteWindows);
         }
      },
      function(Error)
      {
         showDisconnectMessage(Error, LOGdisableRemoteWindows);
      }
   );
}


//
// AJAX magic to fetch more entries.
//

function initRow(Row, PreviousRow, Entry)
{
   Row.className = ($('#'+PreviousRow.id).hasClass('first')) ? 'second' : 'first';
   Row.id = 'entry_row_' + Entry.MessageId;
   Row.onclick = function() { hideDeleteNotify(); showEntry(Entry, '', true); };
   Row.onmouseover = function() { rowHoverIn (Row); };
   Row.onmouseout  = function() { rowHoverOut(Row); };
   Row.style.cursor = 'pointer';

   if (HighlightEntry.MessageId == null ||
       HighlightEntry.MessageId == Entry.MessageId) setHighlightEntry(Entry.MessageId);

   if (FirstMessageId == null) FirstMessageId = Entry.MessageId;

   // IE6 won't let us set the innerHTML of a row, so we have to add each cell separately.
   //
   Row.insertCell(0).innerHTML = Entry.FirstCell;
   Row.insertCell(1).innerHTML = Entry.SecondCell;
}

var LogEntries = [];
var CurrentDequeuePositionObjList = null;

function addRowForward(Entry)
{
   var Row = LogTable.insertRow(1);
   initRow(Row, LogTable.rows[2], Entry);
   LogEntries.unshift(Entry);
}

function addRowReverse(Entry)
{
   var Index = LogTable.rows.length - 1;
   var Row = LogTable.insertRow(Index);
   initRow(Row, LogTable.rows[Index-1], Entry);
   LogEntries.push(Entry);
}

function hideDeleteNotify()
{
   document.getElementById('deleteNotify').style.display = 'none';
}

function showDeleteNotify(DeleteMessage)
{
   document.getElementById('deleteNotify').style.display = '';
   document.getElementById('deleteStatus').innerHTML = DeleteMessage;
}

var Finished = { forward: false, reverse: false };
function setFinished(Direction, IsFinished, ProgressText, PendingSearch)
{
   Finished[Direction] = IsFinished;

   if( ProgressText && (Direction != 'forward' || !PollingForward) )
   {
      document.getElementById(Direction + 'Status').innerHTML = ProgressText;
      document.getElementById(Direction + 'Progress').style.display = '';
   }
   else
   {
      document.getElementById(Direction + 'Progress').style.display = 'none';
   }

   // Display the "No Results" banner if we've finished searching and there are no errors.
   var NoResults = Finished['forward'] && Finished['reverse'] && LogTable.rows.length <= 2
      && document.getElementById('forwardProgress').style.display == 'none'
      && document.getElementById('reverseProgress').style.display == 'none'
      && (PendingSearch==null || PendingSearch==false);

   LogTable.style.display = NoResults ? 'none' : '';
   document.getElementById('noResults').style.display = NoResults ? '' : 'none';

   //display search tips if doing a text search
   NoResultsSearchDiv = $('#no_results_search_terms_help');
   NoResultsContainer = $('#noResultsContainer');

   if (document.getElementById('filterInput').value != '')
   {
      NoResultsSearchDiv.show();;
      NoResultsContainer.height('300px');
      NoResultsContainer.css('margin-top','-150px');
   }
   else
   {
      NoResultsSearchDiv.hide();
      NoResultsContainer.height('50px');
      NoResultsContainer.css('margin-top','-25px' );
   }
}

var SearchState = { forward: null, reverse: null };
function setSearchState(Direction, State)
{
   SearchState[Direction] = State;
}

function searchState(Direction)
{
   if( SearchState[Direction] )
   {
      return SearchState[Direction];
   }
   else if( LogEntries.length < 1 )
   {
      return { Position:0, Date:'' };
   }
   else if( Direction === 'forward' )
   {
      return LogEntries[0];
   }
   else
   {
      var LastIndex = LogEntries.length - 1;
      return LogEntries[LastIndex];
   }
}

function clearSearch()
{
   var CancelSearch = function(RequestId){
      if (RequestId)
      {
         AJAXpost('log_entries_cancel', 'LogBrowserGuid=' + LOGCSlogBrowserGuid + '&LogBrowserRequestId=' + RequestId,
                  function(Data, ContentType) {
                     //we really don't care what we get back
                  },
                  function(Error) {}
                  );
      }
   }

   CancelSearch(Loading['forward']);
   CancelSearch(Loading['reverse']);

   stopPollingForward();

   Loading['forward'] = false;
   Loading['reverse'] = false;

   setFinished('forward', true, null, true);
   setFinished('reverse', true, null, true);

   for(var EntryCount = LogTable.rows.length - 2; EntryCount; --EntryCount)
   {
      LogTable.deleteRow(1);
   }
   LogEntries = [];

   setSearchState('forward', null);
   setSearchState('reverse', null);

   LogTable.scrollTop = 0;

   FirstMessageId = null;
}

function checkForMismatch()
{
   if( document.getElementById('logView').style.display == 'none' )
   {
      var OriginalEntry = CurrentEntry;
      AJAXpost('log_entry_matches', getFormQuery(document.getElementById('FilterForm'), '',
            { Date: CurrentEntry.Date, Position: CurrentEntry.Position }),
         function(Data, ContentType) {
            if( ContentType.match('text/plain') )
            {
               if( /^OK NO(?!\w)/.test(Data) && CurrentEntry == OriginalEntry &&
                   document.getElementById('logView').style.display == 'none' )
               {
                  document.getElementById('searchMismatch').style.display = '';
               }
               else
               {
                  document.getElementById('searchMismatch').style.display = 'none';
               }
            }
            else
            {
               showTimeOutMessage(checkForMismatch);
            }
         },
         function(Error) {
            showDisconnectMessage(Error, checkForMismatch);
         }
      );
   }
}

var onShowMoreComplete = null;
var ShowMoreCount = 0;
function jumpToQueueEnd(DequeueSourceName)
{
   HighlightEntry.MessageId = null;
   //pick first element if needed and possible
   var CurrentDequeuePositionObj = null;
   if (!DequeueSourceName && CurrentDequeuePositionObjList)
   {
      for (var Key in CurrentDequeuePositionObjList)
      {
         CurrentDequeuePositionObj = CurrentDequeuePositionObjList[Key];
         break;
      }
   }
   else if (CurrentDequeuePositionObjList &&
            CurrentDequeuePositionObjList[DequeueSourceName] !== undefined)
   {
      CurrentDequeuePositionObj = CurrentDequeuePositionObjList[DequeueSourceName];
   }
   if (CurrentDequeuePositionObj)
   {
      showLog();
      clearSearch();
      var PositionForShowMore = CurrentDequeuePositionObj; //so closures work properly
      setSearchState('reverse', { Position: PositionForShowMore.Position, Date: PositionForShowMore.Date });
      setFinished('reverse', false);

      //in order to queue up show more requests, we chain some closures together
      //to the show more complete function
      onShowMoreComplete = function(){
         //after reverse is finished we run the forward search, which will start from the
         //top position
         setFinished('forward', false, 'Searching...');

         //second time we scroll to the entry position
         //set up the function that will scroll the entry to the center position
         onShowMoreComplete = function(){
            //TODO - this could be a binary search if it matterred
            //look for the entry we care about, or just after if no exact entry
            TargetMessageId = PositionForShowMore.FileId + '-' + PositionForShowMore.Position;
            var LogIndex = 0;
            while(LogIndex < LogEntries.length)
            {
               var LogEntry = LogEntries[LogIndex];
               if (LogEntry &&
                   FRMCHnaturalCompare( LogEntry.MessageId, TargetMessageId) < 0 )
               {
                  break;
               }
               else
               {
                  LogIndex++;
               }
            }
            if (LogIndex < LogEntries.length)
            {
               //This is not super precise...we really should be adjusting for the padding of
               //the parent containers. But should be close enough.
               $('#entry_row_' + LogEntries[LogIndex].MessageId).each( function(){
               var JLogArea = $('#logArea');
               var LogAreaScrollTop = JLogArea.attr('scrollTop')
               var LogAreaHeight = JLogArea.attr('offsetHeight')
                  JLogArea.attr('scrollTop', this.offsetTop - LogAreaHeight/2 );
                  })
            }
            onShowMoreComplete = null; //finished, no more functions
         }
         showMore('forward');
      }
      showMore('reverse');
   }
}

function cancelBubble(e)
{
   if (!e) var e = window.event;
   e.cancelBubble = true;
   if (e.stopPropagation) e.stopPropagation();
}

function onPendingProcessedClick(e, MessageId)
{
   //stop click event going to the row
   cancelBubble(e);

   //fetch source and dequeue source
   var SourceSelector = document.getElementsByName('Source')[0];
   var Source         = SourceSelector.options[SourceSelector.selectedIndex].value;
   var AllEntriesOption = LOGCSallEntriesOption;
   var ServiceEntriesOption = LOGCSserviceEntriesOption;

   var DequeueSource = '';
   if (Source != ServiceEntriesOption &&
       Source != AllEntriesOption &&
       LOGCSchannelSources[Source] !== undefined &&
       LOGCSchannelSources[Source].length > 0)
   {
      DequeueList = LOGCSchannelSources[Source];
      if (DequeueList.length == 1 &&
          DequeueList[0] == Source)
      {
         //normal channel
         DequeueSource = Source;
      }
      else
      {
         //from channel, go fetch the correct one
         var SelectedList = dequeueSelectedList();
         if (SelectedList.length == 1)
         {
            DequeueSource = SelectedList[0];
         }
      }
   }

   if(!canEditChannel(Source))
   {
      raiseAlert('You do not have permission to edit channel "' + Source + '".');
   }
   else if(!DequeueSource)
   {
      raiseAlert('Select only one Source Channel before attempting to reposition.');
   }
   else
   {
      function repo(ExtraArgs)
      {
         //post request to change positions
         $('#logArea').hide();
         $('#workingProgressDiv').text('Repositioning...');
         $('#workingProgress').show().each( function(){resize(this)} );

         QueryStr = 'Source=' + encodeURIComponent(Source)
                    + '&DequeueSourceName=' + encodeURIComponent(DequeueSource)
                    + '&NewPosition=' + encodeURIComponent(MessageId)
                    + (ExtraArgs || '');

         AJAXpost('reposition_channel', QueryStr,
            function(Data, ContentType) {
               if( ContentType.match('application/json') )
               {
                  ResultObj = JSON.parse(Data);
                  if (ResultObj.Result == 'OK')
                  {
                     LOGCSinitialJumpToQueueEnd = true;
                     requestDequeueInfo();
                  }
                  else if (ResultObj.Result == 'CHANNEL_RUNNING')
                  {
                     raiseAlert('Stop the channel before attempting to reposition it.');
                  }
                  else if (ResultObj.Result == 'EXCEPTION')
                  {
                     raiseAlert('An error occurred while repositioning the channel. Check the Iguana logs.');
                  }
                  else
                  {
                     raiseAlert('Reposition error: ' + ResultObj.Result);
                  }
                  $('#workingProgressDiv').text('');
                  $('#workingProgress').hide()
                  $('#logArea').show();
               }
               else
               {
                  showTimeOutMessage(showLog);
               }
            },
            function(Error) {
               showDisconnectMessage(Error, showLog);
            }
         );
      }

      var ExplanationMessage;
      if (DequeueSource == Source)
      {
         ExplanationMessage = 'Reposition Channel "';
      }
      else
      {
         ExplanationMessage = 'Reposition Source Channel "' + DequeueSource + '" for Channel "';
      }
      $('#repo-dialog').html(ExplanationMessage + Source + '" to start with this message? <br/><br/>All messages before this point will be marked as processed.');

      function close() { $('#repo-dialog').dialog('close'); }
      $('#repo-dialog').dialog({
         modal: true,
         width: 400,
         buttons: {
            "Cancel":             close,
            "After this message": function() { repo('&After=1'); close(); },
            "Start here":         function() { repo();           close(); }
         }
      }).dialog('open');
   }
}

var HtmlPendingSrc = 'images/pending.gif';
var HtmlProcessedSrc = 'images/processed.gif';
function updateEntryDequeuePosition(Entry, HtmlEntry, DequeueSourceName, DequeuePositionCombined)
{
   if (DequeuePositionCombined != '-' &&
       Entry.Type == 'Message' &&
       Entry.Channel === DequeueSourceName)
   {
      if( FRMCHnaturalCompare(Entry.MessageId, DequeuePositionCombined) > 0 )
      {
         HtmlEntry.innerHTML = '<img src="' + HtmlPendingSrc + '">';
      }
      else
      {
         HtmlEntry.innerHTML = '<img src="' + HtmlProcessedSrc + '">';
      }
   }
   else
   {
      HtmlEntry.innerHTML = '';
   }
}

function updateCurrentEntryDequeuePosition()
{
   var DequeuePositionCombined = '-';
   if (CurrentDequeuePositionObjList && CurrentEntry &&
       CurrentDequeuePositionObjList[CurrentEntry.Channel] !== undefined)
   {
      var CurrentDequeuePositionObj = CurrentDequeuePositionObjList[CurrentEntry.Channel];
      DequeuePositionCombined = CurrentDequeuePositionObj.FileId + '-' + CurrentDequeuePositionObj.Position;
   }
   //current entry pending marker
   $('.clsPendingMarkerCell',$('#entryDescription') ).each( function(){
       if (CurrentEntry &&
           DequeuePositionCombined != '-' &&
           CurrentEntry.Type == 'Message')
       {
          var ThisHtml = ( FRMCHnaturalCompare(CurrentEntry.MessageId, DequeuePositionCombined) > 0 ? HtmlPendingSrc : HtmlProcessedSrc );
          ThisLink = '';
          if ($(this).children()[0] !== undefined)
          {
             ThisLink = $(this).children()[0].src;
          }
          if (ThisLink != NORMnormalizeImg(ThisHtml))
          {
                this.innerHTML = '<img src="' + ThisHtml + '">';
          }
       }
       else
       {
          this.innerHTML = '';
       }
     }
   );
}

function updateLogEntriesDequeuePosition()
{
   var ToolTipText = 'Click to reposition channel to begin at this message.';

   for (var LogIndex = 0; LogIndex < LogEntries.length; LogIndex++)
   {
      var ThisEntry = LogEntries[LogIndex];
      if (ThisEntry.Type == 'Message')
      {
         var DequeuePositionCombined = '-';
         if (CurrentDequeuePositionObjList &&
             CurrentDequeuePositionObjList[ThisEntry.Channel] !== undefined)
         {
            var CurrentDequeuePositionObj = CurrentDequeuePositionObjList[ThisEntry.Channel];
            DequeuePositionCombined = CurrentDequeuePositionObj.FileId + '-' + CurrentDequeuePositionObj.Position;
         }

         $('#pending-marker-' + ThisEntry.MessageId).each( function(){
            if (DequeuePositionCombined != '-')
            {
               var ThisHtml = ( FRMCHnaturalCompare(ThisEntry.MessageId, DequeuePositionCombined) > 0 ? HtmlPendingSrc : HtmlProcessedSrc );
               ThisLink = '';
               if ($(this).children()[0] !== undefined)
               {
                  ThisLink = $(this).children()[0].src;
               }

               if (ThisLink != NORMnormalizeImg(ThisHtml))
               {
                  this.innerHTML = '<img src="' + ThisHtml + '">';
                  var MessageIdForClick = ThisEntry.MessageId; //VERY important else the wrong message id would be passed through the click
                  $('*',this).click(function(event){
                                 onPendingProcessedClick(event,MessageIdForClick)
                                 })
                             .mouseover( function(){ TOOLtooltipLink(ToolTipText, null, this);} )
                             .mouseup(TOOLtooltipClose)
                             .mouseout(TOOLtooltipClose);
               }
            }
            else
            {
               this.innerHTML = '';
            }
         });
      }
   }
}

var RequestDequeueInfoTimerId = null;
function requestDequeueInfo()
{
   var SourceSelector = document.getElementsByName('Source')[0];
   var Source         = SourceSelector.options[SourceSelector.selectedIndex].value;
   var AutomaticRequestFlag = '';

   if (RequestDequeueInfoTimerId)
   {
      AutomaticRequestFlag = '&AutomaticRequest=1';
      clearTimeout(RequestDequeueInfoTimerId);
   }

   AJAXpost('log_browse_get_dequeue_position', 'Source=' + encodeURIComponent(Source) + AutomaticRequestFlag,
         function(Data, ContentType) {
            if( ContentType.match('application/json') )
            {
               var d = JSON.parse(Data);
               if (d.ErrorDescription) {
                  clearTimeout(RequestDequeueInfoTimerId);
                  CurrentDequeuePositionObjList = null;
               } else {
                  CurrentDequeuePositionObjList = d;
                  updateLogEntriesDequeuePosition();
                  updateCurrentEntryDequeuePosition();

                  //jump to queue end if set
                  if (LOGCSinitialJumpToQueueEnd)
                  {
                     LOGCSinitialJumpToQueueEnd = false;
                     $('#hrefJumpToQueueEnd').click();
                  }

                  clearTimeout(RequestDequeueInfoTimerId);
                  RequestDequeueInfoTimerId = setTimeout(requestDequeueInfo, 2000);
               }
            }
            else
            {
               CurrentDequeuePositionObjList = null;
               showTimeOutMessage(requestDequeueInfo);
            }
         },
         function(Error) {
            showDisconnectMessage(Error, requestDequeueInfo);
         }
      );
}

function refreshSearch()
{
   clearSearch();
   requestDequeueInfo();

   if ( CurrentEntry != null )
   {
      setSearchState('forward', { Position: CurrentEntry.Position - 1, Date: CurrentEntry.Date });
      setFinished('forward', false, 'Searching...');
      setFinished('reverse', true, ' '); // Delay the reverse search.
      showMore('forward');

      Iterator.disable();
      var Flasher = function() {
         if( LogEntries.length > 0 || Finished['forward'] )
         {
            if( CurrentEntry && (LogEntries.length > 0) &&
                CurrentEntry.MessageId == LogEntries[LogEntries.length - 1].MessageId )
            {
               Iterator.enable();
            }

            setFinished('reverse', false, 'Searching...');
            showMore('reverse');
         }
         else
         {
            setTimeout(Flasher, 100);
         }
      };
      setTimeout(Flasher, 100);
   }
   else if (HighlightEntry.MessageId != null)
   {
      setSearchState('forward', { Position: HighlightEntry.Position - 1, Date: HighlightEntry.Date });
      setFinished('forward', false, 'Searching...');
      setFinished('reverse', true, ' '); // Delay the reverse search.
      showMore('forward');
      var Flasher = function() {
         if( LogEntries.length > 0 || Finished['forward'] )
         {
            setFinished('reverse', false, 'Searching...');
            showMore('reverse');
         }
         else
         {
            setTimeout(Flasher, 100);
         }
      };
      setTimeout(Flasher, 100);
   }
   else
   {
      setFinished('reverse', false, 'Searching...');
      showMore('reverse');
   }

   checkForMismatch();

   // Refresh the message text-view if it is currently displayed.
   //
   if( CurrentViewMode == 'Text' && visible('entryView') )
   {
      showText();
   }
   // Refresh the segment view if it is currently displayed.
   if( (CurrentViewMode == 'SegmentGrammar' || CurrentViewMode == 'SegmentMessage') && visible('entryView') )
   {
      showParsed(CurrentViewMode, 'Preserve', false, 'refresh');
   }

   // Greying out related messages checkbox if message/informational type is selected
   if ( document.getElementById('Type').value == 'messages' || document.getElementById('Type').value == 'info' )
   {
      document.getElementById('Export.Related').checked = false;
      document.getElementById('Export.Related').disabled = true;
   }
   else
   {
      document.getElementById('Export.Related').disabled = false;
   }
}

var Loading = { forward: false, reverse: false };
function showMore(Direction, AutomaticRequest)
{
   AutomaticRequestFlag = typeof(AutomaticRequest) != 'undefined' ? '&AutomaticRequest=1' : '';

   if( !Loading[Direction] && !Finished[Direction] )
   {
      ShowMoreCount++;
      var Magic = Math.random();
      Loading[Direction] = Magic;

      var SearchState, Params;
      var Params = 'LogBrowserGuid=' + LOGCSlogBrowserGuid;
      Params += AutomaticRequestFlag;
      Params += '&LogBrowserRequestId=' + Magic;
      if( Direction === 'forward' )
      {
         SearchState = searchState('forward');
         Params += '&AfterPosition=' + encodeURIComponent(SearchState.Position);
      }
      else
      {
         SearchState = searchState('reverse');
         Params += '&BeforePosition=' + encodeURIComponent(SearchState.Position);
      }

      AJAXpost('log_entries', Params + getFormQuery(document.getElementById('FilterForm'), '&', { Date: SearchState.Date }),
         function(Data, ContentType) { // On success
            if( Loading[Direction] == Magic )
            {
               if( !ContentType.match('text/javascript') )
               {
                  Loading[Direction] = false;
                  showTimeOutMessage(function() { showMore(Direction); });
               }
               else if (!Finished[Direction])
               {
                  var OriginalTop  = LogArea.scrollTop;
                  var OriginalSize = LogArea.scrollHeight;
                  eval(Data);
                  updateLogEntriesDequeuePosition();
                  if( Direction === 'forward' )
                  {
                     LogArea.scrollTop = Math.max(0, OriginalTop - OriginalSize + LogArea.scrollHeight);
                  }
                  Loading[Direction] = false;
                  onScrollLogArea();
               }
            }

            (ShowMoreCount > 0) ? ShowMoreCount-- : '';
            if (!ShowMoreCount && onShowMoreComplete)
            {
               onShowMoreComplete();
            }
         },
         function(Error) { // On error
            (ShowMoreCount > 0) ? ShowMoreCount-- : '';
            if( Loading[Direction] === Magic && !Finished[Direction] )
            {
               Loading[Direction] = false;
               showDisconnectMessage(Error, function() { showMore(Direction); });
            }
         }
      );
   }
}


//
// Date Updating
//

function formatDate(GivenDate, FormattedDate)
{
   var Now       = new Date();
   var DayAgo    = new Date(Now.getTime() - 24*60*60*1000);
   var Today     = [   Now.getFullYear(),    Now.getMonth()+1,    Now.getDate()].join('/');
   var Yesterday = [DayAgo.getFullYear(), DayAgo.getMonth()+1, DayAgo.getDate()].join('/');

   var CleanDate = GivenDate.replace(/\/0(\d)/g, '/$1');  // Remove zero-padding.

   if     ( CleanDate == Today )      { return 'Today'; }
   else if( CleanDate == Yesterday )  { return 'Yesterday'; }
   else
   {
      return FormattedDate || GivenDate;
   }
}

var DateUpdater;
function updateDates(When)
{
   var Now = new Date();

   if( !When )
   {
      When = new Date(Now.getTime() + 24*60*60*1000);
      When.setHours(0);
      When.setMinutes(0);
      When.setSeconds(0);
      When.setMilliseconds(0);
   }

   if( Now < When )
   {
      var Delay = Math.max(When.getTime() - Now.getTime(), 500);
      setTimeout(function() { updateDates(When); }, Delay);
   }
   else
   {
      var DayAgo    = new Date(Now.getTime() - 24*60*60*1000);
      var Today     = [   Now.getFullYear(),    Now.getMonth()+1,    Now.getDate()].join('/');
      var Yesterday = [DayAgo.getFullYear(), DayAgo.getMonth()+1, DayAgo.getDate()].join('/');

      for(var EntryIndex in LogEntries)
      {
         var Entry = LogEntries[EntryIndex];
         var Target;

         try      { Target = document.getElementById('entryDate-' + Entry.MessageId); }
         catch(e) { continue; }

         if     ( Entry.Date == Today )      { Target.innerHTML = 'Today'; }
         else if( Entry.Date == Yesterday )  { Target.innerHTML = 'Yesterday'; }
         else
         {
            Target.innerHTML = Entry.Date;
         }
      }

      var Next = new Date(Now.getTime() + 24*60*60*1000);
      setTimeout(function() { updateDates(Next); }, 500);
   }
}

//
// Polling Magic
//

var PollingForward = false;
function stopPollingForward()
{
   clearTimeout(PollingForward);
   PollingForward = false;
}

var    CountOfNewMessage = 0;
var MaxCountOfNewMessage = 50;
function startPollingForward()
{
   if( PollingForward || Finished['forward']
      && document.getElementById('forwardProgress').style.display == 'none' )
   {
      clearTimeout(PollingForward);
      PollingForward = setTimeout(function() {
         if( CountOfNewMessage <= MaxCountOfNewMessage )
         {
            Finished['forward'] = false;  // Calling setFinished() will flash the "no results" message.
            showMore('forward', true);
         }
      }, 5000);
   }
}

function updateNewMessageCount(HowManyMore)
{
   CountOfNewMessage += HowManyMore;

   if( CountOfNewMessage > 0 )
   {
      document.getElementById('newMessageCount').innerHTML
         = (CountOfNewMessage > MaxCountOfNewMessage
               ? MaxCountOfNewMessage + '+'
               :    CountOfNewMessage)
         + (CountOfNewMessage > 1 ? ' New Entries' : ' New Entry');
   }
}

function clearNewMessageCount()
{
   CountOfNewMessage = 0;

   document.getElementById('newMessageCount').innerHTML = '';
}

//
// Side-bar Goodies
//

// show()/hide(): If we add much more to the side-bar,
// we're going to have to revisit how this is done.

function showSearch()
{
   hideExport();
   hide('searchBarClosed');
   show('searchBarOpen');
   show('searchBar');
}

function hideSearch()
{
   hide('searchBar');
   hide('searchBarOpen');
   show('searchBarClosed');
}

function showExport()
{
   hideSearch();
   hide('exportBarClosed');
   show('exportBarOpen');
   show('exportBar');
}

function hideExport()
{
   hide('exportBar');
   hide('exportBarOpen');
   show('exportBarClosed');
}

function dequeueSelectedList()
{
   var SelectedList = [];
   $('#DequeueSourceName option').each( function(){
         if (this.selected)
         {
            SelectedList.push( this.value );
         }
     });
   return SelectedList;
}

function updateJumpToQueueEndDiv()
{
   var SourceSelector = document.getElementsByName('Source')[0];
   var Source         = SourceSelector.options[SourceSelector.selectedIndex].value;
   $('#JumpToQueueEndDiv').hide().empty();
   var DisplayLink = false;
   var SelectedName = '';

   var AllEntriesOption = LOGCSallEntriesOption;
   var ServiceEntriesOption = LOGCSserviceEntriesOption;

   //link only displayed if one source selected, or not in multiple source mode
   if (LOGCSchannelSources[Source] !== undefined &&
       LOGCSchannelSources[Source].length > 0 &&
       LOGCSchannelSources[Source][0] != Source) //is a from channel channel
   {
      var SelectedList = dequeueSelectedList();
      if (SelectedList.length == 1)
      {
         SelectedName = SelectedList[0];
         DisplayLink = true;
      }
      if (!DisplayLink)
      {
         $('#JumpToQueueEndDiv').append($('<span>Select only one source to go to the source position</span>'));
      }
   }
   else if (LOGCSchannelSources[Source] !== undefined)
   {
      DisplayLink = true;
      $('#SourceInput option').each( function(){
          if (DisplayLink && this.selected)
          {
             DisplayLink = !(this.value == AllEntriesOption || this.value == ServiceEntriesOption);
          }
       });
       if (DisplayLink && LOGCSchannelSources[Source].length == 0)
       {
          //is a to channel channel
          DisplayLink = false;
       }
   }

   if (DisplayLink)
   {
      $('#JumpToQueueEndDiv').append($('<div style="text-align:right;padding:2px 2px 0px 0px;"><a id="hrefJumpToQueueEnd" href="#" style="color:gray;">Last Processed Message</a></div>').click( function(){ jumpToQueueEnd(SelectedName) } ) );
   }
   $('#JumpToQueueEndDiv').show();
}

function updateSourceChannelSelect()
{
   var SourceSelector = document.getElementsByName('Source')[0];
   var Source         = SourceSelector.options[SourceSelector.selectedIndex].value;

   //hide all dequeue info first
   sourceLogsDiv = document.getElementById('includeSourceLogsDiv');

   //clear out list items
   $('#DequeueSourceNameDiv',sourceLogsDiv).empty();
   if (LOGCSchannelSources[Source] !== undefined &&
       LOGCSchannelSources[Source].length > 0 &&
       LOGCSchannelSources[Source][0] != Source) //is a from channel channel
   {
      JDequeueSourceNameSelect = $('<select style="width:195px" MULTIPLE id="DequeueSourceName" name="DequeueSourceName" size="3"></select>')
                                    .click(function(){
                                           refreshSearch();
                                           updateJumpToQueueEndDiv();
                                  });
      $('#jumpToQueueEndDiv').hide();
      DequeueList = LOGCSchannelSources[Source];
      for (var DequeueIndex = 0; DequeueIndex < DequeueList.length; DequeueIndex++)
      {
            OptionTag = '<option>';
            if (LOGCSinitialIncludeSourceLogs ||
                (LOGCSinitialDequeueSourceNames.indexOf(DequeueList[DequeueIndex]) >= 0))
            {
               OptionTag = '<option selected>';
            }
            JOption = $(OptionTag + '</option>')
                          .attr('value',DequeueList[DequeueIndex])
                          .text(DequeueList[DequeueIndex]);
            JDequeueSourceNameSelect.append(JOption);
      }
      $('#DequeueSourceNameDiv',sourceLogsDiv).append(JDequeueSourceNameSelect);
      $('#includeSourceLogsDiv').show();
   }
   else
   {
      $('#includeSourceLogsDiv').hide();
      if (LOGCSchannelSources[Source] !== undefined &&
          LOGCSchannelSources[Source].length > 0) //not a to channel channel
      {
         $('#jumpToQueueEndDiv').show();
      }
      else
      {
         $('#jumpToQueueEndDiv').hide();
      }

      if (LOGCSchannelInfo[Source] &&
          (LOGCSchannelInfo[Source].CountOfViewableDequeue == 0) &&
          (LOGCSchannelInfo[Source].CountOfDequeue > 0))
      {
         $('#DequeueSourceNameDiv',sourceLogsDiv).append($('<span>You do not have view permissions for any of the source channels</span>'));
         $('#includeSourceLogsDiv').show();
      }
   }
}

function onSourceChange()
{
   var SourceSelector = document.getElementsByName('Source')[0];
   var Source         = SourceSelector.options[SourceSelector.selectedIndex].value;
   updateSourceChannelSelect();
   updateJumpToQueueEndDiv();
}

var ClearDateRange = [];

function naturalDateParse(Value)
{
   if (!Value || Value == InputTimeFormatText)
   {
      return '';
   }
   var ParsedDate = Date.parse(Value);
   return (ParsedDate ? ParsedDate.toString('yyyy/MM/dd HH:mm:ss') : null);
}

function setupTimeRangeField(FieldPrefix, InitialValue)
{
   var DateTimePreview = document.getElementById(FieldPrefix+'DateTimePreview');
   var DateTimeTip     = InputTimeFormatText;
   var DateTimeField   = document.getElementById(FieldPrefix+'DateTime_local');
   var FormField       = document.getElementById(FieldPrefix+'Input');
   var JDateTimeField  = $('#'+FieldPrefix+'DateTime_local');
   var InitialDateTime = InitialValue;
   DateTimeField.value = (InitialDateTime==null || InitialDateTime=='') ? DateTimeTip : InitialDateTime;
   FormField.value = naturalDateParse(DateTimeField.value);
   var OldValue    = FormField.value;
   var delta  = function()
                {
                   DateTimeField.style.color = (DateTimeField.value==DateTimeTip) ? '#aaa' : '#000';
                   var DateStr = naturalDateParse(DateTimeField.value);
                   FormField.value = DateStr;
                   toggleFieldClearButton(FieldPrefix + 'DateTime_local', InitialDateTime);
                   if (FormField.value != OldValue)
                   {
                      OldValue = FormField.value;
                      if (DateStr !== null)
                      {
                         //don't submit if the date is invalid
                         delay(function(){hideDeleteNotify(); refreshSearch();}, 500);
                      }
                   }
                   DateTimePreview.innerHTML = (DateStr === null ? 'Invalid Date/Time' : DateStr );
                   DateTimePreview.style.display = (DateTimePreview.innerHTML == '' ? 'none' : '');
                };
   var editDT = function() { if (DateTimeField.value==DateTimeTip) DateTimeField.value = ''; delta(); };
   var doneDT = function() { if (DateTimeField.value=='') DateTimeField.value = DateTimeTip; delta(); };
   JDateTimeField.focus (editDT);
   JDateTimeField.click (editDT);
   JDateTimeField.blur  (doneDT);
   JDateTimeField.keyup (delta);
   JDateTimeField.change(delta);
   JDateTimeField.datepicker({
      defaultDate      : 'Now',
      maxDate          : 'Now',
      dateFormat       : 'yy/mm/dd',
      showOn           : 'none',
      gotoCurrent      : true,
      changeMonth      : true,
      changeYear       : true,
      showButtonPanel  : false,
      constrainInput   : false,
      hideIfNoPrevNext : true,
      onSelect         : delta
      });
   delta();

   //The image is no longer attached to the field, so we
   //manually attach for show/hide.
   $('#calendarImg'+FieldPrefix)
      .click( function(){
           $('#'+FieldPrefix+'DateTime_local').datepicker('show');
         });
   ClearDateRange[FieldPrefix] = function()
   {
      DateTimeField.value = DateTimeTip;
      delta();
   }
}

function showTimeRange()
{
   var RangeTable    = document.getElementById('TimeRangeTable');
   var RangeCheckbox = document.getElementById('ShowTimeRange_local');
   if (RangeCheckbox.checked == false) for (Prefix in ClearDateRange) ClearDateRange[Prefix]();
   RangeTable.style.display = (RangeCheckbox.checked ? '' : 'none');
}

//
// Navigation, Resizing and Initialization
//

var VisibleArea = null;
function resize(Area)
{
   if( Area ) { VisibleArea = Area; }

   var NewHeight
      = WINgetWindowHeight()
      - WINwindowOffsetTop(VisibleArea)
      - WINwindowOffsetLeft(VisibleArea);

   var ReparseWarning = $('#reparseWarning');
   if (ReparseWarning.css('display') != 'none')
   {
      NewHeight -= ReparseWarning.outerHeight();
   }

   VisibleArea.style.height = (NewHeight - 15) + 'px';
   VisibleArea.style.width  = '100%';

   resizeInnerViewContents();
}

var LastEntryData = null;
var RequestingMoreLines = false;
function onScrollEntryArea(Area)
{
   if (RequestingMoreLines)
   {
      return;
   }

   if (((Area.scrollHeight - (Area.scrollTop + Area.offsetHeight)) < 300) &&  //near the bottom
       LastEntryData &&
       LastEntryData.Location &&
       LastEntryData.Params &&
       ((LastEntryData.StartLineOffset + LastEntryData.CountOfLines) < LastEntryData.TotalCountOfLines)) //still got some more
   {
      var OldParams = LastEntryData.Params;
      var NewOffset = LastEntryData.StartLineOffset + LastEntryData.CountOfLines;
      var ThisLocation = LastEntryData.Location;
      var ThisParams = OldParams + '&Offset=' + NewOffset;
      if (LastEntryData.Guid)
      {
         ThisParams += '&Guid=' + LastEntryData.Guid;
      }

      RequestingMoreLines = true;
      AJAXpost(ThisLocation, ThisParams,
         function(Data, ContentType)
         {
            RequestingMoreLines = false;

            if( ContentType.match('application/json') )
            {
               var ThisEntryData = eval(Data);

               if (LastEntryData &&
                   ThisEntryData.MessageId === LastEntryData.MessageId &&
                   ThisEntryData.DisplayMode === LastEntryData.DisplayMode)
               {
                  var ResetEntryContents = (LastEntryData.Guid &&
                                            LastEntryData.Guid != ThisEntryData.Guid &&
                                            ThisEntryData.StartLineOffset == 0);

                  // If the offsets mismatch, we discard the response (unless we are explicitly
                  // resetting the entire text).
                  if (ThisEntryData.StartLineOffset === NewOffset || ResetEntryContents)
                  {
                     LastEntryData = ThisEntryData;
                     //note we don't update LastEntryData.Params
                     LastEntryData.Location = ThisLocation;
                     LastEntryData.Params = OldParams;

                     // We only need to use inner pre tags in IE, and only in non-text view modes.
                     var UseInnerPre = (ThisEntryData.DisplayMode != '' && TOOLisInternetExplorer());

                     var EntryViewContainer = document.getElementById('textViewContainer');
                     if (!EntryViewContainer)
                     {
                        EntryViewContainer = document.getElementById('ParsedViewContainer');
                     }
                     if (EntryViewContainer)
                     {
                        if (ResetEntryContents)
                        {
                           EntryViewContainer.innerHTML = LastEntryData.Message;
                        }
                        else if (UseInnerPre)
                        {
                           $(EntryViewContainer).append('<pre class="innerPreChunk">' + LastEntryData.Message + '</pre>');
                        }
                        else
                        {
                           // jQuery's append() is significantly faster than 'innerHTML += Text' in Safari, FF, and Chrome.
                           $(EntryViewContainer).append(LastEntryData.Message);
                        }

                        if (LastEntryData.StartLineOffset + LastEntryData.CountOfLines >= LastEntryData.TotalCountOfLines)
                        {
                           $('#loadingMoreLinesDiv').remove();
                        }
                     }
                  }
               }
            }
            else
            {
               showTimeOutMessage(showCurrentEntry);
            }
         },
         function(Error)
         {
            RequestingMoreLines = false;
            showDisconnectMessage(Error, showCurrentEntry);
         }
      );
   }
}

function onScrollLogArea()
{
   // We ensure that there are at least two more pages to scroll into, in either direction.

   if( (LogArea.scrollHeight - LogArea.scrollTop) / LogArea.offsetHeight < 3 )
   {
      showMore('reverse');
   }

   if( LogArea.scrollTop / LogArea.offsetHeight < 2 )
   {
      if( LogArea.scrollTop == 0 )
      {
         clearNewMessageCount();
      }

      if( !PollingForward || CountOfNewMessage == 0 )
      {
         showMore('forward');
      }
   }
}

function LOGdisplayNewestEntry()
{
   if( Finished['forward'] || document.getElementsByName('RefMsgId')[0].value )
   {
      var Count = LogTable.rows.length-2;
      if (Count > 0) scrollTo(setHighlightEntry(elementIdToMessageId(LogTable.rows[1].id)));
   }
   else
   {
      HighlightEntry.MessageId = null;
      clearSearch();
      setFinished('reverse', false, 'Searching...');
      showMore('reverse');
   }
}

function LOGdisplayOldestEntry()
{
   if( Finished['reverse'] )
   {
      var Count = LogTable.rows.length-2;
      if (Count > 0) scrollTo(setHighlightEntry(elementIdToMessageId(LogTable.rows[Count].id)));
   }
   else
   {
      HighlightEntry.MessageId = null;
      clearSearch();
      setFinished('forward', false, 'Searching...');
      showMore('forward');
   }
}

function onKeyDown(Event)
{
   if ( window.event ) Event = window.event;
   if ( document.getElementById('logView').style.display == 'none' ) return;
   var SourceElement = (Event.srcElement) ? (Event.srcElement) : (Event.target);
   if (SourceElement != null)
   {
      if (SourceElement.nodeName == 'OPTION' || SourceElement.nodeName == 'SELECT')
      {
         return;
      }
      if (SourceElement.nodeName == 'INPUT')
      {
         if (document.getElementById('filterInput') == SourceElement && 13 == Event.keyCode)
         {
            // no delay for search if Enter is hit in filter input box
            delay(function(){hideDeleteNotify(); refreshSearch();}, 1);
         }
         return;
      }
   }

   switch( Event.keyCode )
   {
   case 13:  // Enter
      var Row = getHighlightedRow();
      if (Row != null) $('#'+Row.id).click();
      break;

   case 38:  // Up-arrow
      var Row = getHighlightedRow();
      if (Row != null)
      {
         var RowIndex = Math.max(1, Row.rowIndex-1);
         scrollTo(setHighlightEntry(elementIdToMessageId(LogTable.rows[RowIndex].id)));
      }
      break;

   case 33:  // Page-up
      var Row = getHighlightedRow();
      if (Row != null)
      {
         var RowIndex = Math.max(1, Row.rowIndex-5);
         scrollTo(setHighlightEntry(elementIdToMessageId(LogTable.rows[RowIndex].id)));
      }
      break;

   case 40:  // Down-arrow
      var Row = getHighlightedRow();
      if (Row != null)
      {
         var RowIndex = Math.min(LogTable.rows.length-2, Row.rowIndex+1);
         scrollTo(setHighlightEntry(elementIdToMessageId(LogTable.rows[RowIndex].id)));
      }
      break;

   case 34:  // Page-down
      var Row = getHighlightedRow();
      if (Row != null)
      {
         var RowIndex = Math.min(LogTable.rows.length-2, Row.rowIndex+5);
         scrollTo(setHighlightEntry(elementIdToMessageId(LogTable.rows[RowIndex].id)));
      }
      break;

   case 36:  // Home
      LOGdisplayNewestEntry();
      break;

   case 35:  // End
      LOGdisplayOldestEntry();
      break;
   }
}

function checkLogUsageBar()
{
   var OtherUsage = document.getElementById('other_usage_bar');
   var IguanaUsage = document.getElementById('iguana_usage_bar');

   var IguanaPercentage = LOGCSiguanaUsagePercentage;
   var OtherPercentage = LOGCSotherUsagePercentage;

   OtherUsage.style.width = (OtherPercentage*2) + 'px';
   IguanaUsage.style.width = (IguanaPercentage*2)+ 'px';
}

function toggleFieldClearButton(FieldName, EmptyValue)
{
   var Field      = document.getElementsByName(FieldName)[0];
   var ClearField = document.getElementById('clear' + FieldName);
   if (Field) Field.style.color = !Field.value || Field.value != EmptyValue ? '#000'    : '#aaa';
   if (ClearField) ClearField.style.visibility =       Field.value != EmptyValue ? 'visible' : 'hidden';
}

function initClearButton(FieldName, ClearProp)
{
   var Field      = document.getElementsByName(FieldName)[0];
   var ClearField = document.getElementById('clear' + FieldName);

   function applyTheme()
   {
      toggleFieldClearButton(FieldName,ClearProp.EmptyValue);
   }

   if( !Field.value )
   {
      Field.value = ClearProp.EmptyValue;
   }
   applyTheme();

   var OldValue = Field.value;
   Field.setOldValue = function() { OldValue = Field.value; };
   Field.getOldValue = function() { return OldValue;        };
   Field.onkeyup = function() {
      if( Field.value != OldValue )
      {
         applyTheme();
         OldValue = Field.value;
         delay(function(){hideDeleteNotify(); refreshSearch();}, 750);
      }
   };

   Field.oldonclick = Field.onclick;
     Field.onclick = function() {
      if( this.value == ClearProp.EmptyValue )
      {
         this.value = '';
         applyTheme();
      }
      if (this.oldonclick)
      {
         this.oldonclick();
      }
   };

   Field.onblur = function() {
      if( Field.value == '' )
      {
         Field.value = ClearProp.EmptyValue;
         applyTheme();
      }
   };

   if (ClearProp.OnClickClear)
   {
      ClearField.onclick = function(){
         ClearProp.OnClickClear();
         applyTheme();
      }
   }
   else
   {
      ClearField.onclick = function() {
         Field.value = ClearProp.EmptyValue;
         OldValue = Field.value;
         applyTheme();
         hideDeleteNotify();
         refreshSearch();
      }
   };
}

//
// Page initialization
//

// Common initialization which is required for all "varieties" of
// the log browser.
//
var DescriptionAreaControl;
function initLogBrowser()
{
   if (document.getElementById('entryDescription'))
   {
      DescriptionAreaControl = new SLIDEcontrol('entryDescription',{content: '', anim_duration : 200});
   }

   var EntryArea = document.getElementById('entryArea');
   if (EntryArea)
   {
      EntryArea.onscroll = function() { onScrollEntryArea(EntryArea) };
   }
}

//
// Log Browser heart beat
//

var LOGheartBeatTimerId = null;
function LOGdoHeartBeat()
{
   if (LOGheartBeatTimerId)
   {
      clearTimeout(LOGheartBeatTimerId);
   }

   AJAXpost('log_browse_heart_beat', 'LogBrowserGuid=' + LOGCSlogBrowserGuid + '&AutomaticRequest=1',
         function(Data, ContentType)
         {
            if (ContentType.match('application/json'))
            {
               HeartBeatResponseObject = JSON.parse(Data);
               if (HeartBeatResponseObject.ErrorDescription) {
                  clearTimeout(LOGheartBeatTimerId);
                  LOGheartBeatTimerId = null;
                  showTimeOutMessage(LOGdoHeartBeat);
               } else {
                  var ShowDisableRemoteWindows = HeartBeatResponseObject && HeartBeatResponseObject.ShowDisableRemoteWindows;
                  document.getElementById('disableRemoteWindows').style.display =  ShowDisableRemoteWindows ? '' : 'none';

                  var LoggedOut = HeartBeatResponseObject && typeof(HeartBeatResponseObject.LoggedIn) !== 'undefined' && HeartBeatResponseObject.LoggedIn == false ;
                  if (LoggedOut)
                  {
                     showTimeOutMessage (function () {});
                  }

                  clearTimeout(LOGheartBeatTimerId);
                  LOGheartBeatTimerId = setTimeout(LOGdoHeartBeat, 5000);
               }
            }
            else
            {
               showTimeOutMessage(LOGdoHeartBeat);
            }
         },
         function(Error)
         {
            showDisconnectMessage(Error, LOGdoHeartBeat);
         }
      );
}

function LOGstartFtsUpdatePoll() {
   var PanelId = '#log_usage_status_panel';
   ftsUpdateError = function(error){
      $(PanelId).html('');
      setTimeout('LOGstartFtsUpdatePoll();', 1000);
   }

   ftsUpdateComplete = function(Data){
      var Status = null;
      try {
         Status = JSON.parse(Data);
      } catch(e) {}

      if (Status && Status.status) {
         var Restart = true;
         StatusStr = '';

         if (Status.status == 'Indexing') {
            StatusStr = 'Indexing '
                      + Status.current_file + ' for text searching (' + Status.percent + '%, '
                      + Status.eta + ' left). ';

            StatusStr += (Status.count_of_unindexed > 1 ? Status.count_of_unindexed-1 + ' file(s) remaining.' : '');
         } else if (Status.status == 'Complete') {
            Restart = false;
         }

         $(PanelId).html(StatusStr);

         if (Restart) {
            setTimeout('LOGstartFtsUpdatePoll();', 1000);
         }
      } else {
         ftsUpdateError();
      }
   }
   AJAXpost('fts_update_status', '', ftsUpdateComplete, ftsUpdateError);
}

$(document).ready(function() {
   $('#export-to-mapper-error').dialog({
      'autoOpen': false,
      'modal': true,
      'title': 'Export to Translator',
      'buttons': {
         'Close': function() { $(this).dialog('close'); }
      }
   });
   $('#export-to-mapper-setup').dialog({
      'autoOpen': false,
      'height': 230,
      'modal': true,
      'title': 'Export to Translator'
   });
   function fail(Text) {
      $('#export-to-mapper-error')
         .text(Text)
         .dialog('open');
   }
   function exportToMapper(OnFinished) {
      var TargetName = $('#mapper-targets option:selected').text();
      var TargetGuid = $('#mapper-targets').val();

      var TheForm = $("#FilterForm");
      fixField('DequeueSourceName', TheForm);
      fixField('Type', TheForm);
      var FormData = TheForm.serialize();
      unFixField('Type', TheForm);
      unFixField('DequeueSourceName', TheForm);

      MiniLogin.getJSON('/export_to_mapper?' + FormData,
         {'Action': 'export', 'TargetGuid': TargetGuid },
         function(Result) {
console.log(Result);
            $('#to-mapper-progress').hide();
            $('#to-mapper-finished')
               .text(Result.Success
                     ? Result.SizeOfImport + ' message(s) exported.'
                     : Result.Error || 'Export failed.')
               .show();
            if(Result.Success) {
               var SucceededPal = $('#to-mapper-redirect');
               SucceededPal.find('a').attr('href', function(i, Link) {
                     return Link
                        .replace(/(&MapperGuid=)[^&]*/, '$1' + encodeURIComponent(TargetGuid))
                        .replace(/(&Index=)[^&]*/, '$1' + (Result.ImportStart + 1));
                  });
               SucceededPal.show();
            }
            if(OnFinished)
               OnFinished();
         });
   }
   function showExportDialog(ExportRelated, Action, ChannelName) {
      $('#mapper-targets').html('<option value="">Loading...</option>');
      $('#to-mapper-progress,#to-mapper-finished,#to-mapper-redirect').hide();
      var Setup = $('#export-to-mapper-setup')
         .dialog('option', 'buttons', {
            'Cancel': function() { $(this).dialog('close'); }})
         .dialog('open');
      MiniLogin.getJSON('/export_to_mapper?Action=list_targets' + (ChannelName ? '&ChannelName=' + ChannelName : ''),
      function(Result) {
         // Remove the drop-down list from the dialog box if there aren't any translator projects
         // for which the user can export the message.
         if (Result.Targets.length == 0) {
            Setup.html("<p> You don't have the necessary permissions to export this message to any translator projects " +
               "or there aren't any projects available. </p>");
            return;
         }

         var Sel = $('#mapper-targets');
         var First = $('option:first', Sel);
         $.map(Result.Targets, function(Target) {
            First.clone()
               .attr('value', Target.Guid)
               .text(Target.Name)
               .appendTo(Sel);
         });
         Sel.change(function() {
            var Buttons = {'Cancel': function() { $(this).dialog('close'); }};
            if(Sel.val()) {
               Buttons['Export'] = function() {
                  Setup.dialog('option', 'buttons', {
                     'Close': function() { $(this).dialog('close'); }
                  });
                  $('#to-mapper-progress').show();
                  if(!ExportRelated) {
                     Action();
                  } else {
                     var Run = function(OnRetry) {
                        Action(exportToMapper, OnRetry);
                     }
                     Run(Run);
                  }
               };
            }
            Setup.dialog('option', 'buttons', Buttons);
         });
         if(ChannelName && ChannelName == Result.Targets[0].ChannelName){
          First.remove();
          Sel.val($('option:first', Sel).val());
          Sel.trigger('change');
         }
         else{
          First.text('Select...');
          Sel.val(First.val());  // Fix for IE 8 (Bug #19497).
         }

         var NewWidth = $('#mapper-targets').width() + 56;
         if (NewWidth > Setup.width()) {
            Setup.dialog('option', 'width', NewWidth);
         }
      });
   }
   $('#export-to-mapper').click(function() {
      var Types = $('select[name="Type"]').val() || [];
      var ExportRelated = $('input[name="Export.Related"]').is(':checked');
      if(!ExportRelated && (Types.length != 1 || Types[0] != 'messages')) {
         fail('Only messages can be exported to the Translator.');
      } else {
         showExportDialog(ExportRelated, exportToMapper, (CurrentEntry ? CurrentEntry.Channel : null));
      }
   });
   $('#entryToMapper').click(function() {
      var Plan = $('input[name="Export.Plan"]').val(CurrentEntry.MessageId);
      showExportDialog(false, function() {
         exportToMapper(function() { Plan.val(''); });
      }, CurrentEntry.Channel);
   });
});
