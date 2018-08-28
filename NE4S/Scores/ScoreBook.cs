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

        public void InsertRange(int index, List<Score> newScores)
        {
            scores.InsertRange(index, newScores);
            SetScoreIndex();
        }

        public void Delete(int begin, int count)
        {
            if (scores.Count < begin + count) count = scores.Count - begin;
            scores.RemoveRange(begin, count);
            SetScoreIndex();
#if DEBUG
            System.Diagnostics.Debug.WriteLine(scores.Count.ToString());
#endif
        }

        public Score At(int index)
        {
            if (scores.Count <= index) return null;
            return scores.ElementAt(index);
        }

        /// <summary>
        /// scores内のScoreのインデックスを更新
        /// リストscoresの内容が変更された際に必ず呼ぶ
        /// </summary>
        private void SetScoreIndex()
        {
            for (int i = 0; i < scores.Count; ++i) scores[i].ScoreIndex = i;
        }
    }
}
