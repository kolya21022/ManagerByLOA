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
	/// Страница с таблицей [Единиц измерения]
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class MeasuresTable : IPageable
	{
		/// <summary>
		/// Критерии фильтрации главного DataGrid страницы
		/// </summary>
		private readonly FilterCriterias _filterCriterias = new FilterCriterias();

		public MeasuresTable()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
			// Указание первой строки выбранным объектом DataGrid
			PageUtil.SelectSpecifiedOrFirstDataGridRow<Measure>(PageDataGrid, null);
		}

		public MeasuresTable(Measure selected)
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
				var measures = MeasuresService.GetAllWithCountUsing();
				if (measures != null && measures.Count > 0)
				{
					// Критерии сортировки указаны в реализации интерфейса IComparable класса
					measures.Sort();
				}
				PageDataGrid.ItemsSource = measures;
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
		/// Нажатие кнопки [Добавить ед.измерения]
		/// </summary>
		private void AddButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			PageSwitcher.Switch(new MeasureEdit());
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
			var measure = frameworkElement.DataContext as Measure;
			if (measure == null)
			{
				return;
			}
			PageSwitcher.Switch(new MeasureEdit(measure));
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
			var measure = frameworkElement.DataContext as Measure;
			if (measure == null)
			{
				return;
			}
			DeleteEntityIfConfirmation(measure); // Удаление сущности, если пользователь подтвердит намерения
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
			var selected = dataGrid.SelectedItem as Measure;
			if (selected == null)
			{
				return;
			}
			var key = eventArgs.Key;

			if (key == Key.Enter)     // Если нажат [Enter] - редактирование
			{
				eventArgs.Handled = true;
				PageSwitcher.Switch(new MeasureEdit(selected));
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
		private static void DeleteEntityIfConfirmation(Measure measure)
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
				MeasuresService.Delete(measure);
				PageSwitcher.Switch(new MeasuresTable());
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
			var selected = dataGrid.SelectedItem as Measure;
			// Проверка случая клика по заголовкам DataGrid или по пустой области без строк
			if (selected == null || PageUtil.IsClickByColumnHeaderOrScrollViewer(eventArgs))
			{
				return;
			}
			PageSwitcher.Switch(new MeasureEdit(selected));
		}

		/// <summary>
		/// Перезаполнение данных главной таблицы с учётом фильтров
		/// </summary>
		private void PageDataGrid_Refilling()
		{
			PageUtil.PageDataGrid_RefillingWithFilters(PageDataGrid, _filterCriterias, MapFilterPredicate);
			ShowCountItemsPageDataGrid();   // Показ нового к-ва записей таблицы
			// Установка фокуса нужна для срабатывания Esc для закрытия
			PageUtil.SelectSpecifiedOrFirstDataGridRow<Measure>(PageDataGrid, null);
			PageUtil.SetFocusOnSelectedRowInDataGrid(PageDataGrid);
		}

		/// <summary>
		/// Метод-предикат (булевый) текущей записи коллекции сущностей, который возвращает true или 
		/// false в зависимости от попадания в диапазон фильтра по всем полям фильтрации.
		/// </summary>
		private bool MapFilterPredicate(object rawEntity)
		{
			var measure = (Measure)rawEntity;
			if (_filterCriterias.IsEmpty)
			{
				return true;
			}
			var result = true;

			// Проверка наличия полей сущности в критериях фильтрации и содержит ли поле искомое значение фильтра
			// Если в фильтре нет поля сущности, поле считается совпадающим по критерию
			string buffer;
			var filter = _filterCriterias;
			result &= !filter.GetValue("Name", out buffer) || FilterCriterias.ContainsLine(measure.Name, buffer);
			result &= !filter.GetValue("ServiceCountUsed", out buffer) || 
			          FilterCriterias.ContainsLong(measure.ServiceCountUsed, buffer);
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
