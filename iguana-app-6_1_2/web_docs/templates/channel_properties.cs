<style type="text/css">
#divChannelList  {
   overflow-y: auto;
   overflow-x: hidden;
}

.inner_scrollable_table {
   width: 100%;
}

select.heading_select {
   margin-top: 3px;
   color: gray;
   text-transform: none;
   letter-spacing: 0.0em;
}

option.gray {
   color: gray;
}

option.black {
   color: black;
}

.tdChannelName { width: 50%; }
.tdAutoStart,
.tdLogLevel    {
   width: 25%;
   text-align: center;
}
</style>
   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/dashboard.html">Dashboard</a> &gt; Channel Properties
         </td>
      </tr>

   <tr>
      <td id="dashboard_body">

         <?cs if:CountOfChannel == 0 ?>
            <p style="text-align: center;">
               No channels have been created. Use the <a href="/dashboard.html">dashboard</a> to add a channel.
            </p>
         <?cs else ?>

            <?cs if:SettingsUpdated == "true" ?>
            <p style="text-align:center; color:green; font-weight:bold;">
               Changes have been saved.
            </p>
            <?cs /if ?>

            <form name="ChannelProperties" id="ChannelProperties">
            <div style="text-align: left;">

               <div class="fixed">
                  <table id="tableHeadings" style="width: 100%;">
                     <tr>
                        <th class="tdChannelName">Channel</th>
                        <th class="tdAutoStart">
                           Start Automatically <input type="checkbox" id="AutoStartAll" />
                        </th>
                        <th class="tdLogLevel">
                           Logging Level <select id="LogLevelAll" class="heading_select"
                              <?cs if:!CurrentUserCanReconfigureSome ?>disabled<?cs /if ?> >
                              <option class="gray" id="optionChangeAll" value="-1">Change all...</option>
                              <?cs each:loggingLevel = LoggingLevels ?>
                                 <option class="black" value="<?cs name:loggingLevel ?>">
                                    <?cs var:loggingLevel ?>
                                 </option>
                              <?cs /each ?>
                           </select>
                        </th>
                     </tr>
                  </table>
               </div>

               <div class="scroller" id="divChannelList">
                  <table id="tableData" class="inner_scrollable_table">

                     <?cs each:channel = Channels ?>
                        <?cs if: name(channel) % 2 ?>
                        <tr id="tr.<?cs var: name(channel) ?>" class="second">  <?cs else ?>  <tr id="tr.<?cs var: name(channel) ?>" class="first">
                        <?cs /if ?>
                           <td id="tdChannelName.<?cs name:channel ?>" class="tdChannelName left" style="white-space: nowrap;">
                              <a href="/channel#Channel=<?cs var:url_escape(channel.Name) ?>"><?cs var:js_escape(channel.Name) ?></a>
                             <?cs if: channel.IsRunning ?>&nbsp(running)<?cs /if ?>
                           </td>
                           <td class="center tdAutoStart">
                              <input type="checkbox" class="AutoStart" data-guid="<?cs var: channel.Guid ?>" name="<?cs var: html_escape(channel.Name) ?>.EnableStartAutomatically"
                                 <?cs if:!channel.CanReconfigure ?>disabled<?cs /if ?>
                                 <?cs if:channel.IsStartAutomaticallyEnabled ?>checked<?cs /if ?> />
                           </td>
                           <td class="center tdLogLevel">
                              <?cs if:channel.CanReconfigure ?>
                                 <select class="LogLevel" id="selectLoggingLevel.<?cs var: name(channel) ?>" name="<?cs var: html_escape(channel.Name) ?>.LoggingLevel">
                                 <?cs each:loggingLevel = LoggingLevels ?>
                                    <option value="<?cs name:loggingLevel ?>" <?cs if:name(loggingLevel) == channel.LoggingLevel ?>selected<?cs /if ?>>
                                       <?cs var:loggingLevel ?>
                                    </option>
                                 <?cs /each ?>
                                 </select>
                              <?cs else ?>
                                 <?cs each:loggingLevel = LoggingLevels ?>
                                    <?cs if:name(loggingLevel) == channel.LoggingLevel ?><?cs var:loggingLevel ?><?cs /if ?>
                                 <?cs /each ?>
                              <?cs /if ?>
                           </td>
                        </tr>
                     <?cs /each ?>
                  </table>
               </div>

               <input type="hidden" name="Action" value="Apply Changes" />

            </div>
            </form>

            <p/><p/>

            <center>


               <table id="buttons"><tr>
                  <td>
                     <?cs if:CurrentUserCanReconfigureSome ?>
                        <a id="ApplyChanges" class="action-button blue">Save Changes</a>
                     <?cs else ?>
                        <div class="button_disable" id="DisabledEditButton" onMouseOver="TOOLtooltipLink('You do not have the necessary permissions to edit these settings.', null, this);" onMouseOut="TOOLtooltipClose();" onmouseup="TOOLtooltipClose();" ><span><img src="../<?cs var:skin("images/icon_save_disable.gif") ?>" /> &nbsp;Save Changes</span></div>
                     <?cs /if ?>
                  </td>
               </tr></table>
            </center>

         <?cs /if ?>

      </td>
   </tr>
   </table>

</div>


<div id="side_panel">

   <table id="side_table">
      <tr>
         <th id="side_header">
            Page Help
         </th>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Overview</h4>
            <p>
               On this page, you can set some of the most commonly used channel properties.
            </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_channel_properties') ?>" target="_blank">Displaying Channel Properties</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
<script type="text/javascript">
$(document).ready(function() {
   SCMbumpTickCheck($(".AutoStart"), $("#AutoStartAll"));
   var LogLevels = $(".LogLevel")
   var LogLevelAll = $("#LogLevelAll");
   LogLevels.on("change", function(event) {
      var AllThem = {};
      event.stopPropagation();
      var OneSelect = $(this);
      var Val = OneSelect.val();
      AllThem[Val] = true;
      console.log(Val);
      LogLevels.each(function() {
         AllThem[Val] = true;
         var States = Object.keys(AllThem);
         if (States.length == 1) {
            LogLevelAll.val(States[0]);
            BumpAllTick.prop("checked", false);
         } else {
            LogLevelAll.val("-1");
         }
      });
   });
   LogLevelAll.on("change", function(event) {
      if ($(this).val() != "-1") {
         LogLevels.val($(this).val());
      }
      LogLevels.trigger("change");
   });
   $("form#ChannelProperties").submit({
      uri: ifware.SettingsScreen.page(),
      form_id: "ChannelProperties"
   }, SettingsHelpers.submitForm);
   
   $("a#ApplyChanges").click(function(event) {
      event.preventDefault();
      $("form#ChannelProperties").submit();
   });
});
</script>

