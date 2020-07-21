namespace Slow471Repro
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.firstTabPage = new System.Windows.Forms.TabPage();
            this.numericGroupBoxes = new System.Windows.Forms.NumericUpDown();
            this.groupBoxesLabel = new System.Windows.Forms.Label();
            this.contentLabel = new System.Windows.Forms.Label();
            this.numericContent = new System.Windows.Forms.NumericUpDown();
            this.createButton = new System.Windows.Forms.Button();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.showPerfResultsButton = new System.Windows.Forms.Button();
            this.chkSuspendDrawing = new System.Windows.Forms.CheckBox();
            this.tabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericGroupBoxes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericContent)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.firstTabPage);
            this.tabControl.Location = new System.Drawing.Point(208, 14);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(890, 738);
            this.tabControl.TabIndex = 2;
            // 
            // firstTabPage
            // 
            this.firstTabPage.AutoScroll = true;
            this.firstTabPage.Location = new System.Drawing.Point(4, 29);
            this.firstTabPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.firstTabPage.Name = "firstTabPage";
            this.firstTabPage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.firstTabPage.Size = new System.Drawing.Size(882, 705);
            this.firstTabPage.TabIndex = 0;
            this.firstTabPage.Text = "Output";
            this.firstTabPage.UseVisualStyleBackColor = true;
            // 
            // numericGroupBoxes
            // 
            this.numericGroupBoxes.Location = new System.Drawing.Point(18, 48);
            this.numericGroupBoxes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericGroupBoxes.Name = "numericGroupBoxes";
            this.numericGroupBoxes.Size = new System.Drawing.Size(180, 26);
            this.numericGroupBoxes.TabIndex = 3;
            this.numericGroupBoxes.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // groupBoxesLabel
            // 
            this.groupBoxesLabel.AutoSize = true;
            this.groupBoxesLabel.Location = new System.Drawing.Point(18, 18);
            this.groupBoxesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.groupBoxesLabel.Name = "groupBoxesLabel";
            this.groupBoxesLabel.Size = new System.Drawing.Size(96, 20);
            this.groupBoxesLabel.TabIndex = 4;
            this.groupBoxesLabel.Text = "Groupboxes";
            // 
            // contentLabel
            // 
            this.contentLabel.AutoSize = true;
            this.contentLabel.Location = new System.Drawing.Point(18, 83);
            this.contentLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.contentLabel.Name = "contentLabel";
            this.contentLabel.Size = new System.Drawing.Size(66, 20);
            this.contentLabel.TabIndex = 5;
            this.contentLabel.Text = "Content";
            // 
            // numericContent
            // 
            this.numericContent.Location = new System.Drawing.Point(18, 108);
            this.numericContent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericContent.Name = "numericContent";
            this.numericContent.Size = new System.Drawing.Size(180, 26);
            this.numericContent.TabIndex = 6;
            this.numericContent.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(18, 186);
            this.createButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(156, 35);
            this.createButton.TabIndex = 8;
            this.createButton.Text = "Create";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(14, 162);
            this.descriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(157, 20);
            this.descriptionLabel.TabIndex = 7;
            this.descriptionLabel.Text = "Click Create and wait";
            // 
            // showPerfResultsButton
            // 
            this.showPerfResultsButton.Location = new System.Drawing.Point(17, 253);
            this.showPerfResultsButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.showPerfResultsButton.Name = "showPerfResultsButton";
            this.showPerfResultsButton.Size = new System.Drawing.Size(156, 35);
            this.showPerfResultsButton.TabIndex = 9;
            this.showPerfResultsButton.Text = "Show PerfResults";
            this.showPerfResultsButton.UseVisualStyleBackColor = true;
            this.showPerfResultsButton.Click += new System.EventHandler(this.ShowPerfResultsButton_Click);
            // 
            // chkSuspendDrawing
            // 
            this.chkSuspendDrawing.AutoSize = true;
            this.chkSuspendDrawing.Location = new System.Drawing.Point(18, 311);
            this.chkSuspendDrawing.Name = "chkSuspendDrawing";
            this.chkSuspendDrawing.Size = new System.Drawing.Size(161, 24);
            this.chkSuspendDrawing.TabIndex = 10;
            this.chkSuspendDrawing.Text = "Suspend Drawing";
            this.chkSuspendDrawing.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1114, 770);
            this.Controls.Add(this.chkSuspendDrawing);
            this.Controls.Add(this.showPerfResultsButton);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.numericContent);
            this.Controls.Add(this.contentLabel);
            this.Controls.Add(this.groupBoxesLabel);
            this.Controls.Add(this.numericGroupBoxes);
            this.Controls.Add(this.tabControl);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Slow .NET 4.7.1 Framework Repro - .NET Version: ";
            this.tabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericGroupBoxes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericContent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage firstTabPage;
        private System.Windows.Forms.NumericUpDown numericGroupBoxes;
        private System.Windows.Forms.Label groupBoxesLabel;
        private System.Windows.Forms.Label contentLabel;
        private System.Windows.Forms.NumericUpDown numericContent;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Button showPerfResultsButton;
        private System.Windows.Forms.CheckBox chkSuspendDrawing;
    }
}

