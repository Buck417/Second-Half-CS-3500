namespace View
{
    partial class GameOverForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mass_label = new System.Windows.Forms.Label();
            this.name_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(105, 199);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Play Again";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.restart_click_handler);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Quartz MS", 28.25F);
            this.label1.Location = new System.Drawing.Point(28, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 45);
            this.label1.TabIndex = 1;
            this.label1.Text = "GAME OVER";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mass_label
            // 
            this.mass_label.AutoSize = true;
            this.mass_label.Location = new System.Drawing.Point(123, 162);
            this.mass_label.Name = "mass_label";
            this.mass_label.Size = new System.Drawing.Size(38, 13);
            this.mass_label.TabIndex = 2;
            this.mass_label.Text = "Mass: ";
            this.mass_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // name_label
            // 
            this.name_label.AutoSize = true;
            this.name_label.Location = new System.Drawing.Point(123, 137);
            this.name_label.Name = "name_label";
            this.name_label.Size = new System.Drawing.Size(38, 13);
            this.name_label.TabIndex = 3;
            this.name_label.Text = "Name:";
            // 
            // GameOverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.name_label);
            this.Controls.Add(this.mass_label);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "GameOverForm";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label mass_label;
        private System.Windows.Forms.Label name_label;
    }
}