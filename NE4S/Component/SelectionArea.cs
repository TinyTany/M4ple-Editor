using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Component
{
    public class SelectionArea
    {
        public Position StartPosition { get; set; } = null;
        public Position EndPosition { get; set; } = null;
        private readonly int minHeight = 6;

        public SelectionArea() { }

        public void Draw(PaintEventArgs e, LaneBook laneBook, Point originLocation)
        {
            if (StartPosition == null || EndPosition == null)
            {
                return;
            }
            var topLeftPosition = new Position(
                Math.Min(StartPosition.Lane, EndPosition.Lane), 
                Math.Max(StartPosition.Tick, EndPosition.Tick));
            var bottomRightPosition = new Position(
                Math.Max(StartPosition.Lane, EndPosition.Lane),
                Math.Min(StartPosition.Tick, EndPosition.Tick));
            ScoreLane futureLane = laneBook.Find(
                x => x.StartTick <= topLeftPosition.Tick && topLeftPosition.Tick <= x.EndTick);
            ScoreLane pastLane = laneBook.Find(
                x => x.StartTick <= bottomRightPosition.Tick && bottomRightPosition.Tick <= x.EndTick);
            int passingLanes = futureLane.Index - pastLane.Index;
            PointF topLeft = new PointF(
                pastLane.HitRect.Left + topLeftPosition.Lane * ScoreInfo.MinLaneWidth - originLocation.X,
                pastLane.HitRect.Bottom - (topLeftPosition.Tick - pastLane.StartTick) * ScoreInfo.MaxBeatHeight - originLocation.Y - minHeight / 2);
            PointF topRight = new PointF(
                pastLane.HitRect.Left + (bottomRightPosition.Lane + 1) * ScoreInfo.MinLaneWidth - originLocation.X,
                pastLane.HitRect.Bottom - (topLeftPosition.Tick - pastLane.StartTick) * ScoreInfo.MaxBeatHeight - originLocation.Y - minHeight / 2);
            PointF bottomLeft = new PointF(
                pastLane.HitRect.Left + topLeftPosition.Lane * ScoreInfo.MinLaneWidth - originLocation.X,
                pastLane.HitRect.Bottom - (bottomRightPosition.Tick - pastLane.StartTick) * ScoreInfo.MaxBeatHeight - originLocation.Y + minHeight / 2);
            PointF bottomRight = new PointF(
                pastLane.HitRect.Left + (bottomRightPosition.Lane + 1) * ScoreInfo.MinLaneWidth - originLocation.X,
                pastLane.HitRect.Bottom - (bottomRightPosition.Tick - pastLane.StartTick) * ScoreInfo.MaxBeatHeight - originLocation.Y + minHeight / 2);
            using (GraphicsPath graphicsPath = new GraphicsPath())
            {
                e.Graphics.SmoothingMode = SmoothingMode.Default;
                var itrLane = pastLane;
                for (int i = 0; i <= passingLanes; ++i, itrLane = laneBook.Next(itrLane))
                {
                    graphicsPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                    graphicsPath.CloseFigure();
                    if(itrLane.StartTick <= Status.DrawTickLast && itrLane.EndTick >= Status.DrawTickFirst)
                    {
                        using (Pen pen = new Pen(Color.White, 1))
                        {
                            pen.DashPattern = new float[] { 4f, 4f };
                            RectangleF clipRect = new RectangleF(
                                itrLane.HitRect.X - originLocation.X,
                                itrLane.HitRect.Y - originLocation.Y,
                                //HACK: 選択領域矩形が少し大きいので見切れないようにする
                                itrLane.HitRect.Width + 1,
                                itrLane.HitRect.Height + 5);
                            e.Graphics.Clip = new Region(clipRect);
                            e.Graphics.DrawPath(pen, graphicsPath);
                        }
                    }
                    topLeft = topLeft.Add(ScoreLane.Width + ScoreInfo.PanelMargin.Left + ScoreInfo.PanelMargin.Right, itrLane.HitRect.Height);
                    topRight = topRight.Add(ScoreLane.Width + ScoreInfo.PanelMargin.Left + ScoreInfo.PanelMargin.Right, itrLane.HitRect.Height);
                    bottomLeft = bottomLeft.Add(ScoreLane.Width + ScoreInfo.PanelMargin.Left + ScoreInfo.PanelMargin.Right, itrLane.HitRect.Height);
                    bottomRight = bottomRight.Add(ScoreLane.Width + ScoreInfo.PanelMargin.Left + ScoreInfo.PanelMargin.Right, itrLane.HitRect.Height);
                    graphicsPath.ClearMarkers();
                }
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }
            e.Graphics.ResetClip();
        }
    }
}