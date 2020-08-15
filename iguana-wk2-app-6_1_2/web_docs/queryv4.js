/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */


function QRYparse(q) 
{
   if (q.length > 1) 
      this.q = q.substring(1, q.length);
   else 
      this.q = null;

   this.keyValuePairs = new Array();
   if(this.q) 
   {
      for(var i=0; i < this.q.split("&").length; i++) 
      {
         this.keyValuePairs[i] = this.q.split("&")[i];
      }
   }
   this.getKeyValuePairs = function() { return this.keyValuePairs; }
   this.getValue = function(s) 
   {
      for(var j=0; j < this.keyValuePairs.length; j++) 
      {
         if(this.keyValuePairs[j].split("=")[0] == s)
            return this.keyValuePairs[j].split("=")[1];
      }
      return false;
   }
   this.getParameters = function() 
   {
      var a = new Array(this.getLength());
      for(var j=0; j < this.keyValuePairs.length; j++) 
      {
         a[j] = this.keyValuePairs[j].split("=")[0];
      }
      return a;
   }
   this.getLength = function() { return this.keyValuePairs.length; }
}

function QRYgetValue(Key)
{
   var Query = new QRYparse(window.location.search);
   return unescape(Query.getValue(Key));
}

