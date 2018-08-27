using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Component
{
    public partial class BarDeleteCustomForm : Form
    {
        ScorePanel sPanel;
        Score score;

        public BarDeleteCustomForm(ScorePanel sPanel, Score score)
        {
            InitializeComponent();
            this.sPanel = sPanel;
            this.score = score;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            sPanel.DeleteScore(score, (int)BarCount.Value);
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
