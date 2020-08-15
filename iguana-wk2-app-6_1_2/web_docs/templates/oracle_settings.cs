<?cs include:"browse_macro.cs" ?>
   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt;
            <a href="/settings#Page=database_settings/view">Databases</a> &gt;
            OCI Oracle
         </td>
      </tr>

      <tr>
         <td id="dashboard_body">
            <center>

               <?cs if:ErrorMessage ?>
                  <h3><font color="red"><?cs var:html_escape(ErrorMessage) ?></font></h3>
               <?cs /if ?>

               <?cs if:StatusMessage ?>
                  <h3><font color="green"><?cs var:html_escape(StatusMessage) ?></font></h3>
               <?cs /if ?>

               <?cs if:!ErrorMessage && Oracle.NeedsIguanaRestart ?>
                  <h3><font color="blue">
                     You need to restart Iguana for these settings to take effect.
                  </font></h3>
               <?cs /if ?>

               <form id="db_settings" method="post">
                  <table class="configuration">
                     <tr>
                        <th colspan="2">OCI Oracle Library in Use</th>
                        <th>Version</th>
                     </tr>
                     <tr>
                        <?cs if: Oracle.LoadedDll.Path ?>
                           <td colspan="2"><?cs var:html_escape(Oracle.LoadedDll.Path) ?></td>
                           <td><?cs var:html_escape(Oracle.LoadedDll.Version) ?></td>
                        <?cs else ?>
                           <td colspan="3" align="center">None Loaded</td>
                        <?cs /if ?>
                     </tr>
                     <tr>
                        <th colspan="3">Preferred Version</th>
                     </tr>
                     <?cs def:preferredDllRadioInput(id,value) ?>
                        <?cs if: value == Oracle.PreferredDll.Path ?>
                           <?cs set:preferred_dll_is_selected = 1 ?>
                           <input id="<?cs var:id ?>" type="radio" class="no_style" name="PreferredDll"
                                 value="<?cs var:html_escape(value) ?>" checked />
                        <?cs else ?>
                           <input id="<?cs var:id ?>" type="radio" class="no_style" name="PreferredDll"
                                 value="<?cs var:html_escape(value) ?>" />
                        <?cs /if ?>
                     <?cs /def ?>
                     <tr onclick="document.getElementById('-auto-').checked = true;">
                        <td><?cs call:preferredDllRadioInput('-auto-','') ?></td>
                        <td>Automatically choose which OCI Oracle shared library to load.</td>
                        <td></td>
                     </tr>
                     <?cs each: dll = Oracle.AvailableDlls ?>
                        <tr onclick="document.getElementById('dll-<?cs var:name(dll) ?>').checked = true;">
                           <td><?cs call:preferredDllRadioInput('dll-' + name(dll), dll.Path) ?></td>
                           <td>
                              Use <?cs var:html_escape(dll.Path) ?>
                              <?cs if:dll.IsNew ?>
                                 <b style="color:#3333FF">*New!</b>
                              <?cs /if ?>
                           </td>
                           <td><?cs var:html_escape(dll.Version) ?></td>
                        </tr>
                     <?cs /each ?>
                     <tr onclick="document.getElementById('useCustomDll').checked = true;">
                        <td>
                           <?cs if:preferred_dll_is_selected ?>
                              <input id="useCustomDll" type="radio" class="no_style" name="PreferredDll" value="-custom-" />
                           <?cs else ?>
                              <input id="useCustomDll" type="radio" class="no_style" name="PreferredDll" value="-custom-" checked ?>
                           <?cs /if ?>
                        </td>
                        <td style="width:500px; border-width:0px;">
                           <?cs if:Oracle.PreferredDll.Path ?>
                              <?cs call:browse_input('CustomDll', Oracle.PreferredDll.Path) ?>
                           <?cs else ?>
                              <?cs call:browse_input('CustomDll', Oracle.LoadedDll.Path) ?>
                           <?cs /if ?>
                        </td>
                        <td/>
                     </tr>
                  </table>
                  <p>
                     <?cs if:Oracle.IgnoreNewDlls ?>
                        <input type="checkbox" class="no_style" name="WarnAboutNewDlls" />
                     <?cs else ?>
                        <input type="checkbox" class="no_style" name="WarnAboutNewDlls" checked />
                     <?cs /if ?>
                     Always warn when new versions of OCI Oracle become available (Windows only).
                  </p>
                  <table id="buttons"><tr><td>
                     <input type="hidden" name="Action" value="update" />
                     <a class="action-button blue" id="ApplyChanges">Save Changes</a>
                  </td></tr></table>
               </form>
               <br />
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
               <?cs if: !Oracle.LoadedDll.Path ?>
                  To enable OCI Oracle,
                  <?cs if: subcount(Oracle.AvailableDlls) ?>
                     select a detected client or
                  <?cs /if ?>

                     use the <i>Browse</i> button and locate

                  the correct client library on your system (oci.dll on Windows).
               <?cs elif: Oracle.PreferredDll.Path && Oracle.LoadedDll.Path != Oracle.PreferredDll.Path ?>
                  <?cs if: !ErrorMessage && Oracle.NeedsIguanaRestart ?>
                     <b>Iguana needs to be restarted to load your preferred version of OCI Oracle.</b>
                  <?cs else ?>
                     <b>Unable to load your preferred version of OCI Oracle.</b>
                     Please select another version.
                  <?cs /if ?>
               <?cs elif: Oracle.NewDllAvailable ?>
                  <?cs if: subcount(Oracle.AvailableDlls) == 1 ?>
                     <b>A new version of OCI Oracle is available.</b> Select the new version
                  <?cs else ?>
                     <b>New versions of OCI Oracle are available.</b> Select a new version
                  <?cs /if ?>
                  or confirm your original preference to hide
                  <?cs if: Oracle.IgnoreNewDlls ?>
                     this warning.
                  <?cs else ?>
                     the warning on the dashboard.
                  <?cs /if ?>
               <?cs elif: !ErrorMessage && Oracle.NeedsIguanaRestart && !Oracle.PreferredDll.Path ?>
                  <b>Iguana needs to be restarted to perform Oracle autodetection.</b>
                  You may safely ignore this warning, as another OCI Oracle library is already
                  loaded, but Iguana may load a different library at next start-up.
               <?cs else ?>
                  Here, you can choose the version of the OCI Oracle shared library that you want to use.
               <?cs /if ?>
            </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_database_version') ?>" target="_blank">Changing the Shared Library</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
<script type="text/javascript">
$(document).ready(function() {
   document.getElementsByName('CustomDll')[0].onfocus = function() {
      document.getElementById('useCustomDll').checked = true;
   }
   $("form#db_settings").submit({uri: ifware.SettingsScreen.page(), form_id: "db_settings"}, SettingsHelpers.submitForm);
   $("a#ApplyChanges").click(function(event) {
      event.preventDefault();
      $(this).blur();
      $("form#db_settings").submit();
   });
});
</script>
