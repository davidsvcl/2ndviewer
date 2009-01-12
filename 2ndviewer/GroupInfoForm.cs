using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenMetaverse;

namespace _2ndviewer
{
    /// <summary>
    /// グループ情報ウィンドウクラス
    /// グループの詳細情報表示を行います。
    /// </summary>
    public partial class GroupInfoForm : Form ,IDisposable
    {
        /// <summary>Second Lifeグリッド通信ライブラリ</summary>
        private GridClient client_;
        /// <summary>グループ情報</summary>
        private Group group_;

        /// <summary>グループ情報</summary>
        Group profile_ = new Group();

        /// <summary>グループプロフィールコールバック</summary>
        GroupManager.GroupProfileCallback GroupProfileCallback;
        /// <summary>グループ画像受信コールバック</summary>
        AssetManager.ImageReceivedCallback ImageReceivedCallback;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GroupInfoForm()
        {
            InitializeComponent();

            listView1.Enabled = false;
            refresh_button.Enabled = false;
        }

        /// <summary>
        /// GroupInfoForm_Load
        /// ウィンドウロード時に呼ばれるメソッドです。
        /// コールバック関数の登録を行います
        /// </summary>
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

        /// <summary>
        /// Dispose
        /// ウィンドウ破棄時に呼ばれるメソッドです。
        /// コールバック関数の削除を行います
        /// </summary>
        void IDisposable.Dispose()
        {
            client_.Groups.OnGroupProfile -= GroupProfileCallback;
            client_.Assets.OnImageReceived -= ImageReceivedCallback;
        }

        #endregion

        /// <summary>通信ライブラリをセットする</summary>
        public void SetClient(GridClient client)
        {
            client_ = client;
        }

        /// <summary>グループをセットする</summary>
        public void SetGroup(Group group)
        {
            group_ = group;
        }

        /// <summary>
        /// Groups_OnGroupProfile
        /// グループ情報受信時に呼ばれるメソッドです。
        /// グループ画像の要求を行います
        /// </summary>
        void Groups_OnGroupProfile(Group group)
        {
            profile_ = group;
            if (group_.InsigniaID != UUID.Zero) client_.Assets.RequestImage(group_.InsigniaID, ImageType.Normal, 113000.0f, 0);
            if(InvokeRequired)BeginInvoke(new MethodInvoker(UpdateProfile));
        }

        /// <summary>
        /// Assets_OnImageReceived
        /// グループ画像受信時に呼ばれるメソッドです。
        /// </summary>
        void Assets_OnImageReceived(ImageDownload image, AssetTexture assetTexture)
        {
            OpenMetaverse.Imaging.ManagedImage img;
            Image bitmap;
            if (image.Success)
            {
                OpenMetaverse.Imaging.OpenJPEG.DecodeToImage(image.AssetData, out img, out bitmap);
                pictureBox.Image = bitmap;
            }
        }

        /// <summary>
        /// UpdateProfile
        /// </summary>
        private void UpdateProfile()
        {
            groupname_label.Text = profile_.Name;
            textBox1.Text = profile_.Charter;
        }

    }
}
