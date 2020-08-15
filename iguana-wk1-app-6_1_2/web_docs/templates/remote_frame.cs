<?cs include:"doctype.cs" ?>
<?cs # vim: set syntax=html :?>

<html>
<head>
   <title><?cs var:html_escape(TargetServerLabel) ?></title>
   <?cs include:"browser_compatibility.cs" ?>
<style type="text/css">
#global_dash_header {
   font-family: Verdana, sans-serif;
   font-size: 11px;
   padding: 10px;
   background-color: #D0D6B8;
   color: #30302C;
   border-left: 1px solid #757671;
   border-right: 1px solid #757671;
   border-bottom: 1px solid #757671;
   -moz-border-radius: 0 0 5px 5px;
   -webkit-border-radius: 0 0 5px 5px;
   -moz-box-shadow: 0 2px 2px #777777;
   -webkit-box-shadow: 0 2px 2px #777777;
   height: 30px;
   background-position: bottom center;
   text-align: center;
   z-index: 999; 
   position: absolute; 
   top: 0px; 
   height: 30px; 
   left: 30%;
   right: 30%;
   margin: 0px;
}

#global_dash_header a{
   color: #215FA3
}

.body{
   font-family: Verdana, Arial, Helvetica, Geneva, Lucida, sans-serif;
   font-size: 8px;
   width: 100%;	
   height:100%;  
   padding: 0px;
   margin: 0px; 
   overflow: hidden; 
}
</style>
</head>

<body>
<div id="global_dash_header">
Back to <a target="_top" href="/dashboard.html"><?cs var:html_escape(ServerLabel) ?></a> 
 | <a target="_top" href="<?cs var:html_escape(TargetSrc) ?>">Remove this frame</a>
</div>
<div style="position: absolute; top: 0px; bottom: 0px; left: 0px; right: 0px; padding: 0px; margin: 0px;">
   <iframe src="<?cs var:html_escape(TargetSrc) ?>"  style="width:100%; height: 100%; padding: 0px; margin: 0px; border: 0px; border-collapse: collapse;">
   <p>Your browser does not support iframes.</p>
    </iframe>
</div>
</body>
</html>

