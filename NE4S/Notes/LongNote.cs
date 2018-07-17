using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Notes
{
    public class LongNote
    {
        private List<Note> notes;

        public LongNote()
        {
            notes = new List<Note>();
        }

        public List<Note> Notes
        {
            get { return this.notes; }
        }
    }
}
