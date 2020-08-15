<?cs # vim: set syntax=html :?>

<?cs def:showError(Value) ?>
<div style="clear:both;">
 <?cs if:Value ?>
   <span style="color:red;" id="<?cs name:Value ?>"><?cs var:html_escape(Value) ?></span>
 <?cs else ?>
   <span id="<?cs name:Value ?>"></span>
 <?cs /if ?>
</div>
<?cs /def ?>

<table id="iguana">
   <tr>
      <td id="cookie_crumb">
         <a href="/settings">Settings</a><span> &gt; Import Channels</span>
      </td>
   </tr>

   <tr>
      <td id="dashboard_body">
      <?cs if:!CurrentUserCanAdmin ?>
         <p style="text-align: center">Only administrators can import channels.</p>
      <?cs else ?>
         <?cs if:StatusMessage ?>
            <h3><font color="green" id="StatusMessage"><?cs var:html_escape(StatusMessage) ?></font></h3>
         <?cs else ?>
            <span id="StatusMessage"></span>
         <?cs /if ?>

            <div id="RepoSelect">
               <label for="RepoSelector">Showing channels on: </label>
               <select name="RepoSelector" id="RepoSelector">
                  <option id="NullRepo" value="">Choose Repository...</option>
            <?cs if:CountOfRepositories ?>
                  <?cs each:Repo = Repositories ?>
                     <option value="<?cs var:js_escape(Repo.name) ?>"><?cs var:html_escape(Repo.name) ?></option>
                  <?cs /each ?>
            <?cs /if ?>
               </select>
            </div><!--/RepoSelect-->
            <?cs if:ErrorMessage ?>
               <h3><font color="red" id="ErrorMessage"><?cs var:html_escape(ErrorMessage) ?></font></h3>
            <?cs else ?>
               <span id="ErrorMessage"></span>
            <?cs /if ?>
            <div id="official_repo_description"></div>
            <div id="import_export_list"></div>
            <div id="SCMloadWheelContainer"><span id="SCMloadWheel" class="SCMloading"></span></div>
         <?cs /if ?>
      </td>
   </tr>
</table>

<div id="side_panel">
   <table id="side_table">
      <tr>
         <th id="side_header">Page Help</th>
      </tr>
      <tr>
         <td id="side_body">
            <h4 class="side_title">Overview</h4>
            <p>On this screen, you can install channels into Iguana from local or remote Git repositories.</p>
            <p>Choose a repository from the menu to see a list of available channels in that repository.</p>
            <h4 class="side_title">Related Functions</h4>
            <p><a href="/settings#Page=repositories">Add or edit repositories</a></p>
            <p><a href="/settings#Page=export">Export Channels</a></p>
            <p>
               If channels from multiple versions are found on the remote repo, they will be displayed separately.
               You can view different channel version groupings via the version radio selector.<br>
               You may only import channels from one channel version grouping at a time.
            </p>
         </td>
      </tr>
      <tr>
         <td class="side_item">
            <h4 class="side_title">Help Links</h4>
            <p><a href="http://help.interfaceware.com/v6/addconfigure-repositories?v=6.0.0">Add and Configure Repositories</a></p>
            <p><a href="http://help.interfaceware.com/category/building-interfaces/repositories?v=6.0.0">Comprehensive Guide to Repositories</a></p>
         </td>
      </tr>
   </table>
</div>

<div id="helpTooltipDiv" class="helpTooltip">
   <b id="helpTooltipTitle"></b>
   <em id="helpTooltipBody"></em>
   <input type="hidden" name="helpTooltipId" id="helpTooltipId" value="0"/>
</div>

<script type="text/javascript">

function ie9crossDomainAjax(url, successCallback, errorCallback) {
   var xdr = new XDomainRequest(); // Use Microsoft XDR

   xdr.open('get', url);

   xdr.onload = function () {
      var dom  = new ActiveXObject('Microsoft.XMLDOM'),
          JSON = $.parseJSON(xdr.responseText);

      dom.async = false;

      if (JSON == null || typeof (JSON) == 'undefined') {
          JSON = $.parseJSON(data.firstChild.textContent);
      }

      successCallback(JSON); // internal function
   };

   xdr.onerror = function() {
      errorCallback(JSON);
   };

   xdr.send();
}

$(document).ready(function() {
   var StarterRepoCount = <?cs var:CountOfRepositories ?>;
   $("#ServerLabel").on("change keyup", SettingsHelpers.updateServerName);
   var RepoMap = {};
   function updateRepoList(Response) {
      var Styles = {};
      var StyleString = "";
      var OptionString = "";
      for (var i = 0; i < Response.length; i++) {
         var RepoVar = Response[i];
         var Pieces = RepoVar.icon_url.split("/");
         var IconBaseName = Pieces[Pieces.length - 1].split(".")[0];
         Styles[IconBaseName] = '#RepoSelector-menu .ui-icon.' + IconBaseName + '{ background-image: url("' + RepoVar.icon_url + '"); }';
         StyleString += Styles[IconBaseName];
         RepoMap[RepoVar.name] = RepoVar;
         OptionString += '<option data-class="' + IconBaseName + '" value="' + RepoVar.name + '">' + RepoVar.display_name + '</option>';
      }
      if (StyleString != "") {
         var Style = '<style type="text/css">' + StyleString + '</style>';
         $("head").append(Style);
         $("#RepoSelector").append(OptionString);
         $("#RepoSelector").iconselectmenu("refresh");
      }
   }
   function handleAbsentRepos() {
      if (StarterRepoCount > 0) { return; }
      $("#official_repo_description").html('<p class="error" style="margin: 40px;">To import channels, you will need to <a href="/settings#Page=repositories">configure Iguana to use one or more repositories</a>.<br><br>Alternatively, if you run Iguana where it has a connection to the internet, you will be able to import official iNTERFACEWARE channels without making configuration changes.');
   }

   var url = "//repo.interfaceware.com/i6/official.json";

   if ('XDomainRequest' in window && window.XDomainRequest !== null) {
      ie9crossDomainAjax(url, updateRepoList, handleAbsentRepos);
   }
   else {
      $.ajax({
         url    : url,
         cache  : false,
         success: updateRepoList,
         error  : handleAbsentRepos
      });
   }

   function showChannels() {
      if (! document.getElementById("RepoSelector")) {
         return;
      }
      var Selector = $("#RepoSelector");
      var R = Selector.val();
      console.log("showChannels R: " + R);
      var Remote = $("option:selected", Selector).attr("data-class");
      console.log("Remote in showChannels: " + Remote); 
      console.log("RepoMap[R]: " + RepoMap[R]);
      console.log("Has own property " + RepoMap.hasOwnProperty(R));
      //If undefined then it is a local version
      if(Remote != undefined){
         if (RepoMap.hasOwnProperty(R) && RepoMap[R].hasOwnProperty("description")) {
            $("#official_repo_description").html(RepoMap[R].description);
         } else {
            $("#official_repo_description").html("");
         }
         ifware.ImportManager.show(R, RepoMap[R]);
      }
      else{
         console.log("This is a local repo");
         $("#official_repo_description").html(""); // Description is undefined
         ifware.ImportManager.show(R);
      }
   }
   showChannels();
   $.widget("custom.iconselectmenu", $.ui.selectmenu, {
      _renderItem: function(ul, item) {
         var li = $("<li>", { text: item.label });
         if ( item.disabled ) {
            li.addClass( "ui-state-disabled" );
         }
         $( "<span>", {
            "style": item.element.attr("data-style"),
            "class": "ui-icon " + item.element.attr("data-class")
         }).appendTo(li);
         return li.appendTo( ul );
      }
   });
   $("#RepoSelector").iconselectmenu({
      width: 300,
      change: showChannels
   }).iconselectmenu("menuWidget").addClass("ui-menu-icons customicons");
});
</script>

