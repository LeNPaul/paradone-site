<?cs # vim: set syntax=html :?>

<?cs include:"browse_macro.cs" ?>

<script defer type="text/javascript"> <!--

function toPluginToggleNumberOfReconnectsTextField()
{
   var ReconnectOptionsComboBox = document.channel_data.DestMaxReconnectsChoiceBox;
   var ReconnectsText = document.getElementById('DestMaxReconnectsText');
   var ReconnectIntervalRow = document.getElementById('DestReconnectIntervalRow');
   
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


function updateDstProgramParametersPreview()
{

   var Input = document.getElementById('DstProgramParameters');
   var Preview = document.getElementById('dst_program_parameters_preview');
   var PreviewDiv = document.getElementById('dst_program_parameters_preview_div');

   if (Input)
   {
      updatePath('DestProgramParameters', Input, Preview, PreviewDiv, true);
   }

}

function getTagElementsByName(tag, name) {
     
     var elem = document.getElementsByTagName(tag);
     var arr = new Array();
     for(i = 0; i < elem.length; i++) {
          att = elem[i].getAttribute("name");
          if(att == name) {
               arr.push(elem[i]);
          }
     }
     return arr;
}


function loggingChange(Checkbox)
{

    Elements = document.getElementsByName("HideIfNotLogged");
    if (Elements.length == 0)
    {
	    Elements = getTagElementsByName("tr", "HideIfNotLogged");
    }

    for(i = 0; i < Elements.length; i++)
    {
        Elements[i].style.display = (Checkbox.checked)? '' : 'none';
    }
}

function purgeChange()
{	
   var Checkbox = document.getElementById("DstDoPurge");
   var PurgeRow = document.getElementById("DstPurgeTimeRow");
   if (!PurgeRow || !Checkbox)
   {
      return;
   }
   PurgeRow.style.display = (Checkbox.checked)? '' : 'none';
}

function showDstExecutablePathAndDirectoryFields()
{
   var ExecutableFields = getStyleObject('DstExecutablePathAndDirectoryFields1');
   if (document.getElementById('DstAutoRun').checked)
   {
      document.getElementById('DstExecutablePathAndDirectoryFields1').style.display = "";
      document.getElementById('DstExecutablePathAndDirectoryFields2').style.display = "";
      document.getElementById('DstExecutablePathAndDirectoryFields3').style.display = "";
      document.getElementById('DstExecutablePathAndDirectoryFields4').style.display = "";
      document.getElementById('DstExecutablePathAndDirectoryFields5').style.display = "";
      if ( document.getElementById('DstExecutablePathAndDirectoryFields7') )
      {
         document.getElementById('DstExecutablePathAndDirectoryFields7').style.display = "";
      }
   }
   else
   {
      document.getElementById('DstExecutablePathAndDirectoryFields1').style.display = "none";
      document.getElementById('DstExecutablePathAndDirectoryFields2').style.display = "none";
      document.getElementById('DstExecutablePathAndDirectoryFields3').style.display = "none";
      document.getElementById('DstExecutablePathAndDirectoryFields4').style.display = "none";
      document.getElementById('DstExecutablePathAndDirectoryFields5').style.display = "none";
      if ( document.getElementById('DstExecutablePathAndDirectoryFields7') )
      {
         document.getElementById('DstExecutablePathAndDirectoryFields7').style.display = "none";
      }
   }
}

function toggleDstWorkingDirectoryTextField()
{
   var UseDefaultWorkingDirectoryComboBox = document.channel_data.DstUseDefaultWorkingDirectory;
   var WorkingDirectoryTextField = document.getElementById('DstWorkingDirectoryDiv');
   
   if(UseDefaultWorkingDirectoryComboBox != null)
   {
      if(UseDefaultWorkingDirectoryComboBox.options[UseDefaultWorkingDirectoryComboBox.selectedIndex].value == "true")
      {
         WorkingDirectoryTextField.style.display="none";
      }
      else
      {
         WorkingDirectoryTextField.style.display="";
      }
   }
}

function onLoadToExecutable()
{
   showDstExecutablePathAndDirectoryFields();
   toggleDstWorkingDirectoryTextField();
   setInterval('updateDstProgramParametersPreview();', 500);
   <?cs if:!Channel.ReadOnlyMode ?>
   loggingChange(document.getElementById('DstLoggingActive'));
   <?cs /if ?>
   purgeChange();

}

--> </script>

<?cs with: dest = Channel.Destination ?>

   <tr class="selected">
      <td class="left_column">Iguana starts executable</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <input type="checkbox" class="no_style" id="DstAutoRun" name="DstAutoRun" onclick="showDstExecutablePathAndDirectoryFields();" style="display:none;" <?cs if:dest.AutoRun ?>checked<?cs /if ?> />
            <?cs if:dest.AutoRun ?>Yes<?cs else ?>No<?cs /if ?>
         <?cs else ?>
            <input type="checkbox" class="no_style" id="DstAutoRun" name="DstAutoRun" onclick="showDstExecutablePathAndDirectoryFields();" <?cs if:dest.AutoRun ?>checked<?cs /if ?> />
         <?cs /if ?>
      </td>
   </tr>
   
   <tr id="DstExecutablePathAndDirectoryFields1" <?cs if:dest.AutoRun ?>style="display:none"<?cs /if ?> class="selected">
      <td class="left_column">Full executable path<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.ExecutablePath) ?>
            <?cs if:Channel.Destination.Error.ExecutablePath ?>
               <div class="configuration_error">
                  <?cs var:Channel.Destination.Error.ExecutablePath ?>
               </div>
            <?cs /if ?>
         <?cs else ?>
            <?cs call:browse_input('DstExecutablePath', dest.ExecutablePath) ?>
         <?cs /if ?>
     </td>
   </tr>
   <tr id="DstExecutablePathAndDirectoryFields2" <?cs if:dest.AutoRun ?>style="display:none"<?cs /if ?> class="selected">
      <td class="left_column">Working directory<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
	    <?cs if:dest.UseDefaultWorkingDirectory != 0 ?>
	       Use executable directory
	    <?cs else ?>
	       <?cs call:browse_readonly(dest.WorkingDirectory) ?>
	    <?cs /if ?>
         <?cs else ?>
	    <select name="DstUseDefaultWorkingDirectory" onchange="toggleDstWorkingDirectoryTextField();" onkeyup="toggleDstWorkingDirectoryTextField();">
	       <option value="true" <?cs if:dest.UseDefaultWorkingDirectory != 0 ?>selected="selected"<?cs /if ?> >Use executable directory</option>
	       <option value="false" <?cs if:dest.UseDefaultWorkingDirectory == 0?>selected="selected"<?cs /if ?> >Use alternate directory</option>
	    </select>
	    <div id="DstWorkingDirectoryDiv" <?cs if: source.UseDefaultWorkingDirectory != 0 ?>style="display:none"<?cs /if ?>>
	    	<br>
   	    	<?cs call:browse_input_folder('DstWorkingDirectory', dest.WorkingDirectory) ?>
	    </div>
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.ExecutableWorkingDirectory ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.ExecutableWorkingDirectory ?>
            </div>
         <?cs /if ?>
      </td>
     </tr>
   <tr id="DstExecutablePathAndDirectoryFields3" <?cs if:dest.AutoRun ?>style="display:none"<?cs /if ?> class="selected">
      <td class="left_column">Executable parameters</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:dest.ProgramParameters != "" ?>               
               <?cs var:html_escape(dest.ProgramParameters) ?>
               <?cs if:dest.ProgramParameters != environment_expand(dest.ProgramParameters) ?>   
                  <div class="path_preview"> Preview: "<?cs var:html_escape(environment_expand(dest.ProgramParameters)) ?>"</div>                        
               <?cs /if ?>
            <?cs /if ?>
         <?cs else ?>
            <input type="text" id="DstProgramParameters" name="DstProgramParameters" value="<?cs var:html_escape(dest.ProgramParameters) ?>"
               style="float:left; padding-bottom: 2px; padding-top: 2px;" class="configuration_long"
               onchange="updateDstProgramParametersPreview();" onkeyup="updateDstProgramParametersPreview();"/> 
            <a id="DstProgramParameters_Icon" class="helpIcon" style="margin-left:5px; position:relative; top:4px;" rel="The following macros may be used as executable parameters: $hostname$ $port$ $channel$ $component$ $clientid$." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
            <div id="dst_program_parameters_preview_div" style="display:none;clear:both;">
               <div id="dst_program_parameters_preview" class="path_preview" val="<?cs var:html_escape(environment_expand(dest.ProgramParameters)) ?>"></div>
            </div>
            <a class="file_browse_button" href="#" onclick="javascript:document.getElementById('DstProgramParameters').value='$hostname$ $port$ $channel$ $component$'; return false" style="margin-left:5px;">
               <span>Use Defaults</span>
            </a>
         <?cs /if ?>

          <?cs if:Channel.Destination.Error.ExecutableProgramParameters ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.ExecutableProgramParameters ?>
            </div>
          <?cs /if ?>

      </td>
   </tr>
   <tr id="DstExecutablePathAndDirectoryFields4" <?cs if:dest.AutoRun ?>style="display:none"<?cs /if ?> class="selected">
      <td class="left_column">Startup timeout<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <table border="0" cellspan="0" cellspacing="0" style="position:relative;">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(dest.StartupTimeOut) ?>
                  <?cs else ?>
                     <input class="number_field" type="text" name="DstStartupTimeOut" id="DstStartupTimeOutInput"
                        value="<?cs var:dest.StartupTimeOut ?>" />
                     <script defer type="text/javascript">
                        VALregisterIntegerValidationFunction(
                           'DstStartupTimeOutInput', 'DstExecutablePathAndDirectoryFields4', 'DstStartupTimeOutErrorMessageContainer', 
                           function() {
                              return document.getElementById('DstAutoRun').checked;
                           },
                           showDestinationTab);
                     </script>
                  <?cs /if ?>
                  <span>milliseconds</span>
                  <span id="DstStartupTimeOutErrorMessageContainer" class="validation_error_message_container"></span>
               </td>
            </tr>
         </table>
         <?cs if:Channel.Destination.Error.StartupTimeOut ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.StartupTimeOut ?>
            </div>
         <?cs /if ?>
       </td>
   </tr>
   <tr id="DstExecutablePathAndDirectoryFields5" <?cs if:dest.AutoRun ?>style="display:none"<?cs /if ?> class="selected">
      <td class="left_column">Shutdown timeout<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
 
         <table border="0" cellspan="0" cellspacing="0" style="position:relative;">
             <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(dest.ShutdownTimeOut) ?>
                  <?cs else ?>
                     <input class="number_field" type="text" name="DstShutdownTimeOut" id="DstShutdownTimeOutInput"
                        value="<?cs var:dest.ShutdownTimeOut ?>" />
                     <script defer type="text/javascript">
                        VALregisterIntegerValidationFunction(
                           'DstShutdownTimeOutInput', 'DstExecutablePathAndDirectoryFields5', 'DstShutdownTimeOutErrorMessageContainer', 
                           function() {
                              return document.getElementById('DstAutoRun').checked;
                           },
                           showDestinationTab);
                     </script>
                  <?cs /if ?>
                  <span>milliseconds</span>
                  <span id="DstShutdownTimeOutErrorMessageContainer" class="validation_error_message_container"></span>
               </td>
             </tr>
         </table>
         <?cs if:Channel.Destination.Error.ShutdownTimeOut ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.ShutdownTimeOut ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected" id="DstExecutablePathAndDirectoryFields6">
      <td class="left_column">Plugin timeout<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.RpcTimeOut) ?>
         <?cs else ?>
            <input type="text" class="number_field" name="DstRpcTimeOut" id="DstRpcTimeOutInput"
               value="<?cs var:dest.RpcTimeOut ?>" />
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction('DstRpcTimeOutInput', 'DstExecutablePathAndDirectoryFields6', 'DstRpcTimeOutErrorMessageContainer', null, showDestinationTab);
            </script>
         <?cs /if ?>
         milliseconds
         <span id="DstRpcTimeOutErrorMessageContainer" class="validation_error_message_container"></span>
         <?cs if:Channel.Destination.Error.RpcTimeOut ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.RpcTimeOut ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <?cs if:Channel.ReadOnlyMode ?>
      <tr id="DstExecutablePathAndDirectoryFields7" <?cs if:dest.AutoRun ?>style="display:none"<?cs /if ?> class="selected">  
         <td class="left_column">Diagnostic tools</td>
         <td class="inner_left" colspan="3">
            <a href="plugin_executable_output?Channel=<?cs var:url_escape(Channel.Name) ?>&Component=Destination">Monitor executable output</a>
         </td>
      </tr>
   <?cs /if ?>
        <?cs if: !Channel.ReadOnlyMode ?>
	   <tr class="channel_configuration_section_heading_row">
      <td>
         <table>
            <tr>
               <td style="width:100%;"><hr /></td>
               <td class="channel_configuration_section_heading">Legacy Logging Settings</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>
        <tr class="selected">  
            <td class="left_column first_row">Legacy database logging</td>
        <td class="inner_left first_row"><input name="LoggingActive" id="DstLoggingActive" type="checkbox" <?cs if: LoggingActive ?>checked="true"<?cs /if ?> onclick="loggingChange(this)"/> <a id="DstLoggingActive" class="helpIcon" tabindex="105" rel="If you have created a plugin for versions of Iguana prior to version 4.0, select this check box if you want Iguana to store plugin logging information in a database.</a> " title="Legacy database logging help" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a></td>
	 </tr>

	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?>>
	    <td class="left_column">Database API<font color="#ff0000">*</font></td>
	    
	    <td><select name="API" value="<?cs var:html_escape(API)?>" >
	    <?cs each: database = DatabaseTypes ?>
	       <option value="<?cs var:html_escape(database.Name) ?>"
	       <?cs if: API == database.Name ?> selected="true" <?cs /if ?>
	       ><?cs var:html_escape(database.Name) ?></option> 
	    <?cs /each ?>
	    </select>
	    </td>
	 </tr>
	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> >
		<td class="left_column">Data source<font color="#ff0000">*</font></td>
	    <td><input type="text" name="Database" value="<?cs var:html_escape(Database)?>" /></td>
	 </tr>
	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> >
		<td class="left_column">Database username</td>
	    <td><input type="text" name="Username" value="<?cs var:html_escape(Username)?>" /></td>
	 </tr>
	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> >
		<td class="left_column">Database password</td>
	    <td><input name="Password" type="password" value="<?cs var:html_escape(Password)?>" /></td>
	 </tr>
 	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> >
		<td class="left_column">Log table constant<font color="#ff0000">*</font></td>
	    <td><input type="text" name="Tablename" value="<?cs var:html_escape(Tablename)?>" />
        <a id="Tablename_Icon" class="helpIcon" tabindex="102" rel="This constant is used in front of the log table name and as a postfix to the index table name for this channel.  This is instead of using the channel name as Iguana 3.3.2 did.  It means the name of the channel can be changed without having to change the names of the legacy log tables.  If this entry is left blank it will default to the channel name, converted to lowercase, without spaces or tabs." title="Log table constant help" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a></td>
	 </tr>
	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> id="DstDoPurgeRow">
     <td class="left_column">Purge logs</td>
	 <td>
	 <table>
	 <tr>
	 <td>
	 <input name="DoPurge" id="DstDoPurge" type="checkbox" <?cs if: DoPurge ?>checked="true"<?cs /if ?> onclick="purgeChange()"/>
	 </td>
	 <td>
	 <div id="DstPurgeTimeRow" <?cs if: !DoPurge ?>style="display:none"<?cs /if ?> >Purge logs older than <input type="text" name="PurgeTime" id="DstPurgeTime" class="number_field" style="width: 30px" value="<?cs var:#PurgeTime ?>" /> day(s)</div>	 
	 </td>
       <td> 
       <script defer type="text/javascript">
         VALregisterIntegerValidationFunction(
            'DstPurgeTime', 'DstPurgeTimeRow', 'DstPurgeTimeErrorMessageContainer', 
            function() {
               return document.getElementById('DstLoggingActive').checked;
            },
            showDestinationTab);
       </script>
       <span id="DstPurgeTimeErrorMessageContainer" class="validation_error_message_container"></span>
       </td>
	 </tr>
	 </table>
	 </td>
	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> >
		<td class="left_column">Custom DB columns</td>
	    <td><input type="text" name="CustomFields" style="width: 500px" value="<?cs var:html_escape(CustomFields)?>"/>
	    <a id="CustomFields_Icon" class="helpIcon" tabindex="103" rel="Custom fields must be given as a semicolon separated list. These fields define the extra custom columns on the log table for this component." title="Custom field help" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
	    </td>
	 </tr>
	 <?cs else ?>
	 	   <tr class="channel_configuration_section_heading_row"  <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?>>
      <td>
         <table>
            <tr>
               <td style="width:100%;"><hr /></td>
               <td class="channel_configuration_section_heading">Legacy Logging Settings</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>
	          <tr class="selected" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?>>  
            <td class="left_column first_row">Legacy database logging</td>
	    <td class="inner_left first_row">
	          <input name="LoggingActive" style="display:none" type="checkbox" <?cs if: LoggingActive ?>checked="true"<?cs /if ?>/>
	          <input name="DoPurge" style="display:none" type="checkbox" <?cs if: DoPurge ?>checked="true"<?cs /if ?>/>
	    <table><tr><td>
	    Yes</td>
	    <td>
	    <?cs if: Database != "" && Tablename != "" ?>
	    <a class="action-button-small blue" href="/export_legacy_log_tables.html?ChannelName=<?cs var:Channel.Name ?>" id="hrefCreateLoggingTablesButton"><span>&nbsp;Create Tables</span></a>
	    <?cs /if ?>
	    </td></tr></table>
	    </td>
	 </tr>
	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> >
	    <td class="left_column">Database API<font color="#ff0000">*</font></td>
	    
	    <td><input type="submit" class="hidden_submit" name="API" value="<?cs var:html_escape(API)?>" /><?cs var:html_escape(API)?>
	    </td>
	 </tr>
	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> >
		<td class="left_column">Data source<font color="#ff0000">*</font></td>
	    <td><input type="submit" class="hidden_submit" name="Database" value="<?cs var:html_escape(Database)?>" /><?cs var:html_escape(Database)?>
	    <?cs if:Channel.Destination.Error.LegacyDataSource ?>
               <div class="configuration_error">
	          <?cs var:Channel.Destination.Error.LegacyDataSource ?>
	       </div>
	    <?cs /if ?>
	    </td>
	 </tr>
	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> >
		<td class="left_column">Database username</td>
	    <td><input type="submit" class="hidden_submit" name="Username" value="<?cs var:html_escape(Username)?>" /><?cs var:html_escape(Username)?></td>
	 </tr>
	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> >
		<td class="left_column">Database password</td>
	    <td><input type="submit" class="hidden_submit" name="Password" type="password" value="<?cs var:html_escape(Password)?>" />******</td>
	 </tr>
 	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> >
		<td class="left_column">Log table constant<font color="#ff0000">*</font></td>
	    <td><input type="submit" class="hidden_submit" name="Tablename" value="<?cs var:html_escape(Tablename)?>" />
	    <?cs if:Channel.Destination.Error.LegacyLogTableConstant ?>
               <div class="configuration_error">
	          <?cs var:Channel.Destination.Error.LegacyLogTableConstant ?>
	       </div>
	    <?cs /if ?>
	    <?cs var:html_escape(Tablename)?> 
	    <span class="configurationFillerText">(logging to table </span><?cs var:html_escape(Tablename)?>_dest1_inb_exe_mes_suc <span class="configurationFillerText">using index table</span> log_indices_<?cs var:html_escape(Tablename) ?><span class="configurationFillerText">)</span></td>
	 </tr>
 	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?> >
		<td class="left_column">Purge logs<font color="#ff0000">*</font></td>
        <td>
	<?cs if: DoPurge ?>
	<input type="submit" class="hidden_submit" name="PurgeTime" value="<?cs var:#PurgeTime?>" />
	<span class="configurationFillerText">older than </span><?cs var:#PurgeTime?> day<?cs if:PurgeTime > 1?>s<?cs /if ?>
			    <?cs if:Channel.Destination.Error.LegacyPurgeTime ?>
               <div class="configuration_error">
	          <?cs var:Channel.Destination.Error.LegacyPurgeTime ?>
	       </div>
	    <?cs /if ?>
	<?cs else ?>
	No
	<?cs /if ?>
	</td>
	 </tr>
	 <tr name="HideIfNotLogged" <?cs if: !LoggingActive || CustomFields == "" ?>style="display:none"<?cs /if ?> >
		<td class="left_column">Custom DB columns</td>
	    <td><input type="submit" class="hidden_submit" name="CustomFields" value="<?cs var:html_escape(CustomFields)?>" /><?cs var:html_escape(CustomFields)?></td>
	 </tr>
	 <?cs /if ?>



   <tr class="selected" id="DestReconnectToDbRow" name="HideIfNotLogged" <?cs if: !LoggingActive ?>style="display:none"<?cs /if ?>>
      <td class="left_column">Reconnect to DB?</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>           
            <?cs if:dest.MaxReconnects == 0 && dest.UnlimitedMaxReconnects == 0 ?>No
            <?cs elif:dest.UnlimitedMaxReconnects != 0?>Yes <span class="configurationFillerText">(unlimited)</span>
            <?cs elif:dest.MaxReconnects > 0 && dest.UnlimitedMaxReconnects == 0 ?>Yes<span class="configurationFillerText">, with limit of</span><?cs /if ?>
         <?cs else ?>    
            <select name="DestMaxReconnectsChoiceBox" onchange="toPluginToggleNumberOfReconnectsTextField();" id="DestMaxReconnectsChoiceBox">
               <option value="never" <?cs if:dest.MaxReconnects == 0 ?>selected="selected"<?cs /if ?> >No</option>
               <option value="unlimited" <?cs if:dest.UnlimitedMaxReconnects != 0?>selected="selected"<?cs /if ?> >Yes (unlimited)</option>
               <option value="other"  <?cs if:dest.MaxReconnects > 0 && dest.UnlimitedMaxReconnects == 0 ?>selected="selected"<?cs /if ?>>Yes, with limit</option>
            </select>
         <?cs /if ?>
         <span id="DestMaxReconnectsText" style="font-size: 11px;<?cs if:dest.UnlimitedMaxReconnects != 0 || dest.MaxReconnects == 0 ?> display: none;<?cs /if ?>" >
            <?cs if:Channel.ReadOnlyMode ?>
               <?cs var:dest.MaxReconnects ?> times
            <?cs else ?>       
               of&nbsp;
               <input class="number_field" type="text" name="DestMaxReconnects" id="DestMaxReconnectsInput"
                  value="<?cs if:dest.MaxReconnects == 0 ?>10<?cs else ?><?cs var:dest.MaxReconnects ?><?cs /if ?>" />
               &nbsp;times
               <span id="DestMaxReconnectsErrorMessageContainer" class="validation_error_message_container"></span>
               <script defer type="text/javascript">
                  VALregisterIntegerValidationFunction(
                     'DestMaxReconnectsInput', 'DestReconnectToDbRow', 'DestMaxReconnectsErrorMessageContainer',
                     function() {
                        var ReconnectOptionsComboBox = document.getElementById('DestMaxReconnectsChoiceBox');
                        return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value == "other";
                     },
                     showDestinationTab);
               </script>
            <?cs /if ?>    
        </span>

     </td>
  </tr>
  <tr class="selected" id="DestReconnectIntervalRow"  name="HideIfNotLogged" <?cs if: !LoggingActive || (dest.MaxReconnects == 0 && dest.UnlimitedMaxReconnects == 0) ?> style="display:none;"<?cs /if ?>>
     <td class="left_column">Reconnection interval<font color="#ff0000">*</font></td>
     <td class="inner_left" colspan="3">
        <?cs if:Channel.ReadOnlyMode ?>
           <?cs var:dest.ReconnectInterval ?>
        <?cs else ?>       
           <input type="text" class="number_field" name="DestReconnectInterval" id="DestReconnectIntervalInput"
              value="<?cs var:dest.ReconnectInterval ?>">
              <script defer type="text/javascript">
                 VALregisterIntegerValidationFunction
                 (
                    'DestReconnectIntervalInput', 'DestReconnectIntervalRow', 'DestReconnectIntervalErrorMessageContainer',
                    function()
                    {
                       var ReconnectOptionsComboBox = document.channel_data.DestMaxReconnectsChoiceBox;
                       return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value != "never";
                    },
                    showDestinationTab 
                 );
              </script>
        <?cs /if ?>    
        &nbsp;milliseconds
        <span id="DestReconnectIntervalErrorMessageContainer" class="validation_error_message_container"></span>
      </td>
   </tr>

   
<?cs /with ?>
