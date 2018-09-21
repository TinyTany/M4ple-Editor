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
                pBox.MouseWheel += new MouseEventHandler(Score_MouseWheel);
                pBox.Paint += new PaintEventHandler(Score_Paint);
                pBox.MouseClick += new MouseEventHandler(Score_MouseClick);
                pBox.MouseEnter += new EventHandler(Score_MouseEnter);
                pBox.MouseDown += new MouseEventHandler(Score_MouseDown);
                pBox.MouseMove += new MouseEventHandler(Score_MouseMove);
                pBox.MouseUp += new MouseEventHandler(Score_MouseUp);
                hScroll.Scroll += new ScrollEventHandler(Score_Scroll);
                //初期化した部品たちをタプルにしてリストに追加
                viewComponentList.Add(new Tuple<ScorePanel, PictureBox, HScrollBar>(sPanel, pBox, hScroll));
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
    }
}
