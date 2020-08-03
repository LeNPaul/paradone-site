<?cs # vim: set syntax=html :?>

<style type="text/css">
   iframe#channel-status-frame {
      <?cs set: browse_background = 'gray' ?>
      background: <?cs var:browse_background ?>;
      opacity:0.8; filter:alpha(opacity=80);
   }
</style>


<iframe id="channel-status-frame" src="/empty.html"
   style="position:absolute; display:none; z-index:5999; border:0">
</iframe>

<div id="ChannelStatusModalWindow" class="modalPopupWindow" style="display: none; width: 460px; z-index:6000">
	<table>
		<tr>
   			<td class="modalIcon">
   				<img src="/<?cs var:skin("images/icon_warning_large.gif") ?>">
   			</td>
   			<td>
   				<h3>Note:</h3>
  				<span class="modalText">This channel is currently running. The channel must be stopped before changes can be made.</span>
  
			</td>
		</tr>
		<tr>
			<td colspan="2">
   				<center>
      			<button type="button" onclick="javascript:ChannelStatusModal.close();" style="
      				width: 60px;
      				color: #444;
      				background: #deddc5;
      				border-top-color:#000;
      				border-top-width:1px;
      				border-bottom-color:#b6b77c;
      				border-bottom-width:1px;
      				border-left-color:#000;
      				border-left-width:1px;
      				border-right-color:#b6b77c;
      				border-right-width:1px;
      				font-size: 9px; 
      				font-weight: bold; 
      				padding: 3px;">OK</button>
   				</center>
   			</td>
   		</tr>
   	</table>
</div>

<script type="text/javascript">
var ChannelStatusModal = function(){
   
   function openModalWindow(obj) {
      var Frame = document.getElementById('channel-status-frame');
      var Page = document.documentElement;

      Frame.style.top  = '0px';
      Frame.style.left = '0px';
      Frame.style.height = Page.scrollHeight + 'px';
      Frame.style.width  = Page.scrollWidth  + 'px';
      Frame.style.display = '';
      
      var div = document.getElementById("ChannelStatusModalWindow");
      div.style.position='absolute';

      div.style.display = '';
      div.style.top  = Math.floor((Page.offsetHeight - div.offsetHeight) / 2) + Page.scrollTop  + 'px';
      div.style.left = Math.floor((Page.offsetWidth  - div.offsetWidth)  / 2) + Page.scrollLeft + 'px';

   }
   
   function closeModalWindow() {
      var Frame = document.getElementById('channel-status-frame');
      Frame.style.display = 'none';
      
      var div = document.getElementById("ChannelStatusModalWindow");
      div.style.display='none';

      var editTab = document.getElementById("editTab").value;
      ChannelName = '<?cs var:js_escape(Channel.Name)?>';
      document.location = "/channel#Channel=" + encodeURIComponent(ChannelName) + "&editTab=" + editTab;
   }
   
   return {
      show: openModalWindow,      
      close: closeModalWindow
   }
}();
</script>
