      <div id="divAjaxError" class="ajax_error" style="display: none">
         <span class="ajax_error_heading">ERROR!</span>
         <div id="divAjaxErrorDescription" class="ajax_error_text"></div>
         <div id="divAjaxErrorDetailsShowLink">[<a href="#" onclick="DASHshowErrorDetails(); return false;">show details</a>]</div>
         <div id="divAjaxErrorDetails" class="ajax_error_text" style="display:none;"></div>
         <div id="divAjaxErrorDetailsHideLink" style="display:none;">[<a href="#" onclick="DASHhideErrorDetails(); return false;">hide details</a>]</div>
      </div>
          
      <div id="spnPanicError" class="error_message" style="display: <?cs if:Service.PanicErrorString ?>''<?cs else ?>none;<?cs /if ?>">
         <div class="error_heading">
            FATAL ERROR
         </div>
         <div class="error_text">
            <span id="spnPanicErrorText"><?cs var:Service.PanicErrorString ?></span>
         </div>
      </div>
         
      <div id="warningMaintenanceExpired" style="display: none" >
         <div class="error_message">
            <div class="error_heading">ERROR</div>
            <div class="error_text">
               Your current license is not compatible with this release of Iguana. Please update your license to be able to start channels.
               You can view or update your license at the <a href="/license_settings.html">Settings &gt; License Entitlement</a> page, 
               or contact your account representative to obtain additional licenses.
            </div>
         </div>
      </div>
         
      <div id="warningLicenseExpired" style="display: none" >
         <div class="error_message">
            <div class="error_heading">ERROR</div>
            <div class="error_text">
               Your current license has expired. Please update your license to be able to start channels.
               You can view or update your license at the <a href="/license_settings.html">Settings &gt; License Entitlement</a> page, 
               or contact your account representative to obtain additional licenses.
            </div>
         </div>
      </div>
         
      <div id="warningLicenseExpiringSoon" style="display: none" >
         <div class="warning_message">
            <div class="warning_heading">WARNING</div>
            <div class="warning_text">
               Your current license will expire soon.
               You can view or update your license at the <a href="/license_settings.html">Settings &gt; License Entitlement</a> page, 
               or contact your account representative to obtain additional licenses.
            </div>
         </div>
      </div>

      <div id="divNoChannels" style="display: none" >
         <div class="info_message">
            <div class="info_heading">You don't have any channels yet</div>
            <div class="info_text">
      <?cs if:CurrentUserCanAdmin ?>
               Add your first channel with the "Add Channel" button below. Or, <a href="http://training.interfaceware.com/course/first-steps/" target="_blank">visit our training center for a 30 minute course/tutorial</a> that will teach you the basics of building interfaces faster with Iguana.
      <?cs else ?>
               <a href="http://training.interfaceware.com/course/first-steps/" target="_blank">Visit our training center for a 30 minute course/tutorial</a> that will teach you the basics of building interfaces faster with Iguana.
      <?cs /if ?>
            </div>
         </div>
      </div>

      <?cs if:XmlFilterConflict ?>
         <div class="error_message">
            <div class="error_heading">ERROR</div>
            <div class="error_text">
               <?cs if:XmlFilterConflict ?>
                  <p>Iguana allows you to define message filters for each channel.  The To/From XML File components
                  have been changed to To/From File components with XML transformation filters.
                  However, conflicts were encountered while upgrading these channels.
                  For details, view the <a href="/log_browse?Source=+Iguana&DateFrom=<?cs var:XmlFilterConflictLogDate ?>&Position=<?cs var:XmlFilterConflictLogPosition ?>">
                  log entry</a>.</p>
               <?cs /if ?>
            </div>
         </div>
      <?cs /if ?>
 
      <div class="error_message" id="panelUserLoginMessage" style="display:none" >
         <div style="margin-left: 0; float:right"><img onclick="DASHclearUserLoginMessage()" src="/<?cs var:skin("images/ex14.gif") ?>" alt="" /></div>
         <div class="error_heading">ERROR</div>
         <div class="error_text" id="divUserLoginMessage"></div>
      </div>

      <div id="spnEmailSettingsChanged" style="display: <?cs if:EmailSettingsChanged ?>''<?cs else ?>none<?cs /if ?>" >
         <div class="warning_message">
            <div class="warning_text">
               Note: Iguana's email notification system allows users to define a set of
               email notification rules.
               You can view or edit these rules from the
               <a href="/settings#Page=email_settings/view">Settings &gt; Email Notification</a> page.
               Email server settings have been automatically migrated.
               For more details, view the <a href="/log_browse?Source=+Iguana&DateFrom=<?cs var:EmailSettingsLogDate ?>&Position=<?cs var:EmailSettingsLogPosition ?>">log entry</a>.
            </div>
            <div style="height: 15px;"><!-- spacer to get a min height from container --></div>
         </div>
      </div>
            
      <div id="spnEmailNotSetup" style="display: <?cs if:!EmailSettingsChanged && EmailNotSetup ?>''<?cs else ?>none<?cs /if ?>" >
         <div class="warning_message">
            <div class="warning_heading">
               WARNING
            </div>
            <div class="warning_text">
            Email notification settings have not been configured.  
            To configure or disable email notification, go to the
            <a href="/settings#Page=email_status">Settings &gt; Email Notification</a> page.
            </div>
         </div>
      </div>

      <div id="lowDiskSpaceMessage" style="display: <?cs if:WarnDiskSpace ?>''<?cs else ?>none<?cs /if ?> ">
         <div class="warning_message">
            <div class="warning_heading">
               WARNING
            </div>
            <div class="warning_text">
               The log partition is running out of space: <?cs var:html_escape(DiskSpaceRemaining) ?> remain.
               <?cs if: AverageUsage != 0 ?>
                  At the current rate of <?cs var:html_escape(AverageUsage) ?>/day (estimated), the
                  partition is likely to be exhausted before Iguana purges old logs.
               <?cs /if ?>
               Free some space on the partition, or adjust
               the <a href="/settings#Page=log_settings/view" >maximum&nbsp;log&nbsp;age</a>. 
               See <a href="/log_usage_statistics" >Logs&nbsp;&gt;&nbsp;Log Usage</a> for log usage statistics.
            </div>
         </div>
      </div>
                           
      <?cs if:MySql.InUse ?>
         <?cs if: !MySql.LoadedDll.Path ?>
            <div class="error_message">
               <div class="error_heading">
                  ERROR
               </div>
               <div class="error_text">
               Iguana was unable to load a MySQL client library.  To specify which version of
               MySQL you would like to use, visit the
               <a href="/settings#Page=database_settings/edit/mysql">Settings &gt; Database &gt; MySQL</a> page.
               </div>
            </div>
         <?cs elif: MySql.PreferredDll.Path && MySql.LoadedDll.Path != MySql.PreferredDll.Path ?>
            <div class="warning_message">
               <div class="warning_heading">
                  WARNING
               </div>
               <div class="warning_text">
               Iguana was unable to load your preferred version of MySQL.  Iguana is currently
               using v<?cs var:html_escape(MySql.LoadedDll.Version) ?>.  To change which 
               version of MySQL you would like to use, or disable this message, visit the
               <a href="/settings#Page=database_settings/edit/mysql">Settings &gt; Database &gt; MySQL</a> page.
               </div>
            </div>
         <?cs elif: MySql.NewDllAvailable
                 && subcount(MySql.AvailableDlls) == 1
                 && MySql.LoadedDll.Path == MySql.AvailableDlls.0.Path ?> 
            <!-- This is the case where most MySQL users will fall into.  Iguana has detected
                 a version of MySQL, the one they've been using, so it isn't really new and
                 we shouldn't bother them about it. -->
         <?cs elif: MySql.NewDllAvailable && !MySql.IgnoreNewDlls ?>
            <div class="warning_message">
               <div class="warning_text">
               Another version of MySQL is available.  Iguana is currently using
               v<?cs var:html_escape(MySql.LoadedDll.Version) ?>.  To change which
               version of MySQL you would like to use, or disable this message, visit the
               <a href="/settings#Page=database_settings/edit/mysql">Settings &gt; Database &gt; MySQL</a> page.
               </div>
            </div>
         <?cs /if ?>
      <?cs /if ?>

      <?cs if:Oracle.InUse ?>
         <?cs if: Oracle.PreferredDll.Path && Oracle.NeedsIguanaRestart ?>
            <div class="warning_message">
               <div class="warning_heading">
                  WARNING
               </div>
               <div class="warning_text">
               Iguana needs to be restarted before your selected version of Oracle OCI
               can be loaded.  To change the version of OCI Oracle you would like to
               use, visit the
               <a href="/settings#Page=database_settings/edit/oracle">Settings &gt; Database &gt; OCI Oracle</a> page.
               </div>
            </div>
         <?cs elif: !Oracle.LoadedDll.Path ?>
            <div class="error_message">
               <div class="error_heading">
                  ERROR
               </div>
               <div class="error_text">
               Iguana was unable to load an OCI Oracle client library.  To specify which version of
               OCI Oracle you would like to use, visit the
               <a href="/settings#Page=database_settings/edit/oracle">Settings &gt; Database &gt; OCI Oracle</a> page.
               </div>
            </div>
         <?cs elif: Oracle.PreferredDll.Path && Oracle.LoadedDll.Path != Oracle.PreferredDll.Path ?>
            <div class="warning_message">
               <div class="warning_heading">
                  WARNING
               </div>
               <div class="warning_text">
               Iguana was unable to load your preferred version of OCI Oracle.  Iguana is currently
               using v<?cs var:html_escape(Oracle.LoadedDll.Version) ?>.  To change which 
               version of OCI Oracle you would like to use, or disable this message, visit the
               <a href="/oracle_settings.html">Settings &gt; Database &gt; OCI Oracle</a> page.
               </div>
            </div>
         <?cs elif: Oracle.NewDllAvailable
                 && subcount(Oracle.AvailableDlls) == 1
                 && Oracle.LoadedDll.Path == Oracle.AvailableDlls.0.Path ?> 
            <!-- This is the case where most OCI Oracle users will fall into.  Iguana has detected
                 a version of OCI Oracle, the one they've been using, so it isn't really new and
                 we shouldn't bother them about it. -->
         <?cs elif: Oracle.NewDllAvailable && !Oracle.IgnoreNewDlls ?>
            <div class="warning_message">
               <div class="warning_text">
               Another version of OCI Oracle is available.  Iguana is currently using
               v<?cs var:html_escape(Oracle.LoadedDll.Version) ?>.  To change which
               version of OCI Oracle you would like to use, or disable this message, visit the
               <a href="/settings#Page=database_settings/edit/oracle">Settings &gt; Database &gt; OCI Oracle</a> page.
               </div>
            </div>
         <?cs /if ?>
      <?cs /if ?>

      <?cs if:RpcConnectionError ?>
         <div id="divRpcConnectionError" class="ajax_error">
            <div id="divRpcConnectionErrorDetails" class="ajax_error_text">
            <?cs if:RpcWebPortConflict ?>
            Plugin Communication port is in use by the Iguana Web Server.<br />
            <?cs else ?>
            Plugin Communication port is in use by another application.<br />
            <?cs /if ?>
            Go to <a href="/settings#Page=rpc_settings/view">Settings > Plugins</a> to choose a different port for the Plugin Communication Server.
            </div>
         </div>
      <?cs /if ?>

      <?cs if:HttpsChannelServerConnectionError ?>
         <div id="divHttpsChannelServerConnectionError" class="ajax_error">
            <div id="divHttpsChannelServerConnectionErrorDetails" class="ajax_error_text">
            <?cs if:HttpsChannelServerWebPortConflict ?>
            HTTPS Channel Server port is in use by the Iguana Web Server.<br />
            <?cs else ?>
            HTTPS Channel Server port is in use by another application.<br />
            <?cs /if ?>
            Go to <a href="/settings#Page=https_channel_settings">Settings > HTTPS Channel Settings</a> to choose a different port for the HTTPS Channel Server.
            </div>
         </div>
      <?cs /if ?>       
