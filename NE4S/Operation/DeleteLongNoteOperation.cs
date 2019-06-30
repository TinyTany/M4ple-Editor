using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;
using System.Diagnostics;

namespace NE4S.Operation
{
    public class DeleteLongNoteOperation : Operation
    {
        public DeleteLongNoteOperation(Model model, LongNote longNote)
        {
            Debug.Assert(model != null);
            Debug.Assert(longNote != null);
            if (model == null || longNote == null)
            {
                Logger.Error("引数にnullのものが存在するため削除操作を行えません。");
                Canceled = true;
                return;
            }

            switch (longNote)
            {
                case Hold hold:
                    {
                        var airable = hold.Find(x => x is AirableNote) as AirableNote;
                        var air = airable?.Air;
                        var airHold = airable?.AirHold;
                        var holdNotes = model.NoteBook.HoldNotes;
                        var airNotes = model.NoteBook.AirNotes;
                        var airHoldNotes = model.NoteBook.AirHoldNotes;
                        Invoke += () =>
                        {
                            holdNotes.Remove(hold);
                            if (air != null)
                            {
                                airNotes.Remove(air);
                            }
                            if (airHold != null)
                            {
                                airHoldNotes.Remove(airHold);
                            }
                        };
                        Undo += () =>
                        {
                            holdNotes.Add(hold);
                            if (air != null)
                            {
                                airNotes.Add(air);
                            }
                            if (airHold != null)
                            {
                                airHoldNotes.Add(airHold);
                            }
                        };
                    }
                    break;
                case Slide slide:
                    {
                        var airable = slide.Find(x => x is AirableNote) as AirableNote;
                        var air = airable?.Air;
                        var airHold = airable?.AirHold;
                        var slideNotes = model.NoteBook.SlideNotes;
                        var airNotes = model.NoteBook.AirNotes;
                        var airHoldNotes = model.NoteBook.AirHoldNotes;
                        Invoke += () =>
                        {
                            slideNotes.Remove(slide);
                            if (air != null)
                            {
                                airNotes.Remove(air);
                            }
                            if (airHold != null)
                            {
                                airHoldNotes.Remove(airHold);
                            }
                        };
                        Undo += () =>
                        {
                            slideNotes.Add(slide);
                            if (air != null)
                            {
                                airNotes.Add(air);
                            }
                            if (airHold != null)
                            {
                                airHoldNotes.Add(airHold);
                            }
                        };
                    }
                    break;
                case AirHold airHold:
                    {
                        var airable = airHold.GetAirable?.Invoke();
                        var air = airable?.Air;
                        Debug.Assert(airable != null);
                        Debug.Assert(air != null);
                        var airNotes = model.NoteBook.AirNotes;
                        var airHoldNotes = model.NoteBook.AirHoldNotes;
                        Invoke += () =>
                        {
                            airHoldNotes.Remove(airHold);
                            if (air != null)
                            {
                                airNotes.Remove(air);
                            }
                            airable?.DetachAir();
                            airable?.DetachAirHold();
                        };
                        Undo += () =>
                        {
                            airHoldNotes.Add(airHold);
                            airable?.AttachAirHold(airHold);
                            if (air != null)
                            {
                                airNotes.Add(air);
                                airable?.AttachAir(air);
                            }
                        };
                    }
                    break;
                default:
                    {
                        Debug.Assert(false, "不明なロングノーツを削除できません。");
                        Logger.Error("不明なロングノーツを削除できません。");
                        Canceled = true;
                    }
                    break;
            }
        }
    }
}
