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
    public class HoldBegin : Note
    {
        public event NoteEventHandler CheckNotePosition, CheckNoteSize;

        public HoldBegin()
        {

        }

        public HoldBegin(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location)
        {
            LaneIndex = laneIndex;
        }

        public override void ReSize(int size)
        {
            base.ReSize(size);
            CheckNoteSize?.Invoke(this);
            return;
        }

        public override void Relocate(Position pos, PointF location)
        {
            //基底のものを使うかこのクラスのものを使うか検討する
            Relocate(pos);
            Relocate(location);
            CheckNotePosition?.Invoke(this);
            return;
        }

        public override void Relocate(Position pos)
        {
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

        //ノーツ左端からサイズ変更するときに使うために作成したけどなんかやだ
        public override void RelocateX(Position newPos, PointF newLocation)
        {
            base.RelocateX(newPos, newLocation);
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
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Orange, Color.DarkOrange))
            {
                e.Graphics.FillRectangle(gradientBrush, drawRect);
            }
            using (Pen pen = new Pen(Color.White, 1))
            {
                e.Graphics.DrawLine(pen, new PointF(drawRect.X + 4, drawRect.Y + 2), new PointF(drawRect.X + drawRect.Width - 4, drawRect.Y + 2));
                //e.Graphics.DrawRectangles(pen, new RectangleF[]{ drawRect });
            }
        }
    }
}
