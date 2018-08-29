using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;
using NE4S.Scores;

namespace NE4S
{
    public class Model
    {
        private NoteBook noteBook;
        private ScoreBook scoreBook;

        public Model()
        {
            noteBook = new NoteBook();
            scoreBook = new ScoreBook();
        }

        public void AddScore(int beatNumer, int beatDenom, int barCount)
        {
            for(int i = 0; i < barCount; ++i)
            {
                scoreBook.Add(beatNumer, beatDenom);
            }
        }

        public void AppendScore(List<Score> newScores)
        {
            scoreBook.Append(newScores);
        }

        public void InsertScore(int index, List<Score> newScores)
        {
            scoreBook.InsertRange(index, newScores);
        }

        public void DeleteScore(int begin, int count)
        {
            scoreBook.Delete(begin, count);
        }

        public Score ScorePrev(Score score)
        {
            return ScoreAt(score.ScoreIndex - 1);
        }

        public Score ScoreNext(Score score)
        {
            return ScoreAt(score.ScoreIndex + 1);
        }

        public Score ScoreAt(int index)
        {
            return scoreBook.At(index);
        }

        public Score ScoreLast()
        {
            return scoreBook.Last();
        }
    }
}
