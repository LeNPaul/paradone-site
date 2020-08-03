<?cs include:"doctype.cs" ?>

<html>
   <head>
      <title>Iguana Log In<?cs if:ServerLabel ?> (<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?></title>
      <?cs include:"browser_compatibility.cs" ?>
      <noscript>
         <?cs include:"styles.cs" ?>
      </noscript>
  </head>

  <body>
      <?cs include:"redirect_no_javascript.cs" ?>
      <form name="redirect" action="<?cs var:html_escape(LoginUrl) ?>" method="POST">
         <?cs each: SavedVariable = Saved ?>
            <input name="<?cs var:"Saved." + name(SavedVariable) + ".Name" ?>" type="hidden" value="<?cs var:html_escape(SavedVariable.Name) ?>" />
            <input name="<?cs var:"Saved." + name(SavedVariable) + ".Value" ?>" type="hidden" value="<?cs var:html_escape(SavedVariable.Value) ?>" />
         <?cs /each ?>
         <input id="RedirectLocation" name="RedirectLocation" type="hidden" value="<?cs var:html_escape(RedirectLocation) ?>" />
         <input name="RedirectRequestMethod" type="hidden" value="<?cs var:html_escape(RedirectRequestMethod) ?>" />
      </form>
   </body>
   <script type="text/javascript">
     //extract any anchors and attach them to the RedirectLocation
     document.getElementById('RedirectLocation').value += location.href.replace(/^[^#]*/,'');
     document.redirect.submit();
   </script>
</html>
