using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Notes
{
    /// <summary>
    /// 長いノーツ(Hold,Slide,AirHold)1つ分を表す
    /// </summary>
    [Serializable()]
    public class LongNote : List<Note>
    {
        //帯の描画位置がちょっと上にずれてるので調節用の変数を用意
        protected static readonly PointF drawOffset = new PointF(2, 0);
        //帯の大きさが縦に少し短いので調整
        protected static readonly float deltaHeight = .2f;

        public LongNote()
        {
            
        }

        protected bool IsPositionAvailable(Note note, Position position)
        {
            if (position.CompareTo(this.OrderBy(x => x.Pos).First().Pos) < 0) return false;
            foreach (Note itrNote in this.Where(x => x != note))
            {
                if (position.Equals(itrNote.Pos)) return false;
            }
            return true;
        }

        public bool IsDrawable() => 
            this.Where(x => x.Pos.Bar >= Status.DrawScoreBarFirst && x.Pos.Bar <= Status.DrawScoreBarLast).Any();

        public virtual void Draw(PaintEventArgs e, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
		{

		}

        /// <summary>
        /// 2つのPosition変数からその仮想的な縦の距離を計算する
        /// </summary>
        /// <param name="pastPosition"></param>
        /// <param name="futurePosition"></param>
        /// <param name="scoreBook"></param>
        /// <returns></returns>
        protected static float PositionDistance(Position pastPosition, Position futurePosition, ScoreBook scoreBook)
        {
            float distance = 0;
            //4分の4拍子1小節分の高さ
            float baseBarSize = ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight;
            Score pastScore = scoreBook.At(pastPosition.Bar - 1), futureScore = scoreBook.At(futurePosition.Bar - 1);
            //2ノーツが同一のScore上にある場合も使える
            if (pastScore.Index == futureScore.Index)
            {
                return (futurePosition.Size - pastPosition.Size) * baseBarSize;
            }
            distance += baseBarSize * (pastScore.BarSize - pastPosition.Size);
            for (int i = pastScore.Index + 1; i <= futureScore.Index - 1; ++i)
            {
                distance += scoreBook.At(i).BarSize * baseBarSize;
            }
            distance += baseBarSize * futurePosition.Size;
            return distance;
        }
    }
}
