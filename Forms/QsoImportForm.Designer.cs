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
            chooseFileButton = new Button();
            filePathLabel = new Label();
            filePreviewTextBox = new TextBox();
            importBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            importProgressBar = new ProgressBar();
            importProgressStep = new Label();
            SuspendLayout();
            // 
            // closeCancelButton
            // 
            closeCancelButton.BackColor = Color.RosyBrown;
            closeCancelButton.FlatStyle = FlatStyle.Popup;
            closeCancelButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            closeCancelButton.Location = new Point(39, 415);
            closeCancelButton.Name = "closeCancelButton";
            closeCancelButton.Size = new Size(180, 38);
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
            importButton.Location = new Point(259, 415);
            importButton.Name = "importButton";
            importButton.Size = new Size(182, 38);
            importButton.TabIndex = 12;
            importButton.Text = "Import";
            importButton.UseVisualStyleBackColor = false;
            importButton.Click += importButton_Click;
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
            // importBackgroundWorker
            // 
            importBackgroundWorker.WorkerReportsProgress = true;
            importBackgroundWorker.DoWork += importBackgroundWorker_DoWork;
            importBackgroundWorker.ProgressChanged += importBackgroundWorker_ProgressChanged;
            importBackgroundWorker.RunWorkerCompleted += importBackgroundWorker_RunWorkerCompleted;
            // 
            // importProgressBar
            // 
            importProgressBar.Location = new Point(13, 464);
            importProgressBar.Name = "importProgressBar";
            importProgressBar.Size = new Size(141, 17);
            importProgressBar.Style = ProgressBarStyle.Marquee;
            importProgressBar.TabIndex = 17;
            importProgressBar.Visible = false;
            // 
            // importProgressStep
            // 
            importProgressStep.Location = new Point(160, 460);
            importProgressStep.Name = "importProgressStep";
            importProgressStep.Size = new Size(330, 25);
            importProgressStep.TabIndex = 18;
            importProgressStep.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // QsoImportForm
            // 
            AccessibleRole = AccessibleRole.None;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = closeCancelButton;
            ClientSize = new Size(502, 488);
            Controls.Add(importProgressStep);
            Controls.Add(importProgressBar);
            Controls.Add(filePreviewTextBox);
            Controls.Add(filePathLabel);
            Controls.Add(chooseFileButton);
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
        private Button chooseFileButton;
        private Label filePathLabel;
        private TextBox filePreviewTextBox;
        private System.ComponentModel.BackgroundWorker importBackgroundWorker;
        private ProgressBar importProgressBar;
        private Label importProgressStep;
    }
}