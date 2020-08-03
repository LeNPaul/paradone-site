/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// Javascript functions related to classes

// Returns true if the specified element has the specified class.
// Returns false otherwise.
function CLSelementHasClass(Element, ClassName)
{
   var ClassPattern = new RegExp('\\b' + ClassName + '\\b', '');
   return Element.className && ClassPattern.test(Element.className);
}

function CLSelementAddClass(Element, ClassName)
{
   if( Element.className !== undefined && !CLSelementHasClass(Element,ClassName) )
   {
      Element.className += ' ' + ClassName;
   }
}

function CLSelementRemoveClass(Element, ClassName)
{
   if( Element.className !== undefined )
   {
      var ClassPattern = new RegExp('(^|\\s)' + ClassName + '($|\\s)', '');

      Element.className = Element.className.replace(ClassPattern, ' ');
   }
}
