﻿using NE4S.Define;
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
                //PictureBoxの追加と初期化
                PictureBox pBox = new PictureBox
                {
                    Size = tabScore.TabPages[i].Size
                };
                //TabPageに初期化したPictureBoxを入れる
                tabScore.TabPages[i].Controls.Add(pBox);
                //HScrollBarの追加と初期化
                HScrollBar hScroll = new HScrollBar
                {
                    Size = new Size(pBox.Width, 17)
                };
                //HScrollBarをPictureBoxに入れる
                pBox.Controls.Add(hScroll);
                //HScrollBarの親コントロール内での位置を設定
                hScroll.Dock = DockStyle.Bottom;
                //初期化したPictureBoxとHScrollBarを使用してScorePanelを追加
                ScorePanel sPanel = new ScorePanel(pBox, hScroll);
                sPanel.SetScore(4, 4, 200);
                tabPageEx.ScorePanel = sPanel;
                //PictureBoxとHScrollBarの各種デリゲートの設定
                pBox.MouseWheel += Score_MouseWheel;
                pBox.Paint += Score_Paint;
                pBox.MouseClick += Score_MouseClick;
                pBox.MouseEnter += Score_MouseEnter;
                pBox.MouseDown += Score_MouseDown;
                pBox.MouseMove += Score_MouseMove;
                pBox.MouseUp += Score_MouseUp;
                hScroll.Scroll += Score_Scroll;
                //
                sPanel.SetEventForEditedWithoutSave(UpdateTextOfTabAndForm);
            }
			InitializeToolStrip();
            #region 各種ボタンに対するイベント紐づけ
            tabScore.SelectedIndexChanged += (s, e) =>
            {
                Text = tabScore.SelectedTab.Text;
                Text += " - M4ple Editor";
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
            tsbExport.Click += (s, e) =>
            {
                ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
                selectedPanel.Export();
            };
            tsbCopy.Click += Copy_Click;
            tsbCut.Click += Cut_Click;
            tsbPaste.Click += Paste_Click;
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
                }
            };
            #endregion
            //ノーツボタンを追加
            NoteButtonManager noteButtonManager = new NoteButtonManager();
            noteButtonManager.ButtonClicked += (s, e) => SetMode(Mode.ADD);
            foreach (NoteButton noteButton in noteButtonManager)
            {
                flpNotePanel.Controls.Add(noteButton);
            }
            
        }

        private bool IsExit()
        {
            foreach(TabPageEx tabPageEx in tabScore.TabPages)
            {
                if (tabPageEx.ScorePanel.IsEditedWithoutSave)
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
            tabScore.SelectedTab.Refresh();
        }

        private void New_Click(object sender, EventArgs e)
        {
            ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            if (selectedPanel.IsEditedWithoutSave)
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
            //新規パネル生成処理
            if (!(tabScore.SelectedTab.Controls[0] is PictureBox pBox)) return;
            if (!(pBox.Controls[0] is HScrollBar hScrollBar)) return;
            ScorePanel newPanel = new ScorePanel(pBox, hScrollBar);
            if (new NewScoreForm(newPanel).ShowDialog() == DialogResult.OK)
            {
                (tabScore.SelectedTab as TabPageEx).ScorePanel = newPanel;
                newPanel.SetEventForEditedWithoutSave(UpdateTextOfTabAndForm);
                //タブ名をデフォルトにする
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
                selectedPanel.SetEventForEditedWithoutSave(UpdateTextOfTabAndForm);
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
            Refresh();
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            selectedPanel.CutNotes();
            Refresh();
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            ScorePanel selectedPanel = (tabScore.SelectedTab as TabPageEx).ScorePanel;
            selectedPanel.PasteNotes();
            Refresh();
        }

        #region イベント渡し
        //現在開かれているタブに対して行う

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
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            (tabScore.SelectedTab as TabPageEx).ScorePanel.PaintPanel(e);
        }

        private void Score_Scroll(object sender, ScrollEventArgs e)
        {
            (tabScore.SelectedTab as TabPageEx).ScorePanel.HSBarScroll(e);
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

        private void TbsInvisibleSlideTap_Click(object sender, EventArgs e)
		{
			tsbInvisibleSlideTap.Checked = !tsbInvisibleSlideTap.Checked;
			Status.InvisibleSlideTap = tsbInvisibleSlideTap.Checked;
		}
    }
}
