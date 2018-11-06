using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;
using System.Drawing.Drawing2D;

namespace NE4S.Notes
{
    public class Slide : LongNote
    {
        /// <summary>
        /// スライド全体の背景色（水色）
        /// </summary>
        private static readonly Color baseColor = Color.FromArgb(200, 0, 164, 146);
        /// <summary>
        /// スライド中継点付近での色（紫色）
        /// </summary>
        private static readonly Color stepColor = Color.FromArgb(200, 125, 23, 155);
        /// <summary>
        /// スライドの中心線みたいなやつ（薄い水色）
        /// </summary>
        private static readonly Color lineColor = Color.FromArgb(200, 3, 181, 161);

        //帯の描画位置がちょっと上にずれてるので調節用の変数を用意
        private static readonly Point drawOffset = new Point(2, 1);

        public Slide()
        {

        }

        public Slide(int size, Position pos, PointF location, int laneIndex)
        {
            SlideBegin slideBegin = new SlideBegin(size, pos, location, laneIndex);
            Add(slideBegin);
            //TODO: posとかlocationとかをいい感じに設定する
            location.Y -= ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv / Status.Beat;
            SlideEnd slideEnd = new SlideEnd(size, pos, location, laneIndex);
            Add(slideEnd);
            Status.SelectedNote = slideEnd;
        }

        /// <summary>
        /// 与えられた座標がスライド帯の上に乗っているか判定します
        /// ただしベジェスライド上では判定されません
        /// </summary>
        /// <param name="locationVirtual"></param>
        /// <param name="scoreBook"></param>
        /// <param name="laneBook"></param>
        /// <returns></returns>
        public bool Contains(PointF locationVirtual, ScoreBook scoreBook ,LaneBook laneBook)
        {
            var list = this.OrderBy(x => x.Pos).ToList();
            foreach(Note note in list)
            {
                if (list.IndexOf(note) >= list.Count - 1) break;
                Note next = list.ElementAt(list.IndexOf(note) + 1);
                if (note is SlideCurve || next is SlideCurve) continue;
                //
                int passingLanes = next.LaneIndex - note.LaneIndex;
                if(passingLanes == 0)
                {
                    PointF topLeft = next.Location.Add(drawOffset);
                    PointF topRight = next.Location.Add(-drawOffset.X, drawOffset.Y).AddX(next.Width);
                    PointF bottomLeft = note.Location.Add(drawOffset);
                    PointF bottomRight = note.Location.Add(-drawOffset.X, drawOffset.Y).AddX(next.Width);
                    using (GraphicsPath hitPath = new GraphicsPath())
                    { 
                        hitPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                        if (hitPath.IsVisible(locationVirtual)) return true;
                    }
                }
                else if(passingLanes >= 1)
                {
                    float positionDistance = PositionDistance(note.Pos, next.Pos, scoreBook);
                    float diffX = (next.Pos.Lane - note.Pos.Lane) * ScoreInfo.MinLaneWidth;
                    #region 最初のレーンでの判定処理
                    PointF topLeft = note.Location.Add(drawOffset).Add(diffX, -positionDistance);
                    PointF topRight = note.Location.Add(-drawOffset.X, drawOffset.Y).AddX(next.Width).Add(diffX, -positionDistance);
                    PointF bottomLeft = note.Location.Add(drawOffset);
                    PointF bottomRight = note.Location.Add(-drawOffset.X, drawOffset.Y).AddX(next.Width);
                    using (GraphicsPath hitPath = new GraphicsPath())
                    {
                        hitPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                        if (hitPath.IsVisible(locationVirtual)) return true;
                    }
                    #endregion
                    #region 以降最後までの判定処理
                    ScoreLane prevLane, curLane;
                    for (prevLane = laneBook.Find(x => x.Contains(note)), curLane = laneBook.Next(prevLane);
                    curLane != null && laneBook.IndexOf(curLane) <= next.LaneIndex;
                    prevLane = curLane, curLane = laneBook.Next(curLane))
                    {
                        topLeft.X = curLane.HitRect.X + next.Pos.Lane * ScoreInfo.MinLaneWidth + drawOffset.X;
                        topLeft.Y += prevLane.HitRect.Height;
                        topRight.X = topLeft.X + next.Width - 2 * drawOffset.X;
                        topRight.Y += prevLane.HitRect.Height;
                        bottomLeft.X = curLane.HitRect.X + note.Pos.Lane * ScoreInfo.MinLaneWidth + drawOffset.X;
                        bottomLeft.Y += prevLane.HitRect.Height;
                        bottomRight.X = bottomLeft.X + note.Width - 2 * drawOffset.X;
                        bottomRight.Y += prevLane.HitRect.Height;
                        using (GraphicsPath hitPath = new GraphicsPath())
                        {
                            hitPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                            if (hitPath.IsVisible(locationVirtual)) return true;
                        }
                    }
                    #endregion
                }
            }
            return false;
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
        {
            var list = this.OrderBy(x => x.Pos).ToList();
            foreach (Note note in list)
            {
                //!(note is SlideEnd)よりもこっちのほうが確実で安全かも
                //↑だと例外で怒られた…
                if (list.IndexOf(note) < list.Count - 1)
                {
                    Note next = list.ElementAt(list.IndexOf(note) + 1);
                    DrawSlideLine(e, note, next, originPosX, originPosY, scoreBook, laneBook, currentPositionX);
                }
                e.Graphics.ResetClip();
                if (note is SlideRelay && !Status.IsSlideRelayVisible) continue;
                if (note is SlideCurve && !Status.IsSlideCurveVisible) continue;
                note.Draw(e, originPosX, originPosY);
            }
        }

        /// <summary>
        /// ノーツ間を繋ぐ帯の描画（直線）
        /// </summary>
        /// 全体的にコードが汚いのでなんとかしたい
        /// あといまの状態だと不可視中継点でもグラデーションの境界となってしまうのでなにかいい方法を考える必要がある
        private void DrawSlideLine(PaintEventArgs e, Note past, Note future, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
        {
            //相対位置
            PointF pastRerativeLocation = new PointF(past.Location.X - originPosX, past.Location.Y - originPosY);
            PointF futureRerativeLocation = new PointF(future.Location.X - originPosX, future.Location.Y - originPosY);
            
            int passingLanes = future.LaneIndex - past.LaneIndex;
            //スライドのノーツとノーツがレーンをまたがないとき
            if(passingLanes == 0)
            {
                PointF topLeft = futureRerativeLocation.Add(drawOffset);
                PointF topRight = futureRerativeLocation.Add(-drawOffset.X, drawOffset.Y).AddX(future.Width);
                PointF bottomLeft = pastRerativeLocation.Add(drawOffset);
                PointF bottomRight = pastRerativeLocation.Add(-drawOffset.X, drawOffset.Y).AddX(past.Width);
                using (GraphicsPath graphicsPath = new GraphicsPath())
                {
                    graphicsPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                    RectangleF gradientRect = graphicsPath.GetBounds();
                    if (gradientRect.Height == 0) gradientRect.Height = 1;
                    using (LinearGradientBrush myBrush = new LinearGradientBrush(gradientRect, baseColor, baseColor, LinearGradientMode.Vertical))
                    {
                        ColorBlend blend = new ColorBlend(4)
                        {
                            Colors = new Color[] { stepColor, baseColor, baseColor, stepColor},
                            Positions = new float[] { 0.0f, 0.3f, 0.7f, 1.0f}
                        };
                        myBrush.InterpolationColors = blend;
                        e.Graphics.FillPath(myBrush, graphicsPath);
                    }
                    using (Pen myPen = new Pen(lineColor, 2))
                    {
                        e.Graphics.DrawLine(myPen, (bottomLeft.X + bottomRight.X) / 2, bottomLeft.Y, (topLeft.X + topRight.X) / 2, topLeft.Y);
                    }
                }
            }
            //スライドのノーツとノーツがレーンをまたぐとき
            else if (passingLanes >= 1)
            {
                float positionDistance = PositionDistance(past.Pos, future.Pos, scoreBook);
                float diffX = (future.Pos.Lane - past.Pos.Lane) * ScoreInfo.MinLaneWidth;
                #region 最初のレーンでの描画
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
                    if (scoreLane != null)
                    {
                        RectangleF clipRect = new RectangleF(scoreLane.HitRect.X - currentPositionX, scoreLane.HitRect.Y, scoreLane.HitRect.Width, scoreLane.HitRect.Height);
                        e.Graphics.Clip = new Region(clipRect);
                    }
                    RectangleF gradientRect = graphicsPath.GetBounds();
                    if (gradientRect.Height == 0) gradientRect.Height = 1;
                    using (LinearGradientBrush myBrush = new LinearGradientBrush(gradientRect, baseColor, baseColor, LinearGradientMode.Vertical))
                    {
                        ColorBlend blend = new ColorBlend(4)
                        {
                            Colors = new Color[] { stepColor, baseColor, baseColor, stepColor },
                            Positions = new float[] { 0.0f, 0.3f, 0.7f, 1.0f }
                        };
                        myBrush.InterpolationColors = blend;
                        e.Graphics.FillPath(myBrush, graphicsPath);
                    }
                    using (Pen myPen = new Pen(lineColor, 2))
                    {
                        e.Graphics.DrawLine(myPen, (bottomLeft.X + bottomRight.X) / 2, bottomLeft.Y, (topLeft.X + topRight.X) / 2, topLeft.Y);
                    }
                }
                #endregion
                #region 以降最後までのレーンでの描画
                {
                    ScoreLane prevLane, curLane;
                    for (prevLane = laneBook.Find(x => x.Contains(past)), curLane = laneBook.Next(prevLane);
                    curLane != null && laneBook.IndexOf(curLane) <= future.LaneIndex;
                    prevLane = curLane, curLane = laneBook.Next(curLane))
                    {
                        topLeft.X = curLane.HitRect.X + future.Pos.Lane * ScoreInfo.MinLaneWidth - currentPositionX + drawOffset.X;
                        topLeft.Y += prevLane.HitRect.Height;
                        topRight.X = topLeft.X + future.Width - 2 * drawOffset.X;
                        topRight.Y += prevLane.HitRect.Height;
                        bottomLeft.X = curLane.HitRect.X + past.Pos.Lane * ScoreInfo.MinLaneWidth - currentPositionX + drawOffset.X;
                        bottomLeft.Y += prevLane.HitRect.Height;
                        bottomRight.X = bottomLeft.X + past.Width - 2 * drawOffset.X;
                        bottomRight.Y += prevLane.HitRect.Height;
                        using (GraphicsPath graphicsPath = new GraphicsPath())
                        {
                            graphicsPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                            RectangleF clipRect = new RectangleF(curLane.HitRect.Location.AddX(-currentPositionX), curLane.HitRect.Size);
                            e.Graphics.Clip = new Region(clipRect);
                            RectangleF gradientRect = graphicsPath.GetBounds();
                            if (gradientRect.Height == 0) gradientRect.Height = 1;
                            using (LinearGradientBrush myBrush = new LinearGradientBrush(gradientRect, baseColor, baseColor, LinearGradientMode.Vertical))
                            {
                                ColorBlend blend = new ColorBlend(4)
                                {
                                    Colors = new Color[] { stepColor, baseColor, baseColor, stepColor },
                                    Positions = new float[] { 0.0f, 0.3f, 0.7f, 1.0f }
                                };
                                myBrush.InterpolationColors = blend;
                                e.Graphics.FillPath(myBrush, graphicsPath);
                            }
                            using (Pen myPen = new Pen(lineColor, 2))
                            {
                                e.Graphics.DrawLine(myPen, (bottomLeft.X + bottomRight.X) / 2, bottomLeft.Y, (topLeft.X + topRight.X) / 2, topLeft.Y);
                            }
                        }
                    }
                }
                
                #endregion
            }
        }

        /// <summary>
        /// ノーツ間を繋ぐ帯の描画（ベジェ）
        /// </summary>
        private void DrawSlideCurve(PaintEventArgs e, Note past, Note curve, Note future, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
        {
            //相対位置
            PointF pastRerativeLocation = new PointF(past.Location.X - originPosX, past.Location.Y - originPosY);
            PointF curveRetativePosition = new PointF(curve.Location.X - originPosX, curve.Location.Y - originPosY);
            PointF futureRerativeLocation = new PointF(future.Location.X - originPosX, future.Location.Y - originPosY);
            PointF curveCenter = curveRetativePosition.AddX(curve.Width / 2f);

            //TODO: 以下コピペしただけなので良しなに変更する
            int passingLanes = future.LaneIndex - past.LaneIndex;
            //スライドのノーツとノーツがレーンをまたがないとき
            if (passingLanes == 0)
            {   
                PointF topLeft = futureRerativeLocation.Add(drawOffset);
                PointF topRight = futureRerativeLocation.Add(-drawOffset.X, drawOffset.Y).AddX(future.Width);
                PointF bottomLeft = pastRerativeLocation.Add(drawOffset);
                PointF bottomRight = pastRerativeLocation.Add(-drawOffset.X, drawOffset.Y).AddX(past.Width);
                using (GraphicsPath graphicsPath = new GraphicsPath())
                {
                    graphicsPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                    RectangleF gradientRect = graphicsPath.GetBounds();
                    if (gradientRect.Height == 0) gradientRect.Height = 1;
                    using (LinearGradientBrush myBrush = new LinearGradientBrush(gradientRect, baseColor, baseColor, LinearGradientMode.Vertical))
                    {
                        ColorBlend blend = new ColorBlend(4)
                        {
                            Colors = new Color[] { stepColor, baseColor, baseColor, stepColor },
                            Positions = new float[] { 0.0f, 0.3f, 0.7f, 1.0f }
                        };
                        myBrush.InterpolationColors = blend;
                        e.Graphics.FillPath(myBrush, graphicsPath);
                    }
                    using (Pen myPen = new Pen(lineColor, 2))
                    {
                        e.Graphics.DrawLine(myPen, (bottomLeft.X + bottomRight.X) / 2, bottomLeft.Y, (topLeft.X + topRight.X) / 2, topLeft.Y);
                    }
                }
            }
            //スライドのノーツとノーツがレーンをまたぐとき
            else if (passingLanes >= 1)
            {
                float positionDistance = PositionDistance(past.Pos, future.Pos, scoreBook);
                float diffX = (future.Pos.Lane - past.Pos.Lane) * ScoreInfo.MinLaneWidth;
                #region 最初のレーンでの描画
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
                    if (scoreLane != null)
                    {
                        RectangleF clipRect = new RectangleF(scoreLane.HitRect.X - currentPositionX, scoreLane.HitRect.Y, scoreLane.HitRect.Width, scoreLane.HitRect.Height);
                        e.Graphics.Clip = new Region(clipRect);
                    }
                    RectangleF gradientRect = graphicsPath.GetBounds();
                    if (gradientRect.Height == 0) gradientRect.Height = 1;
                    using (LinearGradientBrush myBrush = new LinearGradientBrush(gradientRect, baseColor, baseColor, LinearGradientMode.Vertical))
                    {
                        ColorBlend blend = new ColorBlend(4)
                        {
                            Colors = new Color[] { stepColor, baseColor, baseColor, stepColor },
                            Positions = new float[] { 0.0f, 0.3f, 0.7f, 1.0f }
                        };
                        myBrush.InterpolationColors = blend;
                        e.Graphics.FillPath(myBrush, graphicsPath);
                    }
                    using (Pen myPen = new Pen(lineColor, 2))
                    {
                        e.Graphics.DrawLine(myPen, (bottomLeft.X + bottomRight.X) / 2, bottomLeft.Y, (topLeft.X + topRight.X) / 2, topLeft.Y);
                    }
                }
                #endregion
                #region 以降最後までのレーンでの描画
                {
                    ScoreLane prevLane, curLane;
                    for (prevLane = laneBook.Find(x => x.Contains(past)), curLane = laneBook.Next(prevLane);
                    curLane != null && laneBook.IndexOf(curLane) <= future.LaneIndex;
                    prevLane = curLane, curLane = laneBook.Next(curLane))
                    {
                        topLeft.X = curLane.HitRect.X + future.Pos.Lane * ScoreInfo.MinLaneWidth - currentPositionX + drawOffset.X;
                        topLeft.Y += prevLane.HitRect.Height;
                        topRight.X = topLeft.X + future.Width - 2 * drawOffset.X;
                        topRight.Y += prevLane.HitRect.Height;
                        bottomLeft.X = curLane.HitRect.X + past.Pos.Lane * ScoreInfo.MinLaneWidth - currentPositionX + drawOffset.X;
                        bottomLeft.Y += prevLane.HitRect.Height;
                        bottomRight.X = bottomLeft.X + past.Width - 2 * drawOffset.X;
                        bottomRight.Y += prevLane.HitRect.Height;
                        using (GraphicsPath graphicsPath = new GraphicsPath())
                        {
                            graphicsPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                            RectangleF clipRect = new RectangleF(curLane.HitRect.Location.AddX(-currentPositionX), curLane.HitRect.Size);
                            e.Graphics.Clip = new Region(clipRect);
                            RectangleF gradientRect = graphicsPath.GetBounds();
                            if (gradientRect.Height == 0) gradientRect.Height = 1;
                            using (LinearGradientBrush myBrush = new LinearGradientBrush(gradientRect, baseColor, baseColor, LinearGradientMode.Vertical))
                            {
                                ColorBlend blend = new ColorBlend(4)
                                {
                                    Colors = new Color[] { stepColor, baseColor, baseColor, stepColor },
                                    Positions = new float[] { 0.0f, 0.3f, 0.7f, 1.0f }
                                };
                                myBrush.InterpolationColors = blend;
                                e.Graphics.FillPath(myBrush, graphicsPath);
                            }
                            using (Pen myPen = new Pen(lineColor, 2))
                            {
                                e.Graphics.DrawLine(myPen, (bottomLeft.X + bottomRight.X) / 2, bottomLeft.Y, (topLeft.X + topRight.X) / 2, topLeft.Y);
                            }
                        }
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// 2つのPosition変数からその仮想的な縦の距離を計算する
        /// </summary>
        /// <param name="pastPosition"></param>
        /// <param name="futurePosition"></param>
        /// <param name="scoreBook"></param>
        /// <returns></returns>
        private float PositionDistance(Position pastPosition, Position futurePosition, ScoreBook scoreBook)
        {
            float distance = 0;
            //4分の4拍子1小節分の高さ
            float baseBarSize = ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight;
            Score pastScore = scoreBook.At(pastPosition.Bar - 1), futureScore = scoreBook.At(futurePosition.Bar - 1);
            distance += baseBarSize * (pastScore.BarSize - pastPosition.Size);
            for(int i = pastScore.Index + 1; i <= futureScore.Index - 1; ++i)
            {
                distance += scoreBook.At(i).BarSize * baseBarSize;
            }
            distance += baseBarSize * futurePosition.Size;
            return distance;
        }
	}
}
