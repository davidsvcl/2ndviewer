# coding: utf-8
# OnChat.py

import clr

clr.AddReferenceByPartialName("libsecondlife")
from libsecondlife import *

clr.AddReference("System")
from System import *

if message.StartsWith(nickname+"python"):
	client.Self.Chat(fromname + "おっけー", 0, ChatType.Normal)
