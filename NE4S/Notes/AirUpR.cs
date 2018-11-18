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
    public class AirUpR : AirUpC
    {
        public AirUpR()
        {

        }

        public AirUpR(int size, Position pos, PointF location) : base(size, pos, location) { }

        public override bool Contains(PointF location)
        {
            return GetAirPath(0, 0).IsVisible(location);
        }

        protected override GraphicsPath GetAirPath(int originPosX, int originPosY)
        {
            GraphicsPath graphicsPath = base.GetAirPath(originPosX, originPosY);
            PointF pathLocation = graphicsPath.GetBounds().Location;
            var mat = new Matrix();
            mat.Translate(-pathLocation.X, -pathLocation.Y, MatrixOrder.Append);
            mat.Shear(-shearX, 0, MatrixOrder.Append);
            mat.Translate(pathLocation.X + shearX * (airHeight - borderWidth * 2), pathLocation.Y, MatrixOrder.Append);
            graphicsPath.Transform(mat);
            return graphicsPath;
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
