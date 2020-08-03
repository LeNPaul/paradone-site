/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

function TRVgetNodeChildren(TreeId, NodeId) {
   return $('#' + TreeId + ' #nodeChildren_' + NodeId).get(0);
}

function TRVgetNodeTextSpan(TreeId, NodeId) {
   return $('#' + TreeId + ' #nodeText_' + NodeId).get(0);
}

/*
 * This should only be called externally when a node should be expanded
 * without a user having clicked on it.
 */
function TRVexpandNode(TreeId, NodeId) {
   var NodeLi = $('#node_' + NodeId);
   if (NodeLi.hasClass('expandable')) {
      $(NodeLi).removeClass('expandable');
      $(NodeLi).addClass('collapsable');

      var NodeChildren = TRVgetNodeChildren(TreeId, NodeId);
      NodeChildren.innerHTML = '';
      NodeChildren.style.display = '';

      var NodeSpan = TRVgetNodeTextSpan(TreeId, NodeId);
      NodeSpan.innerHTML = '<img src="/js/treeview/images/spinner.gif" />';
   }
}

function TRVonNodeClick(TreeId, NodeLi, e, onToggle) {
   var NodeId = NodeLi.id.substring(NodeLi.id.indexOf('_') + 1);

   if ($(NodeLi).hasClass('expandable')) {
      TRVexpandNode(TreeId, NodeId);

      onToggle(NodeId, true);
   }
   else if ($(NodeLi).hasClass('collapsable')) {
      $(NodeLi).removeClass('collapsable');
      $(NodeLi).addClass('expandable');

      var NodeChildren = TRVgetNodeChildren(TreeId, NodeId);
      NodeChildren.style.display = 'none';
      NodeChildren.innerHTML = '';

      onToggle(NodeId, false);
   }

   // Stop the event from bubbling up to other nodes.
   if (!e) var e = window.event;
   e.cancelBubble = true;
   if (e.stopPropagation) e.stopPropagation();
}

function TRVonNodeMouseOver(NodeText) {
   $(NodeText).addClass('hover');
}

function TRVonNodeMouseOut(NodeText) {
   $(NodeText).removeClass('hover');
}

/*
 * Begin a node in the tree.
 * TreeId - the id of the element in which the tree is contained.
 * NodeId - a unique ID for the node.
 * NodeText - the text for the node.
 * HasChildren - true if the node has any children, false otherwise.
 * IsExpanded - true if the node is expanded, false otherwise.
 * NodeClass - class for styling the node.
 * ToggleCallback - the name of the callback function (as a string) to call when a node is toggled.
 *    The function should accept a node id, representing the node which has been toggled,
 *    and a boolean argument, which will be true if the node is being expanded, or false if it is being collapsed.
 *    For example, function onNodeToggled(NodePath, Expanded)
 */
function TRVopenNode(TreeId, NodeId, NodeText, HasChildren, IsExpanded, NodeClass, ToggleCallback) {
   var Class;
   if (HasChildren && IsExpanded) {
      Class = 'collapsable';
   }
   else if (HasChildren && !IsExpanded) {
      Class = 'expandable';
   }
   else {
      Class = 'leaf';
   }

   var MouseOverFunctions = (HasChildren ? ' onmouseover="TRVonNodeMouseOver(this);" onmouseout="TRVonNodeMouseOut(this);"' : '');

   return '<li id="node_' + NodeId + '" class="' + Class + '">\
              <div onclick="TRVonNodeClick(\'' + TreeId + '\', this.parentNode, event, ' + ToggleCallback + ');">\
                 <span id="treeIcon_' + NodeId + '" class="treeIcon"></span>\
                 <span id="nodeText_' + NodeId + '" class="nodeText ' + NodeClass + '"' + MouseOverFunctions + '>' + NodeText + '</span>\
              </div>\
              <ul id="nodeChildren_' + NodeId + '"' + (IsExpanded ? '' : ' style="display:none;"') + '>';
}

function TRVcloseNode() {
   return '</ul></li>';
}

function TRVrefreshNode(TreeId, NodeId, ExpandedView, CollapsedView, NodeClass) {
   // Modify the node's text (expanded or collapsed view).
   var NodeSpan = TRVgetNodeTextSpan(TreeId, NodeId);
   NodeSpan.innerHTML = $('#' + TreeId + ' #node_' + NodeId).hasClass('collapsable') ? ExpandedView : CollapsedView;

   // Node's class.
   NodeSpan.className = 'nodeText ' + NodeClass;
}

function TRVreplaceNode(TreeId, NodeId, NewNodeHtmlContent) {
   $('#' + TreeId + ' #node_' + NodeId).replaceWith(NewNodeHtmlContent);
}

/*
 * Returns an array of node ids representing all the currently expanded nodes in the tree.
 */
function TRVgetExpandedNodes(TreeId) {
   var ExpandedNodes = [];
   var ExpandedNodesIndex = 0;
   $('#' + TreeId + ' li.collapsable').each(function () {
      ExpandedNodes[ExpandedNodesIndex++] = this.id.split('_')[1];
   });

   return ExpandedNodes;
}

/*
 * Scrolls ScrollingContainer so that the specified node is in the center (or as close
 * as it can be), then highlights the node.
 */
function TRVhighlightNode(TreeId, NodeId, ScrollingContainer) {
   var NodeSpan = $('#' + TreeId + ' #nodeText_' + NodeId);
   if (NodeSpan && NodeSpan.get() && NodeSpan.position()) {
      // We use a timeout in order to let the browser finish rendering any new nodes
      // (so we can accurately calculate their position/dimensions).
      setTimeout(function () {
         // Scroll to show the node.
         var NodeSpanTop = NodeSpan.position().top - $('#' + TreeId).position().top;
         var NodeSpanHeight = NodeSpan.outerHeight();
         var ScrollingContainerHeight = $(ScrollingContainer).innerHeight();
         ScrollingContainer.scrollTop = NodeSpanTop + (NodeSpanHeight - ScrollingContainerHeight) / 2;
         ScrollingContainer.scrollLeft = 0;

         // Highlight the row in light blue, then let the highlighting fade away on its own.
         NodeSpan.stop(true); // stop any previous animation on the node.
         NodeSpan.css('background-color', '#BBD9F6');
         setTimeout(function () {
            NodeSpan.animate({backgroundColor:'#FFFFFF'}, 1500)
         }, 1500);
      }, 0);
   }
   else {
      throw NodeId + ' could not be found in the tree.';
   }
}
