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
        private ToolStripItem[] stripItems, barAddItems, barDeleteItems, laneFillItems;
        private readonly ToolStripItem[] barAddWithNoteItems;
        private readonly ToolStripItem[] barDeleteWithNoteItems;
        private ScorePanel sPanel;
        private ScoreLane selectedLane;
        private Score selectedScore;

        public EditCMenu(ScorePanel sPanel, ScoreLane selectedLane, Score selectedScore)
        {
            barAddItems = new ToolStripItem[]
            {
                new ToolStripMenuItem("選択小節の1つ前", null, BarAddBackward),
                new ToolStripMenuItem("選択小節の1つ先", null, BarAddForward),
                new ToolStripMenuItem("カスタム...", null, BarAddCustom)
            };
            barAddWithNoteItems = new ToolStripItem[]
            {
                new ToolStripMenuItem("選択小節の1つ前", null, BarAddBackwardWithNote),
                new ToolStripMenuItem("選択小節の1つ先", null, BarAddForwardWithNote),
                new ToolStripMenuItem("カスタム...", null, BarAddCustomWithNote)
            };
            barDeleteItems = new ToolStripItem[]
            {
                new ToolStripMenuItem("選択小節", null, BarDeleteSelected),
                new ToolStripMenuItem("カスタム...", null, BarDeleteCustom)
            };
            barDeleteWithNoteItems = new ToolStripItem[]
            {
                new ToolStripMenuItem("選択小節", null, BarDeleteSelectedWithNote),
                new ToolStripMenuItem("カスタム...", null, BarDeleteCustomWithNote)
            };
            laneFillItems = new ToolStripItem[]
            {
                new ToolStripMenuItem("レーン全体", null, LaneFillAll),
                new ToolStripMenuItem("選択レーン以降", null, LaneFill)
            };
            ToolStripMenuItem barAdd = new ToolStripMenuItem("小節を挿入", null);
            ToolStripMenuItem barAddWithNote = new ToolStripMenuItem("小節を挿入", null)
            {
                ToolTipText = "選択した小節の前後に新しい小節を追加します\nすでに配置されているノーツの相対座標は変更されます"
            };
            barAddWithNote.DropDownItems.AddRange(barAddWithNoteItems);
            barAdd.DropDownItems.AddRange(barAddItems);
            ToolStripMenuItem barDelete = new ToolStripMenuItem("小節を削除", null);
            ToolStripMenuItem barDeleteWithNote = new ToolStripMenuItem("小節を削除", null)
            {
                ToolTipText = "選択した小節またはそれ以降の複数の小節を削除します\n削除対象の小節にノーツが配置されている場合、ノーツも削除されます"
            };
            barDeleteWithNote.DropDownItems.AddRange(barDeleteWithNoteItems);
            barDelete.DropDownItems.AddRange(barDeleteItems);
            ToolStripMenuItem laneFill = new ToolStripMenuItem("レーンを詰める", null);
            laneFill.DropDownItems.AddRange(laneFillItems);
            stripItems = new ToolStripMenuItem[]
            {
                barAddWithNote,
                barAdd,
                barDeleteWithNote,
                barDelete,
                new ToolStripMenuItem("選択小節を改行", null, BarDivide),
                laneFill,
                new ToolStripMenuItem("貼り付け", null, Paste)
            };
            Items.AddRange(stripItems);
            Items.Insert(Items.Count - 1, new ToolStripSeparator());
            this.sPanel = sPanel;
            this.selectedLane = selectedLane;
            this.selectedScore = selectedScore;
        }

        private void LaneFill(object sender, EventArgs e)
        {
            sPanel.FillLane(selectedLane);
        }

        private void LaneFillAll(object sender, EventArgs e)
        {
            sPanel.FillLane();
        }

        private void BarDivide(object sender, EventArgs e)
        {
            sPanel.DivideLane(selectedScore);
        }

        private void Paste(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BarDeleteSelected(object sender, EventArgs e)
        {
            sPanel.DeleteScore(selectedScore);
        }

        private void BarDeleteSelectedWithNote(object sender, EventArgs e)
        {
            sPanel.DeleteScoreWithNote(selectedScore);
        }

        private void BarDeleteCustom(object sender, EventArgs e)
        {
            new BarDeleteCustomForm(sPanel, selectedScore).ShowDialog();
        }

        private void BarDeleteCustomWithNote(object sender, EventArgs e)
        {
            new BarDeleteWithNoteCustomForm(sPanel, selectedScore).ShowDialog();
        }

        private void BarAddCustom(object sender, EventArgs e)
        {
            new BarAddCustomForm(sPanel, selectedScore).ShowDialog();
        }

        private void BarAddCustomWithNote(object sender, EventArgs e)
        {
            new BarAddWithNoteCustomForm(sPanel, selectedScore).ShowDialog();
        }

        private void BarAddForward(object sender, EventArgs e)
        {
            sPanel.InsertScoreForward(selectedScore, selectedScore.BeatNumer, selectedScore.BeatDenom, 1);
        }

        private void BarAddBackward(object sender, EventArgs e)
        {
            sPanel.InsertScoreBackward(selectedScore, selectedScore.BeatNumer, selectedScore.BeatDenom, 1);
        }

        private void BarAddForwardWithNote(object sender, EventArgs e)
        {
            sPanel.InsertScoreForwardWithNote(selectedScore, selectedScore.BeatNumer, selectedScore.BeatDenom, 1);
        }

        private void BarAddBackwardWithNote(object sender, EventArgs e)
        {
            sPanel.InsertScoreBackwardWithNote(selectedScore, selectedScore.BeatNumer, selectedScore.BeatDenom, 1);
        }
    }
}
