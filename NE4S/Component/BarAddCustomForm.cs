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
    public partial class BarAddCustomForm : Form
    {
        private ScorePanel sPanel;
        private Score score;

        public BarAddCustomForm(ScorePanel sPanel, Score score)
        {
            InitializeComponent();
            this.sPanel = sPanel;
            this.score = score;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if(Direction.SelectedIndex != -1 && BeatNumer.SelectedIndex != -1 && BeatDenom.SelectedIndex != -1)
            {
                switch (Direction.SelectedIndex)
                {
                    //選択小節の1つ先
                    case 0:
                        sPanel.InsertScoreForward(score, int.Parse(BeatNumer.Text), int.Parse(BeatDenom.Text), (int)BarCount.Value);
                        break;
                    //選択小節の1つ前
                    case 1:
                        sPanel.InsertScoreBackward(score, int.Parse(BeatNumer.Text), int.Parse(BeatDenom.Text), (int)BarCount.Value);
                        break;
                }
                Close();
            }
            else
            {
                MessageBox.Show("未選択箇所があります\n全ての項目に値を指定してください");
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
