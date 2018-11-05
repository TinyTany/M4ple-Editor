﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Notes
{
    public class AirableNote : Note
    {
        private Air air = null;
        private AirHold airHold = null;

        public AirableNote() { }

        public AirableNote(int size, Position pos, PointF location) : base(size, pos, location) { }
    }
}