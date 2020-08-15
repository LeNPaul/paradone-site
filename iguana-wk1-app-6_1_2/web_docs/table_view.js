/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

/* Request location and parameters for the AJAX call to parse (or retrieve more rows from) the message */
var TBVreparseRequestLocation = '';
var TBVreparseRequestParameters = '';

/* The last table looked at in the table view. */
var TBVlastTable = null;

/* Set of tables which are currently expanded. */
var TBVexpandedTables = {};

/* Was the content generated successfully the last time we attempted it? */
var TBVcontentGenerated = false;

/* The GUID of the parse results. */
var TBVparseResultsGuid = null;

/* The number of rows we display in a "chunk" of rows. */
var TBVrowsPerChunk = 500;


/*
 * Make an AJAX request to the specified RequestLocation.
 * Uses the specified RequestParameters.
 * OnSuccess will be called if a response was received successfully, with no error.
 * The parsed JSON object will be passed in as an argument.
 * OnServerError will be called if a response was received from the server, but the
 * response contained an error message (or could not be parsed).  The error message
 * will be passed in as an argument.
 * OnRetry will be called if the mini-login window is displayed, and the user logs
 * in again.
 */
function TBVmakeRequest(RequestLocation, RequestParameters, OnSuccess, OnServerError, OnRetry)
{
   AJAXpostWithId
   (
      RequestLocation,
      RequestParameters,
      'LogBrowserViewEntry',
      function(Data, ContentType)
      {
         if (!ContentType.match('application/json'))
         {
            MiniLogin.show('Your Iguana Session has Expired', OnRetry);
         }
         else
         {
            var JsonResponseObject;
            try
            {
               JsonResponseObject = JSON.parse(Data);
            }
            catch(e)
            {
               OnServerError('The response contains invalid data.');
               return;
            }
            if (JsonResponseObject.ErrorMessage)
            {
               OnServerError(JsonResponseObject.ErrorMessage);
            }
            else
            {
               OnSuccess(JsonResponseObject);
            }
         }
      },
      function (Error)
      {
         MiniLogin.show('Iguana is not Responding', OnRetry);
      }
   );
}

/*
 * Call when the main table view area is resized.
 */
function TBVonTargetResize()
{
   if (document.getElementById('TableGrammarViewWrap') && document.getElementById('treeArea') && document.getElementById('tableDisplay'))
   {
      var TableGrammarViewWrap = $('#TableGrammarViewWrap');
      var TreeArea = $('#treeArea');
      var TableDisplay = $('#tableDisplay');
      var DescriptionArea = document.getElementById('tableViewDescriptionDiv');
      
      var DescriptionHeight = DescriptionArea ? $(DescriptionArea).outerHeight() : 0;
      var NewHeight = TableGrammarViewWrap.innerHeight() - 10 - DescriptionHeight;
      var NewWidth = TableGrammarViewWrap.innerWidth();
      
      TreeArea.css('height', NewHeight);
      TableDisplay.css('height', NewHeight);
      
      TableDisplay.css('width', NewWidth - TreeArea.outerWidth() - 12);
   }
}

function TBVgetTableRows(Node, NodePath, Indent, TableName, Rows)
{
   if (Node.IsTable && Node.Name != TableName)
   {
      return;
   }

   NodePath += Indent + Node.Name + '<br>';
   Indent += '&nbsp;&nbsp;';
   
   for (var RowIndex = 0; RowIndex < Node.Rows.length; ++RowIndex)
   {
      var CurrentRow = Node.Rows[RowIndex];
      var CurrentRowPath = NodePath + Indent + 'Row ' + RowIndex  + '<br>';
      if (Node.IsGroup)
      {
         var CurrentIndent = Indent + '&nbsp;&nbsp;';
         for (var ChildIndex = 0; ChildIndex < CurrentRow.Children.length; ++ChildIndex)
         {
            TBVgetTableRows(CurrentRow.Children[ChildIndex], CurrentRowPath, CurrentIndent, TableName, Rows);
         }
      }
      else // We already know the names are equal, from this function's first "if".
      {
         CurrentRow.Location = CurrentRowPath;
         Rows[Rows.length] = CurrentRow;
      }
   }
}

function TBVbuildTableFromRows(Rows)
{
   if (Rows.length == 0)
   {
      return '<span style="color:#666666;">No rows were inserted into this table.</span>';
   }

   var Result = '<table class="parsedTableView">';
   var GrammarToolCount = 0;
   
   // Setup Grammar Location
   Result += '<tr><td class="empty">';
   var RowIndex;
   for (RowIndex = 0; RowIndex < Rows.length; ++RowIndex)
   {
      Result += '<td class="header"><a id="grammerTool-' + (GrammarToolCount++) + '" name="grammerTool-' + GrammarToolCount + '" class="helpIcon" rel="' +
                Rows[RowIndex].Location + '" title="Grammar Information" href="#" onClick="initializeHelp(this,event);" style="zoom:1;">Row Location...</a>';
   }
      
   for (var ColumnIndex = 0; ColumnIndex < Rows[0].Columns.length; ++ColumnIndex)
   {
      Result += '<tr><td class="header">' + Rows[0].Columns[ColumnIndex].Name;
      for (RowIndex = 0; RowIndex < Rows.length; ++RowIndex)
      {
         Result += '<td>' + TBVhtmlEscape(Rows[RowIndex].Columns[ColumnIndex].Value);
      }
   }
   
   Result += '</table>';
   
   return Result;
}

// If Refresh is false, then that means the user has actually clicked on the
// table they want to see.  In this case, it is the table view's responsibility
// to indicate that something is happening ("please wait...").  If Refresh is true,
// then the log browser is refreshing the message, and should already be displaying
// something along those lines.
//
function TBVshowTable(TableName, ChunkIndex, Refresh)
{
   $("#helpTooltipDiv").fadeOut("slow");
   $('#TableGrammarView span.treeNode').css('color', '#000000');
   
   var TableNameTreeNode = document.getElementById(TableName + '_' + ChunkIndex);
   
   if (TableNameTreeNode)
   {
      $(TableNameTreeNode).css('color', '#215FA3');
      document.getElementById('tableDisplay').innerHTML = '';
      TBVlastTable = {'Name':TableName,'ChunkIndex':ChunkIndex};
      
      var PleaseWaitTableDiv = $('#PleaseWaitTableDiv');
      if (!Refresh)
      {
         PleaseWaitTableDiv.css('width', $('#PleaseWaitTableDiv').parent().innerWidth()-8);
         PleaseWaitTableDiv.css('opacity', 0.75);
         PleaseWaitTableDiv.css('visibility', '');
      }
      
      TBVmakeRequest
      (
         TBVreparseRequestLocation,
         TBVreparseRequestParameters + '&TableRows=' + escape(TableName) + '&ChunkIndex=' + ChunkIndex + '&RowsPerChunk=' + TBVrowsPerChunk + '&Guid=' + TBVparseResultsGuid,
         function(JsonResponseObject)
         {
            try
            {
               TBVupdateContents(JsonResponseObject);
            }
            catch(e)
            {
               TBVdisplayError(e.message ? e.message : e, Target);
            }
            if (!Refresh)
            {
               PleaseWaitTableDiv.fadeTo('fast', 0.0, function()
               {
                 PleaseWaitTableDiv.css('visibility', 'hidden');
               });
            }
         },
         function(ErrorMessage)
         {
            TBVdisplayError(ErrorMessage, Target);
         },
         function(){ TBVshowTable(TableName, StartIndex, EndIndex) }
      );
   }
}

/*
 * HTML escape function.
 */
function TBVhtmlEscape(Value)
{
   return Value.replace(/&/g, '&amp;')
               .replace(/>/g, '&gt;')
               .replace(/</g, '&lt;')
               .replace(/"/g, '&quot;');
}

/*
 * Display an error message in the target element.
 */
function TBVdisplayError(ErrorMessage, Target)
{
   TBVcontentGenerated = false;
   Target.innerHTML = '<div id="ParseSelect" style="padding:5px;"></div>\
                       <pre style="margin:0px;">Error: ' + TBVhtmlEscape(ErrorMessage) + '</pre>';
}

function TBVbuildTree(ResponseData)
{
   var Result = '';
   var Tables = ResponseData.TableList;
   
   var OldExpandedTables = TBVexpandedTables;
   TBVexpandedTables = {};
   
   for (var TableIndex = 0; TableIndex < Tables.length; ++TableIndex)
   {
      var Table = Tables[TableIndex];
      var Expandable = Table.CountOfRow > TBVrowsPerChunk;
      var LiIdString = '';
      var LiClassString = '';
      
      if (Expandable)
      {
         LiIdString = ' id="' + Table.Name + '"';
         if (OldExpandedTables[Table.Name] || (TBVlastTable && TBVlastTable.Name == Table.Name))
         {
            TBVexpandedTables[Table.Name] = 1;
            LiClassString = ' class="open"';
         }
      }
      
      Result += '<li' + LiIdString + LiClassString + '><span class="treeNode table"'
             +  (Expandable ? '>' : ' id="' + Table.Name + '_0" style="cursor:pointer;" onclick="TBVshowTable(\'' + Table.Name + '\', 0, false);">')
             +  TBVhtmlEscape(Table.Name)
             +  ' <span class="tableCountOfRow">(' + Table.CountOfRow + (Table.CountOfRow == 1 ? ' Row' : ' Rows') + ')</span></span>';
      
      if (Expandable)
      {
         Result += '<ul>';
         var ChunkIndex = 0;
         while (ChunkIndex * TBVrowsPerChunk < Table.CountOfRow)
         {
            var ChunkStart = ChunkIndex * TBVrowsPerChunk;
            var ChunkEnd = Math.min(Table.CountOfRow, (ChunkIndex + 1) * TBVrowsPerChunk) - 1;
            Result += '<li><span class="treeNode tableChunk" id="' + Table.Name + '_' + ChunkIndex + '" onClick="TBVshowTable(\''
                   +  Table.Name + '\', ' + ChunkIndex + ', false);" style="cursor:pointer;">'
                   +  (ChunkStart == ChunkEnd ? 'Row ' + ChunkStart : 'Rows ' + ChunkStart + '..' + ChunkEnd)
                   +  '</span></li>';
            ChunkIndex++;
         }
         Result += '</ul>';
      }
      
      Result += '</li>';
   }
   
   return Result;
}

function TBVonTableToggled()
{
   if ($(this).hasClass('collapsable')) // node was expanded
   {
      TBVexpandedTables[this.id] = 1;
   }
   else if (TBVexpandedTables[this.id])
   {
      delete TBVexpandedTables[this.id];
   }
}

/* Helper for TBVgenerateOrUpdateContents */
function TBVupdateContents(JsonResponseObject)
{
   if (JsonResponseObject.ErrorMessage)
   {
      throw JsonResponseObject.ErrorMessage;
   }

   TBVparseResultsGuid = JsonResponseObject.Guid;

   if (JsonResponseObject.TableList)
   {
      // Regenerate table tree.
      $('#TableGrammarView').empty();
      
      // Construct new tree's HTML
      var NewTreeHtml = TBVbuildTree(JsonResponseObject);
      var NewBranches = $('#TableGrammarView').append(NewTreeHtml);
      
      // Tree-ify
      $('#TableGrammarView').treeview({add:NewBranches});
   }
   
   // Show table data, if it exists.
   var TableDisplay = document.getElementById('tableDisplay');
   if (JsonResponseObject.TableRows)
   {
      if (TBVlastTable && JsonResponseObject.TableList)
      {
         // If the table list was updated, we need to "re-select" the table
         // we're displaying.
         var TableNameTreeNode = document.getElementById(TBVlastTable.Name + '_' + TBVlastTable.ChunkIndex);
         if (TableNameTreeNode)
         {
            $(TableNameTreeNode).css('color', '#215FA3');
         }
         else
         {
            TBVlastTable = null;
         }
      }
   
      TableDisplay.innerHTML = JsonResponseObject.TableRows;
   }
   else
   {
      TBVlastTable = null;
      TableDisplay.innerHTML = '';
   }
}

/*
 * Helper for TVBrenderTableView.  Will generate (or update) the contents with the
 * response data.
 */
function TBVgenerateOrUpdateContents(Target, JsonResponseObject, Refresh)
{
   if (Refresh && TBVcontentGenerated)
   {
      TBVupdateContents(JsonResponseObject);
   }
   else if (!JsonResponseObject.TableList)
   {
      throw 'Response from Iguana contained no Table list.';
   }
   else
   {
      TBVparseResultsGuid = JsonResponseObject.Guid;
      TBVcontentGenerated = false;
      TBVlastTable = null;
      TBVexpandedTables = {};
      
      function descriptionDiv() {
         return !JsonResponseObject.Description ? '' :
            '<div id="tableViewDescriptionDiv">'
            + '<img id="tableViewMessageIcon" src="/js/treeview/images/message.gif" />'
            + '<span id="tableViewNameSpan">'
            +    TBVhtmlEscape(JsonResponseObject.Name)
            + '</span> - '
            + TBVhtmlEscape(JsonResponseObject.Description) + '</div>';
      }

      // Generate the entire content
      Target.innerHTML = '\
         <div id="TableGrammarViewWrap">\
            <div id="PleaseWaitTableDiv" class="entryViewPleaseWaitBar" style="position:absolute; visibility:hidden;">Please wait...</div>\
            ' + descriptionDiv() + '\
            <div style="width:27%;height:100%;float:left;overflow-x:auto;overflow-y:auto;padding:5px;background-color:#ffffff;" id="treeArea">\
               <div id="ParseSelect" style="padding-bottom:5px;"></div>\
               <ul id="TableGrammarView" class="nodetree">' + TBVbuildTree(JsonResponseObject) + '</ul>\
            </div>\
            <div id="tableDisplay">\
            </div>\
         </div>';
      
      // Initialize table tree.
      $('#TableGrammarView').treeview({collapsed:true, toggle:TBVonTableToggled});
      
      TBVcontentGenerated = true;
   }
   
   TBVonTargetResize();
}

/*
 * Target - the container into which the table view will be rendered.
 * RequestLocation and RequestParameters are used to form the AJAX request to parse the message.
 * Refresh specifies whether we should "refresh" the contents (if possible), or generate entirely new content.
 * onRenderFinished is called after rendering the content is done.
 */
function TBVrenderTableView(Target, RequestLocation, RequestParameters, Refresh, onRenderFinished)
{
   if (!Refresh || !TBVcontentGenerated)
   {
      Target.innerHTML = '<div class="entryViewPleaseWaitBar">Processing...</div>';
   }
   
   TBVreparseRequestLocation = RequestLocation;
   TBVreparseRequestParameters = RequestParameters;

   TBVmakeRequest
   (
      TBVreparseRequestLocation,
      TBVreparseRequestParameters + (Refresh && TBVlastTable ? '&TableRows=' + TBVlastTable.Name + '&ChunkIndex=' + TBVlastTable.ChunkIndex + '&RowsPerChunk=' + TBVrowsPerChunk : ''),
      function(JsonResponseObject)
      {
         try
         {
            TBVgenerateOrUpdateContents(Target, JsonResponseObject, Refresh);
         }
         catch(e)
         {
            TBVdisplayError(e.message ? e.message : e, Target);
         }
         onRenderFinished();
      },
      function(ErrorMessage)
      {
         TBVdisplayError(ErrorMessage, Target);
         onRenderFinished();
      },
      function(){ TBVrenderTableView(Target, RequestLocation, RequestParameters, Refresh, onRenderFinished) }
   );
}
