<?cs include:"doctype.cs" ?>
<html>
<head>
<title>Iguana <?cs if:ServerName ?>(<?cs var:html_escape(ServerName) ?>) <?cs /if ?></title>
<?cs include:"browser_compatibility.cs" ?>
<!-- Please keep alphabetical order with includes please!! -->

<link rel="stylesheet" type="text/css" href="/js/CodeMirror-4.11/lib/codemirror.css">
<link rel="stylesheet" type="text/css" href="/js/CodeMirror-4.11/theme/ifware.css">
<link rel="stylesheet" type="text/css" href="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css">
<link rel="stylesheet" type="text/css" href="/js/jquery-1.11.2/jquery.svg.css">

<link rel="stylesheet" type="text/css" href="/fonts.css">
<link rel="stylesheet" type="text/css" href="/action-buttons.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/ANNannotation2.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/annotation.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/BTNbutton.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/bulk_test_results.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/dock.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/editor.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/error.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/ifware-hl.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/intellisense.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/node_treeview.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/node_types.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/project_manager.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/sample_data.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/source_control.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/toolbar.css">
<link rel="stylesheet" type="text/css" href="/js/tooltip/tooltip.css">
<link rel="stylesheet" type="text/css" href="/js/treeview/treeview.css">
<link rel="stylesheet" type="text/css" href="/js/treeview22/treeview22.css">
<link rel="stylesheet" type="text/css" href="/mapper/mini-login.css">
<link rel="stylesheet" type="text/css" href="/mini-browser.css">
<link rel="stylesheet" type="text/css" href="/mapper/iguana_editor.css">
<link rel="stylesheet" type="text/css" href="/js/google-code-prettify/prettify.css">
<link rel="stylesheet" type="text/css" href="/js/mapper/sample_data_editor.css">

<!-- Do not add JavaScript files here; add them to the makefile in JSZ -->
<?cs include:"translator_js_files.cs" ?>
<script type="text/javascript">
   $(document).ready(function() {
      MiniLogin.init('<?cs var:js_escape(CurrentUser) ?>',
                     '<?cs var:js_escape(DefaultPassword) ?>');
   });
</script>
<style type="text/css">
pre.prettyprint {
   border: none !important;
}
</style>
</head>
<body>
<div class="EDTloading" />
</body>
</html>
