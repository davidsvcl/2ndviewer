using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _2ndviewer
{
    /// <summary>
    /// �o�[�W�������E�B���h�E�N���X
    /// �o�[�W�������̕\�����s���܂��B
    /// </summary>
    public partial class AboutForm : Form
    {
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// linkLabel1_LinkClicked
        /// �����N�{�^���N���b�N�C�x���g
        /// </summary>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://tuna0.blogspot.com/");
            }
            catch
            {
            }
        }
    }
}
