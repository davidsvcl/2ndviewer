namespace _2ndviewer
{
    partial class MinimapForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MinimapForm));
            this.refresh_button = new System.Windows.Forms.Button();
            this.filter_comboBox = new System.Windows.Forms.ComboBox();
            this.world = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.world)).BeginInit();
            this.SuspendLayout();
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
            // filter_comboBox
            // 
            this.filter_comboBox.AccessibleDescription = null;
            this.filter_comboBox.AccessibleName = null;
            resources.ApplyResources(this.filter_comboBox, "filter_comboBox");
            this.filter_comboBox.BackgroundImage = null;
            this.filter_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filter_comboBox.Font = null;
            this.filter_comboBox.FormattingEnabled = true;
            this.filter_comboBox.Items.AddRange(new object[] {
            resources.GetString("filter_comboBox.Items"),
            resources.GetString("filter_comboBox.Items1")});
            this.filter_comboBox.Name = "filter_comboBox";
            this.filter_comboBox.SelectedIndexChanged += new System.EventHandler(this.filter_comboBox_SelectedIndexChanged);
            // 
            // world
            // 
            this.world.AccessibleDescription = null;
            this.world.AccessibleName = null;
            resources.ApplyResources(this.world, "world");
            this.world.BackgroundImage = null;
            this.world.Font = null;
            this.world.ImageLocation = null;
            this.world.Name = "world";
            this.world.TabStop = false;
            this.world.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.world_MouseDoubleClick);
            this.world.MouseClick += new System.Windows.Forms.MouseEventHandler(this.world_MouseClick);
            // 
            // MinimapForm
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CloseButton = false;
            this.Controls.Add(this.filter_comboBox);
            this.Controls.Add(this.refresh_button);
            this.Controls.Add(this.world);
            this.Font = null;
            this.Icon = null;
            this.Name = "MinimapForm";
            this.ToolTipText = null;
            ((System.ComponentModel.ISupportInitialize)(this.world)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox world;
        private System.Windows.Forms.Button refresh_button;
        public System.Windows.Forms.ComboBox filter_comboBox;
    }
}