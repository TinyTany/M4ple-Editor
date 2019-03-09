using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;
using NE4S.Notes;
using NE4S.Operation;

namespace NE4S.Component
{
    public class NoteEditCMenu : ContextMenuStrip
    {
        private readonly ToolStripItem[] stripItems;

        public NoteEditCMenu(ScorePanel scorePanel, Position clickPosition)
        {
            stripItems = new ToolStripMenuItem[]
            {
                new ToolStripMenuItem("切り取り", null, (s, e) => scorePanel.CutNotes()),
                new ToolStripMenuItem("コピー", null, (s, e) => scorePanel.CopyNotes()),
                new ToolStripMenuItem("削除", null, (s, e) => scorePanel.ClearAreaNotes()),
                new ToolStripMenuItem("左右反転", null, (s, e) => scorePanel.ReverseNotes())
            };
            Items.AddRange(stripItems);
        }
    }

    public class LongNoteEditCMenu : ContextMenuStrip
    {
        private readonly ToolStripItem[] stripItems;

        public LongNoteEditCMenu(ScorePanel scorePanel, LongNote longNote, int tick)
        {
            stripItems = new ToolStripItem[]
            {
                new ToolStripMenuItem(
                    "選択したSlideノーツを最前面に移動",
                    null,
                    (s, e) => scorePanel.LongNoteToFront(longNote)),
                new ToolStripMenuItem(
                    "選択したSlideノーツを最背面に移動",
                    null,
                    (s, e) => scorePanel.LongNoteToBack(longNote)),
                new ToolStripSeparator(),
                new ToolStripMenuItem(
                    "選択箇所でSlideノーツを切断",
                    null,
                    (s, e) => scorePanel.CutSlide(longNote as Slide, tick))
                {
                    Enabled = longNote is Slide
                }
            };
            Items.AddRange(stripItems);
        }
    }
}
