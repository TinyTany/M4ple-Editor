using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Scores
{
    /// <summary>
    /// 各種譜面関係部品に関わる定数たち
    /// </summary>
    public class ScoreInfo
    {
        public static double LaneWidth { get; } = 12;
        public static double LaneHeight { get; } = 2;
        public static int MaxBeatDiv { get; } = 192;
        public static int Lanes { get; } = 16;

        public class LaneMargin
        {
            public static int Top { get; } = 5;
            public static int Bottom { get; } = 5;
            public static int Left { get; } = 30;
            public static int Right { get; } = 30;
        }

        public class PanelMargin
        {
            public static int Top { get; } = 10;
            public static int Bottom { get; } = 10;
            public static int Left { get; } = 10;
            public static int Right { get; } = 10;
        }
    }
}
