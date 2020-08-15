<?cs include:"doctype.cs" ?>

<html>

<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?> &gt; Message Histogram</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("iguana_configuration.css") ?>" />
    <script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>
    <script language="javascript" type="text/javascript" src="<?cs var:iguana_version_js("/flot/jquery.min.js") ?>"></script>
    <script language="javascript" type="text/javascript" src="<?cs var:iguana_version_js("/flot/jquery.flot.min.js") ?>"></script>
    <script language="javascript" type="text/javascript" src="<?cs var:iguana_version_js("/flot/jquery.flot.selection.min.js") ?>"></script>

<style type="text/css">
.legend 
{
   float:  left;
   border: 2px;
   solid   #A0A0A0;
   width:  14px;
   height: 14px;
};
</style>

<script id="source" language="javascript" type="text/javascript">
var PlotOverviewObj = null;
var PlotData = null;
var PlotYScaleMax = null;
var PlotSelection = null;
var UpdateInProgress = false;
var PlotSelecting = false;

var PlotXAxisOpts = { mode: 'time', timeformat : '%h:%M:%S %p' }
var PlotSeriesOpts = { bars : { show: true, barWidth : <?cs var:BarWidth ?>, fill:1, lineWidth : 0} }

var SelectNote = '<span style="position: absolute; \
                 top:50%; \
		 margin-left:auto;\
		 margin-right:auto;\
		 text-align: center;\
		 width: 100%"><h4>Select a range in the overview above.</h4></span>';

function historicalCreateGraph()
{
   PlotOverviewObj = $.plot($("#historical_graph_overview_panel"), PlotData, 
   {
         xaxis: PlotXAxisOpts,
         yaxis: { min: 0, max: PlotYScaleMax },
         legend:{ show: false },
	 selection: { mode: "x" },
	 series: PlotSeriesOpts
   }
   );
   historicalUpdateColors();
}

function historicalYScaleSelectClick(sender)
{
   PlotYScaleMax= (sender.value == 'auto' ? null : parseInt(sender.value));
   historicalCreateGraph();
}

function historicalUpdateColors()
{
   var series = PlotOverviewObj.getData();
   for (var i = 0; i < series.length; ++i)
   {
      $('#color_'+series[i].guid).css('background-color',series[i].color);
   }
}

function historicalSelectionRedraw()
{
   if (PlotSelection)
   {
      var ThisXaxis = {};
      $.extend(true,ThisXaxis,PlotXAxisOpts) //deep copy
      
      var PlotOptions = {
         xaxis: ThisXaxis,
         yaxis: { min: 0, max: PlotYScaleMax },
         legend:{ show: false },
	 series: PlotSeriesOpts
      }
      PlotOptions.xaxis.min = PlotSelection.x1;
      PlotOptions.xaxis.max = PlotSelection.x2;
      $.plot($("#historical_graph_panel"), PlotData, PlotOptions);
   }
   else
   {
      $("#historical_graph_panel")
         .html(SelectNote)
   }
}

function historicalGraphRedraw()
{
   PlotOverviewObj.setData(PlotData);
   PlotOverviewObj.setupGrid();
   PlotOverviewObj.draw();
   historicalUpdateColors();
}

function comparePlotData(Lhs, Rhs)
{
   if (Lhs.guid < Rhs.guid) return -1;
   else if (Lhs.guid > Rhs.guid) return 1;
   return 0;
}

function mergePlotElement(Lhs, Rhs)
{
   Lhs.label = Rhs.label;
   Lhs.color = Rhs.color;

   //append all data from Rhs to Lhs
   Lhs.data = Lhs.data.concat(Rhs.data);

   //remove from beginning of Joined.data
   //if too many from
   if (Lhs.data.length > 1000)
   {
      Lhs.data.splice(0,Lhs.data.length - 1000);
   }
   return Lhs;
}

function mergePlotData(OldData, NewData)
{
   if (!OldData || !OldData.length) return NewData;

   //NewData joins with OldData, adding new items
   //and remove elements from OldData not in new data
   var OldIndex = 0; 
   var NewIndex = 0;
   while( OldIndex < OldData.length &&
          NewIndex < NewData.length)
   {
      var CompareVal = comparePlotData(OldData[OldIndex], NewData[NewIndex]);
      if (CompareVal == 0)
      {
         //merge existing
	 OldData[OldIndex] = mergePlotElement(OldData[OldIndex], NewData[NewIndex]);
	 OldIndex++;
	 NewIndex++;
      }
      else if (CompareVal < 0)
      {
         //remove
	 OldData.splice(OldIndex,1);	 	 
      }
      else
      {
         //add new in place
	 OldData.splice(OldIndex,0, NewData[NewIndex]);
	 OldIndex++;
	 NewIndex++;	 
      }
   }
   //remove remaining
   OldData.splice(OldIndex,OldData.length-OldIndex);
   //add remaining
   OldData = OldData.concat( NewData.splice( NewIndex, NewData.length - NewIndex) );
   return OldData;
}

function historicalGraphUpdateComplete(data)
{
   UpdateInProgress = false;
   var NewData = eval(data);
   if (NewData)
   {
      NewData.sort(comparePlotData);
      PlotData = mergePlotData(PlotData, NewData);
   }
   if (PlotOverviewObj == null)
   {
      historicalCreateGraph();
   }
   else
   {
      historicalGraphRedraw();
      historicalSelectionRedraw();
   }
   if (!PlotSelecting)
   {
      if (PlotSelection) PlotOverviewObj.setSelection(PlotSelection);
      else PlotOverviewObj.clearSelection();
   }
   setTimeout("requestUpdate();", <?cs var:BarWidth/4 ?> );
}

function historicalGraphUpdateError(error)
{
   UpdateInProgress = false;
   var MessageSpan = document.getElementById('historical_graph_panel');
   var ErrorMessage = '<b style="color: red;">Error while updating graph:<br />' + error + '</b>';
   MessageSpan.innerHTML = ErrorMessage
}

function getSelectedIds(Form)
{
   var Query = '';
   for(var ElementIndex=0; ElementIndex < Form.elements.length; ++ElementIndex)
   {
      var Element = Form.elements[ElementIndex];
      if (Element.checked)
      {
	 //search the PlotData for this guid to provide a starting point
	 //so we only have to fetch deltas
	 var StartTime = '';
	 if (PlotData)
         {
	    //TODO - binary search since PlotData is sorted
	    for (var DataIndex = 0; DataIndex < PlotData.length; DataIndex++)
	    {
	       var ThisPlotData = PlotData[DataIndex];
	       if (ThisPlotData.guid == Element.value &&
	           ThisPlotData.data.length)
	       {
	          StartTime = ',' + ThisPlotData.data[ThisPlotData.data.length-1][0];
	          break;
	       }
	    }
         } 
         Query += Element.value + StartTime + ':';
      }
   }
   return Query;
}

function guidSelected(Element)
{
   $("#historical_graph_panel").html('');
   requestUpdate();
}

function selectAll(NewChecked)
{
   Form = document.getElementById('historical_select_form');
   for(var ElementIndex=0; ElementIndex < Form.elements.length; ++ElementIndex)
   {
      var Element = Form.elements[ElementIndex];
      if (Element.checked != NewChecked)
      {
	 Element.checked = NewChecked;
      }
   }
}

function requestUpdate()
{
   if (!UpdateInProgress)
   {
      UpdateInProgress = true;
      SelectForm = document.getElementById('historical_select_form');
      Query = 'ids=' + getSelectedIds(SelectForm);
      AJAXpost('historical_graph_update',
	       Query,
	       historicalGraphUpdateComplete,
               historicalGraphUpdateError
     );
   }
}

function handlePlotSelected(event, ranges)
{
   PlotSelecting = false;
   PlotSelection = {x1 :ranges.xaxis.from, x2: ranges.xaxis.to }
   historicalSelectionRedraw();
}

function handlePlotUnselected(event)
{
   PlotSelection = null;
   historicalSelectionRedraw();
}

$(function () {
   $("#historical_graph_overview_panel")
      .bind("plotselected", handlePlotSelected)
      .bind("plotunselected", handlePlotUnselected)
      .bind("plotselecting", function(){ PlotSelecting = true; })
   historicalSelectionRedraw();
   requestUpdate();
});
</script>

</head>

<body class="tableft" >
<div id="header">
	<div id="logo"><img src="/<?cs var:skin("images/iguana4_logo.gif") ?>"/></div>
	<div id="version"><?cs include:"version.cs" ?></div>
</div>



<!-- START MAIN CONTAINER -->
<div id="main">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
	   <a href="/">Dashboard</a> &gt; Message Histogram
         </td>
      </tr>

      <tr><td id="dashboard_body">
      <center>

<table width="100%">
   <tr><td width="70%">
    <select id="historical_y_scale_select" onChange="historicalYScaleSelectClick(this);">
       <option selected>auto</option>
       <option>50</option>
       <option>100</option>
       <option>150</option>
       <option>200</option>
       <option>250</option>
       <option>300</option>
       <option>400</option>
       <option>500</option>
       <option>600</option>
       <option>700</option>
       <option>800</option>
       <option>900</option>
       <option>1000</option>
       <option>2000</option>
       <option>3000</option>
       <option>4000</option>
       <option>5000</option>
       <option>10000</option>
    </select> (scale)
    <center>
<b>Message Histogram</b>

    <div id="historical_graph_overview_panel" style="height:100px;"></div>
    <div id="historical_graph_panel" style="position:relative; height:500px;">
    </div>

</center>

</td>

<td valign="top">

<div>
<a href="#" onClick="selectAll(true)">Select All</a> 
<a href="#" onClick="selectAll(false)">Select None</a> 
<form action="" id="historical_select_form">
<div style="overflow-y: auto; height: 500px;">
<table width="100%">
<tr><th>Inbound</th><th>Outbound</th></tr>
<?cs each:channel = Channels ?>
<tr style="background-color: <?cs if:name(channel) % 2 ?>#CCCCCC<?cs else ?><?cs /if ?>">
   <td>
      <?cs if:channel.Producer.Label ?>
         <div class="legend" id="color_<?cs var:channel.Producer.Guid ?>"></div>
	 <input value="<?cs var:channel.Producer.Guid ?>" 
	        onclick="guidSelected(this)"
	        type="checkbox" <?cs if:channel.Producer.Checked ?>checked<?cs /if ?> >
         <?cs var:channel.Producer.Label ?>
      <?cs /if ?>
   </td>
   <td>
   <?cs each:source = channel.Consumer ?>
      <?cs if:source.Label ?>
         <br/><div class="legend" id="color_<?cs var:source.Guid ?>"></div>
              <input value="<?cs var:source.Guid ?>" 
      	             onclick="guidSelected(this)"
	             type="checkbox" <?cs if:source.Checked ?>checked<?cs /if ?> >
              <?cs var:source.Label ?>
      <?cs /if ?>
   <?cs /each?>
   </td>
</tr>
<?cs /each ?>
</td>
</tr>
</table>
</form>
</div>
</div>
</td></tr>
</table>
</div>

         </td>
      </tr>
   </table>
</div>

<!-- START SIDE PANEL -->
<div id="side_panel">
   
   <table id="side_table">
      <tr>
         <th id="side_header">
            Page Help
         </th>
      </tr>
      <tr>
		<td id="side_body">
                <p>		
		  Histogram showing number of messages processed in <?cs var:BarWidth/1000 ?> second intervals.
                </p>
		</td>
	  </tr>
  </table>

</div>
<!-- END SIDE PANEL -->

</body>

</html>
