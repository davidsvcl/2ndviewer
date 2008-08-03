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
    public partial class AvatarForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private GridClient client_;
        private ChatForm chatForm_;
        private MovementForm movementForm_;
        System.Collections.Generic.List<Avatar> avatar_array_ = new System.Collections.Generic.List<Avatar>();

        public AvatarForm()
        {
            InitializeComponent();
        }

        public void SetClient(GridClient client)
        {
            client_ = client;
        }

        public void SetChatForm(ChatForm chatForm)
        {
            chatForm_ = chatForm;
        }

        public void SetMovementForm(MovementForm movementForm)
        {
            movementForm_ = movementForm;
        }

        private void searchAvatars(string searchString)
        {
            float radius = float.Parse("20");
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

        private void refresh_button_Click(object sender, EventArgs e)
        {
            searchAvatars("");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                searchAvatars(textBox1.Text);
            }
        }

        private void im_button_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            chatForm_.StartIM(avatar_array_[index].ID, avatar_array_[index].Name);
        }

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

        private void friend_button_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            client_.Friends.OfferFriendship(avatar_array_[index].ID);
        }

        private void follow_button_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            movementForm_.follow_on_ = true;
            movementForm_.followName_ = avatar_array_[index].Name;
            movementForm_.follow_checkBox.Checked = true;
            movementForm_.follow_textBox.Text = avatar_array_[index].Name;
        }

        private void teleport_button_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            client_.Self.SendTeleportLure(avatar_array_[index].ID, "Join me in " + client_.Network.CurrentSim.Name + "!");
        }
    }
}
