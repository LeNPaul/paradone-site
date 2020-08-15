<?cs include:"doctype.cs" ?>

<html>

<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?> &gt; Debug Information</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("iguana_configuration.css") ?>" />
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>
   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-1.11.2.min.js"></script>
</head>

<body class="tableft" >
<div id="header">

   <header>
      <div>
        <img src="<?cs var:skin("/images/iguana_logo.png") ?>" class="iguana_logo"/>
        <span class="version">v. <?cs include:"version.cs" ?></span>
      </div>
   </header>

   <div class="breadcrumb">
   </div>
      
</div>


<!-- START MAIN CONTAINER -->
<div id="main">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
	   <a href="/">Dashboard</a> &gt; Debugging Information
         </td>
      </tr>

      <tr><td id="dashboard_body" style="height: 225px;">
      <center>

            <h2>Iguana Debugging Information</h2>
            <table>
	    <tr><td>
		<table class="configuration">
		<tr><td>Running Time</td><td><?cs var:running_time ?></td></tr>
		<tr><td>Start Time</td><td><?cs var:started_time ?></td></tr>
		<tr><td>Build Time</td><td><?cs var:build_time ?></td></tr>
		<tr><td>Release Tag Time</td><td><?cs var:release_tag_time ?></td></tr>
		</table>
	     </td></tr>
	    <tr><td>
		<table class="configuration">
		<tr><td>Version</td>
			<td <?cs if:!VersionOk ?>style="color:red;"<?cs /if ?> >
			<?cs var:VersionString ?>
			</td>
		</tr>
		<tr><td>Source DateTime</td><td><?cs var:SourceDateTime ?></td></tr>
		<tr><td>Build ID</td><td><?cs var:BuildId ?></td></tr>
		</table>
	     </td></tr>
	      <tr><td>
	        <ul>
		  <li><a href="/log_usage_statistics">Log Usage Statistics</a></li>
		  <li><a href="/log_memory_report">Log Memory Cache Usage</a></li>
		  <li><a href="/channel_positions_status">Channel Positions Status</a></li>
		  <li><a href="/logs.html?DebugMode=true">View Logs in Debug Mode (Showing hidden meta messages)</a></li>
		  <li><a href="/tps_graph.html">View TPS Report/Graph</a></li>
		  <li><a href="/historical_graph.html">View Message Histogram</a></li>
		  <li><a href="/color_list.html">Color List used for TPS graph</a></li>
		  <li><a href="/export_all_tables.html">Export database tables for all channels</a></li>
  		  <li><a href="/thread_debug_info.html">Thread Debug Info</a></li>
  		  <li><a href="/socket_diagnostic.html">Socket Diagnostic</a></li>
        <li><a id="sync_repos" style="text-decoration: underline;">Sync Repositories</a></li>
		</ul>
	    </td></tr>
       <tr><td style="text-align: center;">
         <div id='debug_spinner' style='display: none;'>Syncing...<img src='jqueryFileTree/images/spinner.gif'></div>
       </td></tr>
       </table>
         </center>
         </td>
      </tr>

   </table>
</div>
<!-- END MAIN CONTAINER -->


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
		  Tools for debugging Iguana.
                </p>
		</td>
	  </tr>
	        <tr>
         </td>
      </tr>
	</table>

</div>
<!-- END SIDE PANEL -->

<script>
      $("#sync_repos").click(syncRepos);
      function syncRepos(){
         $('#debug_spinner').css("display", "block");
         $.ajax({
            url : '/sc/sync_repos',
            method : 'GET',
            data : {},
            success : function(Response) {
               syncSuccess(Response);
            },
            error : function(Response) {
               syncError(Response);
            }
         });
      }
      function syncSuccess(Response){
         $('#debug_spinner').css("display", "none");
         console.log(Response);
         if(Response.success){
            alert(Response.description);
         }
         else if(Response.error){
            alert(Response.error.description);
         }
         else{
            console.log("No success or error key found.");
            alert(ResponseJson);
         }
      }
      function syncError(Response){
         var ResponseJson = Response.responseJSON;
         console.log(ResponseJson);
         $('#debug_spinner').css("display", "none");
         if(ResponseJson.error){
            var Error = ResponseJson.error.description;
            console.log(Error);
            alert(Error);
         }
         else{
            alert("The repository sync failed. Please try again or contact iNTERFACEWARE support.");
         }
      }
   </script>
</body>

</html>

