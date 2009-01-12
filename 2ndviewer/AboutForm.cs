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
    /// バージョン情報ウィンドウクラス
    /// バージョン情報の表示を行います。
    /// </summary>
    public partial class AboutForm : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// linkLabel1_LinkClicked
        /// リンクボタンクリックイベント
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
