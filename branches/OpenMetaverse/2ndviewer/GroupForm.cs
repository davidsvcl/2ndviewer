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
    /// �O���[�v�E�B���h�E�N���X
    /// �Q�����Ă���O���[�v�̈ꗗ�̕\�����s���܂��B
    /// </summary>
    public partial class GroupForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>Second Life�O���b�h�ʐM���C�u����</summary>
        private GridClient client_;
        /// <summary>�O���[�v�z��</summary>
        public Dictionary<UUID, Group> Groups_;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public GroupForm()
        {
            InitializeComponent();
            leave_button.Enabled = false;
        }

        /// <summary>�ʐM���C�u�������Z�b�g����</summary>
        public void SetClient(GridClient client)
        {
            client_ = client;
        }

        /// <summary>�O���[�v���X�g�{�b�N�X���č\�z���܂�</summary>
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

        /// <summary>���X�g�{�b�N�X�̑I�����ύX���ꂽ���ɌĂ΂�郁�\�b�h�ł�</summary>
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

        /// <summary>�O���[�v���쐬���܂�</summary>
        private void create_button_Click(object sender, EventArgs e)
        {
            // IMPLEMENT ME
        }

        /// <summary>�O���[�v���A�N�e�B�u�ɂ��܂�</summary>
        private void activate_button_Click(object sender, EventArgs e)
        {
            if (group_listBox.SelectedIndex >= 0)
            {
                Group group = (Group)group_listBox.Items[group_listBox.SelectedIndex];
                client_.Groups.ActivateGroup(group.ID);
            }
        }

        /// <summary>�O���[�v���\���E�B���h�E��\�����܂�</summary>
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

        /// <summary>�O���[�v���痣�E���܂�</summary>
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
