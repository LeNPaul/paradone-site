/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */


//Only define this once per scope. Associative array of all grids.
if (SGSelectGridState == undefined)
{	
   var SGSelectGridState={};
}

//update the data associated with this cell.
function updateCellData(Name, XPos, YPos, NewValue)
{     
	SGSelectGridState[Name].Data[YPos][XPos] = NewValue;
}

//update the display of this cell.
function updateCellVisual(Name, XPos, YPos, NewValue)
{

   var Grid = SGSelectGridState[Name];
   
   if(Grid.DisplayData[YPos][XPos] == NewValue)
   {
      return;
   }
   Grid.DisplayData[YPos][XPos] = NewValue;
   var Id = [Name, XPos, 'x', YPos];
   var Element = document.getElementById(Id.join(''));                        
   var NewElement = Grid.Display(NewValue);
  
   if (NewElement) 
   {
      Element.innerHTML = NewElement;
   }
}

//Iterate through every selected element, and perform action on them.
function processSelected( Name, X, Y, Action )
{
   var Grid = SGSelectGridState[Name];
	
   var XPos;
   var YPos;
   
   var MaxX = Math.max( X, Grid.DraggedFromX );
   var MinX = Math.min( X, Grid.DraggedFromX );

   var MaxY = Math.max( Y, Grid.DraggedFromY );
   var MinY = Math.min( Y, Grid.DraggedFromY );

   var NewValue =  Grid.Data[Grid.DraggedFromY][Grid.DraggedFromX];

   for(XPos = MinX; XPos <= MaxX; XPos++)
   {
      for(YPos = MinY; YPos <= MaxY; YPos++)
      {
         Action(Name, XPos, YPos, NewValue); 
      }
   }	
}

//Apply visual changes due to modification of data.
//This function was accounting for most of the CPU cycles,
//so it has been optimized.
//Only check between min/max and only change the display
//if a cells value has been changed.
function selectGridUpdate(Name)
{

   var XPos;
   var YPos;
   var NewElement;
   var Grid = SGSelectGridState[Name];
   	   
   for(XPos = Grid.MinX; XPos <= Grid.MaxX; XPos++)
   {
      for(YPos = Grid.MinY; YPos <= Grid.MaxY; YPos++)
      {
	 if (Grid.DisplayData[YPos][XPos] == Grid.Data[YPos][XPos])
	 {
            continue;
	 }
	 var NewElement = Grid.Display(Grid.Data[YPos][XPos]);
	 Grid.DisplayData[YPos][XPos] = Grid.Data[YPos][XPos];
	 if (NewElement)
	 {
            var Id = [Name, XPos, 'x', YPos];
    	    var Element = document.getElementById(Id.join(''));                        
            Element.innerHTML = NewElement;
         }
      }
   }
   //Reset min/maxes.   
   Grid.MinX = Math.min(Grid.DraggedFromX, Grid.LastX);
   Grid.MaxX = Math.max(Grid.DraggedFromX, Grid.LastX);
   Grid.MinY = Math.min(Grid.DraggedFromY, Grid.LastY);
   Grid.MaxY = Math.max(Grid.DraggedFromY, Grid.LastY);
}

//Set the data array.
function selectGridSetData(Data, Name)
{
	SGSelectGridState[Name].Clicked = 0;
	SGSelectGridState[Name].Data = Data;
	selectGridUpdate(Name);	
}

//Mouse released over element x,y of grid "name"
//Toggle if it's a click, otherwise apply drag changes
function selectGridReleased(X, Y, Name)
{
   var Grid = SGSelectGridState[Name];
	
   if (Grid.Clicked == 1) 
   {   
      Grid.MinX = Math.min(X, Grid.MinX );
      Grid.MaxX = Math.max(X, Grid.MaxX );
      Grid.MinY = Math.min(Y, Grid.MinY);
      Grid.MaxY = Math.max(Y, Grid.MaxY);
      processSelected(Name, X, Y,  updateCellData);
      Grid.Clicked = 0;
      Grid.Callback(Grid);
   }
   Grid.Dragged = 0;
   selectGridUpdate(Name);
}

//If the mouse is being dragged, then apply visual changes only to the grid.
function selectGridMouseover(X, Y, Name)
{
   var Grid = SGSelectGridState[Name];

   if (Grid.Clicked == 1)  
   {
      Grid.LastX = X;
      Grid.LastY = Y;

      if(Grid.Dragged == 0 && ( Grid.DraggedFromX != X || Grid.DraggedFromY != Y ) )
      {
         Grid.Dragged = 1;
      }
      //Keep track of min/max so that we don't have to update the whole grid.
      Grid.MinX = Math.min(X, Grid.MinX);
      Grid.MaxX = Math.max(X, Grid.MaxX);
      Grid.MinY = Math.min(Y, Grid.MinY);
      Grid.MaxY = Math.max(Y, Grid.MaxY);

      selectGridUpdate(Name);
      processSelected(Name, X, Y, updateCellVisual);
   }
}

//Mousedown event on grid element x,y of grid "name".
function selectGridPressed(X, Y, Name)
{
      var Grid = SGSelectGridState[Name];
	
      if (Grid.Clicked == 0)
      {
         Grid.Clicked = 1;
         Grid.DraggedFromX = X;
         Grid.DraggedFromY = Y;

         var NewValue = Grid.Toggle( Grid.Data[Y][X] );

	 if (NewValue != undefined)
         {
           Grid.Data[Y][X] = NewValue;
         }
         processSelected(Name, X, Y, updateCellVisual); 
	 Grid.MinX = X;
         Grid.MaxX = X;
         Grid.MinY = Y;
         Grid.MaxY = Y;      
      }
}

//If the mouse is moving out of the main cells of the table, onto one of the outer cells
//react as though the mouse button has been released on the last active cell.
function selectGridMouseout(Name)
{
   var Grid = SGSelectGridState[Name];

   if (Grid.Clicked == 1)  
   {
      selectGridReleased(Grid.LastX, Grid.LastY, Name);      
   }
}


//initialize the selectGrid.

//width is the desired width of the grid (could be gleaned from data).

//height is the desired height of the grid (could be gleaned from data).

//name is the desired name of the grid. Used in HTML element ids, and as a key for associative array SGSelectGridState. 
//Must be unique within its context.

//data is a two-dimensional array of height x width, containing the initial data for the grid.

//toggleFunction is a function that takes a possible value of an element in data, and returns the next valid value.
//               This should implement the desired behavior of clicking on an individual cell.

//displayFunction is a function that takes a possible value of an element in data, and returns the associated display HTML.
//              This should implement how every cell appears when it has a certain value.

//callback is a callback function. It is called when the UI modifies the values of the grid. It passes the name as a parameter.

//XAxisLabelHtml is the HTML for the labels on the YAxis. should be bounded in <tr> tags.

//YAxisLabelValues. And array of length height containing text to be displayed on the X axis labels.

function selectGrid(Width, Height, Name, Data, ToggleFunction, DisplayFunction, Callback, XAxisLabelHtml, YAxisLabelValues, Target)
{
   
   SGSelectGridState[Name] = ({ "Clicked":0 , "DraggedFromX":-1, "DraggedFromY":-1, "Width":Width, 
		   "Height":Height, "Dragged":0, "Data":Data, "Toggle":ToggleFunction, "Display":DisplayFunction, "Callback":Callback });

   var DisplayData = new Array(Height);
   //Create a deep copy of data
   {
      var Y;
      var X;
      for(Y = 0; Y < Height; Y++)
      {
	 DisplayData[Y] = new Array(Width);
         for(X = 0; X < Width; X++)
	 {
            DisplayData[Y][X] = Data[Y][X];
         }
      }
   }
   SGSelectGridState[Name].DisplayData = DisplayData;

   var TableBuffer = ['<table class="time_filter_table"' ];
   TableBuffer.push( ' id="' , Name , '" name="' , Name ,'">');

   var YPos;

   TableBuffer.push(XAxisLabelHtml);	

   for(YPos = 0; YPos < Height; YPos++)
   {
      var XPos;
      TableBuffer.push('<tr class="time_filter_row" ');
      TableBuffer.push("/>");

      TableBuffer.push('<td class="time_filter_cell_day" onmouseover="selectGridMouseout(', "'", Name, "'", ')">' , YAxisLabelValues[YPos]);

      for(XPos = 0; XPos < Width; XPos++)
      {

	 TableBuffer.push('<td class="time_filter_cell" id="' , Name , XPos +'x' , YPos ,'" '); 
         TableBuffer.push('onmousedown="selectGridPressed(' , XPos + ', ' , YPos , ", '" , Name , "')" , '"');
         TableBuffer.push('onmouseover="selectGridMouseover(' , XPos , ', ' , YPos , ", '" , Name , "')" , '"');
         TableBuffer.push('onmouseup="selectGridReleased(' , XPos + ', ' , YPos , ", '" , Name , "')" + '">');
         
         //duplicate this code (from Update) to avoid flicker
	 var Element = DisplayFunction(SGSelectGridState[Name].Data[YPos][XPos]);
	 if (Element)
	 {
            TableBuffer.push(Element);
	 }
	 //end duplication
      }
      TableBuffer.push('<td onmouseover="selectGridMouseout(', "'", Name, "'", ')"/>'); 
   }

   TableBuffer.push('<tr class="time_filter_row">');

   for(XPos = 0; XPos < Width + 2; XPos++)
   {
      TableBuffer.push('<td class="time_filter_cell" onmouseover="selectGridMouseout(', "'", Name, "'", ')"><div visibility:"hidden">&nbsp;</div>'); 
   }
   TableBuffer.push("</table>");   
   $(Target).html(TableBuffer.join(''));
}

