<?cs include:"doctype.cs" ?>

<html>
   <head>
      <title>Redirecting...</title>
      <?cs include:"browser_compatibility.cs" ?>
      <noscript>
         <?cs include:"styles.cs" ?>
      </noscript>
   </head>

   <body>

      <?cs include:"redirect_no_javascript.cs" ?>

      <form name="redirect" action="<?cs var:RedirectLocation ?>" method="<?cs var:RedirectRequestMethod ?>">
      <?cs each: SavedVariable = Saved ?>
         <input name="<?cs var:SavedVariable.Name ?>" type="hidden" value="<?cs var:html_escape(SavedVariable.Value) ?>" />
      <?cs /each ?>

    </form>

    <!-- IE seems to trash the hash when we get.  So, its better to post so we're not toast. -->
<script type="text/javascript">
(function() {
   //extract any anchors and attach them to the RedirectLocation
   var Location = "<?cs var:js_escape(RedirectLocation) ?>";
   if (null != Location.match(/^\/settings\?/)) {
      document.location = Location.replace("?", "#");
      return;
   }
   if (null != Location.match(/^\/mapper\/*/)){
      document.redirect.method = "post";
   }
   document.redirect.submit();
})();
</script>
</body>
</html>
