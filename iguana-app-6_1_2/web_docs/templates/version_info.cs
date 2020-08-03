<?cs include:"doctype.cs" ?>

<html>

<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?> &gt; Version </title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("iguana_configuration.css") ?>" />
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>
</head>

<body class="tableft" >
<?cs set:Navigation.CurrentTab = "Dashboard" ?>

<?cs include:"header.cs" ?>


<!-- START MAIN CONTAINER -->
<div id="main">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
	   <a href="/">Dashboard</a> &gt; Version
         </td>
      </tr>

      <tr><td id="dashboard_body" style="height: 225px;">
      <center>

            <h2>Version Info</h2>
		<table class="configuration">
		<tr><td>Version</td>
			<td <?cs if:!VersionOk ?>style="color:red;"<?cs /if ?> >
			<?cs var:VersionString ?>
			<br><a href="http://help.interfaceware.com/v6/iguana-change-log">Change Log</a>
			</td>
		</tr>
		<tr>
			<td>Source DateTime</td>
			<td><?cs var:SourceDateTime ?></td>
		</tr>
		<tr>
			<td>Source Tag</td>
			<td><?cs var:SourceTag ?></td>
		</tr>
		<tr>
			<td>Build ID</td>
			<td><?cs var:BuildId ?></td>
		</tr>
		<tr>
			<td>IPv6 Supported</td>
			<td><?cs if:IpV6Supported ?>Yes<?cs else ?>No<?cs /if ?></td>
		</tr>
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
		          <h4 class="side_title">Overview</h4>
	     <p>
	 This page displays version information for this Iguana service.
	 </p> 
		</td>
	  </tr>
	        <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            
            <ul class="help_link_icon">
	                	<li>
            	<a href="<?cs var:help_link('iguana4_version_information') ?>" target="_blank">Displaying Version Information</a>
            	</li>
            </ul>
         </td>
      </tr>
	</table>

</div>
<!-- END SIDE PANEL -->

</body>

</html>

