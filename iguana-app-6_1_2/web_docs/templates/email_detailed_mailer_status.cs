<?cs each:LogEntry = LogEntries ?>
   <?cs if:LogEntry.IsClientMessage ?>
      <span class="client_message">
         <?cs var:html_escape(LogEntry.Message) ?>
      </span>
   <?cs else ?>
      <span class="server_message">
         <?cs var:html_escape(LogEntry.Message) ?>
      </span>
   <?cs /if ?>
<?cs /each ?>
