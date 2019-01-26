﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NE4S.Scores;

namespace NE4S
{
    /// <summary>
    /// 置いたノーツの位置情報
    /// </summary>
    [Serializable()]
    public class Position
    {
        /// <summary>
        /// ノーツの左端のレーン番号（0-15）
        /// </summary>
        public int Lane { get; private set; }
        public int Tick { get; private set; }

        public Position()
        {
            Lane = 0;
            Tick = 0;
        }

        public Position(Position position)
        {
            Lane = position.Lane;
            Tick = position.Tick;
        }

        public Position(int lane, int tick)
        {
            Lane = lane;
            Tick = tick;
        }

        public bool Equals(Position position)
        {
            if(position == null) { return false; }
            return (Tick == position.Tick) && (Lane == position.Lane);
        }

		public void PrintPosition()
		{
            System.Diagnostics.Debug.WriteLine("(Lane, Tick) = (" + Lane + ", " + Tick + ")");
        }

        public Position Next()
        {
            return new Position(Lane, Tick + (ScoreInfo.MaxBeatDiv / Status.Beat));
        }
    }
}
