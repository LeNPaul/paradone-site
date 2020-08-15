// vim: set syntax=javascript et ts=3 sts=3 sw=3:

<?cs def:type_icon(entry, icon_id) ?>
   <?cs with: type = entry.Type ?>
      <?cs if: entry.IsResubmitted ?>
         <?cs if: type == 'Message' && !entry.IsResubmission ?>
            <?cs set: image = "msg-resubmitted.gif" ?>
            <?cs set: tip   = "This message has been resubmitted." ?>
         <?cs elif: type == 'Resubmission' || entry.IsResubmission ?>
            <?cs set: image = "msg-resubmitted-resubmission.gif" ?>
            <?cs set: tip   = "This message was created by resubmitting another message, " +
                              "and has itself been resubmitted.  Message resubmissions are " +
                              "processed immediately, except for channels whose destination " +
                              "component is To Channel." ?>
         <?cs elif: type == 'Ignored Data Msg' ?>
            <?cs set: image = "msg-resubmitted-unqueued.gif" ?>
            <?cs set: tip   = "This message is unqueued and has been resubmitted.  " +
                              "Unqueued messages are not processed by channels." ?>
         <?cs else ?>
            <?cs set: image = "msg-unknown.gif" ?>
            <?cs set: tip   = "The type of this entry is unknown." ?>
         <?cs /if ?>
      <?cs else ?>
         <?cs if: type == 'Message' && !entry.IsResubmission ?>
            <?cs set: image = "msg-message.gif" ?>
            <?cs set: tip   = "This is a message." ?>
         <?cs elif: type == 'Resubmission' || entry.IsResubmission ?>
            <?cs set: image = "msg-resubmission.gif" ?>
            <?cs set: tip   = "This message was created by resubmitting another message.  " +
                              "Message resubmissions are processed immediately, except " +
                              "for channels whose destination component is To Channel." ?>
         <?cs elif: type == 'Ignored Data Msg' ?>
            <?cs set: image = "msg-unqueued.gif" ?>
            <?cs set: tip   = "This is an unqueued message.  Unqueued messages are not " +
                              "processed by channels." ?>
         <?cs elif: type == 'Ack Msg' ?>
            <?cs set: image = "msg-ack.gif" ?>
            <?cs set: tip   = "This is an ACK message, as sent or received over LLP." ?>
         <?cs elif: type == 'Success' ?>
            <?cs set: image = "msg-success.gif" ?>
            <?cs set: tip   = "This indicates that a message was successfully processed." ?>
         <?cs elif: type == 'Warning' ?>
            <?cs set: image = "msg-warning.gif" ?>
            <?cs set: tip   = "This is a non-fatal error.  For messages, a warning indicates " +
                              "a problem that may be overcome.  E.g., Iguana will retry " +
                              "connecting to a database a few times (as configured)." ?>
         <?cs elif: type == 'Info' ?>
            <?cs set: image = "msg-info.gif" ?>
            <?cs set: tip   = "This is an informational entry." ?>
         <?cs elif: type == 'Debug' ?>
            <?cs set: image = "msg-debug.gif" ?>
            <?cs set: tip   = "This is a debug entry." ?>
         <?cs elif: type == 'Audit Message' ?>
            <?cs set: image = "msg-audit.png" ?>
            <?cs set: tip   = "This is an audit entry." ?>
         <?cs elif: type == 'Error' ?>
            <?cs if: entry.Acknowledged ?>  <?cs set: image = "msg-failure-ack.gif" ?>
            <?cs else ?>                    <?cs set: image = "msg-failure.gif" ?>
            <?cs /if ?>
            <?cs set: tip = "This is an error.  For messages, an error indicates an " +
                            "unrecoverable problem that will cause the channel to stop " +
                            "or the message to be skipped, depending on how the channel " +
                            "is configured." ?>
         <?cs else ?>
            <?cs set: image = "msg-unknown.gif" ?>
            <?cs set: tip   = "The type of this entry is unknown." ?>
         <?cs /if ?>
      <?cs /if ?>
   <?cs /with ?>
   <?cs if: icon_id ?>
      <?cs set: outer_id = 'id="' + icon_id + '"' ?>
      <?cs set: inner_id = 'id="delete-overlay-' + icon_id + '"' ?>
   <?cs else ?>
      <?cs set: outer_id = '' ?>
      <?cs set: inner_id = '' ?>
   <?cs /if ?>
   <?cs set: background = "background-image:url(\'" + skin("images/" + image) + "\');" ?>
   <?cs if: entry.Deleted ?>  <?cs set: inner_style = '' ?>
   <?cs else              ?>  <?cs set: inner_style = 'style="display:none;"' ?>
   <?cs /if ?>
   '<div class="log_type_icon" <?cs var:outer_id ?> style="<?cs var:background ?>" ' +
         'onMouseOver="TOOLtooltipLink(\'<?cs var:tip ?>\', undefined, this, { Left: -15 })" ' +
         'onMouseOut="TOOLtooltipClose()" ' +
         'onMouseUp="TOOLtooltipClose()">' +
      '<div class="delete_overlay" <?cs var:inner_id ?> <?cs var:inner_style ?> >' +
         '<img src="<?cs var:skin("images/deleted_overlay.gif") ?>" />' +
   '</div></div>'
<?cs /def ?>

<?cs def:source(entry, use_link) ?>
   <?cs if: entry.ChannelName ?>
      <?cs if: use_link && entry.Existing ?>
         '<a href="/channel#Channel=' +
            '<?cs var:js_escape(url_escape(entry.ChannelName)) ?>">' +
            '<?cs var:js_escape(html_escape(entry.ChannelName)) ?></a>'
      <?cs else ?>
         '<?cs var:js_escape(html_escape(entry.ChannelName)) ?>'
      <?cs /if ?>
   <?cs elif: entry.QueueID == ServiceLogGuid ?>
      '<i>Iguana Service</i>'
   <?cs elif: entry.QueueID == ZeroGuid ?>
      '<i>Unknown Source</i>'
   <?cs else ?>
      'ID <?cs var:js_escape(html_escape(entry.QueueID)) ?>'
   <?cs /if ?>
   <?cs if: !entry.Existing ?>
      + ' (deleted)'
   <?cs /if ?>
<?cs /def ?>

<?cs def:output_entry_json(entry) ?>
   {
      Position:      '<?cs var:js_escape(entry.Position) ?>',
      Date:          '<?cs var:js_escape(entry.Date) ?>',
      Size:          <?cs var:js_escape(entry.Size) ?>,
      FormattedDate: '<?cs var:js_escape(entry.FormattedDate) ?>',
      RefMsgId:      '<?cs var:js_escape(entry.RefMsgId) ?>',
      Type:          '<?cs var:js_escape(entry.Type) ?>',
      Channel:       '<?cs var:js_escape(entry.ChannelName) ?>',
      MessageId:     '<?cs var:js_escape(entry.MessageId) ?>',
      Acknowledged:  Boolean(<?cs var:#entry.Acknowledged ?>),
      Deleted:       Boolean(<?cs var:#entry.Deleted ?>),
      Link:          'log_browse?refid=<?cs var:js_escape(url_escape(entry.MessageId)) ?>',
      CanParse:      Boolean(<?cs var:#entry.CanParse ?>),
      IsResubmittable: Boolean(<?cs var:#entry.IsResubmittable ?>),
      Description:
         '<table style="border-collapse:collapse">' +
            '<tr>' +
               '<td class="clsDescriptionIconCell" rowspan="1" style="padding:2px 6px 2px 0px">' +
                  <?cs call:type_icon(entry,"") ?> +
               '</td>' +
               '<td align="left" style="padding:0px 0px 0px 0px;">' +
                  '<div style="line-height:16px; white-space:nowrap">' +
                  '<span style="font-weight:bold; font-size:13px;">' + <?cs call:source(entry, 1) ?> + '</span>' +
                  '<span style="font-size:9px; color:#666666; letter-spacing:-0.1em;">&nbsp; (<?cs var:js_escape(html_escape(entry.FormattedSize)) ?>' + '&nbsp;B)</span><br>' +
                  '<span class="clsExpandedEntryDate" style="white-space:nowrap; font-size:10px; letter-spacing:0em;">' +
                     <?cs   if: entry.Date == Today ?>     'Today' +
                     <?cs elif: entry.Date == Yesterday ?> 'Yesterday' +
                     <?cs else ?>
                        '<?cs var:js_escape(html_escape(entry.FormattedDate)) ?>' +
                     <?cs /if ?>
                  '</span>' +
                  '&nbsp;' +
                  '<span style="font-size:9px; letter-spacing:-0.05em;"><?cs var:js_escape(html_escape(entry.Time)) ?></span>' +
                  '</div>' +
               '</td>' +
               '<td class="clsPendingMarkerCell"></td>' +
            '</tr>' +
         '</table>',
      FirstCell:
         '<table id="first-cell-<?cs var:js_escape(html_escape(entry.MessageId)) ?>" class="<?cs if:entry.Deleted!='0' ?>preview_deleted<?cs else ?>preview<?cs /if ?>" >' +
            '<tr>' +
               '<td class="entryType" rowspan="2">' +
                  <?cs call:type_icon(entry, "type-icon-"+entry.MessageId) ?> +
               '</td>' +
               '<td class="entryDate" id="entryDate-<?cs var:js_escape(html_escape(entry.MessageId)) ?>">' +
                  <?cs   if: entry.Date == Today ?>     'Today' +
                  <?cs elif: entry.Date == Yesterday ?> 'Yesterday' +
                  <?cs else ?>
                     '<?cs var:js_escape(html_escape(entry.Date)) ?>' +
                  <?cs /if ?>
               '</td>' +
            '</tr>' +
            '<tr>' +
               '<td class="entryTime">' +
                  '<?cs var:js_escape(html_escape(entry.Time)) ?>' +
               '</td>' +
            '</tr>' +
            '<tr><td class="entryPendingMarker" colspan="2"><span id="pending-marker-<?cs var:js_escape(html_escape(entry.MessageId)) ?>"></span></td></tr>' +
         '</table>',
      SecondCell:
            '<table id="second-cell-<?cs var:js_escape(html_escape(entry.MessageId)) ?>" class="<?cs if:entry.Deleted!='0' ?>preview_deleted<?cs else ?>preview<?cs /if ?>" >' +
            '<tr><td class="entrySource" >' +
               <?cs call:source(entry, 0) ?> +
            '</td></tr>' +
            '<tr><td class="entryPreview">' +
               '<pre style="margin:0; overflow:hidden"><?cs var:js_escape(entry.Preview) ?></pre>' +
            '</td></tr>' +
            '</table>'
   }
<?cs /def ?>

<?cs if:SingleEntryJson ?>
   ({CurrentEntry:<?cs call:output_entry_json(Entries.0) ?>})
<?cs else ?>

// vim: set syntax=javascript et ts=3 sts=3 sw=3:

<?cs each: entry = Entries ?>
   <?cs set: count_of_entry = #count_of_entry + 1 ?>

   <?cs if: Direction == 'forward' ?>
      addRowForward(
   <?cs else ?>
      addRowReverse(
   <?cs /if ?>
   <?cs call:output_entry_json(entry) ?>
   );
<?cs /each ?>

<?cs if:Service.PanicErrorString ?>
   document.getElementById('panicFailureText').innerHTML = '<?cs var:js_escape(Service.PanicErrorString) ?>';
   document.getElementById('panicFailure').style.display = '';
   document.getElementById('lowDiskSpaceMessage').style.display = 'none';
<?cs else ?>
   document.getElementById('panicFailure').style.display = 'none';
<?cs /if ?>

<?cs if: UpdatedRefMsgId ?>
   document.getElementsByName('RefMsgId')[0].value = '<?cs var:js_escape(UpdatedRefMsgId) ?>';
<?cs /if ?>

<?cs if: Direction == 'forward' ?>
   if( PollingForward )
   {
      updateNewMessageCount(<?cs var:#count_of_entry ?>);
   }
<?cs /if ?>

<?cs if: !subcount(QueryErrors) ?>
   document.getElementById('queryError').style.display = 'none';
<?cs else ?>
   document.getElementById('queryError').style.display = '';
   document.getElementById('queryErrorText').innerHTML = (''
      <?cs each: error = QueryErrors ?>
         + '<?cs var:js_escape(html_escape(error.ErrorText)) ?> in '
         + '<span style="color:red">/'
         + '<?cs var:js_escape(html_escape(error.LeadContext)) ?>'
         + '<?cs var:js_escape(html_escape(error.ProblemArea)) ?>/</span>.<br/>'
      <?cs /each ?>
   );
<?cs /if ?>
   
<?cs if:FirstPosition ?>
   <?cs if: Direction == 'forward' ?>
      <?cs set:other_direction = 'reverse' ?>
   <?cs else ?>
      <?cs set:other_direction = 'forward' ?>
   <?cs /if ?>

   if( !searchState('<?cs var:js_escape(other_direction) ?>').Position )
   {
      setSearchState('<?cs var:js_escape(other_direction) ?>', {
         Position: <?cs alt:FirstPosition ?>0<?cs /alt ?>,
         Date:     '<?cs var:js_escape(FirstDate) ?>'
      });
   }
<?cs /if ?>

<?cs if:LastPosition ?>
   setSearchState('<?cs var:js_escape(Direction) ?>', {
      Position: <?cs alt:LastPosition ?>0<?cs /alt ?>,
      Date:     '<?cs var:js_escape(LastDate) ?>'
   });
<?cs /if ?>

<?cs if: ErrorMessage ?>
   setFinished('<?cs var:js_escape(Direction) ?>', true,
      '<span style="color:red">' +
         'Search error: <?cs var:js_escape(html_escape(ErrorMessage)) ?>' +
      '</span>');
<?cs elif: Finished ?>
   setFinished('<?cs var:js_escape(Direction) ?>', true);
<?cs else ?>
   setFinished('<?cs var:js_escape(Direction) ?>', false,
      'Searching for more... <?cs var:js_escape(html_escape(Progress)) ?>');
<?cs /if ?>

<?cs if: !ErrorMessage ?>
   startPollingForward();
<?cs /if ?>

<?cs if: MissingQueueMeta ?>
   warnMissingQueueMeta();
<?cs /if ?>

   updateLogEntriesDequeuePosition();

Iterator.update();

<?cs /if ?>
