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
        private List<Tuple<ScorePanel, PictureBox, HScrollBar>> viewComponentList;

        public MainForm()
        {
            InitializeComponent();
            viewComponentList = new List<Tuple<ScorePanel, PictureBox, HScrollBar>>();
            for(int i = 0; i < tabScore.TabCount; ++i)
            {
                //PictureBoxの追加と初期化
                PictureBox pBox = new PictureBox();
                pBox.Size = tabScore.TabPages[i].Size;
                //TabPageに初期化したPictureBoxを入れる
                tabScore.TabPages[i].Controls.Add(pBox);
                //HScrollBarの追加と初期化
                HScrollBar hScroll = new HScrollBar();
                hScroll.Size = new Size(pBox.Width, 17);
                //HScrollBarをPictureBoxに入れる
                pBox.Controls.Add(hScroll);
                //HScrollBarの親コントロール内での位置を設定
                hScroll.Dock = DockStyle.Bottom;
                //初期化したPictureBoxとHScrollBarを使用してScorePanelを追加
                ScorePanel sPanel = new ScorePanel(pBox, hScroll);
                //PictureBoxとHScrollBarの各種デリゲートの設定
                pBox.MouseWheel += Score_MouseWheel;
                pBox.Paint += Score_Paint;
                pBox.MouseClick += Score_MouseClick;
                pBox.MouseEnter += Score_MouseEnter;
                pBox.MouseDown += Score_MouseDown;
                pBox.MouseMove += Score_MouseMove;
                pBox.MouseUp += Score_MouseUp;
                hScroll.Scroll += Score_Scroll;
                //初期化した部品たちをタプルにしてリストに追加
                viewComponentList.Add(new Tuple<ScorePanel, PictureBox, HScrollBar>(sPanel, pBox, hScroll));
            }
			InitializeToolStrip();
			tsbAdd.Click += tbsAdd_Click;
			tsbEdit.Click += tbsEdit_Click;
			tsbDelete.Click += tbsDelete_Click;
			tsbInvisibleSlideTap.Click += tbsInvisibleSlideTap_Click;
			tscbBeat.SelectedIndexChanged += tscbBeat_SelectedIndexChanged;
			tscbGrid.SelectedIndexChanged += tscbGrid_SelectedIndexChanged;
            //
            NoteButtonManager noteButtonManager = new NoteButtonManager();
            foreach (NoteButton noteButton in noteButtonManager)
            {
                flpNotePanel.Controls.Add(noteButton);
            }
        }

		private void Score_MouseUp(object sender, MouseEventArgs e)
        {
            //クリックされたPictureBoxに対応するScorePanelで処理
            Tuple<ScorePanel, PictureBox, HScrollBar> selectedComponent =
                viewComponentList.Find(x => x.Item2.Equals((PictureBox)sender));
            selectedComponent.Item1.MouseUp(e);
            selectedComponent.Item2.Refresh();
        }

        private void Score_MouseMove(object sender, MouseEventArgs e)
        {
            //クリックされたPictureBoxに対応するScorePanelで処理
            Tuple<ScorePanel, PictureBox, HScrollBar> selectedComponent =
                viewComponentList.Find(x => x.Item2.Equals((PictureBox)sender));
            selectedComponent.Item1.MouseMove(e);
            selectedComponent.Item2.Refresh();
        }

        private void Score_MouseDown(object sender, MouseEventArgs e)
        {
            //クリックされたPictureBoxに対応するScorePanelで処理
            Tuple<ScorePanel, PictureBox, HScrollBar> selectedComponent =
                viewComponentList.Find(x => x.Item2.Equals((PictureBox)sender));
            selectedComponent.Item1.MouseDown(e);
            selectedComponent.Item2.Refresh();
        }

        private void Score_MouseEnter(object sender, EventArgs e)
        {
            //クリックされたPictureBoxに対応するScorePanelで処理
            Tuple<ScorePanel, PictureBox, HScrollBar> selectedComponent =
                viewComponentList.Find(x => x.Item2.Equals((PictureBox)sender));
            selectedComponent.Item1.MouseEnter(e);
            selectedComponent.Item2.Refresh();
        }

        private void Score_MouseClick(object sender, MouseEventArgs e)
        {
            //クリックされたPictureBoxに対応するScorePanelで処理
            Tuple<ScorePanel, PictureBox, HScrollBar> selectedComponent =
                viewComponentList.Find(x => x.Item2.Equals((PictureBox)sender));
            selectedComponent.Item1.MouseClick(e);
            selectedComponent.Item2.Refresh();
        }

        private void Score_MouseWheel(object sender, MouseEventArgs e)
        {
            //クリックされたPictureBoxに対応するScorePanelで処理
            Tuple<ScorePanel, PictureBox, HScrollBar> selectedComponent =
                viewComponentList.Find(x => x.Item2.Equals((PictureBox)sender));
            selectedComponent.Item1.MouseScroll(e.Delta);
            selectedComponent.Item2.Refresh();
        }

        private void Score_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            viewComponentList.Find(x => x.Item2.Equals((PictureBox)sender)).Item1.PaintPanel(e);
        }

        private void Score_Scroll(object sender, ScrollEventArgs e)
        {
            //クリックされたHScrollBarに対応するScorePanelで処理
            Tuple<ScorePanel, PictureBox, HScrollBar> selectedComponent =
                viewComponentList.Find(x => x.Item3.Equals((HScrollBar)sender));
            selectedComponent.Item1.HSBarScroll(e);
            selectedComponent.Item2.Refresh();
        }

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

		private void tbsAdd_Click(object sender, EventArgs e)
		{
			tsbAdd.Checked = true;
			tsbEdit.Checked = false;
			tsbDelete.Checked = false;
			Status.Mode = Mode.ADD;
		}

		private void tbsEdit_Click(object sender, EventArgs e)
		{
			tsbAdd.Checked = false;
			tsbEdit.Checked = true;
			tsbDelete.Checked = false;
			Status.Mode = Mode.EDIT;
		}

		private void tbsDelete_Click(object sender, EventArgs e)
		{
			tsbAdd.Checked = false;
			tsbEdit.Checked = false;
			tsbDelete.Checked = true;
			Status.Mode = Mode.DELETE;
		}

		private void tbsInvisibleSlideTap_Click(object sender, EventArgs e)
		{
			tsbInvisibleSlideTap.Checked = !tsbInvisibleSlideTap.Checked;
			Status.InvisibleSlideTap = tsbInvisibleSlideTap.Checked;
		}

		private void tscbBeat_SelectedIndexChanged(object sender, EventArgs e)
		{
			Status.Beat = int.Parse(tscbBeat.Text);
		}

		private void tscbGrid_SelectedIndexChanged(object sender, EventArgs e)
		{
			Status.Grid = int.Parse(tscbGrid.Text);
		}
	}
}
