using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Define
{
    public enum Mode
    {
        None, Add, Edit, Delete
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

    public enum NoteArea
    {
        None, Left, Center, Right
    }

    public enum PanelSize
    {
        Small, Middle, Big
    }

    public enum ScoreScale
    {
        Half, Default, Double, Quad
    }

    public struct NoteValue
    {
        public const float BPMMIN = 1, BPMMAX = 99999999;
        public const float HSMIN = -9999999, HSMAX = 9999999;
    }
}