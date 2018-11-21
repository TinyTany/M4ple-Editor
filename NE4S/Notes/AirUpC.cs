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
    public class AirUpC : Air
    {
        public AirUpC()
        {

        }

        public AirUpC(int size, Position pos, PointF location) : base(size, pos, location) { }

        public override bool Contains(PointF location)
        {
            return GetAirPath(0, 0).IsVisible(location);
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
        {
            using (GraphicsPath graphicsPath = GetAirPath(originPosX, originPosY))
            using (SolidBrush brush = new SolidBrush(airUpColor))
            using (Pen pen = new Pen(Color.White, borderWidth))
            { 
                e.Graphics.DrawPath(pen, graphicsPath);
                e.Graphics.FillPath(brush, graphicsPath);
            }
        }
    }
}
