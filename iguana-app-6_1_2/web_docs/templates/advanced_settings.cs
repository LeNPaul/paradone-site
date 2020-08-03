<table id="iguana">
<tr>
<td id="cookie_crumb">Settings</td>
</tr>

<tr>
<td id="dashboard_body">

<h1 style="color: #4caf50; text-align: center; margin: 30px 0px -20px 0px; font-weight: 600;">Iguana Program Settings</h1>
    
<div class="section group settings_list"> 
   
   <div class="col span_1_of_2">

      <h1>Software License</h1>

      <h2><a href="/license_settings.html">Iguana ID & License Info</a></h2>
      <p>Obtain your Iguana ID and manage licensing details, including number of channels and license/maintenance expiry.</p>


      <h1>IGUANA Setup</h1>

      <h2><a href="#Page=web_settings/view">Web Server</a></h2>
      <p>Configure the built in web server used by Iguana's user interface and the label of the Iguana server.</p>

      <h2><a href="#Page=database_settings/view">Database</a></h2>
      <p>Manage the database libraries that Iguana is able to detect and access.</p>

      <h2><a href="#Page=log_settings/view">Logging</a></h2>
      <p>Configure the logs for the server. Logs can also be purged here.</p>

      <?cs if:EncryptedLogsAllowed ?>
         <h2><a href="log_locker_settings.html">Log Encryption</a></h2>
         <p>Change your encryption password and enable/disable auto-unlock.</p>
      <?cs /if ?>

      <h2><a href="#Page=rpc_settings/view">Plugins</a></h2>
      <p>Configure the port for plugins communicating on a dedicated TCP/IP server.</p>

      <h2><a href="#Page=https_channel_settings">HTTP(S) Channels</a></h2>
      <p>Set the web server that HTTP(S) channels use to service requests.</p>

      <h2><a href="auth_control.html">Authentication</a></h2>
      <p>Configure this Iguana instance to use an external authentication system.</p>

      <h1>Remote IGUANAS</h1>

      <h2><a href="#Page=remote_iguanas">Remote IGUANA Servers</a></h2>
      <p>Set up this Iguana to monitor multiple remote Iguanas.</p>

      <h2><a href="#Page=remote_controllers">Remote IGUANA Controllers</a></h2>
      <p>Allow remote Iguanas to display screens from this Iguana.</p>

      <h1>System Settings</h1>

      <h2><a href="#Page=email_status">Monitoring</a></h2>
      <p>Configure Iguana to send alerts about channel or system status by email or text.</p>  


      <h2><a href="/update_environment">Environment Variables</a></h2>
      <p>Administer environment variables that are local to this Iguana Server.</p>

   </div>

   <div class="col span_1_of_2">
    
      <h1>Users and Roles</h1>

      <h2><a href="#Page=users/edit?user=<?cs var:url_escape(CurrentUser) ?>">Edit Your Account</a></h2>
      <p>Update your user information, including e-mail, SMS, password, and allowed permissions (via roles).</p>

      <h2><a href="#Page=users">Users</a></h2>
      <p>Review, add, remove, or modify user accounts.</p>

      <h2><a href="#Page=roles">Roles</a></h2>
      <p>Create and remove roles, and set which roles have permission to perform which operations.</p>

      <h2><a href="sync_request.html">Synchronize</a></h2>
      <p>Copy users and roles to and from remote Iguana instances.</p>

      <h2><a href="#Page=channel/group">Channel Groups</a></h2>
      <p>Use channel groups to organize your channels, and manage permissions and notification rules in bulk.</p>  


      <h1>Import/Export</h1>

      <h2><a href="#Page=repositories">Add/Configure Repositories</a></h2>
      <p>Use local, remote or cloud-based Git repositories to store and transfer channels.</p>  

      <h2><a href="#Page=import">Import Channels</a></h2>
      <p>Bring channels into this Iguana instance from a repository.</p>  

      <h2><a href="#Page=export">Export Channels</a></h2>
      <p>Send channels from this Iguana instance to a repository.</p>  

      <?cs if:CurrentUserCanAdmin ?>
         <h1>Source Control Management</h1>
         <h2><a href="#Page=settings/history">Iguana History</a></h2>
         <p>Review configuration changes made to this Iguana instance, and optionally revert it to a previous state.</p>
         <h2><a href="#Page=channel_restore">Restore Deleted Channels</a></h2>
         <p>Examine past channel deletions and restore them to this Iguana instance.</p>  
      <?cs /if ?>
   
   </div>

</div>
    

    
</td>
</tr>

</table>
</div>

<br>

               	
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
               On this page, you can customize your version of Iguana by changing its settings.
            </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_settings') ?>" target="_blank">Changing the Iguana Program Settings</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
   
   <table id="side_table">
      <tr>
         <th id="side_header"> A Tour of Iguana </th>
      </tr>
      <tr>
         <td id="side_body">
            <p>
               New to Iguana? We've created a short tour to help get you started.
            </p>
            <p>
               <a class="action-button blue" onclick="ifware.ImportManager.takeTour();">Take the Tour</a>
            </p>
         </td>
      </tr>
   </table>
</div>
              	
               	<br><br><br>
           
            </center>
         </td>
      </tr>

   </table>

