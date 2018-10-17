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
		private LaneBook laneBook;

        public Model()
        {
            noteBook = new NoteBook();
            scoreBook = new ScoreBook();
			laneBook = new LaneBook();
        }

		public void SetScore(int beatNumer, int beatDenom, int barCount)
		{
			laneBook.SetScore(scoreBook, beatNumer, beatDenom, barCount);
		}

		public void InsertScoreForward(Score score, int beatNumer, int beatDenom, int barCount)
		{
			laneBook.InsetScoreForward(scoreBook, score, beatNumer, beatDenom, barCount);
		}

		public void InsertScoreBackward(Score score, int beatNumer, int beatDenom, int barCount)
		{
			laneBook.InsertScoreBackward(scoreBook, score, beatNumer, beatDenom, barCount);
		}

		public void DivideLane(Score score)
		{
			laneBook.DivideLane(score);
		}

		public void DeleteScore(Score score, int count)
		{
			laneBook.DeleteScore(scoreBook, score, count);
		}

		public void FillLane()
		{
			if (laneBook.Any()) FillLane(laneBook.First());
		}

		public void FillLane(ScoreLane begin)
		{
			laneBook.FillLane(begin);
		}

		public NoteBook NoteBook
		{
			get { return noteBook; }
		}

		public ScoreBook ScoreBook
		{
			get { return scoreBook; }
		}

		public LaneBook LaneBook
		{
			get { return laneBook; }
		}
    }
}
