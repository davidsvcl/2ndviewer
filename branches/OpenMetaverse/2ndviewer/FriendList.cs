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
    class FriendList
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FriendList() {
        }

        /// <summary>オンライン(true)/オフライン(false)</summary>
        public bool Online;
        /// <summary>UUID</summary>
        public UUID UUID;
        /// <summary>アバター名</summary>
        public string Name;
    }
}
