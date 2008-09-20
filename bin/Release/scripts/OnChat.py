# coding: utf-8
# OnChat.py

import clr

clr.AddReferenceByPartialName("OpenMetaverse")
from OpenMetaverse import *

clr.AddReference("System.Net")
from System.Net import *

#clr.AddReference("System.IO")
#from System.IO import *

clr.AddReference("System.Xml")
from System.Xml import *

clr.AddReference("System")
from System import *

if message.StartsWith(nickname+"python"):
	client.Self.Chat(fromname + "おっけー", 0, ChatType.Normal)

# 容姿が霧のようにもやもやしてる時にコールすると直ることがあります
if message.StartsWith(nickname+"rebake"):
	#client.Appearance.RequestCachedBakes()
	client.Appearance.SetPreviousAppearance(0)

# 容姿の変更サンプル
if message.StartsWith(nickname+"容姿一覧"):
	client.Self.Chat("ぬこ、うさぎ、デフォルト男、デフォルト女", 0, ChatType.Normal)
if message.StartsWith(nickname+"ぬこ"):
    target = "Clothing/nuko"
    client.Appearance.WearOutfit(target.Split('/'), 0)
    client.Appearance.SetPreviousAppearance(0)
if message.StartsWith(nickname+"うさぎ"):
    target = "Clothing/kani"
    client.Appearance.WearOutfit(target.Split('/'), 0)
    client.Appearance.SetPreviousAppearance(0)
if message.StartsWith(nickname+"デフォルト男"):
    target = "Clothing/Male Shape & Outfit"
    client.Appearance.WearOutfit(target.Split('/'), 0)
    client.Appearance.SetPreviousAppearance(0)
if message.StartsWith(nickname+"デフォルト女"):
    target = "Clothing/Female Shape & Outfit"
    client.Appearance.WearOutfit(target.Split('/'), 0)
    client.Appearance.SetPreviousAppearance(0)

# 立川君ちょうだいサンプル
if message.StartsWith(nickname+"グローブちょうだい"):
    item = "==Valor==Boxing globe (Blue) L Powered by Yuna"
    inventory.giveItem(inventory.getUUIDbyAvatarName(fromname), item);
    item = "==Valor==Boxing globe (Blue) R Powered by Yuna"
    inventory.giveItem(inventory.getUUIDbyAvatarName(fromname), item);

# アニメーションのサンプル
if message.StartsWith(nickname+"スクリプト一覧"):
	client.Self.Chat("座れ、立ち上がれ、死ね、生き返れ、ちゅうして、ブラッシング、拍手、マッスル、シャドーボクシング、ピース、キック、ストレッチ", 0, ChatType.Normal)
if message.StartsWith(nickname+"座れ"):
	client.Self.AnimationStop(Animations.SIT_GROUND, 1)
	client.Self.AnimationStart(Animations.SIT_GROUND, 1)
if message.StartsWith(nickname+"立ち上がれ"):
	client.Self.AnimationStop(Animations.SIT_GROUND, 1)
if message.StartsWith(nickname+"死ね"):
	client.Self.AnimationStart(Animations.DEAD, 1)
if message.StartsWith(nickname+"生き返れ"):
	client.Self.AnimationStop(Animations.DEAD, 1)
if message.StartsWith(nickname+"ちゅうして"):
	client.Self.AnimationStop(Animations.BLOW_KISS, 1)
	client.Self.AnimationStart(Animations.BLOW_KISS, 1)
if message.StartsWith(nickname+"ブラッシング"):
	client.Self.AnimationStop(Animations.BRUSH, 1)
	client.Self.AnimationStart(Animations.BRUSH, 1)
if message.StartsWith(nickname+"拍手"):
	client.Self.AnimationStop(Animations.CLAP, 1)
	client.Self.AnimationStart(Animations.CLAP, 1)
if message.StartsWith(nickname+"マッスル"):
	client.Self.AnimationStop(Animations.MUSCLE_BEACH, 1)
	client.Self.AnimationStart(Animations.MUSCLE_BEACH, 1)
if message.StartsWith(nickname+"シャドーボクシング"):
	client.Self.AnimationStop(Animations.ONETWO_PUNCH, 1)
	client.Self.AnimationStart(Animations.ONETWO_PUNCH, 1)
if message.StartsWith(nickname+"ピース"):
	client.Self.AnimationStop(Animations.PEACE, 1)
	client.Self.AnimationStart(Animations.PEACE, 1)
if message.StartsWith(nickname+"キック"):
	client.Self.AnimationStop(Animations.ROUNDHOUSE_KICK, 1)
	client.Self.AnimationStart(Animations.ROUNDHOUSE_KICK, 1)
if message.StartsWith(nickname+"ストレッチ"):
	client.Self.AnimationStop(Animations.STRETCH, 1)
	client.Self.AnimationStart(Animations.STRETCH, 1)

if message.StartsWith(nickname+"リンデンオフィシャルブログ"):
	url = "http://blog.secondlife.com/feed/"
	request = HttpWebRequest.Create(url)
	request.Timeout = 5000
	request.ReadWriteTimeout = 20000
	response = request.GetResponse()
	str = response.GetResponseStream()

	xmldoc = XmlDocument()
	xmldoc.Load(str)

	items = xmldoc.SelectNodes("rss/channel/item")
	for item in items:
		title = item.SelectSingleNode("title").InnerText
		client.Self.Chat(title, 0, ChatType.Normal)

