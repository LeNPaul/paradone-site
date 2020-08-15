/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

/* The Message Tree */
var SGVmessageTree = null;

/* Request location and parameters for the AJAX call to parse (or retrieve more nodes from) the message */
var SGVreparseRequestLocation = '';
var SGVreparseRequestParameters = '';

/* Was the content generated successfully the last time we attempted it? */
var SGVcontentGenerated = false;

/* Callback function for displaying errors in the source message. */
var SGVonDisplayErrorOrigin = null;


/* A div which can be shown to indicate that something is happenning. */
var SGVpleaseWaitDiv = '<div id="PleaseWaitSegmentDiv" class="entryViewPleaseWaitBar" style="position:absolute; visibility:hidden;">Please wait...</div>';


/*
 * HTML escape function.
 */
function SGVhtmlEscape(Value)
{
   return Value.replace(/&/g, '&amp;')
               .replace(/>/g, '&gt;')
               .replace(/</g, '&lt;')
               .replace(/"/g, '&quot;');
}

/*
 * Make an AJAX request to the location specified in SGVrenderSegmentView.
 * Uses the specified RequestParameters.
 * OnSuccess will be called if a response was received successfully, with no error.
 * The parsed JSON object will be passed in as an argument.
 * OnServerError will be called if a response was received from the server, but the
 * response contained an error message (or could not be parsed).  The error message
 * will be passed in as an argument.
 * OnRetry will be called if the mini-login window is displayed, and the user logs
 * in again.
 */
function SGVmakeRequest(RequestParameters, OnSuccess, OnServerError, OnRetry)
{
   AJAXpostWithId
   (
      SGVrequestLocation,
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
 * Helper for SGVexpandNode.  Replaces the node at the specified NodePath with NewNode.
 */
function SGVreplaceNode(NodePath, NewNode)
{
   if (NodePath == '0')
   {
      SGVmessageTree.Root = NewNode;
   }
   else
   {
      var CurrentNode = SGVmessageTree.Root;
      var SplitResults = NodePath.split('-');
      
      for (var LevelIndex = 1; SplitResults[LevelIndex] != undefined; ++LevelIndex)
      {
         var ChildNodeIndex = parseInt(SplitResults[LevelIndex]);
         if (LevelIndex == SplitResults.length-1)
         {
            CurrentNode.Children[ChildNodeIndex] = NewNode;
            return;
         }
         else
         {
            CurrentNode = CurrentNode.Children[ChildNodeIndex];
         }
      }
   }
}

/*
 * Gets the node from the message tree corresponding to Nodepath.
 */
function SGVgetNodeFromPath(NodePath)
{
   var CurrentNode = SGVmessageTree.Root;
   var SplitResults = NodePath.split('-');
   
   for (var LevelIndex = 1; SplitResults[LevelIndex] != undefined; ++LevelIndex)
   {
      var ChildNodeIndex = parseInt(SplitResults[LevelIndex]);
      CurrentNode = CurrentNode.Children[ChildNodeIndex];
      if (!CurrentNode)
      {
         return null;
      }
   }
   
   return CurrentNode;
}

/*
 * Actual expanding is done in the treeview code.  This adds the children
 * to the node and changes the text to the "expanded view".
 * NodeToExpandToAndHighlight (optional) - if specified, will send a request
 * to the server to expand all the way down to a specific node.  After the
 * tree is expanded so the specified node is showing, it will be highlighted.
 */
function SGVexpandNode(NodePath, NodeToExpandToAndHighlight)
{
   var NodeSpan = TRVgetNodeTextSpan('SegmentGrammarView', NodePath);
   
   var NodeToReplace = SGVgetNodeFromPath(NodePath);
   
   var RequestParameters = SGVrequestParameters + '&Guid=' + SGVmessageTree.TreeGuid + '&RootNode=' + NodePath;
   if (NodeToExpandToAndHighlight)
   {
      // Expanding wasn't done by clicking on the node.  We'll have to tell the treeview
      // code to expand the node.
      TRVexpandNode('SegmentGrammarView', NodePath);
      RequestParameters += '&ExpandToNodePath=' + NodeToExpandToAndHighlight;
   }
   else
   {
      RequestParameters += '&ExpandNodes=' + NodeToReplace.Address;
   }
   
   SGVmakeRequest
   (
      RequestParameters,
      function(NewResponse)
      {
         SGVmessageTree.TreeGuid = NewResponse.TreeGuid; // in case it changed
         var ExpandedNode = NewResponse.Root;
         SGVreplaceNode(NodePath, ExpandedNode);
         ExpandedNode.OverrideExpand = false; // we don't want to accidentally expand it in the future when we shouldn't.
         
         // Add children
         var NodeChildrenHtml = '';
         var CountOfChildren = SGVcountOfChildrenToDisplay(ExpandedNode);
         for (var ChildIndex = 0; ChildIndex < CountOfChildren; ++ChildIndex)
         {
            NodeChildrenHtml += SGVgenerateNodeHtml(ExpandedNode.Children[ChildIndex], NodePath + '-' + ChildIndex, [], SGVcurrentDisplayFilter());
         }
         
         // Update node text to "expanded" view.
         NodeSpan.innerHTML = ExpandedNode.ExpandedView;
         
         // Add the children.
         var NodeChildren = TRVgetNodeChildren('SegmentGrammarView', NodePath);
         NodeChildren.innerHTML = NodeChildrenHtml;
         
         // Highlight a specific node, if necessary
         if (NodeToExpandToAndHighlight)
         {
            SGVendPleaseWait();
            TRVhighlightNode('SegmentGrammarView', NodeToExpandToAndHighlight, document.getElementById('SegmentTreeviewContainer'));
         }
      },
      function(ErrorMessage)
      {
         NodeSpan.innerHTML = '<span style="color:red;">Error: ' + ErrorMessage + '<br>Please try again.</span>';
      },
      function(){ SGVexpandNode(NodePath, NodeToExpandToAndHighlight) }
   );
}

/*
 * Actual collapsing is done in the treeview code.  This changes the text
 * to the "collapsed view".
 */
function SGVcollapseNode(NodePath)
{
   var Node = SGVgetNodeFromPath(NodePath);
   
   // Update node text to "collapsed" view
   var NodeSpan = TRVgetNodeTextSpan('SegmentGrammarView', NodePath);
   NodeSpan.innerHTML = Node.CollapsedView;
}

/*
 * onclick event for nodes in tree.
 */
function SGVonNodeClick(NodePath, Expanded)
{
   if (Expanded)
   {
      SGVexpandNode(NodePath);
   }
   else
   {
      SGVcollapseNode(NodePath);
   }
}

/*
 * Helper for SGVgenerateNodeHtml.  Returns true if NodePath is in NodePathList,
 * false otherwise.
 */
function SGVnodePathInList(NodePath, NodePathList)
{
   for (var NodePathIndex = 0; NodePathIndex < NodePathList.length; ++NodePathIndex)
   {
      if (NodePath == NodePathList[NodePathIndex])
      {
         return true;
      }
   }
   return false;
}

/*
 * Generate a node's classes (based on node type, etc).
 */
function SGVnodeClasses(Node)
{
   if (Node.NodeType == -1)
   {
      return 'mapNodeTruncatedNodes';
   }
   else
   {
      return 'mapNodeType' + Node.NodeType + (Node.IsMandatory ? ' mandatory' : '');
   }
}

/*
 * Generate the the HTML for a node (and it's child nodes, if applicable).
 */
function SGVgenerateNodeHtml(Node, NodePath, NodesToExpand, FilterFunction)
{
   if (!Node || (FilterFunction && !FilterFunction(Node)))
   {
      return '';
   }
   
   var CountOfChildren = SGVcountOfChildrenToDisplay(Node);
   var ExpandNode = CountOfChildren > 0 &&
                    (Node.OverrideExpand || (SGVnodePathInList(NodePath, NodesToExpand) && Node.Children));
   
   Node.OverrideExpand = false; // we don't want to accidentally expand it in the future when we shouldn't.
   
   var NodeHtml = TRVopenNode
                  (
                     'SegmentGrammarView',
                     NodePath,
                     (ExpandNode ? Node.ExpandedView : Node.CollapsedView),
                     CountOfChildren > 0,
                     ExpandNode,
                     SGVnodeClasses(Node),
                     'SGVonNodeClick'
                  );
   
   if (CountOfChildren > 0 && ExpandNode)
   {
      for (var ChildIndex = 0; ChildIndex < CountOfChildren; ++ChildIndex)
      {
         NodeHtml += SGVgenerateNodeHtml(Node.Children[ChildIndex], NodePath + '-' + ChildIndex, NodesToExpand, FilterFunction);
      }
   }
   
   NodeHtml += TRVcloseNode();
   
   return NodeHtml;
}

/*
 * Filter function for 'Default' display.
 */
function SGVfilterDefault(Node)
{
   return true;
}

/*
 * Filter function for 'Complete grammar' display.
 */
function SGVfilterComplete(Node)
{
   return true;
}

/*
 * Filter function for 'Only present values' display.
 */
function SGVfilterPresent(Node)
{
   return !Node.IsEmpty;
}

/*
 * Returns the count of children to use based on the current display mode.
 */
function SGVcountOfChildrenToDisplay(Node)
{
   var DisplayCheckbox = document.getElementById('ShowCompleteGrammarCheckbox');
   var CountToDisplay = Node.CountOfChildren;
   if (DisplayCheckbox && DisplayCheckbox.checked)
   {
      CountToDisplay = Math.max(Node.CountOfChildGrammar, Node.CountOfChildren);
   }
   return Node.Children ? Math.min(CountToDisplay, Node.Children.length) : CountToDisplay;
}

/*
 * Return the appropriate filter function for the currnt display mode.
 */
function SGVcurrentDisplayFilter(ReadFromCookie)
{
   var DisplayCheckbox = document.getElementById('ShowCompleteGrammarCheckbox');
   
   if (DisplayCheckbox)
   {
      return DisplayCheckbox.checked ? SGVfilterComplete : SGVfilterPresent;
   }
   else if (ReadFromCookie)
   {
      return COOKIEread('Iguana-ShowCompleteGrammar') == 'true' ? SGVfilterComplete : SGVfilterPresent;
   }
   else
   {
      return SGVfilterDefault;
   }
}

/*
 * Make the GUI reflect the fact that an operation is in progress.
 */
function SGVstartPleaseWait()
{
   $('#ShowCompleteGrammarCheckbox').attr('disabled', 'disabled');
   var PleaseWaitSegmentDiv = $('#PleaseWaitSegmentDiv');
   PleaseWaitSegmentDiv.css('width', PleaseWaitSegmentDiv.parent().innerWidth()-8);
   PleaseWaitSegmentDiv.css('opacity', 0.75);
   PleaseWaitSegmentDiv.css('visibility', '');
}

/*
 * Make the GUI reflect the fact that an operation is complete.
 */
function SGVendPleaseWait()
{
   $('#ShowCompleteGrammarCheckbox').removeAttr('disabled');
   $('#PleaseWaitSegmentDiv').fadeTo('fast', 0.0, function()
   {
      $('#PleaseWaitSegmentDiv').css('visibility', 'hidden');
   });
}

/*
 * Generate the HTML of the entire tree (from the pieces we have).
 */
function SGVgenerateTreeHtml(ExpandedNodes, DisplayFilter)
{
   if (!SGVmessageTree.Root)
   {
      SGVcontentGenerated = false;
      return 'No parsed results to display.' + (SGVmessageTree.ErrorList.length ? '  See errors below for details.' : '');
   }
   else
   {
      return SGVgenerateNodeHtml(SGVmessageTree.Root, '0', ExpandedNodes, DisplayFilter);
   }
}

/*
 * Rebuild the entire tree, with the specified nodes expanded.
 */
function SGVrebuildTree(ExpandedNodes)
{
   $('#SegmentGrammarView').empty();
         
   // Construct new tree's HTML
   document.getElementById('SegmentGrammarView').innerHTML =
      SGVgenerateTreeHtml(ExpandedNodes, SGVcurrentDisplayFilter());
} 

/*
 * Change view based on selected display mode.
 */
function SGVonDisplayModeChange(Checkbox)
{
   COOKIEcreate('Iguana-ShowCompleteGrammar', Checkbox.checked ? 'true' : 'false', 365);

   SGVstartPleaseWait();
   
   // We use a timeout event so that the browser has an opportunity to display the changes above.
   setTimeout(function()
   {
      try
      {
         SGVrebuildTree(TRVgetExpandedNodes('SegmentGrammarView'));
      }
      finally
      {
         SGVendPleaseWait();
      }
   }, 0);
}

/*
 * Show/hide warnings.
 */
function SGVonShowWarningsChange(Checkbox)
{
   COOKIEcreate('Iguana-ShowGrammarWarnings', Checkbox.checked ? 'true' : 'false', 365);
   
   SGVdisplayErrorList(SGVmessageTree.ErrorList);
   SGVonTargetResize();
}

/*
 * Helper function for SGVrenderSegmentView.  Returns a comma-delimited list of
 * the node addresses of all currently expanded nodes.
 */
function SGVexpandedNodeAddresses()
{
   // First, get all expanded node paths.
   var ExpandedNodes = TRVgetExpandedNodes('SegmentGrammarView');
   
   // Next, get the node "addresses", instead of index-based paths.
   for (var NodePathIndex = 0; NodePathIndex < ExpandedNodes.length; ++NodePathIndex)
   {
      var Node = SGVgetNodeFromPath(ExpandedNodes[NodePathIndex]);
      ExpandedNodes[NodePathIndex] = Node.Address;
   }

   // Finally, combine all name-based node paths into one list.
   var Result = '';
   for (var NodeAddressIndex = 0; NodeAddressIndex < ExpandedNodes.length; ++NodeAddressIndex)
   {
      Result += (NodeAddressIndex ? ','+ExpandedNodes[NodeAddressIndex] : ExpandedNodes[NodeAddressIndex]);
   }
   return Result;
}

/*
 * Helper function for SGVrenderSegmentView.  Applies a set of "diffs" provided by the server.
 */
function SGVapplyDiffs(Diffs)
{
   var CurrentDisplayFilter = SGVcurrentDisplayFilter();

   var CurrentDiff = null;
   for (var DiffIndex = 0; DiffIndex < Diffs.length; ++DiffIndex)
   {
      CurrentDiff = Diffs[DiffIndex];
      
      if (CurrentDiff.DiffType == 'Refresh')
      {
         // Update all the node's properties, preserving its children.
         var Node = SGVgetNodeFromPath(CurrentDiff.NodePath);
         var NewNode = CurrentDiff.Node;
         for (var Property in NewNode)
         {
            Node[Property] = NewNode[Property];
         }
         TRVrefreshNode('SegmentGrammarView', CurrentDiff.NodePath, Node.ExpandedView, Node.CollapsedView, SGVnodeClasses(Node));
      }
      else if (CurrentDiff.DiffType == 'Replace')
      {
         // Replace the node in the tree, including its children.
         var OldNode = SGVgetNodeFromPath(CurrentDiff.NodePath);
         var NewNode = CurrentDiff.Node;
         SGVreplaceNode(CurrentDiff.NodePath, NewNode);
         
         var NewNodeHtmlContent = SGVgenerateNodeHtml(NewNode, CurrentDiff.NodePath, [], CurrentDisplayFilter);
         TRVreplaceNode('SegmentGrammarView', CurrentDiff.NodePath, NewNodeHtmlContent);
      }
      else if (CurrentDiff.DiffType == 'Guid')
      {
         SGVmessageTree.TreeGuid = CurrentDiff.TreeGuid;
      }
   }
}

/*
 * Displays and highlights a specific node (expanding the tree and/or scrolling if necessary).
 * Will expand necessary nodes to display the error, scroll to the necessary
 * point, and highlight the node.
 */
function SGVshowPath(NodePath)
{
   // Find deepest node that is part of the path.
   var DeepestNodePath = NodePath;
   while (!TRVgetNodeTextSpan('SegmentGrammarView', DeepestNodePath))
   {
      var LastSeparaterIndex = DeepestNodePath.lastIndexOf('-');
      if (LastSeparaterIndex == -1)
      {
         // This should never happen - at least the root node should be displayed.
         throw 'No root node found.';
      }
      DeepestNodePath = DeepestNodePath.substring(0, LastSeparaterIndex);
   }
   
   if (DeepestNodePath == NodePath)
   {
      // Node is already showing - we can just scroll to it and highlight it.
      TRVhighlightNode('SegmentGrammarView', NodePath, document.getElementById('SegmentTreeviewContainer'));
   }
   else
   {
      SGVstartPleaseWait();
      
      // Use a timer to give the "please wait" a chance to display
      setTimeout(function()
      {
         // Some paths can't be shown unless we show complete grammar.
         // We can't really tell whether or not we need to be showing complete
         // grammar, so we just have to show it to be safe.
         var ShowCompleteGrammarCheckbox = document.getElementById('ShowCompleteGrammarCheckbox');
         if (ShowCompleteGrammarCheckbox && !ShowCompleteGrammarCheckbox.checked)
         {
            ShowCompleteGrammarCheckbox.checked = true;
            SGVrebuildTree(TRVgetExpandedNodes('SegmentGrammarView'));
            TOOLtooltipShowAndFade('"Show Complete Grammar" has been turned on.', ShowCompleteGrammarCheckbox, 2000);
         }
         
         SGVexpandNode(DeepestNodePath, NodePath);
      }, 0);
   }
}

/*
 * Highlight the error in the origin message.
 */
function SGVshowOrigin(OriginStart, OriginLength)
{
   SGVonDisplayErrorOrigin(OriginStart, OriginLength);
}

/*
 * Highlight the error in the origin message, and highlight
 * the specific node in the result tree.
 */
function SGVshowOriginAndPath(OriginStart, OriginLength, NodePath)
{
   SGVshowOrigin(OriginStart, OriginLength);
   SGVshowPath(NodePath);
}

/*
 * Displays and populates (or hides) the error panel.
 */
function SGVdisplayErrorList(ErrorList)
{
   var ShowWarnings = document.getElementById('ShowWarningsCheckbox').checked;
   
   var CountOfErrorsAndWarningsDisplayed = 0;
   
   var ErrorListHtml = '';
   for (var ErrorIndex = 0; ErrorIndex < ErrorList.length; ++ErrorIndex)
   {
      var Error = ErrorList[ErrorIndex];
      if (Error.ErrorListTruncated)
      {
         ErrorListHtml += '<div class="mapNodeTruncatedNodes" style="margin-top:3px; padding:1px 0 1px 25px; background-repeat:no-repeat; background-position:3px 0;">' + Error.ErrorText + '</div>';
      }
      else if (Error.IsFatal || ShowWarnings)
      {
         ++CountOfErrorsAndWarningsDisplayed;
         ErrorListHtml += '<div class="' + (Error.IsFatal ? 'errorItem' : 'warningItem');
         if (Error.HasOrigin && SGVonDisplayErrorOrigin && Error.HasPath)
         {
            ErrorListHtml += ' clickable" onclick="SGVshowOriginAndPath(' + Error.OriginStart + ', ' + Error.OriginLength + ', \'' + Error.Path + '\');" ';
         }
         else if (Error.HasOrigin && SGVonDisplayErrorOrigin)
         {
            ErrorListHtml += ' clickable" onclick="SGVshowOrigin(' + Error.OriginStart + ', ' + Error.OriginLength + ');" ';
         }
         else if (Error.HasPath)
         {
            ErrorListHtml += ' clickable" onclick="SGVshowPath(\'' + Error.Path + '\');" ';
         }
         else
         {
            ErrorListHtml += '" ';
         }
         ErrorListHtml +=    'onmouseover="$(this).addClass(\'hover\');" onmouseout="$(this).removeClass(\'hover\');">'
                       +      SGVhtmlEscape(Error.ErrorText)
                       +  '</div>';
      }
   }
   
   document.getElementById('SegmentViewErrorItems').innerHTML = ErrorListHtml;
   document.getElementById('SegmentViewErrorPanel').style.display = (CountOfErrorsAndWarningsDisplayed ? '' : 'none');
}

/*
 * Call when the main segment view area ('Target', in SGVrenderSegmentView) is resized.
 */
function SGVonTargetResize()
{
   var Stc = $('#SegmentTreeviewContainer');
   if (Stc) {
      var NewHeight = Stc.parent().innerHeight() - $('#SegmentViewControls').outerHeight();
      var ErrorPanel = $('#SegmentViewErrorPanel');

      if (ErrorPanel.css('display') != 'none') {
         var Trim = Math.min($('#SegmentViewErrorItems').outerHeight() + 20, 72);
         ErrorPanel.height(Trim);
         NewHeight -= Trim;
      }
      NewHeight -= $('#SegmentViewErrorPanelControls').height();
      Stc.height(NewHeight);
      Stc.css('visibility', '');
   }
}

/*
 * Target - the container into which the tree will be rendered.
 * ParseType - the specific type of segment view.  Should be 'SegmentGrammar' or 'SegmentMessage'.
 * DiffUpdate - indicates whether or not we should do a "diff" update, rather than rebuilding the entire tree.
 * RequestLocation and RequestParameters are used to form the AJAX request to parse the message.
 * Refresh specifies whether we should "refresh" the contents (if possible), or generate entirely new contents.
 * onRenderFinished is called after rendering the content is done.
 * onDisplayError is called when the user wants to display the origin of an error in the plain-text view
 *    of the message.  Function should take two int arguments, Start and Length.
 */
function SGVrenderSegmentView
(
   Target,
   ParseType,
   DiffUpdate,
   RequestLocation,
   RequestParameters,
   Refresh,
   onRenderFinished,
   onDisplayError
)
{
   SGVrequestLocation      = RequestLocation;
   SGVrequestParameters    = RequestParameters;
   SGVonDisplayErrorOrigin = onDisplayError;
   
   function SGVdisplayError(ErrorMessage)
   {
      SGVcontentGenerated = false;
      Target.innerHTML = '<div id="ParseSelect" style="padding:5px;"></div>' + SGVpleaseWaitDiv +
                         '<pre style="margin:0px;">Error: ' + SGVhtmlEscape(ErrorMessage) + '</pre>';
      onRenderFinished();
      SGVonTargetResize();
   }
   
   // If a refresh was requested, and the contents have already been generated.
   if (Refresh && SGVcontentGenerated)
   {
      var ExtraParameters = DiffUpdate ? '&DiffUpdate=true&OldGuid=' + SGVmessageTree.TreeGuid : '';
      SGVmakeRequest
      (
         SGVrequestParameters + '&ExpandNodes=' + SGVexpandedNodeAddresses() + ExtraParameters,
         function(JsonResponseObject)
         {
            if (JsonResponseObject.IsDiffUpdate)
            {
               SGVapplyDiffs(JsonResponseObject.Diffs);
               SGVmessageTree.ErrorList = JsonResponseObject.ErrorList;
            }
            else
            {
               SGVmessageTree = JsonResponseObject;
               SGVrebuildTree([]); // The response from the server will tell us which nodes to expand
            }
            
            SGVdisplayErrorList(SGVmessageTree.ErrorList);
            onRenderFinished();
            SGVonTargetResize();
         },
         SGVdisplayError,
         function(){SGVrenderSegmentView(Target, ParseType, DiffUpdate, SGVrequestLocation, SGVrequestParameters, Refresh, onRenderFinished, onDisplayError)}
      );
      return;
   }
   
   // Clear/reset the view
   Target.innerHTML = '<div class="entryViewPleaseWaitBar">Processing...</div>';
   SGVcontentGenerated = false;
   
   SGVmakeRequest
   (
      SGVrequestParameters + '&ExpandNodes=Message',
      function(JsonResponseObject)
      {
         SGVmessageTree = JsonResponseObject;
         
         var ShowCompleteGrammar = COOKIEread('Iguana-ShowCompleteGrammar');
         var ShowWarnings        = COOKIEread('Iguana-ShowGrammarWarnings');
         if (!ShowWarnings)
         {
            ShowWarnings = 'true';
         }
   
         Target.innerHTML = '\
            <table id="SegmentViewControls" style="width:100%; border-bottom:1px solid #BBBBBB;">\
               <tr>\
                  <td id="ParseSelect" style="padding-bottom:5px;"></td>' +
                  (ParseType == 'SegmentMessage' ? '' :
                  '<td style="text-align:right; padding-right:0px;">\
                     <input id="ShowCompleteGrammarCheckbox" type="checkbox" onchange="javascript:SGVonDisplayModeChange(this);"' +
                     (ShowCompleteGrammar == 'true' ? ' checked="checked"' : '') + '>\
                     <span style="padding-right:5px; position:relative; top:-1px;">Show Complete Grammar</span>\
                  </td>') + '\
               </tr>\
            </table>' +
            SGVpleaseWaitDiv + '\
            <div id="SegmentTreeviewContainer" style="width:100%; overflow:auto; background-color:#ffffff;">\
               <ul id="SegmentGrammarView" class="ifwareTreeview">' +
                  SGVgenerateTreeHtml([], SGVcurrentDisplayFilter(ParseType != 'SegmentMessage')) + '\
               </ul>\
            </div>\
            <div id="SegmentViewErrorPanelControls">\
               <input id="ShowWarningsCheckbox" type="checkbox" onclick="javascript:SGVonShowWarningsChange(this);"' +
               (ShowWarnings == 'true' ? ' checked="checked"' : '') + '/>\
               <span style="padding-right:5px; position:relative; top:-1px;">Show Warnings</span>\
            </div>\
            <div id="SegmentViewErrorPanel" style="display:none;">\
               <div id="SegmentViewErrorItems"></div>\
            </div>';
         
         SGVcontentGenerated = SGVmessageTree.Root ? true : false;
         SGVdisplayErrorList(SGVmessageTree.ErrorList);
         onRenderFinished();
         SGVonTargetResize();
      },
      SGVdisplayError,
      function(){ SGVrenderSegmentView(Target, ParseType, DiffUpdate, SGVrequestLocation, SGVrequestParameters, Refresh, onRenderFinished, onDisplayError) }
   );
}
