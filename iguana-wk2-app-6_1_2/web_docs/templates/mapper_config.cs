<?cs # mapper - the component.
       isInitialized - is the mapper initialized?
       mapperPrefix - the prefix to use for form input names.
       componentType - the component type (eg, "Destination").
       componentName - the component name (eg, "To Translator").
       rowClassName - the class name to give to all rows. ?>
<?cs def:renderMapperForm(mapper, isInitialized, mapperPrefix, componentType, componentName, rowClassName, firstRow, canEdit) ?>

<script type="text/javascript">
$(document).ready(function() {
   <?cs if: ! Channel.ReadOnlyMode && ! IsRunning && ! Channel.IsEncrypted ?>
      DMPRloadHistory('<?cs var:mapper.Guid ?>', '<?cs var:mapperPrefix ?>', '<?cs var:mapper.CommitId ?>');
   <?cs /if ?>
});
</script>
   <?cs if:!isInitialized && !Channel.IsEncrypted ?>
      <tr class="<?cs var:rowClassName ?> selected" >
         <td class="left_column<?cs if:firstRow ?> first_row<?cs /if ?>"></td>
         <td class="inner_left<?cs if:firstRow ?> first_row<?cs /if ?>" colspan="3">
            <span class="configurationFillerText">Save the channel to begin editing the script.</span>
         </td>
      </tr>
   <?cs elif:#isInitialized?>
      <tr class="<?cs var:rowClassName ?> selected">
         <td class="left_column<?cs if:firstRow ?> first_row<?cs /if ?>">Commit</td>
         <td class="inner_left<?cs if:firstRow ?> first_row<?cs /if ?>" colspan="3">
            <?cs if:Channel.ReadOnlyMode || mapper.IsMapperEncrypted ?>
               <span id="MsDisp_<?cs var:mapper.Guid ?>">
                  <?cs if:IsRunning ?>
                     <?cs var:html_escape(mapper.CurrentRunningCommitId) ?>
                     <br />[<?cs var:html_escape(mapper.CommitComment) ?>]
                  <?cs else ?>
                     <?cs if:"No milestone defined." != html_escape(mapper.CommitId) ?>
                        <?cs var:html_escape(mapper.CommitId) ?>
                        <br /><?cs var:html_escape(mapper.CommitComment) ?>
                     <?cs else ?>
                        <?cs # mapper.CommitId == "No milestone defined." ?>
                        No commit is selected.
                     <?cs /if ?>
                  <?cs /if ?>
               </span>
               <br /><br />Will use the selected commit on channel start.
               <?cs if:mapper.Error.Commit ?>
                  <br /><br />
                  <div class="configuration_error_<?cs var:mapper.Guid ?>">
                     <span style="color: red">No commit is selected.  The channel cannot be run.</span>
                     <a class="helpIcon"  tabindex="100" rel="To resolve this problem you need to select a valid commmit.  <a href='<?cs var:help_link('iguana_milestones') ?>' target='_blank'>More information</a> can be found in the online manual." title="Milestone Problem">
                        <img src="/images/help_icon.gif?ver_hash=F299927B" border="0" />
                     </a>
                  </div>
               <?cs else ?>
                  <?cs # ! mapper.Error.Commit ?>
                  <?cs if:mapper.Warning.Commit ?>
                     <br /><br />
                     <img src="/<?cs var:skin("images/icon_warning.gif") ?>" class="icon_warning"> The translator project has uncommitted changes. <a class="helpIcon" tabindex="100" rel="There are changes to the script which have not been committed yet.  <a href='<?cs var:help_link('iguana_milestones') ?>' target='_blank'>More information</a> can be found in the online manual." title="Uncommited Changes" href="#"><img src="/images/help_icon.gif?ver_hash=F299927B" border="0"></a>
                  <?cs /if ?>
               <?cs /if ?>

               <?cs # also generate hidden form elements to pass on commit info when the translator is encrypted ?>
               <?cs if:!Channel.ReadOnlyMode && (mapper.AckStyle != "vmd" || mapper.AckStyle != "fast") ?>
                  <input type="hidden" name="<?cs var:mapperPrefix ?>CommitId" id="<?cs var:mapperPrefix?>CommitId" value="<?cs var:html_escape(mapper.CommitId) ?>" />
                  <input type="hidden" name="<?cs var:mapperPrefix ?>CommitComment" id="<?cs var:mapperPrefix?>CommitComment" value="<?cs var:html_escape(mapper.CommitComment) ?>" />
               <?cs /if ?>
            <?cs else ?>
               <?cs # ! Channel.ReadOnlyMode ?>
               <input type="hidden" name="<?cs var:mapperPrefix ?>CommitComment" id="<?cs var:mapperPrefix ?>CommitComment" value="" />
               <div id="<?cs var:mapperPrefix ?>CommitChoices" class="CommitChoices" />
            <?cs /if ?>
         </td>
      </tr>

      <tr class="<?cs var:rowClassName ?> selected">
         <td class="left_column">Script</td>
         <td class="inner_left" colspan="3">
            <?cs if:mapper.IsMapperEncrypted ?>
               Script is encrypted
            <?cs elif:CurrentUserCanEditScripts ?>
               <?cs # ! mapper.IsMapperEncrypted ?>
               <?cs if:canEdit ?>
                  <a class="edit_permission_required" href="/mapper/?Page=OpenEditor&MapperGuid=<?cs var:mapper.Guid ?>">Edit Script...</a>
               <?cs else ?>
                  <?cs # ! canEdit ?>
                  <div class="configuration_error">Before you edit the script you will need to correct this error.</div>
               <?cs /if ?>
            <?cs else ?>
               <?cs # ! mapper.IsMapperEncrypted && ! CurrentUserCanEditScripts ?>
               You do not have the necessary permissions to edit the script.
            <?cs /if ?>
         </td>
      </tr>

   <?cs /if ?>

<?cs /def ?>
