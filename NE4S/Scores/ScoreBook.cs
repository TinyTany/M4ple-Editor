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

        public void Add(int beatNumer, int beatDenom)
        {
            scores.Add(new Score(beatNumer, beatDenom));
            SetScoreIndex();
        }

        public void Append(List<Score> newScores)
        {
            scores.AddRange(newScores);
            SetScoreIndex();
        }

        private void SetScoreIndex()
        {
            for (int i = 0; i < scores.Count; ++i) scores[i].ScoreIndex = i + 1;
        }
    }
}
