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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellValueTextBox = new System.Windows.Forms.TextBox();
            this.solveButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Location = new System.Drawing.Point(3, 61);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(874, 325);
            this.spreadsheetPanel1.TabIndex = 1;
            this.spreadsheetPanel1.SelectionChanged += new SS.SelectionChangedHandler(this.updateTextBox);
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
            this.cellContentsBox.Location = new System.Drawing.Point(61, 30);
            this.cellContentsBox.Name = "cellContentsBox";
            this.cellContentsBox.ReadOnly = true;
            this.cellContentsBox.Size = new System.Drawing.Size(41, 20);
            this.cellContentsBox.TabIndex = 2;
            this.cellContentsBox.TextChanged += new System.EventHandler(this.cellContentsBox_TextChanged);
            // 
            // cellNameLabel
            // 
            this.cellNameLabel.AutoSize = true;
            this.cellNameLabel.Location = new System.Drawing.Point(0, 33);
            this.cellNameLabel.Name = "cellNameLabel";
            this.cellNameLabel.Size = new System.Drawing.Size(55, 13);
            this.cellNameLabel.TabIndex = 1;
            this.cellNameLabel.Text = "Cell Name";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(877, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openMenuItemClickHandler);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveMenuItemClickHandler);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeMenuItemClickHandler);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // cellValueTextBox
            // 
            this.cellValueTextBox.Location = new System.Drawing.Point(192, 30);
            this.cellValueTextBox.Name = "cellValueTextBox";
            this.cellValueTextBox.Size = new System.Drawing.Size(251, 20);
            this.cellValueTextBox.TabIndex = 4;
            this.cellValueTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cellValueTextBox_KeyPress);
            // 
            // solveButton
            // 
            this.solveButton.Location = new System.Drawing.Point(449, 28);
            this.solveButton.Name = "solveButton";
            this.solveButton.Size = new System.Drawing.Size(75, 23);
            this.solveButton.TabIndex = 5;
            this.solveButton.Text = "Solve";
            this.solveButton.UseVisualStyleBackColor = true;
            this.solveButton.Click += new System.EventHandler(this.solveButtonClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 386);
            this.Controls.Add(this.solveButton);
            this.Controls.Add(this.cellValueTextBox);
            this.Controls.Add(this.cellContentsBox);
            this.Controls.Add(this.cellNameLabel);
            this.Controls.Add(this.cellValueLabel);
            this.Controls.Add(this.spreadsheetPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.Label cellValueLabel;
        private System.Windows.Forms.TextBox cellContentsBox;
        private System.Windows.Forms.Label cellNameLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.TextBox cellValueTextBox;
        private System.Windows.Forms.Button solveButton;
    }
}

