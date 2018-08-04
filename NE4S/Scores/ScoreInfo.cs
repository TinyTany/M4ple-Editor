using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Scores
{
    /// <summary>
    /// 譜面そのものを作るのにとりあえず必要な値たち
    /// </summary>
    public class ScoreInfo
    {
        private double width, height;
        private const double laneWidth = 12, laneHeight = 2;
        private const double leftMargin = 30, rightMargin = 30, topMargin = 5, bottomMargin = 5;
        private const int maxBeatDiv = 192;

        /// <summary>
        /// beatNumber/beatDenom 拍子の譜面を一小節分つくる
        /// </summary>
        /// <param name="beatNumer"></param>
        /// <param name="beatDenom"></param>
        public ScoreInfo(int beatNumer, int beatDenom)
        {
            width = leftMargin + rightMargin + laneWidth * 16;
            if(beatDenom != 0) height = topMargin + bottomMargin + laneHeight * maxBeatDiv * beatNumer / beatDenom;
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
