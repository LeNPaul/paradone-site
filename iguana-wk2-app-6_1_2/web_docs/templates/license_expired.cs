<?cs include:"doctype.cs" ?>

<html>
<head>
   <title>Iguana License Required<?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?></title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("tooltip.js") ?>"></script>
   <link rel="stylesheet" type="text/css" href="jquery-ui/css/south-street/jquery-ui-1.7.2.custom.css" />
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/mapper/source_control.css") ?>" />
   <!-- Do not add JavaScript files here; add them to jsz/makefile instead. -->
   <?cs include:"license_js_files.cs" ?>
   <script type="text/javascript">
      $(document).ready(function() {
          MiniLogin.init('<?cs var:js_escape(CurrentUser) ?>',
                         '<?cs var:js_escape(DefaultPassword) ?>');
      });
   </script>   
</head>
<body class="tabright">

<?cs set:Navigation.CurrentTab = "Settings" ?>
<?cs include:"header.cs" ?>


<div id="main">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            
         </td>
      </tr>

      <tr>
         <td id="dashboard_body">
            <center>
    
               <?cs if:LicenseResult != 6 ?>               		
    
               <h3><?cs var:html_escape(ErrorMessage) ?></h3>	   
    
               <div style="text-align: center; margin-bottom: 40px;">
                   <h1>Activate Iguana</h1>
                   <h2>Iguana ID: <?cs var:IguanaId ?></h2>
                   <a href="edit_license_settings.html" class="action-button animate green" style="font-size: 16px;">Enter your license code</a>
               </div>

               <table class="configuration">
                   <tr>
                   <th colspan="2">Need a License?</th>
                   </tr>
                   <tr>
                   <td><h2>NEW USERS:</h2> Register to activate a free trial license.</td>
                   <td><a id="NewUser" href="" class="action-button animate blue"><span>Start a FREE Trial</span></a></td>
                   </tr>
                   <tr>
                   <td><h2>EXISTING CUSTOMERS:</h2> Log in to your iNTERFACEWARE Members Account to retrieve a license.</td>
                   <td><a id="GetLicense" href="" class="action-button animate blue">Retrieve License</a></td>
                   </tr>
               <table>
               <br/>

               <?cs /if ?>
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
            <tr>
         <td id="side_body">
            <h4 class="side_title">Overview</h4>
	     <p>
	 This page is displayed if you do not yet have a license for your copy of Iguana, or if this version of Iguana is unsupported by your OS.
	 </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="http://wiki.interfaceware.com/872.html?v=6.0.0" target="_blank">Obtaining License Codes</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
   </div>
</body>
</html>

<script type="text/javascript">
    $(document).ready(function(){
        var IguanaURL = [location.protocol, '//', location.host, '/'].join('');
        var RegistrationParams = "?product=iguana&version=<?cs var:VersionVer ?>&instanceid=<?cs var:IguanaId ?>&productlocation=" +
                encodeURIComponent(IguanaURL);
        var a = document.getElementById('GetLicense');
		if (a) {
			a.href = "http://my-iguana.interfaceware.com/getlicense" + RegistrationParams;
		}	
		a = document.getElementById('NewUser');
		if (a) {
			a.href = "http://my-iguana.interfaceware.com/newuser" + RegistrationParams;
		}
    });
</script> 
