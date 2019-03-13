using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes
{
    [Serializable()]
    public class SlideCurve : Note
    {
        public override int NoteID => 4;

        public event PositionCheckHandler IsPositionAvailable;

        public SlideCurve() { }

        public SlideCurve(int size, Position pos, PointF location, int laneIndex) 
            : base(size, pos, location, laneIndex) { }

        public SlideCurve(Note note) : base(note) { }

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
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.White, Color.Gray))
            {
                g.FillRectangle(gradientBrush, drawRect);
            }
            using (Pen pen = new Pen(Color.Blue, 1))
            {
                g.DrawPath(pen, drawRect.RoundedPath());
            }
        }

        public static void Draw(PaintEventArgs e, PointF location, SizeF size)
        {
            RectangleF drawRect = new RectangleF(location.X - size.Width / 2f, location.Y - size.Height / 2f, size.Width, size.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.White, Color.Gray))
            {
                e.Graphics.FillPath(gradientBrush, drawRect.RoundedPath());
            }
            using (Pen pen = new Pen(Color.Blue, 1))
            {
                e.Graphics.DrawPath(pen, drawRect.RoundedPath());
            }
        }
    }
}
