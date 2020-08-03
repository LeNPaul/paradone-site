#----------------------------------------------------------------------------
# Copyright (C) 1997-2007 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: COLchameleonException
#
# Description:
#
# COL based Chameleon Exception
#
# This is an internal class used by iNTERFACEWARE, do not modify.
#
# Revision: $Revision: 1.2 $
#----------------------------------------------------------------------------

import COL;

class ChameleonException(Exception):
    __ColError = None

    def __init__(self,Description):
        self.__ColError = COL.COLerrorCreate()
        self.__ColError.setDescription(Description)

    def __str__(self):
        return self.__ColError.description()

    def parameter(self,Key):
        return self.__ColError.parameter(Key)

    def set_parameter(self,Key,Value):
        return self.__ColError.setParameter(Key,Value)


    def __getattr__(self,name):
        if name == 'description':
            return self.__ColError.description()
        #else
        raise AttributeError

    def __setattr__(self,name,value):
        if name == 'description':
            self.__ColError.setDescription(value)
        else:
            self.__dict__[name] = value        

    def colErrorHandle(self):
        return self.__ColError.handle()

#this function creates an exception that can be thrown, which contains an underlying exception
#object. Should probably be avoided, but to get old code to work we have to leave it here
def CreateLegacyExceptionObject():
    ReturnException = Exception()
    ReturnException.errorobject = ChameleonException('')
    return ReturnException

def CheckTypeChameleonException(E):
    if isinstance(E,ChameleonException):
        return 1
    else:
        return 0
