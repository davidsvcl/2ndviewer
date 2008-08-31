namespace _2ndviewer
{
    partial class FriendForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FriendForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.iMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teleportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.profile_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.listBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // listBox1
            // 
            this.listBox1.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.listBox1, "listBox1");
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Name = "listBox1";
            this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iMToolStripMenuItem,
            this.teleportToolStripMenuItem,
            this.profile_ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // iMToolStripMenuItem
            // 
            this.iMToolStripMenuItem.Name = "iMToolStripMenuItem";
            resources.ApplyResources(this.iMToolStripMenuItem, "iMToolStripMenuItem");
            this.iMToolStripMenuItem.Click += new System.EventHandler(this.iMToolStripMenuItem_Click);
            // 
            // teleportToolStripMenuItem
            // 
            this.teleportToolStripMenuItem.Name = "teleportToolStripMenuItem";
            resources.ApplyResources(this.teleportToolStripMenuItem, "teleportToolStripMenuItem");
            this.teleportToolStripMenuItem.Click += new System.EventHandler(this.teleportToolStripMenuItem_Click);
            // 
            // profile_ToolStripMenuItem
            // 
            this.profile_ToolStripMenuItem.Name = "profile_ToolStripMenuItem";
            resources.ApplyResources(this.profile_ToolStripMenuItem, "profile_ToolStripMenuItem");
            this.profile_ToolStripMenuItem.Click += new System.EventHandler(this.profile_ToolStripMenuItem_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FriendForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CloseButton = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FriendForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem iMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teleportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem profile_ToolStripMenuItem;

    }
}