using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;
using OpenMetaverse;

namespace _2ndviewer
{
    /// <summary>
    /// �R���g���[���E�B���h�E�N���X
    /// �R���g���[����ʕ\�����s���܂��B
    /// </summary>
    public partial class MovementForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>Second Life�O���b�h�ʐM���C�u����</summary>
        private GridClient client_;
        /// <summary>�X�g�[�L���O</summary>
        public bool follow_on_;
        /// <summary>�߂��̂��̂ɍ���</summary>
        public bool sit_on_;
        /// <summary>�X�g�[�L���O����A�o�^�[��</summary>
        public string followName_;
        /// <summary>���������̉�����</summary>
        public string speech_;
        /// <summary>�{�N�V���O</summary>
        public bool boxing_;
        /// <summary>���y�Đ�</summary>
        private WMPLib.WindowsMediaPlayer mediaPlayer_;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public MovementForm()
        {
            InitializeComponent();
            follow_on_ = false;
            boxing_ = false;
            speech_comboBox.Items.Add(StringResource.none);
            System.Speech.Synthesis.SpeechSynthesizer ss = new System.Speech.Synthesis.SpeechSynthesizer();
            System.Collections.ObjectModel.ReadOnlyCollection<System.Speech.Synthesis.InstalledVoice> list = ss.GetInstalledVoices();
            foreach (System.Speech.Synthesis.InstalledVoice voice in list)
            {
                System.Speech.Synthesis.VoiceInfo info = voice.VoiceInfo;
                speech_comboBox.Items.Add(info.Name);
            }
            speech_comboBox.SelectedIndex = 0;
            speech_ = StringResource.none;
            try
            {
                mediaPlayer_ = new WMPLib.WindowsMediaPlayer();
                mediaPlayer_.settings.autoStart = false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e);
                MessageBox.Show("WindowsMediaPlayer error!" + e.ToString());
            }
        }

        /// <summary>�ʐM���C�u�������Z�b�g����</summary>
        public void SetClient(GridClient client)
        {
            client_ = client;
        }

        /// <summary>���y��URL���Z�b�g����</summary>
        public void SetMusicURL(string url)
        {
            try
            {
                mediaPlayer_.controls.stop();
                mediaPlayer_.URL = url;
                if (music_checkBox.Checked == true)
                {
                    mediaPlayer_.controls.play();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e);
            }
        }

        /// <summary>���{�^��</summary>
        private void up_button_Click(object sender, EventArgs e)
        {
            double x;
            if (boost_checkBox.Checked)
            {
                x = client_.Self.GlobalPosition.X + 3;
            }
            else
            {
                x = client_.Self.GlobalPosition.X + 1.3;
            }
            double y = client_.Self.GlobalPosition.Y;
            double z = client_.Self.GlobalPosition.Z;
            if (boxing_checkBox.Checked)
            {
                boxing();
            }
            client_.Self.AutoPilot(x, y, z);
        }

        /// <summary>���{�^��</summary>
        private void left_button_Click(object sender, EventArgs e)
        {
            double x = client_.Self.GlobalPosition.X;
            double y;
            if (boost_checkBox.Checked)
            {
                y = client_.Self.GlobalPosition.Y + 3;
            }
            else
            {
                y = client_.Self.GlobalPosition.Y + 1.3;
            }
            double z = client_.Self.GlobalPosition.Z;
            if (boxing_checkBox.Checked)
            {
                boxing();
            }
            client_.Self.AutoPilot(x, y, z);
        }

        /// <summary>���{�^��</summary>
        private void down_button_Click(object sender, EventArgs e)
        {
            double x;
            if (boost_checkBox.Checked)
            {
                x = client_.Self.GlobalPosition.X - 3;
            }
            else
            {
                x = client_.Self.GlobalPosition.X - 1.3;
            }
            double y = client_.Self.GlobalPosition.Y;
            double z = client_.Self.GlobalPosition.Z;
            if (boxing_checkBox.Checked)
            {
                boxing();
            }
            client_.Self.AutoPilot(x, y, z);
        }

        /// <summary>���{�^��</summary>
        private void right_button_Click(object sender, EventArgs e)
        {
            double x = client_.Self.GlobalPosition.X;
            double y;
            if (boost_checkBox.Checked)
            {
                y = client_.Self.GlobalPosition.Y - 3;
            }
            else
            {
                y = client_.Self.GlobalPosition.Y - 1.3;
            }
            double z = client_.Self.GlobalPosition.Z;
            if (boxing_checkBox.Checked)
            {
                boxing();
            }
            client_.Self.AutoPilot(x, y, z);
        }

        /// <summary>��ԃ{�^��</summary>
        private void fly_button_Click(object sender, EventArgs e)
        {
            client_.Self.Fly(true);
        }

        /// <summary>�~���{�^��</summary>
        private void alight_button_Click(object sender, EventArgs e)
        {
            client_.Self.Fly(false);
        }

        /// <summary>�e���|�[�g�{�^��</summary>
        private void teleport_button_Click(object sender, EventArgs e)
        {
            string location_text = teleportloc_maskedTextBox.Text;
            string sim_name = teleportsim_textBox.Text;
            if (location_text == null || sim_name == null) return;
            string[] loc = location_text.Split('/');
            try
            {
                float x = float.Parse(loc[0]);
                float y = float.Parse(loc[1]);
                float z = float.Parse(loc[2]);
                client_.Self.Teleport(sim_name, new Vector3(x, y, z));
            }
            catch {
                MessageBox.Show(StringResource.failedTeleport, "Error");
            }
        }

        /// <summary>�X�g�[�L���O�`�F�b�N�{�b�N�X</summary>
        private void follow_checkBox_CheckStateChanged(object sender, EventArgs e)
        {
            follow_on_ = follow_checkBox.Checked;
        }

        /// <summary>�X�g�[�L���O�A�o�^�[���ύX</summary>
        private void follow_textBox_TextChanged(object sender, EventArgs e)
        {
            followName_ = follow_textBox.Text;
        }

        /// <summary>����`�F�b�N�{�b�N�X</summary>
        private void sit_checkBox_CheckStateChanged(object sender, EventArgs e)
        {
            sit_on_ = sit_checkBox.Checked;
        }

        /// <summary>���{�^��</summary>
        private void stand_button_Click(object sender, EventArgs e)
        {
            client_.Self.Stand();
        }

        /// <summary>AsciiArt�{�^��</summary>
        private void sp_button_Click(object sender, EventArgs e)
        {
            AAForm aaform = new AAForm();
            if (DialogResult.OK == aaform.ShowDialog(this))
            {
                string text = aaform.textBox1.Text;
                if (text == null) return;
                string[] aa = text.Split('\r');
                for (int i = 0; i < aa.Length;i++ )
                {
                    string chat = aa[i].Replace('\n',' ');
                    client_.Self.Chat(chat, 0, ChatType.Normal);
                }
            }
        }

        /// <summary>���������`�F�b�N�{�b�N�X</summary>
        private void speech_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            speech_ = speech_comboBox.Text;
        }

        /// <summary>�e�����{�^��</summary>
        private void shoot_button_Click(object sender, EventArgs e)
        {
            client_.Self.Movement.Mouselook = true;
            client_.Self.Movement.MLButtonDown = true;
            client_.Self.Movement.SendUpdate();

            client_.Self.Movement.MLButtonUp = true;
            client_.Self.Movement.MLButtonDown = false;
            client_.Self.Movement.FinishAnim = true;
            client_.Self.Movement.SendUpdate();

            client_.Self.Movement.Mouselook = false;
            client_.Self.Movement.MLButtonUp = false;
            client_.Self.Movement.FinishAnim = false;
            client_.Self.Movement.SendUpdate();
        }

        /// <summary>�{�N�V���O</summary>
        public void boxing()
        {
            client_.Self.Movement.MLButtonDown = true;
            client_.Self.Movement.AtPos = true;
            client_.Self.Movement.SendUpdate();

            client_.Self.Movement.AtPos = false;
            client_.Self.Movement.SendUpdate();
            client_.Self.Movement.MLButtonDown = false;
            client_.Self.Movement.FinishAnim = true;
            client_.Self.Movement.SendUpdate();

            client_.Self.Movement.Mouselook = false;
            client_.Self.Movement.MLButtonUp = false;
            client_.Self.Movement.FinishAnim = false;
            client_.Self.Movement.SendUpdate();
        }

        /// <summary>�{�N�V���O�`�F�b�N�{�b�N�X</summary>
        private void boxing_checkBox_CheckStateChanged(object sender, EventArgs e)
        {
            boxing_ = boxing_checkBox.Checked;
        }

        /// <summary>��ɑ���`�F�b�N�{�b�N�X</summary>
        private void run_checkBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (run_checkBox.Checked)
            {
                System.Diagnostics.Trace.WriteLine("Run mode");
                client_.Self.Movement.AlwaysRun = true;
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Walk mode");
                client_.Self.Movement.AlwaysRun = false;
            }
        }

        /// <summary>���y�Đ��`�F�b�N�{�b�N�X</summary>
        private void music_checkBox_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (music_checkBox.Checked == true)
                {
                    mediaPlayer_.controls.play();
                }
                else
                {
                    mediaPlayer_.controls.stop();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);                
            }
        }
    }
}
