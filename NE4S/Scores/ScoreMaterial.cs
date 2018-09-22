﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NE4S.Scores
{
	public class ScoreMaterial
	{
		private Score score;
		private Range scoreRange;
		private RectangleF hitRect;

		public ScoreMaterial(Score score, Range scoreRange, RectangleF hitRect)
		{
			this.score = score;
			this.scoreRange = scoreRange;
			this.hitRect = hitRect;
		}

#if DEBUG
		/// <summary>
		/// 試作
		/// </summary>
		public void CalculatePos(int pX, int pY)
		{
			PointF normalizedPos = new PointF(
				pX - hitRect.X,
				hitRect.Height - (pY - hitRect.Y) + ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv * (scoreRange.Inf - 1) / score.BeatDenom
				);
			Pos pos = new Pos(score.Index + 1, (int)(normalizedPos.Y / ScoreInfo.MaxBeatHeight), ScoreInfo.MaxBeatDiv, (int)(normalizedPos.X / ScoreInfo.MinLaneWidth));
			pos.PrintPos();
		}

		public void CalculatePos(Point p)
		{
			CalculatePos(p.X, p.Y);
		}
#endif

		public Score Score
		{
			get { return score; }
		}

		public Range Range
		{
			get { return scoreRange; }
		}

		public RectangleF HitRect
		{
			get { return hitRect; }
		}
	}
}