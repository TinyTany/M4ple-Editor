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

        public ScoreMaterial(Score score, Range scoreRange, RectangleF hitRect)
		{
			Score = score;
			Range = scoreRange;
			HitRect = hitRect;
			Width = score.Width;
			Height = score.Height * scoreRange.Size() / score.BeatNumer;
		}

#if DEBUG
		/// <summary>
		/// マウス座標（グリッド）から譜面座標を計算
		/// </summary>
		public Position CalculatePos(int pX, int pY)
		{
			PointF normalizedPos = new PointF(
				pX - HitRect.X,
				HitRect.Height - (pY - HitRect.Y) + ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv * (Range.Min - 1) / Score.BeatDenom
				);
			Position pos = new Position(Score.Index + 1, (int)(normalizedPos.Y / ScoreInfo.MaxBeatHeight), ScoreInfo.MaxBeatDiv, (int)(normalizedPos.X / ScoreInfo.MinLaneWidth));
			return pos;
		}

		public Position CalculatePos(Point location)
		{
			return CalculatePos(location.X, location.Y);
		}
#endif
    }
}
