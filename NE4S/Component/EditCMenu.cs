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
        private readonly ToolStripItem[] stripItems;
        private readonly ToolStripItem[] barAddItems;
        private readonly ToolStripItem[] barAddWithNoteItems;
        private readonly ToolStripItem[] barDeleteItems;
        private readonly ToolStripItem[] barDeleteWithNoteItems;
        private readonly ToolStripItem[] laneFillItems;
        private readonly ScorePanel sPanel;
        private readonly ScoreLane selectedLane;
        private readonly Score selectedScore;

        public EditCMenu(ScorePanel sPanel, ScoreLane selectedLane, Score selectedScore, Position clickPosition)
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
            ToolStripMenuItem barAddWithNote = new ToolStripMenuItem("小節を挿入", null)
            {
                ToolTipText = "選択した小節の前後に新しい小節を追加します\nすでに配置されているノーツの相対座標は変更されます"
            };
            barAddWithNote.DropDownItems.AddRange(barAddWithNoteItems);
            ToolStripMenuItem barAdd = new ToolStripMenuItem("小節を挿入（小節のみ）", null)
            {
                ToolTipText = "選択した小節の前後に新しい小節を追加します\nすでに配置されているノーツの相対座標は変更されません"
            };
            barAdd.DropDownItems.AddRange(barAddItems);
            ToolStripMenuItem barDeleteWithNote = new ToolStripMenuItem("小節を削除", null)
            {
                ToolTipText = "選択した小節またはそれ以降の複数の小節を削除します\n削除対象の小節にノーツが配置されている場合、ノーツも削除されます"
            };
            barDeleteWithNote.DropDownItems.AddRange(barDeleteWithNoteItems);
            ToolStripMenuItem barDelete = new ToolStripMenuItem("小節を削除（小節のみ）", null)
            {
                ToolTipText = "選択した小節またはそれ以降の複数の小節を削除します\n小節のみ削除するため、ノーツは削除されません"
            };
            barDelete.DropDownItems.AddRange(barDeleteItems);
            ToolStripMenuItem laneFill = new ToolStripMenuItem("レーンを詰める", null);
            laneFill.DropDownItems.AddRange(laneFillItems);
            stripItems = new ToolStripItem[]
            {
                barAddWithNote,
                barAdd,
                barDeleteWithNote,
                barDelete,
                new ToolStripMenuItem("選択小節を改行", null, BarDivide),
                laneFill,
                new ToolStripSeparator(),
                new ToolStripMenuItem("貼り付け", null, (s, e) => sPanel.PasteNotes(clickPosition)),
                new ToolStripMenuItem("左右反転して貼り付け", null, (s, e) => 
                {
                    sPanel.PasteNotes(clickPosition);
                    sPanel.ReverseNotes();
                })
            };
            Items.AddRange(stripItems);
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
