using NE4S.Data;
using NE4S.Notes.Abstract;
using NE4S.Scores;
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
    [Serializable()]
    public sealed class Hold : LongNote<HoldBegin, HoldEnd>
    {
        public override LongNoteType LongNoteType => LongNoteType.Hold;

        /// <summary>
        /// ホールド全体の背景色（オレンジ色）
        /// </summary>
        private static readonly Color baseColor = Color.FromArgb(200, 200, 200, 0);
        /// <summary>
        /// ホールド端点付近での色（紫色）
        /// </summary>
        private static readonly Color stepColor = Color.FromArgb(200, 125, 23, 155);
        /// <summary>
        /// グラデーション
        /// </summary>
        private static readonly ColorBlend colorBlend = new ColorBlend(4)
        {
            Colors = new Color[] { stepColor, baseColor, baseColor, stepColor },
            Positions = new float[] { 0.0f, 0.3f, 0.7f, 1.0f }
        };

        private Hold() { }

        public Hold(int size, Position pos, PointF location, int laneIndex)
        {
            BeginNote = new HoldBegin(size, pos, location, laneIndex);
            BeginNote.ReflectNotePosition += (dp) => { EndNote?.Relocate(EndNote?.Position + dp); };
            BeginNote.ReflectNoteSize += (s) => { EndNote.ReSize(s); };
            location.Y -= ScoreInfo.UnitBeatHeight * ScoreInfo.MaxBeatDiv / Status.Beat;
            EndNote = new HoldEnd(size, pos.Next(), location, laneIndex);
            EndNote.IsNewTickAvailable += (n, p) => { return BeginNote.Position.Tick <= p.Tick; };
            // HACK: 気持ち悪いからやめろ　外でやれ
            Status.SelectedNote = EndNote;
        }

        public override void Draw(Graphics g, Point drawLocation, LaneBook laneBook)
        {
            DrawHoldLine(g, BeginNote, EndNote, drawLocation, laneBook);
            BeginNote.Draw(g, drawLocation);
            EndNote.Draw(g, drawLocation);
        }

        private static void DrawHoldLine(Graphics g, Note past, Note future, Point drawLocation, LaneBook laneBook)
        {
            float distance = (future.Position.Tick - past.Position.Tick) * ScoreInfo.UnitBeatHeight;
            //グラデーション矩形
            //x座標と幅は適当だけど動いてるはず。重要なのはy座標と高さ
            RectangleF gradientRect = new RectangleF(0, past.Location.Y - distance + drawOffset.Y - drawLocation.Y, 10, distance <= 0 ? 1 : distance);
            //相対位置
            PointF pastRerativeLocation = new PointF(past.Location.X - drawLocation.X, past.Location.Y - drawLocation.Y);
            int passingLanes = future.LaneIndex - past.LaneIndex;
            float positionDistance = (future.Position.Tick - past.Position.Tick) * ScoreInfo.UnitBeatHeight;
            float diffX = (future.Position.Lane - past.Position.Lane) * ScoreInfo.UnitLaneWidth;
            //ノーツfutureの位置はノーツpastの位置に2ノーツの距離を引いて表す。またTopRightの水平位置はfutureのWidthを使うことに注意
            PointF topLeft = pastRerativeLocation.Add(diffX, -positionDistance).Add(drawOffset);
            PointF topRight = pastRerativeLocation.Add(diffX, -positionDistance).Add(-drawOffset.X, drawOffset.Y).AddX(future.Width);
            //以下の2つはレーンをまたがないときと同じ
            PointF bottomLeft = pastRerativeLocation.Add(drawOffset);
            PointF bottomRight = pastRerativeLocation.Add(-drawOffset.X, drawOffset.Y).AddX(past.Width);
            using (GraphicsPath graphicsPath = new GraphicsPath())
            {
                graphicsPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                ScoreLane scoreLane = laneBook.Find(x => x.Contains(past));
                for (int i = 0; i <= passingLanes && scoreLane != null; ++i)
                {
                    if (Status.DrawTickFirst < scoreLane.EndTick && scoreLane.StartTick < Status.DrawTickLast)
                    {
                        RectangleF clipRect = new RectangleF(
                            scoreLane.LaneRect.X - drawLocation.X,
                            scoreLane.LaneRect.Y - drawLocation.Y,
                            scoreLane.LaneRect.Width,
                            scoreLane.LaneRect.Height);
                        g.Clip = new Region(clipRect);
                        using (LinearGradientBrush myBrush = new LinearGradientBrush(gradientRect, baseColor, baseColor, LinearGradientMode.Vertical))
                        {
                            myBrush.InterpolationColors = colorBlend;
                            g.FillPath(myBrush, graphicsPath);
                        }
                    }
                    // インクリメント
                    if (i != passingLanes)
                    {
                        graphicsPath.Translate(ScoreLane.Width + ScorePanel.Margin.Left + ScorePanel.Margin.Right, scoreLane.HitRect.Height);
                        gradientRect.Y += scoreLane.HitRect.Height;
                        scoreLane = laneBook.Next(scoreLane);
                    }
                }
                
            }
            g.ResetClip();
        }

        public static void Draw(PaintEventArgs e, PointF location, SizeF size)
        {
            RectangleF drawRect = new RectangleF(location.X - size.Width / 2, location.Y - size.Height / 2, size.Width, size.Height);
            ColorBlend colorBlend = new ColorBlend(4)
            {
                Colors = new Color[] { baseColor, stepColor },
                Positions = new float[] { 0.0f, 1.0f }
            };
            using (LinearGradientBrush myBrush = new LinearGradientBrush(drawRect, baseColor, baseColor, LinearGradientMode.Vertical))
            {
                myBrush.InterpolationColors = colorBlend;
                e.Graphics.FillRectangle(myBrush, drawRect);
            }
        }
    }
}
