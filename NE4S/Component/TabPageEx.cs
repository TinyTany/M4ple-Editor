using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Component
{
    public class TabPageEx : TabPage
    {
        public ScorePanel ScorePanel { get; set; }

        public TabPageEx(string name) : base(name)
        {
            
        }
    }
}
