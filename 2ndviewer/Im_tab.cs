using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenMetaverse;

namespace _2ndviewer
{
    /// <summary>
    /// �t�����h���X�g�N���X
    /// �t�����h�E�B���h�E�Ɋi�[����t�����h���X�g�p�f�[�^�N���X�ł��B
    /// </summary>
    class Im_tab
    {
        /// <summary>UUID</summary>
        public UUID fromAgentID_;
        /// <summary>�Z�b�V����ID</summary>
        public UUID sessionID_;
        /// <summary>�^�u</summary>
        public System.Windows.Forms.TabPage tabPage_;
        /// <summary>���O�G���A</summary>
        public System.Windows.Forms.TextBox textBox_;

        /// <summary>��r���Z�q</summary>
        public static bool operator ==(Im_tab left, Im_tab right)
        {
            if (left.fromAgentID_ == right.fromAgentID_) return true;
            else return false;
        }

        /// <summary>��r���Z�q</summary>
        public static bool operator !=(Im_tab left, Im_tab right)
        {
            if (left.fromAgentID_ != right.fromAgentID_) return true;
            else return false;
        }

        /// <summary>��r���Z�q</summary>
        public override bool Equals(Object o)
        {
            Im_tab other = o as Im_tab;
            if (this.fromAgentID_ == other.fromAgentID_) return true;
            else return false;
        }

        /// <summary>�n�b�V���R�[�h</summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
