using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;

namespace NE4S
{
    public class Status
    {
        public static int Mode { get; set; } = Define.ADD;
        public static int Beat { get; set; } = 16;
        public static int Grid { get; set; } = 16;
        public static int Note { get; set; } = Define.TAP;
        public static int NoteSize { get; set; } = 4;
        public static double BPM { get; set; } = 120.0;
        public static bool InvisibleSlideTap { get; set; } = true;
		public static bool IsMousePressed { get; set; } = false;
		public static NoteMaterial selectedNoteMaterial { get; set; } = null;
    }
}
