using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NE4S.Scores;
using NE4S.Notes;
using NE4S.Notes.Abstract;

namespace NE4S.Component
{
    [Serializable()]
    public class SelectionArea
    {
        // REVIEW: TODO: 汚いコードでやってることも何も信用できないので全体的に見直す必要あり
        /// <summary>
        /// （マウスによって指定された）矩形の開始位置
        /// </summary>
        public Position StartPosition { get; set; } = null;
        /// <summary>
        /// （マウスによって指定された）矩形の終了位置
        /// </summary>
        public Position EndPosition { get; set; } = null;
        public List<Note> SelectedNoteList { get; private set; } = new List<Note>();
        public List<LongNote> SelectedLongNoteList { get; private set; } = new List<LongNote>();
        /// <summary>
        /// StartPositionとEndPositionから矩形の左上の位置を計算する
        /// </summary>
        public Position TopLeftPosition
        {
            get
            {
                if (StartPosition == null || EndPosition == null)
                {
                    Logger.Warn("StartPositionまたはEndPositionがnullでした");
                    return new Position();
                }
                return new Position(
                    Math.Min(StartPosition.Lane, EndPosition.Lane),
                    Math.Max(StartPosition.Tick, EndPosition.Tick));
            }
        }
        /// <summary>
        /// StartPositionとEndPositionから矩形の右下の位置を計算する
        /// </summary>
        public Position BottomRightPosition
        {
            get
            {
                if (StartPosition == null || EndPosition == null)
                {
                    Logger.Warn("StartPositionまたはEndPositionがnullでした");
                    return new Position();
                }
                return new Position(
                    Math.Max(StartPosition.Lane, EndPosition.Lane),
                    Math.Min(StartPosition.Tick, EndPosition.Tick));
            }
        }
        private readonly int minHeight = 6;

        public SelectionArea() { }

        public SelectionArea(SelectionArea selectionArea)
        {
            Reset(selectionArea);
        }

        public void Reset()
        {
            StartPosition = null;
            EndPosition = null;
            SelectedNoteList.Clear();
            SelectedLongNoteList.Clear();
        }

        public void Reset(SelectionArea selectionArea)
        {
            StartPosition = selectionArea.StartPosition;
            EndPosition = selectionArea.EndPosition;
            SelectedNoteList = new List<Note>(selectionArea.SelectedNoteList);
            SelectedLongNoteList = new List<LongNote>(selectionArea.SelectedLongNoteList);
        }

        public bool Contains(Position position)
        {
            if (StartPosition == null || EndPosition == null || position == null) return false;
            if (position.Tick < BottomRightPosition.Tick) return false;
            if (position.Tick >= TopLeftPosition.Tick) return false;
            if (position.Lane < TopLeftPosition.Lane) return false;
            if (position.Lane > BottomRightPosition.Lane) return false;
            return true;
        }

        public bool Contains(Note note)
        {
            Position position = note.Position;
            if (StartPosition == null || EndPosition == null || position == null) return false;
            if (position.Tick < BottomRightPosition.Tick) return false;
            if (position.Tick > TopLeftPosition.Tick) return false;
            if (position.Lane < TopLeftPosition.Lane) return false;
            if (position.Lane + note.NoteSize - 1 > BottomRightPosition.Lane) return false;
            return true;
        }

        public bool Contains(LongNote longNote)
        {
            foreach(Note note in longNote)
            {
                if (!Contains(note))
                {
                    return false;
                }
            }
            return true;
        }

        public void SetContainsNotes(NoteBook noteBook)
        {
            SelectedNoteList.Clear();
            SelectedLongNoteList.Clear();
            if (Status.IsShortNoteVisible)
            {
                SelectedNoteList = noteBook.ShortNotes.Where(x => Contains(x)).ToList();
            }
            if (Status.IsHoldVisible)
            {
                SelectedLongNoteList = SelectedLongNoteList.Union(noteBook.HoldNotes.Where(x => Contains(x))).ToList();
            }
            if (Status.IsSlideVisible)
            {
                SelectedLongNoteList = SelectedLongNoteList.Union(noteBook.SlideNotes.Where(x => Contains(x))).ToList();
            }
            var b = SelectedNoteList.Any() || SelectedLongNoteList.Any();
            Status.OnCopyChanged(b);
        }

        /// <summary>
        /// 保持している矩形領域内のノーツのリストを空にします。
        /// </summary>
        public void ClearSelectedList()
        {
            SelectedNoteList.Clear();
            SelectedLongNoteList.Clear();
        }

        // REVIEW: この辺の生ノーツデータを手づかみで触る処理はここじゃなくてNoteBookでやったほうが安全じゃない…？
        public void ReverseNotes(NoteBook noteBook, LaneBook laneBook)
        {
            ReverseShortNotes(SelectedNoteList, laneBook, noteBook);
            SelectedLongNoteList.ForEach(x =>
            {
                ReverseShortNotes(x, laneBook, noteBook);
            });
        }

        // REVIEW: この辺の生ノーツデータを手づかみで触る処理はここじゃなくてNoteBookでやったほうが安全じゃない…？
        private void ReverseShortNotes(List<Note> noteList, LaneBook laneBook, NoteBook noteBook)
        {
            noteList.ForEach(x =>
            {
                int reverseLane = BottomRightPosition.Lane - (x.Position.Lane - TopLeftPosition.Lane + x.NoteSize) + 1;
                Position newPosition = new Position(reverseLane, x.Position.Tick);
                x.RelocateOnlyAndUpdate(newPosition, laneBook);
                if (x is AirableNote airable && airable.IsAirAttached)
                {
                    Air newAir = null;
                    if(airable.Air is AirUpL)
                    {
                        newAir = new AirUpR(x);
                    }
                    else if(airable.Air is AirUpR)
                    {
                        newAir = new AirUpL(x);
                    }
                    else if(airable.Air is AirDownL)
                    {
                        newAir = new AirDownR(x);
                    }
                    else if(airable.Air is AirDownR)
                    {
                        newAir = new AirDownL(x);
                    }
                    if (newAir != null)
                    {
                        noteBook.DetachAirFromAirableNote(airable, out _);
                        noteBook.AttachAirToAirableNote(airable, newAir);
                    }
                }
            });
        }

        /// <summary>
        /// 選択矩形の左上の位置が引数のpositionとなるように移動します
        /// </summary>
        /// <param name="position"></param>
        /// <param name="laneBook"></param>
        // REVIEW: この辺の生ノーツデータを手づかみで触る処理はここじゃなくてNoteBookでやったほうが安全じゃない…？
        public void Relocate(Position position, LaneBook laneBook)
        {
            if (StartPosition == null || EndPosition == null || position == null) return;
            Position prevStartPosition = new Position(TopLeftPosition);
            Position areaSize = new Position(BottomRightPosition.Lane - TopLeftPosition.Lane, TopLeftPosition.Tick - BottomRightPosition.Tick);
            int newStartLane = position.Lane;
            if (newStartLane < 0)
            {
                newStartLane = 0;
            }
            else if(newStartLane + areaSize.Lane > 15)
            {
                newStartLane = 15 - areaSize.Lane;
            }
            int newStartTick = position.Tick;
            if (newStartTick - areaSize.Tick < 0)
            {
                newStartTick = areaSize.Tick;
            }
            StartPosition = new Position(
                newStartLane, 
                newStartTick);
            EndPosition = new Position(
                StartPosition.Lane + areaSize.Lane, 
                StartPosition.Tick - areaSize.Tick);
            SelectedNoteList.ForEach(x =>
            {
                Position positionDelta = x.Position - prevStartPosition;
                x.RelocateOnlyAndUpdate(new Position(newStartLane + positionDelta.Lane, newStartTick + positionDelta.Tick), laneBook);
            });
            SelectedLongNoteList.ForEach(x =>
            {
                x.ForEach(y =>
                {
                    Position positionDelta = y.Position - prevStartPosition;
                    y.RelocateOnlyAndUpdate(new Position(newStartLane + positionDelta.Lane, newStartTick + positionDelta.Tick), laneBook);
                });
            });
        }

        public void Draw(Graphics g, LaneBook laneBook, Point originLocation)
        {
            if (StartPosition == null || EndPosition == null)
            {
                return;
            }
            ScoreLane futureLane = laneBook.Find(
                x => x.StartTick <= TopLeftPosition.Tick && TopLeftPosition.Tick <= x.EndTick);
            ScoreLane pastLane = laneBook.Find(
                x => x.StartTick <= BottomRightPosition.Tick && BottomRightPosition.Tick <= x.EndTick);
            int passingLanes = futureLane != null ? futureLane.Index - pastLane.Index : laneBook.Count - pastLane.Index - 1;
            PointF topLeft = new PointF(
                pastLane.LaneRect.Left + TopLeftPosition.Lane * ScoreInfo.UnitLaneWidth - originLocation.X,
                pastLane.LaneRect.Bottom - (TopLeftPosition.Tick - pastLane.StartTick) * ScoreInfo.UnitBeatHeight - originLocation.Y - minHeight / 2);
            PointF topRight = new PointF(
                pastLane.LaneRect.Left + (BottomRightPosition.Lane + 1) * ScoreInfo.UnitLaneWidth - originLocation.X,
                pastLane.LaneRect.Bottom - (TopLeftPosition.Tick - pastLane.StartTick) * ScoreInfo.UnitBeatHeight - originLocation.Y - minHeight / 2);
            PointF bottomLeft = new PointF(
                pastLane.LaneRect.Left + TopLeftPosition.Lane * ScoreInfo.UnitLaneWidth - originLocation.X,
                pastLane.LaneRect.Bottom - (BottomRightPosition.Tick - pastLane.StartTick) * ScoreInfo.UnitBeatHeight - originLocation.Y + minHeight / 2);
            PointF bottomRight = new PointF(
                pastLane.LaneRect.Left + (BottomRightPosition.Lane + 1) * ScoreInfo.UnitLaneWidth - originLocation.X,
                pastLane.LaneRect.Bottom - (BottomRightPosition.Tick - pastLane.StartTick) * ScoreInfo.UnitBeatHeight - originLocation.Y + minHeight / 2);
            using (GraphicsPath graphicsPath = new GraphicsPath())
            {
                var smoothingMode = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.Default;
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
                                itrLane.LaneRect.X - originLocation.X,
                                itrLane.LaneRect.Y - originLocation.Y,
                                //HACK: 選択領域矩形が少し大きいので見切れないようにする
                                itrLane.LaneRect.Width + 1,
                                itrLane.LaneRect.Height + 5);
                            g.Clip = new Region(clipRect);
                            g.DrawPath(pen, graphicsPath);
                        }
                    }
                    topLeft = topLeft.Add(ScoreLane.Width + ScorePanel.Margin.Left + ScorePanel.Margin.Right, itrLane.LaneRect.Height);
                    topRight = topRight.Add(ScoreLane.Width + ScorePanel.Margin.Left + ScorePanel.Margin.Right, itrLane.LaneRect.Height);
                    bottomLeft = bottomLeft.Add(ScoreLane.Width + ScorePanel.Margin.Left + ScorePanel.Margin.Right, itrLane.LaneRect.Height);
                    bottomRight = bottomRight.Add(ScoreLane.Width + ScorePanel.Margin.Left + ScorePanel.Margin.Right, itrLane.LaneRect.Height);
                    graphicsPath.ClearMarkers();
                }
                g.SmoothingMode = smoothingMode;
            }
            g.ResetClip();
        }
    }
}