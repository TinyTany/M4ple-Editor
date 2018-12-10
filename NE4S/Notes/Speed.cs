using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes
{
    public class Speed : AttributeNote
    {
        public Speed(Position position, PointF location, int laneIndex) : base(position, location, laneIndex) { }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
        {
            base.Draw(e, originPosX, originPosY);
        }
    }
}
