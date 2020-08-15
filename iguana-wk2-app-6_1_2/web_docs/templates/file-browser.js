/** @license
 * Copyright (c) 2010-2015 iNTERFACEWARE Inc.  All rights reserved.
 */

function setupFileBrowser(UniqueInstanceId) {
   //console.log("Setting up file browser");
   // File Browser modal globals
   var FILframe = document.getElementById('fileBrowser-frame');
   var FILpopup = document.getElementById('fileBrowser-popup');
   var FILpage =  document.documentElement;
   var FILelement;
   var FILfoldersOnly;
   
   ifware.Settings.FileBrowser = {

      // Show the File Browser
      FILmodalShow: function() {   
         var Page = document.documentElement;
         FILframe.style.top  = '0px';
         FILframe.style.left = '0px';
         FILframe.style.height = $(document).height() + 'px';
         FILframe.style.width  = Page.scrollWidth + 'px';
         FILframe.style.display = '';

         if( FILframe.contentWindow ) { 
            // IE6 doesn't use iframe background colors. 
            FILframe.contentWindow.document.body.style.background = 'gray';
         }
         FILpopup.style.visibility = 'hidden';
         FILpopup.style.display = '';
         FILpopup.style.top  = Math.floor((WINgetWindowHeight() - FILpopup.offsetHeight) / 2) + FILpage.scrollTop  + 'px';
         FILpopup.style.left = Math.floor((FILpage.offsetWidth  - FILpopup.offsetWidth)  / 2) + FILpage.scrollLeft + 'px';
         FILpopup.style.visibility = 'visible';
      },

      FILmodalHide: function() {
         // Hide the File Browser
         FILpopup.style.display = 'none';
         FILframe.style.display = 'none';
      },

      FILgetPath: function() {
         //first check the value of the field - if there is one, extact the path. If not, check for a cookie.
         Path = '';
         if (document.getElementById(FILelement).value != '') {
            Path = $.ajax({
               url: "/file_browser_extract_path",
               global: false,
               type: "POST",
               data: ({path : document.getElementById(FILelement).value}),
               dataType: "text",
               async: false      
               }
            ).responseText;
            try {
               // kind of a cheat.  Usually I just return the path, so if it's JSON, we know
               // the user is not logged in.
               // could also just go all the way and do: if ((DataTree.LoggedIn !== undefined) && (!DataTree.LoggedIn))
               var DataTree = eval('(' + Path + ')');
               Path = '';          
            } 
            catch(e) {
               //do nothing ('cause we'll display whatever was returned)
            }
         }
         if (Path == '') {
            Path = COOKIEread('Iguana-FileBrowserPath-' + UniqueInstanceId);
            if (Path == null || Path == '') {
               Path = './';
            }
         }
         return Path;
      },

      FILselected: function(fromKey) {
         console.log("FILselected");
         // Returns the full path and file name of the selected file to the calling element
         //document.getElementById(FILelement).value = document.getElementById('fileBrowserPath').value + document.getElementById('fileBrowserFile').value;
         AJAXpost('/file_browser_path',  "path=" + document.getElementById('fileBrowserPath').value + "&folders=" + FILfoldersOnly + "&file=" + encodeURIComponent(document.getElementById('fileBrowserFile').value),
            function(Data, ContentType) {
               if (Data != "") {
                  if (Data == "*File*"){
                     document.getElementById(FILelement).value = document.getElementById('fileBrowserPath').value + document.getElementById('fileBrowserFile').value;
                     ifware.Settings.FileBrowser.FILmodalHide();
                  } else {
                     if (fromKey  == '0' && FILfoldersOnly == 'true') {  
                        try {
                           // kind of a cheat.  Usually I just return the path, so if it's JSON, we know
                           // the user is not logged in.
                           // could also just go all the way and do: if ((DataTree.LoggedIn !== undefined) && (!DataTree.LoggedIn))
                           var DataTree = eval('(' + Data + ')');
                           Data = ''; 
                        } catch(e) {
                              //do nothing ('cause we'll display whatever was returned)
                        }
                        document.getElementById(FILelement).value = Data;
                        ifware.Settings.FileBrowser.FILmodalHide();
                     } else {
                        if (FILfoldersOnly == 'true') {
                           ifware.Settings.FileBrowser.FILfolderBrowse(Data);
                        } else {
                           ifware.Settings.FileBrowser.FILbrowse(Data);
                        }
                     }
                  }
               }
            }, null);
         //ifware.Settings.FileBrowser.FILmodalHide();
      },

      FILbrowse: function(root, element) {
         // Launches the file browser for the specific root
         if (element != undefined) {
            FILelement = element;
         }
         if ( root === undefined || root == '' ) {
            root = ifware.Settings.FileBrowser.FILgetPath();
         }
         COOKIEcreate('Iguana-FileBrowserPath-' + UniqueInstanceId, root, 365);
         FILfoldersOnly = 'false';
         document.getElementById('FileTypeSelect').onchange = function() {ifware.Settings.FileBrowser.FILbrowse(root); };
         document.getElementById('fileBrowserVolume').onchange = function() {ifware.Settings.FileBrowser.FILbrowse(this.value); };

         // here I'm setting the select box to the value passed in
         // In the future, I might want to reverse this and set the root here
         document.getElementById('fileBrowserVolume').value = root;
   
         //root = root.replace(/\\/g, '\\\\');
         if (document.getElementById('fileBrowserVolume').style.display == 'none') { ifware.Settings.FileBrowser.FILvolumes(); }
         document.getElementById('fileBrowserFileLabel').innerHTML = 'File:';
   
         //$('#fileBrowser').innerHTML = '';
         $('#fileBrowser').fileTree({ root: root, script: 'file_browser', foldersOnly: FILfoldersOnly, hidden: true }, function(file, folder) {
            document.getElementById('fileBrowserFile').value = file;
         });
      
         ifware.Settings.FileBrowser.FILmodalShow(); 
         FILtextboxFile = document.getElementById('fileBrowserFile');
         FILtextboxFile.value = '';
         FILtextboxFile.focus();
      },

      FILfolderBrowse: function(root, element, first) {
         if (element != undefined) {
            FILelement = element;
         }   
         if ( root === undefined || root == '' ) {
            root = ifware.Settings.FileBrowser.FILgetPath();
         }
         COOKIEcreate('Iguana-FileBrowserPath-' + UniqueInstanceId, root, 365);
         FILfoldersOnly = 'true';
         document.getElementById('FileTypeSelect').onchange = function() {ifware.Settings.FileBrowser.FILfolderBrowse(root); };
         document.getElementById('fileBrowserVolume').onchange = function() {ifware.Settings.FileBrowser.FILfolderBrowse(this.value); };

         // here I'm setting the select box to the value passed in
         // In the future, I might want to reverse this and set the root here
         document.getElementById('fileBrowserVolume').value = root;
         document.getElementById('fileBrowserFileLabel').innerHTML = 'Folder:';
         $('#FileTypeSelectLabel').hide();
         $('#FileTypeSelect').hide();
         //root = root.replace(/\\/g, '\\\\');
         if (document.getElementById('fileBrowserVolume').style.display == 'none') { ifware.Settings.FileBrowser.FILvolumes(); }
   
         //$('#fileBrowser').innerHTML = '';
         $('#fileBrowser').fileTree({ root: root, script: 'file_browser', foldersOnly: FILfoldersOnly, hidden: true }, function(file, folder) {
            document.getElementById('fileBrowserFile').value = file;
         });
      
         ifware.Settings.FileBrowser.FILmodalShow(); 
         FILtextboxFile = document.getElementById('fileBrowserFile');
         if (first) {
            // need to get full path (corrected)
            AJAXpost('/file_browser_path',  "path=" + root + "&file=&folders=true",
               function(Data, ContentType) {
                  if (Data != "") {
                     try {
                        // kind of a cheat.  Usually I just return the path, so if it's JSON, we know
                        // the user is not logged in.
                        // could also just go all the way and do: if ((DataTree.LoggedIn !== undefined) && (!DataTree.LoggedIn))
                        var DataTree = eval('(' + Data + ')');
                        Data = ''; 
                     } catch(e) {
                        //do nothing ('cause we'll display whatever was returned)
                     }
                     FILtextboxFile.value = Data;
                  }
               }, null);
         } else {
            FILtextboxFile.value = root;
         }
         FILtextboxFile.focus();
      },

      onEnter: function(event) {
         KeyCode = window.event ? window.event.keyCode : event.which
         if (KeyCode == 13) {      
            ifware.Settings.FileBrowser.FILselected('1');
         }  
         return !(window.event && window.event.keyCode == 13);  
      },

      FILvolumes: function() {
         // Get a list of available drives/volumes
         AJAXpost('/file_browser_volumes',  null,
            function(Data, ContentType) {
               if ( ContentType.match('application/json')) {
                  var DataTree = eval(Data);
                  if (DataTree.length > 0 ) {
                     options = '<option value="./">Select Drive/Volume...</option>';
                     for (var i = 0; i < DataTree.length; i++) {
                        options += '<option value="' + DataTree[i].drive + '">' + DataTree[i].drive + '</option>';
                     }
                     $("select#fileBrowserVolume").html(options);
                  } else {
                     $("select#fileBrowserVolume").hide();
                  }
               }
            }, null);
         $('select#fileBrowserVolume').show();
      }
   }
}

