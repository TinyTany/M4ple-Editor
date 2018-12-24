using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;

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
}
