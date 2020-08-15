/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

function VNBversionNotifierBanner() {
 var m_FeedLink = "http://www.interfaceware.com/wiki/whatsnew.html";

 var m_CloseCookie = "version_notification_close_date";
 var m_VersionCookie = "version_notification_current_date";

 var m_ClosedForVersion = COOKIEread(m_CloseCookie);

 var m_CurrentVersion = {}; 
 m_CurrentVersion.Major = 0;
 m_CurrentVersion.Minor = 0;
 m_CurrentVersion.Build = 0;
 m_CurrentVersion.BuildExt = "";

 var m_OnShowHandler;
 var m_OnCloseHandler;

 function getCurrentVersion(){
  jQuery.ajax({
   "url": "/current_version",
   "cache": false,
   "datatype": "json",
   "success": function(Result) {
    if(!Result) {
     return;
    }
    else {
     m_CurrentVersion.Major = Result.Major;
     m_CurrentVersion.Minor = Result.Minor;
     m_CurrentVersion.Build = Result.Build;
     m_CurrentVersion.BuildExt = Result.BuildExt;
    }
   }
  });
 }

 function extractFeedItemVersion(FeedItem){
  var Link = FeedItem.link;
  var Match = Link.match(/\d+-\d+-\d+/);

  var Version = {};
  Version.Major = Match[0].replace(/-\d+-\d+$/, "");
  Version.Minor = Match[0].replace(/^\d+-/, "").replace(/-\d+$/, "");
  Version.Build = Match[0].replace(/^\d+-\d+-/, "");
  Version.BuildExt = "";

  return Version;
 }

 function isNewerVersion(FeedItemVersion, CurrentVersion){
  var isNewer = false;

  if(FeedItemVersion.Major != CurrentVersion.Major){
   isNewer = FeedItemVersion.Major < CurrentVersion.Major ? false : true;
  }
  else if(FeedItemVersion.Minor != CurrentVersion.Minor){
   isNewer = FeedItemVersion.Minor < CurrentVersion.Minor ? false : true;
  }
  else if(FeedItemVersion.Build != CurrentVersion.Build){
   isNewer = FeedItemVersion.Build < CurrentVersion.Build ? false : true;
  }
  else{
   isNewer = CurrentVersion.BuildExt == "" ? false : true;
  }
 
  return isNewer;
 }
 
 // VNBversionNotifierBanner Interface

 this.init = function (OnShowHandler, OnCloseHandler){
  m_OnShowHandler = OnShowHandler;  
  m_OnCloseHandler = OnCloseHandler;

  if(!m_ClosedForVersion){ 
   m_ClosedForVersion = "";
   COOKIEcreate(m_CloseCookie, m_ClosedForVersion, 360); 
  }     
 }

 this.close = function (){
  $("div.version").slideUp("slow"); 

  m_ClosedForVersion = COOKIEread(m_VersionCookie);
  COOKIEcreate(m_CloseCookie, m_ClosedForVersion, 360);
  if (typeof(m_OnCloseHandler) == "function") {
   m_OnCloseHandler();
  }
 }

 this.fetchContent = function (){
  if(0 == m_CurrentVersion.Major){
   getCurrentVersion();
   setTimeout("VNBversionNotificationController.fetchContent()", 1000);
   return;
  }

  var CurrentDate = new Date();
  
  jQuery.getFeed({
   url: '/versionfeed.rss',
   data: {"time": CurrentDate.getTime()},
   success: function(feed) {
    feed_item = feed.items[0];

    var CurrentVersionDate = new Date(feed_item.updated);
    var Title = feed_item.title;

    if(CurrentVersionDate != m_ClosedForVersion && isNewerVersion(extractFeedItemVersion(feed_item), m_CurrentVersion)){
     $("div.version").slideDown("slow"); 
     COOKIEcreate(m_VersionCookie, CurrentVersionDate, 360);
     if (typeof(m_OnShowHandler) == "function") {
      m_OnShowHandler();
     }

     $("div.version").html( 
      "<a class='version_link' href=" + m_FeedLink + " target='_blank'>" + Title + "</a><a class='close_link' href='javascript:VNBversionNotificationController.close();'><img src='/images/icon-close.png' title='Close' /></a>"
     );
    }
    else{
     $("div.version").slideUp("slow");  
     if (typeof(m_OnCloseHandler) == "function") {
      m_OnCloseHandler();
     }
    }
   }    
  });
 }
}

var VNBversionNotificationController = new VNBversionNotifierBanner();
