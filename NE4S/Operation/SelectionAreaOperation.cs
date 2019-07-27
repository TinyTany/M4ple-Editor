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
        SelectionArea selectionArea = new SelectionArea();

        public CopyNotesOperation(SelectionArea selectionArea)
        {
            Invoke += () =>
            {
                this.selectionArea.Reset(selectionArea);
                Clipboard.SetDataObject(selectionArea);
                //NOTE: どっちにするか（コピー後矩形を保持or破棄）迷う...
                //selectionArea = new SelectionArea();
            };
        }
    }

    public class CutNotesOperation : Operation
    {
        SelectionArea selectionArea;

        public CutNotesOperation(Model model, SelectionArea selectionArea)
        {
            this.selectionArea = new SelectionArea(selectionArea);

            Invoke += () =>
            {
                new CopyNotesOperation(this.selectionArea).Invoke();
                new ClearAreaNotesOperation(model, selectionArea).Invoke();
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
        SelectionArea selectionArea = null;

        /// <summary>
        /// Clipboardのデータから貼り付けを行います
        /// </summary>
        // REVIEW
        public PasteNotesOperation(Model model, SelectionArea selectionArea, Position position)
        {
            Position pastPosition = null;
            var book = model.NoteBook;

            if (Clipboard.GetDataObject().GetData(typeof(SelectionArea)) is SelectionArea data)
            {
                this.selectionArea = data;
                pastPosition = data.TopLeftPosition;
            }

            Invoke += () =>
            {
                if (this.selectionArea == null) return;
                selectionArea.Reset(this.selectionArea);
                selectionArea.MovePositionDelta = new Position();
                foreach (Note note in selectionArea.SelectedNoteList)
                {
                    book.Put(note);
                    if (note is AirableNote)
                    {
                        AirableNote airable = note as AirableNote;
                        if (airable.IsAirAttached)
                        {
                            book.Put(airable.Air);
                        }
                        if (airable.IsAirHoldAttached)
                        {
                            book.Put(airable.AirHold);
                        }
                    }
                }
                foreach (LongNote longNote in selectionArea.SelectedLongNoteList)
                {
                    book.Put(longNote);
                    longNote.ForEach(x =>
                    {
                        if (x is AirableNote)
                        {
                            AirableNote airable = x as AirableNote;
                            if (airable.IsAirAttached)
                            {
                                book.Put(airable.Air);
                            }
                            if (airable.IsAirHoldAttached)
                            {
                                book.Put(airable.AirHold);
                            }
                        }
                    });
                }
                selectionArea.Relocate(position, model.LaneBook);
            };
            Undo += () =>
            {
                if (selectionArea == null) return;
                selectionArea.Relocate(pastPosition, model.LaneBook);
                new ClearAreaNotesOperation(model, selectionArea).Invoke();
            };
        }
    }

    public class PasteAndReverseNotesOperation : Operation
    {
        PasteNotesOperation paste;
        ReverseNotesOperation reverse;

        public PasteAndReverseNotesOperation(Model model, SelectionArea selectionArea, Position position)
        {
            paste = new PasteNotesOperation(model, selectionArea, position);
            reverse = new ReverseNotesOperation(model, selectionArea);

            Invoke += () =>
            {
                paste.Invoke();
                reverse.Invoke();
            };
            Undo += () =>
            {
                reverse.Undo();
                paste.Undo();
            };
        }
    }

    public class ClearAreaNotesOperation : Operation
    {
        SelectionArea selectionArea;

        public ClearAreaNotesOperation(Model model, SelectionArea selectionArea)
        {
            this.selectionArea = new SelectionArea(selectionArea);

            Invoke += () =>
            {
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
