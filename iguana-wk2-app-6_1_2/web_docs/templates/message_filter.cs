<?cs # vim: set syntax=html :?>

<?cs include:"browse_macro.cs" ?>

<?cs set: useMessageFilter = #Channel.UseMessageFilter ?>

<script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>

<script defer type="text/javascript"> <!--

var CountOfIgnoredMessage = 0;
var ShowRowStyle = null;

function truncateMessageName(MessageName)
{
   if (MessageName.length > 48)
   {
      MessageName = MessageName.substring(0, 48) + "...";
   }
   return MessageName;
}

function CMFhideAllTransformationFields(){
   document.getElementById("filter_scripted_options").style.display = 'none';
   document.getElementById('filter_graphical_options_in').style.display = 'none';
   document.getElementById('filter_graphical_options_out').style.display = 'none';
}

function showTransformationFields(Mode)
{
   CMFhideAllTransformationFields();
   
   if( Mode == "ScriptedTransformation" )        {  document.getElementById("filter_scripted_options").style.display = '';}
   else if( Mode == "GraphicalTransformation" )  { document.getElementById('filter_graphical_options_in').style.display = ''; document.getElementById('filter_graphical_options_out').style.display = ''; }
}

function onFilterAfterLoggingChange()
{
   var FilterAfterLogging = document.getElementById('FilterAfterLogging').value;
   var Prefix;
   if (FilterAfterLogging == "true")
   {
      Prefix = "post";
   }
   else
   {
      Prefix = "pre";
   }
   document.getElementById('logPrePostFilterMessagePrefixSpan').innerHTML = Prefix;
}

function onLoadMessageFilter()
{
   // set the "use filter" checkbox's value again, to prevent firefox's input caching
   // setting it to the incorrect value.
   <?cs if:!Channel.ReadOnlyMode && !Channel.IsEncrypted ?>
      document.getElementById("useFilterCheckbox").checked = <?cs if:useMessageFilter ?>true<?cs else ?>false<?cs /if ?>;
   <?cs /if ?>
   ShowRowStyle = WINgetStyle(document.getElementById("trUseMessageFilter"), "display");
   
   <?cs each:ignoredMessage = Channel.MessageFilter.IgnoredMessages ?>
   addIgnoredMessageRow("<?cs var:html_escape(ignoredMessage) ?>");
   <?cs /each ?>
   
   onFilterAfterLoggingChange();

   <?cs if:!Channel.ReadOnly && !Channel.ReadOnlyMode ?>
   document.getElementById('inputNewIgnoredMessage').onkeydown = function(e) {
      if(window.event) e = window.event;
      if(e.keyCode == 13) // Enter
      {
         // If addNewIgnoredMessage() displays an alert, some browsers ignore the result of
         // this handler and submit the form.  To avoid this, we call it asynchronously.
         //
         setTimeout(addNewIgnoredMessage, 0);
         return false;
      }
   };
   <?cs /if ?>
   
   CMFshowFilterOptions();
}

function createIgnoredMessageBox(MessageName)
{
   var NewRecipientBox = document.createElement('a');
   var NewRecipientSpan = document.createElement('span');
   NewRecipientBox.className = "ignored_message";
   NewRecipientSpan.innerHTML = truncateMessageName(MessageName);
   NewRecipientBox.appendChild(NewRecipientSpan);
   NewRecipientBox.onclick = function() { return false; }
   return NewRecipientBox;
}

function addNewIgnoredMessage()
{
   var IgnoredMessageInput = document.getElementById('inputNewIgnoredMessage');
   
   // strip whitespace
   var IgnoredMessageName = IgnoredMessageInput.value.replace(/^\W+/,'').replace(/\W+$/,'');
   
   if (IgnoredMessageName == '')
   {
      alert('Please enter a message name.');
      return false;
   }
   
   // check if it is a duplicate
   var IgnoredMessagesTable = document.getElementById('ignoredMessagesTableBody');
   var IgnoredMessages = IgnoredMessagesTable.getElementsByTagName('input');
   for (var InputIndex = 0; InputIndex < IgnoredMessages.length; ++InputIndex)
   {
      if (IgnoredMessages[InputIndex].value == IgnoredMessageName &&
          IgnoredMessages[InputIndex].id != "inputNewIgnoredMessage")
      {
         alert('Message "' + IgnoredMessageName + '" has already been added.');
         return false;
      }
   }
   
   addIgnoredMessageRow(IgnoredMessageInput.value);
   
   IgnoredMessageInput.value = "";
   IgnoredMessageInput.focus();
   
   return false;
}

function addIgnoredMessageRow(IgnoredMessageName)
{
   var IgnoredMessagesTable = document.getElementById('ignoredMessagesTableBody');
   var IgnoredMessagesInputRow = document.getElementById('ignoredMessagesInputRow');
   
   var NewRow          = document.createElement('tr');
   var NewValueColumn  = document.createElement('td');
   var NewValueInput   = document.createElement('input');
   var NewRemoveColumn = document.createElement('td');
   var NewRemoveButton = document.createElement('a');
   var NewRemoveSpan   = document.createElement('span');
   
   NewRow.className = "ignored_message_row";
   
   NewValueInput.type  = "hidden";
   NewValueInput.name  = "IgnoredMessage_" + CountOfIgnoredMessage++;
   NewValueInput.id    = NewValueInput.name;
   NewValueInput.value = IgnoredMessageName;
   
   NewRemoveButton.href = "#";
   NewRemoveButton.className = "action-button-small blue";
   NewRemoveButton.appendChild(NewRemoveSpan);
   NewRemoveSpan.appendChild(document.createTextNode("Remove"));
   NewRemoveButton.onclick = function()
   {
      IgnoredMessagesTable.removeChild(NewRow);
      return false;
   }
   
   NewValueColumn.appendChild(  NewValueInput   );
   NewValueColumn.appendChild(  createIgnoredMessageBox(IgnoredMessageName) );
   <?cs if:!Channel.ReadOnly && !Channel.ReadOnlyMode ?>
   NewRemoveColumn.appendChild( NewRemoveButton );
   <?cs /if ?>
   NewRow.appendChild(          NewValueColumn  );
   NewRow.appendChild(          NewRemoveColumn );
   
   IgnoredMessagesTable.insertBefore(NewRow, IgnoredMessagesInputRow);
   
   return false;
}

function CMFuseFilter(){
   return <?cs if:Channel.ReadOnlyMode || Channel.IsEncrypted ?><?cs
             var:#useMessageFilter ?><?cs
          else
             ?>document.getElementById('useFilterCheckbox').checked<?cs
          /if ?>;
}

function CMFfilterMode(){
   return <?cs if:Channel.ReadOnlyMode || Channel.IsEncrypted ?><?cs
             if:Channel.MessageFilter.UseMapperFilter ?>'mapper'<?cs else ?>'legacy'<?cs /if ?><?cs
          else
             ?>$('input[name="FilterType"]:checked').val()<?cs
          /if ?>;
}

function CMFtransformationMode(){
   return <?cs if:Channel.ReadOnlyMode || Channel.IsEncrypted ?>'<?cs
             var:js_escape(Channel.MessageFilter.TransformationMode) ?>'<?cs
          else
             ?>document.getElementById('FilterTransformationMode').value<?cs
          /if ?>;
}

function CMFshowFilterOptions(){
   var UseFilter = CMFuseFilter();
   
   var AllFilterRows = $('tr.disappearingMessageFilterRow');
   
   CMFhideAllTransformationFields();
   
   if (!UseFilter){
      AllFilterRows.css('display', 'none');
   } else {
      AllFilterRows.css('display', '');
      var FilterMode = CMFfilterMode();
      var LegacyFilterRows = $('tr.legacyFilterRow');
      var MapperFilterRows = $('tr.mapperFilterRow');
      if (!FilterMode){
         LegacyFilterRows.css('display', 'none');
         MapperFilterRows.css('display', 'none');
      } else if (FilterMode == 'mapper'){
         LegacyFilterRows.css('display', 'none');
         MapperFilterRows.css('display', '');
      } else {
         var TransformationMode = CMFtransformationMode();
         LegacyFilterRows.css('display', '');
         MapperFilterRows.css('display', 'none');
         showTransformationFields(TransformationMode);
      }
   }
}

--> </script>

<?cs include: "mapper_config.cs" ?>

<?cs with: filter = Channel.MessageFilter ?>
   <tr id="trUseMessageFilter" class="selected">
      <td class="left_column first_row" style="vertical-align:top; padding-top:8px;"><label for="useFilterCheckbox">Use filter</label></td>
      <td class="inner_left first_row" style="vertical-align:top; padding-top:6px;">
         <?cs if:Channel.ReadOnlyMode || Channel.IsEncrypted ?>
            <div style="padding-top: 2px;">
               <?cs if:useMessageFilter ?>Yes<?cs else ?>No<?cs /if ?>
            </div>
            <input id="useFilterCheckbox" type="hidden" name="UseMessageFilter" value="<?cs if:useMessageFilter ?>on<?cs else ?>off<?cs /if ?>" />
         <?cs else ?>
            <input id="useFilterCheckbox" class="no_style" type="checkbox" name="UseMessageFilter" onclick="CMFshowFilterOptions();">
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="disappearingMessageFilterRow selected">
      <td class="left_column">Apply filter</td>
      <td class="inner_left" colspan="3">
         <?cs set:BeforeText = 'before writing the message to the queue' ?>
         <?cs set:AfterText = 'after reading the message from the queue' ?>
         <?cs if:Channel.ReadOnlyMode || Channel.IsEncrypted ?>
            <?cs if:Channel.Destination.IsToChannel ?>
               <?cs var:BeforeText ?>
            <?cs elif:Channel.Source.IsFromChannel ?>
               <?cs var:AfterText ?>
            <?cs elif:filter.FilterAfterLogging ?>
               <?cs var:AfterText ?>
            <?cs else ?>
               <?cs var:BeforeText ?>
            <?cs /if ?>
            <input id="FilterAfterLogging" name="FilterAfterLogging" type="hidden" value="<?cs if:Channel.Source.IsFromChannel || filter.FilterAfterLogging ?>true<?cs else ?>false<?cs /if ?>" />
         <?cs else ?>
            <?cs if:Channel.Destination.IsToChannel ?>
               <input type="hidden" name="FilterAfterLogging" id="FilterAfterLogging" value="false" />
               <select disabled><option selected><?cs var:BeforeText ?></option></select>
            <?cs elif:Channel.Source.IsFromChannel ?>
               <input type="hidden" name="FilterAfterLogging" id="FilterAfterLogging" value="true" />
               <select disabled><option selected><?cs var:AfterText ?></option></select>
            <?cs elif:filter.FilterAfterLogging ?>
            <select name="FilterAfterLogging" id="FilterAfterLogging" onchange="onFilterAfterLoggingChange();">
               <option value="false"        ><?cs var:BeforeText ?></option>
               <option value="true" selected><?cs var:AfterText ?></option>
            </select>
            <?cs else ?>
            <select name="FilterAfterLogging" id="FilterAfterLogging" onchange="onFilterAfterLoggingChange();">
               <option value="false" selected><?cs var:BeforeText ?></option>
               <option value="true"          ><?cs var:AfterText ?></option>
            </select>
            <?cs /if ?>
            <a id="FilterAfterLogging_Icon" class="helpIcon" tabindex="103" rel="<a href='<?cs var:help_link('iguana4_applying_filter') ?>' target='_blank'>Learn more about this option</a>." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="disappearingMessageFilterRow selected">
      <td class="left_column">
         Log <span id="logPrePostFilterMessagePrefixSpan"><?cs if:filter.FilterAfterLogging ?>post<?cs else ?>pre<?cs /if ?></span>-filter message
      </td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode || Channel.IsEncrypted ?>
            <?cs if:filter.LogPrePostFilterMessage ?>Yes<?cs else ?>No<?cs /if ?>
            <input type="hidden" name="LogPrePostFilterMessage" value="<?cs if:filter.LogPrePostFilterMessage ?>on<?cs else ?>off<?cs /if ?>" />
         <?cs else ?>
            <input type="checkbox" name="LogPrePostFilterMessage" <?cs if:filter.LogPrePostFilterMessage ?>checked="checked"<?cs /if ?> />
            <a id="LogPrePostFilterMessage_Icon" class="helpIcon" tabindex="103" rel="The message will appear as an Unqueued type log entry." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="disappearingMessageFilterRow selected">
      <td class="left_column">Filter type</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode || Channel.IsEncrypted ?>
            <?cs if:filter.UseMapperFilter ?>Translator<?cs else ?>Legacy<?cs /if ?>
            <input type="hidden" name="FilterType" value="<?cs if:filter.UseMapperFilter ?>mapper<?cs else ?>legacy<?cs /if ?>" />
         <?cs else ?>
            <input type="radio" name="FilterType" id="FilterTypeTranslator" value="mapper" onclick="javascript:CMFshowFilterOptions();"
               <?cs if:filter.UseMapperFilter ?>checked="checked"<?cs /if ?> /><label for="FilterTypeTranslator">Translator</label>
            <br>
            <input type="radio" name="FilterType" id="FilterTypeLegacy" value="legacy" onclick="javascript:CMFshowFilterOptions();"
               <?cs if:!filter.UseMapperFilter ?>checked="checked"<?cs /if ?> /><label for="FilterTypeLegacy">Legacy</label>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="channel_configuration_section_heading_row disappearingMessageFilterRow mapperFilterRow ">
      <td>
         <table>
            <tr>
               <td style="width:100%;"><hr /></td>
               <td class="channel_configuration_section_heading">Translator Settings</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>
   
   <?cs call:renderMapperForm(filter, filter.IsMapperInitialized, 'Filter', 'Filter', 'Filter', 'disappearingMessageFilterRow mapperFilterRow', 1, 1) ?>

   <tr class="channel_configuration_section_heading_row disappearingMessageFilterRow legacyFilterRow ">
      <td>
         <table>
            <tr>
               <td style="width:100%;"><hr /></td>
               <td class="channel_configuration_section_heading">Legacy Settings</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>

   <tr class="disappearingMessageFilterRow legacyFilterRow selected">
      <td class="left_column first_row">
         Filter VMD path<font color="#ff0000">*</font>
      </td>
      <td class="inner_left first_row" colspan="3">
         <?cs if:Channel.ReadOnlyMode || Channel.IsEncrypted ?>
            <?cs if:filter.Error.FilterVmdPath ?>
               <div class="configuration_error">
                  <?cs var:filter.Error.FilterVmdPath ?>
               </div>
            <?cs else ?> 
               <?cs if:filter.FilterVmdPath != filter.OriginalVmdPath?>
               <?cs /if ?>
               <?cs if:filter.OriginalVmdPath != "" ?>
                  <label>Uploaded from: </label><br><?cs call:browse_readonly(filter.OriginalVmdPath) ?>
              <?cs /if ?>
              <?cs if:filter.FilterVmdPath != "" ?>
                 <br><br><?cs call:download_vmd(Channel.Guid, Channel.editTab, Channel.Name, "FilterVmd") ?>
              <?cs /if ?>
            <?cs /if ?>
         <?cs else ?>
          <?cs if:!filter.Error.FilterVmdPath && filter.OriginalVmdPath != ""?>
            <div class="original_vmd">
               <label>Uploaded from: </label><br><?cs call:browse_readonly(filter.OriginalVmdPath) ?>
            </div><br>
            <?cs /if ?>
            <br><?cs call:browse_input('FilterVmdPath', filter.FilterVmdPath) ?>
            <?cs if:!filter.Error.FilterVmdPath && filter.OriginalVmdPath != ""?>
            <br><a id="useOriginalMessageFilter"><u>Use previously uploaded VMD path</u></a>
            <script type="text/javascript">
               $("#useOriginalMessageFilter").click(function(){
                  $("#FilterVmdPath").val("<?cs var:filter.CleanedOriginal ?>");
               });
            </script>
            <?cs /if ?>
            <?cs if:filter.FilterVmdPath != "" ?>
               <br><br><?cs call:download_vmd(Channel.Guid, Channel.editTab, Channel.Name, "FilterVmd") ?>
               <a id="DownloadVmdBox" style="padding-left: 4px;" class="helpIcon" tabindex="101" rel="Download the currently running VMD from source control." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
            <?cs /if ?>   
         <?cs /if ?>
      </td>
   </tr>

    <tr id="TrackVmdChangesMessage" class="disappearingMessageFilterRow legacyFilterRow selected">
       <td class="left_column">Track VMD Changes</td>
         <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
            <table class="inner">
               <tr>
                  <td class="inner_left">
                     <?cs if:Channel.ReadOnlyMode || Channel.IsEncrypted ?> 
                        <?cs if:filter.KeepUpdated ?>Yes<?cs else ?>No<?cs /if ?>          
                     <?cs else ?>
                        <br><?cs call:keep_updated("UpdatedMessageFilterVmd", filter.KeepUpdated) ?>
                        <a id="KeepVmdUpdatedBox" class="helpIcon" tabindex="101" rel="When enabled, at startup, the channel will compare its current copy of the VMD with the version at the path it was uploaded from. If they are different, the 'uploaded from' version will be automatically copied into the run location and committed to source control before the channel begins processing messages." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                     <?cs /if ?>
                  </td>
                  <td class="inner_right">
                     <?cs if:Channel.MessageFilter.Error.KeepUpdated ?>
                        <div class="configuration_error">
                        <ul class="configuration"><?cs var:Channel.MessageFilter.Error.KeepUpdated ?></ul>
                        </div>
                     <?cs /if ?>
                  </td>
               </tr>     
            </table>
         </td>
   </tr>
   
   <tr class="disappearingMessageFilterRow legacyFilterRow selected">
      <td class="left_column">Error handling</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if: filter.ErrorHandling == 'Stop' ?>
               Stop channel
            <?cs else ?>
               Skip message
            <?cs /if ?>
         <?cs else ?>
            <select class="configuration" name="FilterErrorHandling">
               <?cs if: filter.ErrorHandling == 'Stop' ?>
                  <option value="Skip">          Skip message </option>
                  <option value="Stop" selected> Stop channel </option>
               <?cs else ?>
                  <option value="Skip" selected> Skip message </option>
                  <option value="Stop">          Stop channel </option>
               <?cs /if ?>
            </select>
         <?cs /if ?>
      </td>
   </tr>
   
   
   <tr class="disappearingMessageFilterRow legacyFilterRow selected">
      <td class="left_column">
         Ignored messages
      </td>
      <td class="inner" colspan="3">
         
         <table class="inner" style="float:left;">
            <tbody id="ignoredMessagesTableBody">    
               <tr id="ignoredMessagesInputRow" style="height:0px;">
               </tr>
               <?cs if:!Channel.ReadOnly && !Channel.ReadOnlyMode ?>
                  <tr>
                     <td style="text-align: left;">
                        <input id="inputNewIgnoredMessage" name="NewIgnoredMessageTextField" class="configuration" type="text" size="20" />
                     </td>
                     <td>
                        <a class="action-button-small blue" href="#" onclick="return addNewIgnoredMessage();"> Add Ignored Message </a>
                     </td>
                  </tr>
               <?cs /if ?>
            </tbody>
         </table>
         <?cs if:filter.Error.IgnoredMessages ?>
            <div class="configuration_error">
             <?cs var:filter.Error.IgnoredMessages ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
   
   
 
   
   <tr class="disappearingMessageFilterRow legacyFilterRow selected">
      <td class="left_column">
            Transformation mode
      </td>

      <td class="inner" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs def:transformation_mode_readonly(value,text) ?>
               <?cs if: filter.TransformationMode == value ?>
                  <?cs var:html_escape(text)?>
               <?cs /if ?>
            <?cs /def ?>
            <?cs call:transformation_mode_readonly('NoTransformation',        'No transformation') ?>
            <?cs call:transformation_mode_readonly('ScriptedTransformation',  'Scripted transformation') ?>
            <?cs call:transformation_mode_readonly('GraphicalTransformation', 'Graphical transformation') ?>
            <?cs call:transformation_mode_readonly('XmlToHl7',                'XML to HL7') ?>
            <?cs call:transformation_mode_readonly('Hl7ToXml',                'HL7 to XML') ?>       
         <?cs else ?>
            <?cs def:transformation_mode(value,text) ?>
               <option value="<?cs var:html_escape(value) ?>"
                  <?cs if: filter.TransformationMode == value ?>
                  selected=selected
                  <?cs /if ?>
               >
                  <?cs var:html_escape(text)?>
               </option>
            <?cs /def ?>

            <select name="FilterTransformationMode" id="FilterTransformationMode"
                  onchange="showTransformationFields(value);"
                  onkeyup="showTransformationFields(value);"
            >
               <?cs call:transformation_mode('NoTransformation',        'No transformation') ?>
               <?cs call:transformation_mode('ScriptedTransformation',  'Scripted transformation') ?>
               <?cs call:transformation_mode('GraphicalTransformation', 'Graphical transformation') ?>
               <?cs call:transformation_mode('XmlToHl7',                'XML to HL7') ?>
               <?cs call:transformation_mode('Hl7ToXml',                'HL7 to XML') ?>
            </select>
       <?cs /if ?>
      </td>
   </tr>
   
   <tr id="filter_scripted_options" style="display:none" class="legacyFilterRow selected">
      <td class="left_column">
         Configuration name<font color="#ff0000">*</font>
      </td>
      
      <td class="inner" colspan="3" valign="middle">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(filter.ConfigurationName) ?>
         <?cs else ?>
            <input class="configuration" name="FilterConfigurationName" value="<?cs var:html_escape(filter.ConfigurationName) ?>" />
         <?cs /if ?>
              
         <?cs if:filter.Error.ScriptedTransformationConfigurationName ?>
            <div class="configuration_error">
               <?cs var:filter.Error.ScriptedTransformationConfigurationName ?>
            </div>
         <?cs /if ?>

      </td>
   </tr>

   <tr id="filter_graphical_options_in" style="display:none" class="legacyFilterRow selected">
      <td class="left_column">
         Incoming configuration name<font color="#ff0000">*</font>
      </td>
      
      <td class="inner_left" colspan="3" valign="middle">

         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(filter.IncomingConfigurationName) ?>
         <?cs else ?>
            <input class="configuration" name="FilterIncomingConfigurationName" value="<?cs var:html_escape(filter.IncomingConfigurationName) ?>" />
         <?cs /if ?>

         <?cs if:filter.Error.GraphicalIncomingConfigurationName ?>
            <div class="configuration_error">
               <?cs var:filter.Error.GraphicalIncomingConfigurationName ?>
            </div>
         <?cs /if ?>

      </td>
   </tr>
    <tr id="filter_graphical_options_out" style="display:none" class="legacyFilterRow selected">
      <td class="left_column">
         Outgoing configuration name<font color="#ff0000">*</font>
      </td>
      
      <td class="inner_left" colspan="3" valign="middle">

         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(filter.OutgoingConfigurationName) ?>
         <?cs else ?>
            <input class="configuration" name="FilterOutgoingConfigurationName" value="<?cs var:html_escape(filter.OutgoingConfigurationName) ?>">
         <?cs /if ?>
  
         <?cs if:filter.Error.GraphicalOutgoingConfigurationName ?>
            <div class="configuration_error">
               <?cs var:filter.Error.GraphicalOutgoingConfigurationName ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
   
   
<?cs /with ?>
