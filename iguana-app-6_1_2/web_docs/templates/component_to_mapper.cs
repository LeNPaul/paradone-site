<?cs # vim: set syntax=html :?>

<?cs include: "mapper_config.cs" ?>

<?cs with: dest = Channel.Destination ?>

   <?cs call:renderMapperForm(dest, !Channel.IsNew, 'Dst', 'Destination', dest.Type, '', 0, 1) ?>
   
<?cs /with ?>
