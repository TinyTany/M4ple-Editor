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
        private int width, height;

        struct ScoreInfo
        {
            private const double laneWidth = 12, laneHeight = 2;
            private const int maxBeatDiv = 192;
        }

        public Score(int beatNumer, int beatDenom)
        {
            this.beatNumer = beatNumer;
            this.beatDenom = beatDenom;
        }
    }
}
