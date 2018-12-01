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
    public class AirHoldBegin : Note
    {
        public event NoteEventHandler CheckNotePosition, CheckNoteSize;

        public AirHoldBegin()
        {

        }

        public AirHoldBegin(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location, laneIndex)
        {
            
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
            base.Relocate(pos);
            base.Relocate(location);
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

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
        {
#if DEBUG
            RectangleF drawRect = new RectangleF(
                noteRect.X - originPosX + adjustNoteRect.X,
                noteRect.Y - originPosY + adjustNoteRect.Y,
                noteRect.Width,
                noteRect.Height);

            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                e.Graphics.DrawPath(pen, drawRect.RoundedPath());
            }
#endif
            return;
        }
    }
}
