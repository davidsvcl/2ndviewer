namespace _2ndviewer
{
    partial class OptionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionForm));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nickname_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.news4vip_textBox = new System.Windows.Forms.TextBox();
            this.confirm_checkBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.AccessibleDescription = null;
            this.button1.AccessibleName = null;
            resources.ApplyResources(this.button1, "button1");
            this.button1.BackgroundImage = null;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Font = null;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.AccessibleDescription = null;
            this.button2.AccessibleName = null;
            resources.ApplyResources(this.button2, "button2");
            this.button2.BackgroundImage = null;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Font = null;
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // nickname_textBox
            // 
            this.nickname_textBox.AccessibleDescription = null;
            this.nickname_textBox.AccessibleName = null;
            resources.ApplyResources(this.nickname_textBox, "nickname_textBox");
            this.nickname_textBox.BackgroundImage = null;
            this.nickname_textBox.Font = null;
            this.nickname_textBox.Name = "nickname_textBox";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // news4vip_textBox
            // 
            this.news4vip_textBox.AccessibleDescription = null;
            this.news4vip_textBox.AccessibleName = null;
            resources.ApplyResources(this.news4vip_textBox, "news4vip_textBox");
            this.news4vip_textBox.BackgroundImage = null;
            this.news4vip_textBox.Font = null;
            this.news4vip_textBox.Name = "news4vip_textBox";
            // 
            // confirm_checkBox
            // 
            this.confirm_checkBox.AccessibleDescription = null;
            this.confirm_checkBox.AccessibleName = null;
            resources.ApplyResources(this.confirm_checkBox, "confirm_checkBox");
            this.confirm_checkBox.BackgroundImage = null;
            this.confirm_checkBox.Font = null;
            this.confirm_checkBox.Name = "confirm_checkBox";
            this.confirm_checkBox.UseVisualStyleBackColor = true;
            // 
            // OptionForm
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.confirm_checkBox);
            this.Controls.Add(this.news4vip_textBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nickname_textBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox nickname_textBox;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox news4vip_textBox;
        public System.Windows.Forms.CheckBox confirm_checkBox;
    }
}