using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Component
{
    public class DisplayPanelBox
    {
        public event Action<MouseEventArgs> MouseDown, MouseUp, MouseMove, MouseDrag;
        private readonly PictureBox pictureBox;
        private readonly VScrollBar vScroll;
        private readonly HScrollBar hScroll;
        private bool clicking = false;

        public DisplayPanelBox(PictureBox pBox, VScrollBar vs, HScrollBar hs)
        {
            pictureBox = pBox;
            pictureBox.MouseDown += (s, e) =>
            {
                clicking = true;
                MouseDown.Invoke(e);
            };
            pictureBox.MouseMove += (s, e) =>
            {
                if (clicking)
                {
                    MouseDrag.Invoke(e);
                }
                else
                {
                    MouseMove.Invoke(e);
                }
            };
            pictureBox.MouseUp += (s, e) =>
            {
                clicking = false;
                MouseUp.Invoke(e);
            };
            vScroll = vs;
            hScroll = hs;
        }
    }
}
