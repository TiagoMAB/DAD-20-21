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
            this.crashBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.crashSelector = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.freezeSelector = new System.Windows.Forms.ComboBox();
            this.freezeBtn = new System.Windows.Forms.Button();
            this.unfreezeSelector = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.unfreezeBtn = new System.Windows.Forms.Button();
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
            // crashBtn
            // 
            this.crashBtn.Location = new System.Drawing.Point(365, 78);
            this.crashBtn.Name = "crashBtn";
            this.crashBtn.Size = new System.Drawing.Size(75, 23);
            this.crashBtn.TabIndex = 7;
            this.crashBtn.Text = "Execute";
            this.crashBtn.UseVisualStyleBackColor = true;
            this.crashBtn.Click += new System.EventHandler(this.CrashBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Crash:";
            // 
            // crashSelector
            // 
            this.crashSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.crashSelector.FormattingEnabled = true;
            this.crashSelector.Location = new System.Drawing.Point(91, 78);
            this.crashSelector.Name = "crashSelector";
            this.crashSelector.Size = new System.Drawing.Size(268, 23);
            this.crashSelector.Sorted = true;
            this.crashSelector.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Freeze:";
            // 
            // freezeSelector
            // 
            this.freezeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.freezeSelector.FormattingEnabled = true;
            this.freezeSelector.Location = new System.Drawing.Point(91, 107);
            this.freezeSelector.Name = "freezeSelector";
            this.freezeSelector.Size = new System.Drawing.Size(268, 23);
            this.freezeSelector.Sorted = true;
            this.freezeSelector.TabIndex = 11;
            // 
            // freezeBtn
            // 
            this.freezeBtn.Location = new System.Drawing.Point(365, 107);
            this.freezeBtn.Name = "freezeBtn";
            this.freezeBtn.Size = new System.Drawing.Size(75, 23);
            this.freezeBtn.TabIndex = 12;
            this.freezeBtn.Text = "Execute";
            this.freezeBtn.UseVisualStyleBackColor = true;
            this.freezeBtn.Click += new System.EventHandler(this.FreezeBtn_Click);
            // 
            // unfreezeSelector
            // 
            this.unfreezeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.unfreezeSelector.FormattingEnabled = true;
            this.unfreezeSelector.Location = new System.Drawing.Point(91, 136);
            this.unfreezeSelector.Name = "unfreezeSelector";
            this.unfreezeSelector.Size = new System.Drawing.Size(268, 23);
            this.unfreezeSelector.Sorted = true;
            this.unfreezeSelector.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "Unfreeze:";
            // 
            // unfreezeBtn
            // 
            this.unfreezeBtn.Location = new System.Drawing.Point(365, 136);
            this.unfreezeBtn.Name = "unfreezeBtn";
            this.unfreezeBtn.Size = new System.Drawing.Size(75, 23);
            this.unfreezeBtn.TabIndex = 15;
            this.unfreezeBtn.Text = "Execute";
            this.unfreezeBtn.UseVisualStyleBackColor = true;
            this.unfreezeBtn.Click += new System.EventHandler(this.UnfreezeBtn_Click);
            // 
            // PuppetMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 560);
            this.Controls.Add(this.unfreezeBtn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.unfreezeSelector);
            this.Controls.Add(this.freezeBtn);
            this.Controls.Add(this.freezeSelector);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.crashSelector);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.crashBtn);
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
        private System.Windows.Forms.Button crashBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox crashSelector;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox freezeSelector;
        private System.Windows.Forms.Button freezeBtn;
        private System.Windows.Forms.ComboBox unfreezeSelector;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button unfreezeBtn;
    }
}

