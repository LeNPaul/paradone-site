<?cs # vim: set syntax=html :?>
<?cs include: "mapper_config.cs" ?>

<?cs with: source = Channel.Source ?>
   <tr class="selected" id="SrcPollTimeRow">
      <td class="left_column">
         Use translator
      </td>
      <td class="inner_left" colspan="3">
         <script type="text/javascript">
           $(function(){
              $('.cls_from_http_mapper').toggle(<?cs if:source.UseMapper ?>true<?cs else ?>false<?cs /if ?>);
           })
           function onHttpTranslatorClick(el){
              $('.cls_from_http_mapper').toggle(el.value == 'mapper');
           } 
         </script>

         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode || Channel.IsEncrypted ?>
                     <?cs if:source.UseMapper ?>Yes<?cs else ?>No<?cs /if ?>
                     <?cs # also generate hidden form elements to pass on commit info when the translator is encrypted ?>
                     <?cs if:!Channel.ReadOnlyMode ?>
                        <input type="hidden" name="SrcFromHttpType" value="mapper" />
                     <?cs /if ?>
                  <?cs else ?>
                  <nobr>
                    <input type="radio" name="SrcFromHttpType" value="mapper" onclick="javascript:onHttpTranslatorClick(this);"
                       <?cs if:source.UseMapper ?>checked="checked"<?cs /if ?> /> Translator<br />
                    <input type="radio" name="SrcFromHttpType" value="legacy" onclick="javascript:onHttpTranslatorClick(this);"
                       <?cs if:!source.UseMapper ?>checked="checked"<?cs /if ?> /> Legacy
                  </nobr>
                  <?cs /if ?>
               </td>
               <td class="inner_right">
                  <?cs if:Channel.Source.Error.FromHttpType ?>
                     <div class="configuration_error">
                     <ul class="configuration" id="source_error_from_http"><?cs var:Channel.Source.Error.FromHttpType ?></ul>
                     <script type="text/javascript">
                       <!--
                       $(function(){
                          $('#source_error_from_http')
                             .each(function(){
                                //Do replacement here because of escaping issues in backend
                                var Token = 'Settings-&gt;HTTPS Channel Settings.'
                                this.innerHTML = this.innerHTML.replace(Token,'<a href="/https_channel_settings.html">'+Token+'</a>')
                             })
                       })
                       -->
                     </script>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>
         </table>
      </td>
   </tr>

   <tr class="selected cls_from_http_mapper">
      <td class="left_column first_row">URL path</td>
      <td class="inner_left first_row" colspan="3">
         <table class="inner">
            <tr>
               <td class="inner_left">
                  <?cs if:Channel.ReadOnlyMode ?>
                    
                    <?cs if:source.UsingHttpChannelServer ?><a id="MapperUrlPathHref" target="_blank" href="">
                     <script type="text/javascript">
                       $(function(){
                          var HttpPort = '<?cs var:js_escape(source.HttpChannelServerPort) ?>'
                          var HttpSslMode = <?cs if:source.HttpChannelServerSslMode ?>true<?cs else ?>false<?cs /if ?>;
                          $('#MapperUrlPathHref')
                             .each(function(){
                                var HrefVal = (HttpSslMode ? 'https://' : 'http://') + window.location.hostname + ':' + HttpPort + '/<?cs var:js_escape(source.MapperUrlPath) ?>'
                                this.href = HrefVal;
                                this.innerHTML = HrefVal;
                             })
                       })
                     </script>
                    <?cs /if ?>
                      /<?cs var:html_escape(source.MapperUrlPath) ?>
                    <?cs if:source.UsingHttpChannelServer ?></a><?cs /if ?>
                  <?cs else ?>
                    <nobr>
                    /&nbsp;<input type="text" class="configuration" name="SrcMapperUrlPath"
                           value="<?cs var:source.MapperUrlPath ?>" />
                    </nobr>
                  <?cs /if ?>
               </td>
               <td class="inner_right">
                  <?cs if:Channel.Source.Error.MapperUrlPath ?>
                     <div class="configuration_error">
                     <ul class="configuration"><?cs var:Channel.Source.Error.MapperUrlPath ?></ul>
                     </div>
                  <?cs /if ?>
               </td>
            </tr>
         </table>
      </td>
   </tr>   
   <tr id="ThreadCountRow" class="selected cls_from_http_mapper">
      <td class="left_column first_row">Thread count</td>
      <td class="inner_left first_row" colspan="3">

                  <?cs if:Channel.ReadOnlyMode ?>
                      <?cs var:html_escape(source.InstanceCount) ?>
                  <?cs else ?>
                    <input type="text" class="number_field" name="SrcInstanceCount" id="SrcInstanceCount" value="<?cs var:source.InstanceCount ?>" />
          <a id="SrcInstanceCount_Icon" class="helpIcon" tabindex="100" rel="This is the number of independent Translator threads that will handle requests.<br><br>Note: additional threads are counted as channels for license purposes.<br><br>For more information on From HTTPS components, please see <a href='http://wiki.interfaceware.com/1096.html?v=6.0.0' target='_blank'>the wiki page</a>."
            title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
          <span id="ThreadCountErrorMessageContainer" class="validation_error_message_container">
            <script type="text/javascript">           
              VALregisterIntegerValidationFunction('SrcInstanceCount', 'ThreadCountRow', 'ThreadCountErrorMessageContainer', null, null, 1);
            </script>
          </span>         
                  <?cs /if ?>
     </td>
   
   </tr>   
   <?cs if:Channel.Source.Error.FromHttpType ?> 
      <?cs call:renderMapperForm(source, !Channel.IsNew, 'Src', 'Source', source.Type, 'cls_from_http_mapper', 0, 0) ?>
   <?cs else ?>
      <?cs call:renderMapperForm(source, !Channel.IsNew, 'Src', 'Source', source.Type, 'cls_from_http_mapper', 0, 1) ?>
   <?cs /if ?>
   </tr>
<?cs /with ?>
