using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.Services;
using ManagerByLetterOfAttorney.View.Util;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.View.Pages.TableView
{
	/// <summary>
	/// Страница с таблицей [История выдачи доверенностей предыдущих лет]
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class PrevYearsHistoryTable : IPageable
	{
		/// <summary>
		/// Критерии фильтрации главного DataGrid страницы
		/// </summary>
		private readonly FilterCriterias _filterCriterias = new FilterCriterias();

		public PrevYearsHistoryTable()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
			// Указание первой строки выбранным объектом DataGrid
			PageUtil.SelectSpecifiedOrFirstDataGridRow<HistoryItem>(PageDataGrid, null);
		}

		/// <summary>
		/// Загрузка списка объектов из базы данных, их отображение в таблице, указание их кол-ва в Label
		/// </summary>
		/// <inheritdoc />
		public void AdditionalInitializeComponent()
		{
			FilterBarCoverLabel.Content = PageLiterals.FilterBarCoverLabel; // Сообщение-заглушка панели фильтрации
			try
			{
				var historyItems = ExportAndHistoryService.HistoryPreviousYears();
				if (historyItems != null && historyItems.Count > 0)
				{
					// Критерии сортировки указаны в реализации интерфейса IComparable класса
					historyItems.Sort();
				}
				PageDataGrid.ItemsSource = historyItems;
				ShowCountItemsPageDataGrid();
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
			}
		}

		/// <summary>
		/// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
		/// </summary>
		/// <inheritdoc />
		public void VisualInitializeComponent()
		{
			FontSize = Constants.FontSize;
			PageDataGrid.AlternatingRowBackground = Constants.BackColor1_AthensGray;

			// Заголовок страницы
			TitlePageGrid.Background = Constants.BackColor4_BlueBayoux;
			var titleLabels = TitlePageGrid.Children.OfType<Label>();
			foreach (var titleLabel in titleLabels)
			{
				titleLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}

			// Панель подробностей выделенной записи внизу страницы
			SummaryWrapperGrid.Background = Constants.BackColor4_BlueBayoux;
			SummaryTextBox.BorderBrush = Constants.LineBorderColor4_BlueBayoux;
			SummaryLabel.Foreground = Constants.ForeColor2_PapayaWhip;

			// Панель фильтрации и контекстное меню фильтра главного DataGrid
			var filterBarCoverLabels = FilterBarCoverStackPanel.Children.OfType<Label>();
			foreach (var label in filterBarCoverLabels)
			{
				label.Foreground = Constants.ForeColor1_BigStone;
			}
			FilterBarCoverStackPanel.Background = Constants.BackColor1_AthensGray;
			if (PageDataGrid.ContextMenu != null)
			{
				PageDataGrid.ContextMenu.FontSize = Constants.FontSize;
			}
		}

		/// <summary>
		/// Горячие клавиши текущей страницы
		/// </summary>
		/// <inheritdoc />
		public string PageHotkeys()
		{
			const string filter = PageLiterals.HotkeyLabelFilter;
			const string closeApp = PageLiterals.HotkeyLabelCloseApp;
			const string separator = PageLiterals.HotkeyLabelsSeparator;
			const string displayed = filter + separator + closeApp;
			return displayed;
		}

		/// <summary>
		/// Обработка нажатия клавиш в фокусе всей страницы 
		/// </summary>
		/// <inheritdoc />
		public void Page_OnKeyDown(object senderIsPageOrWindow, KeyEventArgs eventArgs)
		{
			if (eventArgs.Key != Key.Escape)
			{
				return;
			}
			eventArgs.Handled = true;
			PageUtil.ConfirmCloseApplication(); // Если нажат [Esc] - запрос подтверждения выхода у пользователя
		}

		/// <summary>
		/// Отображение числа записей в Label над таблицей
		/// </summary>
		private void ShowCountItemsPageDataGrid()
		{
			const string countItemsPattern = PageLiterals.PatternCountItemsTable;
			var message = string.Format(countItemsPattern, PageDataGrid.Items.Count);
			CountItemsLabel.Content = message;
		}

		/// <summary>
		/// Выставление клавиатурного фокуса ввода на строку DataGrid
		/// </summary>
		private void PageDataGrid_OnLoaded(object senderIsDatagrid, RoutedEventArgs eventArgs)
		{
			PageUtil.SetFocusOnSelectedRowInDataGrid(senderIsDatagrid);
		}

		/// <summary>
		/// Вывод в подробной информации о выделеной записи таблицы в текстовое поле снизу
		/// </summary>
		private void PageDataGrid_OnSelectionChanged(object senderIsDataGrid, SelectionChangedEventArgs eventArgs)
		{
			var summary = string.Empty;
			var dataGrid = senderIsDataGrid as DataGrid;
			if (dataGrid != null)
			{
				var historyItem = dataGrid.SelectedItem as HistoryItem;
				if (historyItem != null)
				{
					summary = GetSummary(historyItem);
				}
			}
			SummaryTextBox.Text = summary;
		}

		/// <summary>
		/// Получение детальной информации об объекте - используется для подстановки в поле просмотра
		/// </summary>
		private static string GetSummary(HistoryItem historyItem)
		{
			var empty = string.Empty;
			var newline = Environment.NewLine;
			const string patternBrackets = "[{0}] ";
			const string patternCount = " [к-во: {0}]";
			const string patternMeasure = " [ед.изм: {0}]";
			var formatPattern = "Год: [{0}]   Порядковый номер: [{1}]   Действительна с [{2:dd.MM.yyyy}] " +
			                    "по [{3:dd.MM.yyyy}]" + newline + "Организация: [{4}]" +
			                    newline + "ТМЦ: [{5}]{6}{7}" + newline + "Сотрудник/курьер: {8}[{9}]   " +
			                    "Должность: [{10}]" + newline + "Паспорт: [{11}]," +
			                    " выдан [{12:dd.MM.yyyy}] {13}";
			var displayPersNumb = historyItem.EmployeePersonnelNumber == null
				? empty : string.Format(patternBrackets, historyItem.EmployeePersonnelNumber);
			double? shipmentCount = historyItem.ShipmentCount;
			var displayCount = shipmentCount == null ? empty : string.Format(patternCount, shipmentCount);
			var displayMeasure = string.IsNullOrWhiteSpace(historyItem.ShipmentMeasure)
				? empty : string.Format(patternMeasure, historyItem.ShipmentMeasure);
			return string.Format(formatPattern, historyItem.Year, historyItem.OrdinalNumber, 
				historyItem.ValidityDateStart, historyItem.ValidityDateEnd,
				historyItem.Company, historyItem.ShipmentName, displayCount, displayMeasure,
				displayPersNumb, historyItem.CourierOrEmployeeFullName, historyItem.CourierOrEmployeeProfession,
				historyItem.PassportSeriesAndNumber, historyItem.PassportIssueDate, 
				historyItem.PassportIssuedByOrganization);
		}

		/// <summary>
		/// Перезаполнение данных главной таблицы с учётом фильтров
		/// </summary>
		private void PageDataGrid_Refilling()
		{
			PageUtil.PageDataGrid_RefillingWithFilters(PageDataGrid, _filterCriterias, MapFilterPredicate);
			ShowCountItemsPageDataGrid(); // Показ нового к-ва записей таблицы
			// Установка фокуса нужна для срабатывания Esc для закрытия
			PageUtil.SelectSpecifiedOrFirstDataGridRow<HistoryItem>(PageDataGrid, null);
			PageUtil.SetFocusOnSelectedRowInDataGrid(PageDataGrid);
		}

		/// <summary>
		/// Метод-предикат (булевый) текущей записи коллекции сущностей, который возвращает true или 
		/// false в зависимости от попадания в диапазон фильтра по всем полям фильтрации.
		/// </summary>
		private bool MapFilterPredicate(object rawEntity)
		{
			var historyItem = (HistoryItem)rawEntity;
			if (_filterCriterias.IsEmpty)
			{
				return true;
			}
			var result = true;

			// Проверка наличия полей сущности в критериях фильтрации и содержит ли поле искомое значение фильтра
			// Если в фильтре нет поля сущности, поле считается совпадающим по критерию
			string buffer;
			var filter = _filterCriterias;
			result &= !filter.GetValue("Year", out buffer) || FilterCriterias.ContainsLong(historyItem.Year, buffer);
			result &= !filter.GetValue("OrdinalNumber", out buffer) ||
			          FilterCriterias.ContainsLong(historyItem.OrdinalNumber, buffer);
			result &= !filter.GetValue("ValidityDateStart", out buffer) ||
			          FilterCriterias.ContainsDate(historyItem.ValidityDateStart, buffer);
			result &= !filter.GetValue("Company", out buffer) ||
			          FilterCriterias.ContainsLine(historyItem.Company, buffer);
			result &= !filter.GetValue("ShipmentName", out buffer) ||
			          FilterCriterias.ContainsLine(historyItem.ShipmentName, buffer);
			result &= !filter.GetValue("CourierOrEmployeeShortName", out buffer) ||
			          FilterCriterias.ContainsLine(historyItem.CourierOrEmployeeShortName, buffer);
			result &= !filter.GetValue("PassportSeriesAndNumber", out buffer) ||
			          FilterCriterias.ContainsLine(historyItem.PassportSeriesAndNumber, buffer);
			result &= !filter.GetValue("PassportIssueDate", out buffer) ||
			          FilterCriterias.ContainsDate(historyItem.PassportIssueDate, buffer);
			return result;
		}

		/// <summary>
		/// Нажатие клавиши в контексном меню - исправление дефекта скрытия фильтра при переключении раскладки ввода
		/// </summary>
		private void PopupFilterContextMenu_OnKeyDown(object senderIsMenuItem, KeyEventArgs eventArgs)
		{
			var key = eventArgs.Key;
			eventArgs.Handled = key == Key.System || key == Key.LeftAlt || key == Key.RightAlt;
		}

		/// <summary>
		/// Обработка нажатия Enter в поле ввода фильтра
		/// </summary>
		private void PopupFilterValue_OnKeyDown(object senderIsTextBox, KeyEventArgs eventArgs)
		{
			if (eventArgs.Key != Key.Return)
			{
				return;
			}
			eventArgs.Handled = true;
			PopupFilterConfirmButton_OnClick(senderIsTextBox, eventArgs);
		}

		/// <summary>
		/// Обработка применения фильтра
		/// </summary>
		private void PopupFilterConfirmButton_OnClick(object senderIsButtonOrTextBox, RoutedEventArgs eventArgs)
		{
			PageUtil.Service_PageDataGridPopupFilterConfirm(senderIsButtonOrTextBox, _filterCriterias);
			PageDataGrid_Refilling();
			FiltersDataGrid_Refilling();
		}

		/// <summary>
		/// Перезаполнение фильтрующего DataGrid и скрытие/отображение соответ. панелей в завис. от критериев фильтра
		/// </summary>
		private void FiltersDataGrid_Refilling()
		{
			PageUtil.RefillingFilterTableAndShowHidePanel(FiltersDataGrid, _filterCriterias, 
				FilterBarTableAndButtonGrid, FilterBarCoverStackPanel);
		}

		/// <summary>
		/// Подстановка имени столбца при открытии контексного меню фильтрации DataGrid, 
		/// установка Tag и перемещение фокуса ввода в поле
		/// </summary>
		private void PageDataGrid_OnContextMenuOpening(object senderIsDataGrid, ContextMenuEventArgs eventArgs)
		{
			PageUtil.Service_PageDataGridWithFilterContextMenuOpening(senderIsDataGrid);
		}

		/// <summary>
		/// Нажатие кнопки [Сброс фильтров] панели фильтрации - удаление всех введённых фильтров
		/// </summary>
		private void AllFilterDeleteButton_Click(object senderIsButton, RoutedEventArgs eventArgs)
		{
			_filterCriterias.ClearAll();    // очистка словаря критериев фильтрации
			FiltersDataGrid_Refilling();    // перезаполнение панели фильтров и скрытие/отображение с учётом критериев
			PageDataGrid_Refilling();       // перезаполнение главного DataGrid
		}

		/// <summary>
		/// Нажатие кнопки [УДЛ] DataGrid фильтрации - удаление указанного фильтра
		/// </summary>
		private void FilterDeleteButton_Click(object senderIsButton, RoutedEventArgs eventArgs)
		{
			var pressedButton = senderIsButton as Button;
			if (pressedButton == null)
			{
				return;
			}
			var deletedColumn = pressedButton.Tag as string; // получение столбца фильтра из св-ва Tag кнопки удаления
			if (string.IsNullOrWhiteSpace(deletedColumn))
			{
				return;
			}
			_filterCriterias.RemoveCriteria(deletedColumn); // удаление критерия фильтрации из словаря
			FiltersDataGrid_Refilling();    // перезаполнение панели фильтров и скрытие/отображение с учётом критериев
			PageDataGrid_Refilling();       // перезаполнение главного DataGrid
		}
	}
}
