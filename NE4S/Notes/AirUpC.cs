using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace NE4S.Notes
{
    [Serializable()]
    public class AirUpC : Air
    {
        public override int NoteID => 1;

        public AirUpC() { }

        public AirUpC(Note note) : base(note) { }

        public AirUpC(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location, laneIndex) { }

        public override bool Contains(PointF location)
        {
            return GetAirPath(new Point()).IsVisible(location);
        }

        public override void Draw(PaintEventArgs e, Point drawLocation)
        {
            using (GraphicsPath graphicsPath = GetAirPath(drawLocation))
            using (SolidBrush brush = new SolidBrush(airUpColor))
            using (Pen pen = new Pen(Color.White, borderWidth))
            { 
                e.Graphics.DrawPath(pen, graphicsPath);
                e.Graphics.FillPath(brush, graphicsPath);
            }
        }
    }
}
