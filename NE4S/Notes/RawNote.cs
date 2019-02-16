using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NE4S.Notes
{
    [Serializable()]
    public class RawNote : Note
    {
        public override int NoteID => -1;
        public RawNoteType NoteType { get; protected set; } = RawNoteType.Undefined;
        public int Measure { get; protected set; } = 0;
        public double Tick /* 小節内のどの辺にいるかを返す。 範囲は [0.0, 1.0) で扱う。 */
        {
            get
            {
                return BeatNum / (double)BeatDen;
            }
        }
        private int BeatNum = 0;
        private int BeatDen = 1;
        public int Identifier { get; protected set; } = -1;

        public enum RawNoteType
        {
            Undefined = -20,

            BPM = -1,
            TimeLine = -2,
            Attribute = -3,

            // まずショートノーツを判定し
            Tap = 0,
            ExTap,
            Flick,
            HellTap,
            AwesomeExTap,
            ExTapDown,

            // 続いてHoldを判定し
            HoldBegin,
            HoldEnd,

            // 続けてSlideを判定し
            SlideBegin,
            SlideTap,
            SlideRelay,
            SlideCurve,
            SlideEnd,

            // AirableNoteが出そろったのでAirを判定し
            AirUpC,
            AirUpL,
            AirUpR,
            AirDownC,
            AirDownL,
            AirDownR,

            // Airが出そろったのでAirHoldを判定します
            AirHoldBegin,
            AirAction,
            AirHoldEnd
        }

        public RawNote(RawNoteType type, int lane, int size, int measure, int beatNum, int beatDen)
            : base(size, new Position(lane, 0), new PointF(), -1)
        {
            Measure = measure;
            BeatNum = beatNum;
            BeatDen = beatDen;
            Identifier = -1;
        }
        public RawNote(RawNoteType type, int lane, int size, int measure, int beatNum, int beatDen, int identifire)
            : base(size, new Position(lane, 0), new PointF(), -1)
        {
            Measure = measure;
            NoteType = type;
            BeatNum = beatNum;
            BeatDen = beatDen;
            Identifier = identifire;
        }

        // 初期化時にはノーツのPositionを確定できない(その小節の拍子数が確定していない)
        // そこで初期化時のPositionにはその小節内での時間軸ソートができるような適当な値を設定したうえで
        // そのノーツ定義が小節を何分割していたかと分割したうちの何番目のノーツ定義だったかを別に保持する
        // 小節の拍子数が確定したならばその定義を持った拍子(Score)を渡してこのメソッドを呼び出すことで
        // Positionのtickを確定させる
        public Position GetFinallyPosition(Scores.Score score)
        {
            int scoreStartTick = score.StartTick;
            int scoreBeatNum = score.BeatNumer;
            int scoreBeatDenom = score.BeatDenom;
            int newPosTick = scoreStartTick + BeatNum * ((Scores.ScoreInfo.MaxBeatDiv / scoreBeatDenom) * scoreBeatNum) / BeatDen;
            return new Position(Position.Lane, newPosTick);
        }
    }
}
