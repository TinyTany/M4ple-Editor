using NE4S.Data;
using NE4S.Notes.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes.Concrete
{
    [Serializable()]
    public sealed class Flick : AirableNote
    {
        public override NoteType NoteType => NoteType.Flick;

        private Flick() { }

		public Flick(int size, Position pos, PointF location, int laneIndex)
            : base(size, pos, location, laneIndex) { }

		public override void Draw(Graphics g, Point drawLocation)
		{
            RectangleF drawRect = new RectangleF(
                noteRect.X - drawLocation.X + adjustNoteRect.X,
                noteRect.Y - drawLocation.Y + adjustNoteRect.Y,
                noteRect.Width,
                noteRect.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Gray, Color.DimGray))
            {
                g.FillRectangle(gradientBrush, drawRect);
            }
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.SkyBlue, Color.DeepSkyBlue))
            {
                g.FillRectangle(gradientBrush, new RectangleF(drawRect.X + noteRect.Width / 4f, drawRect.Y, noteRect.Width / 2f, noteRect.Height));
            }
            using (Pen pen = new Pen(Color.White, 1))
            {
                g.DrawLine(pen, new PointF(drawRect.X + noteRect.Width / 4f + 4, drawRect.Y + 2), new PointF(drawRect.X + drawRect.Width * 3 / 4f - 4, drawRect.Y + 2));
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                g.DrawPath(pen, drawRect.RoundedPath());
            }
        }

        public static void Draw(PaintEventArgs e, PointF location, SizeF size)
        {
            RectangleF drawRect = new RectangleF(location.X - size.Width / 2f, location.Y - size.Height / 2f, size.Width, size.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Gray, Color.DimGray))
            {
                e.Graphics.FillPath(gradientBrush, drawRect.RoundedPath());
            }
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.SkyBlue, Color.DeepSkyBlue))
            {
                e.Graphics.FillRectangle(gradientBrush, new RectangleF(drawRect.X + size.Width / 4f, drawRect.Y, size.Width / 2f, size.Height));
            }
            using (Pen pen = new Pen(Color.White, 1))
            {
                e.Graphics.DrawLine(pen, new PointF(drawRect.X + size.Width / 4f + 4, drawRect.Y + size.Height / 2), new PointF(drawRect.X + drawRect.Width * 3 / 4f - 4, drawRect.Y + size.Height / 2));
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                e.Graphics.DrawPath(pen, drawRect.RoundedPath());
            }
        }
    }
}
