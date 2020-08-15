/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Sorting Javascript

var SORTcolumn = '';
var SORTpreviousColumn = '';
var SORTorder  = '';

var SORTorderCookieName = 'Iguana-SortOrder-' + UniqueInstanceId;
var SORTcolumnCookieName = 'Iguana-SortColumn-' + UniqueInstanceId;

function SORTrefreshSortedHeading()
{
   if ((SORTpreviousColumn != '') && (SORTpreviousColumn != SORTcolumn))
   {
      document.getElementById('img' + SORTpreviousColumn + 'Sort').src = '/images/sort_spacer.gif';
   }

   var HeadingSortImage = document.getElementById('img' + SORTcolumn + 'Sort');

   if ('Desc' == SORTorder)
   {
      HeadingSortImage.src = '/images/sort_down.png';
   }
   else
   {
      HeadingSortImage.src = '/images/sort_up.png';
   }
}

function SORTgetParams()
{
   var GetParams = '';

   if (SORTcolumn != '')
   {
      GetParams += 'SortColumn=' + SORTcolumn;

      if (SORTorder != '')
      {
         GetParams += '&SortOrder=' + SORTorder;
      }
   }
   else if (SORTorder != '')
   {
      GetParams += 'SortOrder=' + SORTorder;
   }

   return GetParams;
}

function SORTsetColumn(NewColumn)
{
   SORTcolumn = NewColumn;
   COOKIEcreate( SORTcolumnCookieName, NewColumn, 365);
}

function SORTsetOrder(NewOrder)
{
   SORTorder = NewOrder;
   COOKIEcreate( SORTorderCookieName, NewOrder, 365);
}

function SORTinitialize(NewColumn, NewOrder)
{
   SORTorder  = NewOrder;
   SORTcolumn = NewColumn;

   SORTrefreshSortedHeading();

   SORTpreviousColumn = SORTcolumn;
}

function SORTbyColumn(NewColumn, InitialNewOrder)
{
   if (NewColumn == SORTcolumn)
   {
      // Change the sort order if the column has not changed.
      if ('Asc' == SORTorder)
      {
         SORTsetOrder('Desc');
      }
      else
      {
         SORTsetOrder('Asc');
      }
   }
   else
   {
      SORTsetOrder(InitialNewOrder);
   }

   SORTsetColumn(NewColumn);
}
