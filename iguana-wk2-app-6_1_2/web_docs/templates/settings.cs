<?cs include:"doctype.cs" ?>
<html>  <?cs # vim: set syntax=html :?>
<head>
<title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Settings</title>
<?cs include:"browser_compatibility.cs" ?>
<link rel="stylesheet" type="text/css" href="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css">
<link rel="stylesheet" type="text/css" href="/js/help_popup/help_popup.css"> 
<link rel="stylesheet" type="text/css" href="/fonts.css">
<link rel="stylesheet" type="text/css" href="/action-buttons.css">
<link rel="stylesheet" type="text/css" href="/iguana.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/BTNbutton.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/source_control.css">
<?cs include:"settings_js_files.cs" ?>

<script type="text/javascript">
$(document).ready(function() {
   initIfwareSettings();
   SettingsHelpers = initSettingsHelpers();

   // Register all links on the settings pages to use the routeTo helper.
   $(document).on("click", "a", function(event){
      if ( (event.target.pathname === "/settings") && (this.hash !== "") ) {
         event.preventDefault();
         SettingsHelpers.routeTo(this.hash);
      }
   });

   // Setup MiniLogin to be used on settings pages.
   $(document).click(function checkStatus() {
      MiniLogin.loggedIn();
   });

   MiniLogin.init('<?cs var:js_escape(CurrentUser) ?>', '<?cs var:js_escape(DefaultPassword) ?>');

   <?cs if:guids ?>
   ifware.ExportGuids = JSON.parse('<?cs var:guids ?>');
   <?cs else ?>
   ifware.ExportGuids = [];
   <?cs /if ?>
});
</script>

</head>
<body class="tabright register-mini-login-click-handler">
   <?cs include:"file-browser.cs" ?>
   <?cs set:Navigation.CurrentTab = "Settings" ?>
   <?cs include:"header.cs" ?>

   <div id="main">
   </div><!--/#main-->

</body>
</html>
