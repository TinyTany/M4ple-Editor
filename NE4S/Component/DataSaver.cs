using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace NE4S.Component
{
    public class DataSaver
    {
        private readonly SaveFileDialog saveFileDialog;
        public string Path { get; set; } = null;

        public DataSaver()
        {
            saveFileDialog = new SaveFileDialog()
            {
                FileName = "NewScore.m4s",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "M4ple譜面ファイル(*.m4s)|*.m4s",
                FilterIndex = 0,
                Title = "名前をつけて保存",
                RestoreDirectory = true
            };
        }

        public bool ShowDialog(Model model)
        {
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Path = saveFileDialog.FileName;
                SerializeData(model);
                return true;
            }
            return false;
        }

        public void SerializeData(Model model)
        {
            FileStream fileStream = new FileStream(Path, FileMode.Create, FileAccess.Write);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, model);
            fileStream.Close();
        }
    }
}
