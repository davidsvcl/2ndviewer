using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;
using libsecondlife;

namespace _2ndviewer
{
    public partial class MainForm : Form
    {
        public WeifenLuo.WinFormsUI.Docking.DockPanel panel_;
        public ChatForm chatForm_;
        public DebugForm debugForm_;
        public FriendForm friendForm_;
        public GroupForm groupForm_;
        public InventoryForm inventoryForm_;
        public MinimapForm minimapForm_;
        public MovementForm movementForm_;
        public ObjectForm objectForm_;
        public SecondLife client_;
        private delegate void SetStatusTextDelegate(string str);
        private int firstOne;
        private string last_getAvatarName;

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

            client_ = new SecondLife();
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
            }
            else
            {
                nickName = StringResource.defaultNickName;
                Microsoft.Win32.RegistryKey regkey_w = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\2ndviewer");
                regkey_w.SetValue("nickName", nickName);

                news4vip = StringResource.defaultNews4VipUrl;
                regkey_w.SetValue("news4vip", news4vip);
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(StringResource.exitMessage, "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
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
                        client_.Friends.AcceptFriendship(im.FromAgentID, im.IMSessionID);
                        break;
                    }
                case InstantMessageDialog.GroupInvitation:
                    {
                        client_.Self.InstantMessage(client_.Self.Name, im.FromAgentID, string.Empty, im.IMSessionID, InstantMessageDialog.GroupInvitationAccept, InstantMessageOnline.Offline, client_.Self.SimPosition, LLUUID.Zero, new byte[0]);
                        break;
                    }
                case InstantMessageDialog.RequestTeleport:
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

        private void Self_OnScriptDialog(string message, string objectName, LLUUID imageID, LLUUID objectID, string firstName, string lastName, int chatChannel, List<string> buttons)
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
        }

        private void Self_OnScriptQuestion(Simulator simulator, LLUUID taskID, LLUUID itemID, string objectName, string objectOwner, ScriptPermission questions)
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
            chatForm_.SystemMessage("\r\n<Alert>" + message);
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
                    LLVector3 pos = av.Position;
                    float distance = 20.0f;
                    if (LLVector3.Dist(pos, client_.Self.SimPosition) < distance)
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
                LLVector3 pos = prim.Position;
                float distance = 10.0f;
                if (LLVector3.Dist(pos, client_.Self.SimPosition) < distance)
                {
                    if (movementForm_.sit_on_ == true)
                    {
                        client_.Self.RequestSit(prim.ID, LLVector3.Zero);
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
                    LLVector3 pos = av.Position;
                    float followDistance = 0.5f;
                    if (movementForm_.boxing_ == true)
                    {
                        movementForm_.boxing();
                    }
                    if (LLVector3.Dist(pos, client_.Self.SimPosition) > followDistance)
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
        //void Avatars_OnAvatarProperties(LLUUID avatarID, Avatar.AvatarProperties properties)
        //{
        //    chatForm_.SystemMessage("\r\n" + properties.AboutText);
        //    chatForm_.SystemMessage("\r\n" + properties.ProfileURL);
        //    chatForm_.SystemMessage("\r\n" + properties.BornOn);
        //}

        void Groups_OnCurrentGroups(Dictionary<LLUUID, Group> groups)
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
            }
        }

        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog(this);
        }

    }
}
