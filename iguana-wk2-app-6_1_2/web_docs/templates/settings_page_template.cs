<?cs include:"doctype.cs" ?>

<html>  <?cs # vim: set syntax=html :?>
<head>
   <title></title>
   <?cs include:"browser_compatibility.cs" ?>
   <link rel="stylesheet" type="text/css" href="jquery-ui-1-8-16/css/ui-lightness/jquery-ui-1.8.16.custom.css" />  
   <?cs include:"styles.cs" ?>
   <style type="text/css">

   #dashboard_body
   {
      background-color:#E4E5E0;
   }

   div.workarea
   {
      border:1px solid #A0A0A0;
      border-collapse:collapse;
      width:500px;  
      background-color:#FFFFFF;
      padding:10px;
   }

   div.border_green
   {
      width:100%;
      height:5px;
      background-color:#71B13C;
      padding:0px;
      display:block;
   }   

   div.border_grey
   {
      width:100%;
      height:2px;
      background-color:#D7D7D7;
      padding:0px;      
   }   

   h1
   {
      text-align:left;
      width:100%;
      padding-bottom:5px;
      font-size:17px;   
      font-weight:bolder;
      color:#222222;
   }

   h2
   {
      text-align:left;
      width:100%;
      height:20px;
      font-size:14px;   
      font-weight:bolder;
      color:#4D7500;
      padding-bottom:10px;
      margin:0px;
   }

   h3
   {
      margin:0px;
      font-size:11px;   
      font-weight:bolder;
      color:#222222;
      width:100%;
      height:20px; 
      padding:2px;
      padding-right:10px; `  
   }

   h3 a
   {
      font-size:11px;   
      font-weight:bolder;
      color:#222222;
      padding:2px;
      padding-right:10px; 
      margin:0px;
      width:100%;
      height:20px; 
     
   }

   div.label_text
   {
      font-size:11px;   
      font-weight:bolder;
      color:#222222;
      padding:2px;
      padding-right:10px; 
      height:20px;
      width:100px;
   }   

   div.value_text
   {
      font-size:11px;   
      color:#222222;
      padding:2px;
      width:150px;
      height:20px;
      position:relative;
      left:100px;
      top:-24px;
   }   

   div.field_text
   {
      font-size:11px;   
      color:#222222;
      padding:2px;
      width:150px;
      height:20px;
      position:relative;
      left:100px;
      top:-24px;
   }   

   div.label_checkbox
   {
      font-size:11px;   
      font-weight:bolder;
      color:#222222;
      padding:2px;
      padding-right:10px; 
      width:200px;
   }   

   div.buttons
   {
      width:100%;
      height:20px;
      padding:10px;
   }

   div#side_header
   { 
      padding-top:10px; 
      height:25px;
   }

   fieldset
   {
      border:none;
      width:100%;
      padding:0px;
      margin:0px;
   }

   ul
   {
      padding-left:0px;
      margin:0px;
   }

   li
   {
      list-style:none;
      width:100%; 
      padding-left:15px;
      padding-right:15px;
   }

   li.generic_control
   {
      height:30px;
   }

   input
   {
      background:#F3F3F3;
   }

   input.checkbox
   {
      background:#FFFFFF;
      margin:0px; 
   }

   a 
   {
      text-decoration:none;
      border:none;
   }

   a.label
   {
      font-size:12px;   
      font-weight:bolder;
   }   

   img
   {
      border:none;
      vertical-align:middle;
   }

   img.arrow
   {
      width:9px;
      height:9px;
   }

   .context_help
   {
      font-weight:normal;
      font-size:9px;
      color:#787A77;
   }   

</style>
</head>
<script type="text/javascript"><!--

// TODO: Add Optional Javascript

--> </script>

<body class="tabright" onload="javascript:onLoad();">

<?cs set:Navigation.CurrentTab = "Settings" ?>
<?cs include:"header.cs" ?>


<div id="main">

   <div id="iguana">

      <div id="dashboard_body">

         <center>
                  
            <div class="workarea">

               <h1>
               <!-- TODO: Add Page Header -->
               </h1>
                 
               <div class="border_green"></div>
                  
               <div class="h2">
                  
               <!-- TODO: Add Subheader -->

               </div>

               <!-- TODO: Add Content -->

               <div class="border_grey" ></div>

               <!-- TODO: Add Optional Page Buttons -->
 
            </div>
                        
         </center>

      </div>
     
   </div>
   
</div>

<div id="side_panel">
   
   <div id="side_table">
      
      <div id="side_header">
      Page Help
      </div>

      <div id="side_body">

         <h4 class="side_title">Overview</h4>

         <!-- TODO: Add Page Help Content -->
 
         <h4 class="side_title">Related Settings</h4>

         <!-- TODO: Add Optional Related Help Pages -->
      
      </div>

      <div class="side_body">

         <h4 class="side_title">Help Links</h4>

         <!-- TODO: Add Optional Related Help Links -->

      </div>
   
    </div>

</div>

</body>

</html> 
