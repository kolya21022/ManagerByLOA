using System;
using ManagerByLetterOfAttorney.Util;

namespace ManagerByLetterOfAttorney.View.Util
{
	/// <summary>
	/// Строковые литералы / сообщения страниц приложения
	/// </summary>
	public static class PageLiterals
	{
		public const string LogicErrorPattern = Constants.LogicErrorPattern;
		public const string FilterBarCoverLabel = "Нажмите ПКМ по столбцу для фильтрации";

		// Состояние страницы редактирования
		public const string EditPageTypeEdit = "[Правка]";
		public const string EditPageTypeAdd = "[Добавление]";

		// Шаблоны надписей над таблицами
		public const string PatternCountItemsTable = "[Записей: {0}]";
		public const string PatternEmployeesTableWorkMonth = "[Раб.месяц: {0}]";
		public const string PatternReportPageTitle = "Отчёт № {0}";

		// Описания хоткеев страниц
		public const string HotkeyLabelsSeparator = ", ";
		public const string HotkeyLabelEdit = "[Enter/двойной клик мышкой] - редактировать";
		public const string HotkeyLabelFilter = "[ПКМ по столбцу] - фильтрация";
		public const string HotkeyLabelDelete = "[Del] - удаление";
		public const string HotkeyLabelJumpNext = "[Enter] - следующее поле";
		public const string HotkeyLabelCloseApp = "[Esc] - закрыть приложение";
		public const string HotkeyLabelCloseWindow = "[Esc] - закрыть это окно";
		public const string HotkeyLabelClosePageBackToList = "[Esc] - выйти к списку";

		// Сообщения подтверждений
		private const string ChangeNotSavedPart = "Введённые изменения полей страницы не сохранены.";
		public static readonly string ConfirmBackToListMessage = ChangeNotSavedPart + Environment.NewLine +
		    "Вы действительно хотите выйти к списку без сохранения изменений?";
		public static readonly string ConfirmPrintWithoutSaveMessage = ChangeNotSavedPart + Environment.NewLine +
		    "Вы действительно хотите перейти на страницу печати без сохранения изменений?";
		public const string СonfirmDeleteMessage = "Вы действительно хотите удалить указанную запись?";
		public const string ConfirmExitMessage = "Вы действительно хотите закрыть приложение?";

		// Заголовки сообщений MessageBox
		public const string HeaderConfirm = "Запрос подтверждения";
		public const string HeaderLogicError = "Ошибка логики приложения";
		public const string HeaderCriticalError = "Критическая ошибка приложения";
		public const string HeaderValidation = "Сообщение проверки корректности данных";
		public const string HeaderInformationOrWarning = "Информационное сообщение / предупреждение";
	}
}
