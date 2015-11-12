namespace View
{
    partial class AgCubio_View
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fps_label = new System.Windows.Forms.Label();
            this.fps_box = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // fps_label
            // 
            this.fps_label.AutoSize = true;
            this.fps_label.Location = new System.Drawing.Point(658, 29);
            this.fps_label.Name = "fps_label";
            this.fps_label.Size = new System.Drawing.Size(30, 13);
            this.fps_label.TabIndex = 0;
            this.fps_label.Text = "FPS:";
            // 
            // fps_box
            // 
            this.fps_box.Location = new System.Drawing.Point(695, 29);
            this.fps_box.Name = "fps_box";
            this.fps_box.ReadOnly = true;
            this.fps_box.Size = new System.Drawing.Size(100, 20);
            this.fps_box.TabIndex = 1;
            // 
            // AgCubio_View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 648);
            this.Controls.Add(this.fps_box);
            this.Controls.Add(this.fps_label);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "AgCubio_View";
            this.Text = "Form1";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AgCubioPaint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AgCubio_View_KeyPress);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AgCubio_View_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label fps_label;
        private System.Windows.Forms.TextBox fps_box;
    }
}

