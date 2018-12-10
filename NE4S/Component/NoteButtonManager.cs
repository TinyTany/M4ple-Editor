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
            //サイズ変更可能，向きはない
            Add(new SizableNoteButton(NoteType.TAP, UpdateSelectedButton));
            Add(new SizableNoteButton(NoteType.EXTAP, UpdateSelectedButton));
            Add(new SizableNoteButton(NoteType.EXTAPDOWN, UpdateSelectedButton));
            Add(new SizableNoteButton(NoteType.AWEXTAP, UpdateSelectedButton));
            Add(new SizableNoteButton(NoteType.FLICK, UpdateSelectedButton));
            Add(new SizableNoteButton(NoteType.HELL, UpdateSelectedButton));
            Add(new SizableNoteButton(NoteType.HOLD, UpdateSelectedButton));
            Add(new SizableNoteButton(NoteType.SLIDE, UpdateSelectedButton));
            Add(new SizableNoteButton(NoteType.SLIDECURVE, UpdateSelectedButton));
            //向き変更可能，サイズは親ノーツに依存なので必要ない
            Add(new AirNoteButton(NoteType.AIRUPC, UpdateSelectedButton));
            Add(new AirNoteButton(NoteType.AIRDOWNC, UpdateSelectedButton));
            //サイズも向きもない
            Add(new NoteButton(NoteType.AIRHOLD, UpdateSelectedButton));
            //数値の変更だけ
            Add(new ValueNoteButton(NoteType.BPM, UpdateSelectedButton));
            Add(new ValueNoteButton(NoteType.HIGHSPEED, UpdateSelectedButton));
            //
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
