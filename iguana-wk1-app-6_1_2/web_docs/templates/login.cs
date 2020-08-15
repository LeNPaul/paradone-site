<?cs include:"doctype.cs" ?>

<html>

<head>
   <title><?cs if:ServerLabel ?> (<?cs var:html_escape(ServerLabel) ?>)<?cs /if ?></title>
   <?cs include:"browser_compatibility.cs" ?>
   <?cs include:"styles.cs" ?>
   <link rel="stylesheet" type="text/css" href="<?cs var:skin("iguana_configuration.css") ?>" />
   <script type="text/javascript" src="<?cs var:iguana_version_js("/js/cookie/cookiev4.js") ?>"></script>
   
   <script type="text/javascript" src="/js/jquery-1.11.2/jquery-1.11.2.min.js"></script>

   <script type="text/javascript" src="<?cs var:iguana_version_js("jquery.jfeed.pack.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("jquery.watermark.min.js") ?>"></script>
   <script type="text/javascript" src="<?cs var:iguana_version_js("version_notifier.js") ?>"></script>

<style type="text/css">

html {
    height: 100%;   
}

body {
    height: 100%;
    margin: 0;
    background-repeat: no-repeat;
    background-attachment: fixed;
    /* GREEN Gradient Background */
    /* Permalink - use to edit and share this gradient: http://colorzilla.com/gradient-editor/#24ab3a+35,8BC34A+100 */
    background: #4CAF50; /* Old browsers */
    background: -moz-linear-gradient(-45deg,  #4CAF50 35%, #8BC34A 100%); /* FF3.6+ */
    background: -webkit-gradient(linear, left top, right bottom, color-stop(35%,#4CAF50), color-stop(100%,#8BC34A)); /* Chrome,Safari4+ */
    background: -webkit-linear-gradient(-45deg,  #4CAF50 35%,#8BC34A 100%); /* Chrome10+,Safari5.1+ */
    background: -o-linear-gradient(-45deg,  #4CAF50 35%,#8BC34A 100%); /* Opera 11.10+ */
    background: -ms-linear-gradient(-45deg,  #4CAF50 35%,#8BC34A 100%); /* IE10+ */
    background: linear-gradient(135deg,  #4CAF50 35%,#8BC34A 100%); /* W3C */
    filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#4CAF50', endColorstr='#8BC34A',GradientType=1 ); /* IE6-9 fallback on horizontal gradient */
}
       
input.user_pass
{
    background-color: #f1f8e9;
    border: 1px solid #dcedc8;
    border-radius: 3px;
    color: #37474F;
    font-family: "Open Sans",sans-serif;
    font-size: 14px;
    height: 26px;
    margin-right: 10px;
    padding: 4px 8px;
    width: 168px;
}

span.news_item_date
{
   font: 10px 'Open Sans', sans-serif;
   color: #999; 
   margin-bottom: 5px; 
   margin-left: 5px;
}

div.login_error
{
   font-family: 'Open Sans', sans-serif;
   font-size: 11px;
   font-weight: bold;
   color: #EF6C00;
   margin-top: -120px;
   margin-left: auto;
   margin-right: auto;
   /* background-image:url('/images/icon_warning.gif'); */
   background-position: top left; 
   background-repeat: no-repeat;
}

div.news_item{
   font: 11px 'Open Sans', sans-serif;
   color: #57584e; 
   line-height: 1.5em;
   margin-top: 5px;
}
a.news_item_title{
   color: #546e7a;
   font-family: 'Open Sans', sans-serif;
   font-size: 12px;
   font-weight: 400;
   line-height: 1.75em;
   text-decoration: none;
}

a.news_item_title:hover {
   text-decoration: underline;
}

img.expand_tab{
   position: absolute; 
   left: 215px; 
   top: 68px; 
   height: 13px; 
   width:74px;
}

div.version {
   background-color: #47b2e8;
   border-radius: 5px;
   box-shadow: 0 1px 1px 1px rgba(0, 0, 0, 0.3);
   height: 30px;
   margin: auto;
   padding: 10px;
   position: relative;
   text-align: center;
   top: 30px;
   width: 300px;
   z-index: 1000;
}

div.version a{
   line-height: 30px !important;
   font-size: 14px;
   font-family: "Open Sans", sans-serif;
   text-decoration:none;
   color: #FFFFFF;
   font-weight: 400;
   letter-spacing: 0.02em;
}

div.version a.close_link{
   float: right;
   padding-right: 4px;
   padding-top: 2px;
}

div.version a.close_link img{
   height:15px;
   width:15px;
   border:0px;
   margin-left: -15px;
}
 
div.login_background {
   position: absolute; 
   top:48%; 
   margin-top:-240px; 
   left: 50%; 
   margin-left:-300px; 
   height:500px; 
   width:600px; 
   background-color:#EEEEEE;
   border-radius: 4px;
   box-shadow: 0 1px 2px rgba(0, 0, 0, 0.5);
   -moz-box-shadow: 0 1px 2px rgba(0, 0, 0, 0.5);
   -webkit-box-shadow: 0 1px 2px rgba(0, 0, 0, 0.5);
} 

div#top_frame {
   background-color: #FFFFFF;
   border-top-left-radius: 4px;
   border-top-right-radius: 4px;
   text-align: center;
} 

img.logo {
   position: relative;
    top: 30px;
} 

div.newsfeed_footer {
    position: absolute; 
    width: 100%; 
    bottom: 2px; 
    text-align: center;
}

div.newsfeed_footer p {
    font-size: 10px;
    line-height: 15px;
    text-transform: uppercase;
    letter-spacing: 0.2em;
    color: rgba(0,0,0,0.15)
}
 
/*------------- custom <div> login_message_content definition (for login message) -------------*/
div.login_caution {
    font-size: 0.75em;
    width: 83%;
    margin: auto;
    position: relative;
    top: 54px;
}

hr.divider {
    position: relative;
    top: 98px;
    height: 4px;
    /* GREEN Gradient Background */
    /* Permalink - use to edit and share this gradient: http://colorzilla.com/gradient-editor/#24ab3a+35,8BC34A+100 */
    background: #4CAF50; /* Old browsers */
    background: -moz-linear-gradient(-45deg,  #4CAF50 35%, #8BC34A 100%); /* FF3.6+ */
    background: -webkit-gradient(linear, left top, right bottom, color-stop(35%,#4CAF50), color-stop(100%,#8BC34A)); /* Chrome,Safari4+ */
    background: -webkit-linear-gradient(-45deg,  #4CAF50 35%,#8BC34A 100%); /* Chrome10+,Safari5.1+ */
    background: -o-linear-gradient(-45deg,  #4CAF50 35%,#8BC34A 100%); /* Opera 11.10+ */
    background: -ms-linear-gradient(-45deg,  #4CAF50 35%,#8BC34A 100%); /* IE10+ */
    background: linear-gradient(135deg,  #4CAF50 35%,#8BC34A 100%); /* W3C */
    filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#4CAF50', endColorstr='#8BC34A',GradientType=1 ); /* IE6-9 fallback on horizontal gradient */
    border-top: 1px solid rgba(0,0,0,0.1);
    border-bottom: 1px solid rgba(0,0,0,0.1);
    margin: 0px 50px;
    border-radius: 2px;
}

button.login-button {
    display: inline-block;
    font-size: 14px;
    line-height: 24px;
    width: 100px;
    position: relative;
    margin: 0px;
    border-top: none;
    border-right: none;
    border-left: none;
}

span.label {
    color: #388e3c;
    display: block;
    font-size: 10px;
    font-weight: 600;
    letter-spacing: 0.05em;
    margin-bottom: 5px;
    text-transform: uppercase;
}

</style>

<script type="text/javascript">
   // ------------------- Javascript ----------------------------

   var NewsFeedExpanded = false;
   function clickNewsFeed()
   {
      var NewState = null;
      ( NewsFeedExpanded ? NewState = { marginTop : '0px' } : NewState = { marginTop : '-165px'  } )
      $('#top_frame').animate( NewState, 
                               duration = "slow")
                             .queue(function(){ 
                                    NewsFeedExpanded = !NewsFeedExpanded
                                    $('#bottom_frame').css('overflow', ( NewsFeedExpanded ? 'auto' : 'hidden'  ));
                                    showOrHideTab("slow");
            $(this).dequeue();})
                   .queue(function(){
                  (NewsFeedExpanded ? $('.news_item').slideDown() : $('.news_item').slideUp() );
                  $(this).dequeue();});
   }

   function showOrHideTab(AnimationTime)
   {
      if (NewsFeedExpanded)
      {
         $('#img_expand').fadeOut(AnimationTime);         
         $('#img_contract').fadeIn(AnimationTime);         
      }   
      else
      {
         $('#img_expand').fadeIn(AnimationTime);         
         $('#img_contract').fadeOut(AnimationTime);         
      }
   }

   function fetchNewsFeed()
   {
       jQuery.getFeed({
        url: '/newsfeed.rss',
        success: function(feed) {
                    
            var html = '';

            for(var i = 0; i < feed.items.length; i++) {
            
                var item = feed.items[i];
                
    ThisDate = new Date(item.updated); 
                html += '<div><a class="news_item_title" target="_blank" href="' + item.link + '">'
                + item.title
                + '</a> ' 
    + '<span class="news_item_date">' + ThisDate.toLocaleDateString() + '</span>';
                
                html += '<div style="display: ' + (NewsFeedExpanded ? '' : 'none')  + ';"'
         +  ' class="news_item">' 
         + item.description + '</div><br/>';
            }
            
            jQuery('#bottom_frame').append(html);
        }    
    });
   }

   var DefaultUserName = '<?cs var:js_escape(DefaultUserName) ?>';
   var DefaultPassword = '<?cs var:js_escape(DefaultPassword) ?>';
   function initLoginFields()
   { 
      if (DefaultUserName && DefaultPassword)
      {
         $('#username').attr('value', DefaultUserName); 
         $('#password').attr('value', DefaultPassword);
      }
   }

   $(document).ready( function()
   {
      CheckLowColor();
      showOrHideTab();
      fetchNewsFeed();
      VNBversionNotificationController.init();
      VNBversionNotificationController.fetchContent();
      setInterval("VNBversionNotificationController.fetchContent()", 24 * 60 * 60 * 1000);  // once a day
      initLoginFields();
      $('#username').focus();
   })

   function readCookie(name)
   {
      var cookieName = name + "=";
      var cookieArray = document.cookie.split(';');
      for(var cookieIndex = 0; cookieIndex < cookieArray.length; cookieIndex++)
      {
         var cookie = cookieArray[cookieIndex];
         while(cookie.charAt(0) == ' ') cookie = cookie.substring(1, cookie.length);
         if(cookie.indexOf(cookieName) == 0) return cookie.substring(cookieName.length, cookie.length);
      }
      return null;
   }


   function CheckLowColor()
   {
      var cookieData = readCookie("IguanaSkinningDirectoryCookie");
      if(screen.colorDepth == 8)
      {
         if(cookieData != 'low_color/')
         {
            document.cookie='IguanaSkinningDirectoryCookie=low_color/; path=/';
            window.location.reload();
         }
      }
   }

   // ----------------------------------------------------------
</script>

</head>


<body>

<!--
<div id="header">
</div>
-->

<!--[if lte IE 8]>
  <div style='border-bottom: 2px solid #ecca2d; background: #fffae4; text-align: center; clear: both; height: 75px; position: relative; z-index:1000;'>
    <div style='position: absolute; right: 3px; top: 3px; font-family: courier new; font-weight: bold;'><a href='#' onclick='javascript:this.parentNode.parentNode.style.display="none"; return false;'><img src='images/ie6nomore-cornerx.gif' style='border: none;' alt='Close this notice'/></a></div>
    <div style='width: 640px; margin: 0 auto; text-align: left; padding: 0; overflow: hidden; color: black;'>
      <div style='width: 75px; float: left;'><img src='images/ie6nomore-warning.gif' alt='Warning!'/></div>
      <div style='width: 270px; float: left; font-family: Arial, sans-serif;'>
        <div style='font-size: 14px; font-weight: bold; margin-top: 12px;'>You are using an outdated browser</div>
        <div style='font-size: 11px; margin-top: 6px; line-height: 14px;'>Please upgrade to a compatible web browser 
        as outlined in Iguana's <a href='http://www.interfaceware.com/wiki/iguana_system_requirements.html'>system requirements</a>.</div>

      </div>
      <div style='width: 73px; float: left;'><a href='http://www.google.com/chrome' target='_blank'><img src='images/ie6nomore-chrome.gif' style='border: none;' title='Get Google Chrome'/></a></div>
      <div style='width: 73px; float: left;'><a href='http://www.firefox.com' target='_blank'><img src='images/ie6nomore-firefox.gif' style='border: none;' title='Get Firefox'/></a></div>
      <div style='width: 73px; float: left;'><a href='http://www.apple.com/safari/download/' target='_blank'><img src='images/ie6nomore-safari.gif' style='border: none;' title='Get Safari'/></a></div>
      <div style='float: left;'><a href='http://windows.microsoft.com/en-US/internet-explorer/products/ie/home' target='_blank'><img src='images/ie6nomore-ie8.gif' style='border: none;' title='Get IE 8 or Higher'></a></div>
    </div>
  </div>
<![endif]-->

<div class="version" style="display:none;"></div>

<div class="login_background">

<div id="outer_frame" style="overflow:hidden; height:460px; width:600px; position:relative;">
   <div id="top_frame" style="position: relative; height: 375px; overflow:hidden;">
          
          <img src="/images/login-logo.png" class="logo"><br/>
<div class="login_caution">This application may provide you with access to sensitive information, including <strong>Protected Health Information (PHI)</strong>. By logging into this application, you acknowledge your responsibility to adhere to the privacy policies, processes, and procedures of your organization or the organization you are acting on the behalf of.</div>
          <hr class="divider">
          
      <div id="login_elements" style="position:absolute; top:295px ;margin-left:50px;">
         <form id="login_form" method="post" action="/login.html">
            <?cs each: SavedVariable = Saved ?>
              <input name="<?cs var:"Saved." + name(SavedVariable) + ".Name" ?>" type="hidden" value="<?cs var:html_escape(SavedVariable.Name) ?>" />
              <input name="<?cs var:"Saved." + name(SavedVariable) + ".Value" ?>" type="hidden" value="<?cs var:html_escape(SavedVariable.Value) ?>" />
            <?cs /each ?>
            <input type="hidden" name="RedirectLocation" value="<?cs var:RedirectLocation ?>">
            <input type="hidden" name="RedirectRequestMethod" value="<?cs var:RedirectRequestMethod ?>">
            <table>
               <tr>
                  <td style="padding:0px; text-align: left; vertical-align: bottom;"><span class="label">Username:</span><input class="user_pass" id="username" name="username"/></td>
                  <td style="padding:0px; text-align: left; vertical-align: bottom;"><span class="label">Password:</span><input class="user_pass" id="password" type="password" name="password" autocomplete="off"/></td>
                  <td style="padding:0px; vertical-align: bottom;">
                   <button type="submit" class="action-button green login-button" />Log in</button>
                  </td>
               </tr>
            </table>
            
            <?cs if:ErrorMessage ?>
              <div class="login_error"><?cs var:html_escape(ErrorMessage) ?></div>
            <?cs elseif:SessionExpired ?>
               <div class="login_error">You have been logged out. To continue your requested operation, re-login here. <br>Otherwise, <a href="../login.html" style="color:#4CAF50">reset the login page</a> to start a new session.</div>
            <?cs /if ?>
            
         </form>

         <img id="img_contract" src="/images/newsfeed_contract.gif" class="expand_tab"
               onclick="javascript:clickNewsFeed();"><!--This is the Collapse Tab clickable area --></img>
         <img id="img_expand" src="/images/newsfeed_expand.gif" class="expand_tab"
               onclick="javascript:clickNewsFeed();"><!--This is the Expand Tab clickable area --></img>
      </div>
   </div>
   <div id="bottom_frame" style="padding-left: 50px; margin-top: 16px; padding-right: 45px; overflow:hidden; height: 225px; border:0px;"></div>
</div><!-- outer_frame -->

<div class="newsfeed_footer" style=""><p>iNTERFACEWARE Newsfeed</p></div>

</div>
</body>

<!--[if lte IE 7]>
   <script type="text/javascript">
      document.getElementById("login_form").style.display = "none";
   </script>
<![endif]-->

</html>
