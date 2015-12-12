namespace View
{
    partial class Start_Game_Form
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
            this.play_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.name_box = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.game_host_box = new System.Windows.Forms.TextBox();
            this.connection_error_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // play_button
            // 
            this.play_button.Location = new System.Drawing.Point(110, 144);
            this.play_button.Name = "play_button";
            this.play_button.Size = new System.Drawing.Size(75, 23);
            this.play_button.TabIndex = 3;
            this.play_button.Text = "Play";
            this.play_button.UseVisualStyleBackColor = true;
            this.play_button.Click += new System.EventHandler(this.play_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Your Name";
            // 
            // name_box
            // 
            this.name_box.Location = new System.Drawing.Point(125, 83);
            this.name_box.Name = "name_box";
            this.name_box.Size = new System.Drawing.Size(100, 20);
            this.name_box.TabIndex = 1;
            this.name_box.TextChanged += new System.EventHandler(this.name_box_TextChanged);
            this.name_box.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inputKeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Game Host";
            // 
            // game_host_box
            // 
            this.game_host_box.Location = new System.Drawing.Point(125, 109);
            this.game_host_box.Name = "game_host_box";
            this.game_host_box.Size = new System.Drawing.Size(100, 20);
            this.game_host_box.TabIndex = 2;
            this.game_host_box.Text = "localhost";
            this.game_host_box.TextChanged += new System.EventHandler(this.game_host_box_TextChanged);
            this.game_host_box.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inputKeyPress);
            // 
            // connection_error_label
            // 
            this.connection_error_label.AutoSize = true;
            this.connection_error_label.Location = new System.Drawing.Point(62, 43);
            this.connection_error_label.Name = "connection_error_label";
            this.connection_error_label.Size = new System.Drawing.Size(131, 13);
            this.connection_error_label.TabIndex = 4;
            this.connection_error_label.Text = "Connection error, try again";
            this.connection_error_label.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.connection_error_label);
            this.Controls.Add(this.game_host_box);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.name_box);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.play_button);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button play_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox name_box;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox game_host_box;
        private System.Windows.Forms.Label connection_error_label;
    }
}