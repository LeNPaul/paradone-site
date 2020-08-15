<?cs # This template must produce proper JSON for the mini-browser. ?>
{"Entries":[
<?cs set: max = subcount(Entries) ?>
<?cs set: i = 0 ?>
<?cs each: entry = Entries ?> 
   {"MessageId":"<?cs var:json_escape(entry.MessageId) ?>",
    "Preview":"<?cs var:json_escape(entry.Preview) ?>"}
   <?cs set: i = #i + 1 ?>
   <?cs if: i < max ?>,<?cs /if ?>
<?cs /each ?>]
,"Sources":[
<?cs set: max = subcount(Sources) ?>
<?cs set: i = 0 ?>
<?cs each: source = Sources ?>
   {"Name":"<?cs var:json_escape(source.Name) ?>"}
   <?cs set: i = #i + 1 ?>
   <?cs if: i < max ?>,<?cs /if ?>
<?cs /each ?>]
<?cs if:Finished ?>
   ,"Complete":true
<?cs else ?>
   ,"Complete":false
   ,"Progress":"<?cs var:json_escape(html_escape(Progress)) ?>"
   ,"Continue":{
      "Position":"<?cs var:#LastPosition ?>",
      "Date":"<?cs var:json_escape(LastDate) ?>"}
<?cs /if ?>}
