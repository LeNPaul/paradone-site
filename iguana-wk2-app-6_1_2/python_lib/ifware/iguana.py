#----------------------------------------------------------------------------
# Copyright (C) 1997-2008 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: ifware.iguana
#
# Description:
#
# Facilitates interaction with Iguana.
#
# Revision: $Revision: 1.8 $
#----------------------------------------------------------------------------

# vim: sts=3 ts=3 sw=3 et

__all__ = [
   'message',
   'enqueue_message',
   'channel_name',
   'vmd_path'
]

import _ifware_iguana

def _api(query, arg=None):
   try:
      response = _ifware_iguana.query(query, arg)
      if response is None:
         error = _ifware_iguana.query('last_error', None)
         raise RuntimeError, error
      else:
         return response
   except NotImplementedError:
      raise NotImplementedError, 'Not connected to Iguana.'

class Message:
   def __getattr__(self, attr):
      try:
         return _api('message', attr)
      except NotImplementedError:
         if attr == 'log_id':
            return '[MessageLogID]'
         else:
            raise

message = Message()

# The following function will put the message in a given channel's queue.
# If no channel name is given, it will put it in the queue of the current channel.
def enqueue_message(messageString, channelName=''):
  try:
    response = _api ('enqueue_message', channelName + '\n' + messageString)
  except NotImplementedError:
    response = '[enqueue_message: Would have enqueued the message to '
    if (channelName == ''):
        response = response + 'the current channel]'
    else:
        response = response + 'channel "' + channelName + '"]'
  return response
  
#
# Gets the current channel name 
def channel_name():
  try:
    response = _api ('channel_name', '')
  except NotImplementedError:
    response = '[channel_name]'
  return response

#
# Gets the full path of the current VMD file
def vmd_path():
  try:
    response = _api ('vmd_path', '')
  except NotImplementedError:
    response = '[vmd_path]'
  return response
