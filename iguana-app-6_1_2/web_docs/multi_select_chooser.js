/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

//Only define this once per scope. Associative array of all choosers
if (MSCholder == undefined)
{
   var MSCholder={};
}

function MSCregister(Key, LeftPanel, RightPanel, ExcludedHeight, ExcludedWidth)
{

   if(MSCholder == undefined)
   {
      return;
   }

   MSCholder[Key] = {};
   MSCholder[Key].leftPanel  = LeftPanel;
   MSCholder[Key].rightPanel = RightPanel;
   MSCholder[Key].excludedHeight = ExcludedHeight;
   MSCholder[Key].excludedWidth = ExcludedWidth;
}
function MSCsetMode(Key, Mode)
{
   if (MSCholder == undefined)
   {
      return;
   }

   MSCholder[Key].Mode = Mode;
   if(Mode == 'edit')
   {
      MSCholder[Key].leftPanel.setAttribute('class', 'custom_multi_select_edit');
      MSCholder[Key].rightPanel.setAttribute('class', 'custom_multi_select_edit');
   }
   else
   {
      MSCholder[Key].leftPanel.setAttribute('class', 'custom_multi_select');
      MSCholder[Key].rightPanel.setAttribute('class', 'custom_multi_select');
   }
}

function MSCshowSearchTooltip(SearchField)
{
   TOOLtooltipLink
   (
      'Filter list of channels based on their name.',
      null,
      SearchField
   );
}

function MSChideSearchTooltip()
{
   TOOLtooltipClose();
}

function MSCresizeHeight(Key)
{

   if (MSCholder == undefined || !MSCholder[Key] )
   {
      return;
   }

   var Chooser = MSCholder[Key];
   var NewHeight = (WINgetWindowHeight() - WINwindowOffsetTop(Chooser.leftPanel)) - Chooser.excludedHeight;

   if (NewHeight < 75)
   {
      NewHeight = 75;
   }

   Chooser.leftPanel.style.height  = NewHeight + 'px';
   Chooser.rightPanel.style.height = NewHeight + 'px';
}

function MSCresizeWidth(Key)
{
   if (MSCholder == undefined || !MSCholder[Key] )
   {
      return;
   }

   var Chooser = MSCholder[Key];
   var NewWidth = ((WINgetWindowWidth() - WINwindowOffsetLeft(Chooser.leftPanel)) - Chooser.excludedWidth) /2;

   if (NewWidth < 100)
   {
      NewWidth = 100;
   }

   Chooser.leftPanel.style.width  = NewWidth + 'px';
   Chooser.rightPanel.style.width = NewWidth + 'px';

}


function MSCresizeTimeout(Key)
{
   MSCresizeWidth(unescape(Key));
   MSCresizeHeight(unescape(Key));
}

function MSCresize(Key)
{
   // Some browsers (like Firefox) don't always call the onresize event at the right time.
   // So we will make sure to run the adjustments when the user's mouse stops moving.
   clearTimeout(MSCholder[Key].resizeTimeoutId);
   MSCholder[Key].resizeTimeoutId = setTimeout('MSCresizeTimeout("' + escape(Key) + '");', 150);
}

function MSCtoggle(Key, Id)
{

   if (MSCholder == undefined || MSCholder[Key].Mode == "view")
   {
      return;
   }

   var Element = document.getElementById(Id);
   var SpanElement = document.getElementById("span_" + Id);

   if(190 < SpanElement.offsetWidth)
   {
      Element.style.width = SpanElement.offsetWidth + "px"; 
   }

   var ClassName = Element.attributes.getNamedItem("class");
   if (ClassName && ClassName.value == "MSCselected")
   {
      Element.setAttribute("class", "MSCunselected");
   }
   else
   {
      Element.setAttribute("class", "MSCselected");
   }

}

function MSCsort(Panel)
{

   var Children = Panel.getElementsByTagName("DIV");
   var ChildNames = [];
   var ChildHash = {};
   for ( var OptionsIndex = Children.length -1; OptionsIndex >= 0 ; --OptionsIndex )
   {
      var Name = Children[OptionsIndex].attributes.getNamedItem("value");

      if (Name && Name.value)
      {
         ChildNames.push(Name.value);
         ChildHash[Name.value] = Children[OptionsIndex];
      }
   }

   ChildNames.sort
   (
      function(x,y)
      { 
         var a = String(x).toUpperCase(); 
         var b = String(y).toUpperCase(); 
         if (a > b) { return 1; } 
         if (a < b) { return -1; }
 
         return 0; 
      }
   );

   for ( var ElementIndex = 0; ElementIndex < ChildNames.length; ++ElementIndex )
   {
      Panel.appendChild(ChildHash[ChildNames[ElementIndex]]);
   }

}

function MSCvalues(Key, Side)
{

   if (MSCholder == undefined)
   {
      return;
   }

   var Chooser = MSCholder[Key];
   var Panel;

   if(Side == "right")
   {
      Panel = Chooser.rightPanel;
   }
   else if (Side == "left")
   {
      Panel = Chooser.leftPanel;
   } 

   var channelStringBuilder = [];
   var Options = Panel.getElementsByTagName("DIV");
   for ( var OptionsIndex = Options.length -1; OptionsIndex >= 0 ; --OptionsIndex )
   {
      channelStringBuilder.push(Options[OptionsIndex].id);
      channelStringBuilder.push(',');
   }
   //Drop the last comma.
   channelStringBuilder[channelStringBuilder.length -1] = "";
   return channelStringBuilder.join('');

}

function MSCmove(Key, ElementUnion, Direction)
{

   if (MSCholder == undefined || !MSCholder[Key] || MSCholder[Key].Mode == "view" )
   {
      return;
   }

   var Chooser = MSCholder[Key];

   var To;
   if(Direction == "right")
   {
     To = Chooser.rightPanel;
   }
   else if(Direction == "left")
   {
     To = Chooser.leftPanel;
   }
   else
   {
      return;
   }
  
   var Element = ElementUnion.element || document.getElementById(ElementUnion.id);

   To.appendChild(Element);

   var directions = { "right": "left", "left":"right" };

   Element.ondblclick = function() 
                        {
                           MSCmove(Key,{"element":Element}, directions[Direction]);
                           filterTextChanged(directions[Direction]);
                        };
   

   Element.setAttribute("class", "MSCunselected");

   MSCsort(To);
}

