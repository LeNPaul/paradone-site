<?cs include:"browse_macro.cs" ?>
<style type="text/css">
td.fixed_width {
   width: 200px;
}
tr.https_channel_server_row{
   display: <?cs if:UseHttpsChannelServer ?>''<?cs else ?>none<?cs /if ?>;
}
tr.https_channel_verify_peer_row{
   display: <?cs if:VerifyPeer && UseHttpsChannelServer ?>''<?cs else ?>none<?cs /if ?>;
}
</style>
<script type="text/javascript">
   function hideOrShowSslRows(){
<?cs if:ReadOnly ?>
      var ShowOrHide = <?cs if:UseHttpsChannelServer ?>true<?cs else ?>false<?cs /if ?>;
      var VerifyPeer = <?cs if:VerifyPeer ?>true<?cs else ?>false<?cs /if ?>;
      var UseHttps = <?cs if:UseHttps ?>true<?cs else ?>false<?cs /if ?>;
      var ServeFiles = <?cs if:ServeFiles ?>true<?cs else ?>false<?cs /if ?>;
<?cs else ?>
      var ShowOrHide = $('#radUseHttpsChannelServer').prop('checked');
      var VerifyPeer = $('#cbVerifyPeer').prop('checked');
      var UseHttps =  $('#cbUseHttps').prop('checked');
      var ServeFiles = $('#ServeFilesCb').prop('checked');
<?cs /if ?>

      var jAllChannelRows = $('.https_channel_server_row', $('#https_settings_table') );
      jAllChannelRows.each( function(){
         (ShowOrHide ? $(this).show() : $(this).hide()  );
      });
      if (ShowOrHide && !UseHttps) {
         $('.https_mode',$('#https_settings_table')).hide();
      }
      var ShowOrHideVerifyPeer = ShowOrHide && UseHttps && VerifyPeer;
      var jVerifyPeerRows = $('.https_channel_verify_peer_row', $('#https_settings_table') );
      jVerifyPeerRows.each( function(){
         (ShowOrHideVerifyPeer ? $(this).show() : $(this).hide()  );
      });
      if (ShowOrHide && !ServeFiles){
         $('.serve_files',$('#https_settings_table')).hide();
      }
   }
</script>
<?cs def:showError(Value, EscapeHtml) ?>

<?cs if:Value ?>
<br><span style="color:red;" id="<?cs name:Value ?>"><?cs if:EscapeHtml ?><?cs var:html_escape(Value) ?><?cs else ?><?cs var:Value ?><?cs /if ?></span>
<?cs else ?>
<span id="<?cs name:Value ?>"></span>
<?cs /if ?>
<?cs /def ?>

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt; HTTP(S) Channel Settings
         </td>
      </tr>

      <tr>
         <td id="dashboard_body">
            <center>
               <?cs if:ErrorMessage ?>
                  <h3><font color="red" id="test.ErrorMessage"><?cs var:html_escape(ErrorMessage) ?></font></h3>
	       <?cs else ?>
	          <span id="test.ErrorMessage"></span>
               <?cs /if ?>

               <?cs if:StatusMessage  ?>
                  <h3><font color="green" id="test.StatusMessage"><?cs var:html_escape(StatusMessage) ?></font></h3>
               <?cs else ?>
	       	  <span id="test.StatusMessage"></span>
	       <?cs /if ?>

         <form name="https_channel_settings_form" id="https_channel_settings_form">
         <input type="hidden" name="Mode" value="apply_changes" />
         <table class="configuration" border="1" style="width:700px" id="https_settings_table">
         <tr class="header">
            <th colspan="2">HTTP(S) Channel Settings</th>
         </tr>
         <tr>
           <td class="left_column fixed_width">From HTTP Channels Server</td>
           <td>
           <?cs if:ReadOnly ?>
             <?cs if:UseHttpsChannelServer ?>
                Use dedicated port
             <?cs else ?>
                Use web server port
             <?cs /if ?>
           <?cs else ?>
             <input type="radio" onclick="hideOrShowSslRows();" id="radUseHttpsChannelServer" name="UseHttpsChannelServer" value="use_https_server" <?cs if:UseHttpsChannelServer ?>checked<?cs /if ?>>Use dedicated port
             <br><input type="radio" onclick="hideOrShowSslRows();" id="radUseWebServer" name="UseHttpsChannelServer" value="use_web_server" <?cs if:!UseHttpsChannelServer ?>checked<?cs /if ?>>Use web server port
           <?cs /if ?>
	   </td>
         </tr>
         <?cs if:ReadOnly ?>
         <tr class="https_channel_server_row" >
           <td class="left_column fixed_width">Server Running</td>
           <td><?cs if:ServerRunning ?>Yes<?cs else ?>No<?cs /if ?></td>
         </tr>
         <?cs /if ?>
         <tr class="https_channel_server_row" id="WebPortRow" >
           <td class="left_column fixed_width">Port</td>
           <td>
             <?cs if:ReadOnly ?>
                <?cs var:html_escape(WebPort) ?>
                <?cs if:html_escape(WebPort) != html_escape(environment_expand(WebPort)) ?>
               <span style="letter-spacing:-2px;">&ndash;&gt;&nbsp;</span>
               <?cs var:html_escape(environment_expand(WebPort)) ?>
            <?cs /if ?>
             <?cs else ?>
                <input type="text" id="WebPort" name="WebPort" class="full_length" onkeyup="expandFields(['WebPort']);" value="<?cs var:html_escape(WebPort) ?>">
                <div id="WebPort_preview_div" style="display:none;">
                  <div id="WebPort_preview" class="path_preview"></div>
                </div>
             <?cs /if ?>
             <?cs call:showError(WebPort_ErrorString, 0) ?>
           </td>
         </tr>
         <tr class="https_channel_server_row">
           <td class="left_column fixed_width">Use HTTPS</td>
           <td>
             <?cs if:ReadOnly ?>
                <?cs if:UseHttps ?>Yes<?cs else ?>No<?cs /if ?>
             <?cs else ?>
                <input type="checkbox" onClick="hideOrShowSslRows();" id="cbUseHttps" name="UseHttps" <?cs if:UseHttps ?>checked<?cs /if ?> >
             <?cs /if ?>
           </td>
         </tr>
         <tr class="https_channel_server_row https_mode">
            <td class="left_column fixed_width">Certificate File</td>
            <td>
              <?cs if:ReadOnly ?>
                  <?cs call:browse_readonly(CertificateFile) ?>
                  <?cs if:!Ssl_ErrorString ?>
                     <a href="/view_ssl_cert.html?Mode=https_channel_server">view certificate</a>
                  <?cs /if ?>
              <?cs else ?>
                  <?cs call:browse_input('CertificateFile', CertificateFile) ?>
              <?cs /if ?>
              <br><?cs call:showError(CertificateFile_ErrorString, 1) ?>
            </td>
         </tr>
         <tr class="https_channel_server_row https_mode">
           <td class="left_column fixed_width">Private Key File</td>
           <td>
              <?cs if:ReadOnly ?>
                 <?cs call:browse_readonly(PrivateKeyFile) ?>
              <?cs else ?>
                  <?cs call:browse_input('PrivateKeyFile', PrivateKeyFile) ?>
              <?cs /if ?>
          <br><?cs call:showError(PrivateKeyFile_ErrorString, 1) ?>
           </td>
         </tr>
         <tr class="https_channel_server_row https_mode">
           <td class="left_column fixed_width">Verify Peer</td>
           <td>
             <?cs if:ReadOnly ?>
                <?cs if:VerifyPeer ?>Yes<?cs else ?>No<?cs /if ?>
             <?cs else ?>
                <input type="checkbox" onClick="hideOrShowSslRows();" id="cbVerifyPeer" name="VerifyPeer" <?cs if:VerifyPeer ?>checked<?cs /if ?> >
             <?cs /if ?>
           </td>
         </tr>
         <tr class="https_channel_verify_peer_row https_mode">
           <td class="left_column fixed_width">Certificate Authority File</td>
           <td>
             <?cs if:ReadOnly ?>
               <?cs call:browse_readonly(CertificateAuthorityFile) ?>
               <?cs if:!CertificateAuthorityFile_ErrorString ?>
                     <a href="/view_ssl_cert.html?Mode=https_channel_server&CertificateType=Authority">view certificate</a>
               <?cs /if ?>
             <?cs else ?>
                  <?cs call:browse_input('CertificateAuthorityFile', CertificateAuthorityFile) ?>
             <?cs /if ?>
             <br><?cs call:showError(CertificateAuthorityFile_ErrorString, 1) ?>
           </td>
         </tr>
         <tr class="https_channel_server_row">
           <td class="left_column fixed_width">Serve Files From a Directory</td>
           <td>
             <?cs if:ReadOnly ?>
                <?cs if:ServeFiles ?>Yes<?cs else ?>No<?cs /if ?>
             <?cs else ?>
                <input name="ServeFiles" onchange="hideOrShowSslRows()" id="ServeFilesCb" type="checkbox"
                   <?cs if:ServeFiles ?>checked<?cs /if ?>
                   />
             <?cs /if ?>
           </td>
         </tr>
         <tr class="https_channel_server_row serve_files">
           <td class="left_column fixed_width">Directory for Files</td>
           <td>
             <?cs if:ReadOnly ?>
                <?cs call:browse_readonly(ServeFilesDirectory) ?>
             <?cs else ?>
                <?cs call:browse_input_folder('ServeFilesDirectory', ServeFilesDirectory) ?><br>
             <?cs /if ?>
             <?cs call:showError(ServeFilesDirectory_ErrorString, 1) ?>
           </td>
         </tr>
         </table>
         </form>

      <div id="buttons">
      <!-- edit link -->
      <?cs if:CanEdit ?>
         <?cs if:ReadOnly ?>
            <a id="EditLink" class="action-button blue" href="/settings#Page=https_channel_settings?Mode=edit" onclick="this.blur();">Edit</a>
         <?cs else ?>
            <a class="action-button blue" id="SaveChanges">Apply Changes</a>
         <?cs /if ?>
      <?cs /if ?>
      </div>

      <!-- end display area -->
            </center>
         </td>
      </tr>

   </table>

</div>
<div id="side_panel">
   <table id="side_table">
      <tr>
         <th id="side_header">
            Page Help
         </th>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Overview</h4>
	     <p>
	 On this page, you can view the <br>HTTP(S) channel server configuration for Iguana.
	 </p>
   <?cs if:CanEdit ?>
	<p>Click <b>Edit</b> to change the settings on this page.</p>
   <?cs else ?>
      <p>You must be logged in as an administrator to edit the settings on this page.</p>
   <?cs /if ?>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_https_channel_settings') ?>" target="_blank">HTTP(S) Channel Settings</a>
                </li>
                <li>
                <a href="http://wiki.interfaceware.com/745.html?v=6.0.0" target="_blank">Serving Files from the Web Service (From HTTP Channel Server) Port</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>

<script type="text/javascript">
$(document).ready(function() {
   function handleOnLoad() {
      hideOrShowSslRows();
      expandFields(["WebPort"]);
   }

   handleOnLoad();

   $("form#https_channel_settings_form").submit({uri: '/https_channel_settings', form_id: "https_channel_settings_form"}, SettingsHelpers.submitForm);
   
   $("a#SaveChanges").click(function(event) {
      event.preventDefault();
      if (VALvalidateFields()) {
        $("form#https_channel_settings_form").submit();
      }
   });

   // VALregisterIntegerValidationFunction('WebPort', 'WebPortRow', 'WebPortErrorMessageContainer', null, null, 1, 65535, true);
   
   var HomeHash = '#Page=https_channel_settings';
   
   $("#EditLink").click(function(event) {
     if (document.location.hash != HomeHash) {
        window.location.reload();
        return;
      }
   return true;
   });
});
</script>


