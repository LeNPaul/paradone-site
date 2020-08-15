<?cs include:"doctype.cs" ?>

<html>
   <head>
      <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Dashboard &gt; Exception</title>
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
            <a href="/">Dashboard</a> &gt; Exception
         </td>
      </tr>
      <tr>
         <td id="dashboard_body">
            <h2><?cs var:html_escape(ErrorTitle) ?></h2>
      <div id="spnPanicError" class="error_message">
         <div class="error_heading">
            ERROR
         </div>
         <div class="error_text">
            <span id="spnPanicErrorText"><?cs var:ErrorDescription ?></span>
         </div>
      </div>
	 </td>
      </tr>
</table>

</div>      
</body>
</html>
