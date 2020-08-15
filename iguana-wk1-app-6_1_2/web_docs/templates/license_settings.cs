<?cs include:"doctype.cs" ?>
<html>
  <head>
      <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Settings &gt; License Entitlement</title>
      <?cs include:"browser_compatibility.cs" ?>
      <?cs include:"styles.cs" ?>
      <link rel="stylesheet" type="text/css" href="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css">
      <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
      <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/startup_status.js") ?>"></script>
      <script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>
      <script type="text/javascript" src="<?cs var:iguana_version_js("tooltip.js") ?>"></script>

      <?cs include:"license_js_files.cs" ?>
      <?cs include:"mini-login.cs" ?>
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
         <table id="iguana">
            <tr>
               <td id="cookie_crumb">
                  <a href="/settings">Settings</a> &gt; License Entitlement
               </td>
            </tr>
            <tr>
               <td id="dashboard_body">
                  <center>
                     <?cs if:SettingsUpdated ?>
                       <div style="margin: 20px 0px 5px 0px;">

                          <?cs if:LogsInitialized ?>
                              <h3><font color="green">License updated successfully.</font></h3>
                          <?cs /if ?>


                          <?cs if:LogsInvalid?>
                             <h3><font color="green">License updated successfully.</font></h3>
                             <div class="error_message" style="margin: 0 50px;">
                               <div class="error_text panic">
                                  There were errors during log validation.
                                  <br>
                                  Restart the service and address the issue before restarting.
                                  If you have questions please contact iNTERFACEWARE support.
                               </div>
                               <div class="error_heading"><br>
                                  ERROR DETAILS
                               </div>
                               <font class="error_text" id="LogsInvalid"> <?cs var:LogsInvalidMessage ?></font>
                             </div>
                          <?cs /if ?>


                          <?cs if:LockerLocked || LockerRequired ?>
                              <h3><font color="green">License updated successfully.</font></h3>
                              <p><?cs var:LockerInvalidMessage ?></p>
                              <table id="lockerButtonTable">
                                <tr>
                                  <td><a class="action-button green btn-large" href="/log_locker_settings.html" onclick="this.blur();">
                                    <?cs if:LockerRequired?>
                                      Create Log Encryption Locker
                                    <?cs else ?>
                                      Unlock Log Encryption Locker
                                    <?cs /if ?>
                                  </a></td>
                                </tr>
                              </table>
                          <?cs /if ?>


                          <?cs if:CriticalError ?>
                             <h3><font color="green">License updated successfully.</font></h3>
                             <div class="error_message" style="margin: 0 50px;">
                               <div class="error_text panic">
                                  There was a unforeseen error during log validation.
                                  <br>
                                  If it is an error that you can address, please stop the Iguana service and do this now before starting it again. <br>
                                  Otherwise, if restarting the service doesn't fix the issue, please contact iNTERFACEWARE support.
                              </div>
                              <div class="error_heading"><br>
                                  ERROR DETAILS
                              </div>
                              <font class="error_text" id="CriticalError"> <?cs var:html_escape(CriticalErrorMessage) ?> </font>
                             </div>
                          <?cs /if ?>


                          <?cs if:LogsInitializing ?>
                             <h3><font color="green">License updated successfully.</font></h3>
                             <h3><font color="green" id="LogsInitializing">Logs are now initializing and applicable channels are being auto-started...</font></h3>
                             
                             <div id="LogsInitializingError" class="error_message" style="display: none; margin: 10px 50px;">
                               <div class="error_text panic">
                                  A critical error has occurred while initializing the logs.
                                  <br>
                                  Iguana is now in panic mode. You will need to stop the Iguana service and address the error before continuing. <br>
                                  If you have any questions, please contact iNTERFACEWARE support.
                              </div>
                              <div class="error_heading"><br>
                                  ERROR DETAILS
                              </div>
                              <font class="error_text" id="LogsInitializingErrorMessage"></font>
                             </div>
                             
                             <div id="status_panel"></div>
                          <?cs /if ?>
                          
                          <table id="dashboardButtonTable" style="display: none;">
                             <tr>
                                <td><a class="action-button green btn-xlarge" href="/dashboard.html" onclick="this.blur();">Go to Dashboard</a></td>
                             </tr>
                          </table>

                       </div>
                     <?cs /if ?>
                     
                     <table class="configuration">
                        <tr>
                           <th colspan="2">Current License Information</th>
                        </tr>
                        <tr>
                           <td class="left_column">Iguana ID</td>
                           <td> <?cs var:IguanaId ?></td>
                        </tr>
                        <tr>
                           <td class="left_column">License Type</td>
                           <td> <?cs var:LicenseType ?></td>
                        </tr>
                        <tr>
                           <td class="left_column">Maximum Number of Concurrently Running Channels</td>
                           <td>
                              <?cs if:MaxCountOfConnection == 0 ?>
                              Unlimited
                              <?cs else ?>
                              <?cs var:MaxCountOfConnection ?>
                              <?cs /if ?>
                           </td>
                        </tr>
                        <tr>
                           <td class="left_column">License Expiry Date</td>
                           <td>
                              <?cs if:CountOfMonthToLicenseExpiry == 0 ?>
                              No Expiry
                              <?cs else ?>
                              <?cs var:LicenseExpiryDate ?>
                              <?cs /if ?>
                           </td>
                        </tr>
                        <tr>
                           <td class="left_column">Maintenance Expiry Date</td>
                           <td>
                              <?cs if:CountOfMonthToMaintenanceExpiry == 0 ?>
                              No Expiry
                              <?cs else ?>
                              <?cs var:MaintenanceExpiryDate ?>
                              <?cs /if ?>
                           </td>
                        </tr>
                        <?cs if:UnlimitedLlpConnections != 0 ?>
                        <tr>
                           <td class="left_column">Unlimited LLP Connections</td>
                           <td>Yes</td>
                        </tr>
                        <?cs /if ?>
                     </table>

                     <!-- /#configuration -->
                     <?cs if:!LogsInvalid && !LogsInitializing && !CriticalError && !LockerLocked && !LockerRequired?>
                       <table id="buttons" style="margin: 20px 0px;">
                             <tr>
                                <td>
                                   <?cs if:CurrentUserCanAdmin ?>
                                      <a id="GetLicense" href="" class="action-button blue" onclick="this.blur()"><span>Log In and Update This Activation</span></a>                 
                                   <?cs /if ?>          
                                </td>
                                <td>
                                   <?cs if:CurrentUserCanAdmin ?>
                                       <a id="ApplyChanges" class="action-button grey" href="/edit_license_settings.html" onclick="this.blur();">Change License Code</span></a> 
                                   <?cs /if ?>
                                </td>
                             </tr>
                       </table>
                     <?cs /if ?>
                     <!-- /#buttons -->
                     
                  </center>
               </td>
               <!-- /#dashboard_body -->
            </tr>
         </table>
         <!-- /#iguana -->
      </div>

      <!-- /#main -->
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
                     This page shows the current license entitlement of this server.
                  </p>
                  <?cs if:CurrentUserCanAdmin ?>
                  <p>Click <b>Enter License Code</b> to update your license information.</p>
                  <?cs /if ?>
               </td>
            </tr>
            <tr>
               <td class="side_item">
                  <h4 class="side_title">Help Links</h4>
                  <ul class="help_link_icon">
                     <li>
                        <a href="<?cs var:help_link('iguana4_config_license') ?>" target="_blank">License Entitlement</a>
                     </li>
                  </ul>
               </td>
            </tr>
         </table>
      </div>

   </body>
</html>

<script type="text/javascript">
  var handleLogInitError = function(ErrorMessage) {
    // if error happens on init thread, will store in updater and return it in poller result so we can display it here.
    $("#LogsInitializing").css("display", "none");
    var MessageWithValidNewlines = ErrorMessage.replace(/\r\n/g, "<br>").replace(/\n/g, "<br>");
    $("#LogsInitializingErrorMessage").html(MessageWithValidNewlines);
    $("#LogsInitializingError").css("display", "block");
  }


  var handleLogInitSuccess = function() {
    $("#dashboardButtonTable").css("display", "inline-block");
    $("#logInitMsg").text("Log initialization and channel auto-start completed!");
  }
         
  function initStatusUpdater() {
     var CurrentData = '<?cs var:js_escape(status) ?>';
     var Updater = new StartupStatusUpdater();
     Updater.init(CurrentData, "status_panel", handleLogInitSuccess, handleLogInitError, true);
     Updater.start();
  }

  $(document).ready(function() {

    var IguanaURL = [location.protocol, '//', location.host, '/'].join('');
    var RegistrationParams = "?product=iguana&version=<?cs var:VersionVer ?>&instanceid=<?cs var:IguanaId ?>&productlocation=" +
                              encodeURIComponent(IguanaURL);
    var a = document.getElementById('GetLicense');
    if (a) {
      a.href = "http://my-iguana.interfaceware.com/getlicense" + RegistrationParams;
    }

    if ($("#LogsInitialized").length) {
      $("#dashboardButtonTable").css("display", "inline-block");
    }
    else if ($("#LogsInitializing").length) {
      console.log("Logs are now initializing after license update. Display progress...");
      initStatusUpdater();
    }
    else if ($("#LogsInvalid").length) {
      var MessageWithValidNewlines = $("#LogsInvalid").text().replace(/\r\n/g, "<br>").replace(/\n/g, "<br>");
      $("#LogsInvalid").html(MessageWithValidNewlines);
    }
    else if ($("#CriticalError").length) {
      var MessageWithValidNewlines = $("#CriticalError").text().replace(/\r\n/g, "<br>").replace(/\n/g, "<br>");;
      $("#CriticalError").html(MessageWithValidNewlines);
    }

  });

</script>
