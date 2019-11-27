using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Scores;
using NE4S.Notes;
using System.IO;
using NE4S.Notes.Abstract;

namespace NE4S.Component
{
    public partial class SusLoader
    {
        private readonly OpenFileDialog susSelectDialog;

        public SusLoader()
        {
            susSelectDialog = new OpenFileDialog()
            {
                FileName = "",
                InitialDirectory = Status.ExportDialogDirectory,
                Filter = "Seaurchin譜面ファイル(*.sus)|*.sus",
                FilterIndex = 0,
                Title = "開く",
                RestoreDirectory = true
            };
        }

        public Model ShowDialog(List<string> message, ref string susFileName)
        {
            Model model = null;
            if (Directory.Exists(Status.ExportDialogDirectory))
            {
                susSelectDialog.FileName = "";
                susSelectDialog.InitialDirectory = Status.ExportDialogDirectory;
            }
            if (susSelectDialog.ShowDialog() == DialogResult.OK)
            {
                susFileName = Path.GetFileName(susSelectDialog.FileName);
                model = LoadSusData(message);
                if (model == null)
                {
                    MessageBox.Show("ファイルを開けませんでした。\nファイルが破損しているか、対応していない可能性があります。", "読み込みエラー");
                }
            }
            return model;
        }

        private Model LoadSusData(List<string> message)
        {
            string path = susSelectDialog.FileName;
            Status.ExportDialogDirectory = Directory.GetParent(path).ToString();
            return AnalyzeSusData(path, message);
        }
        
        public Model AnalyzeSusData(string path, List<string> message)
        {
            Model model = new Model();
            List<RawNote> rawNotes = new List<RawNote>();
            Dictionary<int, float> bpmDefs = new Dictionary<int, float>(); /* {BPM定義番号, BPM} */
            Dictionary<RawNote, int> bpmApplys = new Dictionary<RawNote, int>(); /* {定義位置, BPM定義番号} 解析の都合上楽だからって定義位置RawNoteで表すのはいかがなものかと */
            Dictionary<int, double> tpbApplys = new Dictionary<int, double>(); /* (ticks per beat) {小節番号, 拍数} */
            Dictionary<int, string> notesAttributeDefs = new Dictionary<int, string>(); /* {定義番号, 定義内容} */
            string line;
            int lineCount = 0;
            int currentTimeLineIndex = -1;
            int currentAttributeIndex = -1;
            int measureOffset = 0;
            int lastMeasureCount = 0;
            int baseMeasureCount = -1;
            int measureLineTimeLineIndex = -1;
            string dirPath = Path.GetDirectoryName((new Uri(path)).LocalPath);

            Regex rgxNotes = new Regex(@"^#(\d{3})(\w)(\w)(\w?)\:(\s*\w\w)+\s*$");
            Regex rgxDef = new Regex(@"^#(BPM|TIL|ATR)(\w\w)\:?\s*([^\n]+)$");
            Regex rgxApply = new Regex(@"^#(\d{3})0(2)\:\s*([^\n]+)$");
            Regex rgxCommand = new Regex(@"^#(\w+)(?:\s+([^\n]+$))?");
            Regex rgxWE = new Regex(@"(\d+):([^\n]+)$"); ;
            Regex rgxRequest = new Regex(@"^([^\s]+)(?:\s+([^\s]+))*$");

            #region 譜面を行単位で読み込み
            using (StreamReader reader = File.OpenText(path))
            {
                model.MusicInfo.ExportPath = (new Uri(path)).LocalPath;
                model.MusicInfo.HasExported = false;

                while ((line = reader.ReadLine()) != null)
                {
                    ++lineCount;

                    if (line.Length == 0 || line[0] != '#') continue;

                    #region 拍数変更データ抽出
                    Match matchApply = rgxApply.Match(line);
                    if (matchApply.Success)
                    {
                        int measure = MyUtil.ToInt(matchApply.Groups[1].ToString()) + measureOffset;
                        string type = matchApply.Groups[2].ToString();
                        string arg = matchApply.Groups[3].ToString();

                        if (type.Length == 0)
                        {
                            continue;
                        }

                        if (type[0] == '2')
                        {
                            System.Console.WriteLine("Line {0} : 拍数変更 [{1}][{2}]", lineCount, measure, arg);
                            tpbApplys.Add(measure, Convert.ToDouble(MyUtil.GetRawString(arg)));
                        }
                        else
                        {
                            message.Add("" + lineCount + "行 : SUS有効行ですが解析できませんでした。");
                            System.Console.WriteLine("Line {0} : SUS有効行ですが解析できませんでした。", lineCount);
                        }
                        continue;
                    }
                    #endregion

                    #region ノーツデータ抽出
                    Match matchNotes = rgxNotes.Match(line);
                    if (matchNotes.Success)
                    {
                        int measure = MyUtil.ToInt(matchNotes.Groups[1].ToString()) + measureOffset;
                        int noteGenre = MyUtil.ChangeZto35(matchNotes.Groups[2].ToString()[0]);
                        if (noteGenre < 0 || 5 < noteGenre)
                        {
                            message.Add("" + lineCount + "行 : SUS有効行ですが解析できませんでした。");
                            System.Console.WriteLine("Line {0} : SUS有効行ですが解析できませんでした。", lineCount);
                            continue;
                        }
                        int noteLane = MyUtil.ChangeZto35(matchNotes.Groups[3].ToString()[0]);
                        int noteIdentifier = -1;
                        if (noteGenre == 0)
                        {
                            if (noteLane == 8)
                            {
                                /* 小節定義 */
                                /* 下の処理で捌くよ */
                            }
                            else
                            {
                                message.Add("" + lineCount + "行 : SUS有効行ですが解析できませんでした。");
                                System.Console.WriteLine("Line {0} : SUS有効行ですが解析できませんでした。", lineCount);
                                continue;
                            }
                        }
                        else
                        {
                            if (noteLane < 0 || 15 < noteLane)
                            {
                                message.Add("" + lineCount + "行 : SUS有効行ですが解析できませんでした。");
                                System.Console.WriteLine("Line {0} : SUS有効行ですが解析できませんでした。", lineCount);
                                continue;
                            }

                            if (matchNotes.Groups[4].ToString().Length == 1)
                            {
                                noteIdentifier = MyUtil.ChangeZto35(matchNotes.Groups[4].ToString()[0]);
                            }
                        }
                        CaptureCollection notes = matchNotes.Groups[5].Captures;
                        Console.WriteLine("line {0}: Found '{1}' at position {2}", lineCount, matchNotes.Value, matchNotes.Index);
                        int divCount = notes.Count;

                        for (int j = 0; j < divCount; j++)
                        {
                            Capture c = notes[j];

                            /* 小節定義 */
                            if (noteGenre == 0 && noteLane == 8)
                            {
                                RawNote bpm = new RawNote(RawNote.RawNoteType.BPM, 0, 0, measure, j, divCount);
                                bpmApplys.Add(bpm, MyUtil.ToIntAs36(c.ToString().Trim()));

                                if (lastMeasureCount < measure) lastMeasureCount = measure;

                                continue;
                            }

                            int size = MyUtil.ChangeZto35(c.ToString().Trim()[1]);
                            if (size < 1 || 16 < size) continue;

                            int noteType = MyUtil.ChangeZto35(c.ToString().Trim()[0]);
                            if (noteType < 0 || 9 < noteType) continue;
                            RawNote.RawNoteType rawNoteType = RawNote.RawNoteType.Undefined;
                            switch (noteGenre)
                            {
                                case 1: // ショートノーツ
                                    switch (noteType)
                                    {
                                        case 1: rawNoteType = RawNote.RawNoteType.Tap; break;
                                        case 2: rawNoteType = RawNote.RawNoteType.ExTap; break;
                                        case 3: rawNoteType = RawNote.RawNoteType.Flick; break;
                                        case 4: rawNoteType = RawNote.RawNoteType.HellTap; break;
                                        case 5: rawNoteType = RawNote.RawNoteType.AwesomeExTap; break;
                                        case 6: rawNoteType = RawNote.RawNoteType.ExTapDown; break;
                                        default:
                                            message.Add("" + lineCount + "行 " + (j + 1) + "番目のノーツ定義 : 不明なノーツ定義\"" + c.ToString() + "\"です。ノーツ定義は無視されます。");
                                            continue;
                                    }
                                    break;
                                case 2:
                                    switch (noteType)
                                    {
                                        case 1: rawNoteType = RawNote.RawNoteType.HoldBegin; break;
                                        case 2: rawNoteType = RawNote.RawNoteType.HoldEnd; break;
                                        case 3:
                                            /* Hold中間点は扱えない */
                                            message.Add("" + lineCount + "行 " + (j + 1) + "番目のノーツ定義 : Hold中間点はM4pleEditorでは扱えません。ノーツ定義は無視されます。");
                                            continue; ;
                                        default:
                                            message.Add("" + lineCount + "行 " + (j + 1) + "番目のノーツ定義 : 不明なノーツ定義\"" + c.ToString() + "\"です。ノーツ定義は無視されます。");
                                            continue;
                                    }
                                    break;
                                case 3:
                                    switch (noteType)
                                    {
                                        case 1: rawNoteType = RawNote.RawNoteType.SlideBegin; break;
                                        case 2: rawNoteType = RawNote.RawNoteType.SlideEnd; break;
                                        case 3: rawNoteType = RawNote.RawNoteType.SlideTap; break;
                                        case 4: rawNoteType = RawNote.RawNoteType.SlideCurve; break;
                                        case 5: rawNoteType = RawNote.RawNoteType.SlideRelay; break;
                                        default:
                                            message.Add("" + lineCount + "行 " + (j + 1) + "番目のノーツ定義 : 不明なノーツ定義\"" + c.ToString() + "\"です。ノーツ定義は無視されます。");
                                            continue;
                                    }
                                    break;
                                case 4:
                                    switch (noteType)
                                    {
                                        case 1: rawNoteType = RawNote.RawNoteType.AirHoldBegin; break;
                                        case 2: rawNoteType = RawNote.RawNoteType.AirHoldEnd; break;
                                        case 3: rawNoteType = RawNote.RawNoteType.AirAction; break;
                                        case 4:
                                            /* Air変曲点は扱えない */
                                            message.Add("" + lineCount + "行 " + (j + 1) + "番目のノーツ定義 : Air変曲点はM4pleEditorでは扱えません。ノーツ定義は無視されます。");
                                            continue;
                                        case 5:
                                            /* Air不可視中継点は扱えない */
                                            message.Add("" + lineCount + "行 " + (j + 1) + "番目のノーツ定義 : Air不可視中継点はM4pleEditorでは扱えません。ノーツ定義は無視されます。");
                                            continue;
                                        default:
                                            message.Add("" + lineCount + "行 " + (j + 1) + "番目のノーツ定義 : 不明なノーツ定義\"" + c.ToString() + "\"です。ノーツ定義は無視されます。");
                                            continue;
                                    }
                                    break;
                                case 5:
                                    switch (noteType)
                                    {
                                        case 1: rawNoteType = RawNote.RawNoteType.AirUpC; break;
                                        case 2: rawNoteType = RawNote.RawNoteType.AirDownC; break;
                                        case 3: rawNoteType = RawNote.RawNoteType.AirUpL; break;
                                        case 4: rawNoteType = RawNote.RawNoteType.AirUpR; break;
                                        case 5: rawNoteType = RawNote.RawNoteType.AirDownR; break;
                                        case 6: rawNoteType = RawNote.RawNoteType.AirDownL; break;
                                        case 7:
                                            /* 地面付き上Airは扱えない */
                                            message.Add("" + lineCount + "行 " + (j + 1) + "番目のノーツ定義 : 地面付き上AirはM4pleEditorでは扱えません。ノーツ定義は無視されます。");
                                            continue;
                                        case 8:
                                            /* 地面付き左上Air(↖)は扱えない */
                                            message.Add("" + lineCount + "行 " + (j + 1) + "番目のノーツ定義 : 地面付き左上Air(↖)はM4pleEditorでは扱えません。ノーツ定義は無視されます。");
                                            continue;
                                        case 9:
                                            /* 地面付き右上Air(↗)は扱えない */
                                            message.Add("" + lineCount + "行 " + (j + 1) + "番目のノーツ定義 : 地面付き右上Air(↗)はM4pleEditorでは扱えません。ノーツ定義は無視されます。");
                                            continue;
                                        default:
                                            message.Add("" + lineCount + "行 " + (j + 1) + "番目のノーツ定義 : 不明なノーツ定義\"" + c.ToString() + "\"です。ノーツ定義は無視されます。");
                                            continue;
                                    }
                                    break;
                            }

                            RawNote n = new RawNote(rawNoteType, noteLane, size, measure, j, divCount, noteIdentifier)
                            {
                                TimeLineIndex = currentTimeLineIndex
                            };
                            rawNotes.Add(n);

                            if (lastMeasureCount < measure) lastMeasureCount = measure;

                            System.Console.WriteLine("Note[" + measure + "][" + noteGenre + "][" + noteLane + "][" + noteIdentifier + "][" + n.Tick + "] : " + c);
                        }

                        continue;
                    }
                    #endregion

                    #region BPM,TIL,ATR抽出
                    Match matchDef = rgxDef.Match(line);
                    if (matchDef.Success)
                    {
                        string type = matchDef.Groups[1].ToString().ToUpper();
                        int index = MyUtil.ToIntAs36(matchDef.Groups[2].ToString());
                        string arg = matchDef.Groups[3].ToString();

                        if (type == "BPM")
                        {
                            System.Console.WriteLine("Line {0} : BPM = " + arg, lineCount);
                            bpmDefs.Add(index, (float)Convert.ToDouble(MyUtil.GetRawString(arg)));
                        }
                        else if (type == "TIL")
                        {
                            if (!model.TimeLineBook.Add(index, MyUtil.GetRawString(arg), model.MusicInfo.TicksPerBeat))
                            {
                                message.Add("" + lineCount + "行 : ハイスピードタイムラインの解析に失敗しました。");
                            }
                        }
                        else if (type == "ATR")
                        {
                            /* 真面目に解釈する気ないから文字列で保管しておこうね */
                            notesAttributeDefs.Add(index, MyUtil.GetRawString(arg));
                        }
                        continue;
                    }
                    #endregion

                    #region ヘッダ、コマンドなど抽出
                    Match matchCommand = rgxCommand.Match(line);
                    if (matchCommand.Success)
                    {
                        string command = matchCommand.Groups[1].ToString().ToUpper();
                        string commandArg = matchCommand.Groups[2].ToString();
                        string rawCommandArg = MyUtil.GetRawString(commandArg);

                        #region ヘッダ（楽曲情報など）抽出
                        if (command == "TITLE")
                        {
                            model.MusicInfo.Title = rawCommandArg;
                        }
                        else if (command == "SUBTITLE")
                        {
                            /* これSeaurchinだと一応エラー無く読まれるのよね 使われてないけど */
                            model.MusicInfo.SubTitle = rawCommandArg;
                        }
                        else if (command == "ARTIST")
                        {
                            model.MusicInfo.Artist = rawCommandArg;
                        }
                        else if (command == "GENRE")
                        {
                            /* これSeaurchinだと一応エラー無く読まれるのよね 使われてないけど */
                            model.MusicInfo.Genre = rawCommandArg;
                        }
                        else if (command == "DESIGNER" || command == "SUBARTIST")
                        {
                            model.MusicInfo.Designer = rawCommandArg;
                        }
                        else if (command == "PLAYLEVEL")
                        {
                            /* WE指定ではPLAYLEVEL無効だからね */
                            if (model.MusicInfo.Difficulty != 4)
                            {
                                model.MusicInfo.PlayLevel = rawCommandArg;
                            }
                        }
                        else if (command == "DIFFICULTY")
                        {
                            Match matchWE = rgxWE.Match(rawCommandArg);

                            if (commandArg == "0")
                            {
                                model.MusicInfo.Difficulty = 0;
                                model.MusicInfo.WEKanji = "";
                                model.MusicInfo.WEStars = 1;
                            }
                            else if (commandArg == "1")
                            {
                                model.MusicInfo.Difficulty = 1;
                                model.MusicInfo.WEKanji = "";
                                model.MusicInfo.WEStars = 1;
                            }
                            else if (commandArg == "2")
                            {
                                model.MusicInfo.Difficulty = 2;
                                model.MusicInfo.WEKanji = "";
                                model.MusicInfo.WEStars = 1;
                            }
                            else if (commandArg == "3")
                            {
                                model.MusicInfo.Difficulty = 3;
                                model.MusicInfo.WEKanji = "";
                                model.MusicInfo.WEStars = 1;
                            }
                            else if (matchWE.Success)
                            {
                                model.MusicInfo.PlayLevel = ""; /* WE指定ではPLAYLEVEL無効だからね */
                                model.MusicInfo.Difficulty = 4;
                                model.MusicInfo.WEKanji = matchWE.Groups[2].ToString();
                                model.MusicInfo.WEStars = MyUtil.ToInt(matchWE.Groups[1].ToString());
                            }
                        }
                        else if (command == "SONGID")
                        {
                            model.MusicInfo.SongId = rawCommandArg;
                        }
                        else if (command == "WAVE")
                        {
                            //model.MusicInfo.MusicFileName = (new Uri(new Uri(dirPath), rawCommandArg)).LocalPath;
                            model.MusicInfo.MusicFileName = rawCommandArg;
                        }
                        else if (command == "WAVEOFFSET")
                        {
                            model.MusicInfo.MusicOffset = (decimal)Convert.ToDouble(rawCommandArg);
                        }
                        else if (command == "MOVIE")
                        {
                            //model.MusicInfo.MovieFileName = (new Uri(new Uri(dirPath), rawCommandArg)).LocalPath;
                            model.MusicInfo.MovieFileName = rawCommandArg;
                        }
                        else if (command == "MOVIEOFFSET")
                        {
                            model.MusicInfo.MovieOffset = (decimal)Convert.ToDouble(rawCommandArg);
                        }
                        else if (command == "JACKET")
                        {
                            //model.MusicInfo.JacketFileName = (new Uri(new Uri(dirPath), rawCommandArg)).LocalPath;
                            model.MusicInfo.JacketFileName = rawCommandArg;
                        }
                        else if (command == "BACKGROUND")
                        {
                            //model.MusicInfo.BackgroundFileName = (new Uri(new Uri(dirPath), rawCommandArg)).LocalPath;
                            model.MusicInfo.BackgroundFileName = rawCommandArg;
                        }
                        #endregion

                        #region コマンド抽出
                        else if (command == "REQUEST")
                        {
                            Match matchRequest = rgxRequest.Match(rawCommandArg);
                            if (matchRequest.Success && matchRequest.Groups.Count > 2)
                            {
                                string requestName = matchRequest.Groups[1].ToString();
                                string requestParam = matchRequest.Groups[2].ToString();

                                if (requestName == "ticks_per_beat")
                                {
                                    model.MusicInfo.TicksPerBeat = MyUtil.ToInt(requestParam);
                                }
                                else if (requestName == "metronome")
                                {
                                    /* Seaurchin完全準拠ガバガバbool値変換 */
                                    bool f = requestParam == "1";
                                    f = f || requestParam == "true";
                                    f = f || requestParam == "y";
                                    f = f || requestParam == "yes";
                                    f = f || requestParam == "enable";
                                    f = f || requestParam == "enabled";

                                    model.MusicInfo.Metronome = (f) ? 0 : 1;
                                }
                                else if (requestName == "enable_priority")
                                {
                                    /* Seaurchin完全準拠ガバガバbool値変換 */
                                    bool f = requestParam == "1";
                                    f = f || requestParam == "true";
                                    f = f || requestParam == "y";
                                    f = f || requestParam == "yes";
                                    f = f || requestParam == "enable";
                                    f = f || requestParam == "enabled";

                                    model.MusicInfo.EnablePriority = f;
                                }
                                else if (requestName == "segments_per_second")
                                {
                                    model.MusicInfo.SlideCurveSegment = MyUtil.ToInt(requestParam);
                                }
                                else
                                {
                                    message.Add("" + lineCount + "行 : REQUESTコマンドのリクエスト名が不明な値です。コマンドは無視されます。");
                                }
                            }
                            else
                            {
                                message.Add("" + lineCount + "行 : REQUESTコマンドの定義が不正な形式です。コマンドは無視されます。");
                            }
                        }
                        else if (command == "BASEBPM")
                        {
                            message.Add("" + lineCount + "行 : BASEBPM指定はM4pleEditorでは扱えません。コマンドは無視されます。");
                        }
                        else if (command == "HISPEED")
                        {
                            if (commandArg.Length == 0)
                            {
                                message.Add("" + lineCount + "行 : HISPEEDコマンドの定義番号が不足しています。コマンドは無視されます。");
                                break;
                            }
                            int index = MyUtil.ToIntAs36(commandArg);
                            currentTimeLineIndex = index;
                        }
                        else if (command == "NOSPEED")
                        {
                            currentTimeLineIndex = -1;
                        }
                        else if (command == "ATTRIBUTE")
                        {
                            if (commandArg.Length == 0)
                            {
                                message.Add("" + lineCount + "行 : ATTRIBUTEコマンドの定義番号が不足しています。コマンドは無視されます。");
                                break;
                            }
                            int index = MyUtil.ToIntAs36(commandArg);
                            currentAttributeIndex = index;
                        }
                        else if (command == "NOATTRIBUTE")
                        {
                            currentAttributeIndex = -1;
                        }
                        else if (command == "MEASUREHS")
                        {
                            if (commandArg.Length == 0)
                            {
                                message.Add("" + lineCount + "行 : MEASUREHSコマンドの定義番号が不足しています。コマンドは無視されます。");
                                break;
                            }
                            measureLineTimeLineIndex = MyUtil.ToIntAs36(commandArg);
                        }
                        else if (command == "MEASUREBS")
                        {
                            measureOffset = MyUtil.ToInt(commandArg);

                            if (baseMeasureCount == -1)
                            {
                                baseMeasureCount = measureOffset;
                            }
                            else if (measureOffset < baseMeasureCount)
                            {
                                baseMeasureCount = measureOffset;
                            }
                        }
                        #endregion

                        // どれにも当てはまらないデータの場合
                        else
                        {
                            message.Add("" + lineCount + "行 : SUSコマンドが無効です。");
                            System.Console.WriteLine("Line {0} : SUSコマンドが無効です。", lineCount);
                        }
                        continue;
                    }
                    #endregion

                    // どれにも当てはまらないデータの場合はここに到達する
                    message.Add("" + lineCount + "行 : SUS有効行ですが解析できませんでした。");
                    System.Console.WriteLine("Line {0} : SUS有効行ですが解析できませんでした。", lineCount);
                }
            }
            #endregion

            // 読み込んだ拍数定義データを時間順にソート
            tpbApplys = tpbApplys.OrderBy((x) => x.Key).ToDictionary((x) => x.Key, (x) => x.Value);
            #region 小節を追加
            {
                int currentBeatNum = 4;
                int currentBeatDen = 4;
                for (int i = 0; i < lastMeasureCount + 2; ++i)
                {
                    if (tpbApplys.ContainsKey(i))
                    {
                        double tpb = tpbApplys[i];

                        // tpb == p / 2^n <=> 2^n * tpb == p <=> beatDen * tpb == p
                        int beatDen = 1;
                        while (beatDen < 64 && Math.Abs((beatDen * tpb) - (int)(beatDen * tpb)) > 1e-4)
                        {
                            beatDen *= 2;
                        }
                        currentBeatNum = (int)(beatDen * tpb);
                        currentBeatDen = beatDen * 4;
                    }
                    System.Diagnostics.Debug.Assert(currentBeatDen != 0, "currentBeatDenomIsZero");
                    System.Diagnostics.Debug.Assert(currentBeatNum != 0, "currentBeatNumerIsZero");
                    if (currentBeatDen == 0 || currentBeatNum == 0)
                    {
                        message.Add("" + lineCount + "行 : 無効な小節です。");
                        continue;
                    }
                    model.ScoreBook.Add(currentBeatNum, currentBeatDen);
                }
            }
            #endregion

            // ノーツのポジションを更新 これよりrawNotes[i].Position.Tickが使える
            for (int i = 0; i < rawNotes.Count; ++i)
            {
                rawNotes[i].RelocateOnly(rawNotes[i].GetFinallyPosition(model.ScoreBook.At(rawNotes[i].Measure)));
            }

            // ノーツをソート
            // 判定時刻が早いものを優先的に前へ、同一判定時刻の場合はショートノーツ優先
            for (int i = 0; i < rawNotes.Count; ++i)
            {
                for (int j = i + 1; j < rawNotes.Count; ++j)
                {
                    bool f = (rawNotes[i].Position.Tick > rawNotes[j].Position.Tick)
                        || (rawNotes[i].Position.Tick == rawNotes[j].Position.Tick && rawNotes[i].NoteType > rawNotes[j].NoteType);

                    if (f)
                    {
                        RawNote tmp = rawNotes[i];
                        rawNotes[i] = rawNotes[j];
                        rawNotes[j] = tmp;
                    }
                }
            }

            // BPM定義のポジションを更新
            foreach (KeyValuePair<RawNote, int> bpmApply in bpmApplys)
            {
                bpmApply.Key.RelocateOnly(bpmApply.Key.GetFinallyPosition(model.ScoreBook.At(bpmApply.Key.Measure)));
            }
            bpmApplys = bpmApplys.GroupBy((x) => x.Key.Position.Tick).Select((x) => x.First()).OrderBy((x) => x.Key.Position.Tick).ToDictionary((x) => x.Key, (x) => x.Value);

            #region ノーツを譜面に追加
            for (int i = 0; i < rawNotes.Count; ++i)
            {
                switch (rawNotes[i].NoteType)
                {
                    case RawNote.RawNoteType.Tap:
                        model.NoteBook.Put(new Tap(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1));
                        break;
                    case RawNote.RawNoteType.ExTap:
                        model.NoteBook.Put(new ExTap(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1));
                        break;
                    case RawNote.RawNoteType.Flick:
                        model.NoteBook.Put(new Flick(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1));
                        break;
                    case RawNote.RawNoteType.HellTap:
                        model.NoteBook.Put(new HellTap(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1));
                        break;
                    case RawNote.RawNoteType.AwesomeExTap:
                        model.NoteBook.Put(new AwesomeExTap(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1));
                        break;
                    case RawNote.RawNoteType.ExTapDown:
                        model.NoteBook.Put(new ExTapDown(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1));
                        break;

                    case RawNote.RawNoteType.HoldBegin:
                        {
                            Hold hold = new Hold(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1);
                            int j;
                            for (j = i + 1; j < rawNotes.Count; ++j)
                            {
                                if (rawNotes[j].Position.Tick < rawNotes[i].Position.Tick) continue;
                                if (rawNotes[j].NoteSize != rawNotes[i].NoteSize) continue;
                                if (rawNotes[j].Position.Lane != rawNotes[i].Position.Lane) continue;
                                if (rawNotes[j].NoteType != RawNote.RawNoteType.HoldEnd) continue;
                                if (rawNotes[j].Identifier != rawNotes[i].Identifier) continue;

                                hold[1].Relocate(rawNotes[j].Position);
                                break;
                            }
                            if (j != rawNotes.Count)
                            {
                                model.NoteBook.Put(hold);
                            }
                            break;
                        }
                    case RawNote.RawNoteType.HoldEnd: continue;

                    case RawNote.RawNoteType.SlideBegin:
                        {
                            Slide slide = new Slide(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1);
                            int j;
                            for (j = i + 1; j < rawNotes.Count; ++j)
                            {
                                if (rawNotes[j].Position.Tick < rawNotes[i].Position.Tick) continue;
                                if (rawNotes[j].NoteType < RawNote.RawNoteType.SlideTap || RawNote.RawNoteType.SlideEnd < rawNotes[j].NoteType) continue;
                                if (rawNotes[j].Identifier != rawNotes[i].Identifier) continue;

                                Position stepPos = rawNotes[j].Position;
                                if (rawNotes[j].NoteType == RawNote.RawNoteType.SlideTap)
                                {
                                    slide[1].Relocate(stepPos.Next());
                                    slide.Add(new SlideTap(rawNotes[j].NoteSize, stepPos, new PointF(), -1));
                                    continue;
                                }
                                else if (rawNotes[j].NoteType == RawNote.RawNoteType.SlideRelay)
                                {
                                    slide[1].Relocate(stepPos.Next());
                                    slide.Add(new SlideRelay(rawNotes[j].NoteSize, stepPos, new PointF(), -1));
                                    continue;
                                }
                                else if (rawNotes[j].NoteType == RawNote.RawNoteType.SlideCurve)
                                {
                                    slide[1].Relocate(stepPos.Next());
                                    slide.Add(new SlideCurve(rawNotes[j].NoteSize, stepPos, new PointF(), -1));
                                    continue;
                                }
                                else if (rawNotes[j].NoteType == RawNote.RawNoteType.SlideEnd)
                                {
                                    slide[1].ReSize(rawNotes[j].NoteSize);
                                    slide[1].Relocate(stepPos);
                                    break;
                                }
                            }
                            if (j != rawNotes.Count)
                            {
                                model.NoteBook.Put(slide);
                            }
                            break;
                        }
                    case RawNote.RawNoteType.SlideTap:
                    case RawNote.RawNoteType.SlideRelay:
                    case RawNote.RawNoteType.SlideCurve:
                    case RawNote.RawNoteType.SlideEnd: continue;

                    case RawNote.RawNoteType.AirUpC:
                    case RawNote.RawNoteType.AirUpL:
                    case RawNote.RawNoteType.AirUpR:
                    case RawNote.RawNoteType.AirDownC:
                    case RawNote.RawNoteType.AirDownL:
                    case RawNote.RawNoteType.AirDownR:
                        {
                            Air airNote = null;
                            switch (rawNotes[i].NoteType)
                            {
                                case RawNote.RawNoteType.AirUpC: airNote = new AirUpC(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1); break;
                                case RawNote.RawNoteType.AirUpL: airNote = new AirUpL(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1); break;
                                case RawNote.RawNoteType.AirUpR: airNote = new AirUpR(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1); break;
                                case RawNote.RawNoteType.AirDownC: airNote = new AirDownC(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1); break;
                                case RawNote.RawNoteType.AirDownL: airNote = new AirDownL(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1); break;
                                case RawNote.RawNoteType.AirDownR: airNote = new AirDownR(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1); break;
                            }

                            int j;
                            for (j = model.NoteBook.ShortNotes.ToList().Count - 1; j >= 0; --j)
                            {
                                if (model.NoteBook.ShortNotes.ElementAt(j).NoteSize == rawNotes[i].NoteSize && 
                                    model.NoteBook.ShortNotes.ElementAt(j).Position.Equals(rawNotes[i].Position))
                                {
                                    if (model.NoteBook.ShortNotes.ElementAt(j) is AirableNote && 
                                        !((AirableNote)model.NoteBook.ShortNotes.ElementAt(j)).IsAirAttached)
                                    {
                                        var airable = model.NoteBook.ShortNotes.ElementAt(j) as AirableNote;
                                        model.NoteBook.AttachAirToAirableNote(airable, airNote);
                                        break;
                                    }
                                }
                            }
                            if (j >= 0) break; /* Attach完了 */

                            for (j = model.NoteBook.HoldNotes.Count - 1; j >= 0; --j)
                            {
                                if (model.NoteBook.HoldNotes.ElementAt(j)[1].Size == rawNotes[i].NoteSize && 
                                    model.NoteBook.HoldNotes.ElementAt(j)[1].Position.Equals(rawNotes[i].Position)) // ノーツの追加の仕方的に、1番目にHoldEndあるよね
                                {
                                    if (model.NoteBook.HoldNotes.ElementAt(j)[1] is AirableNote &&
                                        !((AirableNote)model.NoteBook.HoldNotes.ElementAt(j)[1]).IsAirAttached)
                                    {
                                        var airable = model.NoteBook.HoldNotes.ElementAt(j)[1] as AirableNote;
                                        model.NoteBook.AttachAirToAirableNote(airable, airNote);
                                        break;
                                    }
                                }
                            }
                            if (j >= 0) break; /* Attach完了 */

                            for (j = model.NoteBook.SlideNotes.Count - 1; j >= 0; --j)
                            {
                                if (model.NoteBook.SlideNotes.ElementAt(j)[1].Size == rawNotes[i].NoteSize &&
                                    model.NoteBook.SlideNotes.ElementAt(j)[1].Position.Equals(rawNotes[i].Position)) // ノーツの追加の仕方的に、1番目にSlideEndあるよね
                                {
                                    if (model.NoteBook.SlideNotes.ElementAt(j)[1] is AirableNote &&
                                        !((AirableNote)model.NoteBook.SlideNotes.ElementAt(j)[1]).IsAirAttached)
                                    {
                                        var airable = model.NoteBook.SlideNotes.ElementAt(j)[1] as AirableNote;
                                        model.NoteBook.AttachAirToAirableNote(airable, airNote);
                                        break;
                                    }
                                }
                            }
                            if (j >= 0) break; /* Attach完了 */

                            // 設置のためのAirableNoteがないときは新しくTap作ってそれにくっつける
                            {
                                Tap t = new Tap(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1);
                                model.NoteBook.Put(t);
                                model.NoteBook.AttachAirToAirableNote(t, airNote);
                            }

                            break;
                        }

                    case RawNote.RawNoteType.AirHoldBegin:
                        {
                            AirHold ah = new AirHold(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1);
                            bool broken = false; /* 表現できない場合はぶっ壊れAirHold判定 */
                            int j;
                            for (j = i + 1; j < rawNotes.Count; ++j)
                            {
                                if (rawNotes[j].Measure < rawNotes[i].Measure) continue;
                                if (rawNotes[j].NoteType != RawNote.RawNoteType.AirAction && rawNotes[j].NoteType != RawNote.RawNoteType.AirHoldEnd) continue;
                                if (rawNotes[j].Identifier != rawNotes[i].Identifier) continue;

                                if (rawNotes[j].NoteSize != rawNotes[i].NoteSize)
                                {
                                    broken = true; /* Sus的にはサイズ変えられる */
                                    message.Add("M4pleEditorではAirHoldの始点-中継点-終点はノーツ幅が等しい必要があります。表現できないAir-Holdは削除されます。");
                                }
                                if (rawNotes[j].Position.Lane != rawNotes[i].Position.Lane)
                                {
                                    broken = true; /* Sus的には曲げられる */
                                    message.Add("M4pleEditorではAirHoldの始点-中継点-終点は統一レーン上にある必要があります。表現できないAir-Holdは削除されます。");
                                }
                                Position stepPos = rawNotes[j].Position;
                                if (rawNotes[j].NoteType == RawNote.RawNoteType.AirAction)
                                {
                                    ah[1].Relocate(stepPos.Next());
                                    ah.Add(new AirAction(rawNotes[j].NoteSize, stepPos, new PointF(), -1));
                                    continue;
                                }
                                else if (rawNotes[j].NoteType == RawNote.RawNoteType.AirHoldEnd)
                                {
                                    ah[1].Relocate(stepPos);
                                    break;
                                }
                            }
                            if (j != rawNotes.Count && !broken)
                            {
                                for (j = model.NoteBook.ShortNotes.Count - 1; j >= 0; --j)
                                {
                                    if (model.NoteBook.ShortNotes.ElementAt(j).NoteSize == rawNotes[i].NoteSize && 
                                        model.NoteBook.ShortNotes.ElementAt(j).Position.Equals(rawNotes[i].Position))
                                    {
                                        if (model.NoteBook.ShortNotes.ElementAt(j) is AirableNote &&
                                            !((AirableNote)model.NoteBook.ShortNotes.ElementAt(j)).IsAirHoldAttached)
                                        {
                                            var airable = model.NoteBook.ShortNotes.ElementAt(j) as AirableNote;
                                            model.NoteBook.AttachAirHoldToAirableNote(airable, ah, null);
                                            break;
                                        }
                                    }
                                }
                                if (j >= 0) break; /* Attach完了 */

                                for (j = model.NoteBook.HoldNotes.Count - 1; j >= 0; --j)
                                {
                                    if (model.NoteBook.HoldNotes.ElementAt(j)[1].Size == rawNotes[i].NoteSize &&
                                        model.NoteBook.HoldNotes.ElementAt(j)[1].Position.Equals(rawNotes[i].Position)) // ノーツの追加の仕方的に、1番目にHoldEndあるよね
                                    {
                                        if (model.NoteBook.HoldNotes.ElementAt(j)[1] is AirableNote && 
                                            !((AirableNote)model.NoteBook.HoldNotes.ElementAt(j)[1]).IsAirHoldAttached)
                                        {
                                            var airable = ((AirableNote)model.NoteBook.HoldNotes.ElementAt(j)[1]) as AirableNote;
                                            model.NoteBook.AttachAirHoldToAirableNote(airable, ah, null);
                                            break;
                                        }
                                        break;
                                    }
                                }
                                if (j >= 0) break; /* Attach完了 */

                                for (j = model.NoteBook.SlideNotes.Count - 1; j >= 0; --j)
                                {
                                    if (model.NoteBook.SlideNotes.ElementAt(j)[1].Size == rawNotes[i].NoteSize && 
                                        model.NoteBook.SlideNotes.ElementAt(j)[1].Position.Equals(rawNotes[i].Position)) // ノーツの追加の仕方的に、1番目にSlideEndあるよね
                                    {
                                        if (model.NoteBook.SlideNotes.ElementAt(j)[1] is AirableNote &&
                                            !((AirableNote)model.NoteBook.SlideNotes.ElementAt(j)[1]).IsAirHoldAttached)
                                        {
                                            var airable = model.NoteBook.SlideNotes.ElementAt(j)[1] as AirableNote;
                                            model.NoteBook.AttachAirHoldToAirableNote(airable, ah, null);
                                            break;
                                        }
                                    }
                                }
                                if (j >= 0) break; /* Attach完了 */

                                // 設置のためのAirableNoteがないときは新しくTap作ってそれにくっつける
                                {
                                    Tap t = new Tap(rawNotes[i].NoteSize, rawNotes[i].Position, new PointF(), -1);
                                    model.NoteBook.Put(t);
                                    model.NoteBook.AttachAirHoldToAirableNote(t, ah, null);
                                }

                                break;
                            }
                            break;
                        }
                    case RawNote.RawNoteType.AirAction:
                    case RawNote.RawNoteType.AirHoldEnd: continue;

                    default: break;
                }
            }
            #endregion

            #region BPMを追加
            {
                foreach (KeyValuePair<RawNote, int> bpmApply in bpmApplys)
                {
                    for (int i = 0; i < model.NoteBook.AttributeNotes.Count; ++i)
                    {
                        /* 重複するBPM指定は削除しちゃいましょうねー */
                        if (model.NoteBook.AttributeNotes.ElementAt(i) is BPM &&
                            model.NoteBook.AttributeNotes.ElementAt(i).Position.Tick == bpmApply.Key.Position.Tick)
                        {
                            var att = model.NoteBook.AttributeNotes.ElementAt(i);
                            model.NoteBook.UnPut(att);
                            break;
                        }
                    }

                    // NOTE: BPM定義の書式がミスってたときにこれで弾けるけどこれいる？
                    if (!bpmDefs.ContainsKey(bpmApply.Value))
                    {
                        MessageBox.Show("BPM定義が不正です。\r\nBPMノーツが読み込まれないことがあります。", "インポート", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }
                    BPM bpm = new BPM(bpmApply.Key.Position, new PointF(), bpmDefs[bpmApply.Value], - 1);
                    model.NoteBook.Put(bpm);
                }
            }
            #endregion

            #region ハイスピを追加
            {
                /* ハイスピ指定を譜面に追加 */
                /* とりあえず譜面全体に一律に適用されているハイスピ指定のみ画面に表示することにする */
                HighSpeedTimeLine timeLine = null;
                if(measureLineTimeLineIndex >= 0 && model.TimeLineBook.ContainsKey(measureLineTimeLineIndex))
                {
                    timeLine = model.TimeLineBook.Get(measureLineTimeLineIndex);
                }
                /* ハイスピ定義のTickを校正し、かつ定義番号を連番に並べ替える */
                Dictionary<int, int> matching = new Dictionary<int, int>();
                model.TimeLineBook.FinalizePosition(model.ScoreBook, matching);
                
                /* 並べ替えでノーツのハイスピ定義番号ずれちゃうので修正 */
                List<Note> notes = model.NoteBook.GetNotesFromTickRange(0, model.ScoreBook.At(lastMeasureCount + 1).EndTick);
                for(int i=0; i<notes.Count; ++i)
                {
                    notes[i].TimeLineIndex = matching.ContainsKey(notes[i].TimeLineIndex) ? matching[notes[i].TimeLineIndex] : 0;
                }
                
                if(timeLine != null)
                {
                    for(int i=0; i<timeLine.Count; ++i)
                    {
                        model.NoteBook.Put(timeLine[i]);
                    }
                }
                else
                {
                    timeLine = new HighSpeedTimeLine();
                }
                /* 0番は譜面全体(小節線込み)に適用されるハイスピ定義ということにする */
                model.TimeLineBook.Add(0, timeLine);
            }
            #endregion

            /* 一応更新しておいた方が良いかな? */
            model.LaneBook.Clear(model.ScoreBook);
            model.LaneBook.SetScoreToLane(model.ScoreBook);

            {
                /* オフセット指定して出力した譜面をオフセット分巻き戻して表示したいよね というための処理 */
                /* 条件 : 譜面先頭から何の指定もない部分を詰める */
                /* 条件詳細 */
                /* 0小節目から0小節目に最も近いMEASUREBSで指定された小節までの区間が以下の条件を満たす場合、その区間を詰める */
                /* 1. BPM定義が無い(ただし0小節目先頭の定義のみ認める) */
                /* 2. ノーツ定義が無い */
                /* 3. ハイスピ指定が無い */
                /* 4. 拍数指定が無い == 4/4拍子になっている */
                if (baseMeasureCount > 0)
                {
                    List<Note> target = model.NoteBook.GetNotesFromTickRange(0, model.ScoreBook.At(baseMeasureCount).StartTick - 1);
                    if (target.Count == 0)
                    {
                        /* BPM定義すらないのおかしくない? */
                    }
                    else if (target.Count == 1)
                    {
                        /* 0小節目先頭のBPM定義しかないから消していいよね */
                        if (target[0] is BPM && target[0].Position.Tick == 0)
                        {
                            float bpm = (target[0] as BPM).NoteValue;

                            int i;
                            for (i = 0; i < baseMeasureCount; ++i)
                            {
                                if (!(model.ScoreBook.At(i).BeatNumer == 4 && model.ScoreBook.At(i).BeatDenom == 4)) break;
                            }
                            if (i == baseMeasureCount)
                            {
                                /* 消す処理 */
                                ScoreBook scoreList = new ScoreBook();
                                for (int j = 0; j < baseMeasureCount; ++j)
                                {
                                    scoreList.Add(model.ScoreBook.At(j));
                                }
                                /* この辺で死ぬ(エディタ上で小節削除でも死ぬ) */
                                // 多分死なないようになったはず
                                model.LaneBook.DeleteScore(model.ScoreBook, scoreList.First(), baseMeasureCount);
                                model.NoteBook.RelocateNoteTickAfterScoreTick(
                                    scoreList.Last().EndTick + 1, -(scoreList.Last().EndTick - scoreList.First().StartTick + 1));
                                model.LaneBook.OnUpdateNoteLocation();

                                /* もしかすると0小節目先頭のBPM定義消失してるかもしれないので入れ直してあげる */
                                int k;
                                for (k = 0; k < model.NoteBook.AttributeNotes.Count; ++k)
                                {
                                    if (model.NoteBook.AttributeNotes.ElementAt(k) is BPM &&
                                        model.NoteBook.AttributeNotes.ElementAt(k).Position.Tick == 0)
                                    {
                                        /* 0小節目先頭のBPM定義あったので何もしなくてよい */
                                        break;
                                    }
                                }
                                if (k == model.NoteBook.AttributeNotes.Count)
                                {
                                    model.NoteBook.Put(new BPM(new Position(0, 0), new PointF(), bpm, -1));
                                }

                                /* エクスポート時にオフセット再指定したいよね */
                                model.MusicInfo.MeasureOffset = baseMeasureCount;
                            }
                        }
                        else
                        {
                            /* BPM定義じゃないのやっぱりおかしくない? */
                        }
                    }
                    else
                    {
                        /* ノーツがあるっぽいから迂闊に消さない方が良くない? */
                    }
                }
            }

            /* notesAttributeDefs を musicInfo あたりに紐づけておかないと書き出せない */
            /* timeLineDefs を musicInfo あたりに紐づけておかないと書き出せない */

            return model;
        }
    }
}
