<?cs # vim: set syntax=html :?>

<?cs include: "mapper_config.cs" ?>

<?cs with: source = Channel.Source ?>
   <tr class="selected" id="SrcPollTimeRow">
      <td class="left_column">
         Poll time<font color="#ff0000">*</font>
      </td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?> <?cs var:source.PollTime ?> <?cs else
         ?>
         <input type="text" class="number_field" name="SrcPollTime" id="SrcPollTimeInput"
            value="<?cs var:source.PollTime ?>">

         <script defer type="text/javascript">
            VALregisterIntegerValidationFunction('SrcPollTimeInput', 'SrcPollTimeRow', 'SrcPollTimeErrorMessageContainer', null, showSourceTab, 1000);
         </script>

         <?cs /if ?> milliseconds <span id="SrcPollTimeErrorMessageContainer" class="validation_error_message_container">
         </span><?cs if:Source.Error.PollTime ?>
         <div class="configuration_error">
            <?cs var:Source.Error.PollTime ?>
         </div>
         <?cs /if ?>
      </td>
   </tr>

   <?cs call:renderMapperForm(source, !Channel.IsNew, 'Src', 'Source', source.Type, '', 0, 1) ?>
   
<?cs /with ?>
