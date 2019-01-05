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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCut = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPasteReverse = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReverse = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIsShortNote = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIsHold = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiIsSlide = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIsSlideRelay = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIsSlideCurve = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiIsAirHold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiIsAir = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiIsExTapDistinct = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShowHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.tabNoteButton = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.flpNotePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tabScore = new System.Windows.Forms.TabControl();
            this.Score1 = new System.Windows.Forms.TabPage();
            this.Score2 = new System.Windows.Forms.TabPage();
            this.Score3 = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbNew = new System.Windows.Forms.ToolStripButton();
            this.tsbOpen = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.tsbExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbCut = new System.Windows.Forms.ToolStripButton();
            this.tsbCopy = new System.Windows.Forms.ToolStripButton();
            this.tsbPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbEdit = new System.Windows.Forms.ToolStripButton();
            this.tsbDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tslBeat = new System.Windows.Forms.ToolStripLabel();
            this.tscbBeat = new System.Windows.Forms.ToolStripComboBox();
            this.tslGrid = new System.Windows.Forms.ToolStripLabel();
            this.tscbGrid = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbInvisibleSlideTap = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tslEstimatedNotes = new System.Windows.Forms.ToolStripLabel();
            this.tslRealNotes = new System.Windows.Forms.ToolStripLabel();
            this.menuStrip1.SuspendLayout();
            this.tabNoteButton.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabScore.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiEdit,
            this.tsmiView,
            this.tsmiHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1264, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNew,
            this.tsmiOpen,
            this.tsmiSave,
            this.tsmiSaveAs,
            this.tsmiExport,
            this.toolStripSeparator4,
            this.tsmiQuit});
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.tsmiFile.Size = new System.Drawing.Size(67, 20);
            this.tsmiFile.Text = "ファイル(&F)";
            // 
            // tsmiNew
            // 
            this.tsmiNew.Name = "tsmiNew";
            this.tsmiNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsmiNew.Size = new System.Drawing.Size(257, 22);
            this.tsmiNew.Text = "新規作成(&N)";
            // 
            // tsmiOpen
            // 
            this.tsmiOpen.Name = "tsmiOpen";
            this.tsmiOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsmiOpen.Size = new System.Drawing.Size(257, 22);
            this.tsmiOpen.Text = "開く(&O)...";
            // 
            // tsmiSave
            // 
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsmiSave.Size = new System.Drawing.Size(257, 22);
            this.tsmiSave.Text = "上書き保存(&S)";
            // 
            // tsmiSaveAs
            // 
            this.tsmiSaveAs.Name = "tsmiSaveAs";
            this.tsmiSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.tsmiSaveAs.Size = new System.Drawing.Size(257, 22);
            this.tsmiSaveAs.Text = "名前を付けて保存(&A)...";
            // 
            // tsmiExport
            // 
            this.tsmiExport.Name = "tsmiExport";
            this.tsmiExport.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.X)));
            this.tsmiExport.Size = new System.Drawing.Size(257, 22);
            this.tsmiExport.Text = "エクスポート(&X)...";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(254, 6);
            // 
            // tsmiQuit
            // 
            this.tsmiQuit.Name = "tsmiQuit";
            this.tsmiQuit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.tsmiQuit.Size = new System.Drawing.Size(257, 22);
            this.tsmiQuit.Text = "終了(&Q)";
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCut,
            this.tsmiCopy,
            this.tsmiPaste,
            this.tsmiPasteReverse,
            this.tsmiDelete,
            this.tsmiReverse});
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.tsmiEdit.Size = new System.Drawing.Size(57, 20);
            this.tsmiEdit.Text = "編集(&E)";
            // 
            // tsmiCut
            // 
            this.tsmiCut.Name = "tsmiCut";
            this.tsmiCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.tsmiCut.Size = new System.Drawing.Size(252, 22);
            this.tsmiCut.Text = "切り取り";
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.tsmiCopy.Size = new System.Drawing.Size(252, 22);
            this.tsmiCopy.Text = "コピー";
            // 
            // tsmiPaste
            // 
            this.tsmiPaste.Name = "tsmiPaste";
            this.tsmiPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.tsmiPaste.Size = new System.Drawing.Size(252, 22);
            this.tsmiPaste.Text = "貼り付け";
            // 
            // tsmiPasteReverse
            // 
            this.tsmiPasteReverse.Name = "tsmiPasteReverse";
            this.tsmiPasteReverse.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.V)));
            this.tsmiPasteReverse.Size = new System.Drawing.Size(252, 22);
            this.tsmiPasteReverse.Text = "左右反転して貼り付け";
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.tsmiDelete.Size = new System.Drawing.Size(252, 22);
            this.tsmiDelete.Text = "削除";
            // 
            // tsmiReverse
            // 
            this.tsmiReverse.Name = "tsmiReverse";
            this.tsmiReverse.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.tsmiReverse.Size = new System.Drawing.Size(252, 22);
            this.tsmiReverse.Text = "左右反転";
            // 
            // tsmiView
            // 
            this.tsmiView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiIsShortNote,
            this.tsmiIsHold,
            this.toolStripSeparator8,
            this.tsmiIsSlide,
            this.tsmiIsSlideRelay,
            this.tsmiIsSlideCurve,
            this.toolStripSeparator9,
            this.tsmiIsAirHold,
            this.tsmiIsAir,
            this.toolStripSeparator10,
            this.tsmiIsExTapDistinct});
            this.tsmiView.Name = "tsmiView";
            this.tsmiView.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.V)));
            this.tsmiView.Size = new System.Drawing.Size(58, 20);
            this.tsmiView.Text = "表示(&V)";
            // 
            // tsmiIsShortNote
            // 
            this.tsmiIsShortNote.Checked = true;
            this.tsmiIsShortNote.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiIsShortNote.Name = "tsmiIsShortNote";
            this.tsmiIsShortNote.Size = new System.Drawing.Size(215, 22);
            this.tsmiIsShortNote.Text = "ShortNote";
            // 
            // tsmiIsHold
            // 
            this.tsmiIsHold.Checked = true;
            this.tsmiIsHold.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiIsHold.Name = "tsmiIsHold";
            this.tsmiIsHold.Size = new System.Drawing.Size(215, 22);
            this.tsmiIsHold.Text = "Hold";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(212, 6);
            // 
            // tsmiIsSlide
            // 
            this.tsmiIsSlide.Checked = true;
            this.tsmiIsSlide.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiIsSlide.Name = "tsmiIsSlide";
            this.tsmiIsSlide.Size = new System.Drawing.Size(215, 22);
            this.tsmiIsSlide.Text = "Slide";
            // 
            // tsmiIsSlideRelay
            // 
            this.tsmiIsSlideRelay.Checked = true;
            this.tsmiIsSlideRelay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiIsSlideRelay.Name = "tsmiIsSlideRelay";
            this.tsmiIsSlideRelay.Size = new System.Drawing.Size(215, 22);
            this.tsmiIsSlideRelay.Text = "SlideRelay(不可視中継点)";
            // 
            // tsmiIsSlideCurve
            // 
            this.tsmiIsSlideCurve.Checked = true;
            this.tsmiIsSlideCurve.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiIsSlideCurve.Name = "tsmiIsSlideCurve";
            this.tsmiIsSlideCurve.Size = new System.Drawing.Size(215, 22);
            this.tsmiIsSlideCurve.Text = "SlideCurve(曲線ノーツ)";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(212, 6);
            // 
            // tsmiIsAirHold
            // 
            this.tsmiIsAirHold.Checked = true;
            this.tsmiIsAirHold.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiIsAirHold.Name = "tsmiIsAirHold";
            this.tsmiIsAirHold.Size = new System.Drawing.Size(215, 22);
            this.tsmiIsAirHold.Text = "AirHold/AirAction";
            // 
            // tsmiIsAir
            // 
            this.tsmiIsAir.Checked = true;
            this.tsmiIsAir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiIsAir.Name = "tsmiIsAir";
            this.tsmiIsAir.Size = new System.Drawing.Size(215, 22);
            this.tsmiIsAir.Text = "Air";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(212, 6);
            // 
            // tsmiIsExTapDistinct
            // 
            this.tsmiIsExTapDistinct.Checked = true;
            this.tsmiIsExTapDistinct.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiIsExTapDistinct.Name = "tsmiIsExTapDistinct";
            this.tsmiIsExTapDistinct.Size = new System.Drawing.Size(215, 22);
            this.tsmiIsExTapDistinct.Text = "ExTap系ノーツの色を区別する";
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiShowHelp,
            this.toolStripSeparator7,
            this.tsmiVersion});
            this.tsmiHelp.Name = "tsmiHelp";
            this.tsmiHelp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.H)));
            this.tsmiHelp.Size = new System.Drawing.Size(65, 20);
            this.tsmiHelp.Text = "ヘルプ(&H)";
            // 
            // tsmiShowHelp
            // 
            this.tsmiShowHelp.Name = "tsmiShowHelp";
            this.tsmiShowHelp.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.tsmiShowHelp.Size = new System.Drawing.Size(156, 22);
            this.tsmiShowHelp.Text = "ヘルプの表示";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(153, 6);
            // 
            // tsmiVersion
            // 
            this.tsmiVersion.Name = "tsmiVersion";
            this.tsmiVersion.Size = new System.Drawing.Size(156, 22);
            this.tsmiVersion.Text = "バージョン情報";
            // 
            // tabNoteButton
            // 
            this.tabNoteButton.Controls.Add(this.tabPage1);
            this.tabNoteButton.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabNoteButton.Location = new System.Drawing.Point(7, 52);
            this.tabNoteButton.Multiline = true;
            this.tabNoteButton.Name = "tabNoteButton";
            this.tabNoteButton.SelectedIndex = 0;
            this.tabNoteButton.Size = new System.Drawing.Size(187, 846);
            this.tabNoteButton.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.flpNotePanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(179, 820);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Normal";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // flpNotePanel
            // 
            this.flpNotePanel.AutoScroll = true;
            this.flpNotePanel.Location = new System.Drawing.Point(3, 3);
            this.flpNotePanel.Name = "flpNotePanel";
            this.flpNotePanel.Size = new System.Drawing.Size(173, 814);
            this.flpNotePanel.TabIndex = 0;
            // 
            // tabScore
            // 
            this.tabScore.Controls.Add(this.Score1);
            this.tabScore.Controls.Add(this.Score2);
            this.tabScore.Controls.Add(this.Score3);
            this.tabScore.Location = new System.Drawing.Point(200, 52);
            this.tabScore.Name = "tabScore";
            this.tabScore.SelectedIndex = 0;
            this.tabScore.Size = new System.Drawing.Size(1064, 846);
            this.tabScore.TabIndex = 2;
            // 
            // Score1
            // 
            this.Score1.Location = new System.Drawing.Point(4, 22);
            this.Score1.Name = "Score1";
            this.Score1.Padding = new System.Windows.Forms.Padding(3);
            this.Score1.Size = new System.Drawing.Size(1056, 820);
            this.Score1.TabIndex = 1;
            this.Score1.Text = "Score1";
            this.Score1.UseVisualStyleBackColor = true;
            // 
            // Score2
            // 
            this.Score2.Location = new System.Drawing.Point(4, 22);
            this.Score2.Name = "Score2";
            this.Score2.Padding = new System.Windows.Forms.Padding(3);
            this.Score2.Size = new System.Drawing.Size(1056, 820);
            this.Score2.TabIndex = 2;
            this.Score2.Text = "Score2";
            this.Score2.UseVisualStyleBackColor = true;
            // 
            // Score3
            // 
            this.Score3.Location = new System.Drawing.Point(4, 22);
            this.Score3.Name = "Score3";
            this.Score3.Padding = new System.Windows.Forms.Padding(3);
            this.Score3.Size = new System.Drawing.Size(1056, 820);
            this.Score3.TabIndex = 3;
            this.Score3.Text = "Score3";
            this.Score3.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbNew,
            this.tsbOpen,
            this.tsbSave,
            this.tsbExport,
            this.toolStripSeparator1,
            this.tsbCut,
            this.tsbCopy,
            this.tsbPaste,
            this.toolStripSeparator2,
            this.tsbAdd,
            this.tsbEdit,
            this.tsbDelete,
            this.toolStripSeparator3,
            this.tslBeat,
            this.tscbBeat,
            this.tslGrid,
            this.tscbGrid,
            this.toolStripSeparator5,
            this.tsbInvisibleSlideTap,
            this.toolStripSeparator6,
            this.tslEstimatedNotes,
            this.tslRealNotes});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(1264, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbNew
            // 
            this.tsbNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNew.Image = global::NE4S.Properties.Resources.NewFile_16x;
            this.tsbNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNew.Name = "tsbNew";
            this.tsbNew.Size = new System.Drawing.Size(23, 22);
            this.tsbNew.Text = "新規作成";
            // 
            // tsbOpen
            // 
            this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpen.Image = global::NE4S.Properties.Resources.OpenFolder_16x;
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(23, 22);
            this.tsbOpen.Text = "開く";
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = global::NE4S.Properties.Resources.Save_16x;
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "上書き保存";
            // 
            // tsbExport
            // 
            this.tsbExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbExport.Image = global::NE4S.Properties.Resources.ExportFile_16x;
            this.tsbExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbExport.Name = "tsbExport";
            this.tsbExport.Size = new System.Drawing.Size(23, 22);
            this.tsbExport.Text = "上書きエクスポート";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbCut
            // 
            this.tsbCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCut.Image = global::NE4S.Properties.Resources.Cut_16x;
            this.tsbCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCut.Name = "tsbCut";
            this.tsbCut.Size = new System.Drawing.Size(23, 22);
            this.tsbCut.Text = "切り取り";
            // 
            // tsbCopy
            // 
            this.tsbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCopy.Image = global::NE4S.Properties.Resources.Copy_16x;
            this.tsbCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCopy.Name = "tsbCopy";
            this.tsbCopy.Size = new System.Drawing.Size(23, 22);
            this.tsbCopy.Text = "コピー";
            // 
            // tsbPaste
            // 
            this.tsbPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPaste.Image = global::NE4S.Properties.Resources.Paste_16x;
            this.tsbPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPaste.Name = "tsbPaste";
            this.tsbPaste.Size = new System.Drawing.Size(23, 22);
            this.tsbPaste.Text = "貼り付け";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbAdd
            // 
            this.tsbAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbAdd.Image = ((System.Drawing.Image)(resources.GetObject("tsbAdd.Image")));
            this.tsbAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAdd.Name = "tsbAdd";
            this.tsbAdd.Size = new System.Drawing.Size(33, 22);
            this.tsbAdd.Text = "Add";
            // 
            // tsbEdit
            // 
            this.tsbEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbEdit.Image = ((System.Drawing.Image)(resources.GetObject("tsbEdit.Image")));
            this.tsbEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEdit.Name = "tsbEdit";
            this.tsbEdit.Size = new System.Drawing.Size(31, 22);
            this.tsbEdit.Text = "Edit";
            // 
            // tsbDelete
            // 
            this.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbDelete.Image = ((System.Drawing.Image)(resources.GetObject("tsbDelete.Image")));
            this.tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDelete.Name = "tsbDelete";
            this.tsbDelete.Size = new System.Drawing.Size(44, 22);
            this.tsbDelete.Text = "Delete";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // tslBeat
            // 
            this.tslBeat.Name = "tslBeat";
            this.tslBeat.Size = new System.Drawing.Size(30, 22);
            this.tslBeat.Text = "Beat";
            // 
            // tscbBeat
            // 
            this.tscbBeat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbBeat.DropDownWidth = 121;
            this.tscbBeat.Items.AddRange(new object[] {
            "4",
            "8",
            "12",
            "16",
            "24",
            "32",
            "48",
            "64",
            "96",
            "128",
            "192"});
            this.tscbBeat.Name = "tscbBeat";
            this.tscbBeat.Size = new System.Drawing.Size(75, 25);
            // 
            // tslGrid
            // 
            this.tslGrid.Name = "tslGrid";
            this.tslGrid.Size = new System.Drawing.Size(29, 22);
            this.tslGrid.Text = "Grid";
            this.tslGrid.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            // 
            // tscbGrid
            // 
            this.tscbGrid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbGrid.Items.AddRange(new object[] {
            "4",
            "8",
            "16"});
            this.tscbGrid.Name = "tscbGrid";
            this.tscbGrid.Size = new System.Drawing.Size(75, 25);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbInvisibleSlideTap
            // 
            this.tsbInvisibleSlideTap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbInvisibleSlideTap.Image = ((System.Drawing.Image)(resources.GetObject("tsbInvisibleSlideTap.Image")));
            this.tsbInvisibleSlideTap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbInvisibleSlideTap.Name = "tsbInvisibleSlideTap";
            this.tsbInvisibleSlideTap.Size = new System.Drawing.Size(97, 22);
            this.tsbInvisibleSlideTap.Text = "InvisibleSlideTap";
            this.tsbInvisibleSlideTap.ToolTipText = "Slide中継点を不可視にする";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // tslEstimatedNotes
            // 
            this.tslEstimatedNotes.Name = "tslEstimatedNotes";
            this.tslEstimatedNotes.Size = new System.Drawing.Size(141, 22);
            this.tslEstimatedNotes.Text = "Total notes (estimated) : 0";
            this.tslEstimatedNotes.ToolTipText = "シミュレーター上で表示される総ノーツ数（推定）";
            this.tslEstimatedNotes.Visible = false;
            // 
            // tslRealNotes
            // 
            this.tslRealNotes.Name = "tslRealNotes";
            this.tslRealNotes.Size = new System.Drawing.Size(109, 22);
            this.tslRealNotes.Text = "Total notes (real) : 0";
            this.tslRealNotes.ToolTipText = "エディタ上で配置されているノーツオブジェクトの実際の個数";
            this.tslRealNotes.Visible = false;
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "M4ple Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabNoteButton.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabScore.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelp;
        private System.Windows.Forms.TabControl tabNoteButton;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabScore;
        private System.Windows.Forms.TabPage Score1;
        private System.Windows.Forms.TabPage Score2;
        private System.Windows.Forms.TabPage Score3;
        private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton tsbNew;
		private System.Windows.Forms.ToolStripButton tsbOpen;
		private System.Windows.Forms.ToolStripButton tsbSave;
		private System.Windows.Forms.ToolStripButton tsbExport;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton tsbCut;
		private System.Windows.Forms.ToolStripButton tsbCopy;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripLabel tslBeat;
		private System.Windows.Forms.ToolStripComboBox tscbBeat;
		private System.Windows.Forms.ToolStripLabel tslGrid;
		private System.Windows.Forms.ToolStripComboBox tscbGrid;
		private System.Windows.Forms.ToolStripButton tsbAdd;
		private System.Windows.Forms.ToolStripButton tsbEdit;
		private System.Windows.Forms.ToolStripButton tsbDelete;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripButton tsbInvisibleSlideTap;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripLabel tslEstimatedNotes;
		private System.Windows.Forms.ToolStripLabel tslRealNotes;
        private System.Windows.Forms.FlowLayoutPanel flpNotePanel;
        private System.Windows.Forms.ToolStripMenuItem tsmiView;
        private System.Windows.Forms.ToolStripMenuItem tsmiIsSlideRelay;
        private System.Windows.Forms.ToolStripMenuItem tsmiIsSlideCurve;
        private System.Windows.Forms.ToolStripMenuItem tsmiNew;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpen;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveAs;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport;
        private System.Windows.Forms.ToolStripMenuItem tsmiQuit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem tsmiCut;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiIsShortNote;
        private System.Windows.Forms.ToolStripMenuItem tsmiIsHold;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem tsmiIsSlide;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem tsmiIsAirHold;
        private System.Windows.Forms.ToolStripMenuItem tsmiIsAir;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowHelp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem tsmiVersion;
        private System.Windows.Forms.ToolStripMenuItem tsmiIsExTapDistinct;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripButton tsbPaste;
        private System.Windows.Forms.ToolStripMenuItem tsmiPaste;
        private System.Windows.Forms.ToolStripMenuItem tsmiPasteReverse;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmiReverse;
    }
}

