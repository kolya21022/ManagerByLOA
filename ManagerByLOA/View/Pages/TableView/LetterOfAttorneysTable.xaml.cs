using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.Services;
using ManagerByLetterOfAttorney.View.Util;
using ManagerByLetterOfAttorney.View.Pages.Edit;
using ManagerByLetterOfAttorney.Entities.Internal;
using ManagerByLetterOfAttorney.View.Pages.Reports;

namespace ManagerByLetterOfAttorney.View.Pages.TableView
{
	/// <summary>
	/// Страница с таблицей [Доверенностей]
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class LetterOfAttorneysTable : IPageable
	{
		/// <summary>
		/// Критерии фильтрации главного DataGrid страницы
		/// </summary>
		private readonly FilterCriterias _filterCriterias = new FilterCriterias();

		public LetterOfAttorneysTable()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
			// Указание последней строки выбранным объектом DataGrid
			PageUtil.SelectSpecifiedOrLastDataGridRow<LetterOfAttorney>(PageDataGrid, null);
		}

		public LetterOfAttorneysTable(LetterOfAttorney selected)
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
			// Указание переданого объекта выбранным объектом в DataGrid или объекта последней строки в случае с null
			PageUtil.SelectSpecifiedOrLastDataGridRow(PageDataGrid, selected);
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
				var letterOfAttorneys = LetterOfAttorneysService.GetAll();
				if (letterOfAttorneys != null && letterOfAttorneys.Count > 0)
				{
					// Критерии сортировки указаны в реализации интерфейса IComparable класса
					letterOfAttorneys.Sort();
				}
				PageDataGrid.ItemsSource = letterOfAttorneys;
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
			const string edit = PageLiterals.HotkeyLabelEdit;
			const string filter = PageLiterals.HotkeyLabelFilter;
			const string closeApp = PageLiterals.HotkeyLabelCloseApp;
			const string separator = PageLiterals.HotkeyLabelsSeparator;
			const string displayed = edit + separator + filter + separator + closeApp;
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
		/// Нажатие кнопки [Новая доверенность]
		/// </summary>
		private void AddButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			PageSwitcher.Switch(new LetterOfAttorneyEdit());
		}

		/// <summary>
		/// Нажатие кнопки [ИЗМ] главного DataGrid страницы - редактирование объекта выделенной строки
		/// </summary>
		private void EditButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			var frameworkElement = senderIsButton as FrameworkElement;
			if (frameworkElement == null)
			{
				return;
			}
			var letterOfAttorney = frameworkElement.DataContext as LetterOfAttorney;
			if (letterOfAttorney == null)
			{
				return;
			}
			PageSwitcher.Switch(new LetterOfAttorneyEdit(letterOfAttorney));
		}

		/// <summary>
		/// Нажатие кнопки [ПЧТ] главного DataGrid страницы - печать объекта выделенной строки
		/// </summary>
		private void PrintButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			var frameworkElement = senderIsButton as FrameworkElement;
			if (frameworkElement == null)
			{
				return;
			}
			var letterOfAttorney = frameworkElement.DataContext as LetterOfAttorney;
			if (letterOfAttorney == null)
			{
				return;
			}
			PageSwitcher.Switch(new LoaReportPage(letterOfAttorney));
		}

		/// <summary>
		/// Обработка нажатия клавиш главного DataGrid страницы
		/// </summary>
		private void PageDataGrid_OnPreviewKeyDown(object senderIsDataGrid, KeyEventArgs eventArgs)
		{
			var dataGrid = senderIsDataGrid as DataGrid;
			if (dataGrid == null || dataGrid.Items.Count == 0)
			{
				return;
			}
			var selected = dataGrid.SelectedItem as LetterOfAttorney;
			if (selected == null)
			{
				return;
			}
			var key = eventArgs.Key;

			if (key == Key.Enter)     // Если нажат [Enter] - редактирование
			{
				PageSwitcher.Switch(new LetterOfAttorneyEdit(selected));
			}
		}

		/// <summary>
		/// Обработка двойного клика мышкой по строке DataGrid (редактирование сущности)
		/// </summary>
		private void PageDataGrid_OnPreviewMouseDoubleClick(object senderIsDataGrid, MouseButtonEventArgs eventArgs)
		{
			var dataGrid = senderIsDataGrid as DataGrid;
			if (dataGrid == null || dataGrid.Items.Count == 0)
			{
				return;
			}
			var selected = dataGrid.SelectedItem as LetterOfAttorney;
			// Проверка случая клика по заголовкам DataGrid или по пустой области без строк
			if (selected == null || PageUtil.IsClickByColumnHeaderOrScrollViewer(eventArgs))
			{
				return;
			}
			PageSwitcher.Switch(new LetterOfAttorneyEdit(selected));
		}

		/// <summary>
		/// Перезаполнение данных главной таблицы с учётом фильтров
		/// </summary>
		private void PageDataGrid_Refilling()
		{
			PageUtil.PageDataGrid_RefillingWithFilters(PageDataGrid, _filterCriterias, MapFilterPredicate);
			ShowCountItemsPageDataGrid();   // Показ нового к-ва записей таблицы
			// Установка фокуса нужна для срабатывания Esc для закрытия
			PageUtil.SelectSpecifiedOrLastDataGridRow<LetterOfAttorney>(PageDataGrid, null);
			PageUtil.SetFocusOnSelectedRowInDataGrid(PageDataGrid);
		}

		/// <summary>
		/// Метод-предикат (булевый) текущей записи коллекции сущностей, который возвращает true или 
		/// false в зависимости от попадания в диапазон фильтра по всем полям фильтрации.
		/// </summary>
		private bool MapFilterPredicate(object rawEntity)
		{
			var letterOfAttorney = (LetterOfAttorney)rawEntity;
			if (_filterCriterias.IsEmpty)
			{
				return true;
			}
			var result = true;

			// Проверка наличия полей сущности в критериях фильтрации и содержит ли поле искомое значение фильтра
			// Если в фильтре нет поля сущности, поле считается совпадающим по критерию
			string buffer;
			var filter = _filterCriterias;
			result &= !filter.GetValue("OrdinalNumber", out buffer) ||
			          FilterCriterias.ContainsLong(letterOfAttorney.OrdinalNumber, buffer);
			result &= !filter.GetValue("Company.Name", out buffer) ||
			          FilterCriterias.ContainsLine(letterOfAttorney.Company != null 
				          ? letterOfAttorney.Company.Name : string.Empty, buffer);
			result &= !filter.GetValue("ValidityDateStart", out buffer) ||
			          FilterCriterias.ContainsDate(letterOfAttorney.ValidityDateStart, buffer);
			result &= !filter.GetValue("ValidityDateEnd", out buffer) ||
			          FilterCriterias.ContainsDate(letterOfAttorney.ValidityDateEnd, buffer);
			result &= !filter.GetValue("Courier.LastName", out buffer) ||
			          FilterCriterias.ContainsLine(letterOfAttorney.Courier != null
				          ? letterOfAttorney.Courier.LastName : string.Empty, buffer);
			result &= !filter.GetValue("Shipments", out buffer) ||
			          ContainsLineInShipmentsList(letterOfAttorney.Shipments, buffer);
			return result;
		}

		/// <summary>
		/// Проверка наличия в исходной коллекции сущностей Shipment искомых значений.
		/// Строка искомых значений разделяется на несколько по пробелами и проверяется наличие каждого из значений
		/// </summary>
		private static bool ContainsLineInShipmentsList(IList<Shipment> shipments, string finded)
		{
			if (shipments == null)
			{
				return false;
			}

			// В случае с 1 Shipment в списке ищется только в нём
			if (shipments.Count == 1 && shipments[0].Cargo != null) 
			{
				return FilterCriterias.ContainsLine(shipments[0].Cargo.Name, finded);
			}

			// В случае со множеством значений Shipment, они соединяются в 1 строку для упрощения поиска
			var stringBuilder = new StringBuilder();
			foreach (var shipment in shipments)	
			{
				if (shipment.Cargo != null && !string.IsNullOrWhiteSpace(shipment.Cargo.Name))
				{
					stringBuilder.Append(shipment.Cargo.Name);
				}
			}
			return FilterCriterias.ContainsLine(stringBuilder.ToString(), finded);
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
			FiltersDataGrid_Refilling();
			PageDataGrid_Refilling();
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
