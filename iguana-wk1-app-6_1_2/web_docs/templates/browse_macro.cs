<?cs def:browse_readonly(path) ?>
   <?cs if:path != "" ?>
      <?cs var:html_escape(path) ?>
      <?cs if:path != path_expand(path) ?>
         <div class="path_preview"> Absolute Path: "<?cs var:html_escape(path_expand(path)) ?>"</div>
      <?cs /if ?> 
   <?cs /if ?>
<?cs /def ?>

<?cs def:browse_input(name,value) ?>
   <input type="text" class="file" size="43" name="<?cs var:name ?>" id="<?cs var:name ?>" value="<?cs var:html_escape(value) ?>"/>
   <a class="file_browse_button" href="javascript:ifware.Settings.FileBrowser.FILbrowse('', '<?cs var:name ?>');">
      <span>Browse</span>
   </a>
   <div id="<?cs var:name ?>_preview_div" style="display:none;clear:both;padding:0px;margin:0px;">
      <div id="<?cs var:name ?>_path_preview" class="path_preview"></div>               
   </div>

   <script type="text/javascript">
      setInterval("updatePath('<?cs var:name ?>', document.getElementsByName('<?cs var:name ?>')[0], document.getElementById('<?cs var:name ?>_path_preview'),  document.getElementById('<?cs var:name ?>_preview_div') );", 500);
   </script>
<?cs /def ?>

<?cs def:download_vmd(guid, editTab, channelname, vmdtype) ?>
   <a class="file_browse_button" href="javascript:ifware.Settings.Helpers.downloadVmd('<?cs var:guid ?>', document.getElementById('editTab').value, '<?cs var:channelname ?>', '<?cs var:vmdtype ?>');">
      <span>Download running VMD</span>
   </a>
<?cs /def ?>

<?cs def:keep_updated(name, ifUpdated) ?>
   <input type="checkbox" id="<?cs var:name ?>" name="<?cs var:name ?>" <?cs if:ifUpdated ?>checked<?cs /if ?>>
   <label for="<?cs var:name ?>"></label>
<?cs /def ?>

<?cs def:browse_input_folder(name,value) ?>
   <input type="text" class="file" size="43" name="<?cs var:name ?>" id="<?cs var:name ?>" value="<?cs var:html_escape(value) ?>" />
   <a class="file_browse_button" href="javascript:ifware.Settings.FileBrowser.FILfolderBrowse('', '<?cs var:name ?>', true);">
      <span>Browse</span>
   </a>
   <div id="<?cs var:name ?>_preview_div" style="display:none;clear:both;padding:0px;margin:0px;">
      <div id="<?cs var:name ?>_path_preview" class="path_preview"></div>
   </div>

   <script type="text/javascript">
      setInterval("updatePath('<?cs var:name ?>', document.getElementsByName('<?cs var:name ?>')[0], document.getElementById('<?cs var:name ?>_path_preview'),  document.getElementById('<?cs var:name ?>_preview_div') );", 500);
   </script>
<?cs /def ?>

