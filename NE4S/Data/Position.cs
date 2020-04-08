using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NE4S.Scores;

namespace NE4S.Data
{
    /// <summary>
    /// 置いたノーツの位置情報
    /// </summary>
    [Serializable()]
    public sealed class Position
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
            if (position == null)
            {
                Logger.Error("Positionのコピーに失敗しました。引数がnullです。");
                Lane = 0;
                Tick = 0;
                return;
            }
            Lane = position.Lane;
            Tick = position.Tick;
        }

        public Position(int lane, int tick)
        {
            Lane = lane;
            Tick = tick;
        }

        public static Position operator+(Position lhs, Position rhs)
        {
            return new Position(lhs.Lane + rhs.Lane, lhs.Tick + rhs.Tick);
        }

        public static Position operator-(Position lhs, Position rhs)
        {
            return new Position(lhs.Lane - rhs.Lane, lhs.Tick - rhs.Tick);
        }

        public static Position operator-(Position p)
        {
            return new Position(-p.Lane, -p.Tick);
        }

        public static bool operator ==(Position lhs, Position rhs)
        {
            if (object.ReferenceEquals(lhs, rhs)) { return true; }
            if (lhs is null || rhs is null) { return false; }
            return (lhs.Lane == rhs.Lane) && (lhs.Tick == rhs.Tick);
        }

        public static bool operator !=(Position lhs, Position rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return this == (Position)obj;
        }

        public override int GetHashCode()
        {
            var hashCode = 2076266859;
            hashCode = hashCode * -1521134295 + Lane.GetHashCode();
            hashCode = hashCode * -1521134295 + Tick.GetHashCode();
            return hashCode;
        }
        public void Print()
        {
            System.Diagnostics.Debug.WriteLine("(Lane, Tick) = (" + Lane + ", " + Tick + ")");
        }
    }
}
