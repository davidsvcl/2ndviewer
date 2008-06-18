using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libsecondlife;

namespace _2ndviewer
{
    class Im_tab
    {
        public LLUUID fromAgentID_;
        public LLUUID sessionID_;
        public System.Windows.Forms.TabPage tabPage_;
        public System.Windows.Forms.TextBox textBox_;

        public static bool operator ==(Im_tab left, Im_tab right)
        {
            if (left.fromAgentID_ == right.fromAgentID_) return true;
            else return false;
        }
        public static bool operator !=(Im_tab left, Im_tab right)
        {
            if (left.fromAgentID_ != right.fromAgentID_) return true;
            else return false;
        }
        public override bool Equals(Object o)
        {
            Im_tab other = o as Im_tab;
            if (this.fromAgentID_ == other.fromAgentID_) return true;
            else return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
