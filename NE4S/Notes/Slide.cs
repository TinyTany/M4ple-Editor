using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;

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
        private void DrawSlideLine(PaintEventArgs e, Note past, Note future)
        {
            //TODO: まともに動かないので良さげに直す
            PointF[] lineRegion = new PointF[4] {
                future.Location,
                past.Location,
                new PointF(past.Location.X + past.Width, past.Location.Y),
                new PointF(future.Location.X + future.Width, future.Location.Y)
            };
            using (SolidBrush myBrush = new SolidBrush(Color.FromArgb(200, 0, 170, 255)))
            {
                e.Graphics.FillClosedCurve(myBrush, lineRegion);
            }
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
		{
			foreach(Note note in this)
            {
                if (!(note is SlideEnd))
                {
                    Note next = this.ElementAt(IndexOf(note));
                    DrawSlideLine(e, note, next);
                }
                note.Draw(e, originPosX, originPosY);
            }
		}
	}
}
