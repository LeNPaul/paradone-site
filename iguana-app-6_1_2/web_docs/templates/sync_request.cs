<?cs include:"doctype.cs" ?>
<html>  <?cs # vim: set syntax=html :?>
<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Settings &gt; Synchronize Roles & Users</title>
   <?cs include:"browser_compatibility.cs" ?>

   <link rel="stylesheet" type="text/css" href="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css">

   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="transition.css" />

   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-1.11.2.min.js"></script>
   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.min.js"></script>

   <script type="text/javascript" src="<?cs var:iguana_version_js("jquery.watermark.min.js") ?>"></script>

   <script type="text/javascript" src="<?cs var:iguana_version_js("/tooltip.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/sync_request.js") ?>"></script>
   

   <?cs include:"mini-login.cs" ?>

   <script>
   $(document).click(function checkStatus() {
      MiniLogin.loggedIn();
   });
   </script>
</head>

<body class="tabright register-mini-login-click-handler">
<?cs set:Navigation.CurrentTab = "Settings" ?>
<?cs include:"header.cs" ?>

<div id="main">
   <div id="iguana">
      <div id="cookie_crumb">
         <a href="/settings">Settings</a> &gt; Synchronize Users &amp; Roles
      </div>
      <div id="dashboard_body">
         <center>
               <div class="access_control">
                  <div>
                     <div class="border_green"></div>
                     <div class="h1">
                        <div class="h1_link" >
                           <span class="h1">Synchronize Users &amp; Roles</span>
                        </div>
                     </div>
                     <div id="result" style="display:none"> </div>
                     <?cs if:LocalSync ?>
                        <p>
                        Please enter the current admin password to apply the given
                        synchronization request.  All existing users and roles will be
                        replaced.
                        <p>
                        Admin password: <input id="pass" type="password"><br>
                        <input id="query" type="hidden" value="<?cs var:html_escape(Query) ?>">
                        <div style="padding:5px">
                           <table id="buttons"><tr><td>
                              <a class="action-button blue" id="apply"><span>Apply</span></a>
                           </table>
                        </div>
                     <?cs else ?>
                        <p>Select the remote Iguana instances that you want to synchronize
                        with this Iguana.
                        <table class="configuration">
                           <tr class="header">
                           <th><input type="checkbox" id="all" checked="checked">
                           <th>Remote Iguana
                           <th>Admin Password
                           <th>Sync Status
                           <?cs each: target = Targets ?>
                              <tr><td style="text-align:center">
                              <input type="checkbox" id="Use-<?cs var:name(target) ?>"
                                 value="<?cs var:html_escape(target.Link) ?>"
                                 <?cs if:target.Selected ?>checked<?cs /if?>>
                              <td><label for="Use-<?cs var:name(target) ?>"
                                 ><?cs var:html_escape(target.Link) ?></label>
                              <td><input type="password" id="Pass-<?cs var:name(target) ?>">
                              <td>
                                 <div id="idle-<?cs var:name(target) ?>">
                                    <img src="../images/sync-idle.gif"></div>
                                 <div id="busy-<?cs var:name(target) ?>" style="display:none">
                                    <img src="../images/sync-busy.gif">&nbsp;Busy</div>
                                 <div id="okay-<?cs var:name(target) ?>" style="display:none">
                                    <img src="../images/sync-success.gif"
                                    >&nbsp;<a href="#" target="_blank">Undo</a></div>
                                 <div id="fail-<?cs var:name(target) ?>" style="display:none">
                                    <img src="../images/sync-failure.gif"
                                    >&nbsp;<a href="#">Details</a></div>
                           <?cs /each ?>
                        </table>
                        <div style="padding:5px">
                           To add more remote instances to this list, visit the
                           <a href="/settings#Page=remote_iguanas">Remote Iguanas</a> page.
                        </div>
                        <div style="padding:5px">
                           <table id="buttons"><tr><td>
                              <a class="action-button blue" id="sync"><span>Synchronize</span></a>
                           </table>
                        </div>
                        <div id="confirm" style="display:none">
                           <p>Warning: this action will replace all the users and roles on
                           the selected remote Iguanas, with those from this instance.
                           <p>Please enter the admin password to confirm this action:
                           <p>Local Admin Password: <input id="pass" type="password">
                        </div>
                     <?cs /if ?>
                  </div>
               </div>
         </center>
      </div>
   </div>
</div>

<div id="side_panel">
   <div id="side_table">
      <div id="side_header">
         Page Help
      </div>
      <div id="side_body">
         <h4 class="side_title">Overview</h4>
            <?cs if:LocalSync ?>
               <p>This page allows you to restore all users and roles from a previous
               state.  This action can be undone: a link will be logged, allowing you
               to undo this action.
            <?cs else ?>
               <p>Here you can replace all the users and roles on remote Iguana instances
               with those from this server.  Channel groups are not replaced.
               <p>When a Role is transferred to a remote instance, only those permissions
               assigned to channel groups common to both Iguanas are kept.
            <?cs /if ?>
         <h4 class="side_title">Related Settings</h4>
         <a href="/settings#Page=email_status">Email Notification</a><br/>
         <a href="/settings#Page=channel/group">Channel Groups</a>
      </div>
      <div class="side_item">
         <h4 class="side_title">Help Links</h4>
         <ul class="help_link_icon">
            <li><a href="<?cs var:help_link('iguana4_config_users_groups') ?>" target="_blank">Roles & Users</a>
         </ul>
      </div>
    </div>
</div>

</body>
</html>
