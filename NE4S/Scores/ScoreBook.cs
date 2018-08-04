using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Scores
{
    /// <summary>
    /// 譜面そのものをすべてここで管理する
    /// </summary>
    public class ScoreBook
    {
        private List<Score> scores;

        public ScoreBook()
        {
            scores = new List<Score>();
        }
    }
}
