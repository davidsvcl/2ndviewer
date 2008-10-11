using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenMetaverse;

namespace _2ndviewer
{
    public partial class LoginForm : Form
    {
        private GridClient client_;
        private delegate void LogonSuccessDelegate();
        private delegate void LogonFailedDelegate(string message);

        public LoginForm()
        {
            InitializeComponent();
        }

        public void SetClient(GridClient client)
        {
            client_ = client;
            client_.Network.OnLogin += new NetworkManager.LoginCallback(Network_OnLogin);
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\2ndviewer", false);
            if (regkey == null) return;
            firstName_textBox.Text = (string)regkey.GetValue("firstName");
            lastName_textBox.Text = (string)regkey.GetValue("lastName");
            password_textBox.Text = (string)regkey.GetValue("password");
            regkey.Close();
        }

        private void AcountLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(StringResource.CreateAccountUrl);
            }
            catch
            {
            }
        }

        private void login_button_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\2ndviewer");
            regkey.SetValue("firstName", firstName_textBox.Text);
            regkey.SetValue("lastName", lastName_textBox.Text);
            if (password_checkBox.Checked == true)
            {
                regkey.SetValue("password", password_textBox.Text);
            }
            regkey.Close();

            LoginParams loginParams = client_.Network.DefaultLoginParams(firstName_textBox.Text, lastName_textBox.Text, password_textBox.Text, "2ndviewer", "0.1.x");
            if (nameloc_radioButton.Checked == true)
            {
                // FIXME形式未調査
                System.Diagnostics.Trace.WriteLine(nameloc_maskedTextBox.Text);
                string location_text = nameloc_maskedTextBox.Text;
                if (location_text == null) return;
                string[] loc = location_text.Split('/');
                string sim = loc[0];
                int x = int.Parse(loc[1]);
                int y = int.Parse(loc[2]);
                int z = int.Parse(loc[3]);
                loginParams.Start = String.Format("uri:{0}&{1}&{2}&{3}", sim.ToLower(), x, y, z);
            }
            if (uri_checkBox.Checked)
            {
                loginParams.URI = uri_comboBox.Text;
            }
            client_.Network.BeginLogin(loginParams);
        }

        private void LogonSuccess()
        {
            Close();
        }

        private void LogonFailed(string message)
        {
            MessageBox.Show(message, StringResource.failedLogin, MessageBoxButtons.OK);
            this.Enabled = true;
        }

        void Network_OnLogin(LoginStatus login, string message)
        {
            if (login == LoginStatus.Success)
            {
                LogonSuccessDelegate fdlg = new LogonSuccessDelegate(LogonSuccess);
                Invoke(fdlg);
            }
            else if (login == LoginStatus.Failed)
            {
                LogonFailedDelegate fdlg = new LogonFailedDelegate(LogonFailed);
                object[] farg = { message };
                Invoke(fdlg, farg);
            }
        }
    }
}
