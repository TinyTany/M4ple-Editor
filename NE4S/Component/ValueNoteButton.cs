using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Component
{
    public class ValueNoteButton : SizableNoteButton
    {
        public float ValueMin { get; set; }
        public float ValueMax { get; set; }
        public float CurrentValue { get; set; }

        public ValueNoteButton(int noteType, NoteButtonEventHandler handler) : base(noteType, handler)
        {
            
        }

        protected override void PreviewBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isSelected)
            {
                base.PreviewBox_MouseDown(sender, e);
            }
            else
            {
                RefreshButtonArea(e.Location);
                if (buttonArea == ButtonArea.Top)
                {
                    Status.CurrentValue = CurrentValue <= ValueMax - 1 ? ++CurrentValue : CurrentValue = ValueMax;
                }
                else if (buttonArea == ButtonArea.Bottom)
                {
                    Status.CurrentValue = CurrentValue >= ValueMin + 1 ? --CurrentValue : CurrentValue = ValueMin;
                }
            }
        }

        public override void SetSelected()
        {
            base.SetSelected();
            Status.CurrentValue = CurrentValue;
        }
    }
}
