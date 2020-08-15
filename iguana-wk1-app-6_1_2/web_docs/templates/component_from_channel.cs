<?cs # vim: set syntax=html sts=3 et :?>

<?cs if:CountOfAllSourceChannel ?>
   <?cs set:NoSourceErrMessage = "you do not have view permissions for any configured source channels" ?>
<?cs else ?>
   <?cs set:NoSourceErrMessage = "with no source channel selected, this channel will not start." ?>
<?cs /if ?>

<?cs if:!CountOfAllSourceAvailableChannels ?>
   <tr>
      <td colspan="4">
        <center>Configure another channel to use as a source for this channel.</center>
      </td>
   </tr>
<?cs else ?>
   <tr id="source_channels">
      <td class="left_column" style="vertical-align: top; padding-top: 15px">Source channel(s)</td>
      <td class="inner_left">
         <table class="source_dest_list">
               <tr>
	          <?cs if:!SourceChannels.0.Id ?>
                     <td <?cs if:!Channel.ReadOnlyMode ?>colspan="4"<?cs /if ?> >
                        <span style="white-space: nowrap; vertical-align: top; padding-right: 2px;">
                        <img src="/<?cs var:skin("images/icon_warning.gif") ?>" style="position:relative; top:7px;"/>
                        <span style="font-weight:bold; position:relative; top:4px;">Warning:</span>
                        <span style="position:relative; top:4px"><?cs var:NoSourceErrMessage ?></span></td>
                        </span>
                     </td>
		     <?cs if:!Channel.ReadOnlyMode ?></tr><tr><?cs /if ?>
		  <?cs /if ?>

                  <?cs if:SourceChannels.0.Id || !Channel.ReadOnlyMode ?>
                     <th class="source_dest_list" style="width:40%;"><u>Name</u></th>
                     <th class="source_dest_list" style="width:35%;"><u>Current Position</u><br/> <small>(YYYY/MM/DD HH:MM)</small></th>
                     <th ><u>Queued</u>
                     <a id="fromChannelQueuedHelpIcon" class="helpIcon" tabindex="100"
                        rel="Click a link below to reposition a source in the log browser."
                        title="Queued Messages" href="#" onclick="initializeHelp(this,event);">
                        <img src="/<?cs var:skin("images/help_icon.gif") ?>" style="margin-top:1px" border="0" />
                     </a>
                     </th>
		     <?cs if:!Channel.ReadOnlyMode ?>
                     <th class="source_dest_list" ><u>Remove</u></th>
		     <?cs /if ?>
                  <?cs /if ?>
               </tr>
               <?cs each:Source = SourceAvailableChannels ?>
                  <tr id="from_channel_<?cs var:Source.Id ?>">
                     <td style="width;40%">
                        <a href="/channel#Channel=<?cs var:url_escape(Source.Name) ?>&Tab=destination"><?cs var:html_escape(Source.Name) ?></a>
                        <input type="hidden" value="true" name="use_channel_<?cs var:Source.Id ?>"
                               id="use_channel_<?cs var:Source.Id ?>" />
                     </td>
                     <td style="width;30%"><nobr>
                           <span id="queue_date_<?cs var:Source.Id ?>">----/--/--</span> @
                           <span id="queue_time_<?cs var:Source.Id ?>">--:--</span>
                     </nobr></td>
		     <td style="width;30%">
		        <a target="_blank" id="log_view_from_channel_<?cs var:Source.Id ?>">
		        <span id="queue_remaining_<?cs var:Source.Id ?>">--</span>
			</a>
		     </td>
                     <?cs if:!Channel.ReadOnlyMode ?>
                     <td style="width;30%">
                           <input class="action-button-small blue" type="button" value="-" 
                                  <?cs if:PendingDequeueListUpdate != 0 ?>disabled<?cs /if ?>
                                  id="rm_from_channel_<?cs var:Source.Id ?>" 
                                  name="rm_from_channel_<?cs var:Source.Id ?>" 
                                  onclick="FRMCHremoveChannel('<?cs var:Source.Id ?>');" />
                     </td>
                     <?cs /if ?>
                  </tr>
               <?cs /each ?>
         </table>
      </td>

   </tr>
   
   <tr class="selected" <?cs if:Channel.ReadOnlyMode ?>style="display:none;"<?cs /if ?>>
      <td class="left_column" id="AddSourceChannel">
         Add a source channel
      </td>
      <td class="inner_left">
         <table>
            <tr><td>
               <select name="SrcSelectSourceChannel" id="SrcSelectSourceChannel" style="display:none;"
                       <?cs if:PendingDequeueListUpdate != 0 ?>disabled<?cs /if ?> >
                  <option value=""></option>
                  <!-- filled by the javascript -->
               </select>
               <span id="AddSourceChannelEmpty">No channels remaining</span>
            </td></tr>
         </table>
      </td>
   </tr>

   <script type="text/javascript">
      var SourceChannelIdToName =
      {
         <?cs each:Channel = SourceAvailableChannels ?>
            '<?cs var:Channel.Id ?>' : '<?cs var:js_escape(Channel.Name) ?>'<?cs if:name(Channel) != CountOfSourceAvailableChannel-1 ?>,<?cs /if ?>//<?cs var:name(Channel) ?>
         <?cs /each ?>
      };
      var InitialSourceChannels = 
      [
         <?cs each:SourceChannel = SourceChannels ?>
            '<?cs var:SourceChannel.Id ?>'<?cs if:name(SourceChannel) != CountOfSourceChannel-1 ?>,<?cs /if ?>//<?cs var:name(SourceChannel) ?>
         <?cs /each ?>
      ];
      $(document).ready(function() {
      FRMCHinitialize('<?cs var:js_escape(Channel.Name) ?>');
      FRMCHfetchSourceUpdates('<?cs var:js_escape(Channel.Name) ?>', '<?cs var: Channel.Guid ?>');
      });
   </script>
<?cs /if ?>
