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
            Invoke += () =>
            {
                model.NoteBook.Delete(note);
            };
            Undo += () =>
            {
                model.NoteBook.Add(note);
            };
        }
    }
}
