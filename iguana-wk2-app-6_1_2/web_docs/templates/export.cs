<?cs # vim: set syntax=html :?>

<style type="text/css">
#import_export_list {
   padding-top: 0;
   margin-top: 10px;
}
#RepoSelect {
    float: right;
    margin: 16px 10px;
}
</style>

<?cs def:showError(Value) ?>
<div style="clear:both;">
 <?cs if:Value ?>
   <span style="color:red;" id="<?cs name:Value ?>"><?cs var:html_escape(Value) ?></span>
 <?cs else ?>
   <span id="<?cs name:Value ?>"></span>
 <?cs /if ?>
</div>
<?cs /def ?>

<table id="iguana">
   <tr>
      <td id="cookie_crumb">
         <a href="/settings">Settings</a><span> &gt; Export Channels</span>
      </td>
   </tr>

   <tr>
      <td id="dashboard_body">
      <?cs if:!CurrentUserCanAdmin ?>
            <p style="text-align: center">Only administrators can export channels.</p>
      <?cs else ?>
         <?cs if:ErrorMessage ?>
            <!-- Error from the server -->
            <p id="ErrorMessage"><?cs var:html_escape(ErrorMessage) ?></p>
         <?cs else ?>
            <!-- Error from Ajax/JS -->
            <p id="ErrorMessage" syle="display: none"></p>
         <?cs /if ?>

         <?cs if:StatusMessage ?>
            <p id="StatusMessage"><?cs var:html_escape(StatusMessage) ?></p>
         <?cs else ?>
            <p id="StatusMessage"></p>
         <?cs /if ?>

         <div id="import_export_list"></div>

         <div id="RepoSelect">
            <?cs if:CountOfRepositories ?>
               <label for="RepoSelector">Repository to export to: </label>
               <span class="select-style"><select name="RepoSelector" id="RepoSelector">
               <?cs each:Repo = Repositories ?>
                  <option value="<?cs var:js_escape(Repo.name) ?>"><?cs var:html_escape(Repo.name) ?></option>
               <?cs /each ?>
               </select></span>
            <?cs /if ?>
         </div><!--/RepoSelect-->

         <div id="SCMloadWheelContainer">
            <span id="SCMloadWheel" class="SCMloading"> </span>
         </div>
      <?cs /if ?>
      </td>
   </tr>
</table>

<div id="side_panel">
   <table id="side_table">
      <tr>
         <th id="side_header">Page Help</th>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Overview</h4>
            <p>On this screen, you can copy channels from this Iguana to a local or remote Git repository.</p>
            <p>Note: If a channel contains translators, the commit each translator is set to run from will be the version exported.</p>
            <h4 class="side_title">Related Functions</h4>
            <p><a href="/settings#Page=repositories">Add or edit repositories</a></p>
            <p><a href="/settings#Page=import">Import Channels</a></p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <p><a href="http://help.interfaceware.com/v6/addconfigure-repositories?v=6.0.0">Add and Configure Repositories</a></p>
            <p><a href="http://help.interfaceware.com/category/building-interfaces/repositories?v=6.0.0">Comprehensive Guide to Repositories</a></p>
         </td>
      </tr>
   </table>
</div>

<div id="helpTooltipDiv" class="helpTooltip">
   <b id="helpTooltipTitle"></b>
   <em id="helpTooltipBody"></em>
   <input type="hidden" name="helpTooltipId" id="helpTooltipId" value="0">
</div>

<script type="text/javascript">

$(document).ready(function() {
   var StarterRepoCount = <?cs var:CountOfRepositories ?>;
   if (StarterRepoCount > 0) {
      ifware.ImportManager.showLocal();
   } else {
      $("#ErrorMessage").html('<p class="error" style="margin: 40px;">To export channels, you will need to <a href="/settings#Page=repositories">configure Iguana to use one or more repositories</a>.');
   }
});
</script>

