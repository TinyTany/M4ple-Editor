using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Notes
{
    /// <summary>
    /// 長いノーツ(Hold,Slide,AirHold)1つ分を表す
    /// </summary>
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
