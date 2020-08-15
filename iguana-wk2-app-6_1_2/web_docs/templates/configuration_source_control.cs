<?cs include:"doctype.cs" ?>       
<html>  <?cs # vim: set syntax=html :?>
<head>
   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Dashboard &gt; Configuration Source Control</title>
   <?cs include:"browser_compatibility.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css") ?>">   
   <?cs include:"styles.cs" ?> 

<script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>" ></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>   

   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-1.11.2.min.js"></script>
   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.min.js"></script>


<script type="text/javascript" src="<?cs var:iguana_version_js("/js/cookie/cookiev4.js") ?>"></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("path_expand.js") ?>"></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("commit_popup.js") ?>"></script>          
<?cs include:"mini-login.cs" ?>
<script type="text/javascript">

var BrowserDetails = navigator.userAgent;   
var WrapChar = "";
if(BrowserDetails.indexOf("MSIE 6") != -1 || BrowserDetails.indexOf("MSIE 5") != -1)
{
   WrapChar = "<wbr />";
}
else
{
   WrapChar = "&#8203;";
} 

function showDiff(FileName, Extension, User, Date)
{
   window.open("configuration_control_diff.html?FileName=" + FileName + "." + Date + "." + User + "." + Extension);
}
</script>

   <style type="text/css">   
   
   body 
   {
      text-align:center;
      min-width:500px;
   }

   #commit_form
   {
      margin:0 auto;
      width:500px;
      text-align:left;
      color:#30302C;
      background-color:#E4E5E0;
      border: 1px solid #757671;
   }
  
   #files
   {
      background-color:#FFFFFF;
      border: 1px solid #BBBBBB;	  
   }
        
   div.label
   {
      font-size:12px;   
      font-weight:bolder;
      padding:2px;   
   }   
 
   div.content
   {
      padding:2px;   
   }   
      
   td
   {
      text-align:center;
      font-size:11px;   
      font-weight:550;
      color:#000000;  
      padding:5px;     
   }
   
   a.button
   {
      font-size:12px;
      text-align:center;
      width:100px;
      margin-left:172px;   
   }

   div#no_results 
   {
      color: gray;
      font-size: 10pt;
      font-weight: bold;
      font-variant: small-caps;
   }

   </style>
</head>

<body class="tableft">

<?cs set:Navigation.CurrentTab = "configuration_control" ?>
<?cs include:"header.cs" ?>

<div id="main">

<form id="configuration_source_control" action="" method="post">
<table id="iguana">

   <tr> 
      <td id="cookie_crumb">
         Configuration Control
      </td>
   </tr>

   <tr>
      <td id="dashboard_body">

         <div id="commit" style="width:100%;">

            <?cs if:FileCount > 0 ?>

            <div id="commit_form">
               
               <div style="padding:10px;">

                  <div id="files_label" class="label">The following files have been modified:</div>
                  <div id="files_content" class="content">

                     <table id="files" style="width:100%;">
                        <tr>
                           <th style="text-align:left;">
                           Filename 
                           </th>
                           <th>
                           Changed By 
                           </th>
                           <th>
                           Date
                           </th>
                           <th colspan="2">
                           Action
                           </th>
                        </tr> 
                        <?cs each:item = File ?>
                        <?cs set:name=js_escape(item.Name) ?>
                        <?cs set:extension=js_escape(item.Extension) ?>
                        <?cs set:user=js_escape(item.ChangedBy) ?>
                        <?cs set:date=js_escape(item.Date) ?>
                        <tr>
                           <td style="text-align:left;">
                           <?cs var:name ?>.<?cs var:extension ?>
       			   </td>
                           <td>
                           <?cs var:user ?>
       			   </td>
                           <td>
                           <?cs var:date ?>
       			   </td>
                           <?cs if:IsConfigurationControlPrimed ?>
                           <td <?cs if:!IsConfigurationControlEnabled ?>colspan="2"<?cs /if ?> >
                           <a href="javascript:showDiff('<?cs var:name ?>', '<?cs var:user ?>', '<?cs var:date ?>', '<?cs var:extension ?>');" >Diff</a> 
                           </td>
                           <?cs /if ?>
                           <?cs if:IsConfigurationControlEnabled ?>
                           <td>
                           <a href="javascript:showCommitPopup('<?cs var:name ?>', '<?cs var:user ?>', '<?cs var:date ?>', '<?cs var:extension ?>');" >Commit</a> 
                           </td>
                           <?cs /if ?>
                        </tr>
                        <?cs /each ?>                         
                     </table>

                  </div>

               </div>

               <?cs else ?>

               <div style="text-align:center;padding:10px;">
                     
                  <div id="no_results" class="label">All your files are up-to-date.</div>
  
               </div>

               <?cs /if ?>
              
            </div>
	                   
	 </div>                      

         </div>

      </td>
        
   </tr>

</table>

</form>
		
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
         <div id="help_blurb">Configuration files are changed when Iguana settings are modified.  To enable system reversion in cases where system changes are to be discarded, configuration are backed up and shown on this page.<br><br>Persisted configuration file backups are uploaded to a repository and can be downloaded at a later date and reinstalled manually.<?cs if:FileCount > 0 ?><br><br>This pages shows all configuration file backups not yet persisted into the repository.  When a persisted configuration file exists in the repository, then for each backup,  a link is provided to view a comparison between the backup and the persisted configuration file.<br><br>Also, when configuration control is enabled, a link is provided to upload the backup into the repository.  Upon a successful upload, all older backups as well as the backup that was persisted will be deleted from the list.<?cs /if ?></div>    
         </td>
      </tr>

   </table>

</div>

</body>
</html>
