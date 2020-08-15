#----------------------------------------------------------------------------
# Copyright (C) 1997-2007 iNTERFACEWARE Inc.  All Rights Reserved
#
# Module: DLLchameleonTableObject
#
# Description:
#
# DLL based Chameleon Field Object
#
# This is an internal class used by iNTERFACEWARE, do not modify.
#
# Revision: $Revision: 1.2 $
#----------------------------------------------------------------------------

import internal_util;

class ChameleonTableObject:
    RemoveCurrentRowFlag = 0

    def __init__(self, RemoveCurrentRow):
        self.RemoveCurrentRowFlag = RemoveCurrentRow;

    def remove_current_row(self):
        self.RemoveCurrentRowFlag = 1;

#Do not use, called from within Chameleon
def ChameleonTableObjectGetRemoveCurrentRow(Obj):
    return Obj.RemoveCurrentRowFlag
