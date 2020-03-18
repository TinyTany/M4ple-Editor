using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S
{
    /// <summary>
    /// ScoreLaneが持つ各Scoreの範囲
    /// </summary>
    [Serializable()]
    public class ScoreRange
    {
        public int Min { get; set; }
        public int Max { get; set; }

        /// <summary>
        /// Scoreの範囲[Min, Max]を設定
        /// 範囲は1 ≦ Min ＜ Max ≦ bearNumer
        /// </summary>
        /// <param name="min">下限</param>
        /// <param name="max">上限</param>
        public ScoreRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Rangeの範囲サイズ
        /// </summary>
        /// <returns></returns>
        public int Size
        {
            get
            {
                return Max - Min + 1;
            }
        }
    }
}
