using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Notes
{
    public class PreviewNote
    {
        private RectangleF note;

        public PreviewNote()
        {
            note = new RectangleF();
        }

        public void Paint(PaintEventArgs e)
        {
            note.Size = new SizeF(ScoreInfo.LaneWidth * Status.NoteSize, ScoreInfo.NoteHeight);
            e.Graphics.FillRectangle(Brushes.Red, note);
        }

        public PointF Location
        {
            get { return note.Location; }
            set { note.Location = value; }
        }
    }
}
