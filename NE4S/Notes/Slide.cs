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
        public Slide()
        {

        }

        public Slide(int size, Position pos, PointF location, int laneIndex)
        {
            SlideBegin slideBegin = new SlideBegin(size, pos, location);
            slideBegin.LaneIndex = laneIndex;
            Add(slideBegin);
            //TODO: posとかlocationとかをいい感じに設定する
            location.Y -= ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv / Status.Beat;
            SlideEnd slideEnd = new SlideEnd(size, ++pos, location);
            slideEnd.LaneIndex = laneIndex;
            Add(slideEnd);
            Status.selectedNote = slideEnd;
        }

        /// <summary>
        /// ノーツ間を繋ぐ帯の描画
        /// </summary>
        /// <param name="past"></param>
        /// <param name="future"></param>
        private void DrawSlideLine(PaintEventArgs e, Note past, Note future, int originPosX, int originPosY, ScoreBook scoreBook)
        {
            //帯の描画位置がちょっと上にずれてるので調節用の変数を用意
            int MarginX = 2, dY = 2;
            //TODO: レーンをまたぐときの描画処理実装する
            int passingLanes = future.LaneIndex - past.LaneIndex;
            if(passingLanes == 0)
            {
                GraphicsPath graphicsPath = new GraphicsPath();
                PointF TopLeft = new PointF(future.Location.X - originPosX + MarginX, future.Location.Y - originPosY + dY);
                PointF TopRight = new PointF(future.Location.X + future.Width - originPosX - MarginX, future.Location.Y - originPosY + dY);
                PointF BottomLeft = new PointF(past.Location.X - originPosX + MarginX, past.Location.Y - originPosY + dY);
                PointF BottomRight = new PointF(past.Location.X + past.Width - originPosX - MarginX, past.Location.Y - originPosY + dY);
                graphicsPath.AddLines(new PointF[] { TopLeft, BottomLeft, BottomRight, TopRight });
                using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(200, 0, 170, 255)))
                {
                    e.Graphics.FillPath(myBrush, graphicsPath);
                }
            }
            else if(passingLanes >= 1)
            {
                float positionDistance = PositionDistance(past.Pos, future.Pos, scoreBook);
                {
                    float diffX = (future.Pos.Lane - past.Pos.Lane) * ScoreInfo.MinLaneWidth;
                    GraphicsPath graphicsPath = new GraphicsPath();
                    PointF TopLeft = new PointF(past.Location.X + diffX - originPosX + MarginX, past.Location.Y - positionDistance - originPosY + dY);
                    PointF TopRight = new PointF(past.Location.X + diffX + future.Width - originPosX - MarginX, past.Location.Y - positionDistance - originPosY + dY);
                    PointF BottomLeft = new PointF(past.Location.X - originPosX + MarginX, past.Location.Y - originPosY + dY);
                    PointF BottomRight = new PointF(past.Location.X + past.Width - originPosX - MarginX, past.Location.Y - originPosY + dY);
                    graphicsPath.AddLines(new PointF[] { TopLeft, BottomLeft, BottomRight, TopRight });
                    using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(200, 0, 170, 255)))
                    {
                        e.Graphics.FillPath(myBrush, graphicsPath);
                    }
                }
                {
                    float diffX = (past.Pos.Lane - future.Pos.Lane) * ScoreInfo.MinLaneWidth;
                    GraphicsPath graphicsPath = new GraphicsPath();
                    PointF TopLeft = new PointF(future.Location.X - originPosX + MarginX, future.Location.Y - originPosY + dY);
                    PointF TopRight = new PointF(future.Location.X + future.Width - originPosX - MarginX, future.Location.Y - originPosY + dY);
                    PointF BottomLeft = new PointF(future.Location.X + diffX - originPosX + MarginX, future.Location.Y + positionDistance - originPosY + dY);
                    PointF BottomRight = new PointF(future.Location.X + diffX + past.Width - originPosX - MarginX, future.Location.Y + positionDistance - originPosY + dY);
                    graphicsPath.AddLines(new PointF[] { TopLeft, BottomLeft, BottomRight, TopRight });
                    using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(200, 0, 170, 255)))
                    {
                        e.Graphics.FillPath(myBrush, graphicsPath);
                    }
                }
            }
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook)
		{
			foreach(Note note in this)
            {
                if (!(note is SlideEnd))
                {
                    Note next = this.ElementAt(IndexOf(note) + 1);
                    DrawSlideLine(e, note, next, originPosX, originPosY, scoreBook, laneBook);
                }
                note.Draw(e, originPosX, originPosY);
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
