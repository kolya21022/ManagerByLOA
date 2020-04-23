using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.Services;
using ManagerByLetterOfAttorney.View.Util;
using ManagerByLetterOfAttorney.View.Pages.Edit;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.View.Pages.TableView
{
	/// <summary>
	/// Страница с таблицей [Курьеров]
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class CouriersTable : IPageable
	{
		/// <summary>
		/// Критерии фильтрации главного DataGrid страницы
		/// </summary>
		private readonly FilterCriterias _filterCriterias = new FilterCriterias();

		public CouriersTable()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
			// Указание первой строки выбранным объектом DataGrid
			PageUtil.SelectSpecifiedOrFirstDataGridRow<Courier>(PageDataGrid, null);
		}

		public CouriersTable(Courier selected)
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
			// Указание переданого объекта выбранным объектом в DataGrid или объекта первой строки в случае с null
			PageUtil.SelectSpecifiedOrFirstDataGridRow(PageDataGrid, selected);
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
				var couriers = CouriersService.GetAllWithCountUsing();
				if (couriers != null && couriers.Count > 0)
				{
					// Критерии сортировки указаны в реализации интерфейса IComparable класса
					couriers.Sort();
				}
				PageDataGrid.ItemsSource = couriers;
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
			const string delete = PageLiterals.HotkeyLabelDelete;
			const string closeApp = PageLiterals.HotkeyLabelCloseApp;
			const string separator = PageLiterals.HotkeyLabelsSeparator;
			const string displayed = edit + separator + filter + separator + delete + separator + closeApp;
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
		/// Нажатие кнопки [Добавить паспорт курьера]
		/// </summary>
		private void AddButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			// Action-делегат закрытия дополнительного окна добавления/редактирования курьера. 
			// В данном случае null, так как добавление курьера производится в главном окне приложения
			const Action<Courier> actionDelegateCloseExtraWindow = null;
			PageSwitcher.Switch(new CourierEdit(actionDelegateCloseExtraWindow));
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
			var courier = frameworkElement.DataContext as Courier;
			if (courier == null)
			{
				return;
			}
			// Action-делегат закрытия дополнительного окна добавления/редактирования курьера. 
			// В данном случае null, так как редактирование курьера производится в главном окне приложения
			const Action<Courier> actionDelegateCloseExtraWindow = null;
			PageSwitcher.Switch(new CourierEdit(actionDelegateCloseExtraWindow, courier));
		}

		/// <summary>
		/// Нажатие кнопки [УДЛ] главного DataGrid страницы - удаление объекта выделенной строки
		/// </summary>
		private void DeleteButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			var frameworkElement = senderIsButton as FrameworkElement;
			if (frameworkElement == null)
			{
				return;
			}
			var courier = frameworkElement.DataContext as Courier;
			if (courier == null)
			{
				return;
			}
			DeleteEntityIfConfirmation(courier); // Удаление сущности, если пользователь подтвердит намерения
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
			var selected = dataGrid.SelectedItem as Courier;
			if (selected == null)
			{
				return;
			}
			var key = eventArgs.Key;

			if (key == Key.Enter)     // Если нажат [Enter] - редактирование
			{
				eventArgs.Handled = true;
				// Action-делегат закрытия дополнительного окна добавления/редактирования курьера. 
				// В данном случае null, так как редактирование курьера производится в главном окне приложения
				const Action<Courier> actionDelegateCloseExtraWindow = null;
				PageSwitcher.Switch(new CourierEdit(actionDelegateCloseExtraWindow, selected));
			}

			// Если нажата [DeleteById] и текущее значение сервисного поля не противоречит удалению
			else if (key == Key.Delete && (selected.ServiceCountUsed == 0 || selected.ServiceCountUsed == null))
			{
				eventArgs.Handled = true;
				DeleteEntityIfConfirmation(selected);
			}
		}

		/// <summary>
		/// Подтверждение удаления сущности пользователем, удаление в случае одобрения и перезагрузка страницы
		/// </summary>
		private static void DeleteEntityIfConfirmation(Courier courier)
		{
			const string confirmHeader = PageLiterals.HeaderConfirm;
			const string confirmMessage = PageLiterals.СonfirmDeleteMessage;
			const MessageBoxResult defaultButtonFocus = MessageBoxResult.No;
			const MessageBoxButton buttons = MessageBoxButton.OKCancel;
			const MessageBoxImage type = MessageBoxImage.Question;
			var boxResult = MessageBox.Show(confirmMessage, confirmHeader, buttons, type, defaultButtonFocus);
			if (boxResult != MessageBoxResult.OK)
			{
				return;
			}
			try
			{
				CouriersService.Delete(courier);
				PageSwitcher.Switch(new CouriersTable());
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
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
			var selected = dataGrid.SelectedItem as Courier;
			// Проверка случая клика по заголовкам DataGrid или по пустой области без строк
			if (selected == null || PageUtil.IsClickByColumnHeaderOrScrollViewer(eventArgs))
			{
				return;
			}
			// Action-делегат закрытия дополнительного окна добавления/редактирования курьера. 
			// В данном случае null, так как редактирование курьера производится в главном окне приложения
			const Action<Courier> actionDelegateCloseExtraWindow = null;
			PageSwitcher.Switch(new CourierEdit(actionDelegateCloseExtraWindow, selected));
		}

		/// <summary>
		/// Перезаполнение данных главной таблицы с учётом фильтров
		/// </summary>
		private void PageDataGrid_Refilling()
		{
			PageUtil.PageDataGrid_RefillingWithFilters(PageDataGrid, _filterCriterias, MapFilterPredicate);
			ShowCountItemsPageDataGrid();   // Показ нового к-ва записей таблицы
			// Установка фокуса нужна для срабатывания Esc для закрытия
			PageUtil.SelectSpecifiedOrFirstDataGridRow<Courier>(PageDataGrid, null);
			PageUtil.SetFocusOnSelectedRowInDataGrid(PageDataGrid);
		}

		/// <summary>
		/// Метод-предикат (булевый) текущей записи коллекции сущностей, который возвращает true или 
		/// false в зависимости от попадания в диапазон фильтра по всем полям фильтрации.
		/// </summary>
		private bool MapFilterPredicate(object rawEntity)
		{
			var courier = (Courier)rawEntity;
			if (_filterCriterias.IsEmpty)
			{
				return true;
			}
			var result = true;

			// Проверка наличия полей сущности в критериях фильтрации и содержит ли поле искомое значение фильтра
			// Если в фильтре нет поля сущности, поле считается совпадающим по критерию
			string buffer;
			var filter = _filterCriterias;
			result &= !filter.GetValue("LastName", out buffer) || 
			          FilterCriterias.ContainsLine(courier.LastName, buffer);
			result &= !filter.GetValue("FirstName", out buffer) ||
			          FilterCriterias.ContainsLine(courier.FirstName, buffer);
			result &= !filter.GetValue("MiddleName", out buffer) ||
			          FilterCriterias.ContainsLine(courier.MiddleName, buffer);
			result &= !filter.GetValue("PassportSeriesAndNumber", out buffer) ||
			          FilterCriterias.ContainsLine(courier.PassportSeriesAndNumber, buffer);
			result &= !filter.GetValue("PassportIssuedByOrganization", out buffer) ||
			          FilterCriterias.ContainsLine(courier.PassportIssuedByOrganization, buffer);
			result &= !filter.GetValue("PassportIssueDate", out buffer) ||
			          FilterCriterias.ContainsDate(courier.PassportIssueDate, buffer);
			result &= !filter.GetValue("Profession", out buffer) ||
			          FilterCriterias.ContainsLine(courier.Profession, buffer);
			result &= !filter.GetValue("ServiceCountUsed", out buffer) ||
			          FilterCriterias.ContainsLong(courier.ServiceCountUsed, buffer);
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
