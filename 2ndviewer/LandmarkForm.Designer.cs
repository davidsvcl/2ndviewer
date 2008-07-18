namespace _2ndviewer
{
    partial class LandmarkForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LandmarkForm));
            this.label1 = new System.Windows.Forms.Label();
            this.name_textBox = new System.Windows.Forms.TextBox();
            this.simname_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.x_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.y_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.z_textBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.memo_textBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ok_button = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // name_textBox
            // 
            resources.ApplyResources(this.name_textBox, "name_textBox");
            this.name_textBox.Name = "name_textBox";
            // 
            // simname_textBox
            // 
            resources.ApplyResources(this.simname_textBox, "simname_textBox");
            this.simname_textBox.Name = "simname_textBox";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // x_textBox
            // 
            resources.ApplyResources(this.x_textBox, "x_textBox");
            this.x_textBox.Name = "x_textBox";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // y_textBox
            // 
            resources.ApplyResources(this.y_textBox, "y_textBox");
            this.y_textBox.Name = "y_textBox";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // z_textBox
            // 
            resources.ApplyResources(this.z_textBox, "z_textBox");
            this.z_textBox.Name = "z_textBox";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // memo_textBox
            // 
            resources.ApplyResources(this.memo_textBox, "memo_textBox");
            this.memo_textBox.Name = "memo_textBox";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // ok_button
            // 
            this.ok_button.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.ok_button, "ok_button");
            this.ok_button.Name = "ok_button";
            this.ok_button.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // LandmarkForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.ok_button);
            this.Controls.Add(this.memo_textBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.z_textBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.y_textBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.x_textBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.simname_textBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.name_textBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LandmarkForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button ok_button;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.TextBox name_textBox;
        public System.Windows.Forms.TextBox simname_textBox;
        public System.Windows.Forms.TextBox x_textBox;
        public System.Windows.Forms.TextBox y_textBox;
        public System.Windows.Forms.TextBox z_textBox;
        public System.Windows.Forms.TextBox memo_textBox;
    }
}