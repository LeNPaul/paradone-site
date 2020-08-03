#----------------------------------------------------------------------------
# Copyright (C) 1997-2010 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: ifware.timestamp
#
# Exports the following:
#
#    make_timestamp()
#       - Return a new timestamp using the current time, in the HL7 format
#         YYYYMMDDHHMMSS.SSSS.  Subsequent calls will return unique time-
#         stamps.
#
#    is_timestamp(s)
#       - Returns true when given a valid HL7 timestamp (see parse_timestamp()
#         for details); false otherwise.
#
#    parse_timestamp(timestamp)
#       - Parses an HL7-standard formatted timestamp, and returns each
#         part in a tuple:  (year, month, day, hour, minute, second,
#         second_fraction, timezone_offset).  If the timestamp does not
#         conform to the standard, a ValueError is raised.  HL7 specifies
#         TS values as "YYYY[MM[DD[HHMM[SS[.S[S[S[S]]]]]]]][+/-ZZZZ]"; we
#         do not consider the "degree of precision" subfield.
#
# Revision: $Revision: 1.2 $
#----------------------------------------------------------------------------

import time, re
from ifware.calendar import days_in_month

__all__ = [
   'make_timestamp',
   'is_timestamp',
   'parse_timestamp',
]

def make_timestamp():
   global _last_time

   # This will ensure we don't get duplicate timestamps.
   now = time.time()
   if now - _last_time < 0.0001:
      now = _last_time + 0.0001
   _last_time = now

   fraction = int((now - int(now)) * 10000) % 10000

   prefix = time.strftime('%Y%m%d%H%M%S', time.localtime(now))
   return prefix + ('.%04d' % fraction)

def is_timestamp(s):
   try:
      parse_timestamp(s)  # Raises ValueError if s is not a timestamp.
      return 1
   except ValueError:
      return 0

def parse_timestamp(timestamp):
   global _ts_regex
   matches = _ts_regex.findall(timestamp)
   if len(matches) < 1:
      raise ValueError, 'Not a timestamp: ' + timestamp

   # findall() returns the matching strings, but we want numbers.
   (Y,M,D,h,m,s,sf,tz) = _get_numbers(matches[0])
   (has_month,has_day) = matches[0][1:3]

   # Here we validate each part of the timestamp.
   if Y < 1900 or has_month and (M < 1 or M > 12) \
               or has_day and (D < 1 or D > days_in_month(Y,M)):
      raise ValueError, 'Invalid date: ' + timestamp
   if h > 23 or m > 59 or s > 59:
      raise ValueError, 'Invalid time: ' + timestamp
   if abs(tz)/100 > 23 or abs(tz)%100 > 59:
      raise ValueError, 'Invalid timezone: ' + timestamp

   # All values are valid, so we return them.
   return (Y,M,D,h,m,s,sf,tz)


#
# Private Stuff
#

_last_time = 0

# A regular expression that matches timestamps (TS) as per the HL7 standard,
# i.e., YYYY[MM[DD[HHMM[SS[.S[S[S[S]]]]]]]][+/-ZZZZ]
_ts_regex = re.compile(
   r'^'
   r'(\d{4})'     # Year (4 digits)
   r'(?:(\d{2})'  # Month (optional)
   r'(?:(\d{2})'  # Day (optional; requires month)
   r'(?:(\d{2})'  # Hour (optional; requires day)
      r'(\d{2})'  # Minute (required after hour)
   r'(?:'
     r'(\d{2})'       # Second (optional; requires minute)
     r'(\.\d{1,4})?'  # Second-fraction (optional; requires second)
   r')?)?)?)?'
   r'([+-]\d{4})?'  # Timezone (optional)
   r'$'
)

def _get_numbers(alist):
   result = []
   for x in alist:
      try:
         if x.startswith('.'): result.append(float(x))
         else:                 result.append(int(x))
      except ValueError:
         result.append(0)
   return result
