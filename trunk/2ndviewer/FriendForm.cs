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
    public partial class FriendForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private SecondLife client_;
        private ChatForm chatForm_;
        System.Collections.Generic.List<FriendList> friend_array_;
        private delegate void refreshListDelegate(string str);

        public FriendForm()
        {
            InitializeComponent();
            friend_array_ = new System.Collections.Generic.List<FriendList>();
        }

        public void SetClient(SecondLife client)
        {
            client_ = client;
        }

        public void SetChatForm(ChatForm chatForm)
        {
            chatForm_ = chatForm;
        }

        private void refreshList(string str)
        {
            listBox1.Items.Clear();
            friend_array_.Clear();
            if (client_.Friends.FriendList.Count > 0)
            {
                client_.Friends.FriendList.ForEach(delegate(FriendInfo friend)
                {
                    string friend_str;
                    if (friend.IsOnline)
                    {
                        friend_str = "(on)" + friend.Name;
                    }
                    else
                    {
                        friend_str = "(off)" + friend.Name;
                    }
                    listBox1.Items.Add(friend_str);
                    FriendList friendlist = new FriendList();
                    friendlist.UUID = friend.UUID;
                    friendlist.Name = friend.Name;
                    friend_array_.Add(friendlist);
                });
            }
            // old interface
            ////InternalDictionary<LLUUID,libsecondlife.FriendInfo> friends = client_.Friends.FriendList;
            //System.Collections.Generic.List<libsecondlife.FriendInfo> friends = client_.Friends.FriendsList();
            //for (int i = 0; i < friends.Count; i++)
            //{
                //string friend_str;
                //if (friends[i].IsOnline)
                //{
                //    friend_str = "(on)" + friends[i].Name;
                //}
                //else
                //{
                //    friend_str = "(off)" + friends[i].Name;
                //}
                //listBox1.Items.Add(friend_str);
                //FriendList friendlist = new FriendList();
                //friendlist.UUID = friends[i].UUID;
                //friendlist.Name = friends[i].Name;
                //friend_array_.Add(friendlist);
            //}
        }

        public void refresh()
        {
            refreshListDelegate dlg = new refreshListDelegate(refreshList);
            string arg = "";
            Invoke(dlg, arg);
        }

        private void iMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            chatForm_.StartIM(friend_array_[index].UUID, friend_array_[index].Name);
        }

        private void teleportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            client_.Self.SendTeleportLure(friend_array_[index].UUID, "Join me in " + client_.Network.CurrentSim.Name + "!");
        }

        private void profile_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            client_.Avatars.RequestAvatarProperties(friend_array_[index].UUID);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            refresh();
        }
    }
}
