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
    public partial class GroupForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private GridClient client_;
        public Dictionary<UUID, Group> Groups_;

        public GroupForm()
        {
            InitializeComponent();
            leave_button.Enabled = false;
        }

        public void SetClient(GridClient client)
        {
            client_ = client;
        }

        public void UpdateGroups()
        {
            lock (group_listBox)
            {
                Invoke((MethodInvoker)delegate() { group_listBox.Items.Clear(); });
                foreach (Group group in Groups_.Values)
                {
                    Invoke((MethodInvoker)delegate() { group_listBox.Items.Add(group); });
                }
            }
        }

        private void group_listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (group_listBox.SelectedIndex >= 0)
            {
                leave_button.Enabled = true;
            }
            else
            {
                leave_button.Enabled = false;
            }
        }

        private void create_button_Click(object sender, EventArgs e)
        {
            // IMPLEMENT ME
        }

        private void activate_button_Click(object sender, EventArgs e)
        {
            if (group_listBox.SelectedIndex >= 0)
            {
                Group group = (Group)group_listBox.Items[group_listBox.SelectedIndex];
                client_.Groups.ActivateGroup(group.ID);
            }
        }

        private void info_button_Click(object sender, EventArgs e)
        {
            if (group_listBox.SelectedIndex >= 0 && group_listBox.Items[group_listBox.SelectedIndex].ToString() != "none")
            {
                Group group = (Group)group_listBox.Items[group_listBox.SelectedIndex];
                using (GroupInfoForm groupInfoForm = new GroupInfoForm())
                {
                    groupInfoForm.SetClient(client_);
                    groupInfoForm.SetGroup(group);
                    groupInfoForm.ShowDialog();
                }
            }
        }

        private void leave_button_Click(object sender, EventArgs e)
        {
            if (group_listBox.SelectedIndex >= 0 && group_listBox.Items[group_listBox.SelectedIndex].ToString() != "none")
            {
                Group group = (Group)group_listBox.Items[group_listBox.SelectedIndex];
                client_.Groups.LeaveGroup(group.ID);
            }
        }
    }
}
