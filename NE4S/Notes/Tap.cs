﻿using System;
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
			using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Red, Color.DarkRed))
			{
				e.Graphics.FillRectangle(gradientBrush, drawRect);
                //e.Graphics.FillRectangle(gradientBrush, new RectangleF(100, 100, 100, 500));
			}
			using (Pen pen = new Pen(Color.White, 1))
			{
				e.Graphics.DrawRectangles(pen, new RectangleF[]{ drawRect });
			}
		}
#endif
	}
}
