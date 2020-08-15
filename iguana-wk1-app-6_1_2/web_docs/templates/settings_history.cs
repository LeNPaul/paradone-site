<?cs def:showError(Value) ?>
<div style="clear:both;">
<?cs if:Value ?>
<span style="color:red;" id="<?cs name:Value ?>"><?cs var:html_escape(Value) ?></span>
<?cs else ?>
<span id="<?cs name:Value ?>"></span>
<?cs /if ?>
</div>
<?cs /def ?>

<div id="iguana">
<div id="cookie_crumb">
<a href="/settings">Settings</a> &gt; History
</div>

<?cs if:ErrorMessage ?>
<h3><font color="red" id="ErrorMessage"><?cs var:html_escape(ErrorMessage) ?></font></h3>
<?cs else ?>
<span id="ErrorMessage"></span>
<?cs /if ?>

<?cs if:StatusMessage ?>
<h3><font color="green" id="StatusMessage"><?cs var:html_escape(StatusMessage) ?></font></h3>
<?cs else ?>
<span id="StatusMessage"></span>
<?cs /if ?>

<div id="HistoryBody"></div>
<!--/#iguana--></div>
<!--/#main--></div>

<style type="text/css">
#main {
   margin-right: 25px;
   margin-bottom: 25px;
   position: absolute;
   top: 0px;
   right: 0px;
   bottom: 0px;
   left: 0px;
}
#iguana {
   position: absolute;
   top: 90px;
   bottom: 0px;
}
#SCMcommitList {
   top: 0px;
}
#cookie_crumb {
   padding-top: 10px;
   height: 31px;
}
</style>
<script type="text/javascript">
$(document).ready(function() {
   $("#ServerLabel").on("change keyup", SettingsHelpers.updateServerName);
   SCMsettingsReview();
});
</script>

