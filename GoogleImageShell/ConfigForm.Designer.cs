namespace GoogleImageShell
{
    partial class ConfigForm
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
            this.installButton = new System.Windows.Forms.Button();
            this.uninstallButton = new System.Windows.Forms.Button();
            this.menuTextTextBox = new System.Windows.Forms.TextBox();
            this.menuTextLabel = new System.Windows.Forms.Label();
            this.includeFileNameCheckBox = new System.Windows.Forms.CheckBox();
            this.allUsersCheckBox = new System.Windows.Forms.CheckBox();
            this.fileTypeListBox = new System.Windows.Forms.CheckedListBox();
            this.fileTypeLabel = new System.Windows.Forms.Label();
            this.resizeOnUploadCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // installButton
            // 
            this.installButton.Location = new System.Drawing.Point(12, 195);
            this.installButton.Name = "installButton";
            this.installButton.Size = new System.Drawing.Size(175, 30);
            this.installButton.TabIndex = 0;
            this.installButton.Text = "Install";
            this.installButton.UseVisualStyleBackColor = true;
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // uninstallButton
            // 
            this.uninstallButton.Location = new System.Drawing.Point(197, 195);
            this.uninstallButton.Name = "uninstallButton";
            this.uninstallButton.Size = new System.Drawing.Size(175, 30);
            this.uninstallButton.TabIndex = 1;
            this.uninstallButton.Text = "Uninstall";
            this.uninstallButton.UseVisualStyleBackColor = true;
            this.uninstallButton.Click += new System.EventHandler(this.uninstallButton_Click);
            // 
            // menuTextTextBox
            // 
            this.menuTextTextBox.Location = new System.Drawing.Point(110, 12);
            this.menuTextTextBox.Name = "menuTextTextBox";
            this.menuTextTextBox.Size = new System.Drawing.Size(262, 20);
            this.menuTextTextBox.TabIndex = 0;
            this.menuTextTextBox.Text = "Search on Google Images";
            // 
            // menuTextLabel
            // 
            this.menuTextLabel.AutoSize = true;
            this.menuTextLabel.Location = new System.Drawing.Point(12, 15);
            this.menuTextLabel.Name = "menuTextLabel";
            this.menuTextLabel.Size = new System.Drawing.Size(92, 13);
            this.menuTextLabel.TabIndex = 1;
            this.menuTextLabel.Text = "Context menu text";
            // 
            // includeFileNameCheckBox
            // 
            this.includeFileNameCheckBox.AutoSize = true;
            this.includeFileNameCheckBox.Checked = true;
            this.includeFileNameCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeFileNameCheckBox.Location = new System.Drawing.Point(15, 38);
            this.includeFileNameCheckBox.Name = "includeFileNameCheckBox";
            this.includeFileNameCheckBox.Size = new System.Drawing.Size(152, 17);
            this.includeFileNameCheckBox.TabIndex = 2;
            this.includeFileNameCheckBox.Text = "Include file name in search";
            this.includeFileNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // allUsersCheckBox
            // 
            this.allUsersCheckBox.AutoSize = true;
            this.allUsersCheckBox.Location = new System.Drawing.Point(15, 85);
            this.allUsersCheckBox.Name = "allUsersCheckBox";
            this.allUsersCheckBox.Size = new System.Drawing.Size(307, 17);
            this.allUsersCheckBox.TabIndex = 3;
            this.allUsersCheckBox.Text = "Install/uninstall for all users (requires administrator privileges)";
            this.allUsersCheckBox.UseVisualStyleBackColor = true;
            // 
            // fileTypeListBox
            // 
            this.fileTypeListBox.CheckOnClick = true;
            this.fileTypeListBox.FormattingEnabled = true;
            this.fileTypeListBox.Location = new System.Drawing.Point(12, 125);
            this.fileTypeListBox.Name = "fileTypeListBox";
            this.fileTypeListBox.Size = new System.Drawing.Size(360, 64);
            this.fileTypeListBox.TabIndex = 4;
            // 
            // fileTypeLabel
            // 
            this.fileTypeLabel.AutoSize = true;
            this.fileTypeLabel.Location = new System.Drawing.Point(12, 109);
            this.fileTypeLabel.Name = "fileTypeLabel";
            this.fileTypeLabel.Size = new System.Drawing.Size(168, 13);
            this.fileTypeLabel.TabIndex = 5;
            this.fileTypeLabel.Text = "Install/uninstall for these file types:";
            // 
            // resizeOnUploadCheckbox
            // 
            this.resizeOnUploadCheckbox.AutoSize = true;
            this.resizeOnUploadCheckbox.Checked = true;
            this.resizeOnUploadCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resizeOnUploadCheckbox.Location = new System.Drawing.Point(15, 62);
            this.resizeOnUploadCheckbox.Name = "resizeOnUploadCheckbox";
            this.resizeOnUploadCheckbox.Size = new System.Drawing.Size(202, 17);
            this.resizeOnUploadCheckbox.TabIndex = 6;
            this.resizeOnUploadCheckbox.Text = "Resize before uploading large images";
            this.resizeOnUploadCheckbox.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(384, 234);
            this.Controls.Add(this.resizeOnUploadCheckbox);
            this.Controls.Add(this.fileTypeLabel);
            this.Controls.Add(this.fileTypeListBox);
            this.Controls.Add(this.allUsersCheckBox);
            this.Controls.Add(this.includeFileNameCheckBox);
            this.Controls.Add(this.menuTextLabel);
            this.Controls.Add(this.menuTextTextBox);
            this.Controls.Add(this.uninstallButton);
            this.Controls.Add(this.installButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ConfigForm";
            this.ShowIcon = false;
            this.Text = "GoogleImageShell";
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button installButton;
        private System.Windows.Forms.Button uninstallButton;
        private System.Windows.Forms.TextBox menuTextTextBox;
        private System.Windows.Forms.Label menuTextLabel;
        private System.Windows.Forms.CheckBox includeFileNameCheckBox;
        private System.Windows.Forms.CheckBox allUsersCheckBox;
        private System.Windows.Forms.CheckedListBox fileTypeListBox;
        private System.Windows.Forms.Label fileTypeLabel;
        private System.Windows.Forms.CheckBox resizeOnUploadCheckbox;
    }
}