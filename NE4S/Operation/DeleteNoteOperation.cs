using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;
using System.Diagnostics;

namespace NE4S.Operation
{
    public class DeleteNoteOperation : Operation
    {
        public DeleteNoteOperation(Model model, Note note)
        {
            Debug.Assert(model != null);
            Debug.Assert(note != null);
            if (model == null || note == null)
            {
                Logger.Error("引数にnullのものが存在するため削除操作を行えません。");
                Canceled = true;
                return;
            }

            // TODO: Criticalが起こったときの対処どうしよう...
            switch (note)
            {
                case Air air:
                    {
                        var airNotes = model.NoteBook.AirNotes;
                        var baseAirable = air.GetAirable?.Invoke();
                        Debug.Assert(baseAirable != null);
                        if (baseAirable == null)
                        {
                            Logger.Warn("削除するAirのベースとなるAirableNoteが存在しません。");
                        }
                        Invoke += () =>
                        {
                            airNotes.Remove(air);
                            baseAirable?.DetachAir();
                        };
                        Undo += () =>
                        {
                            airNotes.Add(air);
                            baseAirable?.AttachAir(air);
                        };
                    }
                    break;
                case HoldBegin _:
                case HoldEnd _:
                    {
                        var holdNotes = model.NoteBook.HoldNotes;
                        var hold = holdNotes.Find(x => x.Contains(note));
                        Debug.Assert(hold != null);
                        if (hold == null)
                        {
                            Logger.Critical("削除対象のHoldノーツが見つからないため、削除できません。");
                            Canceled = true;
                            return;
                        }
                        var deleteHoldOperation = new DeleteLongNoteOperation(model, hold);
                        Invoke += () =>
                        {
                            deleteHoldOperation.Invoke();
                        };
                        Undo += () =>
                        {
                            deleteHoldOperation.Undo();
                        };
                    }
                    break;
                case SlideBegin _:
                case SlideEnd _:
                    {
                        var slideNotes = model.NoteBook.SlideNotes;
                        var slide = slideNotes.Find(x => x.Contains(note));
                        Debug.Assert(slide != null);
                        if (slide == null)
                        {
                            Logger.Critical("削除対象のSlideノーツが見つからないため、削除できません。");
                            Canceled = true;
                            return;
                        }
                        var deleteSlideOperation = new DeleteLongNoteOperation(model, slide);
                        Invoke += () =>
                        {
                            deleteSlideOperation.Invoke();
                        };
                        Undo += () =>
                        {
                            deleteSlideOperation.Undo();
                        };
                    }
                    break;
                case SlideTap _:
                case SlideRelay _:
                case SlideCurve _:
                    {
                        var slideNotes = model.NoteBook.SlideNotes;
                        var slide = slideNotes.Find(x => x.Contains(note));
                        Debug.Assert(slide != null);
                        if (slide == null)
                        {
                            Logger.Critical("削除対象のSlideノーツが見つからないため、削除できません。");
                            Canceled = true;
                            return;
                        }
                        var slideCopy = new Slide(slide);
                        slide.Remove(note);
                        slideNotes.Remove(slide);
                        Invoke += () =>
                        {
                            slideNotes.Add(slide);
                            slideNotes.Remove(slideCopy);
                        };
                        Undo += () =>
                        {
                            slideNotes.Add(slideCopy);
                            slideNotes.Remove(slide);
                        };
                    }
                    break;
                case AirHoldEnd _:
                    {
                        var airHoldNotes = model.NoteBook.AirHoldNotes;
                        var airHold = airHoldNotes.Find(x => x.Contains(note));
                        Debug.Assert(airHold != null);
                        if (airHold == null)
                        {
                            Logger.Critical("削除対象のAirHoldが見つからないため、削除できません。");
                            Canceled = true;
                            return;
                        }
                        var deleteAirHoldOperation = new DeleteLongNoteOperation(model, airHold);
                        Invoke += () =>
                        {
                            deleteAirHoldOperation.Invoke();
                        };
                        Undo += () =>
                        {
                            deleteAirHoldOperation.Undo();
                        };
                    }
                    break;
                case AirAction _:
                    {
                        var airHoldNotes = model.NoteBook.AirHoldNotes;
                        var airHold = airHoldNotes.Find(x => x.Contains(note));
                        Debug.Assert(airHold != null);
                        if (airHold == null)
                        {
                            Logger.Critical("削除対象のAirHoldが見つからないため、削除できません。");
                            Canceled = true;
                            return;
                        }
                        Invoke += () =>
                        {
                            airHold?.Remove(note);
                        };
                        Undo += () =>
                        {
                            airHold?.Add(note);
                        };
                    }
                    break;
                // NOTE: ここまで降りてくるAirableNoteはすべてShortNote（HoldEndやSlideEndはここまでこないはず）
                case AirableNote airable:
                    {
                        var shortNotes = model.NoteBook.ShortNotes;
                        var airNotes = model.NoteBook.AirNotes;
                        var airHoldNotes = model.NoteBook.AirHoldNotes;
                        var air = airable.Air;
                        var airHold = airable.AirHold;
                        Invoke += () =>
                        {
                            shortNotes.Remove(airable);
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
                            shortNotes.Add(airable);
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
                case AttributeNote attNote:
                    {
                        var attributeNotes = model.NoteBook.AttributeNotes;
                        Invoke += () =>
                        {
                            attributeNotes.Remove(attNote);
                        };
                        Undo += () =>
                        {
                            attributeNotes.Add(attNote);
                        };
                    }
                    break;
                default:
                    {
                        Logger.Error("不明なノーツを削除できません。", true);
                        Canceled = true;
                    }
                    break;
            }
        }
    }
}
