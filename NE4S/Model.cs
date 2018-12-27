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
    public delegate void EditedStatusHandler(bool isEditedWithoutSave);

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
        private MusicInfo musicInfo;
        public MusicInfo MusicInfo {
            get { return musicInfo; }
            set
            {
                musicInfo = value;
                IsEditedWithoutSave = true;
            }
        }
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
        [field: NonSerialized]
        public event EditedStatusHandler IsEditedWithoutSaveChanged;

        public Model()
        {
            NoteBook = new NoteBook();
            NoteBook.DataChanged += ModelEdited;
            ScoreBook = new ScoreBook();
            ScoreBook.DataChanged += ModelEdited;
			LaneBook = new LaneBook();
            LaneBook.DataChanged += ModelEdited;
            LaneBook.UpdateNoteLocation += NoteBook.UpdateNoteLocation;
            MusicInfo = new MusicInfo();
            IsEditedWithoutSave = false;
        }

        private void ModelEdited(object sender, EventArgs e)
        {
            //NOTE: IsEditedWithoutSaveを変更するだけで18ミリ秒も時間食うので無駄な変更はif文で弾く
            if (!IsEditedWithoutSave)
            {
                IsEditedWithoutSave = true;
            }
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
            LaneBook.InsetScoreForwardWithNote(NoteBook, ScoreBook, score, beatNumer, beatDenom, barCount);
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

        public void DeleteScoreWithNote(Score score, int count)
        {
            LaneBook.DeleteScoreWithNote(NoteBook, ScoreBook, score, count);
        }

        public void FillLane()
		{
			if (LaneBook.Any()) FillLane(LaneBook.First());
		}

		public void FillLane(ScoreLane begin)
		{
			LaneBook.FillLane(begin);
        }

		public void AddNote(Note newNote)
		{
			NoteBook.Add(newNote);
        }

		public void AddLongNote(LongNote newLongNote)
		{
			NoteBook.Add(newLongNote);
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
