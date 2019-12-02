using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes.Abstract;
using System.Drawing;

namespace NE4S.Notes
{
    /// <summary>
    /// データのみのノーツクラスを物体化するクラス
    /// </summary>
    public class NoteBox
    {
        private readonly Note note;

        private RectangleF rectangle;

        private NoteBox() { }

        public NoteBox(Note note)
        {
            this.note = note;
        }
    }
}
