using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Notes;

namespace NE4S.Component
{
    public partial class SetValueCustomForm : Form
    {
        public SetValueCustomForm(ValueNoteButton valueNoteButton)
        {
            InitializeComponent();
            buttonOK.Click += (s, e) =>
            {
                Status.CurrentValue = valueNoteButton.CurrentValue = (float)numericUpDown1.Value.MyRound();
                valueNoteButton.Refresh();
                Close();
            };
            buttonCancel.Click += (s, e) =>
            {
                Close();
            };
            numericUpDown1.Minimum = (decimal)valueNoteButton.ValueMin;
            numericUpDown1.Maximum = (decimal)valueNoteButton.ValueMax;
            numericUpDown1.Value = (decimal)valueNoteButton.CurrentValue;
            if (valueNoteButton is BPMNoteButton)
            {
                SetLabelText(0);
            }
            else if (valueNoteButton is SpeedNoteButton)
            {
                SetLabelText(1);
            }
        }

        public SetValueCustomForm(AttributeNote attributeNote)
        {
            InitializeComponent();
            buttonOK.Click += (s, e) =>
            {
                attributeNote.NoteValue = (float)numericUpDown1.Value.MyRound();
                Close();
            };
            buttonCancel.Click += (s, e) =>
            {
                Close();
            };
            if (attributeNote is BPM)
            {
                numericUpDown1.Minimum = (decimal)Define.NoteValue.BPMMIN;
                numericUpDown1.Maximum = (decimal)Define.NoteValue.BPMMAX;
                SetLabelText(0);
            }
            else if (attributeNote is HighSpeed)
            {
                numericUpDown1.Minimum = (decimal)Define.NoteValue.HSMIN;
                numericUpDown1.Maximum = (decimal)Define.NoteValue.HSMAX;
                SetLabelText(1);
            }
            numericUpDown1.Value = (decimal)attributeNote.NoteValue;
        }

        /// <summary>
        /// BPM: 0, HISPEED: 1
        /// </summary>
        /// <param name="type"></param>
        private void SetLabelText(int type)
        {
            switch (type)
            {
                // BPM
                case 0:
                    Text = "BPM指定";
                    labelMain.Text = "BPM";
                    labelSub.Text = "";
                    break;
                // HISPEED
                case 1:
                    Text = "HighSpeed指定";
                    labelMain.Text = "HighSpeed";
                    labelSub.Text = "x";
                    break;
            }
        }
    }
}
