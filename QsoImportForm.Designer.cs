namespace QSOCollector
{
    partial class QsoImportForm
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
            closeCancelButton = new Button();
            importButton = new Button();
            exportProgressBar = new ProgressBar();
            chooseFileButton = new Button();
            filePathLabel = new Label();
            filePreviewTextBox = new TextBox();
            SuspendLayout();
            // 
            // closeCancelButton
            // 
            closeCancelButton.BackColor = Color.RosyBrown;
            closeCancelButton.FlatStyle = FlatStyle.Popup;
            closeCancelButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            closeCancelButton.Location = new Point(33, 413);
            closeCancelButton.Name = "closeCancelButton";
            closeCancelButton.Size = new Size(185, 38);
            closeCancelButton.TabIndex = 11;
            closeCancelButton.Text = "Close";
            closeCancelButton.UseVisualStyleBackColor = false;
            // 
            // importButton
            // 
            importButton.BackColor = Color.Transparent;
            importButton.Enabled = false;
            importButton.FlatStyle = FlatStyle.Popup;
            importButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            importButton.Location = new Point(265, 413);
            importButton.Name = "importButton";
            importButton.Size = new Size(180, 38);
            importButton.TabIndex = 12;
            importButton.Text = "Import";
            importButton.UseVisualStyleBackColor = false;
            importButton.Click += importButton_Click;
            // 
            // exportProgressBar
            // 
            exportProgressBar.Location = new Point(13, 457);
            exportProgressBar.Name = "exportProgressBar";
            exportProgressBar.Size = new Size(477, 19);
            exportProgressBar.TabIndex = 13;
            exportProgressBar.Visible = false;
            // 
            // chooseFileButton
            // 
            chooseFileButton.BackColor = Color.SteelBlue;
            chooseFileButton.FlatStyle = FlatStyle.Popup;
            chooseFileButton.ForeColor = Color.Transparent;
            chooseFileButton.Location = new Point(11, 9);
            chooseFileButton.Name = "chooseFileButton";
            chooseFileButton.Size = new Size(129, 29);
            chooseFileButton.TabIndex = 14;
            chooseFileButton.Text = "Choose ADIF file";
            chooseFileButton.UseVisualStyleBackColor = false;
            chooseFileButton.Click += chooseFileButton_Click;
            // 
            // filePathLabel
            // 
            filePathLabel.Location = new Point(11, 41);
            filePathLabel.Name = "filePathLabel";
            filePathLabel.Size = new Size(479, 78);
            filePathLabel.TabIndex = 15;
            filePathLabel.Text = "No file chosen";
            // 
            // filePreviewTextBox
            // 
            filePreviewTextBox.BackColor = Color.Silver;
            filePreviewTextBox.Location = new Point(13, 122);
            filePreviewTextBox.Multiline = true;
            filePreviewTextBox.Name = "filePreviewTextBox";
            filePreviewTextBox.PlaceholderText = "File preview will be here (limited to 32767 chars) ...";
            filePreviewTextBox.ReadOnly = true;
            filePreviewTextBox.ScrollBars = ScrollBars.Both;
            filePreviewTextBox.Size = new Size(477, 285);
            filePreviewTextBox.TabIndex = 16;
            filePreviewTextBox.WordWrap = false;
            // 
            // QsoImportForm
            // 
            AccessibleRole = AccessibleRole.None;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = closeCancelButton;
            ClientSize = new Size(502, 488);
            Controls.Add(filePreviewTextBox);
            Controls.Add(filePathLabel);
            Controls.Add(chooseFileButton);
            Controls.Add(exportProgressBar);
            Controls.Add(importButton);
            Controls.Add(closeCancelButton);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "QsoImportForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Import from ADIF";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button closeCancelButton;
        private Button importButton;
        private ProgressBar exportProgressBar;
        private Button chooseFileButton;
        private Label filePathLabel;
        private TextBox filePreviewTextBox;
    }
}