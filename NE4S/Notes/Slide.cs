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
        private void DrawSlideLine(PaintEventArgs e, Note past, Note future, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook)
        {
            //帯の描画位置がちょっと上にずれてるので調節用の変数を用意
            int MarginX = 2, dY = 2;
            //相対位置
            PointF pastRerativeLocation = new PointF(past.Location.X - originPosX, past.Location.Y - originPosY);
            PointF futureRerativeLocation = new PointF(future.Location.X - originPosX, future.Location.Y - originPosY);
            
            int passingLanes = future.LaneIndex - past.LaneIndex;
            if(passingLanes == 0)
            {
                PointF TopLeft = new PointF(futureRerativeLocation.X + MarginX, futureRerativeLocation.Y + dY);
                PointF TopRight = new PointF(futureRerativeLocation.X + future.Width - MarginX, futureRerativeLocation.Y + dY);
                PointF BottomLeft = new PointF(pastRerativeLocation.X + MarginX, pastRerativeLocation.Y + dY);
                PointF BottomRight = new PointF(pastRerativeLocation.X + past.Width - MarginX, pastRerativeLocation.Y + dY);
                using (GraphicsPath graphicsPath = new GraphicsPath())
                {
                    graphicsPath.AddLines(new PointF[] { TopLeft, BottomLeft, BottomRight, TopRight });
                    using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(200, 0, 170, 255)))
                    {
                        e.Graphics.FillPath(myBrush, graphicsPath);
                    }
                }
            }
            else if(passingLanes >= 1)
            {
                float positionDistance = PositionDistance(past.Pos, future.Pos, scoreBook);
                #region 最初のレーンでの描画
                {
                    float diffX = (future.Pos.Lane - past.Pos.Lane) * ScoreInfo.MinLaneWidth;
                    
                    //ノーツfutureの位置はノーツpastの位置に2ノーツの距離を引いて表す。またTopRightの水平位置はfutureのWidthを使うことに注意
                    PointF TopLeft = new PointF(pastRerativeLocation.X + diffX + MarginX, pastRerativeLocation.Y - positionDistance + dY);
                    PointF TopRight = new PointF(pastRerativeLocation.X + diffX + future.Width - MarginX, pastRerativeLocation.Y - positionDistance + dY);
                    //以下の2つはレーンをまたがないときと同じ
                    PointF BottomLeft = new PointF(pastRerativeLocation.X + MarginX, pastRerativeLocation.Y + dY);
                    PointF BottomRight = new PointF(pastRerativeLocation.X + past.Width - MarginX, pastRerativeLocation.Y + dY);
                    using (GraphicsPath graphicsPath = new GraphicsPath())
                    {
                        graphicsPath.AddLines(new PointF[] { TopLeft, BottomLeft, BottomRight, TopRight });
                        ScoreLane scoreLane = laneBook.Find(x => x.Contains(past));
                        if (scoreLane != null) e.Graphics.Clip = new Region(scoreLane.HitRect);
                        using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(200, 0, 170, 255)))
                        {
                            e.Graphics.FillPath(myBrush, graphicsPath);
                        }
                    }
                }
                #endregion
                #region 中間のレーンたちでの描画
                {
                    //TODO: ここらへんの描画処理をちゃんと実装する
                    for (int i = past.LaneIndex + 1; i <= future.LaneIndex - 1; ++i)
                    {
                        PointF TopLeft = new PointF(futureRerativeLocation.X + MarginX, futureRerativeLocation.Y + dY);
                        PointF TopRight = new PointF(futureRerativeLocation.X + future.Width - MarginX, futureRerativeLocation.Y + dY);
                        PointF BottomLeft = new PointF(pastRerativeLocation.X + MarginX, pastRerativeLocation.Y + dY);
                        PointF BottomRight = new PointF(pastRerativeLocation.X + past.Width - MarginX, pastRerativeLocation.Y + dY);
                        using (GraphicsPath graphicsPath = new GraphicsPath())
                        {
                            graphicsPath.AddLines(new PointF[] { TopLeft, BottomLeft, BottomRight, TopRight });
                            using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(200, 0, 170, 255)))
                            {
                                e.Graphics.FillPath(myBrush, graphicsPath);
                            }
                        }
                    }
                }
                #endregion
                #region 最後のレーンでの描画
                {
                    float diffX = (past.Pos.Lane - future.Pos.Lane) * ScoreInfo.MinLaneWidth;
                    
                    //以下の2つはレーンをまたがないときと同じ
                    PointF TopLeft = new PointF(futureRerativeLocation.X + MarginX, futureRerativeLocation.Y + dY);
                    PointF TopRight = new PointF(futureRerativeLocation.X + future.Width - MarginX, futureRerativeLocation.Y + dY);
                    //ノーツpastの位置はノーツfutureの位置に2ノーツの距離を足して表す。またBottomRightの水平位置はpastのWidthを使うことに注意
                    PointF BottomLeft = new PointF(futureRerativeLocation.X + diffX + MarginX, futureRerativeLocation.Y + positionDistance + dY);
                    PointF BottomRight = new PointF(futureRerativeLocation.X + diffX + past.Width - MarginX, futureRerativeLocation.Y + positionDistance + dY);
                    using (GraphicsPath graphicsPath = new GraphicsPath())
                    {
                        graphicsPath.AddLines(new PointF[] { TopLeft, BottomLeft, BottomRight, TopRight });
                        ScoreLane scoreLane = laneBook.Find(x => x.Contains(future));
                        if (scoreLane != null) e.Graphics.Clip = new Region(scoreLane.HitRect);
                        using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(200, 0, 170, 255)))
                        {
                            e.Graphics.FillPath(myBrush, graphicsPath);
                        }
                    }
                }
                #endregion
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
                e.Graphics.ResetClip();
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
