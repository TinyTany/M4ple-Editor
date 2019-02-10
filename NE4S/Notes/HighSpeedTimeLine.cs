using NE4S.Scores;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes
{
    public class HighSpeedTimeLine : List<HighSpeed>
    {
        public bool Valid { get; protected set; } = false;
        public bool Finalized { get; protected set; } = false;

        private static Regex rgxTimeLineBase = new Regex(@"(?:(inherit)\s*\:\s*(\w+))|(?:(\d+)\s*'\s*(\d+)(?:\s*\:\s*((?:[\+\-]?\d+(?:\.\d*)?)|i|invisible|v|visible))+)");
        private static string rgxTimeLineBaseTxt = @"(?:(?:inherit)\s*\:\s*(?:\w+))|(?:(?:\d+)\s*'\s*(?:\d+)(?:\s*\:\s*(?:(?:[\+\-]?\d+(?:\.\d*)?)|i|invisible|v|visible))+)";
        private static Regex rgxTimeLine = new Regex(@"(?:(?:^|\s*,\s*)(" + rgxTimeLineBaseTxt + @"))+,?$");

        public HighSpeedTimeLine() { }

        public HighSpeedTimeLine(string define, HighSpeedTimeLineBook timeLineBook)
        {
            Add(define, timeLineBook);
        }

        public bool Add(string define, HighSpeedTimeLineBook timeLineBook)
        {
            Valid = false;
            if (define == null || timeLineBook == null)
            {
                return false;
            }

            if (define.Length == 0)
            {
                Valid = true;
                return true;
            }

            Match timeLinesMatch = rgxTimeLine.Match(define);
            if (timeLinesMatch.Success)
            {
                CaptureCollection timeLinesCollection = timeLinesMatch.Groups[1].Captures;
                for (int i = 0; i < timeLinesCollection.Count; ++i)
                {
                    Match timeLineMatch = rgxTimeLineBase.Match(timeLinesCollection[i].ToString());
                    if (timeLineMatch.Success)
                    {
                        if (timeLineMatch.Groups[1].ToString() == "inherit")
                        {
                            int defNumber = MyUtil.ToIntAs36(timeLineMatch.Groups[2].ToString());
                            if (timeLineBook.ContainsKey(defNumber))
                            {
                                if (!Add(timeLineBook.Get(defNumber)))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                /* 未定義ハイスピ指定の複製は困ります */
                                return false;
                            }
                            continue;
                        }

                        int measure = MyUtil.ToInt(timeLineMatch.Groups[3].ToString());
                        int tick = MyUtil.ToInt(timeLineMatch.Groups[4].ToString());
                        double susTicksPerBeat = 192.0; /* こいつは本来グローバル変数(SUSから指定できる) */
                        double m4pleTicksPerBeat = 48.0;

                        /* measure,tick管理クソみたいになってるけどﾕﾙｼﾃ */
                        
                        CaptureCollection propsCollection = timeLineMatch.Groups[5].Captures;
                        float hsValue = 1;
                        HighSpeed.VisibleState visibleState = HighSpeed.VisibleState.Visible;
                        for (int j = 0; j < propsCollection.Count; ++j)
                        {
                            string value = propsCollection[j].ToString();
                            if (value == "i" || value == "invisible")
                            {
                                visibleState = HighSpeed.VisibleState.Invisible;
                            }
                            else if (value == "v" || value == "visible")
                            {
                                visibleState = HighSpeed.VisibleState.Invisible;
                            }
                            else
                            {
                                hsValue = (float)MyUtil.ToDouble(value);
                            }
                        }
                        HighSpeed hs = new HighSpeed(new Position(measure, (int)(tick * m4pleTicksPerBeat / susTicksPerBeat)), new PointF(), hsValue, -1)
                        {
                            Visible = visibleState
                        };
                        Add(hs);
                    }
                }
                Valid = true;
                return true;
            }
            return false;
        }

        public new bool Add(HighSpeed hs)
        {
            if (hs is null)
            {
                return false;
            }

            if (!Finalized && hs.Position.Lane == -1)
            {
                return false;
            }

            if (Finalized && hs.Position.Lane >= 0)
            {
                return false;
            }

            base.Add(hs);

            return true;
        }

        public bool Add(HighSpeedTimeLine timeLine)
        {
            if (timeLine is null)
            {
                return false;
            }

            if (Finalized != timeLine.Finalized)
            {
                /* 位置校正済みのTimeLineには校正済みのTimeLineを、未校正のTimeLineには未校正のTimeLineを入れてくださいな */
                return false;
            }

            for (int j = 0; j < timeLine.Count; ++j)
            {
                int i;
                for (i = 0; i < this.Count; ++i)
                {
                    if (this[i].Position.Equals(timeLine[j].Position))
                    {
                        /* 同一チックに重複定義があった場合は上書きする方針で */
                        this[i] = timeLine[j];
                    }
                }
                if (i >= this.Count)
                {
                    Add(timeLine[j]);
                }
            }

            return true;
        }

        public bool FinalizePosition(ScoreBook scoreBook)
        {
            if (scoreBook is null)
            {
                return false;
            }

            for (int i = 0; i < this.Count; ++i)
            {
                if (this[i].Position.Lane == -1)
                {
                    /* もう位置の校正は済んでますよ */
                    continue;
                }

                if (scoreBook.At(this[i].Position.Lane) is null)
                {
                    /* あなたの属する小節ないみたいですけど…… */
                    return false;
                }

                this[i].Relocate(new Position(-1, this[i].Position.Tick + scoreBook.At(this[i].Position.Lane).StartTick));
            }

            Finalized = true;
            return true;
        }
    }
}
