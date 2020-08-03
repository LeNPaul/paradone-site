<?cs # vim: set syntax=html :?>

<?cs include:"browse_macro.cs" ?>

<script defer type="text/javascript"> <!--

function updateSrcProgramParametersPreview()
{

   var Input = document.getElementById('SrcProgramParameters');
   var Preview = document.getElementById('src_program_parameters_preview');
   var PreviewDiv = document.getElementById('src_program_parameters_preview_div');

   if (Input)
   {
      updatePath('SourceProgramParameters', Input, Preview, PreviewDiv, true);
   }

}

function showSrcExecutablePathAndDirectoryFields()
{
  
   if (document.getElementById('SrcAutoRun').checked)
   {
      
      document.getElementById('SrcExecutablePathAndDirectoryFields1').style.display = '';
      document.getElementById('SrcExecutablePathAndDirectoryFields2').style.display = '';
      document.getElementById('SrcExecutablePathAndDirectoryFields3').style.display = '';
      document.getElementById('SrcExecutablePathAndDirectoryFields4').style.display = '';
      document.getElementById('SrcExecutablePathAndDirectoryFields5').style.display = '';
      if ( document.getElementById('SrcExecutablePathAndDirectoryFields8') )
      {
         document.getElementById('SrcExecutablePathAndDirectoryFields8').style.display = '';
      }
   }
   else
   {
      document.getElementById('SrcExecutablePathAndDirectoryFields1').style.display = 'none';
      document.getElementById('SrcExecutablePathAndDirectoryFields2').style.display = 'none';
      document.getElementById('SrcExecutablePathAndDirectoryFields3').style.display = 'none';
      document.getElementById('SrcExecutablePathAndDirectoryFields4').style.display = 'none';
      document.getElementById('SrcExecutablePathAndDirectoryFields5').style.display = 'none';
      if ( document.getElementById('SrcExecutablePathAndDirectoryFields8') )
      {
         document.getElementById('SrcExecutablePathAndDirectoryFields8').style.display = 'none';
      }
   }
}

function toggleSrcWorkingDirectoryTextField()
{
   var UseDefaultWorkingDirectoryComboBox = document.channel_data.SrcUseDefaultWorkingDirectory;
   var WorkingDirectoryTextField = document.getElementById('SrcWorkingDirectoryDiv');
   
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

function onLoadFromExecutable()
{
   showSrcExecutablePathAndDirectoryFields();
   toggleSrcWorkingDirectoryTextField();
   setInterval('updateSrcProgramParametersPreview();', 500);
}

--> </script>

<?cs with: source = Channel.Source ?>

   <tr class="selected">
      <td class="left_column">Iguana starts executable</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:source.AutoRun ?>Yes<?cs else ?>No<?cs /if ?>
            <input type="checkbox" class="no_style" id="SrcAutoRun" name="SrcAutoRun" <?cs if:source.AutoRun ?>checked<?cs /if ?> style="display:none;" />
         <?cs else ?>
            <input type="checkbox" class="no_style" id="SrcAutoRun" name="SrcAutoRun" onclick="showSrcExecutablePathAndDirectoryFields();" <?cs if:source.AutoRun ?>checked<?cs /if ?> />
         <?cs /if ?>
      </td>
   </tr>
   <tr id="SrcExecutablePathAndDirectoryFields1" <?cs if:source.AutoRun ?>style="display:none"<?cs /if ?> class="selected">
      <td class="left_column">Full executable path<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">         
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(source.ExecutablePath) ?>
            <?cs if:Channel.Source.Error.ExecutablePath ?>
               <div class="configuration_error">
                  <?cs var:Channel.Source.Error.ExecutablePath ?>
               </div>
            <?cs /if ?>
         <?cs else ?>
            <?cs call:browse_input('SrcExecutablePath', source.ExecutablePath) ?>
         <?cs /if ?>
       </td>
   </tr>
   <tr id="SrcExecutablePathAndDirectoryFields2" <?cs if:source.AutoRun ?>style="display:none"<?cs /if ?> class="selected">
      <td class="left_column">Working directory<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">         
         <?cs if:Channel.ReadOnlyMode ?>
	    <?cs if:source.UseDefaultWorkingDirectory != 0 ?>
	       Use executable directory
	    <?cs else ?>
	       <?cs call:browse_readonly(source.WorkingDirectory) ?>
	    <?cs /if ?>
         <?cs else ?>
	    <select name="SrcUseDefaultWorkingDirectory" onchange="toggleSrcWorkingDirectoryTextField();" onkeyup="toggleSrcWorkingDirectoryTextField();">
	       <option value="true" <?cs if:source.UseDefaultWorkingDirectory != 0 ?>selected="selected"<?cs /if ?> >Use executable directory</option>
	       <option value="false" <?cs if:source.UseDefaultWorkingDirectory == 0?>selected="selected"<?cs /if ?> >Use alternate directory</option>
	    </select>
	    <div id="SrcWorkingDirectoryDiv" <?cs if: source.UseDefaultWorkingDirectory != 0 ?>style="display:none"<?cs /if ?>>
	    	<br>
   	    	<?cs call:browse_input_folder('SrcWorkingDirectory', source.WorkingDirectory) ?>
	    </div>
          <?cs /if ?>
              
         <?cs if:Channel.Source.Error.ExecutableWorkingDirectory ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.ExecutableWorkingDirectory ?>
            </div>
         <?cs /if ?>
       </td>
   </tr>
   <tr id="SrcExecutablePathAndDirectoryFields3" <?cs if:source.AutoRun ?>style="display:none"<?cs /if ?> class="selected">
      <td class="left_column">Executable parameters</td>
      <td class="inner_left" colspan="3">         
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:source.ProgramParameters != "" ?>
               <?cs var:html_escape(source.ProgramParameters) ?>
               <?cs if:source.ProgramParameters != environment_expand(source.ProgramParameters) ?>
                  <div class="path_preview"> Preview: "<?cs var:html_escape(environment_expand(source.ProgramParameters)) ?>"</div>
               <?cs /if ?>
            <?cs /if ?>
         <?cs else ?>
            <input type="text" id="SrcProgramParameters" name="SrcProgramParameters" value="<?cs var:html_escape(source.ProgramParameters) ?>"
               style="float:left;width:75%; padding-bottom: 2px; padding-top: 2px;"
               onchange="updateSrcProgramParametersPreview();" onkeyup="updateSrcProgramParametersPreview();"/>
            <a style="position:relative; top:9px; left:8px;" id="SrcProgramParameters_Icon" class="helpIcon" style="margin-left:5px; position:relative; top:4px;" rel="The following macros may be used as executable parameters: $hostname$ $port$ $channel$ $component$ $clientid$." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
            <a class="file_browse_button" href="#" onclick="javascript:document.getElementById('SrcProgramParameters').value='$hostname$ $port$ $channel$ $component$'; return false" style="margin-left:5px;">
               <span>Use Defaults</span>
            </a>
            <div id="src_program_parameters_preview_div" style="display:none;clear:both;">
               <div id="src_program_parameters_preview" class="path_preview" val="<?cs var:html_escape(environment_expand(source.ProgramParameters)) ?>"></div>
            </div>
         <?cs /if ?>

         <?cs if:Channel.Source.Error.ExecutableProgramParameters ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.ExecutableProgramParameters ?>
            </div>
         <?cs /if ?>
       </td>
   </tr>
   <tr id="SrcExecutablePathAndDirectoryFields4" <?cs if:source.AutoRun ?>style="display:none"<?cs /if ?> class="selected">
      <td class="left_column">Startup timeout<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">         
         <table border="0" cellspan="0" cellspacing="0">           
            <tr>
               <td class="inner_left">                 
                  <?cs if:Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(source.StartupTimeOut) ?>
                  <?cs else ?>
                     <input class="number_field" type="text" name="SrcStartupTimeOut" id="SrcStartupTimeOutInput"
                        value="<?cs var:source.StartupTimeOut ?>" />
                     <script defer type="text/javascript">
                        VALregisterIntegerValidationFunction
                        (
                           'SrcStartupTimeOutInput', 'SrcExecutablePathAndDirectoryFields4', 'SrcStartupTimeOutErrorMessageContainer',
                           function()
                           {
                              return document.getElementById('SrcAutoRun').checked;
                           },
                           showSourceTab 
                        );
                     </script>
                  <?cs /if ?>
                  milliseconds
                  <span id="SrcStartupTimeOutErrorMessageContainer" class="validation_error_message_container"></span>
               </td>
            </tr>
         </table>
         <?cs if:Channel.Source.Error.StartupTimeOut ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.StartupTimeOut ?>
            </div>
         <?cs /if ?>
       </td>
   </tr>
   <tr id="SrcExecutablePathAndDirectoryFields5" <?cs if:source.AutoRun ?>style="display:none"<?cs /if ?> class="selected">
      <td class="left_column">Shutdown timeout<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">         
         <table border="0" cellspan="0" cellspacing="0">           
            <tr>
               <td class="inner_left">                      
                  <?cs if:Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(source.ShutdownTimeOut) ?>
                  <?cs else ?>
                     <input type="text" class="number_field" name="SrcShutdownTimeOut" id="SrcShutdownTimeOutInput"
                        value="<?cs var:source.ShutdownTimeOut ?>" />
                     <script defer type="text/javascript">
                        VALregisterIntegerValidationFunction
                        (
                           'SrcShutdownTimeOutInput', 'SrcExecutablePathAndDirectoryFields5', 'SrcShutdownTimeOutErrorMessageContainer',
                           function()
                           {
                              return document.getElementById('SrcAutoRun').checked;
                           },
                           showSourceTab
                        );
                     </script>
                  <?cs /if ?>
                  milliseconds
                  <span id="SrcShutdownTimeOutErrorMessageContainer" class="validation_error_message_container"></span>
               </td>
            </tr>               
         </table>      
         <?cs if:Channel.Source.Error.ShutdownTimeOut ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.ShutdownTimeOut ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected" id="SrcExecutablePathAndDirectoryFields6">
      <td class="left_column">Polling time<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.PollTime) ?>
         <?cs else ?>
            <input class="number_field" type="text" name="SrcPollTime" id="SrcPollTimeInput"
               value="<?cs var:source.PollTime ?>" />
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction('SrcPollTimeInput', 'SrcExecutablePathAndDirectoryFields6', 'SrcPollTimeErrorMessageContainer', null, showSourceTab);
            </script>
         <?cs /if ?>
         milliseconds
         <span id="SrcPollTimeErrorMessageContainer" class="validation_error_message_container"></span>
         <?cs if:Channel.Source.Error.PollTime ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.PollTime ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
  
   
   <tr class="selected" id="SrcExecutablePathAndDirectoryFields7">
      <td class="left_column">Plugin timeout<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.RpcTimeOut) ?>
         <?cs else ?>
            <input class="number_field" type="text" name="SrcRpcTimeOut" id="SrcRpcTimeOutInput"
               value="<?cs var:source.RpcTimeOut ?>" />
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction('SrcRpcTimeOutInput', 'SrcExecutablePathAndDirectoryFields7', 'SrcRpcTimeOutErrorMessageContainer', null, showSourceTab);
            </script>
         <?cs /if ?>
         milliseconds
         <span id="SrcRpcTimeOutErrorMessageContainer" class="validation_error_message_container"></span>
         <?cs if:Channel.Source.Error.RpcTimeOut ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.RpcTimeOut ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <?cs if:Channel.ReadOnlyMode ?>
      <tr id="SrcExecutablePathAndDirectoryFields8" <?cs if:source.AutoRun ?>style="display:none"<?cs /if ?> class="selected">  
         <td class="left_column">Diagnostic tools</td>
         <td class="inner_left" colspan="3">
            <a href="plugin_executable_output?Channel=<?cs var:url_escape(Channel.Name) ?>&Component=Source">Monitor executable output</a>
         </td>
      </tr>
   <?cs /if ?>

<?cs /with ?>
