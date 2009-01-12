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
    /// �A�o�^�[�ꗗ�E�B���h�E�N���X
    /// �߂��ɂ���A�o�^�[���\���Ǝw�肵���A�o�^�[�ɑ΂��ăR���^�N�g���s���܂��B
    /// </summary>
    public partial class AvatarForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>Second Life�O���b�h�ʐM���C�u����</summary>
        private GridClient client_;
        /// <summary>�`���b�g�E�B���h�E</summary>
        private ChatForm chatForm_;
        /// <summary>�R���g���[���E�B���h�E</summary>
        private MovementForm movementForm_;
        /// <summary>�A�o�^�[�z��</summary>
        System.Collections.Generic.List<Avatar> avatar_array_ = new System.Collections.Generic.List<Avatar>();

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public AvatarForm()
        {
            InitializeComponent();
        }

        /// <summary>�ʐM���C�u�������Z�b�g����</summary>
        public void SetClient(GridClient client)
        {
            client_ = client;
        }

        /// <summary>�`���b�g�E�B���h�E���Z�b�g����</summary>
        public void SetChatForm(ChatForm chatForm)
        {
            chatForm_ = chatForm;
        }

        /// <summary>�R���g���[���E�B���h�E���Z�b�g����</summary>
        public void SetMovementForm(MovementForm movementForm)
        {
            movementForm_ = movementForm;
        }

        /// <summary>���V���̃A�o�^�[���������܂�</summary>
        private void searchAvatars(string searchString)
        {
            //float radius = float.Parse("20");
            avatar_array_.Clear();
            this.listBox1.Items.Clear();

            Vector3 location = client_.Self.SimPosition;
            List<Avatar> avatars = client_.Network.CurrentSim.ObjectsAvatars.FindAll(
                delegate(Avatar avatar)
                {
                    Vector3 pos = avatar.Position;
                    return true;// ((pos != Vector3.Zero) && (Vector3.Dist(pos, location) < radius));
                }

            );

            foreach (Avatar a in avatars)
            {
                string name = a.Name;
                if ((name != null) && (name != client_.Self.Name) && (name.Contains(searchString)))
                {
                    System.Diagnostics.Trace.WriteLine(name);
                    avatar_array_.Add(a);
                    this.listBox1.Items.Add(name);
                }
            }
        }

        /// <summary>�X�V�{�^��</summary>
        private void refresh_button_Click(object sender, EventArgs e)
        {
            searchAvatars("");
        }

        /// <summary>�A�o�^�[����</summary>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                searchAvatars(textBox1.Text);
            }
        }

        /// <summary>IM���J�n</summary>
        private void im_button_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            chatForm_.StartIM(avatar_array_[index].ID, avatar_array_[index].Name);
        }

        /// <summary>�v���t�B�[���̕\��</summary>
        private void profile_button_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            using (ProfileForm profileForm = new ProfileForm())
            {
                profileForm.SetClient(client_);
                profileForm.SetAvatarID(avatar_array_[index].ID);
                profileForm.ShowDialog();
            }
        }

        /// <summary>�t�����h�V�b�v���I�t�@�[����</summary>
        private void friend_button_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            client_.Friends.OfferFriendship(avatar_array_[index].ID);
        }

        /// <summary>�X�g�[�L���O�J�n</summary>
        private void follow_button_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            movementForm_.follow_on_ = true;
            movementForm_.followName_ = avatar_array_[index].Name;
            movementForm_.follow_checkBox.Checked = true;
            movementForm_.follow_textBox.Text = avatar_array_[index].Name;
        }

        /// <summary>�w�肵���A�o�^�[���e���|�[�g�Ŏ����̏ꏊ�ɌĂяo��</summary>
        private void teleport_button_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            client_.Self.SendTeleportLure(avatar_array_[index].ID, "Join me in " + client_.Network.CurrentSim.Name + "!");
        }
    }
}
