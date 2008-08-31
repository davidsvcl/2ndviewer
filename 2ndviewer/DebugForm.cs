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
    public partial class DebugForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private delegate void WriteLineDelegate(string str);
        public int filter_selected;

        public DebugForm()
        {
            InitializeComponent();
            this.filter_comboBox.SelectedIndex = 0;
            filter_selected = 0;
        }

        private void WriteLine(string str)
        {
            string msg;
            msg = textBox1.Text + str;
            textBox1.Text = msg;
            textBox1.SelectionStart = msg.Length;
            textBox1.ScrollToCaret();
        }
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

        private void filter_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            filter_selected = this.filter_comboBox.SelectedIndex;
        }
    }
}
