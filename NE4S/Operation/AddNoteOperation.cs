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
                model.NoteBook.Add(note);
            };
            Undo += () =>
            {
                model.NoteBook.Delete(note);
            };
        }
    }

    public class AddLongNoteOperation : Operation
    {
        public AddLongNoteOperation(Model model, LongNote longNote)
        {
            Invoke += () =>
            {
                model.NoteBook.Add(longNote);
            };
            Undo += () =>
            {
                model.NoteBook.Delete(longNote);
            };
        }

        public AddLongNoteOperation(Model model, AirHold airHold, Air air, AirableNote airable)
        {
            Invoke += () =>
            {
                if (airable != null && !airable.IsAirHoldAttached)
                {
                    model.NoteBook.Add(airHold);
                    airable.AttachAirHold(airHold);
                    if (!airable.IsAirAttached)
                    {
                        model.NoteBook.Add(air);
                        airable.AttachAir(air);
                    }
                }
            };
            Undo += () =>
            {
                model.NoteBook.Delete(airHold);
                airable.DetachAirHold();
                model.NoteBook.Delete(air);
                airable.DetachAir();
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
                model.NoteBook.Add(air);
                airable.AttachAir(air);
            };
            Undo += () =>
            {
                model.NoteBook.Delete(air);
                airable.DetachAir();
            };
        }
    }
}
