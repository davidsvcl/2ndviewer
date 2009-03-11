using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using WeifenLuo.WinFormsUI;
using OpenMetaverse;
using OpenMetaverse.Packets;

namespace _2ndviewer
{
    /// <summary>
    /// アプリケーションのメインウィンドウクラス
    /// メインウィンドウの中に子ウィンドウが入っています。
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>DockWindowを収めるパネル</summary>
        public WeifenLuo.WinFormsUI.Docking.DockPanel panel_;
        /// <summary>ランドマーク(ローカル保存)用のXmlDocument</summary>
        private XmlDocument xmldoc_;
        /// <summary>ランドマーク(ローカル保存)用のルートノード</summary>
        private XmlElement rootNode_;
        /// <summary>チャットウィンドウ</summary>
        public ChatForm chatForm_;
        /// <summary>デバッグウィンドウ</summary>
        public DebugForm debugForm_;
        /// <summary>フレンドウィンドウ</summary>
        public FriendForm friendForm_;
        /// <summary>グループウィンドウ</summary>
        public GroupForm groupForm_;
        /// <summary>アイテムウィンドウ</summary>
        public InventoryForm inventoryForm_;
        /// <summary>ミニマップウィンドウ</summary>
        public MinimapForm minimapForm_;
        /// <summary>コントロール用ウィンドウ</summary>
        public MovementForm movementForm_;
        /// <summary>オブジェクトウィンドウ</summary>
        public ObjectForm objectForm_;
        /// <summary>アバター一覧ウィンドウ</summary>
        public AvatarForm avatarForm_;
        /// <summary>3Dレンダリングウィンドウ</summary>
        public RenderForm renderForm_;
        /// <summary>
        /// Second Lifeグリッド通信ライブラリ
        /// ここで作成したインスタンスを他のウィンドウに渡すことで通信を行います
        /// </summary>
        public GridClient client_;
        /// <summary>ステータスバーにテキストをセットするためのデリゲート</summary>
        private delegate void SetStatusTextDelegate(string str);
        /// <summary>スクリプトダイアログを表示するためのデリゲート</summary>
        private delegate void ShowScriptDialogDelegate(string message, int chatChannel, List<string> buttons);
        /// <summary>Objects_OnObjectUpdatedが一回呼ばれるまでは0、一回呼ばれたら1</summary>
        private int firstOne;
        /// <summary>デバッグウィンドウに最後にセットしたアバター</summary>
        private string last_getAvatarName;
        /// <summary>ランドマーク配列</summary>
        private System.Collections.Generic.List<LandmarkList> landmark_array_;
        /// <summary>メッセージボックスを表示(1)/非表示(0)</summary>
        public int confirm_messageBox;

        /// <summary>
        /// コンストラクタ
        /// 以下の動作をアプリケーションの初期化として行います。
        /// ・子ウィンドウをドッキングパネルにするためのパネルの生成
        /// ・Second Lifeグリッド通信ライブラリインスタンスの生成
        /// ・Second Lifeグリッド通信ライブラリのコールバック(イベントハンドラー)登録
        /// ・レジストリの読み込み
        /// ・子ウィンドウの生成
        /// </summary>
        public MainForm()
        {
            // Localizeテストコード
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            InitializeComponent();

            SuspendLayout();
            panel_ = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            panel_.Parent = this;
            panel_.Dock = System.Windows.Forms.DockStyle.Fill;
            panel_.BringToFront();
            ResumeLayout();

            landmark_array_ = new System.Collections.Generic.List<LandmarkList>();
            xmldoc_ = new XmlDocument();
            makeLandmark();

            client_ = new GridClient();
            client_.Settings.MULTIPLE_SIMS = false;
            client_.Settings.ALWAYS_DECODE_OBJECTS = true;
            client_.Settings.ALWAYS_REQUEST_OBJECTS = true;
            client_.Settings.SEND_AGENT_UPDATES = true;
            client_.Settings.DISABLE_AGENT_UPDATE_DUPLICATE_CHECK = true;
            client_.Settings.USE_TEXTURE_CACHE = true;
            client_.Settings.TEXTURE_CACHE_DIR = Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "cache";
            // Crank up the throttle on texture downloads
            client_.Throttle.Texture = 446000.0f;
            client_.Settings.ALWAYS_REQUEST_PARCEL_ACL = false;
            client_.Settings.ALWAYS_REQUEST_PARCEL_DWELL = false;
            //client_.Settings.OBJECT_TRACKING = false; // We use our own object tracking system
            client_.Settings.AVATAR_TRACKING = true; //but we want to use the libsl avatar system
            client_.Network.OnConnected += new NetworkManager.ConnectedCallback(Network_OnConnected);
            client_.Network.OnDisconnected += new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client_.Self.OnInstantMessage += new AgentManager.InstantMessageCallback(Self_OnInstantMessage);
            client_.Self.OnTeleport += new AgentManager.TeleportCallback(Self_OnTeleport);
            client_.Self.OnScriptDialog += new AgentManager.ScriptDialogCallback(Self_OnScriptDialog);
            client_.Self.OnScriptQuestion += new AgentManager.ScriptQuestionCallback(Self_OnScriptQuestion);
            client_.Self.OnAlertMessage += new AgentManager.AlertMessageCallback(Self_OnAlertMessage);
            client_.Objects.OnObjectUpdated += new ObjectManager.ObjectUpdatedCallback(Objects_OnObjectUpdated);
//            client_.Objects.OnNewPrim += new ObjectManager.NewPrimCallback(Objects_OnNewPrim);
//            client_.Objects.OnObjectPropertiesFamily += new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);
            client_.Friends.OnFriendOnline += new FriendsManager.FriendOnlineEvent(Friends_OnFriendOnline);
            client_.Friends.OnFriendOffline += new FriendsManager.FriendOfflineEvent(Friends_OnFriendOffline);
            // ProfileFormへ移動
            //client_.Avatars.OnAvatarProperties += new AvatarManager.AvatarPropertiesCallback(Avatars_OnAvatarProperties);
            client_.Groups.OnCurrentGroups += new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
            client_.Groups.OnGroupRoles += new GroupManager.GroupRolesCallback(Groups_OnGroupRoles);
            client_.Network.OnEventQueueRunning += new NetworkManager.EventQueueRunningCallback(Network_OnEventQueueRunning);
            client_.Parcels.OnParcelProperties += new ParcelManager.ParcelPropertiesCallback(Parcels_OnParcelProperties);
            client_.Network.RegisterCallback(PacketType.AvatarAppearance, new NetworkManager.PacketCallback(AvatarAppearanceHandler));
            client_.Network.OnCurrentSimChanged += new NetworkManager.CurrentSimChangedCallback(Network_OnCurrentSimChanged);

            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\2ndviewer", false);
            string nickName;
            string news4vip;
            if (regkey != null)
            {
                nickName = (string)regkey.GetValue("nickName");
                if (nickName == null) {
                    nickName = StringResource.defaultNickName;
                    Microsoft.Win32.RegistryKey regkey_w = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\2ndviewer");
                    regkey_w.SetValue("nickName", nickName);
                }
                news4vip = (string)regkey.GetValue("news4vip");
                if (news4vip == null)
                {
                    news4vip = StringResource.defaultNews4VipUrl;
                    Microsoft.Win32.RegistryKey regkey_w = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\2ndviewer");
                    regkey_w.SetValue("news4vip", news4vip);
                }
                confirm_messageBox = (int)regkey.GetValue("confirmMessageBox", 1);
            }
            else
            {
                nickName = StringResource.defaultNickName;
                Microsoft.Win32.RegistryKey regkey_w = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\2ndviewer");
                regkey_w.SetValue("nickName", nickName);

                news4vip = StringResource.defaultNews4VipUrl;
                regkey_w.SetValue("news4vip", news4vip);

                confirm_messageBox = 1;
                regkey_w.SetValue("confirmMessageBox",1);
            }

            movementForm_ = new MovementForm();
            movementForm_.SetClient(client_);
            movementForm_.TabText = "Movement";
            movementForm_.Show(panel_, WeifenLuo.WinFormsUI.Docking.DockState.Document);

            inventoryForm_ = new InventoryForm();
            inventoryForm_.SetClient(client_);
            inventoryForm_.TabText = "Inventory";
            inventoryForm_.Show(panel_, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
            
            chatForm_ = new ChatForm();
            chatForm_.SetClient(client_);
            chatForm_.SetMovementForm(movementForm_);
            chatForm_.SetInventoryForm(inventoryForm_);
            chatForm_.SetNickName(nickName);
            chatForm_.SetNews4Vip(news4vip);
            chatForm_.TabText = "Chat";
            panel_.DockBottomPortion = 0.43;
            chatForm_.Show(panel_, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            client_.Self.OnChat += new AgentManager.ChatCallback(chatForm_.Self_OnChat);

            minimapForm_ = new MinimapForm();
            minimapForm_.SetClient(client_);
            minimapForm_.TabText = "MiniMap";
            minimapForm_.Show(panel_, WeifenLuo.WinFormsUI.Docking.DockState.Document);

            objectForm_ = new ObjectForm();
            objectForm_.SetClient(client_);
            objectForm_.TabText = "Objects";
            objectForm_.Show(panel_, WeifenLuo.WinFormsUI.Docking.DockState.Document);

            groupForm_ = new GroupForm();
            groupForm_.SetClient(client_);
            groupForm_.TabText = "Group";
            groupForm_.Show(panel_, WeifenLuo.WinFormsUI.Docking.DockState.Document);

            avatarForm_ = new AvatarForm();
            avatarForm_.SetClient(client_);
            avatarForm_.SetChatForm(chatForm_);
            avatarForm_.SetMovementForm(movementForm_);
            avatarForm_.TabText = "Avatars";
            avatarForm_.Show(panel_, WeifenLuo.WinFormsUI.Docking.DockState.Document);

            debugForm_ = new DebugForm();
            debugForm_.TabText = "Debug";
            debugForm_.Show(panel_, WeifenLuo.WinFormsUI.Docking.DockState.Document);

            friendForm_ = new FriendForm();
            friendForm_.Show(panel_, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
            friendForm_.SetChatForm(chatForm_);
            friendForm_.TabText = "Friend";
            friendForm_.SetClient(client_);

            if (DialogResult.Yes == MessageBox.Show("Open Render Window?", "Question", MessageBoxButtons.YesNo))
            {
                renderForm_ = new RenderForm();
                renderForm_.Show();
                renderForm_.SetMainForm(this);
                renderForm_.Text = "Render";
                renderForm_.SetClient(client_);
            }

            firstOne = 0;
            LoginForm loginForm = new LoginForm();
            loginForm.SetClient(client_);
            loginForm.ShowDialog();
        }

        /// <summary>
        /// Network_OnCurrentSimChanged
        /// アバターのいるシムが変わった時に呼び出されるメソッドです。
        /// </summary>
        void Network_OnCurrentSimChanged(Simulator PreviousSimulator)
        {
            if (renderForm_ != null)
            {
                if (renderForm_.Visible == true)
                {
                    renderForm_.TextureDownloaderReset();
                    renderForm_.InitLists();
                }
            }
        }

        /// <summary>
        /// Parcels_OnParcelProperties
        /// 音楽変更のトリガーが呼び出されるメソッドです(調査中)。
        /// 再生する音楽のURLを変更します
        /// </summary>
        void Parcels_OnParcelProperties(Parcel parcel, ParcelManager.ParcelResult result, int sequenceID, bool snapSelection)
        {
            System.Diagnostics.Trace.WriteLine(parcel.MusicURL);
            movementForm_.SetMusicURL(parcel.MusicURL);
        }

        /// <summary>
        /// AvatarAppearanceHandler
        /// アバターの容姿に関わる動作で呼び出されるメソッドです(調査中)。
        /// </summary>
        private void AvatarAppearanceHandler(Packet packet, Simulator simulator)
        {
            AvatarAppearancePacket appearance = (AvatarAppearancePacket)packet;

            lock (chatForm_.Appearances) chatForm_.Appearances[appearance.Sender.ID] = appearance;
        }

        /// <summary>
        /// MainForm_FormClosing
        /// メインウィンドウが閉じられようとした時に呼び出されるメソッドです。
        /// ランドマーク用のXMLをファイルに保存します
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(StringResource.exitMessage, "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    xmldoc_.Save("landmark.xml");
                }
                catch
                {
                    MessageBox.Show("landmark.xml save error!");
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// MainForm_FormClosed
        /// メインウィンドウが閉じられた後呼び出されるメソッドです。
        /// Second Lifeグリッド通信ライブラリに対してログアウトを指示します
        /// </summary>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            client_.Network.Logout();
        }

        /// <summary>
        /// Network_OnConnected
        /// グリッドへ接続された時に呼び出されるメソッドです。
        /// Second Lifeグリッド通信ライブラリに対して所持金の精算とアバター容姿のセットを指示します
        /// </summary>
        void Network_OnConnected(object sender)
        {
            client_.Self.RequestBalance();
            client_.Appearance.SetPreviousAppearance(false);
        }

        /// <summary>
        /// Network_OnDisconnected
        /// 切断された時に呼び出されるメソッドです。
        /// </summary>
        void Network_OnDisconnected(NetworkManager.DisconnectType reason, string message)
        {
            MessageBox.Show(StringResource.disconnected + message, "", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Self_OnInstantMessage
        /// InstantMessageを受け取った時呼び出されるメソッドです。
        /// ※ここで言うInstatMessageとはIMだけでなく、フレンドシップオファーなども含まれるため注意が必要です
        /// </summary>
        void Self_OnInstantMessage(InstantMessage im, Simulator simulator)
        {
            switch (im.Dialog)
            {
                case InstantMessageDialog.MessageFromAgent:
                    {
                        chatForm_.IMChat(im.Message, im.FromAgentName, im.FromAgentID, im.IMSessionID);
                        break;
                    }
                case InstantMessageDialog.FriendshipOffered:
                    {
                        if (confirm_messageBox == 1)
                        {
                            if (MessageBox.Show(im.FromAgentName, "Friendship Offered", MessageBoxButtons.YesNo) != DialogResult.Yes) break;
                        }
                        client_.Friends.AcceptFriendship(im.FromAgentID, im.IMSessionID);
                        break;
                    }
                case InstantMessageDialog.GroupInvitation:
                    {
                        if (confirm_messageBox == 1)
                        {
                            if (MessageBox.Show("from:" + im.FromAgentName + " message:" + im.Message, "Group Invite", MessageBoxButtons.YesNo) != DialogResult.Yes) break;
                        }
                        client_.Self.InstantMessage(client_.Self.Name, im.FromAgentID, string.Empty, im.IMSessionID, InstantMessageDialog.GroupInvitationAccept, InstantMessageOnline.Offline, client_.Self.SimPosition, UUID.Zero, new byte[0]);
                        break;
                    }
                case InstantMessageDialog.RequestTeleport:
                    if (confirm_messageBox == 1)
                    {
                        if (MessageBox.Show("from:" + im.FromAgentName + " message:" + im.Message, "Request Teleport", MessageBoxButtons.YesNo) != DialogResult.Yes) break;
                    }
                    client_.Self.TeleportLureRespond(im.FromAgentID, true);
                    break;
                case InstantMessageDialog.GroupNotice:
                    chatForm_.SystemMessage("\r\n<GroupNotice>" + im.Message);
                    break;
                case InstantMessageDialog.MessageFromObject:
                    chatForm_.SystemMessage("\r\n<Message from Object>" + im.Message);
                    break;
                case InstantMessageDialog.MessageBox:
                    chatForm_.SystemMessage("\r\n<MessageBox>" + im.Message);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Self_OnTeleport
        /// テレポート時に呼び出されるメソッドです。
        /// Second Lifeグリッド通信ライブラリに対して所持金の精算とアバター容姿のセットを指示します
        /// ステータスバーのテキストを書き換えます
        /// ※テレポート開始、テレポート停止など、一度のテレポートで複数回呼ばれるため注意が必要です
        /// </summary>
        private void Self_OnTeleport(string message, AgentManager.TeleportStatus status, AgentManager.TeleportFlags flags)
        {
            //chatForm_.SystemMessage(message);
            if (status == AgentManager.TeleportStatus.Finished)
            {
                client_.Self.RequestBalance();
                client_.Appearance.SetPreviousAppearance(false);
                string mes = "";
                mes += "SIM:" + client_.Network.CurrentSim.Name;
                mes += "<" + ((int)client_.Self.SimPosition.X).ToString();
                mes += "," + ((int)client_.Self.SimPosition.Y).ToString();
                mes += "," + ((int)client_.Self.SimPosition.Z).ToString();
                mes += ">";
                chatForm_.SystemMessage(mes);
            }
        }

        /// <summary>
        /// Self_OnScriptDialog
        /// スクリプトによるダイアログ表示で呼び出されるメソッドです。
        /// メッセージ内容とボタンをダイアログボックスに表示します
        /// </summary>
        private void Self_OnScriptDialog(string message, string objectName, UUID imageID, UUID objectID, string firstName, string lastName, int chatChannel, List<string> buttons)
        {
            //throw new NotImplementedException();
            chatForm_.SystemMessage("dialog message:" + message);
            chatForm_.SystemMessage("channel:" + chatChannel);
            string mes = "";
            for (int i = 0; i < buttons.Count; i++)
            {
                mes += buttons[i] + ",";
            }
            chatForm_.SystemMessage("buttons:" + mes);
            chatForm_.SystemMessage("button click command is \"/" + chatChannel + " button\"");
            //client_.Self.Chat("buttonname", chatChannel, ChatType.Normal);
            ShowScriptDialogDelegate dlg = new ShowScriptDialogDelegate(ShowScriptDialog);
            object[] dlgarg = {message, chatChannel, buttons};
            Invoke(dlg, dlgarg);
        }

        /// <summary>
        /// ShowScriptDialog
        /// スクリプトダイアログをダイアログボックス表示するメソッドです。
        /// </summary>
        private void ShowScriptDialog(string message, int chatChannel, List<string> buttons)
        {
            ScriptDialog scriptDialog = new ScriptDialog();
            scriptDialog.SetClient(client_);
            scriptDialog.SetObjects(message, chatChannel, buttons);
            scriptDialog.Show(this);
        }

        /// <summary>
        /// Self_OnScriptQuestion
        /// スクリプトによるダイアログ表示で呼び出されるメソッドです。
        /// 主にセキュリティに関わるダイアログです
        /// メッセージボックスを表示します
        /// </summary>
        private void Self_OnScriptQuestion(Simulator simulator, UUID taskID, UUID itemID, string objectName, string objectOwner, ScriptPermission questions)
        {
            System.Diagnostics.Trace.WriteLine("OnScriptQuestion");
            string mes = objectOwner + "s object " + objectName + " ";
            switch (questions)
            {
                case ScriptPermission.None:
                    mes += "None";
                    break;
                case ScriptPermission.Debit:
                    mes += "Debit";
                    break;
                case ScriptPermission.TakeControls:
                    mes += "TakeControls";
                    break;
                case ScriptPermission.RemapControls:
                    mes += "RemapControls";
                    break;
                case ScriptPermission.TriggerAnimation:
                    mes += "TriggerAnimation";
                    break;
                case ScriptPermission.Attach:
                    mes += "Attach";
                    break;
                case ScriptPermission.ReleaseOwnership:
                    mes += "ReleaseOwnership";
                    break;
                case ScriptPermission.ChangeLinks:
                    mes += "ChangeLinks";
                    break;
                case ScriptPermission.ChangeJoints:
                    mes += "ChangeJoints";
                    break;
                case ScriptPermission.ChangePermissions:
                    mes += "ChangePermissions";
                    break;
                case ScriptPermission.TrackCamera:
                    mes += "TrackCamera";
                    break;
                case ScriptPermission.ControlCamera:
                    mes += "ControlCamera";
                    break;
                default:
                    mes += "...";
                    break;
            }
            if (MessageBox.Show(mes, "Script Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                client_.Self.ScriptQuestionReply(simulator, itemID, taskID, questions);
            }
        }

        /// <summary>
        /// Self_OnAlertMessage
        /// アラートを受け取った時に呼び出されるメソッドです。
        /// </summary>
        private void Self_OnAlertMessage(string message)
        {
            if (!message.StartsWith("Autopilot canceled")) chatForm_.SystemMessage("<Alert>" + message);
        }

        /// <summary>
        /// SetStatusText
        /// ステータスバーにテキストをセットするメソッドです。
        /// </summary>
        private void SetStatusText(string message)
        {
            toolStripStatusLabel1.Text = message;
        }

        /// <summary>
        /// Objects_OnObjectUpdated
        /// オブジェクトに変化が起きた時に呼び出されるメソッドです。
        /// ストーキングを行います
        /// ※オブジェクトに変化が起きたときというのは、オブジェクトまたはアバターが移動した事または何かしらの動作が起きたこと事を指します(調査中)
        /// </summary>
        void Objects_OnObjectUpdated(Simulator simulator, ObjectUpdate update, ulong regionHandle, ushort timeDilation)
        {
            if (firstOne == 0)
            {
                friendForm_.refresh();
                inventoryForm_.InventoryInitialize();
                firstOne = 1;
            }
            Avatar av;
            client_.Network.CurrentSim.ObjectsAvatars.TryGetValue(update.LocalID, out av);
            if (av != null)
            {
                if (av.Name.Length > 0)
                {
                    if (minimapForm_.filter_selected == 0)
                    {
                        minimapForm_.printMap(false);
                    }
                    Vector3 pos = av.Position;
                    float distance = 20.0f;
                    if (Vector3.Distance(pos, client_.Self.SimPosition) < distance)
                    {
                        if (debugForm_ != null)
                        {
                            if (debugForm_.filter_selected == 0)
                            {
                                if (last_getAvatarName != av.Name)
                                {
                                    last_getAvatarName = av.Name;
                                    debugForm_.DebugLog(StringResource.chatLocAv + av.Name);
                                }
                            }
                        }
                    }
                }
                SetStatusTextDelegate dlg = new SetStatusTextDelegate(SetStatusText);
                string message = "L$:" + client_.Self.Balance.ToString();
                message += " SIM:" + client_.Network.CurrentSim.Name;
                message += "<" + ((int)client_.Self.SimPosition.X).ToString();
                message += "," + ((int)client_.Self.SimPosition.Y).ToString();
                message += "," + ((int)client_.Self.SimPosition.Z).ToString();
                message += ">";
                string[] arg = { message };
                try
                {
                    Invoke(dlg, arg);
                }
                catch
                {
                }
            }

            Primitive prim;
            client_.Network.CurrentSim.ObjectsPrimitives.TryGetValue(update.LocalID, out prim);
            if (prim != null)
            {
                Vector3 pos = prim.Position;
                float distance = 10.0f;
                if (Vector3.Distance(pos, client_.Self.SimPosition) < distance)
                {
                    if (movementForm_.sit_on_ == true)
                    {
                        client_.Self.RequestSit(prim.ID, Vector3.Zero);
                        client_.Self.Sit();
                    }
                }
            }

            if (movementForm_.follow_on_ == true)
            {
                if (!update.Avatar)
                {
                    return;
                }
                if (av == null) return;
                if (av.Name == movementForm_.followName_) {
                    Vector3 pos = av.Position;
                    float followDistance = 0.5f;
                    if (movementForm_.boxing_ == true)
                    {
                        movementForm_.boxing();
                    }
                    if (Vector3.Distance(pos, client_.Self.SimPosition) > followDistance)
                    {
                        int followRegionX = (int)(regionHandle >> 32);
                        int followRegionY = (int)(regionHandle & 0xFFFFFFFF);
                        int followRegionZ = (int)(regionHandle);
                        ulong x = (ulong)(pos.X + followRegionX);
                        ulong y = (ulong)(pos.Y + followRegionY);
                        if (movementForm_.boxing_ == true)
                        {
                            movementForm_.boxing();
                        }
                        client_.Self.AutoPilotLocal(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), pos.Z);
                        client_.Self.Movement.TurnToward(pos);
                    }
                    else
                    {
                        client_.Self.AutoPilotCancel();
                    }
                }
            }
        }

        /// <summary>
        /// Friends_OnFriendOnline
        /// フレンドがログインした時に呼び出されるメソッドです。
        /// </summary>
        void Friends_OnFriendOnline(FriendInfo friend)
        {
            chatForm_.SystemMessage("\r\n" + friend.Name + StringResource.onlineMessage);
            friendForm_.refresh();
        }

        /// <summary>
        /// Friends_OnFriendOffline
        /// フレンドがログアウトした時に呼び出されるメソッドです。
        /// </summary>
        void Friends_OnFriendOffline(FriendInfo friend)
        {
            chatForm_.SystemMessage("\r\n" + friend.Name + StringResource.offlineMessage);
            friendForm_.refresh();
        }

        // ProfileFormへ移動
        //void Avatars_OnAvatarProperties(UUID avatarID, Avatar.AvatarProperties properties)
        //{
        //    chatForm_.SystemMessage("\r\n" + properties.AboutText);
        //    chatForm_.SystemMessage("\r\n" + properties.ProfileURL);
        //    chatForm_.SystemMessage("\r\n" + properties.BornOn);
        //}

        void Groups_OnGroupRoles(Dictionary<UUID, GroupRole> roles)
        {
            foreach (GroupRole role in roles.Values)
            {
                System.Diagnostics.Trace.WriteLine("RoleName:" + role.Name);
                System.Diagnostics.Trace.WriteLine("RoleDescription:" + role.Description);
                System.Diagnostics.Trace.WriteLine("RoleTitle:" + role.Title);
                System.Diagnostics.Trace.WriteLine("RoleID:" + role.ID);
            }
        }

        /// <summary>
        /// Groups_OnCurrentGroups
        /// 調査中
        /// </summary>
        void Groups_OnCurrentGroups(Dictionary<UUID, Group> groups)
        {
            foreach(Group group in groups.Values) {
                System.Diagnostics.Trace.WriteLine("group name:" + group.Name);
                System.Diagnostics.Trace.WriteLine("group uuid:" + group.ID);
                client_.Groups.RequestGroupRoles(group.ID);
            }
            groupForm_.Groups_ = groups;
            Invoke(new MethodInvoker(groupForm_.UpdateGroups));
        }

        /// <summary>
        /// Network_OnEventQueueRunning
        /// 調査中
        /// </summary>
        void Network_OnEventQueueRunning(Simulator simulator)
        {
            if (simulator == client_.Network.CurrentSim)
            {
                client_.Groups.RequestCurrentGroups();
            }
        }

//        void Objects_OnNewPrim(Simulator simulator, Primitive prim, ulong regionHandle, ushort timeDilation)
//        {
//            client_.Objects.RequestObjectPropertiesFamily(simulator, prim.ID);
//        }

//        void Objects_OnObjectPropertiesFamily(Simulator simulator, LLObject.ObjectPropertiesFamily properties)
//        {
//            //properties.ObjectID
//            //System.Diagnostics.Trace.WriteLine(properties.Name + "," + properties.LastOwnerID);
//        }

        /// <summary>
        /// exitToolStripMenuItem_Click
        /// メニューから終了を選択した時に呼ばれるメソッドです。
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// addLandmarkToolStripMenuItem_Click
        /// メニューからランドマークを追加を選択した時に呼ばれるメソッドです。
        /// ランドマーク追加用ダイアログを表示し、XMLノードの追加、メニューへの追加を行います
        /// </summary>
        private void addLandmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LandmarkForm landmarkForm = new LandmarkForm();
            landmarkForm.name_textBox.Text = "Mandatory";
            landmarkForm.simname_textBox.Text = client_.Network.CurrentSim.Name;
            landmarkForm.x_textBox.Text = ((int)client_.Self.SimPosition.X).ToString();
            landmarkForm.y_textBox.Text = ((int)client_.Self.SimPosition.Y).ToString();
            landmarkForm.z_textBox.Text = ((int)client_.Self.SimPosition.Z).ToString();
            landmarkForm.memo_textBox.Text = "";
            if (DialogResult.OK == landmarkForm.ShowDialog(this))
            {
                XmlNode myNode = xmldoc_.CreateNode(XmlNodeType.Element, "location", "");
                XmlAttribute attr_title = xmldoc_.CreateAttribute("title");
                attr_title.Value = landmarkForm.name_textBox.Text;
                myNode.Attributes.Append(attr_title);
                rootNode_.AppendChild(myNode);
                XmlNode locNode = xmldoc_.CreateNode(XmlNodeType.Element, "sim", "");
                locNode.InnerText = landmarkForm.simname_textBox.Text;
                myNode.AppendChild(locNode);
                XmlNode xNode = xmldoc_.CreateNode(XmlNodeType.Element, "RegionX", "");
                xNode.InnerText = landmarkForm.x_textBox.Text;
                myNode.AppendChild(xNode);
                XmlNode yNode = xmldoc_.CreateNode(XmlNodeType.Element, "RegionY", "");
                yNode.InnerText = landmarkForm.y_textBox.Text;
                myNode.AppendChild(yNode);
                XmlNode zNode = xmldoc_.CreateNode(XmlNodeType.Element, "RegionZ", "");
                zNode.InnerText = landmarkForm.x_textBox.Text;
                myNode.AppendChild(zNode);
                XmlNode textNode = xmldoc_.CreateNode(XmlNodeType.Element, "text", "");
                textNode.InnerText = landmarkForm.memo_textBox.Text;
                myNode.AppendChild(textNode);

                LandmarkList landmark = new LandmarkList();
                landmark.name = landmarkForm.name_textBox.Text;
                landmark.simName = landmarkForm.simname_textBox.Text;
                landmark.x = float.Parse(landmarkForm.x_textBox.Text);
                landmark.y = float.Parse(landmarkForm.y_textBox.Text);
                landmark.z = float.Parse(landmarkForm.z_textBox.Text);
                landmark_array_.Add(landmark);

                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = landmarkForm.name_textBox.Text;
                item.Click += new EventHandler(landmark_Click);
                landmarkToolStripMenuItem.DropDownItems.Add(item);
            }

        }

        /// <summary>
        /// landmark_Click
        /// メニューからランドマークを選択した時に呼ばれるメソッドです。
        /// テレポートを行います
        /// 名前で探すため、同一名称のランドマークは使用できません
        /// </summary>
        void landmark_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(sender.ToString());
            LandmarkList landmark = null;
            for (int i = 0; i < landmark_array_.Count; i++)
            {
                if (landmark_array_[i].name == sender.ToString())
                {
                    landmark = landmark_array_[i];
                    break;
                }
            }
            if (landmark == null) return;
            System.Threading.Thread process = new System.Threading.Thread(
                delegate()
                {
                    try
                    {
                        client_.Self.Teleport(landmark.simName, new Vector3(landmark.x, landmark.y, landmark.z));
                    }
                    catch
                    {
                        MessageBox.Show(StringResource.failedTeleport, "Error");
                    }
                });
            process.Start();
        }

        /// <summary>
        /// SearchToolStripMenuItem_Click
        /// メニューから検索を選択した時に呼ばれるメソッドです。
        /// 標準のウェブブラウザーを表示しSecondLifeの検索ページを表示します
        /// </summary>
        private void SearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://search.secondlife.com/search.php");
            }
            catch
            {
            }
        }

        /// <summary>
        /// optionToolStripMenuItem_Click
        /// メニューからオプションを選択した時に呼ばれるメソッドです。
        /// オプション設定ダイアログを表示します
        /// </summary>
        private void optionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionForm optionForm = new OptionForm();
            if (DialogResult.OK == optionForm.ShowDialog(this))
            {
                Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\2ndviewer");
                regkey.SetValue("nickName", optionForm.nickname_textBox.Text);
                regkey.SetValue("news4vip", optionForm.news4vip_textBox.Text);
                if (chatForm_ != null)
                {
                    chatForm_.SetNickName(optionForm.nickname_textBox.Text);
                    chatForm_.SetNews4Vip(optionForm.news4vip_textBox.Text);
                }
                if (optionForm.confirm_checkBox.Checked) {
                    confirm_messageBox = 1;
                }
                else {
                    confirm_messageBox = 0;
                }
                regkey.SetValue("confirmMessageBox", confirm_messageBox);
            }
        }

        /// <summary>
        /// versionToolStripMenuItem_Click
        /// メニューからバージョン情報を選択した時に呼ばれるメソッドです。
        /// バージョン情報ダイアログを表示します
        /// </summary>
        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog(this);
        }

        /// <summary>
        /// makeLandmarkItem
        /// XMLノードからメニューバーにランドマークを追加するメソッドです。
        /// </summary>
        private void makeLandmarkItem(XmlNode itemNode, System.Windows.Forms.ToolStripMenuItem folderItem)
        {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = itemNode.Attributes["title"].Value;

            LandmarkList landmark = new LandmarkList();
            landmark.name = itemNode.Attributes["title"].Value;

            foreach (XmlNode top_node in itemNode.ChildNodes)
            {
                if (top_node.Name == "sim")
                {
                    System.Diagnostics.Trace.WriteLine(top_node.InnerText);
                    //item.Text = top_node.InnerText;
                    landmark.simName = top_node.InnerText;
                }
                if (top_node.Name == "RegionX")
                {
                    landmark.x = float.Parse(top_node.InnerText);
                }
                if (top_node.Name == "RegionY")
                {
                    landmark.y = float.Parse(top_node.InnerText);
                }
                if (top_node.Name == "RegionZ")
                {
                    landmark.z = float.Parse(top_node.InnerText);
                }
            }

            landmark_array_.Add(landmark);
            item.Click += new EventHandler(landmark_Click);
            folderItem.DropDownItems.Add(item);
        }

        /// <summary>
        /// makeLandmarkFolder
        /// XMLノードからメニューバーにランドマークフォルダーを追加するメソッドです。
        /// </summary>
        private void makeLandmarkFolder(XmlNode rootNode, System.Windows.Forms.ToolStripMenuItem folderItem)
        {
            foreach (XmlNode top_node in rootNode.ChildNodes)
            {
                if (top_node.Name == "location")
                {
                    makeLandmarkItem(top_node, folderItem);
                }
                else
                {
                    ToolStripMenuItem newFolderItem = new ToolStripMenuItem();
                    newFolderItem.Text = top_node.Attributes["title"].Value;                    
                    folderItem.DropDownItems.Add(newFolderItem);
                    makeLandmarkFolder(top_node, newFolderItem);
                }
            }
        }

        /// <summary>
        /// makeLandmark
        /// XMLファイル(ランドマーク)を読み込みメニューバーに登録するメソッドです。
        /// </summary>
        private void makeLandmark()
        {
            try
            {
                xmldoc_.Load("landmark.xml");

                rootNode_ = xmldoc_.DocumentElement;
                makeLandmarkFolder(rootNode_, landmarkToolStripMenuItem);
            }
            catch
            {
                MessageBox.Show("landmark.xml open error!");
            }
        }
    }

    /// <summary>
    /// ランドマーク格納用データクラス
    /// </summary>
    class LandmarkList
    {
        public LandmarkList()
        {
        }
        public string name;
        public string simName;
        public float x;
        public float y;
        public float z;
    }

    /// <summary>
    /// ScriptDialog
    /// スクリプトダイアログをウィンドウダイアログにするクラスです
    /// </summary>
    class ScriptDialog : Form
    {
        /// <summary>Second Lifeグリッド通信ライブラリ</summary>
        private GridClient client_;
        /// <summary>スクリプトの通信チャンネル</summary>
        private int chatChannel;

        /// <summary>コンストラクタ</summary>
        public ScriptDialog()
        {
            this.Text = "Script";
        }
        /// <summary>通信ライブラリをセットする</summary>
        public void SetClient(GridClient client)
        {
            client_ = client;
        }
        /// <summary>コンポーネントを配置する</summary>
        public void SetObjects(string message, int chatChannel, List<string> buttons)
        {
            this.chatChannel = chatChannel;

            Label label = new Label();
            label.Text = message;
            label.Bounds = new Rectangle(10, 10, 100, 20);
            Controls.Add(label);

            int y = 35;
            int x = 10;
            bool flag = false;
            for (int i = 0; i < buttons.Count; i++)
            {
                Button b = new Button();
                b.Text = buttons[i];
                b.Bounds = new Rectangle(x, y, 90, 25);
                b.Click += new EventHandler(button_Click);
                Controls.Add(b);
                if (flag == false)
                {
                    flag = true;
                    x = 110;
                }
                else
                {
                    flag = false;
                    x = 10;
                    y += 30;
                }
            }

        }
        /// <summary>ボタンクリック時のイベント</summary>
        void button_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(sender.ToString());
            Button b = (Button)sender;
            client_.Self.Chat(b.Text, chatChannel, ChatType.Normal);
            Close();
        }
    }
}
