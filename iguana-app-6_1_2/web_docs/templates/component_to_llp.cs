<?cs # vim: set syntax=html :?>

<?cs include:"browse_macro.cs" ?>

<script defer type="text/javascript"> <!--

$(document).ready(function() {
   $('input[name="DstLlpStart"],input[name="DstLlpEnd"]').click(function() {
      $('#DstLlpCustom').click();
   });

   var Options = $('#DstEscapeOptions');
   $('#DstEscape').click(function() {
      if($(this).attr('checked'))
         Options.show();
      else
         Options.hide();
   });

   $('select[name="DstAckVerificationType"]').change(function() {
      var Value = $(this).val();
      console.log(Value);
      console.log(shouldShowAckRows());
      if(shouldShowAckRows()){
         $('#TrackVmdChangesDst').toggle(Value != "AnyMessage");
      }
   }).change();

   $("#DstHostPort").keyup(function() {
      expandFields(['DstHostPort']);
      VALclearError('DstHostPortRow', 'DstHostPortErrorMessageContainer');
   });

   VALregisterIntegerValidationFunction('DstHostPort', 'DstHostPortRow', 'DstHostPortErrorMessageContainer', null, showDestinationTab, 1, 65535, true);
   expandFields(['DstHostPort']);
   
});

function toggleNumberOfReconnectsTextField() {
   var ReconnectOptionsComboBox = document.channel_data.DstMaxReconnectsChoiceBox;
   var ReconnectsText = document.getElementById('DstMaxReconnectsText');
   var ReconnectIntervalRow = document.getElementById('DstReconnectIntervalRow');
   
   if (ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value == "other") {
      ReconnectsText.style.display="inline";
   } else {
      ReconnectsText.style.display="none";
   }
   if (ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value != "never") {
      ReconnectIntervalRow.style.display = "";
   } else {
      ReconnectIntervalRow.style.display = "none";
   }   
}

function toggleDstConnectionTimeoutTextField() {
   var PersistentConnectionComboBox = document.channel_data.DstPersistentConnectionChoiceBox;
   var ConnectionTimeoutText = document.getElementById('DstConnectionTimeoutText');

   if (PersistentConnectionComboBox.options[PersistentConnectionComboBox.selectedIndex].value == "disconnect_timeout") {
      ConnectionTimeoutText.style.display = "inline";
   } else {
      ConnectionTimeoutText.style.display = "none";
   }
}

function onAckVerificationTypeChange() {
   var AckVerificationTypeComboBox = document.channel_data.DstAckVerificationType;
   var AckVmdPathText = document.getElementById('DstVerifyAckVmdPath');
   var AckVmdTrackChanges = document.getElementById('TrackVmdChangesDst');
   var RetryOnVerificationFailCheckbox = document.getElementById('DstRetryOnAckVerificationFailedCheckbox');
   var ShowHideTrack = AckVerificationTypeComboBox.options[AckVerificationTypeComboBox.selectedIndex].value == "AnyMessage" ||
       !shouldShowAckRows();
   if (AckVerificationTypeComboBox.options[AckVerificationTypeComboBox.selectedIndex].value == "AnyMessage" ||
       !shouldShowAckRows())
   {
      AckVmdPathText.style.display = "none";
      AckVmdTrackChanges.style.display = "none";
      RetryOnVerificationFailCheckbox.disabled = "disabled";
      RetryOnVerificationFailCheckbox.checked = "";
      $("#DstRetryOnAckVerificationFailedLabel").css("opacity", "0.4");
   } else {
      AckVmdPathText.style.display = "";
      AckVmdTrackChanges.style.display = "";
      document.getElementById('TrackVmdChangesDst').style.display = "";
      RetryOnVerificationFailCheckbox.disabled = "";
      $("#DstRetryOnAckVerificationFailedLabel").css("opacity", "1");
   }
}

function shouldShowAckRows() {
   return document.getElementById('DstWaitForAckCheckbox').checked;
}

function showOrHideAckRows() {
   var TableBody = document.getElementById('destinationTabContents');
   var Rows = TableBody.getElementsByTagName('tr');
   
   var displayValue = shouldShowAckRows() ? '' : 'none';
   
   for (var RowIndex = 0; RowIndex < Rows.length; ++RowIndex) {
      if (CLSelementHasClass(Rows[RowIndex], "ack_row")) {
         Rows[RowIndex].style.display = displayValue;
      }
   }
   
   onAckVerificationTypeChange();
   showOrHideResendFields();
   enableOrDisableNumberOfRetries();
   showOrHideAckReconnectionInterval();
}

function showOrHideResendFields() {
   var displayStyle = (document.getElementById('DstRetryOnAckTimeoutCheckbox').checked ||
                      document.getElementById('DstRetryOnAckVerificationFailedCheckbox').checked) &&
                      shouldShowAckRows() ? '' : 'none';

   document.getElementById('ResendAttemptsRow').style.display = displayStyle;
   document.getElementById('DisconnectBetweenRetriesRow').style.display = displayStyle;
}

function enableOrDisableNumberOfRetries() {
   document.getElementById('DstNumberOfRetriesInput').disabled =
      document.getElementById('DstUnlimitedNumberOfRetriesRadioButton').checked ? '' : 'disabled';
}

function showOrHideAckReconnectionInterval() {
   document.getElementById('AckReconnectionIntervalSpan').style.display =
      document.getElementById('DstDisconnectBetweenRetriesCheckbox').checked ? '' : 'none';
}

function onLoadToLlp() {
   <?cs if:!Channel.ReadOnlyMode ?>
   showOrHideAckRows();
   onAckVerificationTypeChange();
   showOrHideResendFields();
   enableOrDisableNumberOfRetries();
   showOrHideAckReconnectionInterval();
   toggleDstConnectionTimeoutTextField();
   toggleNumberOfReconnectsTextField();
   showOrHideClientSslRows();
   expandFields(["DstHostAddress", "DstHostPort"]);
   <?cs /if ?>
}

function shouldShowClientSslRows() {
   return document.getElementById('DstInputUseSsl').checked;
}

function showOrHideClientSslRows() {
   var TableBody = document.getElementById('destinationTabContents');
   var Rows = TableBody.getElementsByTagName('tr');
   var displayValue = shouldShowClientSslRows() ? '' : 'none';   
   for (var RowIndex = 0; RowIndex < Rows.length; ++RowIndex)
   {
      if (CLSelementHasClass(Rows[RowIndex], "client_ssl_row"))
      {
         Rows[RowIndex].style.display = displayValue;
      }
   }
   showOrHideClientSslVerifyPeerRows();
}

function shouldShowClientSslVerifyPeerRows()
{
   return shouldShowClientSslRows() && document.getElementById('DstInputSslVerifyPeer').checked;
}

function showOrHideClientSslVerifyPeerRows() {
   var TableBody = document.getElementById('destinationTabContents');
   var Rows = TableBody.getElementsByTagName('tr');
   var displayValue = shouldShowClientSslVerifyPeerRows() ? '' : 'none';   
   for (var RowIndex = 0; RowIndex < Rows.length; ++RowIndex) {
      if (CLSelementHasClass(Rows[RowIndex], "client_ssl_verify_peer_row")) {
         Rows[RowIndex].style.display = displayValue;
      }
   }
}


--> </script>

<?cs with: dest = Channel.Destination ?>
   <tr class="selected">
      <td class="left_column">Host address<font color="#ff0000">*</font></td>
      <td class="inner_left">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.HostAddress) ?>
	         <?cs if:html_escape(dest.HostAddress) != html_escape(environment_expand(dest.HostAddress)) ?>
		         <span style="letter-spacing:-2px;">&ndash;&gt;&nbsp;</span>
	    	      <?cs var:html_escape(environment_expand(dest.HostAddress)) ?>
		      <?cs /if ?>
	 <?cs else ?>
       <input type="text" class="full_length" name="DstHostAddress" id="DstHostAddress" onkeyup="expandFields(['DstHostAddress']);" value="<?cs var:dest.HostAddress ?>" \>
            <div id="DstHostAddress_preview_div" style="display:none;">
               <div id="DstHostAddress_preview" class="path_preview"></div>
            </div>
	 <?cs /if ?>
	 <?cs if:Channel.Destination.Error.HostAddress ?>
	    <div class="configuration_error">
	       <?cs var:Channel.Destination.Error.HostAddress ?>
	    </div>
	 <?cs /if ?>
      </td>
   </tr>
   

   <tr class="selected" id="DstHostPortRow">
      <td class="left_column">Port<font color="#ff0000">*</font></td>
      <td class="inner_left">
      <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.HostPort) ?>
            <?cs if:html_escape(dest.HostPort) != html_escape(environment_expand(dest.HostPort)) ?>
               <span style="letter-spacing:-2px;">&ndash;&gt;&nbsp;</span>
               <?cs var:html_escape(environment_expand(dest.HostPort)) ?>
            <?cs /if ?>
     <?cs else ?>
       <input type="text" class="full_length" name="DstHostPort" id="DstHostPort" value="<?cs var:dest.HostPort ?>" \>
       <div id="DstHostPort_preview_div" style="display:none;">
          <div id="DstHostPort_preview" class="path_preview"></div>
       </div>
       <span id="DstHostPortErrorMessageContainer" class="configuration_error validation_error_message_container" style="white-space:normal"></span>
     <?cs /if ?>
     <?cs if:Channel.Destination.Error.HostPort ?>
        <div class="configuration_error">
           <?cs var:Channel.Destination.Error.HostPort ?>
        </div>
     <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected">
      <td class="left_column">Message encoding</td>
      <td class="inner" colspan="3">
         <table class="inner" Style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if: Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(dest.Encoding.Description) ?>
                     <br><div style="padding:3px"></div>
                     <?cs if:!dest.Encoding.EscapeNonAscii ?>
                        8-bit characters will not be escaped
                     <?cs elif:!dest.Encoding.EscapeChar ?>
                        8-bit characters of HL7 messages will be escaped
                     <?cs else ?>
                        8-bit characters will be escaped with
                        "<?cs var:html_escape(dest.Encoding.EscapeChar) ?>"
                     <?cs /if ?>
                  <?cs else ?>
                     <select name="DstEncoding">
                        <option value="<?cs var:html_escape(dest.Encoding.Name) ?>" selected>
                           <?cs var:html_escape(dest.Encoding.Description) ?>
                        </option>
                        <option value="<?cs var:html_escape(dest.Encoding.Name) ?>">
                           ---
                        </option>
                        <?cs each:encoding = AllEncodings ?>
                           <?cs if: encoding.Name        != dest.Encoding.Name
                                 || encoding.Description != dest.Encoding.Description ?>
                              <option value="<?cs var:html_escape(encoding.Name) ?>">
                                 <?cs var:html_escape(encoding.Description) ?>
                              </option>
                           <?cs /if ?>
                        <?cs /each ?>
                     </select>
                     <br>
                     <input type="checkbox" id="DstEscape" name="DstEscape"
                        <?cs if:dest.Encoding.EscapeNonAscii ?>checked<?cs /if ?>>
                     <label for="DstEscape">Escape 8-bit characters (HL7)</label>
                     <a class="helpIcon" style="margin-left:5px; position:relative;"
                        title="More Information" href="#" rel="When sending messages out,
                        non-ASCII characters, like accented characters, can be escaped.
                        E.g., <nobr>&quot;&#233;&quot;</nobr> would be sent as
                        <nobr>&quot;\XE9\&quot;</nobr> (in <nobr>Latin-1</nobr> or
                        <nobr>Windows-1252</nobr>) or <nobr>&quot;\XC3\\XA9\&quot;</nobr>
                        (in <nobr>UTF-8</nobr>)."
                        ><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0"></a>
                     <div id="DstEscapeOptions" style="margin-left:2em; 
                              <?cs if:!dest.Encoding.EscapeNonAscii ?>display:none;<?cs /if ?>">
                        <input type="radio" name="DstEscapeMethod" id="DstEscapeAuto" value="auto"
                           <?cs if:!dest.Encoding.EscapeChar ?>checked<?cs /if ?>>
                        <label for="DstEscapeAuto">Automatically detect escape character</label>
                        <br>
                        <input type="radio" name="DstEscapeMethod" id="DstEscapeSpec" value="spec"
                           <?cs if: dest.Encoding.EscapeChar ?>checked<?cs /if ?>>
                        <label for="DstEscapeSpec">Always escape with
                           <input name="DstEscapeChar" size="1"
                                  value="<?cs alt:dest.Encoding.EscapeChar ?>\<?cs /alt ?>"></label>
                     </div>
                  <?cs /if ?>
               </td>               
               <td class="inner_right">
                  <?cs if:Channel.Destination.Error.Encoding ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Destination.Error.Encoding ?></ul>
                     </div>
                  <?cs /if ?>
                  <?cs if:Channel.Destination.Error.Escaping ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Destination.Error.Escaping ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>
         </table>
      </td>
   </tr>

   <?cs if: Channel.ReadOnlyMode ?>
      <tr class="selected">
         <td class="left_column">LLP delimiters
         <td class="inner_left" colspan="3">
            <?cs if:dest.LlpNormal ?> Normal LLP:
            <?cs else ?>              Custom LLP: 
            <?cs /if ?>
            <span class="configurationFillerText">messages begin after</span>
               "<?cs var:html_escape(dest.LlpStart) ?>"
            <span class="configurationFillerText">and continue until</span>
               "<?cs var:html_escape(dest.LlpEnd) ?>"
      </tr>
   <?cs else ?>
      <tr class="selected">
         <td class="left_column">LLP delimiters
         <td class="inner_left" colspan="3">
            <input type="radio" id="DstLlpNormal" name="DstLlpNormal" value="normal" <?cs if:dest.LlpNormal ?>checked<?cs /if ?>>
               <label for="DstLlpNormal">Normal LLP</label><br>
            <input type="radio" id="DstLlpCustom" name="DstLlpNormal" value="custom" <?cs if:!dest.LlpNormal ?>checked<?cs /if ?>>
               <label for="DstLlpCustom">Messages begin after</label>
               <input type="text" style="width:80px" name="DstLlpStart" value="<?cs var:html_escape(dest.LlpStart) ?>">
               <label for="DstLlpCustom">and continue until</label>
               <input type="text" style="width:80px" name="DstLlpEnd" value="<?cs var:html_escape(dest.LlpEnd) ?>">
               <a class="helpIcon" rel="These delimiters will be expected around each message sent
                     over LLP.  Any non-empty strings can be specified.  Non-printable characters must
                     be escaped with a backslash: e.g., \x0D or \r (C/Java-style escapes)."
                  title="Custom LLP Delimiters" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a> 
      </tr>
   <?cs /if ?>
   
   <tr class="channel_configuration_section_heading_row">
      <td>
         <table>
            <tr>
               <td style="width:100%;"><hr /></td>
               <td class="channel_configuration_section_heading">ACKnowledgment Settings</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>
   
   <tr class="selected">
      <td class="left_column first_row">Wait for ACK</td>
      <td class="inner_left first_row" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:dest.WaitForAck ?>Yes<?cs else ?>No<?cs /if ?>
         <?cs else ?>
            <input type="checkbox" name="DstWaitForAck" id="DstWaitForAckCheckbox"
               onclick="showOrHideAckRows();"
               <?cs if:dest.WaitForAck ?>checked="checked"<?cs /if ?> />
         <?cs /if ?>
      </td>
   </tr>
   
   
   
   <tr class="selected ack_row" id="DstAckTimeoutRow" <?cs if:!dest.WaitForAck ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">ACK timeout<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.AckTimeout) ?>
         <?cs else ?>
            <input type="text" class="number_field" name="DstAckTimeout" id="DstAckTimeoutInput"
               value="<?cs var:dest.AckTimeout ?>" />
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction('DstAckTimeoutInput', 'DstAckTimeoutRow', 'DstAckTimeoutErrorMessageContainer', shouldShowAckRows, showDestinationTab);
            </script>
         <?cs /if ?>
         milliseconds
         <span id="DstAckTimeoutErrorMessageContainer" class="validation_error_message_container"></span>
      </td>
   </tr>
   
   <tr class="selected ack_row" <?cs if:!dest.WaitForAck ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">ACK verification</td>
      <td class="inner_left" colspan="3">
          <?cs if:Channel.ReadOnlyMode ?>
            <?cs def:ack_type_readonly(value,text) ?>
               <?cs if: dest.AckVerificationType == value ?>
                  <?cs var:text ?>
               <?cs /if ?>
            <?cs /def ?>
	    <?cs call:ack_type_readonly('AnyMessage',  'Any message') ?>
            <?cs call:ack_type_readonly('AnyACK',      'Any ACK message') ?>
            <?cs call:ack_type_readonly('VerifiedACK', 'Verified ACK message') ?>
     <?cs else ?>
       <select name="DstAckVerificationType" class="configuration" onchange="onAckVerificationTypeChange();">
            <?cs def:ack_type(value,text) ?>
               <?cs if: dest.AckVerificationType == value ?>
                  <option value="<?cs var:value ?>" selected>  <?cs var:text ?>  </option>
               <?cs else ?>
                  <option value="<?cs var:value ?>">  <?cs var:text ?>  </option>
               <?cs /if ?>
            <?cs /def ?>
            <?cs call:ack_type('AnyMessage',  'Any message') ?>
            <?cs call:ack_type('AnyACK',      'Any ACK message') ?>
            <?cs call:ack_type('VerifiedACK', 'Verified ACK message') ?>
         </select>
	<?cs /if ?>
      </td>
   </tr>

   <tr class="selected ack_row" id="DstVerifyAckVmdPath" <?cs if:dest.AckVerificationType == 'AnyMessage' || !dest.WaitForAck ?>style="display: none;"<?cs /if ?>
      <?cs if:!dest.WaitForAck ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">ACK VMD Path<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
	 <?cs if:Channel.ReadOnlyMode ?>
         <?cs if:Channel.Destination.Error.AckConsVmdPath ?>
          <div class="configuration_error">
             <?cs var:Channel.Destination.Error.AckConsVmdPath ?>
               </div>
        <?cs else ?> 
            <?cs if:dest.AckVmdPath != dest.OriginalVmdPath?>
            <?cs /if ?>
            <?cs if:dest.OriginalVmdPath != "" ?>
                  <label>Uploaded from: </label><br><?cs call:browse_readonly(dest.OriginalVmdPath) ?>
              <?cs /if ?>
              <?cs if:dest.AckVmdPath != "" ?>
                 <br><br><?cs call:download_vmd(Channel.Guid, Channel.editTab, Channel.Name, "DstAckVmd") ?>
              <?cs /if ?>
            <?cs /if ?>
	 <?cs else ?>
      <?cs if:! Channel.Destination.Error.AckConsVmdPath && dest.OriginalVmdPath != ""?>
         <div class="original_vmd">
            <label>Uploaded from: </label><br><?cs call:browse_readonly(dest.OriginalVmdPath) ?>
         </div><br>
      <?cs /if ?>
      <?cs call:browse_input('DstAckVmdPath', dest.AckVmdPath) ?>
      <?cs if:! Channel.Destination.Error.AckConsVmdPath && dest.OriginalVmdPath != ""?>
      <br><a id="useOriginalToLLP"><u>Use previously uploaded VMD path</u></a>
      <script type="text/javascript">
         $("#useOriginalToLLP").click(function(){
            $("#DstAckVmdPath").val("<?cs var:dest.CleanedOriginal ?>");
         });
      </script> 
      <?cs /if ?>
      <?cs if:dest.AckVmdPath != "" ?>
        <br><br><br><?cs call:download_vmd(Channel.Guid, Channel.editTab, Channel.Name, "DstAckVmd") ?>
        <a id="DownloadVmdBox" style="padding-left: 4px;" class="helpIcon" tabindex="101" rel="Download the currently running VMD from source control." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
     <?cs /if ?> 
	 <?cs /if ?>
      </td>
   </tr>

    <tr class="selected ack_row" id="TrackVmdChangesDst" <?cs if:dest.AckVerificationType == 'AnyMessage' || !dest.WaitForAck ?>style="display: none;"<?cs /if ?>
      <?cs if:!dest.WaitForAck ?>style="display:none;"<?cs /if ?>>
         <td class="left_column">Track VMD Changes</td>
         <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
            <table class="inner">
               <tr>
                  <td class="inner_left">
                     <?cs if:Channel.ReadOnlyMode ?> 
                        <?cs if:dest.KeepUpdated ?>Yes<?cs else ?>No<?cs /if ?>          
                     <?cs else ?>
                        <br><?cs call:keep_updated("UpdatedAckDestVmd",dest.KeepUpdated) ?>   
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
   
   <tr class="selected ack_row" <?cs if:!dest.WaitForAck ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">Attempt to resend message</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:!dest.RetryOnAckTimeout && !dest.RetryOnAckVerificationFailed ?>
               Never
            <?cs else ?>
               <?cs if:dest.RetryOnAckTimeout ?>When ACK times out<?cs /if ?>
               <?cs if:dest.RetryOnAckTimeout && dest.RetryOnAckVerificationFailed ?><br /><?cs /if ?>
               <?cs if:dest.RetryOnAckVerificationFailed ?>When ACK verification fails<?cs /if ?>
            <?cs /if ?>
         <?cs else ?>
            <input type="checkbox" name="DstRetryOnAckTimeout" id="DstRetryOnAckTimeoutCheckbox"
                onclick="showOrHideResendFields();"
                <?cs if:dest.RetryOnAckTimeout ?>checked="checked"<?cs /if ?> />
            When ACK times out
            <br />
            <input type="checkbox" name="DstRetryOnAckVerificationFailed" id="DstRetryOnAckVerificationFailedCheckbox"
                onclick="showOrHideResendFields();"
                <?cs if:dest.RetryOnAckVerificationFailed ?>checked="checked"<?cs /if ?>
                <?cs if:dest.AckVerificationType == "AnyMessage" ?>disabled="disabled"<?cs /if ?> />
            <span id="DstRetryOnAckVerificationFailedLabel" <?cs if:dest.AckVerificationType == "AnyMessage" ?> style="opacity: 0.4;"<?cs /if ?>>When ACK verification fails</span>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected ack_row" id="ResendAttemptsRow" <?cs if:!dest.WaitForAck || !(dest.RetryOnAckTimeout || dest.RetryOnAckVerificationFailed) ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">Number of resend attempts</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:dest.UnlimitedNumberOfRetries ?>Unlimited
            <?cs else ?><?cs var:html_escape(dest.NumberOfRetries) ?><?cs /if ?>
         <?cs else ?>
            <input type="radio" name="DstUnlimitedNumberOfRetries" value="true"
               onclick="enableOrDisableNumberOfRetries();"
               <?cs if:dest.UnlimitedNumberOfRetries ?>checked="checked"<?cs /if ?> />
            Unlimited
            <br />
            <input type="radio" name="DstUnlimitedNumberOfRetries" value="false"
               id="DstUnlimitedNumberOfRetriesRadioButton"
               onclick="enableOrDisableNumberOfRetries();"
               <?cs if:!dest.UnlimitedNumberOfRetries ?>checked="checked"<?cs /if ?> />
            <input class="number_field" type="text" name="DstNumberOfRetries" id="DstNumberOfRetriesInput"
               value="<?cs var:dest.NumberOfRetries ?>" /> &nbsp;times
            <span id="DstNumberOfRetriesErrorMessageContainer" class="validation_error_message_container"></span>
            <script defer type="text/javascript">
                VALregisterIntegerValidationFunction
                (
                   'DstNumberOfRetriesInput', 'ResendAttemptsRow', 'DstNumberOfRetriesErrorMessageContainer',
                   function()
                   {
                      return shouldShowAckRows() && document.getElementById('DstUnlimitedNumberOfRetriesRadioButton').checked;
                   },
                   showDestinationTab
                );
            </script>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected ack_row" id="DisconnectBetweenRetriesRow" <?cs if:!dest.WaitForAck || !(dest.RetryOnAckTimeout || dest.RetryOnAckVerificationFailed) ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">Disconnect between resend attempts</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:dest.DisconnectBetweenRetries ?>Yes<span class="configurationFillerText">, reconnect after</span> <?cs var:html_escape(dest.AckReconnectInterval) ?> milliseconds
            <?cs else ?>No<?cs /if ?>
         <?cs else ?>
            <input type="checkbox" name="DstDisconnectBetweenRetries" id="DstDisconnectBetweenRetriesCheckbox"
               onclick="showOrHideAckReconnectionInterval();"
               <?cs if:dest.DisconnectBetweenRetries ?>checked="checked"<?cs /if ?> />
            <span id="AckReconnectionIntervalSpan">
               &nbsp;Reconnect after
               <input type="text" class="number_field" name="DstAckReconnectInterval" id="DstAckReconnectIntervalInput"
                  value="<?cs var:dest.AckReconnectInterval ?>"> milliseconds
               <span id="DstAckReconnectIntervalErrorMessageContainer" class="validation_error_message_container"></span>
               <script defer type="text/javascript">
                  VALregisterIntegerValidationFunction
                  (
                     'DstAckReconnectIntervalInput', 'DisconnectBetweenRetriesRow', 'DstAckReconnectIntervalErrorMessageContainer',
                     function()
                     {
                        return shouldShowAckRows() && document.getElementById('DstDisconnectBetweenRetriesCheckbox').checked;
                     },
                     showDestinationTab
                  );
               </script>
            </span>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="selected ack_row" id="ErrorHandlingRow" <?cs if:!dest.WaitForAck ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">ACK error handling</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:dest.AckErrorHandling == 0 ?>Skip message
            <?cs else ?>Stop channel<?cs /if ?>
         <?cs else ?>
            <select class="configuration" name="DstAckErrorHandling">
               <option value="0" <?cs if:dest.AckErrorHandling == 0 ?>selected<?cs /if ?> > Skip message </option>
               <option value="1" <?cs if:dest.AckErrorHandling == 1 ?>selected<?cs /if ?> > Stop channel </option>
            </select>
            <a id="DstAckErrorHandling_Icon" class="helpIcon" tabindex="100" rel="This is the action Iguana will take if the ACK times out or if ACK verification fails, and any subsequent retry attempts also fail." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
         <?cs /if ?>
      </td>
   </tr>
   
   <tr class="channel_configuration_section_heading_row">
      <td>
         <table>
            <tr>
               <td style="width:100%;"><hr /></td>
               <td class="channel_configuration_section_heading">Connection Settings</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>
   
   <tr class="selected" id="DstPersistentConnectionRow">
      <td class="left_column first_row">Persistent connection?</td>
      <td class="inner_left first_row" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?>
                     <?cs if:dest.PersistentConnection ?>Yes<?cs /if ?>
                     <?cs if:!dest.PersistentConnection && (dest.ConnectionTimeout == 0) ?>No, disconnect after every message.<?cs /if ?>
                     <?cs if:!dest.PersistentConnection && (dest.ConnectionTimeout != 0) ?>No, disconnect <span class="configurationFillerText">after</span><?cs /if ?>
                     <?cs else ?>
                        <select name="DstPersistentConnectionChoiceBox" onchange="toggleDstConnectionTimeoutTextField();" onkeyup="toggleDstConnectionTimeoutTextField();">
                        <option value="persistent" <?cs if:dest.PersistentConnection ?>selected="selected"<?cs /if ?> >Yes</option>
                        <option value="disconnect_immediately" <?cs if:!dest.PersistentConnection && (dest.ConnectionTimeout == 0) ?>selected="selected"<?cs /if ?> >No, disconnect after every message.</option>
                        <option value="disconnect_timeout" <?cs if:!dest.PersistentConnection && (dest.ConnectionTimeout != 0) ?>selected="selected"<?cs /if ?>>No, disconnect after a timeout</option>
                     </select>
                  <?cs /if ?>
                  <span id="DstConnectionTimeoutText" style="font-size: 11px;<?cs if:dest.PersistentConnection || (!dest.PersistentConnection && (dest.ConnectionTimeout == 0)) ?> display: none;<?cs /if ?>" >
                     <?cs if:Channel.ReadOnlyMode ?>
                        <?cs var:html_escape(dest.ConnectionTimeout) ?> milliseconds <span class="configurationFillerText">of inactivity</span>
                     <?cs else ?>
                        of&nbsp;
                        <input class="configuration_smaller" name="DstConnectionTimeout" id="DstConnectionTimeoutInput"
                           value="<?cs var:dest.ConnectionTimeout ?>" />
                        &nbsp;milliseconds of inactivity.
                        <span id="DstConnectionTimeoutErrorMessageContainer" class="validation_error_message_container"></span>
                        <script defer type="text/javascript">
                           VALregisterIntegerValidationFunction
                           (
                              'DstConnectionTimeoutInput', 'DstPersistentConnectionRow', 'DstConnectionTimeoutErrorMessageContainer',
                              function()
                              {
                                 var PersistentConnectionComboBox = document.channel_data.DstPersistentConnectionChoiceBox;
                                 return PersistentConnectionComboBox.options[PersistentConnectionComboBox.selectedIndex].value == "disconnect_timeout";
                              },
                              showDestinationTab
                           );
                        </script>
                     <?cs /if ?>
                  </span>
               </td>
            </tr>
         </table>
      </td>
   </tr>
   
   <tr class="selected" id="DstAttemptToReconnectRow">
      <td class="left_column">Attempt to reconnect?</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:!dest.UnlimitedMaxReconnects && (dest.MaxReconnects == 0) ?>No<?cs /if ?>
            <?cs if:dest.UnlimitedMaxReconnects ?>Yes <span class="configurationFillerText">(unlimited)</span><?cs /if ?>
            <?cs if:!dest.UnlimitedMaxReconnects && (dest.MaxReconnects != 0) ?>Yes<span class="configurationFillerText">, with limit of</span><?cs /if ?>
         <?cs else ?>
            <select name="DstMaxReconnectsChoiceBox" onchange="toggleNumberOfReconnectsTextField();" onkeyup="toggleNumberOfReconnectsTextField();">
               <option value="never" <?cs if:!dest.UnlimitedMaxReconnects && (dest.MaxReconnects == 0) ?>selected="selected"<?cs /if ?> >No</option>
               <option value="unlimited" <?cs if:dest.UnlimitedMaxReconnects ?>selected="selected"<?cs /if ?> >Yes (unlimited)</option>
               <option value="other"  <?cs if:!dest.UnlimitedMaxReconnects && (dest.MaxReconnects != 0) ?>selected="selected"<?cs /if ?>>Yes, with limit</option>
            </select>
         <?cs /if ?>
         <span id="DstMaxReconnectsText" style="font-size: 11px;<?cs if:dest.UnlimitedMaxReconnects ?> display: none;<?cs /if ?><?cs if:dest.MaxReconnects == 0 ?> display: none;<?cs /if ?>" >
            <?cs if:Channel.ReadOnlyMode ?>
               <?cs var:html_escape(dest.MaxReconnects) ?> times
            <?cs else ?>
               of&nbsp;
               <input type="text" class="number_field" name="DstMaxReconnects" id="DstMaxReconnectsInput"
                  value="<?cs var:dest.MaxReconnects ?>" />
               <script defer type="text/javascript">
                  VALregisterIntegerValidationFunction
                  (
                     'DstMaxReconnectsInput', 'DstAttemptToReconnectRow', 'DstAttemptToReconnectErrorMessageContainer',
                     function()
                     {
                        var ReconnectOptionsComboBox = document.channel_data.DstMaxReconnectsChoiceBox;
                        return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value == "other";
                     },
                     showDestinationTab
                  );
               </script>
               &nbsp;times&nbsp;
               <a id="configuration_smaller_Icon" class="helpIcon" tabindex="100" rel="This option allows you to specify what action should be taken if the connection to the host is lost unexpectedly." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
            <?cs /if ?>
            <span id="DstAttemptToReconnectErrorMessageContainer" class="validation_error_message_container"></span>
         </span>
   
         <?cs if:Channel.Destination.Error.MaxCountOfReconnect ?>
            <div class="configuration_error">
         <?cs var:Channel.Destination.Error.MaxCountOfReconnect ?>
            </div>
         <?cs /if ?>
          
      </td>
   </tr>
   
   <tr id="DstReconnectIntervalRow" style="<?cs if:!dest.UnlimitedMaxReconnects && (dest.MaxReconnects == 0) ?>display: none;<?cs /if ?>">
      <td class="left_column">Reconnection interval<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs var:html_escape(dest.ConnectionLostReconnectInterval) ?> milliseconds
         <?cs else ?>
            <input type="text" class="number_field" name="DstConnectionLostReconnectInterval" id="DstConnectionLostReconnectIntervalInput"
               value="<?cs var:dest.ConnectionLostReconnectInterval ?>">
            &nbsp;milliseconds
            <span id="DstConnectionLostReconnectIntervalErrorMessageContainer" class="validation_error_message_container"></span>
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction
               (
                  'DstConnectionLostReconnectIntervalInput', 'DstReconnectIntervalRow', 'DstConnectionLostReconnectIntervalErrorMessageContainer',
                  function()
                  {
                     var ReconnectOptionsComboBox = document.channel_data.DstMaxReconnectsChoiceBox;
                     return ReconnectOptionsComboBox.options[ReconnectOptionsComboBox.selectedIndex].value != "never";
                  },
                  showDestinationTab
               );
            </script>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="channel_configuration_section_heading_row">
      <td>
         <table>
            <tr>
               <td style="width:100%;"><hr /></td>
               <td class="channel_configuration_section_heading">SSL Settings</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>

   <tr class="selected" id="DstUseSslRow">
      <td class="left_column first_row">Use SSL</td>
      <td class="inner_left first_row" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?> 
                     <?cs if:dest.UseSsl ?>Yes<?cs else ?>No<?cs /if ?>          
                  <?cs else ?>
                     <input type="checkbox" class="no_style" id="DstInputUseSsl" name="DstUseSsl" <?cs if:dest.UseSsl ?>checked<?cs /if ?> onclick="showOrHideClientSslRows();" />&nbsp;      
                  <?cs /if ?>
               </td>
               <td class="inner_right">
                  <?cs if:Channel.Destination.Error.Ssl ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Destination.Error.Ssl ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>     
         </table>
      </td>
   </tr>

   <tr class="selected client_ssl_row" id="DstSslCertificateKeyFileRow" <?cs if:!dest.UseSsl ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">Certificate file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.SslCertificateKeyFile) ?>
            <?cs if:dest.SslCertificateKeyFile!='' ?>
               <a href="/view_ssl_cert.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&ComponentType=Destination">view certificate</a>
            <?cs /if ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('DstSslCertificateKeyFile', dest.SslCertificateKeyFile) ?>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected client_ssl_row" id="DstSslPrivateKeyFileRow" <?cs if:!dest.UseSsl ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">Private key file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.SslPrivateKeyFile) ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('DstSslPrivateKeyFile', dest.SslPrivateKeyFile) ?>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected client_ssl_row" id="DstSslVerifyPeerRow" <?cs if:!dest.UseSsl ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">Verify peer</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode ?> 
            <?cs if:dest.SslVerifyPeer ?>Yes<?cs else ?>No<?cs /if ?>          
         <?cs else ?>
            <input type="checkbox" class="no_style" id="DstInputSslVerifyPeer" name="DstSslVerifyPeer" <?cs if:dest.SslVerifyPeer ?>checked<?cs /if ?> onclick="showOrHideClientSslVerifyPeerRows();" />&nbsp;      
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected client_ssl_row client_ssl_verify_peer_row" id="DstSslCertificateAuthorityFileRow" <?cs if:!dest.UseSsl || !dest.SslVerifyPeer ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">Certificate authority file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(dest.SslCertificateAuthorityFile) ?>
            <?cs if:dest.SslCertificateAuthorityFile!='' ?>
               <a href="/view_ssl_cert.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&CertificateType=Authority&ComponentType=Destination">view certificate</a>
            <?cs /if ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('DstSslCertificateAuthorityFile', dest.SslCertificateAuthorityFile) ?>
         <?cs /if ?>
      </td>
   </tr>

<?cs /with ?>
