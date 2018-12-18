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
        private readonly float measureConstant = 4f;
        private ScoreBook scoreBook;
        private NoteBook noteBook;

        public ExportForm()
        {
            InitializeComponent();
            scoreBook = null;
            noteBook = null;
            saveFileDialog = new SaveFileDialog()
            {
                FileName = "NewScore.sus",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "Seaurchin譜面ファイル(*.sus)|*.sus",
                FilterIndex = 0,
                Title = "エクスポート",
                RestoreDirectory = true
            };
            exportButton.Click += ExportButton_Click;
            exportCancelButton.Click += (s, e) => Close();
            exportPathButton.Click += (s, e)  => 
            {
                if(saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    exportPathText.Text = saveFileDialog.FileName;
                }
            };
            pathClearButton.Click += (s, e) => exportPathText.Text = "";
            DifficultyBox.SelectedIndexChanged += (s, e) =>
            {
                if(DifficultyBox.SelectedIndex == 4)
                {
                    weText.Enabled = weStarUpDown.Enabled = true;
                    playLevelBox.Enabled = false;
                }
                else
                {
                    weText.Enabled = weStarUpDown.Enabled = false;
                    playLevelBox.Enabled = true;
                }
            };
            DifficultyBox.SelectedIndex = 0;
        }

        public void ShowDialog(ScoreBook scoreBook, NoteBook noteBook)
        {
            this.scoreBook = scoreBook;
            this.noteBook = noteBook;
            ShowDialog();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (exportPathText.Text == "")
            {
                MessageBox.Show("出力先を指定してください");
            }
            else
            {
                Export(exportPathText.Text);
                Close();
            }
        }

        private bool Export(in string path)
        {
            if (scoreBook == null || noteBook == null)
            {
                return false;
            }
            using (StreamWriter streamWriter = new StreamWriter(path, false))
            {
                streamWriter.WriteLine("This score was created by M4ple Editor.");

                /* TODO: この辺に基本情報を書き出す処理書く */
                
                streamWriter.WriteLine("\r\nMeasure's pulse");
                foreach(Score score in scoreBook)
                {
                    if(score == scoreBook.First() || (score.BarSize != scoreBook.Prev(score).BarSize))
                    {
                        streamWriter.WriteLine("#" + score.Index.ToString("D3") + "02: " + (measureConstant * score.BarSize));
                    }
                }
                streamWriter.WriteLine("\r\nBPM");
                WriteBPMNotes(streamWriter);
                if(noteBook.AttributeNotes.Where(x => x is HighSpeed).Any())
                {
                    streamWriter.WriteLine("\r\nHighSpeed");
                    streamWriter.Write("#TIL00: \"");
                    foreach (HighSpeed speed in noteBook.AttributeNotes.Where(x => x is HighSpeed).OrderBy(x => x.Position.Tick))
                    {
                        Score score = scoreBook.Find(x => x.StartTick <= speed.Position.Tick && speed.Position.Tick <= x.EndTick);
                        if (score == null)
                        {
                            System.Diagnostics.Debug.Assert(false, "ハイスピノーツが属するスコアが見つかりませんでした");
                            continue;
                        }
                        streamWriter.Write(score.Index + "'" + (measureConstant * (speed.Position.Tick - score.StartTick)) + ":" + speed.NoteValue + ", ");
                    }
                    streamWriter.WriteLine("\"");
                    streamWriter.WriteLine("#HIGHSPEED 00\r\n#MEASUREHS 00");
                }
                streamWriter.WriteLine("\r\nShortNote");
                foreach (Score score in scoreBook)
                {
                    WriteNotesByScore(noteBook.ShortNotes, score, 1, streamWriter, "");
                }
                streamWriter.WriteLine("\r\nHold");
                WriteLongNotes(noteBook.HoldNotes.ConvertAll(x => x as LongNote), 2, streamWriter);
                streamWriter.WriteLine("\r\nSlide");
                WriteLongNotes(noteBook.SlideNotes.ConvertAll(x => x as LongNote), 3, streamWriter);
                streamWriter.WriteLine("\r\nAirHold");
                WriteLongNotes(noteBook.AirHoldNotes.ConvertAll(x => x as LongNote), 4, streamWriter);
                streamWriter.WriteLine("\r\nAir");
                foreach (Score score in scoreBook)
                {
                    WriteNotesByScore(noteBook.AirNotes.ConvertAll(x => x as Note), score, 5, streamWriter, "");
                }
            }
            return true;
        }

        private void WriteBPMNotes(in StreamWriter streamWriter)
        {
            float defaultValue = -1;
            var bpmNotes = noteBook.AttributeNotes.Where(x => x is BPM);
            //BPMの値と識別番号を対応付ける
            char[] signArray = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            List<KeyValuePair<string, float>> idList = new List<KeyValuePair<string, float>>();
            for(int i = 0; i < signArray.Length; ++i)
            {
                for(int j = 0; j < signArray.Length; ++j)
                {
                    idList.Add(new KeyValuePair<string, float>(
                        signArray[i].ToString() + signArray[j].ToString(),
                        defaultValue));
                }
            }
            idList.RemoveAt(0);
            foreach(BPM bpm in bpmNotes)
            {
                if (!idList.TakeWhile(x => x.Value != defaultValue).Any() || 
                    idList.TakeWhile(x => x.Value != defaultValue).ToList().Find(x => x.Value == bpm.NoteValue).Equals(default(KeyValuePair<string, float>)))
                {
                    int index = idList.IndexOf(idList.Find(x => x.Value == defaultValue));
                    idList[index] = new KeyValuePair<string, float>(idList[index].Key, bpm.NoteValue);
                    streamWriter.WriteLine("#BPM" + idList[index].Key + ": " + idList[index].Value);
                }
            }
            //対応づけをもとにBPMを書き出し
            foreach(Score score in scoreBook)
            {
                var currentScoreNotes = bpmNotes.Where(x => score.StartTick <= x.Position.Tick && x.Position.Tick <= score.EndTick).ToList();
                if (!currentScoreNotes.Any()) continue;
                int lcm = 1;
                currentScoreNotes.ForEach(x =>
                {
                    int tick = x.Position.Tick - score.StartTick;
                    int gcd = MyUtil.Gcd(tick, score.TickSize);
                    lcm = MyUtil.Lcm(lcm, score.TickSize / gcd);
                });
                //
                streamWriter.Write("#" + score.Index.ToString("D3") + "02: ");
                for (int i = 0; i < lcm; ++i)
                {
                    var writeNote = currentScoreNotes.Find(x => x.Position.Tick - score.StartTick == i * score.TickSize / lcm);
                    if (writeNote != null)
                    {
                        string id = idList.Find(x => x.Value == writeNote.NoteValue).Key;
                        streamWriter.Write(id);
                    }
                    else
                    {
                        streamWriter.Write("00");
                    }
                }
                streamWriter.Write("\r\n");
            }
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
                streamWriter.Write("\r\n");
            }
        }

        private void WriteLongNotes(in List<LongNote> longNoteList, in int laneType, in StreamWriter streamWriter)
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
