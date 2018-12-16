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
        //NOTE: これの値の順番を利用してAir系ノーツを判別してるコードがNoteButtonクラスにあるのでうかつに数字を変えないほうがいい
        public const int
            NONE = -1,
            TAP = 0,
            EXTAP = 1,
            AWEXTAP = 2,
            FLICK = 3,
            HELL = 4,
            HOLD = 5,
            SLIDE = 6,
            SLIDECURVE = 9,
            AIRUPC = 10,
            AIRUPL = 11,
            AIRUPR = 12,
            AIRDOWNC = 13,
            AIRDOWNL = 14,
            AIRDOWNR = 15,
            AIRHOLD = 16,
            BPM = 17,
            HIGHSPEED = 20,
            EXTAPDOWN = 21;
    }

    public struct NoteArea
    {
        public const int NONE = -1, LEFT = 0, CENTER = 1, RIGHT = 2;
    }
}