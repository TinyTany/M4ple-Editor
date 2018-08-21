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
    public class Range
    {
        public int Inf { get; set; }
        public int Sup { get; set; }

        /// <summary>
        /// Scoreの範囲[Inf, Sup]を設定
        /// 範囲は1 ≦ Inf ＜ Sup ≦ bearNumer
        /// </summary>
        /// <param name="inf">下限</param>
        /// <param name="sup">上限</param>
        public Range(int inf, int sup)
        {
            Inf = inf;
            Sup = sup;
        }

        public int Size()
        {
            return Sup - Inf + 1;
        }
    }
}
