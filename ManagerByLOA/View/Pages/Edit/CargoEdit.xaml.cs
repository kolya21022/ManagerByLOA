using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.Services;
using ManagerByLetterOfAttorney.View.Util;
using ManagerByLetterOfAttorney.Entities.Internal;
using ManagerByLetterOfAttorney.View.Pages.TableView;

namespace ManagerByLetterOfAttorney.View.Pages.Edit
{
	/// <summary>
	/// Страница редактирования/добавления [Груза в ТМЦ]
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class CargoEdit : IPageable
	{
		/// <summary>
		/// Текущий груз ТМЦ в режиме редактирования
		/// </summary>
		private readonly Cargo _editedCargo;

		/// <summary>
		/// Локальное хранилище списка для поиска [Единиц измерения].
		/// (загружается при создании страницы и служит неизменяемым источником данных при фильтрации)
		/// </summary>
		private List<Measure> _searchMeasuresStorage;

		/// <summary>
		/// Конструктор режима добавления
		/// </summary>
		/// <inheritdoc />
		public CargoEdit()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Конструктор режима редактирования
		/// </summary>
		/// <inheritdoc />
		public CargoEdit(Cargo editedCargo)
		{
			_editedCargo = editedCargo;
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Загрузка списка ед.измерения из БД, указание значений полей при редактировании и 
		/// указание соответсвующих режиму надписей.
		/// </summary>
		/// <inheritdoc />
		public void AdditionalInitializeComponent()
		{
			try
			{
				_searchMeasuresStorage = MeasuresService.GetAll();
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
			}
			SearchMeasureDataGrid.ItemsSource = _searchMeasuresStorage;
			if (_editedCargo == null)
			{
				// Режим добавления
				EditingTypeLabel.Content = PageLiterals.EditPageTypeAdd;
			}
			else
			{
				// Режим редактирования
				EditingTypeLabel.Content = PageLiterals.EditPageTypeEdit;
				NameTextBox.Text = _editedCargo.Name;
				MeasureTextBox.Text = _editedCargo.Measure == null ? string.Empty : _editedCargo.Measure.Name;
			}
			NameTextBox.Focus();
		}

		/// <summary>
		/// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
		/// </summary>
		/// <inheritdoc />
		public void VisualInitializeComponent()
		{
			FontSize = Constants.FontSize;
			FieldsWrapperGrid.Background = Constants.BackColor1_AthensGray;

			// Заголовок страницы
			TitlePageGrid.Background = Constants.BackColor4_BlueBayoux;
			var titleLabels = TitlePageGrid.Children.OfType<Label>();
			foreach (var titleLabel in titleLabels)
			{
				titleLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}
		}

		/// <summary>
		/// Горячие клавиши текущей страницы
		/// </summary>
		/// <inheritdoc />
		public string PageHotkeys()
		{
			const string jumpNext = PageLiterals.HotkeyLabelJumpNext;
			const string closePageBackToList = PageLiterals.HotkeyLabelClosePageBackToList;
			const string separator = PageLiterals.HotkeyLabelsSeparator;
			const string displayed = jumpNext + separator + closePageBackToList;
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
			ConfirmExitIfDataHasBeenChanged(); // Если нажат [Esc] - проверка изменений полей и запрос подтверждения
		}

		/// <summary>
		/// Перевод клавиатурного фокуса к следующему полю ввода при нажатии Enter.
		/// </summary>
		private void JumpToNextWhenPressEnter_OnKeyDown(object senderIsTextBox, KeyEventArgs eventArgs)
		{
			PageUtil.JumpToNextWhenPressEnter_OnKeyDown(eventArgs);
		}

		/// <summary>
		/// Перевод клавиатурного фокуса к кнопке сохранения из контролов поиска [Единиц измерения]
		/// </summary>
		private void JumpToSaveButton()
		{
			SaveButton.Focus();
		}

		/// <summary>
		/// Нажатие кнопки [Сохранить]
		/// </summary>
		private void SaveButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			SaveAndExit();
		}

		/// <summary>
		/// Нажатие кнопки [Отмена (Выйти к списку)]
		/// </summary>
		private void CancelButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			ConfirmExitIfDataHasBeenChanged(); // проверка изменений полей и запрос подтверждения выхода, если изменены
		}

		/// <summary>
		/// Валидация, сохранение и выход к списку сущностей
		/// </summary>
		private void SaveAndExit()
		{
			// Получение значений и подписей полей
			var name = NameTextBox.Text.Trim();
			var fieldMeasure = MeasureLabel.Content.ToString();
			var measureName = MeasureTextBox.Text.Trim();

			// Валидация полей
			var isValidFields = IsValidFieldsWithShowMessageOtherwise();
			if (!isValidFields)
			{
				return;
			}
			// Если единица измерения указана и содержит некиррилические символы - получение подтверждение пользователя
			var isEmptyMeasureName = string.IsNullOrWhiteSpace(measureName);
			if (!isEmptyMeasureName && !Validator.IsCyrillicWithUserConfirmOtherwise(measureName, fieldMeasure))
			{
				return;
			}
			try
			{
				// Если режим добавления - вставка в БД, если редактирования - обновление в БД
				var cargo = _editedCargo == null 
					? CargoesService.Insert(name, measureName) 
					: CargoesService.Update(_editedCargo.Id, name, measureName);
				PageSwitcher.Switch(new CargoesTable(cargo));
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
			}
		}

		/// <summary>
		/// Валидация (проверка корректности) значений полей страницы, и вывод сообщения при некорректности
		/// </summary>
		private bool IsValidFieldsWithShowMessageOtherwise()
		{
			var name = NameTextBox.Text.Trim();
			var fieldName = NameLabel.Content.ToString();
			var measure = MeasureTextBox.Text.Trim();
			var fieldMeasure = MeasureLabel.Content.ToString();

			var isValid = true;
			var errorMessages = new StringBuilder();
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(name, fieldName, 150, errorMessages);
			isValid &= Validator.IsLineMightEmptyAndSizeNoMore(measure, fieldMeasure, 150, errorMessages);
			if (isValid)
			{
				return true;
			}
			const MessageBoxImage messageType = MessageBoxImage.Error;
			const MessageBoxButton messageButtons = MessageBoxButton.OK;
			const string validationHeader = PageLiterals.HeaderValidation;
			MessageBox.Show(errorMessages.ToString(), validationHeader, messageButtons, messageType);

			return false;
		}

		/// <summary>
		/// Проверка, изменились ли поля ввода, и запрос подтверждения, если изменились. Далее выход к списку сущностей
		/// </summary>
		private void ConfirmExitIfDataHasBeenChanged()
		{
			var isFieldsNotChanged = true;
			if (_editedCargo == null)		// Если сущность добавляется
			{
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(NameTextBox.Text); 
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(MeasureTextBox.Text); 
			}
			else							// Если сущность редактируется
			{
				isFieldsNotChanged &= Equals(NameTextBox.Text.Trim(), _editedCargo.Name);
				isFieldsNotChanged &= Equals(MeasureTextBox.Text.Trim(), 
					_editedCargo.Measure == null ? string.Empty : _editedCargo.Measure.Name);
			}
			// Если введённые поля изменились - запрос у пользователя подтверждения намерение выхода к списку сущностей
			if (!isFieldsNotChanged && !PageUtil.ConfirmBackToListWhenFieldChanged())
			{
				return;
			}
			PageSwitcher.Switch(new CargoesTable(_editedCargo));
		}

		/// <summary>
		/// Событие получения фокуса Grid-обёрткой DataGrid и TextBox поиска [Ед. измерения]: отображает DataGrid
		/// </summary>
		private void SearchFieldWrapperGrid_OnGotFocus(object senderIsGrid, RoutedEventArgs eventArgs)
		{
			PageUtil.SearchFieldWrapperGrid_OnGotFocusShowTable(senderIsGrid);
		}

		/// <summary>
		/// Событие утери фокуса Grid-обёрткой DataGrid и TextBox поиска [Ед. измерения]: скрывает DataGrid
		/// </summary>
		private void SearchFieldWrapperGrid_OnLostFocus(object senderIsGrid, RoutedEventArgs eventArgs)
		{
			PageUtil.SearchFieldWrapperGrid_OnLostFocusHideTable(senderIsGrid);
		}

		/// <summary>
		/// Обработка нажатия клавиш [Enter] и [Up] в DataGrid поиска сущностей
		/// </summary>
		private void SearchDataGrid_OnPreviewKeyDown(object senderIsDataGrid, KeyEventArgs eventArgs)
		{
			const int startOfListIndex = 0;
			// DataGrid поиска сущности
			var searchDataGrid = senderIsDataGrid as DataGrid;
			if (searchDataGrid == null)
			{
				return;
			}
			// Grid-обёртка DataGrid и TextBox поиска
			var searchWrapperGrid = VisualTreeHelper.GetParent(searchDataGrid) as Grid;
			if (searchWrapperGrid == null)
			{
				return;
			}
			// TextBox поиска/добавления
			var searchTextBox = searchWrapperGrid.Children.OfType<TextBox>().FirstOrDefault();
			if (searchTextBox == null)
			{
				return;
			}

			// Если фокус ввода на первой записи DataGrid и нажата [Up] - перевод клавиатурного фокуса ввода к TextBox
			if (startOfListIndex == searchDataGrid.SelectedIndex && eventArgs.Key == Key.Up)
			{
				searchTextBox.Focus();
			}

			// Если записей не 0 и нажат [Enter] - заносим текст объекта в TextBox и переводим фокус к след. контролу
			else if (searchDataGrid.Items.Count > 0 && eventArgs.Key == Key.Enter)
			{
				// Выбранная строка (объект) DataGrid поиска сущности
				var rawSelectedItem = searchDataGrid.SelectedItem;
				if (rawSelectedItem == null)
				{
					return;
				}
				string displayed;
				var selectedItemType = rawSelectedItem.GetType();
				if (selectedItemType == typeof(Measure))
				{
					var selectedItem = (Measure)rawSelectedItem;
					displayed = selectedItem.ServiceSearchDisplayed;
				}
				else
				{
					displayed = rawSelectedItem.ToString();
				}
				// Вывод выбраного значения в TextBox поиска/добавления
				searchTextBox.Text = displayed;
				
				// Нужно из-за того, что FocusNavigationDirection.Down определяет следующей Cancel, а не Save
				if (searchDataGrid.Equals(SearchMeasureDataGrid))
				{
					eventArgs.Handled = true;
					// Скрытие DataGrid поиска сущности (правка дефекта, когда DataGrid не скрывался при потере фокуса)
					searchDataGrid.Visibility = Visibility.Collapsed;
					JumpToSaveButton();
				}
				//else // Перемещение фокуса ввода на след. контрол после поискового DataGrid
				//{
				//	var request = new TraversalRequest(FocusNavigationDirection.Down)
				//	{
				//		Wrapped = false
				//	};
				//	eventArgs.Handled = true;
				//	if (dataGrid.MoveFocus(request))
				//	{
				//		// Скрытие текущего DataGrid (правка дефекта, когда DataGrid не скрывался при потере фокуса)
				//		dataGrid.Visibility = Visibility.Collapsed;
				//	}
				//}
			}
		}

		/// <summary>
		/// Обработка нажатия мышки на строку DataGrid поиска сущностей
		/// </summary>
		private void SearchDataGrid_OnPreviewMouseDown(object senderIsDataGrid, MouseButtonEventArgs eventArgs)
		{
			// DataGrid поиска сущности
			var searchDataGrid = senderIsDataGrid as DataGrid;
			if (searchDataGrid == null)
			{
				return;
			}
			// Grid-обёртка DataGrid и TextBox поиска
			var searchWrapperGrid = VisualTreeHelper.GetParent(searchDataGrid) as Grid;
			if (searchWrapperGrid == null)
			{
				return;
			}
			// TextBox поиска/добавления
			var searchTextBox = searchWrapperGrid.Children.OfType<TextBox>().FirstOrDefault();
			if (searchTextBox == null)
			{
				return;
			}
			// Выбранная строка (объект) DataGrid поиска сущности
			var rawSelectedItem = searchDataGrid.SelectedItem;
			if (rawSelectedItem == null)
			{
				return;
			}
			string displayed;
			var selectedItemType = rawSelectedItem.GetType();
			if (selectedItemType == typeof(Measure))
			{
				var selectedItem = (Measure)rawSelectedItem;
				displayed = selectedItem.ServiceSearchDisplayed;
			}
			else
			{
				displayed = rawSelectedItem.ToString();
			}
			// Вывод выбраного значения в TextBox поиска/добавления
			searchTextBox.Text = displayed;

			// Нужно из-за того, что FocusNavigationDirection.Down определяет следующей Cancel, а не Save
			if (searchDataGrid.Equals(SearchMeasureDataGrid))
			{
				eventArgs.Handled = true;
				// Скрытие DataGrid поиска сущности (правка дефекта, когда DataGrid не скрывался при потере фокуса)
				searchDataGrid.Visibility = Visibility.Collapsed;
				JumpToSaveButton();
			}
			//else // Перемещение фокуса ввода на след. контрол после DataGrid поиска сущности
			//{
			//	var nextControlAfterDataGrid = dataGrid.PredictFocus(FocusNavigationDirection.Down) as Control;
			//	if (nextControlAfterDataGrid == null)
			//	{
			//		return;
			//	}
			//	eventArgs.Handled = true;
			//	nextControlAfterDataGrid.Focus();
			//}
		}

		/// <summary>
		/// Обработка события PreviewKeyDown в TextBox поиска/добавления сущности
		/// </summary>
		private void SearchTextBox_OnPreviewKeyDown(object senderIsTextBox, KeyEventArgs eventArgs)
		{
			// TextBox поиска/добавления
			var searchTextBox = senderIsTextBox as TextBox;
			if (searchTextBox == null)
			{
				return;
			}
			// Grid-обёртка DataGrid и TextBox поиска
			var searchWrapperGrid = VisualTreeHelper.GetParent(searchTextBox) as Grid;
			if (searchWrapperGrid == null)
			{
				return;
			}
			// DataGrid поиска сущности
			var searchDataGrid = searchWrapperGrid.Children.OfType<DataGrid>().FirstOrDefault();
			if (searchDataGrid == null)
			{
				return;
			}

			// Если нажата кнопка [Down] - перемещение клавиатурного фокуса на первую строку DataGrid поиска сущности
			if (eventArgs.Key == Key.Down)
			{
				if (searchDataGrid.Items.Count <= 0)
				{
					return;
				}
				searchDataGrid.SelectedIndex = 0;

				// NOTE: эта копипаста ниже не случайна, нужный функционал срабатывает только со второго раза.
				// Решение в указаном ответе: https://stackoverflow.com/a/27792628 Работает, не трогай
				var row = (DataGridRow)searchDataGrid.ItemContainerGenerator.ContainerFromIndex(0);
				if (row == null)
				{
					searchDataGrid.UpdateLayout();
					searchDataGrid.ScrollIntoView(searchDataGrid.Items[0]);
					row = (DataGridRow)searchDataGrid.ItemContainerGenerator.ContainerFromIndex(0);
				}
				if (row != null)
				{
					row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
				}
			}

			// Если нажат [Enter] - перемещение клавиатурного фокуса на контрол, после DataGrid поиска сущности
			else if (eventArgs.Key == Key.Enter)
			{
				// Нужно из-за того, что FocusNavigationDirection.Down определяет следующей Cancel, а не Save
				if (searchDataGrid.Equals(SearchMeasureDataGrid))
				{
					eventArgs.Handled = true;
					searchDataGrid.Visibility = Visibility.Collapsed;
					JumpToSaveButton();
				}
				//else // Перемещение фокуса ввода на след. контрол после DataGrid поиска сущности
				//{
				//	var nextControlAfterDataGrid = searchDataGrid.PredictFocus(FocusNavigationDirection.Down) as Control;
				//	if (nextControlAfterDataGrid == null)
				//	{
				//		return;
				//	}
				//	eventArgs.Handled = true;
				//	nextControlAfterDataGrid.Focus();
				//}
			}
		}

		/// <summary>
		/// Обработка события изменения текста в TextBox поиска/добаления [Единиц измерения].
		/// (Перезаполнение DataGrid поиска сущности с учётом введённого текста)
		/// </summary>
		private void MeasureTextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs eventArgs)
		{
			// TextBox поиска/добавления
			var searchTextBox = senderIsTextBox as TextBox;
			if (searchTextBox == null)
			{
				return;
			}
			// Grid-обёртка DataGrid и TextBox поиска
			var searchWrapperGrid = VisualTreeHelper.GetParent(searchTextBox) as Grid;
			if (searchWrapperGrid == null)
			{
				return;
			}
			// DataGrid поиска сущности
			var searchDataGrid = searchWrapperGrid.Children.OfType<DataGrid>().FirstOrDefault();
			if (searchDataGrid == null)
			{
				return;
			}

			// Разделение искомой строки в массив по пробелам
			const StringComparison comparisonOrdinalIgnoreCase = StringComparison.OrdinalIgnoreCase;
			var searchValues = searchTextBox.Text.Trim().Split(null);
			var searchResultList = new List<Measure>();
			foreach (var measure in _searchMeasuresStorage)
			{
				// Поиск совпадений всех значений массива по требуемым полям сущности
				var isCoincided = true;
				foreach (var searchValue in searchValues)
				{
					var measureName = measure.Name ?? string.Empty;
					isCoincided &= measureName.IndexOf(searchValue, comparisonOrdinalIgnoreCase) >= 0;
				}
				if (isCoincided)
				{
					searchResultList.Add(measure);
				}
			}
			// Перезаполнение DataGrid поиска сущности с учётом найденых значений
			searchDataGrid.ItemsSource = null;
			searchDataGrid.ItemsSource = searchResultList;
		}
	}
}
