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

        public void ShowDialog(Model model)
        {
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SerializeData(model);
            }
            return;
        }

        private void SerializeData(Model model)
        {
            string path = saveFileDialog.FileName;
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, model);
            fileStream.Close();
            return;
        }
    }
}
