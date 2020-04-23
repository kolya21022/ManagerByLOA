using System;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace ManagerByLetterOfAttorney.View.Util
{
	/// <summary>
	/// Утилитарные методы страниц приложения
	/// </summary>
	public static class PageUtil
	{
		/// <summary>
		/// Установка выбраной записью DataGrid указанного объекта, или выбрать первую запись, если объект отсутствует
		/// </summary>
		public static void SelectSpecifiedOrFirstDataGridRow<T>(DataGrid pageDataGrid, T selectedItem)
		{
			if (pageDataGrid.Items.Count == 0)
			{
				return;
			}
			pageDataGrid.SelectedItem = selectedItem == null ? pageDataGrid.Items[0] : selectedItem;
		}

		/// <summary>
		/// Установка выбраной записью DataGrid указанного объекта, или выбрать послед. запись, если объект отсутствует
		/// </summary>
		public static void SelectSpecifiedOrLastDataGridRow<T>(DataGrid pageDataGrid, T selectedItem)
		{
			if (pageDataGrid.Items.Count == 0)
			{
				return;
			}
			pageDataGrid.SelectedItem = selectedItem == null 
				? pageDataGrid.Items[pageDataGrid.Items.Count - 1] 
				: selectedItem;
		}

		/// <summary>
		/// Проверка был ли произведено событие клика мышкой по заголовку DataGrid или по ScrollViewer.
		/// Требуется для не-отображения контексного меню фильтрации в этих случаях.
		/// </summary>
		public static bool IsClickByColumnHeaderOrScrollViewer(RoutedEventArgs eventArgs)
		{
			var dependObj = eventArgs.OriginalSource as DependencyObject;
			if (dependObj is ScrollViewer)
			{
				return true;
			}
			while (dependObj != null && !(dependObj is DataGridCell) && !(dependObj is DataGridColumnHeader))
			{
				dependObj = VisualTreeHelper.GetParent(dependObj);
			}
			return dependObj is DataGridColumnHeader;
		}

		/// <summary>
		/// Установка клавиатурного фокуса ввода на выбраную запись DataGrid (Вызов Focus не работает как нужно)
		/// </summary>
		public static void SetFocusOnSelectedRowInDataGrid(object senderIsDatagrid)
		{
			var dataGrid = senderIsDatagrid as DataGrid;
			if (dataGrid == null)
			{
				return;
			}
			if (dataGrid.Items.Count == 0 || dataGrid.SelectedItem == null)
			{
				Keyboard.Focus(dataGrid);
				return;
			}
			var selected = dataGrid.SelectedItem;

			// NOTE: эта копипаста ниже не случайна, нужный функционал срабатывает только со второго раза.
			// Решение в указаном ответе: https://stackoverflow.com/a/27792628 Работает, не трогай
			var row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(selected);
			if (row == null)
			{
				dataGrid.UpdateLayout();
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
				row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(selected);
			}
			if (row == null)
			{
				return;
			}
			dataGrid.UpdateLayout();
			dataGrid.ScrollIntoView(dataGrid.SelectedItem);
			row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}

		/// <summary>
		/// Перемещение клавиатурного фокуса ввода на следующее поле при нажатии Enter. 
		/// Следующее поле определяется методом FocusNavigationDirection.Next
		/// </summary>
		public static void JumpToNextWhenPressEnter_OnKeyDown(KeyEventArgs eventArgs)
		{
			if (eventArgs.Key != Key.Enter)
			{
				return;
			}
			const FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;
			var request = new TraversalRequest(focusDirection);
			var elementWithFocus = Keyboard.FocusedElement as UIElement;
			if (elementWithFocus == null)
			{
				return;
			}
			if (elementWithFocus.MoveFocus(request))
			{
				eventArgs.Handled = true;
			}
		}

		/// <summary>
		/// Перемещение клавиатурного фокуса на нижележащее поле.
		/// Нижележащее поле определяется методом FocusNavigationDirection.Down
		/// </summary>
		public static void JumpToDown_OnKeyDown(KeyEventArgs eventArgs)
		{
			const FocusNavigationDirection focusDirection = FocusNavigationDirection.Down;
			var request = new TraversalRequest(focusDirection);
			var elementWithFocus = Keyboard.FocusedElement as UIElement;
			if (elementWithFocus == null)
			{
				return;
			}
			if (elementWithFocus.MoveFocus(request))
			{
				eventArgs.Handled = true;
			}
		}

		/// <summary>
		/// Сервисный метод раскрытия выпадающего списка для события GotFocus для поискового ComboBox.
		/// (Поисковый ComboBox: с параметрами IsTextSearchEnabled="True" и IsEditable="True")
		/// </summary>
		public static void Service_SearchComboBox_OnGotFocus(object senderIsComboBox)
		{
			var comboBox = senderIsComboBox as ComboBox;
			if (comboBox != null && comboBox.IsEditable && comboBox.IsDropDownOpen == false 
			    && comboBox.StaysOpenOnEdit)
			{
				comboBox.IsDropDownOpen = true;
			}
		}

		/// <summary>
		/// Сервисный метод раскрытия выпадающего списка для события PreviewMouseUp для поискового ComboBox.
		/// (Поисковый ComboBox: с параметрами IsTextSearchEnabled="True" и IsEditable="True")
		/// </summary>
		public static void Service_SearchComboBox_OnPreviewMouseUp(object senderIsComboBox)
		{
			var comboBox = senderIsComboBox as ComboBox;
			if (comboBox != null && comboBox.IsEditable && comboBox.IsDropDownOpen == false
			    && comboBox.StaysOpenOnEdit)
			{
				comboBox.IsDropDownOpen = true;
			}
		}

		/// <summary>
		/// Сервисный метод раскрытия выпадающего списка для события PreviewMouseDown для поискового ComboBox.
		/// (Поисковый ComboBox: с параметрами IsTextSearchEnabled="True" и IsEditable="True")
		/// </summary>
		public static void Service_SearchComboBox_OnPreviewMouseDown(object senderIsComboBox)
		{
			var comboBox = senderIsComboBox as ComboBox;
			if (comboBox != null && comboBox.IsEditable && comboBox.IsDropDownOpen == false
			    && comboBox.StaysOpenOnEdit)
			{
				comboBox.IsDropDownOpen = true;
			}
		}

		/// <summary>
		/// Сервисный метод обработчик для события PreviewMouseDown для поискового ComboBox.
		/// Нажатии [Enter] клавиатурный фокус перемещается на нижележащее поле, иначе раскрытие выпадающего списка.
		/// Нижележащее поле определяется методом FocusNavigationDirection.Down
		/// </summary>
		public static void Service_SearchComboBox_OnPreviewKeyDown(object senderIsComboBox, KeyEventArgs eventArgs)
		{
			var comboBox = senderIsComboBox as ComboBox;
			if (comboBox == null)
			{
				return;
			}
			if (eventArgs.Key.Equals(Key.Enter) || eventArgs.Key.Equals(Key.Return))
			{
				var nextControlAfterThis = comboBox.PredictFocus(FocusNavigationDirection.Down) as Control;
				if (nextControlAfterThis == null)
				{
					return;
				}
				nextControlAfterThis.Focus();
				eventArgs.Handled = true;
			}
			else
			{
				if (comboBox.IsEditable && comboBox.IsDropDownOpen == false && comboBox.StaysOpenOnEdit)
				{
					comboBox.IsDropDownOpen = true;
				}
			}
		}

		/// <summary>
		/// Сервисный метод обработчик для события PreviewKeyUp для поискового ComboBox.
		/// Нажатии [Enter] клавиатурный фокус перемещается на нижележащее поле.
		/// Нижележащее поле определяется методом FocusNavigationDirection.Down
		/// </summary>
		public static void Service_SearchComboBox_OnPreviewKeyUp(object senderIsComboBox, KeyEventArgs eventArgs)
		{
			var comboBox = senderIsComboBox as ComboBox;
			if (comboBox == null)
			{
				return;
			}
			var key = eventArgs.Key;
			if (!key.Equals(Key.Enter) && !key.Equals(Key.Return))
			{
				return;
			}
			var nextControlAfterThis = comboBox.PredictFocus(FocusNavigationDirection.Down) as Control;
			if (nextControlAfterThis == null)
			{
				return;
			}
			nextControlAfterThis.Focus();
			eventArgs.Handled = true;
		}

		/// <summary>
		/// Попытка установки клавиатурного фокуса ввода ассинхронно в отдельном потоке.
		/// Требуется в некоторых случаях, когда обычный вызов Focus не срабатывает.
		/// Источник: https://stackoverflow.com/a/3771441
		/// </summary>
		private static void SetElementFocusInOtherThread(UIElement element)
		{
			if (!element.Focus())
			{
				element.Dispatcher.BeginInvoke(DispatcherPriority.Input, 
					new ThreadStart(delegate { element.Focus(); }));
			}
		}

		/// <summary>
		/// Сервисный метод фильтрующего главного DataGrid страницы, при открытии контексного меню фильтрации.
		/// Подставляет название столбца в контекстное меню, устанавливает Tag и перемещение фокуса ввода в поле ввода.
		/// Метод работает только с указанной компоновкой элементов, не привязываясь к конкретным объктам страниц:
		/// DataGrid -> ContextMenu -> MenuItem -> MenuItem.Header -> Grid -> [TextBlock + TextBox + Button]
		/// </summary>
		public static void Service_PageDataGridWithFilterContextMenuOpening(object senderIsDataGrid)
		{
			var dataGrid = senderIsDataGrid as DataGrid;   // Главный DataGrid страницы
			if (dataGrid == null)
			{
				return;
			}
			var contextMenu = dataGrid.ContextMenu;        // Контекстное меню этого DataGrid
			if (contextMenu == null)
			{
				return;
			}
			if (dataGrid.Items.Count == 0)
			{
				contextMenu.Visibility = Visibility.Collapsed;
				return;
			}
			
			// Для получения текущего столбца требуется переключить режим выделения DataGrid со строки на ячейку
			dataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
			var currentColumn = dataGrid.CurrentColumn;
			dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;  // Переключение режима выделения обратно на строку

			if (currentColumn == null || currentColumn.Header == null)
			{
				contextMenu.Visibility = Visibility.Collapsed;
				return;
			}
			var columnName = currentColumn.SortMemberPath;                 // Название столбца 
			if (string.IsNullOrWhiteSpace(columnName))
			{
				contextMenu.Visibility = Visibility.Collapsed;
				return;
			}
			contextMenu.Visibility = Visibility.Visible;
			var сontextMenuItems = contextMenu.Items;
			if (сontextMenuItems.Count != 1)
			{
				return;
			}
			var сontextMenuItem = сontextMenuItems[0] as MenuItem;         // MenuItem контекстного меню
			if (сontextMenuItem == null)
			{
				return;
			}
			var filterFieldWrapperGrid = сontextMenuItem.Header as Grid;   // Grid-обёртка полей контекстного меню
			if (filterFieldWrapperGrid == null)
			{
				return;
			}
			var textBlocks = filterFieldWrapperGrid.Children.OfType<TextBlock>().ToArray();
			var textBoxes = filterFieldWrapperGrid.Children.OfType<TextBox>().ToArray();
			if (textBlocks.Length != 1 || textBoxes.Length != 1)
			{
				return;
			}
			var popupFilterName = textBlocks[0];                    // Контрол для надписи
			var popupFilterValue = textBoxes[0];                    // Поле ввода
			popupFilterName.Text = currentColumn.Header.ToString(); // Отображение названия столбца пользователю
			popupFilterValue.Tag = columnName;                      // Установка столбца в Tag поля ввода
			popupFilterValue.Text = string.Empty;
			SetElementFocusInOtherThread(popupFilterValue);         // Установка клавиатурного фокуса в поле ввода
		}

		/// <summary>
		/// Сервисный метод фильтрующего главного DataGrid страницы - применения фильтра.
		/// </summary>
		public static void Service_PageDataGridPopupFilterConfirm(object senderIsButtonOrTextBox, 
			FilterCriterias filterCriterias)
		{
			var popupControl = senderIsButtonOrTextBox as Control; // Control вызвавший событие (TextBox или Button)
			if (popupControl == null)
			{
				return;
			}
			var grid = popupControl.Parent as Grid;                // Grid-обёртка полей контекстного меню
			if (grid == null)
			{
				return;
			}
			var menuItem = grid.Parent as MenuItem;                // MenuItem контекстного меню
			if (menuItem == null)
			{
				return;
			}
			var contextMenu = menuItem.Parent as ContextMenu;      // Контекстное меню DataGrid
			if (contextMenu == null)
			{
				return;
			}
			var textBlocks = grid.Children.OfType<TextBlock>().ToArray();
			var textBoxes = grid.Children.OfType<TextBox>().ToArray();
			if (textBlocks.Length != 1 || textBoxes.Length != 1)
			{
				return;
			}
			var popupFilterName = textBlocks[0];            // Контрол для надписи
			var popupFilterValue = textBoxes[0];            // Поле ввода
			var columnName = (string)popupFilterValue.Tag;  // Получения столбца из свойства Tag поля ввода
			var columnDysplayedName = popupFilterName.Text; // Получение отображаемого имени столбца из надписи
			var filterValue = popupFilterValue.Text.Trim(); // Получение значения фильтра
			if (string.IsNullOrWhiteSpace(filterValue))
			{
				return;
			}

			// Добавление/обновления критерия фильтрации и скрытие контекстного меню
			filterCriterias.UpdateCriteria(columnName, filterValue, columnDysplayedName);
			contextMenu.Visibility = Visibility.Collapsed;
		}

		/// <summary>
		/// Перезаполнение главного DataGrid страницы с учётом фильтров
		/// </summary>
		public static void PageDataGrid_RefillingWithFilters(DataGrid dataGrid, FilterCriterias filterCriterias, 
			Predicate<object> predicate)
		{
			var pageGridCollectionView = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
			pageGridCollectionView.Filter = filterCriterias.IsEmpty ? null : predicate;
			dataGrid.ItemsSource = pageGridCollectionView;
			pageGridCollectionView.Refresh();
		}

		/// <summary>
		/// Перезаполнение фильтрующего DataGrid и скрытие/отображение соответствующих панелей, 
		/// в зависимости от критериев фильтра.
		/// </summary>
		/// <param name="filtersTable">DataGrid отображения фильтра</param>
		/// <param name="filterCriterias">Критерии фильтрации страницы</param>
		/// <param name="filterDataPanel">Панель содержащая DataGrid отображения фильтра и кнопку удаления</param>
		/// <param name="filterCoverPanel">Панель-заглушка для пользователя с сообщением</param>
		public static void RefillingFilterTableAndShowHidePanel(DataGrid filtersTable,
			FilterCriterias filterCriterias, Panel filterDataPanel, Panel filterCoverPanel)
		{
			const Visibility show = Visibility.Visible;
			const Visibility hide = Visibility.Collapsed;

			// В зависимости от наличия критериев фильтрации, скрываем/отображаем соответсвующие панели
			filterDataPanel.Visibility = filterCriterias.IsEmpty ? hide : show;
			filterCoverPanel.Visibility = filterCriterias.IsEmpty ? show : hide;

			// Перезаполняем DataGrid отображения фильтра
			filtersTable.ItemsSource = filterCriterias.DisplayedDictionary;
			var view = CollectionViewSource.GetDefaultView(filtersTable.ItemsSource);
			view.Refresh();
		}

		/// <summary>
		/// Сервисный метод обработки получения фокуса Grid-обёрткой DataGrid и TextBox поиска: показывает DataGrid
		/// </summary>
		public static void SearchFieldWrapperGrid_OnGotFocusShowTable(object senderIsGrid)
		{
			var searchWrapperGrid = senderIsGrid as Grid;
			if (searchWrapperGrid == null)
			{
				return;
			}
			var searchDataGrid = searchWrapperGrid.Children.OfType<DataGrid>().FirstOrDefault();
			if (searchDataGrid == null)
			{
				return;
			}
			searchDataGrid.Visibility = Visibility.Visible;
		}

		/// <summary>
		/// Сервисный метод обработки получения фокуса ввода Grid-обёрткой DataGrid и TextBox поиска: скрывает DataGrid
		/// </summary>
		public static void SearchFieldWrapperGrid_OnLostFocusHideTable(object senderIsGrid)
		{
			var searchWrapperGrid = senderIsGrid as Grid;
			if (searchWrapperGrid == null)
			{
				return;
			}
			var searchDataGrid = searchWrapperGrid.Children.OfType<DataGrid>().FirstOrDefault();
			if (searchDataGrid == null)
			{
				return;
			}
			searchDataGrid.Visibility = Visibility.Collapsed;
		}

		/// <summary>
		/// Запрос подтверждения закрытия приложения, и закрытие в случае положительного выбора
		/// </summary>
		public static void ConfirmCloseApplication()
		{
			const MessageBoxButton messageBoxButtons = MessageBoxButton.YesNo;
			const MessageBoxImage messageBoxType = MessageBoxImage.Question;
			const MessageBoxResult defaultButton = MessageBoxResult.No;
			const string confirmExit = PageLiterals.ConfirmExitMessage;
			const string headerConfirm = PageLiterals.HeaderConfirm;

			var result = MessageBox.Show(confirmExit, headerConfirm, messageBoxButtons, messageBoxType, defaultButton);
			if (result != MessageBoxResult.Yes)
			{
				return;
			}
			Application.Current.Shutdown();
		}

		/// <summary>
		/// Запрос подтверждения выхода со страницы редактирования к странице списка, если изменения не были сохранены
		/// </summary>
		public static bool ConfirmBackToListWhenFieldChanged()
		{
			const string headerConfirm = PageLiterals.HeaderConfirm;
			var message = PageLiterals.ConfirmBackToListMessage;
			const MessageBoxButton buttons = MessageBoxButton.YesNo;
			const MessageBoxImage messageType = MessageBoxImage.Question;
			const MessageBoxResult defaultButtonFocus = MessageBoxResult.No;

			var choiseResult = MessageBox.Show(message, headerConfirm, buttons, messageType, defaultButtonFocus);
			return choiseResult == MessageBoxResult.Yes;
		}
	}
}
