#----------------------------------------------------------------------------
# Copyright (C) 1997-2007 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: ifware.guid
#
# Description:
#
# A simple module to handle guid generation
#
# Revision: $Revision: 1.1 $
#----------------------------------------------------------------------------

# vim: sts=3 ts=3 sw=3 et

__all__ = [
   'generate_guid',
]

import sgpy

def generate_guid(bit_strength = 128):
    return sgpy.SGPYguidGenerate(bit_strength);
