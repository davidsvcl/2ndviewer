using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using libsecondlife;

namespace _2ndviewer
{
    public partial class GroupInfoForm : Form ,IDisposable
    {
        private SecondLife client_;
        private Group group_;

        GroupProfile profile_ = new GroupProfile();

        GroupManager.GroupProfileCallback GroupProfileCallback;
        AssetManager.ImageReceivedCallback ImageReceivedCallback;

        public GroupInfoForm()
        {
            InitializeComponent();

            listView1.Enabled = false;
            refresh_button.Enabled = false;
        }

        private void GroupInfoForm_Load(object sender, EventArgs e)
        {
            pictureBox.Image = null;

            GroupProfileCallback = new GroupManager.GroupProfileCallback(Groups_OnGroupProfile);
            ImageReceivedCallback = new AssetManager.ImageReceivedCallback(Assets_OnImageReceived);

            client_.Groups.OnGroupProfile += GroupProfileCallback;
            client_.Assets.OnImageReceived += ImageReceivedCallback;

            client_.Groups.RequestGroupProfile(group_.ID);
        }

        #region IDisposable メンバ

        void IDisposable.Dispose()
        {
            client_.Groups.OnGroupProfile -= GroupProfileCallback;
            client_.Assets.OnImageReceived -= ImageReceivedCallback;
        }

        #endregion

        public void SetClient(SecondLife client)
        {
            client_ = client;
        }

        public void SetGroup(Group group)
        {
            group_ = group;
        }

        void Groups_OnGroupProfile(GroupProfile group)
        {
            profile_ = group;
            if (group_.InsigniaID != LLUUID.Zero) client_.Assets.RequestImage(group_.InsigniaID, ImageType.Normal, 113000.0f, 0);
            if(InvokeRequired)BeginInvoke(new MethodInvoker(UpdateProfile));
        }

        void Assets_OnImageReceived(ImageDownload image, AssetTexture assetTexture)
        {
            libsecondlife.Image img;
            if (image.Success)
            {
                pictureBox.Image = OpenJPEGNet.OpenJPEG.DecodeToImage(image.AssetData, out img);
            }
        }

        private void UpdateProfile()
        {
            groupname_label.Text = profile_.Name;
            textBox1.Text = profile_.Charter;
        }

    }
}
