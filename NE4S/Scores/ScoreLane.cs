using NE4S.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Scores
{
    /// <summary>
    /// 画面上で譜面を表示するための1本分の譜面レーン
    /// </summary>
    public class ScoreLane
    {
        private List<Score> scores;
        private List<Note> notes;

        public ScoreLane()
        {
            scores = new List<Score>();
            notes = new List<Note>();
        }
    }
}
