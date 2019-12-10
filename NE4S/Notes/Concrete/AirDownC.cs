﻿using NE4S.Data;
using NE4S.Notes.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes.Concrete
{
    /// <summary>
    /// 真下（↓）向きAir
    /// </summary>
    [Serializable()]
    public sealed class AirDownC : Air
    {
        public override NoteType NoteType => NoteType.AirDownC;

        protected static readonly float downOffsetY = 15;

        private AirDownC() { }

        public AirDownC(Note note) : base(note) { }

        public AirDownC(int size, Position pos, PointF location, int laneIndex) 
            : base(size, pos, location, laneIndex) { }

        public override bool Contains(PointF location)
        {
            return GetAirPath(new Point()).IsVisible(location);
        }

        protected override GraphicsPath GetAirPath(Point drawLocation)
        {
            GraphicsPath graphicsPath = base.GetAirPath(drawLocation);
            PointF pathLocation = graphicsPath.GetBounds().Location;
            var mat = new Matrix();
            mat.Translate(-pathLocation.X, -pathLocation.Y, MatrixOrder.Append);
            mat.Scale(1, -1, MatrixOrder.Append);
            mat.Translate(pathLocation.X, pathLocation.Y + downOffsetY, MatrixOrder.Append);
            graphicsPath.Transform(mat);
            return graphicsPath;
        }

        public override void Draw(Graphics g, Point drawLocation)
        {
            using (GraphicsPath graphicsPath = GetAirPath(drawLocation))
            using (SolidBrush brush = new SolidBrush(airDownColor))
            using (Pen pen = new Pen(Color.White, borderWidth))
            {
                g.DrawPath(pen, graphicsPath);
                g.FillPath(brush, graphicsPath);
            }
        }
    }
}
