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
        private List<Note> airNotes;

        public NoteBook()
        {
            shortNotes = new List<Note>();
            holdNotes = new List<Hold>();
            slideNotes = new List<Slide>();
            airHoldNotes = new List<AirHold>();
            airNotes = new List<Note>();
        }
    }
}
