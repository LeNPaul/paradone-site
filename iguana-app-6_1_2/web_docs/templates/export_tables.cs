<?cs include:"doctype.cs" ?>

<html>  <?cs # vim: set syntax=html :?>

<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Channel &gt; Export Tables</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("iguana_configuration.css") ?>" />
</head>

<body class="tableft">

<?cs set:Navigation.CurrentTab = "Dashboard" ?>
<?cs include:"header.cs" ?>


<div id="main">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/dashboard.html">Dashboard</a> &gt; <a href="/channel#Channel=<?cs var:url_escape(Channel.Name) ?>">Channel <?cs var:html_escape(Channel.Name) ?></a> &gt; Exporting Tables
         </td>
      </tr>

      <tr>
         <td id="dashboard_body">
            <center>

               <h2>Exporting Tables for Channel "<?cs var:html_escape(Channel.Name) ?>"</h2>
               
	       <?cs if:IsFilemaker && !ExportPreview ?>
                  <h3><font color="red">Warning: Cannot set primary key for a Filemaker database. See the <a href="<?cs var:help_link('file_maker') ?>"> manual entry </a> for more information.</font></h3>
               <?cs /if ?>
	       
               <?cs if:ErrorMessage ?>
                  <center>
                  <textarea class="export_error"><?cs var:html_escape(ErrorMessage) ?></textarea></center><br>
               <?cs /if ?>

               <?cs if:StatusMessage ?>
                  <center>
                  <textarea class="export_status"><?cs var:html_escape(StatusMessage) ?></textarea></center><br>
               <?cs /if ?>

               <?cs if: !ExportPreview ?>
                  <table id="buttons">
                     <tr>
                        <td>
                        <a class="action-button blue" href="/channel#Channel=<?cs var:url_escape(Channel.Name) ?>">Return to Channel Details</a>
                        </td>
                     </tr>
                  </table>
               <?cs else ?>
               <?cs if:DropTable ?>
                  <h3><font color="red">Warning: Existing tables will be dropped which may result in data loss.</font></h3>
               <?cs /if ?>
	       <?cs if:IsFilemaker ?>
                  <h3><font color="red">Warning: Cannot set primary key for a Filemaker database. See the <a href="<?cs var:help_link('file_maker') ?>"> manual entry </a> for more information.</font></h3>
               <?cs /if ?>
                  <form method="post" action="export_tables.html">
                     <textarea readonly class="export_preview" id="export_preview"
                        wrap="off"><?cs var:html_escape(ExportPreview) ?></textarea>
                     
                     <input type="hidden" name="Action" value="Export" />
                     
                     <input type="hidden" name="ChannelName"             value="<?cs var:html_escape(Channel.Name) ?>" />
                     <input type="hidden" name="ExportLogTables"         value="<?cs var:ExportLogTables ?>" />
                     <input type="hidden" name="ExportSourceTables"      value="<?cs var:ExportSourceTables ?>" />
                     <input type="hidden" name="ExportDestinationTables" value="<?cs var:ExportDestinationTables ?>" />
                     
                     <br/>
                     
                     <table id="buttons" style="margin-top:10px"><tr><td>
                        <input id="ConfirmTableExport" type="submit" class="hidden_submit" value="Confirm Table Export" />
                        <table style="border-collapse:collapse"><tr><td style="padding:0">
                           <a class="action-button blue" href="javascript:document.getElementById('ConfirmTableExport').click();"><span>Confirm Table Export</span></a>
                        </td><td style="padding:0 0 0 10px">
                           <a class="action-button blue" href="/channel#Channel=<?cs var:url_escape(Channel.Name) ?>"><span>Cancel</span></a>
                        </td></tr></table>
                     </td></tr></table>
                  </form>
               <?cs /if ?>
               
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
            <tr>
         <td id="side_body">
            <h4 class="side_title">Overview</h4>
	     <p>
	     This page lists the commands that will be used to export the database tables.
	 </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_exporting_tables') ?>" target="_blank">Exporting the Database Tables</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
   </div>
</body>

</html>

