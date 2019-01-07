using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;

namespace NE4S.Operation
{
    public class AddNoteOperation : Operation
    {
        public AddNoteOperation(Model model, Note note)
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
}
