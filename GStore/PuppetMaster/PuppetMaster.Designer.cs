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
            this.label6 = new System.Windows.Forms.Label();
            this.clientName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.clientBtn = new System.Windows.Forms.Button();
            this.clientScript = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.clientURL = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.serverBtn = new System.Windows.Forms.Button();
            this.serverURL = new System.Windows.Forms.TextBox();
            this.minDelay = new System.Windows.Forms.NumericUpDown();
            this.maxDelay = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.serverName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.statusBtn = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.partitionBtn = new System.Windows.Forms.Button();
            this.partitionName = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDelay)).BeginInit();
            this.groupBox3.SuspendLayout();
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
            this.btnLoadPMFile.Location = new System.Drawing.Point(447, 31);
            this.btnLoadPMFile.Name = "btnLoadPMFile";
            this.btnLoadPMFile.Size = new System.Drawing.Size(75, 23);
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
            this.btnSelectPMFile.Location = new System.Drawing.Point(365, 31);
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
            this.label2.Location = new System.Drawing.Point(12, 482);
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
            this.Logs.Location = new System.Drawing.Point(12, 500);
            this.Logs.Name = "Logs";
            this.Logs.Size = new System.Drawing.Size(689, 229);
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
            this.crashSelector.Location = new System.Drawing.Point(91, 79);
            this.crashSelector.Name = "crashSelector";
            this.crashSelector.Size = new System.Drawing.Size(268, 23);
            this.crashSelector.Sorted = true;
            this.crashSelector.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Freeze:";
            // 
            // freezeSelector
            // 
            this.freezeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.freezeSelector.FormattingEnabled = true;
            this.freezeSelector.Location = new System.Drawing.Point(91, 108);
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
            this.unfreezeSelector.Location = new System.Drawing.Point(91, 137);
            this.unfreezeSelector.Name = "unfreezeSelector";
            this.unfreezeSelector.Size = new System.Drawing.Size(268, 23);
            this.unfreezeSelector.Sorted = true;
            this.unfreezeSelector.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 140);
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 15);
            this.label6.TabIndex = 16;
            this.label6.Text = "Name:";
            // 
            // clientName
            // 
            this.clientName.Location = new System.Drawing.Point(54, 22);
            this.clientName.Name = "clientName";
            this.clientName.Size = new System.Drawing.Size(268, 23);
            this.clientName.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.clientBtn);
            this.groupBox1.Controls.Add(this.clientScript);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.clientURL);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.clientName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(23, 177);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 141);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Client";
            // 
            // clientBtn
            // 
            this.clientBtn.Location = new System.Drawing.Point(117, 109);
            this.clientBtn.Name = "clientBtn";
            this.clientBtn.Size = new System.Drawing.Size(75, 23);
            this.clientBtn.TabIndex = 22;
            this.clientBtn.Text = "Create";
            this.clientBtn.UseVisualStyleBackColor = true;
            this.clientBtn.Click += new System.EventHandler(this.ClientBtn_Click);
            // 
            // clientScript
            // 
            this.clientScript.Location = new System.Drawing.Point(54, 80);
            this.clientScript.Name = "clientScript";
            this.clientScript.Size = new System.Drawing.Size(268, 23);
            this.clientScript.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 15);
            this.label8.TabIndex = 20;
            this.label8.Text = "Script:";
            // 
            // clientURL
            // 
            this.clientURL.Location = new System.Drawing.Point(54, 51);
            this.clientURL.Name = "clientURL";
            this.clientURL.Size = new System.Drawing.Size(268, 23);
            this.clientURL.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 15);
            this.label7.TabIndex = 18;
            this.label7.Text = "URL:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.serverBtn);
            this.groupBox2.Controls.Add(this.serverURL);
            this.groupBox2.Controls.Add(this.minDelay);
            this.groupBox2.Controls.Add(this.maxDelay);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.serverName);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Location = new System.Drawing.Point(365, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(336, 141);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server";
            // 
            // serverBtn
            // 
            this.serverBtn.Location = new System.Drawing.Point(130, 109);
            this.serverBtn.Name = "serverBtn";
            this.serverBtn.Size = new System.Drawing.Size(75, 23);
            this.serverBtn.TabIndex = 7;
            this.serverBtn.Text = "Create";
            this.serverBtn.UseVisualStyleBackColor = true;
            this.serverBtn.Click += new System.EventHandler(this.ServerBtn_Click);
            // 
            // serverURL
            // 
            this.serverURL.Location = new System.Drawing.Point(54, 51);
            this.serverURL.Name = "serverURL";
            this.serverURL.Size = new System.Drawing.Size(268, 23);
            this.serverURL.TabIndex = 6;
            // 
            // minDelay
            // 
            this.minDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.minDelay.Location = new System.Drawing.Point(75, 80);
            this.minDelay.Maximum = new decimal(new int[] {
            1215752191,
            23,
            0,
            0});
            this.minDelay.Name = "minDelay";
            this.minDelay.Size = new System.Drawing.Size(82, 23);
            this.minDelay.TabIndex = 5;
            // 
            // maxDelay
            // 
            this.maxDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.maxDelay.Location = new System.Drawing.Point(240, 80);
            this.maxDelay.Maximum = new decimal(new int[] {
            1215752191,
            23,
            0,
            0});
            this.maxDelay.Name = "maxDelay";
            this.maxDelay.Size = new System.Drawing.Size(82, 23);
            this.maxDelay.TabIndex = 4;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(170, 83);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 15);
            this.label12.TabIndex = 3;
            this.label12.Text = "Max Delay:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 83);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 15);
            this.label11.TabIndex = 3;
            this.label11.Text = "Min Delay:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 54);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 15);
            this.label10.TabIndex = 2;
            this.label10.Text = "URL:";
            // 
            // serverName
            // 
            this.serverName.Location = new System.Drawing.Point(54, 22);
            this.serverName.Name = "serverName";
            this.serverName.Size = new System.Drawing.Size(268, 23);
            this.serverName.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 15);
            this.label9.TabIndex = 0;
            this.label9.Text = "Name:";
            // 
            // statusBtn
            // 
            this.statusBtn.Location = new System.Drawing.Point(535, 107);
            this.statusBtn.Name = "statusBtn";
            this.statusBtn.Size = new System.Drawing.Size(75, 23);
            this.statusBtn.TabIndex = 20;
            this.statusBtn.Text = "Status";
            this.statusBtn.UseVisualStyleBackColor = true;
            this.statusBtn.Click += new System.EventHandler(this.StatusBtn_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkedListBox1);
            this.groupBox3.Controls.Add(this.partitionBtn);
            this.groupBox3.Controls.Add(this.partitionName);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Location = new System.Drawing.Point(23, 324);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(678, 133);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Partition";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(396, 22);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(268, 94);
            this.checkedListBox1.TabIndex = 3;
            // 
            // partitionBtn
            // 
            this.partitionBtn.Location = new System.Drawing.Point(149, 79);
            this.partitionBtn.Name = "partitionBtn";
            this.partitionBtn.Size = new System.Drawing.Size(75, 23);
            this.partitionBtn.TabIndex = 2;
            this.partitionBtn.Text = "Create";
            this.partitionBtn.UseVisualStyleBackColor = true;
            // 
            // partitionName
            // 
            this.partitionName.Location = new System.Drawing.Point(54, 30);
            this.partitionName.Name = "partitionName";
            this.partitionName.Size = new System.Drawing.Size(268, 23);
            this.partitionName.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 33);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(42, 15);
            this.label13.TabIndex = 0;
            this.label13.Text = "Name:";
            // 
            // PuppetMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 741);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.statusBtn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PM_Closing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDelay)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox clientName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button clientBtn;
        private System.Windows.Forms.TextBox clientScript;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox clientURL;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox serverName;
        private System.Windows.Forms.Button serverBtn;
        private System.Windows.Forms.TextBox serverURL;
        private System.Windows.Forms.NumericUpDown minDelay;
        private System.Windows.Forms.NumericUpDown maxDelay;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button statusBtn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox partitionName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button partitionBtn;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
    }
}

