using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using NE4S.Notes.Interface;
using NE4S.Data;

namespace NE4S.Notes.Abstract
{
    [Serializable()]
	public abstract class Air : Note, IAirNote
	{
        //描画とあたり判定位置調整
        protected static readonly PointF drawOffset = new PointF(0, -25);
        protected static readonly Color airUpColor = Color.FromArgb(25, 197, 20);
        protected static readonly Color airDownColor = Color.FromArgb(193, 20, 195);
        /// <summary>
        /// 水平方向の傾斜
        /// 垂直方向の傾斜が0の下で、四角形の高さのshearX倍だけ底辺が水平方向に平行移動する
        /// </summary>
        protected static readonly float shearX = 0.3f;
        /// <summary>
        /// Air最下部から最上部までの高さ
        /// </summary>
        protected static readonly float airHeight = 20;
        /// <summary>
        /// Airを縦で切った時の幅
        /// </summary>
        private static readonly float airLineHeight = 12;
        /// <summary>
        /// Airの縁取りの白い部分の幅
        /// </summary>
        protected static readonly float borderWidth = 3;
        /// <summary>
        /// Airの幅を決める際に使う比率
        /// </summary>
        private static readonly float widthRatio = 0.8f;

        [OptionalField]
        public Func<AirableNote> GetAirable;

        protected Air() { }

        protected Air(Note note) : base(note) { }

        protected Air(int size, Position pos, PointF location, int laneIndex)
            : base(size, pos, location, laneIndex) { }

        public override bool Contains(PointF location)
        {
            return GetAirPath(new Point()).IsVisible(location);
        }

        /// <summary>
        /// 真上向きAirのGraphicsPathを返します
        /// </summary>
        /// <returns></returns>
        protected virtual GraphicsPath GetAirPath(Point drawLocation)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            PointF baseLocation = Location.Add(-drawLocation.X, -drawLocation.Y);
            PointF topCenter = baseLocation.AddX(Width / 2f).AddY(borderWidth).Add(drawOffset);
            PointF topRight = topCenter.AddX(Width / 2f - (NoteSize - 1) * widthRatio).AddY(airHeight - airLineHeight - borderWidth);
            PointF topLeft = topCenter.AddX(-(Width / 2f - (NoteSize - 1) * widthRatio)).AddY(airHeight - airLineHeight - borderWidth);
            PointF bottomCenter = topCenter.AddY(airLineHeight - borderWidth);
            PointF bottomRight = topRight.AddY(airLineHeight - borderWidth);
            PointF bottomLeft = topLeft.AddY(airLineHeight - borderWidth);
            
            graphicsPath.AddLines(new PointF[] { topCenter, topRight, bottomRight, bottomCenter, bottomLeft, topLeft});
            graphicsPath.CloseFigure();
            return graphicsPath;
        }
    }
}
