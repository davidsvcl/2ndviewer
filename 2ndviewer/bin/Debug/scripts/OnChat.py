# coding: utf-8
# OnChat.py

import clr

clr.AddReferenceByPartialName("OpenMetaverse")
from OpenMetaverse import *

clr.AddReference("System")
from System import *

if message.StartsWith(nickname+"python"):
	client.Self.Chat(fromname + "おっけー", 0, ChatType.Normal)
