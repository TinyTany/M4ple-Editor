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
        public string Path { get; private set; } = null;

        public DataLoader()
        {
            openFileDialog = new OpenFileDialog()
            {
                FileName = "",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "M4ple譜面ファイル(*.m4s)|*.m4s",
                FilterIndex = 0,
                Title = "開く",
                RestoreDirectory = true
            };
        }

        public Model ShowDialog()
        {
            Model model = null;
            if (Directory.Exists(Status.OpenDialogDirectory))
            {
                openFileDialog.FileName = "";
                openFileDialog.InitialDirectory = Status.OpenDialogDirectory;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                model = DeserializeData();
                if(model == null)
                {
                    MessageBox.Show("ファイルを開けませんでした。\nファイルが破損しているか、対応していない可能性があります。", "読み込みエラー");
                }
            }
            return model;
        }

        private Model DeserializeData()
        {
            Path = openFileDialog.FileName;
            Status.OpenDialogDirectory = Directory.GetParent(Path).ToString();
            FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            Model model;
            try
            {
                model = binaryFormatter.Deserialize(fileStream) as Model;
            }
            catch (Exception)
            {
                model = null;
                Logger.Error("デシリアライズに失敗しました。", true);
            }
            fileStream.Close();
            return model;
        }
    }
}
