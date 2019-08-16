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
            if (model == null || note == null)
            {
                Logger.Error("引数にnullのものが存在するため削除操作を行えません。", true);
                Canceled = true;
                return;
            }

            // TODO: Criticalが起こったときの対処どうしよう...
            switch (note)
            {
                case Air air:
                    {
                        var baseAirable = air.GetAirable?.Invoke();
                        if (baseAirable == null)
                        {
                            Logger.Warn("削除するAirのベースとなるAirableNoteが存在しません。", true);
                        }
                        if (baseAirable.IsAirHoldAttached)
                        {
                            Logger.Error("AirHoldが取り付けられているAirableNoteからAirのみの取り外しはできません。");
                            Canceled = true;
                            return;
                        }
                        Invoke += () =>
                        {
                            model.NoteBook.DetachAirFromAirableNote(baseAirable, out air);
                        };
                        Undo += () =>
                        {
                            model.NoteBook.AttachAirToAirableNote(baseAirable, air);
                        };
                    }
                    break;
                case HoldBegin _:
                case HoldEnd _:
                    {
                        var holdNotes = model.NoteBook.HoldNotes;
                        var hold = holdNotes.ToList().Find(x => x.Contains(note));
                        if (hold == null)
                        {
                            Logger.Critical("削除対象のHoldノーツが見つからないため、削除できません。", true);
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
                        var slide = slideNotes.ToList().Find(x => x.Contains(note));
                        if (slide == null)
                        {
                            Logger.Critical("削除対象のSlideノーツが見つからないため、削除できません。", true);
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
                        var book = model.NoteBook;
                        var slideNotes = book.SlideNotes;
                        var slide = slideNotes.ToList().Find(x => x.Contains(note));
                        if (slide == null)
                        {
                            Logger.Critical("削除対象のSlideノーツが見つからないため、削除できません。", true);
                            Canceled = true;
                            return;
                        }
                        var after = slide.DeepCopy();
                        var delNote = after.ElementAt(slide.IndexOf(note));
                        after.Remove(delNote);
                        Invoke += () =>
                        {
                            book.UnPut(slide);
                            book.Put(after);
                        };
                        Undo += () =>
                        {
                            book.UnPut(after);
                            book.Put(slide);
                        };
                    }
                    break;
                case AirHoldEnd _:
                    {
                        var airHoldNotes = model.NoteBook.AirHoldNotes;
                        var airHold = airHoldNotes.ToList().Find(x => x.Contains(note));
                        if (airHold == null)
                        {
                            Logger.Critical("削除対象のAirHoldが見つからないため、削除できません。", true);
                            Canceled = true;
                            return;
                        }
                        var baseAirable = airHold.GetAirable?.Invoke();
                        if (baseAirable == null)
                        {
                            Logger.Warn("削除するAirHoldのベースとなるAirableNoteが存在しません。", true);
                        }
                        AirUpC air = null;
                        Invoke += () =>
                        {
                            model.NoteBook.DetachAirHoldFromAirableNote(baseAirable, out airHold, out air);
                        };
                        Undo += () =>
                        {
                            model.NoteBook.AttachAirHoldToAirableNote(baseAirable, airHold, air);
                        };
                    }
                    break;
                case AirAction _:
                    {
                        var airHoldNotes = model.NoteBook.AirHoldNotes;
                        var airHold = airHoldNotes.ToList().Find(x => x.Contains(note));
                        if (airHold == null)
                        {
                            Logger.Critical("削除対象のAirHoldが見つからないため、削除できません。", true);
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
                case AirableNote _:
                case AttributeNote _:
                    {
                        Invoke += () =>
                        {
                            model.NoteBook.UnPut(note);
                        };
                        Undo += () =>
                        {
                            model.NoteBook.Put(note);
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
