using System.Windows.Media;

namespace ManagerByLetterOfAttorney.Util
{
	/// <summary>
	/// Служебные константы приложения (шрифт, цвета, относительные пути отчёта)
	/// </summary>
	public static class Constants
	{
		public const double FontSize = 14.0;
		public const string SerializeError = "Все вложенные классы сущности и этот класс должны быть [Serializable]";
		public const string LogicErrorPattern = "Ошибка логики приложения: {0}";
		public const string BuildDateTimePattern = "{0:yyyy.MM.dd \'/\' HH:mm}";

		// Названия относительных каталогов с отчётами
		public const string ReportFolder = "Reports";
		public const string AppDataFolder = "App_Data";

		// Шаблоны ФИО: полный и сокращённый (фамилия и инициалы с точками)
		public const string EmployeeFullNamePattern = "{0} {1} {2}";
		public const string EmployeeShortNamePattern = "{0} {1}.{2}.";
		
		// ReSharper disable InconsistentNaming  /* Названия цветов: http://chir.ag/projects/name-that-color/ */

		// Цвета текста
		public static readonly SolidColorBrush ForeColor1_BigStone   = Common.BrushHex("#1b293e"); // тёмно-синий
		public static readonly SolidColorBrush ForeColor2_PapayaWhip = Common.BrushHex("#ffefd5"); // бежевый
		public static readonly SolidColorBrush ForeColor3_Yellow     = Common.BrushHex("#ffff00"); // жёлтый

		// Цвета фонов
		public static readonly SolidColorBrush BackColor1_AthensGray  = Common.BrushHex("#eeeef2"); // светло-серый
		public static readonly SolidColorBrush BackColor2_Botticelli  = Common.BrushHex("#d6dbe9"); // серый
		public static readonly SolidColorBrush BackColor3_SanJuan     = Common.BrushHex("#364e6f"); // синий
		public static readonly SolidColorBrush BackColor4_BlueBayoux  = Common.BrushHex("#4d6082"); // синий
		public static readonly SolidColorBrush BackColor5_WaikawaGray = Common.BrushHex("#566c92"); // синий
		public static readonly SolidColorBrush BackColor6_Lochmara    = Common.BrushHex("#007acc"); // голубой
		public static readonly SolidColorBrush BackColor7_BahamaBlue  = Common.BrushHex("#005c99"); // голубой
		public static readonly SolidColorBrush BackColor8_DiSerria    = Common.BrushHex("#d3a35b"); // бежевый

		// Цвета границ и линий
		public static readonly SolidColorBrush LineBorderColor1_BigStone   = Common.BrushHex("#1b293e"); // тёмно-синий
		public static readonly SolidColorBrush LineBorderColor2_Nepal      = Common.BrushHex("#8e9bbc"); // серый
		public static readonly SolidColorBrush LineBorderColor3_SanJuan    = Common.BrushHex("#364e6f"); // синий
		public static readonly SolidColorBrush LineBorderColor4_BlueBayoux = Common.BrushHex("#4d6082"); // синий
		public static readonly SolidColorBrush LineBorderColor5_Sail       = Common.BrushHex("#b8d8f9"); // голубой

		// ReSharper restore InconsistentNaming
	}
}
