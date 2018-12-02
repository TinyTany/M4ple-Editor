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
    [Serializable()]
    public class AirHold : LongNote
    {
        private static readonly float lineWidth = 5f;
        private static readonly Color lineColor = Color.FromArgb(225, 115, 255, 20);

        public delegate void VoidHandler();
        public event VoidHandler DetachAirHold;

        public AirHold()
        {

        }

        public AirHold(int size, Position pos, PointF location, int laneIndex)
        {
            AirHoldBegin airholdBegin = new AirHoldBegin(size, pos, location, laneIndex);
            airholdBegin.CheckNotePosition += CheckNotePosition;
            airholdBegin.CheckNoteSize += CheckNoteSize;
            Add(airholdBegin);
            //TODO: posとかlocationとかをいい感じに設定する
            location.Y -= ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv / Status.Beat;
            AirAction airAction = new AirAction(size, pos.Next(), location, laneIndex);
            airAction.CheckNotePosition += CheckNotePosition;
            airAction.IsPositionAvailable += IsPositionAvailable;
            Add(airAction);
            Status.SelectedNote = airAction;
        }

        public AirHoldBegin AirHoldBegin
        {
            get
            {
                return this.First() as AirHoldBegin;
            }
        }

        public void DetachNote() => DetachAirHold?.Invoke();

        public void Add(AirAction airAction)
        {
            if (!IsPositionAvailable(airAction, airAction.Pos))
            {
                Status.SelectedNote = null;
                return;
            }
            base.Add(airAction);
            airAction.CheckNotePosition += CheckNotePosition;
            airAction.IsPositionAvailable += IsPositionAvailable;
            CheckNotePosition(airAction);
            CheckNoteSize(airAction);
            return;
        }

        public int Size
        {
            get
            {
                if (this.Any())
                {
                    return this.First().Size;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, "サイズが0なのはおかしい");
                    return 0;
                }
            }
        }

        /// <summary>
        /// Slideクラスのやつコピペしたので各所省略できるかも
        /// </summary>
        /// <param name="locationVirtual"></param>
        /// <param name="scoreBook"></param>
        /// <param name="laneBook"></param>
        /// <returns></returns>
        public bool Contains(PointF locationVirtual, ScoreBook scoreBook, LaneBook laneBook)
        {
            var list = this.OrderBy(x => x.Pos).ToList();
            foreach (Note note in list)
            {
                PointF drawOffset = new PointF(note.Width / 2f - lineWidth / 2f, LongNote.drawOffset.Y);
                if (list.IndexOf(note) >= list.Count - 1) break;
                Note next = list.ElementAt(list.IndexOf(note) + 1);
                int passingLanes = next.LaneIndex - note.LaneIndex;
                if (passingLanes == 0)
                {
                    PointF topLeft = next.Location.Add(drawOffset);
                    PointF topRight = next.Location.Add(-drawOffset.X, drawOffset.Y).AddX(next.Width);
                    PointF bottomLeft = note.Location.Add(drawOffset);
                    PointF bottomRight = note.Location.Add(-drawOffset.X, drawOffset.Y).AddX(note.Width);
                    using (GraphicsPath hitPath = new GraphicsPath())
                    {
                        hitPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                        if (hitPath.IsVisible(locationVirtual)) return true;
                    }
                }
                else if (passingLanes >= 1)
                {
                    float positionDistance = PositionDistance(note.Pos, next.Pos, scoreBook);
                    float diffX = (next.Pos.Lane - note.Pos.Lane) * ScoreInfo.MinLaneWidth;
                    #region 最初のレーンでの判定処理
                    PointF topLeft = note.Location.Add(drawOffset).Add(diffX, -positionDistance);
                    PointF topRight = note.Location.Add(-drawOffset.X, drawOffset.Y).AddX(next.Width).Add(diffX, -positionDistance);
                    PointF bottomLeft = note.Location.Add(drawOffset);
                    PointF bottomRight = note.Location.Add(-drawOffset.X, drawOffset.Y).AddX(note.Width);
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

        private void CheckNotePosition(Note note)
        {
            if (note is AirHoldBegin)
            {
                int diffLane;
                foreach (Note itrNote in this.OrderBy(x => x.Pos).Where(x => x != note))
                {
                    diffLane = itrNote.Pos.Lane - note.Pos.Lane;
                    itrNote.RelocateOnly(
                        new Position(itrNote.Pos.Bar, itrNote.Pos.BeatCount, itrNote.Pos.BaseBeat, note.Pos.Lane),
                        new PointF(itrNote.Location.X - diffLane * ScoreInfo.MinLaneWidth, itrNote.Location.Y));
                }
            }
            else if (note is AirAction)
            {
                Note airHoldBegin = this.OrderBy(x => x.Pos).First();
                int diffLane = airHoldBegin.Pos.Lane - note.Pos.Lane;
                note.RelocateOnly(
                        new Position(note.Pos.Bar, note.Pos.BeatCount, note.Pos.BaseBeat, airHoldBegin.Pos.Lane),
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
            foreach (Note itrNote in this.OrderBy(x => x.Pos).Where(x => x != note))
            {
                //ここで普通のReSizeメソッドを使うと無限再帰みたくなっちゃう...
                itrNote.ReSizeOnly(note.Size);
            }
            return;
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
        {
            var list = this.OrderBy(x => x.Pos).ToList();
            foreach (Note note in list)
            {
                if (list.IndexOf(note) < list.Count - 1)
                {
                    Note next = list.Next(note);
                    DrawAirHoldLine(e, note, next, originPosX, originPosY, scoreBook, laneBook, currentPositionX);
                }
                //クリッピングの解除を忘れないこと
                e.Graphics.ResetClip();
                note.Draw(e, originPosX, originPosY);
            }
        }

        private static void DrawAirHoldLine(PaintEventArgs e, Note past, Note future, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
        {
            float distance = PositionDistance(past.Pos, future.Pos, scoreBook);
            PointF drawOffset = new PointF(past.Width / 2f - lineWidth / 2f, LongNote.drawOffset.Y);
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
                    using (SolidBrush myBrush = new SolidBrush(lineColor))
                    {
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
                    using (SolidBrush myBrush = new SolidBrush(lineColor))
                    {
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
                        using (GraphicsPath graphicsPath = new GraphicsPath())
                        {
                            graphicsPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                            RectangleF clipRect = new RectangleF(curLane.HitRect.Location.AddX(-currentPositionX), curLane.HitRect.Size);
                            e.Graphics.Clip = new Region(clipRect);
                            using (SolidBrush myBrush = new SolidBrush(lineColor))
                            {
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
