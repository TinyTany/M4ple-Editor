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
    public class AirAction : Note
    {
        public event NoteEventHandler CheckNotePosition;
        public event PositionCheckHandler IsPositionAvailable;

        public AirAction()
        {

        }

        public AirAction(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location)
        {
            LaneIndex = laneIndex;
        }

        public override void ReSize(int size)
        {
            //AirActionではサイズ変更はしない
            return;
        }

        public override void Relocate(Position pos, PointF location)
        {
            if (!IsPositionAvailable(pos)) return;
            base.Relocate(pos);
            base.Relocate(location);
            CheckNotePosition?.Invoke(this);
            return;
        }

        public override void Relocate(Position pos)
        {
            if (!IsPositionAvailable(pos)) return;
            base.Relocate(pos);
            CheckNotePosition?.Invoke(this);
            return;
        }

        public override void Relocate(PointF location)
        {
            base.Relocate(location);
            CheckNotePosition?.Invoke(this);
            return;
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
        {
            RectangleF drawRect = new RectangleF(
                noteRect.X - originPosX + adjustNoteRect.X,
                noteRect.Y - originPosY + adjustNoteRect.Y,
                noteRect.Width,
                noteRect.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Pink, Color.DeepPink))
            {
                e.Graphics.FillRectangle(gradientBrush, drawRect);
            }
            using (Pen pen = new Pen(Color.White, 1))
            {
                //e.Graphics.DrawRectangles(pen, new RectangleF[]{ drawRect });
            }
        }
    }
}
