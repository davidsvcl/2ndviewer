namespace _2ndviewer
{
    partial class InventoryForm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InventoryForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.attachToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attachToRightHandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attachToLeftHandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detachToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.refresh_button = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AccessibleDescription = null;
            this.tableLayoutPanel1.AccessibleName = null;
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.BackgroundImage = null;
            this.tableLayoutPanel1.Controls.Add(this.treeView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Font = null;
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // treeView1
            // 
            this.treeView1.AccessibleDescription = null;
            this.treeView1.AccessibleName = null;
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.BackgroundImage = null;
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Font = null;
            this.treeView1.Name = "treeView1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.AccessibleDescription = null;
            this.contextMenuStrip1.AccessibleName = null;
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.BackgroundImage = null;
            this.contextMenuStrip1.Font = null;
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.attachToolStripMenuItem,
            this.attachToRightHandToolStripMenuItem,
            this.attachToLeftHandToolStripMenuItem,
            this.detachToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            // 
            // attachToolStripMenuItem
            // 
            this.attachToolStripMenuItem.AccessibleDescription = null;
            this.attachToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.attachToolStripMenuItem, "attachToolStripMenuItem");
            this.attachToolStripMenuItem.BackgroundImage = null;
            this.attachToolStripMenuItem.Name = "attachToolStripMenuItem";
            this.attachToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.attachToolStripMenuItem.Click += new System.EventHandler(this.attachToolStripMenuItem_Click);
            // 
            // attachToRightHandToolStripMenuItem
            // 
            this.attachToRightHandToolStripMenuItem.AccessibleDescription = null;
            this.attachToRightHandToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.attachToRightHandToolStripMenuItem, "attachToRightHandToolStripMenuItem");
            this.attachToRightHandToolStripMenuItem.BackgroundImage = null;
            this.attachToRightHandToolStripMenuItem.Name = "attachToRightHandToolStripMenuItem";
            this.attachToRightHandToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.attachToRightHandToolStripMenuItem.Click += new System.EventHandler(this.attachToRightHandToolStripMenuItem_Click);
            // 
            // attachToLeftHandToolStripMenuItem
            // 
            this.attachToLeftHandToolStripMenuItem.AccessibleDescription = null;
            this.attachToLeftHandToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.attachToLeftHandToolStripMenuItem, "attachToLeftHandToolStripMenuItem");
            this.attachToLeftHandToolStripMenuItem.BackgroundImage = null;
            this.attachToLeftHandToolStripMenuItem.Name = "attachToLeftHandToolStripMenuItem";
            this.attachToLeftHandToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.attachToLeftHandToolStripMenuItem.Click += new System.EventHandler(this.attachToLeftHandToolStripMenuItem_Click);
            // 
            // detachToolStripMenuItem
            // 
            this.detachToolStripMenuItem.AccessibleDescription = null;
            this.detachToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.detachToolStripMenuItem, "detachToolStripMenuItem");
            this.detachToolStripMenuItem.BackgroundImage = null;
            this.detachToolStripMenuItem.Name = "detachToolStripMenuItem";
            this.detachToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.detachToolStripMenuItem.Click += new System.EventHandler(this.detachToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.refresh_button);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // refresh_button
            // 
            this.refresh_button.AccessibleDescription = null;
            this.refresh_button.AccessibleName = null;
            resources.ApplyResources(this.refresh_button, "refresh_button");
            this.refresh_button.BackgroundImage = null;
            this.refresh_button.Font = null;
            this.refresh_button.Name = "refresh_button";
            this.refresh_button.UseVisualStyleBackColor = true;
            this.refresh_button.Click += new System.EventHandler(this.refresh_button_Click);
            // 
            // InventoryForm
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CloseButton = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = null;
            this.Icon = null;
            this.Name = "InventoryForm";
            this.ToolTipText = null;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button refresh_button;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem attachToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detachToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem attachToRightHandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem attachToLeftHandToolStripMenuItem;

    }
}