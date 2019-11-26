using NE4S.Notes.Abstract;
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
    public sealed class SlideTap : Note
    {
        public override NoteType NoteType => NoteType.SlideTap;

        public event Func<Note, Position, bool> IsPositionAvailable;

        private SlideTap() { }

        public SlideTap(int size, Position pos, PointF location, int laneIndex) 
            : base(size, pos, location, laneIndex) { }

        public SlideTap(Note note) : base(note) { }

        public override void Relocate(Position pos, PointF location, int laneIndex)
        {

            if (IsPositionAvailable == null) { return; }
            if (IsPositionAvailable(this, pos))
            {
                base.Relocate(pos);
                base.Relocate(location, laneIndex);
            }
            else if (laneIndex == LaneIndex)
            {
                base.Relocate(new Position(pos.Lane, Position.Tick));
                base.Relocate(new PointF(location.X, Location.Y), laneIndex);
            }
        }

        public override void Relocate(Position pos)
        {
            if (IsPositionAvailable == null) { return; }
            if (IsPositionAvailable(this, pos))
            {
                base.Relocate(pos);
            }
            else
            {
                base.Relocate(new Position(pos.Lane, Position.Tick));
            }
        }

        public override void Draw(Graphics g, Point drawLocation)
        {
            RectangleF drawRect = new RectangleF(
                noteRect.X - drawLocation.X + adjustNoteRect.X,
                noteRect.Y - drawLocation.Y + adjustNoteRect.Y,
                noteRect.Width,
                noteRect.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Blue, Color.DarkBlue))
            {
                g.FillRectangle(gradientBrush, drawRect);
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                g.DrawPath(pen, drawRect.RoundedPath());
            }
        }
    }
}
