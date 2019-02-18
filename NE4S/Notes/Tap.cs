using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NE4S.Notes
{
    [Serializable()]
    public class Tap : AirableNote
    {
        public override int NoteID => 1;

        public Tap()
        {
			
        }

        public Tap(int size, Position pos, PointF location, int laneIndex) : base(size, pos, location, laneIndex)
        {
            
        }

        public override void Draw(Graphics g, Point drawLocation)
        {
            RectangleF drawRect = new RectangleF(
                noteRect.X - drawLocation.X + adjustNoteRect.X,
                noteRect.Y - drawLocation.Y + adjustNoteRect.Y,
                noteRect.Width,
                noteRect.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Red, Color.DarkRed))
            {
                g.FillPath(gradientBrush, drawRect.RoundedPath());
            }
            using (Pen pen = new Pen(Color.White, 1))
            {
                g.DrawLine(pen, new PointF(drawRect.X + 4, drawRect.Y + 2), new PointF(drawRect.X + drawRect.Width - 4, drawRect.Y + 2));
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                g.DrawPath(pen, drawRect.RoundedPath());
            }
            return;
        }

        //NOTE: NoteButtonでのノーツイメージ描画用に作ったけど本当にこんなのでええんか？
        public static void Draw(PaintEventArgs e, PointF location, SizeF size)
        {
            RectangleF drawRect = new RectangleF(location.X - size.Width / 2f, location.Y - size.Height / 2f, size.Width, size.Height);
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(new PointF(0, drawRect.Y), new PointF(0, drawRect.Y + drawRect.Height), Color.Red, Color.DarkRed))
            {
                e.Graphics.FillPath(gradientBrush, drawRect.RoundedPath());
            }
            using (Pen pen = new Pen(Color.White, 1))
            {
                e.Graphics.DrawLine(pen, new PointF(drawRect.X + 4, drawRect.Y + size.Height / 2), new PointF(drawRect.X + drawRect.Width - 4, drawRect.Y + size.Height / 2));
            }
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                e.Graphics.DrawPath(pen, drawRect.RoundedPath());
            }
        }
	}
}
