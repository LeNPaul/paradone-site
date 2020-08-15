   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/">Dashboard</a> &gt; Removed Channel <?cs var:html_escape(Channel) ?>
         </td>
      </tr>

      <tr>
         <td align="center" id="dashboard_body">
            <center>
            <?cs if:StatusMessage ?>
              <!-- TODO font tag -->
              <h3><font color="green"><?cs var:html_escape(StatusMessage) ?></font></h3>
            <?cs /if ?>
	    <table>
            <tr><td><a class="action-button blue" href="/"><span>Return to Dashboard</span></a></td></tr>
	    </table>
            </center>
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
		This page is displayed when a channel is removed.
	   </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_removing_channel') ?>" target="_blank">Removing a Channel</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
