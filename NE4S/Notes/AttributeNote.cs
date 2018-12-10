using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes
{
    public class AttributeNote : Note
    {
        public AttributeNote(Position position, PointF location, int laneIndex)
        {
            Position = position;
            noteRect.Location = location;
            LaneIndex = laneIndex;
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY) { }
    }
}
