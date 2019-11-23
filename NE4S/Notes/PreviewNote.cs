using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using NE4S.Scores;
using NE4S.Define;

namespace NE4S.Notes
{
    public class PreviewNote
    {
        private RectangleF note;
		public PointF Location { get; set; }
        public bool Visible { get; set; } = false;
        public bool Enable { get; set; } = true;

        public PreviewNote()
        {
			note = new RectangleF();
        }

        public void Paint(Graphics g)
        {
            if (!Visible || !Enable) return;
            if (Status.Mode != Mode.Add) return;
            if (Define.NoteType.AIRUPC <= Status.Note && Status.Note <= Define.NoteType.AIRHOLD) return;
            note.Size = new SizeF(ScoreInfo.UnitLaneWidth * Status.NoteSize, ScoreInfo.NoteHeight);
			note.Location = Location;
            //HACK: 今の設定だとノーツ矩形位置調節用の値が(0, -2)なのでこのようにしてるけど嫌だよねこういうの
            note.Y += -2;
            using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                g?.FillRectangle(myBrush, note);
            }
        }
    }
}
