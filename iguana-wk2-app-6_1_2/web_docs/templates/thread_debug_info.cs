<?cs include:"doctype.cs" ?>

<html>

<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?> &gt; Thread Debug Info</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("iguana_configuration.css") ?>" />
    <script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>
    <script language="javascript" type="text/javascript" src="<?cs var:iguana_version_js("/flot/jquery.js") ?>"></script>
    <script language="javascript" type="text/javascript" src="<?cs var:iguana_version_js("/flot/jquery.flot.js") ?>"></script>
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
	   <a href="/">Dashboard</a> &gt; Thread Debug Info
         </td>
      </tr>

      <tr><td id="dashboard_body">
      <center>
        # Threads: <?cs var:html_escape(ThreadCount) ?>
      <table>
        <tr>
          <th><a href="/thread_debug_info.html?sort=time&order=<?cs if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">Create Time</a></th>
          <th><a href="/thread_debug_info.html?sort=name&order=<?cs if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">Name</a></th>
          <th><a href="/thread_debug_info.html?sort=threadid&order=<?cs if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">Thread ID</a></th>
        </tr>
        <?cs each:thread = Threads ?>
        <tr><td><?cs var:html_escape(thread.CreateTime) ?></td>
            <td><?cs var:html_escape(thread.DebugName) ?></td>
            <td><?cs var:html_escape(thread.ThreadId) ?></td>
        </tr>
        <?cs /each ?>
      </table>
     
     </center>
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
                <p>Thread Diagnostic Table lists Iguana internal process threads.</p>
		</td>
	  </tr>
	        <tr>
         </td>
      </tr>
	</table>

</div>
<!-- END SIDE PANEL -->

</body>

</html>
