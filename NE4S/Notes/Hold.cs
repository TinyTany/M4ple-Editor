using NE4S.Scores;
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

        public Hold()
        {

        }

        public Hold(int size, Position pos, PointF location, int laneIndex)
        {
            HoldBegin holdBegin = new HoldBegin(size, pos, location, laneIndex);
            holdBegin.CheckNotePosition += CheckNotePosition;
            holdBegin.CheckNoteSize += CheckNoteSize;
            Add(holdBegin);
            //TODO: posとかlocationとかをいい感じに設定する
            location.Y -= ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv / Status.Beat;
            HoldEnd holdEnd = new HoldEnd(size, pos, location, laneIndex);
            holdEnd.CheckNotePosition += CheckNotePosition;
            holdEnd.CheckNoteSize += CheckNoteSize;
            Add(holdEnd);
            Status.SelectedNote = holdEnd;
        }

        private void CheckNotePosition(Note note)
        {
            if(note is HoldBegin)
            {
                foreach (Note itrNote in this.OrderBy(x => x.Pos).Where(x => x != note))
                {
                    
                }
            }
            else if(note is HoldEnd)
            {
                Note holdBegin = this.OrderBy(x => x.Pos).First();
                int diffLane = holdBegin.Pos.Lane - note.Pos.Lane;
                note.RelocateOnly(
                        new Position(note.Pos.Bar, note.Pos.BeatCount, note.Pos.BaseBeat, holdBegin.Pos.Lane),
                        new PointF(note.Location.X + diffLane * ScoreInfo.MinLaneWidth, note.Location.Y));
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
            foreach(Note itrNote in this.OrderBy(x => x.Pos).Where(x => x != note))
            {
                AirableNote airableNote = itrNote as AirableNote;
                if(airableNote != null)
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

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
        {
            var list = this.OrderBy(x => x.Pos).ToList();
            foreach (Note note in list)
            {
                if(list.IndexOf(note) < list.Count - 1)
                {
                    Note next = list.Next(note);
                    DrawHoldLine(e, note, next, originPosX, originPosY, scoreBook, laneBook, currentPositionX);
                }
                //クリッピングの解除を忘れないこと
                e.Graphics.ResetClip();
                note.Draw(e, originPosX, originPosY);
            }
        }

        private static void DrawHoldLine(PaintEventArgs e, Note past, Note future, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
        {
            float distance = PositionDistance(past.Pos, future.Pos, scoreBook);
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
                        topLeft.X = curLane.HitRect.X + future.Pos.Lane * ScoreInfo.MinLaneWidth - currentPositionX + drawOffset.X;
                        topLeft.Y += prevLane.HitRect.Height;
                        topRight.X = topLeft.X + future.Width - 2 * drawOffset.X;
                        topRight.Y += prevLane.HitRect.Height;
                        bottomLeft.X = curLane.HitRect.X + past.Pos.Lane * ScoreInfo.MinLaneWidth - currentPositionX + drawOffset.X;
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
    }
}
