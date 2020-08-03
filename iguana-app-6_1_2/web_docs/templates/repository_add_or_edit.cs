<?cs # vim: set syntax=html :?>
<?cs include:"browse_macro.cs" ?>

<link rel="stylesheet" type="text/css" href="<?cs var:iguana_version_js("/transition.css") ?>" />
<script type="text/javascript" src="<?cs var:iguana_version_js("/js/help_popup/help_popup.js") ?>"></script>

<div id="iguana">
   <div id="cookie_crumb">
      <a href="/settings">Settings</a> &gt; <a href="/settings#Page=repositories">Repositories</a> &gt; <?cs var:action_to_upper ?>
   </div>
   <div id="dashboard_body" style="text-align: center;">

      <div id="result" class="<?cs if:error_message ?>error<?cs elif:status_message ?>success<?cs else?>indeterminate<?cs /if ?>">

         <div class="result_buttons_system">
            <a id="result_close" ><img src="images/close_button.gif"/></a>
         </div>

         <div id="result_title" class="<?cs if:error_message ?>error<?cs elif:status_message ?>success<?cs /if ?>">

         <?cs if:error_message ?>
            <span class="error_message">Error</span>
            <?cs set:Content=error_message ?>
         <?cs else ?>
            <span class="status_message">Success</span>
            <?cs set:Content=status_message ?>
         <?cs /if ?>

         </div>

         <div id="result_content">
            <?cs var:html_escape(Content) ?>
         </div>

      </div>

      <form class="settings" name="add_or_edit_repo" id="add_or_edit_repo" method="post">

         <h1><?cs if:repo_name ?>Edit<?cs else ?>Add New<?cs /if ?> Repository</h1>

         <div>
            <label>Name</label>
            <div>
               <input value="<?cs var:repo_name ?>" type="text" class="repo_name" name="repo_name" id="repo_name">
               <input type="hidden" value="<?cs var:original_repo_name?>" name="original_repo_name">
               <?cs if:name_error ?>
                  <div class="repo_error">
                     <span><?cs var:name_error_msg ?></span>
                  </div>
               <?cs /if ?>
            </div>
         </div>

         <div>
            <label>Protocol</label>
            <div id="repo_protocol_radios">
               <label><span>Local</span>
                  <input value="local" type="radio" class="repo_protocol" name="repo_protocol" id="repo_protocol_local"
                     <?cs if:repo_protocol == "local" ?>checked<?cs /if ?>
                  >
               </label>

               <label><span>HTTP</span>
                  <input value="http" type="radio" class="repo_protocol" name="repo_protocol" id="repo_protocol_http"
                     <?cs if:repo_protocol == "http" ?>checked<?cs /if ?>
                  >
               </label>

               <label><span>HTTPS</span>
                  <input value="https" type="radio" class="repo_protocol" name="repo_protocol" id="repo_protocol_https"
                     <?cs if:repo_protocol == "https" ?>checked<?cs /if ?>
                  >
               </label>

               <label><span>SSH</span>
                  <input value="ssh" type="radio" class="repo_protocol" name="repo_protocol" id="repo_protocol_ssh"
                     <?cs if:repo_protocol == "ssh" ?>checked<?cs /if ?>
                  >
               </label>
            </div>
            <input type="hidden" value="<?cs var:original_repo_protocol?>" name="original_repo_protocol">
         </div>

         <!-- These three .protocol_input are showed and hidden based on which radio is selected. -->
         <div class="protocol_input" id="local_path_input">
            <label>Path</label>
            <div>
               <?cs call:browse_input_folder('local_path', repo_path) ?>
            </div>
            <?cs if:path_error ?>
               <div class="repo_error">
                  <span><?cs var:path_error_msg ?></span>
               </div>
            <?cs /if ?>
         </div>

         <div class="protocol_input" id="http_path_input">
            <label>HTTP URL</label>
            <div>
               <input value="<?cs var:repo_path ?>" type="text" name="http_path" id="http_path">
            </div>
         </div>

         <div class="protocol_input" id="https_path_input">
            <label>HTTPS URL</label>
            <div>
               <input value="<?cs var:repo_path ?>" type="text" name="https_path" id="https_path">
            </div>
         </div>

         <div class="protocol_input" id="ssh_path_input">
            <label>SSH URL</label>
            <div>
               <input value="<?cs var:repo_path ?>" type="text" name="ssh_path" id="ssh_path">
            </div>

            <label style="margin-top: 12px;">SSH Username (optional)</label>
            <div style="margin-top: 12px;">
               <input value="<?cs var:ssh_username ?>" type="text" name="ssh_username" id="ssh_username">
               <a id="TextQuery_Icon" style="position: inherit" class="helpIcon" tabindex="100" title="More Information" target="_blank" href="#" onclick="return false;" rel="
                  <p>This is only applicable for some Git services, where the username cannot be pulled out of the SSH URL.</p>
                  <p>An example of this would be communicating with a Git repo hosted in an on-premise installation of Microsoft Team Foundation Server</p>
                  <p>Most cloud based services do not require it (i.e. Github, Bitbucket). If not provided here and one is actually required, you will be prompted for it when it is needed.</p>">
                     <img src="/images/help_icon.gif?ver_hash=B6D0EB2A" border="0" />
               </a>
            </div>
            
            <label style="margin-top: 12px;">Private Key</label>
            <div style="margin-top: 12px;">
               <?cs call:browse_input('repo_key_path', repo_key_path) ?>
               <?cs if:repo_key_error ?>
                  <div class="repo_error">
                     <span><?cs var:repo_key_error_msg ?></span>
                  </div>
               <?cs /if ?>
            </div>
            
         </div>

         <div>
            <div>
               <input type="hidden" name="action" value="<?cs var:action ?>"/>
               <a class="action-button blue repo_action_button" href id="submit">Save Repository</a>
            </div>
         </div>

      </form>  

   </div>
   <div id="helpTooltipDiv" class="helpTooltip">
      <b id="helpTooltipTitle"></b>
      <em id="helpTooltipBody"></em>  
      <input type="hidden" name="helpTooltipId" id="helpTooltipId" value="0">
   </div>
</div>

<div id="side_panel">
   <div id="side_table">
      <div id="side_header">
         <span>Page Help</span>
      </div>
      <div id="side_body">
         <h4 class="side_title">Overview</h4>
         <p>On this page, you can configure Iguana to use local or remote Git repositories, for importing and exporting channels.</p>
         <p>URLs and paths should use one of these formats:</p>
         <dl>
            <dt><strong>Windows Path</strong></dt>
            <dd>C:\path\to\repo</dd>
            <dt><strong>Posix Path</strong></dt>
            <dd>/path/to/repo</dd>
            <dt><strong>HTTPS URL</strong></dt>
            <dd>https://path/to/repo</dd>
            <dt><strong>SSH Path</strong></dt>
            <dd>ssh://git@domain.tld/repo.git</dd>
         </dl>
         <h4 class="side_title">Related Functions</h4>
         <p><a href="/settings#Page=import">Import Channels</a></p>
         <p><a href="/settings#Page=export">Export Channels</a></p>
      </div>
      <div class="side_item">
         <h4 class="side_title">Help Links</h4>
         <p><a href="http://help.interfaceware.com/v6/addconfigure-repositories?v=6.0.0">Add and Configure Repositories</a></p>
         <p><a href="http://help.interfaceware.com/category/building-interfaces/repositories?v=6.0.0">Comprehensive Guide to Repositories</a></p>
      </div>
   </div>
</div>

<script type="text/javascript">
$(document).ready(function() {
   ifware.Settings.Repositories.setupAddOrEditPage();
   HLPpopUpinitialize();
});
</script>

