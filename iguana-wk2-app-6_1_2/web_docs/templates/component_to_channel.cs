<?cs # vim: set syntax=html :?>

<?cs if:!CountOfAllDestAvailableChannel ?>
   <tr>
      <td style="text-align: center;" colspan="4">Configure another channel using a 'From Channel' source to use as a destination for this channel.</td>
   </tr>
<?cs else ?>
   <tr id="destination_channels">
      <td class="left_column" style="vertical-align: top; padding-top:15px;">Destination channel(s)</td>
      <td>
         <table class="source_dest_list">
               <tr>
	          <?cs if:!DestinationChannels.0.Id ?>
                     <?cs if:CountOfAllDestinationChannel ?>
                        <td <?cs if:!Channel.ReadOnlyMode ?>colspan="4"<?cs /if ?> >
			<span style="white-space: nowrap; vertical-align: top; padding-right: 2px;">
                        <img src="/<?cs var:skin("images/icon_warning.gif") ?>" style="position:relative; top:7px;"/>
                        <span style="font-weight:bold; position:relative; top:4px;">Warning:</span>
                        <span style="position:relative; top:4px">you do not have view permissions for any configured destination channels</span></td>
                        </span>
			</td>
   		        <?cs if:!Channel.ReadOnlyMode ?></tr><tr><?cs /if ?>
		     <?cs elif:Channel.ReadOnlyMode ?>
			<td>None</td>
                     <?cs /if ?>
		  <?cs /if ?>

                  <?cs if:DestinationChannels.0.Id || !Channel.ReadOnlyMode?>
                  <th class="source_dest_list" width="40%"><u>Name</u></th>
                  <th class="source_dest_list" width="30%"><u>Current Position</u><br/> <small>(YYYY/MM/DD HH:MM)</small></th>
                  <th class="source_dest_list"><u>Queued</u>
                  <a id="toChannelQueuedHelpIcon" class="helpIcon" tabindex="100"
                        rel="Click a link below to reposition a destination in the log browser."
                        title="Queued Messages" href="#" onclick="initializeHelp(this,event);">
                        <img src="/<?cs var:skin("images/help_icon.gif") ?>" style="margin-top:1px" border="0" />
                  </a>
                  </th>
                  <?cs if:!Channel.ReadOnlyMode ?>
                     <th class="source_dest_list"><u>Remove</u></th>
                  <?cs /if ?>
		  <?cs /if ?>
               </tr>
               <?cs each:Dest = DestAvailableChannels ?>
                  <tr id="to_channel_<?cs var:Dest.Id ?>">
                     <td style="width:40%">
		     <a href="/channel#Channel=<?cs var:url_escape(Dest.Name) ?>&Tab=source"><?cs var:html_escape(Dest.Name) ?></a>
                        <input type="hidden" value="true" name="use_to_channel_<?cs var:Dest.Id ?>"
                               id="use_to_channel_<?cs var:Dest.Id ?>" />
                     </td>
                     <td style="width:30%"><nobr>
                           <span id="queue_date_<?cs var:Dest.Id ?>">----/--/--</span> @
                           <span id="queue_time_<?cs var:Dest.Id ?>">--:--</span>
                     </nobr></td>
		     <td>
		        <a target="_blank" id="log_view_to_channel_<?cs var:Dest.Id ?>">
		        <span id="queue_remaining_<?cs var:Dest.Id ?>">--</span>
			</a>
		     </td>
                     <?cs if:!Channel.ReadOnlyMode ?>
                     <td>
                           <input class="action-button-small blue" type="button" value="-" 
                                  id="rm_to_channel_<?cs var:Source.Id ?>" 
                                  name="rm_to_channel_<?cs var:Source.Id ?>" 
                                  onclick="TOCHremoveChannel('<?cs var:Dest.Id ?>');" />
                     </td>
                     <?cs /if ?>
                  </tr>
               <?cs /each ?>
         </table>
      </td> 
   </tr>

   <tr class="selected" <?cs if:Channel.ReadOnlyMode ?>style="display:none;"<?cs /if ?>>
      <td class="left_column" id="AddDestChannel">
         Add a destination channel
      </td>
      <td class="inner_left">
         <select name="DstSelectDestChannel" id="DstSelectDestChannel" style="display: none">
            <option value=""></option>
            <!-- filled by the javascript -->
         </select>
         <span id="AddDestChannelEmpty">No channels remaining</span>
         <a id="DstDestinationSelectHelp" class="helpIcon" tabindex="100"
             rel="Only channels with a 'From Channel' source can be used."
             title="Select Destinations" href="#" onclick="initializeHelp(this,event);">
            <img src="/<?cs var:skin("images/help_icon.gif") ?>" style="margin-top:1px" border="0" />
         </a>
      </td>
   </tr>

   <script type="text/javascript" src="<?cs var:iguana_version_js("/from_channel.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/to_channel.js") ?>"></script>
   <script type="text/javascript">
      var DestChannelIdToName =
      {
         <?cs each:Channel = DestAvailableChannels ?>
            '<?cs var:Channel.Id ?>' : '<?cs var:js_escape(Channel.Name) ?>'<?cs if:name(Channel) != CountOfDestAvailableChannel-1 ?>,<?cs /if ?>//<?cs var:name(Channel) ?>
         <?cs /each ?>
      };
      var InitialDestinationChannels = 
      [
         <?cs each:Dest = DestinationChannels ?>
            '<?cs var:Dest.Id ?>'<?cs if:name(Dest) != CountOfDestinationChannel-1 ?>,<?cs /if ?>//<?cs var:name(Dest) ?>
         <?cs /each ?>
      ];

      $(document).ready(function() {
      var DestNotRemovedBecauseRunning = 
      [
         <?cs each:Dest = DestNotRemovedBecauseRunning ?>
            '<?cs var:Dest.Name ?>'<?cs if:name(Dest) != CountDestNotRemovedBecauseRunning-1 ?>,<?cs /if ?>//<?cs var:name(Dest) ?>
         <?cs /each ?>
      ];

      var DestNotAddedBecauseRunning = 
      [
         <?cs each:Dest = DestNotAddedBecauseRunning ?>
            '<?cs var:Dest.Name ?>'<?cs if:name(Dest) != CountDestNotAddedBecauseRunning-1 ?>,<?cs /if ?>//<?cs var:name(Dest) ?>
         <?cs /each ?>
      ];

      TOCHinitialize('<?cs var:js_escape(Channel.Name) ?>');
      TOCHfetchDestUpdates('<?cs var:js_escape(Channel.Name) ?>', '<?cs var: Channel.Guid ?>');

      $("#to-channel-alert-dialog").dialog(
      {
         bgiframe: true,
         autoOpen: false,
         height: 100,
         width:350,
         modal: true,
         buttons:{
            'Close': function() {
            $(this).dialog('close');
	    }
         }
      });

      var Message = '';
      if (DestNotRemovedBecauseRunning.length)
      {
         Message = 'Some destinations could not be removed because they are running:<br/><br/>';
	 for (Dest in DestNotRemovedBecauseRunning)
	 {
	    Message += DestNotRemovedBecauseRunning[Dest] + '<br/>';
	 }
      }
      if (DestNotAddedBecauseRunning.length)
      {
         (Message ? Message += '<br/>' : '');
         Message += 'Some destinations could not added because they are running:<br/><br/>';
	 for (Dest in DestNotAddedBecauseRunning)
	 {
	    Message += DestNotAddedBecauseRunning[Dest] + '<br/>';
	 }
      }
      if (Message)
      {
         $('#to-channel-alert-dialog').dialog('open');   
         $('#to-channel-alert-dialog-text').html(Message);
      }
      });
   </script>

   <div id="to-channel-alert-dialog" title="Alert" style="display:none;">
      <div id="to-channel-alert-dialog-text"></div>
   </div>

<?cs /if ?>
