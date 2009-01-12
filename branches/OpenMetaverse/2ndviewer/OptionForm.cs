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
    /// �I�v�V�����ݒ�E�B���h�E�N���X
    /// �I�v�V�����ݒ��ʕ\�����s���܂��B
    /// </summary>
    public partial class OptionForm : Form
    {
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public OptionForm()
        {
            InitializeComponent();
            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\2ndviewer", false);
            if (regkey == null) return;
            nickname_textBox.Text = (string)regkey.GetValue("nickName");
            news4vip_textBox.Text = (string)regkey.GetValue("news4vip");
            if (1 == (int)regkey.GetValue("confirmMessageBox", 1))
            {
                confirm_checkBox.Checked = true;
            }
        }
    }
}
