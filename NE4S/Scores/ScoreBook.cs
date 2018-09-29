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
    public class ScoreBook : List<Score>
    {
        public ScoreBook()
        {
            
        }

        public void Add(int beatNumer, int beatDenom)
        {
			Add(new Score(beatNumer, beatDenom));
            SetScoreIndex();
        }

        public void Append(List<Score> newScores)
        {
            AddRange(newScores);
            SetScoreIndex();
        }

        public void InsertRange(int index, List<Score> newScores)
        {
            base.InsertRange(index, newScores);
            SetScoreIndex();
        }

        public void Delete(int begin, int count)
        {
            if (Count < begin + count) count = Count - begin;
            RemoveRange(begin, count);
            SetScoreIndex();
        }

        public Score At(int index)
        {
            if (Count <= index) return null;
            return this.ElementAt(index);
        }

        /// <summary>
        /// scores内のScoreのインデックスを更新
        /// リストscoresの内容が変更された際に必ず呼ぶ
        /// </summary>
        private void SetScoreIndex()
        {
            for (int i = 0; i < Count; ++i) this[i].Index = i;
#if DEBUG
            System.Diagnostics.Debug.WriteLine("ScoreCount : " + Count.ToString());
#endif
        }
    }
}
