<?cs # vim: set syntax=html :?>

<?cs def:showError(Value) ?>
 <?cs if:Value ?>
  <span id="<?cs name:Value ?>" class="error_message">This port is in use by <?cs var:Value ?><br/>To view a list of ports in use by Iguana visit <a href="port_status.html">Dashboard &gt; Ports</a>.</span>
 <?cs else ?>
  <span id="<?cs name:Value ?>" class="error_message"></span>
 <?cs /if ?>
<?cs /def ?>

<table id="iguana">
   <tr>
      <td id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; Plugins
      </td>
   </tr>

   <tr>
      <td id="dashboard_body">
         <center>
         <?cs if:ErrorMessage ?>
            <h3><span class="error_message" id="test.ErrorMessage"><?cs var:html_escape(ErrorMessage) ?></span></h3>
         <?cs else ?>
            <span id="test.ErrorMessage"></span>
         <?cs /if ?>

         <?cs if:StatusMessage ?>
            <h3><span class="status_message" id="test.StatusMessage"><?cs var:html_escape(StatusMessage) ?></span></h3>
         <?cs else ?>
            <span id="test.StatusMessage"></span>
         <?cs /if ?>

         <?cs if:RestartLink ?>
            <p><span id="divRefreshStatus"></span> <a href="<?cs var:html_escape(RestartLink) ?>">Continue</a></p>
         <?cs /if ?>

         <form name="rpc_settings_view.html" id="rpc_settings_view.html" method="post">

            <table class="configuration" border="1" id="test.rpcserver">
               <tr class="header">
                  <th colspan="2">Plugin Settings</th>
               </tr>
               <tr>
                 <td class="left_column">Plugin Communication Port</td>
                 <td>
                  <?cs var:html_escape(RpcPort) ?>
                  <?cs if:html_escape(RpcPort) != html_escape(environment_expand(RpcPort)) ?>
                     <span style="letter-spacing:-2px;">&ndash;&gt;&nbsp;</span>
                     <?cs var:html_escape(environment_expand(RpcPort)) ?>
                  <?cs /if ?>
                 </td>
               </tr>
            </table>
            <span><?cs call:showError(RpcPort_ErrorString) ?></span>
         </form>

         <div id="buttons">
               <!-- edit link -->
               <?cs if:CanAdmin && !HasActiveRpcChannel ?>
                  <a id="edit_button" class="action-button blue" href="#Page=rpc_settings/edit">Edit</a>
               <?cs /if ?>

               <?cs if:RpcServerNotStarted ?>
                  <?cs if:CanAdmin && !HasActiveRpcChannel ?>
                     <a class="action-button blue" href="#Page=rpc_settings/view?RestartServer=true">Start Server</a>
                  <?cs /if ?>
               <?cs else ?>
                  <?cs if:CanAdmin && !HasActiveRpcChannel ?>
                     <a class="action-button blue" href="#Page=rpc_settings/view?RestartServer=true">Restart Server</a>
                  <?cs /if ?>
               <?cs /if ?>

         </div>

         <?cs if:HasActiveRpcChannel && CanAdmin ?>
            <p>
               <span class="admin_only_privilleges">
                  All channels with plugin components must be stopped before editing the plugin settings.
                  <br>Go to the <a href="/">dashboard</a> to stop the required channels.
               </span>
            </p>
         <?cs /if ?>
         </center>
      </td>
   </tr>
</table>

<div id="side_panel">
   <table id="side_table">
      <tr>
         <th id="side_header"> Page Help </th>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Overview</h4>
            <p> This page displays the plugin configuration settings.  </p>
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
</div>

<script type="text/javascript">

$(document).ready(function() {

<?cs if:CanAdmin ?>
   $("#edit_button").on("click", function(event) {
      SettingsHelpers.routeTo(this.hash);
   });
<?cs /if ?>

<?cs if:RestartLink ?>
   SettingsHelpers.submitRestartForm();
   var CountDown = 5;
   console.log(location.hash);
   setTimeout(function() {
      SettingsHelpers.onTimer('<?cs var:RestartLink ?>', CountDown, function(){ return location.hash.indexOf("#Page=rpc_settings") > -1; });
   }, 1000);
<?cs /if ?>
});
</script>

