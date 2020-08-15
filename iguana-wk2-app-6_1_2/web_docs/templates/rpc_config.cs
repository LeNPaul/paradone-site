<?cs # vim: set syntax=html :?>

<?cs def:showError(Value) ?>

<?cs if:Value ?>
<br><span style="color:red;" id="<?cs name:Value ?>">This port is in use by <?cs var:Value ?><br/>To view a list of ports in use by Iguana visit <a href="port_status.html">Dashboard &gt; Ports</a>.</span>
<?cs else ?>
<span id="<?cs name:Value ?>"></span>
<?cs /if ?>
<?cs /def ?>

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt; <a href="/settings#Page=rpc_settings/view">Plugins</a> &gt; Change
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

               <?cs if:StatusMessage ?>
                  <h3><font color="green" id="test.StatusMessage"><?cs var:html_escape(StatusMessage) ?></font></h3>
               <?cs else ?>
	       	  <span id="test.StatusMessage"></span>
	       <?cs /if ?>

		<?cs if:RestartLink ?>
                <span id="divRefreshStatus"></span> <a href="<?cs var:html_escape(RestartLink) ?>">Continue</a> </p>
      <?cs /if ?>

      <!-- begin form -->
     <form name="rpc_settings" id="rpc_settings" method="post" action="">

         <table class="configuration" border="1" id="test.rpcserver">
         <tr class="header">
            <th colspan="2">Plugin Settings</th>
         </tr>
         <tr id="RpcPortRow">
           <td class="left_column">Plugin Communication Port</td>
           <td>
              <input class="text" type="text" name="RpcPort" id="RpcPort" value="<?cs var:html_escape(RpcPort) ?>">
                  <div id="RpcPort_preview_div" style="display:none;">
                     <div id="RpcPort_preview" class="path_preview" val="<?cs var:html_escape(environment_expand(RpcPort)) ?>"></div>
                  </div>
              <span id="RpcPortErrorContainer" class="validation_error_message_container">
           </td>
         </tr>

         </table>
<span id="test.RpcPortErrorMessage"> <?cs call:showError(RpcPort_ErrorString) ?></span>
           <br />
               <table id="buttons"><tr><td>
               <input type="hidden" class="hidden_submit" name="ApplyChanges" value="yes" />
               <a id="ApplyChanges" class="action-button blue">Save Changes and Restart Server</a>
               </td><td>
               <a class="action-button blue" id="RestoreDefaults">Reset to Default</a>
               </td><td>
               </td></tr></table>

      </form>
      <!-- end form -->

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
	       From this page, you can edit the plugin configuration settings.
	    </p>
	    <p>
	    Clicking the <b>Save Changes and Restart Server</b> button will save the new port and restart the Plugin Communication Server.
	    </p>
	 </td>
      </tr>
      <tr>
         <td class="side_item">
	    <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_config_plugin') ?>" target="_blank">Plugin Settings</a>
            	</li>
            </ul>
	 </td>
      </tr>
   </table>
<script type="text/javascript">
$(document).ready(function() {
   $("form#rpc_settings").submit({uri: ifware.SettingsScreen.page(), form_id: "rpc_settings"}, SettingsHelpers.submitForm);
   $("a#ApplyChanges").click(function(event) {
      event.preventDefault();
      $(this).blur();
      if (VALvalidateFields()) {
        $("form#rpc_settings").submit();  
      }
      
   });
   $("#RestoreDefaults").click(function(event) {
      event.preventDefault();
      SettingsHelpers.setDefaults(ifware.SettingsScreen.page());
   });

   $("#RpcPort").keyup(function() {
      expandFields(["RpcPort"]);
      VALclearError("RpcPortRow", "RpcPortErrorContainer");
   });

  VALregisterIntegerValidationFunction('RpcPort', 'RpcPortRow', 'RpcPortErrorContainer', null, null, 1, 65535, true);
  expandFields(['RpcPort']);
});
</script>
