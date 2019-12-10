using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;
using NE4S.Notes;
using NE4S.Operation;
using NE4S.Data;

namespace NE4S.Component
{
    public class NoteEditCMenu : ContextMenuStrip
    {
        private readonly ToolStripItem[] stripItems;

        public NoteEditCMenu(ScorePanel scorePanel, Position clickPosition)
        {
            var b = Status.IsCopyAvailable;
            stripItems = new ToolStripMenuItem[]
            {
                new ToolStripMenuItem("切り取り", null, (s, e) => scorePanel.CutNotes()){ Enabled = b },
                new ToolStripMenuItem("コピー", null, (s, e) => scorePanel.CopyNotes()){ Enabled = b },
                new ToolStripMenuItem("削除", null, (s, e) => scorePanel.ClearAreaNotes()){ Enabled = b },
                new ToolStripMenuItem("左右反転", null, (s, e) => scorePanel.ReverseNotes()){ Enabled = b }
            };
            Items.AddRange(stripItems);
        }
    }

    public class LongNoteEditCMenu : ContextMenuStrip
    {
        private readonly ToolStripItem[] stripItems;

        public LongNoteEditCMenu(ScorePanel scorePanel, LongNote longNote, int tick)
        {
            System.Diagnostics.Debug.Assert(longNote != null, "ヤバイわよ");
            #region Slide切断操作が有効か判断
            var notesBeforeTick = longNote.Where(x => x.Position.Tick <= tick).OrderBy(x => x.Position.Tick);
            var notesAfterTick = longNote.Where(x => x.Position.Tick > tick).OrderBy(x => x.Position.Tick);
            var slideEditable = longNote is Slide;
            if (!notesBeforeTick.Any() || !notesAfterTick.Any())
            {
                slideEditable = false;
            }
            else if (notesBeforeTick.Last() is SlideBegin || notesAfterTick.First() is SlideEnd)
            {
                slideEditable = false;
            }
            #endregion
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
                    Enabled = slideEditable
                }
            };
            Items.AddRange(stripItems);
        }
    }
}
