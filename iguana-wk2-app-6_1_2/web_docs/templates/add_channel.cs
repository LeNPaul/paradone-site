<?cs # vim: set syntax=html :?>

<table id="iguana">
   <tr>
      <td id="cookie_crumb">
         <a href="/">Dashboard</a> &gt; Add Channel
      </td>
   </tr>

   <tr>
      <td id="dashboard_body">
         <center>

            <h2>Add Channel</h2>
            
            <?cs if: !CurrentUserCanAdmin && !ErrorMessage ?>
               <?cs set:ErrorMessage = 'Administrative permissions required to add a channel.' ?>
            <?cs /if ?>
            
            <?cs if:ErrorMessage ?>
               <h3><font color="red"><?cs var:html_escape(ErrorMessage) ?></font></h3>
            <?cs /if ?>
            
            <?cs if:StatusMessage ?>
               <h3><font color="green"><?cs var:html_escape(StatusMessage) ?></font></h3>
            <?cs /if ?>

            <?cs if:CurrentUserCanAdmin ?>
            <form id="NewChannel" method="post" action="/channel/control">
         
               <table class="configuration" style="width: 450px">
                  <tr>
                     <th colspan="2">Select Channel Components</th>
                  </tr>
                  <tr>
                     <td class="left_column">Source</td>
                     <td>
                        <select id="selectSource" class="configuration" name="SourceType">
                           <?cs each: source_type = SourceTypes ?>
                              <option value="<?cs var:html_escape(source_type) ?>"><?cs var:html_escape(source_type) ?></option>
                           <?cs /each ?>
                        </select>
                     </td>
                  </tr>
                  <tr>
                     <td class="left_column">Destination</td>
                     <td>
                        <select id="selectDestination" class="configuration" name="DestinationType">
                           <?cs each: destination_type = DestinationTypes ?>
                              <option value="<?cs var:html_escape(destination_type) ?>"><?cs var:html_escape(destination_type) ?></option>
                           <?cs /each ?>
                        </select>
                     </td>
                  </tr>
               </table>

               <input type="hidden" name="ConfigureChannel" value="Configure Channel" />
               
               <table id="buttons">
                  <tr>
                     <td>
                        <a class="action-button blue" id="BeginChannelConfig">Start Configuring</a>
                        <?cs if:CurrentUserCanAdmin ?>
                           <span style="padding: 0 12px 0 8px">or</span>
                           <a class="" id="import-channel" href="/settings#Page=import">Import a channel</a>
                        <?cs /if ?>
                     </td>
                  </tr>
               </table>
               
            </form>
            <?cs /if ?>
            
            
         </center>
      </td>
   </tr>

</table>

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
	         <p>On this page, you select the source and destination components for the channel you are creating.</p>
            <p>When you have selected the components, click <b>Configure Channel</b> to configure the channel.</p>
            <p><b>To Translator</b> is recommended for many uses including calling web services and external programs.</p>
         </td>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_selecting_channel') ?>" target="_blank">Selecting the Channel Components</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
</div>

<script type="text/javascript">
$(document).ready(function() {
   SettingsHelpers = initSettingsHelpers();
   $("form#NewChannel").submit({uri: '/channel/control', form_id: "NewChannel"}, SettingsHelpers.submitForm);
   $("a#BeginChannelConfig").click(function(event) {
      $("form#NewChannel").submit();
   });
});
</script>

