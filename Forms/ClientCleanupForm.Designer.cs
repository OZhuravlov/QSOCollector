namespace QSOCollector
{
    partial class ClientCleanupForm
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
            cleanTemporarelySavedQsoCheckBox = new CheckBox();
            cleanUdpListenerConfigCheckBox = new CheckBox();
            cleanServerIpAndPortConfigCheckBox = new CheckBox();
            cleanupClientLabel = new Label();
            cancelButton = new Button();
            confirmCleanupButton = new Button();
            SuspendLayout();
            // 
            // cleanTemporarelySavedQsoCheckBox
            // 
            cleanTemporarelySavedQsoCheckBox.AutoSize = true;
            cleanTemporarelySavedQsoCheckBox.Location = new Point(9, 46);
            cleanTemporarelySavedQsoCheckBox.Name = "cleanTemporarelySavedQsoCheckBox";
            cleanTemporarelySavedQsoCheckBox.Size = new Size(156, 19);
            cleanTemporarelySavedQsoCheckBox.TabIndex = 0;
            cleanTemporarelySavedQsoCheckBox.Text = "Temporarely saved QSOs";
            cleanTemporarelySavedQsoCheckBox.UseVisualStyleBackColor = true;
            cleanTemporarelySavedQsoCheckBox.CheckedChanged += cleanTemporarelySavedQsoCheckBox_CheckedChanged;
            // 
            // cleanUdpListenerConfigCheckBox
            // 
            cleanUdpListenerConfigCheckBox.AutoSize = true;
            cleanUdpListenerConfigCheckBox.Location = new Point(9, 71);
            cleanUdpListenerConfigCheckBox.Name = "cleanUdpListenerConfigCheckBox";
            cleanUdpListenerConfigCheckBox.Size = new Size(168, 19);
            cleanUdpListenerConfigCheckBox.TabIndex = 1;
            cleanUdpListenerConfigCheckBox.Text = "UDP Listener configuration";
            cleanUdpListenerConfigCheckBox.UseVisualStyleBackColor = true;
            cleanUdpListenerConfigCheckBox.CheckedChanged += cleanUdpListenerConfigCheckBox_CheckedChanged;
            // 
            // cleanServerIpAndPortConfigCheckBox
            // 
            cleanServerIpAndPortConfigCheckBox.AutoSize = true;
            cleanServerIpAndPortConfigCheckBox.Location = new Point(9, 96);
            cleanServerIpAndPortConfigCheckBox.Name = "cleanServerIpAndPortConfigCheckBox";
            cleanServerIpAndPortConfigCheckBox.Size = new Size(194, 19);
            cleanServerIpAndPortConfigCheckBox.TabIndex = 2;
            cleanServerIpAndPortConfigCheckBox.Text = "Server IP and Port configuration";
            cleanServerIpAndPortConfigCheckBox.UseVisualStyleBackColor = true;
            cleanServerIpAndPortConfigCheckBox.CheckedChanged += cleanServerIpAndPortConfigCheckBox_CheckedChanged;
            // 
            // cleanupClientLabel
            // 
            cleanupClientLabel.AutoSize = true;
            cleanupClientLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cleanupClientLabel.Location = new Point(8, 15);
            cleanupClientLabel.Name = "cleanupClientLabel";
            cleanupClientLabel.Size = new Size(172, 15);
            cleanupClientLabel.TabIndex = 3;
            cleanupClientLabel.Text = "I want to Delete permanently";
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(36, 123);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(93, 23);
            cancelButton.TabIndex = 4;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // confirmCleanupButton
            // 
            confirmCleanupButton.Enabled = false;
            confirmCleanupButton.Location = new Point(172, 123);
            confirmCleanupButton.Name = "confirmCleanupButton";
            confirmCleanupButton.Size = new Size(93, 23);
            confirmCleanupButton.TabIndex = 5;
            confirmCleanupButton.Text = "Do Cleanup";
            confirmCleanupButton.UseVisualStyleBackColor = true;
            confirmCleanupButton.Click += confirmCleanupButton_Click;
            // 
            // ClientCleanupForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(291, 153);
            Controls.Add(confirmCleanupButton);
            Controls.Add(cancelButton);
            Controls.Add(cleanupClientLabel);
            Controls.Add(cleanServerIpAndPortConfigCheckBox);
            Controls.Add(cleanUdpListenerConfigCheckBox);
            Controls.Add(cleanTemporarelySavedQsoCheckBox);
            Name = "ClientCleanupForm";
            Text = "Cleanup Client";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox cleanTemporarelySavedQsoCheckBox;
        private CheckBox cleanUdpListenerConfigCheckBox;
        private CheckBox cleanServerIpAndPortConfigCheckBox;
        private Label cleanupClientLabel;
        private Button cancelButton;
        private Button confirmCleanupButton;
    }
}