using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;

namespace NE4S.Operation
{
    public class DeleteNoteOperation : Operation
    {
        public DeleteNoteOperation(Model model, Note note)
        {
            List<Note> shortNotes = model.NoteBook.ShortNotes;
            List<Hold> holdNotes = model.NoteBook.HoldNotes;
            List<Slide> slideNotes = model.NoteBook.SlideNotes;
            List<AirHold> airHoldNotes = model.NoteBook.AirHoldNotes;
            List<Air> airNotes = model.NoteBook.AirNotes;
            List<AttributeNote> attributeNotes = model.NoteBook.AttributeNotes;
            Slide baseSlide = null;
            AirHold baseAirHold = null;
            Air attachedAir = null;
            AirHold attachedAirHold = null;
            DeleteLongNoteOperation deleteLongNote = null;

            Invoke += () =>
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
                    if (hold != null)
                    {
                        deleteLongNote = new DeleteLongNoteOperation(model, hold);
                        deleteLongNote.Invoke();
                    }
                }
                else if (note is SlideBegin || note is SlideEnd)
                {
                    Slide slide = slideNotes.Find(x => x.Contains(note));
                    if (slide != null)
                    {
                        deleteLongNote = new DeleteLongNoteOperation(model, slide);
                        deleteLongNote.Invoke();
                    }
                }
                else if (note is SlideTap || note is SlideRelay || note is SlideCurve)
                {
                    Slide slide = slideNotes.Find(x => x.Contains(note));
                    slide?.Remove(note);
                    baseSlide = slide;
                }
                else if (note is AirHoldEnd)
                {
                    AirHold airHold = airHoldNotes.Find(x => x.Contains(note));
                    if (airHold != null)
                    {
                        deleteLongNote = new DeleteLongNoteOperation(model, airHold);
                        deleteLongNote.Invoke();
                    }
                }
                else if (note is AirAction)
                {
                    AirHold airHold = airHoldNotes.Find(x => x.Contains(note));
                    airHold?.Remove(note);
                    baseAirHold = airHold;
                }
                else if (note is AirableNote)
                {
                    AirableNote airable = note as AirableNote;
                    airNotes.Remove(airable.Air);
                    attachedAir = airable.Air;
                    airable.DetachAir();
                    airHoldNotes.Remove(airable.AirHold);
                    attachedAirHold = airable.AirHold;
                    airable.DetachAirHold();
                    shortNotes.Remove(note);
                }
                else if (note is AttributeNote)
                {
                    attributeNotes.Remove(note as AttributeNote);
                }
                else shortNotes.Remove(note);
            };
            Undo += () =>
            {
                if (baseSlide != null)
                {
                    baseSlide.Add(note);
                }
                else if (baseAirHold != null)
                {
                    baseAirHold.Add(note);
                }
                else if (deleteLongNote != null)
                {
                    deleteLongNote.Undo();
                }
                else if (note is AirableNote)
                {
                    var airable = note as AirableNote;
                    model.NoteBook.Add(airable);
                    if (attachedAir != null)
                    {
                        airable.AttachAir(attachedAir);
                        airNotes.Add(attachedAir);
                    }
                    if (attachedAirHold != null)
                    {
                        airable.AttachAirHold(attachedAirHold);
                        airHoldNotes.Add(attachedAirHold);
                    }
                }
                else
                {
                    model.NoteBook.Add(note);
                }
            };
        }
    }
}
