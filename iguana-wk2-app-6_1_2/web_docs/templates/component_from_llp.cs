<?cs # vim: set syntax=html :?>

<?cs include:"browse_macro.cs" ?>
<?cs include:"mapper_config.cs" ?>

<script defer type="text/javascript"> <!--

$(document).ready(function() {
   var Options = $('#SrcEscapeOptions');
   $('#SrcEscape').click(function() {
      if($(this).attr('checked'))
         Options.show();
      else
         Options.hide();
   });

   $('input[name="SrcLlpStart"],input[name="SrcLlpEnd"]').click(function() {
      $('#SrcLlpCustom').click();
   });

   $('select[name="SrcAckStyle"]').change(function() {
      var Value = $(this).val();
      $('#AckVmdPathRowId').toggle(Value == 'vmd');
      $('#TrackVmdChangesSrc').toggle(Value == 'vmd');
      $('.AckTransRow').toggle(Value == 'trans');
   }).change();

   $("#SrcPort").keyup(function() {
      expandFields(['SrcPort']);
      VALclearError('SrcPortRow', 'SrcPortErrorMessageContainer');
   });

   VALregisterIntegerValidationFunction('SrcPort', 'SrcPortRow', 'SrcPortErrorMessageContainer', null, showSourceTab, 1, 65535, true);

   expandFields(['SrcPort']);
   checkForPortConflicts();
});

function handlePortConflictResponse(OriginalPort, ResolvedPort, ConflictResponseData) {
   var PortContainer = document.getElementById('SrcPortErrorMessageContainer');
   var PortErrorMessage = '&nbsp;<br><img src="<?cs var:skin("/images/icon_warning.gif") ?>" /> <span style="color:black;"><b>Warning: </b>'; 

   if (ConflictResponseData.indexOf("between 1 and 65535") < 0) {
      if (OriginalPort == ResolvedPort) {
         PortErrorMessage += 'This use of port ' + ResolvedPortText;
      }
      else {
         PortErrorMessage += 'The use of ' + OriginalPort + ' (which resolves to port ' + ResolvedPort + ')';
      }

      PortErrorMessage += ' conflicts with ' + ConflictResponseData;
   } 
   else {
      PortErrorMessage += ConflictResponseData;
   }

   PortErrorMessage += '</span>';
   PortContainer.innerHTML =  PortErrorMessage;
}

function checkForPortConflicts() {
   var UserPort    = document.getElementById('SrcPort');
   var UserPortValue = UserPort ? UserPort.value.trim() : "";

   <?cs if: Channel.ReadOnlyMode ?>
      var Shared        = '<?cs var:#Channel.Source.CheckRemoteHost ?>';
      var RemoteHost    = encodeURIComponent('<?cs var:Channel.Source.RemoteHost.ConfiguredName ?>');
      var AlternateHost = encodeURIComponent('<?cs var:Channel.Source.AlternateRemoteHost.ConfiguredName ?>');
   <?cs else ?>
      var Shared        = document.getElementById('src_check_remote_host').checked ? '1' : '0';
      var RemoteHost    = encodeURIComponent(document.getElementById('src_remote_host').value);
      var AlternateHost = encodeURIComponent(document.getElementById('src_alternate_remote_host').value);
   <?cs /if ?>
  
   var ChannelName = encodeURIComponent("<?cs var:js_escape(Channel.Name) ?>");

   AJAXpost('check_for_port_conflicts', 'Port=' + UserPortValue + '&Channel=' + ChannelName +
         '&Shared=' + Shared + '&RemoteHost=' + RemoteHost + '&AlternateHost=' + AlternateHost,
      function(data) {
         if (data == '') {
            VALclearError('SrcPortRow', 'SrcPortErrorMessageContainer')
         } 
         else {
            var Unexpanded = encodeURIComponent(UserPortValue);
            $.ajax({
               method:  "POST",
               url:     "environment_expand",
               data:    "RawText=" + Unexpanded,
               success: function(Response) {
                  handlePortConflictResponse(UserPortValue, Response.expandedText, data);
               },
               error: function() {
                  handlePortConflictResponse(UserPortValue, UserPortValue, data); 
               }
            });
         }
      },
      function() {
         VALclearError('SrcPortRow', 'SrcPortErrorMessageContainer')
      }
   );
}

function toggleSrcConnectionTimeoutTextField() {
   var ConnectionTimeoutCheckBox = document.getElementById('inputUseConnectionTimeout');
   var TimeoutText = document.getElementById('SrcConnectionTimeout');
   
   if(ConnectionTimeoutCheckBox.checked)
   {
      TimeoutText.style.display="inline";
   }
   else
   {
      TimeoutText.style.display="none";
   }
}

function onLoadFromLlp() {
   checkForPortConflicts();
   <?cs if:!Channel.ReadOnlyMode ?>
      expandFields(["src_remote_host", "src_alternate_remote_host"]);
      showOrHideRemoteHostRows();
      showOrHideListenerSslRows();
   <?cs /if ?>
}

function showOrHideRemoteHostRows() {
   var Display = document.getElementsByName('SrcCheckRemoteHost')[0].checked ? '' : 'none';
   var Rows = document.getElementById('sourceTabContents').getElementsByTagName('tr');
   for(var RowIndex in Rows) {
      var Row = Rows[RowIndex];
      if(CLSelementHasClass(Row, 'remote_host_row')) {
         Row.style.display = Display;
      }
   }
}

function shouldShowListenerSslRows() {
   return document.getElementById('SrcInputUseSsl').checked;
}

function showOrHideListenerSslRows() {
   var TableBody = document.getElementById('sourceTabContents');
   var Rows = TableBody.getElementsByTagName('tr');
   var displayValue = shouldShowListenerSslRows() ? '' : 'none';   
   for (var RowIndex = 0; RowIndex < Rows.length; ++RowIndex) {
      if (CLSelementHasClass(Rows[RowIndex], "listener_ssl_row")) {
         Rows[RowIndex].style.display = displayValue;
      }
   }
   showOrHideListenerSslVerifyPeerRows();
}

function shouldShowListenerSslVerifyPeerRows() {
   return shouldShowListenerSslRows() && document.getElementById('SrcInputSslVerifyPeer').checked;
}

function showOrHideListenerSslVerifyPeerRows() {
   var TableBody = document.getElementById('sourceTabContents');
   var Rows = TableBody.getElementsByTagName('tr');
   var displayValue = shouldShowListenerSslVerifyPeerRows() ? '' : 'none';   
   for (var RowIndex = 0; RowIndex < Rows.length; ++RowIndex) {
      if (CLSelementHasClass(Rows[RowIndex], "listener_ssl_verify_peer_row"))
      {
         Rows[RowIndex].style.display = displayValue;
      }
   }
}

--> </script>

<style  type="text/css">

   .spanConnectionTimeout {
      font-size: 11px; 
   }

</style>

<?cs with: source = Channel.Source ?>
   <tr class="selected">
      <td class="left_column">Message encoding</td>
      <td class="inner" colspan="3">
         <table class="inner" Style="float:left">
            <tr>
               <td class="inner_left">
                  <?cs if: Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(source.Encoding.Description) ?>
                     <br><div style="padding:3px"></div>
                     <?cs if:!source.Encoding.EscapeNonAscii ?>
                        8-bit characters in ACKs will not be escaped
                     <?cs elif:!source.Encoding.EscapeChar ?>
                        8-bit characters of HL7 ACKs will be escaped
                     <?cs else ?>
                        8-bit characters in ACKs will be escaped with
                        "<?cs var:html_escape(source.Encoding.EscapeChar) ?>"
                     <?cs /if ?>
                  <?cs else ?>
                     <select name="SrcEncoding">
                        <option value="<?cs var:html_escape(source.Encoding.Name) ?>" selected>
                           <?cs var:html_escape(source.Encoding.Description) ?>
                        </option>
                        <option value="<?cs var:html_escape(source.Encoding.Name) ?>">
                           ---
                        </option>
                        <?cs each:encoding = AllEncodings ?>
                           <?cs if: encoding.Name        != source.Encoding.Name
                                 || encoding.Description != source.Encoding.Description ?>
                              <option value="<?cs var:html_escape(encoding.Name) ?>">
                                 <?cs var:html_escape(encoding.Description) ?>
                              </option>
                           <?cs /if ?>
                        <?cs /each ?>
                     </select>
                     <br>
                     <input type="checkbox" id="SrcEscape" name="SrcEscape"
                        <?cs if:source.Encoding.EscapeNonAscii ?>checked<?cs /if ?>>
                     <label for="SrcEscape">Escape 8-bit characters in ACKs (HL7)</label>
                     <a class="helpIcon" style="margin-left:5px; position:relative;"
                        title="More Information" href="#" rel="When sending back ACKs,
                        non-ASCII characters, like accented characters, can be escaped.
                        E.g., <nobr>&quot;&#233;&quot;</nobr> would be sent as
                        <nobr>&quot;\XE9\&quot;</nobr> (in <nobr>Latin-1</nobr> or
                        <nobr>Windows-1252</nobr>) or <nobr>&quot;\XC3\\XA9\&quot;</nobr>
                        (in <nobr>UTF-8</nobr>)."
                        ><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0"></a>
                     <div id="SrcEscapeOptions" style="margin-left:2em; 
                              <?cs if:!source.Encoding.EscapeNonAscii ?>display:none;<?cs /if ?>">
                        <input type="radio" name="SrcEscapeMethod" id="SrcEscapeAuto" value="auto"
                           <?cs if:!source.Encoding.EscapeChar ?>checked<?cs /if ?>>
                        <label for="SrcEscapeAuto">Automatically detect escape character</label>
                        <br>
                        <input type="radio" name="SrcEscapeMethod" id="SrcEscapeSpec" value="spec"
                           <?cs if: source.Encoding.EscapeChar ?>checked<?cs /if ?>>
                        <label for="SrcEscapeSpec">Always escape with
                           <input name="SrcEscapeChar" size="1"
                                  value="<?cs alt:source.Encoding.EscapeChar ?>\<?cs /alt ?>"></label>
                     </div>
                  <?cs /if ?>
               </td>               
               <td class="inner_right">
                  <?cs if:Channel.Source.Error.Encoding ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Source.Error.Encoding ?></ul>
                     </div>
                  <?cs /if ?>
                  <?cs if:Channel.Source.Error.Escaping ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Source.Error.Escaping ?></ul>
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
            <?cs if:source.LlpNormal ?> Normal LLP:
            <?cs else ?>                Custom LLP: 
            <?cs /if ?>
            <span class="configurationFillerText">messages begin after</span>
               "<?cs var:html_escape(source.LlpStart) ?>"
            <span class="configurationFillerText">and continue until</span>
               "<?cs var:html_escape(source.LlpEnd) ?>"
      </tr>
   <?cs else ?>
      <tr class="selected">
         <td class="left_column">LLP delimiters
         <td class="inner_left" colspan="3">
            <input type="radio" id="SrcLlpNormal" name="SrcLlpNormal" value="normal" <?cs if:source.LlpNormal ?>checked<?cs /if ?>>
               <label for="SrcLlpNormal">Normal LLP</label><br>
            <input type="radio" id="SrcLlpCustom" name="SrcLlpNormal" value="custom" <?cs if:!source.LlpNormal ?>checked<?cs /if ?>>
               <label for="SrcLlpCustom">Messages begin after</label>
               <input type="text" style="width:80px" name="SrcLlpStart" value="<?cs var:html_escape(source.LlpStart) ?>">
               <label for="SrcLlpCustom">and continue until</label>
               <input type="text" style="width:80px" name="SrcLlpEnd" value="<?cs var:html_escape(source.LlpEnd) ?>">
               <a class="helpIcon" rel="These delimiters will be expected around each message received
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
               <td class="channel_configuration_section_heading">Connection Settings</td>
            </tr>
         </table>
      </td>
      <td colspan="3"><hr /></td>
   </tr>
   
   <tr class="selected" id="SrcPortRow">
      <td class="left_column first_row">Port<font color="#ff0000">*</font></td>
      <td class="inner_left first_row" colspan="3">
         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?>
                     <?cs var:html_escape(source.Port) ?>
                     <?cs if:html_escape(source.Port) != html_escape(environment_expand(source.Port)) ?>
                        <span style="letter-spacing:-2px;">&ndash;&gt;&nbsp;</span>
                        <?cs var:html_escape(environment_expand(source.Port)) ?>
                     <?cs /if ?>
                     <input type="hidden" class="configuration_smaller" name="SrcPort" value="<?cs var:source.Port ?>" id="SrcPort" />
                     <span id="SrcPortErrorMessageContainer" class="validation_error_message_container" style="white-space:normal"></span>
                  <?cs else ?>
                     <nobr>
                     <input type="text" class="full_length" name="SrcPort" onchange="checkForPortConflicts();" id="SrcPort" value="<?cs var:source.Port ?>" />
                     <a id="SrcPort_Icon" class="helpIcon" tabindex="100" rel="To view a list of ports in use by Iguana, visit <a href='port_status.html' target='_blank'>Dashboard &gt; Ports</a>." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a> 
                     <div id="SrcPort_preview_div" style="display:none;">
                        <div id="SrcPort_preview" class="path_preview"></div>
                     </div>
                     <span id="SrcPortErrorMessageContainer" class="validation_error_message_container" style="white-space:normal"></span>
                     </nobr>
                  <?cs /if ?>
               </td>
               <td class="inner_right">
                  <?cs if:Channel.Source.Error.Port ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Source.Error.Port ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>
         </table>
      </td>
   </tr>   

   <tr class="selected">
      <td class="left_column">Enable port sharing</td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?> 
            <?cs if:source.CheckRemoteHost ?>Yes - Connections for this channel are limited to the host(s) below.
            <?cs else ?>No - Only this channel will use this port.<?cs /if ?>
         <?cs else ?>
            <input type="checkbox" class="no_style" id="src_check_remote_host" name="SrcCheckRemoteHost"
               <?cs if:source.CheckRemoteHost ?>checked<?cs /if ?>
               onchange="checkForPortConflicts();"
               onclick="showOrHideRemoteHostRows();" />
            <a id="src_check_remote_host_Icon" class="helpIcon" tabindex="101" rel="When enabled, connections are limited to a given remote host and an optional alternate.  This allows multiple LLP channels to accept connections on the same port." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected remote_host_row" style="<?cs if:!source.CheckRemoteHost ?>display:none;<?cs /if ?>">
      <td class="left_column">Remote host<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs with:host=source.RemoteHost ?>
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:host.ConfiguredName != host.Name ?>
               <?cs var:html_escape(host.ConfiguredName) ?>
               <span style="letter-spacing:-2px;">&ndash;&gt;&nbsp;</span>
            <?cs /if ?>
            <?cs var:html_escape(host.Display) ?>
         <?cs else ?>
            <input type="text" class="configuration" id="src_remote_host" name="SrcRemoteHost"
                   value="<?cs var:html_escape(host.ConfiguredName) ?>"
                   onchange="checkForPortConflicts();"
                   onkeyup="expandFields(['src_remote_host']);" />
            <a id="<?cs var:ComponentType ?>RemoteHost_Icon" class="helpIcon" tabindex="101" rel="Each LLP Listener source component can only connect to exactly one client.  This field contains the IP address or host name of the connecting client." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
            <div id="src_remote_host_preview_div" style="display:none;">
               <div id="src_remote_host_preview" class="path_preview" val="<?cs var:html_escape(host.Name) ?>" ></div>
            </div>
         <?cs /if ?>
         <?cs if:Channel.Source.Error.RemoteHost ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.RemoteHost ?>
            </div>
         <?cs /if ?>
         <?cs /with ?>
     </td>
   </tr>

   <?cs if: !Channel.ReadOnlyMode || source.AlternateRemoteHost.ConfiguredName ?>
   <tr class="selected remote_host_row" style="<?cs if:!source.CheckRemoteHost ?>display:none;<?cs /if ?>">
      <td class="left_column">Alternate remote host</td>
      <td class="inner_left" colspan="3">
         <?cs with:host=source.AlternateRemoteHost ?>
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs if:host.ConfiguredName != host.Name ?>
               <?cs var:html_escape(host.ConfiguredName) ?>
               <span style="letter-spacing:-2px;">&ndash;&gt;&nbsp;</span>
            <?cs /if ?>
            <?cs var:html_escape(host.Display) ?>
         <?cs else ?>
            <input type="text" class="configuration" id="src_alternate_remote_host"
                   name="SrcAlternateRemoteHost" value="<?cs var:html_escape(host.ConfiguredName) ?>"
                   onchange="checkForPortConflicts();"
                   onkeyup="expandFields(['src_alternate_remote_host']);" />
            <a id="<?cs var:ComponentType ?>AlternateRemoteHost_Icon" class="helpIcon" tabindex="101" rel="Each LLP Listener source component can only connect to exactly one client.  This field contains an alternative IP address or host name for the connecting client." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
            <div id="src_alternate_remote_host_preview_div" style="display:none;">
               <div id="src_alternate_remote_host_preview" class="path_preview" val="<?cs var:html_escape(host.Name) ?>" ></div>
            </div>   
         <?cs /if ?>
         <?cs if:Channel.Source.Error.AlternateRemoteHost ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.AlternateRemoteHost ?>
            </div>
         <?cs /if ?>
         <?cs /with ?>
     </td>
   </tr>
   <?cs /if ?>

   <tr class="selected" id="SrcConnectionTimeoutRow">
      <td class="left_column">Connection timeout </td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode ?>                     
            <?cs if:!source.UnlimitedTimeout ?>Yes<?cs else ?>No<?cs /if ?>          
            <span id="SrcConnectionTimeout" class="spanConnectionTimeout" <?cs if:source.UnlimitedTimeout ?>style="display: none;"<?cs /if ?> >
               <span class="configurationFillerText">after</span>
               <?cs var:source.ConnectionTimeout ?> minute<?cs if:source.ConnectionTimeout > 1 ?>s<?cs /if ?>
               <span class="configurationFillerText">of inactivity</span>
            </span>
         <?cs else ?>
            <input type="checkbox" class="no_style" id="inputUseConnectionTimeout" name="SrcUseConnectionTimeout" <?cs if:!source.UnlimitedTimeout ?>checked<?cs /if ?> onclick="toggleSrcConnectionTimeoutTextField();" />&nbsp;      
            <span id="SrcConnectionTimeout" class="spanConnectionTimeout" <?cs if:source.UnlimitedTimeout ?>style="display: none;"<?cs /if ?> >
            after
            <input type="text" class="number_field" name="SrcConnectionTimeout" id="SrcConnectionTimeoutInput"
               value="<?cs var:source.ConnectionTimeout ?>" />
            minute(s) of inactivity
            <span id="SrcConnectionTimeoutErrorMessageContainer" class="validation_error_message_container"></span>
            <script defer type="text/javascript">
               VALregisterIntegerValidationFunction
               (
                  'SrcConnectionTimeoutInput', 'SrcConnectionTimeoutRow', 'SrcConnectionTimeoutErrorMessageContainer',
                  function()
                  {
                     var ConnectionTimeoutCheckBox = document.getElementById('inputUseConnectionTimeout');
                     return ConnectionTimeoutCheckBox.checked;
                  },
                  showSourceTab 
               );
            </script>
            </span>
         <?cs /if ?>

         <?cs if:Channel.Source.Error.ConnectionTimeout ?>
            <div class="configuration_error">
               <?cs var:Channel.Source.Error.ConnectionTimeout ?>
            </div>
         <?cs /if ?>
          
      </td>
   </tr>

   <?cs if:PlatformIpV6Supported ?>
   <tr class="selected">
      <td class="left_column">IPv6 support</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode ?>                     
            <?cs if:source.IpV6Supported ?>Yes<?cs else ?>No<?cs /if ?>          
         <?cs else ?>
            <input type="checkbox" class="no_style" id="SrcIpV6Supported" name="SrcIpV6Supported" <?cs if:source.IpV6Supported ?>checked<?cs /if ?> />     
       <a id="SrcIpV6Supported_Icon" class="helpIcon" tabindex="101" rel="Enabling IPv6 support will allow connections from IPv6 clients. On some platforms IPv6 support will also allow IPv4 connections. See <a href='<?cs var:help_link('iguana_ipv6') ?>'>IPv6 support</a> for more information." title="IPV6 Support" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
         <?cs /if ?>
      </td>
   </tr>
   <?cs else ?>
      <input type="hidden" id="SrcIpV6Supported" name="SrcIpV6Supported" value="<?cs if:source.IpV6Supported ?>on<?cs /if ?>" />     
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
      <td class="left_column first_row">ACK </td>
      <td class="inner_left first_row" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode || Channel.IsEncrypted ?>
            <?cs # using an input tag in order to get value out when serializing the form (for channel config edits) ?>
            <input name="SrcAckStyle" style="display:none;" value=                  
            <?cs if:  source.AckStyle == 'vmd'   ?> "vmd">Legacy VMD
            <?cs elif:source.AckStyle == 'trans' ?> "trans">Translator
            <?cs else ?> "fast">Fast <?cs /if ?>
            </input>
         <?cs else ?>
            <select name="SrcAckStyle">
               <option value="fast"  <?cs if:source.AckStyle == 'fast'  ?>selected<?cs /if ?> >Fast</option>
               <option value="trans" <?cs if:source.AckStyle == 'trans' ?>selected<?cs /if ?> >Translator</option>
               <option value="vmd"   <?cs if:source.AckStyle == 'vmd'   ?>selected<?cs /if ?> >Legacy VMD</option>
            </select>
       <a id="SrcFastAckChoiceBox_Icon" class="helpIcon" tabindex="101" rel="Fast acknowledgment is optimized for high throughput and is recommended for most standard LLP channels. Acknowledgments can also be customized with either the Translator or Legacy VMD options, to provide flexible behavior using a technique called <a href='<?cs var:help_link('autoack') ?>' target='_new'>Auto ACKnowledgment</a>." title="ACK Options" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
         <?cs /if ?>
      </td>
   </tr>

   <tr id="AckVmdPathRowId" class="selected" <?cs if:source.AckStyle != 'vmd' ?>style="display: none;"<?cs /if ?>  >
      <td class="left_column">ACK VMD path<font color="#ff0000">*</font></td>
         <td class="inner_left">
            <?cs if:Channel.ReadOnlyMode ?>
               <?cs if:Channel.Source.Error.AckProdVmdPath ?>
                     <div class="configuration_error">
                        <?cs var:Channel.Source.Error.AckProdVmdPath ?>
                     </div>
                 <?cs else ?> 
                 <?cs if:source.AckVmdPath != source.OriginalVmdPath?>
                  <?cs /if ?>
                  <?cs if:source.OriginalVmdPath != "" ?>
                     <label>Uploaded from: </label><br><?cs call:browse_readonly(source.OriginalVmdPath) ?>
                  <?cs /if ?>
                  <?cs if:source.AckVmdPath != "" ?>
                     <br><br><?cs call:download_vmd(Channel.Guid, Channel.editTab, Channel.Name, "SrcAckVmd") ?>
                  <?cs /if ?>
               <?cs /if ?>
            <?cs else ?>
               <?cs if:!Channel.Source.Error.AckProdVmdPath && source.OriginalVmdPath != "" ?>
                  <div class="original_vmd">
                     <label>Uploaded from: </label><br><?cs call:browse_readonly(source.OriginalVmdPath) ?>
                  </div><br>
               <?cs /if ?>
                  <?cs call:browse_input('SrcAckVmdPath', source.AckVmdPath) ?>
                  <?cs if:!Channel.Source.Error.AckProdVmdPath && source.OriginalVmdPath != "" ?>
                     <br><a id="useOriginalFromLLP"><u>Use previously uploaded VMD path</u></a>
                     <script type="text/javascript">
                        $("#useOriginalFromLLP").click(function(){
                           $("#SrcAckVmdPath").val("<?cs var:source.CleanedOriginal ?>");
                        });
                     </script> 
                  <?cs /if ?>
                   <?cs if:source.AckVmdPath != "" ?>
                     <br><br><br><?cs call:download_vmd(Channel.Guid, Channel.editTab, Channel.Name, "SrcAckVmd") ?>
                     <a id="DownloadVmdBox" style="padding-left: 4px;" class="helpIcon" tabindex="101" rel="Download the currently running VMD from source control." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                  <?cs /if ?>  
            <?cs /if ?>
         </td>
   </tr>

   <tr class="selected" id="TrackVmdChangesSrc" <?cs if:source.AckStyle != 'vmd' ?>style="display: none;"<?cs /if ?> >
      <td class="left_column">Track VMD Changes</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?> 
                     <?cs if:source.KeepUpdated ?>Yes<?cs else ?>No<?cs /if ?>          
                  <?cs else ?>
                     <br><?cs call:keep_updated("UpdatedAckSrcVmd", source.KeepUpdated) ?>      
                     <a id="KeepVmdUpdatedBox" class="helpIcon" tabindex="101" rel="When enabled, at startup, the channel will compare its current copy of the VMD with the version at the path it was uploaded from. If they are different, the 'uploaded from' version will be automatically copied into the run location and committed to source control before the channel begins processing messages." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                  <?cs /if ?>
               </td>
               <td class="inner_right">
                  <?cs if:Channel.Source.Error.KeepUpdated ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Source.Error.KeepUpdated ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>     
         </table>
      </td>
   </tr>
  
   <?cs if: !Channel.ReadOnlyMode || source.AckStyle == 'trans' ?>                     
      <?cs call:renderMapperForm(source, !!source.Guid,
            'SrcAck', 'Source', source.Type, 'AckTransRow', 0, 1) ?>
   <?cs /if ?>

 
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

   <tr class="selected" id="SrcUseSslRow">
      <td class="left_column first_row">Use SSL</td>
      <td class="inner_left first_row" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?> 
                     <?cs if:source.UseSsl ?>Yes<?cs else ?>No<?cs /if ?>          
                  <?cs else ?>
                     <input type="checkbox" class="no_style" id="SrcInputUseSsl" name="SrcUseSsl" <?cs if:source.UseSsl ?>checked<?cs /if ?> onclick="showOrHideListenerSslRows();" />&nbsp;      
                  <?cs /if ?>
               </td>
               <td class="inner_right">
                  <?cs if:Channel.Source.Error.Ssl ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Source.Error.Ssl ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>     
         </table>
      </td>
   </tr>

   <tr class="selected listener_ssl_row" id="SrcSslCertificateKeyFileRow" <?cs if:!source.UseSsl ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">Certificate file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(source.SslCertificateKeyFile) ?>
            <?cs if:source.SslCertificateKeyFile!='' ?>
               <a href="/view_ssl_cert.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&ComponentType=Source">view certificate</a>
            <?cs /if ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('SrcSslCertificateKeyFile', source.SslCertificateKeyFile) ?>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected listener_ssl_row" id="SrcSslPrivateKeyFileRow" <?cs if:!source.UseSsl ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">Private key file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(source.SslPrivateKeyFile) ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('SrcSslPrivateKeyFile', source.SslPrivateKeyFile) ?>
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected listener_ssl_row" id="SrcSslVerifyPeerRow" <?cs if:!source.UseSsl ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">Verify peer</td>
      <td class="inner_left" colspan="3" style="padding-top: 0px; padding-bottom: 0px;">
         <?cs if:Channel.ReadOnlyMode ?> 
            <?cs if:source.SslVerifyPeer ?>Yes<?cs else ?>No<?cs /if ?>          
         <?cs else ?>
            <input type="checkbox" class="no_style" id="SrcInputSslVerifyPeer" name="SrcSslVerifyPeer" <?cs if:source.SslVerifyPeer ?>checked<?cs /if ?> onclick="showOrHideListenerSslVerifyPeerRows();" />&nbsp;      
         <?cs /if ?>
      </td>
   </tr>

   <tr class="selected listener_ssl_row listener_ssl_verify_peer_row" id="SrcSslCertificateAuthorityFileRow" <?cs if:!source.UseSsl || !source.SslVerifyPeer ?>style="display:none;"<?cs /if ?>>
      <td class="left_column">Certificate authority file<font color="#ff0000">*</font></td>
      <td class="inner_left" colspan="3">
         <?cs if:Channel.ReadOnlyMode ?>
            <?cs call:browse_readonly(source.SslCertificateAuthorityFile) ?>
            <?cs if:source.SslCertificateAuthorityFile!='' ?>
               <a href="/view_ssl_cert.html?ChannelName=<?cs var:url_escape(Channel.Name) ?>&CertificateType=Authority&ComponentType=Source">view certificate</a>
            <?cs /if ?>
         <?cs else ?>
            <span id="NaCertificateFile" style="display:none;">N/A</span>
            <?cs call:browse_input('SrcSslCertificateAuthorityFile', source.SslCertificateAuthorityFile) ?>
         <?cs /if ?>
      </td>
   </tr>


<?cs /with ?>
