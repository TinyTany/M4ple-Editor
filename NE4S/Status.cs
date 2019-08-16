using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;

namespace NE4S
{
    /// <summary>
    /// グローバル変数
    /// </summary>
    public static class Status
    {
        public static event Action<bool> CopyChanged, PasteChanged;
        public static void OnCopyChanged(bool b)
        {
            IsCopyAvailable = b;
            CopyChanged?.Invoke(b);
        }
        public static void OnPasteChanged(bool b)
        {
            IsPasteAvailable = b;
            PasteChanged?.Invoke(b);
        }
        public static bool IsCopyAvailable { get; private set; } = false;
        public static bool IsPasteAvailable { get; private set; } = false;

        public static Define.Mode Mode { get; set; } = Define.Mode.Add;
        public static int Beat { get; set; } = 16;
        public static int Grid { get; set; } = 16;
        public static int Note { get; set; } = Define.NoteType.AIRHOLD;
        public static int NoteSize { get; set; } = 4;
        public static float CurrentValue { get; set; } = 0;
        public static bool InvisibleSlideTap { get; set; } = true;
		public static bool IsMousePressed { get; set; } = false;
		public static Note SelectedNote { get; set; } = null;
        public static Define.NoteArea SelectedNoteArea { get; set; } = Define.NoteArea.None;

        public static bool IsSlideRelayVisible { get; set; } = true;
        public static bool IsSlideCurveVisible { get; set; } = true;
        public static bool IsShortNoteVisible { get; set; } = true;
        public static bool IsHoldVisible { get; set; } = true;
        public static bool IsSlideVisible { get; set; } = true;
        public static bool IsAirHoldVisible { get; set; } = true;
        public static bool IsAirVisible { get; set; } = true;
        public static bool IsExTapDistinct { get; set; } = true;
        public static bool IsEconomyMode { get; set; } = false;

        public static int DrawTickFirst { get; set; }
        public static int DrawTickLast { get; set; }

        public static string ExportDialogDirectory
        {
            get
            {
                if (Properties.Settings.Default.ExportDirectory == "")
                {
                    Properties.Settings.Default.ExportDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                return Properties.Settings.Default.ExportDirectory;
            }
            set
            {
                Properties.Settings.Default.ExportDirectory = value;
            }
        }
        public static string OpenDialogDirectory
        {
            get
            {
                if (Properties.Settings.Default.OpenDirectory == "")
                {
                    Properties.Settings.Default.OpenDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                return Properties.Settings.Default.OpenDirectory;
            }
            set
            {
                Properties.Settings.Default.OpenDirectory = value;
            }
        }
        public static string MusicDialogDirectory
        {
            get
            {
                if (Properties.Settings.Default.MusicDirectory == "")
                {
                    Properties.Settings.Default.MusicDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                }
                return Properties.Settings.Default.MusicDirectory;
            }
            set
            {
                Properties.Settings.Default.MusicDirectory = value;
            }
        }
        public static string JacketDialogDirectory
        {
            get
            {
                if (Properties.Settings.Default.JacketDirectory == "")
                {
                    Properties.Settings.Default.JacketDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                }
                return Properties.Settings.Default.JacketDirectory;
            }
            set
            {
                Properties.Settings.Default.JacketDirectory = value;
            }
        }
    }
}
