using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Component
{
    public class EditCMenu : ContextMenuStrip
    {
        private ToolStripItem[] stripItems, barAddItems, barDeleteItems;
        private ScorePanel sPanel;
        private ScoreLane selectedLane;
        private Score selectedScore;

        public EditCMenu(ScorePanel sPanel, ScoreLane selectedLane, Score selectedScore)
        {
            barAddItems = new ToolStripItem[]
            {
                new ToolStripMenuItem("選択小節の直前", null, new EventHandler(BarAddBackward)),
                new ToolStripMenuItem("選択小節の直後", null, new EventHandler(BarAddForward)),
                new ToolStripMenuItem("カスタム...", null, new EventHandler(BarAddCustom))
            };
            barDeleteItems = new ToolStripItem[]
            {
                new ToolStripMenuItem("選択小節", null, new EventHandler(BarDeleteSelected)),
                new ToolStripMenuItem("カスタム...", null, new EventHandler(BarDeleteCustom))
            };
            ToolStripMenuItem barAdd = new ToolStripMenuItem("小節を挿入", null);
            barAdd.DropDownItems.AddRange(barAddItems);
            ToolStripMenuItem barDelete = new ToolStripMenuItem("小節を削除", null);
            barDelete.DropDownItems.AddRange(barDeleteItems);
            stripItems = new ToolStripMenuItem[]
            {
                barAdd,
                barDelete,
                new ToolStripMenuItem("小節を編集...", null, new EventHandler(BarEdit)),
                new ToolStripMenuItem("貼り付け", null, new EventHandler(Paste))
            };
            Items.AddRange(stripItems);
            Items.Insert(Items.Count - 1, new ToolStripSeparator());
            this.sPanel = sPanel;
            this.selectedLane = selectedLane;
            this.selectedScore = selectedScore;
        }

        private void BarEdit(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Paste(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BarDeleteSelected(object sender, EventArgs e)
        {
            sPanel.DeleteScore(selectedLane, selectedScore);
        }

        private void BarDeleteCustom(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BarAddCustom(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BarAddForward(object sender, EventArgs e)
        {
            sPanel.InsertScoreForward(selectedScore.ScoreIndex, selectedScore.BeatNumer, selectedScore.BeatDenom, 1);
        }

        private void BarAddBackward(object sender, EventArgs e)
        {
            sPanel.InsertScoreBackward(selectedScore.ScoreIndex, selectedScore.BeatNumer, selectedScore.BeatDenom, 1);
        }
    }
}
