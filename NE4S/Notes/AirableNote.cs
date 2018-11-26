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
    public class AirableNote : Note
    {
        protected Air air = null;
        protected AirHold airHold = null;
        private AirHoldBegin airHoldBegin = null;

        public AirableNote() { }

        public AirableNote(int size, Position pos, PointF location) : base(size, pos, location) { }

        //airHoldBeginのインデックスも変更しないと描画がおかしくなるぞ！
        public override int LaneIndex
        {
            set {
                base.LaneIndex = value;
                if(airHoldBegin != null)
                {
                    airHoldBegin.LaneIndex = value;
                }
            }
        }

        /// <summary>
        /// Air,AirHoldが取り付けられているか判定します
        /// どちらもついていない時falseを返します
        /// </summary>
        public bool IsAttached
        {
            get
            {
                if(air == null && airHold == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void AttachAir(Air air)
        {
            this.air = air;
            return;
        }

        public void AttachAirHold(AirHold airHold)
        {
            this.airHold = airHold;
            airHoldBegin = airHold.AirHoldBegin;
            System.Diagnostics.Debug.Assert(airHoldBegin != null, "だめです");
            return;
        }

        public void DetachAir()
        {
            air = null;
            return;
        }

        public void DetachAirHold()
        {
            airHold = null;
            airHoldBegin = null;
            return;
        }

        public override void ReSize(int size)
        {
            base.ReSize(size);
            air?.ReSize(size);
            airHoldBegin?.ReSize(size);
            return;
        }

        public override void Relocate(Position pos, PointF location)
        {
            Relocate(location);
            Relocate(pos);
            return;
        }

        public override void Relocate(PointF location)
        {
            base.Relocate(location);
            air?.Relocate(location);
            airHoldBegin?.Relocate(location);
            return;
        }

        public override void Relocate(Position pos)
        {
            base.Relocate(pos);
            air?.Relocate(pos);
            airHoldBegin?.Relocate(pos);
            return;
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
        {
            RectangleF drawRect = new RectangleF(
                noteRect.X - originPosX + adjustNoteRect.X,
                noteRect.Y - originPosY + adjustNoteRect.Y,
                noteRect.Width,
                noteRect.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.LightGreen, Color.GreenYellow))
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
