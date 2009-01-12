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
    /// アスキーアートウィンドウクラス
    /// アスキーアートの登録、呼び出しを行います。
    /// </summary>
    public partial class AAForm : Form
    {
        /// <summary>ディレクトリ配列</summary>
        private String[] dirs_;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AAForm()
        {
            InitializeComponent();
            ReReadDirectory();
        }

        /// <summary>
        /// ReReadDirectory
        /// ディレクトリの再読み込みを行います
        /// </summary>
        private void ReReadDirectory()
        {
            try
            {
                dirs_ = System.IO.Directory.GetFiles("asciiarts");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "FileSystem Error", MessageBoxButtons.OK);
                return;
            }
            this.listBox1.Items.Clear();
            for (int i = 0; i < dirs_.Length; i++)
            {
                this.listBox1.Items.Add(dirs_[i].Substring(10, dirs_[i].Length - 4 - 10));
            }
        }

        /// <summary>
        /// select_button_Click
        /// 選択ボタンクリックイベント
        /// リストボックスから選択されたファイルを読み込み、内容をテキストボックスに反映します
        /// </summary>
        private void select_button_Click(object sender, EventArgs e)
        {
            string text = System.IO.File.ReadAllText(dirs_[listBox1.SelectedIndex], System.Text.Encoding.GetEncoding("UTF-8"));
            title_textBox.Text = dirs_[listBox1.SelectedIndex].Substring(10, dirs_[listBox1.SelectedIndex].Length - 4 - 10);
            this.textBox1.Text = text;
        }

        /// <summary>
        /// add_button_Click
        /// 追加ボタンクリックイベント
        /// テキストボックスの内容をファイルに書き込み、リストボックスに追加します
        /// </summary>
        private void add_button_Click(object sender, EventArgs e)
        {
            if (title_textBox.Text.Length <= 0) {
                MessageBox.Show(StringResource.titleIsNull, "Information", MessageBoxButtons.OK);
                return;
            }
            try
            {
                System.IO.File.WriteAllText("asciiarts\\" + title_textBox.Text + ".txt", textBox1.Text, System.Text.Encoding.GetEncoding("UTF-8"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "FileSystem Error", MessageBoxButtons.OK);
                return;
            }
            this.listBox1.Items.Add(title_textBox.Text);
            ReReadDirectory();
        }

        /// <summary>
        /// clear_button_Click
        /// クリアボタンクリックイベント
        /// テキストボックスの内容を消します
        /// </summary>
        private void clear_button_Click(object sender, EventArgs e)
        {
            this.title_textBox.Text = "";
            this.textBox1.Text = "";
        }
    }
}
