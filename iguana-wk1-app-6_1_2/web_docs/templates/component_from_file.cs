<?cs # vim: set syntax=html :?>

<?cs include:"browse_macro.cs" ?>

<script type="text/javascript" src="<?cs var:iguana_version_js("path_expand.js") ?>"></script>

<script defer type="text/javascript">

var defaultBackgroundColor = null;

var sourcePath = null;

var errorPath = null;

var processedPath = null;

function toggleSrcFtpNumberOfReconnectsTextField()
{
   var SrcUseFtp = document.getElementById('SrcUseFtp');
   if(SrcUseFtp.checked)
   {
      var ReconnectOptionsComboBox = document.channel_data.SrcFtpMaxReconnectsChoiceBox;
      if (ReconnectOptionsComboBox)
      {
         var ReconnectsText = document.getElementById('SrcFtpMaxReconnectsText');
         var ReconnectIntervalRow = document.getElementById('SrcFtpReconnectIntervalRow');
      
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
      	<?cs if:!Channel.Source.FtpUnlimitedMaxReconnects && (Channel.Source.FtpMaxReconnects == 0) ?>
      	$('#SrcFtpReconnectIntervalRow').hide();
      	<?cs else ?>
      	$('#SrcFtpReconnectIntervalRow').show();
      	<?cs /if ?>
      }
   }
   else
   {
     $('#SrcFtpReconnectIntervalRow').hide(); 
   }
}

function secureSrcShellGetHostFingerPrint( idToFill, idError, idSpinner ){
   var HostName = $('input[name=SrcFtpServer]').val();
   var Port = $('input[name=SrcFtpPort]').val();
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


function setSrcRowsDisplay(ClassName, DisplayStyle)
{
   var ToFileTableBody = document.getElementById('sourceTabContents');
   var ToFileRows = ToFileTableBody.getElementsByTagName('tr');
   
   for (var RowIndex = 0; RowIndex < ToFileRows.length; ++RowIndex)
   {
      if (CLSelementHasClass(ToFileRows[RowIndex], ClassName))
      {
         ToFileRows[RowIndex].style.display = DisplayStyle;
      }
   }
}

//this helps set up default port values when the protocol changes
var onSrcFtpProtocolChanged = function()
{
   var OldPorts = { 'ftp' : '21', 'ftps' : '990', 'sftp' : '22' };
   var OldProtocol = '<?cs var:Channel.Source.FtpProtocol ?>';

   return function()
   {
      var NewProtocol = $('#SrcFtpProtocol').val();
      if (NewProtocol != OldProtocol)
      {
         OldPorts[OldProtocol] = $('#SrcFtpPort').val();
         $('#SrcFtpPort').val(OldPorts[NewProtocol]);
         OldProtocol = NewProtocol;

         // Update the help string for the FTP path
         var HelpIcon = document.getElementById('from_ftp_path_help_Icon');
         if (HelpIcon) 
         {
            HelpIcon.rel = ftpFromPathBoxHelpString (NewProtocol);
         }
      }
   }
}();

function useSrcFtpChanged()
{
   var FtpProtocol = document.getElementById('SrcFtpProtocol').value;
   var UseFtpInput = document.getElementById('SrcUseFtp');
   var FtpSslVerifyPeerInput = document.getElementById('SrcFtpSslVerifyPeer');
   var FtpDeleteAfterDownload = document.getElementById('SrcFtpDeleteAfterDownload');
   var ShellPasswordAuth = document.getElementById('SrcFtpSecureShellPasswordAuthentication').value;
   var ShellVerifyHost = document.getElementById('SrcFtpSecureShellVerifyHostFingerprint').checked;

   if (!FtpDeleteAfterDownload)
   {
      //get the checked radio box since we look like we're in edit mode
      for (var RadioIndex = 0; RadioIndex < document.channel_data.SrcFtpDeleteAfterDownload.length; RadioIndex++)
      {
         var RadioElem = document.channel_data.SrcFtpDeleteAfterDownload[RadioIndex];
         if (RadioElem.checked)
	 {
	    FtpDeleteAfterDownload = RadioElem;
         }
      }
   }

   var RowDisplayStyle = document.getElementById('trSrcUseFtp').style.display;

   if (UseFtpInput.checked)
   {
      setSrcRowsDisplay('SrcUseFtpRow', RowDisplayStyle);
      //check based on protocol
      setSrcRowsDisplay('SrcUseFtpSslRow', (FtpProtocol == 'ftps' ? '' : 'none'));
      setSrcRowsDisplay('SrcUseSecureShellFtpRow', (FtpProtocol == 'sftp' ? '' : 'none'));
      
      //password inactive for sftp, unless using password auth
      var PasswordDisplay = (FtpProtocol != 'sftp' || ShellPasswordAuth == 'user_password');
      setSrcRowsDisplay('SrcFtpPasswordRow', (PasswordDisplay ? '' : 'none'));
      if (FtpProtocol == 'sftp'){
	 setSrcRowsDisplay('SrcSecureShellFtpKeyAuth', (PasswordDisplay ? 'none' : ''));
	 setSrcRowsDisplay('SrcSecureShellVerifyHostFingerprint', (ShellVerifyHost ? '' : 'none'));
      }
      //cert authority displayed only if ftps and verify peer
      setSrcRowsDisplay('SrcFtpCertificateAuthorityRow',( (FtpSslVerifyPeerInput.checked && FtpProtocol == 'ftps') ? '' : 'none'))

      //depends on current selection
      onSrcFtpDeleteAfterDownloadChange(FtpDeleteAfterDownload.value);
   }
   else
   {
      //hide everything
      setSrcRowsDisplay('SrcUseFtpRow', 'none');
      setSrcRowsDisplay('SrcUseFtpSslRow', 'none');
      setSrcRowsDisplay('SrcUseSecureShellFtpRow', 'none');
   }

   onSrcFtpProtocolChanged();  // Well, it might have.
}

function onFileFormatChange()
{
   var SrcFileFormat = '<?cs var:Channel.Source.FileFormat ?>';
   SrcFileFormatElem = document.getElementById('SrcFileFormat');
   if (SrcFileFormatElem)
   {     
      //edit mode
      SrcFileFormat = SrcFileFormatElem.value;
   }   

   if( SrcFileFormat == 'ArbitraryText' )
   {     
      $('.clsIgnoreBatchSegments').hide();
      $('.clsDelimiter').hide();
   }
   else if( SrcFileFormat == 'X12' )
   {
      $('.clsIgnoreBatchSegments').show();
      $('.clsDelimiter').hide();
   }
   else
   {
      $('.clsIgnoreBatchSegments').show();
      $('.clsDelimiter').show();
   }
   
   if ( SrcFileFormat == 'CustomHeader' )
   {
      $('.clsCustomHeader').show();
   }
   else
   {
      $('.clsCustomHeader').hide();
   }
}

function onSrcFtpDeleteAfterDownloadChange(NewValue)
{
   var NewRowStyle = (NewValue == "true" ? NewRowStyle = 'none' : NewRowStyle = '');   
   var TableBody = document.getElementById('sourceTabContents');
   var Rows = TableBody.getElementsByTagName('tr');   
   for (var RowIndex = 0; RowIndex < Rows.length; ++RowIndex)
   {
      if (CLSelementHasClass(Rows[RowIndex], "disappearingFtpDownloadedFilesRow"))
      {
         Rows[RowIndex].style.display = NewRowStyle;
      }
   }
}

function onArchiveProcessedFilesChange(NewValue)
{
   var VisibleRowStyle = document.getElementById("SrcArchiveProcessedFilesRow").style.display;   
   var NewRowStyle;
   if (NewValue == "true")
   {
      NewRowStyle = VisibleRowStyle;
   }
   else
   {
      NewRowStyle = "none";
   }
   
   var TableBody = document.getElementById('sourceTabContents');
   var Rows = TableBody.getElementsByTagName('tr');
   
   for (var RowIndex = 0; RowIndex < Rows.length; ++RowIndex)
   {
      if (CLSelementHasClass(Rows[RowIndex], "disappearingArchiveProcessedFilesRow"))
      {
         Rows[RowIndex].style.display = NewRowStyle;
      }
   }

   archiveProcessedFilesStatus = NewValue;

}

function onLoadFromFile()
{
   onFileFormatChange();
   useSrcFtpChanged();
   toggleSrcFtpNumberOfReconnectsTextField();
   <?cs if:!Channel.Source.ArchiveProcessedFiles ?>
      onArchiveProcessedFilesChange("false");
   <?cs /if ?>

   // Update the help string for the FTP path
     var HelpIcon = document.getElementById('from_ftp_path_help_Icon');
     if (HelpIcon) 
     {
        HelpIcon.rel = ftpFromPathBoxHelpString ('<?cs var:Channel.Destination.FtpProtocol ?>');
     }

   onFileFormatChange();
}

function ftpFromPathBoxHelpString (FtpProtocol)
{
    var HelpString = "Location of the directory on the FTP server from which the files are to be retrieved.";
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

<?cs with: source = Channel.Source ?>
   <!-- Added the following line so that the variable value wouldn't get overwritten when saving  -->
   <!-- <input type="hidden" name="Producer.PrependFileInfoFlag" value="<value-of select="PrependFileInfoFlag"/>"/> -->

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
   
   <tr id="trSrcUseFtp" class="selected">
      <td class="left_column first_row">Download from FTP to source directory</td>
      <td class="inner first_row" colspan="3">
         <table class="inner" style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if: Channel.ReadOnlyMode ?>
                     <?cs if: source.UseFtp ?>
                        Yes
                     <?cs else ?>
                        No
                     <?cs /if ?>
                     <input type="checkbox" style="display:none" id="SrcUseFtp" name="SrcUseFtp" <?cs if: source.UseFtp ?>checked="checked" <?cs /if?>/>
                  <?cs else ?>
                     <input type="checkbox" id="SrcUseFtp" name="SrcUseFtp" onchange="useSrcFtpChanged();" onkeyup="useSrcFtpChanged();" onclick="useSrcFtpChanged();" <?cs if: source.UseFtp ?>checked="checked" <?cs /if?>/>
                  <?cs /if ?>
               </td>
            </tr>
         </table>
      </td>
   </tr>

   <tr class="selected SrcUseFtpRow">
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
		     <?cs call:print_ftp_protocol(source.FtpProtocol) ?>
                     <input type="hidden" id="SrcFtpProtocol" name="SrcFtpProtocol" value="<?cs var:source.FtpProtocol ?>"/>
                  <?cs else ?>
		     <select name="SrcFtpProtocol" id="SrcFtpProtocol" onchange="useSrcFtpChanged();" onkeyup="useSrcFtpChanged();" onclick="useSrcFtpChanged();">
		        <option value="ftp" <?cs if:source.FtpProtocol == "ftp" ?>selected<?cs /if ?> ><?cs call:print_ftp_protocol('ftp') ?></option>
		        <option value="ftps" <?cs if:source.FtpProtocol == "ftps" ?>selected<?cs /if ?> ><?cs call:print_ftp_protocol('ftps') ?></option>
		        <option value="sftp" <?cs if:source.FtpProtocol == "sftp" ?>selected<?cs /if ?> ><?cs call:print_ftp_protocol('sftp') ?></option>
		     </select>
                  <?cs /if ?>
               </td>
               <td class="inner_right">
                  <?cs if:Channel.Source.Error.Ftp ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Source.Error.Ftp ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
               <?cs if:!Channel.ReadOnlyMode ?>
                <td>
                  <a id="configuration_smaller_Icon" class="helpIcon" tabindex="100" rel="<ul><li>FTP (standard FTP) : not secured by encryption</li><li>FTPS : encrypted FTP using SSL.</li><li>SFTP: uses SSH to encrypt file transfer communications</li></ul>" title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                </td>
               <?cs /if ?>
            </tr>     
         </table>
      </td>
   </tr>

   <tr class="selected SrcUseSecureShellFtpRow" >
      <td class="left_column">Authentication</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:source.FtpSecureShellPasswordAuthentication ?>
	    <input type="hidden" name="SrcFtpSecureShellPasswordAuthentication" id="SrcFtpSecureShellPasswordAuthentication" value="user_password">
	    Username/Password
	    <?cs else ?>
	    <input type="hidden" name="SrcFtpSecureShellPasswordAuthentication" id="SrcFtpSecureShellPasswordAuthentication" value="private_public_key">
	    Private/Public Key
	    <?cs /if ?>
         <?cs else ?>
	    <select onchange="useSrcFtpChanged()" id="SrcFtpSecureShellPasswordAuthentication" name="SrcFtpSecureShellPasswordAuthentication">
	    <option value="user_password" <?cs if:source.FtpSecureShellPasswordAuthentication ?>selected<?cs /if ?> >Username/Password</option>
	    <option value="private_public_key" <?cs if:!source.FtpSecureShellPasswordAuthentication ?>selected<?cs /if ?> >Private/Public Key</option>
	    </select>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected SrcUseFtpRow">
      <td class="left_column">FTP server<font color="#ff0000">*</font></td>
      <td class="inner" colspan="3">
         <?cs if: Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.FtpServer) ?>
            <?cs if:Channel.Source.Error.FtpServer ?>
               <div class="configuration_error">
                  <?cs var:Channel.Source.Error.FtpServer ?>
               </div>
            <?cs /if ?>
         <?cs else ?>
            <table class="inner" style="float:left">
               <tr>
                  <td class="inner_left">
                     <input type="text" name="SrcFtpServer" value="<?cs var:html_escape(source.FtpServer) ?>" />
                     <?cs if:Channel.Source.Error.FtpServer ?>
                        <div class="configuration_error">
                           <?cs var:Channel.Source.Error.FtpServer ?>
                        </div>
                     <?cs /if ?>
                  </td>
               </tr>
            </table>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected SrcUseFtpRow" id="SrcFtpRow">
      <td class="left_column">FTP port<font color="#ff0000">*</font></td>
      <td class="inner" colspan="3">
         <?cs if: Channel.ReadOnlyMode ?>
            <?cs alt:html_escape(source.FtpPort) ?><?cs /alt ?>
            <?cs if:Channel.Source.Error.FtpPort ?>
               <div class="configuration_error">
                  <?cs var:Channel.Source.Error.FtpPort ?>
               </div>
            <?cs /if ?>
         <?cs else ?>
            <table class="inner" style="float:left">
               <tr>
                  <td class="inner_left">
                     <input type="text" id="SrcFtpPort" name="SrcFtpPort" value="<?cs alt:html_escape(source.FtpPort) ?><?cs /alt ?>" />
                     <script defer type="text/javascript">
                        VALregisterIntegerValidationFunction("SrcFtpPort", "SrcFtpRow",
                           "SrcFtpErrorMessageContainer", null, showSourceTab, 1, 65535);
                     </script>
					 <span id="SrcFtpErrorMessageContainer"
					    class="validation_error_message_container"
					    style="white-space:normal">
					 </span>
                     <?cs if:Channel.Source.Error.FtpPort ?>
                        <div class="configuration_error">
                           <?cs var:Channel.Source.Error.FtpPort ?>
                        </div>
                     <?cs /if ?>
                  </td>
               </tr>
            </table>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected SrcUseFtpRow">
      <td class="left_column">FTP username<font color="#ff0000">*</font></td>
      <td class="inner" colspan="3">
         <?cs if: Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.FtpUsername) ?>
            <?cs if:Channel.Source.Error.FtpUsername ?>
               <div class="configuration_error">
                  <?cs var:Channel.Source.Error.FtpUsername ?>
               </div>
            <?cs /if ?>
         <?cs else ?>
            <table class="inner" style="float:left">
               <tr>
                  <td class="inner_left">
                     <input type="text" name="SrcFtpUsername" value="<?cs var:html_escape(source.FtpUsername) ?>" />
                     <?cs if:Channel.Source.Error.FtpUsername ?>
                        <div class="configuration_error">
                           <?cs var:Channel.Source.Error.FtpUsername ?>
                        </div>
                     <?cs /if ?>
                  </td>
               </tr>
            </table>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected SrcUseFtpRow SrcFtpPasswordRow">
      <td class="left_column">FTP password</td>
      <td class="inner" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner" style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if: Channel.ReadOnlyMode ?>
                     ******
                  <?cs else ?>
                     <input name="SrcFtpPassword" type="password" value="<?cs var:html_escape(source.FtpPassword) ?>" />
                  <?cs /if ?>
               </td>
            </tr>
         </table>
      </td>
   </tr>
   
   <tr class="selected SrcUseFtpRow">
      <td class="left_column">FTP path</td>
      <td class="inner" colspan="3">
         <?cs if: Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.FtpPath) ?>
         <?cs else ?>
            <table class="inner" style="float:left">
               <tr>
                  <td class="inner_left">
                     <input name="SrcFtpPath" type="text" value="<?cs var:html_escape(source.FtpPath) ?>" />
                  </td>
               <?cs if:!Channel.ReadOnlyMode ?>
                <td>
                  <a id="from_ftp_path_help_Icon" class="helpIcon" tabindex="100" rel="..." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                </td>
               <?cs /if ?>
               </tr>
            </table>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected SrcUseSecureShellFtpRow" >
      <td class="left_column">Verify host fingerprint</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode ?> 
            <?cs if:source.FtpSecureShellVerifyHostFingerprint ?>Yes<?cs else ?>No<?cs /if ?>          
            <input type="checkbox" 
	           style="display: none;"
		   id="SrcFtpSecureShellVerifyHostFingerprint" 
		   name="SrcFtpSecureShellVerifyHostFingerprint" 
		   <?cs if:source.FtpSecureShellVerifyHostFingerprint ?>checked<?cs /if ?>/>&nbsp;      
         <?cs else ?>
            <input type="checkbox" 
	           class="no_style" 
		   id="SrcFtpSecureShellVerifyHostFingerprint" 
		   name="SrcFtpSecureShellVerifyHostFingerprint" 
		   <?cs if:source.FtpSecureShellVerifyHostFingerprint ?>checked<?cs /if ?> 
		   onchange="useSrcFtpChanged();" onkeyup="useSrcFtpChanged();" onclick="useSrcFtpChanged();"/>&nbsp;      
           <a id="configuration_smaller_Icon" class="helpIcon" tabindex="100" 
            rel="The host fingerprint is a 32 character MD5 hash representing the unique identity of the SFTP server.  When transferring files from the SFTP server, if the fingerprint of the server that is being connected to does not match the fingerprint indicated in the channel configuration it will generate an error, thus providing a mechanism to guard against receiving messages from an unauthorized machine." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected SrcUseSecureShellFtpRow SrcSecureShellVerifyHostFingerprint" >
      <td class="left_column">Host fingerprint<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?> 
            <?cs var:html_escape(source.FtpSecureShellHostFingerprint) ?>
         <?cs else ?>
            <input id="SrcFtpSecureShellHostFingerprint" name="SrcFtpSecureShellHostFingerprint" type="text" size="40" value="<?cs var:html_escape(source.FtpSecureShellHostFingerprint) ?>">
            <a onclick="javascript:secureSrcShellGetHostFingerPrint('SrcFtpSecureShellHostFingerprint','SrcFtpSecureShellHostFingerprintError','SrcFtpSecureShellHostFingerprintSpinner');">Get Fingerprint</a>
            <span id="SrcFtpSecureShellHostFingerprintSpinner"></span>
            <span id="SrcFtpSecureShellHostFingerprintError"></span>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.FtpSecureShellHostFingerprint ?>
            <div class="configuration_error">
               <?cs var:html_escape(Channel.Source.Error.FtpSecureShellHostFingerprint) ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected SrcUseFtpSslRow" >
      <td class="left_column">Certificate file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(source.FtpSslCertificateKeyFile) ?>
            <?cs if:source.FtpSslCertificateKeyFile!='' ?>
               <a href="/view_ssl_cert.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&ComponentType=Destination">view certificate</a>
            <?cs /if ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('SrcFtpSslCertificateKeyFile', source.FtpSslCertificateKeyFile) ?>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.FtpSslCertificateKey ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.FtpSslCertificateKey ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected SrcUseFtpSslRow" >
      <td class="left_column">Private key file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(source.FtpSslPrivateKeyFile) ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('SrcFtpSslPrivateKeyFile', source.FtpSslPrivateKeyFile) ?>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.FtpSslPrivateKey ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.FtpSslPrivateKey ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected SrcUseFtpSslRow" >
      <td class="left_column">Verify peer</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode ?> 
            <?cs if:source.FtpSslVerifyPeer ?>Yes<?cs else ?>No<?cs /if ?>          
            <input type="checkbox" 
	           style="display: none;"
		   id="SrcFtpSslVerifyPeer" 
		   name="SrcFtpSslVerifyPeer" 
		   <?cs if:source.FtpSslVerifyPeer ?>checked<?cs /if ?>/>&nbsp;      
         <?cs else ?>
            <input type="checkbox" 
	           class="no_style" 
		   id="SrcFtpSslVerifyPeer" 
		   name="SrcFtpSslVerifyPeer" 
		   <?cs if:source.FtpSslVerifyPeer ?>checked<?cs /if ?> 
		   onchange="useSrcFtpChanged();" onkeyup="useSrcFtpChanged();" onclick="useSrcFtpChanged();"/>&nbsp;      
          
            <a id="configuration_smaller_Icon" class="helpIcon" tabindex="100" rel="To verify the authenticity of the server that is sending the messages, select the <span style=font-weight:bold>Verify peer</span> check box." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected SrcUseFtpSslRow SrcFtpCertificateAuthorityRow" >
      <td class="left_column">Certificate authority file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(source.FtpSslCertificateAuthorityFile) ?>
            <?cs if:source.FtpSslCertificateAuthorityFile!='' ?>
               <a href="/view_ssl_cert.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&CertificateType=Authority&ComponentType=Destination">view certificate</a>
            <?cs /if ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('SrcFtpSslCertificateAuthorityFile', source.FtpSslCertificateAuthorityFile) ?>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.FtpSslCertificateAuthority ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.FtpSslCertificateAuthority ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected SrcUseSecureShellFtpRow SrcSecureShellFtpKeyAuth" >
      <td class="left_column">Private key file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(source.FtpSecureShellPrivateKeyFile) ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('SrcFtpSecureShellPrivateKeyFile', source.FtpSecureShellPrivateKeyFile) ?>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.FtpSshPrivateKey ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.FtpSshPrivateKey ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected SrcUseSecureShellFtpRow SrcSecureShellFtpKeyAuth" >
      <td class="left_column">Public key file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(source.FtpSecureShellPublicKeyFile) ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('SrcFtpSecureShellPublicKeyFile', source.FtpSecureShellPublicKeyFile) ?>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.FtpSshPublicKey ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.FtpSshPublicKey ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
 
   <tr class="selected SrcUseFtpRow" id="SrcFtpAttemptToReconnectRow">
      <td class="left_column">Attempt to reconnect?</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:!source.FtpUnlimitedMaxReconnects && (source.FtpMaxReconnects == 0) ?>No<?cs /if ?>
            <?cs if:source.FtpUnlimitedMaxReconnects ?>Yes <span class="configurationFillerText">(unlimited)</span><?cs /if ?>
            <?cs if:!source.FtpUnlimitedMaxReconnects && (source.FtpMaxReconnects != 0) ?>Yes<span class="configurationFillerText">, with limit of</span><?cs /if ?>
         <?cs else ?>
            <select name="SrcFtpMaxReconnectsChoiceBox" onchange="toggleSrcFtpNumberOfReconnectsTextField();" onkeyup="toggleSrcFtpNumberOfReconnectsTextField();">
               <option value="never" <?cs if:!source.FtpUnlimitedMaxReconnects && (source.FtpMaxReconnects == 0) ?>selected="selected"<?cs /if ?> >No</option>
               <option value="unlimited" <?cs if:source.FtpUnlimitedMaxReconnects ?>selected="selected"<?cs /if ?> >Yes (unlimited)</option>
               <option value="other"  <?cs if:!source.FtpUnlimitedMaxReconnects && (source.FtpMaxReconnects != 0) ?>selected="selected"<?cs /if ?>>Yes, with limit</option>
            </select>
         <?cs /if ?>
         <span id="SrcFtpMaxReconnectsText" style="font-size: 11px;<?cs if:source.FtpUnlimitedMaxReconnects ?> display: none;<?cs /if ?><?cs if:source.FtpMaxReconnects == 0 ?> display: none;<?cs /if ?>" >
            <?cs if:Channel.ReadOnlyMode ?>
               <?cs var:html_escape(source.FtpMaxReconnects) ?> times
            <?cs else ?>
               of&nbsp;
               <input class="configuration_smaller" name="SrcFtpMaxReconnects" id="SrcFtpMaxReconnectsInput"
                  value="<?cs var:source.FtpMaxReconnects ?>" />
               <script defer type="text/javascript">
                  VALregisterIntegerValidationFunction
                  (
                     'SrcFtpMaxReconnectsInput', 'SrcFtpAttemptToReconnectRow', 'SrcFtpAttemptToReconnectErrorMessageContainer',
                     function()
                     {
                        var ReconnectOptionsComboBox = document.channel_data.SrcFtpMaxReconnectsChoiceBox;
                        return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value == "other";
                     },
                     showDestinationTab
                  );
               </script>
               &nbsp;times  
            <?cs /if ?>
         </span>
         <?cs if:!Channel.ReadOnlyMode ?>
            &nbsp;
            <a id="configuration_smaller_Icon" class="helpIcon" tabindex="100" rel="This option specifies what action will be taken if the FTP connection is lost, or if the FTP server is busy." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
            <span id="SrcFtpAttemptToReconnectErrorMessageContainer" class="validation_error_message_container"></span>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.FtpMaxCountOfReconnect ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.FtpMaxCountOfReconnect ?>
            </div>
         <?cs /if ?>       
      </td>
   </tr>
   
   <tr class="selected SrcUseFtpRow" id="SrcFtpReconnectIntervalRow" style="<?cs if:!source.FtpUnlimitedMaxReconnects && (source.FtpMaxReconnects == 0) ?>display: none;<?cs /if ?>">
      <td class="left_column">Reconnection interval<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.FtpReconnectInterval) ?> milliseconds
         <?cs else ?>
            <input type="text"
                   name="SrcFtpReconnectInterval"
                   id="SrcFtpReconnectIntervalInput"
                   value="<?cs var:source.FtpReconnectInterval ?>"
            /><span>&nbsp;Milliseconds</span>
            <span id="SrcFtpReconnectIntervalErrorMessageContainer" class="validation_error_message_container"></span>
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction
               (
                  'SrcFtpReconnectIntervalInput', 'SrcFtpReconnectIntervalRow', 'SrcFtpReconnectIntervalErrorMessageContainer',
                  function()
                  {
                     var ReconnectOptionsComboBox = document.channel_data.SrcFtpMaxReconnectsChoiceBox;
                     return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value != "never";
                  },
                  showSourceTab
               );
            </script>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr id="SrcFtpDeleteDownloadedFilesRow" class="selected SrcUseFtpRow">
      <td class="left_column">Action to perform on downloaded files</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <input type="hidden" id="SrcFtpDeleteAfterDownload" name="SrcFtpDeleteAfterDownload" value="<?cs if:source.FtpDeleteAfterDownload ?>true<?cs else ?>false<?cs /if ?>">
            <?cs if:source.FtpDeleteAfterDownload ?>
               Delete remote files after downloading
            <?cs else ?>
               Move remote files after downloading to another remote directory
            <?cs /if ?>
         <?cs else ?>
            <input type="radio" class="no_style" name="SrcFtpDeleteAfterDownload" value="true" <?cs if:source.FtpDeleteAfterDownload ?>checked<?cs /if ?> onclick="onSrcFtpDeleteAfterDownloadChange(this.value);">
            Delete remote files after downloading
            </input>
            <br />
            <input type="radio" class="no_style" name="SrcFtpDeleteAfterDownload" value="false" <?cs if:!source.FtpDeleteAfterDownload ?>checked<?cs /if ?> onclick="onSrcFtpDeleteAfterDownloadChange(this.value);">
            Move remote files after downloading to another remote directory
            </input>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected SrcUseFtpRow disappearingFtpDownloadedFilesRow">
      <td class="left_column">FTP path for downloaded files<font color="#ff0000">*</font></td>
      <td class="inner" colspan="3">
         <?cs if: Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.FtpRemoteDownloadedDir) ?>
            <?cs if:Channel.Source.Error.FtpRemoteDownloadedDir ?>
               <div class="configuration_error">
                  <?cs var:Channel.Source.Error.FtpRemoteDownloadedDir ?>
               </div>
            <?cs /if ?>
         <?cs else ?>
            <table class="inner" style="float:left">
               <tr>
                  <td class="inner_left">
                     <input name="SrcFtpRemoteDownloadedDir" class="configuration_long" value="<?cs var:html_escape(source.FtpRemoteDownloadedDir) ?>" />
                     <?cs if:Channel.Source.Error.FtpRemoteDownloadedDir ?>
                        <div class="configuration_error">
                           <?cs var:Channel.Source.Error.FtpRemoteDownloadedDir ?>
                        </div>
                     <?cs /if ?>
                  </td>
               </tr>
            </table>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="channel_configuration_section_heading_row">
      <td>
         <table>
            <tr>
               <td style="width:100%;"><hr /></td>
               <td class="channel_configuration_section_heading">Basic Options</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>
   
   <tr class="selected">
      <td class="left_column first_row">Source directory<font color="#ff0000">*</font></td>
      <td class="inner_left first_row" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:source.Directory != "" ?>
               <?cs var:html_escape(source.Directory) ?>
               <?cs if:source.Directory != path_expand(source.Directory) ?>
                  <div id="SrcDirectory_path_preview_static" class="path_preview"> Absolute Path: "<?cs var:html_escape(path_expand(source.Directory)) ?>"</div>
               <?cs /if ?>
            <?cs /if ?>
         <?cs else ?>
            <?cs call:browse_input_folder('SrcDirectory', source.Directory) ?>                 
         <?cs /if ?> 
         <?cs if:Channel.Source.Error.SourceDir ?>
            <div class="configuration_error">
            <?cs var:Channel.Source.Error.SourceDir ?>
            </div>
         <?cs /if ?>
      </td>
   </tr> 
   <tr class="selected" id="SrcPollTimeRow">
      <td class="left_column">Poll time<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.PollTime) ?>
         <?cs else ?>
            <input type="text" class="number_field" name="SrcPollTime" id="SrcPollTimeInput"
               value="<?cs var:source.PollTime ?>">
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction(
                     'SrcPollTimeInput', 'SrcPollTimeRow', 'SrcPollTimeErrorMessageContainer',
                     null, showSourceTab, 1000);
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
   <tr class="selected">
      <td class="left_column">
         Extension of files to read<font color="#ff0000">*</font> <br>
         <span style="font-weight:normal;">(e.g., txt, hl7, edi)</span>
      </td>
      <td class="inner" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.Extension) ?>
         <?cs else ?>
            <input type="text" class="configuration_smaller" name="SrcExtension" id="SrcExtension" value="<?cs var:html_escape(source.Extension) ?>">  <a id="SrcExtension_Icon" class="helpIcon" tabindex="100" rel="Separate multiple extensions with commas.<br>To read all files, enter '*'.<br>For files with no extension, enter '&lt;none&gt;'." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.Extension ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.Extension ?>
            </div>
         <?cs /if ?>
          
      </td>
   </tr>
   <tr class="selected" id="SrcMinimumFileAgeRow">
      <td class="left_column">
         Minimum file age<font color="#ff0000">*</font>
      </td>
      <td class="inner" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.MinimumFileAge) ?> seconds
         <?cs else ?>
            <input type="text" class="number_field" name="SrcMinimumFileAge" id="SrcMinimumFileAge"
               value="<?cs var:html_escape(source.MinimumFileAge) ?>">
            seconds
            <a id="SrcMinimumFileAge_Icon" class="helpIcon" tabindex="100" rel="Any files which have been modified more recently than this value will be ignored. This allows Iguana to avoid any files which are still being written." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
            <span id="SrcMinimumFileAgeErrorMessageContainer" class="validation_error_message_container"></span>
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction(
                     'SrcMinimumFileAge', 'SrcMinimumFileAgeRow', 'SrcMinimumFileAgeErrorMessageContainer',
                     null, showSourceTab, 0);
            </script>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.MinimumFileAge ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.MinimumFileAge ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr id="SrcArchiveProcessedFilesRow" class="selected">
      <td class="left_column">Action to perform on processed files</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <input type="hidden" name="SrcArchiveProcessedFiles" value="false">
            <?cs if:!source.ArchiveProcessedFiles ?>
               Delete processed files
            <?cs else ?>
               Move processed files
            <?cs /if ?>
         <?cs else ?>
            <input type="radio" class="no_style" name="SrcArchiveProcessedFiles" value="false" <?cs if:!source.ArchiveProcessedFiles ?>checked<?cs /if ?> onclick="onArchiveProcessedFilesChange(this.value);">
            Delete processed files
            </input>
            <br />
            <input type="radio" class="no_style" id="SrcArchiveProcessedFiles" name="SrcArchiveProcessedFiles" value="true" <?cs if:source.ArchiveProcessedFiles ?>checked<?cs /if ?> onclick="onArchiveProcessedFilesChange(this.value);">
            Move processed files
            </input>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="disappearingArchiveProcessedFilesRow selected">
      <td class="left_column">Full directory path to move error files into<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
          <?cs if:Channel.ReadOnlyMode ?>
             <?cs if:source.ErrorDirectory != "" ?>
                <?cs var:html_escape(source.ErrorDirectory) ?>
                <?cs if:source.ErrorDirectory != path_expand(source.ErrorDirectory) ?>
                   <div id="SrcErrorDirectory_path_preview_static" class="path_preview"> Absolute Path: "<?cs var:html_escape(path_expand(source.ErrorDirectory)) ?>"</div>
               <?cs /if ?>
            <?cs /if ?>
         <?cs else ?>
            <?cs call:browse_input_folder('SrcErrorDirectory', source.ErrorDirectory) ?>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.ErrorDir ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.ErrorDir ?>
            </div>
         <?cs /if ?>            
      </td>
   </tr>
 
   <tr class="disappearingArchiveProcessedFilesRow selected">
      <td class="left_column">Full directory path to move processed files into<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:source.ProcessedDirectory != "" ?>
               <?cs var:html_escape(source.ProcessedDirectory) ?>
               <?cs if:source.ProcessedDirectory != path_expand(source.ProcessedDirectory) ?>
                  <div id="SrcProcessedDirectory_path_preview_static" class="path_preview"> Absolute Path: "<?cs var:html_escape(path_expand(source.ProcessedDirectory)) ?>"</div>
               <?cs /if ?>  
            <?cs /if ?>
	      <?cs else ?>
            <?cs call:browse_input_folder('SrcProcessedDirectory', source.ProcessedDirectory) ?>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.ProcessedDir ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.ProcessedDir ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="channel_configuration_section_heading_row">
      <td>
         <table>
            <tr>
               <td style="width:100%;"><hr /></td>
               <td class="channel_configuration_section_heading">Message Options</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>
   
   <tr class="selected">
      <td class="left_column first_row">Input file type</td>
      <td class="inner first_row" colspan="3">
         <table class="inner" Style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(source.FileFormat) ?>
                     <input type="hidden" name="SrcFileFormat" id="SrcFileFormat" value="<?cs var:source.FileFormat ?>" />
                  <?cs else ?>                
                     <select name="SrcFileFormat" id="SrcFileFormat" onchange="onFileFormatChange()">
                        <?cs def:file_format(value, text) ?>
                           <?cs if: source.FileFormat == value ?>
                              <option value="<?cs var:value ?>" selected> <?cs var:text ?> </option>
                           <?cs else ?>
                              <option value="<?cs var:value ?>"> <?cs var:text ?> </option>
                           <?cs /if ?>
                        <?cs /def ?>
                        <?cs call:file_format('HL7', 'HL7') ?>
                        <?cs call:file_format('X12', 'X12') ?>
                        <?cs call:file_format('ArbitraryText', 'Arbitrary Text') ?>
                        <?cs call:file_format('CustomHeader', 'Custom Message Header') ?>
                     </select>
                     <a id="SrcFileFormat_Icon" class="helpIcon" tabindex="101" rel="If <i>Arbitrary Text</i> is selected, no HL7 or X12 content checking will be performed. When <i>Custom Message Header</i> is selected, a segment header other than 'MSH' can be used to read the file as HL7 style messages." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a> 
                  <?cs /if ?>
               </td>               
            </tr>
         </table>
      </td>
   </tr>

   <tr class="clsCustomHeader selected">
      <td class="left_column">Custom Message Header</td>
      <td class="inner" colspan="3">
         <table class="inner" Style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if: Channel.ReadOnlyMode ?>
                  <?cs var:html_escape(source.CustomHeader) ?>
                  <?cs else ?>
                  <input type="text" name="SrcCustomHeader" id="SrcCustomHeader"
                         value="<?cs var:source.CustomHeader ?>">
                  <?cs /if ?>
               </td>               
               <td class="inner_right">
                  <?cs if:Channel.Source.Error.CustomHeader ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Source.Error.CustomHeader ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>
         </table>
      </td>
   </tr>
   
   <tr class="clsIgnoreBatchSegments selected">
      <td class="left_column">
         Ignored segments list <br>
         <span style="font-weight:normal;">(e.g., FHS, BHS, FTS, BTS)</span>
      </td>
      <td class="inner_left" colspan="3">
          <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.IgnoredSegments) ?>&nbsp;
          <?cs else ?>
             <input type="text" name="SrcIgnoredSegments" value="<?cs var:html_escape(source.IgnoredSegments) ?>" />
          <?cs /if ?>
      </td>
   </tr>
 
   <tr class="clsDelimiter selected">
      <td class="left_column">Replace segment delimiter with</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
             '<?cs var:html_escape(source.SegmentDelimiter) ?>'    
         <?cs else ?>
	      <select name="SrcSegmentDelimiter">
               <?cs def:segment_delimiter(value,text) ?>
                  <?cs if: source.SegmentDelimiter == value ?>
                     <option value="<?cs var:value ?>" selected>  <?cs var:text ?>  </option>
                  <?cs else ?>
                     <option value="<?cs var:value ?>">  <?cs var:text ?>  </option>
                  <?cs /if ?>
               <?cs /def ?>
               <?cs call:segment_delimiter('0x0D',      '0x0D') ?>
               <?cs call:segment_delimiter('0x0A',      '0x0A') ?>
               <?cs call:segment_delimiter('0x0D 0x0A', '0x0D 0x0A') ?>
            </select>
         <?cs /if ?>   
      </td>
   </tr>

  
   <tr class="clsDelimiter selected">
      <td class="left_column">Replace message delimiter?</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if: source.MessageDelimiter != 'None' ?>
               Yes, with '0x0A'
	    <?cs else ?>
	       No
            <?cs /if ?>
         <?cs else ?>
            <select name="SrcMessageDelimiter">
                  <option value="0x0A" <?cs if:source.MessageDelimiter != 'None' ?>selected<?cs /if ?> > Yes, with '0x0A' </option>
                  <option value="None" <?cs if:source.MessageDelimiter == 'None' ?>selected<?cs /if ?> > No </option>
            </select>
         <?cs /if ?>   
      </td>
   </tr>

   <!-- Commented out for now - see #5514 (and #4979)
   <tr class="selected">
      <td class="left_column">Prepend file name and path to message</td>     
      <td class="inner_left" colspan="3">
         <?cs if:source.PrependFileInfo ?>
            <input type="checkbox" class="no_style" name="SrcPrependFileInfo" checked />
         <?cs else ?>
            <input type="checkbox" class="no_style" name="SrcPrependFileInfo" />
         <?cs /if ?>
      </td>
   </tr>
   -->

   <tr class="selected">
      <td class="left_column">Input file encoding</td>
      <td class="inner" colspan="3">
         <table class="inner" Style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if: Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(source.Encoding.Description) ?>
                  <?cs else ?>
                     <select name="SrcEncoding">
                        <option value="<?cs var:html_escape(source.Encoding.Name) ?>" selected>
                           <?cs var:html_escape(source.Encoding.Description) ?>
                        </option>
                        <option value="<?cs var:html_escape(source.Encoding.Name) ?>">
                           ---
                        </option>
                        <?cs each:encoding = AllEncodings ?>
                           <?cs if: encoding.Name        != source.Encoding.Name
                                 || encoding.Description != source.Encoding.Description ?>
                              <option value="<?cs var:html_escape(encoding.Name) ?>">
                                 <?cs var:html_escape(encoding.Description) ?>
                              </option>
                           <?cs /if ?>
                        <?cs /each ?>
                     </select>
                  <?cs /if ?>
               </td>               
               <td class="inner_right">
                  <?cs if:Channel.Source.Error.Encoding ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Source.Error.Encoding ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>
         </table>
      </td>
   </tr>
   
   <tr class="selected">
      <td class="left_column">
         Hex representation of special EOF character <br>
         <span style="font-weight:normal;">(e.g., 0x00, 0x1A)</span>
      </td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(source.EndOfFile) ?>&nbsp;
         <?cs else ?>
            <input type="text" name="SrcEndOfFile" value="<?cs var:html_escape(source.EndOfFile) ?>">
         <?cs /if ?>
      </td>
   </tr>

<?cs /with ?>
