#----------------------------------------------------------------------------
# Copyright (C) 1997-2007 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: SGM
#
# Description:
#
# Wrapper classes for the SGM library
# This is an internal class used by iNTERFACEWARE, do not modify.
#
# Revision: $Revision: 1.5 $
#----------------------------------------------------------------------------

import sgpy;

class SGMsegmentList:
   __Handle = 0;
   
   def __init__(self, Handle):
       self.__Handle = Handle
       sgpy.SGMsegmentListAddRef(self.__Handle)

   def __del__(self):
       sgpy.SGMsegmentListRelease(self.__Handle)
       self.__Handle = 0;

   def handle(self):
      return self.__Handle;

   def segment(self,SegmentIndex):
      return SGMsegment(sgpy.SGMsegmentListGetSegment(self.__Handle,SegmentIndex));

   def countOfSegment(self):
      return sgpy.SGMsegmentListGetCountOfSegment(self.__Handle);

class SGMsegment:
   __Handle = 0;

   def __init__(self, Handle):
      self.__Handle = Handle
      sgpy.SGMsegmentAddRef(self.__Handle)

   def __del__(self):
       sgpy.SGMsegmentRelease(self.__Handle)
       self.__Handle = 0;

   def name(self):
      return SGMvalue(sgpy.SGMsegmentGetName(self.__Handle));

   def field(self,FieldIndex,RepeatIndex):
      return SGMfield(sgpy.SGMsegmentGetField(self.__Handle,FieldIndex,RepeatIndex));

   def countOfField(self):
      return sgpy.SGMsegmentGetCountOfField(self.__Handle);

   def countOfFieldRepeat(self,FieldIndex):
      return sgpy.SGMsegmentGetCountOfFieldRepeat(self.__Handle,FieldIndex);

   def setCountOfFieldRepeat(self,FieldIndex,RepeatValue):
      sgpy.SGMsegmentSetCountOfFieldRepeat(self.__Handle,FieldIndex,RepeatValue);

   def fieldRepeats(self,FieldIndex):
      return SGMfieldRepeats( sgpy.SGMsegmentGetFieldRepeats(self.__Handle, FieldIndex));

def SGMsegmentListCreate():
    Handle = sgpy.SGMsegmentListCreate();
    return SGMsegmentList(Handle);

def SGMparse(SegmentList, InputString):
   assert(isinstance(SegmentList,SGMsegmentList))
   sgpy.SGMparse(SegmentList.handle(),InputString);

class SGMstringPool:
   __Handle = 0;
   
   def __init__(self, Handle):
      self.__Handle = Handle
      sgpy.SGMstringPoolAddRef(self.__Handle);

   def __del__(self):
      sgpy.SGMstringPoolRelease(self.__Handle);
      self.__Handle = 0;

   def handle(self):
      return self.__Handle;

def SGMstringPoolCreate():
   return SGMstringPool(sgpy.SGMstringPoolCreate());

def SGMstringPoolSetValue(StringPool, SGMvalueInstance, StringValue):
   assert( isinstance(StringPool, SGMstringPool) )
   assert( isinstance(SGMvalueInstance, SGMvalue) )
   sgpy.SGMstringPoolSetValue(StringPool.handle(),\
                              SGMvalueInstance.handle(),\
                              StringValue);
   
class SGMvalue:
   __Handle = 0;

   def __init__(self, Handle):
      self.__Handle = Handle
      sgpy.SGMvalueAddRef(self.__Handle);

   def __del__(self):
      sgpy.SGMvalueRelease(self.__Handle);
      self.__Handle = 0;

   def handle(self):
      return self.__Handle;

   def data(self):
      return sgpy.SGMvalueGetData(self.__Handle);

   def size(self):
      return sgpy.SGMvalueGetSize(self.__Handle);

class SGMsubField:
   __Handle = 0;

   def __init__(self, Handle):
      self.__Handle = Handle;
      sgpy.SGMsubFieldAddRef(self.__Handle);

   def __del__(self):
      sgpy.SGMsubFieldRelease(self.__Handle);
      self.__Handle = 0;

   def setCountOfSubSubField(self,Value):
      sgpy.SGMsubFieldSetCountOfSubSubField(self.__Handle,Value);

   def countOfSubSubField(self):
      return sgpy.SGMsubFieldGetCountOfSubSubField(self.__Handle);

   def value(self,SubSubFieldIndex):
      return SGMvalue( sgpy.SGMsubFieldGetValue(self.__Handle,SubSubFieldIndex) )

class SGMfieldRepeats:
   __Handle = 0;

   def __init__(self, Handle):
      self.__Handle = Handle;
      sgpy.SGMfieldRepeatsAddRef(self.__Handle);

   def __del__(self):
      sgpy.SGMfieldRepeatsRelease(self.__Handle);
      self.__Handle = 0;

   def setCountOfFieldRepeat(self,Value):
      sgpy.SGMfieldRepeatsSetCountOfFieldRepeat(self.__Handle, Value);      

   def fieldRepeat(self,RepeatIndex):
      return SGMfield( sgpy.SGMfieldRepeatsGetFieldRepeat(self.__Handle, RepeatIndex));

   def countOfFieldRepeat(self):
      return sgpy.SGMfieldRepeatsGetCountOfFieldRepeat(self.__Handle);

class SGMfield:
   __Handle = 0;

   def __init__(self, Handle):
      self.__Handle = Handle;
      sgpy.SGMfieldAddRef(self.__Handle);

   def __del__(self):
      sgpy.SGMfieldRelease(self.__Handle);
      self.__Handle = 0;

   def setCountOfSubField(self,Value):
      sgpy.SGMfieldSetCountOfSubField(self.__Handle,Value);

   def value(self,SubFieldIndex,SubSubFieldIndex):
      return SGMvalue( sgpy.SGMfieldGetValue(self.__Handle, SubFieldIndex, SubSubFieldIndex));

   def countOfSubField(self):
      return sgpy.SGMfieldGetCountOfSubField(self.__Handle);

   def countOfSubSubField(self,SubField):
      return sgpy.SGMfieldGetCountOfSubSubField(self.__Handle,SubField);

   def subField(self,SubFieldIndex):
      return SGMsubField( sgpy.SGMfieldGetSubField(self.__Handle,SubFieldIndex) )
