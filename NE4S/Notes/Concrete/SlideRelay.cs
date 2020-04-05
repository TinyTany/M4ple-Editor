﻿using NE4S.Notes.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Data;

namespace NE4S.Notes.Concrete
{
    [Serializable()]
    public sealed class SlideRelay : SlideStep
    {
        public override NoteType NoteType => NoteType.SlideRelay;

        private SlideRelay() { }

        public SlideRelay(int size, Position pos, PointF location, int laneIndex)
            : base(size, pos, location, laneIndex) { }

        public SlideRelay(Note note) : base(note) { }

        public override void Draw(Graphics g, Point drawLocation)
        {
            RectangleF drawRect = new RectangleF(
                noteRect.X - drawLocation.X + adjustNoteRect.X,
                noteRect.Y - drawLocation.Y + adjustNoteRect.Y,
                noteRect.Width,
                noteRect.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Blue, Color.DarkBlue))
            {
                //e.Graphics.FillRectangle(gradientBrush, drawRect);
                //e.Graphics.FillRectangle(gradientBrush, new RectangleF(100, 100, 100, 500));
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                g.DrawPath(pen, drawRect.RoundedPath());
            }
        }
    }
}
