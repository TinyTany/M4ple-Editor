using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Data;
using NE4S.Notes.Abstract;
using NE4S.Scores;

namespace NE4S.Notes.Concrete
{
    [Serializable()]
    public sealed class BPM : AttributeNote
    {
        public override NoteType NoteType => NoteType.Bpm;

        private static readonly PointF adjustPoint = new PointF(2, -9);

        private BPM() { }

        public BPM(Position position, PointF location, float noteValue, int laneIndex)
            : base(position, location, noteValue, laneIndex) { }

        public override bool Contains(PointF location)
        {
            RectangleF hitRect = new RectangleF(
                noteRect.X - Position.Lane * ScoreInfo.UnitLaneWidth + ScoreLane.scoreWidth + adjustPoint.X,
                Location.Y + adjustPoint.Y,
                20,
                9);
            return hitRect.Contains(location);
        }

        public override void Draw(Graphics g, Point drawLocation)
        {
            PointF drawPoint = new PointF(
                noteRect.X - Position.Lane * ScoreInfo.UnitLaneWidth - drawLocation.X + ScoreLane.scoreWidth, 
                Location.Y - drawLocation.Y);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            using (Font myFont = new Font("MS UI Gothic", ScoreInfo.FontSize, FontStyle.Bold))
            {
                g.DrawString(
                Value.ToString(),
                myFont,
                Brushes.LimeGreen,
                new PointF(
                    drawPoint.X + adjustPoint.X,
                    drawPoint.Y + adjustPoint.Y));
            }
            using (Pen myPen = new Pen(Color.LimeGreen, 1))
            {
                g.DrawLine(
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
