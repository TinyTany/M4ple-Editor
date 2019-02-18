using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace NE4S.Component
{
    class ManualGraphics : IDisposable
    {
        private readonly BufferedGraphics bufferedGraphics;
        private readonly PictureBox pictureBox;

        public Graphics Graphics
        {
            get
            {
                return bufferedGraphics?.Graphics;
            }
        }

        public ManualGraphics(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            BufferedGraphicsContext currentContext;
            currentContext = BufferedGraphicsManager.Current;
            pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            bufferedGraphics = currentContext.Allocate(Graphics.FromImage(pictureBox.Image), pictureBox.DisplayRectangle);
        }

        public void Clear()
        {
            bufferedGraphics.Graphics.Clear(pictureBox.BackColor);
        }

        public void Refresh()
        {
            bufferedGraphics?.Render();
            
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出する

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。
                bufferedGraphics.Dispose();

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~ManualGraphics() {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
