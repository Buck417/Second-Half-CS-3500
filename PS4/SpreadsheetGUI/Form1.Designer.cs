namespace SpreadsheetGUI
{
    partial class Form1
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
            this.spreadsheetPanel1 = new SS.SpreadsheetPanel();
            this.cellValueLabel = new System.Windows.Forms.Label();
            this.cellContentsBox = new System.Windows.Forms.TextBox();
            this.cellNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Location = new System.Drawing.Point(3, 61);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(960, 393);
            this.spreadsheetPanel1.TabIndex = 0;
            // 
            // cellValueLabel
            // 
            this.cellValueLabel.AutoSize = true;
            this.cellValueLabel.Location = new System.Drawing.Point(132, 33);
            this.cellValueLabel.Name = "cellValueLabel";
            this.cellValueLabel.Size = new System.Drawing.Size(54, 13);
            this.cellValueLabel.TabIndex = 1;
            this.cellValueLabel.Text = "Cell Value";
            // 
            // cellContentsBox
            // 
            this.cellContentsBox.Location = new System.Drawing.Point(113, 10);
            this.cellContentsBox.Name = "cellContentsBox";
            this.cellContentsBox.Size = new System.Drawing.Size(100, 20);
            this.cellContentsBox.TabIndex = 2;
            this.cellContentsBox.TextChanged += new System.EventHandler(this.cellContentsBox_TextChanged);
            // 
            // cellNameLabel
            // 
            this.cellNameLabel.AutoSize = true;
            this.cellNameLabel.Location = new System.Drawing.Point(43, 13);
            this.cellNameLabel.Name = "cellNameLabel";
            this.cellNameLabel.Size = new System.Drawing.Size(55, 13);
            this.cellNameLabel.TabIndex = 1;
            this.cellNameLabel.Text = "Cell Name";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 378);
            this.Controls.Add(this.cellContentsBox);
            this.Controls.Add(this.cellNameLabel);
            this.Controls.Add(this.cellValueLabel);
            this.Controls.Add(this.spreadsheetPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.Label cellValueLabel;
        private System.Windows.Forms.TextBox cellContentsBox;
        private System.Windows.Forms.Label cellNameLabel;
    }
}

