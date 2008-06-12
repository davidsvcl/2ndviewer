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
        public AAForm()
        {
            InitializeComponent();
            this.listBox1.Items.Add("スンマセン");
        }

        private void select_button_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == 0) {
                string text =
                    "　　　　　　　 ∧＿∧　 ／￣￣￣￣￣￣￣\r\n" +
                    "　　　　　　　（；´Д｀）＜　スンマセン、直ぐに片付けます\r\n" +
                    "　　-=≡ 　/　　　 ヽ　 ＼＿＿＿＿＿＿＿\r\n" +
                    "　　　　　 /|　|　　 |. |\r\n" +
                    "　-=≡　/.　＼ヽ／＼＼_\r\n" +
                    "　　　　/　　　 ヽ⌒)==ヽ_）=　∧＿∧\r\n" +
                    "-= 　 /　/⌒＼.＼　||　　|| 　(´･ω･`)\r\n" +
                    "　　／ ／　　　 >　） || 　 ||　( つ旦O\r\n" +
                    "　/　/ 　　　　/ ／＿||＿ ||　と＿)＿) ＿.\r\n" +
                    "　し'　　　　　（＿つ￣(_））￣ (.)）￣ (_））￣(.)）\r\n";
                this.textBox1.Text = text;
            }
        }
    }
}
