#----------------------------------------------------------------------------
# Copyright (C) 1997-2007 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: SGMchameleonSegmentObject
#
# Description:
#
# SGM based Chameleon Segment Object
#
# This is an internal class used by iNTERFACEWARE, do not modify.
#
# Revision: $Revision: 1.2 $
#----------------------------------------------------------------------------

import SGM;
import SGMchameleonFieldObject;

def assertPositive(value):
    if (value < 0):
        raise IndexError("Index cannot be negative, received '"+str(value))
    
class ChameleonSegmentObject:
    __SgmStringPool = None
    __SgmSegment = None
    
    def __init__(self, \
                 StringPool, \
                 SgmSegment\
                 ):
        self.__SgmStringPool = StringPool
        self.__SgmSegment = SgmSegment;

    def field(self,FieldIndex,SubFieldIndex = -1,SubSubFieldIndex = -1):
        global assertPositive
        assertPositive(FieldIndex)
        CountOfField = self.__SgmSegment.countOfField()
        if (FieldIndex >= CountOfField):
            raise IndexError("Field Index " + str(FieldIndex) + " must be between 0 and " + str(CountOfField-1))
        CurrentFieldObject = SGMchameleonFieldObject.ChameleonFieldObject(\
                                 self.__SgmStringPool,\
                                 self.__SgmSegment.fieldRepeats(FieldIndex),\
                                 0)
        if SubFieldIndex != -1:
            CurrentFieldObject = CurrentFieldObject.subfield(SubFieldIndex,SubSubFieldIndex)
        return CurrentFieldObject

    def count_of_field(self):
        return self.__SgmSegment.countOfField();
