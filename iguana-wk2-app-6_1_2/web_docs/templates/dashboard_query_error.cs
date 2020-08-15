<?cs each: error = QueryErrors ?>
   <?cs var:html_escape(error.ErrorText) ?> in <span style="color:red">/<?cs var:html_escape(error.LeadContext) ?><?cs var:html_escape(error.ProblemArea) ?>/</span>.<br/>
<?cs /each ?>
