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
using NE4S.Scores;

namespace NE4S.Notes.Concrete
{
    [Serializable()]
    public sealed class HoldEnd : LongNoteEnd
    {
        public override NoteType NoteType => NoteType.HoldEnd;

        public override event Func<Note, Position, bool> IsNewTickAvailable;

        private HoldEnd() { }

        public HoldEnd(int size, Position pos, PointF location, int laneIndex) 
            : base(size, pos, location, laneIndex) { }

        public override bool ReSize(int size)
        {
            if (!base.ReSize(size)) { return false; }
            return true;
        }

        public override bool Relocate(Position pos, PointF location, int laneIndex)
        {
            if (IsNewTickAvailable == null || !IsNewTickAvailable(this, pos))
            {
                return false;
            }
            // ノーツの水平位置を合わせるための座標調節
            var diffLane = pos.Lane - Position.Lane;
            var newLocation = new PointF(location.X + diffLane * ScoreInfo.UnitLaneWidth, location.Y);
            if (!base.Relocate(pos, newLocation, laneIndex)) { return false; }
            return true;
        }

        public override bool Relocate(Position pos)
        {
            if (IsNewTickAvailable == null || !IsNewTickAvailable(this, pos))
            {
                return false;
            }
            if (!base.Relocate(pos)) { return false; }
            return true;
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
