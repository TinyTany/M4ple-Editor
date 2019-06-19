using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Notes
{
    /// <summary>
    /// あるSlideノーツの連続な部分Slideを表します
    /// </summary>
    public class PartialSlide
    {
        public Slide Slide { get; private set; } = null;
        public List<Note> Partial { get; private set; } = new List<Note>();

        public PartialSlide(Slide slide, Note start, Note end)
        {
            if (slide == null)
            {
                System.Diagnostics.Debug.Assert(false, "引数のslideがnullです");
                return;
            }
            Slide = slide;
            if (!slide.Notes.Contains(start) || !slide.Notes.Contains(end))
            {
                System.Diagnostics.Debug.Assert(false, "引数のnoteが不正です");
                return;
            }
            Partial.AddRange(
                slide.Notes.Where(
                    x => 
                    start.Position.Tick <= x.Position.Tick && x.Position.Tick <= end.Position.Tick));
        }
    }
}
