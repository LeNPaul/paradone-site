<?cs def:email_navigation_tabs(activeTab, onTabClick) ?>
   <div class="navcontainer">
   
   	  <h2>Email Notification</h2>
   
      <a id="settingsTab" href="/settings#Page=email_status"
         onclick="<?cs var:onTabClick ?>"
         class="email_nav_tab<?cs if:activeTab == "Status" ?> current<?cs /if ?>">Status</a>
   
      <a id="settingsTab" href="/settings#Page=email_settings/view"
         onclick="<?cs var:onTabClick ?>"
         class="email_nav_tab<?cs if:activeTab == "Settings" ?> current<?cs /if ?>">Email Server Settings
      </a>
      
      <a id="rulesTab" href="/settings#Page=email_settings/rules/view"
         onclick="<?cs var:onTabClick ?>"
         class="email_nav_tab<?cs if:activeTab == "Rules" ?> current<?cs /if ?>">Notification Rules
      </a>
   </div>
<?cs /def ?>

<?cs def:email_not_setup_page_contents() ?>
   
      <?cs if:ActionResult ?>
      <p>
         <font color="green"><b><?cs var:ActionResult ?></b></font>
      </p>
      <?cs /if ?>
      <h2>Email notification settings have not been configured.</h2>
      <p style="width: 80%;">
         Iguana allows you to define a set of rules which specify recipient(s) of
         email messages for various events.
      </p>
      <p style="width: 80%;">
         <?cs if:CurrentUserCanAdmin ?>
            Click <b>"Configure Email Settings"</b>
            to configure email server settings now, or click <b>"Disable Email Notification"</b>
            to disable this feature (it can be enabled later).
         <?cs /if ?>
      </p>
      
      <br/>
   
      <table id="buttons"><tr>
         <td>
            <?cs if:CurrentUserCanAdmin ?>
               <a class="action-button green" href="/settings#Page=email_settings/view?setup_email=true">Configure Email Settings</a>
            <?cs else ?>
               <div class="button_disable" id="DisabledEditButton1" onMouseOver="TOOLtooltipLink('You do not have the necessary permissions to edit these settings.', null, this);" onMouseOut="TOOLtooltipClose();" onmouseup="TOOLtooltipClose();" ><span>Configure Email Settings</span></div>
            <?cs /if ?>
         </td>
         <td>
            <?cs if:CurrentUserCanAdmin ?>
               <a class="action-button grey" href="/settings#Page=email_status?Action=disable_email_notification">
                  Disable Email Notification
               </a>
            <?cs else ?>
               <div class="button_disable" id="DisabledEditButton2" onMouseOver="TOOLtooltipLink('You do not have the necessary permissions to edit these settings.', null, this);" onMouseOut="TOOLtooltipClose();" onmouseup="TOOLtooltipClose();" ><span>Disable Email Notification</span></div>
            <?cs /if ?>
         </td>
      </tr></table>
   
      
 
<?cs /def ?>

<?cs def:email_disabled_page_contents(activeTab) ?>
   <div class="tabs_and_contents">
   
      <?cs call:email_navigation_tabs(activeTab, "return true;") ?>

      <p>
         <font color="red"><b>Email notification is currently disabled.</b></font>
      </p>
      <p>
         No email messages will be sent while email notification is disabled.
      </p>
      
      <p /><p />
      
      
         <table id="buttons"><tr>
            <td>
               <?cs if:CurrentUserCanAdmin ?>
                  <a class="action-button blue" href="/settings#Page=email_status?Action=enable_email_notification">Enable Email Notification</a>
               <?cs /if ?>
            </td>
         </tr></table>
      
      
   </div>
<?cs /def ?>

<?cs def:email_settings_not_validated_paragraph() ?>
   <p>
      <span style="color: red; font-weight: bold;">Warning: email notification settings have not been validated.</span>
      <br />
      <?cs if:CurrentUserCanAdmin ?>
         To ensure that email notification is working properly, go to the <a href="/settings#Page=email_settings/validation">Validation</a>
         page to validate the current settings or to disable this warning.
      <?cs else ?>
         You must be logged in as an Administrator to validate email settings.
      <?cs /if ?>
   </p>
<?cs /def ?>

<?cs def:link_to_users_and_groups() ?>
   <h4 class="side_title">Related Settings</h4>
   <a href="/settings#Page=roles">Roles & Users</a>
<?cs /def ?>
