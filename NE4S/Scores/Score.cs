using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Scores
{
    /// <summary>
    /// 譜面そのもの
    /// </summary>
    public class Score
    {
        private int beatNumer, beatDenom;
        private double width, height;

        struct ScoreInfo
        {
            public const double laneWidth = 12, laneHeight = 2;
            public const int maxBeatDiv = 192, lanes = 16;
        }

        public Score(int beatNumer, int beatDenom)
        {
            this.beatNumer = beatNumer;
            this.beatDenom = beatDenom;
            width = ScoreInfo.laneWidth * ScoreInfo.lanes;
            height = ScoreInfo.laneHeight * ScoreInfo.maxBeatDiv * beatNumer / beatDenom;
        }

        public double Width
        {
            get { return this.width; }
        }

        public double Height
        {
            get { return this.height; }
        }
    }
}
