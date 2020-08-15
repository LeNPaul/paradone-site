<?cs # vim: set syntax=html :?>

<script defer type="text/javascript">
var ShowTbodyDisplayStyle = "";
var CurrentTab = null;
var CurrentTabContents = null;
var IsRunning = '<?cs var:IsRunning ?>';
var YellowLight = '<?cs var:YellowLight ?>'; 
var StopRequested = false;
var ReadOnlyMode = '<?cs var:Channel.ReadOnlyMode ?>';
var DASHskinningCookieData = COOKIEread('IguanaSkinningDirectoryCookie');

if (DASHskinningCookieData == null){
   DASHskinningCookieData = '';
}

var GroupCount = <?cs var: GroupCount ?>;
var BelongsToCount = <?cs var: BelongsToCount ?>;
var Groups = new Array(<?cs var:GroupCount ?>);
var GroupIDs = new Array(<?cs var:GroupCount ?>);
var StatusTimeoutId;

function initializeGroups(){
   <?cs set:count = #0 ?> 
   <?cs each: group = Groups ?>
   Groups[<?cs var:count ?>] = "<?cs var:js_escape(group.Name) ?>";
   GroupIDs[<?cs var:count ?>] = "<?cs var:js_escape(group.Value) ?>";
   <?cs set:count = count + #1 ?>
   <?cs /each ?>
	
   if (GroupCount == 0 && ReadOnlyMode == 0)
   {
      document.getElementById("tableGroupsBody").style.display = "none";
      return;
   }
}

function showTabContents(SelectedTab, TabContentsId){
   CurrentTab.className = "";
   CurrentTabContents.style.display = "none";
   
   CurrentTab = SelectedTab;
   CurrentTabContents = document.getElementById(TabContentsId);
   CurrentTab.className = "current";
   CurrentTabContents.style.display = ShowTbodyDisplayStyle;
   document.getElementById("editTab").value = SelectedTab.id;

   $("#helpTooltipDiv").fadeOut("slow");
   return false;
}

function showSourceTab(){
   showTabContents(document.getElementById('sourceTab'), 'sourceTabContents');
}

function showDestinationTab(){
   showTabContents(document.getElementById('destinationTab'), 'destinationTabContents');
}

function showHighlight(thisElement){
   $('#channel_configuration_table tr').css("backgroundColor", "");
   focusedElement = thisElement.id;                    
   $(thisElement).parents("tr.selected").css("backgroundColor", "#cbe892");
}

function pullTabFromUrl() {
   try {
      var TabName = decodeURIComponent(window.location.href.match(/[?&]Tab=([^&]*)/)[1]); }
   catch (e) {}
   
   var Tab;
   if (TabName && TabName.match(/^(channel|source|filter|destination|exportTables)$/) && (Tab = document.getElementById(TabName + 'Tab'))){
      setTimeout(function(){showTabContents(Tab,TabName+'TabContents');}, 1);
      return;
   }
   var EditTab = document.getElementById("<?cs var:html_escape(Channel.editTab) ?>");
   if (EditTab) {
      showTabContents(EditTab, "<?cs var:html_escape(Channel.editTab) ?>Contents");
      return;
   }
}

function onLoad(){
   <?cs if:Channel.Source.IsDatabase ?>     onLoadFromDatabase();
   <?cs elif:Channel.Source.IsFile ?>       onLoadFromFile();
   <?cs elif:Channel.Source.IsExecutable ?> onLoadFromExecutable();
   <?cs elif:Channel.Source.IsLLP ?>        onLoadFromLlp();
   <?cs /if ?>

   <?cs if:Channel.Destination.IsFile ?>         onLoadToFile();
   <?cs elif:Channel.Destination.IsExecutable ?> onLoadToExecutable();
   <?cs elif:Channel.Destination.IsDatabase ?>   onLoadToDatabase();
   <?cs elif:Channel.Destination.IsLLP ?>        onLoadToLlp();
   <?cs elif:Channel.Destination.IsHttp ?>       onLoadToHttp();
   <?cs /if ?>

   onLoadMessageFilter();

   CurrentTab = document.getElementById("channelTab");
   CurrentTabContents = document.getElementById("channelTabContents");
   pullTabFromUrl();
   <?cs if:Channel.HasErrors && CurrentUserCanAdmin ?>
      activateErrorTab();
   <?cs /if ?>
   
   TOOLinitialize();
   HLPpopUpinitialize(helpTooltipDivRefresh);
   initializeHelp();
   
   <?cs if:!Channel.IsNew ?>
      statusTooltip();
      sourceTooltip();
      filterTooltip();
      destinationTooltip();
      updateStatus();
   <?cs /if ?>   
   
   initializeGroups();

   // We don't want the channel form to submit if we happen to press enter.
   //
   document.getElementById('channel_data').onkeydown = function(e) {
      if(window.event) e = window.event;
      if(e.keyCode == 13) { 
         // Enter 
         var source = (e.srcElement ? e.srcElement : e.target);
         return source.type == 'textarea';
      }
   }  
}
// Activate the first tab with an "error" on it, if such a tab
// exists.
function activateErrorTab() {
   <?cs if:Channel.Configuration.HasErrors ?>
   // Channel tab is already active
   <?cs elif:Channel.Source.HasErrors ?>
      showTabContents(document.getElementById("sourceTab"), "sourceTabContents");
   <?cs elif:Channel.MessageFilter.HasErrors ?>
      showTabContents(document.getElementById("filterTab"), "filterTabContents");
   <?cs elif:Channel.Destination.HasErrors ?>
      showTabContents(document.getElementById("destinationTab"), "destinationTabContents");
   <?cs /if ?>
}

function updateLoggingLevelMessage(message){
   var MessageSpan = document.getElementById('spanLoggingLevelMessage');
   MessageSpan.innerHTML = message === undefined ? "" : message;
}

var RequestTimeoutId;

function loggingLevelUpdateComplete(data){
   updateLoggingLevelMessage(data.comment);
   clearTimeout(RequestTimeoutId);
   RequestTimeoutId = setTimeout(updateLoggingLevelMessage, 5000);
}

function loggingLevelUpdateError(RequestObject, Status, ErrorString) {
   console.log(RequestObject);
   var ErrorString = RequestObject.responseJSON.error.description;
   var ErrorMessage = '<b style="color: red;">Error while updating logging level:<br />' + ErrorString + '</b>';
   updateLoggingLevelMessage(ErrorMessage);
   clearTimeout(RequestTimeoutId);
}

function requestTimeout() {
   loggingLevelUpdateError('Request timed out.');
}

function updateLoggingLevel(NewLoggingLevel) {
   var ChannelName = "<?cs var:js_escape(Channel.Name) ?>";
   $.ajax({
      url:  '/channel/logging_level',
      method: "POST",
      data: {
         'channel':      ChannelName,
         'logginglevel': NewLoggingLevel
      },
      success: loggingLevelUpdateComplete,
      error:   loggingLevelUpdateError  
   });
   updateLoggingLevelMessage('<b style="color: gray;">Updating logging level...</b>');
   RequestTimeoutId = setTimeout(requestTimeout, 5000);
}
/*
function channelCancel() {
   var editTab = document.getElementById("editTab").value;
   ChannelName = '<?cs var:js_escape(Channel.Name)?>';
   document.location = "/channel#Channel=" + encodeURIComponent(ChannelName) + "&editTab=" + editTab;   
}*/

function channelDoAction(Action) {
   if (Action == 'stop' || Action == 'force_stop') {
      StopRequested = true;
   } else {
      StopRequested = false;
   }
   $.ajax({
      url:  '/dashboard_data',
      method: "POST",
      data: {
         'action'           : Action,
         'channel'          : "<?cs var:js_escape(Channel.Name)?>",
         'FirstChannelIndex': 0,
         'LastChannelIndex' : 0
      },
      success: function() {
         if (Action == 'force_stop') {
            StopRequested = false;
         }
      },
      error: ifware.SettingsScreen.handleError
   });
   clearTimeout(StatusTimeoutId);
   updateStatus();
}

// Setting the LiveStatus tooltip for the jollyrancher icon.
var statusTooltipText;
var statusTooltipOn = 0;

function statusTooltip() {
   var componentStatusImg = document.getElementById('statusImg');
   componentStatusImg.onmouseover = function() {
      TOOLtooltipLink(statusTooltipText, function() { statusTooltipOn = 1; }, this);
   };

   componentStatusImg.onmouseout = function() {
      TOOLtooltipClose();  
      statusTooltipOn = 0;
   };

   componentStatusImg.onmouseup = function() {
      TOOLtooltipClose();
      statusTooltipOn = 0;
   };
}

// Setting the Source tooltip for the component icon.
var sourceTooltipText;
var sourceTooltipOn = 0;

function sourceTooltip() {
   var componentSourceImg = document.getElementById('sourceImg');
   componentSourceImg.onmouseover = function() {
      TOOLtooltipLink
      (
         sourceTooltipText, 
         function() 
         { 
            sourceTooltipOn = 1;

            var StatusSuffix = '';
            <?cs if:Channel.Source.HasErrors ?>       
            StatusSuffix = '_error';
            <?cs elif:Channel.Source.HasWarnings ?>
            StatusSuffix = '_warning';
            <?cs /if ?>  
 
            componentSourceImg.src = "/images/icon_<?cs var:html_escape(Channel.Source.ShortName) ?>_hover" + StatusSuffix + ".gif"
         }, 
         this
       );
   };

   componentSourceImg.onmouseout = function() {
      TOOLtooltipCloseIfNotHover();  
      sourceTooltipOn = 0;
      
      var StatusSuffix = '';
      <?cs if:Channel.Source.HasErrors ?>       
      StatusSuffix = '_error';
      <?cs elif:Channel.Source.HasWarnings ?>
      StatusSuffix = '_warning';
      <?cs /if ?>  
 
      componentSourceImg.src = "/images/icon_<?cs var:html_escape(Channel.Source.ShortName) ?>" + StatusSuffix + ".gif"
   };

   componentSourceImg.onmouseup = componentSourceImg.onmouseout;
}

// Setting the Filter tooltip for the component icon.
var filterTooltipText;
var filterTooltipOn = 0;

function filterTooltip() {
   var componentFilterImg = document.getElementById('filterImg');

   componentFilterImg.onmouseover = function() {
      
      TOOLtooltipLink(filterTooltipText, function() { 
         filterTooltipOn = 1; 
         
         <?cs if:Channel.UseMessageFilter ?>
            <?cs if:Channel.MessageFilter.HasErrors ?>
               componentFilterImg.src = "/images/arrow_filter_hover_error.gif";
            <?cs elif:Channel.MessageFilter.HasWarnings ?>
               componentFilterImg.src = "/images/arrow_filter_hover_warning.gif";
            <?cs else ?>
               componentFilterImg.src = "/images/arrow_filter_hover.gif";
            <?cs /if ?>
         <?cs else ?>
            componentFilterImg.src ="/images/arrow_hover.gif"
         <?cs /if ?>

	  },
	  this);
   };

   componentFilterImg.onmouseout = function() {
      TOOLtooltipCloseIfNotHover();  
      filterTooltipOn = 0;
      
      <?cs if:Channel.UseMessageFilter ?>
         <?cs if:Channel.MessageFilter.HasErrors ?>
            componentFilterImg.src = "/images/arrow_filter_error.gif";
         <?cs elif:Channel.MessageFilter.HasWarnings ?>
            componentFilterImg.src = "/images/arrow_filter_warning.gif";
         <?cs else ?>
            componentFilterImg.src = "/images/arrow_filter.gif";
         <?cs /if ?>
      <?cs else ?>
         componentFilterImg.src ="/images/arrow.gif"
      <?cs /if ?>
   };

   componentFilterImg.onmouseup = componentFilterImg.onmouseout;
}

// Setting the Destination tooltip for the component icon.
var destinationTooltipText;
var destinationTooltipOn = 0;

function destinationTooltip() {
   var componentDestinationImg = document.getElementById('destinationImg');
   
   componentDestinationImg.onmouseover = function() {
      TOOLtooltipLink(destinationTooltipText, function() { 
	   destinationTooltipOn = 1; 

      var StatusSuffix = '';

      <?cs if:Channel.Destination.HasErrors ?>       
         StatusSuffix = '_error';
      <?cs elif:Channel.Destination.HasWarnings ?>
         StatusSuffix = '_warning';
      <?cs /if ?>  

      componentDestinationImg.src = "/images/icon_<?cs var:html_escape(Channel.Destination.ShortName) ?>_hover" + StatusSuffix + ".gif"
   
      }, this);
   };

   componentDestinationImg.onmouseout = function() {
      TOOLtooltipCloseIfNotHover();
      destinationTooltipOn = 0;
      var StatusSuffix = '';

      <?cs if:Channel.Destination.HasErrors ?>       
         StatusSuffix = '_error';
      <?cs elif:Channel.Destination.HasWarnings ?>
         StatusSuffix = '_warning';
      <?cs /if ?>  

      componentDestinationImg.src = "/images/icon_<?cs var:html_escape(Channel.Destination.ShortName) ?>" + StatusSuffix + ".gif"
   };

   componentDestinationImg.onmouseup = componentDestinationImg.onmouseout;
}

function disableExportButtons() {
   <?cs if:Channel.Source.RequiresTables ?>
      document.getElementById('hrefExportSourceTablesButton').style.display = 'none';
      document.getElementById('ExportSourceTablesButton').style.display = '';
   <?cs /if ?>
   
   <?cs if:Channel.Destination.RequiresTables ?>
      document.getElementById('hrefExportDestinationTablesButton').style.display = 'none';
      document.getElementById('ExportDestinationTablesButton').style.display = '';
   <?cs /if ?>
   
   <?cs if:Channel.Source.RequiresTables && Channel.Destination.RequiresTables ?>
      document.getElementById('hrefExportSourceAndDestinationTablesButton').style.display = 'none';
      document.getElementById('ExportSourceAndDestinationTablesButton').style.display = '';
   <?cs /if ?>
}

function enableExportButtons() {
   <?cs if:Channel.Source.RequiresTables ?>
      document.getElementById('hrefExportSourceTablesButton').style.display = '';
      document.getElementById('ExportSourceTablesButton').style.display = 'none';
   <?cs /if ?>
   
   <?cs if:Channel.Destination.RequiresTables ?>
      document.getElementById('hrefExportDestinationTablesButton').style.display = '';
      document.getElementById('ExportDestinationTablesButton').style.display = 'none';
   <?cs /if ?>
   
   <?cs if:Channel.Source.RequiresTables && Channel.Destination.RequiresTables ?>
      document.getElementById('hrefExportSourceAndDestinationTablesButton').style.display = '';
      document.getElementById('ExportSourceAndDestinationTablesButton').style.display = 'none';
   <?cs /if ?>
}

// Sets the value if the element exists and the data exists.
// if element exists, but data does not, sets to '0'
function setElementToNumericValue(ElementName, NumericValue, Link) {
   var Element = document.getElementById(ElementName)
   if (Element) {
      var HtmlResult = ''
      if (Link) {
          HtmlResult += '<a href="' + Link + '">';
      }
      if (NumericValue) {
         HtmlResult += NumericValue;
      } else {
         HtmlResult += '0';
      }
      if (Link) {
          HtmlResult += '</a>';
      }
      Element.innerHTML = HtmlResult;
   }
}

function makeQueuedLink() {
<?cs if:Channel.FirstSourceChannelName ?>
   return FRMCHqueuedLogBrowserLink('<?cs var:js_escape(Channel.Name) ?>', 
                                    '<?cs var:js_escape(Channel.FirstSourceChannelName) ?>');
<?cs else ?>
   return '';
<?cs /if ?>
}

// Replace the "Edit Script" in the tooltip if the user doesn't have permissions
function adjustTooltipEditScript (TooltipText) {
   <?cs if:!CurrentUserCanEditScripts ?>
       var TT = $(TooltipText);
       TT.find (".edit_permission_required").removeAttr("href").text ("You do not have the necessary permissions to edit the script.");
       TooltipText = $('<div>').append(TT).html(); // jQuery does not have outerHtml function, so we wrap in a temporary div
   <?cs /if ?>    

   //TODO - this needs to be someplace else, also used in dashboard.
   TooltipText = TooltipText.replace(/%%IFWARE_WINDOW_LOCATION_HOSTNAME%%/g,window.location.hostname);                 
   TooltipText = TooltipText.replace(/%%SESSION_INFO%%/g, '?'); // Session details?????
   return TooltipText;
}

// This is where we update the status with the return json data
// In general, it just updates the elements.  In the rare case that a channel's status has changed,
// the page is reloaded (in view mode) or a modal is shown and then the page is reloaded (in edit mode)
// If we change the start/stop buttons to ajax calls, we could leave the user on the same page (in edit mode)
// and allow them to stop the channel and then save their changes.
function statusUpdateComplete(DashboardData) {
   console.log(DashboardData);
   console.log(DashboardData.IsRunning, DashboardData.YellowLight);
   if( !DashboardData || (DashboardData.LoggedIn !== undefined) && !DashboardData.LoggedIn ) {
      MiniLogin.show('Your Iguana Session has Expired.');
      return;
   }

   if (DashboardData.IsRunning !== undefined) {
      IsRunning = DashboardData.IsRunning;
   }

   YellowLight = DashboardData.YellowLight;
   if (ReadOnlyMode == '1') {

      <?cs if:CurrentUserCanStartStop ?>

      if (IsRunning) {
         $("#hrefStartButton").hide();
         $("#hrefStopButton").show();
         if (YellowLight && StopRequested) {
            $("#hrefForceStopButton").show();
         } else {
            $("#hrefForceStopButton").hide();
         }
         <?cs if:CurrentUserCanAdmin ?>
         $("#hrefRemoveButton").hide();
         $("#hrefRemoveButtonDisabled").show();
         <?cs /if ?> 
         <?cs if:CurrentUserCanReconfigure ?>			   
         $("#hrefChannelEditButton").hide();
         $("#ChannelEditButton").show();
         <?cs /if ?> 

         disableExportButtons();
      } else {
         $("#hrefForceStopButton").hide();
         $("#hrefStopButton").hide();
         $("#hrefStartButton").show();
         <?cs if:CurrentUserCanAdmin ?>
         $("#hrefRemoveButtonDisabled").hide();
         $("#hrefRemoveButton").show();
         <?cs /if ?> 
         <?cs if:CurrentUserCanReconfigure ?>			   
         $("#ChannelEditButton").hide();
         $("#hrefChannelEditButton").show();
         <?cs /if ?>                
         enableExportButtons();
      }

      <?cs else ?>

      if (IsRunning) {
         $("#ChannelStartButtonNonAdmin").hide();
         $("#ChannelStopButtonNonAdmin").show();
      } else {
         $("#ChannelStopButtonNonAdmin").hide();
         $("#ChannelStartButtonNonAdmin").show();
      }

     : <?cs /if ?>

   } else {
      if (IsRunning){         
         ChannelStatusModal.show();
      }
   }
   

   setElementToNumericValue('spnQueued', DashboardData.CountOfRemaining, makeQueuedLink());
   setElementToNumericValue('spnTotalProcessed', DashboardData.CountOfProcessed);
   setElementToNumericValue('spnCurrentProcessed', DashboardData.CountOfCurrentProcessed);
   setElementToNumericValue('spnTotalEnqueued', DashboardData.CountOfEnqueued);
   setElementToNumericValue('spnInboundTps', DashboardData.InboundTps);
   setElementToNumericValue('spnOutboundTps', DashboardData.OutboundTps);
   
   if (DashboardData.CountOfError ) {
      document.getElementById('hrefCountofError').innerHTML = DashboardData.CountOfError;
      document.getElementById("hrefCountofError").className = 'error';
      $("#CountOfErrorDiv").css({ backgroundColor:"#FFCDCB", border:"#FFA19C 1px solid", padding:"1px 5px 1px 5px", marginLeft:"-6px" });
   } else {
      document.getElementById('hrefCountofError').innerHTML = '0';
      document.getElementById("hrefCountofError").className = '';
      $("#CountOfErrorDiv").css({ backgroundColor:"", border:"0px", padding:"0px 5px 0px 0px", marginLeft:"0px" });
   }

   HrefClearChannelErrors = document.getElementById('href_clearChannelErrors');
   if (HrefClearChannelErrors) {
      HrefClearChannelErrors.innerHTML = (DashboardData.ClearChannelErrorsBusy ? 'busy' : 'clear');
   }

   if(DashboardData.LastActivityTimeStamp) {
      document.getElementById('hrefLastActivity').innerHTML = DashboardData.LastActivityTimeStamp;
   } else {
      document.getElementById('hrefLastActivity').innerHTML = 'N/A';
   }
   statusTooltipText = DashboardData.LiveStatus;

   if (statusTooltipOn == 1) {
      TOOLtooltipRefresh(statusTooltipText, document.getElementById('statusImg'));
   }
   sourceTooltipText = adjustTooltipEditScript(DashboardData.SourceTooltip);
   if (sourceTooltipOn == 1) {
      TOOLtooltipRefresh(sourceTooltipText, document.getElementById('sourceImg'));
   }
   filterTooltipText = adjustTooltipEditScript(DashboardData.FilterTooltip);
   if (filterTooltipOn == 1) {
      TOOLtooltipRefresh(filterTooltipText, document.getElementById('filterImg'));
   }
   destinationTooltipText = adjustTooltipEditScript(DashboardData.DestinationTooltip);

   if (destinationTooltipOn == 1) {
      TOOLtooltipRefresh(destinationTooltipText, document.getElementById('destinationImg'));
   }

   var StatusImage = document.getElementById('statusImg');
   if (DashboardData.GreenLight) {
      StatusImage.src = DASHskinningCookieData + '/images/button-dotgreenv4.gif';
   } else if (DashboardData.YellowLight) {
      StatusImage.src = DASHskinningCookieData + '/images/button-dotyellowv4.gif';
   } else if (DashboardData.RedLight) {
      StatusImage.src = DASHskinningCookieData + '/images/button-dotredv4.gif';
   } else if (DashboardData.OffLight) {
      StatusImage.src = DASHskinningCookieData + '/images/button-dotgrayv4.gif';         
   }
   
   for (Field in DashboardData.AdditionalFields){
      $('#' + Field).text(DashboardData.AdditionalFields[Field]);
   }
}

// Mini login and ajax code
function statusLevelUpdateError(RequestObject, Status, ErrorString) {
   console.log(RequestObject);
   clearTimeout(StatusTimeoutId);
   MiniLogin.show('Iguana is not responding.', DASHonMiniLogin);
}

function updateStatus() {
   clearTimeout(StatusTimeoutId);
   var ChannelGuid = encodeURIComponent("<?cs var:js_escape(Channel.Guid) ?>");
   if (ChannelGuid == '00000000000000000000000000000000') { return false; }
   $.ajax({
      url:  '/channel_status_data.html',
      data: {
         'Channel.Guid': ChannelGuid,
         'AutomaticRequest': 1
      },
      success: statusUpdateComplete,
      error:   statusLevelUpdateError
   });
   StatusTimeoutId = setTimeout(updateStatus, 1000);
}

function DASHonMiniLogin() {
   updateStatus();
}
</script>

<?cs include:"component_status_to_db.cs" ?>
<?cs include:"component_status_from_llp.cs" ?>

<table id="iguana">
<tr>
   <td id="cookie_crumb">
   <?cs if:Channel.IsNew ?>
      <a href="/">Dashboard</a> &gt; <a href="/channel">Add Channel</a> &gt; Configure New Channel
   <?cs elif:Channel.ReadOnlyMode ?>
   <a href="/">Dashboard</a> &gt; Channel
      <select id="ChannelSelect" name="ChannelSelect">
         <option value="AddChannelOption">[Add Channel]</option>
         <?cs var:SourceOptions.ChannelSelectOptions ?>
      </select>
   <?cs else ?>
   <a href="/">Dashboard</a> &gt; <a class="channelCancel" href="/channel#Channel=<?cs var:url_escape(Channel.Name) ?>">Channel <?cs var:html_escape(Channel.Name) ?></a> &gt; Edit
   <?cs /if ?>
   </td>
</tr>
<tr>
   <td id="dashboard_body">
      <div id="divChannelConfiguration">
      <center>
      <form name="create_tables" id="create_tables" method="post" action="channel/control">
         <input type="hidden" name="Channel" value="<?cs var:html_escape(Channel.Name) ?>"/>
         <input type="submit" class="hidden_submit" name="Action" value="CreateTables" />
      </form>
      <form name="channel_data" id="channel_data" method="post" action="channel/control">  
         <input type="hidden" name="Channel.ModifiedTimestamp" value="<?cs var:html_escape(Channel.ModifiedTimestamp) ?>"/>
         <input type="hidden" name="Channel.Guid" value="<?cs var:html_escape(Channel.Guid) ?>"/>
         <div class="tabs_and_contents">
            <div class="navcontainer" style="padding-top:5px;">
               <?cs if:ErrorMessage ?>
                  <p style="font-weight: bold; color: red;margin-top:0px;"><?cs var:html_escape(ErrorMessage) ?></p>   
               <?cs elif:Channel.HasErrors || Channel.Source.HasErrors || Channel.Destination.HasErrors || (Channel.UseMessageFilter && Channel.MessageFilter.HasErrors) ?>
                  <p style="font-weight: bold; color: red;margin-top:0px;">Channel has configuration errors which will prevent it from being run.  Please see below.</p>
               <?cs elif:Channel.HasWarnings || Channel.Source.HasWarnings || Channel.Destination.HasWarnings || (Channel.UseMessageFilter && Channel.MessageFilter.HasWarnings) ?>
                  <p style="margin-top:0px;">Channel has configuration warnings.  Please see below.</p>
               <?cs /if ?>

               <h2 style="padding-top:0px;"><?cs if:!Channel.IsNew ?>Channel: <?cs var:html_escape(Channel.Name) ?><?cs else ?>&nbsp;<?cs /if ?> </h2>
      
               <!-- Channel tab -->
               <a id="channelTab" href="javascript:void(0)" onclick="return showTabContents(this, 'channelTabContents');"<?cs if:!Channel.ReadOnlyMode ?> class="current"<?cs /if?>>
                  <?cs if:Channel.Configuration.HasErrors ?>
                     <img src="/<?cs var:skin("images/icon_error.gif") ?>" class="icon_error">
                  <?cs /if ?>
                  Channel
               </a>
               
               <!-- Source tab -->
               <a id="sourceTab" href="javascript:void(0)" onclick="return showTabContents(this, 'sourceTabContents');">
                  <?cs if:Channel.Source.HasErrors ?>
                     <img src="/<?cs var:skin("images/icon_error.gif") ?>" class="icon_error">
                  <?cs elif:Channel.Source.HasWarnings ?>
                     <img src="/<?cs var:skin("images/icon_warning.gif") ?>" class="icon_warning">
                  <?cs /if ?>
                  Source
               </a>
               
               <!-- Filter tab -->
               <a id="filterTab" href="javascript:void(0)" onclick="return showTabContents(this, 'filterTabContents');">
                  <?cs if:Channel.MessageFilter.HasErrors ?>
                     <img src="/<?cs var:skin("images/icon_error.gif") ?>" class="icon_error">
                  <?cs elif:Channel.MessageFilter.HasWarnings ?>
                     <img src="/<?cs var:skin("images/icon_warning.gif") ?>" class="icon_warning">
                  <?cs /if ?>
                  Filter
               </a>
               
               <!-- Destination tab -->
               <a id="destinationTab" href="javascript:void(0)" onclick="return showTabContents(this, 'destinationTabContents');">
                  <?cs if:Channel.Destination.HasErrors ?>
                     <img src="/<?cs var:skin("images/icon_error.gif") ?>" class="icon_error">
                  <?cs elif:Channel.Destination.HasWarnings ?>
                     <img src="/<?cs var:skin("images/icon_warning.gif") ?>" class="icon_warning">
                  <?cs /if ?>
                  Destination
               </a>

               <?cs if:Channel.Source.RequiresTables || Channel.Destination.RequiresTables ?>
               <!-- Export Database Tables tab -->
               <a id="exportTablesTab" href="javascript:void(0)" onclick="return showTabContents(this, 'exportTablesTabContents');">
                  Tables
               </a>
               <?cs /if ?>
                  
            </div> <!-- /#navcontainer -->

            <div id="channel_configuration_container" class="sc_spinner_container">  
               <table class="channel_configuation_form" id="channel_configuration_table" cellspacing="0">            
                  <tbody id="channelTabContents" style="display:none;">
                     <?cs if:Channel.IsNew ?>
                     <tr class="selected">
                        <td class="left_column first_row">Channel name<font color="#ff0000">*</font></td>
                        <td class="inner_left first_row" colspan="3">
                           <input type="text" class="full_length" name="Channel" value="<?cs var:html_escape(Channel.Name) ?>" maxlength="30" />
                        </td>
                     </tr>
                     <?cs else ?>
                     <input type="hidden" name="Channel" value="<?cs var:html_escape(Channel.Name) ?>" maxlength="30" class="ChannelInput" />
                     <tr class="selected">
                        <td class="left_column first_row">Channel name<font color="#ff0000">*</font></td>
                        <td class="inner_left first_row" colspan="3">
                        <?cs if:Channel.ReadOnlyMode ?>
                           <?cs var:html_escape(Channel.Name) ?>   
                        <?cs else ?>
                           <input id="Channel_New" class="full_length" type="text" name="Channel_New" value="<?cs if:Channel.ChannelNameNew != null ?><?cs var:Channel.ChannelNameNew ?><?cs else ?><?cs var:html_escape(Channel.Name) ?><?cs /if ?>" maxlength="30" /> 
                        <?cs /if ?>
                        </td>
                     </tr>
                     <?cs /if ?>
                     <tr class="selected">
                        <td class="left_column">Description</td>
                        <td class="inner_left" colspan="3">
 
                        <?cs if:Channel.ReadOnlyMode ?>
                           <?cs var:Channel.HyperlinkedDescription ?>
                           <input type="hidden" id="description" name="Description">
                        <?cs else ?>
                            <textarea id="description" class="full_length" name="Description"><?cs var:html_escape(Channel.Description) ?></textarea>
                        <?cs /if ?>
                        </td>
                     </tr>
                     <tr class="selected">
                        <td class="left_column">Start automatically</td>
                        <td class="inner_left" colspan="3">
                        <?cs if:Channel.StartAutomatically ?>
                           <?cs if:Channel.ReadOnlyMode ?>Yes
                           <?cs else ?><input type="checkbox" class="no_style" name="StartAutomatically" checked />
                           <?cs /if ?>
                        <?cs else ?>
                           <?cs if:Channel.ReadOnlyMode ?>No
                           <?cs else ?><input type="checkbox" class="no_style" name="StartAutomatically" />
                           <?cs /if ?>
                        <?cs /if ?>
                        </td>
                     </tr>
                  
                     <tr class="selected">
                        <td class="left_column">Logging level</td>
                        <td class="inner_left" colspan="3">
                           <?cs if:Channel.ReadOnlyMode ?>                                       
                              <?cs each:loggingLevel = LoggingLevels ?>
                                    <?cs if:name(loggingLevel) == Channel.LoggingLevel ?> <?cs var:html_escape(loggingLevel) ?><?cs /if ?>                                     
                              <?cs /each ?>
          
                           <?cs else ?>
                              <select name="LoggingLevel">
                                 <?cs each:loggingLevel = LoggingLevels ?>
                                 <option value="<?cs name:loggingLevel ?>" <?cs if:name(loggingLevel) == Channel.LoggingLevel ?>selected<?cs /if ?>>
                                    <?cs var:loggingLevel ?>
                                 </option>
                              <?cs /each ?>
                              </select>
                           <?cs /if ?>
                        </td>
                     </tr>

                     <?cs if:BelongsToCount != 0 || !Channel.ReadOnlyMode ?>
                     <tr class="selected">			      
                        <td class="left_column">Groups</td>
                        <td> 
                           <table>
                           <tbody id="tableGroupsBody">
                           <?cs if:!Channel.ReadOnlyMode && CurrentUserCanAdmin?>
                              <?cs each: group = Groups ?>
                              <tr id="group_row_<?cs var:html_escape(group.Value) ?>" <?cs if !group.InGroup ?>style="display:none"<?cs /if ?>>
                                 <td>
                                    <a class="recipient"
                                       onMouseOver="TOOLtooltipLink(generateTooltipText('<?cs var:html_escape(js_escape(newline_html(group.Description))) ?>', <?cs var: group.ChannelCount ?>,<?cs var: ChannelCount ?>), null, this);"
                                       onMouseOut="TOOLtooltipClose();"
                                       onmouseup="TOOLtooltipClose();"
                                    >
                                       <span><?cs var:html_escape(group.Name) ?></span></a>
                                 </td>
                                 <td>
                                 <?cs if:group.Name != "All Channels" ?>
                                    <a class="action-button-small grey" href="javascript:removeGroup('<?cs var:group.Value ?>', '<?cs var:js_escape(html_escape(group.Name)) ?>')">
                                       Remove 
                                    </a>
                                 <?cs /if ?>
                                 </td>
                              </tr>
                              <?cs /each ?>
                           <?cs else ?>
                              <tr>
                                 <td>
                                 <?cs each: group = Groups ?>
                                    <?cs if group.InGroup ?>
                                    <a class="recipient" 
                                       href="<?cs if:CurrentUserCanAdmin ?>/settings#Page=channel/group/edit?group=<?cs var:html_escape(url_escape(group.Name)) ?><?cs else ?>/settings#Page=channel/group<?cs /if ?>"
                                       onMouseOver="TOOLtooltipLink(generateTooltipText('<?cs var:html_escape(js_escape(newline_html(group.Description))) ?>', <?cs var: group.ChannelCount ?>,<?cs var: ChannelCount ?>), null, this);"
                                       onMouseOut="TOOLtooltipClose();"
                                       onmouseup="TOOLtooltipClose();"
                                    >
                                       <span><?cs var:html_escape(group.Name) ?></span>
                                    </a>
                                    <?cs /if?>
                                 <?cs /each ?>
                                 </td>
                              </tr>
                           <?cs /if ?>

                           <?cs if:!Channel.ReadOnlyMode && CurrentUserCanAdmin ?>
                              <tr id="groupDropDownRow" <?cs if GroupCount == BelongsToCount ?>style="display:none"<?cs /if ?>>
                                 <td colspan="2">
                                    <select id="groupDropDown" value="noval" onchange="onDropdownChange(this);">
                                       <option value="noval">Add this channel to a group ...</option>
                                          <?cs each: group = Groups ?>
                                             <?cs if:!group.InGroup && "All Channels" != group.Name ?> 
                                             <option id="group_option_<?cs var:group.Value ?>" value="<?cs var:group.Value ?>">
                                                <?cs var:html_escape(group.Name) ?>
                                             </option>
                                             <?cs /if ?>
                                          <?cs /each ?>
                                    </select>
                                 </td>
                                 <td><!-- remove button column --></td>
                              </tr>
                           <?cs /if ?>
                           </tbody>
                           </table>
                           
                           <?cs if:CurrentUserCanAdmin ?>
                              <font color="green" size=1>To create<?cs if GroupCount > 0 ?> or remove<?cs /if?> groups, go to <a href="/settings#Page=channel/group">Settings &gt; Channel Groups</a></font>
                           <?cs /if ?> 
                        </td>
                     </tr>
                     <?cs /if ?>
                     <?cs if:Channel.IsEncrypted && !Channel.IsNew ?>
                     <tr class="selected">
                        <td class="left_column">Encryption key</td>
                        <td class="inner_left" colspan="3">
                           <?cs var:html_escape(Channel.EncryptionKey) ?>
                        </td>
                     </tr>
                     <input type="hidden" name="EncryptionKey" value="<?cs var:html_escape(Channel.EncryptionKey) ?>" />
                     <?cs /if ?>
                  </tbody>
                  <tbody id="sourceTabContents" style="display: none;">
                     <tr class="selected">
                        <td class="left_column first_row">Source</td>
                        <td class="inner_left first_row" colspan="3">
                           <?cs var:html_escape(Channel.Source.Type) ?>
                        </td>
                     </tr> 
                     <input type="hidden" name="SourceType" value="<?cs var:html_escape(Channel.Source.Type) ?>" />
                     <?cs if:   Channel.Source.IsLLP ?>         <?cs linclude: "component_from_llp.cs" ?>
                     <?cs elif: Channel.Source.IsDatabase ?>    <?cs linclude: "component_from_database.cs" ?>
                     <?cs elif: Channel.Source.IsFile ?>        <?cs linclude: "component_from_file.cs" ?>
                     <?cs elif: Channel.Source.IsExecutable ?>  <?cs linclude: "component_from_executable.cs" ?>
                     <?cs elif: Channel.Source.IsHttp ?>        <?cs linclude: "component_from_http.cs" ?>
                     <?cs elif: Channel.Source.IsFromChannel ?> <?cs linclude: "component_from_channel.cs" ?>
                     <?cs elif: Channel.Source.IsMapper ?>      <?cs linclude: "component_from_mapper.cs" ?>
                     <?cs else ?>
                        <tr>
                           <td colspan="4" align="center">
                              <font color="red">Unknown source type.  Please try again.</font>
                           </td>
                        </tr>
                     <?cs /if ?>
                  </tbody>
               
                  <tbody id="filterTabContents" style="display: none;">
                     <input type="hidden" id="inputChannelHasMessageFilter" name="ChannelHasMessageFilter" value="<?cs if:Channel.HasMessageFilter ?>true<?cs else ?>false<?cs /if ?>" />
                     <?cs linclude: "message_filter.cs" ?>
                  </tbody>
               
                  <tbody id="destinationTabContents" style="display: none;">         
                     <tr class="selected">
                        <td class="left_column first_row">Destination</td>
                        <td class="inner_left first_row" colspan="3">
                           <?cs var:html_escape(Channel.Destination.Type) ?>
                        </td>
                     </tr> 
                     <input type="hidden" name="DestinationType" value="<?cs var:html_escape(Channel.Destination.Type) ?>" />
                     
                     <?cs if:   Channel.Destination.IsLLP ?>         <?cs linclude: "component_to_llp.cs" ?>
                     <?cs elif: Channel.Destination.IsDatabase ?>    <?cs linclude: "component_to_database.cs" ?>
                     <?cs elif: Channel.Destination.IsFile ?>        <?cs linclude: "component_to_file.cs" ?>
                     <?cs elif: Channel.Destination.IsExecutable ?>  <?cs linclude: "component_to_executable.cs" ?>
                     <?cs elif: Channel.Destination.IsHttp ?>  <?cs linclude: "component_to_http.cs" ?>
                     <?cs elif: Channel.Destination.IsToChannel ?>  <?cs linclude: "component_to_channel.cs" ?>
                     <?cs elif: Channel.Destination.IsMapper ?>     <?cs linclude: "component_to_mapper.cs" ?>
                     <?cs else ?>
                        <tr>
                           <td colspan="4" align="center">
                           <font color="red">Unknown destination type.  Please try again.</font>
                           </td>
                        </tr>
                     <?cs /if ?>
                  </tbody>

                  <tbody id="groupsTabContents" style="display: none;">         
                  </tbody>

                  <tbody id="exportTablesTabContents" style="display: none;">
                     <tr>
                        <td colspan="4" class="center first_row">
                        <center>
                        When using a database as a
                        <?cs if:Channel.Source.RequiresTables && Channel.Destination.RequiresTables ?>source or destination
                        <?cs elif:Channel.Source.RequiresTables ?>source
                        <?cs elif:Channel.Destination.RequiresTables ?>destination<?cs /if ?>
                        component, you may need to export the database tables before you use the channel.
                        See <a href="<?cs var:help_link('iguana4_exporting_tables') ?>" target="_blank">Exporting the Database Tables</a> for more details.
                        </center>
                        </td>
                     </tr>
                  
                     <?cs def:ifCanExportTables(output) ?>
                        <?cs if:CurrentUserCanAdmin && !IsRunning && Channel.ReadOnlyMode ?><?cs var:output ?><?cs /if ?>
                     <?cs /def ?>
                     
                     <?cs def:ifCannotExportTables(output) ?>
                        <?cs if:!CurrentUserCanAdmin || IsRunning || !Channel.ReadOnlyMode ?><?cs var:output ?><?cs /if ?>
                     <?cs /def ?>
                     
                     <?cs def:exportTablesButtonMouseEvents() ?>
                        <?cs if:!CurrentUserCanAdmin ?>
                           onMouseOver="TOOLtooltipLink('You do not have the necessary permissions to export database tables.', null, this);"
                        <?cs elif:Channel.IsNew ?>
                           onMouseOver="TOOLtooltipLink('You must finish adding the channel before exporting database tables.', null, this);"
                        <?cs elif:!Channel.ReadOnlyMode ?>
                           onMouseOver="TOOLtooltipLink('You cannot export database tables while editing the channel.', null, this);"
                        <?cs else ?>
                           onMouseOver="TOOLtooltipLink('The channel must be stopped before exporting database tables.', null, this);"
                        <?cs /if ?>
                        onMouseOut="TOOLtooltipClose();"
                        onmouseup="TOOLtooltipClose();"
                     <?cs /def ?>

                     <?cs if:Channel.Source.RequiresTables ?>
                     <tr>
                        <td colspan="4" class="center">
                        <center>
                        <table>
                           <tr>
                              <td>
                                 <a id="hrefExportSourceTablesButton" class="action-button blue"
                                    href="/export_tables.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&ExportSourceTables=on"
                                    <?cs call:ifCannotExportTables('style="display:none;"') ?> >
                                    <span>Export Source Tables</span>
                                 </a>
                                 <a id="ExportSourceTablesButton" class="action-button grey disabled"
                                    <?cs call:ifCanExportTables('style="display:none;"') ?>
                                    <?cs call:exportTablesButtonMouseEvents() ?> >
                                    Export Source Tables
                                 </a>
                              </td>
                           </tr>
                        </table>
                        </center>
                        </td>
                     </tr>
                     <?cs /if ?>

                     <?cs if:Channel.Destination.RequiresTables ?>
                     <tr>
                        <td colspan="4" class="center">
                        <center>
                        <table>
                           <tr>
                              <td>
                                 <a id="hrefExportDestinationTablesButton" class="action-button blue"
                                    href="/export_tables.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&ExportDestinationTables=on"
                                    <?cs call:ifCannotExportTables('style="display:none;"') ?> >
                                    <span>Export Destination Tables</span>
                                 </a>
                                 <a class="action-button grey disabled" id="ExportDestinationTablesButton"
                                    <?cs call:ifCanExportTables('style="display:none;"') ?>
                                    <?cs call:exportTablesButtonMouseEvents() ?> >
                                    Export Destination Tables
                                 </a>
                              </td>
                           </tr>
                        </table>
                        </center>
                        </td>
                     </tr>
                     <?cs /if ?>
                  
                     <?cs if:Channel.Source.RequiresTables && Channel.Destination.RequiresTables ?>
                     <!-- In the rare case of a DB -> DB channel, we will have a button to export both source and destination -->
                     <tr>
                        <td colspan="4" class="center">
                        <center>
                        <table>
                           <tr>
                              <td>
                                 <a id="hrefExportSourceAndDestinationTablesButton" class="action-button blue"
                                    href="/export_tables.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&ExportSourceTables=on&ExportDestinationTables=on"
                                    <?cs call:ifCannotExportTables('style="display:none;"') ?> >
                                    <span>Export Source and Destination Tables</span>
                                 </a>
                                 <a class="action-button grey disabled" id="ExportSourceAndDestinationTablesButton"
                                    <?cs call:ifCanExportTables('style="display:none;"') ?>
                                    <?cs call:exportTablesButtonMouseEvents() ?> >
                                    Export Source and Destination Tables
                                 </a>
                              </td>
                           </tr>
                        </table>
                        </center>
                        </td>
                     </tr>
                     <?cs /if ?>
               
                  </tbody>
               
                  <tbody id="tbodyAlwaysVisible">
                  </tbody>
               </table>
            </div>
         <br />
         
      </div> <!-- /#tabs_and_content -->
      
      <?cs if:Channel.IsNew ?>

      <input type="hidden" name="Action.AddChannel" value="Create" />
      <input type="hidden" name="Action" value="AddChannel" />
      <input type="hidden" name="editTab" id="editTab" value="channelTab">
      <input type="hidden" name="channelGroups" id="channelGroups"/>
      <table id="buttons">
         <tr>
            <td>
            <div class="action_button_container">
               <a class="action-button blue sc-job" id="channelSave"><span>&nbsp;Add Channel</span></a>
            </div>
            </td>
         </tr>
      </table>

      <?cs else ?>   

      <input type="hidden" name="editTab" id="editTab" value="channelTab">
      <input type="hidden" name="Channel" id="Channel" value="<?cs var:html_escape(Channel.Name) ?>">
      <input type="hidden" name="channelGroups" id="channelGroups"/>
      <table id="buttons">
         <tr>
         <?cs if:CurrentUserCanReconfigure ?>
            <?cs if:Channel.ReadOnlyMode ?>
               <td>
                  <input type="hidden" name="ReadOnlyMode" id="ReadOnlyMode" value="Edit">
                  <a class="action-button blue" id="hrefChannelEditButton" <?cs if:IsRunning ?>style="display:none;"<?cs /if ?>>
                     Edit Channel
                  </a>
                  <a class="action-button grey disabled" style="<?cs if:!IsRunning ?>display:none;<?cs /if ?>" id="ChannelEditButton" onMouseOver="TOOLtooltipLink('The channel must be stopped before editing.', null, this);" onMouseOut="TOOLtooltipClose();" onmouseup="TOOLtooltipClose();" >
                     Edit Channel
                  </a>
               </td>
            <?cs else ?>
               <td>
                  <input type="hidden" name="Action" value="ApplyChanges" />
                  <input type="hidden" name="Action.ApplyChanges" value="Update" />
                  <a class="action-button blue" id="channelSave">Save Changes</a>
               </td>
               <td>
                  <a class="action-button blue" id="channelCancel">Cancel</a>
               </td>
            <?cs /if ?>                                 
         <?cs /if ?>
         </tr>
      </table>

      <?cs /if ?>
      </form>
      </center>
      </div> <!-- /#divChannelConfiguration --> 
   </td> <!-- /#cashboard_body -->
</tr>
</table>

<div id="side_panel">
   <?cs if:!Channel.IsNew ?>
   <table id="side_table">
   <tr>
      <th id="side_header">
         Control Panel
      </th>
   </tr>
   <tr>
      <td id="side_body" style="padding-bottom:5px;">
         <table id="tableControlPanel" style="width: 100%; margin:0px;padding:0px;">            
            <tr>
               <td style="padding:15px 0px 0px 0px;">
                  <?cs if:CurrentUserCanStartStop ?>
                  <div style="margin-bottom:15px; text-align: center;">
                     <a class="action-button blue" href="javascript:channelDoAction('stop');" id="hrefStopButton" <?cs if:CurrentUserCanStartStop && IsRunning ?> <?cs else ?>style="display:none;"<?cs /if ?>>Stop Channel</a>
                     <a class="action-button red" href="javascript:channelDoAction('force_stop');" id="hrefForceStopButton" style="display:none;">Force Stop</a>
                     <a class="action-button grey disabled" style="<?cs if:CurrentUserCanStartStop && !IsRunning && !Channel.ReadOnlyMode ?> <?cs else ?>display:none;<?cs /if ?>"
                     onMouseOver="TOOLtooltipLink('The channel cannot be started while editing.', null, this);" onMouseOut="TOOLtooltipClose();" onmouseup="TOOLtooltipClose();">Start Channel</a>
                     <a class="action-button blue" href="javascript:channelDoAction('start');" id="hrefStartButton" <?cs if:CurrentUserCanStartStop && !IsRunning && Channel.ReadOnlyMode ?> <?cs else ?>style="display:none;"<?cs /if ?>>Start Channel</a>
                  </div>
                  <?cs /if ?>
                  <div style="margin:0px 0px 0px 53px;">
                     <?cs if:Channel.Source.HasErrors ?>
                        <img src="/images/icon_<?cs var:html_escape(Channel.Source.ShortName) ?>_error.gif" id="sourceImg" class="edit_permission_required">
                     <?cs elif:Channel.Source.HasWarnings ?>
                        <img src="/images/icon_<?cs var:html_escape(Channel.Source.ShortName) ?>_warning.gif" id="sourceImg" class="edit_permission_required">
                     <?cs else ?>
                        <img src="/images/icon_<?cs var:html_escape(Channel.Source.ShortName) ?>.gif" id="sourceImg" class="edit_permission_required">
                     <?cs /if ?>
                     <?cs if:Channel.UseMessageFilter ?>
                        <?cs if:Channel.MessageFilter.HasErrors ?>
                           <img src="/images/arrow_filter_error.gif" id="filterImg" class="edit_permission_required">
                        <?cs elif:Channel.MessageFilter.HasWarnings ?>
                           <img src="/images/arrow_filter_warning.gif" id="filterImg" class="edit_permission_required">
                        <?cs else ?>
                           <img src="/images/arrow_filter.gif" id="filterImg" class="edit_permission_required">
                        <?cs /if ?>
                     <?cs else ?>
                        <img src="/images/arrow.gif" id="filterImg">
                     <?cs /if ?>   
                     <?cs if:Channel.Destination.HasErrors ?>
                        <img src="/images/icon_<?cs var:html_escape(Channel.Destination.ShortName) ?>_error.gif" id="destinationImg" class="edit_permission_required">
                     <?cs elif:Channel.Destination.HasWarnings ?>
                        <img src="/images/icon_<?cs var:html_escape(Channel.Destination.ShortName) ?>_warning.gif" id="destinationImg" class="edit_permission_required">
                     <?cs else ?>
                        <img src="/images/icon_<?cs var:html_escape(Channel.Destination.ShortName) ?>.gif" id="destinationImg" class="edit_permission_required">
                     <?cs /if ?>     
                  </div>
               </td>
            </tr>
         </table>
      </td>
   </tr>
   <?cs if:CurrentUserCanAdmin ?>
   <tr id="export-channel-form-container"
       <?cs if:Channel.Source.Error.Commit || Channel.Destination.Error.Commit || Channel.MessageFilter.Error.Commit ?>
         style="display:none"
       <?cs /if ?>
   >
      <td id="side_body" style="padding-bottom: 0; text-align: center">
         <p>
            <form method="POST" action="/settings#Page=export" id="export-channel-form">
               <input type="hidden" name="redirect_to" value="export">
               <input type="hidden" name="guid" value="<?cs var:Channel.Guid ?>">
               <a href="" id="export-channel">Export</a> this channel
            </form>
         </p>
      </td>
   </tr>
   <?cs /if ?>

   <tr>
      <td id="side_body">   
         <div class="textrow" style="height:40px;">
            <h4 class="side_title" style="float:left;">Status</h4>
            <p style="float:right;margin-top:15px;">
               <?cs if:GreenLight ?>
                 <img src="/<?cs var:skin("images/button-dotgreenv4.gif") ?>" id="statusImg">
               <?cs elif:YellowLight ?>
                 <img src="/<?cs var:skin("images/button-dotyellowv4.gif") ?>" id="statusImg">
               <?cs elif:RedLight ?>
                 <img src="/<?cs var:skin("images/button-dotredv4.gif") ?>" id="statusImg">   
               <?cs elif:OffLight ?> 
                 <img src="/<?cs var:skin("images/button-dotgrayv4.gif") ?>" id="statusImg">
               <?cs /if ?>
            </p>
         </div>
         <div class="textrow" style="padding-right:5px;">
            <p class="alignleft">Last Activity:</p>
            <p class="alignright"><span id="spnLastActivityTimeStamp"><a href="/log_browse?Source=<?cs var:url_escape(Channel.Name)?>&NavAction=End" id="hrefLastActivity"><?cs if:LastActivityTimeStamp ?><?cs var:html_escape(LastActivityTimeStamp) ?><?cs else ?>N/A<?cs /if ?></a></span></p>
         </div>               
         <div class="textrow" style="padding-right:5px;">
            <p class="alignleft">Inbound Speed:</p>
            <p class="alignright"><span id="spnInboundTps"><?cs var:InboundTps ?></span></p>                  
         </div>
         <?cs if:OutboundTps ?>
         <div class="textrow" style="padding-right:5px;">
            <p class="alignleft">Outbound Speed:</p>
            <p class="alignright"><span id="spnOutboundTps"><?cs var:OutboundTps ?></span></p>                  
         </div>
         <?cs /if ?>

         <?cs def:show_clear(js_action, link_text) ?>
            <?cs if:CurrentUserCanReconfigure ?>
               [<a id="href_<?cs var:js_action  ?>" href="javascript:channelDoAction('<?cs var:js_action ?>');"><?cs var:link_text ?></a>]
            <?cs /if ?>
         <?cs /def ?>

         <div class="textrow" id="CountOfErrorDiv" style="padding-right:5px;">
            <p class="alignleft">Errors: 
               <?cs if:CurrentUserCanReconfigure ?>
                  <?cs if:ClearChannelErrorsBusy!=0 ?><?cs set:clear_text='busy' ?><?cs else ?><?cs set:clear_text='clear' ?><?cs /if ?>
                  <?cs call:show_clear('clearChannelErrors',clear_text) ?>
               <?cs /if ?>
            </p>
            <p class="alignright">
               <span id="spnCountOfError">
                  <?cs if: CountOfError ?>
                     <a class="error" id="hrefCountofError" href="/log_browse?Source=<?cs var:url_escape(Channel.Name)?>&Type=errors_unmarked"><?cs var:html_escape(CountOfError) ?></a><?cs else ?><a id="hrefCountofError" href="/log_browse?Source=<?cs var:url_escape(Channel.Name)?>&Type=errors_unmarked">0</a>
                  <?cs /if ?>
               </span>
            </p>
         </div>

         <div class="textrow" style="padding-right:5px;">
            <p class="alignleft">Received:</p>
            <p class="alignright"><span id="spnTotalEnqueued"><?cs var:html_escape(CountOfEnqueued) ?></span></p>
         </div>

         <?cs if:!Channel.Destination.IsToChannel ?>
         <div class="textrow" style="padding-right:5px;">
            <p class="alignleft">Queued:</p>
            <p class="alignright"><span id="spnQueued"><?cs if:CountOfRemaining ?><?cs var:html_escape(CountOfRemaining) ?><?cs else ?>0<?cs /if ?></span></p>
         </div>
         <?cs /if ?>

         <?cs if:!Channel.Destination.IsToChannel ?>
         <div class="textrow" style="padding-right:5px;">
            <p class="alignleft">Total Processed:</p>
            <p class="alignright"><span id="spnTotalProcessed"><?cs if:CountOfProcessed ?><?cs var:html_escape(CountOfProcessed) ?><?cs else ?>0<?cs /if ?></span></p>
         </div>
         <?cs /if ?>

         <?cs if:!Channel.Destination.IsToChannel ?>
         <div class="textrow" style="padding-right:5px;">
            <p class="alignleft">Current Processed: 
               <?cs if:CurrentUserCanReconfigure ?>
                  <?cs call:show_clear('clearCurrentProcessed','clear') ?>
               <?cs /if ?>
            </p>
            <p class="alignright"><span id="spnCurrentProcessed"><?cs if:CountOfCurrentProcessed ?><?cs var:html_escape(CountOfCurrentProcessed) ?><?cs else ?>0<?cs /if ?></span></p>
         </div>
         <?cs /if ?>
      </td>
   </tr>
   <?cs if:Channel.ReadOnlyMode ?>
   <tr>      
      <td class="side_item">
         <h4 class="side_title">Logging Level</h4>

         <div style="width:100%;">
            <select onchange="updateLoggingLevel(this.value);"
               onfocus="updateLoggingLevelMessage('');"
               <?cs if:!CurrentUserCanReconfigure ?>disabled="disabled"<?cs /if ?>>
               <?cs each:loggingLevel = LoggingLevels ?>
                  <option value="<?cs name:loggingLevel ?>" <?cs if:name(loggingLevel) == ChannelLoggingLevel ?>selected<?cs /if ?>>
                     <?cs var:loggingLevel ?>
                  </option>
               <?cs /each ?>
            </select>
         </div>
         <div id="spanLoggingLevelMessage" style="clear:both;padding-top:5px;padding-bottom:0px;margin-bottom:0px;"></div>
            
      </td>
   </tr>
   <?cs /if ?>
   <tr>
   <?cs if:CurrentUserCanAdmin ?>
      <td class="side_item" style="padding-bottom:5px;">
         <table id="tableControlPanel" style="margin:0px;padding:0px; width: 100%;">            
            <tr>
               <td style="padding:15px 0px 0px 0px;">
                  <div style="margin-bottom: 15px; text-align: center;">
                     <form id="remove_<?cs var: Channel.Guid ?>">
                        <input type="hidden" id="channel_name" name="channel_name" value="<?cs var: Channel.Name ?>" />
                        <input type="hidden" id="channel_guid" name="channel_guid" value="<?cs var: Channel.Guid ?>" />
                     </form>
                     <a class="action-button grey" id="hrefRemoveButton" <?cs if: !Channel.ReadOnly  ?> <?cs else ?>style="display:none;"<?cs /if ?>>Remove Channel</a>
                     <a class="action-button grey disabled" id="hrefRemoveButtonDisabled" style="<?cs if: !Channel.ReadOnly ?>display:none;<?cs else ?> <?cs /if ?>"
                     onMouseOver="TOOLtooltipLink('The channel must be stopped before removing.', null, this);" onMouseOut="TOOLtooltipClose();" onmouseup="TOOLtooltipClose();">
                        Remove Channel
                     </a>
                  </div>
               </td>
            </tr>
         </table>
      </td>
   <?cs /if ?>
   </tr>
   </table>

   <?cs /if ?> <!-- if:!Channel.IsNew -->

   <table id="side_table">
      <tr>
         <th id="side_header"> Page Help </th>
      </tr>    
      <tr>
         <td id="side_body">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon" style="margin-bottom:0px;">
            <li>
               <a href="<?cs var:help_link('iguana4_configuring_channel_details') ?>" target="_blank">Configuring the Channel Details</a>
            </li>
            <?cs if:Channel.Source.IsHttp ?>
            <li>
               <a href="http://wiki.interfaceware.com/222.html?v=6.0.0" target="_blank">Calling Iguana as a Web Service</a>
            </li>
            <li>
                <a href="http://wiki.interfaceware.com/745.html?v=6.0.0" target="_blank">Serving Files from the Web Service (From HTTP Channel Server) Port</a>
            </li>
            <?cs /if ?>
            <li>
            <?cs if:Channel.Source.IsLLP ?>
               <a href="<?cs var:help_link('iguana4_llp_listener') ?>" target="_blank">LLP Listener</a>
            <?cs elif: Channel.Source.IsDatabase ?>
               <a href="<?cs var:help_link('iguana4_from_database') ?>" target="_blank">From Database</a>
            <?cs elif: Channel.Source.IsFile ?>
               <a href="<?cs var:help_link('iguana4_from_file') ?>" target="_blank">From File</a>
            </li>
            <li>
               <a href="<?cs var:help_link('iguana4_from_file_ftp_server') ?>" target="_blank">Downloading Files from an FTP Server</a>
            <?cs elif: Channel.Source.IsExecutable ?>
               <a href="<?cs var:help_link('iguana4_from_plugin') ?>" target="_blank">From Plugin</a>
            <?cs elif: Channel.Source.IsHttp ?>
               <a href="<?cs var:help_link('iguana4_from_https') ?>" target="_blank">From HTTPS</a>
            <?cs elif: Channel.Source.IsFromChannel ?>
               <a href="<?cs var:help_link('iguana4_from_channel') ?>" target="_blank">From Channel</a>
            <?cs /if ?>
            </li>
            <li>
               <a href="<?cs var:help_link('iguana4_configuring_filter') ?>" target="_blank">Configuring the Filter</a>
            </li>
            <li>
            <?cs if:Channel.Destination.IsLLP ?>
               <a href="<?cs var:help_link('iguana4_llp_client') ?>" target="_blank">LLP Client</a>
            <?cs elif: Channel.Destination.IsDatabase ?>
               <a href="<?cs var:help_link('iguana4_to_database') ?>" target="_blank">To Database</a>
            <?cs elif: Channel.Destination.IsFile ?>
               <a href="<?cs var:help_link('iguana4_to_file') ?>" target="_blank">To File</a>
            </li>
	         <li>
               <a href="<?cs var:help_link('iguana4_to_file_ftp_server') ?>" target="_blank">Uploading Files to an FTP Server</a>
            <?cs elif: Channel.Destination.IsExecutable ?>
               <a href="<?cs var:help_link('iguana4_to_plugin') ?>" target="_blank">To Plugin</a>
            <?cs elif: Channel.Destination.IsHttp ?>
               <a href="<?cs var:help_link('iguana4_to_https') ?>" target="_blank">To HTTPS</a>
            <?cs elif: Channel.Destination.IsToChannel ?>
               <a href="<?cs var:help_link('iguana4_to_channel') ?>" target="_blank">To Channel</a>
            <?cs elif: Channel.Destination.IsMapper  ?>
               <a href="<?cs var:help_link('iguana_to_translator')?>" target="_blank">To Translator</a>
            <?cs /if ?>
            </li>
            <?cs if:Channel.IsNew ?>
            <li>
               <a href="<?cs var:help_link('iguana4_adding_channel') ?>" target="_blank">Adding the Channel</a>
            </li>
            <?cs elif: CurrentUserCanAdmin && !Channel.ReadOnly ?>
            <li>
               <a href="<?cs var:help_link('iguana4_editing_channel') ?>" target="_blank">Editing a Channel</a>
            </li>
            <?cs /if ?>

            <?cs if:Channel.Destination.IsExecutable ?>
            <li>
               <a href="<?cs var:help_link('iguana4_exporting_legacy_database_tables') ?>" target="_blank">Exporting the Legacy Database Tables</a>
            </li>
            <?cs /if ?>

            <?cs if:Channel.Source.RequiresTables || Channel.Destination.RequiresTables ?>
            <li>
               <a href="<?cs var:help_link('iguana4_exporting_tables') ?>" target="_blank">Exporting the Database Tables</a>
            </li>
            <?cs /if ?>
            </ul>
         </td>
      </tr>
   </table>
</div>

<div id="helpTooltipDiv" class="helpTooltip">
   <b id="helpTooltipTitle"></b>
   <em id="helpTooltipBody"></em>  
   <input type="hidden" name="helpTooltipId" id="helpTooltipId" value="0">
</div>

<script type="text/javascript">

$(document).ready(function() {

   if (typeof runAddEditChannelsPagesSetup == "function") {
      runAddEditChannelsPagesSetup();
   } else {
      console.log("There is a problem with the loading order.");
   }

   $(".channelCancel, #channelCancel").click(function() {
      navigateAway('<?cs var:js_escape(Channel.Name) ?>');
      return false;
   });

});

</script>

