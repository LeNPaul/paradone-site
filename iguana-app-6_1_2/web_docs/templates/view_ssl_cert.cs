<?cs include:"doctype.cs" ?>

<?cs # vim: set syntax=html :?>

<html>

<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>
     &gt; Settings
     &gt; <?cs var:html_escape(ChannelName) ?>
   <?cs if:ComponentType ?>
     &gt; <?cs var:html_escape(ComponentType) ?> : <?cs var:html_escape(ComponentName) ?>
   <?cs /if ?>
     &gt;
   <?cs if:CertificateType=='Authority' ?>
   Certificate Authority File Properties
   <?cs else ?>
   Certificate File Properties
   <?cs /if ?>
   </title>

   <?cs include:"styles.cs" ?>
   <?cs include:"browser_compatibility.cs" ?>
</head>

<body class="tabright">

<?cs include:"header.cs" ?>


<div id="main">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt;
            <?cs if:ChannelName=='Web Server' ?>
                <a href="/settings#Page=web_settings/view">Web Server</a>
            <?cs elif:ChannelName=='HTTPS Channel Settings' ?>
                <a href="/settings#Page=https_channel_settings">HTTPS Channel Settings</a>
            <?cs else ?>
                <a href="/channel#Channel=<?cs var:url_escape(ChannelName) ?>"><?cs var:html_escape(ChannelName) ?></a>
            <?cs /if ?>
            <?cs if:ComponentType ?>
            &gt; <a href="/channel#Channel=<?cs var:url_escape(ChannelName) ?>&editTab=<?cs var:url_escape(ChannelTab) ?>"><?cs var:html_escape(ComponentType) ?> : <?cs var:html_escape(ComponentName) ?></a>
            <?cs /if ?>
            &gt;
            <?cs if:CertificateType=='Authority' ?>
               Certificate Authority File Properties
            <?cs else ?>
               Certificate File Properties
            <?cs /if ?>
         </td>
      </tr>

      <tr>
         <td id="dashboard_body">
            <center>
               <?cs if:ErrorMessage ?>
                  <h3><font color="red" id="test.ErrorMessage"><?cs var:html_escape(ErrorMessage) ?></font></h3>
	       <?cs else ?>
	          <span id="test.ErrorMessage"></span>
               <?cs /if ?>

               <?cs if:StatusMessage ?>
                  <h3><font color="green" id="test.StatusMessage"><?cs var:html_escape(StatusMessage) ?></font></h3>
               <?cs else ?>
	       	  <span id="test.StatusMessage"></span>
	       <?cs /if ?>


      <!-- begin display area -->

         <?cs if:CertificateInfo.HaveError ?>
            <p>Error reading certificate:</p>
            <p><font color="red"><?cs var:html_escape(CertificateInfo.Error) ?></font></p>
	 <?cs /if ?>
	 <?cs if:CertificateInfo.HaveData ?>
            <table class="configuration" border="1" id="test.certificate">
               <tr class="header">
                  <th colspan="2">Certificate Properties</th>
               </tr>
               <tr>
                 <td class="left_column">File Path</td>
                  <td><div id="test.CertificateInfo.FileName"><?cs var:html_escape(CertificateInfo.FileName) ?></div></td>
               </tr>
               <tr>
                  <td class="left_column">Version</td>
                  <td><div id="test.CertificateInfo.Version"><?cs var:html_escape(CertificateInfo.Version) ?></div></td>
               </tr>
               <tr>
                  <td class="left_column">Issuer</td>
                  <td><div id="test.CertificateInfo.Issuer"><?cs var:html_escape(CertificateInfo.Issuer) ?></div></td>
               </tr>
               <tr>
                  <td class="left_column">Subject</td>
                  <td><div id="test.CertificateInfo.Subject"><?cs var:html_escape(CertificateInfo.Subject) ?></div></td>
               </tr>
               <tr>
                  <td class="left_column">Signature Algorithm</td>
                  <td><div id="test.CertificateInfo.SignatureAlgorithm"><?cs var:html_escape(CertificateInfo.SignatureAlgorithm) ?></div></td>
               </tr>
               <tr>
                  <td class="left_column">Serial Number</td>
                  <td><div id="test.CertificateInfo.SerialNumber"><?cs var:html_escape(CertificateInfo.SerialNumber) ?></div></td>
               </tr>
               <tr>
                  <td class="left_column">Not Valid Before</td>
                  <td><div id="test.CertificateInfo.NotValidBefore"><?cs var:html_escape(CertificateInfo.NotValidBefore) ?></div></td>
               </tr>
               <tr>
                  <td class="left_column">Not Valid After</td>
                  <td><div id="test.CertificateInfo.NotValidAfter"><?cs var:html_escape(CertificateInfo.NotValidAfter) ?></div></td>
               </tr>
            </table>
	 <?cs /if ?>


         <p/><p/>

      <!-- end display area -->

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
            <?cs if:ComponentType ?>
   <?cs if:CertificateType=='Authority' ?>
	      This page shows the properties of the certificate authority file that your channel is using.</p>
   <?cs else ?>
	      This page shows the properties of the certificate file that your channel is using.</p>
   <?cs /if ?>
   <?cs else ?>
   This page shows the properties of the certificate file that your web server is using.</p>
   <?cs /if ?>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            <?cs if:ComponentType ?>
            	<li>
            	<a href="<?cs var:help_link('iguana4_llp_listener_certificate') ?>" target="_blank">Viewing the LLP Listener Certificate File</a>
            	</li>
            	<li>
            	<a href="<?cs var:help_link('iguana4_llp_client_certificate') ?>" target="_blank">Viewing the LLP Client Certificate File</a>
            	</li>
            	<li>
            	<a href="<?cs var:help_link('iguana4_to_https_certificate') ?>" target="_blank">Viewing the To HTTPS Certificate File</a>
            	</li>
   <?cs else ?>
            	<li>
            	<a href="<?cs var:help_link('iguana4_https') ?>" target="_blank">Turning HTTPS Support On</a>
            	</li>
   <?cs /if ?>
            </ul>
         </td>
      </tr>
   </table>
</div>

</body>

</html>

