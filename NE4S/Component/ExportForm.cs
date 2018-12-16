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
using NE4S.Notes;
using System.IO;

namespace NE4S.Component
{
    public partial class ExportForm : Form
    {
        public ExportForm()
        {
            InitializeComponent();
        }

        private bool Export(string path, ScoreBook scoreBook, NoteBook noteBook)
        {
            using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8))
            {
                streamWriter.WriteLine("This score was created by M4ple Editor.");

                List<Note> shortNotes = noteBook.ShortNotes;
                streamWriter.WriteLine("ShortNote");
                foreach(Score score in scoreBook)
                {
                    List<Note> currentScoreNotes = shortNotes.Where(x => score.StartTick <= x.Position.Tick && x.Position.Tick <= score.EndTick).ToList();
                    
                }
            }
            return true;
        }
    }
}
