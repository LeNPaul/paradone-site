{

<?cs if:ErrorMessage ?>
  "Error":"<?cs var:json_escape(ErrorMessage) ?>",
  "Success":false
<?cs else ?>
  "Success":true
<?cs /if ?>

<?cs if:CriticalError ?>
, "log_validate_error" : true,
  "log_validate_error_message" : "<?cs var:json_escape(CriticalErrorMessage) ?>"
<?cs /if ?>

<?cs if:LogsInvalid ?>
   , "logs_invalid" : true
<?cs elif:LockerRequired  ?>
   , "locker_required" : true
<?cs elif:LockerLocked  ?>
   , "locker_locked" : true
<?cs /if ?>

<?cs if:LogsInitialized ?>
   , "logs_initialized" = true
<?cs else ?>
   , "logs_initialized" = false
<?cs /if ?>

}
