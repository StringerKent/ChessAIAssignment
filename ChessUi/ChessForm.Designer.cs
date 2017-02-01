namespace ChessUi
{
    partial class ChessForm
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
            this.components = new System.ComponentModel.Container();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderNumbeInGame = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderWhite = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBlack = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxAIblack = new System.Windows.Forms.CheckBox();
            this.checkBoxAI_white = new System.Windows.Forms.CheckBox();
            this.numericUpDownThinkBlack = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownThinkWhite = new System.Windows.Forms.NumericUpDown();
            this.labelScoreAndLine = new System.Windows.Forms.Label();
            this.buttonFlip = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new DoubledBufferedPanel();
            this.progressBarBottom = new System.Windows.Forms.ProgressBar();
            this.progressBarTop = new System.Windows.Forms.ProgressBar();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThinkBlack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThinkWhite)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderNumbeInGame,
            this.columnHeaderWhite,
            this.columnHeaderBlack});
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(739, 30);
            this.listView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(301, 560);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listview_MouseMove);
            // 
            // columnHeaderNumbeInGame
            // 
            this.columnHeaderNumbeInGame.Text = "No";
            // 
            // columnHeaderWhite
            // 
            this.columnHeaderWhite.Text = "White";
            this.columnHeaderWhite.Width = 70;
            // 
            // columnHeaderBlack
            // 
            this.columnHeaderBlack.Text = "Black";
            this.columnHeaderBlack.Width = 70;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1052, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(209, 26);
            this.newToolStripMenuItem.Text = "&New game";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(209, 26);
            this.loadToolStripMenuItem.Text = "&Open";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(209, 26);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.boardToolStripMenuItem,
            this.undoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // boardToolStripMenuItem
            // 
            this.boardToolStripMenuItem.Name = "boardToolStripMenuItem";
            this.boardToolStripMenuItem.Size = new System.Drawing.Size(171, 26);
            this.boardToolStripMenuItem.Text = "&Board";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(171, 26);
            this.undoToolStripMenuItem.Text = "&Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // checkBoxAIblack
            // 
            this.checkBoxAIblack.AutoSize = true;
            this.checkBoxAIblack.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxAIblack.Location = new System.Drawing.Point(13, 34);
            this.checkBoxAIblack.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBoxAIblack.Name = "checkBoxAIblack";
            this.checkBoxAIblack.Size = new System.Drawing.Size(58, 21);
            this.checkBoxAIblack.TabIndex = 3;
            this.checkBoxAIblack.Text = "CPU";
            this.checkBoxAIblack.UseVisualStyleBackColor = true;
            this.checkBoxAIblack.CheckedChanged += new System.EventHandler(this.checkBoxAIblack_CheckedChanged);
            // 
            // checkBoxAI_white
            // 
            this.checkBoxAI_white.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxAI_white.AutoSize = true;
            this.checkBoxAI_white.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxAI_white.Location = new System.Drawing.Point(13, 597);
            this.checkBoxAI_white.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBoxAI_white.Name = "checkBoxAI_white";
            this.checkBoxAI_white.Size = new System.Drawing.Size(58, 21);
            this.checkBoxAI_white.TabIndex = 4;
            this.checkBoxAI_white.Text = "CPU";
            this.checkBoxAI_white.UseVisualStyleBackColor = true;
            this.checkBoxAI_white.CheckedChanged += new System.EventHandler(this.checkBoxAI_white_CheckedChanged);
            // 
            // numericUpDownThinkBlack
            // 
            this.numericUpDownThinkBlack.Location = new System.Drawing.Point(126, 34);
            this.numericUpDownThinkBlack.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDownThinkBlack.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownThinkBlack.Name = "numericUpDownThinkBlack";
            this.numericUpDownThinkBlack.Size = new System.Drawing.Size(45, 22);
            this.numericUpDownThinkBlack.TabIndex = 5;
            this.numericUpDownThinkBlack.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(80, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Think";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(177, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "sec";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(177, 598);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "sec";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(80, 598);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "Think";
            // 
            // numericUpDownThinkWhite
            // 
            this.numericUpDownThinkWhite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numericUpDownThinkWhite.Location = new System.Drawing.Point(126, 597);
            this.numericUpDownThinkWhite.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDownThinkWhite.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownThinkWhite.Name = "numericUpDownThinkWhite";
            this.numericUpDownThinkWhite.Size = new System.Drawing.Size(45, 22);
            this.numericUpDownThinkWhite.TabIndex = 10;
            this.numericUpDownThinkWhite.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // labelScoreAndLine
            // 
            this.labelScoreAndLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelScoreAndLine.AutoSize = true;
            this.labelScoreAndLine.Location = new System.Drawing.Point(435, 600);
            this.labelScoreAndLine.Name = "labelScoreAndLine";
            this.labelScoreAndLine.Size = new System.Drawing.Size(0, 17);
            this.labelScoreAndLine.TabIndex = 11;
            // 
            // buttonFlip
            // 
            this.buttonFlip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFlip.Location = new System.Drawing.Point(658, 30);
            this.buttonFlip.Name = "buttonFlip";
            this.buttonFlip.Size = new System.Drawing.Size(75, 27);
            this.buttonFlip.TabIndex = 13;
            this.buttonFlip.Text = "Flip";
            this.buttonFlip.UseVisualStyleBackColor = true;
            this.buttonFlip.Click += new System.EventHandler(this.buttonFlip_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.progressBarBottom);
            this.panel1.Controls.Add(this.progressBarTop);
            this.panel1.Location = new System.Drawing.Point(12, 62);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(721, 529);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // progressBarBottom
            // 
            this.progressBarBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarBottom.Location = new System.Drawing.Point(1, 525);
            this.progressBarBottom.Name = "progressBarBottom";
            this.progressBarBottom.Size = new System.Drawing.Size(720, 4);
            this.progressBarBottom.TabIndex = 0;
            // 
            // progressBarTop
            // 
            this.progressBarTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarTop.Location = new System.Drawing.Point(1, 0);
            this.progressBarTop.Name = "progressBarTop";
            this.progressBarTop.Size = new System.Drawing.Size(720, 4);
            this.progressBarTop.TabIndex = 0;
            // 
            // ChessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 627);
            this.Controls.Add(this.buttonFlip);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelScoreAndLine);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericUpDownThinkWhite);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDownThinkBlack);
            this.Controls.Add(this.checkBoxAI_white);
            this.Controls.Add(this.checkBoxAIblack);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ChessForm";
            this.Text = "Chess";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThinkBlack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThinkWhite)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DoubledBufferedPanel panel1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderNumbeInGame;
        private System.Windows.Forms.ColumnHeader columnHeaderWhite;
        private System.Windows.Forms.ColumnHeader columnHeaderBlack;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxAIblack;
        private System.Windows.Forms.CheckBox checkBoxAI_white;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown numericUpDownThinkBlack;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDownThinkWhite;
        private System.Windows.Forms.Label labelScoreAndLine;
        private System.Windows.Forms.Button buttonFlip;
        private System.Windows.Forms.ProgressBar progressBarBottom;
        private System.Windows.Forms.ProgressBar progressBarTop;
        private System.Windows.Forms.Timer timer1;
    }
}

