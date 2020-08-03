#----------------------------------------------------------------------------
# Copyright (C) 1997-2010 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: ifware.tracing
#
# Description:
#
# Basic Function-call Tracing and Profiling
#
# Revision: $Revision: 1.5 $
#----------------------------------------------------------------------------

import re, sys, time
from types import FunctionType

__all__ = [
   'start_tracing', 'stop_tracing',
   'trace', 'trace_all',
   'no_trace', 'trace_none',
   'trace_print',
   'trace_history', 'clear_trace_history',
]

def start_tracing(quiet=0):
   global _tracing
   sys.settrace(_do_trace)
   _tracing = not quiet

def stop_tracing():
   global _tracing, _stack
   sys.settrace(None)
   _tracing = 0
   _stack = []

def trace(*functions):
   global _watched
   for f in functions:
      assert type(f) is FunctionType
      _watched[f.func_code] = 1

def trace_all(module=None):
   if module:  objects = module.__dict__.itervalues()
   else:       objects = sys._getframe(1).f_globals.itervalues()
   this_module = dict([(x,None)
      for x in sys._getframe().f_globals.itervalues()
      if type(x) is FunctionType])
   trace(*[x for x in objects
      if type(x) is FunctionType and x not in this_module])

def no_trace(*functions):
   global _watched
   for f in functions:  del _watched[f.func_code]

def trace_none():
   global _watched
   _watched = {}

def trace_print(text):
   if _tracing:
      _print_trace(text, sys._getframe(1))

def trace_history(count=10):
   if count: print '%d items in history; printing at most %d.' % (len(_history), count)
   else:     print '%d items in history.' % (len(_history))
   if not len(_history):
      return
   def name_of(x):
      if x.co_filename == '<stdout>':  return x.co_name
      else:  return '%s (%s)' % (x.co_name, _filename(x.co_filename))
   all = [(t, t-c, c, n, name_of(x)) for x,(n,t,c) in _history.iteritems()]
   all.sort()
   total = sum([parent_time for (_,parent_time,_,_,_) in all], 0.000001)
   width = max(14, *[len(name) for (_,_,_,_,name) in all[-count:]])
   print '%-*s | %-15s | %-15s | %-7s' % \
      (width, '== Function ==', 'Time w/Children', 'Time Alone', '#Calls')
   for call_time, parent_time, child_time, calls, name in all[-count:]:
      print '%-*s | %7.2fs %5.1f%% | %7.2fs %5.1f%% | %7d' % \
         (width, name, call_time, 100*call_time/total, parent_time, 100*parent_time/total, calls)

def clear_trace_history():
   global _history
   _history = {}


# Private Stuff

_tracing = 0
_watched = {}  # [ code: 1, ... ]
_stack = []    # [ (code, start_time), ... ]
_history = {}  # { code: (num_calls, call_time, child_time), ... }
_path_re = re.compile(r'^.*[/\\]')

# There's no sum() in Python 2.2.3, but we might upgrade.
try:
   assert sum([1,2],3) == 6
except:
   def sum(sequence, start=0):
      for x in sequence:  start += x
      return start
   sys._getframe(0).f_globals['sum'] = sum

def _filename(path):
   global _path_re
   return _path_re.sub('', path)

def _print_trace(text, frame):
   print '%-20s | %s%s' % (
      '%s:%d' % (_filename(frame.f_code.co_filename), frame.f_lineno),
      '  ' * len(_stack),
      text)

def _format_args(frame):
   args = []
   for k,v in frame.f_locals.items():
      args.append('%s=%s' % (k, repr(v)))
   return ', '.join(args)

def _do_trace(frame, event, arg):
   global _watched, _stack, _history
   code = frame.f_code
   if event == 'call' and code in _watched:
      if _tracing:
         _print_trace('%s(%s)' % (code.co_name, _format_args(frame)), frame.f_back)
      _stack.append((code, time.time()))
   elif event in ['return', 'exception'] and len(_stack) and code == _stack[-1][0]:
      elapsed = time.time() - _stack.pop()[1]
      (calls, total, child) = _history.get(code, (0,0,0))
      _history[code] = (calls + 1, total + elapsed, child)
      if len(_stack):
         parent = _stack[-1][0]
         (calls, total, child) = _history.get(parent, (0,0,0))
         _history[parent] = (calls, total, child + elapsed)
      if _tracing:
         if event == 'return':
            _print_trace('%s => %s' % (code.co_name, repr(arg)), frame.f_back)
         else:  # 'exception'
            _print_trace('%s FAILED %s' % (code.co_name, repr(arg[1])), frame.f_back)
   return _do_trace


# Testing

if __name__ == '__main__':
   def test2(x):
      trace_print('Hello, world!')
      return x * 2
   def test1(x):
      y = test2(x*3)
      return x / (y - 42)
   def test():
      try:
         print test1(7)
         print 'Failed.'
      except:
         print 'Okay.'
   trace(test, test1, test2)
   start_tracing()
   print '*** Tracing ON ***'
   test()
   no_trace(test)
   print '*** no_trace(test) ***'
   test()
   stop_tracing()
   print '*** Tracing OFF ***'
   test()
   trace_history()
