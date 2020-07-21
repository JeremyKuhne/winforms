namespace CreateControlsCSharp
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnStartPerformanceTest = new System.Windows.Forms.Button();
            this.nupRuns = new System.Windows.Forms.NumericUpDown();
            this.pBarRunsDone = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblAverageRunTime = new System.Windows.Forms.Label();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.runColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkDestroyPanelHandle = new System.Windows.Forms.CheckBox();
            this.chkSuspenLayout = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nupRuns)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStartPerformanceTest
            // 
            this.btnStartPerformanceTest.Location = new System.Drawing.Point(408, 234);
            this.btnStartPerformanceTest.Name = "btnStartPerformanceTest";
            this.btnStartPerformanceTest.Size = new System.Drawing.Size(215, 64);
            this.btnStartPerformanceTest.TabIndex = 0;
            this.btnStartPerformanceTest.Text = "Start Performance Test";
            this.btnStartPerformanceTest.UseVisualStyleBackColor = true;
            this.btnStartPerformanceTest.Click += new System.EventHandler(this.BtnStartPerformanceTest_Click);
            // 
            // nupRuns
            // 
            this.nupRuns.Location = new System.Drawing.Point(480, 30);
            this.nupRuns.Name = "nupRuns";
            this.nupRuns.Size = new System.Drawing.Size(115, 29);
            this.nupRuns.TabIndex = 1;
            this.nupRuns.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // pBarRunsDone
            // 
            this.pBarRunsDone.Location = new System.Drawing.Point(274, 164);
            this.pBarRunsDone.Name = "pBarRunsDone";
            this.pBarRunsDone.Size = new System.Drawing.Size(472, 31);
            this.pBarRunsDone.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(410, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 21);
            this.label1.TabIndex = 3;
            this.label1.Text = "Runs:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.lblAverageRunTime);
            this.panel1.Controls.Add(this.dgvResults);
            this.panel1.Location = new System.Drawing.Point(222, 330);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(524, 623);
            this.panel1.TabIndex = 4;
            // 
            // lblAverageRunTime
            // 
            this.lblAverageRunTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAverageRunTime.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblAverageRunTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAverageRunTime.Location = new System.Drawing.Point(0, 591);
            this.lblAverageRunTime.Name = "lblAverageRunTime";
            this.lblAverageRunTime.Size = new System.Drawing.Size(524, 32);
            this.lblAverageRunTime.TabIndex = 6;
            this.lblAverageRunTime.Text = "Average run time:";
            // 
            // dgvResults
            // 
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.runColumn,
            this.resultColumn});
            this.dgvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResults.Location = new System.Drawing.Point(0, 0);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.RowHeadersWidth = 62;
            this.dgvResults.RowTemplate.Height = 28;
            this.dgvResults.Size = new System.Drawing.Size(524, 623);
            this.dgvResults.TabIndex = 5;
            // 
            // runColumn
            // 
            this.runColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(3);
            this.runColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.runColumn.FillWeight = 20F;
            this.runColumn.HeaderText = "Run";
            this.runColumn.MinimumWidth = 8;
            this.runColumn.Name = "runColumn";
            // 
            // resultColumn
            // 
            this.resultColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(3);
            this.resultColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.resultColumn.FillWeight = 80F;
            this.resultColumn.HeaderText = "Result (ms)";
            this.resultColumn.MinimumWidth = 8;
            this.resultColumn.Name = "resultColumn";
            // 
            // chkDestroyPanelHandle
            // 
            this.chkDestroyPanelHandle.AutoSize = true;
            this.chkDestroyPanelHandle.Checked = true;
            this.chkDestroyPanelHandle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDestroyPanelHandle.Location = new System.Drawing.Point(414, 75);
            this.chkDestroyPanelHandle.Name = "chkDestroyPanelHandle";
            this.chkDestroyPanelHandle.Size = new System.Drawing.Size(184, 25);
            this.chkDestroyPanelHandle.TabIndex = 5;
            this.chkDestroyPanelHandle.Text = "Destroy Panel Handle";
            this.chkDestroyPanelHandle.UseVisualStyleBackColor = true;
            // 
            // chkSuspenLayout
            // 
            this.chkSuspenLayout.AutoSize = true;
            this.chkSuspenLayout.Checked = true;
            this.chkSuspenLayout.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSuspenLayout.Location = new System.Drawing.Point(414, 106);
            this.chkSuspenLayout.Name = "chkSuspenLayout";
            this.chkSuspenLayout.Size = new System.Drawing.Size(239, 38);
            this.chkSuspenLayout.TabIndex = 6;
            this.chkSuspenLayout.Text = "Suspend Drawing";
            this.chkSuspenLayout.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 965);
            this.Controls.Add(this.chkSuspenLayout);
            this.Controls.Add(this.chkDestroyPanelHandle);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pBarRunsDone);
            this.Controls.Add(this.nupRuns);
            this.Controls.Add(this.btnStartPerformanceTest);
            this.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nupRuns)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartPerformanceTest;
        private System.Windows.Forms.NumericUpDown nupRuns;
        private System.Windows.Forms.ProgressBar pBarRunsDone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblAverageRunTime;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.DataGridViewTextBoxColumn runColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultColumn;
        private System.Windows.Forms.CheckBox chkDestroyPanelHandle;
        private System.Windows.Forms.CheckBox chkSuspenLayout;
    }
}

