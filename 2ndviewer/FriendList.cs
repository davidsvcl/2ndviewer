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
    class FriendList
    {
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public FriendList() {
        }

        /// <summary>�I�����C��(true)/�I�t���C��(false)</summary>
        public bool Online;
        /// <summary>UUID</summary>
        public UUID UUID;
        /// <summary>�A�o�^�[��</summary>
        public string Name;
    }
}
