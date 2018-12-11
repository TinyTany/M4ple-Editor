using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Component
{
    public partial class SetValueCustomForm : Form
    {
        private ValueNoteButton valueNoteButton = null;

        public SetValueCustomForm(ValueNoteButton valueNoteButton)
        {
            InitializeComponent();
            buttonOK.Click += ButtonOK_Click;
            buttonCancel.Click += ButtonCancel_Click;
            this.valueNoteButton = valueNoteButton;
            numericUpDown1.Minimum = (decimal)valueNoteButton.ValueMin;
            numericUpDown1.Maximum = (decimal)valueNoteButton.ValueMax;
            numericUpDown1.Value = (decimal)valueNoteButton.CurrentValue;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            Status.CurrentValue = valueNoteButton.CurrentValue = (float)numericUpDown1.Value.MyRound();
            valueNoteButton.Refresh();
            Close();
            Dispose();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }
    }
}
