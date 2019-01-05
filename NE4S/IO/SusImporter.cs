using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NE4S.IO
{
    public class SusImporter
    {
        public SusImporter()
        {

        }

        public bool Import(in string path, Model model)
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                while (streamReader.Peek() > -1)
                {
                    string dataLine = streamReader.ReadLine();
                    if (!dataLine.Any() || dataLine[0] != '#')
                    {
                        continue;
                    }
                    ReadHeader(dataLine, model);
                }
            }
            return true;
        }

        private void ReadHeader(in string dataLine, Model model)
        {
            string fineText = dataLine;
            if (dataLine.Contains("#TITLE"))
            {
                fineText = dataLine.Replace("#TITLE", "");
                fineText = NormalizeString(fineText);
                model.MusicInfo.Title = fineText;
            }
            else if (dataLine.Contains("#ARTIST"))
            {
                fineText = dataLine.Replace("#ARTIST", "");
                fineText = NormalizeString(fineText);
                model.MusicInfo.Artist = fineText;
            }
            else if (dataLine.Contains("#DESIGNER"))
            {
                fineText = dataLine.Replace("#DESIGNER", "");
                fineText = NormalizeString(fineText);
                model.MusicInfo.Designer = fineText;
            }
            else if (dataLine.Contains("#DIFFICULTY"))
            {
                fineText = dataLine.Replace("#DIFFICULTY", "");
                fineText = NormalizeString(fineText);
                if (fineText.Contains(":"))
                {
                    fineText = fineText.Split(':')[1];
                    model.MusicInfo.Difficulty = 4;
                    model.MusicInfo.WEKanji = fineText;
                }
                else if(int.TryParse(fineText, out int result))
                {
                    model.MusicInfo.Difficulty = result;
                }
            }
            else if (dataLine.Contains("#PLAYLEVEL"))
            {
                fineText = dataLine.Replace("#PLAYLEVEL", "");
                if (!int.TryParse(fineText, out int result) || model.MusicInfo.Difficulty != 4)
                {
                    model.MusicInfo.PlayLevel = fineText;
                }
                else if (result > 0)
                {
                    model.MusicInfo.WEStars = result;
                }
            }
            else if (dataLine.Contains("#SONGID"))
            {
                fineText = dataLine.Replace("#SONGID", "");
                fineText = NormalizeString(fineText);
                model.MusicInfo.SongId = fineText;
            }
            else if (dataLine.Contains("#WAVE"))
            {
                fineText = dataLine.Replace("#WAVE", "");
                fineText = NormalizeString(fineText);
                model.MusicInfo.MusicFileName = fineText;
            }
            else if (dataLine.Contains("#WAVEOFFSET"))
            {
                fineText = dataLine.Replace("#WAVEOFFSET", "");
                fineText = NormalizeString(fineText);
                if (decimal.TryParse(fineText, out decimal result))
                {
                    model.MusicInfo.MusicOffset = result;
                }
            }
            else if (dataLine.Contains("#JACKET"))
            {
                fineText = dataLine.Replace("#JACKET", "");
                fineText = NormalizeString(fineText);
                model.MusicInfo.JacketFileName = fineText;
            }
            else if (dataLine.Contains("#REQUEST"))
            {
                fineText = dataLine.Replace("#REQUEST", "");
                fineText = NormalizeString(fineText);
                if (fineText.Contains("mertonome") || fineText.Contains("metronome"))
                {
                    if (fineText.Contains("enabled"))
                    {
                        model.MusicInfo.Metronome = 0;
                    }
                    else
                    {
                        model.MusicInfo.Metronome = 1;
                    }
                }
                else if (fineText.Contains("segment_per_second") || fineText.Contains("segments_per_second"))
                {
                    if (fineText.Contains(" "))
                    {
                        fineText = fineText.Split(' ')[1];
                        if (int.TryParse(fineText, out int result) && result > 0)
                        {
                            model.MusicInfo.SlideCurveSegment = result;
                        }
                    }
                }
            }
        }

        private void ReadData(in string dataLine, Model model)
        {

        }

        private string NormalizeString(in string str)
        {
            string retStr = str;
            retStr = retStr.Trim();
            retStr = retStr.Trim('\"');
            return retStr;
        }
    }
}
