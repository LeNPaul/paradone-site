/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

var SkinNameList = new Array("Default", "256 Color");
var SkinDirectoryList = new Array("", "low_color/");

function SkinningWindow()
{
    if (!document.getElementById('skinning_window'))
    {
       var body = document.getElementsByTagName("body")[0];
       var div = document.createElement("div");
    
       div.id = "skinning_window";
       div.innerHTML = composeSkinWindowBody();
    
       alignToMiddle(div);
    
       body.appendChild(div);
    }
    else
    {
        closeSkinningWindow();
    }
};

function composeSkinWindowBody()
{
    /*var text = "";
    for (SkinIndex = 0; SkinIndex < SkinNameList.length; SkinIndex++) 
    {
        text += "<a href=\"javascript:setSkin(\'" + SkinDirectoryList[SkinIndex] + "\')\">" + SkinNameList[SkinIndex] + "</a><br />\n";
    }
    text += "<br /><p style=\"text-align: right\"><a href=\"javascript:closeSkinningWindow()\">Close</a></p>";
    return text;*/
	///Skin option removed - see #19896
	return "<h3>Iguana Color Depth</h3>\
	        Choose your display settings:<br>\
	        <a href=\"javascript:setSkin(\'\')\"><img src=\"./images/color_swatch_high.jpg\"></a>"
}

function closeSkinningWindow()
{
    var window = document.getElementById("skinning_window");
    if (window != null) 
    {
        window.parentNode.removeChild(window);
    }
};

function alignToMiddle(element)
{
    element.style.top = (document.documentElement.scrollTop + (document.documentElement.clientHeight / 2) - 125) + "px";
    element.style.left = (document.documentElement.scrollLeft + (document.documentElement.clientWidth / 2) - 175) + "px";
};

function setCookie(name, value)
{
   var cookieDate = new Date();
   cookieDate.setFullYear (cookieDate.getFullYear() + 1);
   document.cookie = name + "=" + value + "; expires=" + cookieDate.toGMTString(); 
};

function clearCookie(name)
{
    var currentDate = new Date();
    currentDate.setTime(currentDate.getTime() - 1);
    document.cookie = name + "=; expires=" + currentDate.toGMTString();
};

function readCookie(name)
{
    var cookieName = name + "=";
    var cookieArray = document.cookie.split(';');
    for (var cookieIndex = 0; cookieIndex < cookieArray.length; cookieIndex++) 
    {
        var cookie = cookieArray[cookieIndex];
        while (cookie.charAt(0) == ' ') 
        {
            cookie = cookie.substring(1, cookie.length);
        }
        if (cookie.indexOf(cookieName) == 0) 
        {
            return cookie.substring(cookieName.length, cookie.length);
        }
    }
    return null;
};

function setSkin(directory)
{
	var cookieData = readCookie("IguanaSkinningDirectoryCookie");
    if (directory == null) 
    {
        clearCookie("IguanaSkinningDirectoryCookie");
		if(cookieData != null)
		{
			window.location.reload();
		}
		else
		{
			closeSkinningWindow();
		}
    }
    else 
    {
        setCookie("IguanaSkinningDirectoryCookie", directory);
		if(cookieData != directory)
		{
			window.location.reload();
		}
		else
		{
			closeSkinningWindow();
		}
    }
};
