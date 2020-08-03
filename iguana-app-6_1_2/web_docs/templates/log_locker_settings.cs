<?cs include:"doctype.cs" ?>
<html>
   <head>
      <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Settings &gt; License Entitlement</title>
      <?cs include:"browser_compatibility.cs" ?>
      <?cs include:"styles.cs" ?>
      <link rel="stylesheet" type="text/css" href="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css">
      <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
      <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/startup_status.js") ?>"></script>
      <script type="text/javascript" src="<?cs var:iguana_version_js("/js/cookie/cookiev4.js") ?>"></script>
      <script type="text/javascript" src="<?cs var:iguana_version_js("log_locker_controller.js") ?>"></script>
      <script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>
      <script type="text/javascript" src="<?cs var:iguana_version_js("tooltip.js") ?>"></script>
      <script type="text/javascript" src="<?cs var:iguana_version_js("templates/file-browser.js") ?>"></script>
      <?cs include:"license_js_files.cs" ?>
      <?cs include:"mini-login.cs" ?>
      <?cs include:"file-browser.cs" ?>
      <?cs include:"log_locker_dialogs.cs" ?>
      <script>
         $(document).click(function checkStatus() {
            MiniLogin.loggedIn();
         });
      </script>
   </head>
   <body class="tabright register-mini-login-click-handler">
      <?cs set:Navigation.CurrentTab = "Settings" ?>
      <?cs include:"header.cs" ?>
      <div id="main">
         <table id="iguana"><tbody>
            <tr> 
               <td id="cookie_crumb"><a href="/settings">Settings</a> &gt; Log Encryption</td> 
            </tr>

            <tr>
               <td id="dashboard_body" class="log_locker_settings">
                  <center>
                     <?cs if:ErrorMessage?>
                        <span id="ErrorMessage"><h3 style="color:red;"><?cs var:html_escape(ErrorMessage) ?></h3></span>
                     <?cs else ?>
                        <span id="ErrorMessage"><h3 style="color:red;"></h3></span>
                     <?cs /if ?>
                     <?cs if:StatusMessage?>
                        <span id="StatusMessage"><h3 style="color:green;"><?cs var:html_escape(StatusMessage) ?></h3></span>
                     <?cs else ?>
                       <span id="StatusMessage"><h3 style="color:green;"></h3></span>
                     <?cs /if ?>

                     <?cs if:InvalidLicense?>
                        <h3 style="color:red;">You cannot setup or view log encryption settings without a valid license.</h3>
                     <?cs elif:!EncryptedLogsEligible?>
                        <h3 style="color:red;">Your current license does not allow for encrypted log support.</h3>
                     <?cs else ?>

                     <table id="locker_table" class="configuration">
                        <div id="LogsInitializingError" class="error_message" style="display: none; margin: 10px 50px;">
                           <div class="error_text panic">
                              A critical error has occurred while initializing the logs. <br>
                              Iguana is now in panic mode. You will need to stop the Iguana service and address the error before continuing. <br>
                              If you have any questions, please contact iNTERFACEWARE support.
                           </div>
                           <div class="error_heading"><br>
                               ERROR DETAILS
                           </div>
                           <font class="error_text" id="LogsInitializingErrorMessage"></font>
                        </div>
                       
                        <div id="status_panel" style="display: none;"></div>

                        <tr class="header">
                           <?cs if:LogsAreInitialized?>
                              <th colspan="2">Log Encryption Settings</th>
                           <?cs else ?>
                           <!--
                              <div id="init_instruction_msg">In order to initialize your encrypted logs and continue you will need to unlock a pre-existing locker or create a new one.</div>
                              <br> -->
                              <th colspan="2">Initialize Log Encryption Locker</th>
                           <?cs /if ?>
                        </tr>

                        <?cs if:!LogsAreInitialized?>
                        <tr>
                           <td class="left_column" colspan="2">
                              <div class="iguana_toggle">
                                 <button class="toggle_element create_new_locker_toggle">Create New</button>
                                 <button class="toggle_element unlock_existing_locker_toggle">Unlock&nbsp;Existing</button>
                              </div>
                           </td>
                        </tr>
                        <?cs /if ?>

                        <tr>
                           <td class="left_column">Log Locker Directory</td>
                           <td>
                              <?cs if:LogsAreInitialized?>
                                 <span id="log_locker_directory"><i>Not Set</i></span>
                              <?cs else ?>
                                    <input type="text" size="40" name="log_locker_directory" id="log_locker_directory" value="">
                                    <a class="file_browse_button"><span>Browse</span></a>
                                    <div id="abs_path_preview_div" style="clear: both; padding: 0px; margin: 0px;">
                                       <div id="abs_path_preview" class="path_preview"></div>
                                    </div>
                              <?cs /if ?>
                           </td>
                        </tr>

                        <tr id="log_locker_password_container">
                           <td class="left_column">Log Locker Password</td>
                           <td>
                              <?cs if:LogsAreInitialized?>
                                 <span id="log_locker_password_placeholder">***************</span>
                                 <span id="update_locker_password">
                                    <a class="action-button-small blue" class="edit_button" style="margin-left: 20px;">Change</a>
                                 </span>
                              <?cs else ?>
                                 <input type="password" size="40" name="log_locker_password" id="log_locker_password" value="">
                              <?cs /if ?>
                           </td>
                        </tr>

                        <?cs if:!LogsAreInitialized?>
                        <tr class="create_new_locker">
                           <td class="left_column">Re-type Log Locker Password</td>
                           <td>
                              <input type="password" size="40" name="log_locker_password_confirmation" id="log_locker_password_confirmation" value="">
                           </td>
                        </tr>
                        <?cs /if ?>

                        <?cs if:!LogsAreInitialized?>
                        <tr class="unlock_existing_locker">
                           <td class="left_column">Log Locker Status</td>
                           <!-- Or Unlocked/Locked -->
                           <td><span id="log_locker_status">Doesn't Exist</span></td>
                        </tr>
                        <?cs else ?>
                        <tr>
                           <td class="left_column">Log Locker Status</td>
                           <td><span id="log_locker_status" style="color: green; font-weight: bold;">Unlocked</span></td>
                        </tr>
                        <?cs /if ?>
                        
                        <?cs if:LogsAreInitialized?>
                        <tr>
                           <td class="left_column">Auto-Unlock Enabled</td>
                           <td>
                              <span id="auto_unlock_enabled">No</span>
                              <span id="update_auto_unlock">
                                 <a class="action-button-small blue" class="edit_button" style="margin-left: 20px;">Enable</a>
                              </span>
                           </td>
                        </tr>
                        <?cs else ?>
                        <!--"create_new_locker" class only shows when create new view is selected -->
                        <tr class="create_new_locker">
                           <td class="left_column">Enable Auto-Unlock</td>
                           <td><input type="checkbox" class="no_style" name="log_locker_autounlock_enable" id="log_locker_autounlock_enable"></td>
                        </tr>
                        <!--"unlock_existing_locker" class only show when "unlock existing" view is selected -->
                        <tr class="unlock_existing_locker">
                           <td class="left_column">Auto-Unlock Exists</td>
                           <td><span id="auto_unlock_enabled">No</span></td>
                        </tr>
                        <?cs /if ?>

                        <?cs if:!LogsAreInitialized?>
                           <table id="buttons">
                           <tbody>
                              <tr>
                                 <td class="unlock_existing_locker">
                                    <a class="action-button blue btn-large" id="unlock_button">Unlock</a>
                                 </td>
                                 <td class="create_new_locker">
                                    <a class="action-button blue btn-large" id="create_button">Create</a>
                                 </td>
                              </tr>
                           </tbody>
                           </table>
                           <table id="dashboardButtonTable" style="display: none;">
                              <tr>
                                 <td>
                                    <a class="action-button green btn-xlarge" href="/dashboard.html">Go to Dashboard</a>
                                </td>
                             </tr>
                          </table>
                        <?cs /if ?>

                     </table> <!-- Configuration GUI -->
                     <?cs /if ?> <!-- EncryptedLogsEligible -->
                  </center>

               </td>
            </tr>
         </tbody></table> <!-- /#iguana -->

      </div> <!-- /#main -->

      <div id="side_panel">
         <table id="side_table">
            <tr>
               <th id="side_header">
                  Page Help
               </th>
            </tr>
            <tr>
            <tr>
               <td id="side_body">
                  <h4 class="side_title">Overview</h4>
                  <p>
                     This page shows the current settings for the log encryption locker. <br><br>
                     The locker secures a unique encryption key that is used for encrypting and decrypting the Iguana logs.
                     This key can only be retrieved by unlocking the locker with a user specified password. <br><br>
                     When the auto-unlock feature is enabled it will encrypt your specified password into an
                     auto-unlocker file. This enables Iguana to automatically unlock your locker on startup without having to specify the password each time. <br><br>
                     Follow the help link below for more infomation.
                  </p>
               </td>
            </tr>
            <tr>
               <td class="side_item">
                  <h4 class="side_title">Help Links</h4>
                  <p><a href="http://help.interfaceware.com/category/reference/logs/log-encryption?v=6.1.0">Log Encryption Settings</a></p>
               </td>
            </tr>
         </table>
      </div>
   </body>
</html>

<script type="text/javascript">
   $(document).ready(function() {
      var Controller = new LogLockerPageController();
      Controller.init();
   });
</script>
