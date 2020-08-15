/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Configuration for Iguana version of mapper.
var MPRapp = {};
MPRapp.type = 'Iguana';

MPRapp.pageLookup = {};

PAGsetupCommonMapCalls(MPRapp.pageLookup);

MPRapp.defaultRequest = function(ErrorMessage) {
   ERRrenderDefaultError(ErrorMessage, true);
};

MPRapp.renderError = function(ErrorMessage, Params) {
   ERRrenderError(ErrorMessage, Params, false);
};


MPRapp.header = '<div class="header"><span class="logo"><img src="/mapper/logo_utrans.png"></span>';

function MAPretrieveHeaderHTML(Page, Params) {
   Params.User = escape(Params.User);
   Params.ComponentName = escape(Params.ComponentName);

   var Header = "";
   Header += '<div class="header">\
               <div class="header-left">\
                 <span class="logo">\
                 <img src="/mapper/logo_utrans.png">\
                 </span>\
                 <span class="crumb">';
   Header += MAPsetBreadCrumb(Page, Params);
   Header += '  </span>\
               </div>\
               <div class="header-right">\
                 <span class="help">\
                   <a href="http://www.interfaceware.com/wiki/translator.html?v=6.0.0" target="_manual">Help</a>\
                 </span>\
                 <span class="login">Welcome, ';
   Header += Params.User;
   Header += '     <ul id="menu">\
                     <li>\
                       <a href="#" onClick="return false;">&blacktriangledown;</a>\
                       <ul>\
                         <li><a href="/settings#Page=users/edit?user=' + Params.User + '"><img src="/images/icon-account.png">My Account</a></li>\
                         <li><a href="/login.html"><img src="/images/icon-logout.png">Log Out</a></li>\
                       </ul>\
                     </li>\
                   </ul>\
                 </span>\
               </div>\
             </div>';
   return Header;
}

function MAPretrieveDefaultHeaderHTML() {
   var Header = "";
   Header += '<div class="header">\
             <div class="header-left">\
             <span class="logo">\
              <img src="/mapper/logo_utrans.png">\
             </span>\
             <span class="crumb">\
              <a href="/">Dashboard</a> &gt; Error\
             </span>\
             </div>\
             <div class="header-right">\
             <span class="help">\
              <a href="http://www.interfaceware.com/wiki/translator.html" target="_manual">Help</a>\
             </span>\
             </div>\
             </div>';
   return Header;
}

function MAPretrieveDocTitle(Page, Params) {
   return PAGbaseTitle + " > Dashboard > Channel " + PAGgetField(Params, "ChannelName") + " > Configuration";
}

function MAPretrieveParams(Page, Params) {
   var SubsetParams = {};
   if ("lua_editor" == Page) {
      SubsetParams.ChannelGuid = Params.ChannelGuid;
      SubsetParams.ChannelName = Params.ChannelName;
      SubsetParams.ComponentType = Params.ComponentType;
      SubsetParams.ComponentName = Params.ComponentName;
      SubsetParams.User = Params.User;
      SubsetParams.Index = Params.Index;
      SubsetParams.Module = Params.Module;
   }
   else if ("message_preview" == Page) {
      SubsetParams.ChannelGuid = Params.ChannelGuid;
      SubsetParams.ChannelName = Params.ChannelName;
      SubsetParams.ComponentType = Params.ComponentType;
      SubsetParams.ComponentName = Params.ComponentName;
      SubsetParams.User = Params.User;
      SubsetParams.Index = Params.Index;
      SubsetParams.Module = Params.Module;
   }

   return SubsetParams;
}

function MAPsetBreadCrumb(Page, Params) {
   if (Params.DashboardLinkOnly) {
      return "<a href='/'>Dashboard</a> &rang; " + Params.DashboardLinkOnly;
   }
   var BreadCrumb = "";

   //used in project_manager.js
   MPRapp.ExtraOtherFileMenuOptions =
       [
          {
             name: 'Create DB Tables',
             className: 'optionCreateDbTables',
             func: function(File, FileNode) {
                IGNMPRexportTablesExecute(File.label, File.editor.ifware.ChannelGuid);
             },
             shouldShowFunc: function(File, FileNode) {
                return (File.type == 'vmd');
             }
          }
       ];

   var ChannelInfo = { Guid: PAGgetField(Params, "ChannelGuid"), Name: PAGgetField(Params, "ChannelName") };
   var ComponentInfo = { Type: PAGgetOptionalField(Params, "ComponentType"), Name: PAGgetOptionalField(Params, "ComponentName") };

   BreadCrumb = "<a href='/'>Dashboard</a> &rang; ";
   BreadCrumb += "<a href='/channel#Channel=" + PAGescapeQuotes(encodeURIComponent(ChannelInfo.Name)) + "'>" + PAGhtmlEscape(ChannelInfo.Name) + "</a> &rang;&nbsp; ";

   var TabName = "";
   if ("Destination" == ComponentInfo.Type) {
      TabName = "destinationTab";
   }
   else if ("Filter" == ComponentInfo.Type) {
      TabName = "filterTab";
   }
   else {
      TabName = "sourceTab";
   }

   BreadCrumb += "<a href='/channel#Channel=" + PAGescapeQuotes(encodeURIComponent(ChannelInfo.Name)) + "&editTab=" + TabName + "'>" + ComponentInfo.Name + "</a> &rang;&nbsp; ";

   if ("lua_editor" == Page) {
      BreadCrumb += "Script";
   } else if ("stage_commit" == Page) {
      BreadCrumb += "Commit to Source Control";
   } else if ("review_commits" == Page || "review_file_commits" == Page) {
      BreadCrumb += "Review Commit History";
   } else if ("message_edit" == Page) {
      BreadCrumb += Params.NewMsg == 1 ? "New Message" : "Edit Message (" + PAGgetField(Params, "Index") + " of " + PAGgetField(Params, "TotalSd") + ")";
   } else if ("message_preview" == Page) {
      BreadCrumb += "Messages";
   } else if ("error" == Page) {
      BreadCrumb += "Error";
   } 

   return BreadCrumb;
}
