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
    public class HellTap : AirableNote
    {
        public HellTap()
        {

        }

        public HellTap(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location, laneIndex)
		{
		}

		public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
		{
            RectangleF drawRect = new RectangleF(
                noteRect.X - originPosX + adjustNoteRect.X,
                noteRect.Y - originPosY + adjustNoteRect.Y,
                noteRect.Width,
                noteRect.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.DarkSlateBlue, Color.DarkBlue))
            {
                e.Graphics.FillRectangle(gradientBrush, drawRect);
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                e.Graphics.DrawRectangles(pen, new RectangleF[] { drawRect });
            }
        }
	}
}
