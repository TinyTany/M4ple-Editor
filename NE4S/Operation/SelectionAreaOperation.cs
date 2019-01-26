using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;
using NE4S.Component;
using System.Windows.Forms;

namespace NE4S.Operation
{
    public class CopyNotesOperation : Operation
    {
        public CopyNotesOperation(SelectionArea selectionArea)
        {
            Invoke += () =>
            {
                Clipboard.SetDataObject(selectionArea);
                //NOTE: どっちにするか（コピー後矩形を保持or破棄）迷う...
                //selectionArea = new SelectionArea();
            };
        }
    }

    public class CutNotesOperation : Operation
    {
        SelectionArea selectionArea = new SelectionArea();

        public CutNotesOperation(Model model, SelectionArea selectionArea)
        {
            Invoke += () =>
            {
                this.selectionArea.Reset(selectionArea);
                new CopyNotesOperation(selectionArea).Invoke();
                new ClearAreaNotesOperation(model, selectionArea).Invoke();
                selectionArea.Reset();
            };
            Undo += () =>
            {
                new CopyNotesOperation(this.selectionArea).Invoke();
                new PasteNotesOperation(
                    model,
                    selectionArea,
                    this.selectionArea.TopLeftPosition).Invoke();
            };
        }
    }

    public class PasteNotesOperation : Operation
    {
        public PasteNotesOperation(Model model, SelectionArea selectionArea, Position position)
        {
            Invoke += () =>
            {
                if (Clipboard.GetDataObject().GetData(typeof(SelectionArea)) is SelectionArea data)
                {
                    selectionArea.Reset(data);
                    selectionArea.MovePositionDelta = new Position();
                    foreach (Note note in selectionArea.SelectedNoteList)
                    {
                        model.NoteBook.Add(note);
                        if (note is AirableNote)
                        {
                            AirableNote airable = note as AirableNote;
                            if (airable.IsAirAttached)
                            {
                                model.NoteBook.Add(airable.Air);
                            }
                            if (airable.IsAirHoldAttached)
                            {
                                model.NoteBook.Add(airable.AirHold);
                            }
                        }
                    }
                    foreach (LongNote longNote in selectionArea.SelectedLongNoteList)
                    {
                        model.NoteBook.Add(longNote);
                        longNote.ForEach(x =>
                        {
                            if (x is AirableNote)
                            {
                                AirableNote airable = x as AirableNote;
                                if (airable.IsAirAttached)
                                {
                                    model.NoteBook.Add(airable.Air);
                                }
                                if (airable.IsAirHoldAttached)
                                {
                                    model.NoteBook.Add(airable.AirHold);
                                }
                            }
                        });
                    }
                    selectionArea.Relocate(position, model.LaneBook);
                }
            };
            Undo += () =>
            {
                new ClearAreaNotesOperation(model, selectionArea).Invoke();
            };
        }
    }

    public class ClearAreaNotesOperation : Operation
    {
        SelectionArea selectionArea = new SelectionArea();

        public ClearAreaNotesOperation(Model model, SelectionArea selectionArea)
        {
            Invoke += () =>
            {
                this.selectionArea.Reset(selectionArea);
                selectionArea.ClearAllNotes(model.NoteBook);
                selectionArea.Reset();
            };
            Undo += () =>
            {
                new CopyNotesOperation(this.selectionArea).Invoke();
                new PasteNotesOperation(
                    model,
                    selectionArea,
                    this.selectionArea.TopLeftPosition).Invoke();
            };
        }
    }

    public class ReverseNotesOperation : Operation
    {
        public ReverseNotesOperation(Model model, SelectionArea selectionArea)
        {
            Invoke += () =>
            {
                selectionArea.ReverseNotes(model.NoteBook, model.LaneBook);
            };
            Undo += () =>
            {
                selectionArea.ReverseNotes(model.NoteBook, model.LaneBook);
            };
        }
    }
}
