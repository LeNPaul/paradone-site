<?cs include:"doctype.cs" ?>

<html>
   <head>
      <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>Serious Error</title>
      <?cs include:"browser_compatibility.cs" ?>
      <?cs include:"styles.cs" ?>
   </head>
<body class="tableft">

<?cs set:Navigation.CurrentTab = "Dashboard" ?>
<?cs include:"header_panic.cs" ?>


<div id="main_panic">
   <div id="iguana">
            <h2><?cs var:html_escape(ErrorTitle) ?></h2>

      <div id="spnPanicError" class="error_message">
         <div class="error_text panic">
            Iguana has encountered a serious error. All channels have been stopped. Iguana must be restarted to resume regular service.<br>
            <br>
            Details of the error are explained below. Manual intervention may be required.<br>
            <br>
            If you have questions, please contact support@interfaceware.com<br>
         </div>
         <div class="error_heading"><br>
            ERROR DETAILS
         </div>
         <div class="error_text">
            <span id="spnPanicErrorText"><?cs var:ErrorDescription ?></span>
         </div>
      </div>
</div>

</div>      
</body>
</html>

<script type="text/javascript">
   document.addEventListener("DOMContentLoaded", function(event) { 
      var Content = document.getElementById("spnPanicErrorText").innerHTML;
      document.getElementById("spnPanicErrorText").innerHTML = Content.replace(/\r\n/g, "<br>").replace(/\n/g, "<br>");
   });
</script>
