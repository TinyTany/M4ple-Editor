using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;
using System.Diagnostics;
using NE4S.Notes.Concrete;
using NE4S.Notes.Interface;

namespace NE4S.Notes.Abstract
{
    [Serializable()]
    public abstract class AirableNote : Note, IAirableNote
    {
        public Air Air { get; private set; } = null;
        public AirHold AirHold { get; private set; } = null;
        private AirHoldBegin airHoldBegin = null;

        protected AirableNote() { }

        protected AirableNote(int size, Position pos, PointF location, int laneIndex) 
            : base(size, pos, location, laneIndex) { }

        protected AirableNote(Note note) : base(note)
        {
            if (note is AirableNote airable)
            {
                if (airable.Air != null)
                {
                    Air = new Air(airable.Air);
                }
                if (airable.AirHold != null)
                {
                    AirHold = new AirHold(airable.AirHold);
                }
            }
        }

        public bool IsAirHoldAttachable
        {
            get
            {
                if (IsAirHoldAttached) { return false; }
                if (IsAirAttached && Air.GetType() != typeof(AirUpC)) { return false; }
                return true;
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
                return Air != null;
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
                return AirHold != null;
            }
        }

        public bool AttachAir(Air air)
        {
            if (air == null)
            {
                Logger.Error("AirableNoteへのAirの紐づけに失敗しました。引数airがnullです。");
                return false;
            }
            Air = air;
            air.GetAirable += () => this;
            return true;
        }

        public bool AttachAirHold(AirHold airHold)
        {
            if (airHold == null)
            {
                Logger.Error("AirableNoteへのAirHoldの紐づけに失敗しました。引数airHoldがnullです。");
                return false;
            }
            AirHold = airHold;
            airHold.GetAirable += () => this;
            airHoldBegin = airHold.AirHoldBegin;
            Debug.Assert(airHoldBegin != null);
            return true;
        }

        public bool DetachAir()
        {
            Air = null;
            return true;
        }

        public bool DetachAirHold()
        {
            AirHold = null;
            airHoldBegin = null;
            return true;
        }

        public override void ReSize(int size)
        {
            base.ReSize(size);
            Air?.ReSize(size);
            airHoldBegin?.ReSize(size);
            return;
        }

        /// <summary>
        /// このノーツの位置変更とそれに付随するAir、AirHoldノーツの位置のみを変更します
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="location"></param>
        /// <param name="laneIndex"></param>
        public new void RelocateOnly(Position pos, PointF location, int laneIndex)
        {
            base.RelocateOnly(pos, location, laneIndex);
            Air?.RelocateOnly(pos, location, laneIndex);
            //AirHold全体に変更を反映させるためRelocateを使う
            airHoldBegin?.Relocate(pos, location, laneIndex);
        }

        public override void Relocate(Position pos, PointF location, int laneIndex)
        {
            Relocate(location, laneIndex);
            Relocate(pos);
        }

        public override void Relocate(PointF location, int laneIndex)
        {
            base.Relocate(location, laneIndex);
            Air?.Relocate(location, laneIndex);
            airHoldBegin?.Relocate(location, laneIndex);
            return;
        }

        public override void Relocate(Position pos)
        {
            base.Relocate(pos);
            Air?.Relocate(pos);
            airHoldBegin?.Relocate(pos);
            return;
        }

        public override void RelocateOnlyAndUpdate(Position position, LaneBook laneBook)
        {
            base.RelocateOnlyAndUpdate(position, laneBook);
            Air?.RelocateOnlyAndUpdate(position, laneBook);
            airHoldBegin?.Relocate(position);
            airHoldBegin?.UpdateLocation(laneBook);
        }

        public override void Draw(Graphics g, Point drawLocation)
        {
            RectangleF drawRect = new RectangleF(
                noteRect.X - drawLocation.X + adjustNoteRect.X,
                noteRect.Y - drawLocation.Y + adjustNoteRect.Y,
                noteRect.Width,
                noteRect.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.LightGreen, Color.GreenYellow))
            {
                g.FillRectangle(gradientBrush, drawRect);
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                g.DrawPath(pen, drawRect.RoundedPath());
            }
        }
    }
}