/** @license
 * jQuery File Tree Plugin
 *
 * Version 1.02
 *
 * Cut down and modified quite a lot - Art
 *
 * Cory S.N. LaViska
 * A Beautiful Site (http://abeautifulsite.net/)
 * 24 March 2008
 *
 * Visit http://abeautifulsite.net/notebook.php?article=58 for more information
 *
 * Usage: $('.fileTreeDemo').fileTree( options, callback )
 *
 * Options:  root           - root folder to display; default = /
 *           script         - location of the serverside AJAX file to use; default = jqueryFileTree.php
 *           folderEvent    - event to trigger expand/collapse; default = click
 *
 * History:
 *
 * 1.01 - updated to work with foreign characters in directory/file names (12 April 2008)
 * 1.00 - released (24 March 2008)
 *
 * TERMS OF USE
 * 
 * This plugin is dual-licensed under the GNU General Public License and the MIT License and
 * is copyright 2008 A Beautiful Site, LLC. 
 */

if(jQuery) (function($){
   
   $.extend($.fn, {
      fileTree: function(o, h, f) {
         // Defaults
         if( !o ) var o = {};
         if( o.root == undefined ) o.root = '/';
         if( o.script == undefined ) o.script = 'file_browser';
         if( o.folderEvent == undefined ) o.folderEvent = 'click';
         
         $(this).each( function() {
            
            function showTree(c, t, v) {
               $(c).addClass('wait');
               $(".jqueryFileTree.start").remove();
               $.ajax({
                  url: o.script,
                  type: "POST",
                  data: { dir: t, type: document.getElementById('FileTypeSelect').value, folders: v, hidden: o.hidden},
                  success: function(Response) {
                     var data = Response.html;
                     $(c).find('.start').html('');
                     $(c).removeClass('wait').append(data);
                     $(c).find('UL:hidden').show(); 
                     bindTree(c, v);
                     $('#FolderName').html(Response.full_path);
                  },
                  error: function(data) {
                     console.log(data + " failed");
                  }
               });
            }
            
            function bindTree(t, v) {
               $(t).find('LI A:not(".stepUp")').bind(o.folderEvent, function(event) {
                  if (v == 'false')
                  {                  
                     if( !$(this).parent().hasClass('directory') ) {
                        h($(this).attr('rel'), $(this).attr('rev'));
                        $('LI A.currentSelect').each( function() { $(this).removeClass('currentSelect'); });
                        $(this).addClass('currentSelect');
                     }
                  }
                  return false;
               });
               // Prevent A from triggering the # on non-click events
               if( o.folderEvent.toLowerCase != 'click' ) $(t).find('LI A').bind('click', function() { return false; });
              
            }
            // Loading message
            $(this).html('<ul class="jqueryFileTree start"><li class="wait">Loading...<li></ul>');
            // Get the initial file list
            showTree( $(this), o.root, o.foldersOnly );
            
         });
      }
   });
   
})(jQuery);
