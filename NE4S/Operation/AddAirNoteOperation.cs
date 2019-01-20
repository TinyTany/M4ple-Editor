using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;

namespace NE4S.Operation
{
    public class AddAirNoteOperation : Operation
    {
        public AddAirNoteOperation(Model model, Air air, AirableNote airable)
        {
            Invoke += () =>
            {
                model.NoteBook.Add(air);
                airable.AttachAir(air);
            };
            Undo += () =>
            {
                model.NoteBook.Delete(air);
                airable.DetachAir();
            };
        }
    }
}
