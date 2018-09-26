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
        public Tap()
        {

        }

		public Tap(int size, Pos pos) : base(size, pos) { }

#if DEBUG
		public override void Draw(PaintEventArgs e, RectangleF hitRect)
		{
			//描画時にレーンの線の間にノーツがうまくハマるようにする
			++hitRect.X; --hitRect.Width; hitRect.Y -= 2;
			using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(255, 255, 0, 0)))
			{
				e.Graphics.FillRectangle(myBrush, hitRect);
			}
		}
#endif
	}
}
