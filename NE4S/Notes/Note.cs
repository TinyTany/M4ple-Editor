using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Notes
{
    public class Note
    {
        private int size, lane;
        private Pos pos;

        public Note()
        {
            size = lane = 0;
            pos = new Pos();
        }

        public int Size
        {
            get { return this.size; }
        }

        public int Lane
        {
            get { return this.lane; }
        }
    }
}
