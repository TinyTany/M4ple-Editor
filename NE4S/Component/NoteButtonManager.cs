using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Define;

namespace NE4S.Component
{
    public class NoteButtonManager : List<NoteButton>
    {
        public NoteButtonManager()
        {
            Add(new NoteButton(NoteType.TAP, UpdateSelectedButton));
            Add(new NoteButton(NoteType.EXTAP, UpdateSelectedButton));
            Add(new NoteButton(NoteType.AWEXTAP, UpdateSelectedButton));
            Add(new NoteButton(NoteType.FLICK, UpdateSelectedButton));
            Add(new NoteButton(NoteType.HELL, UpdateSelectedButton));
            Add(new NoteButton(NoteType.HOLD, UpdateSelectedButton));
            Add(new NoteButton(NoteType.SLIDE, UpdateSelectedButton));
            Add(new NoteButton(NoteType.SLIDECURVE, UpdateSelectedButton));
            Add(new NoteButton(NoteType.AIRUPC, UpdateSelectedButton));
            Add(new NoteButton(NoteType.AIRDOWNC, UpdateSelectedButton));
            Add(new NoteButton(NoteType.AIRHOLD, UpdateSelectedButton));
            this.First().SetSelected();
        }

        public void UpdateSelectedButton(NoteButton noteButton)
        {
            foreach(NoteButton itrButton in this)
            {
                if(itrButton == noteButton)
                {
                    itrButton.SetSelected();
                }
                else
                {
                    itrButton.SetUnSelected();
                }
            }
            return;
        }
    }
}
