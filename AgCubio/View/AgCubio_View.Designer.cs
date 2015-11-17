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
            this.fps_value_label = new System.Windows.Forms.Label();
            this.mass_label = new System.Windows.Forms.Label();
            this.mass_label_value = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // fps_label
            // 
            this.fps_label.AutoSize = true;
            this.fps_label.Location = new System.Drawing.Point(722, 23);
            this.fps_label.Name = "fps_label";
            this.fps_label.Size = new System.Drawing.Size(30, 13);
            this.fps_label.TabIndex = 0;
            this.fps_label.Text = "FPS:";
            // 
            // fps_value_label
            // 
            this.fps_value_label.AutoSize = true;
            this.fps_value_label.Location = new System.Drawing.Point(758, 23);
            this.fps_value_label.Name = "fps_value_label";
            this.fps_value_label.Size = new System.Drawing.Size(13, 13);
            this.fps_value_label.TabIndex = 1;
            this.fps_value_label.Text = "0";
            // 
            // mass_label
            // 
            this.mass_label.AutoSize = true;
            this.mass_label.Location = new System.Drawing.Point(717, 45);
            this.mass_label.Name = "mass_label";
            this.mass_label.Size = new System.Drawing.Size(35, 13);
            this.mass_label.TabIndex = 2;
            this.mass_label.Text = "Mass:";
            // 
            // mass_label_value
            // 
            this.mass_label_value.AutoSize = true;
            this.mass_label_value.Location = new System.Drawing.Point(758, 45);
            this.mass_label_value.Name = "mass_label_value";
            this.mass_label_value.Size = new System.Drawing.Size(19, 13);
            this.mass_label_value.TabIndex = 3;
            this.mass_label_value.Text = "0  ";
            // 
            // AgCubio_View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 648);
            this.Controls.Add(this.mass_label_value);
            this.Controls.Add(this.mass_label);
            this.Controls.Add(this.fps_value_label);
            this.Controls.Add(this.fps_label);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "AgCubio_View";
            this.Load += new System.EventHandler(this.AgCubio_View_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AgCubioPaint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AgCubio_View_KeyPress);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AgCubio_View_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label fps_label;
        private System.Windows.Forms.Label fps_value_label;
        private System.Windows.Forms.Label mass_label;
        private System.Windows.Forms.Label mass_label_value;

        public void SetFPSLabel(string x)
        {
            fps_value_label.Text = x;
        }

        public void SetMassLabel(string x)
        {
            mass_label_value.Text = x;
        }

  
    }
}

