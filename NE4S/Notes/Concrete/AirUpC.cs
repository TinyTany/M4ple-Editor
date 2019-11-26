using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using NE4S.Notes.Abstract;

namespace NE4S.Notes.Concrete
{
    /// <summary>
    /// 真上（↑）向きAir
    /// </summary>
    [Serializable()]
    public sealed class AirUpC : Air
    {
        public override NoteType NoteType => NoteType.AirUpC;

        private AirUpC() { }

        public AirUpC(Note note) : base(note) { }

        public AirUpC(int size, Position pos, PointF location, int laneIndex) 
            : base(size, pos, location, laneIndex) { }

        public override bool Contains(PointF location)
        {
            return GetAirPath(new Point()).IsVisible(location);
        }

        public override void Draw(Graphics g, Point drawLocation)
        {
            using (GraphicsPath graphicsPath = GetAirPath(drawLocation))
            using (SolidBrush brush = new SolidBrush(airUpColor))
            using (Pen pen = new Pen(Color.White, borderWidth))
            { 
                g.DrawPath(pen, graphicsPath);
                g.FillPath(brush, graphicsPath);
            }
        }
    }
}
