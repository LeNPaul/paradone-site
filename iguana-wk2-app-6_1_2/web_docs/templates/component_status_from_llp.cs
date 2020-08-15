<?cs def:component_status_from_llp() ?>

<p>
Channel Listening via LLP on <?cs var:Port ?> with <?cs var:CountOfConnection ?> connections.
</p>

<?cs if:CountOfConnection != 0 ?>
<table class="configuration">
<tr><th>From</th><th>Established</th><th>Last Message</th></tr>
<?cs var:ConnectionRowData ?>
</table>
<?cs /if ?>
<?cs /def ?>
