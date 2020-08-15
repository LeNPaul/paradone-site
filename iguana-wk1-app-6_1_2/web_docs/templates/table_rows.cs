<?cs # If no rows were found, but we were looking for some chunk other than the first,
       the problem may be that this chunk no longer exists, not necessarily that no
       rows were inserted.  If this is the case, we won't display anything. ?>
<?cs if:TableFound && !RowsFound && ChunkIndex == 0 ?>
   <span style="color:#666666;">No rows were inserted into this table.</span>
<?cs elif:TableFound && RowsFound ?>
<table class="parsedTableView">
   <tr>
      <td class="empty">
   <?cs each:row = Rows ?>
      <td class="header">
         <a id="grammerTool-<?cs var:name(row) ?>" name="grammerTool-<?cs var:name(row) ?>"
            class="helpIcon" rel="<?cs var:row.Location ?>" title="Grammar Information" href="#"
            onClick="initializeHelp(this,event);" style="zoom:1;">Row Location...</a>
   <?cs /each ?>
<?cs each:column = Columns ?>
   <tr>
      <td class="header"><?cs var:html_escape(column.Name) ?>
      <?cs each:value = column.Rows ?><td><?cs var:value.Value ?><?cs /each ?>
<?cs /each ?>
</table>
<?cs /if ?><? # else, display nothing ?>
