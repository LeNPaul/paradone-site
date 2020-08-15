#----------------------------------------------------------------------------
# Copyright (C) 1997-2009 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: ifware.calendar
#
# Exports the following:
#
#    age(birthdate, [today])
#       - Given the birthdate of someone, "YYYYMMDD", this will return
#         how old that person is as of today, as (years, months, days).
#         You may optionally specify an alternative to today's date.
#
#    leapyear(year)
#       - Returns true if year, an integer, is a leapyear; false otherwise.
#
#    days_in_month(year, month)
#       - Returns the number of days in a given month, with 29 days
#         in Februrary for leapyears.
#
# Revision: $Revision: 1.5 $
#----------------------------------------------------------------------------


import time

__all__ = [
   'age',
   'days_in_month',
   'leapyear',
]

def leapyear(year):
   """Returns true only when year, an integer, is a leapyear.

   >>> [leapyear(x) for x in [2000, 2001, 2004, 2100]]
   [True, False, True, False]
   """
   return (year % 4 == 0) and (year % 100 != 0 or year % 400 == 0)

def days_in_month(year, month):
   """Returns the number of days in a given month, with 29 days in
   Februrary for leapyears.

   >>> days_in_month(2000, 2)
   29
   >>> days_in_month(2001, 2)
   28
   """
   base = { 1:31, 2:28, 3:31,  4:30,  5:31,  6:30,
            7:31, 8:31, 9:30, 10:31, 11:30, 12:31 } [month]
   if month == 2 and leapyear(year):
      return base + 1
   else:
      return base

def split_date(date):
   if len(date) < 8 or not date[0:8].isdigit():
      raise ValueError, "Invalid date: " + date
   (year, month, day) = int(date[0:4]), int(date[4:6]), int(date[6:8])
   if month < 1 or month > 12:
      raise ValueError, "Date has invalid month: " + date
   elif day < 1 or day > days_in_month(year, month):
      raise ValueError, "Date has invalid day: " + date
   return (year, month, day)
    
def age(birthdate, today = None):
   """Given the birthdate of someone, "YYYYMMDD", this will return
   how old that person is as of today, as (years, months, days).
   You may optionally specify an alternative to today's date.

   >>> age('20070215', '20080215')  # One calendar year.
   (1, 0, 0)
   >>> age('20070228', '20080229')  # A year and a leapday.
   (1, 0, 1)
   >>> age('20080229', '20090301')  # A year and a day after a leapday.
   (1, 0, 1)
   >>> age('20070131', '20080228')  # A year and almost a month.
   (1, 0, 28)
   >>> age('20070130', '20080229')  # A year and a month (leapyear).
   (1, 1, 0)
   >>> age('20070227', '20080301')  # Three days after first birthday.
   (1, 0, 3)

   >>> age('20080228', '20080229')  # Born just before leapday.
   (0, 0, 1)
   >>> age('20080228', '20080428')  # Two months later.
   (0, 2, 0)
   >>> age('20080228', '20090227')  # One day before first birthday.
   (0, 11, 30)
   >>> age('20080228', '20091128')  # Three months before second birthday.
   (1, 9, 0)
   >>> age('20080228', '20100301')  # One day after second birthday.
   (2, 0, 1)
   """
   if not today:
      today = time.strftime("%Y%m%d")
   if today < birthdate:
      return age(today, birthdate)

   (from_year, from_month, from_day) = split_date(birthdate)
   (  to_year,   to_month,   to_day) = split_date(today)

   (years, months, days) = (0,0,0)
   
   if from_year < to_year:
      years  = to_year  - from_year  - 1
      months = to_month - from_month + 11  # (12-from_month) + (to_month-1)
   elif from_month == to_month:
      return (0, 0, to_day-from_day)
   else:
      months = to_month - from_month - 1

   if from_day <= to_day:
      months += 1
      days    = to_day - from_day
   elif to_day == days_in_month(to_year, to_month):
      months += 1
   else:
      days_last_month = 31  # Days in December.
      if to_month > 1:
         days_last_month = days_in_month(to_year, to_month-1)
      if from_day <= days_last_month:
         days = days_last_month-from_day + to_day
      else:
         days = to_day

   if months >= 12:
      years  += 1
      months -= 12

   return (years, months, days)

# Run this file through the Python interpreter directly, and it
# will execute all embedded unit tests:  python calendar.py -v
#
if __name__ == "__main__":
   import doctest
   doctest.testmod()
