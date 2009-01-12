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
    /// プロフィールウィンドウクラス
    /// プロフィール画面表示を行います。
    /// </summary>
    public partial class ProfileForm : Form, IDisposable
    {
        /// <summary>Second Lifeグリッド通信ライブラリ</summary>
        private GridClient client_;
        /// <summary>アバターUUID</summary>
        private UUID avatarID_;
        /// <summary>アバター画像UUID</summary>
        private UUID slImageID_;
        /// <summary>ファーストライフ画像UUID</summary>
        private UUID flImageID_;

        /// <summary></summary>
        private delegate void PropertiesUpdateDelegate(UUID avatarID, Avatar.AvatarProperties properties);
        /// <summary>画像受信コールバック</summary>
        AssetManager.ImageReceivedCallback ImageReceivedCallback;

        /// <summary></summary>
        private delegate void SetAvatarTextDelegate(string avatar);
        /// <summary></summary>
        private delegate void SetPartnerTextDelegate(string partner);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProfileForm()
        {
            InitializeComponent();
            pictureBox1.Image = null;
        }

        #region IDisposable メンバ

        /// <summary>
        /// Dispose
        /// </summary>
        void IDisposable.Dispose()
        {
            client_.Avatars.OnAvatarProperties -= Avatars_OnAvatarProperties;
            client_.Avatars.OnAvatarNames -= Avatars_OnAvatarNames;
            client_.Assets.OnImageReceived -= ImageReceivedCallback;
        }

        #endregion

        /// <summary>通信ライブラリをセットする</summary>
        public void SetClient(GridClient client)
        {
            client_ = client;
        }

        /// <summary>アバターIDをセットする</summary>
        public void SetAvatarID(UUID uuid)
        {
            avatarID_ = uuid;
        }

        /// <summary>画面表示時に呼ばれるメソッド</summary>
        private void ProfileForm_Load(object sender, EventArgs e)
        {
            client_.Avatars.OnAvatarProperties += new AvatarManager.AvatarPropertiesCallback(Avatars_OnAvatarProperties);
            client_.Avatars.RequestAvatarProperties(avatarID_);

            client_.Avatars.OnAvatarNames += new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
            client_.Avatars.RequestAvatarName(avatarID_);

            ImageReceivedCallback = new AssetManager.ImageReceivedCallback(Assets_OnImageReceived);
            client_.Assets.OnImageReceived += ImageReceivedCallback;
        }

        /// <summary></summary>
        void Avatars_OnAvatarNames(Dictionary<UUID, string> names)
        {
            foreach (KeyValuePair<UUID, string> kvp in names)
            {
                //System.Diagnostics.Trace.WriteLine("key;"+kvp.Key);
                //System.Diagnostics.Trace.WriteLine("value;" + kvp.Value);
                if (kvp.Key == avatarID_)
                {
                    BeginInvoke(new SetAvatarTextDelegate(SetAvatarText), new object[] { kvp.Value });
                }
                else
                {
                    BeginInvoke(new SetPartnerTextDelegate(SetPartnerText), new object[] { kvp.Value });
                }
            }
        }

        /// <summary></summary>
        private void SetAvatarText(string avatar)
        {
            name_textBox.Text = avatar;
        }

        /// <summary></summary>
        private void SetPartnerText(string partner)
        {
            partner_textBox.Text = partner;
        }

        /// <summary></summary>
        void Avatars_OnAvatarProperties(UUID avatarID, Avatar.AvatarProperties properties)
        {
            PropertiesUpdateDelegate dlg = new PropertiesUpdateDelegate(PropertiesUpdate);
            object[] arg = { avatarID, properties };
            Invoke(dlg, arg);
        }

        /// <summary></summary>
        private void PropertiesUpdate(UUID avatarID, Avatar.AvatarProperties properties)
        {
            firstlist_textBox.Text = properties.FirstLifeText;
            if (properties.Partner != UUID.Zero)
            {
                client_.Avatars.RequestAvatarName(properties.Partner);
            }
            //properties.Partner
            //partner_textBox.Text =
            about_textBox.Text = properties.AboutText;
            web_textBox.Text = properties.ProfileURL;
            born_textBox.Text = properties.BornOn;
            if (properties.ProfileImage != UUID.Zero)
            {
                slImageID_ = properties.ProfileImage;
                client_.Assets.RequestImage(properties.ProfileImage, ImageType.Normal);
            }
            if (properties.FirstLifeImage != UUID.Zero)
            {
                flImageID_ = properties.FirstLifeImage;
                client_.Assets.RequestImage(properties.FirstLifeImage, ImageType.Normal);
            }
        }

        /// <summary></summary>
        private void view_button_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(web_textBox.Text);
        }

        /// <summary></summary>
        void Assets_OnImageReceived(ImageDownload image, AssetTexture assetTexture)
        {
            OpenMetaverse.Imaging.ManagedImage img;
            Image bitmap;
            if (image.Success)
            {
                if (image.ID == slImageID_)
                {
                    OpenMetaverse.Imaging.OpenJPEG.DecodeToImage(image.AssetData, out img, out bitmap);
                    pictureBox1.Image = bitmap;
                }
                else if (image.ID == flImageID_)
                {
                    OpenMetaverse.Imaging.OpenJPEG.DecodeToImage(image.AssetData, out img, out bitmap);
                    pictureBox2.Image = bitmap;
                }
            }
        }

        /// <summary></summary>
        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
