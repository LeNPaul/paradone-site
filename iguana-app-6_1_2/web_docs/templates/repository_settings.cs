<?cs # vim: set syntax=html :?>
<link rel="stylesheet" type="text/css" href="<?cs var:iguana_version_js("/transition.css") ?>" />

<table id="iguana">
   <tr>
      <td id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; Repositories
      </td>
   </tr>

   <tr>
      <td id="dashboard_body" style="text-align: center;">
      <center>

      <?cs if:CountOfRepositories ?>
         <div id="result" class="<?cs if:error_message ?>error<?cs elif:status_message ?>success<?cs else?>indeterminate<?cs /if ?>">

            <div class="result_buttons_system">
               <a id="result_close" ><img src="images/close_button.gif"/></a>
            </div>

            <div id="result_title" class="<?cs if:error_message ?>error<?cs elif:status_message ?>success<?cs /if ?>">

               <?cs if:error_message ?>
               Error
               <?cs set:Content=error_message ?>

               <?cs else ?>
               Success
               <?cs set:Content=status_message ?>

               <?cs /if ?>

            </div>

            <div id="result_content">
               <?cs var:html_escape(Content) ?>
            </div>

         </div>

         <table id="repo-list-table" class="Repos" style="width: 80%;">
            <thead>
            <tr class="header">
               <th>Repository</th>
               <th>Path / URL</th>
               <?cs if:CanAdmin ?><th /><?cs /if ?>
            </tr>
            </thead>

            <tbody>
            <?cs each:Repo = Repositories ?>
            <tr>
               <td><strong><?cs var:html_escape(Repo.name) ?></strong></td>
               <td><?cs var: Repo.path ?></td>

               <?cs if:CanAdmin ?>
               <td>
                  <a class="repo_edit_button" href="#Page=repositories/add_or_edit?action=edit&repo_name=<?cs var:Repo.name ?>">edit</a>
                  <span>/</span>
                  <a class="remove_repo" data-repo_name="<?cs var:Repo.name ?>" href>delete</a>
               </td>
               <?cs /if ?>
            </tr>
            <?cs /each ?>
            </tbody>
         </table>
      <?cs else ?>
         <p>There are no repositories configured.</p>
      <?cs /if ?>


         <div id="buttons">
         <?cs if:CanAdmin ?>
            <a class="action-button blue" href="#Page=repositories/add_or_edit" id="add_repo">New Repository</a>
         <?cs /if ?>
         </div>

      </center>
      </td>
   </tr>
</table> <!-- End table#iguana -->
</div> <!-- End #main -->

<div id="side_panel">
   <div id="side_table">
      <div id="side_header">
         <span>Page Help</span>
      </div>
      <div id="side_body">
         <h4 class="side_title">Overview</h4>

         <p>On this page, you can configure Iguana to use local or remote Git repositories, for importing and exporting channels.</p>
         <h4 class="side_title">Related Functions</h4>
         <p><a href="/settings#Page=import">Import Channels</a></p>
         <p><a href="/settings#Page=export">Export Channels</a></p>
      </div>
      <div class="side_item">
         <h4 class="side_title">Help Links</h4>
         <p><a href="http://help.interfaceware.com/v6/addconfigure-repositories?v=6.0.0">Add and Configure Repositories</a></p>
         <p><a href="http://help.interfaceware.com/category/building-interfaces/repositories?v=6.0.0">Comprehensive Guide to Repositories</a></p>
      </div>
   </div>
</div>

<script type="text/javascript">
$(document).ready(function() {
   ifware.Settings.Repositories.setupListingPage();
});
</script>

