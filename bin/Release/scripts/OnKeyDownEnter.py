# coding: utf-8
# OnKeyDownEnter.py

import clr

#clr.AddReferenceByPartialName("OpenMetaverse")
#from OpenMetaverse import *

clr.AddReference("System")
from System import *

if chat_textbox.StartsWith("デバッグ"):
	chat_textbox = "デバッグ中"
