using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NE4S.Scores
{
    [Serializable()]
	public class ScoreMaterial
	{
        public Score Score { get; }
        public Range Range { get; }
        public RectangleF HitRect { get; set; }
        public float Width { get; }
        public float Height { get; }
        public int StartTick { get; private set; }

        public ScoreMaterial(Score score, Range scoreRange, RectangleF hitRect)
		{
			Score = score;
			Range = scoreRange;
			HitRect = hitRect;
			Width = score.Width;
			Height = score.Height * scoreRange.Size / score.BeatNumer;
		}

        /// <summary>
        /// ノーツの相対位置情報を返す
        /// タプルに入ってる値はレーン，tickの順
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Position GetPosition(PointF location)
        {
            //原点をLane=0, Tick = StartTickにする
            PointF normalizedPos = new PointF(
                location.X - HitRect.X,
                location.Y - (HitRect.Y + HitRect.Height));
            Position position = new Position(
                (int)(normalizedPos.X / ScoreInfo.MinLaneWidth),
                StartTick + -(int)(normalizedPos.Y / ScoreInfo.MaxBeatHeight));
            return position;
        }
    }
}