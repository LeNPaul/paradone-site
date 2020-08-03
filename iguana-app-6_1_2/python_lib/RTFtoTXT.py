"""
   This work is derived from PyRtfLib, an RTF conversion library written
   by Loic Fejoz and released under the Lesser GNU Public License (LGPL).
   This modified version only converts RTF to plain text.

   Copyright (C) 2005-2008 iNTERFACEWARE Inc. <info@interfaceware.com>
   Copyright (C) 2004 Loic Fejoz <loic@fejoz.net>

   This library is free software; you can redistribute it and/or modify it
   under the terms of the GNU Lesser General Public License as published by
   the Free Software Foundation; either version 2.1 of the License, or (at
   your option) any later version.

   This library is distributed in the hope that it will be useful, but WITHOUT
   ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
   FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
   License for more details.

   You should have received a copy of the GNU Lesser General Public License
   along with this library; if not, write to the Free Software Foundation,
   Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA   
"""

import sys

class RtfException(Exception):
    pass

plaintext = 1
control = 2
argument = 3
backslash = 4
escapedChar = 5

class RtfParser(object):

    def __init__(self,unicode=False):
        self.state = plaintext
        self.arg = ''
        self.token = ''
        self.unicode = unicode

    def getChar(self,code):
        """ called when an escaped char is found """
        return chr(code)

    def getNonBreakingSpace(self):
        return " "

    def pushState(self):
        pass

    def popState(self):
        pass

    def putChar(self):
        pass

    def doControl(self,token,arg):
        pass

    def feed(self,txt):
        for c in txt:
            self.feedChar(c)

    def feedChar(self,char):
        if self.state == plaintext: #this is just normal user content
            if char == '\\':
                self.state = backslash
            elif char == '{':
                self.pushState()
            elif char == '}':
                self.popState()
            else:
                self.putChar(char)
        elif self.state == backslash: #something special like a command or escape
            if char == '\\' or char == '{' or char == '}':
                self.putChar(char)
                self.state = plaintext
            else:
                if char.isalpha() or char in ('*','-','|'):
                    self.state = control
                    self.token = char
                elif char == "'":
                    self.state = escapedChar
                    self.escapedChar = ''
                elif char in ['\\','{','}']:
                    self.putChar(char)
                    self.state = plaintext
                elif char == "~": #non breking space
                    self.putChar(self.getNonBreakingSpace())
                    self.state = plaintext
                else:
                    raise RtfException,'unexpected %s after \\' % char
        elif self.state == escapedChar:
            self.escapedChar = self.escapedChar + char
            if len(self.escapedChar) == 2:
		try: # sometimes there are invalid chars - this replaces with ? SK 20060303
                  char = self.getChar(int(self.escapedChar,16))
		except:
		  char = "?"
                self.putChar(char)
                self.state = plaintext
        elif self.state == control: #collecting the command token
            if char.isalpha():
                self.token = self.token + char
            elif char.isdigit() or char== '-':
                self.state = argument
                self.arg = char
            else:
                self.doControl(self.token,self.arg)
                self.state = plaintext
                if char == '\\':
                    self.state = backslash
                elif char == '{':
                    self.pushState()
                elif char == '}':
                    self.popState()
                else:
                    if not char.isspace():
                        self.putChar(char)
        elif self.state == argument: #collecting the optional command argument
            if char.isdigit():
                self.arg = self.arg + char
            else:
                self.state = plaintext
                self.doControl(self.token,self.arg)
                if char == '\\':
                    self.state = backslash
                elif char == '{':
                    self.pushState()
                elif char == '}':
                    self.popState()
                else:
                    if not char.isspace():
                        self.putChar(char)

outputString = ""

class Destination:
    def __init__(self,foutput,parser):
        self.foutput = foutput
        self.name = 'Destination'
        self.parser = parser
        self.ansicpg = None

    def __repr__(self):
        return self.name

    def putChar(self,str):
        pass

    def doControl(self,token,arg):
        pass

    def pushState(self,list):
        list.append(self)

    def popState(self,list):
        list.pop()

    def close(self):
        pass

class RtfDestination(Destination):
    def __init__(self, foutput, parser, fontTable=None, colorTable=None):
        Destination.__init__(self, foutput, parser)
        self.name = 'Rtf'
        self.fontCounter = 0
        self.styles = ['']
        self.italic = False
        if fontTable:
            self.fontTable = fontTable
        else:
            self.fontTable = FontTableDestination(self.foutput,self.parser)
        if colorTable:
            self.colorTable = colorTable
        else:
            self.colorTable = ColorTableDestination(self.foutput,self.parser)
        self.characterSet = None
        self.ansicpg = None
        self.reset()

    def reset(self):
        self.close()

    def doControl(self,token,arg):
        if token in ['*','stylesheet','info']:
            #skip everything
            self.parser.setDest(Destination(self.foutput,self.parser))
        elif token == 'rtf':
            pass
        elif token in ['ansi','mac','pc','pca']:
            self.characterSet = token
            if token == 'pc':
                self.ansicpg = '437'
            elif token == 'pca':
                self.ansicpg = '850'
        elif token == 'ansicpg':
            self.ansicpg = arg
        elif token == 'fonttbl':
            self.parser.setDest(self.fontTable)
        elif token == 'colortbl':
            self.parser.setDest(self.colorTable)
        elif token == 'par':
            self.foutput.write('\n')
        elif token in  ('b','i','strike','ql','qr','qj','qc'):
            #bold italic strike
            #open = self.tags[token][0]
            #close = self.tags[token][1]
            #self.treatIt(token,arg,open,close)
            pass
        elif token == 'ul':
            #underline
            self.styles.append('ul')
        elif token == 'ulnone':
            self.styles.pop()
        elif token == 'fs':
            #font size
            if self.styles and self.styles[-1] == 'fs':
                self.styles.pop()
            size = int(arg) / 2
            size = size - 9
            if size >= 0:
                size = '+' + str(size)
            else:
                size = str(size)
            self.styles.append('fs')
        elif token == 'f':
            #font
            font = self.fontTable.getFont(int(arg))
        elif token == 'cf':
            #foreground color
            color = self.colorTable.getColor(int(arg) - 1)
            if self.styles and self.styles[-1] == 'cf':
                self.styles.pop()
            self.styles.append('cf')
        elif token in  ['pard','plain']:
            self.reset()
        else:
            #skip unknown tag
            #print token, arg
            pass

    def treatIt(self,token,arg,open,close):
        toggle = getattr(self,token)
        if toggle:
            self.foutput.write(close)
            self.styles.pop()
        else:
            self.foutput.write(open)
            self.styles.append(token)
        setattr(self,token,not toggle)

    def putChar(self,str):
        str.replace('\r','\n')
        self.foutput.write(str)

    def close(self):
        #close all pending types
        foutput = self.foutput
        styles = self.styles
        styles.reverse()
        self.styles = ['']

    def pushState(self,list):
        newRtf = RtfDestination(self.foutput, self.parser, self.fontTable, self.colorTable)
        list.append(newRtf)

    def popState(self,list):
        self.close()
        list.pop()

class Font:
    def __init__(self):
        self.name = ''

    def getStyle(self):
        return ''

class FontTableDestination(Destination):
    def __init__(self,foutput,parser):
        Destination.__init__(self,foutput,parser)
        self.fontTable = []
        self.name = 'FontTable'

    def getFont(self,index):
        if len(self.fontTable) > index:
            return self.fontTable[index]
        else:
             return self.fontTable[0]

    def putChar(self,str):
        font = self.fontTable[-1]
        font.name = font.name + str

    def doControl(self,token,arg):
        if token == 'f':
            self.fontTable.append(Font())
        elif  token in ['fnil','froman','fswiss','fmodern','fscript','fdecor','ftech','fbidi']:
            self.fontTable[-1].family = token
        elif token == 'fcharset':
            self.fontTable[-1].charset = arg
        else:
            font = self.fontTable[-1]
            setattr(font,token,arg)

class Color:
    def __init__(self):
        self.red = 0
        self.green = 0
        self.blue = 0

    def __str__(self):
        r = hex(self.red)[2:]
        if len(r) == 1:
            r = '0' + r
        g = hex(self.green)[2:]
        if len(g) == 1:
            g = '0' + g
        b = hex(self.blue)[2:]
        if len(b) == 1:
            b = '0' + b
        return '#%s%s%s' % (r,g,b)

    def __repr__(self):
        return '%i %i %i' % (self.red,self.green,self.blue)

class ColorTableDestination(Destination):
    def __init__(self,foutput,parser):
        Destination.__init__(self,foutput,parser)
        self.name = 'ColorTable'
        self.colorTable = []

    def getColor(self,index):
        return self.colorTable[index]

    def putChar(self,str):
        if str == ';':
            self.colorTable.append(Color())

    def doControl(self,token,arg):
        if len(self.colorTable) == 0:
            self.colorTable.append(Color())
        color = self.colorTable[-1]
        setattr(color,token,int(arg))

class Rtf2Txt(RtfParser):
    def __init__(self,foutput):
        RtfParser.__init__(self)
        self.foutput = foutput
        self.destinations = [RtfDestination(foutput,self)]
        self._ansicpg = 'latin_1'

    def setAnsiCpg(self,codePage):
        self._ansicpg = codePage or self._ansicpg

    def getAnsiCpg(self):
        try:
            return "cp%d" % int(self._ansicpg)
        except:
            return self._ansicpg

    ansicpg = property(getAnsiCpg,setAnsiCpg,doc='the code page used')

    def getChar(self,code):
        return unicode(chr(code),self.ansicpg)

    def append(self,arg):
        self.destinations.append(arg)

    def pop(self):
        return self.destinations.pop()

    def setDest(self,dest):
        self.destinations[-1] = dest
        self.ansicpg = dest.ansicpg

    def pushState(self):
        dest = self.destinations[-1]
        dest.pushState(self)
        self.ansicpg = dest.ansicpg

    def popState(self):
        dest = self.destinations[-1]
        dest.popState(self)

    def putChar(self,ch):
        dest = self.destinations[-1]
        if not (ch=='\n' or ch=='\r'):
            dest.putChar(ch)

    def doControl(self,token,arg):
        dest = self.destinations[-1]
        dest.doControl(token,arg)

    def close(self):
        for dest in self.destinations:
            dest.close()


def getTxtFromRtf(rtf):
    """ get the Text from a string that contain Rtf """
    import StringIO
    rtf = rtf.replace("\\0x0A\\","\n")
    rtf = rtf.replace("\\0x0D\\","\r")
    s = StringIO.StringIO()
    parser = Rtf2Txt(s)
    parser.feed(rtf)
    parser.close()
    tempString = s.getvalue()
    tempString = tempString.replace("\n","\\0x0A\\")
    tempString = tempString.replace("\r","\\0x0D\\")
    return tempString
