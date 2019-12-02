using NE4S.Notes.Abstract;
using NE4S.Notes.Concrete;
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
                Logger.Error("引数のslideがnullです", true);
                return;
            }
            Slide = slide;
            if (!slide.Contains(start) || !slide.Contains(end))
            {
                Logger.Error("引数のnoteが不正です", true);
                return;
            }
            Partial.AddRange(
                slide.Where(
                    x => 
                    start.Position.Tick <= x.Position.Tick && x.Position.Tick <= end.Position.Tick));
        }
    }
}
