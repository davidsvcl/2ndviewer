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
    /// �A�C�e���E�B���h�E�N���X
    /// �A�C�e���̕\�����s���܂��B
    /// </summary>
    public partial class InventoryForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>Second Life�O���b�h�ʐM���C�u����</summary>
        private GridClient client_;
        /// <summary>�J�����g�C�x���g���t�H���_</summary>
        private InventoryFolder currentDirectory_;
        /// <summary>�c���[�̓W�J�p�f���Q�[�g</summary>
        private delegate void ExpandDelegate();

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public InventoryForm()
        {
            InitializeComponent();
            currentDirectory_ = null;
        }

        /// <summary>�ʐM���C�u�������Z�b�g����</summary>
        public void SetClient(GridClient client)
        {
            client_ = client;
            treeView1.Client = client;
        }

        /// <summary>����������</summary>
        public void InventoryInitialize()
        {
            InventoryManager Manager = client_.Inventory;
            OpenMetaverse.Inventory Inventory = Manager.Store;
            currentDirectory_ = Inventory.RootFolder;
            List<InventoryBase> currentContents = Inventory.GetContents(currentDirectory_);
            // Try and find an InventoryBase with the corresponding name.
            foreach (InventoryBase item in currentContents)
            {
                // Allow lookup by UUID as well as name:
                if (item.Name == "Objects" || item.UUID.ToString() == "Objects")
                {
                    if (item is InventoryFolder)
                    {
                        currentDirectory_ = item as InventoryFolder;
                        break;
                    }
                    else
                    {
                    }
                }
            }
            ExpandDelegate edlg = new ExpandDelegate(Expand);
            Invoke(edlg);
        }

        /// <summary>�c���[��W�J����</summary>
        private void Expand()
        {
            foreach (TreeNode childNode in treeView1.Nodes)
            {
                System.Diagnostics.Trace.WriteLine(childNode.Text);
                if (childNode.Text == "Objects")
                {
                    childNode.Expand();
                    return;
                }
            }
        }

        /// <summary>���g�p</summary>
        private void RefreshTree()
        {
        }

        /// <summary>���t���b�V���{�^��</summary>
        private void refresh_button_Click(object sender, EventArgs e)
        {
            RefreshTree();
        }

        /// <summary>�|�b�v�A�b�v���j������A�^�b�`��I���������ɌĂ΂�郁�\�b�h</summary>
        private void attachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            attach(OpenMetaverse.AttachmentPoint.Default);
        }

        /// <summary>�|�b�v�A�b�v���j������E��ɃA�^�b�`��I���������ɌĂ΂�郁�\�b�h</summary>
        private void attachToRightHandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            attach(AttachmentPoint.RightHand);
        }

        /// <summary>�|�b�v�A�b�v���j�����獶��ɃA�^�b�`��I���������ɌĂ΂�郁�\�b�h</summary>
        private void attachToLeftHandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            attach(AttachmentPoint.LeftHand);
        }

        /// <summary>�A�^�b�`���\�b�h</summary>
        private void attach(AttachmentPoint point)
        {
            if (currentDirectory_ == null) InventoryInitialize();
            if (currentDirectory_ == null) return;
            string inventoryName = this.treeView1.SelectedNode.Text;

            InventoryManager Manager = client_.Inventory;
            OpenMetaverse.Inventory Inventory = Manager.Store;

            // WARNING: Uses local copy of inventory contents, need to download them first.
            List<InventoryBase> contents = Inventory.GetContents(currentDirectory_);
            foreach (InventoryBase b in contents)
            {
                if (inventoryName == b.Name || inventoryName == b.UUID.ToString())
                {
                    if (b is OpenMetaverse.InventoryItem)
                    {
                        OpenMetaverse.InventoryItem item = b as OpenMetaverse.InventoryItem;
                        try
                        {
                            client_.Appearance.Attach(item, point);
                            break;
                        }
                        catch
                        {
                            MessageBox.Show(StringResource.failedAttach);
                            break;
                        }
                    }
                    else
                    {
                    }
                }
            }
        }

        /// <summary>�f�^�b�`���\�b�h</summary>
        private void detachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentDirectory_ == null) InventoryInitialize();
            if (currentDirectory_ == null) return;
            string inventoryName = this.treeView1.SelectedNode.Text;

            InventoryManager Manager = client_.Inventory;
            OpenMetaverse.Inventory Inventory = Manager.Store;
            // WARNING: Uses local copy of inventory contents, need to download them first.
            List<InventoryBase> contents = Inventory.GetContents(currentDirectory_);
            foreach (InventoryBase b in contents)
            {
                if (inventoryName == b.Name || inventoryName == b.UUID.ToString())
                {
                    if (b is OpenMetaverse.InventoryItem)
                    {
                        OpenMetaverse.InventoryItem item = b as OpenMetaverse.InventoryItem;
                        try
                        {
                            client_.Appearance.Detach(item as OpenMetaverse.InventoryItem);
                            break;
                        }
                        catch
                        {
                            MessageBox.Show(StringResource.failedDetach);
                            break;
                        }
                    }
                    else
                    {
                    }
                }
            }
        }

        /// <summary>�A�o�^�[������UUID���擾����</summary>
        public UUID getUUIDbyAvatarName(string avatarName)
        {
            Vector3 location = client_.Self.SimPosition;
            List<Avatar> avatars = client_.Network.CurrentSim.ObjectsAvatars.FindAll(
                delegate(Avatar avatar)
                {
                    Vector3 pos = avatar.Position;
                    return true;// ((pos != Vector3.Zero) && (Vector3.Dist(pos, location) < radius));
                }

            );

            foreach (Avatar a in avatars)
            {
                string name = a.Name;
                if ((name != null) && (name != client_.Self.Name) && (name.Contains(avatarName)))
                {
                    System.Diagnostics.Trace.WriteLine(name+ ","+a.ID);
                    return a.ID;
                }
            }
            return UUID.Zero;
        }

        /// <summary>�w�肵������ɃA�C�e����^����</summary>
        public void giveItem(UUID target, string itemName)
        {
            if (currentDirectory_ == null) InventoryInitialize();
            if (currentDirectory_ == null) return;
            string inventoryName = itemName;

            InventoryManager Manager = client_.Inventory;
            OpenMetaverse.Inventory Inventory = Manager.Store;
            // WARNING: Uses local copy of inventory contents, need to download them first.
            List<InventoryBase> contents = Inventory.GetContents(currentDirectory_);
            foreach (InventoryBase b in contents)
            {
                if (inventoryName == b.Name || inventoryName == b.UUID.ToString())
                {
                    if (b is OpenMetaverse.InventoryItem)
                    {
                        OpenMetaverse.InventoryItem item = b as OpenMetaverse.InventoryItem;
                        try
                        {
                            client_.Appearance.Detach(item as OpenMetaverse.InventoryItem);
                            Manager.GiveItem(item.UUID, item.Name, item.AssetType, target, true);
                            break;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                            break;
                        }
                    }
                    else
                    {
                    }
                }
            }
        }
    }
}
