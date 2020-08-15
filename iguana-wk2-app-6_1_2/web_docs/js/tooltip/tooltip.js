/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Tooltip Javascript
var TOOLactiveTimeout;
var TOOLtooltipText = '';
var TOOLtooltipShowCallback = null;
var TOOLtooltipElement = null;
var TOOLtooltipOffset;
var TOOLmousePositionX = 0;
var TOOLmousePositionY = 0;

// Some "constants":
var TOOLwindowSizeOffset = 10; // The minimum distance between the tooltip and the window's edge.
var TOOLpositionOffset = 10; // The distance from the cursor to the tooltip when the tooltip is calculated relative to the mouse cursor.
var TOOLarrowOffset = 20; // The distance from the arrow to the edge of the tooltip.
var TOOLarrowWidth = 10; // The width of the tooltip arrow image.
var TOOLborderSize = 2;  // The size of the tooltip border.

function TOOLinitialize() {
   if (document.captureEvents) {
      document.captureEvents(Event.MOUSEMOVE);
   }
   document.onmousemove = TOOLmouseMove;
}

function TOOLmouseMove(Event) {
   if (!Event) {
      var Event = window.event;
   }
   TOOLmousePositionX = Event.clientX;
   TOOLmousePositionY = Event.clientY;
}

function TOOLcalculatePosition(MousePosition, WindowPosition, WindowSize, TooltipSize) {
   var Position = MousePosition + TOOLpositionOffset;
   if ((Position + TooltipSize) > (WindowSize - TOOLwindowSizeOffset)) {
      Position = MousePosition - TooltipSize - TOOLpositionOffset;
   }
   if (Position < TOOLwindowSizeOffset) {
      Position = TOOLwindowSizeOffset;
   }
   return Position + WindowPosition;
}

var TOOLnormDiv;
function TOOLnormalizeInnerHtml(html) {
   if (!TOOLnormDiv) {
      TOOLnormDiv = document.createElement('div');
   }
   TOOLnormDiv.innerHTML = html;
   return TOOLnormDiv.innerHTML;
}

function TOOLpositionWithMousePosition(TooltipDiv, TooltipTopDiv, TooltipBottomDiv) {
   // Hide the arrows.
   TooltipTopDiv.style.display = 'none';
   TooltipBottomDiv.style.display = 'none';

   // If the tooltip text has been written and remains unchanged, don't attempt to overwrite same content.
   if (TOOLnormalizeInnerHtml(TOOLtooltipText) != document.getElementById('divTooltipBody').innerHTML) {
      document.getElementById('divTooltipBody').innerHTML = TOOLtooltipText;
      TooltipDiv.style.left = TOOLcalculatePosition(TOOLmousePositionX, WINgetWindowLeft(), WINgetWindowWidth(), TooltipDiv.offsetWidth) + 'px';
      TooltipDiv.style.top = TOOLcalculatePosition(TOOLmousePositionY, WINgetWindowTop(), WINgetWindowHeight(), TooltipDiv.offsetHeight) + 'px';
   }
}

function TOOLpositionLeftWithElement(TooltipDiv, TooltipTopDiv, TooltipBottomDiv) {
   var ElementWindowOffsetLeft = WINwindowOffsetLeft(TOOLtooltipElement);
   var Position = ElementWindowOffsetLeft + TOOLtooltipElement.offsetWidth - TOOLarrowOffset + (TOOLtooltipOffset.Left || 0);
   var ArrowPosition = 0;

   if ((Position + TooltipDiv.offsetWidth) > (WINgetWindowWidth() - TOOLwindowSizeOffset)) {
      Position = ElementWindowOffsetLeft - TooltipDiv.offsetWidth + TOOLarrowOffset - TOOLarrowWidth + TOOLtooltipElement.offsetWidth + (TOOLtooltipOffset.Left || 0);
      ArrowPosition = TooltipDiv.offsetWidth - TOOLarrowOffset;
      TooltipTopDiv.style.backgroundImage = 'url(/js/tooltip/images/tooltip_arrow_top_left.gif)';
      TooltipBottomDiv.style.backgroundImage = 'url(/js/tooltip/images/tooltip_arrow_bottom_left.gif)';
   } else {
      ArrowPosition = TOOLarrowOffset;
      TooltipTopDiv.style.backgroundImage = 'url(/js/tooltip/images/tooltip_arrow_top_right.gif)';
      TooltipBottomDiv.style.backgroundImage = 'url(/js/tooltip/images/tooltip_arrow_bottom_right.gif)';
   }
   if (Position < TOOLwindowSizeOffset) {
      Position = TOOLwindowSizeOffset;
      ArrowPosition = ElementWindowOffsetLeft - TOOLwindowSizeOffset - TOOLarrowWidth;
   }
   TooltipTopDiv.style.backgroundPosition = ArrowPosition + 'px';
   TooltipBottomDiv.style.backgroundPosition = ArrowPosition + 'px';
   TooltipDiv.style.left = (Position + WINgetWindowLeft()) + 'px';
}

function TOOLpositionTopWithElement(TooltipDiv, TooltipTopDiv, TooltipBottomDiv) {
   var ElementWindowOffsetTop = WINwindowOffsetTop(TOOLtooltipElement)
   var Position = ElementWindowOffsetTop + TOOLtooltipElement.offsetHeight - TOOLborderSize + (TOOLtooltipOffset.Top || 0);
   var TopVisible = false;
   var BottomVisible = false;
   if ((Position + TooltipDiv.offsetHeight) > (WINgetWindowHeight() - TOOLwindowSizeOffset)) {
      Position = ElementWindowOffsetTop - TooltipDiv.offsetHeight + TOOLborderSize - (TOOLtooltipOffset.Top || 0);
      BottomVisible = true;
   } else {
      TopVisible = true;
   }
   if (Position < TOOLwindowSizeOffset) {
      Position = TOOLwindowSizeOffset;
      // Hide both arrows, because at this point it's very
      // unlikely they'll be pointing at the right thing.
      TopVisible = false;
      BottomVisible = false;
   }
   TooltipTopDiv.style.visibility = TopVisible ? 'visible' : 'hidden';
   TooltipBottomDiv.style.visibility = BottomVisible ? 'visible' : 'hidden';
   TooltipDiv.style.top = (Position + WINgetWindowTop()) + 'px';
}

function TOOLpositionWithElement(TooltipDiv, TooltipTopDiv, TooltipBottomDiv) {
   // Yes this can happen.  I don't know why but it does. -- Sajeed
   if ("" == TOOLtooltipText) return;
   // Set the width of the top and bottom divs to 1 to allow the text to size the tooltip.
   TooltipTopDiv.style.width = '1px';
   TooltipBottomDiv.style.width = '1px';
   // If the tooltip text has been written and remains unchanged, don't attempt to overwrite
   // it with same content since new "write" operation would force tooltip size adjustment.
   if (TOOLnormalizeInnerHtml(TOOLtooltipText) != document.getElementById('divTooltipBody').innerHTML) {
      document.getElementById('divTooltipBody').innerHTML = TOOLtooltipText;
      TOOLpositionLeftWithElement(TooltipDiv, TooltipTopDiv, TooltipBottomDiv);
      TOOLpositionTopWithElement(TooltipDiv, TooltipTopDiv, TooltipBottomDiv);
   }
   // Now set the correct width of the top and bottom divs.
   TooltipTopDiv.style.width = TooltipDiv.offsetWidth + 'px';
   TooltipBottomDiv.style.width = TooltipDiv.offsetWidth + 'px';
}

// Using this function requires the inclusion of jquery.
function TOOLtooltipShowAndFade(Text, Element, TimeToFade) {
   var TooltipDiv = $('#divTooltip');
   var restoreTooltip = function () {
      TOOLtooltipClose();
      TooltipDiv.fadeTo(0, 1.0); // undo the damage done to the div's opacity
   }
   var fadeTooltip = function () {
      // Only fade if we're still displaying the same tooltip.
      if (TOOLtooltipElement == Element) {
         TooltipDiv.fadeTo('slow', 0.0, restoreTooltip);
      }
   }
   TOOLtooltipLink(Text, null, Element);
   setTimeout(fadeTooltip, TimeToFade);
}

function TOOLtooltipShow() {
   var TooltipDiv = document.getElementById('divTooltip');
   var TooltipTopDiv = document.getElementById('divTooltipTop');
   var TooltipBottomDiv = document.getElementById('divTooltipBottom');

   if (null == TOOLtooltipElement) {
      TOOLpositionWithMousePosition(TooltipDiv, TooltipTopDiv, TooltipBottomDiv);
   } else {
      TOOLpositionWithElement(TooltipDiv, TooltipTopDiv, TooltipBottomDiv);
   }

   TooltipDiv.style.visibility = 'visible';

   if (TOOLtooltipShowCallback != null) {
      TOOLtooltipShowCallback();
   }
}

function TOOLtooltipRefresh(Text) {
   TOOLtooltipText = Text;
   TOOLtooltipShow();
}

function TOOLtooltipClose() {
   $(document).find('#divTooltip').remove();
   clearTimeout(TOOLactiveTimeout);
   window.status = '';
}

function TOOLisTooltipShowing() {
   return 'hidden' != WINgetStyle(document.getElementById('divTooltip'), 'visibility');
}

function TOOLtooltipLink(Text, ShowCallback, Element, Offset) {
   $(document).find('body').append('<div id="divTooltip" class="tooltip"><div id="divTooltipTop" class="tooltip_top"></div><div id="divTooltipBody" class="tooltip_body"></div><div id="divTooltipBottom" class="tooltip_bottom"></div></div>');
   console.log(Element);
   TOOLtooltipText = Text;
   TOOLtooltipShowCallback = ShowCallback;
   TOOLtooltipElement = Element;
   TOOLtooltipOffset = Offset || { Left:0, Top:0 };
   if (undefined === TOOLtooltipShowCallback) {
      TOOLtooltipShowCallback = null;
   }
   if (undefined === TOOLtooltipElement) {
      TOOLtooltipElement = null;
   }
   TOOLactiveTimeout = setTimeout('TOOLtooltipShow();', 300);
}

