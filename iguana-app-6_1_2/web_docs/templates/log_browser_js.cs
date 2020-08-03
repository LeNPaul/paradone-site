<?cs # 
   Contains javascript variable and function definitions required by log_browser.js. 
   Include inside a script tag
?>

//
// General Stuff
//

// Paths to icons that may need to be switched using javascript
var MsgFailureAckIconSrc = '<?cs var:skin("images/msg-failure-ack.gif") ?>';
var MsgFailureIconSrc    = '<?cs var:skin("images/msg-failure.gif") ?>';

var ResubmitIconGreySrc  = '<?cs var:skin('images/log-browser-resubmit-grey.gif') ?>';
<?cs if:CurrentUserCanAdmin ?>
var ResubmitIconSrc      = '<?cs var:skin('images/log-browser-resubmit.gif') ?>' ;
<?cs else ?>
var ResubmitIconSrc      = ResubmitIconGreySrc;
<?cs /if ?>

var LOGCScurrentUserCanAdmin = <?cs if:CurrentUserCanAdmin ?>true<?cs else ?>false<?cs /if ?>;
var LOGCStypeFilter = '<?cs var:javascript_escape(TypeFilter) ?>';
var LOGCSstandardHl7LibraryVmdPath = '<?cs var:js_escape(html_escape(StandardHl7LibraryVmdPath)) ?>';
var LOGCShelpIconLocation = '/<?cs var:skin("images/help_icon.gif") ?>';
var LOGCScalendarIconLocation = '/<?cs var:skin("images/calendar.gif")?>';
var LOGCSallEntriesOption = '<?cs var:js_escape(SourceOptions.AllEntriesOption) ?>';
var LOGCSserviceEntriesOption = '<?cs var:js_escape(SourceOptions.ServiceEntriesOption) ?>';

var LOGCSiguanaUsagePercentage = <?cs var: #IguanaUsagePercentage ?>;
var LOGCSotherUsagePercentage  = <?cs var: #OtherUsagePercentage ?>;

<?cs if:CurrentUserCanAdmin ?>
   function canEditChannel(ChannelName) { return true; }
<?cs else ?>
   var canEditChannel = function() {
      var EditableChannels = '<?cs var:js_escape(EditableChannels) ?>'.split('\n');
      return function(ChannelName) {
         return EditableChannels.indexOf(ChannelName) >= 0;
      };
   }();
<?cs /if ?>

//
// View Switching
//

function LOGCSdropdownButtonImgSrc(Mode, Pressed)
{
   if (Mode == 'Text')
   {
      return Pressed ? '<?cs var:skin('images/log_views/dropdown-text-on.gif') ?>' : '<?cs var:skin('images/log_views/dropdown-text.gif') ?>';
   }
   else if (Mode == 'Hex')
   {
      return Pressed ? '<?cs var:skin('images/log_views/dropdown-hex-on.gif') ?>' : '<?cs var:skin('images/log_views/dropdown-hex.gif') ?>';
   }
   else if (Mode == 'SegmentMessage')
   {
      return Pressed ? '<?cs var:skin('images/log_views/dropdown-segment-on.gif') ?>' : '<?cs var:skin('images/log_views/dropdown-segment.gif') ?>';
   }
   else if (Mode == 'SegmentGrammar')
   {
      return Pressed ? '<?cs var:skin('images/log_views/dropdown-grammar-on.gif') ?>' : '<?cs var:skin('images/log_views/dropdown-grammar.gif') ?>';
   }
   else if (Mode == 'Table')
   {
      return Pressed ? '<?cs var:skin('images/log_views/dropdown-table-on.gif') ?>' : '<?cs var:skin('images/log_views/dropdown-table.gif') ?>';
   }
   else if (Mode == 'TableText')
   {
      return Pressed ? '<?cs var:skin('images/log_views/dropdown-tableText-on.gif') ?>' : '<?cs var:skin('images/log_views/dropdown-tableText.gif') ?>';
   }
   else if (Mode == 'SQL')
   {
      return Pressed ? '<?cs var:skin('images/log_views/dropdown-sql-on.gif') ?>' : '<?cs var:skin('images/log_views/dropdown-sql.gif') ?>';
   }
   else if (Mode == 'PluginOutput')
   {
      return Pressed ? '<?cs var:skin('images/log_views/dropdown-plugin-on.gif') ?>' : '<?cs var:skin('images/log_views/dropdown-plugin.gif') ?>';
   }
   else if (Mode == 'MapperOutput')
   {
      return Pressed ? '<?cs var:skin('images/log_views/dropdown-translator-on.gif') ?>' : '<?cs var:skin('images/log_views/dropdown-translator.gif') ?>';
   }
}

function LOGCSdropdownOptionImgSrc(Option, Pressed, Disabled)
{
   if (Option.hasClass('Text'))
   {
      return Disabled ? '<?cs var:skin('images/log_views/square-text-disabled.gif') ?>' :
             (Pressed ? '<?cs var:skin('images/log_views/square-text-on.gif') ?>' : '<?cs var:skin('images/log_views/square-text.gif') ?>');
   }
   else if (Option.hasClass('Hex'))
   {
      return Disabled ? '<?cs var:skin('images/log_views/square-hex-disabled.gif') ?>' :
             (Pressed ? '<?cs var:skin('images/log_views/square-hex-on.gif') ?>' : '<?cs var:skin('images/log_views/square-hex.gif') ?>');
   }
   else if (Option.hasClass('SegmentMessage'))
   {
      return Disabled ? '<?cs var:skin('images/log_views/square-segment-disabled.gif') ?>' :
             (Pressed ? '<?cs var:skin('images/log_views/square-segment-on.gif') ?>' : '<?cs var:skin('images/log_views/square-segment.gif') ?>');
   }
   else if (Option.hasClass('SegmentGrammar'))
   {
      return Disabled ? '<?cs var:skin('images/log_views/square-grammar-disabled.gif') ?>' :
             (Pressed ? '<?cs var:skin('images/log_views/square-grammar-on.gif') ?>' : '<?cs var:skin('images/log_views/square-grammar.gif') ?>');
   }
   else if (Option.hasClass('Table'))
   {
      return Disabled ? '<?cs var:skin('images/log_views/square-table-disabled.gif') ?>' :
             (Pressed ? '<?cs var:skin('images/log_views/square-table-on.gif') ?>' : '<?cs var:skin('images/log_views/square-table.gif') ?>');
   }
   else if (Option.hasClass('TableText'))
   {
      return Disabled ? '<?cs var:skin('images/log_views/square-tableText-disabled.gif') ?>' :
             (Pressed ? '<?cs var:skin('images/log_views/square-tableText-on.gif') ?>' : '<?cs var:skin('images/log_views/square-tableText.gif') ?>');
   }
   else if (Option.hasClass('SQL'))
   {
      return Disabled ? '<?cs var:skin('images/log_views/square-sql-disabled.gif') ?>' :
             (Pressed ? '<?cs var:skin('images/log_views/square-sql-on.gif') ?>' : '<?cs var:skin('images/log_views/square-sql.gif') ?>');
   }
   else if (Option.hasClass('PluginOutput'))
   {
      return Disabled ? '<?cs var:skin('images/log_views/square-plugin-disabled.gif') ?>' :
             (Pressed ? '<?cs var:skin('images/log_views/square-plugin-on.gif') ?>' : '<?cs var:skin('images/log_views/square-plugin.gif') ?>');
   }
   else if (Option.hasClass('MapperOutput'))
   {
      return Disabled ? '<?cs var:skin('images/log_views/square-translator-disabled.gif') ?>' :
             (Pressed ? '<?cs var:skin('images/log_views/square-translator-on.gif') ?>' : '<?cs var:skin('images/log_views/square-translator.gif') ?>');
   }
}

function LOGCSshowMarkButton()
{
   ButtonElement = document.getElementById('entryMarked');
   ImageElement = document.getElementById('entryMarkedImg');
   
   if (ButtonElement)
   {
      ButtonElement.style.display = (CurrentEntry.Type == 'Error' ? '' : 'none');
   }
   
   if (ImageElement)
   {
      if (CurrentEntry.Acknowledged)
      {
         if (CurrentEntry.Deleted)
         { 
            ImageElement.src = '<?cs var:skin('images/log-browser-ack-yes-grey.gif') ?>';
         }
         else
         {
            ImageElement.src = '<?cs var:skin('images/log-browser-ack-yes.gif') ?>';
         }
      }
      else
      {
         if (CurrentEntry.Deleted)
         { 
            ImageElement.src = '<?cs var:skin('images/log-browser-ack-no-grey.gif') ?>';
         }
         else
         {
            ImageElement.src = '<?cs var:skin('images/log-browser-ack-no.gif') ?>';
         }
      }
   }

}

<?cs set:ResubmitDialogPadding = 16 ?>
<?cs set:ResubmitDialogBorder = 5 ?>
<?cs set:ResubmitDialogBottomPadding = 10 ?>
<?cs set:ResubmitDialogMargin = 36 ?>

function LOGCSgetAvailableResubmitDialogHeight(IncludeEditDiv, IncludePreviewDiv)
{
   var AvailableHeight = $('#resubmitDialog').innerHeight() - <?cs var:#ResubmitDialogBottomPadding ?>
                         - $('#resubmitTopControls').outerHeight()
                         - $('#resubmitEditControls').outerHeight()
                         - $('#resubmitPreviewControls').outerHeight()
                         - $('#resubmitComplete').outerHeight();
   
   var AvailableHeightDivisor = 0;
   if (IncludeEditDiv)
   {
      AvailableHeightDivisor++;
      AvailableHeight -= 22; // resubmitEdit's top and bottom margin and border widths
   }
   if (IncludePreviewDiv)
   {
      AvailableHeightDivisor++;
      AvailableHeight -= 12; // resubmitPreview's top and bottom margin and border widths
   }
   // Make sure we don't divide by 0.
   AvailableHeightDivisor = (AvailableHeightDivisor ? AvailableHeightDivisor : 1);
   
   return Math.floor(AvailableHeight/AvailableHeightDivisor);
}

function LOGCSresizeResubmitDialog()
{
   var Page = document.documentElement;
   var Frame = document.getElementById('resubmitFrame');
   var ResubmitDialog = document.getElementById('resubmitDialog');
   
   Frame.style.display = 'none';
   Frame.style.height = Math.max(Page.scrollHeight, WINgetWindowHeight()) + 'px';
   Frame.style.width  = Page.scrollWidth  + 'px';
   Frame.style.display = '';
   
   if( Frame.contentWindow )  // IE6 doesn't use iframe background colors.
   {
      Frame.contentWindow.document.body.style.background = 'gray';
   }
   
   show('resubmitDialog');
   
   ResubmitDialog.style.height = (WINgetWindowHeight()
                                 - (2*<?cs var:#ResubmitDialogMargin ?>)
                                 - (2*<?cs var:#ResubmitDialogBorder ?> + <?cs var:#ResubmitDialogBottomPadding ?>)) + 'px';
   ResubmitDialog.style.width  = (WINgetWindowWidth()
                                 - (2*<?cs var:#ResubmitDialogMargin ?>)
                                 - (2*<?cs var:#ResubmitDialogBorder ?>))  + 'px';
   
   var ResubmitEdit = $('#messageEditor');
   var ResubmitPreview = $('#resubmitPreview');
   
   var AvailableHeight = LOGCSgetAvailableResubmitDialogHeight(visible('resubmitEdit'), visible('resubmitPreview'));
   
   var AvailableWidth  = $(ResubmitDialog).innerWidth();
   
   // 6 or 2 == element's padding and border width for left and right
   ResubmitEdit.css(   'height', AvailableHeight + 'px');
   ResubmitEdit.css(   'width',  (AvailableWidth - (2*<?cs var:#ResubmitDialogPadding ?> + 6)) + 'px');
   ResubmitPreview.css('height', AvailableHeight + 'px');
   ResubmitPreview.css('width',  (AvailableWidth - (2*<?cs var:#ResubmitDialogPadding ?> + 2)) + 'px');
   
   resizeInnerViewContents();
}

function LOGCSinitParsersBySource()
{
   return {
             <?cs each: route = DatabaseRoutes ?>
                '<?cs var:js_escape(route.Source.Name) ?>': [
                   <?cs each: parser = route.Parsers ?>
                      {Guid:'<?cs var:js_escape(parser.Guid) ?>', Name:'<?cs var:js_escape(parser.Name) ?>'},
                   <?cs /each ?>
                   {Guid:'HL7', Name:'HL7 Standard Library'}
                ],
             <?cs /each ?>
             undefined: ['pop me'] 
          };
}

function LOGCSinitIsToPluginBySource()
{
   return {
             <?cs each: channel = SourceOptions.Channels ?>
                '<?cs var:js_escape(channel.Name) ?>' : <?cs var:#channel.IsToPlugin ?>,
             <?cs /each ?>
             undefined : undefined
          };
}

function LOGCSinitIsToMapperBySource()
{
   return {
             <?cs each: channel = SourceOptions.Channels ?>
                '<?cs var:js_escape(channel.Name) ?>' : <?cs var:#channel.IsToMapper ?>,
             <?cs /each ?>
             undefined : undefined
          };
}


//
// Iteration
//

<?cs if:exclude_iterator_defintion ?>
var Iterator = null;
<?cs else ?>

var Iterator = function()
{
   var CurrentIndex = 0;
   function findCurrentIndex()
   {
      if( CurrentIndex >= LogEntries.length )
      {
         CurrentIndex = Math.floor(LogEntries.length / 2);
      }

      if( LogEntries[CurrentIndex].MessageId == CurrentEntry.MessageId )
      {
         return true;
      }

      var CurrentDate = new Date(CurrentEntry.Date);
      var IndexDate   = new Date(LogEntries[CurrentIndex].Date);

      var Delta = +1;  // Search backward in time.

      if( IndexDate < CurrentDate || !(IndexDate > CurrentDate) &&
          1*LogEntries[CurrentIndex].Position < 1*CurrentEntry.Position )
      {
         Delta = -1;  // Search forward in time.
      }

      var Index = CurrentIndex + Delta;
      while( Index >= 0 && Index < LogEntries.length )
      {
         if( LogEntries[Index].MessageId == CurrentEntry.MessageId )
         {
            CurrentIndex = Index;
            return true;
         }

         Index += Delta;
      }

      return false;
   }

   function showTooltip(Tip)
   {
      return function() {
         TOOLtooltipLink(Tip, undefined, this, { Left: -8 });

         this.onmouseout = function() {
            TOOLtooltipClose();
         }
      }
   }

   function apply(List, Action)
   {
      for(var Index in List)
      {
         Action(List[Index]);
      }
   }

   var Next = {
      Action: moveNext,
      Tip:    'Display next matching entry.',
      NormalImage:   '<?cs var:skin("images/msg-next.gif") ?>',
      HoverImage:    '<?cs var:skin("images/msg-next-hover.gif") ?>',
      PressedImage:  '<?cs var:skin("images/msg-next-active.gif") ?>',
      DisabledImage: '<?cs var:skin("images/msg-next-disabled.gif") ?>'
   };

   var Prev = {
      Action: movePrev,
      Tip:    'Display previous matching entry.',
      NormalImage:   '<?cs var:skin("images/msg-prev.gif") ?>',
      HoverImage:    '<?cs var:skin("images/msg-prev-hover.gif") ?>',
      PressedImage:  '<?cs var:skin("images/msg-prev-active.gif") ?>',
      DisabledImage: '<?cs var:skin("images/msg-prev-disabled.gif") ?>'
   };

   function enableButton(Button, Details)
   {
      Button.src     = Details.NormalImage;
      Button.onclick = Details.Action;

      Button.onmouseover = function() {
         this.src = Details.HoverImage;
         TOOLtooltipLink(Details.Tip, undefined, this, { Left: -12 });
      };
      Button.onmouseout = function() {
         this.src = Details.NormalImage;
         TOOLtooltipClose();
      };
      Button.onmousedown = function() {
         this.src = Details.PressedImage;
      };
      Button.onmouseup = function() {
         this.src = Details.HoverImage;
      };
   }

   function disableButton(Button, Details)
   {
      Button.src     = Details.DisabledImage;
      Button.onclick = null;

      Button.onmouseover = function() {
         TOOLtooltipLink(Details.Tip, undefined, this, { Left: -12 });
      };
      Button.onmouseout = function() {
         TOOLtooltipClose();
      };
      Button.onmousedown = null;
      Button.onmouseup   = null;
   }

   var NextButtons;
   var EnabledNext;
   function enableNext()
   {
      if( !EnabledNext )
      {
         EnabledNext = true;
         enableButton(document.getElementById('entryNext'), Next);
      }
   }

   var PrevButtons;
   var EnabledPrev;
   function enablePrev()
   {
      if( !EnabledPrev )
      {
         EnabledPrev = true;
         enableButton(document.getElementById('entryPrev'), Prev);
      }
   }

   function disableNext()
   {
      if( EnabledNext )
      {
         EnabledNext = false;
         disableButton(document.getElementById('entryNext'), Next);
      }
   }

   function disablePrev()
   {
      if( EnabledPrev )
      {
         EnabledPrev = false;
         disableButton(document.getElementById('entryPrev'), Prev);
      }
   }

   function moveNext()
   {
      var MoveOk = true;
      if( !findCurrentIndex() )
      {
         disableNext();
         disablePrev();
         MoveOk = false;
      }
      else
      {
         if( CurrentIndex > 0 )
         {
            showEntry( LogEntries[--CurrentIndex], 'next' );
            enablePrev();
         }

         if( CurrentIndex <= 0 && Finished['forward'] )
         {
            disableNext();
            MoveOk = false;
         }
         else if( CurrentIndex < 5 )
         {
            showMore('forward');
         }
      }
      return MoveOk;
   }

   function movePrev()
   {
      var MoveOk = true;
      if( !findCurrentIndex() )
      {
         disableNext();
         disablePrev();
         MoveOk = false;
      }
      else
      {
         if( CurrentIndex < LogEntries.length - 1 )
         {
            showEntry( LogEntries[++CurrentIndex], 'previous' );
            enableNext();
         }

         if( CurrentIndex >= LogEntries.length - 1 && Finished['reverse'] )
         {
            disablePrev();
            MoveOk = false;
         }
         else if( LogEntries.length - CurrentIndex <= 5 )
         {
            showMore('reverse');
         }
      }
      return MoveOk;
   }

   var Enabled;
   function updateButtons()
   {
      if( Enabled && CurrentEntry && findCurrentIndex() )
      {
         var EnableNext = CurrentIndex > 0                     || !Finished["forward"];
         var EnablePrev = CurrentIndex < LogEntries.length - 1 || !Finished["reverse"];

         if( EnableNext ) enableNext(); else disableNext();
         if( EnablePrev ) enablePrev(); else disablePrev();
      }
   }

   return {
      enable: function() {
         enableNext();
         enablePrev();
         Enabled = true;
         updateButtons();
      },
      disable: function() {
         disableNext();
         disablePrev();
         Enabled = false;
      },
      update: updateButtons,
      next: moveNext,
      prev: movePrev
   };
}();

<?cs /if ?>


//
// Side-bar Goodies
//

var LOGCSinitialDequeueSourceNames = '<?cs var:js_escape(DequeueSourceName) ?>';
var LOGCSinitialIncludeSourceLogs = '<?cs var:js_escape(IncludeSourceLogs) ?>' == 'true';
var LOGCSinitialJumpToQueueEnd = '<?cs var:js_escape(JumpToQueueEnd) ?>' == 'true';

var LOGCSchannelSources  = new Array();
<?cs each: channel = SourceOptions.Channels ?>
   LOGCSchannelSources['<?cs var:js_escape(channel.Name) ?>'] = [
   <?cs each: dequeueinfo = channel.DequeueInfo ?>'<?cs var:js_escape(dequeueinfo.SourceName) ?>',<?cs /each ?>'bogus'];
   LOGCSchannelSources['<?cs var:js_escape(channel.Name) ?>'].pop();
<?cs /each?>

var LOGCSchannelInfo = new Array();
<?cs each: channel = SourceOptions.Channels ?>
   LOGCSchannelInfo['<?cs var:js_escape(channel.Name) ?>'] = 
   { 
      SourceType: '<?cs var:js_escape(channel.SourceType) ?>',
      DestType: '<?cs var:js_escape(channel.DestType) ?>',
      HasFilter: <?cs if:channel.HasFilter ?>true<?cs else ?>false<?cs /if ?>,
      FilterAfterLogging: <?cs if:js_escape(channel.FilterAfterLogging) ?>true<?cs else ?>false<?cs /if ?>,
      CountOfViewableDequeue: <?cs var:#channel.CountOfViewableDequeue ?>,
      CountOfDequeue: <?cs var:#channel.CountOfDequeue ?>
   }
<?cs /each?>

//
// Row pop-out stuff.
//

var LOGCSisChildWindow        = <?cs if:IsChildWindow ?>true<?cs else ?>false<?cs /if ?>;
var LOGCSlogBrowserGuid       = '<?cs var:LogBrowserGuid ?>'; // valid iff LOGCSisChildWindow == false
var LOGCSremoteValidationKey  = '<?cs var:RemoteValidationKey ?>';

var LOGCSrowPopoutButtonImage      = '<?cs var:skin('images/pop-out.gif') ?>';
var LOGCSrowPopoutButtonImageHover = '<?cs var:skin('images/pop-out-hover.gif') ?>';


<?cs # Clearsilver definitions ?>

<?cs def:add_channel(name,value) ?>
   <option value="<?cs var:html_escape(value) ?>"><?cs var:html_escape(name) ?></option>
<?cs /def ?>

<?cs def:imageTipWithOffset(Text, Image, Offset) ?>
    onMouseOver="TOOLtooltipLink('<?cs var:Text ?>', undefined,
            <?cs if: Image == 'this' ?> this
            <?cs else ?> document.getElementById('<?cs var:Image ?>')
            <?cs /if ?>,
            { Left: <?cs var:#Offset - 15 ?> })"
    onMouseOut="TOOLtooltipClose()"
    onMouseUp="TOOLtooltipClose()"
<?cs /def ?>
<?cs def:imageTip(Text) ?>
    <?cs call:imageTipWithOffset(Text, 'this', 0) ?>
<?cs /def ?>

<?cs set:     view_tip = 'Change the view mode for this entry.' ?>
<?cs set:     wrap_tip = 'Toggle line-wrapping.' ?>
<?cs set:wrap_tip_grey = 'You cannot wrap text in this view mode.' ?>
<?cs set:      hex_tip = 'View hex-dump of this entry.' ?>
<?cs set:     text_tip = 'View plain-text for this entry.' ?>
<?cs set:parsedSgm_tip = 'View segments and composites generated by parse.' ?>
<?cs set:parsedSgG_tip = 'View segment grammar populated with parse results.' ?>
<?cs set:   parsed_tip = 'View tables generated by complete parse. (Plain-text)' ?>
<?cs set:parsedTbl_tip = 'View tables generated by complete parse. (Graphical)' ?>
<?cs set:parsedSQL_tip = 'View SQL statements generated by complete parse.' ?>
<?cs set: resubmit_tip = 'Resubmit or forward this message.' ?>
<?cs set: resubmit_tip_nonadmin = 'Only administrators can resubmit or forward messages.' ?>
<?cs set:  related_tip = 'Show all entries related to this.' ?>
<?cs set:to_mapper_tip = 'Use this message in the Translator editor.' ?>
<?cs set:     link_tip = 'Show me a direct link to this entry.' ?>
<?cs set:download_message_tip = 'Download this message to a file.' ?>
<?cs set:   delete_tip = 'Delete this entry.' ?>
<?cs set:   delete_tip_nonadmin = 'Only administrators can delete messages.' ?>
<?cs set: mark_msg_tip_nonadmin = 'Only administrators can mark/unmark messages.' ?>
<?cs set: mark_msg_tip = 'Mark/Unmark this entry.' ?>
<?cs set: undelete_tip = 'Undelete this entry.' ?>
<?cs set: undelete_tip_nonadmin = 'Only administrators can undelete messages.' ?>

<?cs set: preiewView_tip = 'Change the preview mode for this message.' ?>
<?cs set: hexPreview_tip = 'Preview hex-dump of this message.' ?>
<?cs set: textPreview_tip = 'Preview plain-text for this message.' ?>
<?cs set: parsedPreviewSgm_tip = 'Preview segments and composites generated by parse.' ?>
<?cs set: parsedPreviewSgG_tip = 'Preview segment grammar populated with parse results.' ?>
<?cs set: parsedPreview_tip = 'Preview tables generated by complete parse. (Plain-text)' ?>
<?cs set: parsedPreviewTbl_tip = 'Preview tables generated by complete parse. (Graphical)' ?>
<?cs set: parsedPreviewSQL_tip = 'Preview SQL statements generated by complete parse.' ?>
<?cs set: pluginPreview_tip = 'View the plugin resubmission preview.' ?>
<?cs set: mapperPreview_tip = 'Preview the output generated by the translator.' ?>

<?cs def:imageTipCanAdmin(admin,non_admin) ?>
    <?cs if:CurrentUserCanAdmin ?>
        <?cs call:imageTip(admin) ?>
    <?cs else ?>
        <?cs call:imageTip(non_admin) ?>
    <?cs /if ?>
<?cs /def ?>

<?cs def:imgSrcCanAdmin(Src) ?>
    <?cs if:CurrentUserCanAdmin ?>
        <?cs set:imgSrcCanAdminSrcName = Src + '.gif' ?>
    <?cs else ?>
        <?cs set:imgSrcCanAdminSrcName = Src + '-grey.gif' ?>
    <?cs /if ?>
    <?cs var:skin(imgSrcCanAdminSrcName) ?>
<?cs /def ?>

<?cs def:defineHelpTooltipDiv() ?>
   <div id="helpTooltipDiv" class="helpTooltip" style="zoom:1">
      <b id="helpTooltipTitle" onClick="$('#helpTooltipDiv').fadeOut('slow');"></b>
      <em id="helpTooltipBody"></em>
      <input type="hidden" id="helpTooltipId" value="0" />
   </div>
<?cs /def ?>
