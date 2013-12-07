namespace Lorei
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
            this.EnableButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lastCommandLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // EnableButton
            // 
            this.EnableButton.Location = new System.Drawing.Point(205, 12);
            this.EnableButton.Name = "EnableButton";
            this.EnableButton.Size = new System.Drawing.Size(268, 55);
            this.EnableButton.TabIndex = 0;
            this.EnableButton.Text = "Enable/Disable";
            this.EnableButton.UseVisualStyleBackColor = true;
            this.EnableButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 33F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 52);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enabled";
            // 
            // lastCommandLabel
            // 
            this.lastCommandLabel.AutoSize = true;
            this.lastCommandLabel.Location = new System.Drawing.Point(13, 73);
            this.lastCommandLabel.Name = "lastCommandLabel";
            this.lastCommandLabel.Size = new System.Drawing.Size(0, 13);
            this.lastCommandLabel.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 98);
            this.Controls.Add(this.lastCommandLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EnableButton);
            this.Name = "Form1";
            this.Text = "Lanaguage Operated Request and Execution Interface";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button EnableButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lastCommandLabel;
    }
}

