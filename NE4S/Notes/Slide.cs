using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes
{
    public class Slide : LongNote
    {
        public Slide()
        {

        }

        public Slide(int size, Position pos, PointF location)
        {
            Add(new SlideBegin(size, pos, location));
            //TODO: posとかlocationとかをいい感じに設定する
            Add(new SlideEnd(size, pos, location));
        }

        public override void Draw(PaintEventArgs e)
		{
			
		}
	}
}
