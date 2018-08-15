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

        public Score(int beatNumer, int beatDenom)
        {
            this.beatNumer = beatNumer;
            this.beatDenom = beatDenom;
            width = ScoreInfo.LaneWidth * ScoreInfo.Lanes;
            height = ScoreInfo.LaneHeight * ScoreInfo.MaxBeatDiv * beatNumer / beatDenom;
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
