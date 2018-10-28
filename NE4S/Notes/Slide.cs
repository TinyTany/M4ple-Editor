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
        private void DrawSlideLine(PaintEventArgs e, Note past, Note future, int originPosX, int originPosY)
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

            }
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
		{
			foreach(Note note in this)
            {
                if (!(note is SlideEnd))
                {
                    Note next = this.ElementAt(IndexOf(note) + 1);
                    DrawSlideLine(e, note, next, originPosX, originPosY);
                }
                note.Draw(e, originPosX, originPosY);
            }
		}
	}
}
