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
        private List<ScorePanel> sPanels;
        private List<PictureBox> pBoxes;
        private List<HScrollBar> hScrolls;


        public MainForm()
        {
            InitializeComponent();
            sPanels = new List<ScorePanel>();
            pBoxes = new List<PictureBox>();
            hScrolls = new List<HScrollBar>();
            for(int i = 0; i < tabScore.TabCount; ++i)
            {
                //PictureBoxの追加と初期化
                pBoxes.Add(new PictureBox());
                pBoxes[i].Size = tabScore.TabPages[i].Size;
                //TabPageに初期化したPictureBoxを入れる
                tabScore.TabPages[i].Controls.Add(pBoxes[i]);
                //初期化したPictureBoxを使用してScorePanelを追加
                sPanels.Add(new ScorePanel(pBoxes[i].Width, pBoxes[i].Height));
                //HScrollBarの追加と初期化
                hScrolls.Add(new HScrollBar());
                hScrolls[i].Size = new Size(pBoxes[i].Width, 17);
                //HScrollBarをPictureBoxに入れる
                pBoxes[i].Controls.Add(hScrolls[i]);
                //HScrollBarの親コントロール内での位置を設定
                hScrolls[i].Dock = DockStyle.Bottom;
                //PictureBoxとHScrollBarの各種デリゲートの設定
                pBoxes[i].MouseWheel += new MouseEventHandler(Score_MouseWheel);
                pBoxes[i].Paint += new PaintEventHandler(Score_Paint);
                hScrolls[i].Scroll += new ScrollEventHandler(Score_Scroll);
            }
        }

        private void Score_MouseWheel(object sender, MouseEventArgs e)
        {
            for(int i = 0; i < tabScore.TabCount; ++i)
            {
                if (((PictureBox)sender).Equals(pBoxes[i]))
                {
                    sPanels[i].MouseScroll(e.Delta);
                    break;
                }
            }
            ((PictureBox)sender).Refresh();
        }

        private void Score_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < tabScore.TabCount; ++i)
            {
                if (((PictureBox)sender).Equals(pBoxes[i]))
                {
                    sPanels[i].PaintPanel(e);
                    break;
                }
            }
        }

        private void Score_Scroll(object sender, ScrollEventArgs e)
        {

        }
    }
}
