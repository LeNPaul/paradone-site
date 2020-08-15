<?cs include:"doctype.cs" ?>

<html>
<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Dashboard &gt; Export All Database Tables</title>
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
            <a href="/dashboard.html">Dashboard</a> &gt; Export All Tables
         </td>
      </tr>

	<tr>
		<td align="center" id="dashboard_body">
		<center>
     
 		<h3>"From Database" channels</h3>
		<?cs each:fromChannel = FromChannels ?>
			<a href="/channel#Channel=<?cs var:fromChannel.Name ?>"><?cs var:fromChannel.Name ?></a> 
			<?cs if: fromChannel.IsRunning ?>&nbsp(running)<?cs /if ?>
			<br />
		<?cs /each ?>
		<br />
		<form method="post" action="export_all_tables.html">
                     <?cs if:ErrorMessageFrom ?>
			<textarea readonly class="export_preview" id="export_preview" wrap="off"><?cs var:html_escape(ErrorMessageFrom) ?></textarea>
      		     <?cs elif:StatusMessageFrom ?>
			<textarea readonly class="export_preview" id="export_preview" wrap="off"><?cs var:html_escape(StatusMessageFrom) ?></textarea>
	             <?cs else ?>
			<textarea readonly class="export_preview" id="export_preview" wrap="off"><?cs var:html_escape(ExportPreviewFrom) ?></textarea>
	             <?cs /if ?>
                     
                     	<input type="hidden" name="Action" value="ExportFrom" />                     
                     	<br/>
                     
                	<table id="buttons" style="margin-top:10px"><tr><td>
                        <input id="ConfirmTableFromExport" type="submit" class="hidden_submit" value="Confirm Table From Export" />			
                        <table style="border-collapse:collapse"><tr><td style="padding:0">
                           <a class="action-button blue" href="javascript:document.getElementById('ConfirmTableFromExport').click();">
			   	<span>Export All Tables for "From Database" Channels</span>
			   </a>
                        </td></tr></table>
                     </td></tr></table>
   		</form>
		<br />
                <h3>"To Database" channels</h3>
		<?cs each:toChannel = ToChannels ?>
			<a href="/channel#Channel=<?cs var:toChannel.Name ?>"><?cs var:toChannel.Name ?></a> 
			<?cs if: toChannel.IsRunning ?>&nbsp(running)<?cs /if ?>
			<br />
		<?cs /each ?>
		<br />
		<form method="post" action="export_all_tables.html">
                     <?cs if:ErrorMessageTo ?>
			<textarea readonly class="export_preview" id="export_preview" wrap="off"><?cs var:html_escape(ErrorMessageTo) ?></textarea>
      		     <?cs elif:StatusMessageTo ?>
			<textarea readonly class="export_preview" id="export_preview" wrap="off"><?cs var:html_escape(StatusMessageTo) ?></textarea>
	             <?cs else ?>
			<textarea readonly class="export_preview" id="export_preview" wrap="off"><?cs var:html_escape(ExportPreviewTo) ?></textarea>
	             <?cs /if ?>
                     
                     <input type="hidden" name="Action" value="ExportTo" />
                     <br/>                     
                     <table id="buttons" style="margin-top:10px"><tr><td>
                        <input id="ConfirmTableToExport" type="submit" class="hidden_submit" value="Confirm Table To Export" />
                        <table style="border-collapse:collapse"><tr><td style="padding:0">
                           <a class="action-button blue" href="javascript:document.getElementById('ConfirmTableToExport').click();">
			   	<span>Export All Tables for "To Database" Channels</span>
			   </a>
                        </td></tr></table>
                     </td></tr></table>
   		     </form> 
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
               .
            </p>
	    <p>
	    </p>
         </td>
      </tr>
   </table>
</div>

</body>

</html>
