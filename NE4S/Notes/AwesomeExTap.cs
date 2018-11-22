using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes
{
    [Serializable()]
    public class AwesomeExTap : AirableNote
    {
        public AwesomeExTap()
        {

        }

		public AwesomeExTap(int size, Position pos, PointF location) : base(size, pos, location)
		{

		}

		public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
		{
			
		}
	}
}
