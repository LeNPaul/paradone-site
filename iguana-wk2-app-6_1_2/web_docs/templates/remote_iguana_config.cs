<link rel="stylesheet" type="text/css" href="<?cs var:iguana_version_js("/transition.css") ?>" />

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt; Remote Iguana<?cs if: (RemoteType == 'remote_controllers') ?> Controller<?cs /if ?> Settings
         </td>
      </tr>
      <tr>
         <td id="dashboard_body" style="text-align: center;"><center>

    <form name="remote_iguana_settings" id="remote_iguana_settings" method="post">
    <input type="hidden" id="max_row_id" name="max_row_id" value="<?cs var:html_escape(CountOfRemoteIguana) ?>">
    <div>
         <table class="configuration" id="remote_iguana_table" border="1">
            <tr class="header">
            <th><?cs var:html_escape(PageTitle) ?></th>
            <th>Add/Remove</th>
            </tr>
            <?cs each:Remote = RemoteIguana ?>
            <tr id="row_<?cs name:Remote ?>" >
          <td class="remote_iguana_entry">
                   <input type="hidden" name="OrigRemoteIguana_<?cs name:Remote ?>"
                     value="<?cs var:html_escape(Remote.OrigId) ?>">
                   <input type="text" name="RemoteIguana_<?cs name:Remote ?>"
                     value="<?cs var:html_escape(Remote.Id) ?>">
                   <?cs if:Remote.Error ?><small style="color: #e00101"><?cs var:html_escape(Remote.Error) ?></small><?cs /if ?>
              </td>
          <td><a class="remove_button action-button-small blue">Remove</a></td>
            </tr>
       <?cs /each ?>
       <?cs if:CanAdmin ?>
       <tr id="adding_row">
              <td class="remote_iguana_entry"><input type="text" id="NewRowField"></input>
                  <small style="color: #e00101"><span id="new_row_error"></span></small>
              </td>
              <td><a class="action-button-small blue" id="add_button">Add</a></td>
            </tr>
       <?cs /if ?>
    </table>
    </div>
      <p>

      <div id="buttons">
      <!-- edit link -->
      <?cs if:CanAdmin ?>
         <input type="hidden" name="SaveChanges" value="yes" />
         <a class="action-button blue" id="apply_changes_button">Save Changes</a>
      <?cs else ?>
         <span class="config_warning">You do not have the necessary permissions to edit these settings.</span>
     <?cs /if ?>
    </form>
         <a class="action-button blue" href="/settings#Page=<?cs var:RemoteType ?>"><span>Cancel</span></a>
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
   <?cs if: (RemoteType == 'remote_controllers') ?>
   var AddressRegex = /^(http|https|either):\/\/[^:]+:[0-9\*]+/;
   var SampleAddress = "[http/https/either]://<host>:<port or *>";
   <?cs else ?>
   var AddressRegex = /^(http|https):\/\/[^:]+:[0-9]+/;
   var SampleAddress = "[http/https]://<host>:<port>";
   <?cs /if ?>
   var RowId = <?cs var:js_escape(CountOfRemoteIguana) ?>;
   function addRemoteRow(){
      function noDuplicates(NewValue){
         var CountOfDuplicates = 0;
         $(':input[type=text]').each( function(){
            if (this.value == NewValue) CountOfDuplicates++;
         });

         //the select includes the adding input field.
         if (CountOfDuplicates > 1){
            $('#new_row_error').text( 'This entry is already in the list of remote iguanas.' );
            return false;
         }
         return true;
      }
      var NewRowValue = $('#NewRowField').prop('value');
      if (NewRowValue){
         if(!NewRowValue.match(AddressRegex)) {
            $('#new_row_error').text( 'Invalid format. New entry should be of the form ' + SampleAddress);
         } else if (noDuplicates(NewRowValue)) {
            $('#new_row_error').text('');
            var RowHtml = '<tr id="row_' + RowId + '">'
                        + '  <td><input size="50" type="text" name="RemoteIguana_' + RowId + '"></td>'
                        + '  <td><a class="remove_button action-button-small blue">Remove</a></td>'
                        + '</tr>';

            $(RowHtml).insertBefore($('#adding_row'));
            $(':input[name=RemoteIguana_'+RowId+']').prop('value',NewRowValue);
            RowId++;
            $('#NewRowField').prop('value','');
            return true;
         }
      } else {
         return true; //doing nothing is success
      }
      return false;
   }
   $("form#remote_iguana_settings").submit({uri: '<?cs var:RemoteType ?>/edit', form_id: "remote_iguana_settings"}, SettingsHelpers.submitForm);
   function doSubmit(){
      if(addRemoteRow()){
         $('#max_row_id').val(RowId);
         $("form#remote_iguana_settings").submit();
      }
   }

   function onLoad(){
      $(document).on('click', '.remove_button', function(event) {
         event.preventDefault();
         var ElRowId = $(this).parent().parent().prop('id');
         $('#' + ElRowId).remove();
      });

      $(document).on('click', '#add_button', function() {
         addRemoteRow();
      });

      //this sets up Enter key behaviour to add a row
      $(document).keypress(function(e){
         if (e.keyCode === 13) {
            e.preventDefault();
            addRemoteRow();
         }
      });
   }
   $("#apply_changes_button").click(doSubmit);
   onLoad();
});
</script>


