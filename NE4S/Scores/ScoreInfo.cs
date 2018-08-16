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
        /// <summary>
        /// 1最小レーンの横幅
        /// </summary>
        public static double LaneWidth { get; } = 12;
        /// <summary>
        /// 最大分数時の縦幅
        /// </summary>
        public static double LaneHeight { get; } = 2;
        /// <summary>
        /// 最大分数
        /// </summary>
        public static int MaxBeatDiv { get; } = 192;
        /// <summary>
        /// 各レーンでの4/4拍子が最大何小節分入るか
        /// </summary>
        public static double BarPerLane { get; } = 2;
        /// <summary>
        /// レーン数（不変）
        /// </summary>
        public static int Lanes { get; } = 16;

        /// <summary>
        /// ScoreLaneのMargin
        /// </summary>
        public class LaneMargin
        {
            public static int Top { get; } = 5;
            public static int Bottom { get; } = 5;
            public static int Left { get; } = 30;
            public static int Right { get; } = 30;
        }

        /// <summary>
        /// ScorePanelのMargin
        /// </summary>
        public class PanelMargin
        {
            public static int Top { get; } = 10;
            public static int Bottom { get; } = 10;
            public static int Left { get; } = 10;
            public static int Right { get; } = 10;
        }
    }
}
