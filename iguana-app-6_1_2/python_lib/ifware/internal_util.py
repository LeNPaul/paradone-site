#----------------------------------------------------------------------------
# Copyright (C) 1997-2007 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: ifware.internal_util
#
# Description:
#
# A collection of utility functions that are used internally by Chameleon
# and Iguana
#
# Revision: $Revision: 1.2 $
#----------------------------------------------------------------------------

__all__ = [
   'reload_all_source_file_modules',
   'raise_attribute_error'
]

import imp
import sys

#raises nicely printed attribute error
def raise_attribute_error(InvalidAttr,ValidAttrList):
    ErrorString = "Invalid attribute '" + str(InvalidAttr) + "'. Valid attributes are: "
    for AttrIndex in range(len(ValidAttrList)):
        ErrorString = ErrorString + "'" + str(ValidAttrList[AttrIndex])+ "'"
        if AttrIndex+1 != len(ValidAttrList):
            ErrorString = ErrorString + ", "
    raise AttributeError(ErrorString)    


#finds the type as returned by the imp module
#of the given module name
def check_module_type(full_module_name,path = None):
   split_result = full_module_name.split(".",1)
   #print "Checking ",full_module_name
   package_name = None
   module_name = None
   if len(split_result) == 1:
      package_name = None
      module_name = split_result[0]
   else:
      package_name = split_result[0]
      module_name = split_result[1]
   if package_name:
      #print "   In Package:",package_name
      #recurse
      #make sure module is loadad
      package_info = imp.find_module(package_name,path);
      package_module =imp.load_module( package_name,
                                       package_info[0],
                                       package_info[1],
                                       package_info[2])
      if '__path__' in dir(package_module):
         return check_module_type(module_name,package_module.__path__)
      else:
         #use packages type if there is no path
         return package_info[2][2]
   else:
      #print "   In Module:",module_name
      module_props = None
      try:
         #try loading it with given path, if it fails
         #try with no path
         module_props = imp.find_module(module_name,path);
      except ImportError:
         module_props = imp.find_module(module_name,None);
      return module_props[2][2];

def reload_all_source_file_modules():
   module_names = sys.modules.keys()
   for module_name in module_names:
      #skip if key wasn't loaded
      if sys.modules[module_name] == None:
         continue
      try:
         module_type = check_module_type(module_name);
         if module_type in [imp.PY_SOURCE,imp.PKG_DIRECTORY]:
            #print "YES Reloading :",module_name
            reload( sys.modules[module_name] )
         else:
            pass
            #print "NO  Reloading : ",module_name
      except ImportError,E:
         #print 'ERR Reloading : ',E
         continue
      
