<?cs include:"doctype.cs" ?>

<html>

<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?> &gt; Channel Routes</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>

   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css") ?>">
   
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("iguana_configuration.css") ?>" />
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>

   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-1.11.2.min.js"></script>
   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.min.js"></script>

<style type="text/css" >

th.routes{
   height: 20px; 
   width: 400px;
}

th.routes_arrow{
   width: 25px;
}

td.routes{
   vertical-align: middle; 
   text-align: left;
}

</style>


<script id="source" language="javascript" type="text/javascript">

function channelLink(Name)
{
   return '<a href="/channel#Channel=' + encodeURIComponent(Name)  + '">' + Name + '</a>'; 
}

var Model = null;
var DisplayMode = 'by_source';
var DisplayAsc = true;

function tableHtml()
{
   var sortArrowHtml = function(ColumnType)
   {
      Out = '';
      if (ColumnType == DisplayMode)
      {
         imgStyle = 'style="vertical-align:middle; margin-left: 5px; padding-bottom: ' + (DisplayAsc ? ' ' : '-') + '4px"';
	 Out = '<img '+ imgStyle + ' src="/images/' + (DisplayAsc ? 'sort_up.png' : 'sort_down.png') + ' ">'
      }
      return Out;
   }

   return '<table style="border-collapse: separate">' +
          '<tr><th class="routes" id="source_to_dest">Source Channel' + sortArrowHtml('by_source') + '</th>' + 
          '<th class="routes_arrow"></th>' + 
          '<th class="routes" id="dest_to_source">Destination Channel' + sortArrowHtml('by_dest') + '</th></tr></table>';
}

function tableRow(Left,Right)
{
   return '<tr><td class="routes">' + Left + '</td><td><img src="/images/arrow_right.gif"></td><td class="routes">' + Right + '</td></tr>';
}

function collectionIter(Collection, DirAsc, Action, Context)
{
   if (DirAsc) for ( Key in Collection ){ Action(Collection, Key, Context) }
   else
   {
      Keys = []
      for (Key in Collection) Keys.push(Key);
      Keys.reverse()
      for (Index in Keys){ Action(Collection, Keys[Index], Context) }
   }
}

function checkViewable(ChannelGuid, RouteMap)
{
   for (RouteIndex in RouteMap[ChannelGuid] )
   {
      if ( Model.Channels[ RouteMap[ChannelGuid][RouteIndex] ] ) return true;
   }
   return false;
}

var NoDestPermissionMsg ='You do not have view permissions for the destination channels';
var NoSourcePermissionMsg = 'You do not have view permissions for the source channels';

function addNoneRow(JTable, Channel)
{
   if (Channel.SourceType == 'From Channel') JTable.append(tableRow('None',channelLink(Channel.Name)));
   else JTable.append(tableRow(channelLink(Channel.Name), 'None'));
}

function displayBySource()
{
   $('#route_panel').empty();
   JTable = $(tableHtml());
   if (Model)
   {
      Action = function(Channels,Guid)
      {
         var Channel = Channels[Guid];
         var ConnectionHtml = '';
	 var Destinations = [];
	 var CountOfViewable = 0;
	 var DestList = Model.SourcesToDestinations[Guid];
         var UnviewableDestinations = [];
	 for (ConnectionIndex in DestList)
         {
	    var DestChannel = Channels[ DestList[ConnectionIndex] ];
	    if (DestChannel) Destinations.push( DestChannel.Name );
	    else UnviewableDestinations.push( DestList[ConnectionIndex] );
         }
	 Destinations.sort();
	 if (!DisplayAsc) Destinations.reverse();
	 if (Destinations.length == 0)
         {
	     if (UnviewableDestinations.length)
	     {
		JTable.append(tableRow(channelLink(Channel.Name),NoDestPermissionMsg));
	     }
	     else if (!Model.DestinationsToSources[Guid])
	     {
	        addNoneRow(JTable, Channel);
	     }
	     else if (!checkViewable(Guid, Model.DestinationsToSources))
	     {
		JTable.append(tableRow(NoSourcePermissionMsg,channelLink(Channel.Name)));
	     }
	 }
	 else 
	 {
	    for(Dest in Destinations) ConnectionHtml += channelLink(Destinations[Dest]) + '<br>';
            if (ConnectionHtml != '') JTable.append(tableRow(channelLink(Channel.Name),ConnectionHtml));
         }
      }
      collectionIter(Model.Channels, DisplayAsc, Action);
   }
   $('#route_panel').append(JTable);
}

function displayByDestination()
{
   $('#route_panel').empty();
   JTable = $(tableHtml());
   if (Model)
   {
      Action = function(Channels,Guid)
      {
         var Channel = Channels[Guid];
         var SourcesHtml = '';
	 var Sources = [];
         var SourceList = Model.DestinationsToSources[Guid];
	 var UnviewableSources = [];
         for (ConnectionIndex in SourceList)
         {
	    var SourceChannel = Channels[ SourceList[ConnectionIndex] ];
	    if (SourceChannel) Sources.push(SourceChannel.Name);
	    else UnviewableSources.push(SourceList[ConnectionIndex]);
         }
	 Sources.sort();
	 if (!DisplayAsc) Sources.reverse();
	 if (Sources.length == 0)
	 {
	     if (UnviewableSources.length)
	     {
		JTable.append(tableRow(NoSourcePermissionMsg ,channelLink(Channel.Name)));
	     }
	     else if (!Model.SourcesToDestinations[Guid])
	     {
	        addNoneRow(JTable, Channel);
	     }
	     else if (!checkViewable(Guid, Model.SourcesToDestinations))
	     {
		 JTable.append(tableRow(channelLink(Channel.Name), NoDestPermissionMsg));
	     }
	 }
	 else
	 {
	    for(Src in Sources) SourcesHtml += channelLink(Sources[Src])  + '<br>';
            if (SourcesHtml != '') JTable.append(tableRow(SourcesHtml ,channelLink(Channel.Name)));
         }
      }
      collectionIter(Model.Channels, DisplayAsc, Action);
   }
   $('#route_panel').append(JTable);
}

function displayError(Error)
{
   $('#route_panel').empty();
   JError = $('<div style="text-align: center;"></div>').append(Error);
   $('#route_panel').append(JError);
}

function setupColumn(columnId, onClickAction)
{
   $('#' + columnId)
      .addClass('sortable')	
      .click(onClickAction)
      .mouseover(function(){ $(this).removeClass('sortable').addClass('sortable_hover') } )
      .mouseout(function(){ $(this).removeClass('sortable_hover').addClass('sortable') });
}

function toggleDisplaySort(NewMode)
{
   if (DisplayMode != NewMode) DisplayAsc = true;
   else DisplayAsc = !DisplayAsc;
   DisplayMode = NewMode;
}

function doDisplay()
{
   if (!Model.CountOfAllChannels) {
      displayError('There are no channels configured.');
   }
   else if(!Model.CountOfViewableChannels) {
      displayError('You do not have view permissions for any channel.');
   }
   else if (DisplayMode == 'by_dest') displayByDestination();
   else displayBySource();

   $('tr:odd').addClass('first');
   $('tr:even').addClass('second');

   setupColumn('source_to_dest', function(){
      toggleDisplaySort('by_source');
      doDisplay();
   })
   setupColumn('dest_to_source', function(){
      toggleDisplaySort('by_dest');
      doDisplay();
   })
}

function routesUpdateComplete(Data)
{
   Model = eval(Data);
   doDisplay();
}

function routesUpdateError(error)
{
   displayError(error);
}

function requestUpdate()
{
   AJAXpost('route_info',
	    '',
            routesUpdateComplete,
            routesUpdateError
   );
}

$(function() {
   requestUpdate();
});
</script>

</head>

<body class="tableft">

<?cs set:Navigation.CurrentTab = "Dashboard" ?>
<?cs include:"header.cs" ?>

<!-- START MAIN CONTAINER -->
<div id="main">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
	   <a href="/">Dashboard</a> &gt; Channel Routes
         </td>
      </tr>

      <tr><td style="text-align: center;" id="dashboard_body">
          <center><div id="route_panel"></div></center>
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
             <h4 class="side_title">Overview</h4>
            <p>
		This page lists all channel routes that are defined for this Iguana server.
		Messages may be routed from each source channel to its specified destination channel(s).</p>
		<p>
		The routes listed here can be sorted by source channel or by destination channel.
            </p>
        </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('channel_routes') ?>" target="_blank">Displaying the Channel Routes</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
</div>
<!-- END SIDE PANEL -->

</body>

</html>
