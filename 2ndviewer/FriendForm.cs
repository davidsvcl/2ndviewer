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
    /// <summary>
    /// フレンドウィンドウクラス
    /// フレンド情報の表示と指定したフレンドに対してコンタクトを行います。
    /// </summary>
    public partial class FriendForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>Second Lifeグリッド通信ライブラリ</summary>
        private GridClient client_;
        /// <summary>チャットウィンドウ</summary>
        private ChatForm chatForm_;
        /// <summary>フレンド配列</summary>
        System.Collections.Generic.List<FriendList> friend_array_;
        /// <summary>リストボックスをリフレッシュするためのデリゲート</summary>
        private delegate void refreshListDelegate();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FriendForm()
        {
            InitializeComponent();
            friend_array_ = new System.Collections.Generic.List<FriendList>();
        }

        /// <summary>通信ライブラリをセットする</summary>
        public void SetClient(GridClient client)
        {
            client_ = client;
        }

        /// <summary>チャットウィンドウをセットする</summary>
        public void SetChatForm(ChatForm chatForm)
        {
            chatForm_ = chatForm;
        }

        /// <summary>
        /// refreshList
        /// フレンドリストを再構築します
        /// </summary>
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

        /// <summary>
        /// refresh
        /// フレンドリストを更新するメソッドです。
        /// </summary>
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

        /// <summary>
        /// iMToolStripMenuItem_Click
        /// ポップアップメニューからIMを選択した時に呼ばれるメソッドです。
        /// </summary>
        private void iMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            chatForm_.StartIM(friend_array_[index].UUID, friend_array_[index].Name);
        }

        /// <summary>
        /// teleportToolStripMenuItem_Click
        /// ポップアップメニューからテレポートを選択した時に呼ばれるメソッドです。
        /// </summary>
        private void teleportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            client_.Self.SendTeleportLure(friend_array_[index].UUID, "Join me in " + client_.Network.CurrentSim.Name + "!");
        }

        /// <summary>
        ///  profile_ToolStripMenuItem_Click
        /// ポップアップメニューからプロフィール表示を選択した時に呼ばれるメソッドです。
        /// </summary>
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

        /// <summary>
        /// button1_Click
        /// 更新ボタンを選択した時に呼ばれるメソッドです。
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            refresh();
        }

        /// <summary>
        /// listBox1_DrawItem
        /// フレンドリストを描画するメソッドです。
        /// Onlineは赤で、Offlineは黒で文字を描画します
        /// </summary>
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
