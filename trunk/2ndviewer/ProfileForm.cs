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
    public partial class ProfileForm : Form, IDisposable
    {
        private SecondLife client_;
        private LLUUID avatarID_;
        private LLUUID slImageID_;
        private LLUUID flImageID_;

        private delegate void PropertiesUpdateDelegate(LLUUID avatarID, Avatar.AvatarProperties properties);
        AssetManager.ImageReceivedCallback ImageReceivedCallback;

        private delegate void SetAvatarTextDelegate(string avatar);
        private delegate void SetPartnerTextDelegate(string partner);

        public ProfileForm()
        {
            InitializeComponent();
            pictureBox1.Image = null;
        }

        #region IDisposable メンバ

        void IDisposable.Dispose()
        {
            client_.Avatars.OnAvatarProperties -= Avatars_OnAvatarProperties;
            client_.Avatars.OnAvatarNames -= Avatars_OnAvatarNames;
            client_.Assets.OnImageReceived -= ImageReceivedCallback;
        }

        #endregion

        public void SetClient(SecondLife client)
        {
            client_ = client;
        }

        public void SetAvatarID(LLUUID uuid)
        {
            avatarID_ = uuid;
        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            client_.Avatars.OnAvatarProperties += new AvatarManager.AvatarPropertiesCallback(Avatars_OnAvatarProperties);
            client_.Avatars.RequestAvatarProperties(avatarID_);

            client_.Avatars.OnAvatarNames += new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
            client_.Avatars.RequestAvatarName(avatarID_);

            ImageReceivedCallback = new AssetManager.ImageReceivedCallback(Assets_OnImageReceived);
            client_.Assets.OnImageReceived += ImageReceivedCallback;
        }

        void Avatars_OnAvatarNames(Dictionary<LLUUID, string> names)
        {
            foreach (KeyValuePair<LLUUID, string> kvp in names)
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

        private void SetAvatarText(string avatar)
        {
            name_textBox.Text = avatar;
        }
        private void SetPartnerText(string partner)
        {
            partner_textBox.Text = partner;
        }

        void Avatars_OnAvatarProperties(LLUUID avatarID, Avatar.AvatarProperties properties)
        {
            PropertiesUpdateDelegate dlg = new PropertiesUpdateDelegate(PropertiesUpdate);
            object[] arg = { avatarID, properties };
            Invoke(dlg, arg);
        }

        private void PropertiesUpdate(LLUUID avatarID, Avatar.AvatarProperties properties)
        {
            firstlist_textBox.Text = properties.FirstLifeText;
            if (properties.Partner != LLUUID.Zero)
            {
                client_.Avatars.RequestAvatarName(properties.Partner);
            }
            //properties.Partner
            //partner_textBox.Text =
            about_textBox.Text = properties.AboutText;
            web_textBox.Text = properties.ProfileURL;
            born_textBox.Text = properties.BornOn;
            if (properties.ProfileImage != LLUUID.Zero)
            {
                slImageID_ = properties.ProfileImage;
                client_.Assets.RequestImage(properties.ProfileImage, ImageType.Normal);
            }
            if (properties.FirstLifeImage != LLUUID.Zero)
            {
                flImageID_ = properties.FirstLifeImage;
                client_.Assets.RequestImage(properties.FirstLifeImage, ImageType.Normal);
            }
        }

        private void view_button_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(web_textBox.Text);
        }

        void Assets_OnImageReceived(ImageDownload image, AssetTexture assetTexture)
        {
            libsecondlife.Image img;
            if (image.Success)
            {
                if(image.ID == slImageID_) pictureBox1.Image = OpenJPEGNet.OpenJPEG.DecodeToImage(image.AssetData, out img);
                else if (image.ID == flImageID_) pictureBox2.Image = OpenJPEGNet.OpenJPEG.DecodeToImage(image.AssetData, out img);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
