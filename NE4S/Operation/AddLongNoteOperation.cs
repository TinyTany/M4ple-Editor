using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;

namespace NE4S.Operation
{
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
}
