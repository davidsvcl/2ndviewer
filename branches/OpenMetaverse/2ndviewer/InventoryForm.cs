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
    public partial class InventoryForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private GridClient client_;
        private InventoryFolder currentDirectory_;

        public InventoryForm()
        {
            InitializeComponent();
        }

        public void SetClient(GridClient client)
        {
            client_ = client;
            treeView1.Client = client;

        }

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
                    }
                    else
                    {
                    }
                }
            }
        }

        private void RefreshTree()
        {
        }

        private void refresh_button_Click(object sender, EventArgs e)
        {
            RefreshTree();
        }

        private void attachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            attach(OpenMetaverse.AttachmentPoint.Default);
        }

        private void attachToRightHandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            attach(AttachmentPoint.RightHand);
        }

        private void attachToLeftHandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            attach(AttachmentPoint.LeftHand);
        }

        private void attach(AttachmentPoint point)
        {
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
                        }
                        catch
                        {
                            MessageBox.Show(StringResource.failedAttach);
                        }
                    }
                    else
                    {
                    }
                }
            }
        }

        private void detachToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                        }
                        catch
                        {
                            MessageBox.Show(StringResource.failedDetach);
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
