var LogLockerUpdateDialogController = (function() {
   var m_UpdateType = "";
   var m_UpdateData = null;
   
   var m_Page = null;
   var m_LockerSettings = null;
   
   var m_Popup = null;


   // 
   // Common utilities
   //

   function clearUpdateInputsAndEventHandlers() {
      if (m_UpdateType === "Password") {
         $("#password_update_dialog #current_password").val("");
         $("#password_update_dialog #new_password").val("");
         $("#password_update_dialog #new_password_confirmation").val("");   
      }
      else {
         $("#autounlock_update_dialog #password").val("");   
      }
      
      $(".log_locker_update_dialog").off("keypress");
   }

   function handleUpdateFail(JqXHR, TextStatus, ErrorThrown) {
      console.log(JqXHR); console.log(TextStatus); console.log(ErrorThrown);

      var ErrorMessage = m_UpdateType + " update error: <br>";

      if (JqXHR.responseJSON.error) {
         ErrorMessage =  ErrorMessage + JqXHR.responseJSON.error.description;
      }
      else {
         ErrorMessage =  ErrorMessage + JqXHR.status + TextStatus + " : " + ErrorThrown;
      }

      m_Popup.dialog("close");
      m_Page.displayError(ErrorMessage);
   }

   function displayValidationError(Msg) {
      if (m_UpdateType === "Password") {
         $("#password_update_dialog .validate_error").show();
         $("#password_update_dialog .validate_error span").html(Msg);
      }
      else {
         $("#autounlock_update_dialog .validate_error").show();
         $("#autounlock_update_dialog .validate_error span").html(Msg);
      }
   }

   function clearValidationError() {
      if (m_UpdateType === "Password") {
         $("#password_update_dialog .validate_error").hide();
         $("#password_update_dialog .validate_error span").html("");   
      }
      else {
         $("#autounlock_update_dialog .validate_error").hide();
         $("#autounlock_update_dialog .validate_error span").html("");
      }
   }

   function attachDialogEventHandlers() {
      var SubmitButtonId = "";

      if (m_UpdateType === "Password") {
         SubmitButtonId = "password_update_dialog_submit";
      }
      else {
         SubmitButtonId = "autounlock_update_dialog_submit";
      }

      $(".log_locker_update_dialog").keypress(function(e) {
         // Trigger update when enter clicked.
         if (e.which == 13) {
            var Selector = "#" + SubmitButtonId;
            $(Selector).click();
         }
      });
   }


   // 
   // Auto-Unlock update dialog
   //

   function updateAutoUnlockSuccess() {
      m_Popup.dialog("close");
      var UpdateVerb = m_LockerSettings.auto_unlock_enabled ? "disabled" : "enabled";
      m_Page.displaySuccess("Auto-Unlock successfully " + UpdateVerb + "!");
      m_Page.init(); // Refresh page data to update auto-unlock status.
   }
   

   function submitAutoUnlockUpdate() {
      var Password = $("#autounlock_update_dialog #password").val();

      if (Password.length < 8 || Password.length > 125) {
         displayValidationError("Password not valid. <br> Must be between 8 and 125 characters in length.");
         return;
      }

      // Reverse of what it is now.
      var ToEnable = ! m_LockerSettings.auto_unlock_enabled;

      $.ajax({
         url    : "log_locker/update/autounlock",
         method : "POST",
         data : {
            password : Password,
            enabled  : ToEnable
         },
         success: updateAutoUnlockSuccess,
         error  : handleUpdateFail
      });
   }

   function setEnableAutoUnlockMessageIfNeeded() {
      if (m_LockerSettings.auto_unlock_enabled)  {
         // No need to display this if disabling.
         $("#autounlock_enable_message").hide();
         m_Popup.dialog("option", "minHeight", 140);
      }
      else {
         $("#autounlock_enable_message").show();
         m_Popup.dialog("option", "minHeight", 160);
      }
   }

   function openAutoUnlockDialog() {
      var AutoUnlockAction = m_LockerSettings.auto_unlock_enabled ? "Disable" : "Enable";
      
      m_Popup = $("#autounlock_update_dialog").dialog({
         'autoOpen': false,
         'minWidth': 500,
         'closeOnEscape': true,
         'beforeClose' : clearUpdateInputsAndEventHandlers,
         'close' : clearValidationError,
         'modal': true,
         'resizable': false,
         'title': AutoUnlockAction + " Auto-Unlock"
      });

      m_Popup.dialog("option", "buttons", [
         {
            text  : AutoUnlockAction,
            click : submitAutoUnlockUpdate,
            id    : "autounlock_update_dialog_submit"
         },
         {
            text  : "Cancel",
            click : function() {
               clearValidationError();
               m_Popup.dialog("close"); 
            }
         }]
      );

      $("#autounlock_update_dialog #autounlock_action").text(AutoUnlockAction.toLowerCase());
      setEnableAutoUnlockMessageIfNeeded();
      attachDialogEventHandlers();
   }


   // 
   // Password update dialog
   //

   function updatePasswordSuccess() {
      m_Popup.dialog("close");
      m_Page.displaySuccess("Password successfully updated!");
      // no need to refresh page, values won't change after password change.
   }

   function validatePasswordChange() {
      m_UpdateData = {};
      var OldPassword = $("#password_update_dialog #current_password").val();
      var NewPassword = $("#password_update_dialog #new_password").val();
      var NewPasswordConfirm = $("#password_update_dialog #new_password_confirmation").val();

      if (OldPassword.length < 8 || OldPassword.length > 125) {
         displayValidationError("Current password not valid. <br> Must be between 8 and 125 characters in length.");
         return false;
      }

      if (NewPassword.length < 8 || NewPassword.length > 125) {
         displayValidationError("Invalid new password. <br> Must be between 8 and 125 characters in length.");
         return false;
      }

      if (NewPassword !== NewPasswordConfirm) {
         displayValidationError("New password and password confirmation do not match!");
         return false;
      }

      m_UpdateData.old_password = OldPassword;
      m_UpdateData.new_password = NewPassword;
      m_UpdateData.new_password_confirmation = NewPasswordConfirm;
      return true;
   }

   function submitPasswordChange() {
      if ( ! validatePasswordChange() ) {
         return;
      }

      $.ajax({
         url    : "log_locker/update/password",
         method : "POST",
         data   : m_UpdateData,
         success: updatePasswordSuccess,
         error  : handleUpdateFail
      });
   }

   function openPasswordDialog() {
      m_Popup = $('#password_update_dialog').dialog({
         'autoOpen': false,
         'minWidth': 500,
         'minHeight': 300,
         'closeOnEscape': true,
         'beforeClose' : clearUpdateInputsAndEventHandlers,
         'modal': true,
         'resizable': false,
         'title': 'Update Encryption Locker Password'
      });

      m_Popup.dialog("option", "buttons", [
         {
            text  : 'Change Password',
            click : submitPasswordChange,
            id    : "password_update_dialog_submit"
         },
         {
            text  : "Cancel",
            click : function() { 
               m_Popup.dialog("close"); 
            }
         }]
      );

      attachDialogEventHandlers();
   }

   //
   // Public
   //
   return {
      open : function(UpdateType, Page, LockerSettings) {
         console.log("Popup open!");
         clearValidationError();
         m_UpdateType = UpdateType;
         m_Page = Page;
         m_LockerSettings = LockerSettings;
         m_UpdateData = null;
         
         m_Page.clearError(true);
         m_Page.clearSuccess();

         if (m_Popup && m_Popup.dialog("isOpen")) {
            m_Popup.dialog("close");
            m_Popup = null;
         }

         if (m_UpdateType === "Password") {
            openPasswordDialog();
         }
         else if (m_UpdateType == "Auto-Unlock") {
            openAutoUnlockDialog();
         }
         else {
            error("Invalid log locker update type.");
         }

         m_Popup.dialog('open');
      }
   }
})();








var LogLockerPageController = function() {
   var m_Page = this;

   var m_LockerSettings = {};

   var m_LockerStatus = {
      UNLOCKED : "Exists and Unlocked",
      LOCKED   : "Exists and Locked",
      NONE     : "Doesn't Exist"
   };

   var m_AutoUnlockStatus = {
      ENABLED  : "Yes",
      DISABLED : "No"
   };

   // Not Initialized view only.
   var m_CurrentTabSelection = "";

   // Not Initialized view only (unlock existing tab).
   // Contains the currently typed directory and flags for
   // whether a locker and auto-unlocker exist at that location.
   var m_ExistingLockerData = {};

   // Not Initialized view only (create new tab).
   // Contains the directory/password/password check/auto-unlock choice.
   var m_NewLockerData = {};

   // Not Initialized view only (both tabs).
   // Will grab locker/auto unlock existence data using the currently typed directory.
   // Refreshes every key stroke.
   var m_LatestExistenceCheckResult = {};

   var m_ServerError = false;

   m_Page.clearError = function(ClearServerError) {
      if (ClearServerError || ! m_ServerError) {
         // Only clear if what is displayed isn't an error from the server.
         // Will clear once submit is made.
         $("#ErrorMessage h3").html("");    
      }
   }

   m_Page.clearSuccess  = function() {
      $("#StatusMessage h3").html("");
   }

   m_Page.displayError = function(ErrorText) {
      $("#ErrorMessage h3").html(ErrorText);
   }

   m_Page.displaySuccess = function(SuccessMessage) {
      $("#StatusMessage h3").html(SuccessMessage);
   }



   // 
   // Non-Initialized View
   //

   function displayDashboardButton() {
      $("#dashboardButtonTable").show(); 
      $(document).keypress(function(e) {
         if (e.which == 13) {
            window.location = $("#dashboardButtonTable a").attr("href");
         }
      });
   }

   function enableSubmitEvents() {
      $("#unlock_button").click(initializeLocker);
      $("#create_button").click(initializeLocker);

      $(document).keypress(function(e) {
         // Trigger submit when enter clicked.
         if (e.which == 13) {
            if (m_CurrentTabSelection == "CreateNew") {
               $("#create_button").click();
               return;
            }
            $("#unlock_button").click();
         }
      });
   }

   function disableSubmitEvents() {
      $("#unlock_button").off("click");
      $("#create_button").off("click");
      $(document).unbind("keypress.key13");
   }


   function resetNonInitializedViewElements() {
      m_ExistingLockerData = {};
      m_NewLockerData = {};
      $("#log_locker_status").text(m_LockerStatus.NONE);
      
      $("#auto_unlock_enabled").text(m_AutoUnlockStatus.DISABLED);
      $("#log_locker_autounlock_enable").prop("checked", false);
      
      $("#log_locker_password_container").show();
      $("#log_locker_password").val("");
      $("#log_locker_password_confirmation").val("");
      $("#log_locker_directory").val("");
      $("#abs_path_preview").text("");
   }

   function switchToUnlockExistingTab() {
      m_CurrentTabSelection = "UnlockExisting";
      $(".create_new_locker").hide();
      $(".unlock_existing_locker").show();
      
      $(".iguana_toggle .create_new_locker_toggle").removeClass("is_selected");
      $(".iguana_toggle .unlock_existing_locker_toggle").addClass("is_selected");
   }

   function switchToCreateNewTab() {
      m_CurrentTabSelection = "CreateNew";
      $(".unlock_existing_locker").hide();
      $(".create_new_locker").show();
      
      $(".iguana_toggle .unlock_existing_locker_toggle").removeClass("is_selected");
      $(".iguana_toggle .create_new_locker_toggle").addClass("is_selected");

   }

   function renderNonInitializedLockerDirectory() {
      var Directory = "";
      
      if (m_LockerSettings.directory !== "") {
         Directory = m_LockerSettings.directory;
      }
      else {
         // Default to working directory.
         Directory = m_LockerSettings.working_directory;
      }

      $("#log_locker_directory").val(Directory);
   }

   function renderCreateNew() {
      switchToCreateNewTab();
      resetNonInitializedViewElements();
      renderNonInitializedLockerDirectory();
   }

   function renderUnlockExisting() {
      switchToUnlockExistingTab();
      resetNonInitializedViewElements();
      renderNonInitializedLockerDirectory();
      
      if (m_LockerSettings.directory == "") {
         $("#log_locker_status").text(m_LockerStatus.NONE);
      }
      else if (m_LockerSettings.exists && m_LockerSettings.locked) {
         $("#log_locker_status").text(m_LockerStatus.LOCKED);
      }
      else if (m_LockerSettings.exists && ! m_LockerSettings.locked) {
         // This shouldn't happen. It should be locked if logs not initialzed.
         // Get them to type the password again.
         $("#log_locker_status").text(m_LockerStatus.LOCKED);
      }

      if (m_LockerSettings.auto_unlock_enabled) {
         $("#log_locker_password_container").hide();
         $("#auto_unlock_enabled").text(m_AutoUnlockStatus.ENABLED);
      }
   }




   function resetLockerExistenceCheckValues() {
      // Based on the result of the existence check call and what view is 
      // currentley selected, elements will be changed. If a valid directory
      // isn't typed, reset them back to their default state.
      $("#abs_path_preview").text("");
      $("#log_locker_status").text(m_LockerStatus.NONE);
      $("#auto_unlock_enabled").text(m_AutoUnlockStatus.DISABLED);
      $("#log_locker_password_container").show();
      m_LatestExistenceCheckResult = {};
   }

   function handleLockerExistenceCheckSuccess(Response, Code, JqXHR) {
      m_LatestExistenceCheckResult = Response;
      console.log(Response);

      var ResolvedDirectoryPath = Response["resolved_directory"];
      console.log(ResolvedDirectoryPath);

      if (ResolvedDirectoryPath) {
         $("#abs_path_preview").text("Absolute Path: \"" + ResolvedDirectoryPath + "\"");
      }
      
      if (m_CurrentTabSelection == "CreateNew") {
         // Don't need to display below in this view.
         return;
      }

      var LockerExists     = Response["locker_exists"];
      var AutoUnlockExists = Response["auto_unlock_exists"];

      console.log("Locker Exists: " + LockerExists);
      console.log("Auto-Unlock Exists: " + AutoUnlockExists);
   
      if (!LockerExists) {
         $("#log_locker_status").text(m_LockerStatus.NONE);
         $("#auto_unlock_enabled").text(m_AutoUnlockStatus.DISABLED);
         $("#log_locker_password_container").show();
         return;
      }
      
      $("#log_locker_status").html("<span style='font-color: green; font-weight: bold;'>" + 
                                    m_LockerStatus.LOCKED +"</font>");
      
      if (AutoUnlockExists) {
         $("#auto_unlock_enabled").html("<span style='font-color: green; font-weight: bold;'>" + 
                                         m_AutoUnlockStatus.ENABLED +"</font>");
         // If auto unlock exists, don't need to type password.
         $("#log_locker_password_container").hide();
      }
   }

   function handleLockerExistenceCheckError(JqXHR, TextStatus, ErrorThrown) {
      resetLockerExistenceCheckValues()
      var ErrorMessage = "Error retrieving whether encryption locker exists at current location. <br>" +
                         JqXHR.status + " : " + TextStatus + " - " + ErrorThrown;
      displayError(ErrorMessage);
   }

   function checkForLockerExistence() {

      var CurrentDirectory = $("#log_locker_directory").val().trim();
      console.log(CurrentDirectory);

      $.ajax({
         url     : "log_locker/exists",
         method  : "GET",
         data    : {
            "directory" : CurrentDirectory
         },
         success : handleLockerExistenceCheckSuccess,
         error   : handleLockerExistenceCheckError
      });
   }

   function handleLogInitError(ErrorMessage) {
      // if error happens on init thread, will store in updater and return it in poller result so we can display it here.
      $("#LogInitializingMessage").css("display", "none");
      var MessageWithValidNewlines = ErrorMessage.replace(/\r\n/g, "<br>").replace(/\n/g, "<br>");
      $("#LogsInitializingErrorMessage").html(MessageWithValidNewlines);
      $("#LogsInitializingError").css("display", "block");
   }

   function handleLogInitSuccess() {
     $("#logInitMsg").text("Log initialization and channel auto-start completed!");
     displayDashboardButton();
   }


   function createLogInitStatusDisplay() {
      $("#locker_table").hide();
      $("#status_panel").show();
      var Updater = new StartupStatusUpdater();
      Updater.init("", "status_panel", handleLogInitSuccess, handleLogInitError, true);
      Updater.start();
   }

   function handleLockerInitializeError(JqXHR, TextStatus, ErrorThrown) {
      console.log(JqXHR);
      console.log(TextStatus);
      console.log(ErrorThrown);

      var ErrorMessage = "<u>Problem encountered during locker initialization</u><br>";

      if (JqXHR.responseJSON.error) {
         var RawMsg = JqXHR.responseJSON.error.description;
         ErrorMessage =  ErrorMessage + RawMsg.replace(/\r\n/g, "<br>").replace(/\n/g, "<br>");
      }
      else {
         ErrorMessage =  ErrorMessage + JqXHR.status + TextStatus + " : " + ErrorThrown;
      }
      
      m_Page.displayError(ErrorMessage);                  
      m_ServerError = true;
      enableSubmitEvents();
   }

   function handleLockerInitializeSuccess(Response, Code, JqXHR) {
      // $("#init_instruction_msg").hide();
      $("#buttons").hide();
      m_Page.clearError();

      var SuccessMessage = "";

      if (Response["unlock_success"] && Response["auto_unlocked"]) {
         SuccessMessage = "Sucessfully auto-unlocked log encryption locker!";
      }
      else if (Response["unlock_success"]) {
         SuccessMessage = "Sucessfully unlocked log encryption locker!";
      }
      else if (Response["locker_created"] && Response["auto_unlock_enabled"]) {
         SuccessMessage = "Sucessfully created log encryption locker and enabled auto-unlock!";
      }
      else if (Response["locker_created"] ) {
         SuccessMessage = "Sucessfully created log encryption locker!";
      }

      if (Response["logs_initializing"]) {
         createLogInitStatusDisplay();
         SuccessMessage = SuccessMessage + 
                          "<br><br> <span id=\"LogInitializingMessage\">" + 
                             "Encrypted logs are now initializing and applicable channels are being auto-started..." +
                          "</span>";
      }
      else {
         // Logs already initialized? Shouldn't happen but will get errors
         // on dashboard/when attempting to navigate there if the case.
         displayDashboardButton();
      }
      
      m_Page.displaySuccess(SuccessMessage);
   }

   function setInitializeRequestData() {
      var RequestData = {}

      if (m_CurrentTabSelection == "CreateNew") {
         RequestData = m_NewLockerData;
         RequestData["action"] = "create";
      }
      else {
         RequestData = m_ExistingLockerData;
         RequestData["action"] = "unlock";
      }

      return RequestData;
   }

   function validateInitializeExistingLockerInputs() {
      m_ExistingLockerData = {};

      if ( ! m_LatestExistenceCheckResult["locker_exists"] ) {
         var ErrorMsg = "A log encryption locker does not exist in the current location. <br>" +
                        "Make sure the directory is correct and the locker status indicates one exists.";
         m_Page.displayError(ErrorMsg);
         return false;
      }

      var Directory = $("#log_locker_directory").val().trim(); 

      if (Directory == "" || Directory === "Not Set") {
         m_Page.displayError("Locker directory cannot be blank or left in default state.");
         return false;
      }

      m_ExistingLockerData.directory = Directory;

      var AutoUnlockExists = m_LatestExistenceCheckResult["auto_unlock_exists"];
      var Password = $("#log_locker_password").val();

      if ( ! AutoUnlockExists ) {
         if (Password.length < 8 || Password.length > 125) {
            m_Page.displayError("Passwords must be between 8 and 125 characters in length. <br> Make sure your password is typed correctly.");
            return false;
         }

         m_ExistingLockerData.password = Password;
      }

      return true;
   }

   function validateInitializeNewLockerInputs() {
      m_NewLockerData = {};

      var Directory = $("#log_locker_directory").val().trim();
      
      if (Directory == "" || Directory === "Not Set") {
         m_Page.displayError("Locker directory cannot be blank or left in default state.");
         return false;
      }

      m_NewLockerData.directory = Directory;

      var Password = $("#log_locker_password").val();
      var PasswordRetype = $("#log_locker_password_confirmation").val();

      if (Password.length < 8 || Password.length > 125) {
         m_Page.displayError("Password must be between 8 and 125 characters in length.");
         return false;
      }
      else if(Password !== PasswordRetype) {
         m_Page.displayError("Password and password confirmation do not match. Double check and try again.");
         return false;
      }

      m_NewLockerData.password = Password;
      m_NewLockerData.password_confirmation = PasswordRetype;
      
      var AutoUnlockEnabled = $("#log_locker_autounlock_enable:checked").length > 0;
      m_NewLockerData.enable_auto_unlock = AutoUnlockEnabled;
      
      return true;
   }

   function validateInitializeInputs() {
      if (m_CurrentTabSelection == "CreateNew") {
         // TODO : - Display confirmation box telling them to not forget their password/backup the locker?
         //        - If autounlock, remind them the implications (make sure directory is )
         return validateInitializeNewLockerInputs();
      }

      return validateInitializeExistingLockerInputs();
   }

   function initializeLocker() {
      var InputsAreValid = validateInitializeInputs();
      
      if (InputsAreValid) {
         disableSubmitEvents();
         m_Page.clearError(true);
         var RequestData = setInitializeRequestData();
         
         $.ajax({
            url     : "log_locker/initialize",
            method  : "POST",
            data    : RequestData,
            success : handleLockerInitializeSuccess,
            error   : handleLockerInitializeError
         });
      }
   }

   function handleNonInitializedToggleClick(ClickedToggleElement) {
      m_Page.clearError();

      if ($(this).hasClass("is_selected")) {
         console.log("already selected!");
         return;
      }
      
      console.log(m_CurrentTabSelection);

      if (m_CurrentTabSelection == "CreateNew") {
         renderUnlockExisting();
      }
      else {
         renderCreateNew();
      }
   }

   function registerNotInitializedEvents() {
      //$("input").click(m_Page.clearError);

      $("#log_locker_directory").keyup(checkForLockerExistence);
      $("#log_locker_directory").blur(checkForLockerExistence);

      enableSubmitEvents();

      $(".iguana_toggle .toggle_element").click(function(){ 
         handleNonInitializedToggleClick(this);
         checkForLockerExistence();
      });

      $(".file_browse_button").click(function() {
         ifware.Settings.FileBrowser.FILfolderBrowse('', 'log_locker_directory', true);
      });
   }

   function renderNotInitialized() {
      if (! m_LockerSettings.exists) {
         console.log("Locker doesn't exist at current directory...");
         renderCreateNew();
      }
      else {
         console.log("Locker exists at current directory...");
         renderUnlockExisting();
      }

      registerNotInitializedEvents();
      $("#log_locker_directory").blur();
   }



   //
   // Initialized View
   //

   function displayAutoUnlockUpdateDialog() {
      LogLockerUpdateDialogController.open("Auto-Unlock", m_Page, m_LockerSettings);
   }

   function displayPasswordChangeDialog() {
      LogLockerUpdateDialogController.open("Password", m_Page, m_LockerSettings);
   }

   function registerInitializedEvents() {
      $("#update_locker_password").click(function() {
         displayPasswordChangeDialog();
      });

      $("#update_auto_unlock").click(function() {
         displayAutoUnlockUpdateDialog();
      });

   }

   function renderInitialized() {
      $("#log_locker_directory").text(m_LockerSettings.directory );
      
      var AutoUnlockMessage = m_LockerSettings.auto_unlock_enabled ? 
                              m_AutoUnlockStatus.ENABLED : m_AutoUnlockStatus.DISABLED;

      var AutoUnlockButtonText = m_LockerSettings.auto_unlock_enabled ? "Disable" : "Enable";


      $("#auto_unlock_enabled").text(AutoUnlockMessage);
      $("#update_auto_unlock a").text(AutoUnlockButtonText);

      registerInitializedEvents();
   }



   // 
   // General and Initialization
   //


   function getUpdatedSettingsError(JqXHR, TextStatus, ErrorThrown) {
      var ErrorMessage = "Error getting log locker settings: <br>" +
                         JqXHR.status + " : " + TextStatus + " - " + ErrorThrown;
      $("#ErrorMessage").html(ErrorMessage);
   }

   function getUpdatedSettingsSuccess(Response, Code, JqXHR) {
      m_LockerSettings = Response;

      if (m_LockerSettings.invalid_license || ! m_LockerSettings.eligible) {
         console.log("Invalid license or not eligible.");
         return;
      }

      var LogsInitialized = m_LockerSettings.logs_initialized;

      if (LogsInitialized) {
         renderInitialized();
      }
      else {
         renderNotInitialized();
      }

      $("#log_locker_password").focus();
   }

   function getUpdatedSettings() {
      $.ajax({
         url    : "log_locker/settings",
         method : "GET",
         success: getUpdatedSettingsSuccess,
         error  : getUpdatedSettingsError
      });
   }

   this.init = function() {
      getUpdatedSettings();
   }

};