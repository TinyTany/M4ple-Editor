using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Define
{
    public struct Mode
    {
        public const int NONE = -1, ADD = 0, EDIT = 1, DELETE = 2;
    }

    public struct NoteType
    {
        public const int
            NONE = -1,
            SHORTNOTE_FIRST = 0,
            TAP = 0,
            EXTAP = 1,
            AWEXTAP = 2,
            EXTAPDOWN = 3,
            FLICK = 4,
            HELL = 5,
            SHORTNOTE_LAST = 5,
            HOLD = 6,
            SLIDE = 7,
            SLIDECURVE = 9,
            AIRNOTE_FIRST = 10,
            AIRUPC = 10,
            AIRUPL = 11,
            AIRUPR = 12,
            AIRDOWNC = 13,
            AIRDOWNL = 14,
            AIRDOWNR = 15,
            AIRNOTE_LAST = 15,
            AIRHOLD = 16,
            BPM = 17,
            HIGHSPEED = 20;
    }

    public struct NoteArea
    {
        public const int NONE = -1, LEFT = 0, CENTER = 1, RIGHT = 2;
    }

    public struct PanelSize
    {
        public const int SMALL = 0, MIDDLE = 1, BIG = 2;
    }
}