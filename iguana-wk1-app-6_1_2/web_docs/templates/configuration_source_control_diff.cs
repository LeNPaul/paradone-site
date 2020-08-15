<?cs include:"doctype.cs" ?>

<script type="text/javascript" src="<?cs var:iguana_version_js("/js/ajax/ajax.js") ?>" ></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("/js/utils/window.js") ?>"></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("normalize.js") ?>"></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("/js/cookie/cookiev4.js") ?>"></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("jsdifflib/difflib.js") ?>"></script>
<script type="text/javascript" src="<?cs var:iguana_version_js("jsdifflib/diffview.js") ?>"></script>
<script type="text/javascript"> 

// Build the difference table.
function buildDiffView()
{           
   var AJAXCommandName = 'retrieve_configuration_content';
   var AJAXFileNameVariable = 'FileName=';
                    	 
   AJAXpost(AJAXCommandName, AJAXFileNameVariable + '<?cs var:FileName ?>', 
      function(data)
      { 
         var Response = eval(data);
         
         if(undefined != Response.LoggedIn && !Response.LoggedIn)
         {
            alert('Iguana is not responding.  Please return to the previous page and try again.');
         }
         else
         { 
            var RepositoryFileContent = difflib.stringAsLines(Response.RepositoryFileContent);
            var LocalFileContent = difflib.stringAsLines(Response.LocalFileContent);
            
            var Matcher = new difflib.SequenceMatcher(RepositoryFileContent, LocalFileContent);
            var Opcodes = Matcher.get_opcodes();
               
            var DiffResult = document.getElementById("result");
            var Wait = document.getElementById("wait");
            
            Wait.style.display = "none";
            DiffResult.style.display = "";
      
            var RepositoryFileTitle = "<?cs var:FileName ?>" + "-Repository";
            var LocalFileTitle = "<?cs var:FileName ?>" + "-Local";
            
            DiffResult.appendChild(diffview.buildView({baseTextLines:RepositoryFileContent, newTextLines:LocalFileContent, opcodes:Opcodes, baseTextName:RepositoryFileTitle, newTextName:LocalFileTitle, contextSize:null, viewType:0}));
   
         }

      },
                  
      function(data) 
      {
         alert('Iguana is not responding.  Please return to the previous page and try again.');
      }
   );           
}

// AJAX function that checks for commit errors for the last commit operations.
function checkForCheckoutErrors()
{  
   var AJAXCommandName = 'check_configuration_checkout';
                 	 
   AJAXpost(AJAXCommandName, "", 
      function(data)
      { 
         var Response = eval(data);
         
         if(undefined != Response.LoggedIn && !Response.LoggedIn)
         {
            alert('Iguana is not responding.  Please return to the previous page and try again.');
         }
         else
         { 
            var IsDetermined = Response.IsDetermined;
            
            if(IsDetermined)
            {
               setTimeout("buildDiffView()", 500);
            }
            else
            {                              
               setTimeout("checkForCheckoutErrors()", 500); 
            }
         }
      },
      
      function(data) 
      {
         alert('Iguana is not responding.  Please return to the previous page and try again.');
      }
   );
}

// Executes the checkout of the configuration module.
function onLoad()
{
   var AJAXCommandName = 'checkout_configuration';
      
   AJAXpost(AJAXCommandName, "", 
      function(data)
      { 
         setTimeout("checkForCheckoutErrors()", 500);             
      },
   
      function(data) 
      {
         alert('Iguana is not responding.  Please return to the previous page and try again.');
      }
   );
}      

</script>

<html>
<head>
   <title>Iguana - Diffing <?cs var:FileName ?></title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("jsdifflib/diffview.css") ?>" >
 
   <style type="text/css">   
   
   #wait
   {
      font-size:12px;
      font-weight:bold;   
   }
               
   </style>
</head>

<body class="tableft" onload="javascript:onLoad();">

<center>  
<div id="main" style="margin:25px;">

<table id="iguana">

   <tr>
      <td id="dashboard_body">
      
      <center>      
      <div id="diff" name="diff">
         <div id="wait" name="wait" style="display:inline;"><br><br><br>Please wait while we compare the versions. <img style="vertical-align:middle;" src="images/spinner.gif" /></div>
         <pre>
         <div id="result" name="result" style="display:none;"></div>
         </pre>
      </div>               
      </center>  
      
      </td>
       
   </tr>

</table>

</div>
</center>
			
</body>

</html>