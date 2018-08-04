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
    public class Score : ScoreInfo
    {
        private int beatNumer, beatDenom;

        public Score(int beatNumer, int beatDenom) : base(beatNumer, beatDenom)
        {
            this.beatNumer = beatNumer;
            this.beatDenom = beatDenom;
        }
    }
}
