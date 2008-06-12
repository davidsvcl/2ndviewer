namespace _2ndviewer
{
    partial class DebugForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.filter_comboBox = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AccessibleDescription = null;
            this.tableLayoutPanel1.AccessibleName = null;
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.BackgroundImage = null;
            this.tableLayoutPanel1.Controls.Add(this.filter_comboBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 0, 1);
            this.tableLayoutPanel1.Font = null;
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
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
            // textBox1
            // 
            this.textBox1.AccessibleDescription = null;
            this.textBox1.AccessibleName = null;
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.BackgroundImage = null;
            this.textBox1.Font = null;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            // 
            // DebugForm
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
            this.Name = "DebugForm";
            this.ToolTipText = null;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.ComboBox filter_comboBox;
    }
}