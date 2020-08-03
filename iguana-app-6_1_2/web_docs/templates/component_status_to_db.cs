<?cs def:component_status_to_db() ?>
<p>
Using the VMD file '<?cs var:VmdFilePath ?>' 
<br/>to feed into a <?cs var:DatabaseApi ?> database 
<br/>using data source '<?cs var:DataSource ?>' as user <?cs var:UserName ?>.
<?cs if:ShowPresentButNull?><br/>
<br/>The VMD is <?cs var:PresentButNull ?>configured to use <a href="<?cs var:help_link('null_empty') ?>" target="_blank"> present but null handling</a>.
<br/>
<?cs /if?>
<br/><?cs var:RunningStatus ?>

</p>
<?cs /def ?>
