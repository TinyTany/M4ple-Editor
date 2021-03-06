﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;
using NE4S.Scores;
using System.Windows.Forms;
using System.Drawing;

namespace NE4S
{
    [Serializable()]
    public class Model
    {
        public NoteBook NoteBook { get; }
        public ScoreBook ScoreBook { get; }
        public LaneBook LaneBook { get; }
        public MusicInfo MusicInfo { get; set; }

        /* Serializableぶっ壊れるけど、SUSで完全代用できるようになるからいらないよね!!(暴論) */
        /* と思ったけど、シリアライズ対象から外せば問題ないよね!!(わからん) */
        [field: NonSerialized]
        public HighSpeedTimeLineBook TimeLineBook { get; set; } = new HighSpeedTimeLineBook();

        public Model()
        {
            NoteBook = new NoteBook();
            ScoreBook = new ScoreBook();
			LaneBook = new LaneBook();
            LaneBook.UpdateNoteLocation += NoteBook.UpdateNoteLocation;
            MusicInfo = new MusicInfo();
        }

		public void SetScore(int beatNumer, int beatDenom, int barCount)
		{
			LaneBook.SetScore(ScoreBook, beatNumer, beatDenom, barCount);
		}

		public void InsertScoreForward(Score score, int beatNumer, int beatDenom, int barCount)
		{
			LaneBook.InsetScoreForward(ScoreBook, score, beatNumer, beatDenom, barCount);
		}

        public void InsertScoreForwardWithNote(Score score, int beatNumer, int beatDenom, int barCount)
        {
            LaneBook.InsertScoreForwardWithNote(NoteBook, ScoreBook, score, beatNumer, beatDenom, barCount);
        }


        public void InsertScoreBackward(Score score, int beatNumer, int beatDenom, int barCount)
		{
			LaneBook.InsertScoreBackward(ScoreBook, score, beatNumer, beatDenom, barCount);
        }

        public void InsertScoreBackwardWithNote(Score score, int beatNumer, int beatDenom, int barCount)
        {
            LaneBook.InsertScoreBackwardWithNote(NoteBook, ScoreBook, score, beatNumer, beatDenom, barCount);
        }

        public void DivideLane(Score score)
		{
            LaneBook.DivideLane(score);
        }

		public void DeleteScore(Score score, int count)
		{
			LaneBook.DeleteScore(ScoreBook, score, count);
        }

        public void FillLane()
		{
			if (LaneBook.Any()) FillLane(LaneBook.First());
		}

		public void FillLane(ScoreLane begin)
		{
			LaneBook.FillLane(begin);
        }

        public void PaintNote(Graphics g, Point drawLocation)
        {
            NoteBook.Paint(g, drawLocation, LaneBook);
        }

        public Slide SelectedSlide(PointF location)
        {
            return NoteBook.SelectedSlide(location, LaneBook);
        }

        public AirHold SelectedAirHold(PointF location)
        {
            return NoteBook.SelectedAirHold(location, LaneBook);
        }
    }
}
