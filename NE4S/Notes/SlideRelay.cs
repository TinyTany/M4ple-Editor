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
    public class SlideRelay : Note
    {
        public override int NoteID => 5;

        public event PositionCheckHandler IsPositionAvailable;

        public SlideRelay()
        {

        }

        public SlideRelay(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location, laneIndex)
        {
            
        }

        public override void Relocate(Position pos, PointF location, int laneIndex)
        {

            if (IsPositionAvailable == null) { return; }
            if (IsPositionAvailable(this, pos))
            {
                base.Relocate(pos);
                base.Relocate(location, laneIndex);
            }
            else if(laneIndex == LaneIndex)
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
                //e.Graphics.FillRectangle(gradientBrush, drawRect);
                //e.Graphics.FillRectangle(gradientBrush, new RectangleF(100, 100, 100, 500));
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                g.DrawPath(pen, drawRect.RoundedPath());
            }
        }
    }
}
