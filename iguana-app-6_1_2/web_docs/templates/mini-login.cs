<?cs # vim: set syntax=html :?>

<?cs # Include this file inside your head tag, with the script includes.
       You also need iguana.css, jQuery, and jQuery UI (Dialog). ?>

<script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajaximpl.js") ?>"></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("/mini-login.js") ?>"></script>
<script type="text/javascript">
   $(document).ready(function() {
      MiniLogin.init('<?cs var:js_escape(CurrentUser) ?>', '<?cs var:js_escape(DefaultPassword) ?>');
   });
</script>
