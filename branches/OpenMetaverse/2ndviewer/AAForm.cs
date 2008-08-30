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
    public partial class AAForm : Form
    {
        private String[] dirs_;
        public AAForm()
        {
            InitializeComponent();
            ReReadDirectory();
        }

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

        private void select_button_Click(object sender, EventArgs e)
        {
            string text = System.IO.File.ReadAllText(dirs_[listBox1.SelectedIndex], System.Text.Encoding.GetEncoding("UTF-8"));
            title_textBox.Text = dirs_[listBox1.SelectedIndex].Substring(10, dirs_[listBox1.SelectedIndex].Length - 4 - 10);
            this.textBox1.Text = text;
        }

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

        private void clear_button_Click(object sender, EventArgs e)
        {
            this.title_textBox.Text = "";
            this.textBox1.Text = "";
        }
    }
}
