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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientCleanupForm));
            cleanTemporarelySavedQsoCheckBox = new CheckBox();
            cleanUdpListenerConfigCheckBox = new CheckBox();
            cleanServerIpAndPortConfigCheckBox = new CheckBox();
            cleanupClientLabel = new Label();
            cancelButton = new Button();
            confirmCleanupButton = new Button();
            pictureBox1 = new PictureBox();
            label3 = new Label();
            label1 = new Label();
            understandCheckBox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // cleanTemporarelySavedQsoCheckBox
            // 
            cleanTemporarelySavedQsoCheckBox.AutoSize = true;
            cleanTemporarelySavedQsoCheckBox.Location = new Point(247, 35);
            cleanTemporarelySavedQsoCheckBox.Name = "cleanTemporarelySavedQsoCheckBox";
            cleanTemporarelySavedQsoCheckBox.Size = new Size(126, 19);
            cleanTemporarelySavedQsoCheckBox.TabIndex = 0;
            cleanTemporarelySavedQsoCheckBox.Text = "Saved locally QSOs";
            cleanTemporarelySavedQsoCheckBox.UseVisualStyleBackColor = true;
            cleanTemporarelySavedQsoCheckBox.CheckedChanged += cleanTemporarelySavedQsoCheckBox_CheckedChanged;
            // 
            // cleanUdpListenerConfigCheckBox
            // 
            cleanUdpListenerConfigCheckBox.AutoSize = true;
            cleanUdpListenerConfigCheckBox.Location = new Point(247, 60);
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
            cleanServerIpAndPortConfigCheckBox.Location = new Point(247, 85);
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
            cleanupClientLabel.Location = new Point(238, 9);
            cleanupClientLabel.Name = "cleanupClientLabel";
            cleanupClientLabel.Size = new Size(238, 15);
            cleanupClientLabel.TabIndex = 3;
            cleanupClientLabel.Text = "I want to Delete permanently from Client";
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(390, 174);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(93, 23);
            cancelButton.TabIndex = 4;
            cancelButton.Text = "No";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // confirmCleanupButton
            // 
            confirmCleanupButton.Enabled = false;
            confirmCleanupButton.Location = new Point(247, 174);
            confirmCleanupButton.Name = "confirmCleanupButton";
            confirmCleanupButton.Size = new Size(93, 23);
            confirmCleanupButton.TabIndex = 5;
            confirmCleanupButton.Text = "Yes";
            confirmCleanupButton.UseVisualStyleBackColor = true;
            confirmCleanupButton.Click += confirmCleanupButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.None;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(3, 7);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(209, 187);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 13;
            pictureBox1.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(246, 127);
            label3.Name = "label3";
            label3.Size = new Size(259, 15);
            label3.TabIndex = 16;
            label3.Text = "Do you still want to remove data from Client?";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(246, 107);
            label1.Name = "label1";
            label1.Size = new Size(220, 15);
            label1.TabIndex = 15;
            label1.Text = "Attention: this action is not reversible!";
            // 
            // understandCheckBox
            // 
            understandCheckBox.AutoSize = true;
            understandCheckBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            understandCheckBox.Location = new Point(247, 149);
            understandCheckBox.Name = "understandCheckBox";
            understandCheckBox.Size = new Size(139, 19);
            understandCheckBox.TabIndex = 14;
            understandCheckBox.Text = "I understand all risks";
            understandCheckBox.UseVisualStyleBackColor = true;
            understandCheckBox.CheckedChanged += understandCheckBox_CheckedChanged;
            // 
            // ClientCleanupForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(517, 212);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(understandCheckBox);
            Controls.Add(pictureBox1);
            Controls.Add(confirmCleanupButton);
            Controls.Add(cancelButton);
            Controls.Add(cleanupClientLabel);
            Controls.Add(cleanServerIpAndPortConfigCheckBox);
            Controls.Add(cleanUdpListenerConfigCheckBox);
            Controls.Add(cleanTemporarelySavedQsoCheckBox);
            Name = "ClientCleanupForm";
            Text = "Cleanup Client";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
        private PictureBox pictureBox1;
        private Label label3;
        private Label label1;
        private CheckBox understandCheckBox;
    }
}