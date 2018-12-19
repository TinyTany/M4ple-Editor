using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Component
{
    public partial class NewScoreForm : Form
    {
        public NewScoreForm(ScorePanel scorePanel)
        {
            InitializeComponent();
            buttonOK.Click += (s, e) =>
            {
                if(beatNumer.SelectedIndex != -1 && beatDenom.SelectedIndex != -1)
                {
                    scorePanel.SetScore(int.Parse(beatNumer.Text), int.Parse(beatDenom.Text), (int)numOfMeasure.Value);
                    Close();
                }
                else
                {
                    MessageBox.Show("未選択箇所があります\n全ての項目に値を指定してください");
                }
            };
            buttonOK.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;
            beatNumer.SelectedIndex = 3;
            beatDenom.SelectedIndex = 2;
        }
    }
}
