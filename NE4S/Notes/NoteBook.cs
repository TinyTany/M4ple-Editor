﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

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

        //今はshortNotesのみ
        public Note SelectedNote(PointF location)
        {
            return shortNotes.FindLast(x => x.Contains(location));
        }

#if DEBUG
		public void Paint(PaintEventArgs e, int originPosX, int originPosY)
		{
			foreach (Note note in shortNotes) note.Draw(e, originPosX, originPosY);
		}
#endif
	}
}
