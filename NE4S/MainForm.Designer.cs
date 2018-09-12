namespace NE4S
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ファイルToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.編集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ヘルプToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabNoteButton = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabScore = new System.Windows.Forms.TabControl();
            this.Score1 = new System.Windows.Forms.TabPage();
            this.Score2 = new System.Windows.Forms.TabPage();
            this.Score3 = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.menuStrip1.SuspendLayout();
            this.tabNoteButton.SuspendLayout();
            this.tabScore.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルToolStripMenuItem,
            this.編集ToolStripMenuItem,
            this.ヘルプToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1264, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ファイルToolStripMenuItem
            // 
            this.ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
            this.ファイルToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.ファイルToolStripMenuItem.Text = "ファイル";
            // 
            // 編集ToolStripMenuItem
            // 
            this.編集ToolStripMenuItem.Name = "編集ToolStripMenuItem";
            this.編集ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.編集ToolStripMenuItem.Text = "編集";
            // 
            // ヘルプToolStripMenuItem
            // 
            this.ヘルプToolStripMenuItem.Name = "ヘルプToolStripMenuItem";
            this.ヘルプToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.ヘルプToolStripMenuItem.Text = "ヘルプ";
            // 
            // tabNoteButton
            // 
            this.tabNoteButton.Controls.Add(this.tabPage1);
            this.tabNoteButton.Controls.Add(this.tabPage2);
            this.tabNoteButton.Location = new System.Drawing.Point(11, 52);
            this.tabNoteButton.Multiline = true;
            this.tabNoteButton.Name = "tabNoteButton";
            this.tabNoteButton.SelectedIndex = 0;
            this.tabNoteButton.Size = new System.Drawing.Size(200, 837);
            this.tabNoteButton.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(192, 811);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Normal";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(192, 857);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Custom";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabScore
            // 
            this.tabScore.Controls.Add(this.Score1);
            this.tabScore.Controls.Add(this.Score2);
            this.tabScore.Controls.Add(this.Score3);
            this.tabScore.Location = new System.Drawing.Point(213, 52);
            this.tabScore.Name = "tabScore";
            this.tabScore.SelectedIndex = 0;
            this.tabScore.Size = new System.Drawing.Size(1039, 837);
            this.tabScore.TabIndex = 2;
            // 
            // Score1
            // 
            this.Score1.Location = new System.Drawing.Point(4, 22);
            this.Score1.Name = "Score1";
            this.Score1.Padding = new System.Windows.Forms.Padding(3);
            this.Score1.Size = new System.Drawing.Size(1031, 811);
            this.Score1.TabIndex = 1;
            this.Score1.Text = "Score1";
            this.Score1.UseVisualStyleBackColor = true;
            // 
            // Score2
            // 
            this.Score2.Location = new System.Drawing.Point(4, 22);
            this.Score2.Name = "Score2";
            this.Score2.Padding = new System.Windows.Forms.Padding(3);
            this.Score2.Size = new System.Drawing.Size(1031, 811);
            this.Score2.TabIndex = 2;
            this.Score2.Text = "Score2";
            this.Score2.UseVisualStyleBackColor = true;
            // 
            // Score3
            // 
            this.Score3.Location = new System.Drawing.Point(4, 22);
            this.Score3.Name = "Score3";
            this.Score3.Padding = new System.Windows.Forms.Padding(3);
            this.Score3.Size = new System.Drawing.Size(1031, 811);
            this.Score3.TabIndex = 3;
            this.Score3.Text = "Score3";
            this.Score3.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(1264, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 901);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tabScore);
            this.Controls.Add(this.tabNoteButton);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabNoteButton.ResumeLayout(false);
            this.tabScore.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ファイルToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 編集ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ヘルプToolStripMenuItem;
        private System.Windows.Forms.TabControl tabNoteButton;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabScore;
        private System.Windows.Forms.TabPage Score1;
        private System.Windows.Forms.TabPage Score2;
        private System.Windows.Forms.TabPage Score3;
        private System.Windows.Forms.ToolStrip toolStrip1;
    }
}

