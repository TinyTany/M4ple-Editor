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
using NE4S.Data;

namespace NE4S.Notes.Abstract
{
    [Serializable()]
    public abstract class AirableNote : Note, IAirableNote
    {
        public Air Air { get; private set; } = null;
        public AirHold AirHold { get; private set; } = null;
        private AirHoldBegin airHoldBegin = null;

        protected AirableNote() { }

        protected AirableNote(int size, Position pos, PointF location) 
            : base(size, pos, location) { }

        protected AirableNote(Note note) : base(note)
        {
            if (note is AirableNote airable)
            {
                if (airable.Air != null)
                {
                    Air = Air.Factory(airable.Air);
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

        public bool DetachAir(out Air air)
        {
            air = Air;
            Air = null;
            return true;
        }

        public bool DetachAirHold(out AirHold ah)
        {
            ah = AirHold;
            AirHold = null;
            airHoldBegin = null;
            return true;
        }

        public override bool ReSize(int size)
        {
            if (!base.ReSize(size)) { return false; }
            Air?.ReSize(size);
            airHoldBegin?.ReSize(size);
            return true;
        }

        public override bool Relocate(Position pos)
        {
            if (!base.Relocate(pos)) { return false; }
            Air?.Relocate(pos);
            airHoldBegin?.Relocate(pos);
            return true;
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