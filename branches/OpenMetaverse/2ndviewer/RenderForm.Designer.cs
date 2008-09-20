namespace _2ndviewer
{
    partial class RenderForm
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
            this.glControl = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.AccumBits = ((byte)(0));
            this.glControl.AutoCheckErrors = false;
            this.glControl.AutoFinish = false;
            this.glControl.AutoMakeCurrent = true;
            this.glControl.AutoSwapBuffers = true;
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.ColorBits = ((byte)(32));
            this.glControl.DepthBits = ((byte)(16));
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(334, 266);
            this.glControl.StencilBits = ((byte)(0));
            this.glControl.TabIndex = 8;
            this.glControl.TabStop = false;
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseMove);
            this.glControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseClick);
            this.glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseDown);
            this.glControl.Resize += new System.EventHandler(this.glControl_Resize_1);
            this.glControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseUp);
            // 
            // RenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 266);
            this.Controls.Add(this.glControl);
            this.Name = "RenderForm";
            this.Text = "RenderForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenderForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private Tao.Platform.Windows.SimpleOpenGlControl glControl;
    }
}