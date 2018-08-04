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
        ScorePanel scorePanel;

        public MainForm()
        {
            InitializeComponent();
            Score1.MouseWheel += new MouseEventHandler(Score_MouseWheel);
            Score1.Paint += new PaintEventHandler(Score_Paint);
            scorePanel = new ScorePanel(Score1.Height);
        }

        private void Score_MouseWheel(object sender, MouseEventArgs e)
        {
            scorePanel.MouseScroll(e.Delta);
            ((TabPage)sender).Refresh();
        }

        private void Score_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.FillRectangle(Brushes.Black, e.ClipRectangle);//背景塗りつぶし
            scorePanel.PaintPanel(e);
        }
    }
}
