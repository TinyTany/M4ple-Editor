using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Component
{
    public class BarAddWithNoteCustomForm : BarAddCustomForm
    {
        public BarAddWithNoteCustomForm(ScorePanel scorePanel, Score score) : base(scorePanel, score)
        {
            Text = "カスタム小節挿入";
        }

        protected override void OK_Click(object sender, EventArgs e)
        {
            if (Direction.SelectedIndex != -1 && BeatNumer.SelectedIndex != -1 && BeatDenom.SelectedIndex != -1)
            {
                switch (Direction.SelectedIndex)
                {
                    //選択小節の1つ先
                    case 0:
                        sPanel.InsertScoreForwardWithNote(score, int.Parse(BeatNumer.Text), int.Parse(BeatDenom.Text), (int)BarCount.Value);
                        break;
                    //選択小節の1つ前
                    case 1:
                        sPanel.InsertScoreBackwardWithNote(score, int.Parse(BeatNumer.Text), int.Parse(BeatDenom.Text), (int)BarCount.Value);
                        break;
                }
                Close();
            }
            else
            {
                MessageBox.Show("未選択箇所があります\n全ての項目に値を指定してください");
            }
        }
    }
}
