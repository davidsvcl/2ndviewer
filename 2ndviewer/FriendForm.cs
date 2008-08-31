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

namespace _2ndviewer
{
    public partial class FriendForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private GridClient client_;
        private ChatForm chatForm_;
        System.Collections.Generic.List<FriendList> friend_array_;
        private delegate void refreshListDelegate();

        public FriendForm()
        {
            InitializeComponent();
            friend_array_ = new System.Collections.Generic.List<FriendList>();
        }

        public void SetClient(GridClient client)
        {
            client_ = client;
        }

        public void SetChatForm(ChatForm chatForm)
        {
            chatForm_ = chatForm;
        }

        private void refreshList()
        {
            listBox1.Items.Clear();
            friend_array_.Clear();
            if (client_.Friends.FriendList.Count > 0)
            {
                client_.Friends.FriendList.ForEach(delegate(FriendInfo friend)
                {
                    FriendList friendlist = new FriendList();
                    //string friend_str;
                    if (friend.IsOnline)
                    {
                        //friend_str = "(on)" + friend.Name;
                        friendlist.Online = true;
                    }
                    else
                    {
                        //friend_str = "(off)" + friend.Name;
                        friendlist.Online = false;
                    }
                    listBox1.Items.Add(friend.Name);
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
            try
            {
                Invoke(dlg);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());
            }
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
            using (ProfileForm profileForm = new ProfileForm())
            {
                profileForm.SetClient(client_);
                profileForm.SetAvatarID(friend_array_[index].UUID);
                profileForm.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (friend_array_ == null) return;
            if (friend_array_.Count <= 0) return;

            Brush brush = null;
            if (friend_array_[e.Index].Online)
            {
                brush = new SolidBrush(Color.Red);
            }
            else
            {
                if ((e.State & DrawItemState.Selected) != DrawItemState.Selected)
                {
                    brush = new SolidBrush(Color.Black);
                }
                else
                {
                    brush = new SolidBrush(e.ForeColor);
                }
            }
            string name = ((ListBox)sender).Items[e.Index].ToString();
            e.Graphics.DrawString(name, e.Font, brush, e.Bounds);

            brush.Dispose();
            e.DrawFocusRectangle();
        }
    }
}
