using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;

namespace NE4S.Operation
{
    public class AddShortNoteOperation : Operation
    {
        public AddShortNoteOperation(Model model, Note note)
        {
            Invoke += () =>
            {
                model.NoteBook.Put(note);
            };
            Undo += () =>
            {
                model.NoteBook.UnPut(note);
            };
        }
    }

    public class AddLongNoteOperation : Operation
    {
        public AddLongNoteOperation(Model model, LongNote longNote)
        {
            Invoke += () =>
            {
                model.NoteBook.Put(longNote);
            };
            Undo += () =>
            {
                model.NoteBook.UnPut(longNote);
            };
        }

        public AddLongNoteOperation(Model model, AirHold airHold, AirUpC air, AirableNote airable)
        {
            Invoke += () =>
            {
                model.NoteBook.AttachAirHoldToAirableNote(airable, airHold, air);
            };
            Undo += () =>
            {
                model.NoteBook.DetachAirHoldFromAirableNote(airable, out airHold, out air);
            };
        }
    }

    public class AddStepNoteOperation : Operation
    {
        public AddStepNoteOperation(Slide slide, SlideTap slideTap)
        {
            Invoke += () =>
            {
                slide.Add(slideTap);
            };
            Undo += () =>
            {
                slide.Remove(slideTap);
            };
        }

        public AddStepNoteOperation(Slide slide, SlideRelay slideRelay)
        {
            Invoke += () =>
            {
                slide.Add(slideRelay);
            };
            Undo += () =>
            {
                slide.Remove(slideRelay);
            };
        }

        public AddStepNoteOperation(Slide slide, SlideCurve slideCurve)
        {
            Invoke += () =>
            {
                slide.Add(slideCurve);
            };
            Undo += () =>
            {
                slide.Remove(slideCurve);
            };
        }

        public AddStepNoteOperation(AirHold airHold, AirAction airAction)
        {
            Invoke += () =>
            {
                airHold.Add(airAction);
            };
            Undo += () =>
            {
                airHold.Remove(airAction);
            };
        }
    }

    public class AddAirNoteOperation : Operation
    {
        public AddAirNoteOperation(Model model, Air air, AirableNote airable)
        {
            Invoke += () =>
            {
                model.NoteBook.AttachAirToAirableNote(airable, air);
            };
            Undo += () =>
            {
                model.NoteBook.DetachAirFromAirableNote(airable, out air);
            };
        }
    }
}
