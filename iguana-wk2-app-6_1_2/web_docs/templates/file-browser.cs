<?cs # vim: set syntax=html :?>

<?cs # Include this file inside your body-tag, at or near the top. Also, make sure the page has /js/jquery132/jquery-1.3.2.js (NOW 1.11.2) and /js/utils/window.js included.  Also, make sure the main template - where you include this file - uses the UniqueInstance ID (see channel.cs or dashboard.cs) as it's required to set a valid per-instance cookie. You must also include browse_macro.cs on the actual page that will be calling the file browser clear silver doesn't seem to allow definitions to propogate through includes ?>

<script type="text/javascript" src="<?cs var:iguana_version_js("jqueryFileTree/jqueryFileTree.js") ?>"></script> 
<link rel="stylesheet" type="text/css" href="<?cs var:skin("jqueryFileTree/jqueryFileTree.css") ?>" />

<iframe id="fileBrowser-frame" src="/empty.html" style="position:absolute; display:none; z-index:5999; border:0;background: grey;opacity:0.8; filter:alpha(opacity=80);"></iframe>


<div id="fileBrowser-popup" style="position:absolute; display:none; z-index:6000;">

         <table id="popup_window">
	        <tr>
               <th id="popup_header">File Browser</th>
            </tr>
            <tr>
               <td id="popup_body">
               
                  <table >
                     <tr>
                        <td style="margin:0px; padding:15px 0px 0px 0px;">
                           <select id="fileBrowserVolume" style="display:none; width:360px; padding:0px;"></select>
                        </td>
                     </tr>
	                 <tr>
		                <td style="margin:0px; padding:5px 0px 0px 0px;">
		                   <div style="background:#EEEEEE; width:348px; font-size: 11px; color:#666666; border-left: 1px solid #BBBBBB;	border-right: 1px solid #BBBBBB; border-top: 1px solid #BBBBBB; padding: 5px;"> 
		                      <label></label>
                              <span id="FolderName"></span>
                           </div>
		                   <div id="fileBrowser" class="fileBrowser"></div>
		                </td>
	                 </tr>   
                     <tr>
		                <td style="">         
                           <form style="" onsubmit="return false;">
                              <label style="display: inline-block; margin: 10px 0" id="FileTypeSelectLabel">Type:
                              <select id="FileTypeSelect" style="margin-left: 10px">
                                 <option value="*.*">All Files (*.*)</option>
                                 <option value="vmd">VMDs (*.vmd)</option>
                              </select>
                              </label>
                              <br/>

                              <label style="" id="fileBrowserFileLabel">&nbsp;</label>
                              <input type="text" id="fileBrowserFile" style="margin-left: 17px; width: 260px" onkeypress="return ifware.Settings.FileBrowser.onEnter(event)">
                           </form>
		                </td>
	                 </tr>  
	                 <tr>
		                <td align="center">
		                   <table>
			                  <tr>
				                 <td><a id="fileBrowser-select" class="action-button green"><span>Select</span></a></td>
				                 <td><a id="fileBrowser-cancel" class="action-button green"><span>Cancel</span></a></td>          
			                  </tr>
		                   </table>
		                </td>
	                 </tr>
                  </table>
                  
	           </td>
	        </tr>
         </table>

</div>
<script type="text/javascript">
$(document).ready(function() {
   if (typeof ifware === "undefined") {
      console.log("ifware global var not defined yet, defining...");
      ifware = {};
   }

   ifware.Settings = ifware.Settings || {};
   
   setupFileBrowser('<?cs var:UniqueInstanceId ?>');
   
   $("a#fileBrowser-select").click(function() {
      ifware.Settings.FileBrowser.FILselected('0');
   });

   $("a#fileBrowser-cancel").click(ifware.Settings.FileBrowser.FILmodalHide);
});
</script>

