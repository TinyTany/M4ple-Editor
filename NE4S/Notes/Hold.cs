﻿using NE4S.Scores;
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
    public class Hold : LongNote
    {
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

        public Hold() { }

        public Hold(int size, Position pos, PointF location, int laneIndex)
        {
            HoldBegin holdBegin = new HoldBegin(size, pos, location, laneIndex);
            holdBegin.CheckNotePosition += CheckNotePosition;
            holdBegin.CheckNoteSize += CheckNoteSize;
            Add(holdBegin);
            location.Y -= ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv / Status.Beat;
            HoldEnd holdEnd = new HoldEnd(size, pos.Next(), location, laneIndex);
            holdEnd.CheckNotePosition += CheckNotePosition;
            holdEnd.CheckNoteSize += CheckNoteSize;
            holdEnd.IsPositionAvailable += IsPositionAvailable;
            Add(holdEnd);
            Status.SelectedNote = holdEnd;
        }

        private void CheckNotePosition(Note note, int deltaTick)
        {
            if(note is HoldBegin)
            {
                int diffLane;
                foreach (Note itrNote in this.OrderBy(x => x.Position.Tick).Where(x => x != note))
                {
                    diffLane = itrNote.Position.Lane - note.Position.Lane;
                    //貫通する
                    itrNote.Relocate(
                        new Position(note.Position.Lane, itrNote.Position.Tick + deltaTick),
                        new PointF(itrNote.Location.X - diffLane * ScoreInfo.MinLaneWidth, itrNote.Location.Y - deltaTick * ScoreInfo.MaxBeatHeight),
                        itrNote.LaneIndex);
                }
            }
            else if(note is HoldEnd)
            {
                Note holdBegin = this.OrderBy(x => x.Position.Tick).First();
                int diffLane = holdBegin.Position.Lane - note.Position.Lane;
                (note as AirableNote).RelocateOnly(
                        new Position(holdBegin.Position.Lane, note.Position.Tick),
                        new PointF(note.Location.X + diffLane * ScoreInfo.MinLaneWidth, note.Location.Y),
                        note.LaneIndex);
            }
            else
            {
                //ここに入ることは無いはずだし入ったとしても何もしない一応警告でも出しとけ
                System.Diagnostics.Debug.Assert(false, "不正なノーツの種類です。");
            }
            return;
        }

        private void CheckNoteSize(Note note)
        {
            foreach(Note itrNote in this.OrderBy(x => x.Position.Tick).Where(x => x != note))
            {
                if (itrNote is AirableNote airableNote)
                {
                    airableNote.ReSize(note.Size);
                }
                else
                {
                    //ここで普通のReSizeメソッドを使うと無限再帰みたくなっちゃう...
                    itrNote.ReSizeOnly(note.Size);
                }
            }
            return;
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY, LaneBook laneBook, int currentPositionX)
        {
            base.Draw(e, originPosX, originPosY, laneBook, currentPositionX);
            var list = this.OrderBy(x => x.Position.Tick).ToList();
            foreach (Note note in list)
            {
                if(list.IndexOf(note) < list.Count - 1)
                {
                    Note next = list.Next(note);
                    DrawHoldLine(e, note, next, originPosX, originPosY, laneBook, currentPositionX);
                }
                //クリッピングの解除を忘れないこと
                e.Graphics.ResetClip();
                note.Draw(e, originPosX, originPosY);
            }
        }

        private static void DrawHoldLine(PaintEventArgs e, Note past, Note future, int originPosX, int originPosY, LaneBook laneBook, int currentPositionX)
        {
            float distance = (future.Position.Tick - past.Position.Tick) * ScoreInfo.MaxBeatHeight;
            //グラデーション矩形
            //x座標と幅は適当だけど動いてるはず。重要なのはy座標と高さ
            RectangleF gradientRect = new RectangleF(0, past.Location.Y - distance + drawOffset.Y, 10, distance <= 0 ? 1 : distance);
            //相対位置
            PointF pastRerativeLocation = new PointF(past.Location.X - originPosX, past.Location.Y - originPosY);
            PointF futureRerativeLocation = new PointF(future.Location.X - originPosX, future.Location.Y - originPosY);

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
                    using (LinearGradientBrush myBrush = new LinearGradientBrush(gradientRect, baseColor, baseColor, LinearGradientMode.Vertical))
                    {
                        myBrush.InterpolationColors = colorBlend;
                        e.Graphics.FillPath(myBrush, graphicsPath);
                    }
                }
            }
            //スライドのノーツとノーツがレーンをまたぐとき
            else if (passingLanes >= 1)
            {
                float positionDistance = (future.Position.Tick - past.Position.Tick) * ScoreInfo.MaxBeatHeight;
                float diffX = (future.Position.Lane - past.Position.Lane) * ScoreInfo.MinLaneWidth;
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
                    using (LinearGradientBrush myBrush = new LinearGradientBrush(gradientRect, baseColor, baseColor, LinearGradientMode.Vertical))
                    {
                        myBrush.InterpolationColors = colorBlend;
                        e.Graphics.FillPath(myBrush, graphicsPath);
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
                        topLeft.X = curLane.HitRect.X + future.Position.Lane * ScoreInfo.MinLaneWidth - currentPositionX + drawOffset.X;
                        topLeft.Y += prevLane.HitRect.Height;
                        topRight.X = topLeft.X + future.Width - 2 * drawOffset.X;
                        topRight.Y += prevLane.HitRect.Height;
                        bottomLeft.X = curLane.HitRect.X + past.Position.Lane * ScoreInfo.MinLaneWidth - currentPositionX + drawOffset.X;
                        bottomLeft.Y += prevLane.HitRect.Height;
                        bottomRight.X = bottomLeft.X + past.Width - 2 * drawOffset.X;
                        bottomRight.Y += prevLane.HitRect.Height;
                        //
                        gradientRect.Y += prevLane.HitRect.Height;
                        using (GraphicsPath graphicsPath = new GraphicsPath())
                        {
                            graphicsPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                            RectangleF clipRect = new RectangleF(curLane.HitRect.Location.AddX(-currentPositionX), curLane.HitRect.Size);
                            e.Graphics.Clip = new Region(clipRect);
                            using (LinearGradientBrush myBrush = new LinearGradientBrush(gradientRect, baseColor, baseColor, LinearGradientMode.Vertical))
                            {
                                myBrush.InterpolationColors = colorBlend;
                                e.Graphics.FillPath(myBrush, graphicsPath);
                            }
                        }
                    }
                }

                #endregion
            }
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
