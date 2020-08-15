/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Refresh Javascript

var REFrefreshTimeout = null;
var REFinterval  = 0;
var REFcountdown = 0;
var REFrefreshOn = true;
var REFdisplayTime = null;


function REFonTimer(OnRefresh)
{
   var CountdownDone = false;

   if (REFrefreshOn)
   {
      CountdownDone = REFdoCountdown();
   }

   if (CountdownDone)
   {
      REFclearTimer();
   }
   else
   {
      REFrestartTimer(OnRefresh);
   }

   return CountdownDone;
}

function REFrestartTimer(OnRefresh)
{
   if (OnRefresh === undefined)
   {
      REFrefreshTimeout = setTimeout('REFonTimer()', 1000);
   }
   else
   {
      REFrefreshTimeout = setTimeout(OnRefresh, 1000);
   }
}

function REFclearTimer()
{
   clearTimeout(REFrefreshTimeout);
}

function REFdisplayCountdown()
{
   var Message = '';

   if (0 == REFinterval)
   {
      Message = '';
   }
   else
   {
      if (REFrefreshOn)
      {
         Message = REFdisplayTime;
      }
      else
      {
         Message = 'Auto-refresh paused.';
      }
   }

   $('#divRefreshStatus').innerHTML = Message;
}

function REFdoCountdown()
{
   if (!(REFcountdown > 0))
   {
      return false;
   }

   --REFcountdown;
   REFdisplayCountdown();

   if (REFcountdown <= 0) 
   {
      return true;
   }
   else
   {
      return false;
   }
}

function REFgetParams()
{
   return 'refresh=' + REFinterval + "&AutomaticRequest=1";
}

function REFrefreshEnable(RefreshOn)
{
   var StopButton  = document.getElementById('btnRefreshStop');
   var StartButton = document.getElementById('btnRefreshStart');

   REFrefreshOn = RefreshOn;

   if( StopButton && StartButton )
   {
      if (0 == REFinterval)
      {
         StopButton.style.display = 'none';
         StartButton.style.display = 'none';
         return;
      }

      if (RefreshOn)
      {
         StopButton.style.display = '';
         StartButton.style.display = 'none';
      }
      else
      {
         StopButton.style.display = 'none';
         StartButton.style.display = '';
      }
   }

   REFdisplayCountdown();
}

function REFinitialize(OnRefresh, DisplayTime)
{
   var IntervalFromQuery = QRYgetValue('refresh');

   if(IntervalFromQuery == '0')
   {
      REFinterval = 0;
   }
   else
   {
      REFinterval = parseInt(IntervalFromQuery);     
   }
   
   // Default refresh interval is 1 second
   REFinterval = 1;
   REFrefreshOn = true;

   REFinitializeWithExistingInterval(OnRefresh, DisplayTime);
}

function REFinitializeWithExistingInterval(OnRefresh, DisplayTime)
{
   
   // Default refresh interval is 1 seconds
   IntervalSelect = 1;  

   REFcountdown = REFinterval;

   //REFrefreshEnable(REFrefreshOn); // This hides/displays the start/stop button according to setting.

   if (REFinterval != 0)
   {
      REFclearTimer();
      REFrestartTimer(OnRefresh);
   }

   REFdisplayTime = DisplayTime;

   REFdisplayCountdown();
}

function REFsetInterval(NewInterval)
{
   REFinterval = NewInterval;
   REFrefreshOn = true;
}
