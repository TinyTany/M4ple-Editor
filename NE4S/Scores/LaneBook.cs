using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Scores
{
    public class LaneBook : List<ScoreLane>
    {
        public LaneBook()
        {
            
        }

        /// <summary>
        /// 指定されたlaneの次の要素を返します
        /// </summary>
        /// <param name="lane"></param>
        /// <returns></returns>
        public ScoreLane Next(ScoreLane lane)
        {
            if(!Contains(lane) || IndexOf(lane) == Count - 1)
            {
                return null;
            }
            else
            {
                return this.ElementAt(IndexOf(lane) + 1);
            }
        }
    }
}
