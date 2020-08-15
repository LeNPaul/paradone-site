<?cs # vim: set syntax=html :?>

<?cs # Include this file inside your body-tag, at or near the top. ?>

<style type="text/css"><!--
   /* CALDENDAR BORDER */
   div#calendar-popup {
      background-color: #ffffff;
      border: solid #757671 2px;
      padding: 2px 15px 15px 15px;
   }
   tr#calendar-header td {
      padding: 2px;
      background-color: #ffffff;
      border-bottom: solid #777777 1px;
      padding-top: 5px;
      padding-bottom: 5px;
   }
   table#calendar {
      border: solid #9a9c8a 1px;
   }
   table#calendar tr#week-header {
      background-color: #777777;
      text-align: center;
   }
   table#calendar tr#week-header td {
      font-family: Verdana;
      font-size: 10px;
      font-weight: normal;
      color: white;
      padding-top: 5px;
      padding-bottom: 5px;
      padding-left: 0px;
      padding-right: 0px;
   }
   table#calendar tr.week td {
      text-align: right;
      font-family: Arial, Verdana;
      font-size: 10px;
      padding-top: 2px;
      padding-bottom: 2px;
      padding-left: 8px;
      padding-right: 8px;
   }
   table#calendar col.weekday {
      background-color: #E5E5E5;
   }
   table#calendar col.weekend {
      background-color: #D5D5D5;
   }
   /* DATE BORDERS */
   table#calendar td {
      border: solid #999999 0px;
   }
   table#calendar td.today {
      
   }
   table#calendar td.selected-day {
      background-color: #b9e441;
      border-top: solid #98c126 1px;
      border-left: solid #98c126 1px;
      border-bottom: solid #d5ed92 1px;
      border-right: solid #d5ed92 1px;
   }
   #calendar-month, #calendar-year {
      color: #333333;
      font-family: Verdana, Arial;
      font-size: 10px;
      font-weight: bold;
      text-transform: normal;
      background-color: #eeeeee;
      border: solid #a9a9a9 1px;
      margin-top: 3px;
      margin-bottom: 3px;
   }
   <?cs set: highlight_color = '#bdbdbd' ?>

   <?cs # If the calendar would be displayed too far to the right or bottom of the screen,
          is is moved inside; these values specify the padding to impose on it from the edge. ?>
   <?cs set: min_padding_right  = 20 ?>
   <?cs set: min_padding_bottom = min_padding_right ?>
--></style>

<iframe id="calendar-frame" src="/empty.html"
   style="position:absolute; display:none; z-index:4999; opacity:0; filter:alpha(opacity=0)">
</iframe>

<div id="calendar-popup" style="position:absolute; display:none; z-index:5000">
   <table style="border-collapse:collapse">
      <tr id="calendar-header">
         <td align="center" valign="center" style="padding:0">
            <img id="prev-month" src="<?cs var:skin('images/arrow_back.gif') ?>">
         <td align="center">
            <select id="calendar-month">
               <option value="1">January  <option value="2">February  <option value="3">March
               <option value="4">April    <option value="5">May       <option value="6">June
               <option value="7">July     <option value="8">August    <option value="9">September
               <option value="10">October <option value="11">November <option value="12">December
            </select>
            <select id="calendar-year">
               <option value="2000">2000  <!-- This is the earliest year initially displayed. -->
            </select>
         <td align="center" valign="center" style="padding:0">
            <img id="next-month" src="<?cs var:skin('images/arrow_forward.gif') ?>">
      <tr><td colspan="3" style="padding:0">
         <center>
         <table id="calendar" cellspacing="0">
            <colgroup span="7" align="center">
               <col class="weekend sunday">
               <col span="5" class="weekday">
               <col class="weekend saturday">
            </colgroup>
            <tr id="week-header">
               <td>Su <td>Mo <td>Tu <td>We <td>Th <td>Fr <td>Sa
            <tr class="week odd-week">
               <td> <td> <td> <td> <td> <td> <td>
            <tr class="week even-week">
               <td> <td> <td> <td> <td> <td> <td>
            <tr class="week odd-week">
               <td> <td> <td> <td> <td> <td> <td>
            <tr class="week even-week">
               <td> <td> <td> <td> <td> <td> <td>
            <tr class="week odd-week">
               <td> <td> <td> <td> <td> <td> <td>
            <tr class="week even-week">
               <td> <td> <td> <td> <td> <td> <td>
         </table>
         </center>
   </table>
</div>

<script type="text/javascript"><!--

/*
   Calendar.show(CurrentDate, Invoker [, ShowTime [, OnSelection]])

   Displays the calendar near Invoker.  If ShowTime is true, the time-selector will be included.
   Calls OnSelection(NewDateString, NewDate) when the user selects a date/time--see formatDate()
   for the format of NewDateString; if OnChange is omitted, the 'value' property of Invoker will
   be assigned NewDateString.  When displayed, the calendar will show the CurrentDate.
   CurrentDate and Invoker may be strings.
   

   Calendar.formatDate(SomeDate [, ShowTime])

   A utility function to format a Date object (ShowTime) according to the calendar's convention.
   E.g., "2008/08/13 14:42:00".  If ShowTime is absent or false, the time is omitted.
*/

var Calendar = function()  // This is immediately applied (i.e., Calendar is not a function).
{
   var CalendarDiv   = document.getElementById('calendar-popup');
   var CalendarFrame = document.getElementById('calendar-frame');

   var MonthSelector = document.getElementById('calendar-month');
   var YearSelector  = document.getElementById('calendar-year');

   var PrevMonthButton = document.getElementById('prev-month');
   var NextMonthButton = document.getElementById('next-month');

   //
   // Some generic goodies customized a bit (if at all) for the Calendar.
   //

   function getEventTarget(iEvent)
   {
      var Event = iEvent ? iEvent : (window.event ? window.event : '');

      if( Event.target )
      {
         // This bit of magic ensures that the target is a real element; some browsers
         // send events to text nodes, for instance, not the enclosing element.
         //
         return Event.target.nodeType != 1 ? Event.target.parentNode : Event.target;
      }
      else
      {
         return Event.srcElement;
      }
   }

   function getPosition(Element)
   {
      var Left=0, Top=0;
      
      for(; Element; Element = Element.offsetParent)
      {
         Left += Element.offsetLeft;
         Top  += Element.offsetTop;
      }

      return { left: Left, top: Top };
   }

   function getPositionForCalendar(NearbyElement)
   {
      if( typeof NearbyElement == 'string' )
      {
         return getPositionForCalendar( document.getElementById(NearbyElement) );
      }
      else
      {
         var Position = getPosition(NearbyElement);
         Position.top += NearbyElement.clientHeight;
         
         var MaxWidth  = document.documentElement.clientWidth  - <?cs var:#min_padding_right ?>;
         var MaxHeight = document.documentElement.clientHeight - <?cs var:#min_padding_bottom ?>;

         if( Position.left + CalendarDiv.clientWidth > MaxWidth)
         {
            Position.left = MaxWidth - CalendarDiv.clientWidth;
         }

         if( Position.top + CalendarDiv.clientHeight > MaxHeight)
         {
            Position.top = MaxHeight - CalendarDiv.clientHeight;
         }

         return Position;
      }
   }

   function highlightThis()
   {
      var OldBackground = this.style.backgroundColor;
      this.style.backgroundColor = '<?cs var:js_escape(highlight_color) ?>';

      this.onmouseout = function() {
         this.style.backgroundColor = OldBackground;
      };
   }

   function useHoverImage(NewImage)
   {
      return function() {
         var OldImage = this.src;
         this.src = NewImage;

         this.onmouseout = function() {
            this.src = OldImage;
         }
      };
   }

   //
   // Functions that make manipulating and comparing Dates a lot easier.
   //

   function parseDate(SomeDate)
   {
      if( typeof SomeDate != 'string' )
      {
         return SomeDate;
      }
      else
      {
         var Result = new Date();
         var Numbers = SomeDate.match(/\d+/g);

         if( Numbers )
         {
            switch(Numbers.length)
            {
               default:  // More than six.
               case 6:
                  Result.setSeconds(Numbers[5]);
               case 5:
                  Result.setMinutes(Numbers[4]);
                  Result.setHours(Numbers[3]);
               case 4:
               case 3:
                  Result.setDate(Numbers[2]);
                  Result.setMonth(Numbers[1] - 1);
                  Result.setFullYear(Numbers[0]);
               case 2:
               case 1:
               case 0:
            }
         }

         return Result;
      }
   }

   function formatDate(SomeDate, ShowTime)
   {
      function pad(Value) { return Value < 10 ? '0' + Value : Value; }

      var Result =     SomeDate.getFullYear() + '/'
                 + pad(SomeDate.getMonth()+1) + '/'
                 + pad(SomeDate.getDate());

      if( ShowTime )
      {
         Result += ' ' + pad(SomeDate.getHours()) + ':'
                       + pad(SomeDate.getMinutes()) + ':'
                       + pad(SomeDate.getSeconds());
      }

      return Result;
   }

   function withYear(SomeDate, Year)
   {
      var Result = new Date(SomeDate);
      Result.setFullYear(Year);
      return Result;
   }

   function withMonth(SomeDate, Month)
   {
      var Result = new Date(SomeDate);
      Result.setMonth(Month);
      return Result;
   }

   function withDay(SomeDate, Day)
   {
      var Result = new Date(SomeDate);
      Result.setDate(Day);
      return Result;
   }

   function daysInMonth(SomeDate)
   {
      switch(SomeDate.getMonth()+1)
      {
         case 2:
            var Year = SomeDate.getFullYear();
            return Year%4 == 0 && (Year%100 != 0 || Year%400 == 0) ? 29 : 28;
         case 4: case 6: case 9: case 11:
            return 30;
         default:
            return 31;
      }
   }

   function prevMonth(SomeDate)
   {
      var Month = SomeDate.getMonth();

      if( Month > 0 )  // Months are zero-based.
      {
         return withMonth(SomeDate, Month-1);
      }

      return withYear(withMonth(SomeDate,11), SomeDate.getFullYear() - 1);
   }

   function nextMonth(SomeDate)
   {
      var Month = SomeDate.getMonth();

      if( Month < 11 )  // Months are zero-based.
      {
         return withMonth(SomeDate, Month+1);
      }

      return withYear(withMonth(SomeDate,0), SomeDate.getFullYear() + 1);
   }

   function sameDay(Date1, Date2)
   {
      return new Date(Date1.getFullYear(), Date1.getMonth(), Date1.getDate()).valueOf()
          == new Date(Date2.getFullYear(), Date2.getMonth(), Date2.getDate()).valueOf();
   }

   function sameDayOrEarlier(Date1, Date2)
   {
      return new Date(Date1.getFullYear(), Date1.getMonth(), Date1.getDate())
          <= new Date(Date2.getFullYear(), Date2.getMonth(), Date2.getDate());
   }

   function sameMonthOrEarlier(Date1, Date2)
   {
      return new Date(Date1.getFullYear(), Date1.getMonth())
          <= new Date(Date2.getFullYear(), Date2.getMonth());
   }

   function earlierMonth(Date1, Date2)
   {
      return new Date(Date1.getFullYear(), Date1.getMonth())
           < new Date(Date2.getFullYear(), Date2.getMonth());
   }

   //
   // The real Calendar code.
   //

   var CalendarTable = document.getElementById('calendar');
   var CountOfDayCell = (CalendarTable.rows.length - 1) * 7;
   var SelectedCell;

   function addOption(Selector, Text, Before)
   {  
      var Option = document.createElement('OPTION');
      Option.text  = Text;
      Option.value = Text;

      if( Before !== undefined )
      {
         try      { Selector.add(Option, Selector.options[Before]); }
         catch(e) { Selector.add(Option, Before); }  // IE6
      }
      else
      {
         try      { Selector.add(Option, null); }  // The Standard.
         catch(e) { Selector.add(Option); }        // The IE6 Fix.
      }
   }

   function updateCalendarNavigation(Today, SelectedDate, OnClick)
   {
      MonthSelector.selectedIndex = SelectedDate.getMonth();
      MonthSelector.onchange = function() {
         updateCalendar(withMonth(SelectedDate, this.value - 1), OnClick);
      };

      var Year = SelectedDate.getFullYear();
      var MinYear = parseInt(YearSelector.options[0].text);
      var MaxYear = parseInt(YearSelector.options[ YearSelector.options.length - 1 ].text);

      while(Year < MinYear)  { addOption(YearSelector, --MinYear, 0); }
      while(Year > MaxYear)  { addOption(YearSelector, ++MaxYear); }

      YearSelector.selectedIndex = Year - MinYear;
      YearSelector.onchange = function() {
         updateCalendar(withYear(SelectedDate, parseInt(this.value)), OnClick);
      };

      PrevMonthButton.onmouseover = useHoverImage('<?cs var:skin("images/arrow_back_hover.gif") ?>');
      PrevMonthButton.onclick = function() {
         updateCalendar(
            sameMonthOrEarlier(SelectedDate, Today) ? prevMonth(SelectedDate) : Today, OnClick);
      };

      if( earlierMonth(SelectedDate, Today) )
      {
         NextMonthButton.onmouseover = useHoverImage('<?cs var:skin("images/arrow_forward_hover.gif") ?>');
         NextMonthButton.onclick = function() {
            updateCalendar(nextMonth(SelectedDate), OnClick);
         };
      }
      else
      {
         NextMonthButton.src = '<?cs var:skin("images/arrow_forward.gif") ?>';
         NextMonthButton.onmouseover = null;
         NextMonthButton.onclick = null;
      }
   }

   function dayCell(CellIndex)
   {
      var Col =  CellIndex % 7;
      var Row = (CellIndex - Col) / 7 + 1;
      return CalendarTable.rows[Row].cells[Col];
   }

   function resetCalendar()
   {
      SelectedCell = null;

      for(var CellIndex = 0; CellIndex < CountOfDayCell; ++CellIndex)
      {
         var Cell = dayCell(CellIndex);
         Cell.className = '';
         Cell.innerHTML = '&nbsp;';
         Cell.onclick = null;
         Cell.onmouseover = null;
      }
   }

   function selectCellFunctor(Cell, SelectedDate, OnClick)
   {
      // Javascript will only create a single function for each lambda expression inside another
      // function.  Even if you appear to be creating multiple functions inside a loop, you're not.
      // To get around this, we use this functor to create the functions we need in the loop.

      return function() {
         if( SelectedCell )
         {
            SelectedCell.className = SelectedCell.className.replace(' selected-day', '');
         }

         SelectedCell = Cell;
         SelectedCell.className += ' selected-day';

         OnClick(SelectedDate);
      }
   }

   function updateCalendar(SelectedDate, OnClick)
   {
      var Today = new Date();

      resetCalendar();
      updateCalendarNavigation(Today, SelectedDate, OnClick);

      var FirstCell = withDay(SelectedDate, 1).getDay();  // Day of week, starting with Sunday.
      var LastCell  = FirstCell + daysInMonth(SelectedDate);

      for(var CellIndex = FirstCell; CellIndex < LastCell; ++CellIndex)
      {
         var Cell = dayCell(CellIndex);
         var CurrentDay = withDay(SelectedDate, CellIndex - FirstCell + 1);

         Cell.innerHTML = CurrentDay.getDate();

         if( !sameDayOrEarlier(CurrentDay, Today) )
         {
            Cell.className += ' subtle';
         }
         else
         {
            Cell.onmouseover = highlightThis;
            Cell.onclick = selectCellFunctor(Cell, CurrentDay, OnClick);
         }

         if( sameDay(CurrentDay, SelectedDate) )
         {
            SelectedCell = Cell;
            SelectedCell.className += ' selected-day';
         }

         if( sameDay(CurrentDay, Today) )
         {
            Cell.className += ' today';
         }
      }
   }

   function showFrame(Position)
   {
      CalendarFrame.style.width  = CalendarDiv.clientWidth  + 'px';
      CalendarFrame.style.height = CalendarDiv.clientHeight + 'px';
      CalendarFrame.style.left   = Position.left - 2 + 'px';
      CalendarFrame.style.top    = Position.top  - 2 + 'px';

      CalendarFrame.style.display = 'block';
   }

   function hideCalendar()
   {
      resetCalendar();
      CalendarDiv.style.display = 'none';
      CalendarFrame.style.display = 'none';
   }

   function trapOutsideClick()
   {
      var OldHandler = document.onmousedown;
      document.onmousedown = function(Event) {
         for(var Target = getEventTarget(Event); Target; Target = Target.offsetParent )
         {
            if( Target == CalendarDiv )
            {
               return;  // The click is inside the calendar.
            }
         }

         hideCalendar();
         document.onmousedown = OldHandler;
      };
   }

   function showCalendar(iCurrentDate, Invoker, ShowTime, OnSelection)
   {
      var CurrentDate = parseDate(iCurrentDate);

      updateCalendar(CurrentDate, function(SelectedDate) {
         hideCalendar();
         if( OnSelection )
         {
            OnSelection(formatDate(SelectedDate, ShowTime), SelectedDate);
         }
         else
         {
            Invoker.value = formatDate(SelectedDate, ShowTime);
         }
      });

      CalendarDiv.style.visibility = 'hidden';
      CalendarDiv.style.display = 'block';

      var Position = getPositionForCalendar(Invoker);
      showFrame(Position);

      CalendarDiv.style.left = Position.left + 'px';
      CalendarDiv.style.top  = Position.top  + 'px';

      CalendarDiv.style.visibility = 'visible';

      trapOutsideClick();
   }

   // "Export" the public functions of Calendar.
   //
   return {
      show: showCalendar,
      formatDate: formatDate
   };
}();
--></script>
