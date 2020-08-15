<?cs if:ProductTier ?>
   <span><?cs var:html_escape(ProductTier) ?> - </span>
<?cs /if ?>

<a href="/version_info.html">
   v. <?cs var:html_escape(VersionInfo) ?>
</a>

<?cs if:ServerLabel ?>
   <span> - </span>
   <span class="version_info"><?cs var:html_escape(ServerLabel) ?></span>
<?cs /if ?>

