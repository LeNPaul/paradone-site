/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

if (RTAtable == undefined)
{
	var RTAtable = {};
        var RTAminimumTextAreaHeight = 30;
        RTAtable.resize = RTAcalibrate; 
}

//Determine if scrollheight is an accurate representation of the height of the scrolling area.
//If not, we always need to force a resize to make sure that the textarea will shrink properly.
function RTAcalibrate(ValueField)
{
      var ScrollHeight1 = ValueField.scrollHeight;
      ValueField.style.height = (ValueField.scrollHeight + 10) + "px";
      RTAtable.forceResize = ScrollHeight1 != ValueField.scrollHeight;
      if(RTAtable.forceResize)
      {
               ValueField.style.height = "0px";
      }
      ValueField.style.height = Math.max(RTAminimumTextAreaHeight, ValueField.scrollHeight) + "px";
      RTAtable.resize = RTAresize;
}

//Start a timer, if one hasn't already been started.
function RTAresizeTimer(ValueField)
{
   //Might be the first time.
   if(RTAtable[ValueField] == undefined)
   {
      RTAtable[ValueField] = {};
   }

   if(!RTAtable[ValueField].Timer)
   {
      RTAtable[ValueField].Timer = setTimeout
	             (
		         function()
   		         {
		            RTAtable.resize(ValueField); 
			    RTAtable[ValueField].Timer=null;
	                  }
		        , 100
			); 
   }
}

//Don't call this directly! Call RTAresizeTimer instead.
function RTAresize(ValueField)
{
    
   //This will force a resize if the textarea needs to shrink, or we're in ForceResize mode.
   if(RTAtable.forceResize)
   {	   
       ValueField.style.height = "0px";
   }
   
   ValueField.style.height = 'auto';
   ValueField.style.height = Math.max(RTAminimumTextAreaHeight, ValueField.scrollHeight) + "px";
}
