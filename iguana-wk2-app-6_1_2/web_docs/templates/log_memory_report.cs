<?cs include:"doctype.cs" ?>

<html>
<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Logs &gt; Memory Cache Status</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
</head>
<body class="tableft">
<?cs set:Navigation.CurrentTab = "Dashboard" ?>
<?cs include:"header.cs" ?>
<div id="main">
   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/log_browse">Logs</a> &gt; Memory Cache Status
         </td>
      </tr>

	<tr>
		<td align="center">
		<table>
		<tr>
		<td>
		<center>
		<table>
		<tr><th>Queue File Id</th>
		    <th>This gives detailed information on<br> the status of the log memory cache.</th>
		    <th>This gives detailed information on<br> the readers.</th>
		</tr>
	<?cs each: Stat = Stats ?> 
	        <tr><td valign="top"><?cs var:Stat.QueueFileId ?></td><td valign="top">
                
		<table class="configuration">
		<tr><th>Parameter</th><th>Value</th></tr>
		<tr><td>Total size of log</td><td><?cs var:Stat.TotalLogSize ?></td>
		<tr><td>Last committed position</td><td><?cs var:Stat.TotalCommittedLogSize ?></td>
		<tr><td>Segment size</td><td><?cs var:Stat.SegmentSize ?></td>
		<tr><td>Total segments</td><td><?cs var:Stat.CountOfTotalSegment ?></td>
		<tr><td>Total msgs in read cache</td><td><?cs var:Stat.CountOfReadCache ?></td>
		<tr><td>Total msgs in write cache</td><td><?cs var:Stat.CountOfWriteCache ?></td>
		</table>
		
		</td><td valign="top">
		
	        <table class="configuration" >
		<tr><th>#</th><th>Reader Position</th><th>Reader Name</th></tr>
		<?cs var:Stat.ReaderReport ?>
		</table>

		</td></tr>
	<?cs /each ?>
		</table>

		</center>
		</td>
	</tr>
   </table>

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
               This page displays the log memory cache status.
            </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_logs') ?>" target="_blank">Working with the Logs</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
</div>

</body>

</html>
