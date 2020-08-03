/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

function COOKIEcreate(name, value, days) {
   if (days) {
      var date = new Date();
      date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
      var expires = "; expires=" + date.toGMTString();
   }
   else var expires = "";
   document.cookie = name + "=" + value + expires + "; path=/";
}

function COOKIEread(name) {
   var nameEQ = name + "=";
   var ca = document.cookie.split(';');
   for (var i = 0; i < ca.length; i++) {
      var c = ca[i];
      while (c.charAt(0) == ' ') c = c.substring(1, c.length);
      if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
   }
   return null;
}

function COOKIEerase(name) {
   COOKIEcreate(name, "", -1);
}

