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
    [Serializable()]
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
        /// <summary>
        /// グラデーション
        /// </summary>
        private static readonly ColorBlend colorBlend = new ColorBlend(4)
        {
            Colors = new Color[] { stepColor, baseColor, baseColor, stepColor },
            Positions = new float[] { 0.0f, 0.3f, 0.7f, 1.0f }
        };

        /// <summary>
        /// 空のSlide
        /// </summary>
        public Slide() { }

        // HACK: SlideEndにAirとかAirHoldがついているときとかも考慮したコピーコンストラクタになっている？？？
        public Slide(Slide slide)
        {
            slide.ForEach(x =>
            {
                if (x is SlideBegin)
                {
                    Add(new SlideBegin(x));
                }
                else if (x is SlideTap)
                {
                    Add(new SlideTap(x));
                }
                else if (x is SlideRelay)
                {
                    Add(new SlideRelay(x));
                }
                else if (x is SlideCurve)
                {
                    Add(new SlideCurve(x));
                }
                else if (x is SlideEnd)
                {
                    Add(new SlideEnd(x));
                }

            });
        }

        public Slide(int size, Position pos, PointF location, int laneIndex)
        {
            SlideBegin slideBegin = new SlideBegin(size, pos, location, laneIndex);
            Add(slideBegin);
            location.Y -= ScoreInfo.UnitBeatHeight * ScoreInfo.MaxBeatDiv / Status.Beat;
            SlideEnd slideEnd = new SlideEnd(size, pos.Next(), location, laneIndex);
            Add(slideEnd);
            Status.SelectedNote = slideEnd;
        }

        /// <summary>
        /// このSlide内のノーツに対してのTick方向の移動が有効かを調べます
        /// </summary>
        /// <param name="note"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        protected override bool IsPositionTickAvailable(Note note, Position position)
        {
            var list = this.OrderBy(x => x.Position.Tick).Where(x => x != note);
            var listUnderPosition = list.Where(x => x.Position.Tick < position.Tick);
            var listOverPosition = list.Where(x => x.Position.Tick > position.Tick);
            if (!this.Any()) return true;
            if (note is SlideBegin && position.Tick > list.First().Position.Tick) return false;
            if (note is SlideEnd && position.Tick < list.Last().Position.Tick) return false;
            if (note is SlideCurve && (!listUnderPosition.Any() || listUnderPosition.Last() is SlideCurve)) return false;
            if (note is SlideCurve && (!listOverPosition.Any() || listOverPosition.First() is SlideCurve)) return false;
            if (!(IsCurvesValid(listUnderPosition.ToList()) && IsCurvesValid(listOverPosition.ToList()))) return false;
            foreach (Note itrNote in list)
            {
                if (itrNote is SlideBegin && position.Tick < itrNote.Position.Tick) return false;
                else if (itrNote is SlideEnd && position.Tick > itrNote.Position.Tick) return false;
                else if (position.Tick == itrNote.Position.Tick) return false;
            }
            return true;
        }

        public bool IsPositionTickAvailable(PartialSlide partialSlide, Position delta)
        {
            // UNDONE
            return true;
        }

        private bool IsCurvesValid(List<Note> list)
        {
            foreach(Note note in list)
            {
                Note next = list.Next(note);
                if (next == null) break;
                if (note is SlideCurve && next is SlideCurve) return false;
            }
            return true;
        }

        #region Add系のメソッド（なぜかいっぱいある）
        // 普段使うのは1番上のやつだけ
        // それ以外は全部補助的なメソッドなので直には使わない

        /// <summary>
        /// Slide中継点系のノーツのみAddされます。それ以外はAddせずにアサートを吐きます。
        /// </summary>
        /// <param name="note"></param>
        public new void Add(Note note)
        {
            if (note is SlideBegin begin) { Add(begin); }
            else if (note is SlideEnd end) { Add(end); }
            else if (note is SlideTap tap) { Add(tap); }
            else if (note is SlideRelay relay) { Add(relay); }
            else if (note is SlideCurve curve) { Add(curve); }
            else
            {
                System.Diagnostics.Debug.Assert(false, "Slideに対して不正なノーツ追加です。");
            }
        }

        private void Add(SlideBegin slideBegin)
        {
            if (!IsPositionTickAvailable(slideBegin, slideBegin.Position))
            {
                Status.SelectedNote = null;
                return;
            }
            base.Add(slideBegin);
            slideBegin.IsPositionAvailable += IsPositionTickAvailable;
            return;
        }

        private void Add(SlideTap slideTap)
        {
            if (!IsPositionTickAvailable(slideTap, slideTap.Position))
            {
                Status.SelectedNote = null;
                return;
            }
            base.Add(slideTap);
            slideTap.IsPositionAvailable += IsPositionTickAvailable;
            return;
        }

        private void Add(SlideRelay slideRelay)
        {
            if (!IsPositionTickAvailable(slideRelay, slideRelay.Position))
            {
                Status.SelectedNote = null;
                return;
            }
            base.Add(slideRelay);
            slideRelay.IsPositionAvailable += IsPositionTickAvailable;
            return;
        }

        private void Add(SlideCurve slideCurve)
        {
            if (!IsPositionTickAvailable(slideCurve, slideCurve.Position))
            {
                Status.SelectedNote = null;
                return;
            }
            base.Add(slideCurve);
            slideCurve.IsPositionAvailable += IsPositionTickAvailable;
            return;
        }

        private void Add(SlideEnd slideEnd)
        {
            if (!IsPositionTickAvailable(slideEnd, slideEnd.Position))
            {
                Status.SelectedNote = null;
                return;
            }
            base.Add(slideEnd);
            slideEnd.IsPositionAvailable += IsPositionTickAvailable;
            return;
        }
        #endregion

        /// <summary>
        /// 削除後にノーツのチェックも行う
        /// </summary>
        /// <param name="note"></param>
        public new void Remove(Note note)
        {
            base.Remove(note);
            Note past = this.OrderBy(x => x.Position.Tick).Where(x => x.Position.Tick < note.Position.Tick).Last();
            Note future = this.OrderBy(x => x.Position.Tick).Where(x => x.Position.Tick > note.Position.Tick).First();
            if(past is SlideCurve && future is SlideCurve)
            {
                base.Remove(future);
            }
        }

        /// <summary>
        /// 与えられた座標がスライド帯の上に乗っているか判定します
        /// </summary>
        /// <param name="locationVirtual"></param>
        /// <param name="laneBook"></param>
        /// <returns></returns>
        public bool Contains(PointF locationVirtual, LaneBook laneBook)
        {
            var list = this.OrderBy(x => x.Position.Tick).ToList();
            foreach(Note note in list)
            {
                if (list.IndexOf(note) >= list.Count - 1) break;
                Note next = list.ElementAt(list.IndexOf(note) + 1);
                if (note is SlideCurve) continue;
                else if (next is SlideCurve)
                {
                    var ret = ContainsInCurve(note, next, list.ElementAt(list.IndexOf(next) + 1), laneBook, locationVirtual);
                    if (ret) return true;
                }
                //
                int passingLanes = next.LaneIndex - note.LaneIndex;
                if(passingLanes == 0)
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
                else if(passingLanes >= 1)
                {
                    float positionDistance = (next.Position.Tick - note.Position.Tick) * ScoreInfo.UnitBeatHeight;
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

        /// <summary>
        /// SlideCurveの当たり判定
        /// </summary>
        /// <param name="past"></param>
        /// <param name="curve"></param>
        /// <param name="future"></param>
        /// <param name="laneBook"></param>
        /// <param name="locationVirtual"></param>
        /// <returns></returns>
        private bool ContainsInCurve(Note past, Note curve, Note future, LaneBook laneBook, PointF locationVirtual)
        {
            int passingLanes = future.LaneIndex - past.LaneIndex;
            using (GraphicsPath graphicsPath = MyUtil.CreateSlideCurvePath(past, curve, future, drawOffset))
            {
                ScoreLane lane = laneBook.Find(x => x.Contains(past));
                for (int i = 0; i <= passingLanes && lane != null; ++i)
                {
                    if (graphicsPath.IsVisible(locationVirtual)) return true;
                    else
                    {
                        graphicsPath.Translate(
                            ScoreLane.scoreWidth + ScoreLane.Margin.Left + ScorePanel.Margin.Right + ScorePanel.Margin.Left + ScoreLane.Margin.Right,
                            lane.HitRect.Height);
                        lane = laneBook.Next(lane);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// このスライドを描画します。
        /// </summary>
        /// ノーツのリストに対して前から回して、そのノーツとそれと次のノーツまでの帯を描画し、そのノーツを描画
        /// <param name="e"></param>
        /// <param name="originPosX"></param>
        /// <param name="originPosY"></param>
        /// <param name="scoreBook"></param>
        /// <param name="laneBook"></param>
        /// <param name="currentPositionX"></param>
        public override void Draw(Graphics g, Point drawLocation, LaneBook laneBook)
        {
            if (g == null) return;
            base.Draw(g, drawLocation, laneBook);
            var list = this.OrderBy(x => x.Position.Tick).ToList();
            RectangleF gradientRect = new RectangleF();
            var stepList = list.Where(x => x is SlideBegin || x is SlideTap || x is SlideEnd).ToList();
            Note gradientNote, gradientNext, next, curve;
            foreach (Note note in list)
            {
                // 画面外の場合は描画しないようにしてなるべく処理を軽くしたい
                if (note.Position.Tick > Status.DrawTickLast)
                {
                    continue;
                }
                if (list.IndexOf(note) < list.Count - 1 && !(note is SlideCurve))
                {
                    //スライド帯のグラデーション用矩形を設定する
                    if(note is SlideBegin || note is SlideTap || note is SlideEnd)
                    {
                        gradientNote = note;
                        gradientNext = stepList.Next(note);
                        if(gradientNext != null)
                        {
                            float distance = (gradientNext.Position.Tick - gradientNote.Position.Tick) * ScoreInfo.UnitBeatHeight;
                            //x座標と幅は適当な値を入れたけどちゃんとうごいてるっぽい？重要なのはy座標と高さ
                            gradientRect = new RectangleF(0, gradientNote.Location.Y - distance + drawOffset.Y - drawLocation.Y, 10, distance);
                        }
                    }
                    //スライド帯を描画する
                    next = list.Next(note);
                    if(!(next is SlideCurve))
                    {
                        // 画面外の場合は描画しないようにしてなるべく処理を軽くしたい
                        if (next.Position.Tick < Status.DrawTickFirst)
                        {
                            continue;
                        }
                        DrawSlideLine(g, note, next, drawLocation, laneBook, ref gradientRect);
                    }
                    else
                    {
                        curve = next;
                        //SlideRelayは末尾に来ることはないし，SlideRelayが2つ以上連続に並ぶことはないという確信の元実装
                        //↑実際ノーツの束縛処理でそうなるような実装をしている（した）
                        if (list.IndexOf(curve) < list.Count - 1)
                        {
                            next = list.Next(curve);
                            // 画面外の場合は描画しないようにしてなるべく処理を軽くしたい
                            if (next.Position.Tick < Status.DrawTickFirst)
                            {
                                continue;
                            }
                            DrawSlideCurve(g, note, curve, next, drawLocation, laneBook, ref gradientRect);
                        }
                    }
                }
                //クリッピングの解除を忘れないこと
                g.ResetClip();
                //非表示設定のノーツは描画しないようにする
                if (note is SlideRelay && !Status.IsSlideRelayVisible) continue;
                if (note is SlideCurve && !Status.IsSlideCurveVisible) continue;
                note.Draw(g, drawLocation);
            }
        }

        /// <summary>
        /// ノーツ間を繋ぐ帯の描画（直線）
        /// </summary>
        private static void DrawSlideLine(Graphics g, Note past, Note future, Point drawLocation, LaneBook laneBook, ref RectangleF gradientRect)
        {
            if (gradientRect.Width <= 0) gradientRect.Width = 1;
            if (gradientRect.Height <= 0) gradientRect.Height = 1;
            //相対位置
            PointF pastRerativeLocation = new PointF(past.Location.X - drawLocation.X, past.Location.Y - drawLocation.Y);
            int passingLanes = future.LaneIndex - past.LaneIndex;
            float positionDistance = (future.Position.Tick - past.Position.Tick) * ScoreInfo.UnitBeatHeight;
            float diffX = (future.Position.Lane - past.Position.Lane) * ScoreInfo.UnitLaneWidth;

            //ノーツfutureの位置はノーツpastの位置に2ノーツの距離を引いて表す。またTopRightの水平位置はfutureのWidthを使うことに注意
            PointF topLeft = pastRerativeLocation.Add(diffX, -positionDistance).Add(drawOffset);
            PointF topRight = pastRerativeLocation.Add(diffX, -positionDistance).Add(-drawOffset.X, drawOffset.Y).AddX(future.Width);
            //以下の2つはレーンをまたがないときと同じ
            PointF bottomLeft = pastRerativeLocation.Add(drawOffset).AddY(deltaHeight);
            PointF bottomRight = pastRerativeLocation.Add(-drawOffset.X, drawOffset.Y).AddX(past.Width).AddY(deltaHeight);
            using (GraphicsPath graphicsPath = new GraphicsPath())
            {
                graphicsPath.AddPolygon(new PointF[] { topLeft, bottomLeft, bottomRight, topRight });
                ScoreLane scoreLane = laneBook.Find(x => x.Contains(past));
                RectangleF clipRect;
                for (int i = 0; i <= passingLanes && scoreLane != null; ++i)
                {
                    if (Status.DrawTickFirst < scoreLane.EndTick && scoreLane.StartTick < Status.DrawTickLast)
                    {
                        clipRect = new RectangleF(
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
                        using (Pen myPen = new Pen(lineColor, 2))
                        {
                            g.DrawLine(
                                myPen,
                                (graphicsPath.PathPoints[1].X + graphicsPath.PathPoints[2].X) / 2,
                                graphicsPath.PathPoints[1].Y,
                                (graphicsPath.PathPoints[0].X + graphicsPath.PathPoints[3].X) / 2,
                                graphicsPath.PathPoints[0].Y);
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
        }

        /// <summary>
        /// ノーツ間を繋ぐ帯の描画（ベジェ）
        /// </summary>
        private static void DrawSlideCurve(Graphics g, Note past, Note curve, Note future, Point drawLocation, LaneBook laneBook, ref RectangleF gradientRect)
        {
            if (gradientRect.Width <= 0) gradientRect.Width = 1;
            if (gradientRect.Height <= 0) gradientRect.Height = 1;
            //相対位置
            PointF pastRerativeLocation = new PointF(past.Location.X - drawLocation.X, past.Location.Y - drawLocation.Y);

            int passingLanes = future.LaneIndex - past.LaneIndex;

            float positionDistanceFuture = (future.Position.Tick - past.Position.Tick) * ScoreInfo.UnitBeatHeight;
            float positionDistanceCurve = (curve.Position.Tick - past.Position.Tick) * ScoreInfo.UnitBeatHeight;
            float diffXFuture = (future.Position.Lane - past.Position.Lane) * ScoreInfo.UnitLaneWidth;
            float diffXCurve = (curve.Position.Lane - past.Position.Lane) * ScoreInfo.UnitLaneWidth;

            //ノーツfutureの位置はノーツpastの位置に2ノーツの距離を引いて表す。またTopRightの水平位置はfutureのWidthを使うことに注意
            PointF topLeft = pastRerativeLocation.Add(diffXFuture, -positionDistanceFuture).Add(drawOffset);
            PointF topRight = pastRerativeLocation.Add(diffXFuture, -positionDistanceFuture).Add(-drawOffset.X, drawOffset.Y).AddX(future.Width);
            //以下の2つはレーンをまたがないときと同じ
            PointF bottomLeft = pastRerativeLocation.Add(drawOffset).AddY(deltaHeight);
            PointF bottomRight = pastRerativeLocation.Add(-drawOffset.X, drawOffset.Y).AddX(past.Width).AddY(deltaHeight);
            //3つのそれぞれのノーツの中心の座標
            PointF topCenter = topLeft.AddX(future.Width / 2f - drawOffset.X);
            PointF bottomCenter = bottomLeft.AddX(past.Width / 2f - drawOffset.X);
            PointF curveCenter = pastRerativeLocation.Add(diffXCurve, -positionDistanceCurve).AddX(curve.Width / 2f);
            //
            //下からアンカーまでの比率
            float ratio = (curveCenter.Y - bottomCenter.Y) / (topCenter.Y - bottomCenter.Y);
            //カーブノーツのY座標で水平にスライドを切ったときのスライド幅
            float widthAnchor = (topRight.X - topLeft.X) * ratio + (bottomRight.X - bottomLeft.X) * (1 - ratio);
            using (GraphicsPath graphicsPath = new GraphicsPath())
            {
                graphicsPath.AddBezier(bottomLeft, curveCenter.AddX(-widthAnchor / 2f), topLeft);
                graphicsPath.AddLine(topLeft, topRight);
                graphicsPath.AddBezier(topRight, curveCenter.AddX(widthAnchor / 2f), bottomRight);
                graphicsPath.AddLine(bottomLeft, bottomRight);
                ScoreLane scoreLane = laneBook.Find(x => x.Contains(past));
                RectangleF clipRect;
                for (int i = 0; i <= passingLanes && scoreLane != null; ++i)
                {
                    if (Status.DrawTickFirst < scoreLane.EndTick && scoreLane.StartTick < Status.DrawTickLast)
                    {
                        clipRect = new RectangleF(
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
                        using (Pen myPen = new Pen(lineColor, 2))
                        {
                            g.DrawBezier(myPen, bottomCenter, curveCenter, topCenter);
                        }
                    }
                    // インクリメント
                    bottomCenter = bottomCenter.Add(ScoreLane.Width + ScorePanel.Margin.Left + ScorePanel.Margin.Right, scoreLane.HitRect.Height);
                    curveCenter = curveCenter.Add(ScoreLane.Width + ScorePanel.Margin.Left + ScorePanel.Margin.Right, scoreLane.HitRect.Height);
                    topCenter = topCenter.Add(ScoreLane.Width + ScorePanel.Margin.Left + ScorePanel.Margin.Right, scoreLane.HitRect.Height);
                    if (i != passingLanes)
                    {
                        graphicsPath.Translate(ScoreLane.Width + ScorePanel.Margin.Left + ScorePanel.Margin.Right, scoreLane.HitRect.Height);
                        gradientRect.Y += scoreLane.HitRect.Height;
                        scoreLane = laneBook.Next(scoreLane);
                    }
                }
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
            using (Pen myPen = new Pen(lineColor, 2))
            {
                e.Graphics.DrawLine(myPen, location.X, drawRect.Bottom, location.X, drawRect.Top);
            }
        }
    }
}
