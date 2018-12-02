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

        public AirableNote(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location, 0)
        {
            LaneIndex = laneIndex;
        }

        public Air GetAirForDelete() => air;

        public AirHold GetAirHoldForDelete() => airHold;

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
        /// Airが取り付けられているか判定します
        /// 取り付けられている場合trueを返します
        /// </summary>
        public bool IsAirAttached
        {
            get
            {
                return air != null;
            }
        }

        /// <summary>
        /// AirHoldが取り付けられているか判定します
        /// 取り付けられている場合trueを返します
        /// </summary>
        public bool IsAirHoldAttached
        {
            get
            {
                return airHold != null;
            }
        }

        public void AttachAir(Air air)
        {
            this.air = air;
            air.DetachAir += DetachAir;
        }

        public void AttachAirHold(AirHold airHold)
        {
            this.airHold = airHold;
            airHold.DetachAirHold += DetachAirHold;
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

        /// <summary>
        /// このノーツの位置変更とそれに付随するAir、AirHoldノーツの位置のみを変更します
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="location"></param>
        public new void RelocateOnly(Position pos, PointF location)
        {
            base.RelocateOnly(pos, location);
            air?.RelocateOnly(pos, location);
            //AirHold全体に変更を反映させるためRelocateを使う
            airHoldBegin?.Relocate(pos, location);
        }

        public override void Relocate(Position pos, PointF location)
        {
            Relocate(location);
            Relocate(pos);
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
