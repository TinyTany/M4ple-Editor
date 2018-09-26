using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
