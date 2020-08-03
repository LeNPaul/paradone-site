<?cs # vim: set syntax=html :?>

<?cs include:"browse_macro.cs" ?>

<script defer type="text/javascript"> <!--

$(document).ready(function() {
   // Show/hide retry details for non-fatal database errors.
   var toggleRetry = function() {
      var open = $('input[name="DstRetryNonFatal"]:checked').length > 0;
      $('#DstRetryNonFatalDiv').toggle(open);
   };
   $('input[name="DstRetryNonFatal"]').click(toggleRetry);
   toggleRetry();
   // Auto-generate test link for non-fatal database error patterns.
   $('input[name="DstRetryNonFatalPattern"]').blur(function() {
      var f = encodeURIComponent( $(this).val() );
      var s = encodeURIComponent('<?cs var:js_escape(Channel.Name) ?>');
      $('a#DstRetryNonFatalPatternTest').attr('href', 'logs.html?'+
         'Filter=!+'+f+'&Source='+s+'&Type=errors_marked,errors_unmarked,warnings');
   }).blur();
});

function toggleNumberOfReconnectsTextField()
{
   var ReconnectOptionsComboBox = document.channel_data.DstMaxReconnectsChoiceBox;
   var ReconnectsText = document.getElementById('DstMaxReconnectsText');
   var ReconnectIntervalRow = document.getElementById('DstReconnectIntervalRow');
   
   if(ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value == "other")
   {
      ReconnectsText.style.display="";
   }
   else
   {
      ReconnectsText.style.display="none";
   }
   if(ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value != "never")
   {
      ReconnectIntervalRow.style.display = "";
   }
   else
   {
      ReconnectIntervalRow.style.display = "none";
   }   
}

function onLoadToDatabase()
{
   showDatabaseNote('Dst', document.channel_data.DstDatabaseApi.value);
}

--> </script>

<?cs with: dest = Channel.Destination ?>
   <?cs def:database_settings(ComponentDatabase, ComponentError) ?>
      <?cs set:ComponentType = "Dst" ?> 
      <?cs include: "component_database.cs" ?>
   <?cs /def ?>
   <?cs call:database_settings(dest.Database, dest.Error) ?>

   <script defer type="text/javascript">
      //Doesn't work in onLoadToDatabase
      //Put it here, after the component_database.cs include.
      DstupdateDatasourcePreview();
      DstupdateDatauserPreview();
      setInterval("DstupdateDatasourcePreview(); DstupdateDatauserPreview();", 500);
   </script>
   
   <tr class="selected">
      <td class="left_column">On database error</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:dest.RetryNonFatal.Enabled ?>
               <?cs if:dest.RetryNonFatal.Pattern ?>
                  <span class="configurationFillerText">Retry after non-fatal database errors matching</span>
                  <?cs var:html_escape(dest.RetryNonFatal.Pattern) ?>
               <?cs else ?>
                  <span class="configurationFillerText">Retry after</span> ALL
                  <span class="configurationFillerText">database errors </span>
               <?cs /if ?>
               <span class="configurationFillerText">at most</span>
               <?cs var:#dest.RetryNonFatal.Limit ?>
               <span class="configurationFillerText">time(s), delaying</span>
               <?cs var:html_escape(dest.RetryNonFatal.Delay) ?>
               <span class="configurationFillerText">second(s) each time; otherwise</span>
               <?cs if:dest.ActionOnDbError   == "skip" ?> skip message
               <?cs elif:dest.ActionOnDbError == "stop" ?> stop channel
               <?cs else ?> unknown
               <?cs /if ?>
               <?cs if:dest.Error.RetryNonFatal ?>
                  <div class="configuration_error"> <?cs var:html_escape(dest.Error.RetryNonFatal) ?> </div>
               <?cs /if ?>
            <?cs elif:dest.ActionOnDbError == "skip" ?> Skip message
            <?cs elif:dest.ActionOnDbError == "stop" ?> Stop channel
            <?cs else ?> Unknown
            <?cs /if ?>
         <?cs else ?>
            <input type="checkbox" id="DstRetryNonFatal" name="DstRetryNonFatal" <?cs if:dest.RetryNonFatal.Enabled ?>checked<?cs /if ?>>
               <label for="DstRetryNonFatal">Retry after non-fatal database errors</label><br>
               <div id="DstRetryNonFatalDiv" style="margin-left:4em; display:none">
                  <div><nobr>
                     matching <input size="40" name="DstRetryNonFatalPattern" value="<?cs var:html_escape(dest.RetryNonFatal.Pattern) ?>">
                     (<a id="DstRetryNonFatalPatternTest" target="_blank" href="#">search logs</a>)
                     <a class="helpIcon" tabindex="100" title="More Information" href="#"
                        rel="<?cs include:"search_tip_logs.cs" ?>
                        <p> <a href=&quot;<?cs var:help_link('iguana4_searching_the_logs') ?>&quot; target=&quot;_blank&quot;>Learn More About Searching the Logs</a>
                        <br><a href=&quot;<?cs var:help_link('using_regex_metacharacters') ?>&quot; target=&quot;_blank&quot;>Learn More About Regular Expressions</a>
                        "><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a> 
                  </nobr></div>
                  <div>
                     at most <input size="3" name="DstRetryNonFatalLimit" value="<?cs var:#dest.RetryNonFatal.Limit ?>"> time(s), delaying
                     <input size="3" name="DstRetryNonFatalDelay" value="<?cs var:html_escape(dest.RetryNonFatal.Delay) ?>"> second(s) each time
                  </div>
               </div>
            <!--/input-->
            Otherwise
            <input type="radio" id="DstSkipOnError" name="DstActionOnDbError" value="skip" <?cs if:dest.ActionOnDbError == "skip" ?>checked<?cs /if ?> >
            <label for="DstSkipOnError">skip message</label> or
            <input type="radio" id="DstStopOnError" name="DstActionOnDbError" value="stop" <?cs if:dest.ActionOnDbError == "stop" ?>checked<?cs /if ?> >
            <label for="DstStopOnError">stop channel</label>
         <?cs /if ?>
      </td>
   </tr>


   <tr class="selected">
      <td class="left_column">Full Generator VMD path<font color="#ff0000">*</font></td>
         <td class="inner_left" colspan="3">
            <?cs if:Channel.ReadOnlyMode ?>
                  <?cs if:dest.Error.ParserVmdPath ?>
                     <div class="configuration_error">
                        <?cs var:dest.Error.ParserVmdPath ?>
                     </div>
                   <?cs else ?>
                     <?cs if:dest.ParserVmdPath != dest.OriginalVmdPath ?>
                        <?cs /if ?> 
                        <?cs if:dest.OriginalVmdPath != "" ?>
                           <label>Uploaded from: </label><br><?cs call:browse_readonly(dest.OriginalVmdPath) ?>
                        <?cs /if ?>
                        <?cs if:dest.ParserVmdPath != "" ?>
                           <br><br><?cs call:download_vmd(Channel.Guid, Channel.editTab, Channel.Name, "DstParserVmd") ?>
                        <?cs /if ?>

                  <?cs /if ?>  
       <?cs else ?>
         <?cs if:!dest.Error.ParserVmdPath && dest.OriginalVmdPath != "" ?>
               <div class="original_vmd">
                  <label>Uploaded from: </label><br><?cs call:browse_readonly(dest.OriginalVmdPath) ?><br>
             </div>
         <?cs /if ?>
               <br><?cs call:browse_input('DstParserVmdPath', dest.ParserVmdPath) ?>
                <?cs if:!dest.Error.ParserVmdPath && dest.OriginalVmdPath != "" ?>
                     <br><a id="useOriginalToDB"><u>Use previously uploaded VMD path</u></a>
                  <script type="text/javascript">
                     $("#useOriginalToDB").click(function(){
                        $("#DstParserVmdPath").val("<?cs var:dest.CleanedOriginal ?>");
                     });
                  </script>
               <?cs /if ?>
               <?cs if:dest.ParserVmdPath != "" ?>
                  <br><br><br><?cs call:download_vmd(Channel.Guid, Channel.editTab, Channel.Name, "DstParserVmd") ?>
                  <a id="DownloadVmdBox" style="padding-left: 4px;" class="helpIcon" tabindex="101" rel="Download the currently running VMD from source control." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
               <?cs /if ?>     
       <?cs /if ?>
         </td>
   </tr>

   <tr class="selected" id="TrackVmdChangesDst" >
      <td class="left_column">Track VMD Changes</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?> 
                     <?cs if:dest.KeepUpdated ?>Yes<?cs else ?>No<?cs /if ?>          
                  <?cs else ?>
                     <br><?cs call:keep_updated("UpdatedParserDestVmd", dest.KeepUpdated) ?>    
                     <a id="KeepVmdUpdatedBox" class="helpIcon" tabindex="101" rel="When enabled, at startup, the channel will compare its current copy of the VMD with the version at the path it was uploaded from. If they are different, the 'uploaded from' version will be automatically copied into the run location and committed to source control before the channel begins processing messages." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                  <?cs /if ?>
               </td>
               <td class="inner_right">
                  <?cs if:Channel.Destination.Error.KeepUpdated ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Destination.Error.KeepUpdated ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>     
         </table>
      </td>
   </tr>

   <tr class="selected">
      <td class="left_column">On VMD parse error</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:dest.ActionOnParseError == "skip" ?>
               Skip message
            <?cs elif:dest.ActionOnParseError == "stop" ?>
               Stop channel
            <?cs else ?>
               Unknown
            <?cs /if ?>
         <?cs else ?>
            <input type="radio" name="DstActionOnParseError" value="skip" <?cs if:dest.ActionOnParseError == "skip" ?>checked<?cs /if ?> >Skip message</input>
            <br><input type="radio" name="DstActionOnParseError" value="stop" <?cs if:dest.ActionOnParseError == "stop" ?>checked<?cs /if ?> >Stop channel</input>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected">
      <td class="left_column">Data insertion order</td>
      <td class="inner_left" colspan="3">
         <?cs if:!Channel.ReadOnlyMode ?>
            <input type="radio" name="DstInsertSorted" value=""
               <?cs if:!dest.InsertSorted ?>checked<?cs /if ?> >In table grammar order
            <br>
            <input type="radio" name="DstInsertSorted" value="on"
               <?cs if: dest.InsertSorted ?>checked<?cs /if ?> >In alphabetical order by table name
            <a id="DstInsertSorted_Icon" class="helpIcon" tabindex="100"
               rel="In rare circumstances deadlocks can occur if two or more 
                    channels insert data into the same tables, but in
                    different orders.  Order is normally dictated by each
                    message's table grammar.  Here you can force insertions
                    and updates to be done alphabetically by table name."
               title="More Information" href="#"
               ><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a> 
         <?cs elif:dest.InsertSorted ?>
            In alphabetical order by table name
         <?cs else ?>
            In table grammar order
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected" id="DstReconnectToDbRow">
      <td class="left_column">Reconnect to DB?</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:dest.MaxReconnects == 0 && dest.UnlimitedMaxReconnects == 0 ?>No
            <?cs elif:dest.UnlimitedMaxReconnects != 0?>Yes <span class="configurationFillerText">(unlimited)</span>
            <?cs elif:dest.MaxReconnects > 0 && dest.UnlimitedMaxReconnects == 0 ?>Yes<span class="configurationFillerText">, with limit of</span><?cs /if ?>
         <?cs else ?>
            <select name="DstMaxReconnectsChoiceBox" onchange="toggleNumberOfReconnectsTextField();" onkeyup="toggleNumberOfReconnectsTextField();">
               <option value="never" <?cs if:dest.MaxReconnects == 0 ?>selected="selected"<?cs /if ?> >No</option>
               <option value="unlimited" <?cs if:dest.UnlimitedMaxReconnects != 0?>selected="selected"<?cs /if ?> >Yes (unlimited)</option>
               <option value="other"  <?cs if:dest.MaxReconnects > 0 && dest.UnlimitedMaxReconnects == 0 ?>selected="selected"<?cs /if ?>>Yes, with limit</option>
            </select>
         <?cs /if ?>
         <span id="DstMaxReconnectsText" style="font-size: 11px;<?cs if:dest.UnlimitedMaxReconnects != 0 || dest.MaxReconnects == 0 ?> display: none;<?cs /if ?>" >
            <?cs if:Channel.ReadOnlyMode ?>
               <?cs var:html_escape(dest.MaxReconnects) ?> <span>times</span>
            <?cs else ?>
               <span>of</span>
               <input type="text" class="number_field" name="DstMaxReconnects" id="DstMaxReconnectsInput"
                  value="<?cs if:dest.MaxReconnects == 0 ?>10<?cs else ?><?cs var:dest.MaxReconnects ?><?cs /if ?>" />
               <span>times</span>
               <span id="DstMaxReconnectsErrorMessageContainer" class="validation_error_message_container"></span>
               <script defer type="text/javascript">
                  VALregisterIntegerValidationFunction(
                     'DstMaxReconnectsInput', 'DstReconnectToDbRow', 'DstMaxReconnectsErrorMessageContainer',
                     function() {
                        var ReconnectOptionsComboBox = document.channel_data.DstMaxReconnectsChoiceBox;
                        return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value == "other";
                     },
                     showDestinationTab);
               </script>
            <?cs /if ?>
         </span>         
      </td>
   </tr>
   
   <tr class="selected" id="DstReconnectIntervalRow"<?cs if:dest.MaxReconnects == 0 && dest.UnlimitedMaxReconnects == 0 ?> style="display:none;"<?cs /if ?>>
      <td class="left_column">Reconnection interval<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
                  
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.ReconnectInterval) ?>
         <?cs else ?>
            <input class="number_field" type="text" name="DstReconnectInterval" id="DstReconnectIntervalInput"
               value="<?cs var:dest.ReconnectInterval ?>">
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction(
                  'DstReconnectIntervalInput', 'DstReconnectIntervalRow', 'DstReconnectIntervalErrorMessageContainer',
                  function() {
                     var ReconnectOptionsComboBox = document.channel_data.DstMaxReconnectsChoiceBox;
                     return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value != "never";
                  },
                  showDestinationTab);
            </script>
         <?cs /if ?>
         <span>milliseconds</span>
         <span id="DstReconnectIntervalErrorMessageContainer" class="validation_error_message_container"></span>

      </td>
   </tr>

   <tr class="selected" id="RowDestTimeout"
      <?cs if:Channel.ReadOnlyMode && dest.Database.Type != "ODBC - MS SQL Server" ?>
         style="display: none;"
      <?cs /if ?>
   >
      <td class="left_column">Database timeout<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.DatabaseTimeout) ?>
         <?cs else ?>
            <input class="configuration_smaller" name="DstDatabaseTimeout" id="DstDatabaseTimeoutInput"
               value="<?cs var:dest.DatabaseTimeout ?>">
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction
               (
                  'DstDatabaseTimeoutInput', 'RowDestTimeout', 'DstDatabaseTimeoutErrorMessageContainer',
                  function()
                  {
                     var TimeoutRow = document.getElementById('RowDestTimeout');
                     return WINgetStyle(TimeoutRow, 'display') != 'none';
                  },
                  showDestinationTab, 10, 25000
               );
            </script>
         <?cs /if ?>
         &nbsp;seconds
         <span id="DstDatabaseTimeoutErrorMessageContainer" class="validation_error_message_container"></span>
      </td>
   </tr>

<?cs /with ?>
