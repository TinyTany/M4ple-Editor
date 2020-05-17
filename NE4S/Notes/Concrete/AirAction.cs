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
    public class AirAction : Note
    {
        public override NoteType NoteType => NoteType.AirAction;

        public event Action<Note, int> CheckNotePosition;
        public event Func<Note, Position, bool> IsPositionAvailable;
        private readonly PointF locationOffset = new PointF(0, 1);
        private readonly SizeF sizeOffset = new SizeF(0, -2);

        protected AirAction() { }

        public AirAction(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location, laneIndex) { }

        public AirAction(Note note) : this(note.NoteSize, note.Position, note.Location, note.LaneIndex) { }

        public override void ReSize(int size)
        {
            //AirActionではサイズ変更はしない
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

        public override bool Contains(PointF location)
        {
            RectangleF hitRect = new RectangleF(
                noteRect.X + adjustNoteRect.X + locationOffset.X, 
                noteRect.Y + adjustNoteRect.Y + locationOffset.Y,
                noteRect.Width + sizeOffset.Width,
                noteRect.Height + sizeOffset.Height);
            return hitRect.Contains(location);
        }

        public override void Draw(Graphics g, Point drawLocation)
        {
            RectangleF drawRect = new RectangleF(
                noteRect.X - drawLocation.X + adjustNoteRect.X + locationOffset.X,
                noteRect.Y - drawLocation.Y + adjustNoteRect.Y + locationOffset.Y,
                noteRect.Width + sizeOffset.Width,
                noteRect.Height + sizeOffset.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Pink, Color.DeepPink))
            {
                g.FillRectangle(gradientBrush, drawRect);
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                g.DrawPath(pen, drawRect.RoundedPath());
            }
        }

        public static void Draw(PaintEventArgs e, PointF location, SizeF size)
        {
            RectangleF drawRect = new RectangleF(location.X - size.Width / 2f, location.Y - size.Height / 2f, size.Width, size.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Pink, Color.DeepPink))
            {
                e.Graphics.FillPath(gradientBrush, drawRect.RoundedPath());
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                e.Graphics.DrawPath(pen, drawRect.RoundedPath());
            }
        }
    }
}
