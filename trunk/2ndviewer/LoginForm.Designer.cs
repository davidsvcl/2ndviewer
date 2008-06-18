namespace _2ndviewer
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nameloc_maskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.nameloc_radioButton = new System.Windows.Forms.RadioButton();
            this.lastloc_radioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.firstName_textBox = new System.Windows.Forms.TextBox();
            this.lastName_textBox = new System.Windows.Forms.TextBox();
            this.password_textBox = new System.Windows.Forms.TextBox();
            this.login_button = new System.Windows.Forms.Button();
            this.password_checkBox = new System.Windows.Forms.CheckBox();
            this.uri_checkBox = new System.Windows.Forms.CheckBox();
            this.uri_comboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nameloc_maskedTextBox);
            this.groupBox1.Controls.Add(this.nameloc_radioButton);
            this.groupBox1.Controls.Add(this.lastloc_radioButton);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // nameloc_maskedTextBox
            // 
            resources.ApplyResources(this.nameloc_maskedTextBox, "nameloc_maskedTextBox");
            this.nameloc_maskedTextBox.Name = "nameloc_maskedTextBox";
            // 
            // nameloc_radioButton
            // 
            resources.ApplyResources(this.nameloc_radioButton, "nameloc_radioButton");
            this.nameloc_radioButton.Name = "nameloc_radioButton";
            this.nameloc_radioButton.TabStop = true;
            this.nameloc_radioButton.UseVisualStyleBackColor = true;
            // 
            // lastloc_radioButton
            // 
            resources.ApplyResources(this.lastloc_radioButton, "lastloc_radioButton");
            this.lastloc_radioButton.Checked = true;
            this.lastloc_radioButton.Name = "lastloc_radioButton";
            this.lastloc_radioButton.TabStop = true;
            this.lastloc_radioButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // firstName_textBox
            // 
            resources.ApplyResources(this.firstName_textBox, "firstName_textBox");
            this.firstName_textBox.Name = "firstName_textBox";
            // 
            // lastName_textBox
            // 
            resources.ApplyResources(this.lastName_textBox, "lastName_textBox");
            this.lastName_textBox.Name = "lastName_textBox";
            // 
            // password_textBox
            // 
            resources.ApplyResources(this.password_textBox, "password_textBox");
            this.password_textBox.Name = "password_textBox";
            // 
            // login_button
            // 
            resources.ApplyResources(this.login_button, "login_button");
            this.login_button.Name = "login_button";
            this.login_button.UseVisualStyleBackColor = true;
            this.login_button.Click += new System.EventHandler(this.login_button_Click);
            // 
            // password_checkBox
            // 
            resources.ApplyResources(this.password_checkBox, "password_checkBox");
            this.password_checkBox.Name = "password_checkBox";
            this.password_checkBox.UseVisualStyleBackColor = true;
            // 
            // uri_checkBox
            // 
            resources.ApplyResources(this.uri_checkBox, "uri_checkBox");
            this.uri_checkBox.Name = "uri_checkBox";
            this.uri_checkBox.UseVisualStyleBackColor = true;
            // 
            // uri_comboBox
            // 
            this.uri_comboBox.FormattingEnabled = true;
            this.uri_comboBox.Items.AddRange(new object[] {
            resources.GetString("uri_comboBox.Items"),
            resources.GetString("uri_comboBox.Items1"),
            resources.GetString("uri_comboBox.Items2")});
            resources.ApplyResources(this.uri_comboBox, "uri_comboBox");
            this.uri_comboBox.Name = "uri_comboBox";
            // 
            // LoginForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uri_comboBox);
            this.Controls.Add(this.uri_checkBox);
            this.Controls.Add(this.password_checkBox);
            this.Controls.Add(this.login_button);
            this.Controls.Add(this.password_textBox);
            this.Controls.Add(this.lastName_textBox);
            this.Controls.Add(this.firstName_textBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.Shown += new System.EventHandler(this.LoginForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton nameloc_radioButton;
        private System.Windows.Forms.RadioButton lastloc_radioButton;
        private System.Windows.Forms.MaskedTextBox nameloc_maskedTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox firstName_textBox;
        private System.Windows.Forms.TextBox lastName_textBox;
        private System.Windows.Forms.TextBox password_textBox;
        private System.Windows.Forms.Button login_button;
        private System.Windows.Forms.CheckBox password_checkBox;
        private System.Windows.Forms.CheckBox uri_checkBox;
        private System.Windows.Forms.ComboBox uri_comboBox;
    }
}