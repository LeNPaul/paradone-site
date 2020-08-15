   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt; Databases
         </td>
      </tr>

      <tr>
         <td id="dashboard_body">
            <center>

               <?cs if:ErrorMessage  ?>
                  <h3><font color="red"><?cs var:html_escape(ErrorMessage) ?></font></h3>
               <?cs /if ?>

               <?cs if:StatusMessage ?>
                  <h3><font color="green"><?cs var:html_escape(StatusMessage) ?></font></h3>
               <?cs /if ?>

               <table class="configuration">
                  <tr>
                     <th>Database API</th>
                     <th>Path to Shared Library</th>
                     <th>Version</th>
                  </tr>
                  <tr>
                     <td class="left_column">MySQL Compliant</td>
                     <td><table style="border-collapse:collapse"><tr>
                        <td>
                           <?cs alt:html_escape(MySql.LoadedDll.Path) ?> Not Loaded. <?cs /alt ?>
                        </td>
                        <?cs if:html_escape(UserCanAdmin) == 1?>
                           <td>
                              <a class="action-button-small blue" href="/settings#Page=database_settings/edit/mysql">Edit</a>
                           </td>
                        <?cs /if ?>
                     </tr></table></td>
                     <td>
                        <?cs var:html_escape(MySql.LoadedDll.Version) ?>
                     </td>
                  </tr>
                  <tr>
                     <td class="left_column">OCI Oracle</td>
                     <td><table style="border-collapse:collapse"><tr>
                        <td>
                           <?cs alt:html_escape(Oracle.LoadedDll.Path) ?> Not Loaded. <?cs /alt ?>
                        </td>
                        <?cs if:html_escape(UserCanAdmin) == 1?>
                           <td>
                              <a class="action-button-small blue" href="/settings#Page=database_settings/edit/oracle">Edit</a>
                           </td>
                        <?cs /if ?>
                     </tr></table></td>
                     <td>
                        <?cs var:html_escape(Oracle.LoadedDll.Version) ?>
                     </td>
                  </tr>
                  <tr>
                     <td class="left_column">ODBC</td>
                     <td>
                        <?cs alt:html_escape(Odbc.LoadedDll.Path) ?> Not Loaded. <?cs /alt ?>
                     </td>
                     <td><?cs var:html_escape(Odbc.LoadedDll.Version) ?></td>
                  </tr>

	       </table>

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
               On this page, you can view the database client libraries in use by Iguana.
            </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_config_database') ?>" target="_blank">Database Settings</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
