using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes
{
    public class Flick : Note
    {
        public Flick()
        {

        }

		public Flick(int size, Pos pos) : base(size, pos)
		{
		}

		public override void Draw(PaintEventArgs e, RectangleF hitRect)
		{
			base.Draw(e, hitRect);
		}
	}
}
