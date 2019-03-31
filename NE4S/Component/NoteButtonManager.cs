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
        public event EventHandler ButtonClicked;
        public NoteButton SelectedButton { get { return Find(x => x.IsSelected); } }

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
            Add(new BPMNoteButton(NoteType.BPM, UpdateSelectedButton)
            {
                ValueMin = NoteValue.BPMMIN, ValueMax = NoteValue.BPMMAX, CurrentValue = 120
            });
            Add(new SpeedNoteButton(NoteType.HIGHSPEED, UpdateSelectedButton)
            {
                ValueMin = NoteValue.HSMIN, ValueMax = NoteValue.HSMAX, CurrentValue = 1
            });
            //
            this.First().SetSelected();
        }

        public void SelectedButtonIncrease()
        {
            if(SelectedButton is BPMNoteButton bpmButton)
            {
                if(bpmButton.CurrentValue + 1 <= bpmButton.ValueMax)
                {
                    Status.CurrentValue++;
                    bpmButton.CurrentValue++;
                    bpmButton.Refresh();
                }
            }
            else if(SelectedButton is SpeedNoteButton speedButton)
            {
                if(speedButton.CurrentValue + 1 <= speedButton.ValueMax)
                {
                    Status.CurrentValue++;
                    speedButton.CurrentValue++;
                    speedButton.Refresh();
                }
            }
            else if(SelectedButton is SizableNoteButton sizableButton)
            {
                if(sizableButton.NoteSize + 1 <= Scores.ScoreInfo.Lanes)
                {
                    Status.NoteSize++;
                    sizableButton.NoteSize++;
                    sizableButton.Refresh();
                }
            }
            else if(SelectedButton is AirNoteButton airButton)
            {
                airButton.ChangeNoteDirection(NoteArea.Right);
                airButton.Refresh();
            }
        }

        public void SelectedButtonDecrease()
        {
            if (SelectedButton is BPMNoteButton bpmButton)
            {
                if (bpmButton.CurrentValue - 1 >= bpmButton.ValueMin)
                {
                    Status.CurrentValue--;
                    bpmButton.CurrentValue--;
                    bpmButton.Refresh();
                }
            }
            else if (SelectedButton is SpeedNoteButton speedButton)
            {
                if (speedButton.CurrentValue - 1 >= speedButton.ValueMin)
                {
                    Status.CurrentValue--;
                    speedButton.CurrentValue--;
                    speedButton.Refresh();
                }
            }
            else if (SelectedButton is SizableNoteButton sizableButton)
            {
                if (sizableButton.NoteSize - 1 >= 1)
                {
                    Status.NoteSize--;
                    sizableButton.NoteSize--;
                    sizableButton.Refresh();
                }
            }
            else if (SelectedButton is AirNoteButton airButton)
            {
                airButton.ChangeNoteDirection(NoteArea.Left);
                airButton.Refresh();
            }
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
            ButtonClicked?.Invoke(noteButton, new EventArgs());
            return;
        }
    }
}
