using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Notes
{
    [Serializable()]
    public sealed class AirHoldEnd : AirAction
    {
        public override int NoteID => 2;

        private AirHoldEnd() { }

        public AirHoldEnd(int size, Position pos, PointF location, int laneIndex) 
            : base(size, pos, location, laneIndex) { }

        public AirHoldEnd(Note note) 
            : this(note.Size, note.Position, note.Location, note.LaneIndex) { }
    }
}