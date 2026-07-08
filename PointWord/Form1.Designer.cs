namespace PointWord
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            splitMain = new SplitContainer();
            panelLeft = new Panel();
            lblStatus = new Label();
            tblSettings = new TableLayoutPanel();
            txtTargetText = new TextBox();
            txtFillText = new TextBox();
            numGridCount = new NumericUpDown();
            numCellSize = new NumericUpDown();
            numFillFontSize = new NumericUpDown();
            cmbFillFontFamily = new ComboBox();
            numTargetFontSize = new NumericUpDown();
            numTargetScale = new NumericUpDown();
            cmbTargetFontFamily = new ComboBox();
            numMaskThreshold = new NumericUpDown();
            numPaddingRatio = new NumericUpDown();
            panelMode = new FlowLayoutPanel();
            rbWholeText = new RadioButton();
            rbPerChar = new RadioButton();
            numBlockColumns = new NumericUpDown();
            numBlockSpacing = new NumericUpDown();
            panelDebug = new FlowLayoutPanel();
            chkDrawGridDebug = new CheckBox();
            chkShowMaskDebug = new CheckBox();
            panelColors = new FlowLayoutPanel();
            btnBackgroundColor = new Button();
            btnFillTextColor = new Button();
            panelActions = new FlowLayoutPanel();
            btnCopyPreview = new Button();
            btnExportPng = new Button();
            panelRight = new Panel();
            picturePreview = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            panelLeft.SuspendLayout();
            tblSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numGridCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCellSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFillFontSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTargetFontSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTargetScale).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaskThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPaddingRatio).BeginInit();
            panelMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numBlockColumns).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBlockSpacing).BeginInit();
            panelDebug.SuspendLayout();
            panelColors.SuspendLayout();
            panelActions.SuspendLayout();
            panelRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picturePreview).BeginInit();
            SuspendLayout();
            // 
            // splitMain
            // 
            splitMain.Dock = DockStyle.Fill;
            splitMain.FixedPanel = FixedPanel.Panel1;
            splitMain.Location = new Point(0, 0);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(panelLeft);
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(panelRight);
            splitMain.Panel1MinSize = 340;
            splitMain.Size = new Size(1360, 860);
            splitMain.SplitterDistance = 400;
            splitMain.TabIndex = 0;
            // 
            // panelLeft
            // 
            panelLeft.AutoScroll = true;
            panelLeft.Controls.Add(tblSettings);
            panelLeft.Controls.Add(lblStatus);
            panelLeft.Dock = DockStyle.Fill;
            panelLeft.Location = new Point(0, 0);
            panelLeft.Name = "panelLeft";
            panelLeft.Padding = new Padding(12);
            panelLeft.Size = new Size(400, 860);
            panelLeft.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.Location = new Point(12, 824);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(376, 24);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "Ready";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tblSettings
            // 
            tblSettings.AutoSize = true;
            tblSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tblSettings.ColumnCount = 2;
            tblSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            tblSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblSettings.Controls.Add(CreateLabel("TargetText"), 0, 0);
            tblSettings.Controls.Add(txtTargetText, 1, 0);
            tblSettings.Controls.Add(CreateLabel("FillText"), 0, 1);
            tblSettings.Controls.Add(txtFillText, 1, 1);
            tblSettings.Controls.Add(CreateLabel("GridCount"), 0, 2);
            tblSettings.Controls.Add(numGridCount, 1, 2);
            tblSettings.Controls.Add(CreateLabel("CellSize(px)"), 0, 3);
            tblSettings.Controls.Add(numCellSize, 1, 3);
            tblSettings.Controls.Add(CreateLabel("FillFontSize"), 0, 4);
            tblSettings.Controls.Add(numFillFontSize, 1, 4);
            tblSettings.Controls.Add(CreateLabel("FillFontFamily"), 0, 5);
            tblSettings.Controls.Add(cmbFillFontFamily, 1, 5);
            tblSettings.Controls.Add(CreateLabel("TargetFontSize"), 0, 6);
            tblSettings.Controls.Add(numTargetFontSize, 1, 6);
            tblSettings.Controls.Add(CreateLabel("TargetScale"), 0, 7);
            tblSettings.Controls.Add(numTargetScale, 1, 7);
            tblSettings.Controls.Add(CreateLabel("TargetFontFamily"), 0, 8);
            tblSettings.Controls.Add(cmbTargetFontFamily, 1, 8);
            tblSettings.Controls.Add(CreateLabel("MaskThreshold"), 0, 9);
            tblSettings.Controls.Add(numMaskThreshold, 1, 9);
            tblSettings.Controls.Add(CreateLabel("PaddingRatio"), 0, 10);
            tblSettings.Controls.Add(numPaddingRatio, 1, 10);
            tblSettings.Controls.Add(CreateLabel("Mode"), 0, 11);
            tblSettings.Controls.Add(panelMode, 1, 11);
            tblSettings.Controls.Add(CreateLabel("Block Columns"), 0, 12);
            tblSettings.Controls.Add(numBlockColumns, 1, 12);
            tblSettings.Controls.Add(CreateLabel("Block Spacing"), 0, 13);
            tblSettings.Controls.Add(numBlockSpacing, 1, 13);
            tblSettings.Controls.Add(CreateLabel("Debug"), 0, 14);
            tblSettings.Controls.Add(panelDebug, 1, 14);
            tblSettings.Controls.Add(CreateLabel("Colors"), 0, 15);
            tblSettings.Controls.Add(panelColors, 1, 15);
            tblSettings.Controls.Add(CreateLabel("Actions"), 0, 16);
            tblSettings.Controls.Add(panelActions, 1, 16);
            tblSettings.Dock = DockStyle.Top;
            tblSettings.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            tblSettings.Location = new Point(12, 12);
            tblSettings.Name = "tblSettings";
            tblSettings.RowCount = 17;
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblSettings.Size = new Size(376, 690);
            tblSettings.TabIndex = 0;
            // 
            // txtTargetText
            // 
            txtTargetText.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtTargetText.Location = new Point(143, 3);
            txtTargetText.Margin = new Padding(3);
            txtTargetText.Multiline = true;
            txtTargetText.Name = "txtTargetText";
            txtTargetText.ScrollBars = ScrollBars.Vertical;
            txtTargetText.Size = new Size(230, 72);
            txtTargetText.TabIndex = 0;
            // 
            // txtFillText
            // 
            txtFillText.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtFillText.Location = new Point(143, 84);
            txtFillText.Name = "txtFillText";
            txtFillText.Size = new Size(230, 23);
            txtFillText.TabIndex = 1;
            // 
            // numGridCount
            // 
            numGridCount.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numGridCount.Location = new Point(143, 113);
            numGridCount.Maximum = new decimal(new int[] { 128, 0, 0, 0 });
            numGridCount.Minimum = new decimal(new int[] { 8, 0, 0, 0 });
            numGridCount.Name = "numGridCount";
            numGridCount.Size = new Size(230, 23);
            numGridCount.TabIndex = 2;
            numGridCount.Value = new decimal(new int[] { 32, 0, 0, 0 });
            // 
            // numCellSize
            // 
            numCellSize.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numCellSize.Location = new Point(143, 142);
            numCellSize.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            numCellSize.Minimum = new decimal(new int[] { 4, 0, 0, 0 });
            numCellSize.Name = "numCellSize";
            numCellSize.Size = new Size(230, 23);
            numCellSize.TabIndex = 3;
            numCellSize.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // numFillFontSize
            // 
            numFillFontSize.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numFillFontSize.DecimalPlaces = 1;
            numFillFontSize.Location = new Point(143, 171);
            numFillFontSize.Maximum = new decimal(new int[] { 96, 0, 0, 0 });
            numFillFontSize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numFillFontSize.Name = "numFillFontSize";
            numFillFontSize.Size = new Size(230, 23);
            numFillFontSize.TabIndex = 4;
            numFillFontSize.Value = new decimal(new int[] { 11, 0, 0, 0 });
            // 
            // cmbFillFontFamily
            // 
            cmbFillFontFamily.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbFillFontFamily.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFillFontFamily.FormattingEnabled = true;
            cmbFillFontFamily.Location = new Point(143, 200);
            cmbFillFontFamily.Name = "cmbFillFontFamily";
            cmbFillFontFamily.Size = new Size(230, 23);
            cmbFillFontFamily.TabIndex = 5;
            // 
            // numTargetFontSize
            // 
            numTargetFontSize.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numTargetFontSize.Location = new Point(143, 229);
            numTargetFontSize.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            numTargetFontSize.Minimum = new decimal(new int[] { 24, 0, 0, 0 });
            numTargetFontSize.Name = "numTargetFontSize";
            numTargetFontSize.Size = new Size(230, 23);
            numTargetFontSize.TabIndex = 6;
            numTargetFontSize.Value = new decimal(new int[] { 520, 0, 0, 0 });
            // 
            // numTargetScale
            // 
            numTargetScale.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numTargetScale.DecimalPlaces = 2;
            numTargetScale.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            numTargetScale.Location = new Point(143, 258);
            numTargetScale.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
            numTargetScale.Minimum = new decimal(new int[] { 10, 0, 0, 131072 });
            numTargetScale.Name = "numTargetScale";
            numTargetScale.Size = new Size(230, 23);
            numTargetScale.TabIndex = 7;
            numTargetScale.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // cmbTargetFontFamily
            // 
            cmbTargetFontFamily.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbTargetFontFamily.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTargetFontFamily.FormattingEnabled = true;
            cmbTargetFontFamily.Location = new Point(143, 287);
            cmbTargetFontFamily.Name = "cmbTargetFontFamily";
            cmbTargetFontFamily.Size = new Size(230, 23);
            cmbTargetFontFamily.TabIndex = 8;
            // 
            // numMaskThreshold
            // 
            numMaskThreshold.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numMaskThreshold.Location = new Point(143, 316);
            numMaskThreshold.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numMaskThreshold.Name = "numMaskThreshold";
            numMaskThreshold.Size = new Size(230, 23);
            numMaskThreshold.TabIndex = 9;
            numMaskThreshold.Value = new decimal(new int[] { 96, 0, 0, 0 });
            // 
            // numPaddingRatio
            // 
            numPaddingRatio.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numPaddingRatio.DecimalPlaces = 2;
            numPaddingRatio.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            numPaddingRatio.Location = new Point(143, 345);
            numPaddingRatio.Maximum = new decimal(new int[] { 20, 0, 0, 131072 });
            numPaddingRatio.Name = "numPaddingRatio";
            numPaddingRatio.Size = new Size(230, 23);
            numPaddingRatio.TabIndex = 10;
            numPaddingRatio.Value = new decimal(new int[] { 8, 0, 0, 131072 });
            // 
            // panelMode
            // 
            panelMode.AutoSize = true;
            panelMode.Controls.Add(rbWholeText);
            panelMode.Controls.Add(rbPerChar);
            panelMode.FlowDirection = FlowDirection.LeftToRight;
            panelMode.Location = new Point(143, 374);
            panelMode.Name = "panelMode";
            panelMode.Size = new Size(205, 29);
            panelMode.TabIndex = 11;
            panelMode.WrapContents = false;
            // 
            // rbWholeText
            // 
            rbWholeText.AutoSize = true;
            rbWholeText.Checked = true;
            rbWholeText.Location = new Point(3, 3);
            rbWholeText.Name = "rbWholeText";
            rbWholeText.Size = new Size(84, 19);
            rbWholeText.TabIndex = 0;
            rbWholeText.TabStop = true;
            rbWholeText.Text = "WholeText";
            rbWholeText.UseVisualStyleBackColor = true;
            // 
            // rbPerChar
            // 
            rbPerChar.AutoSize = true;
            rbPerChar.Location = new Point(93, 3);
            rbPerChar.Name = "rbPerChar";
            rbPerChar.Size = new Size(109, 19);
            rbPerChar.TabIndex = 1;
            rbPerChar.Text = "PerChar Blocks";
            rbPerChar.UseVisualStyleBackColor = true;
            // 
            // numBlockColumns
            // 
            numBlockColumns.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numBlockColumns.Location = new Point(143, 409);
            numBlockColumns.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            numBlockColumns.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numBlockColumns.Name = "numBlockColumns";
            numBlockColumns.Size = new Size(230, 23);
            numBlockColumns.TabIndex = 12;
            numBlockColumns.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // numBlockSpacing
            // 
            numBlockSpacing.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numBlockSpacing.Location = new Point(143, 438);
            numBlockSpacing.Maximum = new decimal(new int[] { 128, 0, 0, 0 });
            numBlockSpacing.Name = "numBlockSpacing";
            numBlockSpacing.Size = new Size(230, 23);
            numBlockSpacing.TabIndex = 13;
            numBlockSpacing.Value = new decimal(new int[] { 16, 0, 0, 0 });
            // 
            // panelDebug
            // 
            panelDebug.AutoSize = true;
            panelDebug.Controls.Add(chkDrawGridDebug);
            panelDebug.Controls.Add(chkShowMaskDebug);
            panelDebug.FlowDirection = FlowDirection.TopDown;
            panelDebug.Location = new Point(143, 467);
            panelDebug.Name = "panelDebug";
            panelDebug.Size = new Size(101, 48);
            panelDebug.TabIndex = 14;
            panelDebug.WrapContents = false;
            // 
            // chkDrawGridDebug
            // 
            chkDrawGridDebug.AutoSize = true;
            chkDrawGridDebug.Location = new Point(3, 3);
            chkDrawGridDebug.Name = "chkDrawGridDebug";
            chkDrawGridDebug.Size = new Size(79, 19);
            chkDrawGridDebug.TabIndex = 0;
            chkDrawGridDebug.Text = "Draw Grid";
            chkDrawGridDebug.UseVisualStyleBackColor = true;
            // 
            // chkShowMaskDebug
            // 
            chkShowMaskDebug.AutoSize = true;
            chkShowMaskDebug.Location = new Point(3, 28);
            chkShowMaskDebug.Name = "chkShowMaskDebug";
            chkShowMaskDebug.Size = new Size(88, 19);
            chkShowMaskDebug.TabIndex = 1;
            chkShowMaskDebug.Text = "Show Mask";
            chkShowMaskDebug.UseVisualStyleBackColor = true;
            // 
            // panelColors
            // 
            panelColors.AutoSize = true;
            panelColors.Controls.Add(btnBackgroundColor);
            panelColors.Controls.Add(btnFillTextColor);
            panelColors.FlowDirection = FlowDirection.LeftToRight;
            panelColors.Location = new Point(143, 521);
            panelColors.Name = "panelColors";
            panelColors.Size = new Size(226, 29);
            panelColors.TabIndex = 15;
            panelColors.WrapContents = false;
            // 
            // btnBackgroundColor
            // 
            btnBackgroundColor.Location = new Point(3, 3);
            btnBackgroundColor.Name = "btnBackgroundColor";
            btnBackgroundColor.Size = new Size(110, 23);
            btnBackgroundColor.TabIndex = 0;
            btnBackgroundColor.Text = "Background";
            btnBackgroundColor.UseVisualStyleBackColor = true;
            // 
            // btnFillTextColor
            // 
            btnFillTextColor.Location = new Point(119, 3);
            btnFillTextColor.Name = "btnFillTextColor";
            btnFillTextColor.Size = new Size(104, 23);
            btnFillTextColor.TabIndex = 1;
            btnFillTextColor.Text = "Fill Color";
            btnFillTextColor.UseVisualStyleBackColor = true;
            // 
            // panelActions
            // 
            panelActions.AutoSize = true;
            panelActions.Controls.Add(btnCopyPreview);
            panelActions.Controls.Add(btnExportPng);
            panelActions.FlowDirection = FlowDirection.LeftToRight;
            panelActions.Location = new Point(143, 556);
            panelActions.Name = "panelActions";
            panelActions.Size = new Size(266, 29);
            panelActions.TabIndex = 16;
            panelActions.WrapContents = false;
            // 
            // btnCopyPreview
            // 
            btnCopyPreview.Location = new Point(3, 3);
            btnCopyPreview.Name = "btnCopyPreview";
            btnCopyPreview.Size = new Size(130, 23);
            btnCopyPreview.TabIndex = 0;
            btnCopyPreview.Text = "Copy Preview";
            btnCopyPreview.UseVisualStyleBackColor = true;
            // 
            // btnExportPng
            // 
            btnExportPng.Location = new Point(139, 3);
            btnExportPng.Name = "btnExportPng";
            btnExportPng.Size = new Size(124, 23);
            btnExportPng.TabIndex = 1;
            btnExportPng.Text = "Export PNG";
            btnExportPng.UseVisualStyleBackColor = true;
            // 
            // panelRight
            // 
            panelRight.Controls.Add(picturePreview);
            panelRight.Dock = DockStyle.Fill;
            panelRight.Location = new Point(0, 0);
            panelRight.Name = "panelRight";
            panelRight.Padding = new Padding(12);
            panelRight.Size = new Size(956, 860);
            panelRight.TabIndex = 0;
            // 
            // picturePreview
            // 
            picturePreview.BackColor = Color.White;
            picturePreview.BorderStyle = BorderStyle.FixedSingle;
            picturePreview.Dock = DockStyle.Fill;
            picturePreview.Location = new Point(12, 12);
            picturePreview.Name = "picturePreview";
            picturePreview.Size = new Size(932, 836);
            picturePreview.SizeMode = PictureBoxSizeMode.Zoom;
            picturePreview.TabIndex = 0;
            picturePreview.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1360, 860);
            Controls.Add(splitMain);
            MinimumSize = new Size(1080, 700);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PointWord";
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            panelLeft.ResumeLayout(false);
            panelLeft.PerformLayout();
            tblSettings.ResumeLayout(false);
            tblSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numGridCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCellSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFillFontSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTargetFontSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTargetScale).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaskThreshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPaddingRatio).EndInit();
            panelMode.ResumeLayout(false);
            panelMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numBlockColumns).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBlockSpacing).EndInit();
            panelDebug.ResumeLayout(false);
            panelDebug.PerformLayout();
            panelColors.ResumeLayout(false);
            panelActions.ResumeLayout(false);
            panelRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picturePreview).EndInit();
            ResumeLayout(false);
        }

        private static Label CreateLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 6, 0, 10),
            };
        }

        #endregion

        private SplitContainer splitMain;
        private Panel panelLeft;
        private Panel panelRight;
        private TableLayoutPanel tblSettings;
        private FlowLayoutPanel panelMode;
        private FlowLayoutPanel panelDebug;
        private FlowLayoutPanel panelColors;
        private FlowLayoutPanel panelActions;
        private TextBox txtTargetText;
        private TextBox txtFillText;
        private NumericUpDown numGridCount;
        private NumericUpDown numCellSize;
        private NumericUpDown numFillFontSize;
        private ComboBox cmbFillFontFamily;
        private NumericUpDown numTargetFontSize;
        private NumericUpDown numTargetScale;
        private ComboBox cmbTargetFontFamily;
        private NumericUpDown numMaskThreshold;
        private NumericUpDown numPaddingRatio;
        private NumericUpDown numBlockColumns;
        private NumericUpDown numBlockSpacing;
        private RadioButton rbWholeText;
        private RadioButton rbPerChar;
        private CheckBox chkDrawGridDebug;
        private CheckBox chkShowMaskDebug;
        private Button btnBackgroundColor;
        private Button btnFillTextColor;
        private Button btnCopyPreview;
        private Button btnExportPng;
        private PictureBox picturePreview;
        private Label lblStatus;
    }
}
