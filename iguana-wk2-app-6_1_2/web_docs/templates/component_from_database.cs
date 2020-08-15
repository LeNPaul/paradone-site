<?cs # vim: set syntax=html :?>

<?cs include:"browse_macro.cs" ?>

<script defer type="text/javascript"> <!--

function fromDBToggleNumberOfReconnectsTextField()
{
   var ReconnectOptionsComboBox = document.channel_data.SrcMaxReconnectsChoiceBox;
   var ReconnectsText = document.getElementById('SrcMaxReconnectsText');
   var ReconnectIntervalRow = document.getElementById('SrcReconnectIntervalRow');
   
   if(ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value == "other")
   {
      ReconnectsText.style.display="";
   }
   else
   {
      ReconnectsText.style.display="none";
   }
   if(ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value != "never")
   {
      ReconnectIntervalRow.style.display = "";
   }
   else
   {
      ReconnectIntervalRow.style.display = "none";
   }   
}

function toggleOrderOptions(checked)
{
   var options = getStyleObject('src_database_order_options');
   
   if( checked )
   {
      options.display = "inline";
   }
   else
   {
      options.display = "none";
   }
}

function onLoadFromDatabase()
{
   toggleOrderOptions(document.getElementById('SrcOrderByColumn').checked);
   showDatabaseNote('Src', document.channel_data.SrcDatabaseApi.value);
}

--> </script>

<?cs with: source = Channel.Source ?>
   <?cs def:database_settings(ComponentDatabase, ComponentError) ?>
      <?cs set:ComponentType = "Src" ?> 
      <?cs include: "component_database.cs" ?>
   <?cs /def ?>
   <?cs call:database_settings(source.Database, source.Error) ?>

   <script defer type="text/javascript">
      //Doesn't work in onLoadFromDatabase
      //Put it here, after the component_database.cs include.
      SrcupdateDatasourcePreview();
      SrcupdateDatauserPreview();
      setInterval("SrcupdateDatasourcePreview(); SrcupdateDatauserPreview();", 500);
   </script>

   <tr class="selected">
      <td class="left_column">Full Generator VMD path<font color="#ff0000">*</font></td>
        <td class="inner_left" colspan="3">
           <?cs if:Channel.ReadOnlyMode ?>
             <?cs if:source.Error.GeneratorVmdPath ?>
                <div class="configuration_error">
                   <?cs var:source.Error.GeneratorVmdPath ?>
                </div>
              <?cs else ?>
              <?cs if:source.GeneratorVmdPath != source.OriginalVmdPath ?>  
               <?cs /if ?>
              <?cs if:source.OriginalVmdPath != "" ?>
                  <label>Uploaded from: </label><br><?cs call:browse_readonly(source.OriginalVmdPath) ?>
              <?cs /if ?>
              <?cs if:source.GeneratorVmdPath != "" ?>
                 <br><br><?cs call:download_vmd(Channel.Guid, Channel.editTab, Channel.Name, "SrcGeneratorVmd") ?>
              <?cs /if ?>
            <?cs /if ?>
     <?cs else ?>
        <?cs if:!source.Error.GeneratorVmdPath && source.OriginalVmdPath != ""?>
            <div class="original_vmd">
                <label>Uploaded from: </label><br><?cs call:browse_readonly(source.OriginalVmdPath) ?>
            </div><br>
        <?cs /if ?>
            <?cs call:browse_input('SrcGeneratorVmdPath', source.GeneratorVmdPath) ?>
            <?cs if:!source.Error.GeneratorVmdPath && source.OriginalVmdPath != ""?>
            <br><a id="useOriginalFromDB"><u>Use previously uploaded VMD path</u></a>
            <script type="text/javascript">
               $("#useOriginalFromDB").click(function(){
                  $("#SrcGeneratorVmdPath").val("<?cs var:source.CleanedOriginal ?>");
               });
            </script> 
            <?cs /if ?>
             <?cs if:source.GeneratorVmdPath != "" ?>
                 <br><br><br><?cs call:download_vmd(Channel.Guid, Channel.editTab, Channel.Name, "SrcGeneratorVmd") ?>
                  <a id="DownloadVmdBox" style="padding-left: 4px;" class="helpIcon" tabindex="101" rel="Download the currently running VMD from source control." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
              <?cs /if ?> 
     <?cs /if ?>
        </td>
   </tr>

   <tr class="selected" id="TrackVmdChangesSrc" >
      <td class="left_column">Track VMD Changes</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?> 
                     <?cs if:source.KeepUpdated ?>Yes<?cs else ?>No<?cs /if ?>          
                  <?cs else ?>
                     <br><?cs call:keep_updated("UpdatedGeneratorSrcVmd", source.KeepUpdated) ?>      
                     <a id="KeepVmdUpdatedBox" class="helpIcon" tabindex="101" rel="When enabled, at startup, the channel will compare its current copy of the VMD with the version at the path it was uploaded from. If they are different, the 'uploaded from' version will be automatically copied into the run location and committed to source control before the channel begins processing messages." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                  <?cs /if ?>
               </td>
               <td class="inner_right">
                  <?cs if:Channel.Source.Error.KeepUpdated ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Source.Error.KeepUpdated ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>     
         </table>
      </td>
   </tr>   
   
   <tr class="selected" id="SrcPollTimeRow">
      <td class="left_column">Poll time<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.PollTime) ?>
     <?cs else ?>
       <input type="text" class="number_field " name="SrcPollTime" id="SrcPollTimeInput"
          value="<?cs var:source.PollTime ?>">
       <script defer type="text/javascript">
          VALregisterIntegerValidationFunction('SrcPollTimeInput', 'SrcPollTimeRow', 'SrcPollTimeErrorMessageContainer', null, showSourceTab, 1000);
        </script>
     <?cs /if ?> milliseconds
     <span id="SrcPollTimeErrorMessageContainer" class="validation_error_message_container"></span>
     <?cs if:source.Error.PollTime ?>
        <div class="configuration_error">
               <?cs var:source.Error.PollTime ?>      
        </div>
         <?cs /if ?>
      </td>
   </tr>
   

   <tr class="selected">
    
      <td class="left_column">Order messages by column</td>
      <td class="inner_left" colspan="3">
         <?cs if:source.OrderByColumn ?>
             <?cs if:Channel.ReadOnlyMode ?>
                <input type="hidden" name="SrcOrderByColumn" id="SrcOrderByColumn" onClick="toggleOrderOptions(checked)" />
                Yes<span class="configurationFillerText">, order by column</span> <?cs var:html_escape(source.OrderColumnName) ?>,
                <?cs if:source.OrderDescending ?>
                   Descending
                <?cs else ?>
                   Ascending
                <?cs /if ?>
             <?cs else ?>
                <input type="checkbox" class="no_style" name="SrcOrderByColumn" id="SrcOrderByColumn" onClick="toggleOrderOptions(checked)" checked="true" />
             <?cs /if ?>
         <?cs else ?>
             <?cs if:Channel.ReadOnlyMode ?>
                No<input type="hidden" name="SrcOrderByColumn" id="SrcOrderByColumn" onClick="toggleOrderOptions(checked)" />
	     <?cs else ?>
            <input type="checkbox" class="no_style" name="SrcOrderByColumn" id="SrcOrderByColumn" onClick="toggleOrderOptions(checked)" />
             <?cs /if ?>
         <?cs /if ?>
	 
         <div id="src_database_order_options" style="display:none">
            &nbsp;&nbsp;Column
             <input type="text" style="width: 100px" name="SrcOrderColumnName"
               value="<?cs var:html_escape(source.OrderColumnName) ?>" />	     
           <select name="SrcOrderDescending">
               <?cs if:source.OrderDescending ?>
                  <option value="false"> Ascending </option>
                  <option value="true" selected> Descending </option>
               <?cs else ?>
                  <option value="false" selected> Ascending </option>
                  <option value="true"> Descending </option>
            </select>
	    <?cs /if ?>
         </div>
         <?cs if:source.Error.OrderByColumn ?>
            <div class="configuration_error">
               <?cs var:source.Error.OrderByColumn ?>      
            </div>
         <?cs /if ?>
      </td>
   </tr>
   <tr class="selected" id="SrcReconnectToDbRow">
      <td class="left_column">Reconnect to DB?</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>           
            <?cs if:source.MaxReconnects == 0 && source.UnlimitedMaxReconnects == 0 ?>No
            <?cs elif:source.UnlimitedMaxReconnects != 0?>Yes <span class="configurationFillerText">(unlimited)</span>
            <?cs elif:source.MaxReconnects > 0 && source.UnlimitedMaxReconnects == 0 ?>Yes<span class="configurationFillerText">, with limit of</span><?cs /if ?>
         <?cs else ?>    
            <select name="SrcMaxReconnectsChoiceBox" onchange="fromDBToggleNumberOfReconnectsTextField();" id="SrcMaxReconnectsChoiceBox"
               <option value="never" <?cs if:source.MaxReconnects == 0 ?>selected="selected"<?cs /if ?> >No</option>
               <option value="unlimited" <?cs if:source.UnlimitedMaxReconnects != 0?>selected="selected"<?cs /if ?> >Yes (unlimited)</option>
               <option value="other"  <?cs if:source.MaxReconnects > 0 && source.UnlimitedMaxReconnects == 0 ?>selected="selected"<?cs /if ?>>Yes, with limit</option>
            </select>
         <?cs /if ?>
         <span id="SrcMaxReconnectsText" style="font-size: 11px;<?cs if:source.UnlimitedMaxReconnects != 0 || source.MaxReconnects == 0 ?> display: none;<?cs /if ?>" >
            <?cs if:Channel.ReadOnlyMode ?>
               <?cs var:source.MaxReconnects ?> times
            <?cs else ?>       
               of&nbsp;
               <input class=" number_field" type="text" name="SrcMaxReconnects" id="SrcMaxReconnectsInput"
                  value="<?cs if:source.MaxReconnects == 0 ?>10<?cs else ?><?cs var:source.MaxReconnects ?><?cs /if ?>" />
               &nbsp;times
               <span id="SrcMaxReconnectsErrorMessageContainer" class="validation_error_message_container"></span>
               <script defer type="text/javascript">
                  VALregisterIntegerValidationFunction
                  (
                     'SrcMaxReconnectsInput', 'SrcReconnectToDbRow', 'SrcMaxReconnectsErrorMessageContainer',
                     function()
                     {
                        var ReconnectOptionsComboBox = document.getElementById('SrcMaxReconnectsChoiceBox');
                        return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value == "other";
                     },
                     showSourceTab
                  );
               </script>
            <?cs /if ?>    
        </span>

     </td>
  </tr>
  
  <tr class="selected" id="SrcReconnectIntervalRow"<?cs if:source.MaxReconnects == 0 && source.UnlimitedMaxReconnects == 0 ?> style="display:none;"<?cs /if ?>>
     <td class="left_column">Reconnection interval<font color="#ff0000">*</font></td>
     <td class="inner_left" colspan="3">
        <?cs if:Channel.ReadOnlyMode ?>
           <?cs var:source.ReconnectInterval ?>
        <?cs else ?>       
           <input class=" number_field" type="text" name="SrcReconnectInterval" id="SrcReconnectIntervalInput"
              value="<?cs var:source.ReconnectInterval ?>">
              <script defer type="text/javascript">
                 VALregisterIntegerValidationFunction
                 (
                    'SrcReconnectIntervalInput', 'SrcReconnectIntervalRow', 'SrcReconnectIntervalErrorMessageContainer',
                    function()
                    {
                       var ReconnectOptionsComboBox = document.channel_data.SrcMaxReconnectsChoiceBox;
                       return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value != "never";
                    },
                    showSourceTab 
                 );
              </script>
        <?cs /if ?>    
        &nbsp;milliseconds
        <span id="SrcReconnectIntervalErrorMessageContainer" class="validation_error_message_container"></span>
      </td>
   </tr>

   <tr class="selected" id="RowSrcTimeout"
      <?cs if:Channel.ReadOnlyMode && source.Database.Type != "ODBC - MS SQL Server" ?>
         style="display: none;"
      <?cs /if ?>
   >
      <td class="left_column">Database timeout<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.DatabaseTimeout) ?>
         <?cs else ?>
            <input class="number_field" type="text" name="SrcDatabaseTimeout" id="SrcDatabaseTimeoutInput"
               value="<?cs var:source.DatabaseTimeout ?>">
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction
               (
                  'SrcDatabaseTimeoutInput', 'RowSrcTimeout', 'SrcDatabaseTimeoutErrorMessageContainer',
                  function()
                  {
                     var TimeoutRow = document.getElementById('RowSrcTimeout');
                     return WINgetStyle(TimeoutRow, 'display') != 'none';
                  },
                  showSourceTab, 10, 25000
               );
            </script>
         <?cs /if ?>
         &nbsp;seconds
         <span id="SrcDatabaseTimeoutErrorMessageContainer" class="validation_error_message_container"></span>
      </td>
   </tr>

<?cs /with ?>
