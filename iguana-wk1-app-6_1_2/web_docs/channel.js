
function IsMemberOf(ID) {
   var Options = document.getElementById("groupDropDown").options;
   for (i = 1; i < Options.length; ++i) {
      if (ID == Options[i].value) {
         return false;
      }
   }
   return true;
}

function buildGroups() {
   var GroupList = document.getElementById("channelGroups");
   var Dropdown  = document.getElementById("groupDropDown");
   if (! Dropdown) {
      //No groups exist
      GroupList.value = "";
      return;
   }
   var StringBuilder = [];
   for (var i = 0; i < GroupIDs.length; i++) {
      if (IsMemberOf(GroupIDs[i])) {
         StringBuilder.push(GroupIDs[i]);
      }
   }
   GroupList.value = StringBuilder.join(',');
}

function generateTooltipText(DescriptionText, GroupChannelCount, GlobalChannelCount)
{
	var stringBuilder = [];
        if (DescriptionText != "")
	{
	   stringBuilder.push(DescriptionText);
	   stringBuilder.push("<br><br>");
	}

        if (GlobalChannelCount < 1)
	{
	   stringBuilder.push("<b>No channels defined.</b>");
	   return stringBuilder.join('');
	}

	stringBuilder.push("Contains <b>");
	stringBuilder.push(GroupChannelCount);
	stringBuilder.push("</b> of <b>");
	stringBuilder.push(GlobalChannelCount);

	if (GlobalChannelCount == 1)
	{
	   stringBuilder.push("</b> channel.");
	}
	else
	{
	   stringBuilder.push("</b> channels.");
	}

	return stringBuilder.join('');

}

function onDropdownChange(Dropdown)
{

   if (!Dropdown || Dropdown.value == "noval")
   {
      return;
   }

   document.getElementById("group_row_" + Dropdown.value).style.display = "";

   for (i = Dropdown.length - 1; i >= 1; i--)
   {
      if (Dropdown.options[i].selected)
      {
         Dropdown.remove(i);
         break;
      }
   }

   BelongsToCount++;
   if (BelongsToCount == GroupCount)
   {
      document.getElementById("groupDropDownRow").style.display = "none";
   }

   Dropdown.value = "noval";
}

function removeGroup(ID, Name)
{
   var DropDown = document.getElementById("groupDropDown");

   document.getElementById("group_row_" + ID).style.display = "none";

   var Option = document.createElement('option');
   Option.text = Name;
   Option.value = ID;

   var InsertIndex = 1;

   if(DropDown.options.length > 1)
   {
      for (i = 0; i < Groups.length; ++i)
      {
         if(Groups[i] == Name)
         {
            break;
         }

         if (document.getElementById("groupDropDown").options[InsertIndex].text == Groups[i])
         {
            ++InsertIndex;
         }
      }
   }

   try {
     DropDown.options.add(Option, InsertIndex); // standards compliant; doesn't work in IE
   }
   catch(ex) {
     DropDown.options.add(Option, DropDown.selectedIndex); // IE only
   }

   document.getElementById("groupDropDownRow").style.display = "";
   --BelongsToCount;
}

function getStyleObject(objectId)
{
   if(document.getElementById && document.getElementById(objectId))
   {
      return document.getElementById(objectId).style;
   }
   else if (document.all && document.all(objectId))
   {
      return document.all(objectId).style;
   }
   else if (document.layers && document.layers[objectId])
   {
      return getObjNN4(document,objectId);
   }
   else
   {
      return false;
   }
}

function showDatabaseNote(Prefix, ShowValue)
{
   var DBTypeHelp = document.getElementById(Prefix + "DBTypeHelp");

   if (ShowValue == "MySQL" && DBTypeHelp)
   {
      DBTypeHelp.innerHTML = document.getElementById(Prefix + "_mysql_details").innerHTML;
   }
   else if (ShowValue == "OCI - Oracle" && DBTypeHelp)
   {
      DBTypeHelp.innerHTML = document.getElementById(Prefix + "_oci_details").innerHTML;
   }
   else if (DBTypeHelp)
   {
      DBTypeHelp.innerHTML = document.getElementById(Prefix + "_odbc_details").innerHTML;
   }

   if(Prefix == "Dst" && ShowValue != "")
   {
      var RowDestTimeout = document.getElementById("RowDestTimeout");

      if(ShowValue == "ODBC - MS SQL Server")
      {
         RowDestTimeout.style.display="";
      }
      else
      {
         RowDestTimeout.style.display="none";
      }
   }
   if(Prefix == "Src" && ShowValue != "")
   {
      var RowSrcTimeout = document.getElementById("RowSrcTimeout");

      if(ShowValue == "ODBC - MS SQL Server")
      {
         RowSrcTimeout.style.display="";
      }
      else
      {
         RowSrcTimeout.style.display="none";
      }
   }
}

function showDatabaseNoteSource(Prefix)
{
   var DataSourceIdHelp = document.getElementById(Prefix + "DataSourceIdHelp");
   ShowValue = document.getElementById(Prefix + 'DBType').value;

   if (ShowValue == "MySQL" && DataSourceIdHelp)
   {
      DataSourceIdHelp.innerHTML = document.getElementById(Prefix + "_mysql_database").innerHTML;
   }
   else if (ShowValue == "OCI - Oracle" && DataSourceIdHelp)
   {
      DataSourceIdHelp.innerHTML = document.getElementById(Prefix + "_oci_oracle_database").innerHTML;
   }
   else if (ShowValue == "SQLite" && DataSourceIdHelp)
   {
      DataSourceIdHelp.innerHTML = document.getElementById(Prefix + "_sqlite_database").innerHTML;
   }
   else if (DataSourceIdHelp)
   {
      DataSourceIdHelp.innerHTML = document.getElementById(Prefix + "_odbc_database").innerHTML;
   }
}

function initializeHelp()
{
   if (ReadOnlyMode != '1')
   {
      var highlightFormElements = ['input', 'textarea', 'select'];
      for (var countElements=0; countElements<highlightFormElements.length; countElements++)
      {
         $('#channel_configuration_table ' + highlightFormElements[countElements]).each(
         function()
         {
            this.onfocus = function() { showHighlight(this); };
            this.onmouseover = function() { showHighlight(this);  };
            this.onblur = function() { clearHighlight(this); };
            this.onmouseout = function() { clearHighlight(this); };
         });
      };
   }
}




function clearHighlight(thisElement)
{
   $('#channel_configuration_table tr').css("backgroundColor", "");
   $(thisElement).parents("tr.selected").css("backgroundColor", "");
}

// This function is used to refresh the content of dynamic help windows (modal)
// If you include a div inside of the "rel" tag - see HLPpopUpinitialize() - you can refresh that div by specifying an
function helpTooltipDivRefresh(focusedElement)
{

   if (focusedElement == "SrcDataSourceId")
   {
      showDatabaseNoteSource("Src");
   }
   else if (focusedElement == "DstDataSourceId")
   {
      showDatabaseNoteSource("Dst");
   }
   else if (focusedElement == "SrcDBType")
   {
      showDatabaseNote("Src",document.getElementById(focusedElement).value);
   }
   else if (focusedElement == "DstDBType")
   {
      showDatabaseNote("Dst",document.getElementById(focusedElement).value);
   }
}

function DASHonSelectChannel(selectNode) {
   var Option = selectNode.options[selectNode.selectedIndex].text;
   if (Option) {
      if (Option == '[Add Channel]' && selectNode.options[selectNode.selectedIndex].value == 'AddChannelOption') {
         document.location = "/channel"
      } else {
         navigateAway(Option);
      }
   }
}

function navigateAway(ChannelName) {
   var TabName = document.getElementById("editTab").value.match(/^(.*)Tab$/)[1];
   ifware.SettingsScreen.clearHashTally();
   document.location = "/channel#Channel=" + encodeURIComponent(ChannelName) + 
                       (TabName
                       ? "&Tab=" + TabName
                       : '');
}

// This code was in the document.ready block at the end of the channel.cs page
// but it was moved here so you can debug it properly.
function runAddEditChannelsPagesSetup() {
   SettingsHelpers = initSettingsHelpers();
   onLoad(); // in channel.cs
   var ChannelGuid = $("#channel_guid").val();
   function clearAllTimers() {
      clearTimeout(RequestTimeoutId);
      clearTimeout(StatusTimeoutId);
   }

   $("#ChannelSelect").on("change", function() {
      clearAllTimers();
      DASHonSelectChannel(this);
      return false;
   });

   $("form#channel_data").submit({ uri: "/channel/control", form_id: "channel_data"}, SettingsHelpers.submitForm);

   function submitTheForm(event) {
      clearAllTimers();
      $("form#channel_data").submit();
   }

   $("a#hrefChannelEditButton").click(function() {
      VALfieldValidationFunctions = [];
      submitTheForm();
   });

   $("#channelSave").click(function() {
      buildGroups();
      submitTheForm();
   });

   $("#channelCancel, .channelCancel").click(function() {
      clearAllTimers();
      ifware.SettingsScreen.cancel();
   });

   $("form#remove_" + ChannelGuid).submit({uri: '/channel/remove', form_id: "remove_" + ChannelGuid}, SettingsHelpers.submitForm);


   $("#export-channel").click(function(e) {
      e.preventDefault();
      $("#export-channel-form").submit();
   });

   var Remover =  $("#hrefRemoveButton");
   Remover.click(function(event) {
      event.preventDefault();
      if (! confirm('Are you sure you want to remove this channel?')) {
         return false;
      }
      clearAllTimers();
      $("form#remove_" + ChannelGuid).submit();
   });
}
