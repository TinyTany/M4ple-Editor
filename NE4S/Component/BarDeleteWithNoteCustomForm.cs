using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Component
{
    public class BarDeleteWithNoteCustomForm : BarDeleteCustomForm
    {
        public BarDeleteWithNoteCustomForm(ScorePanel scorePanel, Score score) : base(scorePanel, score)
        {
            Text = "カスタム小節削除";
        }

        protected override void OK_Click(object sender, EventArgs e)
        {
            sPanel.DeleteScoreWithNote(score, (int)BarCount.Value);
            Close();
        }
    }
}
