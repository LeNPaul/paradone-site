<?cs # vim: set syntax=html :?>

<div id="header">

<script type="text/javascript" src="<?cs var:iguana_version_js("/skinningwindow.js") ?>"></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>


<header>
   <div>
     <a href="/dashboard.html"><img src="<?cs var:skin("/images/iguana_logo.png") ?>" class="iguana_logo"/></a>
     <span class="version"><?cs include:"version.cs" ?></span>
   </div>
   <nav>
      <span class="pages">
         <a href="/dashboard.html">Dashboard</a>
         <a href="/logs.html">Logs</a>
         <a href="/settings">Settings</a>
         <?cs if:CurrentUserCanAdmin ?>
            <a href="/settings#Page=settings/history">History</a>
         <?cs /if ?>
         <a href="<?cs var:help_link('iguana') ?>" target="_manual">Help</a>
      </span>
      <span class="user">Welcome, &nbsp;<?cs var:CurrentUser ?></span>
         
      <ul id="menu">
          <li>
              <a href="#">&blacktriangledown;</a>
              <ul>
                  <li><a href='/settings#Page=users/edit?user=<?cs var:js_escape(CurrentUser) ?>'><img src="/images/icon-account.png"> My Account</a></li>
                  <li><a href='<?cs var:LogoutLink ?>'><img src="/images/icon-logout.png"> Log Out</a></li>
              </ul>
          </li>       
      </ul>  
           
   </nav>
</header>

       
         

<div class="breadcrumb">
</div>

</div>
