using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Notes.Interface;
using NE4S.Scores;

namespace NE4S.Notes.Abstract
{
    [Serializable()]
    public abstract class AttributeNote : IAttributeNote
    {
        public double Value { get; private set; }
        public int Tick { get; private set; }
        public abstract NoteType NoteType { get; }

        protected AttributeNote() { }

        protected AttributeNote(int tick, double value)
        {
            Tick = tick;
            Value = value;
        }

        public abstract bool SetValue(double value);

        public abstract bool Relocate(int tick);
    }
}
