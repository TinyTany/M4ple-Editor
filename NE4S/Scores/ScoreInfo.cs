﻿using System;
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
        /// 譜面パネルの定数
        /// </summary>
        public static float ScaleConstant { get; set; } = .5f;
        /// <summary>
        /// 1最小レーンの横幅
        /// </summary>
        public static float UnitLaneWidth { get; } = 12;
        /// <summary>
        /// 最大分数時の縦幅
        /// </summary>
        public static float UnitBeatHeight { get; set; } = .25f;
        /// <summary>
        /// 最大分数
        /// 1tick
        /// </summary>
        public static int MaxBeatDiv { get; } = 1536;
        /// <summary>
        /// 各レーンでの4/4拍子が最大何小節分入るか
        /// </summary>
        public static float LaneMaxBar { get; set; } = 2;
        /// <summary>
        /// レーン数（不変）
        /// </summary>
        public static int Lanes { get; } = 16;
        /// <summary>
        /// フォントサイズ（小節番号など）
        /// </summary>
        public static float FontSize { get; } = 8;
        /// <summary>
        /// ノーツの縦幅
        /// </summary>
        public static float NoteHeight { get; } = 5;
    }
}
