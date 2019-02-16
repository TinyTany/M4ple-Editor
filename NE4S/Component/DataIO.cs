using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NE4S.Component
{
    public class DataIO
    {
        public DataSaver DataSaver { get; private set; }
        public DataLoader DataLoader { get; private set; }
        public string FileName { get; set; } = null;

        /// <summary>
        /// 譜面を新規作成する際にもこのコンストラクタを呼んでください
        /// </summary>
        public DataIO()
        {
            DataSaver = new DataSaver();
            DataLoader = new DataLoader();
        }

        public bool Save(Model model)
        {
            if(DataSaver.Path == null)
            {
                return SaveAs(model);
            }
            else
            {
                DataSaver.SerializeData(model);
                return true;
            }
        }

        public bool SaveAs(Model model)
        {
            bool isSaved = DataSaver.ShowDialog(model);
            if (isSaved)
            {
                FileName = Path.GetFileName(DataSaver.Path);
            }
            return isSaved;
        }

        public Model Load()
        {
            Model model = DataLoader.ShowDialog();
            //正常にデータの読み込みが行われた時
            if(model != null)
            {
                DataSaver.Path = DataLoader.Path;
                FileName = Path.GetFileName(DataSaver.Path);
                // NOTE: 拍数の描画をするようにしたので、古いバージョンのm4sを読み込んだときでもうまくいくように更新をかける
                model.ScoreBook.SetScoreIndex();
            }
            return model;
        }
    }
}
