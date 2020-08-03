<?cs # vim: set syntax=html :?>

<?cs include:"browse_macro.cs" ?>

<script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>

<script defer type="text/javascript"> 

$(document).ready(function() {
   var Options = $('#DstEscapeOptions');
   $('#DstEscape').click(function() {
      if($(this).attr('checked'))
         Options.show();
      else
         Options.hide();
   });
});

var LastPreview = '';

//this helps set up default port values when the protocol changes
var onFtpProtocolChanged = function()
{
   var OldPorts = { 'ftp' : '21', 'ftps' : '990', 'sftp' : '22' };
   var OldProtocol = '<?cs var:Channel.Destination.FtpProtocol ?>';

   return function()
   {
      var NewProtocol = $('#DstFtpProtocol').val();
      if (NewProtocol != OldProtocol)
      {
         OldPorts[OldProtocol] = $('#DstFtpPort').val();
         $('#DstFtpPort').val(OldPorts[NewProtocol]);
         OldProtocol = NewProtocol;

         // Update the help string for the FTP path
         var HelpIcon = document.getElementById('to_ftp_path_help_Icon');
         if (HelpIcon) 
         {
            HelpIcon.rel = ftpToPathBoxHelpString (NewProtocol);
         }
      }
   }
}();

function setRowsDisplay(ClassName, DisplayStyle)
{
   var ToFileTableBody = document.getElementById('destinationTabContents');
   var ToFileRows = ToFileTableBody.getElementsByTagName('tr');
   
   for (var RowIndex = 0; RowIndex < ToFileRows.length; ++RowIndex)
   {
      if (CLSelementHasClass(ToFileRows[RowIndex], ClassName))
      {
         ToFileRows[RowIndex].style.display = DisplayStyle;
      }
   }
}

function writeToSeparateFileChanged()
{
   var WriteToSeparateFileInput = document.getElementById('DstWriteToSeparateFile');
   var RowDisplayStyle = document.getElementById('trDstWriteMessageToSeparateFile').style.display;
   
   if (WriteToSeparateFileInput.value == "on")
   {
      // separate files
      setRowsDisplay('DstSingleFileRow',    'none');
      setRowsDisplay('DstSeparateFilesRow', RowDisplayStyle);
   }
   else
   {
      // the same file
      setRowsDisplay('DstSingleFileRow',    RowDisplayStyle);
      setRowsDisplay('DstSeparateFilesRow', 'none');
   }
   
   onFileIdTypeChange();
}

function toggleFtpNumberOfReconnectsTextField()
{
   var UseFtpInput = document.getElementById('DstUseFtp');
   if (UseFtpInput.checked)
   {
      var ReconnectOptionsComboBox = document.channel_data.DstFtpMaxReconnectsChoiceBox;
      if (ReconnectOptionsComboBox)
      {
         var ReconnectsText = document.getElementById('DstFtpMaxReconnectsText');
         var ReconnectIntervalRow = document.getElementById('DstFtpReconnectIntervalRow');
   
         if(ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value == "other")
         {
	    ReconnectsText.style.display="inline";
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
      else
      {
	//in read only mode
      	<?cs if:!Channel.Destination.FtpUnlimitedMaxReconnects && (Channel.Destination.FtpMaxReconnects == 0) ?>
      	$('#DstFtpReconnectIntervalRow').hide();
      	<?cs else ?>
      	$('#DstFtpReconnectIntervalRow').show();
      	<?cs /if ?>
      }
   }
   else
   {
     $('#DstFtpReconnectIntervalRow').hide(); 
   }
}

function secureDstShellGetHostFingerPrint( idToFill, idError, idSpinner ){
   var HostName = $('input[name=DstFtpServer]').val();
   var Port = $('input[name=DstFtpPort]').val();
   $('#'+idError).html('');  
   $('#'+idSpinner).empty().append('<img src="/images/spinner.gif" style="width:15px; height: 15px;">');
   AJAXpostWithId('get_ssh_fingerprint', 'hostname=' + encodeURIComponent(HostName) + '&port=' + encodeURIComponent(Port),
       'ssh host id fingerprint ajax call',
       function(data) {
         var Result = eval('(' + data +')' );
         $('#'+idSpinner).empty();
         $('#'+idError).empty();  
	 if (Result.Success){
	    $('#'+idToFill).attr('value',Result.Result);
         }
	 else{
            var ErrorMsg = $('<div class="configuration_error"></div>').text(Result.Result);
            $('#'+idError).empty().append(ErrorMsg);
	 }
   },
   null);
}

function useFtpChanged()
{
   var FtpProtocol = document.getElementById('DstFtpProtocol').value;
   var UseFtpInput = document.getElementById('DstUseFtp');
   var FtpSslVerifyPeerInput = document.getElementById('DstFtpSslVerifyPeer');
   var RowDisplayStyle = document.getElementById('trDstUseFtp').style.display;
   var ShellPasswordAuth = document.getElementById('DstFtpSecureShellPasswordAuthentication').value;
   var ShellVerifyHost = document.getElementById('DstFtpSecureShellVerifyHostFingerprint').checked;

   if (UseFtpInput.checked)
   {
      setRowsDisplay('DstUseFtpRow', RowDisplayStyle);
      //check based on protocol
      setRowsDisplay('DstUseFtpSslRow', (FtpProtocol == 'ftps' ? '' : 'none'));
      setRowsDisplay('DstUseSecureShellFtpRow', (FtpProtocol == 'sftp' ? '' : 'none'));
      
      //password inactive for sftp, unless using password auth
      var PasswordDisplay = (FtpProtocol != 'sftp' || ShellPasswordAuth == 'user_password');
      setRowsDisplay('DstFtpPasswordRow', (PasswordDisplay ? '' : 'none'));
      if (FtpProtocol == 'sftp'){
	 setRowsDisplay('DstSecureShellFtpKeyAuth', (PasswordDisplay ? 'none' : ''));
	 setRowsDisplay('DstSecureShellVerifyHostFingerprint', (ShellVerifyHost ? '' : 'none'));
      }

      //cert authority displayed only if ftps and verify peer
      setRowsDisplay('DstFtpCertificateAuthorityRow',( (FtpSslVerifyPeerInput.checked && FtpProtocol == 'ftps') ? '' : 'none'));
   }
   else
   {
      //hide everything
      setRowsDisplay('DstUseFtpRow', 'none');
      setRowsDisplay('DstUseFtpSslRow', 'none');
      setRowsDisplay('DstUseSecureShellFtpRow', 'none');
   }
}

function showOrHidePaddedDigit()
{
   var PaddedDigitInputRow = document.getElementById('trCountOfPaddedDigit');
   var WriteToSeparateFileInput = document.getElementById('DstWriteToSeparateFile');
   var FileIdType = document.getElementById('DstFileIdType');
   
   if (WriteToSeparateFileInput.value == "on" &&
       FileIdType.value == "PaddedIndex")
   {
      var RowDisplayStyle = document.getElementById('trDstWriteMessageToSeparateFile').style.display;
      PaddedDigitInputRow.style.display = RowDisplayStyle;
   }
   else
   {
      PaddedDigitInputRow.style.display = 'none';
   }
}

function showOrHideNextFileIndex()
{
   var NextFileIndexRow = document.getElementById('trDstNextFileIndex');
   var WriteToSeparateFileInput = document.getElementById('DstWriteToSeparateFile');
   var FileIdType = document.getElementById('DstFileIdType');
   
   if (WriteToSeparateFileInput.value == "on" &&
       (FileIdType.value == "PaddedIndex" || FileIdType.value == "IntegerIndex"))
   {
      var RowDisplayStyle = document.getElementById('trDstWriteMessageToSeparateFile').style.display;
      NextFileIndexRow.style.display = RowDisplayStyle;
   }
   else
   {
      NextFileIndexRow.style.display = 'none';
   }
}

var CustomIdSelected = false;

function initCustomTimestamp()
{
   var WriteToSeparateFileInput = document.getElementById('DstWriteToSeparateFile');
   var FileIdType = document.getElementById('DstFileIdType');
   
   if (WriteToSeparateFileInput.value == "on" &&
       (FileIdType.value == "CustomId"))
   {
    CustomIdSelected = true;

    <?cs if:!Channel.ReadOnlyMode ?>
    document.getElementById("DstOutputFilenameMask_Icon").rel =  "The output file mask must contain '%Y' (year), '%m' (month), '%d' (day), '%H' (hour), '%M' (minute), '%S' (second) and '%f' (millisecond)."
    <?cs /if ?>
   }
}

function showOrHideSampleMaskForCustomTimestamp()
{
   var WriteToSeparateFileInput = document.getElementById('DstWriteToSeparateFile');
   var FileIdType = document.getElementById('DstFileIdType');
   
   if (WriteToSeparateFileInput.value == "on" &&
       (FileIdType.value == "CustomId"))
   {
      if(!CustomIdSelected)
      {
         document.getElementById("DstOutputFilenameMask").value = "output_%Y_%m_%d_%H_%M_%S_%f.hl7";
      }

      <?cs if:!Channel.ReadOnlyMode ?>
      document.getElementById("DstOutputFilenameMask_Icon").rel =  "The output file mask must contain '%Y' (year), '%m' (month), '%d' (day), '%H' (hour), '%M' (minute), '%S' (second) and '%f' (millisecond)."
      <?cs /if ?>

      CustomIdSelected = true;
   }
   else
   {
      if(CustomIdSelected)
      {
         document.getElementById("DstOutputFilenameMask").value = "output_%i.hl7";
      }

      <?cs if:!Channel.ReadOnlyMode ?>
      document.getElementById("DstOutputFilenameMask_Icon").rel = "The output file mask must contain '%i', which represents the file ID selected above."
      <?cs /if ?>

      CustomIdSelected = false;
   }
}

function onFileIdTypeChange()
{
   showOrHidePaddedDigit();
   showOrHideNextFileIndex();
   showOrHideSampleMaskForCustomTimestamp();
   updatePreview();
}

function generatePreview(FilenameMask, FileIdType)
{
   var Preview;
   var FileId;
   
   if (FileIdType == "UniqueId")
   {
      FileId = '20080919123345_00002';
   }
   else if (FileIdType == "TimeStamp")
   {
      FileId = '20080919123345027';
   }
   else if (FileIdType == "PaddedIndex")
   {
      var CountOfDigitInput = document.getElementById('DstCountOfPaddedDigit');
      FileId = document.getElementById('DstNextFileIndex').value;
      if ( FileId.length > CountOfDigitInput.value )
      {
         FileId = '2';
      }

      while ( FileId.length < CountOfDigitInput.value )
      {
         FileId = '0' + FileId;
      }
   }
   else
   {
      FileId = document.getElementById('DstNextFileIndex').value;
   }
   
   Preview = FilenameMask.replace('%i', FileId);
   
   // Highlight errors in mask
   Preview = Preview.replace(/[^\.%\w ]|%i/g, '<span class="invalidFilenameMaskCharacter">$&</span>');
   
   return 'Preview: "' + Preview + '"';
}

function updatePreview()
{
   var PreviewSpan = document.getElementById("filenamePreview");
   var PreviewWrapDiv = document.getElementById('filenamePreview_div');

   var FilenameMask = document.getElementById('DstOutputFilenameMask').value;
   var FileIdType   = document.getElementById('DstFileIdType').value;
      
   if("CustomId" != FileIdType)
   {      
      if (FilenameMask != '')
      {  
         var NewPreview = generatePreview(FilenameMask, FileIdType);
             
         if (NewPreview != LastPreview)
         {
            PreviewSpan.innerHTML = NewPreview;
            LastPreview = NewPreview;
         }
      
         PreviewSpan.style.display = '';
         PreviewWrapDiv.style.display = '';
      }
      else
      {
         PreviewSpan.style.display = 'none';
         PreviewWrapDiv.style.display = 'none';
      }
   }
   else
   {                
      if (FilenameMask != '')
      {    
         expandDate(FilenameMask, PreviewSpan);
           
         PreviewSpan.style.display = '';
         PreviewWrapDiv.style.display = '';
      }
      else
      {
         PreviewSpan.style.display = 'none'; 
         PreviewWrapDiv.style.display = 'none';
      }   
   }
}   

function updateToFileDate()
{

   <?cs if:!Channel.ReadOnlyMode ?>
      var Input = document.getElementsByName('DstSingleOutputFilename')[0];
      var Preview = document.getElementById('DstSingleOutputFilename_path_preview');
      var PreviewDiv = document.getElementById('DstSingleOutputFilename_path_preview_div');
      updateDate('Tofile', Input, Preview, PreviewDiv);
   <?cs /if ?>

}

function onLoadToFile()
{
   initCustomTimestamp();

   writeToSeparateFileChanged();
   useFtpChanged();
   toggleFtpNumberOfReconnectsTextField(); //must be after useFtpChanged
   setTimeout(updatePreview, 500);
   updateToFileDate();

   // Update the help string for the FTP path
   var HelpIcon = document.getElementById('to_ftp_path_help_Icon');
   if (HelpIcon) 
   {
      HelpIcon.rel = ftpToPathBoxHelpString ('<?cs var:Channel.Destination.FtpProtocol ?>');
   }
}

function ftpToPathBoxHelpString (FtpProtocol)
{
    var HelpString = "Location of the directory on the FTP server into which the generated files are to be written.";
    if (FtpProtocol == 'sftp')
    {
        HelpString = HelpString + "<br><br> The FTP path field must contain the full path of the directory from which files are to be read.";
    }
    else
    {
        HelpString = HelpString + "<br><br> The directory specified in the FTP path field is relative to the home directory for the FTP user."; 
    }
    return HelpString;
}

</script>

<?cs with: dest = Channel.Destination ?>
   <tr class="channel_configuration_section_heading_row">
      <td>
         <table>
            <tr>
               <td style="width:100%;"><hr /></td>
               <td class="channel_configuration_section_heading">File Options</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>
   
   <tr class="selected">
      <td class="left_column first_row">Destination directory<font color="#ff0000">*</font></td>
      <td class="inner_left first_row" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:dest.OutputDirectory != "" ?>
                     <?cs var:html_escape(dest.OutputDirectory) ?>
                     <?cs if:dest.OutputDirectory != path_expand(dest.OutputDirectory) ?>
                           <div id="DstOutputDirectory_path_preview_static" class="path_preview"> Absolute Path: "<?cs var:html_escape(path_expand(dest.OutputDirectory)) ?>"</div>
                     <?cs /if ?>
            <?cs /if ?>
         <?cs else ?>
            <?cs call:browse_input_folder('DstOutputDirectory', dest.OutputDirectory) ?>  
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.OutputDirectory ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.OutputDirectory ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
   
     
   <tr id="trDstWriteMessageToSeparateFile" class="selected">
      <td class="left_column">Write each message to</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            
            <?cs if:dest.WriteToSeparateFile ?>
               Separate files
               <input type="hidden" id="DstWriteToSeparateFile" name="DstWriteToSeparateFile" value="on">
            <?cs else ?>
               A single file
               <input type="hidden" id="DstWriteToSeparateFile" name="DstWriteToSeparateFile" value="off">
            <?cs /if ?>
         <?cs else ?>
            <select id="DstWriteToSeparateFile" name="DstWriteToSeparateFile" onchange="writeToSeparateFileChanged();" onkeyup="writeToSeparateFileChanged();" onclick="writeToSeparateFileChanged();">
               <?cs if:dest.WriteToSeparateFile ?>
                  <option value="off">A single file</input>
                  <option value="on" selected>Separate files</input>
               <?cs else ?>
                  <option value="off" selected>A single file</input>
                  <option value="on">Separate files</input>
               <?cs /if ?>
            </select>
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.WriteToSeparateFile ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.WriteToSeparateFile ?>
            </div>
         <?cs /if ?>
              
      </td>
   </tr>
   
   
   <tr class="DstSingleFileRow selected">
      <td class="left_column">File<font color="#ff0000">*</font></td>
      <td class="inner_left">
        <?cs if:Channel.ReadOnlyMode ?>         
            <?cs if:dest.SingleOutputFilename != "" ?>
                     <?cs var:html_escape(dest.SingleOutputFilename) ?>
                     <?cs if:dest.SingleOutputFilename != dest.SingleOutputFilenamePreview ?>
                           <div id="DstSingleOutputFilename_path_preview_static" class="path_preview"> Preview: "<?cs var:html_escape(dest.SingleOutputFilenamePreview) ?>"</div>
                     <?cs /if ?>
            <?cs /if ?>                        
         <?cs else ?>                         
                  <input type="text" class="configuration_long" onchange="updateToFileDate();" onkeyup="updateToFileDate();" name="DstSingleOutputFilename" value="<?cs var:html_escape(dest.SingleOutputFilename) ?>" />
                  <a id="DstSingleOutputFilename_Icon" class="helpIcon" tabindex="100" rel="Single file names support rollover functionality. To achieve this date and/or time specifiers must be included in the file name. Consult the <a href='<?cs var:help_link('iguana4_to_file_format_specifiers') ?>' target='_blank'>Iguana manual</a> for a complete list of available options.<br><br>For example, to rollover output files over every day, a day ('%d'), a month ('%m') and year ('%Y') specifier must be included in the file name, e.g., 'output_%m%d%Y.txt'.<br><br>Note:  failure to include the appropriate specifiers can lead to file name collisions. In these cases, a warning message will be generated when the channel configuration is saved. Follow the directions given in the warning message to correct the file name." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a> 
                  <div id="DstSingleOutputFilename_path_preview_div" style="display:inline;">
                     <div id="DstSingleOutputFilename_path_preview" class="path_preview" val="<?cs var:html_escape(dest.SingleOutputFilenamePreview) ?>"></div>
                  </div>          
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.SingleOutputFilename ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.SingleOutputFilename ?>
            </div>
         <?cs /if ?>
         <?cs if:Channel.Destination.Warning.SingleOutputFilename ?>
            <div class="configuration_warning">
                     <img src="/<?cs var:skin("images/icon_warning.gif") ?>" style="position:relative; top:-1px;"/>
                     <span style="position:relative; top:-3px;"><b>Warning:</b> <?cs var:Channel.Destination.Warning.SingleOutputFilename ?></span>
            </div>
         <?cs /if ?>                     
      </td>
   </tr>
   
   <tr class="DstSeparateFilesRow selected">
      <td class="left_column">Use as file ID</td>
      <td class="inner_left">
         <?cs if:Channel.ReadOnlyMode ?>
            <input type="hidden" id="DstFileIdType" name="DstFileIdType" value="<?cs var:dest.FileIdType ?>">
       
       
            <?cs if:dest.FileIdType == "UniqueId"     ?>Unique ID <span class="configurationFillerText">(YYYYMMDDhhmmss_XXXXX)</span><?cs /if ?>
            <?cs if:dest.FileIdType == "TimeStamp"    ?>Timestamp <span class="configurationFillerText">(to the millisecond)</span><?cs /if ?>
            <?cs if:dest.FileIdType == "CustomId"     ?>Custom Timestamp<?cs /if ?>
            <?cs if:dest.FileIdType == "IntegerIndex" ?>Integer index<?cs /if ?>
            <?cs if:dest.FileIdType == "PaddedIndex"  ?>Padded integer index<?cs /if ?>
         <?cs else ?>
            <select id="DstFileIdType" name="DstFileIdType" onchange="onFileIdTypeChange();" onkeyup="onFileIdTypeChange();">
               <option value="UniqueId"     <?cs if:dest.FileIdType == "UniqueId"     ?>selected<?cs /if ?>>Unique ID (YYYYMMDDhhmmss_XXXXX)</option>
               <option value="TimeStamp"    <?cs if:dest.FileIdType == "TimeStamp"    ?>selected<?cs /if ?>>Timestamp (to the millisecond)  </option>
               <option value="CustomId"     <?cs if:dest.FileIdType == "CustomId"     ?>selected<?cs /if ?>>Custom Timestamp                </option>
               <option value="IntegerIndex" <?cs if:dest.FileIdType == "IntegerIndex" ?>selected<?cs /if ?>>Integer index                   </option>
               <option value="PaddedIndex"  <?cs if:dest.FileIdType == "PaddedIndex"  ?>selected<?cs /if ?>>Padded integer index            </option>
            </select>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="DstSeparateFilesRow selected" id="trCountOfPaddedDigit" style="display: none;">
      <td class="left_column">Number of padded digits</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <input type="hidden" id="DstCountOfPaddedDigit" name="DstCountOfPaddedDigit" value="<?cs var:dest.CountOfPaddedDigit ?>">
            <?cs var:html_escape(dest.CountOfPaddedDigit) ?>
         <?cs else ?>
            <select id="DstCountOfPaddedDigit" name="DstCountOfPaddedDigit" onchange="updatePreview();" onkeyup="updatePreview();">

               <?cs loop:digitCountIndex = #1, #20, #1 ?>
                  <option value="<?cs var:digitCountIndex ?>"
                     <?cs if:dest.CountOfPaddedDigit == digitCountIndex ?>selected<?cs /if ?>>
                     <?cs var:digitCountIndex ?>
                  </option>
               <?cs /loop ?>
            </select>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="DstSeparateFilesRow selected" id="trDstNextFileIndex" style="display: none;">
      <td class="left_column">
         Next file index<font color="#ff0000">*</font>
      </td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner" style="width: 400px;">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(dest.NextFileIndex) ?>
                     <input type="hidden" id="DstNextFileIndex" name="DstNextFileIndex" value="<?cs var:html_escape(dest.NextFileIndex) ?>" />
                  <?cs else ?>
                     <input name="DstNextFileIndex" id="DstNextFileIndex"
                        onchange="updatePreview();"
                        onkeyup="updatePreview();"
                        style="width: 150px;"
                        value="<?cs var:html_escape(dest.NextFileIndex) ?>" />
                     <span id="DstNextFileIndexErrorMessageContainer" class="validation_error_message_container"></span>
                     <script defer type="text/javascript">
                        VALregisterIntegerValidationFunction
                        (
                           'DstNextFileIndex', 'trDstNextFileIndex', 'DstNextFileIndexErrorMessageContainer', 
                           function()
                           {
                              var WriteToSeparateFileInput = document.getElementById('DstWriteToSeparateFile');
                              var FileIdType = document.getElementById('DstFileIdType');
                              return WriteToSeparateFileInput.value == "on" && (FileIdType.value == "PaddedIndex" || FileIdType.value == "IntegerIndex");
                           },
                           showDestinationTab, 0
                        );
                     </script>
                  <?cs /if ?>
               </td>
            </tr>
         </table>
         <?cs if:Channel.Destination.Error.NextFileIndex ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.NextFileIndex ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
      
   <tr class="DstSeparateFilesRow selected">
      <td class="left_column">
         Output file mask<font color="#ff0000">*</font><br />
      </td>
      <td class="inner_left" colspan="3">
         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(dest.OutputFilenameMask) ?>
                     <input type="hidden" id="DstOutputFilenameMask" name="DstOutputFilenameMask" value="<?cs var:html_escape(dest.OutputFilenameMask) ?>" />
                  <?cs else ?>
                        <input id="DstOutputFilenameMask" class="configuration_long" onchange="updatePreview();" onkeyup="updatePreview();" name="DstOutputFilenameMask" value="<?cs var:html_escape(dest.OutputFilenameMask) ?>" />
                        <a id="DstOutputFilenameMask_Icon" class="helpIcon" tabindex="100" rel="In the Output file mask field, type a file mask that will be used to create the file names for the output messages. This mask must contain %i, which represents the file identifier specified by the Use as file id list box.<br><br>For example, if the file id for a message is the index number 28, and the output file mask is output_%i.hl7, the output file name for this message is output_28.hl7" title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a> 
                  <?cs /if ?>
                  <div id="filenamePreview_div" style="display:none;">
                  <div id="filenamePreview" class="path_preview"></div>
                  </div>
               </td>          
            </tr>
         </table>
         <?cs if:Channel.Destination.Error.OutputFilenameMask ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.OutputFilenameMask ?>
            </div>
         <?cs /if ?>                  
      </td>
   </tr>
   
   
   <tr class="DstSeparateFilesRow selected">
      <td class="left_column">Temporary file extension</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.TemporaryFileExtension) ?>
         <?cs else ?>
           <input class="configuration_smaller" name="DstTemporaryFileExtension" value="<?cs var:html_escape(dest.TemporaryFileExtension) ?>" />
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.TemporaryExtension ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.TemporaryExtension ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected">
      <td class="left_column">Output file encoding</td>
      <td class="inner" colspan="3">
         <table class="inner" Style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if: Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(dest.Encoding.Description) ?>
                     <br><div style="padding:3px"></div>
                     <?cs if:!dest.Encoding.EscapeNonAscii ?>
                        8-bit characters will not be escaped
                     <?cs elif:!dest.Encoding.EscapeChar ?>
                        8-bit characters of HL7 messages will be escaped
                     <?cs else ?>
                        8-bit characters will be escaped with
                        "<?cs var:html_escape(dest.Encoding.EscapeChar) ?>"
                     <?cs /if ?>
                  <?cs else ?>
                     <select name="DstEncoding">
                        <option value="<?cs var:html_escape(dest.Encoding.Name) ?>" selected>
                           <?cs var:html_escape(dest.Encoding.Description) ?>
                        </option>
                        <option value="<?cs var:html_escape(dest.Encoding.Name) ?>">
                           ---
                        </option>
                        <?cs each:encoding = AllEncodings ?>
                           <?cs if: encoding.Name        != dest.Encoding.Name
                                 || encoding.Description != dest.Encoding.Description ?>
                              <option value="<?cs var:html_escape(encoding.Name) ?>">
                                 <?cs var:html_escape(encoding.Description) ?>
                              </option>
                           <?cs /if ?>
                        <?cs /each ?>
                     </select>
                     <br>
                     <input type="checkbox" id="DstEscape" name="DstEscape"
                        <?cs if:dest.Encoding.EscapeNonAscii ?>checked<?cs /if ?>>
                     <label for="DstEscape">Escape 8-bit characters (HL7)</label>
                     <a class="helpIcon" style="margin-left:5px; position:relative;"
                        title="More Information" href="#" rel="When writing messages out,
                        non-ASCII characters, like accented characters, can be escaped.
                        E.g., <nobr>&quot;&#233;&quot;</nobr> would be sent as
                        <nobr>&quot;\XE9\&quot;</nobr> (in <nobr>Latin-1</nobr> or
                        <nobr>Windows-1252</nobr>) or <nobr>&quot;\XC3\\XA9\&quot;</nobr>
                        (in <nobr>UTF-8</nobr>)."
                        ><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0"></a>
                     <div id="DstEscapeOptions" style="margin-left:2em; 
                              <?cs if:!dest.Encoding.EscapeNonAscii ?>display:none;<?cs /if ?>">
                        <input type="radio" name="DstEscapeMethod" id="DstEscapeAuto" value="auto"
                           <?cs if:!dest.Encoding.EscapeChar ?>checked<?cs /if ?>>
                        <label for="DstEscapeAuto">Automatically detect escape character</label>
                        <br>
                        <input type="radio" name="DstEscapeMethod" id="DstEscapeSpec" value="spec"
                           <?cs if: dest.Encoding.EscapeChar ?>checked<?cs /if ?>>
                        <label for="DstEscapeSpec">Always escape with
                           <input name="DstEscapeChar" size="1"
                                  value="<?cs alt:dest.Encoding.EscapeChar ?>\<?cs /alt ?>"></label>
                     </div>
                  <?cs /if ?>
               </td>               
               <td class="inner_right">
                  <?cs if:Channel.Destination.Error.Encoding ?>
                     <div class="configuration_error">
                        <ul class="configuration"><?cs var:Channel.Destination.Error.Encoding ?></ul>
                     </div>
                  <?cs /if ?>
                  <?cs if:Channel.Destination.Error.Escaping ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Destination.Error.Escaping ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>
         </table>
      </td>
   </tr>

   <tr class="channel_configuration_section_heading_row">
      <td>
         <table>
            <tr>
               <td style="width:100%;"><hr /></td>
               <td class="channel_configuration_section_heading">FTP Options</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>
   
   <tr id="trDstUseFtp" class="selected">
      <td class="left_column first_row"><label for="DstUseFtp">Upload to FTP</label></td>
      <td class="inner first_row" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner" style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if: Channel.ReadOnlyMode ?>
                     <?cs if: dest.UseFtp ?>
                        Yes
                     <?cs else ?>
                        No
                     <?cs /if ?>
                     <input type="checkbox" style="display:none" id="DstUseFtp" name="DstUseFtp" <?cs if: dest.UseFtp ?>checked="checked" <?cs /if?>/>
                  <?cs else ?>
                     <input type="checkbox" id="DstUseFtp" name="DstUseFtp" onchange="useFtpChanged();" onkeyup="useFtpChanged();" onclick="useFtpChanged();" <?cs if: dest.UseFtp ?>checked="checked" <?cs /if?>/>
                  <?cs /if ?>
               </td>
            </tr>
         </table>
      </td>
   </tr>

   <tr class="selected DstUseFtpRow">

       <?cs def:print_ftp_protocol(name) ?>
           <?cs if: name == "ftp" ?>FTP
	   <?cs elif: name=="ftps" ?>FTPS (FTP over SSL)
	   <?cs elif: name=="sftp" ?>SFTP (Secure Shell FTP)	      
	   <?cs else ?><?cs var:name ?>
	   <?cs /if ?>
       <?cs /def ?>

      <td class="left_column">FTP protocol</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?> 
		     <?cs call:print_ftp_protocol(dest.FtpProtocol) ?>
                     <input type="hidden" id="DstFtpProtocol" name="DstFtpProtocol" value="<?cs var:dest.FtpProtocol ?>"/>
                  <?cs else ?>
		     <select name="DstFtpProtocol" id="DstFtpProtocol" 
		        onchange="useFtpChanged(); onFtpProtocolChanged();" 
			onkeyup="useFtpChanged(); onFtpProtocolChanged();" 
			onclick="useFtpChanged(); onFtpProtocolChanged();">
		        <option value="ftp" <?cs if:dest.FtpProtocol == "ftp" ?>selected<?cs /if ?> ><?cs call:print_ftp_protocol('ftp') ?></option>
		        <option value="ftps" <?cs if:dest.FtpProtocol == "ftps" ?>selected<?cs /if ?> ><?cs call:print_ftp_protocol('ftps') ?></option>
		        <option value="sftp" <?cs if:dest.FtpProtocol == "sftp" ?>selected<?cs /if ?> ><?cs call:print_ftp_protocol('sftp') ?></option>
		     </select>
                  <?cs /if ?>
               </td>
               <td class="inner_right">
                  <?cs if:Channel.Destination.Error.Ftp ?>
                     <div class="configuration_error">
                        <ul class="configuration"><?cs var:Channel.Destination.Error.Ftp ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
               <?cs if:!Channel.ReadOnlyMode ?>
                <td>
                  <a id="configuration_smaller_Icon" class="helpIcon" tabindex="100" rel="<ul><li>FTP (standard FTP): not secured by encryption</li><li>FTPS: encrypted FTP using SSL</li><li>SFTP: uses SSH to encrypt file transfer communications</li></ul>" title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                </td>
               <?cs /if ?>
            </tr>     
         </table>
      </td>
   </tr>

   <tr class="selected DstUseSecureShellFtpRow" >
      <td class="left_column">Authentication</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:dest.FtpSecureShellPasswordAuthentication ?>
	    <input type="hidden" name="DstFtpSecureShellPasswordAuthentication" id="DstFtpSecureShellPasswordAuthentication" value="user_password">
	    Username/Password
	    <?cs else ?>
	    <input type="hidden" name="DstFtpSecureShellPasswordAuthentication" id="DstFtpSecureShellPasswordAuthentication" value="private_public_key">
	    Private/Public Key
	    <?cs /if ?>
         <?cs else ?>
	    <select onchange="useFtpChanged()" id="DstFtpSecureShellPasswordAuthentication" name="DstFtpSecureShellPasswordAuthentication">
	    <option value="user_password" <?cs if:dest.FtpSecureShellPasswordAuthentication ?>selected<?cs /if ?> >Username/Password</option>
	    <option value="private_public_key" <?cs if:!dest.FtpSecureShellPasswordAuthentication ?>selected<?cs /if ?> >Private/Public Key</option>
	    </select>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected DstUseFtpRow">
      <td class="left_column">FTP server<font color="#ff0000">*</font></td>
      <td class="inner" colspan="3">
         <?cs if: Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.FtpServer) ?>
            <?cs if:Channel.Destination.Error.FtpServer ?>
               <div class="configuration_error">
                  <?cs var:Channel.Destination.Error.FtpServer ?>
               </div>
            <?cs /if ?>
         <?cs else ?>
            <table class="inner" style="float:left">
               <tr>
                  <td class="inner_left">
                     <input type="text" name="DstFtpServer" value="<?cs var:html_escape(dest.FtpServer) ?>" />
                     <?cs if:Channel.Destination.Error.FtpServer ?>
                        <div class="configuration_error">
                           <?cs var:Channel.Destination.Error.FtpServer ?>
                        </div>
                     <?cs /if ?>
                  </td>
               </tr>
            </table>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected DstUseFtpRow" id="DstFtpRow">
      <td class="left_column">FTP port<font color="#ff0000">*</font></td>
      <td class="inner" colspan="3">
         <?cs if: Channel.ReadOnlyMode ?>
            <?cs alt:html_escape(dest.FtpPort) ?><?cs /alt ?>
            <?cs if:Channel.Destination.Error.FtpPort ?>
               <div class="configuration_error">
                  <?cs var:Channel.Destination.Error.FtpPort ?>
               </div>
            <?cs /if ?>
         <?cs else ?>
            <table class="inner" style="float:left">
               <tr>
                  <td class="inner_left">
                     <input type="text" class="number_field" name="DstFtpPort" id="DstFtpPort" value="<?cs alt:html_escape(dest.FtpPort) ?><?cs /alt ?>" size="5" />
					 <script defer type="text/javascript">
                        VALregisterIntegerValidationFunction("DstFtpPort", "DstFtpRow",
                           "DstFtpErrorMessageContainer", null, showDestinationTab, 1, 65535);
                     </script>
					 <span id="DstFtpErrorMessageContainer"
					    class="validation_error_message_container"
					    style="white-space:normal">
					 </span>
                     <?cs if:Channel.Destination.Error.FtpPort ?>
                        <div class="configuration_error">
                           <?cs var:Channel.Destination.Error.FtpPort ?>
                        </div>
                     <?cs /if ?>
                  </td>
               </tr>
            </table>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected DstUseFtpRow">
      <td class="left_column">FTP username<font color="#ff0000">*</font></td>
      <td class="inner" colspan="3">
         <?cs if: Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.FtpUsername) ?>
            <?cs if:Channel.Destination.Error.FtpUsername ?>
               <div class="configuration_error">
                  <?cs var:Channel.Destination.Error.FtpUsername ?>
               </div>
            <?cs /if ?>
         <?cs else ?>
            <table class="inner" style="float:left">
               <tr>
                  <td class="inner_left">
                     <input type="text" name="DstFtpUsername" value="<?cs var:html_escape(dest.FtpUsername) ?>" />
                     <?cs if:Channel.Destination.Error.FtpUsername ?>
                        <div class="configuration_error">
                           <?cs var:Channel.Destination.Error.FtpUsername ?>
                        </div>
                     <?cs /if ?>
                  </td>
               </tr>
            </table>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected DstUseFtpRow DstFtpPasswordRow">
      <td class="left_column">FTP password</td>
      <td class="inner" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner" style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if: Channel.ReadOnlyMode ?>
                     ******
                  <?cs else ?>
                     <input name="DstFtpPassword" type="password" value="<?cs var:html_escape(dest.FtpPassword) ?>" />
                  <?cs /if ?>
               </td>
            </tr>
         </table>
      </td>
   </tr>
   
   <tr class="selected DstUseFtpRow">
      <td class="left_column">FTP path</td>
      <td class="inner" colspan="3">
         <?cs if: Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.FtpPath) ?>
         <?cs else ?>
            <table class="inner" style="float:left">
               <tr>
                  <td class="inner_left">
                     <input type="text" name="DstFtpPath" value="<?cs var:html_escape(dest.FtpPath) ?>" />
                  </td>
  
                  <?cs if:!Channel.ReadOnlyMode ?>
                    <td>
                      <a id="to_ftp_path_help_Icon" class="helpIcon" tabindex="100" rel="..." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                    </td>
                  <?cs /if ?>
               </tr>
            </table>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected DstUseSecureShellFtpRow" >
      <td class="left_column">Verify host fingerprint</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode ?> 
            <?cs if:dest.FtpSecureShellVerifyHostFingerprint ?>Yes<?cs else ?>No<?cs /if ?>          
            <input type="checkbox" 
	           style="display: none;"
		   id="DstFtpSecureShellVerifyHostFingerprint" 
		   name="DstFtpSecureShellVerifyHostFingerprint" 
		   <?cs if:dest.FtpSecureShellVerifyHostFingerprint ?>checked<?cs /if ?>/>&nbsp;      
         <?cs else ?>
            <input type="checkbox" 
	           class="no_style" 
		   id="DstFtpSecureShellVerifyHostFingerprint" 
		   name="DstFtpSecureShellVerifyHostFingerprint" 
		   <?cs if:dest.FtpSecureShellVerifyHostFingerprint ?>checked<?cs /if ?> 
		   onchange="useFtpChanged();" onkeyup="useFtpChanged();" onclick="useFtpChanged();"/>&nbsp;      
           <a id="configuration_smaller_Icon" class="helpIcon" tabindex="100" 
            rel="The host fingerprint is a 32 character MD5 hash representing the unique identity of the SFTP server.  When transferring files to the SFTP server, if the fingerprint of the server that is being connected to does not match the fingerprint indicated in the channel configuration it will generate an error, thus providing a mechanism to guard against sending messages to an unauthorized machine." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected DstUseSecureShellFtpRow DstSecureShellVerifyHostFingerprint" >
      <td class="left_column">Host fingerprint<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?> 
            <?cs var:html_escape(dest.FtpSecureShellHostFingerprint) ?>
         <?cs else ?>
            <input id="DstFtpSecureShellHostFingerprint" name="DstFtpSecureShellHostFingerprint" type="text" size="40" value="<?cs var:html_escape(dest.FtpSecureShellHostFingerprint) ?>">
            <a onclick="javascript:secureDstShellGetHostFingerPrint('DstFtpSecureShellHostFingerprint','DstFtpSecureShellHostFingerprintError','DstFtpSecureShellHostFingerprintSpinner');">Get Fingerprint</a>
            <span id="DstFtpSecureShellHostFingerprintSpinner"></span>
            <span id="DstFtpSecureShellHostFingerprintError"></span>
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.FtpSecureShellHostFingerprint ?>
            <div class="configuration_error">
               <?cs var:html_escape(Channel.Destination.Error.FtpSecureShellHostFingerprint) ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected DstUseFtpSslRow" >
      <td class="left_column">Certificate file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.FtpSslCertificateKeyFile) ?>
            <?cs if:dest.FtpSslCertificateKeyFile!='' ?>
               <a href="/view_ssl_cert.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&ComponentType=Destination">view certificate</a>
            <?cs /if ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('DstFtpSslCertificateKeyFile', dest.FtpSslCertificateKeyFile) ?>
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.FtpSslCertificateKey ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.FtpSslCertificateKey ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected DstUseFtpSslRow" >
      <td class="left_column">Private key file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.FtpSslPrivateKeyFile) ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('DstFtpSslPrivateKeyFile', dest.FtpSslPrivateKeyFile) ?>
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.FtpSslPrivateKey ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.FtpSslPrivateKey ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected DstUseFtpSslRow" >
      <td class="left_column">Verify peer</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode ?> 
            <?cs if:dest.FtpSslVerifyPeer ?>Yes<?cs else ?>No<?cs /if ?>          
            <input type="checkbox" 
	           style="display: none;"
		   id="DstFtpSslVerifyPeer" 
		   name="DstFtpSslVerifyPeer" 
		   <?cs if:dest.FtpSslVerifyPeer ?>checked<?cs /if ?>/>&nbsp;      
         <?cs else ?>
            <input type="checkbox" 
	           class="no_style" 
		   id="DstFtpSslVerifyPeer" 
		   name="DstFtpSslVerifyPeer" 
		   <?cs if:dest.FtpSslVerifyPeer ?>checked<?cs /if ?> 
		   onchange="useFtpChanged();" onkeyup="useFtpChanged();" onclick="useFtpChanged();"/>&nbsp;      

           <a id="configuration_smaller_Icon" class="helpIcon" tabindex="100" 
           
            rel="To verify the authenticity of the server that is sending the messages, select the <span style='font-weight:bold'>Verify peer</span> check box." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected DstUseFtpSslRow DstFtpCertificateAuthorityRow" >
      <td class="left_column">Certificate authority file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.FtpSslCertificateAuthorityFile) ?>
            <?cs if:dest.FtpSslCertificateAuthorityFile!='' ?>
               <a href="/view_ssl_cert.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&CertificateType=Authority&ComponentType=Destination">view certificate</a>
            <?cs /if ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('DstFtpSslCertificateAuthorityFile', dest.FtpSslCertificateAuthorityFile) ?>
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.FtpSslCertificateAuthority ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.FtpSslCertificateAuthority ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected DstUseSecureShellFtpRow DstSecureShellFtpKeyAuth" >
      <td class="left_column">Private key file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.FtpSecureShellPrivateKeyFile) ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('DstFtpSecureShellPrivateKeyFile', dest.FtpSecureShellPrivateKeyFile) ?>
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.FtpSshPrivateKey ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.FtpSshPrivateKey ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected DstUseSecureShellFtpRow DstSecureShellFtpKeyAuth" >
      <td class="left_column">Public key file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.FtpSecureShellPublicKeyFile) ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('DstFtpSecureShellPublicKeyFile', dest.FtpSecureShellPublicKeyFile) ?>
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.FtpSshPublicKey ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.FtpSshPublicKey ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected DstUseFtpRow">
      <td class="left_column">Use remote temporary file</td>
      <td class="inner" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner" style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if: Channel.ReadOnlyMode ?>
                     <?cs if:dest.FtpUseRemoteTempFile ?>
                        Yes
                     <?cs else ?>
                        No
                     <?cs /if ?>
                  <?cs else ?>
                     <input name="DstFtpUseRemoteTempFile" type="checkbox" <?cs if:dest.FtpUseRemoteTempFile ?>checked="checked"<?cs /if ?> />
                  <?cs /if ?>
                  <?cs if:!Channel.ReadOnlyMode ?>
                    <td>
                      <a id="configuration_smaller_Icon" class="helpIcon" tabindex="100" rel="Select the <span style='font-weight:bold'>Use remote temporary file</span> check box to upload a temporary file, then rename it to the final file name when the upload is complete. This is useful for preventing remote systems from reading a partially uploaded file." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                    </td>
                  <?cs /if ?>                  
               </td>
            </tr>
         </table>
      </td>
   </tr>

   <tr class="selected DstUseFtpRow" id="DstFtpAttemptToReconnectRow">
      <td class="left_column">Attempt to reconnect?</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:!dest.FtpUnlimitedMaxReconnects && (dest.FtpMaxReconnects == 0) ?>No<?cs /if ?>
            <?cs if:dest.FtpUnlimitedMaxReconnects ?>Yes <span class="configurationFillerText">(unlimited)</span><?cs /if ?>
            <?cs if:!dest.FtpUnlimitedMaxReconnects && (dest.FtpMaxReconnects != 0) ?>Yes<span class="configurationFillerText">, with limit of</span><?cs /if ?>
         <?cs else ?>
            <select name="DstFtpMaxReconnectsChoiceBox" onchange="toggleFtpNumberOfReconnectsTextField();" onkeyup="toggleFtpNumberOfReconnectsTextField();">
               <option value="never" <?cs if:!dest.FtpUnlimitedMaxReconnects && (dest.FtpMaxReconnects == 0) ?>selected="selected"<?cs /if ?> >No</option>
               <option value="unlimited" <?cs if:dest.FtpUnlimitedMaxReconnects ?>selected="selected"<?cs /if ?> >Yes (unlimited)</option>
               <option value="other"  <?cs if:!dest.FtpUnlimitedMaxReconnects && (dest.FtpMaxReconnects != 0) ?>selected="selected"<?cs /if ?>>Yes, with limit</option>
            </select>
         <?cs /if ?>
         <span id="DstFtpMaxReconnectsText" style="font-size: 11px;<?cs if:dest.FtpUnlimitedMaxReconnects ?> display: none;<?cs /if ?><?cs if:dest.FtpMaxReconnects == 0 ?> display: none;<?cs /if ?>" >
            <?cs if:Channel.ReadOnlyMode ?>
               <?cs var:html_escape(dest.FtpMaxReconnects) ?><span>times</span>
            <?cs else ?>
               <span>of</span>
               <input class="number_field" type="text" name="DstFtpMaxReconnects" id="DstFtpMaxReconnectsInput"
                  value="<?cs var:dest.FtpMaxReconnects ?>" />
               <script defer type="text/javascript">
                  VALregisterIntegerValidationFunction
                  (
                     'DstFtpMaxReconnectsInput', 'DstFtpAttemptToReconnectRow', 'DstFtpAttemptToReconnectErrorMessageContainer',
                     function()
                     {
                        var ReconnectOptionsComboBox = document.channel_data.DstFtpMaxReconnectsChoiceBox;
                        return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value == "other";
                     },
                     showDestinationTab
                  );
               </script>
               <span>times</span>
            <?cs /if ?>
         </span>
         <?cs if:!Channel.ReadOnlyMode ?>
            &nbsp;
            <a id="configuration_smaller_Icon" class="helpIcon" tabindex="100" rel="This option specifies what action will be taken if the FTP connection is lost, or if the FTP server is busy." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
            <span id="DstFtpAttemptToReconnectErrorMessageContainer" class="validation_error_message_container"></span>
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.FtpMaxCountOfReconnect ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.FtpMaxCountOfReconnect ?>
            </div>
         <?cs /if ?>       
      </td>
   </tr>
   
   <tr class="selected DstUseFtpRow" id="DstFtpReconnectIntervalRow" style="<?cs if:!dest.FtpUnlimitedMaxReconnects && (dest.FtpMaxReconnects == 0) ?>display: none;<?cs /if ?>">
      <td class="left_column">Reconnection interval<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.FtpReconnectInterval) ?><span>milliseconds</span>
         <?cs else ?>
            <input class="number_field" type="text" name="DstFtpReconnectInterval" id="DstFtpReconnectIntervalInput"
               value="<?cs var:dest.FtpReconnectInterval ?>">
            <span>milliseconds</span>
            <span id="DstFtpReconnectIntervalErrorMessageContainer" class="validation_error_message_container"></span>
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction(
                  'DstFtpReconnectIntervalInput', 'DstFtpReconnectIntervalRow', 'DstFtpReconnectIntervalErrorMessageContainer',
                  function()
                  {
                     var ReconnectOptionsComboBox = document.channel_data.DstFtpMaxReconnectsChoiceBox;
                     return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value != "never";
                  },
                  showDestinationTab);
            </script>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected DstUseFtpRow">
      <td class="left_column">Keep local files</td>
      <td class="inner" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner" style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if: Channel.ReadOnlyMode ?>
                     <?cs if:dest.KeepLocalFiles ?>
                        Yes
                     <?cs else ?>
                        No
                     <?cs /if ?>
                  <?cs else ?>
                     <input name="DstKeepLocalFiles" type="checkbox" <?cs if:dest.KeepLocalFiles ?>checked="checked"<?cs /if ?> />
                  <?cs /if ?>
                  <?cs if:!Channel.ReadOnlyMode ?>
                    <td>
                      <a id="configuration_smaller_Icon" class="helpIcon" tabindex="100" rel="Select the <span style='font-weight:bold'>Keep local files</span> check box if the generated files are to be kept on the Iguana server after uploading them to the FTP server." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                    </td>
                  <?cs /if ?>                  
               </td>
            </tr>
         </table>
      </td>
   </tr>

   <tr class="selected DstUseFtpRow">

       <?cs def:print_ftp_overwrite(name) ?>
          <span class="configurationFillerText">
          Existing remote files will
	  </span>
           <?cs if: name == "overwrite" ?>be overwritten
	   <?cs elif: name=="skip" ?>be skipped
	   <?cs elif: name=="stop_channel" ?> stop the channel
	   <?cs else ?><?cs var:name ?>
	   <?cs /if ?>
       <?cs /def ?>

      <td class="left_column">FTP overwrite handling</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?> 
		     <?cs call:print_ftp_overwrite(dest.FtpOverwrite) ?>
                     <input type="hidden" id="DstFtpOverwrite" name="DstFtpOverwrite" value="<?cs var:dest.FtpOverwrite ?>"/>
                  <?cs else ?>
                   <span class="configurationFillerText">
		     Existing remote files will 
		   </span>
		   <select name="DstFtpOverwrite">
		     <option <?cs if:dest.FtpOverwrite=='overwrite'?>selected<?cs /if ?> value="overwrite">be overwritten</option> 
		     <option <?cs if:dest.FtpOverwrite=='skip'?>selected<?cs /if ?> value="skip">be skipped</option> 
		     <option <?cs if:dest.FtpOverwrite=='stop_channel'?>selected<?cs /if ?> value="stop_channel">stop the channel</option> 
		     </select>
                  <?cs /if ?>
               </td>
            </tr>     
         </table>
      </td>
   </tr>

<?cs /with ?>
