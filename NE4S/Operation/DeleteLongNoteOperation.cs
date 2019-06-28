using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;

namespace NE4S.Operation
{
    public class DeleteLongNoteOperation : Operation
    {
        public DeleteLongNoteOperation(Model model, LongNote longNote)
        {
            List<Hold> holdNotes = model.NoteBook.HoldNotes;
            List<Slide> slideNotes = model.NoteBook.SlideNotes;
            List<AirHold> airHoldNotes = model.NoteBook.AirHoldNotes;
            List<Air> airNotes = model.NoteBook.AirNotes;
            Air attachedAir = null;
            AirHold attachedAirHold = null;
            AirableNote baseAirable = null;

            Invoke += () =>
            {
                if (longNote is Hold) holdNotes.Remove(longNote as Hold);
                else if (longNote is Slide) slideNotes.Remove(longNote as Slide);
                else if (longNote is AirHold airHold)
                {
                    baseAirable = airHold.GetAirable?.Invoke();
                    airHoldNotes.Remove(airHold);
                    airHold.DetachNote();
                }
                //終点にくっついてるかもしれないAir系ノーツの破棄
                if (longNote?.Find(x => x is AirableNote) is AirableNote airable)
                {
                    if (airable.IsAirAttached)
                    {
                        airNotes.Remove(airable.Air);
                        attachedAir = airable.Air;
                        airable.DetachAir();
                    }
                    if (airable.IsAirHoldAttached)
                    {
                        airHoldNotes.Remove(airable.AirHold);
                        attachedAirHold = airable.AirHold;
                        airable.DetachAirHold();
                    }
                }
            };
            Undo += () =>
            {
                if (longNote is Hold) holdNotes.Add(longNote as Hold);
                else if (longNote is Slide) slideNotes.Add(longNote as Slide);
                else if (longNote is AirHold ah)
                {
                    airHoldNotes.Add(ah);
                    baseAirable?.AttachAirHold(ah);
                }
                var airable = longNote?.Find(x => x is AirableNote) as AirableNote;
                if (attachedAir != null)
                {
                    airable?.AttachAir(attachedAir);
                    airNotes.Add(attachedAir);
                }
                if (attachedAirHold != null)
                {
                    airable?.AttachAirHold(attachedAirHold);
                    airHoldNotes.Add(attachedAirHold);
                }
            };
        }
    }
}
