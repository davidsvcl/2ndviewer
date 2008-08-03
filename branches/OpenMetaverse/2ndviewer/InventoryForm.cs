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
        Inventory inventory_;
        InventoryManager manager_;
        System.Collections.Generic.List<InventoryItem> inventory_array_;

        public InventoryForm()
        {
            InitializeComponent();
            inventory_array_ = new System.Collections.Generic.List<InventoryItem>();
        }

        public void SetClient(GridClient client)
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
                //foreach (InventoryBase i in manager_.FolderContents(f.UUID, client_.Self.AgentID, true, true, InventorySortOrder.ByName, 9000))
                f.DownloadContents(TimeSpan.FromSeconds(10));
                foreach (InventoryBase i in f)
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
            inventory_ = client_.InventoryStore;

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
            /*
            //System.Diagnostics.Trace.WriteLine(this.treeView1.SelectedNode.Text);
            UUID inventoryItems;
            String[] SearchFolders = { "" };
            //client_.Inventory.RequestFolderContents(client_.InventoryStore.RootFolder.UUID, client_.Self.AgentID, true, true, InventorySortOrder.ByDate);
            SearchFolders[0] = "Objects";
            //inventoryItems = client_.Inventory.FindObjectByPath(client_.InventoryStore.RootFolder.UUID, client_.Self.AgentID, SearchFolders[0], 1000);
            inventoryItems = client_.Inventory.FindObjectByPath(client_.InventoryStore.RootFolder.UUID, client_.Self.AgentID, SearchFolders[0], TimeSpan.FromSeconds(30));
            SearchFolders[0] = this.treeView1.SelectedNode.Text;
            //inventoryItems = client_.Inventory.FindObjectByPath(inventoryItems, client_.Self.AgentID, SearchFolders[0], 1000);
            inventoryItems = client_.Inventory.FindObjectByPath(inventoryItems, client_.Self.AgentID, SearchFolders[0], TimeSpan.FromSeconds(30));
            //OpenMetaverse.InventoryItem myitem;
            //myitem = client_.Inventory.FetchItem(inventoryItems, client_.Self.AgentID, 1000);
            ItemData itemData;
            client_.Inventory.FetchItem(inventoryItems, client_.Self.AgentID, TimeSpan.FromSeconds(30), out itemData);
            try
            {
                //client_.Appearance.Attach(myitem as OpenMetaverse.InventoryItem, point);
                client_.Appearance.Attach(itemData, point);
            }
            catch
            {
                MessageBox.Show(StringResource.failedAttach);
            }
            */
        }

        private void detachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            System.Diagnostics.Trace.WriteLine(this.treeView1.SelectedNode.Text);
            UUID inventoryItems;
            String[] SearchFolders = { "" };
            client_.Inventory.RequestFolderContents(client_.Inventory.Store.RootFolder.UUID, client_.Self.AgentID, true, true, InventorySortOrder.ByDate);
            SearchFolders[0] = "Objects";
            inventoryItems = client_.Inventory.FindObjectByPath(client_.Inventory.Store.RootFolder.UUID, client_.Self.AgentID, SearchFolders[0], 1000);
            SearchFolders[0] = this.treeView1.SelectedNode.Text;
            inventoryItems = client_.Inventory.FindObjectByPath(inventoryItems, client_.Self.AgentID, SearchFolders[0], 1000);
            OpenMetaverse.InventoryItem myitem;
            myitem = client_.Inventory.FetchItem(inventoryItems, client_.Self.AgentID, 1000);
            try
            {
                client_.Appearance.Detach(myitem as OpenMetaverse.InventoryItem);
            }
            catch {
                MessageBox.Show(StringResource.failedDetach);
            }
            */
        }
    }
}
