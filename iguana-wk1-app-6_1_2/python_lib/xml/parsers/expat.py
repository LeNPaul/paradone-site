"""Interface to the Expat non-validating XML parser."""
__version__ = '$Revision: 1.2 $'

import sys

try:
    from pyexpat import *
except ImportError:
    del sys.modules[__name__]
    del sys
    raise

del sys
