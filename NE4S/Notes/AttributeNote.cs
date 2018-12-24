using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Notes
{
    [Serializable()]
    public class AttributeNote : Note
    {
        public float NoteValue { get; protected set; }

        public AttributeNote(Position position, PointF location, int laneIndex)
        {
            Position = position;
            noteRect.Location = location;
            LaneIndex = laneIndex;
            NoteValue = Status.CurrentValue;
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY) { }
    }
}
