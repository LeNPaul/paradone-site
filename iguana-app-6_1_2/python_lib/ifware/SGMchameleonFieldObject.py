#----------------------------------------------------------------------------
# Copyright (C) 1997-2007 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: SGMchameleonFieldObject
#
# Description:
#
# SGM based Chameleon Field Object
#
# This is an internal class used by iNTERFACEWARE, do not modify.
#
# Revision: $Revision: 1.9 $
#----------------------------------------------------------------------------

import SGM;
import internal_util;

def assertPositive(value):
    if (value < 0):
        raise IndexError("Index cannot be negative, received '"+str(value))

def assertNotReadOnly(read_only,obj_name):
    if read_only:
        raise NotImplementedError(obj_name +" object cannot be modified.")

_value_key = 'value'

class ChameleonSubSubFieldObject:
    __SgmStringPool = None
    __SgmValue = 0
    __ReadOnly = 0
    
    def __init__(self, \
                 StringPool, \
                 Value,\
                 ReadOnly\
                 ):
        self.__SgmStringPool = StringPool
        self.__SgmValue = Value;
        self.__ReadOnly = ReadOnly

    def clear(self):
        assertNotReadOnly(self.__ReadOnly, 'Sub sub field')
        setattr(self,_value_key, '')

    def count_of_repeat(self):
        return 1

    def count_of_subfield(self,RepeatFieldIndex = 0):
        if (RepeatFieldIndex != 0):
            raise IndexError("Sub Sub field can only have 1 repeat (itself).")
        return 0

    def subfield(self,SubFieldIndex,SubSubFieldIndex = -1):
        raise IndexError("Sub Sub Field does not have any sub fields")

    def __getattr__(self,name):
        if name == _value_key:
            return self.__SgmValue.data()
        elif name == 'sub':
            return self
        #else
        internal_util.raise_attribute_error(str(name), [_value_key, 'sub'])

    def __setattr__(self,name,value):
        if name == _value_key:
            assertNotReadOnly(self.__ReadOnly, 'Sub sub field')
            return SGM.SGMstringPoolSetValue(self.__SgmStringPool,self.__SgmValue,value)
        else:
            self.__dict__[name] = value
            
    def is_null(self):
        return self.__SgmValue.size() == 0

    def repeat_field(self,RepeatIndex):
        global assertPositive
        assertPositive(RepeatIndex)
        if RepeatIndex == 0:
            return self
        if RepeatIndex >= self.count_of_repeat():
           raise IndexError("Sub Sub field can only have 1 repeat (itself).")
       
    def set_count_of_subfield( self, SubFieldCount ):
        raise IndexError("Sub Sub Field does not have any sub fields")

    def set_count_of_repeat( self, CountOfRepeat ):
        global assertPositive
        assertPositive(CountOfRepeat)
        if CountOfRepeat != 1:
           raise IndexError("Sub Sub field can only have 1 repeat (itself).")           

class ChameleonSubFieldObject:
    __SgmStringPool = None
    __SgmSubField = None;
    __ValueKeyName = ''
    __ReadOnly = 0
    
    def __init__(self, \
                 StringPool, \
                 SubField,\
                 ReadOnly\
                 ):
        self.__ReadOnly = ReadOnly
        self.__SgmStringPool = StringPool
        self.__SgmSubField = SubField;

    def clear(self):
        assertNotReadOnly(self.__ReadOnly, 'Sub field')
        self.set_count_of_subfield(0)

    def count_of_repeat(self):
        return 1

    def count_of_subfield(self,RepeatFieldIndex = 0):
        if (RepeatFieldIndex != 0):
            raise IndexError("Sub field can only have 1 repeat (itself).")           
        return self.__SgmSubField.countOfSubSubField();

    def subfield(self,SubFieldIndex,SubSubFieldIndex = -1):
        global assertPositive
        assertPositive(SubFieldIndex)
        if SubFieldIndex >= self.count_of_subfield():
            assertNotReadOnly(self.__ReadOnly, 'Sub field')
            self.set_count_of_subfield(SubFieldIndex+1);
        SubSubFieldObj = ChameleonSubSubFieldObject(self.__SgmStringPool,\
                                                    self.__SgmSubField.value(SubFieldIndex),\
                                                    self.__ReadOnly\
                                                    )
        if SubSubFieldIndex != -1:
            return SubSubFieldObj.subfield(SubSubFieldIndex)
        return SubSubFieldObj

    def __getattr__(self,name):
        if name == _value_key:
            return getattr(self.subfield(0),_value_key)
        elif name == 'sub':
            return self
        #else
        internal_util.raise_attribute_error(str(name), [_value_key, 'sub'])

    def __setattr__(self,name,value):
        if name == _value_key:
            assertNotReadOnly(self.__ReadOnly, 'Sub field')
            setattr(self.subfield(0),_value_key, value)
        else:
            self.__dict__[name] = value
            
    def is_null(self):
        if self.count_of_subfield() == 0:
            return 1
        else:
            return 0

    def repeat_field(self,RepeatIndex):
        global assertPositive
        assertPositive(RepeatIndex)
        if RepeatIndex == 0:
            return self
        if RepeatIndex >= self.count_of_repeat():
           raise IndexError("Sub field can only have 1 repeat (itself).")           

    def set_count_of_subfield( self, SubFieldCount ):
        global assertPositive
        assertPositive(SubFieldCount)
        assertNotReadOnly(self.__ReadOnly, 'Sub field')
        self.__SgmSubField.setCountOfSubSubField(SubFieldCount);

    def set_count_of_repeat( self, CountOfRepeat ):
        global assertPositive
        assertPositive(CountOfRepeat)
        assertNotReadOnly(self.__ReadOnly, 'Sub field')
        if CountOfRepeat != 1:
           raise IndexError("Sub field can only have 1 repeat (itself).")

class ChameleonFieldObject:
    __SgmStringPool = None
    __SgmRepeats = None;
    __SgmField = None;
    __RepeatIndex = 0;
    __ValueKeyName = ''
    __ReadOnly = 0
    
    def __init__(self, \
                 StringPool, \
                 Repeats, \
                 RepeatIndex, \
                 ReadOnly = 0\
                 ):
        self.__SgmStringPool = StringPool
        self.__RepeatIndex = RepeatIndex;
        self.__SgmRepeats = Repeats;
        self.__SgmField = self.__SgmRepeats.fieldRepeat(RepeatIndex);
        self.__ReadOnly = ReadOnly

    def clear(self):
        assertNotReadOnly(self.__ReadOnly, 'Field')
        self.set_count_of_subfield(0)

    def count_of_repeat(self):
        return self.__SgmRepeats.countOfFieldRepeat();

    def count_of_subfield(self,RepeatFieldIndex = -1):
        if (RepeatFieldIndex != -1):
            return self.repeat_field(RepeatFieldIndex).count_of_subfield()
        return self.__SgmField.countOfSubField();

    def subfield(self,SubFieldIndex,SubSubFieldIndex=-1):
        global assertPositive
        assertPositive(SubFieldIndex)
        if SubFieldIndex >= self.count_of_subfield():
            assertNotReadOnly(self.__ReadOnly, 'Field')
            self.set_count_of_subfield(SubFieldIndex+1);
        SubFieldObj= ChameleonSubFieldObject(self.__SgmStringPool,\
                                             self.__SgmField.subField(SubFieldIndex),\
                                             self.__ReadOnly\
                                             )
        if (SubSubFieldIndex != -1):
            return SubFieldObj.subfield(SubSubFieldIndex)
        return SubFieldObj

    def __getattr__(self,name):
        if name == _value_key:
            return getattr(self.subfield(0),_value_key)
        elif name == 'sub':
            return self
        #else
        internal_util.raise_attribute_error(str(name), [_value_key, 'sub'])

    def __setattr__(self,name,value):
        if name == _value_key:
            assertNotReadOnly(self.__ReadOnly, 'Field')
            setattr(self.subfield(0),_value_key, value)
        else:
            self.__dict__[name] = value
            
    def is_null(self):
        if self.count_of_subfield() == 0:
            return 1
        else:
            return 0

    def repeat_field(self,RepeatIndex):
        global assertPositive
        assertPositive(RepeatIndex)
        if RepeatIndex >= self.count_of_repeat():
            assertNotReadOnly(self.__ReadOnly, 'Field')
            self.set_count_of_repeat( RepeatIndex+1)
        return ChameleonFieldObject(self.__SgmStringPool,\
                                    self.__SgmRepeats,\
                                    RepeatIndex,\
                                    self.__ReadOnly\
                                    );

    def set_count_of_subfield( self, SubFieldCount ):
        global assertPositive
        assertPositive(SubFieldCount)
        assertNotReadOnly(self.__ReadOnly, 'Field')
        self.__SgmField.setCountOfSubField(SubFieldCount);

    def set_count_of_repeat( self, CountOfRepeat ):
        global assertPositive
        assertPositive(CountOfRepeat)
        assertNotReadOnly(self.__ReadOnly, 'Field')
        self.__SgmRepeats.setCountOfFieldRepeat(CountOfRepeat)
        
    
