using NE4S.Data;
using NE4S.Notes.Abstract;
using NE4S.Scores;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes.Concrete
{
    [Serializable()]
    public sealed class AirHoldEnd : AirAction
    {
        public override NoteType NoteType => NoteType.AirHoldEnd;

        private AirHoldEnd() { }

        public AirHoldEnd(int size, Position pos, PointF location, int laneIndex) 
            : base(size, pos, location, laneIndex) { }

        public AirHoldEnd(Note note) 
            : this(note.Size, note.Position, note.Location, note.LaneIndex) { }
    }
}