using System;
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
        /// <summary>
        /// この変数は使用しない
        /// プロパティを常に使う
        /// </summary>
        private bool isEditedWithoutSave;
        public bool IsEditedWithoutSave
        {
            get { return isEditedWithoutSave; }
            set
            {
                isEditedWithoutSave = value;
                IsEditedWithoutSaveChanged?.Invoke(isEditedWithoutSave);
            }
        }
        public delegate void EditedStatusHandler(bool isEditedWithoutSave);
        [field: NonSerialized]
        public event EditedStatusHandler IsEditedWithoutSaveChanged;

        public Model()
        {
            NoteBook = new NoteBook();
            ScoreBook = new ScoreBook();
			LaneBook = new LaneBook();
            LaneBook.UpdateNoteLocation += NoteBook.UpdateNoteLocation;
            IsEditedWithoutSave = false;
        }

		public void SetScore(int beatNumer, int beatDenom, int barCount)
		{
			LaneBook.SetScore(ScoreBook, beatNumer, beatDenom, barCount);
		}

		public void InsertScoreForward(Score score, int beatNumer, int beatDenom, int barCount)
		{
			LaneBook.InsetScoreForward(ScoreBook, score, beatNumer, beatDenom, barCount);
            IsEditedWithoutSave = true;
		}

		public void InsertScoreBackward(Score score, int beatNumer, int beatDenom, int barCount)
		{
			LaneBook.InsertScoreBackward(ScoreBook, score, beatNumer, beatDenom, barCount);
            IsEditedWithoutSave = true;
        }

		public void DivideLane(Score score)
		{
			LaneBook.DivideLane(score);
            IsEditedWithoutSave = true;
        }

		public void DeleteScore(Score score, int count)
		{
			LaneBook.DeleteScore(ScoreBook, score, count);
            IsEditedWithoutSave = true;
        }

		public void FillLane()
		{
			if (LaneBook.Any()) FillLane(LaneBook.First());
		}

		public void FillLane(ScoreLane begin)
		{
			LaneBook.FillLane(begin);
            IsEditedWithoutSave = true;
        }

		public void AddNote(Note newNote)
		{
			NoteBook.Add(newNote);
            IsEditedWithoutSave = true;
        }

		public void AddLongNote(LongNote newLongNote)
		{
			NoteBook.Add(newLongNote);
            IsEditedWithoutSave = true;
        }

        public void PaintNote(PaintEventArgs e, int originPosX, int originPosY, int currentPositionX)
        {
            NoteBook.Paint(e, originPosX, originPosY, ScoreBook, LaneBook, currentPositionX);
        }

        public Slide SelectedSlide(PointF location)
        {
            return NoteBook.SelectedSlide(location, ScoreBook, LaneBook);
        }

        public AirHold SelectedAirHold(PointF location)
        {
            return NoteBook.SelectedAirHold(location, ScoreBook, LaneBook);
        }
    }
}
