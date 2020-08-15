/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Tooltip Javascript

var TOOLactiveTimeout;
var TOOLopenTimeout;
var TOOLcloseTimeout;
var TOOLtooltipText = '';
var TOOLtooltipShowCallback = null;
var TOOLtooltipElement = null;
var TOOLtooltipOffset;
var TOOLmousePositionX = 0;
var TOOLmousePositionY = 0;
var TOOLtooltipHover = undefined;
var TOOLtooltipOnMouseOut = null;
var TOOLisInitialized = false;

// Some "constants":
var TOOLwindowSizeOffset = 10; // The minimum distance between the tooltip and the window's edge.
var TOOLpositionOffset   = 10; // The distance from the cursor to the tooltip when the tooltip is calculated relative to the mouse cursor.
var TOOLarrowOffset      = 20; // The distance from the arrow to the edge of the tooltip.
var TOOLarrowWidth       = 10; // The width of the tooltip arrow image.
var TOOLborderSize       = 2;  // The size of the tooltip border.

// Note: this is not a reliable technique for determining a browser.
// It is better to detect whether specific features are available.
// In this case, however, we are not detecting a feature, but avoiding
// a bug in IE6, so we will use this function.
function TOOLisInternetExplorer()
{
   return navigator.appName == "Microsoft Internet Explorer";
}

function TOOLhideIframe()
{
   if (!TOOLisInternetExplorer())
   {
      return;
   }
   
   var Iframe = document.getElementById('iframeTooltip');
   if (Iframe)
   {
      Iframe.style.display = 'none';
   }
}

function TOOLplaceAndDisplayIframe()
{
   if (!TOOLisInternetExplorer())
   {
      return;
   }
   
   var TooltipDiv = document.getElementById('divTooltip');
   var TooltipDivBody = document.getElementById('divTooltipBody');

   // Increase default zIndex of div by 1, so that div appears before iframe
   TooltipDiv.style.zIndex = TooltipDiv.style.zIndex+1;

   var Iframe =  document.getElementById('iframeTooltip');

   // Match Iframe position with the tooltip body
   Iframe.style.position = "absolute";
   Iframe.style.left = TooltipDiv.offsetLeft + 'px';
   Iframe.style.top = TooltipDiv.offsetTop + TooltipDivBody.offsetTop + 'px';
   Iframe.style.width = TooltipDiv.offsetWidth + 'px';
   Iframe.style.height = TooltipDivBody.offsetHeight + 'px';
   Iframe.style.opacity = 0;
   Iframe.style.filter = "alpha(opacity=0)";

   Iframe.style.display = "block";
}

function TOOLinitialize()
{
   if (document.captureEvents)
   {
      document.captureEvents(Event.MOUSEMOVE);
   }

   document.onmousemove = TOOLmouseMove;
   //attach a function to determine if we are in the tip or not, one time
   if (TOOLtooltipHover === undefined){
       TOOLtooltipHover = false;
       $('#divTooltipBody').hover(function() {
                                      TOOLtooltipHover = true;                                      
                                  }, 
                                  function(){
                                      TOOLtooltipHover = false; 
                                      if (TOOLtooltipOnMouseOut) TOOLtooltipOnMouseOut();
                                  });
       $('#divTooltipTop').hover(function() {
                                      TOOLtooltipHover = true;                                      
                                  }, 
                                  function(){
                                      TOOLtooltipHover = false; 
                                      if (TOOLtooltipOnMouseOut) TOOLtooltipOnMouseOut();
                                  });
   }

   TOOLisInitialized = true;
}

function TOOLmouseMove(Event)
{
   if (!Event)
   {
      var Event = window.event;
   }

   TOOLmousePositionX = Event.clientX;
   TOOLmousePositionY = Event.clientY;
}

function TOOLcalculatePosition(MousePosition, WindowPosition, WindowSize, TooltipSize)
{
   var Position = MousePosition + TOOLpositionOffset;

   if ((Position + TooltipSize) > (WindowSize - TOOLwindowSizeOffset))
   {
      Position = MousePosition - TooltipSize - TOOLpositionOffset;
   }

   if (Position < TOOLwindowSizeOffset)
   {
      Position = TOOLwindowSizeOffset;
   }

   return Position + WindowPosition;
}

function TOOLpositionWithMousePosition(TooltipDiv, TooltipTopDiv, TooltipBottomDiv)
{
   // Hide the arrows.
   TooltipTopDiv.style.display    = 'none';
   TooltipBottomDiv.style.display = 'none';

   // If the tooltip text has been written and remains unchanged, don't attempt to overwrite same content.
   if (NORMnormalizeInnerHtml(TOOLtooltipText) != document.getElementById('divTooltipBody').innerHTML)
   {
   document.getElementById('divTooltipBody').innerHTML = TOOLtooltipText;

   TooltipDiv.style.left = TOOLcalculatePosition(TOOLmousePositionX, WINgetWindowLeft(), WINgetWindowWidth(),  TooltipDiv.offsetWidth)  + 'px';
   TooltipDiv.style.top  = TOOLcalculatePosition(TOOLmousePositionY, WINgetWindowTop(),  WINgetWindowHeight(), TooltipDiv.offsetHeight) + 'px';
   }
}

function TOOLpositionLeftWithElement(TooltipDiv, TooltipTopDiv, TooltipBottomDiv)
{
   var ElementWindowOffsetLeft = WINwindowOffsetLeft(TOOLtooltipElement);
   var Position = ElementWindowOffsetLeft + TOOLtooltipElement.offsetWidth - TOOLarrowOffset
                + (TOOLtooltipOffset.Left || 0);
   var ArrowPosition = 0;

   if ((Position + TooltipDiv.offsetWidth) > (WINgetWindowWidth() - TOOLwindowSizeOffset))
   {
      Position = ElementWindowOffsetLeft - TooltipDiv.offsetWidth + TOOLarrowOffset - TOOLarrowWidth
               + TOOLtooltipElement.offsetWidth + (TOOLtooltipOffset.Left || 0);
      ArrowPosition = TooltipDiv.offsetWidth - TOOLarrowOffset;

      TooltipTopDiv.style.backgroundImage    = 'url(/images/tooltip_arrow_top_left.gif)';
      TooltipBottomDiv.style.backgroundImage = 'url(/images/tooltip_arrow_bottom_left.gif)';
   }
   else
   {
      ArrowPosition = TOOLarrowOffset;

      TooltipTopDiv.style.backgroundImage    = 'url(/images/tooltip_arrow_top_right.gif)';
      TooltipBottomDiv.style.backgroundImage = 'url(/images/tooltip_arrow_bottom_right.gif)';
   }

   if (Position < TOOLwindowSizeOffset)
   {
      Position = TOOLwindowSizeOffset;
      ArrowPosition = ElementWindowOffsetLeft - TOOLwindowSizeOffset - TOOLarrowWidth;
   }

   TooltipTopDiv.style.backgroundPosition    = ArrowPosition + 'px';
   TooltipBottomDiv.style.backgroundPosition = ArrowPosition + 'px';

   TooltipDiv.style.left = (Position + WINgetWindowLeft()) + 'px';
}

function TOOLpositionTopWithElement(TooltipDiv, TooltipTopDiv, TooltipBottomDiv)
{
   var ElementWindowOffsetTop = WINwindowOffsetTop(TOOLtooltipElement)
   var Position = ElementWindowOffsetTop + TOOLtooltipElement.offsetHeight - TOOLborderSize
                + (TOOLtooltipOffset.Top || 0);
   var TopVisible    = false;
   var BottomVisible = false;

   if ((Position + TooltipDiv.offsetHeight) > (WINgetWindowHeight() - TOOLwindowSizeOffset))
   {
      Position = ElementWindowOffsetTop - TooltipDiv.offsetHeight + TOOLborderSize
               - (TOOLtooltipOffset.Top || 0);
      BottomVisible = true;
   }
   else
   {
      TopVisible = true;
   }

   if (Position < TOOLwindowSizeOffset)
   {
      Position = TOOLwindowSizeOffset;

      // Hide both arrows, because at this point it's very 
      // unlikely they'll be pointing at the right thing.
      TopVisible    = false;
      BottomVisible = false;
   }

   TooltipTopDiv.style.visibility    = TopVisible    ? 'visible' : 'hidden';
   TooltipBottomDiv.style.visibility = BottomVisible ? 'visible' : 'hidden';

   TooltipDiv.style.top = (Position + WINgetWindowTop()) + 'px';
}

function TOOLpositionWithElement(TooltipDiv, TooltipTopDiv, TooltipBottomDiv)
{
   // Yes this can happen.  I don't know why but it does. -- Sajeed
   if("" == TOOLtooltipText)
   {
      return;
   }

   // Set the width of the top and bottom divs to 1 to allow the text to size the tooltip.
   TooltipTopDiv.style.width    = '1px';
   TooltipBottomDiv.style.width = '1px';

   // If the tooltip text has been written and remains unchanged, don't attempt to overwrite  
   //   it with same content since new "write" operation would force tooltip size adjustment.
   if (NORMnormalizeInnerHtml(TOOLtooltipText) != document.getElementById('divTooltipBody').innerHTML)
   {
   document.getElementById('divTooltipBody').innerHTML = TOOLtooltipText;

   TOOLpositionLeftWithElement(TooltipDiv, TooltipTopDiv, TooltipBottomDiv);
   TOOLpositionTopWithElement(TooltipDiv,  TooltipTopDiv, TooltipBottomDiv);     
   }
   // Now set the correct width of the top and bottom divs.
   TooltipTopDiv.style.width    = TooltipDiv.offsetWidth + 'px';
   TooltipBottomDiv.style.width = TooltipDiv.offsetWidth + 'px';
   
}

// Using this function requires the inclusion of jquery.
function TOOLtooltipShowAndFade(Text, Element, TimeToFade)
{
   var TooltipDiv = $('#divTooltip');

   var restoreTooltip = function()
   {
      TOOLtooltipClose();
      TooltipDiv.fadeTo(0, 1.0); // undo the damage done to the div's opacity
   }

   var fadeTooltip = function()
   {
      // Only fade if we're still displaying the same tooltip.
      if (TOOLtooltipElement == Element)
      {
         TooltipDiv.fadeTo('slow', 0.0, restoreTooltip);
      }
   }

   TOOLtooltipLink(Text, null, Element);
   setTimeout(fadeTooltip, TimeToFade);
}

function TOOLtooltipRefresh(Text, Element)
{
   if(undefined != Element && Element !== TOOLtooltipElement) return;

   TOOLtooltipText = Text;
   TOOLtooltipShow();
}

function TOOLtooltipCloseIfNotHover(){
    //delay for a bit to give user a chance to move from the 
    //trigger element, to the tooltip, then attach the one time
    //event that will close the tool tip once we exit it
    function checkForHover(){
        if (TOOLtooltipHover){
            TOOLtooltipOnMouseOut = function(){
                TOOLtooltipClose();
                TOOLtooltipOnMouseOut = null;
            }
        }
        else TOOLtooltipClose();
    }
    setTimeout(checkForHover,200);
}

function TOOLisTooltipShowing()
{
   return 'hidden' != WINgetStyle(document.getElementById('divTooltip'), 'visibility');
}

function TOOLisHoverOverElement(){
   if(null == TOOLtooltipElement){
      return;
   }

   var Element = TOOLtooltipElement;
   var Position = {x: Element.offsetLeft || 0, y:Element.offsetTop || 0};
   var ScrollX = 0;
   var ScrollY = 0;
   while (Element = Element.offsetParent) {
      if(undefined != Element.offsetLeft){
         Position.x += Element.offsetLeft;
      }
      if(undefined != Element.offsetTop){
         Position.y += Element.offsetTop;
      }
   }

   var Element = TOOLtooltipElement;
   while (Element = $(Element).parent()[0]) {
      if(undefined != Element.scrollLeft){
         ScrollX += Element.scrollLeft;
      }
      if(undefined != Element.scrollTop){     
         ScrollY += Element.scrollTop;
      }
   }

   var HoverX = ((Position.x - 15) < (TOOLmousePositionX + ScrollX)) && ((Position.x + TOOLtooltipElement.clientWidth + 15)  > (TOOLmousePositionX + ScrollX));
   var HoverY = ((Position.y - 15) < (TOOLmousePositionY + ScrollY)) && ((Position.y + TOOLtooltipElement.clientHeight + 15)  > (TOOLmousePositionY + ScrollY));

   return HoverX && HoverY;
}

function TOOLtooltipHide(){
    document.getElementById('divTooltip').style.visibility       = 'hidden';
    document.getElementById('divTooltipTop').style.visibility    = 'hidden';
    document.getElementById('divTooltipBottom').style.visibility = 'hidden';
    
    document.getElementById('divTooltipBody').innerHTML = '';

    clearInterval(TOOLcloseTimeout);
    window.status = '';
  
    TOOLhideIframe();
}

function TOOLtooltipClose()
{
   if(!TOOLisHoverOverElement() && !TOOLtooltipHover){
      TOOLtooltipHide();
   }
   else{
      clearInterval(TOOLcloseTimeout);
      TOOLcloseTimeout = setInterval('TOOLtooltipClose()', 1000);
   }
}

function TOOLtooltipShow()
{
   if(!TOOLisHoverOverElement()){
    return;
   }
   var TooltipDiv       = document.getElementById('divTooltip');
   var TooltipTopDiv    = document.getElementById('divTooltipTop');
   var TooltipBottomDiv = document.getElementById('divTooltipBottom');

   if (null == TOOLtooltipElement)
   {
      TOOLpositionWithMousePosition(TooltipDiv, TooltipTopDiv, TooltipBottomDiv);
   }
   else
   {
      TOOLpositionWithElement(TooltipDiv, TooltipTopDiv, TooltipBottomDiv);
   }

   TOOLplaceAndDisplayIframe();
   TooltipDiv.style.visibility = 'visible';

   if (TOOLtooltipShowCallback != null)
   {
      TOOLtooltipShowCallback();
   }
}

function TOOLtooltipLink(Text, ShowCallback, Element, Offset)
{
   if((null == Element)){
      return;
   }

   if(!TOOLisInitialized){
      TOOLinitialize();
   }

   if(Element === TOOLtooltipElement){
      if(TOOLisTooltipShowing()){
         TOOLtooltipRefresh(Text, Element);
      }
      else{
         clearTimeout(TOOLactiveTimeout);

         TOOLtooltipText = Text;
         TOOLactiveTimeout = setTimeout("TOOLtooltipShow();", 300);
      }
   }
   else{
      if(TOOLisTooltipShowing()){
         TOOLtooltipHide();
      }

      clearTimeout(TOOLopenTimeout);
      clearTimeout(TOOLactiveTimeout);      

      TOOLtooltipText         = Text;
      TOOLtooltipShowCallback = ShowCallback;
      TOOLtooltipElement      = Element;
      TOOLtooltipOffset       = Offset || { Left: 0, Top: 0 };

      if (undefined === TOOLtooltipShowCallback)
      {
         TOOLtooltipShowCallback = null;
      }

      if (undefined === TOOLtooltipElement)
      {
         TOOLtooltipElement = null;
      }

      TOOLactiveTimeout = setTimeout("TOOLtooltipShow();", 300);
   }
}

document.write('<div id="divTooltip" class="tooltip"><div id="divTooltipTop" class="tooltip_top"></div><div id="divTooltipBody" class="tooltip_body"></div><div id="divTooltipBottom" class="tooltip_bottom"></div></div><iframe src="/empty.html" id="iframeTooltip" style="display: none;"></iframe>');
