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
        public MusicInfo MusicInfo { get; set; }
        #region 使わなくなったメンバ
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
                //IsEditedWithoutSaveChanged?.Invoke(isEditedWithoutSave);
            }
        }
        [field: NonSerialized]
        public event Action<bool> IsEditedWithoutSaveChanged;

        private void ModelEdited(object sender, EventArgs e)
        {
            //NOTE: IsEditedWithoutSaveを変更するだけで18ミリ秒も時間食うので無駄な変更はif文で弾く
            if (!IsEditedWithoutSave)
            {
                IsEditedWithoutSave = true;
            }
        }
        #endregion

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

        public void PaintNote(PaintEventArgs e, Point drawLocation)
        {
            NoteBook.Paint(e, drawLocation, LaneBook);
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
