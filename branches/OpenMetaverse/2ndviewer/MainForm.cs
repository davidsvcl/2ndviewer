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

namespace _2ndviewer
{
    public partial class MainForm : Form
    {
        public WeifenLuo.WinFormsUI.Docking.DockPanel panel_;
        private XmlDocument xmldoc_;
        private XmlElement rootNode_;
        public ChatForm chatForm_;
        public DebugForm debugForm_;
        public FriendForm friendForm_;
        public GroupForm groupForm_;
        public InventoryForm inventoryForm_;
        public MinimapForm minimapForm_;
        public MovementForm movementForm_;
        public ObjectForm objectForm_;
        public AvatarForm avatarForm_;
        public GridClient client_;
        private delegate void SetStatusTextDelegate(string str);
        private delegate void ShowScriptDialogDelegate(string message, int chatChannel, List<string> buttons);
        private int firstOne;
        private string last_getAvatarName;
        private System.Collections.Generic.List<LandmarkList> landmark_array_;
        public int confirm_messageBox;

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
            client_.Network.OnConnected += new NetworkManager.ConnectedCallback(Network_OnConnected);
            client_.Network.OnDisconnected += new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client_.Self.OnInstantMessage += new AgentManager.InstantMessageCallback(Self_OnInstantMessage);
            client_.Self.OnTeleport += new AgentManager.TeleportCallback(Self_OnTeleport);
            client_.Self.OnScriptDialog += new AgentManager.ScriptDialogCallback(Self_OnScriptDialog);
            client_.Self.OnScriptQuestion += new AgentManager.ScriptQuestionCallback(Self_OnScriptQuestion);
            client_.Self.OnAlertMessage += new AgentManager.AlertMessage(Self_OnAlertMessage);
            client_.Objects.OnObjectUpdated += new ObjectManager.ObjectUpdatedCallback(Objects_OnObjectUpdated);
//            client_.Objects.OnNewPrim += new ObjectManager.NewPrimCallback(Objects_OnNewPrim);
//            client_.Objects.OnObjectPropertiesFamily += new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);
            client_.Friends.OnFriendOnline += new FriendsManager.FriendOnlineEvent(Friends_OnFriendOnline);
            client_.Friends.OnFriendOffline += new FriendsManager.FriendOfflineEvent(Friends_OnFriendOffline);
            // ProfileFormへ移動
            //client_.Avatars.OnAvatarProperties += new AvatarManager.AvatarPropertiesCallback(Avatars_OnAvatarProperties);
            client_.Groups.OnCurrentGroups += new GroupManager.CurrentGroupsCallback(Groups_OnCurrentGroups);
            client_.Network.OnEventQueueRunning += new NetworkManager.EventQueueRunningCallback(Network_OnEventQueueRunning);
            client_.Parcels.OnParcelProperties += new ParcelManager.ParcelPropertiesCallback(Parcels_OnParcelProperties);

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
            
            chatForm_ = new ChatForm();
            chatForm_.SetClient(client_);
            chatForm_.SetMovementForm(movementForm_);
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

            inventoryForm_ = new InventoryForm();
            inventoryForm_.SetClient(client_);
            inventoryForm_.TabText = "Inventory";
            inventoryForm_.Show(panel_, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);

            friendForm_ = new FriendForm();
            friendForm_.Show(panel_, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
            friendForm_.SetChatForm(chatForm_);
            friendForm_.TabText = "Friend";
            friendForm_.SetClient(client_);

            firstOne = 0;
            LoginForm loginForm = new LoginForm();
            loginForm.SetClient(client_);
            loginForm.ShowDialog();
        }

        void Parcels_OnParcelProperties(Parcel parcel, ParcelManager.ParcelResult result, int sequenceID, bool snapSelection)
        {
            System.Diagnostics.Trace.WriteLine(parcel.MusicURL);
            movementForm_.SetMusicURL(parcel.MusicURL);
        }

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

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            client_.Network.Logout();
        }

        void Network_OnConnected(object sender)
        {
            client_.Self.RequestBalance();
            client_.Appearance.SetPreviousAppearance(false);
        }

        void Network_OnDisconnected(NetworkManager.DisconnectType reason, string message)
        {
            MessageBox.Show(StringResource.disconnected + message, "", MessageBoxButtons.OK);
        }

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

        private void ShowScriptDialog(string message, int chatChannel, List<string> buttons)
        {
            ScriptDialog scriptDialog = new ScriptDialog();
            scriptDialog.SetClient(client_);
            scriptDialog.SetObjects(message, chatChannel, buttons);
            scriptDialog.Show(this);
        }

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

        private void Self_OnAlertMessage(string message)
        {
            if (!message.StartsWith("Autopilot canceled")) chatForm_.SystemMessage("<Alert>" + message);
        }

        private void SetStatusText(string message)
        {
            toolStripStatusLabel1.Text = message;
        }

        void Objects_OnObjectUpdated(Simulator simulator, ObjectUpdate update, ulong regionHandle, ushort timeDilation)
        {
            if (firstOne == 0)
            {
                friendForm_.refresh();
                //inventoryForm_.InventoryInitialize();
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
                        client_.Self.AutoPilotCancel();
                        if (pos.Z > 1)
                        {
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
        }

        void Friends_OnFriendOnline(FriendInfo friend)
        {
            chatForm_.SystemMessage("\r\n" + friend.Name + StringResource.onlineMessage);
            friendForm_.refresh();
        }

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

        void Groups_OnCurrentGroups(Dictionary<UUID, Group> groups)
        {
            groupForm_.Groups_ = groups;
            Invoke(new MethodInvoker(groupForm_.UpdateGroups));
        }

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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

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

        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog(this);
        }

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

    class ScriptDialog : Form
    {
        private GridClient client_;
        private int chatChannel;
        public ScriptDialog()
        {
            this.Text = "Script";
        }
        public void SetClient(GridClient client)
        {
            client_ = client;
        }
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
        void button_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(sender.ToString());
            Button b = (Button)sender;
            client_.Self.Chat(b.Text, chatChannel, ChatType.Normal);
            Close();
        }
    }
}
