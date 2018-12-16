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

                /* TODO: この辺に基本情報やBPM,ハイスピの情報を書き出す処理書く */

                streamWriter.WriteLine("\nSet measure's pulse");
                foreach(Score score in scoreBook)
                {
                    if(score == scoreBook.First() || (score.BarSize != scoreBook.Prev(score).BarSize))
                    {
                        streamWriter.WriteLine("#" + score.Index.ToString("D3") + "02: " + (4.0 * score.BarSize));
                    }
                }
                streamWriter.WriteLine("\nShortNote");
                List<Note> shortNotes = noteBook.ShortNotes;
                foreach (Score score in scoreBook)
                {
                    List<Note> currentScoreNotes = shortNotes.Where(x => score.StartTick <= x.Position.Tick && x.Position.Tick <= score.EndTick).ToList();
                    for(int lane = 0; lane < ScoreInfo.Lanes; ++lane)
                    {
                        List<Note> currentLaneNotes = currentScoreNotes.Where(x => x.Position.Lane == lane).ToList();
                        if (!currentLaneNotes.Any()) continue;

                    }
                }
            }
            return true;
        }
    }
}
