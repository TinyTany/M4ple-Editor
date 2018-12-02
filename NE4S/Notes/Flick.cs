using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes
{
    [Serializable()]
    public class Flick : AirableNote
    {
        public Flick()
        {

        }

		public Flick(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location, laneIndex)
		{
		}

		public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
		{
            RectangleF drawRect = new RectangleF(
                noteRect.X - originPosX + adjustNoteRect.X,
                noteRect.Y - originPosY + adjustNoteRect.Y,
                noteRect.Width,
                noteRect.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Gray, Color.DimGray))
            {
                e.Graphics.FillRectangle(gradientBrush, drawRect);
            }
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.SkyBlue, Color.DeepSkyBlue))
            {
                e.Graphics.FillRectangle(gradientBrush, new RectangleF(drawRect.X + noteRect.Width / 4f, drawRect.Y, noteRect.Width / 2f, noteRect.Height));
            }
            using (Pen pen = new Pen(Color.White, 1))
            {
                e.Graphics.DrawLine(pen, new PointF(drawRect.X + noteRect.Width / 4f + 4, drawRect.Y + 2), new PointF(drawRect.X + drawRect.Width * 3 / 4f - 4, drawRect.Y + 2));
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                e.Graphics.DrawPath(pen, drawRect.RoundedPath());
            }
        }
	}
}
