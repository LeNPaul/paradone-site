<?cs include:"doctype.cs" ?>

<html>

<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Settings &gt; License Entitlement &gt; Enter License Code</title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>

   <script type="text/javascript">
      function checkForNewLicense()
      {
         if (document.getElementById('license_update').NewLicenseHex.value == "")
         {
            alert("Please paste a license key into the specified area.");
            document.getElementById('license_update').NewLicenseHex.focus();
            return false;
         }
         else
         {
            return true;
         }
      }
   </script>
</head>

<body class="tabright">

<?cs set:Navigation.CurrentTab = "Settings" ?>
<?cs include:"header.cs" ?>


<div id="main">

   <table id="iguana">
      <tr>
         <td id="cookie_crumb">
            <a href="/settings">Settings</a> &gt; <a href="/license_settings.html">License Entitlement</a> &gt; Enter License Code
         </td>
      </tr>

      <tr>
         <td id="dashboard_body">
         
               <center>
               <?cs if:ErrorMessage ?>
                  <h3><font color="red"><span id="license_error"><?cs var:ErrorMessage ?></span><br>License has not been updated.</font></h3>
               <?cs /if ?>
               <p>
			Please copy and paste the license code you have received from iNTERFACEWARE into the area below and click <b>Update License</b>.
	       </p> 
               <form name="license_update" id="license_update" method="post" action="submit_license_settings.html" onSubmit="return checkForNewLicense();">
                  <table class="configuration">
                     <tr><th>License Code:</th></tr>
                     <tr>
                        <td><textarea rows="5" name="NewLicenseHex" wrap="soft"></textarea></td>
                     </tr>
                  </table>
<br>
<table id="buttons"><tr><td>
<input type="submit" name="update_license" class="hidden_submit" value="Update License">
<a class="action-button green" href="javascript:document.license_update.update_license.click();">Update License</a> 
</td></tr></table>
               </form>

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
	     Use this page to update your license code information.
	 </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <ul class="help_link_icon">
            	<li>
            	<a href="<?cs var:help_link('iguana4_config_license') ?>" target="_blank">License Entitlement</a>
            	</li>
            </ul>
         </td>
      </tr>
   </table>
   </div>
</body>
</html>

<script type="text/javascript">
  $(document).ready(function() {
    if ($("#license_error").length) {
      var ErrorMessage = $("#license_error").text();
      var MessageWithValidNewlines = ErrorMessage.replace(/\r\n/g, "<br>").replace(/\n/g, "<br>");
      $("#license_error").html(MessageWithValidNewlines);
    }
  });

</script>
