using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Component
{
    public class ValueNoteButton : NoteButton
    {
        public ValueNoteButton(int noteType, NoteButtonEventHandler handler) : base(noteType, handler) { }
    }
}
