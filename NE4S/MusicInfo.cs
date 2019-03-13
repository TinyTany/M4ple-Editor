using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S
{
    [Serializable()]
    public class MusicInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Designer { get; set; }
        public int Difficulty { get; set; }
        public string WEKanji { get; set; }
        public decimal WEStars { get; set; } = 1;
        public string PlayLevel { get; set; }
        public string SongId { get; set; }
        public string MusicFileName { get; set; }
        public decimal MusicOffset { get; set; }
        public string MovieFileName { get; set; }
        public decimal MovieOffset { get; set; }
        public string JacketFileName { get; set; }
        public string BackgroundFileName { get; set; }
        public string ExportPath { get; set; }
        public decimal MeasureOffset { get; set; }
        public int Metronome { get; set; }
        public decimal SlideCurveSegment { get; set; } = 20;
        [field: NonSerialized]
        public bool HasExported { get; set; } = false;
        [field: NonSerialized]
        public string SubTitle { get; set; }
        [field: NonSerialized]
        public string Genre { get; set; }
        [field: NonSerialized]
        public bool EnablePriority { get; set; } = false;
        [field: NonSerialized]
        public int TicksPerBeat { get; set; } = 192; /* SUSでのデフォルト値 */

        public MusicInfo() { }
    }
}
