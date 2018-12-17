using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Component
{
    public class LongLaneSignProvider
    {
        private readonly char[] signArray;
        private List<KeyValuePair<char, TickRange>> keyValuePairs;

        private class TickRange
        {
            public int StartTick { get; private set; }
            public int EndTick { get; private set; }

            public TickRange(int startTick, int endTick)
            {
                StartTick = startTick;
                EndTick = endTick;
            }
        }

        public LongLaneSignProvider()
        {
            signArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            keyValuePairs = new List<KeyValuePair<char, TickRange>>();
            foreach(char sign in signArray)
            {
                keyValuePairs.Add(new KeyValuePair<char, TickRange>(sign, null));
            }
        }

        public string GetAvailableSign(int startTick, int endTick)
        {
            var keyValuePair = keyValuePairs.Find(x => x.Value == null || (endTick < x.Value.StartTick || x.Value.EndTick < startTick));
            if (keyValuePair.Equals(default(KeyValuePair<char, TickRange>)))
            {
                System.Diagnostics.Debug.Assert(false, "レーン識別番号を取得できませんでした");
                return signArray.Last().ToString();
            } 
            keyValuePair = new KeyValuePair<char, TickRange>(keyValuePair.Key, new TickRange(startTick, endTick));
            return keyValuePair.Key.ToString();
        }

    }
}
