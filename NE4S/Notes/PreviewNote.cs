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
        public bool Visible { get; set; } = true;
        public bool Enable { get; set; } = true;

        public PreviewNote()
        {
            note = new RectangleF();
        }

        public void Paint(PaintEventArgs e)
        {
            if (!Visible || !Enable) return;
            note.Size = new SizeF(ScoreInfo.LaneWidth * Status.NoteSize, ScoreInfo.NoteHeight);
            using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
            {
                e.Graphics.FillRectangle(myBrush, note);
            }
        }

        public PointF Location
        {
            get { return note.Location; }
            set { note.Location = value; }
        }
    }
}
