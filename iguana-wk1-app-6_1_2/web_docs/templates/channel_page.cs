<?cs include:"doctype.cs" ?>
<html>  <?cs # vim: set syntax=html :?>
<head>
<title>Iguana &gt; Dashboard &gt; Channel Configuration</title>
<?cs include:"browser_compatibility.cs" ?>
<link rel="stylesheet" type="text/css" href="iguana_configuration.css" /> 
<link rel="stylesheet" type="text/css" href="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css">
<link rel="stylesheet" type="text/css" href="/js/help_popup/help_popup.css" /> 
<link rel="stylesheet" type="text/css" href="/fonts.css" />
<link rel="stylesheet" type="text/css" href="/action-buttons.css" />
<link rel="stylesheet" type="text/css" href="/iguana.css" />
<link rel="stylesheet" type="text/css" href="/js/mapper/source_control.css" /> 
<?cs include:"settings_js_files.cs" ?>
<script type="text/javascript">
$(document).ready(function() {
   initIfwareSettings();
});
</script>
</head>
<body class="tableft">
<?cs include:"mini-login.cs" ?>
<?cs include:"file-browser.cs" ?>
<?cs include:"browse_macro.cs" ?>   
<?cs if: Channel.Source.IsFromChannel ?>
   <?cs linclude:"calendar.cs" ?>
<?cs /if ?>
<?cs include:'channel_status_change.cs' ?>
<?cs set:Navigation.CurrentTab = "Dashboard" ?>
<?cs include:"header.cs" ?>
<div id="main">

</div><!--/#main-->

</body>
</html>

