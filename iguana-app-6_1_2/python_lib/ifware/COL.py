#----------------------------------------------------------------------------
# Copyright (C) 1997-2007 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: COL
#
# Description:
#
# Wrapper classes for the COL library
# This is an internal class used by iNTERFACEWARE, do not modify.
#
# Revision: $Revision: 1.1 $
#----------------------------------------------------------------------------

import sgpy;

class COLerror:
    __Handle = 0;

    def __init__(self, Handle):
        self.__Handle = Handle
        sgpy.COLerrorAddRef(self.__Handle)

    def __del__(self):
        sgpy.COLerrorRelease(self.__Handle)
        self.__Handle = 0

    def handle(self):
        return self.__Handle;

    def description(self):
        return sgpy.COLerrorGetDescription(self.__Handle)

    def setDescription(self,Description):
        sgpy.COLerrorSetDescription(self.__Handle,Description)

    def countOfParameter(self):
        return sgpy.COLerrorGetCountOfParameter(self.__Handle)

    def hasParameter(self,Key):
        return sgpy.COLerrorGetHasParameter(self.__Handle,Key)

    def parameter(self,Key):
        return sgpy.COLerrorGetParameter(self.__Handle,Key)

    def setParameter(self,Key,Value):
        sgpy.COLerrorSetParameter(self.__Handle,Key,Value)

    def key(self,ParameterIndex):
        return sgpy.COLerrorGetKey(self.__Handle,ParameterIndex)

def COLerrorCreate():
    Handle = sgpy.COLerrorCreate()
    return COLerror(Handle)

