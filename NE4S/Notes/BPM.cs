using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Notes
{
    [Serializable()]
    public class BPM : AttributeNote
    {
        private static readonly PointF adjustPoint = new PointF(2, -9);

        public BPM(Position position, PointF location, int laneIndex) : base(position, location, laneIndex) { }

        public override bool Contains(PointF location)
        {
            RectangleF hitRect = new RectangleF(
                noteRect.X - Position.Lane * ScoreInfo.MinLaneWidth + ScoreLane.scoreWidth + adjustPoint.X,
                Location.Y + adjustPoint.Y,
                20,
                9);
            return hitRect.Contains(location);
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
        {
            PointF drawPoint = new PointF(
                noteRect.X - Position.Lane * ScoreInfo.MinLaneWidth - originPosX + ScoreLane.scoreWidth, 
                Location.Y);
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            using (Font myFont = new Font("MS UI Gothic", ScoreInfo.FontSize, FontStyle.Bold))
            {
                e.Graphics.DrawString(
                NoteValue.ToString(),
                myFont,
                Brushes.LimeGreen,
                new PointF(
                    drawPoint.X + adjustPoint.X,
                    drawPoint.Y + adjustPoint.Y));
            }
            using (Pen myPen = new Pen(Color.LimeGreen, 1))
            {
                e.Graphics.DrawLine(
                    myPen,
                    drawPoint.X - ScoreLane.scoreWidth, 
                    drawPoint.Y, 
                    drawPoint.X,
                    drawPoint.Y);
            }
#if DEBUG
            /*
            using (Pen myPen = new Pen(Color.White))
            {
                RectangleF hitRect = new RectangleF(
                noteRect.X - Position.Lane * ScoreInfo.MinLaneWidth + ScoreLane.scoreWidth + adjustPoint.X,
                Location.Y + adjustPoint.Y,
                20,
                9);
                e.Graphics.DrawRectangles(myPen, new RectangleF[] { hitRect });
            }
            //*/
#endif
        }
    }
}
