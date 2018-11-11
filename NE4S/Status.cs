using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;

namespace NE4S
{
    public static class Status
    {
        public static int Mode { get; set; } = Define.Mode.ADD;
        public static int Beat { get; set; } = 16;
        public static int Grid { get; set; } = 16;
        public static int Note { get; set; } = Define.NoteType.SLIDE;
        public static int NoteSize { get; set; } = 4;
        public static double BPM { get; set; } = 120.0;
        public static bool InvisibleSlideTap { get; set; } = true;
		public static bool IsMousePressed { get; set; } = true;
		public static Note SelectedNote { get; set; } = null;
        public static int SelectedNoteArea { get; set; } = Define.NoteArea.NONE;

        public static bool IsSlideRelayVisible { get; set; } = true;
        public static bool IsSlideCurveVisible { get; set; } = true;
        public static bool IsShortNoteVisible { get; set; } = true;
        public static bool IsHoldVisible { get; set; } = true;
        public static bool IsSlideVisible { get; set; } = true;
        public static bool IsAirHoldVisible { get; set; } = true;
    }
}
