using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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
        public static float MinLaneWidth { get; } = 12;
        /// <summary>
        /// 最大分数時の縦幅
        /// </summary>
        public static float MaxBeatHeight { get; } = 2;
        /// <summary>
        /// 最大分数
        /// </summary>
        public static int MaxBeatDiv { get; } = 192;
        /// <summary>
        /// 各レーンでの4/4拍子が最大何小節分入るか
        /// </summary>
        public static float LaneMaxBar { get; } = 2;
        /// <summary>
        /// レーン数（不変）
        /// </summary>
        public static int Lanes { get; } = 16;
        /// <summary>
        /// フォントサイズ（小節番号など）
        /// </summary>
        public static float FontSize { get; } = 8;
        /// <summary>
        /// 小節番号描画時の位置ずれ分の座標
        /// </summary>
        public static PointF ScoreIndexPos { get; } = new PointF(-26.5f, -7);
        /// <summary>
        /// ノーツの縦幅
        /// </summary>
        public static float NoteHeight { get; } = 5;

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
