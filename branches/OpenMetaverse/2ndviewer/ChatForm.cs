using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;
using OpenMetaverse;
using OpenMetaverse.Packets;
using IronPython.Hosting;

namespace _2ndviewer
{
    /// <summary>
    /// チャットウィンドウクラス
    /// チャットの送受信を行います。
    /// またIronPythonを仲介します。
    /// </summary>
    public partial class ChatForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>Second Lifeグリッド通信ライブラリ</summary>
        private GridClient client_;
        /// <summary>コントロールウィンドウ</summary>
        private MovementForm movementForm_;
        /// <summary>アイテムウィンドウ</summary>
        private InventoryForm inventoryForm_;
        /// <summary>IronPython仲介エンジン</summary>
        private PythonEngine pe_;
        /// <summary>ニックネーム、デフォルト「立川君」</summary>
        private string nickName_;
        /// <summary>ニュー速VIPのURL</summary>
        private string news4vip_;
        /// <summary>IMタブ配列</summary>
        System.Collections.Generic.List<Im_tab> uuid_array_;
        /// <summary>踊れコマンドとニュー速VIP用乱数</summary>
        Random rnd_;
        /// <summary>ニュー速VIPのタイトル配列</summary>
        System.Collections.Generic.List<string> news4vip_subs_;
        /// <summary>翻訳コマンドの状態</summary>
        private bool translate_;
        /// <summary>容姿配列</summary>
        public Dictionary<UUID, AvatarAppearancePacket> Appearances = new Dictionary<UUID, AvatarAppearancePacket>();

        /// <summary>チャットログにテキストを追加するためのデリゲート</summary>
        private delegate void WriteLineDelegate(string str);
        /// <summary>IMタブを追加するためのデリゲート</summary>
        private delegate void AddTabDelegate(Im_tab im_tab, string fromName, string message);
        /// <summary>IMタブにテキストを追加するためのデリゲート</summary>
        private delegate void IMTabWriteLineDelegate(Im_tab im_tab, string fromName, string message);
        /// <summary>ストーキングを開始するためのデリゲート</summary>
        private delegate void FollowDelegate(bool check, string name);
        /// <summary>Yahoo!ニュースを発言するためのデリゲート</summary>
        private delegate void YahooNewsDelegate(string name);
        /// <summary>Lindenオフィシャルブログを発言するためのデリゲート</summary>
        private delegate void LindenOfficialBlogDelegate(string name);
        /// <summary>ソラマメブログを発言するためのデリゲート</summary>
        private delegate void SlmameDelegate(string name);
        /// <summary>ニュー速VIPのスレ発言するためのデリゲート</summary>
        private delegate void News4VipDelegate(string name);
        /// <summary>翻訳するためのデリゲート</summary>
        private delegate void TranslateDelegate(string fromname, string message);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChatForm()
        {
            InitializeComponent();
            uuid_array_ = new System.Collections.Generic.List<Im_tab>();
            news4vip_subs_ = new System.Collections.Generic.List<string>();
            translate_ = false;
            rnd_ = new Random();
            pe_ = new PythonEngine();
        }

        /// <summary>
        /// SetClient
        /// 通信ライブラリをセットする
        /// IronPythonエンジンにグローバル変数を追加する
        /// IronPythonのグローバル変数にダミー値をセットする
        /// </summary>
        public void SetClient(GridClient client)
        {
            client_ = client;
            pe_.Globals.Add("client",client_);
            string dummy = "dummy";
            pe_.Globals.Add("message", dummy);
            pe_.Globals.Add("chat_textbox", dummy);
            UUID dummyuuid = UUID.Zero;
            pe_.Globals.Add("fromAgentID",dummyuuid);
            pe_.Globals.Add("sessionID",dummyuuid);
        }

        /// <summary>コントロールウィンドウをセットする</summary>
        public void SetMovementForm(MovementForm movementForm)
        {
            movementForm_ = movementForm;
        }

        /// <summary>アイテムウィンドウをセットする</summary>
        public void SetInventoryForm(InventoryForm inventoryForm)
        {
            inventoryForm_ = inventoryForm;
            pe_.Globals.Add("inventory", inventoryForm_);
        }

        /// <summary>ニックネームをセットする</summary>
        public void SetNickName(string nickname)
        {
            nickName_ = nickname;
            pe_.Globals.Remove("nickname");
            pe_.Globals.Add("nickname", nickName_);
            pe_.Globals.Remove("fromname");
            pe_.Globals.Add("fromname", nickName_);
        }

        /// <summary>ニュー速VIPのURLをセットする</summary>
        public void SetNews4Vip(string url)
        {
            news4vip_ = url;
        }

        /// <summary>
        /// WriteLine
        /// チャットログにテキストを追加し、最後の行へスクロールする
        /// </summary>
        private void WriteLine(string str)
        {
            chatLog_textBox.AppendText(str);
            chatLog_textBox.SelectionStart = chatLog_textBox.Text.Length;
            chatLog_textBox.ScrollToCaret();
        }

        /// <summary>
        /// AddTab
        /// IMタブを追加する
        /// </summary>
        private void AddTab(Im_tab im_tab, string fromName, string message)
        {
            im_tab.tabPage_ = new TabPage();
            im_tab.tabPage_.Name = fromName;
            im_tab.tabPage_.Padding = new System.Windows.Forms.Padding(3);
            im_tab.tabPage_.Size = new System.Drawing.Size(278, 189);
            im_tab.tabPage_.TabIndex = 1;
            im_tab.tabPage_.Text = fromName;
            im_tab.tabPage_.UseVisualStyleBackColor = true;
            tabControl1.Controls.Add(im_tab.tabPage_);

            im_tab.textBox_ = new TextBox();
            im_tab.tabPage_.Controls.Add(im_tab.textBox_);
            im_tab.textBox_.Dock = DockStyle.Fill;
            im_tab.textBox_.Location = new System.Drawing.Point(3, 3);
            im_tab.textBox_.Multiline = true;
            im_tab.textBox_.Name = fromName;
            im_tab.textBox_.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            im_tab.textBox_.Size = new System.Drawing.Size(272, 183);
            im_tab.textBox_.TabIndex = 0;

            im_tab.textBox_.Text = fromName + ":" + message;
            im_tab.textBox_.SelectionStart = message.Length;
            im_tab.textBox_.ScrollToCaret();

            uuid_array_.Add(im_tab);
        }

        /// <summary>
        /// StartIM
        /// IMタブを追加する
        /// </summary>
        public void StartIM(UUID toAgentID, string toName)
        {
            Im_tab im_tab = new Im_tab();
            im_tab.fromAgentID_ = toAgentID;
            im_tab.sessionID_ = UUID.Random();

            im_tab.tabPage_ = new TabPage();
            im_tab.tabPage_.Name = toName;
            im_tab.tabPage_.Padding = new System.Windows.Forms.Padding(3);
            im_tab.tabPage_.Size = new System.Drawing.Size(278, 189);
            im_tab.tabPage_.TabIndex = 1;
            im_tab.tabPage_.Text = toName;
            im_tab.tabPage_.UseVisualStyleBackColor = true;
            tabControl1.Controls.Add(im_tab.tabPage_);

            im_tab.textBox_ = new TextBox();
            im_tab.tabPage_.Controls.Add(im_tab.textBox_);
            im_tab.textBox_.Dock = DockStyle.Fill;
            im_tab.textBox_.Location = new System.Drawing.Point(3, 3);
            im_tab.textBox_.Multiline = true;
            im_tab.textBox_.Name = toName;
            im_tab.textBox_.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            im_tab.textBox_.Size = new System.Drawing.Size(272, 183);
            im_tab.textBox_.TabIndex = 0;

            uuid_array_.Add(im_tab);
        }

        /// <summary>
        /// IMTabWriteLine
        /// IMタブにテキストを追加する
        /// </summary>
        private void IMTabWriteLine(Im_tab im_tab, string fromName, string message)
        {
            im_tab.textBox_.AppendText("\r\n" + fromName + ":" + message);
            im_tab.textBox_.SelectionStart = im_tab.textBox_.Text.Length;
            im_tab.textBox_.ScrollToCaret();
        }

        /// <summary>
        /// Follow
        /// ストーキングの開始/停止をコントロールウィンドウにセットする
        /// </summary>
        private void Follow(bool check, string name)
        {
            movementForm_.follow_checkBox.Checked = check;
            movementForm_.follow_textBox.Text = name;
        }

        /// <summary>
        /// Self_OnChat
        /// チャット受信時に呼び出されるメソッドです。
        /// チャットコマンドの解析を行います
        /// OnChat.pyにつながります
        /// </summary>
        public void Self_OnChat(string message, ChatAudibleLevel audible, ChatType type, ChatSourceType sourceType, string fromName, UUID id, UUID ownerid, Vector3 position)
        {
            if (message.Length <= 0) return;
            string msg = "\r\n" + fromName + ":" + message;
            if (movementForm_.chatlog_checkBox.Checked) {
                System.IO.File.AppendAllText(System.IO.Directory.GetCurrentDirectory()+"\\chat.txt", msg);
            }
            if (fromName != client_.Self.Name)
            {
                if (message.StartsWith(nickName_+"コマンド"))
                {
                    client_.Self.Chat(nickName_+","+nickName_+"おいで,"+nickName_+"とまれ,"+nickName_+"踊れ,"+nickName_+"ポーズ一覧,"+nickName_+"123(ポーズ),"+nickName_+"おちつけ,"+nickName_+"ニュース,リンデン,ソラマメ,VIP", 0, ChatType.Normal);
                }
                else if (message.StartsWith(nickName_+"デバッグ")) {
                    client_.Appearance.SetPreviousAppearance(false);
                }
                else if (message.StartsWith(nickName_+"踊れ"))
                {
                    int dec = rnd_.Next(15)+1;
                    string dance_msg = "Dance"+ dec;
                    client_.Self.Chat(dance_msg, 123, ChatType.Normal);
                }
                else if (message.StartsWith(nickName_+"ポーズ一覧"))
                {
                    string dance_msg = "stop,_kata3,ClubDance,orz,kneel,KataLong,PrivDance,spider,dead,Sexy1,Doggy,cat,dog,spank,sexy_pose,show1,show2,show3,Sexy2,show4,show5,cleaning,Model Pose";
                    client_.Self.Chat(dance_msg, 0, ChatType.Normal);
                }
                else if (message.StartsWith(nickName_+"123"))
                {
                    int pos = message.IndexOf("3");
                    string dance_msg = message.Substring(pos + 1, message.Length - nickName_.Length - 3);
                    client_.Self.Chat(dance_msg, 123, ChatType.Normal);
                }
                else if (message.StartsWith(nickName_+"おちつけ"))
                {
                    client_.Self.Chat("stop", 123, ChatType.Normal);
                }
                else if (message.StartsWith(nickName_+"おいで"))
                {
                    movementForm_.follow_on_ = true;
                    movementForm_.followName_ = fromName;
                    FollowDelegate fdlg = new FollowDelegate(Follow);
                    object[] farg = { true, fromName };
                    Invoke(fdlg, farg);
                }
                else if (message.StartsWith(nickName_+"とまれ"))
                {
                    movementForm_.follow_on_ = false;
                    movementForm_.followName_ = fromName;
                    FollowDelegate fdlg = new FollowDelegate(Follow);
                    object[] farg = { false, fromName };
                    Invoke(fdlg, farg);
                }
                else if (message.StartsWith(nickName_+"ニュース"))
                {
                    YahooNewsDelegate ydlg = new YahooNewsDelegate(YahooNews);
                    string[] yarg = { fromName };
                    Invoke(ydlg, yarg);
                }
                else if (message.StartsWith("リンデン"))
                {
                    LindenOfficialBlogDelegate lodlg = new LindenOfficialBlogDelegate(LindenOfficialBlog);
                    string[] loarg = { fromName };
                    Invoke(lodlg, loarg);
                }
                else if (message.StartsWith("ソラマメ"))
                {
                    SlmameDelegate slmdlg = new SlmameDelegate(Slmame);
                    string[] slmarg = { fromName };
                    Invoke(slmdlg, slmarg);
                }
                else if (message.IndexOf("VIP") != -1 || message.IndexOf("vip") != -1 || message.IndexOf("Vip") != -1)
                {
                    News4VipDelegate vipdlg = new News4VipDelegate(News4Vip);
                    string[] viparg = { fromName };
                    Invoke(vipdlg, viparg);
                }
                else if (message.StartsWith(nickName_+"マネ"))
                {
                    int pos = message.IndexOf("ネ");
                    string targetName = message.Substring(pos + 1, message.Length - nickName_.Length - 2);
                    List<DirectoryManager.AgentSearchData> matches;
                    uint SerialNum = 2;

                    if (client_.Directory.PeopleSearch(DirectoryManager.DirFindFlags.People, targetName, 0, 1000 * 10,
                        out matches) && matches.Count > 0)
                    {
                        UUID target = matches[0].AgentID;
                        targetName += String.Format(" ({0})", target);
        
                        if (Appearances.ContainsKey(target))
                        {
                            #region AvatarAppearance to AgentSetAppearance
        
                            AvatarAppearancePacket appearance = Appearances[target];
        
                            AgentSetAppearancePacket set = new AgentSetAppearancePacket();
                            set.AgentData.AgentID = client_.Self.AgentID;
                            set.AgentData.SessionID = client_.Self.SessionID;
                            set.AgentData.SerialNum = SerialNum++;
                            set.AgentData.Size = new Vector3(2f, 2f, 2f); // HACK
        
                            set.WearableData = new AgentSetAppearancePacket.WearableDataBlock[0];
                            set.VisualParam = new AgentSetAppearancePacket.VisualParamBlock[appearance.VisualParam.Length];
        
                            for (int i = 0; i < appearance.VisualParam.Length; i++)
                            {
                                set.VisualParam[i] = new AgentSetAppearancePacket.VisualParamBlock();
                                set.VisualParam[i].ParamValue = appearance.VisualParam[i].ParamValue;
                            }
        
                            set.ObjectData.TextureEntry = appearance.ObjectData.TextureEntry;
        
                            #endregion AvatarAppearance to AgentSetAppearance
        
                            // Detach everything we are currently wearing
                            client_.Appearance.AddAttachments(new List<InventoryBase>(), true);
        
                            // Send the new appearance packet
                            client_.Network.SendPacket(set);
        
                        }
                        else
                        {
                            MessageBox.Show("Don't know the appearance of avatar " + targetName);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Couldn't find avatar " + targetName);
                    }
                }
                else
                {
                    System.Text.RegularExpressions.Regex regHiragana = new System.Text.RegularExpressions.Regex(
                        @"\p{IsHiragana}*", System.Text.RegularExpressions.RegexOptions.None);
                    System.Text.RegularExpressions.Regex regKanji = new System.Text.RegularExpressions.Regex(
                        @"\p{IsCJKUnifiedIdeographs}*", System.Text.RegularExpressions.RegexOptions.None);
                    System.Collections.ArrayList hiraganaFields = new System.Collections.ArrayList();
                    System.Collections.ArrayList kanjiFields = new System.Collections.ArrayList();
                    System.Text.RegularExpressions.Match m = regHiragana.Match(message);
                    while (m.Success)
                    {
                        string field = m.Groups[0].Value;
                        if(field.Length>0)hiraganaFields.Add(field);
                        m = m.NextMatch();
                    }
                    m = regKanji.Match(message);
                    while (m.Success)
                    {
                        string field = m.Groups[0].Value;
                        if (field.Length > 0) kanjiFields.Add(field);
                        m = m.NextMatch();
                    }
                    /*
                    for (int i = 0; i < hiraganaFields.Count; i++)
                    {
                        client_.Self.Chat(hiraganaFields[i].ToString(), 0, ChatType.Normal);
                    }
                    for (int i = 0; i < kanjiFields.Count; i++)
                    {
                        client_.Self.Chat(kanjiFields[i].ToString(), 0, ChatType.Normal);
                    }
                     * */
                    bool found = false;
                    int dec = rnd_.Next(10);
                    if (news4vip_subs_.Count > 0 && dec == 1)
                    {
                        for (int i = 0; i < news4vip_subs_.Count; i++)
                        {
                            for (int j = 0; j < kanjiFields.Count; j++)
                            {
                                if (0 < news4vip_subs_[i].IndexOf(kanjiFields[j].ToString()))
                                {
                                    string munou_msg = news4vip_subs_[i].Substring(news4vip_subs_[i].IndexOf(">") + 1, news4vip_subs_[i].Length - news4vip_subs_[i].IndexOf(">") - 1);
                                    client_.Self.Chat(munou_msg, 0, ChatType.Normal);
                                    found = true;
                                    break;
                                }
                            }
                            if (found) break;
                        }
                    }
                }
                if (translate_ == true)
                {
                    TranslateDelegate transdlg = new TranslateDelegate(Translate);
                    string[] transarg = { fromName, message };
                    Invoke(transdlg, transarg);
                }
            }
            if (message.StartsWith(nickName_ + "翻訳やめ"))
            {
                translate_ = false;
                client_.Self.Chat("翻訳をやめます", 0, ChatType.Normal);
            }
            else if (message.StartsWith(nickName_ + "翻訳"))
            {
                translate_ = true;
                client_.Self.Chat("翻訳を開始します", 0, ChatType.Normal);
            }
            try
            {
                pe_.Globals.Remove("message");
                pe_.Globals.Add("message", message);
                pe_.Globals.Remove("fromname");
                pe_.Globals.Add("fromname", fromName);
                pe_.ExecuteFile("scripts\\OnChat.py");
            }
            catch (Exception e)
            {
                //System.Diagnostics.Trace.WriteLine(e);
                SystemMessage("PythonError(scripts\\OnChat.py):\r\n" + e.ToString());
            }
            Speech(message);
            WriteLineDelegate dlg = new WriteLineDelegate(WriteLine);
            string[] arg = { msg };
            Invoke(dlg, arg);
        }

        /// <summary>
        /// Speech
        /// 音声読み上げメソッドです。
        /// </summary>
        private void Speech(string message)
        {
            string speech = movementForm_.speech_;
            if (speech != StringResource.none)
            {
                try
                {
                    System.Speech.Synthesis.SpeechSynthesizer ss = new System.Speech.Synthesis.SpeechSynthesizer();
                    ss.SelectVoice(speech);
                    ss.Speak(message);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// SystemMessage
        /// システムメッセージをチャットログに追加するメソッドです。
        /// </summary>
        public void SystemMessage(string message)
        {
            if (message.Length <= 0) return;
            message += "\r\n";
            WriteLineDelegate dlg = new WriteLineDelegate(WriteLine);
            string[] arg = { message };
            Invoke(dlg, arg);
        }

        /// <summary>
        /// IMChat
        /// IMタブにをIMログに追加するメソッドです。
        /// 該当するIMタブがなければIMタブを追加します
        /// OnIMChat.pyにつながります
        /// </summary>
        public void IMChat(string message, string fromName, UUID fromAgentID, UUID sessionID)
        {
            if (message.Length <= 0) return;

            Im_tab new_im = new Im_tab();
            new_im.fromAgentID_ = fromAgentID;
            new_im.sessionID_ = sessionID;

            bool found = false;
            for (int i = 0; i < uuid_array_.Count; i++)
            {
                if (new_im == uuid_array_[i])
                {
                    // 見つかった
                    found = true;
                    IMTabWriteLineDelegate imtwdlg = new IMTabWriteLineDelegate(IMTabWriteLine);
                    Object[] imtwarg = {uuid_array_[i], fromName, message };
                    Invoke(imtwdlg, imtwarg);
                }
            }
            if (found == false)
            {
                // 見つからなかったから新しいIMタブを作成する
                AddTabDelegate atdlg = new AddTabDelegate(AddTab);
                Object[] atdlgarg = {new_im, fromName, message};
                Invoke(atdlg, atdlgarg);
            }

            try
            {
                pe_.Globals.Remove("message");
                pe_.Globals.Add("message", message);
                pe_.Globals.Remove("fromname");
                pe_.Globals.Add("fromname", fromName);
                pe_.Globals.Remove("fromAgentID");
                pe_.Globals.Add("fromAgentID", fromAgentID);
                pe_.Globals.Remove("sessionID");
                pe_.Globals.Add("sessionID", sessionID);
                pe_.ExecuteFile("scripts\\OnIMChat.py");
            }
            catch (Exception e)
            {
                //System.Diagnostics.Trace.WriteLine(e);
                SystemMessage("PythonError(scripts\\OnIMChat.py):\r\n" + e.ToString());
            }
            Speech(fromName + " " + message);
            string msg = "\r\n" + "<IM>" + fromName + ":" + message;
            if (movementForm_.chatlog_checkBox.Checked)
            {
                System.IO.File.AppendAllText(System.IO.Directory.GetCurrentDirectory() + "\\chat.txt", msg);
            }
            WriteLineDelegate dlg = new WriteLineDelegate(WriteLine);
            string[] arg = { msg };
            Invoke(dlg, arg);
        }

        /// <summary>
        /// chat_textBox_KeyDown
        /// チャット発言エリアでエンターキーを押した時呼ばれるメソッドです。
        /// 「/チャンネル番号 メッセージ」で特定チャンネルに向けて発言できます
        /// OnKeyDownEnter.pyにつながります
        /// </summary>
        private void chat_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                try
                {
                    pe_.Globals.Remove("chat_textbox");
                    pe_.Globals.Add("chat_textbox", chat_textBox.Text);
                    pe_.ExecuteFile("scripts\\OnKeyDownEnter.py");
                    chat_textBox.Text = pe_.EvaluateAs<string>("chat_textbox");
                }
                catch (Exception exc)
                {
                    SystemMessage("PythonError(scripts\\OnChatKeyDownEnter.py):\r\n" + exc.ToString());
                }
                int i = tabControl1.SelectedIndex;
                if (i <= 0)
                {
                    if (0 == chat_textBox.Text.IndexOf('/'))
                    {
                        int index;
                        string channel_str;
                        try
                        {
                            index = chat_textBox.Text.IndexOf(' ');
                            channel_str = chat_textBox.Text.Substring(1, index);
                        }
                        catch
                        {
                            MessageBox.Show("channel chat format is \"/channelnumber strings\"");
                            return;
                        }
                        int channel = int.Parse(channel_str);
                        string text = chat_textBox.Text.Substring(index + 1, chat_textBox.Text.Length - index - 1);
                        client_.Self.Chat(text, channel, ChatType.Normal);
                    }
                    else
                    {
                        client_.Self.Chat(chat_textBox.Text, 0, ChatType.Normal);
                    }
                }
                else {
                    Im_tab im = uuid_array_[i - 1];
                    client_.Self.InstantMessage(im.fromAgentID_, chat_textBox.Text, im.sessionID_);
                    if (movementForm_.chatlog_checkBox.Checked)
                    {
                        System.IO.File.AppendAllText(System.IO.Directory.GetCurrentDirectory() + "\\chat.txt", "\r\n<IM>" + im.tabPage_.Text + ":" + chat_textBox.Text);
                    }
                    im.textBox_.AppendText("\r\n" + client_.Self.Name + ":" + chat_textBox.Text);
                    im.textBox_.SelectionStart = im.textBox_.Text.Length;
                    im.textBox_.ScrollToCaret();
                }
                chat_textBox.Text = "";
            }
        }

        /// <summary>
        /// closeToolStripMenuItem_Click
        /// ポップアップメニューの閉じるを選択時呼ばれるメソッドです。
        /// </summary>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = this.tabControl1.SelectedIndex;
            if (index <= 0)
            {
            }
            else
            {
                this.tabControl1.Controls.RemoveAt(index);
                uuid_array_.RemoveAt(index - 1);
            }
        }




        /// <summary>
        /// YahooNews
        /// Yahoo!ニュースを取得しチャットで発言するメソッドです。
        /// </summary>
        private void YahooNews(string name)
        {
            try
            {
                System.Net.HttpWebRequest request = null;
                System.Net.HttpWebResponse response = null;
                string yahooNewsURL = "http://api.news.yahoo.co.jp/NewsWebService/V1/Topics?appid=YahooDemo&num=30&sort=pvindex&topflg=1";
                request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(yahooNewsURL);
                request.Timeout = 5000;
                request.ReadWriteTimeout = 20000;
                response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream str = response.GetResponseStream();
                //System.IO.StreamReader sr = new System.IO.StreamReader(str);
                //string xml = sr.ReadToEnd();

                System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                xmldoc.Load(str);
                System.Xml.XmlElement root = xmldoc.DocumentElement;
                foreach (System.Xml.XmlNode top_node in root.ChildNodes)
                {
                    foreach (System.Xml.XmlNode next_node in top_node.ChildNodes)
                    {
                        if (next_node.Name == "title")
                        {
                            string msg = next_node.InnerText + " ";
                            client_.Self.Chat(msg, 0, ChatType.Normal);
                        }
                        if (next_node.Name == "url")
                        {
                            string msg = next_node.InnerText;
                            client_.Self.Chat(msg, 0, ChatType.Normal);
                        }
                    }
                }

            }
            catch (Exception e) {
                client_.Self.Chat("エラー:"+e.Message, 0, ChatType.Normal);
            }
        }

        /// <summary>
        /// YahooNews
        /// Yahoo!ニュースを取得しチャットで発言するメソッドです。
        /// </summary>
        private void LindenOfficialBlog(string name)
        {
            try
            {
                System.Net.HttpWebRequest request = null;
                System.Net.HttpWebResponse response = null;
                string yahooNewsURL = "http://blog.secondlife.com/feed/";
                request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(yahooNewsURL);
                request.Timeout = 5000;
                request.ReadWriteTimeout = 20000;
                response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream str = response.GetResponseStream();
                //System.IO.StreamReader sr = new System.IO.StreamReader(str);
                //string xml = sr.ReadToEnd();

                System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                xmldoc.Load(str);
                System.Xml.XmlElement root = xmldoc.DocumentElement;
                foreach (System.Xml.XmlNode top_node in root.ChildNodes)
                {
                    foreach (System.Xml.XmlNode next_node in top_node.ChildNodes)
                    {
                        foreach (System.Xml.XmlNode next_node2 in next_node.ChildNodes)
                        {
                            if (next_node2.Name == "title")
                            {
                                string msg = next_node2.InnerText + " ";
                                client_.Self.Chat(msg, 0, ChatType.Normal);
                            }
                            if (next_node2.Name == "link")
                            {
                                string msg = next_node2.InnerText;
                                client_.Self.Chat(msg, 0, ChatType.Normal);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                client_.Self.Chat("エラー:" + e.Message, 0, ChatType.Normal);
            }
        }

        /// <summary>
        /// Slmame
        /// Slmameブログを取得しチャットで発言するメソッドです。
        /// </summary>
        private void Slmame(string name)
        {
            try
            {
                System.Net.HttpWebRequest request = null;
                System.Net.HttpWebResponse response = null;
                string yahooNewsURL = "http://www.slmame.com/entry.rdf";
                request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(yahooNewsURL);
                request.Timeout = 5000;
                request.ReadWriteTimeout = 20000;
                response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream str = response.GetResponseStream();
                //System.IO.StreamReader sr = new System.IO.StreamReader(str);
                //string xml = sr.ReadToEnd();

                System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                xmldoc.Load(str);
                System.Xml.XmlElement root = xmldoc.DocumentElement;
                foreach (System.Xml.XmlNode top_node in root.ChildNodes)
                {
                    foreach (System.Xml.XmlNode next_node in top_node.ChildNodes)
                    {
                        if (next_node.Name == "title")
                        {
                            string msg = next_node.InnerText + " ";
                            client_.Self.Chat(msg, 0, ChatType.Normal);
                        }
                        if (next_node.Name == "description")
                        {
                            string msg = next_node.InnerText;
                            client_.Self.Chat(msg, 0, ChatType.Normal);
                        }
                        if (next_node.Name == "link")
                        {
                            string msg = next_node.InnerText;
                            client_.Self.Chat(msg, 0, ChatType.Normal);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                client_.Self.Chat("エラー:" + e.Message, 0, ChatType.Normal);
            }
        }

        /// <summary>
        /// News4Vip
        /// ニュー速VIPのスレ一覧を取得しチャットで発言するメソッドです。
        /// </summary>
        private void News4Vip(string name)
        {
            try
            {
                System.Net.HttpWebRequest request = null;
                System.Net.HttpWebResponse response = null;
                string yahooNewsURL = news4vip_;// "http://ex25.2ch.net/news4vip/subject.txt";
                request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(yahooNewsURL);
                request.Timeout = 5000;
                request.ReadWriteTimeout = 20000;
                response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream str = response.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(str, System.Text.Encoding.GetEncoding(932));
                string text = sr.ReadToEnd();

                string[] texts = System.Text.RegularExpressions.Regex.Split(text, "\n");
                string msg = texts[0].Substring(texts[0].IndexOf(">") + 1, texts[0].Length - texts[0].IndexOf(">") - 1);
                client_.Self.Chat(msg, 0, ChatType.Normal);
                if (news4vip_subs_.Count == 0)
                {
                    for (int i = 0; i < texts.Length; i++)
                    {
                        news4vip_subs_.Add(texts[i]);
                    }
                }
            }
            catch (Exception e) {
                client_.Self.Chat("エラー:" + e.Message, 0, ChatType.Normal);
            }

        }

        /// <summary>
        /// Translate
        /// 翻訳しチャットで発言するメソッドです。
        /// </summary>
        private void Translate(string fromname, string message)
        {
            try
            {
                bool isJapanese = false;
                if (System.Text.RegularExpressions.Regex.IsMatch(message, "\\p{IsHiragana}"))
                {
                    isJapanese = true;
                }
                if (System.Text.RegularExpressions.Regex.IsMatch(message, "\\p{IsKatakana}"))
                {
                    isJapanese = true;
                }
                if (System.Text.RegularExpressions.Regex.IsMatch(message, "\\p{IsCJKUnifiedIdeographs}"))
                {
                    isJapanese = true;
                }

                string translateURL;
                if (isJapanese)
                {
                    translateURL = "http://translate.livedoor.com/get/?trns_type=2,1&k=translate&ie=utf8&src_text=" + System.Web.HttpUtility.UrlEncode(message);
                }
                else
                {
                    translateURL = "http://translate.livedoor.com/get/?trns_type=1,2&k=translate&ie=utf8&src_text=" + System.Web.HttpUtility.UrlEncode(message);
                }
                //translateURL = "http://www.excite.co.jp/world/english/?wb_lp=ENJA&before=hi";
                //translateURL = "http://www.excite.co.jp/world/english/?wb_lp=JAEN&before=" + System.Web.HttpUtility.UrlEncode("");
                System.Net.HttpWebRequest request = null;
                System.Net.HttpWebResponse response = null;
                request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(translateURL);
                request.Timeout = 5000;
                request.ReadWriteTimeout = 20000;
                response = (System.Net.HttpWebResponse)request.GetResponse();
                //response.ContentEncoding = System.Text.Encoding.GetEncoding("shift-jis");
                System.IO.Stream str = response.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(str, System.Text.Encoding.GetEncoding("utf-8"));
                string text = sr.ReadToEnd();

                System.Text.RegularExpressions.Regex regAfter = new System.Text.RegularExpressions.Regex(
                        "<textarea [^>].*tar_text.*>(?<text>.*?)</textarea>", System.Text.RegularExpressions.RegexOptions.None);
                System.Text.RegularExpressions.Match m = regAfter.Match(text);
                m = regAfter.Match(text);
                string after = "";
                while (m.Success)
                {
                    string field = m.Groups["text"].Value;
                    if (field.Length > 0) after = field;
                    m = m.NextMatch();
                }
                int index = fromname.IndexOf(' ');
                fromname = fromname.Substring(0, index);
                client_.Self.Chat(fromname + " said: " + after, 0, ChatType.Normal);
            }
            catch (Exception e)
            {
                client_.Self.Chat("エラー:" + e.Message, 0, ChatType.Normal);
            }
        }
    }
}
