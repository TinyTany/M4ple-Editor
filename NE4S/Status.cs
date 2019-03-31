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

        public static string ExportDialogDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string OpenDialogDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string MusicDialogDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        public static string JacketDialogDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    }
}
