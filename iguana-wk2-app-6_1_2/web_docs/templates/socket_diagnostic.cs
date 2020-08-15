<?cs include:"doctype.cs" ?>

<html>

<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?> &gt; Socket Diagnostic</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("iguana_configuration.css") ?>" />
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
	   <a href="/">Dashboard</a> &gt; Socket Diagnostic
         </td>
      </tr>

      <tr><td id="dashboard_body">
      <center>
        Sockets: <?cs var:html_escape(SocketCount) ?>
      <table>
        <tr>
          <th><a href="/socket_diagnostic.html?sort=handle&order=<?cs  if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">Handle</a></th>
          <th><a href="/socket_diagnostic.html?sort=connecttime&order=<?cs   if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">ConnectTime</a></th>
          <th><a href="/socket_diagnostic.html?sort=localport&order=<?cs   if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">LocalPort</a></th>
          <th><a href="/socket_diagnostic.html?sort=remotehostname&order=<?cs  if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">RemoteHostName</a></th>
          <th><a href="/socket_diagnostic.html?sort=remotehostip&order=<?cs if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">RemoteHostIP</a></th>
          <th><a href="/socket_diagnostic.html?sort=remoteport&order=<?cs if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">RemotePort</a></th>
          <th><a href="/socket_diagnostic.html?sort=bytesreceived&order=<?cs if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">BytesReceived</a></th>
          <th><a href="/socket_diagnostic.html?sort=bytessent&order=<?cs    if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">BytesSent</a></th>
          <th><a href="/socket_diagnostic.html?sort=lastreceivetime&order=<?cs  if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">LastReceiveTime</a></th>
          <th><a href="/socket_diagnostic.html?sort=lastsendtime&order=<?cs if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">LastSendTime</a></th>
          <th><a href="/socket_diagnostic.html?sort=state&order=<?cs if:SortOrder=='asc'?>desc<?cs else ?>asc<?cs /if ?>">State</a></th>
        </tr>
        <?cs each:sock = Socket ?>
        <tr>
            <td><?cs var:html_escape(sock.Handle) ?></td>
            <td><?cs var:html_escape(sock.ConnectTime) ?></td>
            <td><?cs var:html_escape(sock.LocalPort) ?></td>
            <td><?cs var:html_escape(sock.RemoteHostName) ?></td>
            <td><?cs var:html_escape(sock.RemoteHostIP) ?></td>
            <td><?cs var:html_escape(sock.RemotePort) ?></td>
            <td><?cs var:html_escape(sock.BytesReceived) ?></td>
            <td><?cs var:html_escape(sock.BytesSent) ?></td>
            <td><?cs var:html_escape(sock.LastReceiveTime) ?></td>
            <td><?cs var:html_escape(sock.LastSendTime) ?></td>
            <td><?cs var:html_escape(sock.State) ?></td>
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
                <p>		
		          Socket Diagnostic shows Iguana web and LLP socket usage.
                </p>
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
