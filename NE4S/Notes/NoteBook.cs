using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using NE4S.Scores;

namespace NE4S.Notes
{
    /// <summary>
    /// 置かれている全ノーツをまとめる
    /// </summary>
    public class NoteBook
    {
        private List<Note> shortNotes;
        private List<Hold> holdNotes;
        private List<Slide> slideNotes;
        private List<AirHold> airHoldNotes;
        private List<Air> airNotes;

        public NoteBook()
        {
            shortNotes = new List<Note>();
            holdNotes = new List<Hold>();
            slideNotes = new List<Slide>();
            airHoldNotes = new List<AirHold>();
            airNotes = new List<Air>();
        }

		public void Add(Note newNote)
		{
			if (newNote is Air) airNotes.Add(newNote as Air);
			else shortNotes.Add(newNote);
		}

		public void Add(LongNote newLongNote)
		{
			if (newLongNote is Hold) holdNotes.Add(newLongNote as Hold);
			else if (newLongNote is Slide) slideNotes.Add(newLongNote as Slide);
			else if (newLongNote is AirHold) airHoldNotes.Add(newLongNote as AirHold);
		}

		public void Delete(Note note)
		{
			if (note is Air) airNotes.Remove(note as Air);
			else shortNotes.Remove(note);
		}

		public void Delete(LongNote longNote)
		{
			if (longNote is Hold) holdNotes.Remove(longNote as Hold);
			else if (longNote is Slide) slideNotes.Remove(longNote as Slide);
			else if (longNote is AirHold) airHoldNotes.Remove(longNote as AirHold);
		}

		public void Relocate(Note note, Position pos)
		{
			if(note != null) note.Relocate(pos);
		}

		public void Resize(Note note, int size)
		{
			if (note != null) note.ReSize(size);
		}

        /// <summary>
        /// クリックされてるノーツがあったら投げる
        /// なかったらnullを投げる
        /// ノーツのどのへんがクリックされたかも特定する
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Note SelectedNote(PointF location, ref int noteArea)
        {
            Note selectedNote;
            foreach (AirHold airHold in airHoldNotes.Reverse<AirHold>())
            {
                selectedNote = airHold.Find(x => x.Contains(location));
                if (selectedNote != null && !(selectedNote is AirHoldBegin))
                {
                    MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                    return selectedNote;
                }
            }
            selectedNote = airNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null)
            {
                MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                return selectedNote;
            }
            selectedNote = shortNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null)
            {
                MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                return selectedNote;
            }
            foreach (Slide slide in slideNotes.Reverse<Slide>())
            {
                selectedNote = slide.Find(x => x.Contains(location));
                if (selectedNote != null)
                {
                    MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                    return selectedNote;
                }
            }
            foreach (Hold hold in holdNotes.Reverse<Hold>())
            {
                selectedNote = hold.Find(x => x.Contains(location));
                if (selectedNote != null)
                {
                    MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                    return selectedNote;
                }
            }
            return null;
        }

        /// <summary>
        /// クリックされてるノーツがあったら投げる
        /// なかったらnullを投げる
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Note SelectedNote(PointF location)
        {
            Note selectedNote;
            foreach (AirHold airHold in airHoldNotes.Reverse<AirHold>())
            {
                selectedNote = airHold.Find(x => x.Contains(location));
                if (selectedNote != null)
                {
                    return selectedNote;
                }
            }
            selectedNote = airNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null)
            {
                return selectedNote;
            }
            selectedNote = shortNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null)
            {
                return selectedNote;
            }
            foreach (Slide slide in slideNotes.Reverse<Slide>())
            {
                selectedNote = slide.Find(x => x.Contains(location));
                if (selectedNote != null)
                {
                    return selectedNote;
                }
            }
            foreach (Hold hold in holdNotes.Reverse<Hold>())
            {
                selectedNote = hold.Find(x => x.Contains(location));
                if (selectedNote != null)
                {
                    return selectedNote;
                }
            }
            return null;
        }

        public Slide SelectedSlide(PointF locationVirtual, ScoreBook scoreBook, LaneBook laneBook)
        {
            return slideNotes.FindLast(x => x.Contains(locationVirtual, scoreBook, laneBook));
        }

        public AirHold SelectedAirHold(PointF locationVirtual, ScoreBook scoreBook, LaneBook laneBook)
        {
            return airHoldNotes.FindLast(x => x.Contains(locationVirtual, scoreBook, laneBook));
        }

#if DEBUG
        //今はちょっとだけ実装
        //TODO: 範囲外のノーツは描画しないようにして軽くする
        public void Paint(PaintEventArgs e, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
		{
            foreach (Hold hold in holdNotes)
            {
                hold.Draw(e, originPosX, originPosY, scoreBook, laneBook, currentPositionX);
            }
            foreach (Slide slide in slideNotes)
            {
                slide.Draw(e, originPosX, originPosY, scoreBook, laneBook, currentPositionX);
            }
            //お試し
            //範囲外のノーツは描画しないようにするというこころ
            foreach (Note note in shortNotes.Where(
                x => x.Location.X > originPosX && x.Location.X < originPosX + 1031))
            {
                note.Draw(e, originPosX, originPosY);
            }
            foreach (Air air in airNotes)
            {
                air.Draw(e, originPosX, originPosY);
            }
            foreach (AirHold airHold in airHoldNotes)
            {
                airHold.Draw(e, originPosX, originPosY, scoreBook, laneBook, currentPositionX);
            }
		}
#endif
	}
}
