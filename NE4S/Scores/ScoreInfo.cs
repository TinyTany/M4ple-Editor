using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Scores
{
    public class ScoreInfo
    {
        private int width, height;
        private const int laneWidth = 12, laneHeight = 2;
        private const int leftMargin = 30, rightMargin = 30, topMargin = 5, bottomMargin = 5;
        private const int maxBeatDiv = 192;

        public ScoreInfo(int beatNumer, int beatDenom)
        {
            width = leftMargin + rightMargin + laneWidth * 16;
            height = topMargin + bottomMargin + laneHeight * maxBeatDiv * beatNumer / beatDenom;
        }

        public int Width
        {
            get { return this.width; }
        }

        public int Height
        {
            get { return this.height; }
        }
    }
}
