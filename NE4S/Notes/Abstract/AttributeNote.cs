using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Notes.Interface;
using NE4S.Scores;

namespace NE4S.Notes.Abstract
{
    [Serializable()]
    public abstract class AttributeNote : Note, IAttributeNote
    {
        public double Value { get; private set; }

        protected AttributeNote() { }

        protected AttributeNote(Position position, PointF location, float noteValue, int laneIndex)
        {
            Position = position;
            noteRect.Location = location;
            LaneIndex = laneIndex;
            NoteValue = noteValue;
            Size = 1;
        }

        public override void Draw(Graphics g, Point drawLocation) { }
    }
}
