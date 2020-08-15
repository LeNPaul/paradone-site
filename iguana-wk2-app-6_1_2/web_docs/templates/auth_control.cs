<?cs include:"doctype.cs" ?>

<html class="register-mini-login-click-handler">  <?cs # vim: set syntax=html :?>
<head>
   <title>Iguana <?cs if:AuthUriLabel ?>(<?cs var:html_escape(AuthUriLabel) ?>) <?cs /if ?>&gt; Settings &gt; Authentication</title>
   <?cs include:"browser_compatibility.cs" ?>

   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css") ?>">

   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("transition.css") ?>" />

   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-1.11.2.min.js"></script>
   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.min.js"></script>

   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/tooltip.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("access_control.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("class.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("validation.js") ?>"></script>

   
   <?cs include:"mini-login.cs" ?>

   <script>
   $(document).click(function checkStatus() {
      MiniLogin.loggedIn();
   });

   $(function() {
      $("form#auth_form").on("submit", function() {
         var Ret = VALvalidateFields();
         return Ret;
      });
   });
   </script>

   <style>
   #AuthUrlRow div {
      width: 100%;
      text-align: left;
      float: left;
      padding-bottom: 20px;
   }

   #AuthUrlRow input[type=text] {
      width: 450px;
   }

   #AuthUrlRow input {
      margin-left: 10px;
   }

   #AuthUrlRow {
      padding: 20px;
   }

   #AuthUrlErrorContainer {
      display: inline-block;
      margin: 7px 0 2px 180px;
   }
   </style>
</head>

<body class="tabright">

<?cs set:Navigation.CurrentTab = "Settings" ?>
<?cs include:"header.cs" ?>

<div id="main">
<div id="iguana">
   <div id="cookie_crumb">
      <a href="/settings">Settings</a> &gt; Authentication
   </div>
   <div id="dashboard_body">
      <center>
      <form id="auth_form" name="auth_form" method="post" action="/auth_control.html">
         <input type="hidden" name="save" value="true" />

         <div id="result" class="<?cs if:ErrorMessage ?>error<?cs elif:StatusMessage ?>success<?cs else?>indeterminate<?cs /if ?>">
            <div class="result_buttons_system">
               <a id="result_close" ><img src="images\close_button.gif"/></a>
            </div>

            <div id="result_title" class="<?cs if:ErrorMessage ?>error<?cs elif:StatusMessage ?>success<?cs /if ?>">
               <?cs if:ErrorMessage ?>
                  <span>Error</span>
                  <?cs set:Content=ErrorMessage ?>
               <?cs else ?>
                  <span>Success</span>
                  <?cs set:Content=StatusMessage ?>
               <?cs /if ?>
            </div>

            <div id="result_content">
               <?cs var:html_escape(Content) ?>
            </div>
         </div>

         <div class="access_control">
            <div class="h1">
               <div class="h1_link" >
                  <span class="h1">Authentication Settings</span>
               </div>
            </div>
            <div id="result" style="display:none"></div>

            <?cs if:CurrentUserCanAdmin ?>

            <div id="AuthUrlRow">
               <div>
                  <label class="left_column">Use External Authentication
                     <input type="checkbox" name="use_ext_auth" id="use_ext_auth"
                        <?cs if:use_ext_auth ?> checked="checked" <?cs /if ?>/>
                  </label>
               </div>

               <div>
                  <label class="left_column">External Authentication URL
                     <input name="auth_url" id="test.AuthUriLabel" type="text"
                        value="<?cs var:html_escape(auth_url) ?>"/>
                     <br/>
                     <span id="AuthUrlErrorContainer" class="validation_error_message_container">
                  </label>
               </div>
            </div>

            <script defer type="text/javascript">
               VALregisterTextUrlValidationFunction("test.AuthUriLabel", "AuthUrlRow", "AuthUrlErrorContainer", null, null);
            </script>

            <div id="buttons">
               <input type="submit" class="action-button blue" value="Save"/>
            </div>

            <?cs else ?>
               <p>You must have administrator permissions to view external authentication settings.</p>
            <?cs /if ?>

         </div>
      </form>
      </center>
   </div> <!-- End #dashboard_body -->
</div> <!-- End #iguana -->
</div> <!-- End #main -->


<div id="side_panel">
   <div id="side_table">
      <div id="side_header">
         <span>Page Help</span>
      </div>
      <div id="side_body">
         <h4 class="side_title">Overview</h4>

         <?cs if:CurrentUserCanAdmin ?>
            <p>User logins are authenticated using one of two methods. The default is to
            use standard Iguana User &amp; Role authentication. Alternatively, an external
            authentication method can be chosen here by supplying Iguana with a URL to an
            external authentication mechanism.</p>
         <?cs else ?>
            <p>From this page, you can edit the authentication settings for Iguana.
            <p>You must have administrator permissions to change the authentication settings.
         <?cs /if ?>

         <h4 class="side_title">Related Settings</h4>

         <a href="/settings#Page=email_status">Email Notification</a><br/>
         <a href="/settings#Page=channel/group">Channel Groups</a>
      </div>

      <div class="side_item">
         <h4 class="side_title">Help Links</h4>
         <ul class="help_link_icon">
            <li>
               <a href="<?cs var:help_link('iguana4_settings') ?>" target="_blank">Authentication</a>
            </li>
         </ul>
      </div>
    </div>
</div>
</body>
</html>
