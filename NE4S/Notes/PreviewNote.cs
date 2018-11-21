﻿using System;
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
		public PointF Location { get; set; }
        public bool Visible { get; set; } = false;
        public bool Enable { get; set; } = true;

        public PreviewNote()
        {
			note = new RectangleF();
        }

        public void Paint(PaintEventArgs e)
        {
            if (!Visible || !Enable) return;
            note.Size = new SizeF(ScoreInfo.MinLaneWidth * Status.NoteSize, ScoreInfo.NoteHeight);
			note.Location = Location;
            //HACK: 今の設定だとノーツ矩形位置調節用の値が(0, -2)なのでこのようにしてるけど嫌だよねこういうの
            note.Y += -2;
            using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                e?.Graphics.FillRectangle(myBrush, note);
            }
        }
    }
}
