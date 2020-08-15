<div class="log_locker_update_dialog" id="password_update_dialog" style="display: none;">
   <table width="100%">
      <tr class="validate_error" style="display: none;">
         <td colspan="2" align="center">
           <span style="color: red;"></span>
         </td>
      </tr>
      <tr>
         <td colspan="2" id="update_password_msg" align="center">
            Please enter your current and new password below. <br>
            <b>DO NOT FORGET YOUR NEW PASSWORD!</b><br>Forgetting your password will result in an inability to access your log data.
         </td>
      </tr>
      <tr>
         <td align="right">Current Password:</td>
         <td><input id="current_password" type="password" class="full_length"></td>
      </tr>
      <tr>
         <td align="right">New Password:</td>
         <td><input id="new_password" type="password" class="full_length"></td>
      </tr>
         <td align="right">Retype New Password:
         <td><input id="new_password_confirmation" type="password" class="full_length"></td>
      </tr>
   </table>
</div>


<div class="log_locker_update_dialog" id="autounlock_update_dialog" style="display: none;">
   <table width="100%">
      <tr class="validate_error" style="display: none;">
         <td colspan="2" align="center">
           <span style="color: red;"></span>
         </td>
      </tr>
      <tr>
         <td colspan="2" id="update_autounlock_msg" align="center">
            To <span id="autounlock_action">enable</span> auto-unlock, you must enter your password. <br>
            <span id="autounlock_enable_message">
               When enabling auto-unlock <br><b>DO NOT FORGET YOUR PASSWORD!</b><br>Forgetting your password will result in an inability to access your log data if auto-unlock is disabled again.<br>
            </span>
            <br>
         </td>
      </tr>
      <tr>
         <td align="left" style="width: 125px;">Log Locker Password:</td>
         <td><input id="password" type="password" class="full_length"></td>
      </tr>
   </table>
</div>
