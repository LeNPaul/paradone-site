/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

//Tooltip globals
var DASHstartStopStateNone   = 0;
var DASHstartStopStateHover  = 1;
var DASHstartStopStateActive = 2;
var ToolTipIndex = "";
var ToolTipType = "";
var ToolTipText = "";
var DASHtooltipComponent = '';

//counts/names
var DASHserverTime = null;

//start stop
var DASHstartStopState = DASHstartStopStateNone;
var DASHstartStopComponent = null;

//Search Filter
var DASHsearchFieldTypingTimeoutId = null;
var DASHfilterString = '';

var DASHskinningCookieData = COOKIEread('IguanaSkinningDirectoryCookie');
if (DASHskinningCookieData == null)
{
   DASHskinningCookieData = '';
}

var DASHstartStopStateNone   = 0;
var DASHstartStopStateHover  = 1;
var DASHstartStopStateActive = 2;
var DASHstartStopState = DASHstartStopStateNone;
var DASHstartStopComponent = null;

var DASHstartImage       = DASHskinningCookieData + '/images/button-startv4.gif';
var DASHstartImageHover  = DASHskinningCookieData + '/images/button-start-hover.gif';
var DASHstartImageActive = DASHskinningCookieData + '/images/button-start-active.gif';
var DASHstartImageDisabled = DASHskinningCookieData + '/images/button-start-grey.gif';
var DASHstopImage        = DASHskinningCookieData + '/images/button-stopv4.gif';
var DASHstopImageHover   = DASHskinningCookieData + '/images/button-stop-hover.gif';
var DASHstopImageActive  = DASHskinningCookieData + '/images/button-stop-active.gif';
var DASHstopImageDisabled = DASHskinningCookieData + '/images/button-stop-grey.gif';
var DASHarrowFilterImgSrc = DASHskinningCookieData + '/images/arrow_filter.gif';
var DASHarrowImgSrc = DASHskinningCookieData + '/images/arrow.gif';
var DASHarrowFilterHoverImgSrc = DASHskinningCookieData + '/images/arrow_filter_hover.gif';
var DASHarrowHoverImgSrc = DASHskinningCookieData + '/images/arrow_hover.gif';
var DASHarrowFilterHoverErrorImgSrc = DASHskinningCookieData + '/images/arrow_filter_hover_error.gif';
var DASHarrowFilterErrorImgSrc = DASHskinningCookieData + '/images/arrow_filter_error.gif';
var DASHarrowFilterHoverWarningImgSrc = DASHskinningCookieData + '/images/arrow_filter_hover_warning.gif';
var DASHarrowFilterWarningImgSrc = DASHskinningCookieData + '/images/arrow_filter_warning.gif';

var DASHmodel = null;
var DASHmodelQueryOptions = null;

var DASHupdateStatusHtml = '';

var DASHcolumnAfterServerNameIndex = 3;
var DASHnotSelectedLightStatus = 3;

var DASHselectedServers = null;

//also check DWBdashboardRender
var DASHmodelQueryOptionsDefault =
{
   ChannelList : {
      MinChannelRowIndex : 0,
      MaxChannelRowIndex : 30,
      Ascending : true,
      SortName : 'Name'},
   ServerList : {
      SortColumnName : 'Server Name',
      Ascending : true
   }
};

var DASHchannelTable = null;
var DASHserverTable = null;

function DASHsortInfoCookieName(){
   return DASHcookieName('Iguana-Dashboard-SortInfo');
}

function DASHjsonStringify(Object){
   if (window.JSON){
      return JSON.stringify(Object);
   }
   else{
      //this is a limited stringifier, doesn't escape,
      //doesn't do arrays
      var typeToStr;
      function stringifyObj( Object ){
         Output = '{';
         var IsFirst = true;
         for (NameIndex in Object){
            if (!IsFirst) Output += ',';
            IsFirst = false;
            var Obj = Object[NameIndex];
            var ToStr = typeToStr[typeof Obj];
            Output += '"' + NameIndex + '":' + (ToStr ? ToStr(Obj) : 'null');
         }
         Output += '}';
         return Output;
      }
      typeToStr = { 'number' : function(Obj){ return '' + Obj },
                    'string' : function(Obj){ return '"' + Obj + '"'},
                    'boolean' : function(Obj) { return (Obj ? 'true' : 'false')},
                    'object' : stringifyObj }
      return stringifyObj(Object);
   }
}

function DASHsaveQueryOptions(){
   DASHinitQueryOptions();
   COOKIEcreate(DASHsortInfoCookieName(), DASHjsonStringify(DASHmodelQueryOptions), 365);
}

function DASHgoToServerLink(ServerData, Url){
   if (ServerData.HrefPrefix){
      return '/remote_frame.html?TargetSrc='
         + encodeURIComponent(ServerData.HrefPrefix + Url)
         + '&TargetServerLabel='
         + encodeURIComponent(ServerData.ServerLabel);
   }
   else{
      return Url;
   }
}

function DASHinitQueryOptions(){
   if (!DASHmodelQueryOptions){
      try{
         DASHmodelQueryOptions = DASHparseJson(COOKIEread(DASHsortInfoCookieName()));
      }
      catch(e){
         DASHmodelQueryOptions = null;
      }
      if (!DASHmodelQueryOptions){
         DASHmodelQueryOptions = DASHmodelQueryOptionsDefault;
      }
   }
}

function DASHselectedServersCookieName(){
   return DASHcookieName('Iguana-Dashboard-SelectedServers');
}

function DASHsaveSelectedServersOptions(){
   var TmpObj = { SelectedServers : DASHselectedServers };
   COOKIEcreate(DASHselectedServersCookieName(), DASHjsonStringify(TmpObj), 365);
}

function DASHloadSelectedServersOptions(){
   try{
      var TmpObj = DASHparseJson(COOKIEread(DASHselectedServersCookieName()));
      DASHselectedServers = TmpObj.SelectedServers;
   }
   catch(e){
      DASHselectedServers = null;
   }
}

function DASHhtmlEscape(Text)
{
   Text = Text.replace(/;/g, '&#59;');
   Text = Text.replace(/&(?!#59;)/g, '&amp;');
   Text = Text.replace(/</g, '&lt;');
   Text = Text.replace(/>/g, '&gt;');
   Text = Text.replace(/"/g, '&quot;');
   return Text;
}

function DASHcookieName(Key)
{
   return Key + '-' + UniqueInstanceId + '-' + DASHcurrentUser;
}

/*  REFRESH START */
function DASHonRefreshTimer()
{
   if (REFonTimer('DASHonRefreshTimer()'))
   {
      DASHrefresh(REFgetParams());
   }
}

function DASHrefreshEnable(RefreshOn)
{
   REFrefreshEnable(RefreshOn);
   DASHrefresh(REFgetParams());
}

function DASHsetRefreshInterval(NewInterval)
{
   REFsetInterval(NewInterval);
   DASHrefresh(REFgetParams());
}
/* REFRESH END */

/* SEARCH FILTER START*/
function DASHstopMagnifyingGlass()
{
   // Stop the magnifying glass from animating, if the contents returned from the request
   // were filtered based on the value in the filter input field.
   if (DASHhtmlEscape(DASHgetFilterStringFromField()) == DASHfilterString)
   {
      var MagnifyingGlassImg = document.getElementById('imgMagnifyingGlass');
      MagnifyingGlassImg.src = DASHskinningCookieData + '/images/icon_search.gif';
      MagnifyingGlassImg.alt = '';
   }
}

function DASHanimateMagnifyingGlass()
{
   var MagnifyingGlassImg = document.getElementById('imgMagnifyingGlass');
   if (MagnifyingGlassImg.alt != 'Searching...')
   {
      MagnifyingGlassImg.src = DASHskinningCookieData + '/images/icon_search_pulse.gif';
      MagnifyingGlassImg.alt = 'Searching...';
   }
}

function DASHdisplaySearchCount(TotalCountOfChannel, CountOfChannel, QueryError)
{
   var DisplayString = '';
   if (QueryError)
   {
      DisplayString  = '<div style="width: 200px;">' +
                       '<img src="' + DASHiconWarningImgSrc + '" />' +
                       '<span>' + QueryError + '</span></div>';
   }
   else if (DASHgetFilterStringFromField())
   {
      if (CountOfChannel) DisplayString = 'Showing ' + CountOfChannel + ' of ' + TotalCountOfChannel + ' channels';
      else DisplayString = 'No matching channels';
   }

   $('#spnSearchCountDisplay')
      .html(DisplayString)
      .show();
}

function DASHgetFilterStringFromField() {
   return $("#inputChannelFilter").val();
}

function DASHfilter()
{
   COOKIEcreate(DASHcookieName('Iguana-ChannelFilterString'), escape(DASHgetFilterStringFromField()), 365);

   // Make a request to refresh all rows, rather than just the visible ones
   DASHrefresh('');
}

function DASHmakeFilterClearButtonVisible()
{
   $('#imgClearFilterIcon').css('visibility','visible');
}

function DASHonFilterStringChange(FilterString) {
   if (FilterString.length == 0) {
      DASHclearChannelFilter();
   } else {
      DASHmakeFilterClearButtonVisible();
      DASHanimateMagnifyingGlass();
      REFclearTimer();
      if (DASHsearchFieldTypingTimeoutId) {
         clearTimeout(DASHsearchFieldTypingTimeoutId);
      }
      DASHsearchFieldTypingTimeoutId = setTimeout("DASHfilter();", 500);
   }
}

function DASHclearChannelFilter() {
   $("#inputChannelFilter").val('');
   document.getElementById('imgClearFilterIcon').style.visibility = 'hidden';
   DASHanimateMagnifyingGlass();
   REFclearTimer();
   DASHfilter();
}

function DASHshowSearchTooltip(SearchField) {
   TOOLtooltipLink('Search for channels based on their name or description.', null, SearchField);
}

function DASHhideSearchTooltip()
{
   TOOLtooltipClose();
}

/* SEARCH FILTER END */

/* ERROR DISPLAY START */
function DASHcheckAndDisplayEmailSettings(ServerData)
{
   var EmailSettingsChangedSpan = document.getElementById('spnEmailSettingsChanged');
   var EmailNotSetupSpan        = document.getElementById('spnEmailNotSetup');
   if (ServerData.EmailSettingsChanged)
   {
      EmailSettingsChangedSpan.style.display = '';
      EmailNotSetupSpan.style.display        = 'none';
   }
   else if ((ServerData.EmailNotSetup) && (ServerData.CountOfChannel > 0))
   {
      EmailSettingsChangedSpan.style.display = 'none';
      EmailNotSetupSpan.style.display        = '';
   }
   else
   {
      EmailSettingsChangedSpan.style.display = 'none';
      EmailNotSetupSpan.style.display        = 'none';
   }
}

function DASHcheckAndDisplayLicenseWarnings(ServerData)
{
   if (ServerData.MaintenanceExpired) {
     $('#warningMaintenanceExpired').show();
   }
   if (ServerData.LicenseExpired) {
     $('#warningLicenseExpired').show();
     $('#warningLicenseExpiringSoon').hide();
   } else if (ServerData.LicenseExpiringSoon) {
     $('#warningLicenseExpired').hide();
     $('#warningLicenseExpiringSoon').show();
   }
}

function DASHcheckAndDisplayPanic(ServerData)
{
   if( ServerData.PanicErrorString )
   {
      document.getElementById('spnPanicErrorText').innerHTML = ServerData.PanicErrorString;
      document.getElementById('spnPanicError').style.display = '';
      document.getElementById('lowDiskSpaceMessage').style.display = 'none';
   }
   else
   {
      document.getElementById('spnPanicError').style.display = 'none';
      // TODO: It would be nice if we could update the warning text.
      document.getElementById('lowDiskSpaceMessage').style.display
         = ServerData.WarnDiskSpace ? '' : 'none';
   }
}

function DASHdisplayUpgradeLogFilePopUp(Message)
{
   if (Message)
   {
      $('#upgrade_log_file_popup_msg').html(Message);
      $('#upgrade_log_file_popup').dialog('open');
   }
}

function DASHdisplayUserLoginMessage(LoginMessage)
{
   if (LoginMessage)
   {
      $('#panelUserLoginMessage').show();

      //Note we append a link to the message here to start channels...
      //a bit of a cludge but its the easiest way to do it.
      LoginMessage += '<br/><a onclick="DASHstartAutomaticChannels()" href="#">Start all channels configured to automatically start.</a>';

      $('#divUserLoginMessage').html(LoginMessage);
   }
   else
   {
      $('#panelUserLoginMessage').hide();
      $('#divUserLoginMessage').html('');
   }
}

function DASHsessionParams(ServerData){
   if (ServerData.IsLocal) {
      return '';
   }
   else {
      return 'SessionId=' +encodeURIComponent(ServerData.SessionId) +'&SessionKey=' + encodeURIComponent(ServerData.SessionKey);
   }
}

function DASHhrefPrefix(ServerData){
   if (ServerData.IsLocal) {
      return '';
   }
   else {
      return ServerData.Https + '://' + ServerData.Host + ':' + ServerData.Port
   }
}

function DASHserverId(ServerData){
   if (ServerData.IsLocal) {
      return '';
   }
   else {
      return ServerData.Https + '://' + ServerData.Host + ':' + ServerData.Port
   }
}

function DASHcheckAndDisplayNoChannels(ServerData, TotalChannelCount) {
   JDashTable = $('#divDashboardTable');
   if (TotalChannelCount == 0 && ServerData.ChannelFilterString == '') {
      JDashTable.hide();
      $("#divNoChannels").show();
   } else if (ServerData.HtmlEscapedChannelQueryError != '') {
      JDashTable.hide();
   } else if (TotalChannelCount == 0 && ServerData.ChannelFilterString == '') {
      JDashTable.hide();
      $("#divNoChannels").show();
   } else {
      JDashTable.show();
      $('#divNoChannels').hide();
   }
}

function DASHdisplayNoChannelsMessage(CurrentUserCanAdmin) {
   var NoChannelsDiv = $('#divNoChannels');
   var NoChannelsText = $('#spanNoChannelsText');
   NoChannelsDiv.show();
   var Filling = CurrentUserCanAdmin
                 ? "You don't have any channels yet.<br><br>You can add your first channel with the \"Add Channel\" button below. Or, <a href=\"http://training.interfaceware.com/iguana-overview/\" target=\"_blank\">visit our training center for a 30 minute course/tutorial</a> that will teach you the basics of building interfaces faster with Iguana."
                 : "You don't have any channels yet.<br><br><a href=\"http://training.interfaceware.com/iguana-overview/\" target=\"_blank\">visit our training center for a 30 minute course/tutorial</a> that will teach you the basics of building interfaces faster with Iguana.";
   NoChannelsText.html(Filling);
}

function DASHdisplayNoChannelsToDisplayMessage() {
   var NoChannelsDiv = document.getElementById('divNoChannels');
   var NoChannelsText = document.getElementById('spanNoChannelsText');
   NoChannelsDiv.style.display    = '';
   NoChannelsDiv.style.paddingTop = (document.getElementById('trChannelHeadings').offsetHeight + 10) + 'px';
   NoChannelsText.innerHTML = '';
}

function DASHdisplayError(ErrorText, ErrorDetails)
{
   $('#divAjaxError').show();
   $('#divAjaxErrorDescription').html(ErrorText);
   $('#divAjaxErrorDetails').html(ErrorDetails);
}

function DASHshowErrorDetails()
{
   $('#divAjaxErrorDetails').show();
   $('#divAjaxErrorDetailsShowLink').hide();
   $('#divAjaxErrorDetailsHideLink').show();
   DASHcheckAndDisplayErrorPanel();
}

function DASHhideErrorDetails()
{
   $('#divAjaxErrorDetails').hide();
   $('#divAjaxErrorDetailsShowLink').show();
   $('#divAjaxErrorDetailsHideLink').hide();
   DASHcheckAndDisplayErrorPanel();
}

function DASHclearError() {
   $('#divAjaxError').hide();
   $('#divAjaxErrorDescription').html('');
   $('#divAjaxErrorDetails').html('');
}

/* ERROR DISPLAY END */


/*BULK OPERATIONS START*/

function DASHbulkSaveCookie(){
   //span multiple cookies since this count can be large.
   var BulkObjString = '';
   var IsFirst = true;
   var BlockSize = 50;
   var ThisBlockCount = 0;
   var TotalBlockCount = 0;

   function writeCookie(){
      if (BulkObjString){
         COOKIEcreate(DASHcookieName('Iguana-ChannelBulkAction-' + (TotalBlockCount++)), BulkObjString, 365);
      }
      BulkObjString = '';
      IsFirst = true;
      ThisBlockCount = 0;
   }

   for (BulkKey in DASHbulkObj){
      if (!IsFirst) BulkObjString += '*&/\*';
      BulkObjString += BulkKey + "=" + DASHbulkObj[BulkKey];
      IsFirst = false;

      ThisBlockCount++;
      if (ThisBlockCount >= BlockSize) writeCookie();
   }
   writeCookie();

   COOKIEcreate(DASHcookieName('Iguana-ChannelBulkAction-Size'), TotalBlockCount, 365);
}

function DASHbulkReadCookie(){
   var BlockCountStr = COOKIEread( DASHcookieName('Iguana-ChannelBulkAction-Size') );
   var BlockCount = 0;
   if (BlockCountStr) BlockCount = parseInt(BlockCountStr);

   var Output = {}
   for (BlockIndex = 0; BlockIndex < BlockCount; BlockIndex++){
      var ThisCookie = COOKIEread( DASHcookieName('Iguana-ChannelBulkAction-' + BlockIndex) );
      if (ThisCookie){
         var TempBulkArray = ThisCookie.split('*&/\*');
         for (BulkIndex in  TempBulkArray){
            var Parts = TempBulkArray[BulkIndex].split("=");
            Output[Parts[0]] = Parts[1];
         }
      }
   }
   return Output;
}

DASHbulkObj = DASHbulkReadCookie();

function DASHstartAutomaticChannels()
{
   var makeQueryLocation = function()
   {
      return window.location.protocol + '//' + window.location.host + '/dashboard_action';
   };
   AJAXpost(makeQueryLocation(), "action=start_all_automatic", null, null);
}

var DASHrefreshTimeLeft = 60;
var DASHlastRefreshStatusMsg = '';
var DASHonRefreshWaitingTimer;

function DASHclearOnRefreshWaiting(){
   if (DASHupdateStatusHtml == DASHlastRefreshStatusMsg){
      DASHlastRefreshStatusMsg = '';
      DASHupdateStatusHtml = DASHlastRefreshStatusMsg;
      $('#spnUpdateStatus').html(DASHupdateStatusHtml);
   }
   DASHrefreshTimeLeft = 60;
   if (DASHonRefreshWaitingTimer) clearTimeout(DASHonRefreshWaitingTimer);
}

function DASHstartOnRefreshWaiting(){
   DASHclearOnRefreshWaiting();
   DASHonRefreshWaitingTimer = setTimeout(DASHonRefreshWaiting,15000);
}

function DASHonRefreshWaiting(){
   if (DASHrefreshTimeLeft){
      if (DASHupdateStatusHtml == DASHlastRefreshStatusMsg){
         DASHlastRefreshStatusMsg = 'Waiting for response from server (' + DASHrefreshTimeLeft + ' seconds remaining).';
         DASHupdateStatusHtml = DASHlastRefreshStatusMsg;
         $('#spnUpdateStatus').html(DASHupdateStatusHtml);
      }
      DASHrefreshTimeLeft--;
      DASHonRefreshWaitingTimer = setTimeout(DASHonRefreshWaiting,1000);
   }
   //TODO - we didn't get a response back from the server
}

function DASHselectedServersParam(){
  var ServerSelectList = 'SelectedServers=';
   if (DASHselectedServers){
      //newline delimited
      for (ServerIndex in DASHselectedServers){
         if (DASHselectedServers[ServerIndex]){
            ServerSelectList += encodeURIComponent(ServerIndex + '\n');
         }
      }
   }
   else{
      ServerSelectList += 'all';
   }
   return ServerSelectList;
}

function DASHrefresh(PostParams) {
   var ChannelFilterString = DASHgetFilterStringFromField();
   var ChannelListOptions = '&MinChannelRowIndex=' + encodeURIComponent(DASHmodelQueryOptions.ChannelList.MinChannelRowIndex)
      + '&' + 'MaxChannelRowIndex=' + encodeURIComponent(DASHmodelQueryOptions.ChannelList.MaxChannelRowIndex)
      + '&' + 'SortOrder=' + (DASHmodelQueryOptions.ChannelList.Ascending ? 'Asc' : 'Desc')
      + '&' + 'SortColumn=' + encodeURIComponent(DASHmodelQueryOptions.ChannelList.SortName);
   PostParams = 'include_machine_info=true&include_remote_servers=true&ChannelFilterString=' + encodeURIComponent(ChannelFilterString)
                + ChannelListOptions
                + '&' + DASHselectedServersParam()
                + (PostParams == '' ? '' : '&' + PostParams);
   DASHstartOnRefreshWaiting();
   AJAXpost(window.location.protocol + '//' + window.location.host + '/dashboard_data', PostParams, DASHonDashboardData, DASHonAjaxError);
}

function DASHfetchAllChannelData(onComplete){
   //this is a call that returns the channels
   //using the search string, but not limiting by window or bothering to sort it
   //note that this will probably touch the session, delaying timeout
   var ChannelFilterString = DASHgetFilterStringFromField();
   PostParams = 'include_remote_servers=true&ChannelFilterString=' + encodeURIComponent(ChannelFilterString)
                + '&' +DASHselectedServersParam();
   AJAXpost(window.location.protocol + '//' + window.location.host + '/dashboard_data',
            PostParams,
            function(ResponseText, ResponseType, Request)
            {
               try{
                  onComplete(DASHparseJson(ResponseText));
               }
               catch(e){} //don't care
            },
            null);
}

function DASHbulkSelectAllChannels()
{
   var SelectMessage = 'Selecting channels...';
   if (document.getElementById('bulkSelectAll').checked)
   {
      function handleBulkSelectAll(AllChannelData){
         if (DASHupdateStatusHtml == SelectMessage) DASHupdateStatusHtml = '';
         $('#spnUpdateStatus').html(DASHupdateStatusHtml);

         DASHbulkObj = {};
         $('input[name=inputBulk]').each(function() {
               this.checked = (!this.disabled ? 'checked' : '');
            });

         if (AllChannelData && AllChannelData.Servers && AllChannelData.Servers.length && AllChannelData.Channels)
         {
            for( ChannelIndex = 0; ChannelIndex < AllChannelData.Channels.length; ChannelIndex++){
               var ChannelData = AllChannelData.Channels[ChannelIndex];
               var ServerData = AllChannelData.Servers[ChannelData.ServerIndex];
               var InputValue = DASHbulkInputValue(ServerData,ChannelData.Channel);
               if (InputValue && !DASHbulkInputDisabled(ChannelData.Channel)){
                  DASHbulkObj[InputValue] = ChannelData.Channel.Name;
               }
            }
         }
         DASHbulkSaveCookie();
      }

      //Since the data from the usual refresh is windowed, we have to do
      //a fetch for all channel data to fill the bulk array
      if (DASHupdateStatusHtml == '') DASHupdateStatusHtml = SelectMessage;
      $('#spnUpdateStatus').html(DASHupdateStatusHtml);
      DASHfetchAllChannelData(handleBulkSelectAll);
   }
   else
   {
      DASHbulkObj = {};
      $('input[name=inputBulk]').each(function() {
            this.checked = '';
         });
      DASHbulkSaveCookie();
   }
}

function DASHsetupBulkActionSelectList() {
   if ( $("#divDashboardTable input[type=checkbox]:checked").length == 0 ) {
      $("#bulkDropDown").prop("disabled", true);
   } else {
      $("#bulkDropDown").prop("disabled", false);
   }
}

function DASHbulkDoAction(action) {
   var ajaxString;
   var BulkKeyCount = 0;

   if (action.value == '0') {
      return;
   }

   var ChannelsMap = {};
   var ActionValue = action.value;
   var ChannelGuidsArray = [];
   var ChannelGuidsNoPermissionArray = [];
   var AllChannelData;

   for (BulkKey in DASHbulkObj){
      BulkKeyCount++;
   }

   var makeQueryLocation = function() {
      return window.location.protocol + '//' + window.location.host + '/dashboard_action';
   };

   var makeQueryString = function(ChannelGuidsArray, action, password) {
      var ajaxString = 'action=' + action;
      ajaxString += '&password=' + encodeURIComponent(password);


      for (x=0;x<ChannelGuidsArray.length;x++ ) {
         ajaxString += '&Channel_' + x + '=' + encodeURIComponent(ChannelGuidsArray[x]);
      }

      return ajaxString;
   };

   var makeQueryJSON = function(ChannelGuidsArray, action, password) {
      console.log("makeQueryJSON");
      var Params = {};
      Params["action"]   = action;
      Params["password"] = password;

      for (x=0;x<ChannelGuidsArray.length;x++ ) {
         var ChannelKey = "Channel_" + x;
         Params[ChannelKey] = ChannelGuidsArray[x];
      }
      return Params;
   };

   //action -> english
   ActionToEnglishMap = {
      start:  'be started',
      stop:   'be stopped',
      delete: 'be deleted',
      clearChannelErrors: 'have their errors cleared',
      clearChannelQueues: 'have their queues cleared',
      exportChannels: 'be exported'
   }

   function bulkActionComplete(JDialog){
      JDialog.dialog('destroy');
      action.selectedIndex = 0;
      action.blur();
      DASHrefresh('');
      setTimeout('DASHrefresh("")', 3000);
      DASHbulkObj = DASHbulkReadCookie();
   }

   function createRemoteGuidToNameMap(ChannelGuidsArray){
      console.log("createRemoteGuidToNameMap");
      console.log(ChannelsMap);
      var GuidNameMap = {};
      for(var i = 0; i < ChannelGuidsArray.length; i++){
         //If local, won't have appended server at end of string.
         var ChannelGuidAndRemoteServer = ChannelGuidsArray[i]; 
         var ChannelGuid = ChannelsMap[ChannelGuidAndRemoteServer].Channel.Guid;
         var IsRemote    = (ChannelGuid !== ChannelGuidAndRemoteServer);
         if(IsRemote){
            var ChannelName = ChannelsMap[ChannelGuidAndRemoteServer].Channel.Name;
            GuidNameMap[ChannelGuid] = ChannelName;
         }
         // If the ChannelGuid != ChannelGuidAndRemoteServer, then it isn't just a local guid 
         // and indeed has a @http://domain.tld appended at the end. Which means it is a remote channel.
      }
      return GuidNameMap;
   }

   function isDeleteSuccessMessage(Message){
      var Match = Message.match(/Channel \"(.+)\" removed successfully./);  
      return Match && Match.length > 0
   }

   function onBulkDeleteSuccess(ResponseJSON){
      console.log(ResponseJSON);
      var Message            = "";
      var ShowStatusMessages = true;
      
      if( ! ResponseJSON.success ){
         Message = "<p>None of the selected channels could be removed:<ul>";
      }
      else if( ResponseJSON.partial_success ){
         Message = "<p>Not all of the selected channels could be removed:<ul>";
      }
      else{
         Message = "<p>The selected channels were successfully removed!";
         ShowStatusMessages = false;
      }

      if(ShowStatusMessages){
         var DeleteStatusMessages = ResponseJSON.status_messages || [];
         var SuccessMessages = [];
         for(var i = 0; i < DeleteStatusMessages.length; i++){
            if( isDeleteSuccessMessage(DeleteStatusMessages[i]) ){
               SuccessMessages.push(DeleteStatusMessages[i]);
               continue;
            }
            Message += "<li>" + DeleteStatusMessages[i] + "</li>";
         }
         Message += "<br>";
         for(var i = 0; i < SuccessMessages.length; i++){
            // Want to group all the successes together at the bottom.
            Message += "<li>" + SuccessMessages[i] + "</li>";  
         }
         Message += "</ul>";
      }
      Message+= "</p>";
      
      $('#bulk_confirm_msg').html(Message);
      $('#bulk_confirm_dialog_form').dialog('option','buttons', {
         OK: function() {
            bulkActionComplete($(this));
         },
      });
   }

   function onBulkDeleteError(RequestObject, Status, ErrorString){
      console.log(RequestObject);
      console.log(Status);
      console.log(ErrorString);
      if(Status == "abort"){ return; } //if we abort it isn't an error.
      
      var ErrorMessage = "";
      if(RequestObject.responseJSON && RequestObject.responseJSON.remotes_selected){
         ErrorMessage += "<p>The selected channels were not deleted due to them being located on a remote server.</p>";
      }
      else if(RequestObject.responseJSON){
         var ErrorDescription = RequestObject.responseJSON.error
            ? RequestObject.responseJSON.error.description
            : RequestObject.status + " - " + RequestObject.statusText;   
         ErrorMessage = "An error occurred: <p>" + ErrorDescription + "</p>";
      }
      else if(RequestObject.responseText){
         ErrorMessage = "An error occurred: <p>" + RequestObject.responseText + "</p>";
      }
      if(!ErrorMessage){
         ErrorMessage = "An unexpected error occurred.";
      }
      $('#bulk_confirm_msg').html(ErrorMessage);
      $('#bulk_confirm_dialog_form').dialog('option','buttons', {
         OK: function() {
            bulkActionComplete($(this));
         },
      });
   }

   function checkPermissionsAndGo(AllChannelData) {
      if ("clearChannelQueues" == ActionValue) {
         $('#clearQueuesForm').show();
      }

      //map from inputvalue -> channel
      if (AllChannelData && AllChannelData.Servers && AllChannelData.Servers.length && AllChannelData.Channels) {
         for (ChannelIndex = 0; ChannelIndex < AllChannelData.Channels.length; ChannelIndex++) {
            var ChannelData = AllChannelData.Channels[ChannelIndex];
            var ServerData = AllChannelData.Servers[ChannelData.ServerIndex];
            var InputValue = DASHbulkInputValue(ServerData,ChannelData.Channel);

            if (InputValue) {
               ChannelsMap[InputValue] = ChannelData;
            }
         }
      }
      
      var ExportableChannels = {};
      var NeededPermission   = '';
      
      if ('start' == ActionValue || 'stop' == ActionValue) {
         NeededPermission = 'CanStartStop';
      } 
      else if ('clearChannelErrors' == ActionValue || 'clearChannelQueues' == ActionValue) {
         NeededPermission = 'CanReconfigure';
      } 
      else if ('exportChannels' == ActionValue) {
         $.ajax({
            url: "/sc/view_local_channels",
            async: false,
            data: { for_export: true },
            success: function (data) {
               for (var k in data.channel_list) {
                  ExportableChannels[data.channel_list[k].guid] = true;
               }
            }
         });
      }

      //we clear out stale items so that they don't matter,
      //this can happen when sharing iguana config files around
      var StaleBulkItems = [];

      for (SelectedChannel in DASHbulkObj) {
         var ChannelData = ChannelsMap[SelectedChannel];

         if (ChannelData) {
            // Need to be admin to delete, will verify on server. Option won't even be in dropdown if not admin anyways.
            if (ChannelData.Channel[NeededPermission] === true || ExportableChannels[SelectedChannel] || ActionValue == "delete"){
               ChannelGuidsArray.push(SelectedChannel);
            } 
            else {
               ChannelGuidsNoPermissionArray.push(SelectedChannel);
            }
         } 
         else {
            StaleBulkItems.push(SelectedChannel);
         }
      }

      for (StaleItem in StaleBulkItems) {
         delete DASHbulkObj[StaleBulkItems[StaleItem]];
      }

      if (StaleBulkItems.length) {
         DASHbulkSaveCookie();
      }

      function makeChannelList(InputArray, JParent){
         //sort by servers
         var JOutput = $('<div style="display: none; overflow: auto;"><hr/></div>');
         JParent.append(JOutput);
         var ServerToChannels = {};

         for(i = 0; i < InputArray.length; ++i) {
            var ThisChannel = ChannelsMap[InputArray[i]];
            var ThisServer = AllChannelData.Servers[ThisChannel.ServerIndex];

            if (!ServerToChannels[ThisServer.ServerLabel]) {
               ServerToChannels[ThisServer.ServerLabel] = [];
            }

            ServerToChannels[ThisServer.ServerLabel].push(ThisChannel.Channel.Name);
         }

         for (ServerIndex in ServerToChannels) {
            var ChannelNames = ServerToChannels[ServerIndex];
            var JBulkChannelList = $('<div style=" " class="clsBulkChannelList"></div>');

            for (ChannelIndex in ChannelNames){
               JBulkChannelList.append( ChannelNames[ChannelIndex] ).append('<br>');
            }

            if (AllChannelData.Servers.length > 1) {
               var JServerNode = $('<div></div>');
               var JServerTitle = $('<div style="cursor: pointer;"></div>');
               JServerTitle.append('<span class="clsPlusMinus">+</span>')
                  .append('<span style="text-decoration: underline;"> ' + ChannelNames.length + ' channel' + (ChannelNames.length == 1 ? '' : 's') + ' on ' + ServerIndex + '</span>')
                  .click(function() {
                            $('.clsBulkChannelList',$(this).parent()).toggle();
                            var plus_minus = $('.clsPlusMinus',$(this)).html();
                            plus_minus = (plus_minus == '+' ? '&ndash;' : '+');
                            $('.clsPlusMinus',$(this)).html(plus_minus);
                         });
               JBulkChannelList.css({'margin-left': '20px', 'margin-bottom' : '10px', 'display' : 'none' });
               JServerNode.append(JServerTitle).append(JBulkChannelList);
               JOutput.append(JServerNode);
            } else {
               //single server case, don't bother with the drop down
               JOutput.append(JBulkChannelList);
            }
         }

         return JOutput;
      }

      function makeConfirmBlock(ConfirmMsg, MsgId, InputArray, IsExport){
         if (InputArray.length || IsExport) {
            var JMsg = $('<div>' + ConfirmMsg + ' <span class="clsBulkDetails" style="cursor: pointer; text-decoration: underline;">(show details)</span></div>');
            var JChannelList = makeChannelList(InputArray, $('#' + MsgId).append(JMsg));

            $('.clsBulkDetails', JMsg).click(function(){
               JChannelList.toggle();
               var DetailsHtml = $(this).html();
               DetailsHtml = (DetailsHtml == '(show details)' ? '(hide details)' : '(show details)');
               $(this).html(DetailsHtml);
            });
         }
      }

      $('#bulk_confirm_dialog_form').dialog('option','buttons', {
         Cancel: function() {
            bulkActionComplete($(this));
         },
         Submit: SubmitAction
      });

      if ('exportChannels' == ActionValue) {
         var IsExport = true;
         var Msg = " because the channel has no commits or is remote ";
      } else {
         var Msg = " because you do not have the necessary permissions ";
      }

      var ConfirmMsg = '<span>' + ChannelGuidsArray.length + ' channel' + (ChannelGuidsArray.length == 1 ? '' : 's') + ' will ' + ActionToEnglishMap[ActionValue] + '</span>';
      var PermissionMsg = '<span>' + ChannelGuidsNoPermissionArray.length
                        + ' channel' + (ChannelGuidsNoPermissionArray.length == 1 ? '' : 's') + ' will not '
                        +  ActionToEnglishMap[ActionValue] + Msg + '</span>';

      var BulkConfirmMsgId = 'bulk_confirm_msg';

      $('#' + BulkConfirmMsgId).empty();

      makeConfirmBlock(ConfirmMsg, BulkConfirmMsgId, ChannelGuidsArray, IsExport);

      if (ChannelGuidsNoPermissionArray.length) {
         $('#' + BulkConfirmMsgId).append('<br/>');
         makeConfirmBlock(PermissionMsg, BulkConfirmMsgId, ChannelGuidsNoPermissionArray);
      }
   } // end of nested function checkPermissionsAndGo

   // DASHbulkDoAction code actually starts here.

   if ( ! BulkKeyCount) {
      return;
   }

   var SubmitAction = function() {
      AJAXpost(makeQueryLocation(), makeQueryString(ChannelGuidsArray, ActionValue, ''), null, null)
      bulkActionComplete($(this));
   };

   $('#clearQueuesForm').hide();

   if ("clearChannelQueues" == ActionValue) {
      SubmitAction = function() {
         AJAXsynchronousPost(
            makeQueryLocation(),
            makeQueryString(ChannelGuidsArray, ActionValue, $('#ClearQueuesPassword').val()),
            function(ResponseText, ResponseType, Request) {
               if (ResponseText) {
                  $('#bulk_confirm_error').html(ResponseText);
               } else {
                  $('#bulk_confirm_error').html('');
                  bulkActionComplete($('#bulk_confirm_dialog_form'));
               }
            },
            null
         );

         $('#ClearQueuesPassword').val('');//always clear password
      }
   }
   else if ("delete" == ActionValue) {
      SubmitAction = function() {
         var Params = makeQueryJSON(ChannelGuidsArray, ActionValue, $('#ClearQueuesPassword').val());
         Params["remote_guid_name_map"] = JSON.stringify( createRemoteGuidToNameMap(ChannelGuidsArray) );
         // The bulk_confirm_msg is inside of the the bulk_confirm_dialog_form.
         $('#bulk_confirm_msg').html('Deleting Channels...<img class="DASHspinner" src="/js/mapper/images/spinner.gif" />');
         $('#bulk_confirm_dialog_form').dialog('option','buttons', {});
         $.ajax({
            url    : "/dashboard_action",
            method : "POST",
            data   : Params,
            success: onBulkDeleteSuccess,
            error  : onBulkDeleteError
         });
      }
   } 
   else if ("exportChannels" === ActionValue) {
      SubmitAction = function() {
         var ChannelsToExport = [];

         $("#divDashboardTable input[type=checkbox]:checked").each(function(inx,elm) {
            ChannelsToExport.push(elm.value);
         });

         var H = '<form method="POST" action="/settings#Page=export">';
         for (var i = 0; i < ChannelsToExport.length; i++) {
            H += '   <input name="guid_' + i + '" type="hidden" value="' + ChannelsToExport[i] + '"/>';
         }

         H += '<input name="num_channels" type="hidden" value="' + ChannelsToExport.length +'"/>';
         H += '<input name="redirect_to" type="hidden" value="export"/>';
         H += '</form>';

         var TheForm = $(H).appendTo('body');
         TheForm.submit();
      }
   }

   // note: if the submit button name is changed, be sure to go update
   // the code in initialize() that handle the enter key event
   $('#bulk_confirm_dialog_form').dialog({
      bgiframe: true,
      width:  500,
      modal: true,
      buttons: {
         cancel: function() {
            bulkActionComplete($(this));
         }
      },
      close: function(ev,ui) {
         bulkActionComplete($(this));
      }
   }).dialog('open');

   if ($.isEmptyObject(DASHbulkObj)) {
      $("#bulk_confirm_dialog_form").dialog("option", "buttons", {Cancel: function(){ bulkActionComplete($(this));}})
         .find('#bulk_confirm_msg').html('There are no exportable channels.');
   } 
   else {
      $('#bulk_confirm_msg').html('Checking permissions....');
      //need to get all data to figure out permissions
      DASHfetchAllChannelData(checkPermissionsAndGo);
   }
}

function DASHbulkObjUpdate(item) {
   var $item = $(item);

   if ($item.prop('checked') === true) {
      DASHbulkObj[$item.val()] = $item.data('channel_name');
   } else {
      delete DASHbulkObj[$item.val()];
   }

   DASHbulkSaveCookie();
}

function DASHbulkStatus()
{
   BulkStatus = document.getElementById('bulkStatus');
   if (BulkStatus && DASHmodel)
   {
      var UpdateCount = 0;
      var ErrorsBusy = false;
      for (ServerIndex in DASHmodel.Servers){
         var ServerData = DASHmodel.Servers[ServerIndex].DashboardData;
         if (ServerData){
            if (ServerData.CountOfPendingBulkAction > 0){
               UpdateCount += ServerData.CountOfPendingBulkAction;
            }
            ErrorsBusy = (ErrorsBusy ? ErrorsBusy : ServerData.ClearChannelErrorsBusy);
         }
      }
      if (UpdateCount > 0)
      {
         BulkStatus.innerHTML = 'Updating Channels: ' + UpdateCount + ' remaining...';
      }
      else
      {
         BulkStatus.innerHTML = (ErrorsBusy ? 'Busy Clearing Errors...' : '');
      }
   }
}
/*BULK OPERATIONS END */

function DASHelementSetDisplay(Element,DisplayVal) {
   //this avoids redrawing if the value hasn't changed
   if (Element) {
      if (!DisplayVal && Element.style.display == '') {
         Element.style.display = 'none';
      }
      else if (DisplayVal && Element.style.display == 'none') {
         Element.style.display = '';
      }
   }
}

function DASHpopulateComponentIcon(TabType, ComponentData, ChannelData, ComponentIcon, ServerData)
{
   var CurrentToolTipComponent = DASHserverId(ServerData) + '.' + ChannelData.Index + '.' + ComponentData.Type;

   var StatusSuffix = '';
   if(ComponentData.HasErrors){
    StatusSuffix = '_error';
   }
   else if(ComponentData.HasWarnings){
    StatusSuffix = '_warning';
   }
   var SessionData = ServerData.SessionParams
                     ? ServerData.SessionParams
                     : '';

   if (ComponentIcon.src != DASHnormalizeHref(DASHskinningCookieData + '/images/icon_' + ComponentData.ShortName + StatusSuffix + '.gif'))
   {
      ComponentIcon.src = DASHskinningCookieData + '/images/icon_' + ComponentData.ShortName + (DASHtooltipComponent == CurrentToolTipComponent ? '_hover' : '') + StatusSuffix + '.gif';
      ComponentIcon.alt = ComponentData.ShortName;
   }
   var NewLinkSrc = DASHgoToServerLink(ServerData, DASHchannelUriPrefix(ServerData, SessionData) + 'Channel=' + encodeURIComponent(ChannelData.Name) + '&Tab=' + TabType);
   if (ComponentIcon.parentNode.href != NewLinkSrc){
      ComponentIcon.parentNode.href = NewLinkSrc;
   }

   function makeChannelComponent(ToolTipData){
      //TODO - this needs to be someplace else, also used in channel.cs
      //note we take into account global dash data

      var ThisHost;
      if (ServerData.IsLocal) ThisHost = window.location.hostname;
      else ThisHost = ServerData.Host;

      ToolTipData.SourceTooltip = ToolTipData.SourceTooltip.replace(/%%IFWARE_WINDOW_LOCATION_HOSTNAME%%/g,ThisHost);
      return (TabType == 'source' ? ToolTipData.SourceTooltip : ToolTipData.DestinationTooltip);
   }

   ComponentIcon.onmouseover = function(){
      ToolTipIndex = ChannelData.Index;
      DASHchannelStatusToolTip(ChannelData,ServerData,this,CurrentToolTipComponent, makeChannelComponent,
                               function(){
                                  ComponentIcon.src = DASHskinningCookieData + '/images/icon_' + ComponentIcon.alt + '_hover' + StatusSuffix + '.gif';
                                  ToolTipType = (TabType == 'source' ? 'Source' : 'Destination');
                               });
   };

   ComponentIcon.onmouseout = function()
   {
      ToolTipIndex = "";
      ToolTipType = "";
      TOOLtooltipCloseIfNotHover();
      this.src = DASHskinningCookieData + '/images/icon_' + ComponentData.ShortName + StatusSuffix + '.gif';
      DASHtooltipComponent = '';
   };

   ComponentIcon.onmouseup = ComponentIcon.onmouseout;
}

function DASHpopulateFilterIcon(ChannelData, FilterIcon, ServerData)
{
   var SessionData = ServerData.SessionParams
                     ? ServerData.SessionParams
                     : '';
   var FilterImgSrc = '';

   var UsingFilter = false;
   if (ChannelData.UseMessageFilter == 1 && !ChannelData.Filter.IsEmpty) {
      UsingFilter = true;
   }

   var CurrentToolTipComponent = DASHserverId(ServerData) + '.' + ChannelData.Index + '.Filter';
   FilterIcon.alt = (UsingFilter ? 'arrow_filter' : 'arrow');
   if (DASHtooltipComponent == CurrentToolTipComponent){
    var FilterHoverImg = "";
    if(ChannelData.Filter.HasErrors){
     FilterHoverImg = DASHarrowFilterHoverErrorImgSrc;
    }
    else if(ChannelData.Filter.HasWarnings){
     FilterHoverImg = DASHarrowFilterHoverWarningImgSrc;
    }
    else{
     FilterHoverImg = DASHarrowFilterHoverImgSrc;
    }

    FilterImgSrc = (UsingFilter ? FilterHoverImg : DASHarrowHoverImgSrc);
   }
   else{
    var FilterImg = "";
    if(ChannelData.Filter.HasErrors){
     FilterImg = DASHarrowFilterErrorImgSrc;
    }
    else if(ChannelData.Filter.HasWarnings){
     FilterImg = DASHarrowFilterWarningImgSrc;
    }
    else{
     FilterImg = DASHarrowFilterImgSrc;
    }

    FilterImgSrc = (UsingFilter ? FilterImg  : DASHarrowImgSrc);
   }

   if (FilterIcon.src != DASHnormalizeHref(FilterImgSrc))
   {
      FilterIcon.parentNode.href = DASHgoToServerLink(ServerData, DASHchannelUriPrefix(ServerData, SessionData) + 'Channel=' + encodeURIComponent(ChannelData.Name)  + '&Tab=filter');
      FilterIcon.src = FilterImgSrc;
   }

   function makeFilterText(ToolTipData){
      return ToolTipData.FilterTooltip;
   }

   var StatusSuffix = '';
   if(ChannelData.Filter.HasErrors){
    StatusSuffix = '_error';
   }
   else if(ChannelData.Filter.HasWarnings){
    StatusSuffix = '_warning';
   }

   FilterIcon.onmouseover = function(){
      ToolTipIndex = ChannelData.Index;
      ToolTipType = 'Filter';
      DASHchannelStatusToolTip(ChannelData,ServerData,this,CurrentToolTipComponent, makeFilterText,
                               function(){
                                  FilterIcon.src = DASHskinningCookieData + '/images/' + FilterIcon.alt + '_hover' + StatusSuffix + '.gif';
                               });
   };

   FilterIcon.onmouseout = function()
   {
      ToolTipIndex = "";
      ToolTipType = "";
      TOOLtooltipCloseIfNotHover();
      this.src = DASHskinningCookieData + '/images/' + this.alt + StatusSuffix + '.gif';
      DASHtooltipComponent = '';
   };

   FilterIcon.onmouseup = FilterIcon.onmouseout;
}

function DASHpopulateTypeIcons(ChannelData, Cell, ChannelIndex, ServerData)
{
   var JImg = $('img',Cell);
   var JNewNode;
   if (JImg.size() == 0) {
     JNewNode = $( '<nobr><a><img alt="" width="38" height="16" border="0" title=""></a> <a><img alt="" width="11" height="16" border="0" title=""></a> <a><img alt="" width="38" height="16" border="0" title=""></a></nobr>'  );
     JImg = $('img', JNewNode);
   }

   DASHpopulateComponentIcon('source', ChannelData.Source, ChannelData, JImg.get(0), ServerData);
   DASHpopulateFilterIcon(ChannelData, JImg.get(1), ServerData);
   DASHpopulateComponentIcon('destination', ChannelData.Destination, ChannelData, JImg.get(2), ServerData);

   //this really speeds up the initial load, since it's not part of the dom until final appending
   if (JNewNode) {
      $(Cell).empty().append(JNewNode);
   }
}

function DASHpopulateStatus(ChannelData, Cell, ChannelIndex, ServerData)
{
   if ($('img',Cell).size() == 0) {
     $(Cell).empty().append('<img width="16" height="16">');
   }
   var StatusImage = $('img',Cell).get(0);

   // 0 == Red, 1 == Yellow, 2 == Green, 3 == Off
   if (ChannelData.Light == 0) // Red
   {
      if (StatusImage.src != DASHnormalizeHref(DASHskinningCookieData + '/images/button-dotredv4.gif'))
      {
         StatusImage.src = DASHskinningCookieData + '/images/button-dotredv4.gif';
      }
   }
   else if (ChannelData.Light == 1) // Yellow
   {
      if (StatusImage.src != DASHnormalizeHref(DASHskinningCookieData + '/images/button-dotyellowv4.gif'))
      {
         StatusImage.src = DASHskinningCookieData + '/images/button-dotyellowv4.gif';
      }
   }
   else if (ChannelData.Light == 2) // Green
   {
      if (StatusImage.src != DASHnormalizeHref(DASHskinningCookieData + '/images/button-dotgreenv4.gif'))
      {
         StatusImage.src = DASHskinningCookieData + '/images/button-dotgreenv4.gif';
      }
   }
   else if (ChannelData.Light == 3) // Off
   {
      if (StatusImage.src != DASHnormalizeHref(DASHskinningCookieData + '/images/button-dotgrayv4.gif'))
      {
         StatusImage.src = DASHskinningCookieData + '/images/button-dotgrayv4.gif';
      }
   }

   function makeStatusText(ToolTipData){
      return ToolTipData.LiveStatus;
   }

   var CurrentToolTipComponent = DASHserverId(ServerData) + '.' + ChannelData.Index + '.Status';

   StatusImage.onmouseover = function(){
      ToolTipIndex = ChannelData.Index;
      ToolTipType = 'Filter';
      DASHchannelStatusToolTip(ChannelData,ServerData,this,CurrentToolTipComponent, makeStatusText,
                               null);
   };

   StatusImage.onmouseout = function()
   {
      ToolTipIndex = "";
      ToolTipType = "";
      TOOLtooltipClose();
      DASHtooltipComponent = '';
   };
   StatusImage.onmouseup = StatusImage.onmouseout;
   return StatusImage;
}

function DASHpreloadImages()
{
   // This preloads the start and stop hover and active images.  This is done to prevent
   // a delay before seeing the images when the user hovers over or clicks on the buttons.
   if (document.images)
   {
      StartImageHover = new Image(75, 16);
      StartImageHover.src = DASHstartImageHover;
      StartImageActive = new Image(75, 16);
      StartImageActive.src = DASHstartImageActive;

      StopImageHover = new Image(75, 16);
      StopImageHover.src = DASHstopImageHover;
      StopImageActive = new Image(75, 16);
      StopImageActive.src = DASHstopImageActive;

      (new Image(11,16)).src = DASHarrowFilterImgSrc;
      (new Image(11,16)).src = DASHarrowFilterErrorImgSrc;
      (new Image(11,16)).src = DASHarrowFilterWarningImgSrc;
      (new Image(11,16)).src = DASHarrowImgSrc;
   }
}
DASHpreloadImages();

var DASH_a;
function DASHnormalizeHref(href)
{
   if (!DASH_a) { DASH_a = document.createElement('a'); }
   DASH_a.href = href;
   return DASH_a.href;
}

function DASHbulkInputDisabled(ChannelData){
   var CanStartStop = ChannelData.CanStartStop;
   var CanReconfigure = ChannelData.CanReconfigure;
   return !(CanStartStop || CanReconfigure);
}

function DASHbulkInputValue(ServerData, ChannelData){
   if (ChannelData.Guid){
      return ServerData.IsLocal ? ChannelData.Guid : ChannelData.Guid + '@' + DASHserverId(ServerData);
   }
   return '';
}

function DASHcreateBulk(ChannelData, Cell, ChannelIndex, ServerData)
{
   var InputValue = DASHbulkInputValue(ServerData, ChannelData);
   var CanStartStop = ChannelData.CanStartStop;
   var CanReconfigure = ChannelData.CanReconfigure;
   var InputChecked = (DASHbulkObj[ InputValue ] !== undefined);

   var JInputBox = $('input',Cell);
   var JNewNode;
   if (JInputBox.size() == 0) {
      JNewNode = $('<input name="inputBulk" type="checkbox" data-channel_name="' + ChannelData.EscName + '" style="border:none; background:none;" onClick="DASHbulkObjUpdate(this);">');
      JInputBox = JNewNode;
   }

   JInputBox
      .attr('value', InputValue)
      .toggleClass( 'canStartStop', CanStartStop )
      .toggleClass( 'canReconfigure', CanReconfigure )
      .prop('disabled', DASHbulkInputDisabled(ChannelData))
      .prop('checked', InputChecked);

   if (JNewNode) {
      $(Cell).empty().append(JNewNode);
   }
}

function DASHpopulateButton(ChannelData, StartStopImage, ButtonImage, ButtonImageHover, ButtonImageActive, StartStopFunction, ServerData, ToolTipFunc, ToolTipFuncOut)
{
   var CurrentComponentId = ServerData.HrefPrefix + ChannelData.Index;
   if ((DASHstartStopState == DASHstartStopStateHover) && (DASHstartStopComponent == CurrentComponentId))
   {
      if (StartStopImage.src != DASHnormalizeHref(ButtonImageHover))
      {
         StartStopImage.src = ButtonImageHover;
      }
   }
   else if ((DASHstartStopState == DASHstartStopStateActive) && (DASHstartStopComponent == CurrentComponentId))
   {
      if (StartStopImage.src != DASHnormalizeHref(ButtonImageActive))
      {
         StartStopImage.src = ButtonImageActive;
      }
   }
   else
   {
      if (StartStopImage.src != DASHnormalizeHref(ButtonImage))
      {
          StartStopImage.src = ButtonImage;
      }
   }

   StartStopImage.onmouseover = function()
   {
      this.src = ButtonImageHover;
      DASHstartStopState = DASHstartStopStateHover;
      DASHstartStopComponent = ServerData.HrefPrefix + ChannelData.Index;
      ToolTipFunc(this);
      return false;
   };

   StartStopImage.onmouseout = function()
   {
      this.src = ButtonImage;
      DASHstartStopState = DASHstartStopStateNone;
      DASHstartStopComponent = null;
      ToolTipFuncOut();
      return false;
   };

   StartStopImage.onmousedown = function()
   {
     if(ChannelData.CanStartStop)
     {
      this.src = ButtonImageActive;
      DASHstartStopState = DASHstartStopStateActive;
        DASHstartStopComponent = ServerData.HrefPrefix + ChannelData.Index;
      return false;
     }
   };

   StartStopImage.onmouseup = function()
   {
     if(ChannelData.CanStartStop)
     {
      this.src = ButtonImageHover;
      DASHstartStopState = DASHstartStopStateHover;
        DASHstartStopComponent = ServerData.HrefPrefix + ChannelData.Index;
      StartStopFunction();
      return false;
     }
   };
}

function DASHpopulateStartStopButton(ChannelData, Cell, ChannelIndex, ServerData)
{
   var JImage = $('img', Cell);
   var JNewNode;
   if (JImage.size() == 0) {
      JNewNode = $('<img width="75" height=16>');
      JImage = JNewNode;
   }
   var StartStopImage = JImage.get(0);

   var ButtonImages = { 'stop' : [ DASHstopImage, DASHstopImageHover, DASHstopImageActive  ],
                        'start' : [  DASHstartImage, DASHstartImageHover, DASHstartImageActive ],
                        'stop_disabled' : [DASHstopImageDisabled, DASHstopImageDisabled, DASHstopImageDisabled],
                        'start_disabled' : [DASHstartImageDisabled, DASHstartImageDisabled, DASHstartImageDisabled] };

   var ToolTipActionTxt = (ChannelData.IsRunning ? 'stop' : 'start');
   StartStopImage.alt = (ChannelData.IsRunning ? 'Stop' : 'Start');
   var ClickAction = (ChannelData.IsRunning ? function() { DASHstopChannel(ChannelData.Name, ServerData) } : function(){DASHstartChannel(ChannelData.Name, ServerData)} );

   var ButtonImageKey = 'start';
   if (ChannelData.IsRunning) {
      ButtonImageKey = 'stop';
   }
   var ToolTipFunc = function(){};
   var ToolTipFuncOut = function(){};

   if (!ChannelData.CanStartStop) {
      ButtonImageKey += '_disabled';
      ClickAction = function(){};
      ToolTipFunc = function(Img){ TOOLtooltipLink('You do not have the necessary permissions to ' + ToolTipActionTxt + ' this channel.', null, Img) };
      ToolTipFuncOut = TOOLtooltipClose;
   }

   var ThisImgs = ButtonImages[ButtonImageKey];
   DASHpopulateButton( ChannelData, StartStopImage, ThisImgs[0],ThisImgs[1],ThisImgs[2], ClickAction, ServerData, ToolTipFunc, ToolTipFuncOut );

   if (JNewNode) {
      $(Cell).empty().append( JNewNode  );
   }
}

function DASHgenerateNameTooltipHtml(ChannelData)
{
      if (ChannelData.HtmlDescription.length == 0)
      {
         ChannelData.HtmlDescription = "<i>No description</i>";
      }

      if (ChannelData.GroupList.length == 0)
      {
         ChannelData.GroupList = "<i>None</i>";
      }

      if (ChannelData.HtmlDescription.length > 0 || ChannelData.GroupList.length > 0)
      {
         var string = '<table class="ComponentToolTip"><tr><th colspan ="2" class="ToolTipHeading">Channel Information</th></tr>';
         if(ChannelData.HtmlDescription.length > 0)
         {
            string = string + "<tr><th>Description</th><td>" + ChannelData.HtmlDescription;
         }
         if(ChannelData.GroupList.length > 0)
         {
            string = string + "<tr><th>Groups</th><td>" + ChannelData.GroupList + '</td></tr>';
         }

         return string + '</table>';
      }
}

function DASHgenerateEncryptionTooltipHtml(ChannelData) {
   if (ChannelData.IsEncrypted && ChannelData.ExtensionsKeyType.length > 0) {
      var string = '<table class="ComponentToolTip"><tr><th colspan ="2" class="ToolTipHeading">Encrypted</th></tr>';
      string = string + "<tr><th>Required Key</th><td>" + ChannelData.ExtensionsKeyType + "</td></tr>";
      string = string + "</table>";
      return string;
   }
}

function DASHpopulateChannelName(ChannelData, Cell, ChannelIndex, ServerData) {
   var JAnchor =$('a',Cell);
   var OldChannelName = "";
   var JNewNode;
   if (JAnchor.size() == 0) {
      JNewNode = $('<a></a>');
      JAnchor = JNewNode;
   }
   else{
      OldChannelName = JAnchor.find("span.text").html();
   }

   var ChannelNameAnchor = JAnchor.get(0);
   var ChannelName = ChannelData.HtmlName;
   var SessionData = ServerData.SessionParams
                     ? ServerData.SessionParams
                     : '';

   var ChannelEncrypted = ChannelData.IsEncrypted;
   ChannelNameAnchor.href = DASHgoToServerLink(ServerData, DASHchannelUriPrefix(ServerData, SessionData) + 'Channel=' + encodeURIComponent(ChannelData.Name));

   if (ChannelName != OldChannelName) {
      ChannelNameAnchor.innerHTML = (
         "<span class='decoration'></span>" +
         "<span class='text' style='display:inline-block'>" +
            ChannelName +
         "</span>"
      );
   }

   $(ChannelNameAnchor).find(".text").get(0).onmouseover = function() {
      DASHtooltipComponent = (ServerData.IsLocal ? ChannelData.Index + '.Description' : '');
      var string = DASHgenerateNameTooltipHtml(ChannelData);
      if(string && string.length > 0) {
         TOOLtooltipLink(string , null, $(ChannelNameAnchor).find(".text").get(0));
      }
      return true;
   };

   // apply a decoration before the channel name if the channel is encrypted
   /*
   if (ChannelEncrypted) {
      $(ChannelNameAnchor).find(".decoration").addClass("decoration_encrypted");
   } else {
      $(ChannelNameAnchor).find(".decoration").removeClass("decoration_encrypted");
   }
   // also generate a tooltip for it that provides encryption details
   var EncryptDecoration = $(ChannelNameAnchor).find(".decoration_encrypted");
   if (EncryptDecoration.length > 0) {
      EncryptDecoration.get(0).onmouseover = function() {
         var string = DASHgenerateEncryptionTooltipHtml(ChannelData);
         var deco = $(ChannelNameAnchor).find(".decoration_encrypted");
         if (string && string.length > 0 && deco.length > 0) {
            TOOLtooltipLink(string, null, deco.get(0));
         }
         return true;
      }
   }
   */

   ChannelNameAnchor.onmouseout = function() {
      TOOLtooltipClose();
      DASHtooltipComponent = '';
   };

   ChannelNameAnchor.onmouseup = function() {
      TOOLtooltipClose();
      DASHtooltipComponent = '';
   };
   if (JNewNode) {
      $(Cell).empty().append( JNewNode );
   }
}

function DASHpopulateQueued(ChannelData, QueuedCell, ChannelIndex, ServerData) {
   var JSpan = $('span', QueuedCell);
   var JNewNode;
   if (JSpan.size() == 0) {
      JNewNode = $('<span></span>');
      JSpan = JNewNode;
   }
   var CountOfRemaining = ChannelData.CountOfRemaining;
   var QueuedHtml = '';
   if ('' == CountOfRemaining) {
      QueuedHtml = '--';
   }
   else {
      var Url = FRMCHqueuedLogBrowserLink(ChannelData.Name,ChannelData.FirstSourceChannelName) + '&' + ServerData.SessionParams + '&' + ServerData.SessionParams;
      QueuedHtml = '<a href="' + DASHgoToServerLink(ServerData, Url) + '">' + CountOfRemaining + '</a>';
   }
   if (JSpan.get(0).innerHTML != QueuedHtml)
   {
      JSpan.get(0).innerHTML = QueuedHtml;
   }

   if (JNewNode) {
      $(QueuedCell).empty().append( JNewNode );
   }
}

function DASHpopulateErrors(ChannelData, Cell, ChannelIndex, ServerData)
{
   var JErrorAnchor = $('a',Cell);
   var JNewNode;
   if (JErrorAnchor.size() == 0) {
      JNewNode = $('<a class="error"></a>');
      JErrorAnchor = JNewNode;
   }

   var ErrorAnchor = JErrorAnchor.get(0);
   ErrorAnchor.href = DASHgoToServerLink(ServerData, '/log_browse?Source=' + encodeURIComponent(ChannelData.Name) + '&Type=errors_unmarked' + '&' + ServerData.SessionParams);

   var CountOfError = (ChannelData.CountOfError == '' ? '--' : ChannelData.CountOfError);
   if (ErrorAnchor.innerHTML != CountOfError)
   {
      ErrorAnchor.innerHTML = CountOfError;
   }
   if (JNewNode) {
      $(Cell).empty().append( JNewNode );
   }
}

function DASHchannelStatusToolTip(ChannelData, ServerData, Container, CurrentToolTipComponent, DataFunction, ToolTipFunction, ExtraStatusParams) {
   var StatusUrl = '/channel_status_data.html';
   var StatusParams = 'Channel='+encodeURIComponent(ChannelData.Name) + (ExtraStatusParams ? '&' + ExtraStatusParams : '');
   if (!ServerData.IsLocal) {
      StatusUrl = '/channel_status_remote_data.html';
      StatusParams += '&ServerId=' + encodeURIComponent(DASHserverId(ServerData));
   }
   DASHtooltipComponent = CurrentToolTipComponent;
   function doPost(){
      if (DASHtooltipComponent == CurrentToolTipComponent) {
         var ToolTipData = '';

         $.ajax({
            url: StatusUrl,
            data: StatusParams,
            async: false,
            success: function(Data) {
               ToolTipData = Data;
            },
            error: function() {}
         });

         ToolTipText = (ToolTipData.Error ? ToolTipData.Error : DataFunction(ToolTipData));

         // Massage the Tooltip text and Edit Script URL, if it is present.
         var TT = $(ToolTipText);
         if (!ChannelData.CanEditScripts) {
            TT.find (".edit_permission_required").removeAttr("href").text ("You do not have the necessary permissions to edit the script.");
            ToolTipText = $('<div>').append(TT).html(); // jQuery does not have outerHtml function, so we wrap in a temporary div
         } else {
            var RawURL = TT.find(".edit_permission_required").attr("href");
            if (RawURL) {
               // The session params, if needed, want to be first.
               var SessionParams = "";
               var SessionParamsRaw = DASHsessionParams(ServerData);
               if (SessionParamsRaw) {
                  SessionParams = SessionParamsRaw + "&";
               }

               // Replace the macro in the URL with the session info, if necessary, or just a ?
               var CookedURL = DASHgoToServerLink(ServerData, RawURL.replace(/%%SESSION_INFO%%/g, "?" + SessionParams));

               // Rebuild the HREF and render back to a blob of HTML for the user-agent.
               TT.find(".edit_permission_required").attr("href", CookedURL);
               ToolTipText = $('<div>').append(TT).html();
            }
         }

         TOOLtooltipLink(ToolTipText, ToolTipFunction, Container);
         //refresh. If we mouse out it should do nothing.
         setTimeout(doPost, 750);
      }
   }
   doPost();
};

function DASHpopulateLastActivity(ChannelData, Cell, ChannelIndex, ServerData)
{
   var JSpan = $('span', Cell);
   var JNewNode;
   if (JSpan.size() == 0) {
      JNewNode = $('<span class="text" style="display:inline-block"></span>');
      JSpan = JNewNode;
   }

   var LastActivitySpan = JSpan.get(0);
   var LastActivityText = ChannelData.LastActivity;
   var LastActivityLink;

   if ('' == LastActivityText)
   {
      LastActivityText = '--';
   }

   LastActivityLink = '<a href="' + DASHgoToServerLink(ServerData,'/log_browse?' + ServerData.SessionParams + '&Source=' + encodeURIComponent(ChannelData.Name)) + '">' + LastActivityText + '</a>';

   if (NORMnormalizeInnerHtml(LastActivityLink) != LastActivitySpan.innerHTML)
   {
      LastActivitySpan.innerHTML = LastActivityLink;
   }

   var CurrentToolTipComponent = DASHserverId(ServerData) + '.' + ChannelData.Index + '.LastActivity';

   LastActivitySpan.onmouseover = function(){
      ToolTipIndex = ChannelData.Index;
      ToolTipType = "ProcessedCounts";
      DASHchannelStatusToolTip(ChannelData,ServerData,this,CurrentToolTipComponent, function(ToolTipData){return (ToolTipData.ProcessedCounts ? ToolTipData.ProcessedCounts.Summary : '')}, null,'TooltipForDashboard=1')
   };

   LastActivitySpan.onmouseout = function()
   {
      ToolTipIndex = "";
      ToolTipType = "";
      TOOLtooltipClose();
      DASHtooltipComponent = '';
   };

   LastActivitySpan.onmouseup = function()
   {
      ToolTipIndex = "";
      ToolTipType = "";
      TOOLtooltipClose();
      DASHtooltipComponent = '';
   };
   if (JNewNode) {
      $(Cell).empty().append(JNewNode);
   }
}

function DASHpopulateServer(ChannelData, Cell, ChannelIndex, ServerData)
{
   var ServerLink = ServerData.HrefPrefix;
   if (ServerLink != '') {
      var Url = '?' + ServerData.SessionParams;
      Cell.innerHTML = '<a href="' + DASHgoToServerLink(ServerData,Url)  + '">' + ServerData.ServerLabelHighlighted + '</a>';
   } else {
      Cell.innerHTML = ServerData.ServerLabelHighlighted;
   }
}

function DASHclearUserLoginMessage()
{
   DASHrefresh('clear_user_login_message=true');
}

function DASHclearUpgradeLogFileMessage()
{
   DASHrefresh('clear_upgrade_log_file_message=true');
}

function DASHstartChannel(ChannelName, ServerData)
{
   DASHrefresh('action=start&channel=' + encodeURIComponent(ChannelName) + (ServerData && !ServerData.IsLocal ? '&server_id=' + encodeURIComponent(DASHserverId(ServerData)) : ''));
}

function DASHstopChannel(ChannelName, ServerData)
{
   DASHrefresh('action=stop&channel=' + encodeURIComponent(ChannelName) + (ServerData && !ServerData.IsLocal ? '&server_id=' + encodeURIComponent(DASHserverId(ServerData)) : ''));
}

function DASHclearServiceErrors(ServerData)
{
   DASHrefresh('action=clearServiceErrors' + (ServerData && !ServerData.IsLocal ? '&server_id=' + encodeURIComponent(DASHserverId(ServerData)) : ''));
}

function DASHparseJson(Input)
{
   try
   {
      var JsonData = Input;
      if (JsonData.length) {
         if (JsonData[0] == '(') {
            var Out = JsonData.substr(1,JsonData.length);
            JsonData = Out;
         }
         if (JsonData[JsonData.length - 1] == ')') {
            var Out = JsonData.substr(0,JsonData.length-1);
            JsonData = Out;
         }
      }
      //the jQuery json parse does a lot of validation,
      //taking up almost half the parsing time. We just call the
      //native json parse here, failing that, we just eval.
      if (window.JSON) {
         return window.JSON.parse(JsonData);
      }
      else {
         return eval('(' + JsonData + ')');
      }
   }
   catch(e)
   {
      throw 'The response contains invalid data.';
   }
}

var DashboardTopPanelOrigTop = 80;

function DASHcheckAndDisplayErrorPanel(){
   var HaveWarnings = false;
   var JWarningDivs = $('#dashboard_warnings_panel>div');
   JWarningDivs.each( function() {
         if (this.style.display != 'none') {
            HaveWarnings = true;
         }
      });
   var VisibleHeight = 0;
   if (HaveWarnings) {
      //need to show the container panel first,
      //else the contained elements
      //will have no height
      $('#dashboard_warnings_panel').show();
      JWarningDivs.each( function() {
         if (this.style.display != 'none') {
            VisibleHeight += $(this).outerHeight();
         }
         });
      $('#dashboard_warnings_panel').height( VisibleHeight );
      //add some margin
      $('#dashboard_top_panel').css('top', DashboardTopPanelOrigTop+VisibleHeight + 10);
   }
   else {
      $('#dashboard_warnings_panel').hide();
      $('#dashboard_top_panel').css('top', DashboardTopPanelOrigTop);
   }
}

function DASHcalculateChannelCounts(Model){
   var TotalCountOfChannel = 0;
   var CountOfChannel = 0;
   for (ServerIndex in Model.Servers) {
      var ThisDash = Model.Servers[ServerIndex].DashboardData;
      if (ThisDash) {
         TotalCountOfChannel += (ThisDash.TotalCountOfChannel ? ThisDash.TotalCountOfChannel : 0);
         CountOfChannel += (ThisDash.CountOfChannel ? ThisDash.CountOfChannel : 0);
      }
   }
   Model.TotalCountOfChannel = TotalCountOfChannel;
   Model.CountOfChannel = CountOfChannel;
}

function DASHinitSelectServerList(){
   if (!DASHselectedServers){
      //init with every server set
      DASHselectedServers = {};
      for (ServerIndex in DASHmodel.Servers) {
         if (ServerIndex > 0){
            var ThisServer = DASHmodel.Servers[ServerIndex];
            DASHselectedServers[DASHserverId(ThisServer)] = true;
         }
      }
   }
   else{
      //if servers appear that are not in
      //the DASHselectedServer list at all, default to true
      //this will ensure that newly added servers default to selected
      //without a refresh
      var CountChanged = 0;
      for (ServerIndex in DASHmodel.Servers) {
         if (ServerIndex > 0){
            var ThisServer = DASHmodel.Servers[ServerIndex];
            if (DASHselectedServers[DASHserverId(ThisServer)] === undefined){
               DASHselectedServers[DASHserverId(ThisServer)] = true;
               CountChanged++;
            }
         }
      }
      if (CountChanged) DASHsaveSelectedServersOptions();
   }
}

// Request is just the object that was used to make the Ajax request. We don't
// really care about this.
function DASHonDashboardData(ResponseText, ResponseContentType, Request, Initial) {
   DASHclearOnRefreshWaiting();

   try {
      if (!ResponseContentType.match('application/json')) {
         DASHonAjaxError();
         return;
      }

      DASHmodel = DASHparseJson(ResponseText);

      if (DASHmodel.LoggedIn !== undefined && !DASHmodel.LoggedIn) {
         MiniLogin.show('Your Iguana Session has Expired', DASHonMiniLogin);
         return;
      }

      if (DASHmodel.ErrorDescription !== undefined) {
         alert(DASHmodel.ErrorDescription );
      }

      SCMbumpStatus(DASHmodel, ifware.ImportManager.statusMap, "Will stop channel");
      var LocalServerData = DASHmodel.Servers[0].DashboardData;

      for (ServerIndex in DASHmodel.Servers) {
         var ThisServer = DASHmodel.Servers[ServerIndex];
         //append in the href info so we don't have to build it everytime
         ThisServer.HrefPrefix = DASHhrefPrefix(ThisServer);
         ThisServer.SessionParams = DASHsessionParams(ThisServer);
         ThisServer.ServerLabelHtml = $('<div/>').text( ThisServer.ServerLabel ).html();
      }

      //Server bulk select, init if needed
      DASHinitSelectServerList();

      DASHclearError();

      DASHcheckAndDisplayPanic(LocalServerData);
      DASHcheckAndDisplayLicenseWarnings(LocalServerData);
      DASHcheckAndDisplayEmailSettings(LocalServerData);

      //reinit filter
      DASHstopMagnifyingGlass();
      DASHfilterString = LocalServerData.ChannelFilterString;

      DASHserverTime = LocalServerData.Time;

      //total up the channel counts from all servers
      DASHcalculateChannelCounts(DASHmodel);
      DASHdisplaySearchCount(DASHmodel.TotalCountOfChannel, DASHmodel.CountOfChannel, LocalServerData.HtmlEscapedChannelQueryError);

      //redraw dashboard/server grid, see callbacks
      DASHchannelTable.setRowCount(DASHmodel.CountOfChannel);
      DASHserverTable.setRowCount(DASHmodel.Servers.length);

      //check and display warnings/errors/popups
      DASHcheckAndDisplayNoChannels(LocalServerData, DASHmodel.CountOfChannel);
      DASHdisplayUserLoginMessage(LocalServerData.UserLoginMessage);
      DASHdisplayUpgradeLogFilePopUp(LocalServerData.UserUpgradeLogFileMessage);

      //Bulk stuff
      DASHbulkStatus();

      REFinitializeWithExistingInterval('DASHonRefreshTimer()', DASHserverTime);
      TOOLinitialize();

      // If the function was called from DASHinitialize then we don't bother
      // checking the channel count. In this case the JSON data passed in may be
      // out of date since the InitialJsonData is generated with dashboard.cs
      // and will be cached if the navigation buttons are used. It's better to
      // instead only attempt to initiate the demo after receiving responses from
      // dashboard_data requests.
      if (!Initial) {
         // The entry point to the demo code is in a timeout function because
         // there is a possible error case that we haven't figured out yet.
         // See #22826. This modifies the execution path for the function such
         // that if an error does occur it will be caught at the console level
         // rather than in the below catch block. This will prevent the error
         // from being displayed on the dashboard.
         setTimeout(function(){ifware.ImportManager.checkForDemo(DASHmodel);}, 0);
      }

      DASHsetupBulkActionSelectList();
   } catch(Error) {
      var Description = Error.message || Error.description || Error;
      DASHdisplayError('An error occurred after reading the refresh response from Iguana.', Description);
      REFinitializeWithExistingInterval('DASHonRefreshTimer()', DASHserverTime);
   }
   //Determine if any of the errors are displayed, if they are,
   //show the error panel
   DASHcheckAndDisplayErrorPanel();
}

function DASHonMiniLogin()
{
   DASHrefresh('');
}

function DASHonAjaxError(StatusText)
{
   DASHclearOnRefreshWaiting();
   MiniLogin.show('Iguana is not responding.', DASHonMiniLogin);
}

function DASHonChannelListVisibleRange(MinIndex,MaxIndex){
   if (DASHmodel && DASHmodel.Channels){
      //always set range now so the refreshes can be precise
      DASHmodelQueryOptions.ChannelList.MinChannelRowIndex = MinIndex;
      DASHmodelQueryOptions.ChannelList.MaxChannelRowIndex = MaxIndex;
      if (MinIndex < DASHmodel.MinChannelRowIndex || MaxIndex > DASHmodel.MaxChannelRowIndex){
         //need new data
         DASHrefresh(REFgetParams());
      }
      else return true;
   }
   return false;
}

function DASHonChannelListTableData(ColumnInfo, ColIndex, RowIndex, Cell)
{
   if (DASHmodel && DASHmodel.Channels){
      if (RowIndex >= DASHmodel.MinChannelRowIndex && RowIndex < DASHmodel.MaxChannelRowIndex){
         var AllChannelIndex = RowIndex - DASHmodel.MinChannelRowIndex;
         if (AllChannelIndex < 0) AllChannelIndex = 0;

         var ChannelData = DASHmodel.Channels[AllChannelIndex].Channel;
         var ChannelIndex = ChannelData.Index;
         var ServerData = DASHmodel.Servers[ DASHmodel.Channels[AllChannelIndex].ServerIndex ];

         var DataFunctions = {
            'Bulk' : DASHcreateBulk,
            'Channel' : DASHpopulateChannelName,
            'StartStop' : DASHpopulateStartStopButton,
            'Status' : DASHpopulateStatus,
            'Type' : DASHpopulateTypeIcons,
            'Server' : DASHpopulateServer,
            'Errors' : DASHpopulateErrors,
            'Last Activity' : DASHpopulateLastActivity,
            'Queued' : DASHpopulateQueued
         }
         var ThisDataFunc = DataFunctions[ColumnInfo.name];
         if (ThisDataFunc) ThisDataFunc(ChannelData, Cell, ChannelIndex, ServerData);
      }
      //else out of range
   }
   else $(Cell).html('');
}

function DASHonChannelListTableHeader(ColumnInfo, ColIndex, Cell, LabelSpan)
{
   var NameMap = { 'Channel' : function(Span){ $(Span).html('Channel Name') } ,
                   'Bulk' :   function(Span) {
                      if (!$('input:checkbox[name=bulkSelectAll]',Span).size()) {
                         $(Span).html('<input type="checkbox" name="bulkSelectAll" id="bulkSelectAll" onClick="DASHbulkSelectAllChannels()" class="no_style">');
                      }
                   },
                   'StartStop' : function(Span){ $(Span).html('Start/Stop')},
                   'Status' : function(Span){ $(Span).html('<img src="/images/status.png">')}
   };
   (NameMap[ColumnInfo.name] ? NameMap[ColumnInfo.name](LabelSpan) : $(LabelSpan).html(ColumnInfo.name));
}

var LowerCasePool = {};
var LowerCasePoolSize = 0;

function DASHgetLowerCase(Input){
   //TODO - manage contents/pool size
   //a little better
   if (LowerCasePoolSize > 10000) {
      LowerCasePool = {};
      LowerCasePoolSize = 0;
   }
   var Lookup = LowerCasePool[Input];
   if (!Lookup) {
      LowerCasePool[Input] = Input.toLowerCase();
      LowerCasePoolSize++;
      Lookup = LowerCasePool[Input];
   }
   return Lookup;
}

function DASHarrayCompare(Lhs, Rhs){
   //loop through the fields in an array
   //comparing each element
   for (Key in Lhs) {
      var ThisLhs = Lhs[Key];
      var ThisRhs = Rhs[Key];
      //case insensitive for strings
      if (ThisLhs.toLowerCase && ThisRhs.toLowerCase) {
         ThisLhs = DASHgetLowerCase(ThisLhs);
         ThisRhs = DASHgetLowerCase(ThisRhs);
      }
      if (ThisLhs > ThisRhs) return 1;
      else if (ThisLhs < ThisRhs) return -1;
      //equal, loop again
   }
   return 0; //everything equal
}

function DASHonSortChannelListArray(ColumnInfo, ColIndex, SortDirection)
{
   //if options have changed
   var NewSortDir = (SortDirection == 'asc' ? true : false);
   if (DASHmodelQueryOptions.ChannelList.Ascending != NewSortDir ||
       DASHmodelQueryOptions.ChannelList.SortName != ColumnInfo.sort_name){
      //refresh query
      DASHmodelQueryOptions.ChannelList.Ascending = NewSortDir;
      DASHmodelQueryOptions.ChannelList.SortName = ColumnInfo.sort_name;

      DASHrefresh(REFgetParams());

      //save
      DASHsaveQueryOptions();
   }
   return null;
}

function DASHonServerTableHeader(ColumnInfo, ColIndex, Cell, LabelSpan)
{
   if (ColumnInfo.name == 'Status') $(LabelSpan).html('<img src="/images/status.png">');
   else if (ColumnInfo.name == 'Select') {
      if (!$('input:checkbox[name=bulkServerSelectAll]',LabelSpan).size()) {
         $(LabelSpan).html('<input type="checkbox" name="bulkServerSelectAll" id="bulkServerSelectAll" onClick="DASHbulkServerSelectAll(this)" class="no_style">');
      }
   }
   else $(LabelSpan).html( ColumnInfo.name );
}

function DASHserviceErrorHtml(ServerData, DashboardData)
{
   var HtmlOut = '<div class="' + (parseInt(DashboardData.CountOfError) > 0 ? 'serviceHasErrors' : '') + '">';

   if (DashboardData.CurrentUserCanAdmin) {
      HtmlOut += '<a class="countOfError" href="'
              +      DASHgoToServerLink(ServerData, '/log_browse?'
              +      ServerData.SessionParams
              +      '&Source=%20Iguana&Type=errors_unmarked')
              + '">'
              + DashboardData.CountOfError
              + '</a>';
      if (DashboardData.ClearChannelErrorsBusy) {
         HtmlOut += ' : <span style="color:#949494; cursor:default;">\
                        [<span style="text-decoration:underline;">busy</span>] \
                     </span>'
      } else {
         var LinkHtml = '[clear]';
         if (DashboardData.CountOfError > 0){
            LinkHtml = '<a class="clsClearErrorLink" href="#clearErrors">[clear]</a>';
         }

         HtmlOut += ' : <span>' + LinkHtml + '</span>';
      }
   } else {
      HtmlOut += '<span>' + DashboardData.CountOfError + '</span>';
   }

   return HtmlOut + '</div>';
}

function DASHserviceErrorEditLink(Cell, ServerData){
   if (ServerData.DashboardData){
      JClearLink = $('a.clsClearErrorLink',Cell);
      JClearLink.click(function(){DASHclearServiceErrors(ServerData)});
   }
}

function DASHserverLightStatus(ServerData){
   var ServerErrorEntry = DASHserverErrorEntry(ServerData);
   if (!ServerErrorEntry) return 2;
   else if (ServerErrorEntry == 'Updating...') return 1;
   else if (ServerErrorEntry == 'Starting Iguana...') return 1;
   else if (ServerErrorEntry == 'Not selected') return DASHnotSelectedLightStatus;
   return 0;
}

function DASHserverErrorEntry(ServerData){
   if (ServerData){
      if (!ServerData.DashboardData){
         return ServerData.Error
      }
      else if (!ServerData.IsLocal){
          //only return error messages for remote
          //iguana's
          if(ServerData.DashboardData.PanicErrorString){
              return ServerData.DashboardData.PanicErrorString;
          }
          else if (ServerData.DashboardData.UserLoginMessage){
              return ServerData.DashboardData.UserLoginMessage;
          }
      }
   }
   return '';
}

function DASHonServerColumnSpan(ColumnInfo, ColIndex, ServerIndex){
   if (ColIndex == DASHcolumnAfterServerNameIndex){
      var ServerData = (DASHmodel && DASHmodel.Servers ? DASHmodel.Servers[ServerIndex] : null);
      if (DASHserverErrorEntry(ServerData)){
         //there's an error, span across the remaining columns
         return (DASHserverTable.column_headings.length - DASHcolumnAfterServerNameIndex);
      }
   }
   return 1;
}

function DASHonServerSelect(Checkbox){
   var ServerIndex = Number(Checkbox.value);
   var ServerData = (DASHmodel && DASHmodel.Servers ? DASHmodel.Servers[ServerIndex] : null);
   if (ServerData && ServerIndex > 0){
      var ServerKey = DASHserverId(ServerData);
      DASHselectedServers[ServerKey] = Checkbox.checked;
      DASHsaveSelectedServersOptions();
      DASHrefresh(REFgetParams());
   }
}

function DASHbulkServerSelectAll(Checkbox){
   if (DASHmodel){
      DASHselectedServers = {};
      for (ServerIndex in DASHmodel.Servers) {
         if (ServerIndex > 0){
            var ThisServer = DASHmodel.Servers[ServerIndex];
            DASHselectedServers[DASHserverId(ThisServer)] = Checkbox.checked;
         }
      }
      $('input[name=ServerSelect]')
         .each(
               function(){
                  if (this.value != '0'){
                     //skip local
                     this.checked = Checkbox.checked;
                  }
               }
              );
      DASHsaveSelectedServersOptions();
      DASHrefresh(REFgetParams());
   }
}

function DASHformatBytes(Value,Multiplier){
    var AsNum = Number(Value);
    if (!isNaN(AsNum)){
        AsNum *= Multiplier;
        //Tweaked from
        //from http://codeaid.net/javascript/convert-size-in-bytes-to-human-readable-format-(javascript)
        var sizes = ['B','KB', 'MB', 'GB', 'TB'];
        var i = parseInt(Math.floor(Math.log(AsNum) / Math.log(1024)));
        if (i < sizes.length) return '' + (AsNum / Math.pow(1024, i)).toFixed(2) + ' ' + sizes[i];
    }
    return Value;
}

function DASHiguanaVersionPreSix(ServerData) {
   return parseFloat(ServerData.DashboardData.VersionString) < 6;
}

function DASHchannelUriPrefix(ServerData, SessionData) {
   if (DASHiguanaVersionPreSix(ServerData)) {
      return '/channel.html?' + SessionData + '&';
   }
   return '/channel?' + SessionData + '#';
}

function DASHonServerTableData(ColumnInfo, ColIndex, ServerIndex, Cell) {
   function makeServerLabelHtml(ServerData) {
      var ServerLink = ServerData.HrefPrefix;
      if (ServerLink) {
         var Url = '/dashboard.html?' + ServerData.SessionParams;
         return '<a href="' + DASHgoToServerLink(ServerData, Url) + '">' + ServerData.ServerLabelHtml + '</a>';
      } else {
         return ServerData.ServerLabelHtml;
      }
   }
   function makeVersionLabelHtml(ServerData, VersionInfo){
      var Url = '/version_info.html?' + ServerData.SessionParams;
      return '<a href="' + DASHgoToServerLink(ServerData, Url)  + '">' + (VersionInfo ? VersionInfo : 'details')  + '</a>';
   }

   function makeServerStatus(ServerData){
      var LightSrcs = [ DASHskinningCookieData + '/images/button-dotredv4.gif',
                        DASHskinningCookieData + '/images/button-dotyellowv4.gif',
                        DASHskinningCookieData + '/images/button-dotgreenv4.gif',
                        DASHskinningCookieData + '/images/button-dotgrayv4.gif'];
      var LightStatus = DASHserverLightStatus(ServerData);
      return [LightStatus, '<img src="'+LightSrcs[LightStatus]+'" width="16" height="16">'];
   }

   var ServerData = (DASHmodel && DASHmodel.Servers ? DASHmodel.Servers[ServerIndex] : null);
   if (ServerData) {
      var DashboardData = ServerData.DashboardData;
      var DataStrings = null;
      var NewHtml = '';
      var HrefPrefix = ServerData.HrefPrefix;
      var ServerErrorEntry = DASHserverErrorEntry(ServerData);

      if (ColumnInfo.name == 'Select'){
         var Checked =  (ServerIndex == 0? 'checked disabled' : (DASHselectedServers[ DASHserverId(ServerData)] ? 'checked' : ''));
         NewHtml = '<input type="checkbox" name="ServerSelect" value="' + ServerIndex + '" '+ Checked + ' onClick="javascript:DASHonServerSelect(this)"  >';
      } else if (!ServerErrorEntry) {
         DataStrings = {
         'Status':makeServerStatus(ServerData)[1],
         'Server Name': makeServerLabelHtml(ServerData),
         'Time': DashboardData.Time,
         'Uptime':  '' + DashboardData.Uptime,
         'Running Channels' : '' + DashboardData.CountOfRunningChannel + ' of ' + DashboardData.TotalCountOfChannel,
         'Version': makeVersionLabelHtml(ServerData, DashboardData.VersionString),
         'Service Errors' : DASHserviceErrorHtml(ServerData, DashboardData),
         'Queued' : (DashboardData.TotalCountOfRemaining !== undefined ? ''+DashboardData.TotalCountOfRemaining : '--'),
         'In Msgs/sec': '' + DashboardData.InboundTps,
         'Out Msgs/sec' : '' + DashboardData.OutboundTps,
         'Db Connections' : '' +DashboardData.NumberDatabaseConnections,
         'In/Out Ports' : '<a href="' + DASHgoToServerLink( ServerData, '/port_status.html?' + ServerData.SessionParams) + '">' +
         DashboardData.NumberListeningPorts + ' of ' + DashboardData.NumberListeningPortsTotal + '</a>' +
         '/<a href="' + DASHgoToServerLink(ServerData,'/outbound_llp_ports.html?' + ServerData.SessionParams) + '">' + DashboardData.NumberOfRunningConsumers  + '&nbsp;of&nbsp;' + DashboardData.NumberOfConsumers + '</a>',
         'CPU %' : '' + (DashboardData.MachineInfo ? DashboardData.MachineInfo.CPUPercent : '--'),
         'Disk Free' : '' + (DashboardData.MachineInfo ? DASHformatBytes(DashboardData.MachineInfo.DiskFreeMB,1024*1024) : '--'),
         'Process ID' : '' + (DashboardData.MachineInfo ? DashboardData.MachineInfo.ProcessId : '--'),
         'Memory' : '' + (DashboardData.MachineInfo ? DASHformatBytes(DashboardData.MachineInfo.MemoryKB,1024) : '--'),
         'Resident Memory' : '' + (DashboardData.MachineInfo ? DASHformatBytes(DashboardData.MachineInfo.MemRssKB,1024) : '--'),
         'File Handles' : '' + (DashboardData.MachineInfo ? DashboardData.MachineInfo.OpenFD : '--'),
         'Threads' : '' + (DashboardData.MachineInfo ? DashboardData.MachineInfo.ThreadCount : '--'),
         'Response Time (sec)' : '' + (DashboardData.DashboardResponseTime < 1000 ? '< 1' : DashboardData.DashboardResponseTime/1000),
         'Channel Info': '<a href="' + (DASHiguanaVersionPreSix(ServerData)
                                       ? DASHgoToServerLink(ServerData, "/channel_properties.html?" + ServerData.SessionParams)
                                       : DASHgoToServerLink(ServerData, '/settings?' + ServerData.SessionParams + '#Page=channel/properties')) 
                                     + '">Properties</a> | <a href="' + DASHgoToServerLink(ServerData,'/routes.html?' + ServerData.SessionParams) + '">Routes</a>',
         'Web API': '<a href="' + HrefPrefix + '/monitor_query?'+ServerData.SessionParams +'" target="_new">Expanded</a>/' + '<a href="' + HrefPrefix + '/monitor_query?Compact=Y'+ServerData.SessionParams +'" target="_new">Compact</a>'
        }
         NewHtml = DataStrings[ColumnInfo.name];
      }
      else {
         var ServerStatus = makeServerStatus(ServerData);
         if (ColumnInfo.name == 'Server Name') {
            NewHtml = makeServerLabelHtml(ServerData);
         } else if (ColumnInfo.name == 'Status'){
            NewHtml = ServerStatus[1];
         } else if (ColIndex == DASHcolumnAfterServerNameIndex){
            NewHtml = (ServerStatus[0] < 1 ? '<div class="serviceHasErrors">'+ ServerErrorEntry + '</div>' : ServerErrorEntry);
         } else{
            NewHtml = '--';
         }
      }

      if (NewHtml != $(Cell).html()) {
         $(Cell).html(NewHtml);
         var CellEditTbl = {
            'Service Errors': DASHserviceErrorEditLink
         }
         if (CellEditTbl[ColumnInfo.name]){
            CellEditTbl[ColumnInfo.name](Cell, ServerData);
         }
      }
   } else {
      $(Cell).html('');
   }
}

function DASHonServerSortArray(ColumnInfo, ColIndex, SortDirection) {
   if (DASHmodel)
   {
      //for the closure
      var ServerData;
      var DashboardData;
      var IsError;

      function dashIntVal(Key1, Key2) {
         //this will ignore values if server
         //is in error
         if (IsError) return 0;
         //TODO - there really should be a better way to do this
         if (DashboardData && DashboardData[Key1]) {
             if (Key2 && DashboardData[Key1] && DashboardData[Key1][Key2]){
                 return parseInt(DashboardData[Key1][Key2]);
             }
             else if (!Key2 && DashboardData[Key1]){
                 return parseInt(DashboardData[Key1]);
             }
         }
         return 0;
      }

      SortData = {
         'Status' : function(){
            return [(!IsError ? DASHserverLightStatus(ServerData) : 0), ServerData.ServerLabel];
         },
         'Server Name' : function() {
            return [ServerData.ServerLabel];
         },
         'Uptime' : function() {
            var UptimeVal = (DashboardData && DashboardData.Uptime) || '';
            var Tokens = ['years', 'months', 'days', 'hours', 'minutes', 'second']
            var TokenMultipliers = [1e10,1e8,1e6,1e4,1e2,1];
            var ParseValue;
            var TokenType;
            for (TokenKey in Tokens) {
               var SubStrIndex = UptimeVal.indexOf(Tokens[TokenKey]);
               if (SubStrIndex != -1) {
                  ParseValue = parseInt(UptimeVal.substring(0, SubStrIndex));
                  TokenType = Tokens[TokenKey];
                  break;
               }
            }
            var ResultKey = 0.0;
            if (ParseValue) {
               //form a YYYYMMDDHHMMSS number, which is sortable
               for (TokenKey in Tokens) {
                  if (Tokens[TokenKey] == TokenType) {
                     ResultKey += ParseValue*TokenMultipliers[TokenKey];
                     break;
                  }
               }
            }
            return [(!IsError ? ResultKey : '--'), ServerData.ServerLabel];
         },
         'Running Channels' : function() {
            return [ dashIntVal('CountOfRunningChannel'), ServerData.ServerLabel ];
         },
         'Service Errors' : function() {
            return [ dashIntVal('CountOfError'), ServerData.ServerLabel ];
         },
         'Version' : function() {
            return [ (!IsError ? (DashboardData.VersionString || 'details') : '--'), ServerData.ServerLabel];
         },
         'Db Connections' : function() {
            return [ dashIntVal('NumberDatabaseConnections'), ServerData.ServerLabel ];
         },
         'Queued' : function() {
            return [ dashIntVal('TotalCountOfRemaining'), ServerData.ServerLabel ];
         },
         'In Msgs/sec' : function() {
            return [ dashIntVal('InboundTps'), ServerData.ServerLabel ];
         },
         'Out Msgs/sec' : function() {
            return [ dashIntVal('OutboundTps'), ServerData.ServerLabel ];
         },
         'CPU %' : function() {
             return [ dashIntVal('MachineInfo','CPUPercent'), ServerData.ServerLabel ];
         },
         'Disk Free' : function() {
             return [ dashIntVal('MachineInfo','DiskFreeMB'), ServerData.ServerLabel ];
         },
         'Process ID' : function() {
             return [ dashIntVal('MachineInfo','ProcessId'), ServerData.ServerLabel ];
         },
         'Memory' : function() {
             return [ dashIntVal('MachineInfo','MemoryKB'), ServerData.ServerLabel ];
         },
         'Resident Memory' : function() {
             return [ dashIntVal('MachineInfo','MemRssKB'), ServerData.ServerLabel ];
         },
         'File Handles' : function() {
             return [ dashIntVal('MachineInfo','OpenFD'), ServerData.ServerLabel ];
         },
         'Threads' : function() {
             return [ dashIntVal('MachineInfo','ThreadCount'), ServerData.ServerLabel ];
         },
         'Response Time (sec)' : function() {
            var ResponseTime = dashIntVal('DashboardResponseTime');
            if (!IsError){
               ResponseTime = (ResponseTime < 1 ? 1 : ResponseTime);
            }
            return [ ResponseTime, ServerData.ServerLabel ];
         }
      }

      //save if needed
      var NewSortDir = (SortDirection == 'asc' ? true : false);
      var NewSortName = ColumnInfo.name;
      if (DASHmodelQueryOptions.ServerList.Ascending != NewSortDir ||
          DASHmodelQueryOptions.ServerList.SortColumnName != NewSortName){
         DASHmodelQueryOptions.ServerList.Ascending = NewSortDir;
         DASHmodelQueryOptions.ServerList.SortColumnName = NewSortName;
         DASHsaveQueryOptions();
      }

      //adjust sort array depending on the
      //sort direction, and if the server has been disabled
      //the goal is to keep disabled servers at the bottom of the list
      function adjustForDisabled(ServerData, NormalSortArray){
         var ThisStatus = DASHserverLightStatus(ServerData);
         var DisabledValue =  (SortDirection == 'asc' ? true : false);
         NormalSortArray.unshift(ThisStatus == DASHnotSelectedLightStatus ? DisabledValue : !DisabledValue);
      }

      var SortDataFunc = SortData[ColumnInfo.name];
      if (SortDataFunc) {
         Out = []
         for (ServerIndex in DASHmodel.Servers) {
            ServerData = DASHmodel.Servers[ServerIndex];
            DashboardData = DASHmodel.Servers[ServerIndex].DashboardData;
            IsError = DASHserverErrorEntry(ServerData) != '';
            var SortArray = SortDataFunc();
            adjustForDisabled(ServerData, SortArray);
            Out.push( SortArray );
         }
         return Out;
      }
   }
   return null;
}

function DASHpreInitialize(ServerTime)
{
   DASHserverTime = ServerTime;
}

function DASHinitializeChannelTable(DefaultSortName, DefaultSortAscending)
{
  var ColumnInfo =
     [ {name : 'Bulk', no_select: true, min_width: 30, data_style: 'width: 30px; text-align: center'},
  {name : 'StartStop', min_width: 110, data_style: 'width: 110px; text-align: center' },
  {name : 'Status', min_width: 30, no_select: true,sort_name : 'Status', data_style: 'width: 30px; text-align: center', sortable: true},
  {name : 'Type', min_width: 95, sort_name: 'Type', data_style: 'width: 95px; text-align: center', sortable: true },
  {name : 'Server', min_width: 150, sort_name: 'Server', data_style: '', sortable: true, hidden: true },
  {name : 'Channel', min_width: 150, no_select: true, sort_name: 'Name', data_style: '', sortable: true },
  {name : 'Last Activity', min_width: 200, sort_name : 'LastActivity', data_style: 'width: 200px; text-align: center', sortable: true},
  {name : 'Errors', min_width: 70, sort_name : 'Errors', data_style: 'width: 70px; text-align: center', sortable: true },
  {name : 'Queued', min_width: 70, sort_name : 'Queued', data_style: 'width: 70px; text-align: center', sortable: true }
  ];
  //look for column name from sort name
  var DefaultSortColumnName = 'Channel';
  for (ColIndex in ColumnInfo){
     if (ColumnInfo[ColIndex].sort_name == DefaultSortName){
        DefaultSortColumnName = ColumnInfo[ColIndex].name;
        break;
     }
  }
  //hide columns that need to be hidden
  for (ColIndex in ColumnInfo){
     var ColInfo = ColumnInfo[ColIndex];
      if (DASHchannelColSelect[ColInfo.name] != null){
         ColInfo.hidden = (DASHchannelColSelect[ColInfo.name] === false);
      }
  }

  DASHchannelTable = new IGNtable('divDashboardTable',
                                   ColumnInfo,
                                   {
                                      default_sort_name : DefaultSortColumnName,
                                      default_sort_dir : (DefaultSortAscending ? 'asc' : 'desc'),
                                      rows_per_page : 100,
                                      page_select : { container_id : 'ChannelListPagingContainer' }
                                   },
                                   DASHonChannelListTableHeader,
                                   DASHonChannelListTableData,
                                   DASHonSortChannelListArray,
                                   DASHonChannelListVisibleRange
                                   );
  DASHsetupColSelectPullOut(DASHchannelTable,'divChannelSlideOut', DASHchannelColSelect, 'Channels', {'StartStop' : 'Start/Stop'});
}

function DASHinitializeServerTable(DefaultSortName, DefaultSortAscending)
{
   var ColumnInfo =
[  {name : 'Select', min_width: 30, data_style: 'width: 30px; text-align: center', sortable: false,no_select : true},
   {name : 'Status', min_width: 30, data_style: 'width: 30px; text-align: center', sortable: true,no_select : true, sort_function : DASHarrayCompare},
   {name : 'Server Name',min_width: 150, data_style: '',sortable: true, no_select : true, sort_function : DASHarrayCompare},
   {name : 'Time', min_width: 80, data_style: 'width: 80px; text-align: left', sortable:false, hidden: false, no_select : true},
   {name : 'Uptime', min_width: 65, data_style: 'width: 65px; text-align: center', sortable: true, hidden: true, sort_function : DASHarrayCompare },
   {name : 'Running Channels', min_width: 140, data_style: 'width: 140px; text-align: center', sortable: true, sort_function : DASHarrayCompare },
   {name : 'Version', min_width: 100, data_style: 'width: 100px', sortable: true, hidden: true, sort_function : DASHarrayCompare },
   {name : 'Service Errors', min_width: 120, data_style: 'width: 120px; text-align: center', sortable: true, sort_function : DASHarrayCompare },
   {name : 'Queued', min_width: 80, data_style: 'width: 80px; text-align: center', sortable: true, sort_function : DASHarrayCompare },
   {name : 'In Msgs/sec', min_width: 100, data_style: 'width: 100px; text-align: center', sortable: true, sort_function : DASHarrayCompare },
   {name : 'Out Msgs/sec', min_width: 110, data_style: 'width: 110px; text-align: center', sortable: true, sort_function : DASHarrayCompare },
   {name : 'Db Connections', min_width: 120, data_style: 'width: 120px; text-align: center', sortable: true, sort_function : DASHarrayCompare, hidden: true },
   {name : 'In/Out Ports', min_width: 100, data_style: 'width: 100px; text-align: center', sortable: false, hidden: true },
   {name : 'CPU %', min_width: 75, data_style: 'width: 75px; text-align: center', sortable: true, hidden: true , sort_function : DASHarrayCompare},
   {name : 'Disk Free', min_width: 80, data_style: 'width: 80px; text-align: center', sortable: true, hidden: true , sort_function : DASHarrayCompare},
   {name : 'Process ID', min_width: 75, data_style: 'width: 75px; text-align: center', sortable: true, hidden: true , sort_function : DASHarrayCompare},
   {name : 'Memory', min_width: 75, data_style: 'width: 75px; text-align: center', sortable: true, hidden: true , sort_function : DASHarrayCompare},
   {name : 'Resident Memory', min_width: 120, data_style: 'width: 120px; text-align: center', sortable: true, hidden: true , sort_function : DASHarrayCompare},
   {name : 'File Handles', min_width: 90, data_style: 'width: 90px; text-align: center', sortable: true, hidden: true , sort_function : DASHarrayCompare},
   {name : 'Threads', min_width: 75, data_style: 'width: 75px; text-align: center', sortable: true, hidden: true , sort_function : DASHarrayCompare},
   {name : 'Response Time (sec)', min_width: 140, data_style: 'width: 140px; text-align: center', sortable: true, hidden: true , sort_function : DASHarrayCompare},
   {name : 'Channel Info', min_width: 120, data_style: 'width: 120px; text-align: center', sortable: false, hidden: true },
   {name : 'Web API', min_width: 100, data_style: 'width: 120px; text-align: center', sortable: false }
   ];
   //hide columns that need to be hidden
   for (ColIndex in ColumnInfo){
      var ColInfo = ColumnInfo[ColIndex];
      if (DASHserverColSelect[ColInfo.name] != null){
         ColInfo.hidden = (DASHserverColSelect[ColInfo.name] === false);
      }
   }
   DASHserverTable = new IGNtable('divServerTable',
   ColumnInfo,
   { default_sort_name : DefaultSortName,
     default_sort_dir : (DefaultSortAscending ? 'asc' : 'desc'),
     min_table_width: 800 },
   DASHonServerTableHeader,
   DASHonServerTableData,
   DASHonServerSortArray,
   null,
   DASHonServerColumnSpan
  );
  DASHsetupColSelectPullOut(DASHserverTable,'divServerSlideOut', DASHserverColSelect, 'Servers');
}

function DASHcolSelectReadCookie(TableName){
   var CookieStr = COOKIEread( DASHcookieName('Iguana-ColSelect-' + TableName));
   var Output = {};
   try{
      Output = DASHparseJson(CookieStr);
   }catch(e){}
   return Output;
}

function DASHcolSelectWriteCookie(ColMap, TableName){
   COOKIEcreate(DASHcookieName('Iguana-ColSelect-' + TableName), DASHjsonStringify(ColMap), 365);
}

var DASHchannelColSelect = DASHcolSelectReadCookie('Channels');
var DASHserverColSelect = DASHcolSelectReadCookie('Servers');

function DASHsetupColSelectPullOut(Table, SelectElementId, VisibleMap, TableName, ColNameMap)
{
   $('#' + SelectElementId).append('<div class="dashboard_column_select_all">select <a class="select_bulk" href="#">all</a> | <a class="select_bulk" href="#">none</a></div>');
   $('#' + SelectElementId + ' .select_bulk').click( function(){
           var CurrentChecked = ($(this).text() == 'all');
           $('#' + SelectElementId + ' input:checkbox').each(function(){
              if (CurrentChecked != $(this).is(':checked')){
                  $(this).parent().click();
              }
           })
   });

   var Cols = Table.column_headings;
   for (ColIndex in Cols) {
      var Col = Cols[ColIndex];
      if (!Col.no_select){
         var Visible = !Col.hidden;
         var JEntry = $(  '<div _colname="' + Col.name + '" _colchecked="' +(!Visible ? 'false' : 'true' ) + '"  class="dashboard_column_select">'
                          + '<input type="checkbox" ' + (!Visible ? '' : 'checked') + '></input>'+(ColNameMap && ColNameMap[Col.name] ? ColNameMap[Col.name] : Col.name)+'</div>');
         $('#' + SelectElementId).append(JEntry);
         JEntry.click( function() {
               var IsChecked = $(this).attr('_colchecked') == 'true';
               var ColName = $(this).attr('_colname');
               Table.toggleColumn(ColName,!IsChecked);
               IsChecked = !IsChecked;
               $(this).attr('_colchecked', (IsChecked ? 'true' : 'false') );
               $('input:checkbox', this).prop('checked',IsChecked);
               VisibleMap[ColName] = IsChecked;
               DASHcolSelectWriteCookie(VisibleMap,TableName);
            })
            .hover( function(){$(this).css( {'background':'#22A0D3', 'color':'white'})},
                    function(){$(this).css( {'background':'', 'color':''})})
      }
   }
}


function DASHinitialize(InitialJsonData) {
   REFinitialize('DASHonRefreshTimer()', DASHserverTime);
   TOOLinitialize();

   DASHinitQueryOptions();
   DASHloadSelectedServersOptions();

   DASHinitializeChannelTable(DASHmodelQueryOptions.ChannelList.SortName, DASHmodelQueryOptions.ChannelList.Ascending);
   DASHinitializeServerTable(DASHmodelQueryOptions.ServerList.SortColumnName, DASHmodelQueryOptions.ServerList.Ascending);

   //init HLP
   HLPpopUpinitialize(null);

   //this sets up Enter key behaviour for the clear queue dialog box
   $(document).keypress(function(e){
         if (e.keyCode === 13 && $('#bulk_confirm_dialog_form').is(":visible")) {
            e.preventDefault();
            $('[aria-labelledby$=bulk_confirm_dialog_form]').find
               (":button:contains('Submit')").click();
         }
   });

   //setup popup for log file upgrade msg
   $('#upgrade_log_file_popup').dialog({
         bgiframe: true,
         width: 500,
         autoOpen: false,
         modal: true,
         close : DASHclearUpgradeLogFileMessage
   })

   //some errors are displayed on first load, and not through ajax, so
   //show the panel if needed
   DASHcheckAndDisplayErrorPanel();

   // Pass in null for the request object.
   DASHonDashboardData(InitialJsonData,'application/json', null, true);

   //init filters from url's search parameter
   var FilterFromQuery = QRYgetValue('search');
   if (FilterFromQuery != 'false') {
      $('#inputChannelFilter').val(FilterFromQuery);
      DASHonFilterStringChange(FilterFromQuery);
   } else {
      //manually set up the filter field from initial data
      //must be called after DASHonDashboardData() or DASHmodel could be null
      $('#inputChannelFilter').val(DASHmodel.Servers[0].DashboardData.ChannelFilterString);
      if (DASHgetFilterStringFromField().length > 0) {
         DASHmakeFilterClearButtonVisible();
      }
   }

   // Refresh the dashboard right away after using the InitialJsonData rather
   // than waiting for the interval to fire off. This way, if the InitialJsonData
   // was cached and out of date we can update the dashboard sooner with the
   // correct data and initiate the channel demo a second earlier.
   DASHrefresh("");
}

window.onbeforeunload = function(){
   TOOLtooltipClose();
}

