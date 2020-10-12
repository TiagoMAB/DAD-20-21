namespace PuppetMaster
{
    partial class PuppetMaster
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.PMFile = new System.Windows.Forms.TextBox();
            this.btnLoadPMFile = new System.Windows.Forms.Button();
            this.SelectPMFile = new System.Windows.Forms.OpenFileDialog();
            this.btnSelectPMFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.Logs = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Script file: ";
            // 
            // PMFile
            // 
            this.PMFile.Location = new System.Drawing.Point(91, 32);
            this.PMFile.Name = "PMFile";
            this.PMFile.Size = new System.Drawing.Size(268, 23);
            this.PMFile.TabIndex = 1;
            // 
            // btnLoadPMFile
            // 
            this.btnLoadPMFile.Location = new System.Drawing.Point(446, 32);
            this.btnLoadPMFile.Name = "btnLoadPMFile";
            this.btnLoadPMFile.Size = new System.Drawing.Size(77, 23);
            this.btnLoadPMFile.TabIndex = 2;
            this.btnLoadPMFile.Text = "Load";
            this.btnLoadPMFile.UseVisualStyleBackColor = true;
            this.btnLoadPMFile.Click += new System.EventHandler(this.LoadPMFileClick);
            // 
            // SelectPMFile
            // 
            this.SelectPMFile.FileOk += new System.ComponentModel.CancelEventHandler(this.SelectedPMFile);
            // 
            // btnSelectPMFile
            // 
            this.btnSelectPMFile.Location = new System.Drawing.Point(365, 32);
            this.btnSelectPMFile.Name = "btnSelectPMFile";
            this.btnSelectPMFile.Size = new System.Drawing.Size(75, 23);
            this.btnSelectPMFile.TabIndex = 3;
            this.btnSelectPMFile.Text = "Select";
            this.btnSelectPMFile.UseVisualStyleBackColor = true;
            this.btnSelectPMFile.Click += new System.EventHandler(this.SelectPMFileClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(13, 300);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(30, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Log:";
            // 
            // Logs
            // 
            this.Logs.FormattingEnabled = true;
            this.Logs.ItemHeight = 15;
            this.Logs.Location = new System.Drawing.Point(12, 318);
            this.Logs.Name = "Logs";
            this.Logs.Size = new System.Drawing.Size(571, 229);
            this.Logs.TabIndex = 6;
            // 
            // PuppetMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 560);
            this.Controls.Add(this.Logs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSelectPMFile);
            this.Controls.Add(this.btnLoadPMFile);
            this.Controls.Add(this.PMFile);
            this.Controls.Add(this.label1);
            this.Name = "PuppetMaster";
            this.Text = "PuppetMaster";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PMFile;
        private System.Windows.Forms.Button btnLoadPMFile;
        private System.Windows.Forms.OpenFileDialog SelectPMFile;
        private System.Windows.Forms.Button btnSelectPMFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox Logs;
    }
}

