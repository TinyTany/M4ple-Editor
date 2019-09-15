using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Component
{
    public class LongLaneSignProvider
    {
        private readonly char[] signArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private readonly List<KeyValuePair<char, (int start, int end)>> 
            keyValuePairs = new List<KeyValuePair<char, (int start, int end)>>(64);

        public LongLaneSignProvider()
        {
            foreach(var sign in signArray)
            {
                keyValuePairs.Add(new KeyValuePair<char, (int, int)>(sign, (0, 0)));
            }
        }

        public string GetAvailableSign(int startTick, int endTick)
        {
            var keyValuePair = keyValuePairs.Find(x => x.Value == (0, 0) || (endTick < x.Value.start || x.Value.end < startTick));
            if (keyValuePair.Equals(default(KeyValuePair<char, (int, int)>)))
            {
                Logger.Error("レーン識別番号を取得できませんでした", true);
                return signArray.Last().ToString();
            }
            int index = keyValuePairs.IndexOf(keyValuePair);
            keyValuePairs[index] = new KeyValuePair<char, (int, int)>(keyValuePair.Key, (startTick, endTick));
            return keyValuePair.Key.ToString();
        }
    }
}
