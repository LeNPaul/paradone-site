/** @license
 * Copyright (c) 2010-2011 iNTERFACEWARE Inc.  All rights reserved.
 */

// iNTERFACEWARE logging library mk 1
// Starting off with wrapper around Firebug's logging output


var LOG = {};

LOG.Indent = '';
LOG.FuncTable = {};

LOG.startFunctionLog = function (Name, Args) {
   console.log('>' + Name + LOG.formatArgsAsString(Args));
   LOG.Indent += ' ';
}

LOG.startGroup = function (Name, Args) {
   var Format = Name + '(';
   var ArgArray = [];
   var CountOfArg = Args.length
   for (var i = 0; i < CountOfArg; i++) {
      ArgArray[i + 1] = Args[i];
      ArgType = typeof(Args[i]);
      if (ArgType == "string") Format += "\'%s\',";
      else if (ArgType == "number") Format += "%d,";
      else Format += "%o";
   }
   Format += ')';
   ArgArray[0] = Format;
   console.group.apply(console, ArgArray);
}

LOG.endFunc = function (Name) {
   LOG.Indent = LOG.Indent.substring(0, LOG.Indent.length - 1);
   console.log('<' + Name);
}

LOG.endGroup = function (Name) {
   console.groupEnd();
}

LOG.printObject = function (Object) {
   var ObjectType = typeof(Object);

   if ('string' == ObjectType) {
      return "'" + Object + "'";
   }
   if ('boolean' == ObjectType || 'number' == ObjectType) {
      return Object.toString();
   }

   if (typeof(Object.toSource) == "function") {
      return Object.toSource();
   }

   return '[' + Object.toString() + ']';
}

LOG.formatArgsAsString = function () {
   var ArgumentString = '';

   var CountOfArg = arguments.length;
   if (CountOfArg > 0) {
      ArgumentString += '(';
      CountOfArg--;
      for (var i = 0; i < CountOfArg; i++) {
         ArgumentString += LOG.printObject(arguments[i]);
         ArgumentString += ', ';
      }
      ArgumentString += LOG.printObject(arguments[CountOfArg]);
      ArgumentString += ')';
   }
   else {
      ArgumentString += '()';
   }
   return ArgumentString;
}

LOG.traceFunction = function (FunctionSpace, Name) {
   if (typeof(LOG.FuncTable[Name]) !== 'undefined') {
      console.log("Warning! Function", Name, "already is being traced. There could be a namespace conflict.");
   }
   LOG.FuncTable[Name] = FunctionSpace[Name];
   FunctionSpace[Name] = function () {
      LOG.startFunctionLog(Name, arguments);
      try {
         LOG.FuncTable[Name].apply(this, arguments);
      }
      catch (Error) {
         console.log("Exception thrown (re-raised by log.js): ", Error);
         LOG.endFunc(Name);
         throw Error;
      }
      LOG.endFunc(Name);
   }
}

LOG.traceFunctionsMatching = function (FunctionSpace, MatchExpression) {
   var RegEx = new RegExp(MatchExpression, '');  // Assume case sensitive
   var TraceList = '';
   var IgnoreTraceList = '';
   for (i in FunctionSpace) {
      if (i === 'LOG') {
         IgnoreTraceList += " " + i;
         continue;  // Don't override LOG functions - recursive mess!
      }
      if (typeof(FunctionSpace[i]) == 'function' && RegEx.test(i)) {
         TraceList += " " + i;
         LOG.traceFunction(FunctionSpace, i);
      }
      else {
         IgnoreTraceList += " " + i;
      }
   }
   if (TraceList.length > 0) console.log("Tracing:" + TraceList);
   if (IgnoreTraceList.length > 0) console.log("Not tracing:" + IgnoreTraceList);
}

// Change behavior of the log.js library based on browser features
LOG.turnLoggingOn = function () {
   if (typeof(console) !== 'undefined') {
      if (console.log && !console.orig_log) {
         console.orig_log = console.log
         console.log = function (x) {
            console.orig_log(LOG.Indent, x);
         }
      }

      if (typeof(console.debug) === 'function') {
         try {
            console.debug('Found console.debug method so using it.');
            console.log = console.debug;
         }
         catch (Err) {
            console.log('console.debug found, but throws exception so not using it.');
            console.log(Err);
         }
      }
      if (typeof(console.group) === 'function') {
         try {
            console.group('Found console.group, so using it.');
            console.groupEnd();
            LOG.startFunctionLog = LOG.startGroup;
            LOG.endFunc = LOG.endGroup;
         }
         catch (Err) {
            console.log('console.group and console.groupEnd found, but throws exception so not using it.');
            console.log(Err);
         }
      }
   }
}

LOG.turnLoggingOff = function () {
   console.log = function () {
   };
}

LOG.activate = function (IsOn) {
   if (IsOn) LOG.turnLoggingOn();
   else LOG.turnLoggingOff();
}

if (typeof console === "undefined") {
   console = { log:function () {
   } };
}

if (typeof console.assert == "undefined") {
   console.assert = function () {
   };
}

LOG.turnLoggingOff();
