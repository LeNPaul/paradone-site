/** @license
 * Copyright (c) 2010-2015 iNTERFACEWARE Inc.  All rights reserved.
 */

// return string with single quotes escaped
function PAGescapeQuotes(Input) {
   return Input.replace(/'/g, "&#39;").replace(/"/, "&quot;");
}

function PAGencodeAllTheThings(Input) {
  return encodeURIComponent(Input).replace(/[!'()*]/g, function(Char) {
    return '%' + Char.charCodeAt(0).toString(16);
  });
}

// NOTE: This can only be called on an encoded string.  With decoded strings, pluses will be incorrectly converted
// to the general encoding.
function PAGconvertSpaceEncoding(EncodedInput) {
   return EncodedInput.replace(/\+/g, "%20");
}


function PAGhtmlEscape(Text) {
   return Text.replace(/;/g, '&#59;')
      .replace(/&(?!#59;)/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;');
}

