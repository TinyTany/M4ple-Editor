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
    public class AirDownR : AirDownC
    {
        public override int NoteID => 5;

        public AirDownR() { }

        public AirDownR(Note note) : base(note) { }

        public AirDownR(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location, laneIndex) { }

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
            mat.Shear(-shearX, 0, MatrixOrder.Append);
            mat.Translate(pathLocation.X + shearX * (airHeight - borderWidth * 2), pathLocation.Y, MatrixOrder.Append);
            graphicsPath.Transform(mat);
            return graphicsPath;
        }

        public override void Draw(PaintEventArgs e, Point drawLocation)
        {
            using (GraphicsPath graphicsPath = GetAirPath(drawLocation))
            using (SolidBrush brush = new SolidBrush(airDownColor))
            using (Pen pen = new Pen(Color.White, borderWidth))
            {
                e.Graphics.DrawPath(pen, graphicsPath);
                e.Graphics.FillPath(brush, graphicsPath);
            }
        }
    }
}
