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
        private readonly SaveFileDialog saveFileDialog;
        private ScoreBook scoreBook;
        private NoteBook noteBook;

        public ExportForm(ScoreBook scoreBook, NoteBook noteBook)
        {
            InitializeComponent();
            this.scoreBook = scoreBook;
            this.noteBook = noteBook;
            saveFileDialog = new SaveFileDialog()
            {
                FileName = "NewScore.sus",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "Seaurchin譜面ファイル(*.sus)|*.sus",
                FilterIndex = 0,
                Title = "エクスポート",
                RestoreDirectory = true
            };
            ExportButton.Click += ExportButton_Click;
            exportCancelButton.Click += (s, e) => Close();
            ExportPathButton.Click += (s, e)  => 
            {
                if(saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    exportPathText.Text = saveFileDialog.FileName;
                }
            };
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (exportPathText.Text == "")
            {
                MessageBox.Show("出力先を指定してください");
            }
            else
            {
                Export(exportPathText.Text, scoreBook, noteBook);
                Close();
            }
        }

        private bool Export(in string path, in ScoreBook scoreBook, in NoteBook noteBook)
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
                foreach (Score score in scoreBook)
                {
                    WriteNotesByScore(noteBook.ShortNotes, score, 1, streamWriter, "");
                }
                streamWriter.WriteLine("\nHold");
                WriteLongNotes(noteBook.HoldNotes.ConvertAll(x => x as LongNote), scoreBook, 2, streamWriter);
                streamWriter.WriteLine("\nSlide");
                WriteLongNotes(noteBook.SlideNotes.ConvertAll(x => x as LongNote), scoreBook, 3, streamWriter);
                streamWriter.WriteLine("\nAirHold");
                WriteLongNotes(noteBook.AirHoldNotes.ConvertAll(x => x as LongNote), scoreBook, 4, streamWriter);
                streamWriter.WriteLine("\nAir");
                foreach (Score score in scoreBook)
                {
                    WriteNotesByScore(noteBook.AirNotes.ConvertAll(x => x as Note), score, 5, streamWriter, "");
                }
            }
            return true;
        }

        /// <summary>
        /// notesのうちscoreに属するものに対してデータの書き出しを行います
        /// </summary>
        /// <param name="notes"></param>
        /// <param name="score"></param>
        /// <param name="laneType">レーン種別</param>
        /// <param name="streamWriter"></param>
        /// <param name="longLaneSign">ロングノーツの際に使うロングレーンの識別番号</param>
        private void WriteNotesByScore(in List<Note> notes, Score score, in int laneType, in StreamWriter streamWriter, in string longLaneSign)
        {
            List<Note> currentScoreNotes = notes.Where(x => score.StartTick <= x.Position.Tick && x.Position.Tick <= score.EndTick).ToList();
            for (int lane = 0; lane < ScoreInfo.Lanes; ++lane)
            {
                List<Note> currentLaneNotes = currentScoreNotes.Where(x => x.Position.Lane == lane).ToList();
                if (!currentLaneNotes.Any()) continue;
                int lcm = 1;
                currentLaneNotes.ForEach(x =>
                {
                    // 1 ≦ tick ≦ TickSize にする
                    int tick = x.Position.Tick - score.StartTick;
                    int gcd = MyUtil.Gcd(tick, score.TickSize);
                    lcm = MyUtil.Lcm(lcm, score.TickSize / gcd);
                });
                //
                streamWriter.Write("#" + score.Index.ToString("D3") + laneType + lane.ToString("x") + longLaneSign + ": ");
                for (int i = 0; i < lcm; ++i)
                {
                    Note writeNote = currentLaneNotes.Find(x => x.Position.Tick - score.StartTick == i * score.TickSize / lcm);
                    if (writeNote != null)
                    {
                        streamWriter.Write(writeNote.NoteID);
                        if (writeNote.Size == 16)
                        {
                            streamWriter.Write("g");
                        }
                        else
                        {
                            streamWriter.Write(writeNote.Size.ToString("x"));
                        }
                    }
                    else
                    {
                        streamWriter.Write("00");
                    }
                }
                streamWriter.WriteLine("");
            }
        }

        private void WriteLongNotes(in List<LongNote> longNoteList, ScoreBook scoreBook, in int laneType, in StreamWriter streamWriter)
        {
            LongLaneSignProvider signProvider = new LongLaneSignProvider();
            foreach (LongNote longNote in longNoteList)
            {
                string sign = signProvider.GetAvailableSign(longNote.StartTick, longNote.EndTick);
                foreach(Score score in scoreBook.Where(x => x.EndTick >= longNote.StartTick && x.StartTick <= longNote.EndTick))
                {
                    WriteNotesByScore(longNote, score, laneType, streamWriter, sign);
                }
            }
        }
    }
}
