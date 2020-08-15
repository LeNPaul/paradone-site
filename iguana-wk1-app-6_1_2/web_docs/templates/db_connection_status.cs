<?cs include:"doctype.cs" ?>
<?cs # vim: set syntax=html :?>
<html>
<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Dashboard &gt; Database Connections</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="jquery-ui-1-8-16/css/ui-lightness/jquery-ui-1.8.16.custom.css" />
   
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script> 
   <script type="text/javascript" src="<?cs var:iguana_version_js("/refreshv4.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/tooltip.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/jquery142/jquery-1.4.2.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("jquery-ui-1-8-16/js/jquery-ui-1.8.16.custom.min.js") ?>"></script>
   <?cs include:"mini-login.cs" ?>

   <style type="text/css">
      .subtle
      {
         text-align: left;
         color: #808080;
      }
      .started
      {
         text-align: left;
         color: #32CD32;
         text-decoration: underline;
      }
      .stopped
      {
         text-align: left;
         color: #686868;
         text-decoration: underline;
      }
      .vmd
      {
         text-align: left;
         color: red;
         text-decoration: underline;
      }
      div.connection_list
      {
         height: 100%;
         text-align: left;
         overflow-y: auto;
      }
      .hidden_heading
      {
         visibility: hidden;
      }
   </style> 
 
   <script type="text/javascript">
   
   function DBgenerateChannelUrl(ChannelName)
   {
      var ChannelUrl = "/channel#Channel=";
      ChannelUrl += escape(ChannelName);
      return ChannelUrl;
   }
   
   var DBajaxSupported = true;

   function DBisJson(Text)
   {
      return /^[\],:{}\s]*$/.test(Text.replace(/\\["\\\/bfnrtu]/g, '@').
      replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').
      replace(/(?:^|:|,)(?:\s*\[)+/g, ''));
   }
   
   function DBparseJson(Text)
   {
      if (DBisJson(Text))
      {
         return eval('(' + Text + ')');
      }
      else
      {
         throw 'The response contains invalid data.';
      }
   }
   
   function DBpopulateConnectionPoolStatus(DBconnectionStatusData)
   {
      var Text = "Iguana is configured to perform <b>case ";
      if (DBconnectionStatusData.CaseSensitivity == "0")
      {
         Text += "in"
      }
      Text += "sensitive</b> database connection pooling comparisons.<br>Idle connections "

      if (DBconnectionStatusData.UnlimitedDbIdleConnection == 0)
      {
         Text += "are set to time out after " + DBconnectionStatusData.DbIdleConnectionTime + " minute";
         if (DBconnectionStatusData.DbIdleConnectionTime > 1)
         {
            Text += "s";
         }
         Text += ".";
      }
      else
      {
         Text += "do <b>not</b> time out.";
      }
      var WarningText = document.getElementById("tdConnectionPoolingStatus");
      WarningText.innerHTML = Text;
   }

   function DBreplaceValue(Original, NewValue)
   {
      return (Original.innerHTML != NewValue)
   }
   
   function DBcreateCell(NewRow, Key, Name)
   {
      var Cell = NewRow.insertCell(-1);
      Cell.id = 'td' + Key + Name;
   }

   function DBaddRow(Key, RowIndex)
   {
      var Table = document.getElementById("dbConnectionTable");
      var NewRow = Table.insertRow(parseInt(RowIndex) + 1);
      NewRow.id = 'tr' + Key;
      
      DBcreateCell(NewRow, Key, 'Type');
      DBcreateCell(NewRow, Key, 'Database');
      DBcreateCell(NewRow, Key, 'Username');
      DBcreateCell(NewRow, Key, 'Total');
      DBcreateCell(NewRow, Key, 'Channels');
   }

   function DBcheckForRemovedRows(ConnectionList)
   {
      var Table = document.getElementById("dbConnectionTable");
      var TableLength = Table.rows.length;
      for (IndexOfRows = 1; IndexOfRows < TableLength; IndexOfRows++)
      {
         var CurrentRow = Table.rows[IndexOfRows];
         var Exists = false;
         for (IndexOfConnections in ConnectionList)
         {
            if ( CurrentRow.id == "trHeading" || CurrentRow.id == "trFooter")
            {
               Exists = true;
               break;
            }
            else if (  ( 'tr' + ConnectionList[IndexOfConnections].Key ) == CurrentRow.id )
            {
               Exists = true;
               break;
            }
            else
            {
            }
         }
         if (!Exists && parseInt(IndexOfRows))
         {
            Table.deleteRow(IndexOfRows);
            IndexOfRows--;
            TableLength = Table.rows.length;

            if (ConnectionList.length == TableLength -2)
            {
               break;
            }
         }
         
      }
   }
   
   function DBpopulateConnectionInfo(DBconnectionStatusData)
   {
      var ConnectionList = DBconnectionStatusData.Connection;
      
      for (Entry in ConnectionList)
      {
         var CurrentRow = document.getElementById("tr" + ConnectionList[Entry].Key);
         if (CurrentRow == null)
         {
            DBaddRow(ConnectionList[Entry].Key, Entry);
            CurrentRow = document.getElementById("tr" + ConnectionList[Entry].Key);
         }
         var Type = document.getElementById("td" + ConnectionList[Entry].Key + "Type");
         var Database = document.getElementById("td" + ConnectionList[Entry].Key + "Database");
         var UserName =  document.getElementById("td" + ConnectionList[Entry].Key + "Username");
         var Total = document.getElementById("td" + ConnectionList[Entry].Key + "Total")
         if (DBreplaceValue(Type, ConnectionList[Entry].Type))
         {
            Type.innerHTML = ConnectionList[Entry].Type;
         }
         if (DBreplaceValue(Database, DBescape(ConnectionList[Entry].Database)))
         {
            Database.innerHTML = DBescape(ConnectionList[Entry].Database);
         }
         if (DBreplaceValue(UserName, DBescape(ConnectionList[Entry].Username)))
         {
            
            UserName.innerHTML = DBescape(ConnectionList[Entry].Username);
         }
         
         var TotalCell = ConnectionList[Entry].Total;
         if (ConnectionList[Entry].Unused > 0)
         {
            TotalCell += " (" + ConnectionList[Entry].Unused + " unused)";
         }
         
         if (DBreplaceValue(Total, TotalCell))
         {
            
            Total.innerHTML = TotalCell;
         }
         
      }
      var Table = document.getElementById("dbConnectionTable");
      if (Table.rows.length  -2 != ConnectionList.length)
      {
         DBcheckForRemovedRows(ConnectionList);
      }
   }
   function DBescape(Text)
   {
      Text = Text.replace(/&/g, '&amp;');
      Text = Text.replace(/</g, '&lt;');
      Text = Text.replace(/>/g, '&gt;');
      Text = Text.replace(/"/g, '&quot;');
      return Text;
   }
   
   function DBpopulateChannels(ConnectionList)
   {
      for (Entry in ConnectionList)
      {
         var UniqueIdCell = document.getElementById("td" + ConnectionList[Entry].Key + "Channels");
         var ChannelList = ConnectionList[Entry].Channels;
         
         var Text= "";
         for (IndexOfChannels in ChannelList)
         {
            
            var ChannelName = ChannelList[IndexOfChannels].Name;
            var ChannelUrl = DBgenerateChannelUrl(ChannelName);
            
            var SpanTail = " onClick=\"location.assign('"+ ChannelUrl +"'); \" onmouseover=\"this.style.cursor='pointer'\">";
            if (ChannelList[IndexOfChannels].Python)
            {
               Text += "Python connection from ";
            }
            Text += "<span ";
            if (ChannelList[IndexOfChannels].Started)
            {
               Text += "class =\"started\" " + SpanTail + DBescape(ChannelList[IndexOfChannels].Name) + "</span> (running)";
            }
            else
            {
               Text += "class = \"stopped\" " + SpanTail+ DBescape(ChannelList[IndexOfChannels].Name) + "</span>";;
            }
            
            
            if (IndexOfChannels < ChannelList.length)
            {
               Text += "<br>";
            }
         }
         if (DBreplaceValue(UniqueIdCell, Text))
         {
            UniqueIdCell.innerHTML = Text;
         }
      }
   }
   
   function DBonAjaxSuccess(ResponseText, ResponseContentType)   
   {
      var DBconnectionStatusData = DBparseJson(ResponseText);
      DBpopulateConnectionPoolStatus(DBconnectionStatusData);
      DBpopulateConnectionInfo(DBconnectionStatusData);
      DBpopulateChannels(DBconnectionStatusData.Connection);
      DBserverTime = DBconnectionStatusData.Time;
      if (REFrefreshOn)
      {
         REFinitializeWithExistingInterval("DBajaxRefresh()", DBserverTime );
      }
   }

   function DBonAjaxError(ResponseText, ResponseContentType)
   {
      DBajaxSupported = false;
      REFrefreshEnable(false);
      MiniLogin.show('Iguana is not Responding', DBajaxRefresh);
   }
   
   function DBonRefreshTimer()
   {
      DBajaxRefresh();
   }

   function DBajaxRefresh()
   {
      
      if (DBajaxSupported)
      {
         // Refresh with Ajax.
         AJAXpost(window.location.protocol + '//' + window.location.host + '/db_status_refresh', '', DBonAjaxSuccess, DBonAjaxError);
      }
      else
      {
         // Refresh without Ajax.
         window.location.href = window.location.protocol + '//' + window.location.host + '/db_connection_status.html';
      }
   }
   
   function DBrefreshEnable(EnableRefresh)
   {
      var StopButton = document.getElementById("btnRefreshStart");
      var StartButton = document.getElementById("btnRefreshStop");
      REFrefreshEnable(EnableRefresh);
      if (EnableRefresh)
      {
         StopButton.display = "none";
         StartButton.display = '';
         REFinitializeWithExistingInterval("DBajaxRefresh()", DBserverTime );
      }
      else
      {
         StartButton.display = 'none';
         StartButton.display = '';
         
      }
   }
   
   var DBserverTime = null; 
   function initialize()
   {
      DBresizeWindow();
      DBserverTime = "<?cs var: Time?>";
      try
      {
         AJAXnewHttpRequest();
      }
      catch(e)
      {
         DBajaxSupported = false;
      }

      var TableRows = "<?cs var:TableRows?>";
      if (TableRows)
      {
         REFinterval = 1;
         REFrefreshOn = true;
         REFinitializeWithExistingInterval("DBajaxRefresh()", DBserverTime );
      }
      window.onresize = onResize;
      onResize();
   }

   var DBresizeTimeoutId;
   function onResize()
   {
      clearTimeout(DBresizeTimeoutId);
      DBresizeTimeoutId = setTimeout('DBresizeWindow();', 150);
   }
   
   function DBresizeDivHeight(ConnectionListDiv)
   {
      var NewHeight = (WINgetWindowHeight() - WINwindowOffsetTop(ConnectionListDiv)) - 50;

      if (NewHeight < 75)
      {
         NewHeight = 75;
      }
      ConnectionListDiv.style.height = NewHeight + 'px';
      
   }
   
   function DBresizeTables()
   {
      var HeadingsTableRow = document.getElementById('dbConnectionTableHeadings').rows[0].cells;
      var ConnectionTableRow = document.getElementById('dbConnectionTable').rows[0].cells;

      var TypeCell = ConnectionTableRow[0];
      var DatabaseCell = ConnectionTableRow[1];  
      var UsernameCell = ConnectionTableRow[2];
      var TotalCell = ConnectionTableRow[3];
      var ChannelCell = ConnectionTableRow[4];

      var TypeHeading = HeadingsTableRow[0];
      var DatabaseHeading = HeadingsTableRow[1];
      var UsernameHeading = HeadingsTableRow[2];
      var TotalHeading = HeadingsTableRow[3];
      var ChannelHeading = HeadingsTableRow[4];
      
      TypeHeading.style.width  = (TypeCell.clientWidth - parseInt(WINgetStyle(TypeHeading, 'paddingLeft')) - parseInt(WINgetStyle(TypeHeading, 'paddingRight'))) + 'px';
      
      DatabaseHeading.style.width  = (DatabaseCell.clientWidth    - parseInt(WINgetStyle(DatabaseHeading, 'paddingLeft'))    - parseInt(WINgetStyle(DatabaseHeading, 'paddingRight'))) + 'px';
      
      UsernameHeading.style.width  = (UsernameCell.clientWidth    - parseInt(WINgetStyle(UsernameHeading, 'paddingLeft'))    - parseInt(WINgetStyle(UsernameHeading, 'paddingRight'))) + 'px';
     
      // Total connection is a tricky cell because there's not much info inside
      // and the heading is long... so we always hardcode it to be 100px
      // but it doesn't work in IE.... so i'll resize everything but that row
      //TotalCell.width = '100px'; 
      //TotalHeading.style.width  = '100px';
      

      ChannelHeading.style.width  = (ChannelCell.clientWidth    - parseInt(WINgetStyle(ChannelHeading, 'paddingLeft'))    - parseInt(WINgetStyle(ChannelHeading, 'paddingRight'))) + 'px';
   }
   
   function DBresizeWindow()
   {
      
      // resize div height
      var ConnectionListDiv = document.getElementById('divConnectionList');
      var DivHeadingTable = document.getElementById('divTableHeadings'); 
      var TableData = document.getElementById('dbConnectionTable');
      var HeadingTable = document.getElementById('dbConnectionTableHeadings');
	  
	  DBresizeDivHeight(ConnectionListDiv);
      
      // resize div width
		//ConnectionListDiv.style.width = '0px';
		TableData.style.width = '0px';
		TableData.style.width = (ConnectionListDiv.clientWidth  - 1 )+ 'px';
       
      // resize table headings
      DBresizeTables();
      
      // Table heading width - chop off extra headings (only needed for firefox) 
      HeadingTable.style.width = (TableData.clientWidth + 2) + 'px';
      DivHeadingTable.style.width = ConnectionListDiv.clientWidth  + 'px'; 
   }
   
   function DBmanualRefresh()
   {
      window.location.href = window.location.protocol + '//' + window.location.host + '/db_connection_status.html';
   }

   </script>
</head>

<body class="tableft" onload="initialize();">

<?cs set:Navigation.CurrentTab = "Dashboard" ?>
<?cs include:"header.cs" ?>

<div id="main">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/dashboard.html">Dashboard</a> &gt; Database Connections
         </td>
         <td style="background-color: #e4e5e0; 
            padding-top: 6px;
            padding-bottom: 4px;
            padding-right: 17px;
            color: #787a77;
            text-align: right;
            font-size: 9px;
            font-weight: normal;
            border-bottom: 1px solid #757671;
            display: none;"
         >
         <span id="divRefreshStatus"><?cs var:html_escape(Time)?></span>
         &nbsp;
         <input type="button" id="btnRefreshStop" value="Pause" style="display: none; background-color: #DDDDDD; border: #BBBBBB 1px solid; font-family: verdana; font-size: 7pt; color: #787a77; padding-top: 0px; padding-bottom: 0px;" onclick="DBrefreshEnable(false)" />
         <input type="button" id="btnRefreshStart" value="Resume" style="display: none; background-color: #DDDDDD; border: #BBBBBB 1px solid; font-family: verdana; font-size: 7pt; color: #787a77; padding-top: 0px; padding-bottom: 0px;" onclick="DBrefreshEnable(true)" />
         <input type="button" id="btnRefreshRefresh" value="Refresh" style="display: none; background-color: #DDDDDD; border: #BBBBBB 1px solid; font-family: verdana; font-size: 7pt; color: #787a77; padding-top: 0px; padding-bottom: 0px;" onclick="DBmanualRefresh()" />
 
         </td>
      </tr>

	<tr>
		<td colspan="2" align="center" id="dashboard_body">
		<center>
		<form action="" method="post" name="db_status">

		<?cs if:TableRows ?>
         <div id="divTableHeadings" align="left" style="position: absolute; overflow:hidden;">
            <table id="dbConnectionTableHeadings" class="configuration"  >
               <tr id="trHeading"><th id="thType">Type</th><th id="thDatabase">Database</th><th id="thUsername">Username</th><th id="thTotal">Total Connections</th><th id="thChannels">Channels</th></tr>
            </table>
         </div>
         
         <div id="divConnectionList" class="connection_list" >
         
	       	   <table id="dbConnectionTable" class="configuration" style="width:100%">
                   <tr id="trHeadingHidden" class="hidden_heading"><th id="thTypeHidden" class="hidden_heading">Type</th><th id="thDatabaseHidden" class="hidden_heading">Database</th><th id="thUsernameHidden" class="hidden_heading">Username</th><th id="thTotalHidden" class="hidden_heading">Total Connections</th><th id="thChannelsHidden" class="hidden_heading">Channels</th></tr>
                   <?cs def:add_db_row(dbEntry)?>
                     
                     <tr id="tr<?cs var:html_escape(dbEntry.key)?>">
                        <td id="td<?cs var:html_escape(dbEntry.key)?>Type"> <?cs var:html_escape(dbEntry.type)?></td>
                        <td id="td<?cs var:html_escape(dbEntry.key)?>Database"><?cs var:html_escape(dbEntry.database)?></td>
                        <td id="td<?cs var:html_escape(dbEntry.key)?>Username"><?cs var:html_escape(dbEntry.username)?></td>
                        <td id="td<?cs var:html_escape(dbEntry.key)?>Total"><?cs var:html_escape(dbEntry.total)?><?cs if:dbEntry.unused != 0?> (<?cs var:html_escape(dbEntry.unused)?> unused)<?cs /if ?> </td>
                        <td id="td<?cs var:html_escape(dbEntry.key)?>Channels">
                        <?cs each:Channel = dbEntry.channel ?>
                           <?cs if:Channel.vmd ?>Python connection from <?cs /if?>
                           <span <?cs if:Channel.started?>
                                  class="started"
                                  <?cs else?>
                                  class="stopped"
                                  <?cs /if?>     onClick="" onmouseover="this.style.cursor='pointer'"> <?cs var:html_escape(Channel.name)?></span><?cs if:Channel.started?>&nbsp(running)<?cs /if ?>
                           <br>

                        <?cs /each ?>
                        </td>
                     </tr>
                   <?cs /def ?>
                   <?cs each:row = Connection ?>
                     <?cs call:add_db_row(row)?>
                   <?cs /each?>
                   
                   <tr id="trFooter" class="footer">
            
               
            
            <td colspan="5" >
               <table width=100%><tr>
                  <td >
                  <span class="subtle" id="tdConnectionPoolingStatus">
                  Iguana is configured to perform <b>case <?cs if:CaseSensitivity == 0 ?>in<?cs /if?>sensitive</b> database connection pooling comparisons.
                  <?cs if:UnlimitedDbIdleConnection == 0?>
                     <br>Idle connections are set to time out after <?cs var:#DbIdleConnectionTime?> minute<?cs if:#DbIdleConnectionTime > 1 ?>s<?cs /if ?>.
                  <?cs else ?>
                     <br>Idle connections do <b>not</b> time out.
                  <?cs /if ?>
                  </span>
                  </td>
                  <td>
                     <?cs if:CurrentUserCanAdmin ?>
                        <a class="button" style="float:right" href="javascript:document.getElementById('ClearConnections').click();"><span>Clear Unused Connections</span></a>
                     <?cs else ?>
                        <div style="float:right" class="button_disable"
                           onMouseOver="TOOLtooltipLink('You do not have the necessary permissions to clear unused database connections.', null, document.getElementById('ToolTipRelocate'));"
                           onMouseOut="TOOLtooltipClose();"
                           onmouseup="TOOLtooltipClose();">
                           <span>Clear Unused Connections</span>
                        </div>
                        </td>
                        <td><img id="ToolTipRelocate" src="images/sort_spacer.gif" style="width:0px;height:0px;position:relative;right:85px;">
                     <?cs /if ?>
                  </td>
               </tr></table>
   		   </td>
		</tr>
		   </table>
                <input id="ClearConnections" type="submit" class="hidden_submit" value="Clear Connections" name="clear_connections" />	
		<?cs else ?>
Under Construction...
		<?cs /if ?>
		</form>
		</center>

        </td>
	</tr>
   </table>
   </div>
</div>
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
               On this page, you can view the status of Iguana's database connection pool.
            </p>
	    <p>
                See <a href="databases.html">Settings &gt; Databases</a> for server-wide database configuration options. 
	    </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_viewing_database_connections') ?>" target="_blank">Viewing the Database Connections</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
</div>

</body>
</html>
