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
            }
			InitializeToolStrip();
			tsbAdd.Click += TbsAdd_Click;
			tsbEdit.Click += TbsEdit_Click;
			tsbDelete.Click += TbsDelete_Click;
			tsbInvisibleSlideTap.Click += TbsInvisibleSlideTap_Click;
			tscbBeat.SelectedIndexChanged += (s, e) => { Status.Beat = int.Parse(tscbBeat.Text); };
            tscbGrid.SelectedIndexChanged += (s, e) => { Status.Grid = int.Parse(tscbGrid.Text); };
            tsbOpen.Click += TsbOpen_Click;
            tsbSave.Click += TsbSave_Click;
            //
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
            //ノーツボタンを追加
            NoteButtonManager noteButtonManager = new NoteButtonManager();
            foreach (NoteButton noteButton in noteButtonManager)
            {
                flpNotePanel.Controls.Add(noteButton);
            }
        }

        private void TsbOpen_Click(object sender, EventArgs e)
        {
            DataLoader dataLoader = new DataLoader();
            Model model = dataLoader.ShowDialog();
            if (model == null)
            {
                return;
            }
            //現在開かれているタブを判別してそれにたいしてロードするようにする
            (tabScore.SelectedTab as TabPageEx).ScorePanel.SetModelForIO(model);
            tabScore.SelectedTab.Controls[0].Refresh();
            return;
        }

        private void TsbSave_Click(object sender, EventArgs e)
        {
            //現在開かれているタブを判別してそれを対象にセーブするようにする
            Model model = (tabScore.SelectedTab as TabPageEx).ScorePanel.GetModelForIO();
            DataSaver dataSaver = new DataSaver();
            dataSaver.ShowDialog(model);
            return;
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

        #region 汚い...汚くない？
        private void TbsAdd_Click(object sender, EventArgs e)
		{
			tsbAdd.Checked = true;
			tsbEdit.Checked = false;
			tsbDelete.Checked = false;
			Status.Mode = Mode.ADD;
		}

		private void TbsEdit_Click(object sender, EventArgs e)
		{
			tsbAdd.Checked = false;
			tsbEdit.Checked = true;
			tsbDelete.Checked = false;
			Status.Mode = Mode.EDIT;
		}

		private void TbsDelete_Click(object sender, EventArgs e)
		{
			tsbAdd.Checked = false;
			tsbEdit.Checked = false;
			tsbDelete.Checked = true;
			Status.Mode = Mode.DELETE;
		}
        #endregion

        private void TbsInvisibleSlideTap_Click(object sender, EventArgs e)
		{
			tsbInvisibleSlideTap.Checked = !tsbInvisibleSlideTap.Checked;
			Status.InvisibleSlideTap = tsbInvisibleSlideTap.Checked;
		}
    }
}
