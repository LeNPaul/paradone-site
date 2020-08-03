<?cs # vim: set syntax=html :?>

<?cs with: dest = Channel.Destination ?>
<?cs include:"browse_macro.cs" ?>

<script defer type="text/javascript"> <!--

function onLoadToHttp()
{
   <?cs if:!Channel.ReadOnlyMode ?>
      showOrHideHttpRows();
   <?cs /if ?>
}

function showOrHideHttpRows()
{
   var TableBody = document.getElementById('destinationTabContents');
   var Rows = TableBody.getElementsByTagName('tr');
   var displayValue =  document.getElementById('DstInputSslVerifyPeer').checked ? '' : 'none';   
   for (var RowIndex = 0; RowIndex < Rows.length; ++RowIndex)
   {
      if (CLSelementHasClass(Rows[RowIndex], "http_ssl_verify_peer_row"))
      {
         Rows[RowIndex].style.display = displayValue;
      }
   }
}
-->
</script>

   <tr class="selected">
      <td class="left_column">Remote HTTPS host name<font color="#ff0000">*</font></td>
      <td class="inner_left">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.RemoteHost) ?>
         <?cs else ?>
            <input type="text" name="DstRemoteHost" value="<?cs var:html_escape(dest.RemoteHost) ?>" />
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.RemoteHost ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.RemoteHost ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
   

   <tr class="selected" id="DstRemotePortRow">
      <td class="left_column">Remote host port<font color="#ff0000">*</font></td>
      <td class="inner_left">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.RemotePort) ?>
         <?cs else ?>
            <input class="number_field" type="text" name="DstRemotePort" id="DstRemotePortInput"
               value="<?cs var:html_escape(dest.RemotePort) ?>" />
            <span id="DstRemotePortErrorMessageContainer" class="validation_error_message_container"></span>
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction('DstRemotePortInput', 'DstRemotePortRow', 'DstRemotePortErrorMessageContainer', null, showDestinationTab, 1, 65535);
            </script>
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.RemotePort ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.RemotePort ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
  

   <tr class="selected">
      <td class="left_column">Remote channel name<font color="#ff0000">*</font></td>
      <td class="inner_left">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.RemoteChannelName) ?>
         <?cs else ?>
            <input type="text" name="DstRemoteChannelName" value="<?cs var:html_escape(dest.RemoteChannelName) ?>" />
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.RemoteChannelName ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.RemoteChannelName ?>
            </div>
         <?cs /if ?>
      </td>
   </tr>
   

   <tr class="selected" id="DstHttpTimeoutRow">
      <td class="left_column">Timeout<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.HttpTimeout) ?> milliseconds
         <?cs else ?>
            <input class="number_field" type="text" name="DstHttpTimeout" id="DstHttpTimeout"
               value="<?cs var:html_escape(dest.HttpTimeout) ?>" />
            milliseconds
            <a id="DstHttpTimeout_Icon" class="helpIcon" tabindex="100" rel="Must be at least 1000 milliseconds." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a> 
            <span id="DstHttpTimeoutErrorMessageContainer" class="validation_error_message_container"></span>
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction('DstHttpTimeout', 'DstHttpTimeoutRow', 'DstHttpTimeoutErrorMessageContainer', null, showDestinationTab);
            </script>
         <?cs /if ?>
         <?cs if:Channel.Destination.Error.Timeout ?>
            <div class="configuration_error">
               <?cs var:Channel.Destination.Error.Timeout ?>
            </div>
         <?cs /if ?>               
      </td>
   </tr>

   <tr class="selected" id="DstSslCertificateKeyFileRow" >
      <td class="left_column">Certificate file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.SslCertificateKeyFile) ?>
            <?cs if:dest.SslCertificateKeyFile!='' ?>
               <a href="/view_ssl_cert.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&ComponentType=Destination">view certificate</a>
            <?cs /if ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('DstSslCertificateKeyFile', dest.SslCertificateKeyFile) ?>
         <?cs /if ?>
                  <?cs if:Channel.Destination.Error.SslCertificate ?>
                     <div class="configuration_error">
                        <?cs var:Channel.Destination.Error.SslCertificate ?>
                     </div>
                  <?cs /if ?>
      </td>
   </tr>

   <tr class="selected" id="DstSslPrivateKeyFileRow">
      <td class="left_column">Private key file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.SslPrivateKeyFile) ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('DstSslPrivateKeyFile', dest.SslPrivateKeyFile) ?>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected" id="DstSslVerifyPeerRow">
      <td class="left_column">Verify peer</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode ?> 
            <?cs if:dest.SslVerifyPeer ?>Yes<?cs else ?>No<?cs /if ?>          
         <?cs else ?>
            <input type="checkbox" class="no_style" id="DstInputSslVerifyPeer" name="DstSslVerifyPeer" <?cs if:dest.SslVerifyPeer ?>checked<?cs /if ?> onclick="showOrHideHttpRows();" />&nbsp;      
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected http_ssl_verify_peer_row" id="DstSslCertificateAuthorityFileRow" <?cs if:!dest.SslVerifyPeer ?>style="display:none;"<?cs /if ?> >
      <td class="left_column">Certificate authority file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <table class="inner">
            <tr>
               <td class="inner_left">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.SslCertificateAuthorityFile) ?>
            <?cs if:dest.SslCertificateAuthorityFile!='' ?>
               <a href="/view_ssl_cert.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&CertificateType=Authority&ComponentType=Destination">view certificate</a>
            <?cs /if ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('DstSslCertificateAuthorityFile', dest.SslCertificateAuthorityFile) ?>
         <?cs /if ?>
               </td>
               <td class="inner_right">
                  <?cs if:Channel.Destination.Error.SslCertificateAuthority ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Destination.Error.SslCertificateAuthority ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>     
         </table>
      </td>
   </tr>
   
<?cs /with ?>
