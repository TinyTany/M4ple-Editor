using NE4S.Notes.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Data;

namespace NE4S.Notes.Concrete
{
    [Serializable()]
    public sealed class SlideBegin : LongNoteBegin
    {
        public override NoteType NoteType => NoteType.SlideBegin;

        public override event Func<Note, Position, bool> PositionChanging;

        private SlideBegin() { }

        public SlideBegin(int size, Position pos, PointF location, int laneIndex)
            : base(size, pos, location, laneIndex) { }

        public SlideBegin(Note note) : base(note) { }

        #region HACK: このコードは、SlideStep, SlideBegin, SlideEndで全く同じものがWETされているので注意！
        public override bool Relocate(Position pos, PointF location, int laneIndex)
        {
            // TODO: ロジックの確認
            if (PositionChanging == null) { return false; }
            if (PositionChanging(this, pos))
            {
                return base.Relocate(pos, location, laneIndex);
            }
            else if (laneIndex == LaneIndex)
            {
                return base.Relocate(new Position(pos.Lane, Position.Tick), new PointF(location.X, Location.Y), laneIndex);
            }
            return false;
        }

        public override bool Relocate(Position pos)
        {
            if (PositionChanging == null) { return false; }
            if (PositionChanging(this, pos))
            {
                return base.Relocate(pos);
            }
            else
            {
                return base.Relocate(new Position(pos.Lane, Position.Tick));
            }
        }
        #endregion

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
            using (Pen pen = new Pen(Color.White, 1))
            {
                g.DrawLine(pen, new PointF(drawRect.X + 4, drawRect.Y + 2), new PointF(drawRect.X + drawRect.Width - 4, drawRect.Y + 2));
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                g.DrawPath(pen, drawRect.RoundedPath());
            }
        }

        public static void Draw(PaintEventArgs e, PointF location, SizeF size)
        {
            RectangleF drawRect = new RectangleF(location.X - size.Width / 2f, location.Y - size.Height / 2f, size.Width, size.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Blue, Color.DarkBlue))
            {
                e.Graphics.FillPath(gradientBrush, drawRect.RoundedPath());
            }
            using (Pen pen = new Pen(Color.White, 1))
            {
                e.Graphics.DrawLine(pen, new PointF(drawRect.X + 4, drawRect.Y + size.Height / 2), new PointF(drawRect.X + drawRect.Width - 4, drawRect.Y + size.Height / 2));
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                e.Graphics.DrawPath(pen, drawRect.RoundedPath());
            }
        }
    }
}
