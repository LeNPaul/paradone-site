<?cs def:users_groups_navigation_tabs(activeTab, onTabClick) ?>
   <div class="navcontainer">
   
      <h2>&nbsp;</h2>
   
      <a id="settingsTab" href="access_control.html"
         onclick="<?cs var:onTabClick ?>"
         <?cs if:activeTab == "Users" ?>class="current"<?cs /if ?>>
         Users
      </a>
   
      <a id="settingsTab" href="/groups_view.html"
         onclick="<?cs var:onTabClick ?>"
         <?cs if:activeTab == "Groups" ?>class="current"<?cs /if ?>>
        Groups
      </a>

         
   </div>
<?cs /def ?>