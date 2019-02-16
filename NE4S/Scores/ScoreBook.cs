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
    public class ScoreBook : List<Score>
    {
        #region 使わなくなったメンバ
        public event EventHandler DataChanged;
        #endregion

        public ScoreBook() { }

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
            if (index < 0 || index >= Count) return null;
            return this.ElementAt(index);
        }

		public Score Prev(Score score)
		{
            if (score == null) return this.Last();
			if (score.Index <= 0) return null;
			return this.ElementAt(score.Index - 1);
		}

		public Score Next(Score score)
		{
            if (score == null) return this.First();
			if (score.Index >= Count - 1) return null;
			return this.ElementAt(score.Index + 1);
		}

        /// <summary>
        /// ScoreBook内のScoreのインデックスとTick数と拍数の描画の有無を更新
        /// リストscoresの内容が変更された際に必ず呼ぶ
        /// </summary>
        private void SetScoreIndex()
        {
            for (int i = 0; i < Count; ++i)
            {
                this[i].Index = i;
                if (i == 0 || this[i].BeatDenom != this[i-1].BeatDenom || this[i].BeatNumer != this[i-1].BeatNumer)
                {
                    this[i].IsBeatVisible = true;
                }
                else
                {
                    this[i].IsBeatVisible = false;
                }
            }
            int tick = 0;
            foreach(Score score in this)
            {
                score.StartTick = tick;
                tick = score.EndTick + 1;
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("ScoreCount : " + Count.ToString());
#endif
        }
    }
}
