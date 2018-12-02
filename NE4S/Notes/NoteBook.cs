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
    [Serializable()]
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
            if (note is Air)
            {
                Air air = note as Air;
                airNotes.Remove(air);
                air.DetachNote();
            }
            else if (note is HoldBegin || note is HoldEnd)
            {
                Hold hold = holdNotes.Find(x => x.Contains(note));
                if (hold != null) holdNotes.Remove(hold);
                //終点にAirやAirHoldがくっついていたときの処理
                HoldEnd holdEnd = hold.Find(x => x is HoldEnd) as HoldEnd;
                airNotes.Remove(holdEnd.GetAirForDelete());
                airHoldNotes.Remove(holdEnd.GetAirHoldForDelete());
            }
            else if (note is SlideBegin || note is SlideEnd)
            {
                Slide slide = slideNotes.Find(x => x.Contains(note));
                if (slide != null) slideNotes.Remove(slide);
                //終点にAirやAirHoldがくっついていたときの処理
                SlideEnd slideEnd = slide.Find(x => x is SlideEnd) as SlideEnd;
                airNotes.Remove(slideEnd.GetAirForDelete());
                airHoldNotes.Remove(slideEnd.GetAirHoldForDelete());
            }
            else if (note is SlideTap || note is SlideRelay || note is SlideCurve)
            {
                Slide slide = slideNotes.Find(x => x.Contains(note));
                slide?.Remove(note);
            }
            else if (note is AirAction)
            {
                AirHold airHold = airHoldNotes.Find(x => x.Contains(note));
                airHold?.Remove(note);
                if (airHold != null && !airHold.Where(x => x is AirAction).Any())
                {
                    airHoldNotes.Remove(airHold);
                    airHold.DetachNote();
                }
            }
            else if (note is AirableNote)
            {
                AirableNote airable = note as AirableNote;
                airNotes.Remove(airable.GetAirForDelete());
                airHoldNotes.Remove(airable.GetAirHoldForDelete());
                shortNotes.Remove(note);
            }
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
            //AirHold
            foreach (AirHold airHold in airHoldNotes.Reverse<AirHold>())
            {
                selectedNote = airHold.Find(x => x.Contains(location));
                if (selectedNote != null && !(selectedNote is AirHoldBegin))
                {
                    MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                    return selectedNote;
                }
            }
            //Air
            selectedNote = airNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null)
            {
                MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                return selectedNote;
            }
            //ShortNote
            selectedNote = shortNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null)
            {
                MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                return selectedNote;
            }
            //Slide
            foreach (Slide slide in slideNotes.Reverse<Slide>())
            {
                selectedNote = slide.Find(x => x.Contains(location));
                if (selectedNote != null)
                {
                    MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                    return selectedNote;
                }
            }
            //Hold
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
                if (selectedNote != null && !(selectedNote is AirHoldBegin))
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

        public void Paint(PaintEventArgs e, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
		{
            //Hold
            holdNotes.Where(x => x.IsDrawable()).ToList().ForEach(x => x.Draw(e, originPosX, originPosY, scoreBook, laneBook, currentPositionX));
            //Slide
            slideNotes.Where(x => x.IsDrawable()).ToList().ForEach(x => x.Draw(e, originPosX, originPosY, scoreBook, laneBook, currentPositionX));
            //ShortNote
            shortNotes.Where(x => x.Pos.Bar >= Status.DrawScoreBarFirst && x.Pos.Bar <= Status.DrawScoreBarLast).ToList().ForEach(x => x.Draw(e, originPosX, originPosY));
            //Air
            airNotes.Where(x => x.Pos.Bar >= Status.DrawScoreBarFirst && x.Pos.Bar <= Status.DrawScoreBarLast).ToList().ForEach(x => x.Draw(e, originPosX, originPosY));
            //AirHold
            airHoldNotes.Where(x => x.IsDrawable()).ToList().ForEach(x => x.Draw(e, originPosX, originPosY, scoreBook, laneBook, currentPositionX));
		}
	}
}
