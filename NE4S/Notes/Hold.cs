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

        public override void Draw(PaintEventArgs e, Point drawLocation, LaneBook laneBook)
        {
            base.Draw(e, drawLocation, laneBook);
            var list = this.OrderBy(x => x.Position.Tick).ToList();
            foreach (Note note in list)
            {
                if(list.IndexOf(note) < list.Count - 1)
                {
                    Note next = list.Next(note);
                    DrawHoldLine(e, note, next, drawLocation, laneBook);
                }
                //クリッピングの解除を忘れないこと
                e.Graphics.ResetClip();
                note.Draw(e, drawLocation);
            }
        }

        private static void DrawHoldLine(PaintEventArgs e, Note past, Note future, Point drawLocation, LaneBook laneBook)
        {
            float distance = (future.Position.Tick - past.Position.Tick) * ScoreInfo.MaxBeatHeight;
            //グラデーション矩形
            //x座標と幅は適当だけど動いてるはず。重要なのはy座標と高さ
            RectangleF gradientRect = new RectangleF(0, past.Location.Y - distance + drawOffset.Y - drawLocation.Y, 10, distance <= 0 ? 1 : distance);
            //相対位置
            PointF pastRerativeLocation = new PointF(past.Location.X - drawLocation.X, past.Location.Y - drawLocation.Y);
            int passingLanes = future.LaneIndex - past.LaneIndex;
            float positionDistance = (future.Position.Tick - past.Position.Tick) * ScoreInfo.MaxBeatHeight;
            float diffX = (future.Position.Lane - past.Position.Lane) * ScoreInfo.MinLaneWidth;
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
                        e.Graphics.Clip = new Region(clipRect);
                        using (LinearGradientBrush myBrush = new LinearGradientBrush(gradientRect, baseColor, baseColor, LinearGradientMode.Vertical))
                        {
                            myBrush.InterpolationColors = colorBlend;
                            e.Graphics.FillPath(myBrush, graphicsPath);
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
