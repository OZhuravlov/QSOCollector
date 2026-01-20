namespace QSOCollector.Forms
{
    partial class ServerCleanupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerCleanupForm));
            cleanupClientLabel = new Label();
            understandCheckBox = new CheckBox();
            confirmCleanupButton = new Button();
            cancelButton = new Button();
            label1 = new Label();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // cleanupClientLabel
            // 
            cleanupClientLabel.AutoSize = true;
            cleanupClientLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cleanupClientLabel.Location = new Point(222, 13);
            cleanupClientLabel.Name = "cleanupClientLabel";
            cleanupClientLabel.Size = new Size(287, 15);
            cleanupClientLabel.TabIndex = 4;
            cleanupClientLabel.Text = "You are about to remove all the QSOs from Server";
            // 
            // understandCheckBox
            // 
            understandCheckBox.AutoSize = true;
            understandCheckBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            understandCheckBox.Location = new Point(222, 135);
            understandCheckBox.Name = "understandCheckBox";
            understandCheckBox.Size = new Size(139, 19);
            understandCheckBox.TabIndex = 5;
            understandCheckBox.Text = "I understand all risks";
            understandCheckBox.UseVisualStyleBackColor = true;
            understandCheckBox.CheckedChanged += understandCheckBox_CheckedChanged;
            // 
            // confirmCleanupButton
            // 
            confirmCleanupButton.Enabled = false;
            confirmCleanupButton.Location = new Point(231, 171);
            confirmCleanupButton.Name = "confirmCleanupButton";
            confirmCleanupButton.Size = new Size(93, 23);
            confirmCleanupButton.TabIndex = 9;
            confirmCleanupButton.Text = "Yes";
            confirmCleanupButton.UseVisualStyleBackColor = true;
            confirmCleanupButton.Click += confirmCleanupButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(368, 171);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(93, 23);
            cancelButton.TabIndex = 8;
            cancelButton.Text = "No";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(222, 56);
            label1.Name = "label1";
            label1.Size = new Size(220, 15);
            label1.TabIndex = 10;
            label1.Text = "Attention: this action is not reversible!";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(222, 34);
            label2.Name = "label2";
            label2.Size = new Size(366, 15);
            label2.TabIndex = 11;
            label2.Text = "Make sure all QSO data has been alraedy exported into ADIF files";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.None;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(10, 15);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(187, 189);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 12;
            pictureBox1.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(222, 97);
            label3.Name = "label3";
            label3.Size = new Size(265, 15);
            label3.TabIndex = 13;
            label3.Text = "Do you still want to remove data from Server?";
            // 
            // ServerCleanupForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(632, 216);
            Controls.Add(label3);
            Controls.Add(pictureBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(confirmCleanupButton);
            Controls.Add(cancelButton);
            Controls.Add(understandCheckBox);
            Controls.Add(cleanupClientLabel);
            Name = "ServerCleanupForm";
            Text = "Remove QSO from Server";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label cleanupClientLabel;
        private CheckBox understandCheckBox;
        private Button confirmCleanupButton;
        private Button cancelButton;
        private Label label1;
        private Label label2;
        private PictureBox pictureBox1;
        private Label label3;
    }
}