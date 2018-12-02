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
    public class DataLoader
    {
        private readonly OpenFileDialog openFileDialog;

        public DataLoader()
        {
            openFileDialog = new OpenFileDialog()
            {
                FileName = "Score.m4s",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "M4ple譜面ファイル(*.m4s)|*.m4s",
                FilterIndex = 0,
                Title = "開く",
                RestoreDirectory = true
            };
        }

        public Model ShowDialog()
        {
            Model model = null;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                model = DeserializeData();
            }
            return model;
        }

        private Model DeserializeData()
        {
            string path = openFileDialog.FileName;
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            Model model = binaryFormatter.Deserialize(fileStream) as Model;
            fileStream.Close();
            System.Diagnostics.Debug.Assert(model != null, "デシリアライズ失敗");
            return model;
        }
    }
}
