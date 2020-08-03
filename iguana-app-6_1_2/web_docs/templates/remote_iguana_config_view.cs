 <link rel="stylesheet" type="text/css" href="<?cs var:iguana_version_js("/transition.css") ?>" />

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt; Remote Iguana<?cs if: (RemoteType == 'remote_controllers') ?> Controller<?cs /if ?> Settings
         </td>
      </tr>

      <tr>
         <td id="dashboard_body" style="text-align: center;"><center>
         <?cs if:CountOfRemoteIguana > 0?>
            <table class="configuration" style="width: 20%" border="1">
            <tr class="header">
            <th><?cs var:html_escape(PageTitle) ?></th>
            </tr>
            <?cs each:Remote = RemoteIguana ?>
	       <td><?cs var:html_escape(Remote.OrigId) ?></td>
	       </tr>
	    <?cs /each ?>
            </table>
         <?cs else ?>
            There are no remote Iguana<?cs if: (RemoteType == 'remote_controllers') ?> Controller<?cs /if ?>s configured.
         <?cs /if ?>

      <div id="buttons">
      <!-- edit link -->
      <?cs if:CanAdmin ?>
         <a class="action-button blue" id="EditButton">Edit</a>
     <?cs /if ?>
      </div>
        </center>
         </td>
      </tr>
   </table>
</div>

<div id="side_panel">
   <div id="side_table">
      <div id="side_header">
         <span>Page Help</span>
      </div>
      <div id="side_body">
       <h4 class="side_title">Overview</h4>       
       <?cs if: (RemoteType == 'remote_iguanas') ?>
          <p>This page is used to list Iguana instances that can be accessed remotely through the dashboard. The same instances are also used when synchronizing user/permissions.</p>
          <p>Instances should be of the form 'https://server:port' or 'http://server:port'.</p>
       <?cs else ?>
          <p>This page is used to list Iguana instances that can display screens from this Iguana inside frames, through the global dashboard.</p>
          <p>Instances should be of the form 'https://server:port', 'http://server:port', or 'either://server:port'. 
          <br>
          <br>Use * to allow all ports.</p>
       <?cs /if ?>
       <h4 class="side_title">Related Functions</h4>
        <?cs if: (RemoteType == 'remote_iguanas') ?>
         <p><a href="/settings#Page=remote_controllers">Remote Iguana Controllers</a></p>
       <?cs else ?>
         <p><a href="/settings#Page=remote_iguanas">Remote Iguana Servers</a></p>
       <?cs /if ?>
      </div>
      <div class="side_item">
         <h4 class="side_title">Help Links</h4>
         <p><a href="http://help.interfaceware.com/v6/remote-iguana-settings?v=6.1.0">Remote Iguana Server Settings</a></p>
         <p><a href="http://help.interfaceware.com/v6/remote-iguana-controllers?v=6.1.0">Remote Iguana Controller Settings</a></p>
      </div>
   </div>
</div>
<script type="text/javascript">
$(document).ready(function() {
   var EditHash = '#Page=<?cs var:RemoteType ?>/edit';
   $("#EditButton").click(function(event) {
      event.preventDefault();
      if (document.location.hash == EditHash) {
         window.location.reload();
         return;
      }
      document.location.hash = EditHash;
   });
});
</script>

