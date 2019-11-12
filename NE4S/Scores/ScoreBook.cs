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
    [Serializable()]
    public class ScoreBook// : List<Score>
    {
        private readonly List<Score> scores = new List<Score>();

        public ScoreBook() { }

        #region
        public Score First() => scores.First();
        public Score Last() => scores.Last();
        public IEnumerator<Score> GetEnumerator() => scores.GetEnumerator();
        public Score Find(Predicate<Score> p) => scores.Find(p);
        public IEnumerable<Score> Where(Func<Score, bool> p) => scores.Where(p);
        public void ForEach(Action<Score> a) => scores.ForEach(a);
        public void Add(Score score) => scores.Add(score);
        #endregion

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
        }

		public void Resize(int index, int beatNumer, int beatDenom)
		{
			if(At(index) != null)
			{
				At(index).BeatNumer = beatNumer;
				At(index).BeatDenom = beatDenom;
            }
		}

        public Score At(int index)
        {
            if (index < 0 || index >= scores.Count) return null;
            return scores.ElementAt(index);
        }

		public Score Prev(Score score)
		{
            if (score == null) return scores.Last();
			if (score.Index <= 0) return null;
			return scores.ElementAt(score.Index - 1);
		}

		public Score Next(Score score)
		{
            if (score == null) return scores.First();
			if (score.Index >= scores.Count - 1) return null;
			return scores.ElementAt(score.Index + 1);
		}

        /// <summary>
        /// ScoreBook内のScoreのインデックスとTick数と拍数の描画の有無を更新
        /// リストscoresの内容が変更された際に必ず呼ぶ
        /// </summary>
        public void SetScoreIndex()
        {
            for (int i = 0; i < scores.Count; ++i)
            {
                scores[i].Index = i;
                if (i == 0 || scores[i].BeatDenom != scores[i-1].BeatDenom || scores[i].BeatNumer != scores[i-1].BeatNumer)
                {
                    scores[i].IsBeatVisible = true;
                }
                else
                {
                    scores[i].IsBeatVisible = false;
                }
            }
            int tick = 0;
            foreach(Score score in scores)
            {
                score.StartTick = tick;
                tick = score.EndTick + 1;
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("ScoreCount : " + scores.Count.ToString());
#endif
        }
    }
}
