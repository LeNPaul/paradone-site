<?cs include:"doctype.cs" ?>
<html>  <?cs # vim: set syntax=html :?>
<head>

   <title>Iguana <?cs if:ServerLabel ?>(<?cs var:html_escape(ServerLabel) ?>) <?cs /if ?>&gt; Logs</title>
   <?cs include:"browser_compatibility.cs" ?>
   <script type="text/javascript"><!--
   UniqueInstanceId = '<?cs var:UniqueInstanceId ?>';
   --></script>
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("log.css") ?>" />
   
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.css") ?>">   
   
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("jquery.treeview.css") ?>" />
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/treeview/treeview.css") ?>" />
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("/js/help_popup/help_popup.css") ?>" />
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("syntax_highlight.css") ?>" />

   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-1.11.2.min.js"></script>
   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-ui-1.11.2/jquery-ui.min.js"></script>   
   
   <script type="text/javascript" src="<?cs var:iguana_version_js("jquery.treeview.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("jquery.color.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/treeview/treeview.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/tooltip.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/slider.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/cookie/cookiev4.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("from_channel.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("segment_view.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("table_view.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("log_browser.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/help_popup/help_popup.js") ?>"></script>
   <?cs include:"mini-login.cs" ?>

<style type="text/css">

iframe#popoutErrorFrame
{
   width: 100%;
   height: 100%;
}

div#popoutErrorDialog
{
   position: absolute;
   z-index: 2;
   background-color: #FFFFFF;
   border: solid #606060 5px;
   font-size: 11px;
}

div#popoutErrorDialog div.errorHeader
{
   text-align: center;
   font-weight: bold;
   padding: 5px 8px 5px 8px;
   background-color: #FFFF66;
   border-bottom: solid #606060 1px;
}

div#popoutErrorDialog div#popoutErrorText
{
   overflow: auto;
   padding: 5px;
}

</style>

<script type="text/javascript">
<!--

<?cs set:exclude_iterator_defintion = 1 ?>
<?cs include:"log_browser_js.cs" ?>

function getViewModeHash()
{
   if (window.location.hash != '')
   {
      return window.location.hash.substring(1);
   }
   else
   {
      return 'Text';
   }
}

function disableWindowAndDisplayError(ErrorMessage, AllowReload, DialogTitle)
{
   var Frame = document.getElementById('popoutErrorFrame');
   if( Frame.contentWindow )  // IE6 doesn't use iframe background colors.
   {
      Frame.contentWindow.document.body.style.background = 'gray';
   }
   Frame.style.display = '';
   
   document.getElementById('popoutErrorTitle').innerHTML = (DialogTitle ? DialogTitle : 'Disconnected From Log Browser');
   document.getElementById('popoutErrorText').innerHTML = ErrorMessage +
      (AllowReload ? '<br><br>You may close this window, or try <a onclick="javascript:window.location.reload();" href="' + window.location.hash + '">reloading</a>.' : '');
      
   document.getElementById('popoutErrorDialog').style.display = '';
   
   resizeErrorFrameAndDialog();
}

function enableWindowAndHideError()
{
   var Frame = document.getElementById('popoutErrorFrame');
   Frame.style.display = 'none';
      
   document.getElementById('popoutErrorDialog').style.display = 'none';
}

function resizeErrorFrameAndDialog()
{
   var Page = document.documentElement;
   var Dialog = document.getElementById('popoutErrorDialog');
   
   if (Dialog.style.display != '')
   {
      return;
   }
   
   var PageHeight = WINgetWindowHeight();
   var PageWidth  = WINgetWindowWidth();
   
   Dialog.style.width = '';
   var DialogHeight = $(Dialog).outerHeight();
   var DialogWidth  = $(Dialog).outerWidth();
   
   var MaxDialogWidth = PageWidth*(7/10);
   if (DialogWidth >= MaxDialogWidth)
   {
      Dialog.style.width = MaxDialogWidth + 'px';
      DialogWidth = MaxDialogWidth;
   }
   
   Dialog.style.top  = Math.max(0, (PageHeight-DialogHeight)/2) + 'px';
   Dialog.style.left = Math.max(0, (PageWidth-DialogWidth)/2)   + 'px';
}

var CurrentMessageId = '';
var WaitingOnFirstRequest = true;

// Send a "comet" request to the server.
// Server will keep the connection open until it actually has something to
// send to us.
//
// See http://en.wikipedia.org/wiki/Comet_(programming) for more details.
//
function doCurrentEntryCometRequest()
{
   AJAXpost('/comet_current_entry',
            'ParentGuid=<?cs var:ParentGuid ?>&RemoteValidationKey=<?cs var:RemoteValidationKey ?>&CurrentMessageId=' + CurrentMessageId,
      function(Data, ContentType)
      {
         WaitingOnFirstRequest = false;
      
         if( ContentType.match('application/json') )
         {
            var ResponseData = JSON.parse(Data);
            
            if (ResponseData.ErrorMessage)
            {
               disableWindowAndDisplayError(ResponseData.ErrorMessage, true);
            }
            else if (ResponseData.LogBrowserClosed)
            {
               disableWindowAndDisplayError('Main log browser window is no longer active.', true);
            }
            else if (ResponseData.ExplicitlyDisabled)
            {
               disableWindowAndDisplayError('This window has been disabled by the main log browser window.', false);
            }
            else if (ResponseData.LackingPermissions)
            {
               disableWindowAndDisplayError('You do not have the necessary permissions to view the current entry.', false, 'Not Permitted');
               
               // If the main log browser moves back to a message we can see, we want to know about it.
               CurrentMessageId = ResponseData.MessageId;
               doCurrentEntryCometRequest();
            }
            else if (!ResponseData.CurrentEntry)
            {
               disableWindowAndDisplayError('Message from Iguana contains no log entry.', true);
            }
            else
            {
               try
               {
                  enableWindowAndHideError();
                  
                  var Direction = 'Refresh';
                  var NewMessageId = ResponseData.CurrentEntry.MessageId;
                  if (CurrentMessageId != '' && CurrentMessageId > NewMessageId)
                  {
                     Direction = 'previous';
                  }
                  else if (CurrentMessageId != '' && CurrentMessageId < NewMessageId)
                  {
                     Direction = 'next';
                  }
                  CurrentMessageId = NewMessageId;
                  showEntry(ResponseData.CurrentEntry, Direction);
                  doCurrentEntryCometRequest();
                  return;
               }
               catch(Error)
               {
                  disableWindowAndDisplayError(Error.description ? Error.description : Error, true);
               }
            }
         }
         else
         {
            showTimeOutMessage(doCurrentEntryCometRequest);
         }
      },
      function(Error)
      {
         WaitingOnFirstRequest = false;
         showDisconnectMessage(Error, doCurrentEntryCometRequest);
      }
   );
}

function onWindowResize()
{
   resizeErrorFrameAndDialog();
   DescriptionAreaControl.resizeFrames();
   resize(document.getElementById('entryArea'));
}

var ResizeTimeoutId = null;
function onLoad()
{
   CurrentViewMode = getViewModeHash();
   initLogBrowser();

   window.onresize = function()
   {
      // Some browsers (like Firefox) don't always call the onresize event at the right time.
      // So we will make sure to run the adjustments when the user's mouse stops moving.
      clearTimeout(ResizeTimeoutId);
      ResizeTimeoutId = setTimeout('onWindowResize();', 150);
   }
   
   doCurrentEntryCometRequest();
   
   setTimeout(function()
   {
      if (WaitingOnFirstRequest)
      {
         // We have not received a message from the server since the page was opened
         // 10 seconds ago.  The log browser window must be inactive.
         document.getElementById('entryArea').innerHTML =
            '<div class="entryViewPleaseWaitBar">\
                Still waiting...<br/>\
                Main log browser window may no longer be active.\
                You may continue to wait, or try again.\
             </div>';
      }
   }, 10000);
   
   onWindowResize();
}
   
//-->
</script>

</head>

<body class="tableft" onLoad="onLoad()" style="background-color:#FFFFFF; overflow:hidden;">

<?cs set:showEntryView = 1 ?>
<?cs set:minimalControls = 1 ?>
<?cs include:"log_entry_view.cs" ?>

<?cs call:defineHelpTooltipDiv() ?>

<iframe id="popoutErrorFrame" src="/empty.html" style="display:none;">
</iframe>

<div id="popoutErrorDialog" style="display:none;">
   <div id="popoutErrorTitle" class="errorHeader">Disconnected From Log Browser</div>
   <div id="popoutErrorText"></div>
</div>

</body>
</html>
