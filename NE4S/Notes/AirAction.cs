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
    public class AirAction : Note
    {
        public event NoteEventHandler CheckNotePosition;
        public event PositionCheckHandler IsPositionAvailable;
        private readonly PointF locationOffset = new PointF(0, 1);
        private readonly SizeF sizeOffset = new SizeF(0, -2);

        public AirAction() { }

        public AirAction(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location, laneIndex) { }

        public override void ReSize(int size)
        {
            //AirActionではサイズ変更はしない
            return;
        }

        public override void Relocate(Position pos, PointF location, int laneIndex)
        {
            if (IsPositionAvailable == null || !IsPositionAvailable(this, pos)) return;
            base.Relocate(pos);
            base.Relocate(location, laneIndex);
            CheckNotePosition?.Invoke(this);
            return;
        }

        public override void Relocate(Position pos)
        {
            if (IsPositionAvailable == null || !IsPositionAvailable(this, pos)) return;
            base.Relocate(pos);
            CheckNotePosition?.Invoke(this);
            return;
        }

        public override void Relocate(PointF location, int laneIndex)
        {
            base.Relocate(location, laneIndex);
            CheckNotePosition?.Invoke(this);
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

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
        {
            RectangleF drawRect = new RectangleF(
                noteRect.X - originPosX + adjustNoteRect.X + locationOffset.X,
                noteRect.Y - originPosY + adjustNoteRect.Y + locationOffset.Y,
                noteRect.Width + sizeOffset.Width,
                noteRect.Height + sizeOffset.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Pink, Color.DeepPink))
            {
                e.Graphics.FillRectangle(gradientBrush, drawRect);
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                e.Graphics.DrawPath(pen, drawRect.RoundedPath());
            }
        }
    }
}
