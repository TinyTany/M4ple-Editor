using NE4S.Define;
using NE4S.Scores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Component;

namespace NE4S
{
    public partial class MainForm : Form
    {
        private readonly int tabPageCount = 3;

        public MainForm()
        {
            InitializeComponent();
            tabScore.TabPages.Clear();
            tabScore.Font = new Font("Yu Gothic UI", 9);
            tabNoteButton.Font = tabScore.Font;
            for(int i = 0; i < tabPageCount; ++i)
            {
                TabPageEx tabPageEx = new TabPageEx("NewScore" + (i+1));
                tabScore.TabPages.Add(tabPageEx);
                // PictureBoxの追加と初期化
                PictureBox pBox = new PictureBox
                {
                    Size = tabScore.TabPages[i].Size
                };
                // TabPageに初期化したPictureBoxを入れる
                tabScore.TabPages[i].Controls.Add(pBox);
                // ScrollBarの追加と初期化
                HScrollBar hScroll = new HScrollBar
                {
                    Size = new Size(pBox.Width, 17),
                    Value = 0,
                    Minimum = 0,
                    Dock = DockStyle.Bottom
                };
                VScrollBar vScroll = new VScrollBar
                {
                    Size = new Size(17, pBox.Height),
                    Value = 0,
                    Minimum = 0,
                    Maximum = 0,
                    Dock = DockStyle.Right,
                    Visible = false
                };
                // HScrollBarをPictureBoxに入れる
                pBox.Controls.Add(hScroll);
                pBox.Controls.Add(vScroll);
                // 初期化したPictureBoxとHScrollBarを使用してScorePanelを追加
                ScorePanel sPanel = new ScorePanel(pBox, hScroll, vScroll);
                sPanel.OperationManager.StatusChanged += (undo, redo) =>
                {
                    tsbUndo.Enabled = tsmiUndo.Enabled = undo;
                    tsbRedo.Enabled = tsmiRedo.Enabled = redo;
                };
                sPanel.OperationManager.Edited += () =>
                {
                    UpdateTextOfTabAndForm(true);
                    sPanel.IsEdited = true;
                };
                sPanel.SetScore(4, 4, 200);
                tabPageEx.ScorePanel = sPanel;
                // PictureBoxとHScrollBarの各種デリゲートの設定
                pBox.MouseWheel += Score_MouseWheel;
                pBox.Paint += Score_Paint;
                pBox.MouseClick += Score_MouseClick;
                pBox.MouseEnter += Score_MouseEnter;
                pBox.MouseDown += Score_MouseDown;
                pBox.MouseMove += Score_MouseMove;
                pBox.MouseUp += Score_MouseUp;
                hScroll.Scroll += Score_Scroll;
                vScroll.Scroll += Score_Scroll;
            }
			InitializeToolStrip();
            #region ノーツボタン追加
            NoteButtonManager noteButtonManager = new NoteButtonManager();
            noteButtonManager.ButtonClicked += (s, e) => SetMode(Mode.ADD);
            flpNotePanel.Size = tabNoteButton.TabPages[0].Size;
            flpNotePanel.Location = new Point();
            foreach (NoteButton noteButton in noteButtonManager)
            {
                flpNotePanel.Controls.Add(noteButton);
            }
            #endregion
            #region 各種ボタンに対するイベント紐づけ
            tabScore.SelectedIndexChanged += (s, e) =>
            {
                Text = tabScore.SelectedTab.Text;
                Text += " - M4ple Editor";
                ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
                selectedPanel.OperationManager.OnStatusChanged();
            };
            #region ToolStripButton
            tsbAdd.Click += (s, e) => SetMode(Mode.ADD);
            tsbEdit.Click += (s, e) => SetMode(Mode.EDIT);
            tsbDelete.Click += (s, e) => SetMode(Mode.DELETE);
			tsbInvisibleSlideTap.Click += TbsInvisibleSlideTap_Click;
			tscbBeat.SelectedIndexChanged += (s, e) => { Status.Beat = int.Parse(tscbBeat.Text); };
            tscbGrid.SelectedIndexChanged += (s, e) => { Status.Grid = int.Parse(tscbGrid.Text); };
            tsbNew.Click += New_Click;
            tsbOpen.Click += Open_Click;
            tsbSave.Click += Save_Click;
            tsbImport.Click += (s, e) =>
            {
                // NOTE: tsmiImport.Clickも同じ
                ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
                selectedPanel.Import();
            };
            tsbExport.Click += (s, e) =>
            {
                ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
                selectedPanel.Export();
            };
            tsbCopy.Click += Copy_Click;
            tsbCut.Click += Cut_Click;
            tsbPaste.Click += Paste_Click;
            tsbUndo.Click += Undo_Click;
            tsbRedo.Click += Redo_Click;
            tsbUndo.Enabled = tsbRedo.Enabled = false;
            #endregion
            #region ToolStripMenuItem(表示)
            tsmiIsShortNote.Click += (s, e) =>
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)s;
                Status.IsShortNoteVisible = menuItem.Checked = !menuItem.Checked;
                Refresh();
            };
            tsmiIsHold.Click += (s, e) =>
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)s;
                Status.IsHoldVisible = menuItem.Checked = !menuItem.Checked;
                Refresh();
            };
            tsmiIsSlide.Click += (s, e) =>
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)s;
                Status.IsSlideVisible = menuItem.Checked = !menuItem.Checked;
                Refresh();
            };
            tsmiIsSlideRelay.Click += (s, e) => 
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)s;
                Status.IsSlideRelayVisible = menuItem.Checked = !menuItem.Checked;
                Refresh();
            };
            tsmiIsSlideCurve.Click += (s, e) =>
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)s;
                Status.IsSlideCurveVisible = menuItem.Checked = !menuItem.Checked;
                Refresh();
            };
            tsmiIsAirHold.Click += (s, e) =>
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)s;
                Status.IsAirHoldVisible = menuItem.Checked = !menuItem.Checked;
                Refresh();
            };
            tsmiIsAir.Click += (s, e) =>
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)s;
                Status.IsAirVisible = menuItem.Checked = !menuItem.Checked;
                Refresh();
            };
            tsmiIsExTapDistinct.Click += (s, e) =>
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)s;
                Status.IsExTapDistinct = menuItem.Checked = !menuItem.Checked;
                Refresh();
            };
            tsmiIsEconomyMode.Click += (s, e) =>
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)s;
                Status.IsEconomyMode = menuItem.Checked = !menuItem.Checked;
                Refresh();
            };
            #endregion
            #region ToolStlipMenuItem(ファイル)
            tsmiNew.Click += New_Click;
            tsmiOpen.Click += Open_Click;
            tsmiSave.Click += Save_Click;
            tsmiSaveAs.Click += (s, e) =>
            {
                ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
                bool isSaved = selectedPanel.SaveAs();
                UpdateTextOfTabAndForm(!isSaved);
            };
            tsmiImport.Click += (s, e) =>
            {
                // NOTE: tsbImportも同じ
                ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
                selectedPanel.Import();
            };
            tsmiExport.Click += (s, e) =>
            {
                ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
                selectedPanel.ExportAs();
            };
            tsmiQuit.Click += (s, e) =>
            {
                Close();
            };
            FormClosing += (s, e) =>
            {
                e.Cancel = !IsExit();
            };
            #endregion
            #region ToolStripMenuItem(編集)
            tsmiUndo.Click += Undo_Click;
            tsmiRedo.Click += Redo_Click;
            tsmiUndo.Enabled = tsmiRedo.Enabled = false;
            tsmiCopy.Click += Copy_Click;
            tsmiCut.Click += Cut_Click;
            tsmiPaste.Click += Paste_Click;
            tsmiPasteReverse.Click += (s, e) =>
            {
                ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
                selectedPanel.PasteNotes();
                selectedPanel.ReverseNotes();
                Refresh();
            };
            tsmiDelete.Click += (s, e) =>
            {
                ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
                selectedPanel.ClearAreaNotes();
                Refresh();
            };
            tsmiReverse.Click += (s, e) =>
            {
                ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
                selectedPanel.ReverseNotes();
                Refresh();
            };
            #endregion
            #region ToolStripMenuItem(ウィンドウ)
            tsmiSizeSmall.Click += (s, e) => SetPanelSize(PanelSize.SMALL);
            tsmiSizeMiddle.Click += (s, e) => SetPanelSize(PanelSize.MIDDLE);
            tsmiSizeBig.Click += (s, e) => SetPanelSize(PanelSize.BIG);
            #endregion
            #region ToolStripMenuItem(ヘルプ)
            tsmiShowHelp.Click += (s, e) =>
            {
                System.Diagnostics.Process.Start("https://github.com/TinyTany/M4ple/wiki");
            };
            tsmiVersion.Click += (s, e) =>
            {
                new VersionInfoForm().ShowDialog();
            };
            #endregion
            #endregion
            #region ショートカットキー
            KeyDown += (s, e) =>
            {
                if (ModifierKeys == Keys.Alt) return;
                switch (e.KeyCode)
                {
                    case Keys.A:
                        tsbAdd.PerformClick();
                        break;
                    case Keys.E:
                        tsbEdit.PerformClick();
                        break;
                    case Keys.D:
                        tsbDelete.PerformClick();
                        break;
                    case Keys.S:
                        tsbInvisibleSlideTap.PerformClick();
                        break;
                    case Keys.B:
                        if (ModifierKeys == Keys.Shift)
                        {
                            tscbBeat.SelectedIndex = 
                                tscbBeat.SelectedIndex == 0 ? tscbBeat.Items.Count - 1 : --tscbBeat.SelectedIndex;
                        }
                        else
                        {
                            tscbBeat.SelectedIndex = (tscbBeat.SelectedIndex + 1) % tscbBeat.Items.Count;
                        }
                        break;
                    case Keys.G:
                        if (ModifierKeys == Keys.Shift)
                        {
                            tscbGrid.SelectedIndex =
                                tscbGrid.SelectedIndex == 0 ? tscbGrid.Items.Count - 1 : --tscbGrid.SelectedIndex;
                        }
                        else
                        {
                            tscbGrid.SelectedIndex = (tscbGrid.SelectedIndex + 1) % tscbGrid.Items.Count;
                        }
                        break;
                    case Keys.OemPeriod:
                        noteButtonManager.SelectedButtonIncrease();
                        break;
                    case Keys.Oemcomma:
                        noteButtonManager.SelectedButtonDecrease();
                        break;
                }
            };
            #endregion
            Resize += (s, e) =>
            {
                // HACK: 16と42はマジックナンバー的なもの（なぜかこの値で調整しないと大きさが合わない）
                tabScore.Size = new Size(
                    Width - tabScore.Location.X - tabScore.Margin.Right - 16, 
                    Height - tabScore.Location.Y - tabScore.Margin.Bottom - 42);
                foreach(TabPageEx tabPageEx in tabScore.TabPages)
                {
                    tabPageEx.ScorePanel.ReSizePanel(tabScore.SelectedTab.Size);
                }
                tabNoteButton.Height = tabScore.Height;
                flpNotePanel.Height = tabNoteButton.TabPages[0].Height;
            };
            SetPanelSize(PanelSize.BIG);
            //
            /*
            var data = AppDomain.CurrentDomain?.SetupInformation?.ActivationArguments?.ActivationData?[0];
            if(data != null)
            {

            }
            //*/
        }

        private bool IsExit()
        {
            foreach(TabPageEx tabPageEx in tabScore.TabPages)
            {
                if (tabPageEx.ScorePanel.IsEdited)
                {
                    DialogResult dialogResult =
                    MessageBox.Show(
                        "変更されているファイルがあります。本当に終了しますか？",
                        "終了",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    if (dialogResult == DialogResult.Yes)
                    {
                        return true;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// valueの値を考慮して、タブのテキストとフォームのテキストを更新します
        /// </summary>
        /// <param name="isEditedWithoutSave"></param>
        private void UpdateTextOfTabAndForm(bool isEditedWithoutSave)
        {
            ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            if (selectedPanel.FileName != null)
            {
                tabScore.SelectedTab.Text = selectedPanel.FileName;
            }
            else
            {
                int tabIndex = tabScore.SelectedIndex;
                tabScore.SelectedTab.Text = "NewScore" + (tabIndex + 1);
            }
            if (isEditedWithoutSave)
            {
                tabScore.SelectedTab.Text += "*";
            }
            Text = tabScore.SelectedTab.Text;
            Text += " - M4ple Editor";
        }

        private void New_Click(object sender, EventArgs e)
        {
            ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            if (selectedPanel.IsEdited)
            {
                DialogResult dialogResult =
                    MessageBox.Show(
                        "ファイルは変更されています。保存しますか？",
                        "新規作成",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                if (dialogResult == DialogResult.Yes)
                {
                    selectedPanel.Save();
                }
                else if (dialogResult == DialogResult.No)
                {
                    SetNewPanel();
                }
                else if (dialogResult == DialogResult.Cancel) { }
            }
            else
            {
                SetNewPanel();
            }
        }

        private void SetNewPanel()
        {
            // 新規パネル生成処理
            if (!(tabScore.SelectedTab.Controls[0] is PictureBox pBox)) return;
            if (!(pBox.Controls[0] is HScrollBar hScrollBar)) return;
            if (!(pBox.Controls[1] is VScrollBar vScrollBar)) return;
            ScorePanel newPanel = new ScorePanel(pBox, hScrollBar, vScrollBar);
            if (new NewScoreForm(newPanel).ShowDialog() == DialogResult.OK)
            {
                (tabScore.SelectedTab as TabPageEx).ScorePanel = newPanel;
                newPanel.OperationManager.StatusChanged += (undo, redo) =>
                {
                    tsbUndo.Enabled = tsmiUndo.Enabled = undo;
                    tsbRedo.Enabled = tsmiRedo.Enabled = redo;
                };
                tsbUndo.Enabled = tsbRedo.Enabled = false;
                tsmiUndo.Enabled = tsmiRedo.Enabled = false;
                newPanel.OperationManager.Edited += () =>
                {
                    UpdateTextOfTabAndForm(true);
                    newPanel.IsEdited = true;
                };
                // タブ名をデフォルトにする
                int tabIndex = tabScore.SelectedIndex;
                tabScore.SelectedTab.Text = "NewScore" + (tabIndex + 1);
                //
                tabScore.SelectedTab.Controls[0].Refresh();
            }
        }

        private void Open_Click(object sender, EventArgs e)
        {
            //現在開かれているタブを判別してそれにたいしてロードするようにする
            ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            if (selectedPanel.Load())
            {
                UpdateTextOfTabAndForm(false);
                tabScore.SelectedTab.Controls[0].Refresh();
            }
            return;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            //現在開かれているタブを判別してそれを対象にセーブするようにする
            bool isSaved = (tabScore.SelectedTab as TabPageEx).ScorePanel.Save();
            UpdateTextOfTabAndForm(!isSaved);
            return;
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            selectedPanel.CopyNotes();
            selectedPanel.Refresh();
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            selectedPanel.CutNotes();
            selectedPanel.Refresh();
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            selectedPanel.PasteNotes();
            selectedPanel.Refresh();
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            selectedPanel.Undo();
            selectedPanel.Refresh();
        }

        private void Redo_Click(object sender, EventArgs e)
        {
            ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            selectedPanel.Redo();
            selectedPanel.Refresh();
        }

        #region イベント渡し
        // 現在開かれているタブに対して行う

        private void Score_MouseUp(object sender, MouseEventArgs e)
        {
            (tabScore.SelectedTab as TabPageEx).ScorePanel.MouseUp(e);
            tabScore.SelectedTab.Controls[0].Refresh();
        }

        private void Score_MouseMove(object sender, MouseEventArgs e)
        {
            (tabScore.SelectedTab as TabPageEx).ScorePanel.MouseMove(e);
            tabScore.SelectedTab.Controls[0].Refresh();
        }

        private void Score_MouseDown(object sender, MouseEventArgs e)
        {
            (tabScore.SelectedTab as TabPageEx).ScorePanel.MouseDown(e);
            tabScore.SelectedTab.Controls[0].Refresh();
        }

        private void Score_MouseEnter(object sender, EventArgs e)
        {
            (tabScore.SelectedTab as TabPageEx).ScorePanel.MouseEnter(e);
            tabScore.SelectedTab.Controls[0].Refresh();
        }

        private void Score_MouseClick(object sender, MouseEventArgs e)
        {
            (tabScore.SelectedTab as TabPageEx).ScorePanel.MouseClick(e);
            tabScore.SelectedTab.Controls[0].Refresh();
        }

        private void Score_MouseWheel(object sender, MouseEventArgs e)
        {
            (tabScore.SelectedTab as TabPageEx).ScorePanel.MouseScroll(e.Delta);
            tabScore.SelectedTab.Controls[0].Refresh();
        }

        private void Score_Paint(object sender, PaintEventArgs e)
        {
            if (!Status.IsEconomyMode)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            }
            (tabScore.SelectedTab as TabPageEx).ScorePanel.PaintPanel(e);
        }

        private void Score_Scroll(object sender, ScrollEventArgs e)
        {
            ScorePanel selected = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            if (sender is HScrollBar)
            {
                selected.HSBarScroll(e);
            }
            else if (sender is VScrollBar)
            {
                selected.VSBarScroll(e);
            }
            tabScore.SelectedTab.Controls[0].Refresh();
        }
        #endregion

        private void InitializeToolStrip()
		{
			tscbBeat.SelectedIndex = tscbBeat.Items.IndexOf(Status.Beat.ToString());
			tscbGrid.SelectedIndex = tscbGrid.Items.IndexOf(Status.Grid.ToString());
			tsbInvisibleSlideTap.Checked = Status.InvisibleSlideTap;
			switch (Status.Mode)
			{
				case Mode.ADD:
					tsbAdd.Checked = true;
					break;
				case Mode.EDIT:
					tsbEdit.Checked = true;
					break;
				case Mode.DELETE:
					tsbDelete.Checked = true;
					break;
				default:
					break;
			}
		}

        public void SetMode(int mode)
        {
            if (mode != Mode.ADD && mode != Mode.EDIT && mode != Mode.DELETE)
            {
                return;
            }
            Status.Mode = mode;
            switch (mode)
            {
                case Mode.ADD:
                    tsbAdd.Checked = true;
                    tsbEdit.Checked = false;
                    tsbDelete.Checked = false;
                    break;
                case Mode.EDIT:
                    tsbAdd.Checked = false;
                    tsbEdit.Checked = true;
                    tsbDelete.Checked = false;
                    break;
                case Mode.DELETE:
                    tsbAdd.Checked = false;
                    tsbEdit.Checked = false;
                    tsbDelete.Checked = true;
                    break;
                default:
                    break;
            }
            Refresh();
        }

        public void SetPanelSize(int panelSize)
        {
            // HACK: とりあえず決めうちにした
            int[] formHeight = { 558, 750, 940 };
            switch (panelSize)
            {
                case PanelSize.SMALL:
                    ScoreInfo.LaneMaxBar = 1;
                    tsmiSizeSmall.Checked = true;
                    tsmiSizeMiddle.Checked = false;
                    tsmiSizeBig.Checked = false;
                    Height = formHeight[0];
                    break;
                case PanelSize.MIDDLE:
                    ScoreInfo.LaneMaxBar = 1.5f;
                    tsmiSizeSmall.Checked = false;
                    tsmiSizeMiddle.Checked = true;
                    tsmiSizeBig.Checked = false;
                    Height = formHeight[1];
                    break;
                case PanelSize.BIG:
                    ScoreInfo.LaneMaxBar = 2;
                    tsmiSizeSmall.Checked = false;
                    tsmiSizeMiddle.Checked = false;
                    tsmiSizeBig.Checked = true;
                    Height = formHeight[2];
                    break;
            }
            ScoreLane.RefreshLaneSize();
            foreach (TabPageEx tabPageEx in tabScore.TabPages)
            {
                tabPageEx.ScorePanel.RefreshLaneSize();
            }
        }

        private void TbsInvisibleSlideTap_Click(object sender, EventArgs e)
		{
			tsbInvisibleSlideTap.Checked = !tsbInvisibleSlideTap.Checked;
			Status.InvisibleSlideTap = tsbInvisibleSlideTap.Checked;
		}
    }
}
