using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;
using libsecondlife;

namespace _2ndviewer
{
    public partial class InventoryForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private SecondLife client_;
        Inventory inventory_;
        InventoryManager manager_;
        System.Collections.Generic.List<InventoryItem> inventory_array_;

        public InventoryForm()
        {
            InitializeComponent();
            inventory_array_ = new System.Collections.Generic.List<InventoryItem>();
        }

        public void SetClient(SecondLife client)
        {
            client_ = client;
        }

        private void RefreshTree()
        {
            int current = -1;
            for (int i = 0; i < inventory_array_.Count; i++ )
            {
                InventoryItem item = inventory_array_[i];
                if (item.folder_ == true)
                {
                    this.treeView1.Nodes.Add(item.Name_);
                    current++;
                }
                else
                {
                    this.treeView1.Nodes[current].Nodes.Add(item.Name_);
                }
            }
        }

        private void PrintFolder(InventoryFolder f, int indent)
        {
            try
            {
                foreach (InventoryBase i in manager_.FolderContents(f.UUID, client_.Self.AgentID, true, true, InventorySortOrder.ByName, 9000))
                {
                    InventoryItem item = new InventoryItem();
                    item.indent_ = indent;
                    item.Name_ = i.Name;
                    item.uuid_ = i.UUID;
                    if (i is InventoryFolder) item.folder_ = true;
                    else item.folder_ = false;
                    inventory_array_.Add(item);
                    if (i is InventoryFolder)
                    {
                        InventoryFolder folder = (InventoryFolder)i;
                        PrintFolder(folder, indent + 1);
                    }
                }
            }
            catch {
                System.Diagnostics.Trace.WriteLine("What Happen!!!");
            }
        }

        public void InventoryInitialize()
        {
            this.treeView1.Nodes.Clear();
            inventory_array_.Clear();

            manager_ = client_.Inventory;
            inventory_ = manager_.Store;

            InventoryFolder rootFolder = inventory_.RootFolder;
            PrintFolder(rootFolder, 0);
        }

        private void refresh_button_Click(object sender, EventArgs e)
        {
            InventoryInitialize();
            RefreshTree();
        }

        private void attachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            attach(libsecondlife.AttachmentPoint.Default);
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
            //System.Diagnostics.Trace.WriteLine(this.treeView1.SelectedNode.Text);
            LLUUID inventoryItems;
            String[] SearchFolders = { "" };
            client_.Inventory.RequestFolderContents(client_.Inventory.Store.RootFolder.UUID, client_.Self.AgentID, true, true, InventorySortOrder.ByDate);
            SearchFolders[0] = "Objects";
            inventoryItems = client_.Inventory.FindObjectByPath(client_.Inventory.Store.RootFolder.UUID, client_.Self.AgentID, SearchFolders[0], 1000);
            SearchFolders[0] = this.treeView1.SelectedNode.Text;
            inventoryItems = client_.Inventory.FindObjectByPath(inventoryItems, client_.Self.AgentID, SearchFolders[0], 1000);
            libsecondlife.InventoryItem myitem;
            myitem = client_.Inventory.FetchItem(inventoryItems, client_.Self.AgentID, 1000);
            try
            {
                client_.Appearance.Attach(myitem as libsecondlife.InventoryItem, point);
            }
            catch
            {
                MessageBox.Show(StringResource.failedAttach);
            }
        }

        private void detachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(this.treeView1.SelectedNode.Text);
            LLUUID inventoryItems;
            String[] SearchFolders = { "" };
            client_.Inventory.RequestFolderContents(client_.Inventory.Store.RootFolder.UUID, client_.Self.AgentID, true, true, InventorySortOrder.ByDate);
            SearchFolders[0] = "Objects";
            inventoryItems = client_.Inventory.FindObjectByPath(client_.Inventory.Store.RootFolder.UUID, client_.Self.AgentID, SearchFolders[0], 1000);
            SearchFolders[0] = this.treeView1.SelectedNode.Text;
            inventoryItems = client_.Inventory.FindObjectByPath(inventoryItems, client_.Self.AgentID, SearchFolders[0], 1000);
            libsecondlife.InventoryItem myitem;
            myitem = client_.Inventory.FetchItem(inventoryItems, client_.Self.AgentID, 1000);
            try
            {
                client_.Appearance.Detach(myitem as libsecondlife.InventoryItem);
            }
            catch {
                MessageBox.Show(StringResource.failedDetach);
            }
        }
    }
}
