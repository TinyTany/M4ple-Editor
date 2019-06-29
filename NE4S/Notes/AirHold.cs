using NE4S.Scores;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
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
        [OptionalField]
        public Func<AirableNote> GetAirable;

        public AirHold() { }

        public AirHold(int size, Position pos, PointF location, int laneIndex)
        {
            AirHoldBegin airholdBegin = new AirHoldBegin(size, pos, location, laneIndex);
            airholdBegin.CheckNotePosition += CheckNotePosition;
            airholdBegin.CheckNoteSize += CheckNoteSize;
            Add(airholdBegin);
            location.Y -= ScoreInfo.UnitBeatHeight * ScoreInfo.MaxBeatDiv / Status.Beat;
            AirHoldEnd airHoldEnd = new AirHoldEnd(size, pos.Next(), location, laneIndex);
            airHoldEnd.CheckNotePosition += CheckNotePosition;
            airHoldEnd.IsPositionAvailable += IsPositionTickAvailable;
            Add(airHoldEnd);
            Status.SelectedNote = airHoldEnd;
        }

        public AirHold(AirHold ah)
        {
            ah.ForEach(x =>
            {
                switch (x)
                {
                    case AirHoldBegin airHoldBegin:
                        {
                            var note = new AirHoldBegin(airHoldBegin);
                            Add(note);
                            note.CheckNotePosition += CheckNotePosition;
                            note.CheckNoteSize += CheckNoteSize;
                        }
                        break;
                    case AirHoldEnd airHoldEnd:
                        {
                            var note = new AirHoldEnd(airHoldEnd);
                            Add(note);
                            note.CheckNotePosition += CheckNotePosition;
                            note.IsPositionAvailable += IsPositionTickAvailable;
                        }
                        break;
                    case AirAction airAction:
                        {
                            var note = new AirAction(airAction);
                            base.Add(note);
                            note.CheckNotePosition += CheckNotePosition;
                            note.IsPositionAvailable += IsPositionTickAvailable;
                        }
                        break;
                    default:
                        Logger.Warn("AirHoldの要素ではないノーツです");
                        break;
                }
            });
        }

        protected override bool IsPositionTickAvailable(Note note, Position position)
        {
            var list = this.OrderBy(x => x.Position.Tick).Where(x => x != note);
            if (position.Tick < this.OrderBy(x => x.Position.Tick).First().Position.Tick) return false;
            if (note is AirHoldEnd && position.Tick < list.Last().Position.Tick) return false;
            foreach (Note itrNote in list)
            {
                if (itrNote is AirHoldBegin && position.Tick < itrNote.Position.Tick) return false;
                else if (itrNote is AirHoldEnd && position.Tick > itrNote.Position.Tick) return false;
                else if (position.Tick == itrNote.Position.Tick) return false;
            }
            return true;
        }

        public AirHoldBegin AirHoldBegin
        {
            get
            {
                return this.OrderBy(x => x.Position.Tick).First() as AirHoldBegin;
            }
        }

        public void DetachNote() => DetachAirHold?.Invoke();

        public void Add(AirAction airAction)
        {
            if (!IsPositionTickAvailable(airAction, airAction.Position))
            {
                Status.SelectedNote = null;
                return;
            }
            base.Add(airAction);
            airAction.CheckNotePosition += CheckNotePosition;
            airAction.IsPositionAvailable += IsPositionTickAvailable;
            CheckNotePosition(airAction, 0);
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
        /// <param name="laneBook"></param>
        /// <returns></returns>
        public bool Contains(PointF locationVirtual, LaneBook laneBook)
        {
            var list = this.OrderBy(x => x.Position.Tick).ToList();
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
                    float positionDistance = (next.Position.Tick - note.Position.Tick) * ScoreInfo.MaxBeatDiv;
                    float diffX = (next.Position.Lane - note.Position.Lane) * ScoreInfo.UnitLaneWidth;
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
                        topLeft.X = curLane.LaneRect.X + next.Position.Lane * ScoreInfo.UnitLaneWidth + drawOffset.X;
                        topLeft.Y += prevLane.LaneRect.Height;
                        topRight.X = topLeft.X + next.Width - 2 * drawOffset.X;
                        topRight.Y += prevLane.LaneRect.Height;
                        bottomLeft.X = curLane.LaneRect.X + note.Position.Lane * ScoreInfo.UnitLaneWidth + drawOffset.X;
                        bottomLeft.Y += prevLane.LaneRect.Height;
                        bottomRight.X = bottomLeft.X + note.Width - 2 * drawOffset.X;
                        bottomRight.Y += prevLane.LaneRect.Height;
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

        private void CheckNotePosition(Note note, int deltaTick)
        {
            if (note is AirHoldBegin)
            {
                int diffLane;
                foreach (Note itrNote in this.OrderBy(x => x.Position.Tick).Where(x => x != note))
                {
                    diffLane = itrNote.Position.Lane - note.Position.Lane;
                    itrNote.RelocateOnly(
                        new Position(note.Position.Lane, itrNote.Position.Tick + deltaTick),
                        new PointF(itrNote.Location.X - diffLane * ScoreInfo.UnitLaneWidth, itrNote.Location.Y - deltaTick * ScoreInfo.UnitBeatHeight),
                        itrNote.LaneIndex);
                }
            }
            else if (note is AirAction)
            {
                Note airHoldBegin = this.OrderBy(x => x.Position.Tick).First();
                int diffLane = airHoldBegin.Position.Lane - note.Position.Lane;
                note.RelocateOnly(
                        new Position(airHoldBegin.Position.Lane, note.Position.Tick),
                        new PointF(note.Location.X + diffLane * ScoreInfo.UnitLaneWidth, note.Location.Y),
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
            foreach (Note itrNote in this.OrderBy(x => x.Position.Tick).Where(x => x != note))
            {
                //ここで普通のReSizeメソッドを使うと無限再帰みたくなっちゃう...
                itrNote.ReSizeOnly(note.Size);
            }
            return;
        }

        public override void Draw(Graphics g, Point drawLocation, LaneBook laneBook)
        {
            base.Draw(g, drawLocation, laneBook);
            var list = this.OrderBy(x => x.Position.Tick).ToList();
            foreach (Note note in list)
            {
                // 画面外の場合は描画しないようにしてなるべく処理を軽くしたい
                if (note.Position.Tick > Status.DrawTickLast)
                {
                    continue;
                }
                if (list.IndexOf(note) < list.Count - 1)
                {
                    Note next = list.Next(note);
                    // 画面外の場合は描画しないようにしてなるべく処理を軽くしたい
                    if (next.Position.Tick < Status.DrawTickFirst)
                    {
                        continue;
                    }
                    DrawAirHoldLine(g, note, next, drawLocation, laneBook);
                }
                //クリッピングの解除を忘れないこと
                g.ResetClip();
                note.Draw(g, drawLocation);
            }
        }

        private static void DrawAirHoldLine(Graphics g, Note past, Note future, Point drawLocation, LaneBook laneBook)
        {
            float distance = (future.Position.Tick - past.Position.Tick) * ScoreInfo.UnitBeatHeight;
            PointF drawOffset = new PointF(past.Width / 2f - lineWidth / 2f, LongNote.drawOffset.Y);
            //相対位置
            PointF pastRerativeLocation = new PointF(past.Location.X - drawLocation.X, past.Location.Y - drawLocation.Y);
            PointF futureRerativeLocation = new PointF(future.Location.X - drawLocation.X, future.Location.Y - drawLocation.Y);

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
                        g.FillPath(myBrush, graphicsPath);
                    }
                }
            }
            //スライドのノーツとノーツがレーンをまたぐとき
            else if (passingLanes >= 1)
            {
                float positionDistance = (future.Position.Tick - past.Position.Tick) * ScoreInfo.UnitBeatHeight;
                float diffX = (future.Position.Lane - past.Position.Lane) * ScoreInfo.UnitLaneWidth;
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
                        RectangleF clipRect = new RectangleF(
                            scoreLane.LaneRect.X - drawLocation.X,
                            scoreLane.LaneRect.Y - drawLocation.Y,
                            scoreLane.LaneRect.Width, 
                            scoreLane.LaneRect.Height);
                        g.Clip = new Region(clipRect);
                    }
                    using (SolidBrush myBrush = new SolidBrush(lineColor))
                    {
                        g.FillPath(myBrush, graphicsPath);
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
                        topLeft.X = curLane.LaneRect.X + future.Position.Lane * ScoreInfo.UnitLaneWidth - drawLocation.X + drawOffset.X;
                        topLeft.Y += prevLane.LaneRect.Height;
                        topRight.X = topLeft.X + future.Width - 2 * drawOffset.X;
                        topRight.Y += prevLane.LaneRect.Height;
                        bottomLeft.X = curLane.LaneRect.X + past.Position.Lane * ScoreInfo.UnitLaneWidth - drawLocation.X + drawOffset.X;
                        bottomLeft.Y += prevLane.LaneRect.Height;
                        bottomRight.X = bottomLeft.X + past.Width - 2 * drawOffset.X;
                        bottomRight.Y += prevLane.LaneRect.Height;
                        using (GraphicsPath graphicsPath = new GraphicsPath())
                        {
                            graphicsPath.AddLines(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                            RectangleF clipRect = new RectangleF(curLane.LaneRect.Location.Sub(drawLocation), curLane.LaneRect.Size);
                            g.Clip = new Region(clipRect);
                            using (SolidBrush myBrush = new SolidBrush(lineColor))
                            {
                                g.FillPath(myBrush, graphicsPath);
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
            using (SolidBrush myBrush = new SolidBrush(lineColor))
            {
                e.Graphics.FillRectangle(myBrush, drawRect);
            }
        }
    }
}
