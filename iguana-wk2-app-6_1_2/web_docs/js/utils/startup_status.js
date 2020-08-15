var StartupStatusUpdater = function () {
   var m_IsComplete = false;

   // By default, identifies a completion as Iguana starting up and main thread running.
   // If m_IncludeLogInitAndChannelAutostart is true, will signal complete only if fully started AND logs are initialized 
   // AND all configured channels have then been autostarted. 
   // (log init is delayed during initial startup if invalid license/invalid log encryption locker, and channels can't autostart
   // until logs are initialized.)
   var m_IncludeLogInitAndChannelAutostart = false;

   var m_UpdateInterval = 1000;
   
   var m_CurrentData = "";
   var m_CurrentOffset = 0;
   var m_StatusContainerId = "";
   
   var m_OnStartupComplete = null;
   var m_OnLogInitError = null;

   function replaceNewLines(data) {
      return data.replace(/\r\n/g, '<br/>').replace(/\r/g, '<br/>').replace(/\n/g, '<br/>');
   }

   function startupStatusUpdateError(JqXHR, TextStatus, ErrorThrown) {
      var ErrorMessage = "<span style='color: red;''>Error getting startup status: <br>" +
                         JqXHR.status + " : " + TextStatus + " - " + ErrorThrown + "</span>";
      $("#" + m_StatusContainerId).html(ErrorMessage);
   }

   function populateStatusView() {
      var StatusPanel  = document.getElementById(m_StatusContainerId);
      NewData = replaceNewLines(m_CurrentData);
      StatusPanel.innerHTML += NewData;
      
      if (NewData.length) {
         StatusPanel.scrollTop = StatusPanel.scrollHeight;
      }  
   }

   function updateStartupStatus(Response, Code, JqXHR) {
      console.log("---- updateStartupStatus ----");
      console.log(Response); console.log(Code); console.log(JqXHR);

      m_IsComplete = m_IncludeLogInitAndChannelAutostart ? Response.startup_complete && Response.logs_initialized :
                                        Response.startup_complete;

      m_CurrentData = Response.data;
      m_CurrentOffset += m_CurrentData.length;

      if (m_IsComplete) {
         m_OnStartupComplete();
      }
      else {
         setTimeout(requestStatusUpdate, m_UpdateInterval);
      }

      populateStatusView();

      if (Response.log_init_error_occurred) {
         console.log("Error initialization logs!");
         m_OnLogInitError(Response.log_init_error_message);
      }
   }

   function requestStatusUpdate() {
      //we provide an offset so we don't fetch all the data all the time
      $.ajax({
         url    : 'startup_status_update',
         method : "GET",
         data   : { "offset" : m_CurrentOffset },
         success: updateStartupStatus,
         error  : startupStatusUpdateError
      });
   }

   this.start = function() {
      requestStatusUpdate();
   }

   this.init = function(InitialData, StatusContainerId, OnStartupComplete, OnLogInitError,
                        IncludeLogInitAndChannelAutostart) {
      m_CurrentData = InitialData;
      m_StatusContainerId = StatusContainerId;
      m_OnStartupComplete = OnStartupComplete;

      // If set, will only stop pinging and be considered "done" when startup is complete AND logs are initialized. 
      m_IncludeLogInitAndChannelAutostart = IncludeLogInitAndChannelAutostart;
      
      // If log init was delayed until after initial startup (due to license/locker), then we
      // need to handle if an error is thrown on the initialization thread. 
      // During initial startup if an error happens, it will just halt/quit the process so a handler for this is not needed.
      m_OnLogInitError = OnLogInitError;
   }

}
