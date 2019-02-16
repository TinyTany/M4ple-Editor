using NE4S.Scores;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes
{
    public class HighSpeedTimeLineBook
    {
        /* SUSインポート時の解析中は別だけど、解析が済んだら(or新規追加時は)定義番号は連番で扱う前提の実装 */
        /* 定義番号0番は小節線にも適用される前提 */
        private Dictionary<int, HighSpeedTimeLine> book;

        public HighSpeedTimeLineBook()
        {
            book = new Dictionary<int, HighSpeedTimeLine>();
        }

        /// <summary>
        /// 定義番号と定義内容から新規にHighSpeedTimeLineを作成し、リストに追加します。
        /// </summary>
        /// <param name="defNumber"></param>
        /// <param name="define"></param>
        /// <returns></returns>
        public bool Add(int defNumber, string define, int ticksPerBeat)
        {
            HighSpeedTimeLine timeLine = new HighSpeedTimeLine(define, this);
            if (timeLine.Valid)
            {
                Add(defNumber, timeLine);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Add(int defNumber, HighSpeedTimeLine timeLine)
        {
            if (book.ContainsKey(defNumber))
            {
                return book[defNumber].Add(timeLine);
            }
            else
            {
                book.Add(defNumber, timeLine);
                return true;
            }
        }

        public bool ContainsKey(int defNumber)
        {
            return book.ContainsKey(defNumber);
        }

        public HighSpeedTimeLine Get(int defNumber)
        {
            return (ContainsKey(defNumber)) ? book[defNumber] : null;
        }

        public void FinalizePosition(ScoreBook scoreBook, Dictionary<int, int> matching)
        {
            Dictionary<int, HighSpeedTimeLine> newBook = new Dictionary<int, HighSpeedTimeLine>();
            int newDefNumber = 1;
            foreach (KeyValuePair<int, HighSpeedTimeLine> pair in book)
            {
                pair.Value.FinalizePosition(scoreBook);
                newBook.Add(newDefNumber, pair.Value);
                matching.Add(pair.Key, newDefNumber);
                ++newDefNumber;
            }
            book = newBook;
        }
    }
}