using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Component
{
    public partial class VersionInfoForm : Form
    {
        public VersionInfoForm()
        {
            InitializeComponent();
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            ((Bitmap)logo.Image).MakeTransparent();
            button1.Click += (s, e) => Close();
        }
    }
}
