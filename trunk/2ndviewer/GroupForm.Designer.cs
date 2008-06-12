namespace _2ndviewer
{
    partial class GroupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupForm));
            this.group_listBox = new System.Windows.Forms.ListBox();
            this.create_button = new System.Windows.Forms.Button();
            this.activate_button = new System.Windows.Forms.Button();
            this.info_button = new System.Windows.Forms.Button();
            this.leave_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // group_listBox
            // 
            this.group_listBox.FormattingEnabled = true;
            resources.ApplyResources(this.group_listBox, "group_listBox");
            this.group_listBox.Name = "group_listBox";
            this.group_listBox.SelectedIndexChanged += new System.EventHandler(this.group_listBox_SelectedIndexChanged);
            // 
            // create_button
            // 
            resources.ApplyResources(this.create_button, "create_button");
            this.create_button.Name = "create_button";
            this.create_button.UseVisualStyleBackColor = true;
            this.create_button.Click += new System.EventHandler(this.create_button_Click);
            // 
            // activate_button
            // 
            resources.ApplyResources(this.activate_button, "activate_button");
            this.activate_button.Name = "activate_button";
            this.activate_button.UseVisualStyleBackColor = true;
            this.activate_button.Click += new System.EventHandler(this.activate_button_Click);
            // 
            // info_button
            // 
            resources.ApplyResources(this.info_button, "info_button");
            this.info_button.Name = "info_button";
            this.info_button.UseVisualStyleBackColor = true;
            this.info_button.Click += new System.EventHandler(this.info_button_Click);
            // 
            // leave_button
            // 
            resources.ApplyResources(this.leave_button, "leave_button");
            this.leave_button.Name = "leave_button";
            this.leave_button.UseVisualStyleBackColor = true;
            this.leave_button.Click += new System.EventHandler(this.leave_button_Click);
            // 
            // GroupForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CloseButton = false;
            this.Controls.Add(this.leave_button);
            this.Controls.Add(this.info_button);
            this.Controls.Add(this.activate_button);
            this.Controls.Add(this.create_button);
            this.Controls.Add(this.group_listBox);
            this.Name = "GroupForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button create_button;
        private System.Windows.Forms.Button activate_button;
        private System.Windows.Forms.Button info_button;
        private System.Windows.Forms.Button leave_button;
        public System.Windows.Forms.ListBox group_listBox;
    }
}