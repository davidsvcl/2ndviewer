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
    /// デバッグウィンドウクラス
    /// デバッグ情報の表示を行います。
    /// </summary>
    public partial class DebugForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>デバッグログにテキストを追加するためのデリゲート</summary>
        private delegate void WriteLineDelegate(string str);
        /// <summary>デバッグフィルター</summary>
        public int filter_selected;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DebugForm()
        {
            InitializeComponent();
            this.filter_comboBox.SelectedIndex = 1;
            filter_selected = 1;
        }

        /// <summary>
        /// WriteLine
        /// デバッグログにテキストを追加し、最後の行へスクロールする
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
        /// デバッグログにテキストを追加します
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
        /// フィルターを変更すると呼ばれるメソッドです
        /// </summary>
        private void filter_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            filter_selected = this.filter_comboBox.SelectedIndex;
        }
    }
}
