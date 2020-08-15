<?cs include:"doctype.cs" ?>
<?cs # vim: set syntax=html :?>
<html>
<head>
<meta http-equiv="X-UA-Compatible" content="IE=Edge" />

<link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css") ?>">

<script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>

<script type="text/javascript" src="/js/jquery-1.11.2/jquery-1.11.2.min.js"></script>
<script type="text/javascript" src="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.min.js"></script>

<script type="text/javascript" src="<?cs var:iguana_version_js("/tooltip.js") ?>"></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>

<?cs include:"mini-login.cs" ?>

<script>
$(document).click(function checkStatus() {
   MiniLogin.loggedIn();
});
</script>


<script type="text/javascript">

// Controls.
var RawRevertButtonHTML = "<a href=\"javascript:onRevert('%NAMEHASH%');\"><img id=\"revert_%NAMEHASH%\" class=\"image_button\" src=\"images/revert_disabled.gif\" onmouseover=\"javascript:setPointerCursor(this, '%NAMEHASH%'); TOOLtooltipLink('Revert back to the global setting of the highlighted variable.', null, this);\" onmouseout=\"javascript:setDefaultCursor(this); TOOLtooltipClose();\" /></a>";
var RawDeleteButtonHTML = "<a href=\"javascript:onDelete('%NAMEHASH%');\"><img id=\"delete_%NAMEHASH%\" class=\"image_button\" src=\"images/ignore_disabled.gif\" onmouseover=\"javascript:setPointerCursor(this, '%NAMEHASH%'); TOOLtooltipLink('Delete the highlighted variable.', null, this);\" onmouseout=\"javascript:setDefaultCursor(this); TOOLtooltipClose();\"/></a>";
var RawSaveButtonHTML = "<a href=\"javascript:onSave('%NAMEHASH%');\"><img id=\"save_%NAMEHASH%\" class=\"image_large_button\" src=\"images/save.gif\" onmouseover=\"javascript:setPointerCursor(this, '%NAMEHASH%'); TOOLtooltipLink('Save the highlighted variable.', null, this);\" onmouseout=\"javascript:setDefaultCursor(this); TOOLtooltipClose();\"/></a>";
var RawCancelButtonHTML = "<a href=\"javascript:onCancel('%NAMEHASH%');\"><img id=\"cancel_%NAMEHASH%\" class=\"image_large_button\" src=\"images/cancel.gif\" onmouseover=\"javascript:setPointerCursor(this, '%NAMEHASH%'); TOOLtooltipLink('Cancel the edit.', null, this);\" onmouseout=\"javascript:setDefaultCursor(this); TOOLtooltipClose();\" /></a>";
var RawSplitPathButtonHTML = "<input id=\"format_%NAMEHASH%\" class=\"checkbox\" type=\"checkbox\" onclick=\"javascript:onFormatValue('%NAMEHASH%');\" /> SPLIT";
var RawValueEditTextHTML = "<textarea id=\"value_%NAMEHASH%\" class=\"variable_value_edit\" onfocus=\"javascript:resetFieldHeight(this);expandValue('%NAMEHASH%');\" onkeyup=\"javascript:adjustValueForSplit(event);adjustFieldHeight(this);expandValue('%NAMEHASH%');\" >%VALUE%</textarea>";
var RawNameEditTextHTML = "<textarea id=\"name_%NAMEHASH%\" class=\"variable_name_edit\" onfocus=\"javascript:resetFieldHeight(this);\" onkeyup=\"javascript:adjustFieldHeight(this);\">%NAME%</textarea>";

var NewSaveButtonHTML = "<a href=\"javascript:onSaveNew();\"><img id=\"save_%NAME%\" class=\"image_large_button\" src=\"images/save.gif\" onmouseover=\"javascript:setPointerCursor(this); TOOLtooltipLink('Save the highlighted variable.', null, this);\" onmouseout=\"javascript:setDefaultCursor(this); TOOLtooltipClose();\"/></a>";
var NewCancelButtonHTML = "<a href=\"javascript:onCancelNew();\"><img id=\"cancel_%NAME%\" class=\"image_large_button\" src=\"images/cancel.gif\" onmouseover=\"javascript:setPointerCursor(this); TOOLtooltipLink('Cancel the edit.', null, this);\" onmouseout=\"javascript:setDefaultCursor(this); TOOLtooltipClose();\"/></a>";
var NewSplitPathButtonHTML = "<input id=\"format\" class=\"checkbox\" type=\"checkbox\" onclick=\"javascript:onFormatValue();\" /> SPLIT";
var NewValueEditTextHTML = "<textarea id=\"value\" class=\"variable_value_edit\" onfocus=\"javascript:resetFieldHeight(this);expandValue();\" onkeyup=\"javascript:adjustValueForSplit(event);adjustFieldHeight(this);expandValue();\" ></textarea>";

// Preview Controls.
var RawPreviewHTML = "<textarea id=\"preview_%NAMEHASH%\" class=\"path_preview\" readonly></textarea>";
var NewPreviewHTML = "<textarea id=\"preview\" class=\"path_preview\" readonly></textarea>";

// Button Cell.
var RawButtonsNonEditCellHTML = "<div id=\"list_item_buttons_%NAMEHASH%\" class=\"clearfix\"><div id=\"delete_div_%NAMEHASH%\" style=\"float:right;\">" + RawDeleteButtonHTML + "&nbsp;</div><div id=\"revert_div_%NAMEHASH%\" style=\"float:right;\">" + RawRevertButtonHTML + "&nbsp;</div></div>";
var RawButtonsEditCellHTML = "<div id=\"list_item_buttons_edit_form_%NAMEHASH%\" class=\"clearfix\" style=\"display:none;\"><div id=\"cancel_div_%NAMEHASH%\" style=\"float:right;\">" + RawCancelButtonHTML + "&nbsp;</div><div id=\"save_div_%NAMEHASH%\" style=\"float:right;\">" + RawSaveButtonHTML + "&nbsp;</div><br><br><br><div id=\"split_path_%NAMEHASH%\" class=\"split_path\" style=\"float:right;\">" + RawSplitPathButtonHTML + "&nbsp;</div></div>";
var RawButtonsCellHTML = RawButtonsNonEditCellHTML + RawButtonsEditCellHTML;
var RawButtonsDisabledCellHTML = "<div id=\"list_item_buttons_%NAMEHASH%\" class=\"clearfix\"><div id=\"delete_div_%NAMEHASH%\" style=\"float:right;\" onmouseover=\"TOOLtooltipLink('You do not have the necessary permissions to delete this environment variable.', null, this);\" onmouseout=\"TOOLtooltipClose();\" onmouseup=\"TOOLtooltipClose();\"><img id=\"delete_%NAMEHASH%\" class=\"image_button\" src=\"images/ignore_disabled.gif\" />&nbsp;</div><div id=\"revert_div_%NAMEHASH%\" style=\"float:right;\" onmouseover=\"TOOLtooltipLink('You do not have the necessary permissions to revert this environment variable.', null, this);\" onmouseout=\"TOOLtooltipClose();\" onmouseup=\"TOOLtooltipClose();\"><img id=\"revert_%NAMEHASH%\" class=\"image_button\" src=\"images/revert_disabled.gif\" />&nbsp;</div></div>";

var NewButtonsCellHTML = "<div id=\"cancel_div\" style=\"float:right;\" class=\"clearfix\">" + NewCancelButtonHTML + "&nbsp;</div><div id=\"save_div\" style=\"float:right;\">" + NewSaveButtonHTML + "&nbsp;</div><br><br><div id=\"split_path\" class=\"split_path\" style=\"float:right;\">" + NewSplitPathButtonHTML + "&nbsp;</div>";

// Value Cell.
var RawValueNonEditHTML = "<div id=\"list_item_value_%NAMEHASH%\">%WRAPPEDEXPANDEDVALUE%<input type=\"hidden\" id=\"list_item_value_hidden_%NAMEHASH%\" value=\"%ENCODEDVALUE%\" /></div>";
var RawValueEditHTML = "<div id=\"list_item_value_edit_form_%NAMEHASH%\" class=\"variable_value_edit\" style=\"display:none;\">" + RawValueEditTextHTML + "</div><div id=\"list_item_value_edit_preview_%NAMEHASH%\" style=\"display:none;\"><br>" + RawPreviewHTML + "</div>";
var RawValueCellHTML = RawValueNonEditHTML + RawValueEditHTML + "<input id=\"mask_%NAMEHASH%\" type=\"hidden\" value=\"\">";

var NewValueCellHTML = "<div id=\"list_item_value_edit_form\" class=\"variable_value_edit\">" + NewValueEditTextHTML + "</div><div id=\"list_item_value_edit_preview\" style=\"display:none;\"><br>" + NewPreviewHTML + "</div>";

// Name Cell.
var RawNameNonEditHTML = "<div id=\"list_item_name_%NAMEHASH%\">%WRAPPEDNAME%<input type=\"hidden\" id=\"list_item_name_hidden_%NAMEHASH%\" value=\"%ENCODEDNAME%\" /></div>";
var RawNameEditHTML = "<div id=\"list_item_name_edit_form_%NAMEHASH%\" style=\"width=100%;display:none;\">" + RawNameEditTextHTML + "</div>";
var RawNameCellHTML = RawNameNonEditHTML + RawNameEditHTML;

var NewNameCellHTML = "<div id=\"list_item_name_edit_form\" style=\"width=100%;\"><textarea class=\"variable_name_edit\" id=\"name\" onfocus=\"javascript:resetFieldHeight(this);\" onkeyup=\"javascript:adjustFieldHeight(this);\"></textarea></div>";

// Denotes the view selected: 0 = show both deleted and not deleted, 1 = show not deleted only and 2 = show deleted only.
var ShowDeleted = 0;

// Denotes whether we are in the process of creating a new variable.
var IsNewVariableFormVisible = false;

// Because the Environment page deals with strings that may not have natural line breaks in the text, we need to manually add them.
// In order to do this, we need to determine the line feed char for the current browser.
var BrowserDetails = navigator.userAgent;
var WrapChar = "&#8203;";

// Shows the tooltip for the currently highlighted row.
function showToolTip(NameHash)
{
   var MaskedValue = document.getElementById("mask_" + NameHash).value;

   if("" != MaskedValue)
   {
      TOOLtooltipLink
      (
         "<table class=\"ComponentToolTip\"><tr><th class=\"ToolTipHeading\">" +
         "System environment variable has been modified for Iguana" +
         "</th></tr><tr><td><div style=\"float:left; padding-right:10px; font-weight:bold; color: #505040;\">Original Value</div><div style=\"float:left;\">" +
         MaskedValue + "</div></td></tr></table>",
         null,
         document.getElementById("row_" + NameHash).cells[0]
      );
   }
}

// Closes any open tooltip.
function closeToolTip(NameHash)
{
   TOOLtooltipClose();
}

// Sets the cursor to pointer for elements in the currently highlight row.
function setPointerCursor(LinkField, NameHash)
{
   if(IsNewVariableFormVisible)
   {
      if(undefined == NameHash)
      {
         LinkField.style.cursor = "pointer";
      }
   }
   else
   {
      if(NameHash == SelectedNameHash || "" == SelectedNameHash)
      {
         LinkField.style.cursor = "pointer";
      }
   }
}

// Sets the cursor to pointer for elements in the side panel.
function setSidePanelPointerCursor(LinkField)
{
   if(!IsNewVariableFormVisible && "" == SelectedNameHash)
   {
      LinkField.style.cursor = "pointer";
   }
}

// Sets the cursor to default image.
function setDefaultCursor(LinkField)
{
   LinkField.style.cursor = "default";
}

// Function determines if the current value is an aggregate of multiple path separated by the system define separator.
// Used by setSplitPath AJAX.
function isPath(Value)
{
   var RegExp = "";
   var FileRegExp = "";

   if(":" == "<?cs var:PathSeparator ?>")
   {
      FileRegExp = "[^\\#\\$\\&;:/\\*\\?\"'\\<\\>\\|\\(\\)~`]+";

      var PathRegExp = "(/(" + FileRegExp + "/)*" + FileRegExp + "/?|\\.|\\.\\.)";

      RegExp = "^(" + PathRegExp + ":)*" + PathRegExp + ":?$" ;
   }
   else if(";" == "<?cs var:PathSeparator ?>")
   {
      FileRegExp = "[^=\\^\\&\\\\/:\\*\\?\"\\<\\>\\|;,]+";

      var EnvironmentVariableRegExp = "%" + FileRegExp + "%";
      var DriveRegExp = "[A-Za-z]:";
      var PathRegExp = "[" + DriveRegExp + "\\\\([" + FileRegExp + "|" + EnvironmentVariableRegExp + "]*\\\\)*[" + FileRegExp + "|" + EnvironmentVariableRegExp + "]*\\\\?|" + EnvironmentVariableRegExp + "\\\\([" + FileRegExp + "|" + EnvironmentVariableRegExp + "]*\\\\)*[" + FileRegExp + "|" + EnvironmentVariableRegExp + "]*\\\\?|" + EnvironmentVariableRegExp + "\\\\?|" + DriveRegExp + "\\\\?|!\\w+\\.!\\w+|!\\w+\\.\\.!\\w+]";

      RegExp = "^(" + PathRegExp + "?;)*" + PathRegExp + "?;?$";
   }

   if(Value.match(RegExp))
   {
      return true;
   }
   else
   {
      return false;
   }
}

// Generates the value cell HTML from the value cell template defined above.
function generateValueCellHTML(Name, ExpandedValue, Value, NameHash) {
   var EncodedName = encodeURIComponent(Name);
   var EncodedValue = encodeURIComponent(Value);
   var ValueCellHTML = "";

   if (undefined != Name) {
      var WrappableExpandedValue = ExpandedValue;
      var Re = new RegExp(/(.*?)/g);
      if (Re.unicode != undefined) {
         Re = new RegExp('(.*?)', 'gu');
      }
      WrappableExpandedValue = WrappableExpandedValue.replace(Re, WrapChar);
      WrappableExpandedValue = WrappableExpandedValue.replace(/\$&#8203;/g, "$");
      ValueCellHTML = RawValueCellHTML;
      ValueCellHTML = ValueCellHTML.replace(/%NAME%/g, Name);
      ValueCellHTML = ValueCellHTML.replace(/%EXPANDEDVALUE%/g, ExpandedValue);
      ValueCellHTML = ValueCellHTML.replace(/%WRAPPEDEXPANDEDVALUE%/g, WrappableExpandedValue);
      ValueCellHTML = ValueCellHTML.replace(/%VALUE%/g, Value);
      ValueCellHTML = ValueCellHTML.replace(/%NAMEHASH%/g, NameHash);
      ValueCellHTML = ValueCellHTML.replace(/%ENCODEDNAME%/g, EncodedName);
      ValueCellHTML = ValueCellHTML.replace(/%ENCODEDVALUE%/g, EncodedValue);
   } else {
      ValueCellHTML = NewValueCellHTML;
   }
   return ValueCellHTML;
}

// Generates the value cell HTML from the value cell template defined above.
function generateNameCellHTML(Name, NameHash)
{
   var EncodedName = encodeURIComponent(Name);

   var NameCellHTML = "";

   if(undefined != Name)
   {
      var WrappableName = Name;

      WrappableName = Name.replace(/(.*?)/g, WrapChar);
      WrappableName = WrappableName.replace(/\$&#8203;/g, "$");

      NameCellHTML = RawNameCellHTML;
      NameCellHTML = NameCellHTML.replace(/%NAME%/g, Name);
      NameCellHTML = NameCellHTML.replace(/%WRAPPEDNAME%/g, WrappableName);
      NameCellHTML = NameCellHTML.replace(/%NAMEHASH%/g, NameHash);
      NameCellHTML = NameCellHTML.replace(/%ENCODEDNAME%/g, EncodedName);
   }
   else
   {
      NameCellHTML = NewNameCellHTML;
   }

   return NameCellHTML;
}

// Generates the buttons cell HTML from the buttons cell template defined above.
function generateButtonsCellHTML(Name, NameHash)
{
   var ButtonsCellHTML = "";

   if(undefined != Name)
   {
      <?cs if:CurrentUserCanAdmin ?>
      ButtonsCellHTML = RawButtonsCellHTML;
      <?cs else ?>
      ButtonsCellHTML = RawButtonsDisabledCellHTML;
      <?cs /if ?>

      ButtonsCellHTML = ButtonsCellHTML.replace(/%NAME%/g, Name);
      ButtonsCellHTML = ButtonsCellHTML.replace(/%NAMEHASH%/g, NameHash);
   }
   else
   {
      ButtonsCellHTML = NewButtonsCellHTML;
   }

   return ButtonsCellHTML;
}

// Handler for "Split path" state changes.
function onFormatValue(NameHash)
{
   var ValueFieldId = "";
   var SplitPathId = "";
   var SplitPathDivId = "";

   if(undefined != NameHash)
   {
      ValueFieldId = "value_" + NameHash;
      SplitPathId = "format_" + NameHash;
      SplitPathDivId = "split_path_" + NameHash;
   }
   else
   {
      ValueFieldId = "value";
      SplitPathId = "format";
      SplitPathDivId = "split_path";
   }

   var ValueField = document.getElementById(ValueFieldId);
   var Value = ValueField.value;


   // Trim white space from ends.
   ValueField.value = Value.replace(/^\s*/g, "").replace(/\s*$/g, "");

   // If the format checkbox is selected then format the value field contents
   // such that ";" (Windows) or ":" (POSIX) delimiter represents the end of line.
   // Otherwise, unformat it.
   if(document.getElementById(SplitPathId).checked)
   {
	  ValueField.value = formatValue(Value);
   }
   else
   {
      ValueField.value = unFormatValue(Value);
   }

   resetFieldHeight(ValueField);
}

// Removes the delimiter ';' from the inputted path string and replaces it with a line break.
function formatValue(Value)
{
   var FormattedValue = Value;
   return FormattedValue.replace(/<?cs var:PathSeparator ?>/g, "\n");
}

// Reverses the formatPath function.
function unFormatValue(Value)
{
   var UnFormattedValue = Value;
   return UnFormattedValue.replace(/[\r]?\n/g, "<?cs var:PathSeparator ?>");
}

// Denotes if we are dealing with a server running on Windows or not.
var IsWindowsServer = true;

// Adds a row to the variable table.
function addVariableListRow (Name, ExpandedValue, Value, MaskedValue, NameHash, Index, IsMasked)
{
   // We don't want to add variables that have no values on Window only.
   if(IsWindowsServer && Value == "" && MaskedValue == "")
   {
      return;
   }

   var tbl = document.getElementById("list_body");

   var row;

   // If index is -1 we add the row to the end.  Otherwise we add the row at the specified location.
   if(-1 != Index)
   {
      row = tbl.insertRow(Index);
   }
   else
   {
      row = tbl.insertRow(tbl.rows.length)
   }

   // Setting some properties of the row.
   row.id = "row_" + NameHash;

   row.onmouseover = function(){ onVariableRowEntered(NameHash); showToolTip(NameHash); };
   row.onmouseup = function(){ closeToolTip(NameHash); };

   // Rows at contain variables that are masking global values must look different.
   // Also we want to retain its masked value.
   if(IsMasked)
   {
      if("" != Value)
      {
         row.className = "masked_variable_row";  // italic
      }
      else
      {
         row.className = "deleted_variable_row"; // strike through
      }
   }
   else
   {
      row.className = "variable_row";
   }

   // Now for the cells: name, value and buttons cells in particular
   var cellName = row.insertCell(0);
   var cellValue = row.insertCell(1);
   var cellButtons = row.insertCell(2);

   // Name cell.
   cellName.innerHTML = generateNameCellHTML(Name, NameHash);
   cellName.className = "variable_name";

   <?cs if:CurrentUserCanAdmin ?>
      cellName.onclick = function() { onEdit(NameHash, cellName); }
   <?cs /if ?>

   // Value cell.
   if("" != Value)
   {
      cellValue.innerHTML = generateValueCellHTML(Name, ExpandedValue, Value, NameHash);
   }
   else
   {
      cellValue.innerHTML = generateValueCellHTML(Name, MaskedValue, MaskedValue, NameHash);
   }

   cellValue.className = "variable_value";

   if(IsMasked)
   {
      document.getElementById("mask_" + NameHash).value = MaskedValue;
   }
   else
   {
      document.getElementById("mask_" + NameHash).value = "";
   }


   <?cs if:CurrentUserCanAdmin ?>
      cellValue.onclick = function() { onEdit(NameHash, cellValue); }
   <?cs /if ?>

   // Buttons cell.
   cellButtons.innerHTML = generateButtonsCellHTML(Name, NameHash);

   if("" == MaskedValue)
   {
      document.getElementById("delete_" + NameHash).src = "../images/delete_disabled.gif";
   }

   cellButtons.className = "variable_buttons";

   tbl.focus();
}

// Removes a row from the variable table.
function removeVariableListRow(Name, MaskedValue, NameHash, IsMasked)
{
   var row = document.getElementById("row_" + NameHash);

   if(!IsMasked)
   {
      row.parentNode.removeChild(row);
      return;
   }
   else
   {
      row.className = "deleted_variable_row";
   }

   // Update the Name cell.
   row.cells[0].innerHTML = generateNameCellHTML(Name, NameHash);
   row.cells[0].className = "variable_name";

   <?cs if:CurrentUserCanAdmin ?>
      row.cells[0].onclick = function() { onEdit(NameHash, row.cells[0]); }
   <?cs /if ?>

   // Update the Value cell.
   row.cells[1].innerHTML = generateValueCellHTML(Name, MaskedValue, MaskedValue, NameHash);
   document.getElementById("mask_" + NameHash).value = MaskedValue;

   row.cells[1].className = "variable_value";

   <?cs if:CurrentUserCanAdmin ?>
      row.cells[1].onclick = function() { onEdit(NameHash, row.cells[1]); }
   <?cs /if ?>

   // Update the Button cell.
   row.cells[2].innerHTML = generateButtonsCellHTML(Name, NameHash);

   if("" == MaskedValue)
   {
      document.getElementById("delete_" + NameHash).src = "../images/delete_disabled.gif";
   }

   onToggleShowDeleted();

   var tbl = document.getElementById("list_body");
   tbl.focus();
}

// Determines the row index of the highlighted variable.
function findRowIndex(NameHash)
{
   var tbl = document.getElementById("list_body");

   for(var RowIndex = 0; RowIndex < tbl.rows.length; ++RowIndex)
   {
      if(tbl.rows[RowIndex].id == "row_" + NameHash)
      {
         return RowIndex;
      }
   }

   return -1;
}

// Updates a row in the variable table.
function updateVariableListRow(Name, ExpandedValue, Value, MaskedValue, NameHash, IsMasked)
{
   // Get the row.
   var row = document.getElementById("row_" + NameHash);

   if(null == row)   // If not found then add it.
   {
      // First row is no_results so we start at one.
      var RowIndex = 1;

      var tbl = document.getElementById("list_body");

      if(1 < tbl.rows.length)
      {

         for(var CurrentNameHash = tbl.rows[RowIndex].id.replace("row_", ""); Name.toLowerCase() >= decodeURIComponent(document.getElementById("list_item_name_hidden_" + CurrentNameHash).value).toLowerCase(); CurrentNameHash = tbl.rows[RowIndex].id.replace("row_", ""))
         {
            // Ok move on to the next row.
            ++RowIndex;
            if(RowIndex >= tbl.rows.length)
            {
               break;
            }
         }
      }

      addVariableListRow(Name, ExpandedValue, Value, MaskedValue, NameHash, RowIndex, false);

      return;
   }
   else
   {
      if(IsWindowsServer)
      {
         if("" == Value && !IsMasked)
         {
            row.parentNode.removeChild(row);
            return;
         }
         else
         {
            if("" == Value && IsMasked)
            {
               row.className = "deleted_variable_row";
            }
            else if("" != Value && IsMasked)
            {
               row.className = "masked_variable_row";
            }
            else
            {
               row.className = "variable_row";
            }
         }
      }
      else
      {
         if(IsMasked)
         {
            row.className = "masked_variable_row";
         }
         else
         {
            row.className = "variable_row";
         }
      }

      // Update the Name cell.
      row.cells[0].innerHTML = generateNameCellHTML(Name, NameHash);
      row.cells[0].className = "variable_name";

      <?cs if:CurrentUserCanAdmin ?>
         row.cells[0].onclick = function() { onEdit(NameHash, row.cells[0]); }
      <?cs /if ?>

      // Update the Value cell.
      if(IsWindowsServer && "" == Value)
      {
         row.cells[1].innerHTML = generateValueCellHTML(Name, MaskedValue, MaskedValue, NameHash);
      }
      else
      {
         row.cells[1].innerHTML = generateValueCellHTML(Name, ExpandedValue, Value, NameHash);
      }

      if(IsMasked)
      {
         document.getElementById("mask_" + NameHash).value = MaskedValue;
      }
      else
      {
         document.getElementById("mask_" + NameHash).value = "";
      }

      row.cells[1].className = "variable_value";

      <?cs if:CurrentUserCanAdmin ?>
         row.cells[1].onclick = function() { onEdit(NameHash, row.cells[1]); }
      <?cs /if ?>

      // Update the Button cell.
      row.cells[2].innerHTML = generateButtonsCellHTML(Name, NameHash);

      if("" == MaskedValue)
      {
         document.getElementById("delete_" + NameHash).src = "../images/delete_disabled.gif";
      }

      onToggleShowDeleted();

      var tbl = document.getElementById("list_body");
      tbl.focus();
   }
}

// Finds and refreshes variable dependencies in the table.
function refreshVariableListRow(Name, ExpandedValue, Value, NameHash)
{
   var MaskedValue = document.getElementById("mask_" + NameHash).value;

   updateVariableListRow(Name, ExpandedValue, Value, MaskedValue, NameHash, MaskedValue != "");
}

// Denotes the currently selected variable for editing.
var SelectedNameHash = "";

// Denotes the currently highlighted variable.
var HoveredNameHash = "";

// Denotes the name and value of the currently selected variable for editing.
var ValueBeforeEdit = "";
var NameBeforeEdit = "";

// 'NEW VARIABLE' button click handler.
function onNew()
{
   checkServerConnection();

   // If some other variables was being edited, then prevent this edit.
   if("" != SelectedNameHash || IsNewVariableFormVisible)
   {
      return;
   }

   // Change the cursor to indicate we are in edit mode.
   var tbl = document.getElementById("list_body");
   tbl.style.cursor = "default";

   // Insert new row and show the edit forms.
   var tbl = document.getElementById("list_body");
   var row = tbl.insertRow(0);

   row.onmouseover = function(){ onVariableRowEntered(); };
   row.onmouseout = function(){ onVariableRowExited(); };

   row.className = "variable_row";

   // Now for the cells: name, value and buttons cells in particular
   var cellName = row.insertCell(0);
   var cellValue = row.insertCell(1);
   var cellButtons = row.insertCell(2);

   // The new Name cell.
   cellName.innerHTML = generateNameCellHTML();
   cellName.className = "variable_name_edit";

   // Set the size of the name edit field.
   var NameField = document.getElementById("name");

   var tbl = document.getElementById("list_floating_header_table");
   var row = tbl.rows[0];

   NameField.style.width = (row.cells[0].clientWidth - 30) + "px";

   // The new Value cell.
   cellValue.innerHTML = generateValueCellHTML();
   cellValue.className = "variable_value_edit";

   // Set the size of the value edit field.
   var ValueField = document.getElementById("value");

   ValueField.style.width = (row.cells[1].clientWidth - 30) + "px";

   // The new Buttons cell.
   cellButtons.innerHTML = generateButtonsCellHTML();
   cellButtons.className = "variable_buttons";

   document.getElementById("list").scrollTop = 0;
   document.getElementById("list_body").scrollTop = 0;

   onVariableRowEntered();

   // I know this code looks weird, but it is the only way I found to get the caret to show up in FF 3.5.
   document.getElementById("value").focus();
   document.getElementById("name").focus();

   document.getElementById("value").focus();
   document.getElementById("name").focus();

   IsNewVariableFormVisible = true;
}

// Click handler for the 'SAVE' button in the new variable form.
function onSaveNew()
{
   var NewName = document.getElementById("name").value;
   var NewValue = document.getElementById("value").value;

   if(IsWindowsServer && "" == NewValue)
   {
      alert("This update will delete the variable if it exists. If the variable does not exist, this update has no effect and will be discarded.");
   }

   // Trim leading and trailing white space.
   NewName = NewName.replace(/^\s*/g, "").replace(/\s*$/g, "");
   NewValue = NewValue.replace(/^\s*/g, "").replace(/\s*$/g, "");

   // Unformat the value field contents if necessary.
   if(document.getElementById("format").checked)
   {
      NewValue = unFormatValue(NewValue);
   }

   // New name cannot contain white space or equal signs.
   if (!NewName.match(/^[^\s=]+$/))
   {
      alert("The variable name cannot contain white space or equal signs.  Please remove the illegal character(s) and try again.");
      return;
   }

   // Set the new variable.
   setVariable(NewName, NewValue, NewName);

   onCancelNew();
}

// Click handler for the 'CANCEL' button in the new variable form.
function onCancelNew()
{
   checkServerConnection();

   // Change the cursor to indicate we are leaving edit mode.
   var tbl = document.getElementById("list_body");
   tbl.style.cursor = "pointer";
   tbl.focus();

   // Remove the row from the list.
   var tbl = document.getElementById("list_body");
   var row = tbl.rows[0];

   row.parentNode.removeChild(row);

   IsNewVariableFormVisible = false;

   // Scroll bar visibilty may change so we need to resize the floating header.
   onResize(VisibleArea, VisibleTable, VisibleHeaderArea, VisibleHeaderTable);
}

// Edit button click handler.
function onEdit(NameHash, CellClicked)
{
   // If some other variables was being edited, then prevent this edit.
   if(("" != SelectedNameHash && NameHash != SelectedNameHash) || IsNewVariableFormVisible)
   {
      return;
   }

   // Change the cursor to indicate we are in edit mode.
   var tbl = document.getElementById("list_body");
   tbl.style.cursor = "default";

   // Get data in the row and then call the AJAX function.
   var row = document.getElementById("row_" + NameHash);

   if("deleted_variable_row" == row.className)
   {
      row.className = "deleted_variable_row_edit";
   }

   // Prepare the Name cell for editing.
   row.cells[0].onclick = "";
   row.cells[0].className = "variable_name_edit";

   // Prepare the Value cell for editing.
   row.cells[1].onclick = "";
   row.cells[1].className = "variable_value_edit";

   // Store the name and value before the edit.
   ValueBeforeEdit = document.getElementById("value_" + NameHash).value;
   NameBeforeEdit =  document.getElementById("name_" + NameHash).value;

   // Set the newly selected variable for editting.
   SelectedNameHash = NameHash;

   // Set the size of the name edit field.
   var NameField = document.getElementById("name_" + NameHash);

   var tbl = document.getElementById("list_floating_header_table");
   var row = tbl.rows[0];

   NameField.style.width = (row.cells[0].clientWidth - 30) + "px";

   adjustFieldHeight(NameField);

   // Show name edit form.
   document.getElementById("list_item_name_" + NameHash).style.display = "none";
   document.getElementById("list_item_name_edit_form_" + NameHash).style.display = "";

   // Set the size of the value edit field.
   var ValueField = document.getElementById("value_" + NameHash);

   ValueField.style.width = (row.cells[1].clientWidth - 30) + "px";

   adjustFieldHeight(ValueField);

   // Show value edit form.
   document.getElementById("list_item_value_" + NameHash).style.display = "none";
   document.getElementById("list_item_value_edit_form_" + NameHash).style.display = "";

   // Show the edit buttons.
   document.getElementById("list_item_buttons_" + NameHash).style.display = "none";
   document.getElementById("list_item_buttons_edit_form_" + NameHash).style.display = "";

   // See onNew for explaination of this series of focus calls.
   document.getElementById("value_" + NameHash).focus();
   document.getElementById("name_" + NameHash).focus();

   document.getElementById("value_" + NameHash).focus();
   document.getElementById("name_" + NameHash).focus();

   // Determine which cell was clicked.  That will be the cell with focus.
   // Note: we use the class name so do this before we change it below.
   var FieldIdToFocus = "";
   if("variable_name_edit" == CellClicked.className)
   {
      FieldIdToFocus = "name_" + NameHash;
   }
   else if("variable_value_edit" == CellClicked.className)
   {
      FieldIdToFocus = "value_" + NameHash;
   }

   // Now focus on the one we want.
   document.getElementById(FieldIdToFocus).focus();

   // Scroll bar visibilty may change so we need to resize the floating header.
   onResize(VisibleArea, VisibleTable, VisibleHeaderArea, VisibleHeaderTable);

   // Determine the default state of the split checkbox.
   setSplitState(NameHash);
}

// 'SAVE' button click handler.
function onSave(NameHash)
{
   // Change the cursor to indicate we leaving edit mode.
   var tbl = document.getElementById("list_body");
   tbl.style.cursor = "pointer";

   var ValueField = document.getElementById("value_" + NameHash);
   var NameField = document.getElementById("name_" + NameHash);

   if(IsWindowsServer && "" == ValueField.value)
   {
      alert("This update will delete the variable if it exists. If the variable does not exist, this update has no effect and will be discarded.");
   }

   // Unformat the value field contents if necessary.
   if(document.getElementById("format_" + NameHash).checked)
   {
      ValueField.value = unFormatValue(ValueField.value);
   }

   // Trim leading and trailing white space.
   NameField.value = NameField.value.replace(/^\s*/g, "").replace(/\s*$/g, "");
   ValueField.value = ValueField.value.replace(/^\s*/g, "").replace(/\s*$/g, "");

   // New name cannot contain white space or equal signs.
   if (!NameField.value.match(/^[^\s=]+$/))
   {
      alert("The variable name cannot contain white space or equal signs.  Please remove the illegal character(s) and try again.");
      return;
   }

   // Get the new value and set the variable.
   setVariable(NameField.value, ValueField.value, NameBeforeEdit);
}

// 'CANCEL' button click handler.
function onCancel(NameHash)
{
   checkServerConnection();

   // Change the cursor to indicate we leaving edit mode.
   var tbl = document.getElementById("list_body");
   tbl.style.cursor = "pointer";
   tbl.focus();

   var ValueField = document.getElementById("value_" + NameHash);
   var NameField = document.getElementById("name_" + NameHash);

   ValueField.value = ValueBeforeEdit;
   NameField.value = NameBeforeEdit;

   // Get data in the row and then call the AJAX function.
   var row = document.getElementById("row_" + NameHash);

   // Reset the cells as they were before the edit.

   // The Name cell.
   row.cells[0].className = "variable_name";
   row.cells[0].onclick = function() { onEdit(NameHash, row.cells[0]); };

   // The Value cell.
   row.cells[1].className = "variable_value";
   row.cells[1].onclick = function() { onEdit(NameHash, row.cells[1]); };

   // Hide the preview.
   document.getElementById("list_item_value_edit_preview_" + NameHash).style.display = "none";

   // Hide the edit forms.
   document.getElementById("list_item_name_edit_form_" + NameHash).style.display = "none";
   document.getElementById("list_item_name_" + NameHash).style.display = "";

   document.getElementById("list_item_value_edit_form_" + NameHash).style.display = "none";
   document.getElementById("list_item_value_" + NameHash).style.display = "";

   document.getElementById("list_item_buttons_edit_form_" + NameHash).style.display = "none";
   document.getElementById("list_item_buttons_" + NameHash).style.display = "";

   // Reset the class for deleted variables.
   if("deleted_variable_row_edit" == row.className)
   {
      row.className = "deleted_variable_row";
   }

   // Indicate that we are finished with this variable by unsetting the globals.
   SelectedNameHash = "";
   ValueBeforeEdit = "";
   NameBeforeEdit = "";

   // Scroll bar visibilty may change so we need to resize the floating header.
   onResize(VisibleArea, VisibleTable, VisibleHeaderArea, VisibleHeaderTable);

}

// Delete button click handler.
function onDelete(NameHash)
{
   <?cs if:CurrentUserCanAdmin ?>

   // If some other variables was being edited, then prevent this edit.
   if(("" != SelectedNameHash && NameHash != SelectedNameHash) || IsNewVariableFormVisible)
   {
      return;
   }

   var row = document.getElementById("row_" + NameHash);

   // Can't delete something that is already deleted.
   if("deleted_variable_row" == row.className)
   {
      return;
   }

   // Remove focus for non-system variables as they will disappear.
   var ImageSrc = document.getElementById("delete_" + NameHash).src;
   if(ImageSrc.indexOf("ignore") < 0)
   {
      onVariableRowExited(NameHash);
   }

   // Set the variable to "" will remove it on Windows.
   var Name = decodeURIComponent(document.getElementById("list_item_name_hidden_" + NameHash).value);
   deleteVariable(Name, "", Name);

   <?cs /if ?>
}

// Revert button click handler.
function onRevert(NameHash)
{
   <?cs if:CurrentUserCanAdmin ?>

   // If some other variables was being edited, then prevent this edit.
   if(("" != SelectedNameHash && NameHash != SelectedNameHash) || IsNewVariableFormVisible)
   {
      return;
   }

   var row = document.getElementById("row_" + NameHash);

   // Can't revert is there's nothing to revert to.
   if("" == document.getElementById("mask_" + NameHash).value)
   {
      return;
   }

   // We can revert variables to their masked value.
   var Name = decodeURIComponent(document.getElementById("list_item_name_hidden_" + NameHash).value);
   setVariable(Name, document.getElementById("mask_" + NameHash).value, Name);

   <?cs /if ?>
}

// Functions that changes the row color on a mouseover.
function onVariableRowEntered(NameHash)
{
   // If some other variables was being edited, then prevent this edit.
   if("" != SelectedNameHash || IsNewVariableFormVisible)
   {
      return;
   }

   if("" != HoveredNameHash && null != HoveredNameHash)
   {
      onVariableRowExited(HoveredNameHash);
   }

   if(undefined != NameHash)
   {
      document.getElementById("row_" + NameHash).style.backgroundColor = "#dcedc8";

      HoveredNameHash = NameHash;

      <?cs if:CurrentUserCanAdmin ?>

      var row = document.getElementById("row_" + NameHash);

      // Can't delete something that is already deleted.
      if("deleted_variable_row" != row.className)
      {
         var ImageSrc = document.getElementById("delete_" + NameHash).src;
         if(ImageSrc.indexOf("ignore") < 0)
         {
            document.getElementById("delete_" + NameHash).src = "../images/delete.gif";
         }
         else
         {
            document.getElementById("delete_" + NameHash).src = "../images/ignore.gif";
         }
      }

      if("" != document.getElementById("mask_" + NameHash).value)
      {
         document.getElementById("revert_" + NameHash).src = "../images/revert.gif";
      }

      <?cs /if ?>
   }
   else // New variables satisfy this case.
   {
      var tbl = document.getElementById("list_body");
      tbl.rows[0].style.backgroundColor = "#dcedc8";

      HoveredNameHash = null;
   }
}

// Functions that changes the row color on a mouseexit.
function onVariableRowExited(NameHash)
{
   // If some other variables was being edited, then prevent this edit.
   if("" != SelectedNameHash || IsNewVariableFormVisible)
   {
      return;
   }

   if(undefined != NameHash && null != document.getElementById("delete_" + NameHash))
   {
      var ImageSrc = document.getElementById("delete_" + NameHash).src;
      if(ImageSrc.indexOf("ignore") < 0)
      {
         document.getElementById("delete_" + NameHash).src = "../images/delete_disabled.gif";
      }
      else
      {
         document.getElementById("delete_" + NameHash).src = "../images/ignore_disabled.gif";
      }

      document.getElementById("revert_" + NameHash).src = "../images/revert_disabled.gif";

      document.getElementById("row_" + NameHash).style.backgroundColor = "#F9F9F9";
   }
   else // New variables satisfy this case.
   {
      if(IsNewVariableFormVisible)
      {
         var tbl = document.getElementById("list_body");
         tbl.rows[0].style.backgroundColor = "#F9F9F9";
      }
   }

   HoveredNameHash = "";
}

// Moves header left and right when we scroll the horizontally.
function adjustHeaderOnScroll()
{
   VisibleHeaderTable.style.left = - VisibleArea.scrollLeft + "px";
}

// Function that adjusts the textarea size that holds the variable value.  Shrinking is not supported at this time.
function resetFieldHeight(Field)
{
   // Reset the field to the initial default value and then adjust to the correct value.
   Field.style.height = "16px";
   Field.style.overflow = "hidden";

   adjustFieldHeight(Field);
}

// Function that adjusts the textarea size that holds the variable value.  Shrinking is not supported at this time.
function adjustFieldHeight(Field)
{
   // Check if the content can fit in the current size.  If not, expand it.
   if(Field.offsetHeight < Field.scrollHeight)
   {
      if(Field.offsetWidth < Field.scrollWidth)
      {
         Field.style.overflow = "auto";
         Field.style.height = Field.scrollHeight + 32 + "px";
      }
      else
      {
         Field.style.height = Field.scrollHeight + "px";
      }
   }
}

// Shows or hides the deleted variables in the current session.
// Deleted global variables are always presented even after reloads of the page.  Thus, they can always be reverted.
// New variables created via this UI but then deleted, will be lost after the session ends.
function onToggleShowDeleted()
{

   // Show the list.
   document.getElementById("no_results").style.display = "none";

   var ShowDeleted = document.getElementById("show_deleted_variable_toggle").value;

   var tbl = document.getElementById("list_body");

   var Count = 0;
   var TotalHeight = 0;

   // Using the toggle, we determine the css class to use to either hide or show the row.
   if(ShowDeleted == 0) // Show all.
   {
      for(var RowIndex = 0; RowIndex < tbl.rows.length; ++RowIndex)
      {
   	     if("invisible_deleted_variable_row" == tbl.rows[RowIndex].className)
   	     {
   	        tbl.rows[RowIndex].className = "deleted_variable_row";
   	     }
   	     else if("invisible_masked_variable_row" == tbl.rows[RowIndex].className)
   	     {
   	        tbl.rows[RowIndex].className = "masked_variable_row";
   	     }
   	     else if("invisible_variable_row" == tbl.rows[RowIndex].className)
   	     {
   	        tbl.rows[RowIndex].className = "variable_row";
  	     }

   	     ++Count;
   	     TotalHeight += tbl.rows[RowIndex].clientHeight + 5;
   	  }


   }
   else if(ShowDeleted == 1) // Hide deleted.
   {
      for(var RowIndex = 0; RowIndex < tbl.rows.length; ++RowIndex)
      {
   	     if("deleted_variable_row" == tbl.rows[RowIndex].className)
   	     {
   	        tbl.rows[RowIndex].className = "invisible_deleted_variable_row";
   	     }
   	     else if("invisible_masked_variable_row" == tbl.rows[RowIndex].className)
   	     {
   	        tbl.rows[RowIndex].className = "masked_variable_row";

   	        ++Count;
   	        TotalHeight += tbl.rows[RowIndex].clientHeight + 5;
   	     }
   	     else if("invisible_variable_row" == tbl.rows[RowIndex].className)
   	     {
   	        tbl.rows[RowIndex].className = "variable_row";

   	        ++Count;
   	        TotalHeight += tbl.rows[RowIndex].clientHeight + 5;
  	     }
   	     else if("masked_variable_row" == tbl.rows[RowIndex].className)
   	     {
   	        ++Count;
   	        TotalHeight += tbl.rows[RowIndex].clientHeight + 5;
   	     }
   	     else if("variable_row" == tbl.rows[RowIndex].className)
   	     {
   	        ++Count;
   	        TotalHeight += tbl.rows[RowIndex].clientHeight + 5;
  	     }
   	  }
   }
   else if(ShowDeleted == 2) // Show deleted only.
   {
      for(var RowIndex = 0; RowIndex < tbl.rows.length; ++RowIndex)
      {
   	     if("invisible_deleted_variable_row" == tbl.rows[RowIndex].className)
   	     {
   	        tbl.rows[RowIndex].className = "deleted_variable_row";

   	        ++Count;
   	        TotalHeight += tbl.rows[RowIndex].clientHeight + 5;
   	     }
   	     else if("masked_variable_row" == tbl.rows[RowIndex].className)
   	     {
   	        tbl.rows[RowIndex].className = "invisible_masked_variable_row";
   	     }
   	     else if("variable_row" == tbl.rows[RowIndex].className)
   	     {
   	        tbl.rows[RowIndex].className = "invisible_variable_row";
  	     }
  	     else if("deleted_variable_row" == tbl.rows[RowIndex].className)
  	     {
  	        ++Count;
   	        TotalHeight += tbl.rows[RowIndex].clientHeight + 5;
  	     }
   	  }
   }

   // Hide the list if there's nothing to see.
   if(0 == Count)
   {
      document.getElementById("no_results").style.display = "";
   }
   else
   {
      document.getElementById("no_results").style.display = "none";
   }

   // Scroll bar visibilty may change so we need to resize the floating header.
   onResize(VisibleArea, VisibleTable, VisibleHeaderArea, VisibleHeaderTable);

   // Resize the table to fit the variable we want to show.
   if(TotalHeight < tbl.clientHeight)
   {
      // Now adjust the size of the table to fit the variables.
      tbl.style.height = TotalHeight + "px";
   }

}

// Denotes if we are waiting for a asynchronous AJAX call to return is data.  We want to serialize our AJAX calls.
var IsAJAXInProgress = false;

// AJAX function that sets the latest value for the inputted variable name.
function setVariable(Name, Value, OldName)
{
   if(Name.match(/^system\#.+$/))
   {
      alert("Names with the format \"system#*\" where * can be anything are not allowed.  This is because such names conflict with the system place holders used in this page to reference system variable values outside of Iguana.");
      return;
   }
   IsAJAXInProgress = true;

   var EncodedValue = encodeURIComponent(Value);
   var EncodedName = encodeURIComponent(Name);
   var EncodedOldName = encodeURIComponent(OldName);

   var AJAXCommandName;
   var AJAXVariableName;
   var AJAXVariableValue;
   var AJAXVariableOldName;

   AJAXCommandName = "set_environment_variable";
   AJAXVariableName = "Name=";
   AJAXVariableValue = "Value=";
   AJAXVariableOldName = "OldName=";

   AJAXpost(AJAXCommandName, AJAXVariableName + EncodedName + "&" + AJAXVariableValue + EncodedValue + "&" + AJAXVariableOldName + EncodedOldName,
      function(data)
      {
         try
         {
            var Response = JSON.parse(data);

            if(!Response) // Response.result represent an AJAX error.
            {
               alert("Environment variable was not set because of an OS error.");
            }
            else if(0 != Response.errorCode) // Response.errorCode represent an API error.
            {
               if(1 == Response.errorCode) // General error.
               {
                  alert("Environment variable was not set.  An unknown error has occurred.");
               }
               else if(2 == Response.errorCode)  // Bad definition.
               {
                  alert("Environment variable was not set because it is not well defined.  Please ensure there are no circular definitions.");
               }
            }
            else
            {
               // We need to get the stored name as this may be different from the inputted name
               // (i.e. under Windows, variables are case insensitive).
               var StoredName = Response.name;
               var StoredValue = Response.value;
               var ExpandedValue = Response.expandedValue;
               var MaskedValue = Response.maskedValue;
               var NameHash = Response.nameHash;
               var OldNameHash = Response.oldNameHash;
               var IsFromSystem = Response.isFromSystem;

               // Update the variable.
               updateVariableListRow(StoredName, ExpandedValue, StoredValue, MaskedValue, NameHash, IsFromSystem && StoredValue != MaskedValue);

               // If the name was changed then we also have to update the variable with the old name.
               if(Name != OldName)
               {
                  updateVariableListRow(OldName, "", "", Response.oldValue, OldNameHash, "" != Response.oldValue);
               }

               // Now update all the variables affected by the change above.
               var VariablesToRefreshCount = Response.variablesToRefreshCount;

               for(var VariablesToRefreshCountIndex = 0; VariablesToRefreshCountIndex < VariablesToRefreshCount; ++VariablesToRefreshCountIndex)
               {
                  var VariableToRefreshName = eval("Response.variableToRefreshName" + VariablesToRefreshCountIndex);
                  var VariableToRefreshExpandedValue = eval("Response.variableToRefreshExpandedValue" + VariablesToRefreshCountIndex);
                  var VariableToRefreshValue = eval("Response.variableToRefreshValue" + VariablesToRefreshCountIndex);
                  var VariableToRefreshNameHash = eval("Response.variableToRefreshNameHash" + VariablesToRefreshCountIndex);

                  refreshVariableListRow(VariableToRefreshName, VariableToRefreshExpandedValue, VariableToRefreshValue, VariableToRefreshNameHash);
               }

               // Indicate that we are finished with this variable by unsetting the globals.
               SelectedNameHash = "";
               ValueBeforeEdit = "";
               NameBeforeEdit = "";

               // Scroll bar visibilty may change so we need to resize the floating header.
               onResize(VisibleArea, VisibleTable, VisibleHeaderArea, VisibleHeaderTable);
            }
         }
         catch(err)
         {
            alert("Environment variable was not set because of an unknown error.");
         }

         IsAJAXInProgress = false;
      },

      function(data)
      {
         IsAJAXInProgress = false;
         MiniLogin.show('Iguana is not Responding', function() { setVariable(Name, Value, OldName) });
      }
   );
}

// AJAX function that deletes the inputted variable name.
function deleteVariable(Name)
{
   IsAJAXInProgress = true;

   var EncodedName = encodeURIComponent(Name);

   var AJAXCommandName;
   var AJAXVariableName;

   AJAXCommandName = "delete_environment_variable";
   AJAXVariableName = "Name=";

   AJAXpost(AJAXCommandName, AJAXVariableName + EncodedName,
      function(data)
      {
         try
         {
            var Response = JSON.parse(data);

            if(!Response) // Response.result represent an AJAX error.
            {
               alert("Environment variable was not deleted because of an OS error.");
            }
            else if(0 != Response.errorCode) // Response.errorCode represent an API error.  Currently only bad variable definitions (i.e. circular definitions)
                                             // result in an error. Hence the alert below.
            {
               alert("Environment variable was not deleted because of an OS error.");
            }
            else
            {
               var StoredName = Response.name;
               var NameHash = Response.nameHash;
               var MaskedValue = Response.maskedValue;
               var IsDeleted = Response.isDeleted;

               if(IsDeleted)
               {
                  removeVariableListRow(StoredName, MaskedValue, NameHash, "" != MaskedValue);
               }
               else
               {
                  updateVariableRowList(StoredName, "", "", MaskedValue, NameHash, "" != MaskedValue);
               }

               // Now update all the variables affected by the change above.
               var VariablesToRefreshCount = Response.variablesToRefreshCount;

               for(var VariablesToRefreshCountIndex = 0; VariablesToRefreshCountIndex < VariablesToRefreshCount; ++VariablesToRefreshCountIndex)
               {
                  var VariableToRefreshName = eval("Response.variableToRefreshName" + VariablesToRefreshCountIndex);
                  var VariableToRefreshExpandedValue = eval("Response.variableToRefreshExpandedValue" + VariablesToRefreshCountIndex);
                  var VariableToRefreshValue = eval("Response.variableToRefreshValue" + VariablesToRefreshCountIndex);
                  var VariableToRefreshNameHash = eval("Response.variableToRefreshNameHash" + VariablesToRefreshCountIndex);

                  refreshVariableListRow(VariableToRefreshName, VariableToRefreshExpandedValue, VariableToRefreshValue, VariableToRefreshNameHash);
               }

               // Indicate that we are finished with this variable by unsetting the globals.
               SelectedNameHash = "";
               ValueBeforeEdit = "";
               NameBeforeEdit = "";
            }
         }
         catch(err)
         {
            alert("Environment variable was not set because of an unknown error.");
         }

         IsAJAXInProgress = false;
      },

      function(data)
      {
         IsAJAXInProgress = false;
         MiniLogin.show('Iguana is not Responding', function() { deleteVariable(Name) });
      }
   );
}

// Expand(New|Existing)Value helper function.
function generatePreviewText(ExpandedText, UnexpandedText)
{
   var PreviewText = ExpandedText;

   if(null != PreviewText && ExpandedText != UnexpandedText)
   {
      if(2000 < PreviewText.length)
      {
         return "The resolved value is too long to display."
      }
      else
      {
         return 'Preview: "' + PreviewText + '"';
      }

   }
   else
   {
      return "";
   }
}

// AJAX function that gets the expanded value of the variable being edited.
function expandValue(NameHash)
{
   var UnexpandedValue = "";
   var PreviewId = "";
   var PreviewDivId = "";

   IsAJAXInProgress = true;

   if(undefined != NameHash)
   {
      UnexpandedValue = document.getElementById("value_" + NameHash).value;

      if(document.getElementById("format_" + NameHash).checked)
      {
         UnexpandedValue = unFormatValue(UnexpandedValue)
      }

      PreviewId = "preview_" + NameHash;
      PreviewDivId = "list_item_value_edit_preview_" + NameHash;
   }
   else
   {
      UnexpandedValue = document.getElementById("value").value;

      if(document.getElementById("format").checked)
      {
         UnexpandedValue = unFormatValue(UnexpandedValue)
      }

      PreviewId = "preview";
      PreviewDivId = "list_item_value_edit_preview";
   }

   var EncodedUnexpandedValue = encodeURIComponent(UnexpandedValue);

   if ("" == EncodedUnexpandedValue)
   {
      return;
   }

   var AJAXCommandName;
   var AJAXVariableName;
   var PreviewPrefix;

   AJAXCommandName = 'environment_expand_ext';
   AJAXVariableName = 'RawText=';

   AJAXpost(AJAXCommandName, AJAXVariableName + EncodedUnexpandedValue,
      function(data)
      {
         try
	     {
            var Response = JSON.parse(data);

            var PreviewText = "";

            if(Response)
	        {
	           PreviewText = generatePreviewText(Response.expandedText, UnexpandedValue);
 	        }

            var Preview = document.getElementById(PreviewId);
            var PreviewDiv = document.getElementById(PreviewDivId);

            Preview.value = PreviewText;

	        if("" != PreviewText)
	        {
	           PreviewDiv.style.display = "inline";

                   var HeaderTbl = document.getElementById("list_floating_header_table");
                   var HeaderRow = HeaderTbl.rows[0];

                   Preview.style.width = HeaderRow.cells[1].clientWidth - 30 + "px";

	           adjustFieldHeight(Preview);
	        }
	        else
	        {
	           PreviewDiv.style.display = "none";
            }
         }
         catch(err)
         {
            // Exception means that the response is not valid JSON.
            PreviewDiv.style.display =  "none";
         }

         IsAJAXInProgress = false;
      },

      function(data)
      {
         IsAJAXInProgress = false;
         MiniLogin.show('Iguana is not Responding', function() { expandValue(NameHash) });
      }
   );
}

// AJAX function that gets the expanded value of the variable being edited.
function setSplitState(NameHash)
{
   IsAJAXInProgress = true;

   if(undefined != NameHash)
   {
      SplitId = "format_" + NameHash;

      Value = document.getElementById("value_" + NameHash).value;

      if(document.getElementById(SplitId).checked)
      {
         Value = unFormatValue(Value)
      }
   }
   else
   {
      SplitId = "format";

      Value = document.getElementById("value").value;

      if(document.getElementById(SplitId).checked)
      {
         Value = unFormatValue(Value)
      }

   }

   var UnexpandedText = encodeURIComponent(Value);

   if ("" == UnexpandedText)
   {
      return;
   }

   var AJAXCommandName;
   var AJAXVariableName;

   AJAXCommandName = 'environment_expand';
   AJAXVariableName = 'RawText=';

   AJAXpost(AJAXCommandName, AJAXVariableName + UnexpandedText,
      function(data)
      {
         try
	     {
            var Response = JSON.parse(data);

            if(Response)
	        {
	           var ExpandedValue = Response.expandedText;

	           if(ExpandedValue.match("<?cs var:PathSeparator ?>") && isPath(ExpandedValue))
	           {
	              document.getElementById(SplitId).checked = true;

	              onFormatValue(NameHash);
	           }
	           else
	           {
   	              document.getElementById(SplitId).checked = false;
	           }
 	        }
         }
         catch(err)
         {
            // Exception means that the response is not valid JSON.
            document.getElementById(SplitId).checked = false;
         }

         IsAJAXInProgress = false;
      },

      function(data)
      {
         IsAJAXInProgress = false;
         MiniLogin.show('Iguana is not Responding', function() { setSplitState(Name) });
      }
   );
}

// AJAX function that checks the web server connection.
function checkServerConnection()
{
   if(IsAJAXInProgress)
   {
      setTimeout("checkServerConnection()", 10000);
      return;
   }

   AJAXCommandName = 'environment_expand';
   AJAXVariableName = 'RawText=' + Math.floor(Math.random()*999999);

   AJAXpost(AJAXCommandName, AJAXVariableName + '&AutomaticRequest=1',
      function(data)
      {
         var Response = JSON.parse(data);

         if(undefined != Response.LoggedIn && !Response.LoggedIn)
         {
            MiniLogin.show(Response.ErrorDescription, function() { checkServerConnection(); });
         }
         else
         {
            setTimeout("checkServerConnection()", 30000);
         }
      },

      function(data)
      {
         MiniLogin.show('Iguana is not Responding', function() { window.location = "update_environment"; });
      }
   );
}

// Handler for key up events on value edit boxes.
function adjustValueForSplit(Event)
{
   switch( window.event ? window.event.keyCode : Event.which )
   {
   case 58:  // :
   case 59:  // ;
      if(IsNewVariableFormVisible)
      {
         onFormatValue();
      }
      else
      {
         onFormatValue(SelectedNameHash);
      }
      break;
   default:
      return true;
   }

   return false;
}

// Denotes whether the Control and Shift keys are pressed respectively.
var CtrlKeyMode = false;
var ShftKeyMode = false;

// The keyup handler for the document.
function onKeyUp(Event)
{
   switch( window.event ? window.event.keyCode : Event.which )
   {
   case 16: // Shift

      ShftKeyMode = false;
      break;

   case 17: // Control

      CtrlKeyMode = false;
      break;

   default:
      return true;
   }

   return false;
}

// The keydown handler for the document.
function onKeyDown(Event)
{
   switch( window.event ? window.event.keyCode : Event.which )
   {
   case 16: // Shift
      ShftKeyMode = true;
      break;
   case 17: // Control
      CtrlKeyMode = true;
      break;

      <?cs if:CurrentUserCanAdmin ?>

   case 65:  // a

      if(!ShftKeyMode || !CtrlKeyMode)
      {
         return true;
      }

      document.getElementById("list_body").focus();

      onNew();

      break;

      <?cs /if ?>

   default:
      return processKeyDown(Event);
   }

   return false;
}

// The defered keydown handler for the document.
function processKeyDown(Event)
{
   switch( window.event ? window.event.keyCode : Event.which )
   {
   case 38:  // Up-arrow

      if(!CtrlKeyMode)
      {
         return true;
      }

      var row = null;

      var List = document.getElementById("list");
      var ListBody = document.getElementById("list_body");

      ListBody.focus();

      if("" != HoveredNameHash && null != HoveredNameHash)
      {
         var RowIndex = findRowIndex(HoveredNameHash);

         if(RowIndex > 0)
         {
            row = ListBody.rows[RowIndex - 1];

            // Bring the row into view.
            List.scrollTop = List.scrollTop - document.getElementById(row.id).clientHeight - 2;

         }
         else
         {
            row = ListBody.rows[0];
         }

         onVariableRowExited(HoveredNameHash);
      }
      else
      {
         // Bring the row into view.
         List.scrollTop = 0;

         row = ListBody.rows[0];
      }

      // Then highlight it.
      onVariableRowEntered(row.id.replace("row_", ""));

      break;

   case 40:  // Down-arrow

      if(!CtrlKeyMode)
      {
         return true;
      }

      var row = null;

      var List = document.getElementById("list");
      var ListBody = document.getElementById("list_body");

      ListBody.focus();

      if("" != HoveredNameHash && null != HoveredNameHash)
      {
         var RowIndex = findRowIndex(HoveredNameHash);

         if(RowIndex < ListBody.rows.length - 1)
         {
            // Bring the row into view.
            List.scrollTop = List.scrollTop + document.getElementById("row_" + HoveredNameHash).clientHeight + 2;

            row = ListBody.rows[RowIndex + 1];
         }
         else
         {
            row = ListBody.rows[ListBody.rows.length - 1];
         }

         onVariableRowExited(HoveredNameHash);
      }
      else
      {
         // Bring the row into view.
         List.scrollTop = 0;

         row = ListBody.rows[0];
      }

      // Then highlight it.
      onVariableRowEntered(row.id.replace("row_", ""));

      break;

      <?cs if:CurrentUserCanAdmin ?>

   case 32:  // Space

      if(!ShftKeyMode || !CtrlKeyMode)
      {
         return true;
      }

      document.getElementById("list_body").focus();

      if("" != HoveredNameHash && null != HoveredNameHash)
      {
         onEdit(HoveredNameHash, document.getElementById("row_" + HoveredNameHash).cells[1]);
      }

      break;

   case 67:  // c

      if(!ShftKeyMode || !CtrlKeyMode)
      {
         return true;
      }

      document.getElementById("list_body").focus();

      if("" != SelectedNameHash)
      {
         onCancel(SelectedNameHash);
      }
      else if(IsNewVariableFormVisible)
      {
         onCancelNew();
      }

      break;

   case 68:  // d

      if(!ShftKeyMode || !CtrlKeyMode)
      {
         return true;
      }

      document.getElementById("list_body").focus();

      if("" != HoveredNameHash && null != HoveredNameHash)
      {
         onDelete(HoveredNameHash);
      }

      break;

   case 82:  // r

      if(!ShftKeyMode || !CtrlKeyMode)
      {
         return true;
      }

      document.getElementById("list_body").focus();

      if("" != HoveredNameHash && null != HoveredNameHash)
      {
         onRevert(HoveredNameHash);
      }

      break;

   case 83:  // s

      if(!ShftKeyMode || !CtrlKeyMode)
      {
         return true;
      }

      document.getElementById("list_body").focus();

      if("" != SelectedNameHash)
      {
         onSave(SelectedNameHash);
      }
      else if(IsNewVariableFormVisible)
      {
         onSaveNew();
      }

      break;

   case 78:  // n

      if(!ShftKeyMode || !CtrlKeyMode)
      {
         return true;
      }

      if("" != SelectedNameHash)
      {
         document.getElementById("name_" + SelectedNameHash).focus();
      }
      else if(IsNewVariableFormVisible)
      {
         document.getElementById("name").focus();
      }

      break;

   case 86:  // v

      if(!ShftKeyMode || !CtrlKeyMode)
      {
         return true;
      }

      if("" != SelectedNameHash)
      {
         document.getElementById("value_" + SelectedNameHash).focus();
      }
      else if(IsNewVariableFormVisible)
      {
         document.getElementById("value").focus();
      }

      break;

      <?cs /if ?>

   default:
      return true;

   }

   return false;
}

// Denotes the containing div and list table respectively.
var VisibleArea = null;
var VisibleTable = null;
var VisibleHeaderArea = null;
var VisibleHeaderTable = null;

// Function that calculates the variable table size from the size of the containing window.
function onResize(Area, Table, HeaderArea, HeaderTable)
{
   VisibleArea = Area;
   VisibleTable = Table;
   VisibleHeaderArea = HeaderArea;
   VisibleHeaderTable = HeaderTable;

   var NewWidth = (WINgetWindowWidth() - WINwindowOffsetLeft(VisibleArea)) - 310;

   if (NewWidth < 400)
   {
      NewWidth = 400;
   }

   VisibleArea.style.width = (NewWidth + 18) + "px";

   VisibleTable.style.width = NewWidth + "px";
   VisibleHeaderArea.style.width = NewWidth + "px";
   VisibleHeaderTable.style.width = NewWidth + "px";

   var NewHeight = WINgetWindowHeight() - WINwindowOffsetTop(VisibleArea) - WINwindowOffsetLeft(VisibleArea);

   VisibleArea.style.height = (NewHeight - 5) + "px";

   // Set the table heading width; chop off extra headings (only needed for Firefox).
   VisibleHeaderArea.style.width = VisibleArea.clientWidth + "px";

   if("none" == document.getElementById("no_results").style.display)
   {
      // If some other variables was being edited, then prevent this edit.
      if("" != SelectedNameHash || IsNewVariableFormVisible)
      {
         // Get the name and value fields in the edit form.
         var NameField = null;
         var ValueField = null;

         if("" != SelectedNameHash)
         {
            // Set the size of the edit fields.
            NameField = document.getElementById("name_" + SelectedNameHash);
            ValueField = document.getElementById("value_" + SelectedNameHash);
            PreviewField = document.getElementById("preview_" + SelectedNameHash);
         }
         else if(IsNewVariableFormVisible)
         {
            // Set the size of the edit fields.
            NameField = document.getElementById("name");
            ValueField = document.getElementById("value");
            PreviewField = document.getElementById("preview");
         }

         var tbl = document.getElementById("list_floating_header_table");
         var row = tbl.rows[0];

         NameField.style.width = (row.cells[0].clientWidth - 30) + "px";
         ValueField.style.width = (row.cells[1].clientWidth - 30) + "px";

         adjustFieldHeight(NameField);
         adjustFieldHeight(ValueField);

         if(null != PreviewField || undefined != PreviewField)
         {
            PreviewField.style.width = (row.cells[1].clientWidth - 30) + "px";
            adjustFieldHeight(PreviewField);
         }
      }

      var NameFloatingHeader = document.getElementById("name_floating_header");
      var ValueFloatingHeader = document.getElementById("value_floating_header");
      var ActionFloatingHeader = document.getElementById("action_floating_header");

      var NameHeader = document.getElementById("name_header");
      var ValueHeader = document.getElementById("value_header");
      var ActionHeader = document.getElementById("action_header");

      NameFloatingHeader.style.width = (NameHeader.clientWidth - parseInt(WINgetStyle(NameFloatingHeader, "paddingLeft")) - parseInt(WINgetStyle(NameFloatingHeader, "paddingRight"))) + "px";
      ValueFloatingHeader.style.width = (ValueHeader.clientWidth - parseInt(WINgetStyle(ValueFloatingHeader, "paddingLeft")) - parseInt(WINgetStyle(ValueFloatingHeader, "paddingRight"))) + "px";
      ActionFloatingHeader.style.width = (ActionHeader.clientWidth - parseInt(WINgetStyle(ActionFloatingHeader, "paddingLeft")) - parseInt(WINgetStyle(ActionFloatingHeader, "paddingRight"))) + "px";

   }
}

// Page load handler.  Uses ClearSilver to populate the variable table.
function onLoad()
{
   document.onkeydown = onKeyDown;
   document.onkeyup = onKeyUp;

   // Heartbeat for our server connection.
   setTimeout("checkServerConnection()", 10000);

   var IguanaPath = "<?cs var:html_escape(IguanaConfigurationDirectory) ?>IguanaEnv.txt";
   IguanaPath = IguanaPath.replace(/\//g, "/" + WrapChar).replace(/\\/g, "\\" + WrapChar);

   <?cs if:CurrentUserCanAdmin ?>
   document.getElementById("help_blurb").innerHTML = "<p>From this page, you can view, add, edit and delete the environment variables defined for this Iguana server.</p>"
                                                     + "<p>Any changes made on this screen only affect this Iguana server. Other processes are not affected. Changes that you "
                                                     + "make can be viewed in the configuration file " + IguanaPath + ".</p>";
   <?cs else ?>
   document.getElementById("help_blurb").innerHTML = "<p>From this page, you can view the environment variables defined for this Iguana server. Users with administrative "
                                                     + "permissions can add, edit and delete these environment variables.</p>"
                                                     + "<p>The changes to the Iguana environment variables can be viewed in the configuration file " + IguanaPath + ".</p>";

   <?cs /if ?>

   <?cs set:count = #0 ?>

   <?cs if:ServerRuntime == "WINDOWS" ?>
   IsWindowsServer = true;
   <?cs else ?>
   IsWindowsServer = false;
   <?cs /if ?>

   <?cs each:item = Variable ?>
   <?cs set:name=js_escape(item.Name) ?>
   <?cs set:expandedValue=js_escape(item.ExpandedValue) ?>
   <?cs set:value=js_escape(item.Value) ?>
   <?cs set:maskedValue=js_escape(item.MaskedValue) ?>
   <?cs set:nameHash=js_escape(item.NameHash) ?>
   <?cs set:isFromSystem=item.IsFromSystem ?>

   <?cs if:!isFromSystem ?>
      addVariableListRow("<?cs var:name ?>", "<?cs var:expandedValue ?>", "<?cs var:value ?>", "<?cs var:maskedValue ?>", "<?cs var:nameHash ?>", -1, false);
   <?cs else ?>

      <?cs if:maskedValue != value ?>
         addVariableListRow("<?cs var:name ?>", "<?cs var:expandedValue ?>", "<?cs var:value ?>", "<?cs var:maskedValue ?>", "<?cs var:nameHash ?>", -1, true);
      <?cs else ?>
         addVariableListRow("<?cs var:name ?>", "<?cs var:expandedValue ?>", "<?cs var:value ?>", "<?cs var:maskedValue ?>", "<?cs var:nameHash ?>", -1, false);
      <?cs /if ?>

   <?cs /if ?>

   <?cs set:count = count + #1 ?>

   <?cs /each ?>

   // Adjust the size of the variable table and anchor it to the window bottom.
   var ListArea  = document.getElementById("list");
   var ListTable = document.getElementById("list_table");
   var ListHeaderArea = document.getElementById("list_floating_header");
   var ListHeaderTable = document.getElementById("list_floating_header_table");

   <?cs if:count == #0 ?>

      document.getElementById("no_results").style.display = "";

   <?cs else ?>

      document.getElementById("no_results").style.display = "none";

   <?cs /if ?>

   // Resize to fit the window.
   onResize(ListArea, ListTable, ListHeaderArea, ListHeaderTable);

   // Further resize to fit the number of variables (if few exist).
   onToggleShowDeleted();

   var OnResizeTimer;
   window.onresize = function() {
      clearTimeout(OnResizeTimer);
      OnResizeTimer = setTimeout(function() { onResize(VisibleArea, VisibleTable, VisibleHeaderArea, VisibleHeaderTable); }, 150);
   };
   window.onscroll = window.onresize;

   TOOLinitialize();
}

</script>
<title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Settings &gt; Environment Variables</title>
<?cs include:"browser_compatibility.cs" ?>
<?cs include:"styles.cs" ?>
<style type="text/css">

   table
   {
      width:100%;
   }

   a
   {
      cursor:pointer;
   }

   img
   {
      border:0px;
   }

   img.image_button
   {
      width:20px;
      height:20px;
      cursor: inherit;
   }

   img.image_large_button
   {
      width:40px;
      height:40px;
      cursor: inherit;
   }

   table th
   {
      text-align:left;
      padding-left:10px;
   }

   table td.variable_name
   {
      padding:10px;
      width:20%;
      font-weight:bold;
      vertical-align:middle;
   }

   table td.variable_name_edit
   {
      padding:10px;
      width:20%;
      height:100%;
      text-align:center;
      vertical-align:top;
   }

   table td.variable_value
   {
      padding:10px;
      width:70%;
   }

   table td.variable_value_edit
   {
      padding:10px;
      width:70%;
      text-align:center;
      vertical-align:top;
   }

   table td.variable_buttons
   {
      padding:10px;
      padding-right:20px;
      width:10%;
      vertical-align:top;
      text-decoration:none;
      font-style:normal;
   }
                                   
   table tr.variable_row td.variable_name
   {
      font-style:normal;
      color:rgb(52,52,52);
      text-decoration:none;
   }

   table tr.variable_row td.variable_value
   {
      font-style:normal;
      color:rgb(52,52,52);
      text-decoration:none;
   }

   table tr.masked_variable_row td.variable_name
   {
      font-style:italic;
      color:rgb(52,52,52);
      text-decoration:none;
   }

   table tr.masked_variable_row td.variable_value
   {
      font-style:italic;
      color:rgb(52,52,52);
      text-decoration:none;
   }

   table tr.deleted_variable_row td.variable_name
   {
      font-style:normal;
      color:#b7b7b7;
      text-decoration:line-through;
   }

   table tr.deleted_variable_row td.variable_value
   {
      font-style:normal;
      color:#b7b7b7;
      text-decoration:line-through;
   }

   table tr.deleted_variable_row_edit td.variable_name
   {
      font-style:normal;
      color:rgb(52,52,52);
      text-decoration:line-through;
   }

   table tr.deleted_variable_row_edit td.variable_value
   {
      font-style:normal;
      color:rgb(52,52,52);
      text-decoration:line-through;
   }

   table tr.invisible_deleted_variable_row
   {
      display:none;
   }

   table tr.invisible_masked_variable_row
   {
      display:none;
   }

   table tr.invisible_variable_row
   {
      display:none;
   }

   input.checkbox
   {
      border:none;
      background:none;
      padding:0px;
      margin:0px;
   }

   textarea.variable_name_edit
   {
      font-family:Verdana;
      font-size:11px;
      height:16px;
      resize:vertical;
   }

   textarea.variable_value_edit
   {
      font-family:Verdana;
      font-size:11px;
      height:16px;
      resize:vertical;
   }

   textarea.path_preview
   {
      font-family:Verdana;
      font-size:11px;
      margin: 0px;
      color: #777777;
      background-color:#ECEFF1;
      border:0px;
      overflow:hidden;
   }

   div.split_path
   {
      text-align:left;
      font-size:9px;
      font-weight:bold;
   }

   div#list
   {
      overflow-y:auto;
      overflow-x:auto;
      width: 100%;
   }

   table#list_table
   {
      overflow: hidden;
      <?cs if:CurrentUserCanAdmin ?>
      cursor: pointer;
      <?cs else ?>
      cursor: default;
      <?cs /if ?>
      width: 100%;
   }

   div#list_floating_header
   {
      position: absolute;
      background-color: #FFFFFF;
      overflow: hidden;
      width: 100%;
   }

   table#list_floating_header_table
   {
      position: relative;
      background-color: #FFFFFF;
      overflow: hidden;
      width: 100%;
   }

   #list_body tr
   {
      height:30px;
      background-color: #F9F9F9;
   }

   #list_header
   {
      visibility: hidden;
   }

   #no_results
   {
      color: gray;
      font-size: 20pt;
      font-weight: bold;
      font-variant: small-caps;
      cursor: default;
   }

   td#side_body
   {
      padding:10px;
   }

   a.button
   {
      float:none;
   }

   .clearfix:after
   {
      content: ".";
      display: block;
      height: 0;
      clear: both;
      visibility: hidden;
   }

   /* Hides from IE-mac \*/
   * html .clearfix { height: 1%; }
   .clearfix { display: block; }
   /* End hide from IE-mac */

</style>

</head>

<body class="tabright" onload="javascript:onLoad();">

<?cs set:Navigation.CurrentTab = "Settings" ?>
<?cs include:"header.cs" ?>

<div id="main">

<form id="set_environment_variable" action="" method="post">
<table id="iguana">
	<tr>
    	<td id="cookie_crumb">
        	<a href="/settings">Settings</a> &gt; Environment Variables
        </td>
    </tr>

	<tr>
	   <td id="dashboard_body">
           <div id="list_floating_header">
              <table id="list_floating_header_table">
                 <tr>
                    <th id="name_floating_header">
                    Name
                    </th>
                    <th id="value_floating_header">
                    Value
                    </th>
                    <th id="action_floating_header">
                    Action
                    </th>
                 </tr>
               </table>
           </div>

           <div id="list" onscroll="javascript:adjustHeaderOnScroll();">
           <table id="list_table">
              <thead id="list_header">
              <tr>
                 <th id="name_header">
                 Name
                 </th>
                 <th id="value_header">
                 Value
                 </th>
                 <th id="action_header">
                 Action
                 </th>
              </tr>
              </thead>
              <tbody id="list_body">
                <tr id="no_results" style="display:none">
                    <td align="center" valign="center" colspan="3">
                    <div>No matching entries.</div>
                    </td>
                 </tr>
              </tbody>
           </table>
           </div>

        </td>
	</tr>

</table>

</form>

</div>

<div id="side_panel">

      <table id="side_table">

         <tr>
            <th id="side_header">
               Control Panel
            </th>
         </tr>

         <tr>
         <?cs if:CurrentUserCanAdmin ?>
            <td id="side_body">
               <div align="center" valign="center">
               <a class="action-button blue" href="javascript:onNew();">Add Variable</a>
               </div>
            </td>
         <?cs /if ?>
         </tr>

         <tr>
            <td id="side_body">
            <div align="center">
            <select id="show_deleted_variable_toggle" onchange="javascript:onToggleShowDeleted();" >
            <option value="0">Show All</option>
            <option value="1">Hide Deleted</option>
            <option value="2">Show Deleted Only</option>
            </select>
            </div>
            </td>
         </tr>


      </table>
   <table id="side_table">
      <tr>
         <th id="side_header">
            Page Help
         </th>
      </tr>
      <tr>
         <td id="side_body">
         <h4 class="side_title">Overview</h4>
         <div id="help_blurb"></div>
         <h4 class="side_title">Help Links</h4>
         <ul class="help_link_icon">
         <li><a href="<?cs var:help_link('iguana4_environment_variables') ?>" target="_blank">Environment variables</a>
         </ul>

         </td>
      </tr>

   </table>
   </div>

</body>
</html>
