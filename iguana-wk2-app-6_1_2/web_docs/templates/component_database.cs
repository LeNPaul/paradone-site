<?cs # vim: set syntax=html :?>
<SCRIPT language="Javascript" Type="Text/Javascript">
<!--
function <?cs var:ComponentType ?>RefreshSource(Value, DBtype, DBdataSource, DBuserName, DBpassword)
{
   var DataSource = document.getElementById("<?cs var:ComponentType ?>DataSourceId");
   var UserName = document.getElementById("<?cs var:ComponentType ?>DbUserNameId");
   var Password = document.getElementById("<?cs var:ComponentType ?>DbPasswordId");
   if (Value != DBtype)
   {
      DataSource.value = "";
      UserName.value = "";
      Password.value = "";
   }
   else
   {
      DataSource.value = DBdataSource;
      UserName.value = DBuserName;
      Password.value = DBpassword;
   }
}
//-->


function <?cs var:ComponentType ?>updateDatasourcePreview()
{

   var Input = document.getElementById('<?cs var:ComponentType ?>DataSourceId');
   var Preview = document.getElementById('<?cs var:ComponentType ?>database_source_preview');
   var PreviewDiv = document.getElementById('<?cs var:ComponentType ?>database_source_preview_div');

   if (Input)
   {
      updatePath('<?cs var:ComponentType ?>DataSource', Input, Preview, PreviewDiv, true);
   }

}

function <?cs var:ComponentType ?>updateDatauserPreview()
{

   var Input = document.getElementById('<?cs var:ComponentType ?>DbUserNameId');
   var Preview = document.getElementById('<?cs var:ComponentType ?>database_user_preview');
   var PreviewDiv = document.getElementById('<?cs var:ComponentType ?>database_user_preview_div');
   
   if (Input)
   {	   
      updatePath('<?cs var:ComponentType ?>DataUser', Input, Preview, PreviewDiv, true);
   }

}

function <?cs var:ComponentType ?>onDbApiSelectChange(value)
{
   showDatabaseNote('<?cs var:js_escape(ComponentType) ?>', value); 
   <?cs var:ComponentType ?>RefreshSource(value, 
   '<?cs var:js_escape(ComponentDatabase.Type) ?>',
   '<?cs var:js_escape(ComponentDatabase.DataSource) ?>',
   '<?cs var:js_escape(ComponentDatabase.UserName) ?>',
   '<?cs var:js_escape(ComponentDatabase.Password) ?>');
}

</script>
<tr class="selected">
   <td class="left_column">Database API</td>
   <td class="inner" colspan="3">
      <table class="inner">
         <tr>
            <td class="inner_left">
               <?cs if:Channel.ReadOnlyMode ?>
                  <?cs each: database = DatabaseTypes ?>
                     <?cs if: database.Name == ComponentDatabase.Type || !default_db && database.IsSupported ?>
                        <?cs set: default_db = database.Name ?>
                     <?cs /if ?>
                  <?cs /each ?>
                  <?cs var:html_escape(ComponentDatabase.Type) ?>
                  <input type=hidden id="<?cs var:ComponentType ?>DBType" name="<?cs var:ComponentType ?>DatabaseApi">
               <?cs else ?>
                  <select id="<?cs var:ComponentType ?>DBType" name="<?cs var:ComponentType ?>DatabaseApi" onchange="<?cs var:ComponentType ?>onDbApiSelectChange(value)">   
                     <?cs each: database = DatabaseTypes ?>
                        <?cs if: database.Name == ComponentDatabase.Type || !default_db && database.IsSupported ?>
                           <?cs set: default_db = database.Name ?>
                        <?cs /if ?>
                     <?cs /each ?>
                     <?cs each: database = DatabaseTypes ?>
                        <?cs if: database.Name == default_db ?>
                           <option selected value="<?cs var:html_escape(database.Name) ?>">
                        <?cs else ?>
                           <option value="<?cs var:html_escape(database.Name) ?>">
                        <?cs /if ?>
                        <?cs var:html_escape(database.Name) ?>
                        <?cs if: !database.IsSupported ?>
                           -- Not Available
                        <?cs /if ?>
                        </option>
                     <?cs /each ?>
                  </select> 
                  <a id="<?cs var:ComponentType ?>DBType_Icon" class="helpIcon" tabindex="100" rel="<div id='<?cs var:ComponentType ?>DBTypeHelp'></div>To view or change Iguana's database settings, visit <a href='/settings#Page=database_settings/view'>Settings&nbsp;&gt;&nbsp;Databases</a>." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a> 
               <?cs /if ?>
               <?cs if:ComponentError.DatabaseApi ?>
                  <div class="configuration_error">
                     The selected database is not available.
                  </div>
               <?cs /if ?>
            </td>
            <td class="inner_right">
               <?cs each: database = DatabaseTypes ?>
                  <?cs if: database.Name == 'OCI - Oracle' ?>
                     <?cs set: oci_ready = database.IsSupported ?>
                  <?cs elif: database.Name == 'ODBC - PostgreSQL' ?>
                     <?cs set: odbc_ready = database.IsSupported ?>
                  <?cs /if ?>
               <?cs /each ?>
               <div id="<?cs var:ComponentType ?>_mysql_details" style="display:none">
                  <?cs if: MySql.LoadedDll.Path ?>               
                        Iguana is configured to use MySQL
                        v<?cs var:html_escape(MySql.LoadedDll.Version) ?>.<br><br>
                  <?cs else ?>               
                        MySQL is not available.  If you have MySQL installed, you can
                        enable support for it via
                        <a href="/settings#Page=database_settings/edit/mysql">Settings&nbsp;&gt;&nbsp;Databases&nbsp;&gt;&nbsp;MySQL</a>.<br><br>               
                  <?cs /if ?>
               </div>
               <div id="<?cs var:ComponentType ?>_oci_details" style="display:none">
                  <?cs if: !oci_ready ?>               
                        Oracle OCI is not available.<br><br>               
                  <?cs /if ?>
               </div>
               <div id="<?cs var:ComponentType ?>_odbc_details" style="display:none">
                  <?cs if: !odbc_ready ?>               
                        ODBC is not available.<br><br>               
                  <?cs /if ?>
               </div>
      
            </td>
         </tr>
      </table>
   </td>
</tr>

<tr class="selected">
   <td class="left_column">Data source<font color="#ff0000">*</font></td>
   <td class="inner" colspan="3">
      <?cs if:Channel.ReadOnlyMode ?>
         <?cs if:ComponentDatabase.DataSource != "" ?>
            <?cs var:html_escape(ComponentDatabase.DataSource) ?>
            <?cs if:ComponentDatabase.DataSource != environment_expand(ComponentDatabase.DataSource) ?>
               <div id="<?cs var:ComponentType ?>DatabaseSource_preview_static" class="path_preview"> Preview: "<?cs var:html_escape(environment_expand(ComponentDatabase.DataSource)) ?>"</div>
            <?cs /if ?>
         <?cs /if ?>
         <?cs if:ComponentError.DataSource ?>
            <div class="configuration_error">
             <?cs var:ComponentError.DataSource ?>
            </div>
         <?cs /if ?>
      <?cs else ?>
         <input type="text" id="<?cs var:ComponentType ?>DataSourceId" class="configuration" name="<?cs var:ComponentType ?>DataSource" value="<?cs var:html_escape(ComponentDatabase.DataSource) ?>" onchange="<?cs var:ComponentType ?>updateDatasourcePreview();" onkeyup="<?cs var:ComponentType ?>updateDatasourcePreview();"/> <a id="<?cs var:ComponentType ?>DataSourceId_Icon" class="helpIcon" tabindex="101" rel="<div id='<?cs var:ComponentType ?>DataSourceIdHelp'></div>To view or change Iguana's database settings, visit <a href='/settings#Page=database_settings/view'>Settings&nbsp;&gt;&nbsp;Databases</a>." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a> 
         <div id="<?cs var:ComponentType ?>database_source_preview_div" style="display:none;">
	         <div id="<?cs var:ComponentType ?>database_source_preview" class="path_preview" val="<?cs var:html_escape(environment_expand(ComponentDatabase.DataSource)) ?>"></div>
         </div>
	    <?cs /if ?>            
      <div id="<?cs var:ComponentType ?>_mysql_database" style="display:none">                 
         Name of the database schema if locally hosted.<br>
         <br>
         For remote databases, use the form 'schema@host' or 'schema@host:port', where 'host' is the IP address or hostname of the server.
      </div>
      <div id="<?cs var:ComponentType ?>_sqlite_database" style="display:none">                 
         The database file name.
      </div>
      <div id="<?cs var:ComponentType ?>_oci_oracle_database" style="display:none">         
         Oracle service name (e.g., name in tnsnames.ora file)<br>
         Oracle connection string<br>
         <br>
          Oracle Instant Client connection string <br />
          (e.g., '&lt;host&gt;:&lt;port&gt;/&lt;servicename&gt;')
      </div>
      <div id="<?cs var:ComponentType ?>_odbc_database" style="display:none">                  
            Name of the ODBC system data source (System DSN)                  
      </div>
   </td>
</tr>
         
<tr class="selected">
   <td class="left_column">Database username</td>
   <td class="inner" colspan="3">
      <table class="inner">
         <tr>
            <td class="inner_left">
               <?cs if:Channel.ReadOnlyMode ?>
                  <?cs if:ComponentDatabase.UserName != "" ?>
                     <?cs var:html_escape(ComponentDatabase.UserName) ?>
                     <?cs if:ComponentDatabase.UserName != environment_expand(ComponentDatabase.UserName) ?>
                        <div id="<?cs var:ComponentType ?>DatabaseUser_preview_static" class="path_preview"> Preview: "<?cs var:html_escape(environment_expand(ComponentDatabase.UserName)) ?>"</div>
                     <?cs /if ?>
                  <?cs /if ?>
               <?cs else ?>
                  <input type="text" id="<?cs var:ComponentType ?>DbUserNameId" class="configuration" name="<?cs var:ComponentType ?>DatabaseUserName" value="<?cs var:html_escape(ComponentDatabase.UserName) ?>" onchange="<?cs var:ComponentType ?>updateDatauserPreview();" onkeyup="<?cs var:ComponentType ?>updateDatauserPreview();"/> <a id="<?cs var:ComponentType ?>DbUserNameId_Icon" class="helpIcon" tabindex="102" rel="Required if data source uses username authentication." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a>
                  <div id="<?cs var:ComponentType ?>database_user_preview_div" style="display:none;">
                     <div id="<?cs var:ComponentType ?>database_user_preview" class="path_preview" val="<?cs var:html_escape(environment_expand(ComponentDatabase.UserName)) ?>"></div>
                  </div>
	            <?cs /if ?>
            </td>
         </tr>
      </table>
   </td>
</tr>

<tr class="selected">
   <td class="left_column">Database password</td>
   <td class="inner" colspan="3">
      <table class="inner">
         <tr>
            <td class="inner_left">
               <?cs if:Channel.ReadOnlyMode ?>
                  ******
               <?cs else ?>
                  <input type="text" id="<?cs var:ComponentType ?>DbPasswordId" type="password" class="configuration" name="<?cs var:ComponentType ?>DatabasePassword" value="<?cs var:html_escape(ComponentDatabase.Password) ?>" />  <a id="<?cs var:ComponentType ?>DbPasswordId_Icon" class="helpIcon" tabindex="103" rel="Required if data source uses<br>password authentication." title="More Information" href="#"><img src="<?cs var:skin("/images/help_icon.gif") ?>" border="0" /></a> 
               <?cs /if ?>
               <?cs if:ComponentError.Password ?>
                  <div class="configuration_error">
                     <?cs var:ComponentError.Password ?>
                  </div>
               <?cs /if ?>
            </td>
         </tr>
      </table>
   </td>
</tr>
