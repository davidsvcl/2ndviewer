using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenMetaverse;

namespace _2ndviewer
{
    /// <summary>
    /// フレンドリストクラス
    /// フレンドウィンドウに格納するフレンドリスト用データクラスです。
    /// </summary>
    class Im_tab
    {
        /// <summary>UUID</summary>
        public UUID fromAgentID_;
        /// <summary>セッションID</summary>
        public UUID sessionID_;
        /// <summary>タブ</summary>
        public System.Windows.Forms.TabPage tabPage_;
        /// <summary>ログエリア</summary>
        public System.Windows.Forms.TextBox textBox_;

        /// <summary>比較演算子</summary>
        public static bool operator ==(Im_tab left, Im_tab right)
        {
            if (left.fromAgentID_ == right.fromAgentID_) return true;
            else return false;
        }

        /// <summary>比較演算子</summary>
        public static bool operator !=(Im_tab left, Im_tab right)
        {
            if (left.fromAgentID_ != right.fromAgentID_) return true;
            else return false;
        }

        /// <summary>比較演算子</summary>
        public override bool Equals(Object o)
        {
            Im_tab other = o as Im_tab;
            if (this.fromAgentID_ == other.fromAgentID_) return true;
            else return false;
        }

        /// <summary>ハッシュコード</summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
