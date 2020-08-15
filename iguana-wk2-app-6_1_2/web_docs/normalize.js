/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */


var NORM_div;
function NORMnormalizeInnerHtml(html)
{
  if (!NORM_div) { NORM_div = document.createElement('div'); } 
   NORM_div.innerHTML = html;
   return NORM_div.innerHTML;
}

var NORM_a;
function NORMnormalizeHref(href)
{
   if (!NORM_a) { NORM_a = document.createElement('a'); }
   NORM_a.href = href;
   return NORM_a.href;
}

var NORM_img;
function NORMnormalizeImg(src)
{
   if (!NORM_img) { NORM_img = document.createElement('img'); }
   NORM_img.src = src;
   return NORM_img.src;
}
