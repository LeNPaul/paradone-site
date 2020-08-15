<?cs # vim: set syntax=html :?>
<?cs include:"email_notification_macros.cs" ?>

<script type="text/javascript">

var PossibleRecipients = {};
var Tooltips = {};

<?cs each:possibleRecipient = PossibleRecipients ?>
   PossibleRecipients["<?cs var:javascript_escape(possibleRecipient.Name) ?>"] = {
   NameToDisplay: "<?cs var:javascript_escape(possibleRecipient.NameToDisplay) ?>",
   // For Users
   Email: "<?cs var:javascript_escape(possibleRecipient.EmailAddress) ?>",
   Sms:   "<?cs var:javascript_escape(possibleRecipient.SmsAddress) ?>",
   // For Groups
   CountOfUser:      <?cs var:#possibleRecipient.CountOfUser ?>,
   UsersWithAddress: <?cs var:#possibleRecipient.UsersWithAddress ?>
};

Tooltips["<?cs var:javascript_escape(possibleRecipient.Name) ?>"] = "<?cs var:javascript_escape(possibleRecipient.Tooltip) ?>";
<?cs /each ?>

<?cs each: r = BadRecipients ?>
   Tooltips['<?cs var:js_escape(r.Name) ?>'] = '<?cs var:js_escape(r.Tooltip) ?>';
<?cs /each ?>

function onTabClick() {
   // Validate before switching tabs. It's much easier.
   if (VALvalidateFields()) {
      VALfieldValidationFunctions = [];
      return true;
   } else {
      return false;
   }
}

function setSourceClick() {
   var ErrorDiv = document.getElementById("SourceErrorDiv");
   ErrorDiv.style.display = "none";
   var InputSelect = document.getElementById("SourceTypeSelect");
   InputSelect.name = "SourceType";
   InputSelect.style.display = "";
   toggleStandardSourceType();
}

function toggleStandardSourceType() {
   toggleSourceType("SourceTypeSelect", "trStandardRuleChannels", "trStandardRuleGroups");
}

function toggleSourceType(SourceId, ChannelsId, GroupsId) {
   var SourceDropdown = document.getElementById(SourceId);
   var ChannelDropdown = document.getElementById(ChannelsId);
   var GroupDropdown = document.getElementById(GroupsId);

   if(SourceDropdown.selectedIndex < 0) {
      return;
   }
   if (SourceDropdown.options[SourceDropdown.selectedIndex].value == "Channel") {
      ChannelDropdown.style.display = SourceDropdown.style.display;
      GroupDropdown.style.display = "none";
   } else if (SourceDropdown.options[SourceDropdown.selectedIndex].value == "Group") {
      ChannelDropdown.style.display = "none";
      GroupDropdown.style.display = SourceDropdown.style.display;
   } else {
      ChannelDropdown.style.display = "none";
      GroupDropdown.style.display = "none";
   }
}

$(document).ready(function() {
   $('#no_recipients').show();
   $('#recipientDropDown').hide();
   var CountOfRecipient = 0;

   function validateBeforeSubmit(){
     var StandardButton = document.getElementById("StandardRuleTypeRadioButton");
      var ProcessStandardRule =
         <?cs if:RuleType == "Channel Inactivity" ?>
            false
         <?cs elseif:RuleType == "Standard" ?>
            true
         <?cs else ?>
            StandardButton.checked
         <?cs /if ?>;

      if (ProcessStandardRule) {
         var SourceDropdown = document.getElementById("SourceTypeSelect");
         var ChannelDropdown = document.getElementById("StandardChannelSelect");
         var GroupDropdown = document.getElementById("StandardGroupSelect");
      } else {
         var SourceDropdown = document.getElementById("InactivitySourceTypeSelect");
         var ChannelDropdown = document.getElementById("InactivityChannelSelect");
         var GroupDropdown = document.getElementById("InactivityGroupSelect");
      }

      var Dropdown;
      if (SourceDropdown.options[SourceDropdown.selectedIndex].value == "Channel") {
         Dropdown = ChannelDropdown;
      } else if (SourceDropdown.options[SourceDropdown.selectedIndex].value == "Group") {
         Dropdown = GroupDropdown;
      }

      if(!Dropdown) {
console.log("No drop");
         return true;
      } else {
         var Go = false;
         var Name = Dropdown.options[Dropdown.selectedIndex].value;
         sourceExists(SourceDropdown.options[SourceDropdown.selectedIndex].value, Name,
            function(status, type, name) {
               if (status) {
console.log("Status");
                  Go = true;
               } else {
                  alert("Warning: " + type + " " + name + " no longer exists.");
                  Dropdown.remove(Dropdown.selectedIndex);
console.log("Warning");
              }
            });
console.log("End");
      }
      return Go;
   }

   function toggleInactivitySourceType() {
      toggleSourceType("InactivitySourceTypeSelect", "trInactivityRuleChannels", "trInactivityRuleGroups");
   }

   $("#InactivitySourceTypeSelect").change(toggleInactivitySourceType);

   $("#SourceTypeSelect").change(toggleStandardSourceType);

   function makeUserFriendlyRecipientName(Name, StartEmailAddress, EndEmailAddress) {
      var RecipientString = Name;
      var Recipient = PossibleRecipients[Name];
      if(Recipient)   {
         if(Recipient.CountOfUser) {
            // A group.
            var Count = Recipient.UsersWithAddress;
            RecipientString += (Count == 1)
                               ? ' (' + Count + ' user has an email or SMS address)'
                               : ' (' + Count + ' users have email or SMS addresses)';
         } else {
            // A user.
            if(Recipient.Email) {
               RecipientString += ' <' + Recipient.Email + '>';
            }
            if(Recipient.Sms) {
               RecipientString += ' <' + Recipient.Sms + '>';
            }
         }
      }
      return RecipientString;
   }

   function addRecipientToDropDown(DropDown, Name, NameToDisplay, Color) {
      var NewOption = document.createElement('option');
      NewOption.value = Name;
      NewOption.appendChild(document.createTextNode(makeUserFriendlyRecipientName(NameToDisplay)));
      NewOption.style.color = Color;
      DropDown.appendChild(NewOption);
   }

   function recipientAlreadySelected(Name) {
      for (var RecipientIndex = 0; RecipientIndex < CountOfRecipient; ++RecipientIndex) {
         var RecipientInput = document.getElementById("Recipient_" + RecipientIndex);
         if (RecipientInput && RecipientInput.value == Name) {
            return true;
         }
      }
      return false;
   }

   function populateDropDown(DropDown) {
      var Nobody = document.getElementById("no_recipients");
      while (DropDown.hasChildNodes()) {
         DropDown.removeChild(DropDown.firstChild);
      }
      addRecipientToDropDown(DropDown, "Select recipient...", "Select recipient...", "gray");
      for (var Key in PossibleRecipients) {
         if (!recipientAlreadySelected(Key)) {
            addRecipientToDropDown(DropDown, Key, PossibleRecipients[Key].NameToDisplay, "black");
         }
      }
      if (DropDown.childNodes.length == 1) {
         DropDown.style.display = 'none';
         Nobody.style.display = 'block';
      } else {
         DropDown.style.display = 'inline';
         Nobody.style.display = 'none';
      }
   }

   function createRecipientBox(RecipientName, RecipientNameToDisplay, Available) {
      var NewRecipientBox = document.createElement('a');
      var NewRecipientSpan = document.createElement('span');
      NewRecipientBox.className = "recipient recipient_no_link"
         + (Available ? "" : " recipient_broken");
      NewRecipientSpan.innerHTML = RecipientNameToDisplay;
      NewRecipientBox.appendChild(NewRecipientSpan);
      NewRecipientBox.href = "#";
      NewRecipientBox.onclick = function() { return false; }
      NewRecipientBox.onmouseover = function () {
         if (Tooltips[RecipientName] == undefined) {
            TOOLtooltipLink("User or role does not exist", null, NewRecipientBox);
         } else {
            TOOLtooltipLink(Tooltips[RecipientName], null, NewRecipientBox);
         }
      }
      NewRecipientBox.onmouseout = function() {
         TOOLtooltipClose();
      }
      return NewRecipientBox;
   }

   function addRecipientRow(RecipientName, RecipientNameToDisplay, Available) {
      if (RecipientNameToDisplay == 'Select recipient...') {
         return;
      }
      var TableRecipients = document.getElementById('tableRecipientsBody');
      var DropDownRow     = document.getElementById('recipientDropDownRow');
      var NewRow          = document.createElement('tr');
      var NewValueColumn  = document.createElement('td');
      var NewValueInput   = document.createElement('input');
      var NewRemoveColumn = document.createElement('td');
      var NewRemoveButton = document.createElement('a');
      NewValueInput.type  = "hidden";
      NewValueInput.name  = "Recipient_" + CountOfRecipient++;
      NewValueInput.id    = NewValueInput.name;
      NewValueInput.value = RecipientName;
      NewRemoveButton.className = "action-button-small blue";
      NewRemoveButton.textContent = "Remove";
      NewRemoveButton.onclick = function() {
         TableRecipients.removeChild(NewRow);
         populateDropDown(document.getElementById("recipientDropDown"));
         return false;
      }
      NewRow.style.lineHeight = "18px";
      NewValueColumn.appendChild(  NewValueInput   );
      NewValueColumn.appendChild(  createRecipientBox(RecipientName, RecipientNameToDisplay, Available) );
      NewRemoveColumn.appendChild( NewRemoveButton );
      NewRow.appendChild(          NewValueColumn  );
      NewRow.appendChild(          NewRemoveColumn );
      TableRecipients.insertBefore(NewRow, DropDownRow);
   }

   function onDropDownChange(DropDown) {
      addRecipientRow(DropDown.value, DropDown.options[DropDown.selectedIndex].text, true);
      populateDropDown(DropDown);
   }
   $("#recipientDropDown").on("change", function() {
      onDropDownChange(this);
   });

   function onScriptChange(DropDown) {
      var Input = document.getElementById("NotifyScriptName");
      console.info(DropDown.selected.name);
   }

   function onScriptCheck(CheckBox) {
      document.getElementById("scriptDropDown").style.display = CheckBox.checked ? "" : "none";
   }

   function removeRecipientRow(RowId) {
      var TableRecipients = document.getElementById('tableRecipients');
      TableRecipients.removeChild(document.getElementById(RowId));
   }

   function setRowDisplay(RowId, NewStyle) {
      document.getElementById(RowId).style.display = NewStyle;
   }

   function setAllStandardRuleRowsDisplay(NewStyle) {
      setRowDisplay("trStandardRuleSource", NewStyle);
      setRowDisplay("trStandardRuleType", NewStyle);
      setRowDisplay("trStandardRuleTextQuery", NewStyle);
      setRowDisplay("trStandardRuleEmailLimit", NewStyle);
      setRowDisplay("trStandardRuleIncludeLogEntryInEmail", NewStyle);
      setRowDisplay("SourceTypeSelect", NewStyle == "none" ? NewStyle : "");
      toggleStandardSourceType();
   }

   function setAllChannelInactivityRuleRowsDisplay(NewStyle) {
      setRowDisplay("trChannelInactivityRuleChannel", NewStyle);
      setRowDisplay("trChannelInactivityRuleInterval", NewStyle);
      setRowDisplay("trChannelInactivityRuleFilter", NewStyle);
      setRowDisplay("trChannelInactivityResume", NewStyle);
      setRowDisplay("InactivitySourceTypeSelect", NewStyle == "none" ? NewStyle : "");
      toggleInactivitySourceType();
   }

   function showConfigurationTable(SelectedElement) {
      var trShow = WINgetStyle(document.getElementById("trRuleType"), "display");
      if (SelectedElement.value == "Standard") {
         setAllStandardRuleRowsDisplay(trShow);
         setAllChannelInactivityRuleRowsDisplay('none');
      } else {
         setAllStandardRuleRowsDisplay('none');
         setAllChannelInactivityRuleRowsDisplay(trShow);
      }
      document.getElementById("divRuleConfiguration").style.display = 'inline';
   }

   function emailRuleEditOnLoad() {
      <?cs each:recipient = Recipients ?>
      addRecipientRow("<?cs var:javascript_escape(recipient.Name) ?>",
                      "<?cs var:javascript_escape(recipient.NameToDisplay) ?>",
                      <?cs var:#recipient.Available ?>
                      );
      <?cs /each ?>
      populateDropDown(document.getElementById("recipientDropDown"));

      <?cs if:RuleType != "Standard" ?>
      toggleInactivitySourceType();
      <?cs /if ?>
      <?cs if:RuleType != "Channel Inactivity" ?>
      toggleStandardSourceType();
      <?cs /if ?>
      HLPpopUpinitialize();
   }

   function toggle(Input) {
      if (Input == 0) return 1;
      return 0;
   }

   function display(Input) {
      if (Input == 0) return '<div class="on" />';
      return '<div class="off"/>';
   }

   function parseInactivityDataString() {
      var Element = document.getElementById("InactivityDataString");
      if (!Element.value) return false;
      var SplitStrings = Element.value.split(' ');
      if(!SplitStrings || SplitStrings.length < 2) return false;
      var Width = parseInt(SplitStrings[0]);
      var Height = parseInt(SplitStrings[1]);

      if(SplitStrings.length != (Width*Height) + 2) return false;

      var Data = new Array(Height);
      var Y;
      for( YIndex = 0; YIndex < Height; YIndex++) {
         Data[YIndex] = new Array(Width);
      }
      var XPos;
      var YPos;
      for(YPos = 0; YPos < Height; YPos++){
         for(XPos = 0; XPos < Width; XPos++){
            // +2 because SplitStrings begins with Width and Height before real data.
            Data[YPos][XPos] = parseInt(SplitStrings[(YPos * Width) + XPos + 2]);
         }
      }
      return Data;
   }

   function updateTable(Name, Data) {
      selectGridSetData(Data, Name);
   }

   //Turn data into a string of form "width height x x x x x ... x x x"
   function writeTimeRules(Grid) {
      var Data = Grid.Data;
      var Width = Grid.Width;
      var Height = Grid.Height;
      var XPos;
      var YPos;
      var DataString = "";
      DataString += Width;
      DataString += " ";
      DataString += Height;
      if (Width > 0 && Height > 0) {
         for(YPos = 0; YPos < Height; YPos++) {
            for(XPos = 0; XPos < Width; XPos++) {
               DataString += (" " + Data[YPos][XPos]);
            }
         }
         var Element = document.getElementById("InactivityDataString");
         Element.value = DataString;
      }
   }

   function setChannelClick() {
      var ErrorDiv = document.getElementById("ChannelErrorDiv");
      ErrorDiv.style.display = "none";
      var InputSelect = document.getElementById("InactivitySourceTypeSelect");
      InputSelect.name = "InactivitySourceType";
      InputSelect.style.display = "";
      toggleInactivitySourceType();
   }
   $("#reset_channel").click(setChannelClick);
   emailRuleEditOnLoad();
   var Name = "rules_table";

   //Can do this without all of the concats, but it becomes an ugly ball of html.
   var XAxisLabelBuffer = ['   <tr class="time_filter_row" onmouseover="selectGridMouseout(', "'", Name, "'", ')">',
                           '   <td class="time_filter_cell_day"></td>' ,
                           '   <td class="time_filter_cell" colspan="4" onmouseover="selectGridMouseout(', "'", Name, "'", ')">Midnight - 4am</td>' ,
                           '   <td class="time_filter_cell" colspan="4" onmouseover="selectGridMouseout(', "'", Name, "'", ')">4am - 8am</td>' ,
                           '   <td class="time_filter_cell" colspan="4" onmouseover="selectGridMouseout(', "'", Name, "'", ')">8am - Noon</td>' ,
                           '   <td class="time_filter_cell" colspan="4" onmouseover="selectGridMouseout(', "'", Name, "'", ')">Noon - 4pm</td>' ,
                           '   <td class="time_filter_cell" colspan="4" onmouseover="selectGridMouseout(', "'", Name, "'", ')">4pm - 8pm</td>' ,
                           '   <td class="time_filter_cell" colspan="4" onmouseover="selectGridMouseout(', "'", Name, "'", ')">8pm - Midnight</td>' ,
                           '   </tr>',
                           '   <tr class="time_filter_row" onmouseover="selectGridMouseout(', "'", Name, "'", ')">',
       		           '   <td class="time_filter_cell_day" onmouseover="selectGridMouseout(', "'", Name, "'", ')"></td>'];

   var HourIndex;
   for(HourIndex = 0; HourIndex < 24; HourIndex++) {
      XAxisLabelBuffer.push('   <td class="time_filter_cell" onmouseover="selectGridMouseout(', "'", Name, "'", ')">' , HourIndex , '</td>');
   }
   XAxisLabelBuffer.push('   <td class="time_filter_cell" onmouseover="selectGridMouseout(', "'", Name, "'", ')"/>');
   var GridData = parseInactivityDataString();
   if (!GridData) {
                                    //Default all active if there are no rules.
                                    GridData =
                                    [
                                      [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
				      [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
                                      [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
				      [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
                                      [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
                                      [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
                                      [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]
                                    ];
                                 }

                                 selectGrid(24,7, Name, GridData, toggle, display, writeTimeRules, XAxisLabelBuffer.join(''), ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], "#TimerTableGrid");
   VALregisterIntegerValidationFunction('inputMaximumEmailsPerHour', 'trStandardRuleEmailLimit', 'MaximumEmailsPerHourErrorMessageContainer',
      function() {
         var MaxEmailsPerHourRow = document.getElementById('trStandardRuleEmailLimit');
         return WINgetStyle(MaxEmailsPerHourRow, 'display') != 'none';
      }, null, 1, 60);
   VALregisterIntegerValidationFunction('InactivityIntervalInput', 'trChannelInactivityRuleInterval', 'InactivityIntervalErrorMessageContainer',
      function() {
         var InactivityIntervalRow = document.getElementById('trChannelInactivityRuleInterval');
         return WINgetStyle(InactivityIntervalRow, 'display') != 'none';
      }, null);

   $("#StandardRuleTypeRadioButton").click(function() {
      showConfigurationTable(this);
   });
   $("#ChannelInactivityRuleTypeRadioButton").click(function() {
      showConfigurationTable(this);
   });
   $("form#rule_settings").submit({uri: "/email_settings/rules/view", form_id: "rule_settings"}, SettingsHelpers.submitForm);
   $("a#ApplyChanges").click(function(event) {
      event.preventDefault();
      $(this).blur();
      if (validateBeforeSubmit()) {
         $("form#rule_settings").submit();
      }
   });

});

</script>


<?cs def:hideIfNotRuleType(ruleType) ?>
   <?cs if:RuleType != ruleType ?>style="display: none;"<?cs /if ?>
<?cs /def ?>

<table id="iguana">
   <tr>
      <td id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; <a href="/settings#Page=email_status" onclick="return onTabClick();">Email Notification</a> &gt; Edit Rule
      </td>
   </tr>
   <tr>
      <td id="dashboard_body">
         <center>

            <div class="tabs_and_contents">

               <?cs call:email_navigation_tabs("Rules", "return onTabClick();") ?>
               <br />

               <!-- Firefox supports the onchange event for forms, so this makes use of it -->
               <form name="rule_settings" id="rule_settings">

                  <?cs if:NewRule ?>
                     <input type="hidden" name="NewRule" value="true" />
                  <?cs else ?>
                     <input type="hidden" name="RuleKey" value="<?cs var:html_escape(RuleKey) ?>" />
                  <?cs /if ?>

                  <!-- Rule Type -->
                  <?cs if:!RuleType ?>
                  <table class="configuration">
                     <tr id="trRuleType">
                        <td class="left_column">Rule Type</td>
                        <td>
                           <input id="StandardRuleTypeRadioButton" type="radio" class="no_style" name="RuleType" value="Standard">
                              Standard Rule
                           </input><br />
                           <input id="ChannelInactivityRuleTypeRadioButton" type="radio" class="no_style" name="RuleType" value="Channel Inactivity">
                              Channel Inactivity Rule
                           </input>
                        </td>
                     </tr>
                  </table>
                  <br />
                  <?cs else ?>
                     <input type="hidden" name="RuleType" value="<?cs var:html_escape(RuleType) ?>" />
                  <?cs /if ?>

                  <div id="divRuleConfiguration" <?cs if:!RuleType ?>style="display: none;"<?cs /if ?>>

                     <table class="configuration">
                        <tr>
                           <th colspan="2"><?cs var:html_escape(RuleType) ?> Email Notification Rule</th>
                        </tr>

                        <!-- Source -->
                        <tr id="trStandardRuleSource" <?cs call:hideIfNotRuleType("Standard") ?>>
                        <td class="left_column">Source Type</td>
                        <td>
                           <select <?cs if: !SourceIsUnknown ?>name="SourceType" <?cs else ?>style ="display:none" <?cs /if ?> id="SourceTypeSelect">
                           <?cs def:add_source(sourceName) ?>
                              <?cs if: sourceName == SourceType ?>
                                 <option value="<?cs var:html_escape(sourceName) ?>" selected><?cs var:html_escape(sourceName) ?></option>
                              <?cs else ?>
                                 <option value="<?cs var:html_escape(sourceName) ?>"><?cs var:html_escape(sourceName) ?></option>
                              <?cs /if ?>
                           <?cs /def ?>
                           <option value="<?cs var:html_escape(SourceOptions.AllEntriesOption) ?>" <?cs if:SourceIsAllEntries ?>selected<?cs /if ?>>
                              <?cs var:html_escape(SourceOptions.AllEntriesOption) ?>
                           </option>
                           <option value="<?cs var:html_escape(SourceOptions.ServiceEntriesOption) ?>" <?cs if:SourceIsService ?>selected<?cs /if ?>>
                              <?cs var:html_escape(SourceOptions.ServiceEntriesOption) ?>
                           </option>
                           <?cs each: type = SourceOptions.Types ?>
                              <?cs call:add_source(type.Name) ?>
                           <?cs /each ?>
                           </select>
            <?cs if: SourceIsUnknown && RuleType == "Standard" ?>
                              <div id="SourceErrorDiv">
                     <a href="javascript:setSourceClick()">
                     <div class="configuration_error">
                  <font color="#ff0000">Error: Source is unknown. Click to select a new source.</font>
                </div>
                       </a>
               </div>
            <?cs /if ?>
                         </td>
                        </tr>

         <tr id="trStandardRuleChannels" style ="display:none">
                        <td class="left_column">Channel</td>
                        <td>
                           <select name="StandardChannelSelect" id="StandardChannelSelect">
                                 <?cs each: channel = SourceOptions.Channels ?>
                                    <option value="<?cs var:html_escape(channel.Name) ?>" <?cs if: channel.Name == SetSource ?>selected<?cs /if ?> >
                                    <?cs var:html_escape(channel.Name) ?>
                                    </option>
                                 <?cs /each ?>
                              </select>
              </td>
         </tr>

         <tr id="trStandardRuleGroups" style ="display:none">
                        <td class="left_column">Group</td>
                   <td>
                           <select name="StandardGroupSelect" id="StandardGroupSelect">
                                 <?cs each: group = SourceOptions.Groups ?>
                                    <option value="<?cs var:html_escape(group.Name) ?>" <?cs if: group.Name == SetSource ?>selected<?cs /if ?> >
                                    <?cs var:html_escape(group.Name) ?>
                                    </option>
                                 <?cs /each ?>
                            </select>
                <br>
                            <font color="green" size=1>To create or remove groups, go to <a href="/settings#Page=channel/group">Settings &gt; Channel Groups</a></font>
              </td>
         </tr>

                        <!-- Type -->
                        <tr id="trStandardRuleType" <?cs call:hideIfNotRuleType("Standard") ?>>
                        <td class="left_column">Type</td>
                        <td>
                           <select name="LogType">
                           <?cs def:add_type(name,value) ?>
                              <?cs if: value == LogType && !LogTypeIsAllTypes || (value == 'errors' && NewRule) ?>
                                 <option value="<?cs var:html_escape(value) ?>" selected><?cs var:html_escape(name) ?></option>
                              <?cs else ?>
                                 <option value="<?cs var:html_escape(value) ?>"><?cs var:html_escape(name) ?></option>
                              <?cs /if ?>
                           <?cs /def ?>
                           <option value="" <?cs if:LogTypeIsAllTypes ?>selected<?cs /if ?>>All</option>
                           <?cs call:add_type('Messages', 'messages') ?>
                           <?cs call:add_type('ACK Messages', 'ack_messages') ?>
                           <?cs call:add_type('Errors', 'errors') ?>
                           <?cs call:add_type('Warnings', 'warnings') ?>
                           <?cs call:add_type('Successes', 'successes') ?>
                           <?cs call:add_type('Informational', 'info') ?>
                           <?cs call:add_type('Debug', 'debug') ?>
                           </select>
                        </td>
                        </tr>

                        <!-- Text Query -->
                        <tr id="trStandardRuleTextQuery" <?cs call:hideIfNotRuleType("Standard") ?>>
                        <td class="left_column">Text Query</td>
                        <td>
                           <input style="width: 300px;" type="text" name="TextQuery" value="<?cs var:html_escape(TextQuery) ?>">
                           <a id="TextQuery_Icon" class="helpIcon" tabindex="100" title="More Information" target="_blank" href="#" onclick="return false;"
                              rel="<?cs include:"search_tip_logs.cs" ?>
                                   <a href=&quot;<?cs var:help_link('iguana4_email_notification_rule_standard_query') ?>&quot; target=&quot;_blank&quot;>Learn More About Regular Expressions In Text Queries</a></br>">
                              <img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" />
                           </a>
                        </td>
                        </tr>

                        <!-- Maximum Emails Per Hour -->
                        <tr id="trStandardRuleEmailLimit" <?cs call:hideIfNotRuleType("Standard") ?>>
                        <td class="left_column">Limit of Messages Sent</td>
                        <td>
                           <input size="2" class="number_field" type="text" name="MaximumEmailsPerHour" id="inputMaximumEmailsPerHour"
                              value="<?cs alt:MaximumEmailsPerHour ?>5<?cs /alt ?>">
                           per hour (limit <?cs var:#EmailLimitPerHour ?>)
                           <span id="MaximumEmailsPerHourErrorMessageContainer" class="validation_error_message_container"></span>
                        </td>
                        </tr>

                        <!-- Include Log Entry Text in Email -->
                        <tr id="trStandardRuleIncludeLogEntryInEmail" <?cs call:hideIfNotRuleType("Standard") ?>>
                           <td class="left_column">Include Log Entry Text in Email Message</td>
                           <td>
                              <table><tr>
                                 <td>
                                    <input type="checkbox" class="no_style" name="IncludeLogEntryInEmail" <?cs if:IncludeLogEntryInEmail ?>checked="checked"<?cs /if ?> />
                                 </td>
                                 <td style="width: 270px;">
                                    Note: this should only be selected if you are certain that the log entries contain no confidential information.
                                 </td>
                              </tr></table>
                           </td>
                        </tr>

                        <!-- Channel Inactivity Channel -->
                        <tr id="trChannelInactivityRuleChannel" <?cs call:hideIfNotRuleType("Channel Inactivity") ?>>
                           <td class="left_column">Source Type</td>
                           <td>
               <select <?cs if: !SourceIsUnknown ?>name="InactivitySourceType" <?cs else ?>style ="display:none" <?cs /if ?>id="InactivitySourceTypeSelect">
                                 <?cs each: type = SourceOptions.Types ?>
                                    <option value="<?cs var:html_escape(type.Name) ?>" <?cs if: type.Name == InactivitySourceType ?>selected<?cs /if ?> >
                                    <?cs var:html_escape(type.Name) ?>
                                    </option>
                                 <?cs /each ?>
                              </select>
          <?cs if: SourceIsUnknown && RuleType == "Channel Inactivity" ?>
                              <div id="ChannelErrorDiv">
                <a id="reset_channel" style="cursor: pointer;">
                <div class="configuration_error">
                   <font color="#ff0000">Error: Channel is unknown. Click to select a new channel.</font>
                </div>
             </a>
          </div>
                              <?cs /if ?>
                           </td>
                        </tr>

         <tr id="trInactivityRuleChannels" style ="display:none">
                        <td class="left_column">Channel</td>
                        <td>
                           <select name="InactivityChannelSelect" id="InactivityChannelSelect">
                                 <?cs each: channel = SourceOptions.Channels ?>
                                    <option value="<?cs var:html_escape(channel.Name) ?>" <?cs if: channel.Name == SetSource ?>selected<?cs /if ?> >
                                    <?cs var:html_escape(channel.Name) ?>
                                    </option>
                                 <?cs /each ?>
                              </select>
              </td>
         </tr>

         <tr id="trInactivityRuleGroups" style ="display:none">
                        <td class="left_column">Group</td>
                   <td>
                           <select name="InactivityGroupSelect" id="InactivityGroupSelect">
                                 <?cs each: channel = SourceOptions.Groups ?>
                                    <option value="<?cs var:html_escape(channel.Name) ?>" <?cs if: channel.Name == SetSource ?>selected<?cs /if ?> >
                                    <?cs var:html_escape(channel.Name) ?>
                                    </option>
                                 <?cs /each ?>
                            </select>
         <br>
                           <font color="green" size=1>To create or remove groups, go to <a href="/settings#Page=channel/group">Settings &gt; Channel Groups</a></font>
         </td>
         </tr>

                        <!-- Inactivity interval -->
                        <tr id="trChannelInactivityRuleInterval" <?cs call:hideIfNotRuleType("Channel Inactivity") ?>>
                           <td class="left_column">Alert if Channel is Inactive For</td>
                           <td>
                              <input size="2" type="text" name="InactivityInterval" id="InactivityIntervalInput"
                                 value="<?cs var:#InactivityInterval ?>">
                              <select name="InactivityIntervalUnits">
                              <?cs each:units = IntervalUnits ?>
                                 <option value="<?cs var:html_escape(units) ?>" <?cs if:units == InactivityIntervalUnits ?>selected<?cs /if ?>>
                                    <?cs var:html_escape(units) ?>
                                 </option>
                              <?cs /each ?>
                              </select>
                              <span id="InactivityIntervalErrorMessageContainer" class="validation_error_message_container"></span>
                              <script defer type="text/javascript">

                              </script>
                           </td>
                        </tr>

                        <!-- Temporal Filter -->
                        <tr id="trChannelInactivityRuleFilter" <?cs call:hideIfNotRuleType("Channel Inactivity") ?>>
                           <td class="left_column">Notification Schedule</td>
                           <td>
                              <input type="hidden" value="<?cs var:html_escape(InactivityDataString) ?>" name="InactivityDataString" id="InactivityDataString" />
                              <table><tr><td>
                           <span id="TimerTableGrid"></span>
                           </td></tr>
                           <tr><td>
                              <div ALIGN=CENTER>
               <table>
            <tr>
               <td class="time_filter_cell"><div class="on" /></td>
               <td> - Send notification</td>
               <td class="time_filter_cell"><div class="off" /></td>
               <td> - Don't send notification</td>
            </tr>
         </table>
         </div>
                           </td></tr></table>
                           </td>
                        </tr>

                        <!-- Resurrection -->
                        <tr id="trChannelInactivityResume" <?cs call:hideIfNotRuleType("Channel Inactivity") ?>>
                           <td class="left_column">Notify When Channel Activity Resumes</td>
                           <td>
               <input type="checkbox" class="no_style" name="NotifyOnResume" id="NotifyOnResume" <?cs if:NotifyOnResume ?>checked="checked"<?cs /if ?>/>
                           </td>
                        </tr>

                        <!-- Recipients -->
                        <tr>
                           <td class="left_column">Recipients</td>
                           <td>
                              <table>
                                 <tbody id="tableRecipientsBody">
                                    <tr id="recipientDropDownRow">
                                       <td colspan="2">
                                          <div id="no_recipients" style="display:none">
                                             No enabled user accounts have email or SMS addresses.
                                          </div>
                                          <!-- will be populated in the body's onload function -->
                                          <select id="recipientDropDown"></select>
                                       </td>
                                       <td><!-- remove button column --></td>
                                    </tr>
                                 </tbody>
                              </table>
                              <font color="green" size=1>To edit roles or users, go to <a href="/settings#Page=roles">Settings &gt; Roles & Users</a></font>
                           </td>
                        </tr>
                     </table>

                     <p/><p/>

                     <?cs if:CurrentUserCanAdmin ?>
                        <div id="buttons">
                        <input type="hidden" name="Action" value="apply_rule_changes"/>
                        <a class="action-button blue" id="ApplyChanges">Save Changes</a>
                        <a class="action-button blue" href="/settings#Page=email_settings/rules/view">Cancel</a>
                        </div>
                     <?cs /if ?>

                  </div>

               </form>
            </div>
         </center>
      </td>
   </tr>
</table>

<div id="side_panel">
   <table id="side_table">
      <tr>
         <th id="side_header">
            Page Help
         </th>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Overview</h4>
            <?cs if:!RuleType ?>
               <p>
                  From this page, you can create a rule that controls when and to whom email notification messages are sent. You
                  can use this rule to monitor channel activity, such as when a user starts or stops a channel or when a channel
                  has stopped because an error has occurred.
               </p>

          <p>
                  Click <b>Standard Rule</b> to create a standard email notification rule or <b>Channel Inactivity Rule</b> to send
                  email notification when a channel is inactive.
               </p>
            <?cs else ?>
               <p>
                  From this page, you can edit an email notification rule that you have created.
               </p>
          <?cs if:RuleType == "Channel Inactivity" ?>
                  <p>
                     To disable notification during off-peak hours, click and drag to modify the "Notification Schedule".
        </p>
        <p>
           Inactivity notifications will not be sent during day/week combinations that are colored with "Don't send notification".
                  </p>
          <?cs /if ?>
            <?cs /if ?>
            <?cs call:link_to_users_and_groups() ?>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
       <ul class="help_link_icon">
       <?cs if:!RuleType ?>
         <li>
               <a href="<?cs var:help_link('iguana4_email_notification_rules') ?>" target="_blank">Defining Email Notification Rules</a>
         </li>
       <?cs else ?>
         <li>
               <a href="<?cs var:help_link('iguana4_editing_email_notification') ?>" target="_blank">Editing an Email Notification Rule</a>
         </li>
       <?cs /if ?>
       <li>
            <a href="<?cs var:help_link('iguana4_email_notification_rule_examples') ?>" target="_blank">Email Notification Rule Examples</a>
       </li>
       <li>
            <a href="<?cs var:help_link('iguana4_email_notification_rule_standard_query') ?>" target="_blank">Specifying Match Criteria In Standard Rule Text Queries</a>
       </li>
       </ul>
         </td>
      </tr>
   </table>
</div>

<div id="helpTooltipDiv" class="helpTooltip">
   <b id="helpTooltipTitle"></b>
   <em id="helpTooltipBody"></em>
   <input type="hidden" name="helpTooltipId" id="helpTooltipId" value="0">
</div>
