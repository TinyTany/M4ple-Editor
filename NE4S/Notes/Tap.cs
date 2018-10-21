using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace NE4S.Notes
{
    public class Tap : Note
    {
		private Air air;

        public Tap()
        {
			air = new Air();
        }

		//TODO: Air初期化して
		public Tap(int size, Position pos, PointF location) : base(size, pos, location) { }

#if DEBUG
		public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
		{
			RectangleF drawRect = new RectangleF(
				hitRect.X - originPosX,
				hitRect.Y - originPosY,
				hitRect.Width,
				hitRect.Height);
			using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(255, 255, 0, 0)))
			{
				e.Graphics.FillRectangle(myBrush, drawRect);
			}
			using (Pen pen = new Pen(Color.White))
			{
				e.Graphics.DrawRectangles(pen, new RectangleF[]{ drawRect });
			}
		}
#endif
	}
}
