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
        public List<Note> ShortNotes { get; private set; } = new List<Note>();
        public List<Hold> HoldNotes { get; private set; } = new List<Hold>();
        public List<Slide> SlideNotes { get; private set; } = new List<Slide>();
        public List<AirHold> AirHoldNotes { get; private set; } = new List<AirHold>();
        public List<Air> AirNotes { get; private set; } = new List<Air>();
        public List<AttributeNote> AttributeNotes { get; private set; } = new List<AttributeNote>();

        public NoteBook()
        {
            //HACK: 開始時BPMを無理やり設定
            Status.CurrentValue = 120;
            AttributeNotes.Add(new BPM(
                new Position(0, 0),
                new PointF(
                    ScoreInfo.PanelMargin.Left + ScoreInfo.LaneMargin.Left,
                    ScoreInfo.PanelMargin.Top + ScoreLane.Height - ScoreInfo.LaneMargin.Bottom),
                0));
        }

		public void Add(Note newNote)
		{
            if (newNote is Air) AirNotes.Add(newNote as Air);
            else if (newNote is AttributeNote) AttributeNotes.Add(newNote as AttributeNote);
            else ShortNotes.Add(newNote);
		}

		public void Add(LongNote newLongNote)
		{
			if (newLongNote is Hold) HoldNotes.Add(newLongNote as Hold);
			else if (newLongNote is Slide) SlideNotes.Add(newLongNote as Slide);
			else if (newLongNote is AirHold) AirHoldNotes.Add(newLongNote as AirHold);
		}

		public void Delete(Note note)
		{
            if (note is Air)
            {
                Air air = note as Air;
                AirNotes.Remove(air);
                air.DetachNote();
            }
            else if (note is HoldBegin || note is HoldEnd)
            {
                Hold hold = HoldNotes.Find(x => x.Contains(note));
                if (hold != null) HoldNotes.Remove(hold);
                //終点にAirやAirHoldがくっついていたときの処理
                HoldEnd holdEnd = hold.Find(x => x is HoldEnd) as HoldEnd;
                AirNotes.Remove(holdEnd.GetAirForDelete());
                AirHoldNotes.Remove(holdEnd.GetAirHoldForDelete());
            }
            else if (note is SlideBegin || note is SlideEnd)
            {
                Slide slide = SlideNotes.Find(x => x.Contains(note));
                if (slide != null) SlideNotes.Remove(slide);
                //終点にAirやAirHoldがくっついていたときの処理
                SlideEnd slideEnd = slide.Find(x => x is SlideEnd) as SlideEnd;
                AirNotes.Remove(slideEnd.GetAirForDelete());
                AirHoldNotes.Remove(slideEnd.GetAirHoldForDelete());
            }
            else if (note is SlideTap || note is SlideRelay || note is SlideCurve)
            {
                Slide slide = SlideNotes.Find(x => x.Contains(note));
                slide?.Remove(note);
            }
            else if (note is AirAction)
            {
                AirHold airHold = AirHoldNotes.Find(x => x.Contains(note));
                airHold?.Remove(note);
                if (airHold != null && !airHold.Where(x => x is AirAction).Any())
                {
                    AirHoldNotes.Remove(airHold);
                    airHold.DetachNote();
                }
            }
            else if (note is AirableNote)
            {
                AirableNote airable = note as AirableNote;
                AirNotes.Remove(airable.GetAirForDelete());
                AirHoldNotes.Remove(airable.GetAirHoldForDelete());
                ShortNotes.Remove(note);
            }
            else if (note is AttributeNote)
            {
                AttributeNotes.Remove(note as AttributeNote);
            }
            else ShortNotes.Remove(note);
		}

		public void Delete(LongNote longNote)
		{
			if (longNote is Hold) HoldNotes.Remove(longNote as Hold);
			else if (longNote is Slide) SlideNotes.Remove(longNote as Slide);
			else if (longNote is AirHold) AirHoldNotes.Remove(longNote as AirHold);
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
            foreach (AirHold airHold in AirHoldNotes.Reverse<AirHold>())
            {
                if (!Status.IsAirHoldVisible) break;
                selectedNote = airHold.Find(x => x.Contains(location));
                if (selectedNote != null && !(selectedNote is AirHoldBegin))
                {
                    MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                    return selectedNote;
                }
            }
            //Air
            selectedNote = AirNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null && Status.IsAirVisible)
            {
                MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                return selectedNote;
            }
            //ShortNote
            selectedNote = ShortNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null && Status.IsShortNoteVisible)
            {
                MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                return selectedNote;
            }
            //Slide
            foreach (Slide slide in SlideNotes.Reverse<Slide>())
            {
                if (!Status.IsSlideVisible) break;
                selectedNote = slide.Find(x => x.Contains(location));
                if (selectedNote != null)
                {
                    MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                    return selectedNote;
                }
            }
            //Hold
            foreach (Hold hold in HoldNotes.Reverse<Hold>())
            {
                if (!Status.IsHoldVisible) break;
                selectedNote = hold.Find(x => x.Contains(location));
                if (selectedNote != null)
                {
                    MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                    return selectedNote;
                }
            }
            //AttributeNote
            selectedNote = AttributeNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null)
            {
                noteArea = Define.NoteArea.CENTER;
                return selectedNote;
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
            //この変数は使用しない
            int noteArea = 0;
            return SelectedNote(location, ref noteArea);
        }

        public Slide SelectedSlide(PointF locationVirtual, ScoreBook scoreBook, LaneBook laneBook)
        {
            return SlideNotes.FindLast(x => x.Contains(locationVirtual, scoreBook, laneBook));
        }

        public AirHold SelectedAirHold(PointF locationVirtual, ScoreBook scoreBook, LaneBook laneBook)
        {
            return AirHoldNotes.FindLast(x => x.Contains(locationVirtual, scoreBook, laneBook));
        }

        public void UpdateNoteLocation(LaneBook laneBook)
        {
            ShortNotes.ForEach(x => x.UpdateLocation(laneBook));
            HoldNotes.ForEach(x => x.UpdateLocation(laneBook));
            SlideNotes.ForEach(x => x.UpdateLocation(laneBook));
            AirHoldNotes.ForEach(x => x.UpdateLocation(laneBook));
            AirNotes.ForEach(x => x.UpdateLocation(laneBook));
            AttributeNotes.ForEach(x => x.UpdateLocation(laneBook));
        }

        public void RelocateNoteTickAfterScoreTick(int scoreTick, int deltaTick)
        {
            ShortNotes.Where(x => x.Position.Tick >= scoreTick).ToList().ForEach(x => x.RelocateOnly(new Position(x.Position.Lane, x.Position.Tick + deltaTick)));
            AirNotes.Where(x => x.Position.Tick >= scoreTick).ToList().ForEach(x => x.RelocateOnly(new Position(x.Position.Lane, x.Position.Tick + deltaTick)));
            AttributeNotes.Where(x => x.Position.Tick >= scoreTick).ToList().ForEach(x => x.RelocateOnly(new Position(x.Position.Lane, x.Position.Tick + deltaTick)));
        }

        public void Paint(PaintEventArgs e, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
		{
            //AttributeNote
            AttributeNotes.Where(x => x.Position.Tick >= Status.DrawTickFirst && x.Position.Tick <= Status.DrawTickLast).ToList().ForEach(x => x.Draw(e, originPosX, originPosY));
            //Hold
            if (Status.IsHoldVisible)
            {
                HoldNotes.Where(x => x.IsDrawable()).ToList().ForEach(x => x.Draw(e, originPosX, originPosY, laneBook, currentPositionX));
            }
            //Slide
            if (Status.IsSlideVisible)
            {
                SlideNotes.Where(x => x.IsDrawable()).ToList().ForEach(x => x.Draw(e, originPosX, originPosY, laneBook, currentPositionX));
            }
            //ShortNote
            if (Status.IsShortNoteVisible)
            {
                ShortNotes.Where(x => x.Position.Tick >= Status.DrawTickFirst && x.Position.Tick <= Status.DrawTickLast).ToList().ForEach(x => x.Draw(e, originPosX, originPosY));
            }
            //Air
            if (Status.IsAirVisible)
            {
                AirNotes.Where(x => x.Position.Tick >= Status.DrawTickFirst && x.Position.Tick <= Status.DrawTickLast).ToList().ForEach(x => x.Draw(e, originPosX, originPosY));
            }
            //AirHold
            if (Status.IsAirHoldVisible)
            {
                AirHoldNotes.Where(x => x.IsDrawable()).ToList().ForEach(x => x.Draw(e, originPosX, originPosY, laneBook, currentPositionX));
            }
        }
	}
}
