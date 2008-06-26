# coding: utf-8
# OnChat.py

import clr

clr.AddReferenceByPartialName("libsecondlife")
from libsecondlife import *

clr.AddReference("System")
from System import *

if message.StartsWith(nickname+"python"):
	client.Self.InstantMessage(fromAgentID, fromname + "IMおっけー", sessionID)
