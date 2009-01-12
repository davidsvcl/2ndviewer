using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;

namespace _2ndviewer
{
    /// <summary>
    /// �f�o�b�O�E�B���h�E�N���X
    /// �f�o�b�O���̕\�����s���܂��B
    /// </summary>
    public partial class DebugForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>�f�o�b�O���O�Ƀe�L�X�g��ǉ����邽�߂̃f���Q�[�g</summary>
        private delegate void WriteLineDelegate(string str);
        /// <summary>�f�o�b�O�t�B���^�[</summary>
        public int filter_selected;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public DebugForm()
        {
            InitializeComponent();
            this.filter_comboBox.SelectedIndex = 1;
            filter_selected = 1;
        }

        /// <summary>
        /// WriteLine
        /// �f�o�b�O���O�Ƀe�L�X�g��ǉ����A�Ō�̍s�փX�N���[������
        /// </summary>
        private void WriteLine(string str)
        {
            string msg;
            msg = textBox1.Text + str;
            textBox1.Text = msg;
            textBox1.SelectionStart = msg.Length;
            textBox1.ScrollToCaret();
        }

        /// <summary>
        /// DebugLog
        /// �f�o�b�O���O�Ƀe�L�X�g��ǉ����܂�
        /// </summary>
        public void DebugLog(string message)
        {
            if (message.Length <= 0) return;
            string msg = "\r\n" + message;
            WriteLineDelegate dlg = new WriteLineDelegate(WriteLine);
            string[] arg = { msg };
            try
            {
                Invoke(dlg, arg);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e);
            }
        }

        /// <summary>
        /// filter_comboBox_SelectedIndexChanged
        /// �t�B���^�[��ύX����ƌĂ΂�郁�\�b�h�ł�
        /// </summary>
        private void filter_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            filter_selected = this.filter_comboBox.SelectedIndex;
        }
    }
}
