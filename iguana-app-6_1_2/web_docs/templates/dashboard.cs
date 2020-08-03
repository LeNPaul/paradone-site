<?cs include:"doctype.cs" ?>
<html>  <?cs # vim: set syntax=html :?>

<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Dashboard</title>
   <?cs include:"browser_compatibility.cs" ?>

   <!-- CSS -->
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin('dashboard.css') ?>" />
   <link rel="stylesheet" type="text/css" href="<?cs var:iguana_version_js('/js/mapper/source_control.css') ?>" />
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/help_popup/help_popup.css") ?>" />
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css") ?>">

   <!-- JavaScript -->
   <script type="text/javascript"><!--
   UniqueInstanceId = '<?cs var:UniqueInstanceId ?>';
   DASHiconWarningImgSrc = '/<?cs var:skin('images/icon_warning.gif') ?>';
   DASHcurrentUser = '<?cs var:js_escape(CurrentUser) ?>';
   --></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/cookie/cookiev4.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("queryv4.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("refreshv4.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("sort.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("tooltip.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("dashboard.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("class.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/help_popup/help_popup.js") ?>"></script>

   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-1.11.2.min.js"></script>
   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.min.js"></script>

   <script type="text/javascript" src="<?cs var:iguana_version_js("jquery.jfeed.pack.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("from_channel.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("table.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("version_notifier.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/source_control.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("ixport.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/ifware.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/local_storage.js") ?>"></script>
   <?cs include:"mini-login.cs" ?>
   
   <script type="text/javascript">
   var SplitterCookieKey = 'Iguana-Dashboard-Splitter-Pos';
   // We store the fraction that the splitter is from the bottom of its container
   // rather than the top so that it will tend towards the bottom of the screen
   // when the window is resized.
   function saveSplitterPosition(FractionFromBottom){
      COOKIEcreate(DASHcookieName(SplitterCookieKey), '' + FractionFromBottom, 365);
   }

   function getSavedSplitterPosition(){
      var StrPos = COOKIEread( DASHcookieName(SplitterCookieKey));
      var IntPos = (StrPos ? Number(StrPos) : -1);
      return IntPos;
   }
   function adjustPanels(splitter_height, splitter_offset){
      var margin_from_splitter = 5;
      var toppos = Math.round(splitter_offset.top);

      var doc_offset = $(window).height();
      var bottom_panel_top_pos = toppos + splitter_height + margin_from_splitter;
      var top_panel_bottom_pos = doc_offset - (bottom_panel_top_pos - (2 * margin_from_splitter) - splitter_height);

      $('#dashboard_bottom_panel').css('top', bottom_panel_top_pos);
      $('#dashboard_top_panel').css('bottom', top_panel_bottom_pos);
   }

   function onSplitterMove( Splitter, e, ui ) {
      // By default jQuery modifies an element's "top" style property when
      // it's dragged, but we want the splitter to move in terms of its
      // bottom position.
      Splitter.css("top", ""); // erase
      Splitter.css("bottom", getSplitterBottom(Splitter, ui.position.top));
      
      adjustPanels(Splitter.height(), ui.offset);
      saveSplitterPosition(getSplitterFraction(Splitter, ui.position.top));
   }

   function getSplitterFraction(Splitter, top_position){
      var splitter_fraction = getSplitterBottom(Splitter, Math.round(top_position)) / Splitter.parent().height();
      return splitter_fraction;
   }
   
   // Calculates the bottom position of the splitter in relation to
   // its parent.
   function getSplitterBottom(Splitter, top_position) {
      return Splitter.parent().height() - Math.round(top_position) - Splitter.height();
   }

   function positionSplitter(Splitter, splitter_fraction){
      var container_height = $(Splitter).parent().height();
      var new_bottom = container_height * splitter_fraction;

      if (new_bottom > container_height){
         new_bottom = container_height;
      } else if (new_bottom < 0) {
         new_bottom = 0;
      }

      Splitter.css('bottom', new_bottom);
   }

   function initializeSplitter() {
      var Splitter = $('#dashboard_splitter');
      $(Splitter).draggable(
         {axis : 'y', 
          containment : 'parent', 
          drag: function(e, ui) { onSplitterMove($(this), e, ui) },
          stop: function(e, ui) { onSplitterMove($(this), e, ui) },
          cursor: 'n-resize'
         }
      );

      var saved_fraction = getSavedSplitterPosition();

      if (saved_fraction >= 0){
         positionSplitter(Splitter,saved_fraction);
      }

      // splitter will be positioned at the bottom otherwise
      adjustPanels($(Splitter).height(),$(Splitter).offset());

      $(window).resize( function() {  
         var Splitter = $('#dashboard_splitter');
         var splitter_fraction = getSplitterFraction(Splitter, Splitter.position().top);
         // NOTE: If this line being being commented out causes issues see #27030
         //positionSplitter(Splitter, splitter_fraction);
         adjustPanels(Splitter.height(), Splitter.offset());
      });
   }

   var SliderMouseInside = false;
   var SliderOpen = false;

   function initializeOptions() {
      $('body').mouseup(function(){ 
         if(!SliderMouseInside && SliderOpen) $('#divSlideOutTab').click();
      });
 
      function sliderHoverIn(){
         SliderMouseInside = true;
      }

      function sliderHoverOut(){
         SliderMouseInside = false;
      }

      $('#divSlideOutTab').click(function(){ 
          options = { left: 0 };
          if (SliderOpen) options.left = -250;
             $('#divSlideOut').animate( options, (SliderOpen ? 250 : 500) );
             SliderOpen = !SliderOpen; 
          }
      ).hover(sliderHoverIn,sliderHoverOut);

      $('#divSlideOutContents').hover(sliderHoverIn,sliderHoverOut);
   }

   $(function() {
      initializeSplitter();
      initializeOptions();
      DASHpreInitialize('<?cs var:javascript_escape(Time) ?>');
      DASHinitialize('<?cs var:javascript_escape(InitialJsonData) ?>');
      VNBversionNotificationController.init();
      VNBversionNotificationController.fetchContent();
      setInterval("VNBversionNotificationController.fetchContent()", 24 * 60 * 60 * 1000);  // once a day
   });
   </script>
</head>

<body class="tableft" style="width: 100%; height: 100%; margin: 0; padding: 0; overflow: hidden;">

<?cs set:Navigation.CurrentTab = "Dashboard" ?>

<div class="version" style="display:none;"></div>

<?cs include:"header.cs" ?>

<!-- see the js file for how this div is handled -->
<div id="dashboard_warnings_panel" style="font-size: 11px; display: none; position: absolute; height: 300px; top: 80px; left: 25px; right: 25px; overflow: auto;">
   <?cs include:"dashboard_warnings.cs" ?>
</div>

<div id="dashboard_splitter_container">
   <div id="dashboard_splitter"></div>
</div>

<div id="divSlideOut">
   <div>
      <div id="divSlideOutContents">
         <div>
            <div style="width:100%; height: 100%">
               <span class="dashboard_column_select_header">Channel Columns</span>
               <div id="divChannelSlideOut">
               </div>
            </div>
         </div>
         <div>
            <div style="">
               <span class="dashboard_column_select_header">Server Columns</span>
               <div id="divServerSlideOut"></div>
            </div>
         </div>
      </div>
      <div id="divSlideOutTab">
      </div>       
   </div>
</div>

<div id="dashboard_top_panel">
   <div id="dashboard_top_panel_container" style="position: absolute; top: 0px; bottom: 0px; left: 25px; right: 25px;" class="dashboard_backsplash">
      <div style="position: absolute; height: 30px; top: 0px; left: 0px; right: 0px;">
      <table class="dashboard_top">
         <tr>
            <td style="width: 40px;" id="cookie_crumb"> Dashboard </td>
            <td style="width: 400px; text-align:center;">
               <span id="spnUpdateStatus"></span>
            </td>
            <td style="background-color: transparent; padding-top: 2px; padding-bottom: 1px; padding-right: 17px; color: #787a77; text-align: right; font-size: 9px; font-weight: normal;">
               <table class="search_field_container">
                  <tr>
                     <td>
                        <span style="display: none; font-weight: bold; color: black;" id="spnSearchCountDisplay"/>
                     </td>
                     <td style="padding-left:0px; padding-top:0px; padding-bottom:0px;">
                        <div class="search_field">
                           <img id="imgMagnifyingGlass" src="/<?cs var:skin("images/icon_search.gif") ?>"/>
                           <input id="inputChannelFilter" value=""
                                  onkeyup="javascript:DASHonFilterStringChange(this.value);"
                                  onmouseover="javascript:DASHshowSearchTooltip(this);"
                                  onmouseout="javascript:DASHhideSearchTooltip(this);"
                                  onmouseup="javascript:DASHhideSearchTooltip(this);"
                           />
                           <a onclick="javascript:DASHclearChannelFilter();">
                              <img id="imgClearFilterIcon" alt="" src="/<?cs var:skin("images/ex14.gif") ?>"/>
                           </a>
                        </div>
                     </td>
                     <td style="padding:0px;">
                        <a id="TextQuery_Icon"
                           class="helpIcon"
                           tabindex="100"
                           title="More Information"
                           target="_blank"
                           href="#"
                           onclick="return false;"
                           rel="<?cs include:"search_tip_channels.cs" ?>
                                 <a href=&quot;<?cs var:help_link('iguana4_dashboard_search_text') ?>&quot; target=&quot;_blank&quot;>Learn More About Specifying Search Criteria In A Dashboard Search</a></br>
                                 <p>For additional Dashboard information:</p>
                                 <li>
                                 <a href='<?cs var:help_link('iguana4_working_with_channels') ?>' target='_blank'>Working With Channels</a>
                                 </li>
                                 <li>
                                 <a href='<?cs var:help_link('iguana4_viewing_and_editing_server') ?>' target='_blank'>Viewing and Editing Server Information</a>
                                 </li>"
                        >
                           <img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" />
                        </a>
                     </td>
                  </tr>
               </table>
            </td>
         </tr>
      </table>
      </div> <!-- End #dashboard_top -->

      <div id="divDashboardTable_panel" style="position: absolute; bottom: 50px; top: 48px; left: 8px; right: 8px; padding: 0; margin: 0;">
         <div id="divDashboardTable" style="position: absolute; width: 100%; height: 100%;"></div>
      </div>

      <div style="position: absolute; bottom: 4px; height: 40px; left: 5px; right: 5px;">
         <table border=0 id="buttons" width="100%" style="padding: 0;">
            <tr>
               <td align="left" width="25%">  
                  <table style="border-spacing: 0px; border-collapse: collapse; padding: 0; margin: 0;">
                     <tr>
                        <td style="padding: 0px; margin: 0px;">
                           <select style="width:180px" id="bulkDropDown" name="bulkDropDown" onChange="DASHbulkDoAction(this);">
                              <option value="0">Channel Actions</option>
                              <option value="start">Start Channel(s)</option>
                              <option value="stop">Stop Channel(s)</option>
                              <option value="clearChannelErrors">Clear/Mark Channel Errors</option>
                              <option value="clearChannelQueues">Clear Channel Queue(s)</option>
                              
                              <?cs if:CurrentUserCanAdmin ?>
                              <option value="delete">Delete Channel(s)</option>
                              <option value="exportChannels">Export Channel(s)</option>
                              <?cs /if ?>
                           </select>
                        </td>
                        </tr>
                        <tr>
                        <td style="padding: 0px; margin: 0px;">
                           <span id="bulkStatus"><?cs if:ClearChannelErrorsBusy!=0 ?>Busy Clearing Errors... <?cs /if ?></span>
                        </td>
                     </tr>  
                  </table>
               </td>
               <td align="center">
                  <div style="margin-left: auto; margin-right: auto;" id="ChannelListPagingContainer"></div>
               </td>
               <td width="25%" align="right" >
               <?cs if:CurrentUserCanAdmin ?>
                  <a class="action-button blue" id="add_channel" href="/channel">Add Channel</a>
               <?cs /if ?>
               </td>
            </tr>
         </table>
      </div>
   </div> <!-- End #dashboard_top_panel_container -->
</div> <!-- End #dashboard_top_panel -->

<div id="dashboard_bottom_panel" class="dashboard_backsplash">
   <div id="divServerTablePanel">
      <div id="divServerTable"></div>
   </div>
</div>

</div>

<div id="bulk_confirm_dialog_form" title="Channel Action" style="display:none;">
   <div id="bulk_confirm_msg"></div>
   <span id="bulk_confirm_permissions_msg"/></span>

   <form id="clearQueuesForm">
   <p>Enter password to clear queues for the selected channels.</p>
   <p>Queues will only be cleared for channels that are not running.</p>
   <fieldset>
      <label for="ClearQueuesPassword">Password</label>
      <input type="password" name="ClearQueuesPassword" id="ClearQueuesPassword" value="" autocomplete="off"/>
   </fieldset>
   </form>
   <span id="bulk_confirm_error" style="width: 100%; color: red; font-size: 10px; text-align: center;"></span>
</div>

<div id="helpTooltipDiv" class="helpTooltip">
   <b id="helpTooltipTitle"></b>
   <em id="helpTooltipBody"></em>  
   <input type="hidden" name="helpTooltipId" id="helpTooltipId" value="0">
</div>

<div id='no_permissions_popup' style='width:450px;overflow:hidden;' title='Permission Error'>
   <div id='no_permissions_msg'></div>
</div>

<div id='upgrade_log_file_popup' style='width:450px;overflow:hidden;' title='Iguana Log File Upgrade'>
   <div id='upgrade_log_file_popup_msg'></div>
</div>

</body>
</html>

