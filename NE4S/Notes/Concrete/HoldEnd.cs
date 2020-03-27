using NE4S.Data;
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
    public sealed class HoldEnd : AirableNote
    {
        public override NoteType NoteType => NoteType.HoldEnd;

        public event Action<Note> CheckNoteSize;
        public event Action<Note, int> CheckNotePosition;
        public event Func<Note, Position, bool> IsPositionAvailable;

        private HoldEnd() { }

        public HoldEnd(int size, Position pos, PointF location, int laneIndex) 
            : base(size, pos, location, laneIndex) { }

        public override void ReSize(int size)
        {
            base.ReSize(size);
            CheckNoteSize?.Invoke(this);
            return;
        }

        public override void Relocate(Position pos, PointF location, int laneIndex)
        {
            if (IsPositionAvailable == null || !IsPositionAvailable(this, pos)) return;
            int deltaTick = pos.Tick - Position.Tick;
            base.Relocate(pos);
            base.Relocate(location, laneIndex);
            CheckNotePosition?.Invoke(this, deltaTick);
            return;
        }

        public override void Relocate(Position pos)
        {
            if (IsPositionAvailable == null || !IsPositionAvailable(this, pos)) return;
            int deltaTick = pos.Tick - Position.Tick;
            base.Relocate(pos);
            CheckNotePosition?.Invoke(this, deltaTick);
            return;
        }

        public override void Relocate(PointF location, int laneIndex)
        {
            base.Relocate(location, laneIndex);
            CheckNotePosition?.Invoke(this, 0);
            return;
        }

        public override void Draw(Graphics g, Point drawLocation)
        {
            if (Air != null || AirHold != null)
            {
                base.Draw(g, drawLocation);
                return;
            }
            RectangleF drawRect = new RectangleF(
                noteRect.X - drawLocation.X + adjustNoteRect.X,
                noteRect.Y - drawLocation.Y + adjustNoteRect.Y,
                noteRect.Width,
                noteRect.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Orange, Color.DarkOrange))
            {
                g.FillRectangle(gradientBrush, drawRect);
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                g.DrawPath(pen, drawRect.RoundedPath());
            }
            return;
        }
    }
}
