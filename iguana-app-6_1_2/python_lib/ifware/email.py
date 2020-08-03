#----------------------------------------------------------------------------
# Copyright (C) 1997-2007 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: ifware.email
#
# Description:
#
# A simple module for sending email.
#
# Revision: $Revision: 1.3 $
#----------------------------------------------------------------------------

# vim: sts=3 ts=3 sw=3 et

__all__ = [
   'send_mail',
]

import re
import time
import smtplib
import socket

class Error(Exception):
   '''Base error type and also thrown for other SMTP errors.'''
   pass
   
class ConnectError(Error):
   '''Failed to connect to the SMTP server.'''
   pass
   
class AuthenticationError(Error):
   '''Invalid username or password.'''
   pass
   
class SenderRefused(Error):
   '''The specified sender was not accepted by the server.'''
   pass
   
class RecipientsRefused(Error):
   '''Failed to send the message to one or more recipients.'''
   pass

def send_mail(server, sender, to, subject, message,
      reply_to='', cc=[], bcc=[],
      user='', password='', port=0,
      extra_headers=[],
      ):

   '''Send a simple email.
   
   To send an email, you must supply the following:
   
   server  - The name of your SMTP server.  The port can also be specified
             after a colon, e.g., 'mail.megacorp.com:1234'.
   sender  - The email address that the email should appear to be 'From:'.
   to      - The list of recipients that the email will be sent 'To:'.
   subject - The subject of the email.
   message - The main body of the email.

   Other arguments are listed next.  Exceptions are thrown for complete
   failures, but if the email is successfully sent to at least one recipient,
   a dict is returned with any errors (i.e., {address:error_value, ...}).
   The format of 'error_value' is not final, but may be helpful for debugging.

   [ Optional Arguments ]

   reply_to - The address replies should be sent (Reply-To:).
   cc       - The list of recipients who should be CC'd on this email.
   bcc      - The list of recipients who should get a copy of this email,
              but their addresses should not appear in it.

   [ Optional Arguments for SMTP Server Authentication ]
   
   user     - The username to login to the SMTP server.
   password - The password.
   
   [ Other Optional Arguments ]
   
   port          - An alternative way of specifying the SMTP port.
   extra_headers - This is an advanced option.  If you want to specify
                   any other email headers, list each line here.
   
   [ An Example ]
   
   >>> send_mail(
   ...    server='smtp.megacorp.com',
   ...    sender='Western Hospital - Server #6',
   ...    to=['me@megacorp.com'],
   ...    subject='Iguana Log Event',
   ...    message='Something bad happened!')
   {}
   '''

   assert type(server) is str
   assert type(sender) is str
   assert type(to) is list
   assert type(subject) is str
   assert type(message) in (str, unicode)
   assert type(reply_to) is str
   assert type(cc) is list
   assert type(bcc) is list
   assert type(user) is str
   assert type(password) is str
   assert type(port) is int
   assert type(extra_headers) is list
   
   if extra_headers:
      extra_headers = '\r\n'.join(extra_headers) + '\r\n'
   else:
      extra_headers = ''

   def header(name, contents):
      return contents and '%s: %s\r\n' % (name, contents) or ''

   now = time.strftime('%a, %d %b %Y %H:%M:%S +0000', time.gmtime())
   headers = (
      header('Date', now) +
      header('From', sender) +
      header('Reply-To', reply_to) +
      header('To', ', '.join(to)) +
      header('CC', ', '.join(cc)) +
      header('Subject', subject) +
      header('User-Agent', 'iNTERFACEWARE Chameleon/Iguana') +
      extra_headers + '\r\n')

   try:
      smtp = smtplib.SMTP(server, port)
      try:
         if user:
            smtp.login(user, password)
         return smtp.sendmail(sender, to + cc + bcc, headers + message)
      finally:
         smtp.quit()
   except socket.error, reason:
      raise ConnectError, reason
   except smtplib.SMTPAuthenticationError, reason:
      raise AuthenticationError, reason
   except smtplib.SMTPRecipientsRefused, reasons:
      raise RecipientsRefused, reasons
   except smtplib.SMTPSenderRefused, reason:
      raise SenderRefused, reason
   except smtplib.SMTPError, reason:
      raise Error, reason
