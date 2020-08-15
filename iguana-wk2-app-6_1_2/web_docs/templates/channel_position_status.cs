<?cs include:"doctype.cs" ?>

<html>
<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Dashboard &gt; Channel Position Status</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
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
   </style>

</head>

<body class="tableft">

<?cs set:Navigation.CurrentTab = "Dashboard" ?>
<?cs include:"header.cs" ?>


<div id="main">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/dashboard.html">Dashboard</a> &gt; Channel Position Status
         </td>
      </tr>

	<tr>
		<td align="center" id="dashboard_body">
		<center>

	       	   <table class="configuration">
                   <tr><th>Position</th><th>Channel</th></tr>		
		   <?cs each:dequeue = positions ?>
		   <tr><td><?cs var:dequeue.log_id ?></td><td><?cs var:dequeue.channel_info ?></td></tr>
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
               BLAH
            </p>
	    <p>
	    </p>
         </td>
      </tr>
   </table>
</div>

</body>

</html>
