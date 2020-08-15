/** @license
 * Copyright (c) 2015 iNTERFACEWARE Inc.  All rights reserved.
 */

// polyfill of LOG_VAR
// Normally the backend expands LOG_VAR(x) to something like console.log("x =", x);
LOG_VAR =  // this line gets removed by sed, and this anonymous function is never exposed
(function() {
   var CalleePattern = /(https?.*[0-9])/; 
   function GetCallee() {
      try {
         throw new Error();
      } catch(E) {
         if (E.stack) {
            var Stack = E.stack.split("\n");
            var ExpectedStackPosition = 3;   // callee, LOG_VAR, GetCallee
            if (Stack.length > ExpectedStackPosition) {
               var CallerLine = Stack[ExpectedStackPosition];
               var RegexMatch = CalleePattern.exec(CallerLine);
               if (RegexMatch !== null) {
                  return RegexMatch[1];
               }
            }
         }
      }
   }
   return function() {
      var FormattedArgs = [];
      var Callee = GetCallee();
      if (typeof Callee !== "undefined") {
         FormattedArgs.push(Callee);
      }
      for (var i = 0; i < arguments.length; i++) {
         var VarName = "";
         if (i > 0) {
            VarName += ", ";
         }
         VarName += "? =";
         FormattedArgs.push(VarName);
         FormattedArgs.push(arguments[i]);
      }
      console.log.apply(console, FormattedArgs);
   };
})();
