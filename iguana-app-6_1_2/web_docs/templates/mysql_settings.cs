<?cs include:"browse_macro.cs" ?>
   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt;
            <a href="/settings#Page=database_settings/view">Databases</a> &gt;
            MySQL Compliant
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

               <?cs if:!ErrorMessage && MySql.NeedsIguanaRestart ?>
                  <h3><font color="blue">
                     You need to restart Iguana for these settings to take effect.
                  </font></h3>
               <?cs /if ?>

               <form id="db_settings" name="db_settings" method="post">
                  <table class="configuration">
                     <tr>
                        <th colspan="2">MySQL Compliant Library in Use</th>
                        <th>Version</th>
                     </tr>
                     <tr>
                        <?cs if: MySql.LoadedDll.Path ?>
                           <td colspan="2"><?cs var:html_escape(MySql.LoadedDll.Path) ?></td>
                           <td><?cs var:html_escape(MySql.LoadedDll.Version) ?></td>
                        <?cs else ?>
                           <td colspan="3" align="center">None Loaded</td>
                        <?cs /if ?>
                     </tr>
                     <tr>
                        <th colspan="2">Preferred Version</th>
                        <th>Version</th>
                     </tr>
                     <?cs def:preferredDllRadioInput(id,value) ?>
                        <?cs if: value == MySql.PreferredDll.Path ?>
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
                        <td>Automatically choose which MySQL compliant shared library to load.</td>
                        <td></td>
                     </tr>
                     <?cs each: dll = MySql.AvailableDlls ?>
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
                        <td style="width:500px; border-width: 0px;">
                           <?cs if:MySql.PreferredDll.Path ?>
                              <?cs call:browse_input('CustomDll', MySql.PreferredDll.Path) ?>
                           <?cs else ?>
                              <?cs call:browse_input('CustomDll', MySql.LoadedDll.Path) ?>
                           <?cs /if ?>
                        </td>
                        <td/>
                     </tr>
                  </table>
                  <p>
                     <?cs if:MySql.IgnoreNewDlls ?>
                        <input type="checkbox" class="no_style" name="WarnAboutNewDlls" />
                     <?cs else ?>
                        <input type="checkbox" class="no_style" name="WarnAboutNewDlls" checked />
                     <?cs /if ?>
                     Always warn when new versions of MySQL compliant libraries become available (Windows only).
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
               <?cs if: !MySql.LoadedDll.Path ?>
                  To enable MySQL compliant database,
                  <?cs if: subcount(MySql.AvailableDlls) ?>
                     select a detected client or
                  <?cs /if ?>

                     use the <i>Browse</i> button and locate

                  the correct client library on your system (libmySQL.dll or libmariadb.dll on Windows).
               <?cs elif: MySql.PreferredDll.Path && MySql.LoadedDll.Path != MySql.PreferredDll.Path ?>
                  <?cs if: !ErrorMessage && MySql.NeedsIguanaRestart ?>
                     <b>Iguana needs to be restarted to load your preferred version of the MySQL compliant database library.</b>
                  <?cs else ?>
                     <b>Unable to load your preferred version of MySQL compliant library.</b>
                     Please select another version.
                  <?cs /if ?>
               <?cs elif: MySql.NewDllAvailable ?>
                  <?cs if: subcount(MySql.AvailableDlls) == 1 ?>
                     <b>A new version of MySQL compliant library is available.</b> Select the new version
                  <?cs else ?>
                     <b>New versions of MySQL compliant libraries are available.</b> Select a new version
                  <?cs /if ?>
                  or confirm your original preference to hide
                  <?cs if: MySql.IgnoreNewDlls ?>
                     this warning.
                  <?cs else ?>
                     the warning on the dashboard.
                  <?cs /if ?>
               <?cs elif: !ErrorMessage && MySql.NeedsIguanaRestart && !MySql.PreferredDll.Path ?>
                  <b>Iguana needs to be restarted to perform MySQL autodetection.</b>
                  You may safely ignore this warning, as another MySQL compliant library is already
                  loaded, but Iguana may load a different library at next start-up.
               <?cs else ?>
                  Here, you can choose the version of the MySQL compliant shared library that you want to use.
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

