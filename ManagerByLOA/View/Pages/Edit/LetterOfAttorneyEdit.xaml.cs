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
using ManagerByLetterOfAttorney.View.Menus;
using ManagerByLetterOfAttorney.View.Windows;
using ManagerByLetterOfAttorney.Entities.External;
using ManagerByLetterOfAttorney.Entities.Internal;
using ManagerByLetterOfAttorney.View.Pages.Reports;
using ManagerByLetterOfAttorney.View.Pages.TableView;

namespace ManagerByLetterOfAttorney.View.Pages.Edit
{
	/// <summary>
	/// Страница редактирования/добавления [Доверенности]
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class LetterOfAttorneyEdit : IPageable
	{
		/// <summary>
		/// Порядковый номер доверенности.
		/// (При добавлении: полученый максимальный текущий из БД + 1, при редактировании - номер переданного объекта)
		/// </summary>
		private long? _ordinalNumberOfLoa;

		/// <summary>
		/// Текущая доверенность в режиме редактирования.
		/// </summary>
		private readonly LetterOfAttorney _editedLoa;

		/// <summary>
		/// Редактируемый список перевозимых ТМЦ по текущей доверенности
		/// </summary>
		private readonly List<Shipment> _newShipments = new List<Shipment>();

		/// <summary>
		/// Локальное хранилище списка для поиска [Организаций].
		/// (загружается при создании страницы и служит неизменяемым источником данных при фильтрации)
		/// </summary>
		private List<Company> _searchCompaniesStorage = new List<Company>();

		/// <summary>
		/// Локальное хранилище списка для поиска [Единиц измерения].
		/// (загружается при создании страницы и служит неизменяемым источником данных при фильтрации)
		/// </summary>
		private List<Measure> _searchMeasuresStorage = new List<Measure>();

		/// <summary>
		/// Локальное хранилище списка для поиска [Грузов ТМЦ].
		/// (загружается при создании страницы и служит неизменяемым источником данных при фильтрации)
		/// </summary>
		private List<Cargo> _searchCargoesStorage = new List<Cargo>();

		/// <summary>
		/// Ранее выбранный курьер при [продолжении ввода] ещё 1 доверенности из страницы печати
		/// </summary>
		private readonly Courier _newLoaWithSelectedCourier;

		/// <summary>
		/// Ранее выбранный сотрудник при [продолжении ввода] ещё 1 доверенности из страницы печати
		/// </summary>
		private readonly Employee _newLoaWithSelectedEmployee;

		/// <summary>
		/// Конструктор режима добавления
		/// </summary>
		/// <inheritdoc />
		public LetterOfAttorneyEdit()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Конструктор режима добавления из страницы печати (Новая доверенность указанному [курьеру])
		/// </summary>
		/// <inheritdoc />
		public LetterOfAttorneyEdit(Courier courier)
		{
			_newLoaWithSelectedCourier = courier;
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Конструктор режима добавления из страницы печати (Новая доверенность указанному [сотруднику])
		/// </summary>
		/// <inheritdoc />
		public LetterOfAttorneyEdit(Employee employee)
		{
			_newLoaWithSelectedEmployee = employee;
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Конструктор режима редактирования
		/// </summary>
		/// <inheritdoc />
		public LetterOfAttorneyEdit(LetterOfAttorney editedLoa)
		{
			_editedLoa = editedLoa;
			// Клонирование оригинального списка ТМЦ
			_newShipments = editedLoa.Shipments.SerializableListClone();
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Инициализация данными полей формы, загрузка соответсвующих списков из БД в локальные хранилища,
		/// заполнение таблицы перевозимых ТМЦ (в случае редактирования)
		/// </summary>
		/// <inheritdoc />
		public void AdditionalInitializeComponent()
		{
			// Загрузка списков и списков локальных хранилищ из БД
			List<Employee> employees;
			List<Courier> couriers;
			try
			{
				couriers = CouriersService.GetAll();
				const bool withoutDismissed = false;   // не выбирать уволенных сотрудников
				employees = EmployeesService.GetAll(withoutDismissed);
				_searchCompaniesStorage = CompaniesService.GetAllInternalAndExternal();
				_searchMeasuresStorage = MeasuresService.GetAll();
				_searchCargoesStorage = CargoesService.GetAll();
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
				return;
			}

			// Критерии сортировок указаны в реализации интерфейса IComparable классов сущностей
			if (employees != null && employees.Count > 0)
			{
				employees.Sort();
			}
			if (couriers != null && couriers.Count > 0)
			{
				couriers.Sort();
			}
			if (_searchCompaniesStorage != null && _searchCompaniesStorage.Count > 0)
			{
				_searchCompaniesStorage.Sort();
			}
			if (_searchCargoesStorage != null && _searchCargoesStorage.Count > 0)
			{
				_searchCargoesStorage.Sort();
			}
			if (_searchMeasuresStorage != null && _searchMeasuresStorage.Count > 0)
			{
				_searchMeasuresStorage.Sort();
			}

			// Заполнение ComboBox'ов и DataGrid'ов поиска сущностей списками
			EmployeeNamesComboBox.ItemsSource = employees;
			CourierNamesComboBox.ItemsSource = couriers;

			SearchCompaniesDataGrid.Items.Clear();
			SearchCompaniesDataGrid.ItemsSource = _searchCompaniesStorage;
			SearchMeasuresDataGrid.Items.Clear();
			SearchMeasuresDataGrid.ItemsSource = _searchMeasuresStorage;
			SearchCargoesDataGrid.Items.Clear();
			SearchCargoesDataGrid.ItemsSource = _searchCargoesStorage;

			// Режим редактирования
			if (_editedLoa != null)
			{
				EditingTypeLabel.Content = PageLiterals.EditPageTypeEdit;
				ShowReportButton.Visibility = Visibility.Visible;          // Видимость кнопки печати доверенности

				// Порядковый номер редактируемой доверенности в текущем году
				_ordinalNumberOfLoa = _editedLoa.OrdinalNumber;
				OrdinalNumberLabel.Content = _editedLoa.OrdinalNumber.ToString();

				// Тип перевозящего груз - сотрудник 
				if (_editedLoa.Courier != null && _editedLoa.Courier.GetType() == typeof(Employee))
				{
					IsEmployeeRadioButton.IsChecked = true;
					EmployeeNamesComboBox.SelectedItem = (Employee)_editedLoa.Courier;
				}
				// Тип перевозящего груз - курьер
				else if (_editedLoa.Courier != null && _editedLoa.Courier.GetType() == typeof(Courier))
				{
					CourierNamesComboBox.SelectedItem = _editedLoa.Courier;
					IsCourierRadioButton.IsChecked = true;
				}
				else
				{
					IsEmployeeRadioButton.IsChecked = true;
				}

				CompanyTextBox.Text = _editedLoa.Company.Name;
				ValidityDateStartDatePicker.SelectedDate = _editedLoa.ValidityDateStart;
				ValidityDateEndDatePicker.SelectedDate = _editedLoa.ValidityDateEnd;

				// Запрет редактирования дат
				ValidityDateStartDatePicker.IsEnabled = false;
				ValidityDateEndDatePicker.IsEnabled = false;

				ReloadShipmentsTable();
			}
			// Режим добавления
			else
			{
				EditingTypeLabel.Content = PageLiterals.EditPageTypeAdd;
				ShowReportButton.Visibility = Visibility.Collapsed;        // Невидимость кнопки печати доверенности
				try
				{
					// Новый порядковый номер в текущем году
					_ordinalNumberOfLoa = LetterOfAttorneysService.NextFreeOrdinalNumber();
				}
				catch (StorageException ex)
				{
					Common.ShowDetailExceptionMessage(ex);
					return;
				}
				OrdinalNumberLabel.Content = _ordinalNumberOfLoa;

				// Срок действия доверенности по-умолчанию в днях, начиная с текущего
				var defaultCountDaysValidity = Properties.Settings.Default.DefaultLoaCountDaysValidity;
				ValidityDateStartDatePicker.SelectedDate = DateTime.Today;
				ValidityDateEndDatePicker.SelectedDate = DateTime.Today.AddDays(defaultCountDaysValidity);

				// Ранее выбранный сотрудник или курьер при [продолжении ввода] ещё 1 доверенности из страницы печати
				if (_newLoaWithSelectedEmployee != null)
				{
					IsEmployeeRadioButton.IsChecked = true;
					EmployeeNamesComboBox.SelectedItem = _newLoaWithSelectedEmployee;
				}
				else if (_newLoaWithSelectedCourier != null)
				{
					CourierNamesComboBox.SelectedItem = _newLoaWithSelectedCourier;
					IsCourierRadioButton.IsChecked = true;
				}
				else
				{
					IsEmployeeRadioButton.IsChecked = true;
				}
			}
		}

		/// <summary>
		/// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
		/// </summary>
		/// <inheritdoc />
		public void VisualInitializeComponent()
		{
			FontSize = Constants.FontSize;
			FieldsWrapperGrid.Background = Constants.BackColor1_AthensGray;
			CargoesWrapperRectangle.Fill = Constants.BackColor4_BlueBayoux;
			CourierTypeWrapperRectangle.Fill = Constants.BackColor2_Botticelli;
			OrdinalNumberWrapperRectangle.Fill = Constants.BackColor2_Botticelli;
			CargoLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			CountLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			MeasureLabel.Foreground = Constants.ForeColor2_PapayaWhip;

			// Заголовок страницы
			TitlePageGrid.Background = Constants.BackColor4_BlueBayoux;
			var titleLabels = TitlePageGrid.Children.OfType<Label>();
			foreach (var titleLabel in titleLabels)
			{
				titleLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}
			
			// Цвета фона и текста панели сотрудника
			EmployeeWrapperGrid.Background = Constants.BackColor4_BlueBayoux;
			var employeeWrapperLabels = EmployeeWrapperGrid.Children.OfType<Label>();
			foreach (var employeeWrapperLabel in employeeWrapperLabels)
			{
				employeeWrapperLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}
			var employeeWrapperValuesLabels = new[]
			{
				EmployeePersonnelNumberLabel,
				EmployeePassportSeriesAndNumberLabel,
				EmployeePassportIssueDateLabel,
				EmployeePassportIssuedByOrganizationLabel,
				EmployeeProfessionLabel
			};
			foreach (var employeeWrapperValueLabel in employeeWrapperValuesLabels)
			{
				employeeWrapperValueLabel.Background = Constants.BackColor5_WaikawaGray;
			}

			// Цвета фона и текста панели курьера
			CourierWrapperGrid.Background = Constants.BackColor6_Lochmara;
			var courierWrapperLabels = CourierWrapperGrid.Children.OfType<Label>();
			foreach (var courierWrapperLabel in courierWrapperLabels)
			{
				courierWrapperLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}
			var courierWrapperValuesLabels = new[]
			{
				CourierPassportSeriesAndNumberLabel,
				CourierPassportIssueDateLabel,
				CourierPassportIssuedByOrganizationLabel,
				CourierProfessionLabel
			};
			foreach (var courierWrapperValueLabel in courierWrapperValuesLabels)
			{
				courierWrapperValueLabel.Background = Constants.BackColor7_BahamaBlue;
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
		/// Изменение выбраной записи списка сотрудников и заполнение соответсвующих полей страницы
		/// </summary>
		private void EmployeesComboBox_OnSelectionChanged(object senderIsComboBox, SelectionChangedEventArgs eventArgs)
		{
			var comboBox = senderIsComboBox as ComboBox;
			var employee = comboBox != null && comboBox.SelectedItem is Employee 
				? (Employee)comboBox.SelectedItem
				: null;
			var personnelNumber = string.Empty;
			var profession = string.Empty;
			var passportSeriesAndNumber = string.Empty;
			var passportIssuedByOrganization = string.Empty;
			DateTime? passportIssueDate = null;
			if (employee != null)
			{
				personnelNumber = employee.PersonnelNumber.ToString();
				profession = employee.Profession;
				passportSeriesAndNumber = employee.PassportSeriesAndNumber;
				passportIssuedByOrganization = employee.PassportIssuedByOrganization;
				passportIssueDate = employee.PassportIssueDate;
			}

			EmployeePersonnelNumberLabel.Content = personnelNumber;
			EmployeeProfessionLabel.Content = profession;
			EmployeePassportSeriesAndNumberLabel.Content = passportSeriesAndNumber;
			EmployeePassportIssuedByOrganizationLabel.Content = passportIssuedByOrganization;
			EmployeePassportIssueDateLabel.Content = passportIssueDate;
		}

		/// <summary>
		/// Изменение выбраной записи списка курьеров и заполнение соответсвующих полей страницы
		/// </summary>
		private void CourierNamesComboBox_OnSelectionChanged(object senderIsComboBox, SelectionChangedEventArgs evArgs)
		{
			var comboBox = senderIsComboBox as ComboBox;
			var courier = comboBox != null && comboBox.SelectedItem is Courier
				? (Courier)comboBox.SelectedItem
				: null;
			var profession = string.Empty;
			var passportSeriesAndNumber = string.Empty;
			var passportIssuedByOrganization = string.Empty;
			DateTime? passportIssueDate = null;
			if (courier != null)
			{
				profession = courier.Profession;
				passportSeriesAndNumber = courier.PassportSeriesAndNumber;
				passportIssuedByOrganization = courier.PassportIssuedByOrganization;
				passportIssueDate = courier.PassportIssueDate;
			}

			CourierProfessionLabel.Content = profession;
			CourierPassportSeriesAndNumberLabel.Content = passportSeriesAndNumber;
			CourierPassportIssuedByOrganizationLabel.Content = passportIssuedByOrganization;
			CourierPassportIssueDateLabel.Content = passportIssueDate;
		}

		/// <summary>
		/// Нажатие кнопки [Добавить в список] на панели ТМЦ - добавление введённой ТМЦ в список ТМЦ этой деверенности
		/// </summary>
		private void AddShipmentButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			// Получение значений и подписей полей
			var cargo = CargoTextBox.Text.Trim();
			var fieldCargo = CargoLabel.Content.ToString();
			var measure = MeasureTextBox.Text.Trim();
			var fieldMeasure = MeasureLabel.Content.ToString();
			var count = CountInventoryDoubleUpDown.Value;

			// Валидация полей
			if (!IsValidShipmentWithShowMessageOtherwise(cargo, fieldCargo, measure, fieldMeasure))
			{
				CargoTextBox.Focus();
				return;
			}
			// Запрос подтверждения, если единица измерения есть и не содержит не-кириллицу
			if (!string.IsNullOrWhiteSpace(measure) && 
				!Validator.IsCyrillicWithUserConfirmOtherwise(measure, fieldMeasure))
			{
				return;
			}

			// Создание перевозимой ТМЦ, добавление в список и перезагрузка таблицы
			var nullableMeasure = string.IsNullOrWhiteSpace(measure) ? null : new Measure { Name = measure };
			var shipment = new Shipment
			{
				Count = count,
				Cargo = new Cargo { Name = cargo, Measure = nullableMeasure }
			};
			_newShipments.Add(shipment);
			ReloadShipmentsTable();

			// Очистка полей и фокус ввода на ТМЦ
			CargoTextBox.Clear();
			MeasureTextBox.Clear();
			CountInventoryDoubleUpDown.Value = null;
			CargoTextBox.Focus();
		}

		/// <summary>
		/// Валидация перевозимой ТМЦ при добавлении в список, и вывод сообщения при некорректности
		/// </summary>
		private static bool IsValidShipmentWithShowMessageOtherwise(string cargo, string fieldCargo, 
			string measure, string fieldMeasure)
		{
			var isValid = true;
			var errorMessages = new StringBuilder();
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(cargo, fieldCargo, 150, errorMessages);
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
		/// Валидация полей доверенности, и вывод сообщения при некорректности
		/// </summary>
		private bool IsValidLetterOfAttorneyWithShowMessageOtherwise()
		{
			var validityDateStart = ValidityDateStartDatePicker.SelectedDate;
			var fieldValidityDateStart = ValidityDateStartLabel.Content.ToString();

			var validityDateEnd = ValidityDateEndDatePicker.SelectedDate;
			var fieldValidityDateEnd = ValidityDateEndLabel.Content.ToString();
			
			var company = CompanyTextBox.Text;
			var fieldCompany = CompaniesLabel.Content.ToString();

			var courierOrEmployee = IsCourierRadioButton.IsChecked == true 
				? CourierNamesComboBox.SelectedItem as Courier
				: IsEmployeeRadioButton.IsChecked == true ? EmployeeNamesComboBox.SelectedItem as Employee : null;
			var fieldCourierOrEmployee = IsCourierRadioButton.IsChecked == true
				? CourierNamesLabel.Content.ToString()
				: EmployeeNameLabel.Content.ToString();

			var isValid = true;
			var errorMessages = new StringBuilder();
			isValid &= Validator.IsNotNullSelectedObject(courierOrEmployee, fieldCourierOrEmployee, errorMessages);
			isValid &= Validator.IsNotNullSelectedObject(validityDateStart, fieldValidityDateStart, errorMessages);
			isValid &= Validator.IsNotNullSelectedObject(validityDateEnd, fieldValidityDateEnd, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(company, fieldCompany, 150, errorMessages);
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
		/// Нажатие кнопки [Назад к списку]
		/// </summary>
		private void BackToListButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			eventArgs.Handled = true;
			ConfirmExitIfDataHasBeenChanged(); // Если нажат [Esc] - проверка изменений полей и запрос подтверждения
		}

		/// <summary>
		/// Нажатие кнопки [Сохранить]
		/// </summary>
		private void SaveButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			const string validationHeader = PageLiterals.HeaderValidation;
			const MessageBoxButton messageButtons = MessageBoxButton.OK;
			const MessageBoxImage messageType = MessageBoxImage.Error;

			// Проверка наличия хоть одного перевозимого ТЦМ в списке 
			if (_newShipments.Count == 0)
			{
				const string isAbsentShipmentsMessage = "Не указано ни одной перевозимой ТМЦ";
				MessageBox.Show(isAbsentShipmentsMessage, validationHeader, messageButtons, messageType);
				return;
			}
			// Валидация полей
			if (!IsValidLetterOfAttorneyWithShowMessageOtherwise())
			{
				return;
			}

			var nullableOrdinal = _ordinalNumberOfLoa;
			var nullableValidityDateEnd = ValidityDateEndDatePicker.SelectedDate;
			var nullableValidityDateStart = ValidityDateStartDatePicker.SelectedDate;

			// Такой сценарий не должен происходить благодаря валидации, но статический анализатор IDE ругается 
			if (nullableValidityDateStart == null || nullableValidityDateEnd == null || nullableOrdinal == null)
			{
				const string cause = "Даты начала/конца действия и/или порядковый номер не могут быть не указаны";
				MessageBox.Show(cause, validationHeader, messageButtons, messageType);
				return;
			}
			var ordinal = (long)nullableOrdinal;
			var dateEnd = (DateTime)nullableValidityDateEnd;
			var dateStart = (DateTime)nullableValidityDateStart;

			var company = CompanyTextBox.Text;
			var courierEmployee = IsCourierRadioButton.IsChecked == true 
				? CourierNamesComboBox.SelectedItem as Courier
				: IsEmployeeRadioButton.IsChecked == true ? EmployeeNamesComboBox.SelectedItem as Employee : null;
			var shipments = _newShipments;
			try
			{
				// Если режим добавления - вставка в БД, если редактирования - обновление в БД
				var letterOfAttorney = _editedLoa == null 
					? LetterOfAttorneysService.Insert(ordinal, dateStart, dateEnd, courierEmployee, company, shipments)
					: LetterOfAttorneysService.Update(_editedLoa.Id, ordinal, dateStart, dateEnd, courierEmployee, 
						company, shipments);
				if (_editedLoa == null)
				{
					// Обновление максимального (последнего) порядкового номера доверенности в меню
					SideMenu.UpdatePrintedLoaOrdinalNumber();
				}
				// Открытие новой доверенности в режиме редактирования (для возможности печати)
				PageSwitcher.Switch(new LetterOfAttorneyEdit(letterOfAttorney));
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
			}
		}

		/// <summary>
		/// Перезаполнение таблицы перевозимых ТМЦ из локального поля
		/// </summary>
		private void ReloadShipmentsTable()
		{
			ShipmentsDataGrid.ItemsSource = null;
			ShipmentsDataGrid.ItemsSource = _newShipments;
		}

		/// <summary>
		/// Сервисный метод раскрытия выпадающего списка для события GotFocus для поискового ComboBox.
		/// </summary>
		private void SearchComboBox_OnGotFocus(object senderIsComboBox, RoutedEventArgs eventArgs)
		{
			OnGotFocus(eventArgs);
			PageUtil.Service_SearchComboBox_OnGotFocus(senderIsComboBox);
		}

		/// <summary>
		/// Сервисный метод раскрытия выпадающего списка для события PreviewMouseUp для поискового ComboBox.
		/// </summary>
		private void SearchComboBox_OnPreviewMouseUp(object senderIsComboBox, MouseButtonEventArgs eventArgs)
		{
			OnPreviewMouseUp(eventArgs);
			PageUtil.Service_SearchComboBox_OnPreviewMouseUp(senderIsComboBox);
		}

		/// <summary>
		/// Сервисный метод раскрытия выпадающего списка для события PreviewMouseDown для поискового ComboBox.
		/// </summary>
		private void SearchComboBox_OnPreviewMouseDown(object senderIsComboBox, MouseButtonEventArgs eventArgs)
		{
			OnPreviewMouseDown(eventArgs);
			PageUtil.Service_SearchComboBox_OnPreviewMouseDown(senderIsComboBox);
		}

		/// <summary>
		/// Сервисный метод обработчик для события PreviewMouseDown для поискового ComboBox.
		/// Нажатии [Enter] клавиатурный фокус перемещается на нижележащее поле, иначе раскрытие выпадающего списка.
		/// Нижележащее поле определяется методом FocusNavigationDirection.Down
		/// </summary>
		private void SearchComboBox_OnPreviewKeyDown(object senderIsComboBox, KeyEventArgs eventArgs)
		{
			OnPreviewKeyDown(eventArgs);
			PageUtil.Service_SearchComboBox_OnPreviewKeyDown(senderIsComboBox, eventArgs);
		}

		/// <summary>
		/// Сервисный метод обработчик для события PreviewKeyUp для поискового ComboBox.
		/// Нажатии [Enter] клавиатурный фокус перемещается на нижележащее поле.
		/// Нижележащее поле определяется методом FocusNavigationDirection.Down
		/// </summary>
		private void SearchComboBox_OnPreviewKeyUp(object senderIsComboBox, KeyEventArgs eventArgs)
		{
			OnPreviewKeyUp(eventArgs);
			PageUtil.Service_SearchComboBox_OnPreviewKeyUp(senderIsComboBox, eventArgs);
		}

		/// <summary>
		/// Нажатие кнопки [Печать]
		/// </summary>
		private void ShowReportButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			if (_editedLoa == null)
			{
				const MessageBoxImage messageBoxType = MessageBoxImage.Error;
				const MessageBoxButton messageBoxButtons = MessageBoxButton.OK;
				const string cause = "Попытка печати ещё не существующей доверенности";
				MessageBox.Show(cause, PageLiterals.HeaderLogicError, messageBoxButtons, messageBoxType);
				return;
			}

			// Сценарий, при котором в поля страницы внесли изменения, но не сохранили в БД: запрос подтвержения
			var isFieldsNotChanged = IsPageFieldsNotCanged();
			if (!isFieldsNotChanged)
			{
				var message = PageLiterals.ConfirmPrintWithoutSaveMessage;
				const MessageBoxImage type = MessageBoxImage.Question;
				const MessageBoxButton buttons = MessageBoxButton.YesNo;
				const MessageBoxResult defaultButton = MessageBoxResult.No;
				var choiseResult = MessageBox.Show(message, PageLiterals.HeaderConfirm, buttons, type, defaultButton);
				if (choiseResult != MessageBoxResult.Yes)
				{
					return;
				}
			}
			PageSwitcher.Switch(new LoaReportPage(_editedLoa));
		}

		/// <summary>
		/// Удаление перевозимой ТМЦ из списка
		/// </summary>
		private void DeleteButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			var frameworkElement = senderIsButton as FrameworkElement;
			if (frameworkElement == null)
			{
				return;
			}
			var shipment = frameworkElement.DataContext as Shipment;
			if (shipment == null)
			{
				return;
			}
			_newShipments.Remove(shipment);
			ReloadShipmentsTable();
		}

		/// <summary>
		/// Нажатие радио-кнопки категории перевозящего - Сотрудник
		/// </summary>
		private void IsEmployeeRadioButton_OnChecked(object senderIsRadioButton, RoutedEventArgs eventArgs)
		{
			EmployeeWrapperGrid.Visibility = Visibility.Visible;
			CourierWrapperGrid.Visibility = Visibility.Collapsed;
		}

		/// <summary>
		/// Нажатие радио-кнопки категории перевозящего - Курьер
		/// </summary>
		private void IsCourierRadioButton_OnChecked(object senderIsRadioButton, RoutedEventArgs eventArgs)
		{
			EmployeeWrapperGrid.Visibility = Visibility.Collapsed;
			CourierWrapperGrid.Visibility = Visibility.Visible;
		}

		/// <summary>
		/// Нажатие кнопки [Новый] на панели курьера - добавление нового курьера в отдельном окне
		/// </summary>
		private void AddCourierButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			// Отображение дополнительного модального окна добавления курьера
			var courierEditExtraWindow = new CourierEditExtraWindow
			{
				Owner = Window.GetWindow(this)
			};
			courierEditExtraWindow.ShowDialog();
			if (!courierEditExtraWindow.DialogResult.HasValue || !courierEditExtraWindow.DialogResult.Value)
			{
				return;
			}
			// Если ввод нового курьера произошёл - загружаем список из БД, обновляем ComboBox и выбираем введённого
			try
			{
				var selectedCourier = courierEditExtraWindow.GetSavedCourier();
				var couriers = CouriersService.GetAll();
				couriers.Sort();
				CourierNamesComboBox.ItemsSource = couriers;
				CourierNamesComboBox.SelectedItem = selectedCourier;
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
			}
		}

		/// <summary>
		/// Нажатие кнопки [редактирования] на панели курьера - редактирование выбранного в ComboBox курьера
		/// </summary>
		private void EditCourierButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			var courier = CourierNamesComboBox.SelectedItem as Courier;
			if (courier == null)
			{
				const MessageBoxButton buttons = MessageBoxButton.OK;
				const MessageBoxImage type = MessageBoxImage.Information;
				const MessageBoxResult defaultButton = MessageBoxResult.OK;
				const string message = "Для редактирования, курьера нужно сначала выбрать из выпадающего списка";
				MessageBox.Show(message, PageLiterals.HeaderInformationOrWarning, buttons, type, defaultButton);
				return;
			}
			// Отображение дополнительного модального окна редактирования курьера
			var courierEditExtraWindow = new CourierEditExtraWindow(courier)
			{
				Owner = Window.GetWindow(this)
			};
			courierEditExtraWindow.ShowDialog();
			if (!courierEditExtraWindow.DialogResult.HasValue || !courierEditExtraWindow.DialogResult.Value)
			{
				return;
			}
			// Если изменение курьера произошло и изменения сохранены - обновляем список из БД, выбираем введённого
			try
			{
				var selectedCourier = courierEditExtraWindow.GetSavedCourier();
				var couriers = CouriersService.GetAll();
				couriers.Sort();
				CourierNamesComboBox.ItemsSource = couriers;
				CourierNamesComboBox.SelectedItem = selectedCourier;
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
			}
		}

		/// <summary>
		/// Обработка события изменения текста в TextBox поиска/добаления [Организации].
		/// (Перезаполнение DataGrid поиска сущности с учётом введённого текста)
		/// </summary>
		private void CompanyTextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs eventArgs)
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

			// Разделение введенного пользователем текста по пробелам на массив слов
			var searchResult = new List<Company>();
			var searchValues = searchTextBox.Text.Trim().Split(null);
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			foreach (var company in _searchCompaniesStorage)
			{
				// Поиск совпадений всех значений массива по требуемым полям сущности
				var isCoincided = true;
				foreach (var searchValue in searchValues)
				{
					// Если сущность - суперкласс с 1 полем, ищем только по нему
					if (company.GetType() == typeof(Company))
					{
						isCoincided &= company.Name.IndexOf(searchValue, comparisonIgnoreCase) >= 0;
					}
					// Если сущность - подкласс с несколькими полями, ищем по этим нескольким полям
					else if (company.GetType() == typeof(CompanyWithUnp))
					{
						var companyWithUnp = (CompanyWithUnp)company;
						var companyNameWithCity = companyWithUnp.ServiceSearchResultDisplayed;
						var unp = companyWithUnp.Unp ?? string.Empty;
						isCoincided &= companyNameWithCity.IndexOf(searchValue, comparisonIgnoreCase) >= 0
									   || unp.IndexOf(searchValue, comparisonIgnoreCase) >= 0;
					}
				}
				// Если в полях сущности есть введённые слова, добавляем объект в буферный список
				if (isCoincided)
				{
					searchResult.Add(company);
				}
			}
			// Перезаполнение DataGrid поиска сущности с учётом найденых значений
			searchDataGrid.ItemsSource = null;
			searchDataGrid.ItemsSource = searchResult;
		}

		/// <summary>
		/// Обработка события изменения текста в TextBox поиска/добаления [перевозимых грузов ТМЦ].
		/// (Перезаполнение DataGrid поиска сущности с учётом введённого текста)
		/// </summary>
		private void CargoTextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs e)
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

			// Разделение введенного пользователем текста по пробелам на массив слов
			const StringComparison comparisonOrdinalIgnoreCase = StringComparison.OrdinalIgnoreCase;
			var searchValues = searchTextBox.Text.Trim().Split(null);
			var searchResult = new List<Cargo>();
			foreach (var cargo in _searchCargoesStorage)
			{
				// Поиск совпадений всех значений массива по требуемым полям сущности
				var isCoincided = true;
				foreach (var searchValue in searchValues)
				{
					isCoincided &= cargo.Name.IndexOf(searchValue, comparisonOrdinalIgnoreCase) >= 0;
				}
				// Если в полях сущности есть введённые слова, добавляем объект в буферный список
				if (isCoincided)
				{
					searchResult.Add(cargo);
				}
			}
			// Перезаполнение DataGrid поиска сущности с учётом найденых значений
			searchDataGrid.ItemsSource = null;
			searchDataGrid.ItemsSource = searchResult;
		}

		/// <summary>
		/// Обработка события изменения текста в TextBox поиска/добаления [Единиц измерения грузов ТМЦ].
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

			// Разделение введенного пользователем текста по пробелам на массив слов
			const StringComparison comparisonOrdinalIgnoreCase = StringComparison.OrdinalIgnoreCase;
			var searchValues = searchTextBox.Text.Trim().Split(null);
			var searchResult = new List<Measure>();
			foreach (var measure in _searchMeasuresStorage)
			{
				// Поиск совпадений всех значений массива по требуемым полям сущности
				var isCoincided = true;
				foreach (var searchValue in searchValues)
				{
					var name = measure.Name ?? string.Empty;
					isCoincided &= name.IndexOf(searchValue, comparisonOrdinalIgnoreCase) >= 0;
				}
				// Если в полях сущности есть введённые слова, добавляем объект в буферный список
				if (isCoincided)
				{
					searchResult.Add(measure);
				}
			}
			// Перезаполнение DataGrid поиска сущности с учётом найденых значений
			searchDataGrid.ItemsSource = null;
			searchDataGrid.ItemsSource = searchResult;
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
				// Если выбраная строка - перевозимый груз ТМЦ, в поле [Ед. изм] заносится [Единица измерения] этой ТМЦ
				var selectedCargo = searchDataGrid.SelectedItem as Cargo;
				if (selectedCargo != null)
				{
					MeasureTextBox.Text = selectedCargo.Measure == null ? string.Empty : selectedCargo.Measure.Name;
				}
				// Перевод фокуса ввода на следующий визуальный элемент после [DataGrid] поиска сущности
				var nextControlAfterDataGrid = searchDataGrid.PredictFocus(FocusNavigationDirection.Down) as Control;
				if (nextControlAfterDataGrid == null)
				{
					return;
				}
				eventArgs.Handled = true;
				nextControlAfterDataGrid.Focus();
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
			if (selectedItemType == typeof(CompanyWithUnp))     // Если тип найденой сущности: [Внешние организации]
			{
				var selectedItem = (CompanyWithUnp)rawSelectedItem;
				displayed = selectedItem.ServiceSearchResultDisplayed;
			}
			else if (selectedItemType == typeof(Company))       // Если тип найденой сущности: [Внутренние организации]
			{
				var selectedItem = (Company)rawSelectedItem;
				displayed = selectedItem.ServiceSearchResultDisplayed;
			}
			else if (selectedItemType == typeof(Measure))       // Если тип найденой сущности: [Единицы измерения]
			{
				var selectedItem = (Measure)rawSelectedItem;
				displayed = selectedItem.ServiceSearchDisplayed;
			}
			else if (selectedItemType == typeof(Cargo))         // Если тип найденой сущности: [Грузы ТМЦ]
			{
				var selectedItem = (Cargo)rawSelectedItem;
				displayed = selectedItem.ServiceSearchResultDisplayed;
				// В поле [Единиц измерения] заносится [Единица измерения] этого перевозимого груза ТМЦ
				MeasureTextBox.Text = selectedItem.Measure == null ? string.Empty : selectedItem.Measure.Name;
			}
			else
			{
				displayed = rawSelectedItem.ToString();
			}
			// Вывод выбраного значения в TextBox поиска/добавления
			searchTextBox.Text = displayed;

			// Перевод фокуса ввода на нижележащий визуальный элемент после [DataGrid] поиска сущности
			var nextControlAfterDataGrid = searchDataGrid.PredictFocus(FocusNavigationDirection.Down) as Control;
			if (nextControlAfterDataGrid == null)
			{
				return;
			}
			eventArgs.Handled = true;
			nextControlAfterDataGrid.Focus();
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
				if (selectedItemType == typeof(CompanyWithUnp)) // Если тип найденой сущности: [Внешние организации]
				{
					var selectedItem = (CompanyWithUnp)rawSelectedItem;
					displayed = selectedItem.ServiceSearchResultDisplayed;
				}
				else if (selectedItemType == typeof(Company))   // Если тип найденой сущности: [Внутренние организации]
				{
					var selectedItem = (Company)rawSelectedItem;
					displayed = selectedItem.ServiceSearchResultDisplayed;
				}
				else if (selectedItemType == typeof(Measure))   // Если тип найденой сущности: [Единицы измерения]
				{
					var selectedItem = (Measure)rawSelectedItem;
					displayed = selectedItem.ServiceSearchDisplayed;
				}
				else if (selectedItemType == typeof(Cargo))     // Если тип найденой сущности: [Грузы ТМЦ]
				{
					var selectedItem = (Cargo)rawSelectedItem;
					displayed = selectedItem.ServiceSearchResultDisplayed;
					// В поле [Единиц измерения] заносится [Единица измерения] этого перевозимого груза ТМЦ
					MeasureTextBox.Text = selectedItem.Measure == null ? string.Empty : selectedItem.Measure.Name;
				}
				else
				{
					displayed = rawSelectedItem.ToString();
				}
				// Вывод выбраного значения в TextBox поиска/добавления
				searchTextBox.Text = displayed;

				// Перевод фокуса ввода на нижележащий визуальный элемент после [DataGrid] поиска сущности
				var request = new TraversalRequest(FocusNavigationDirection.Down)
				{
					Wrapped = false
				};
				eventArgs.Handled = true;
				if (searchDataGrid.MoveFocus(request))
				{
					searchDataGrid.Visibility = Visibility.Collapsed;
				}
			}
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
		/// Обработка нажатия [Enter] в поле [количества ед.изм]: перенос фокуса на кнопку [добавления в список].
		/// (Метод требуется, так как стандартный метод FocusNavigationDirection.Next не срабатывает)
		/// </summary>
		private void JumpToButtonAppendCargo_OnKeyDown(object senderIsDoubleUpDown, KeyEventArgs eventArgs)
		{
			var control = senderIsDoubleUpDown as Control;
			if (control == null || eventArgs.Key != Key.Enter)
			{
				return;
			}
			eventArgs.Handled = true;
			AddInventoryButton.Focus();
		}

		/// <summary>
		/// Проверка, изменились ли поля ввода, и запрос подтверждения, если изменились. Далее выход к списку сущностей
		/// </summary>
		private void ConfirmExitIfDataHasBeenChanged()
		{
			var isFieldsNotChanged = IsPageFieldsNotCanged();
			// Если введённые поля изменились - запрос у пользователя подтверждения намерение выхода к списку сущностей
			if (!isFieldsNotChanged && !PageUtil.ConfirmBackToListWhenFieldChanged())
			{
				return;
			}
			PageSwitcher.Switch(new LetterOfAttorneysTable(_editedLoa));
		}

		/// <summary>
		/// Проверка - изменились ли поля ввода с момента загрузки страницы
		/// </summary>
		private bool IsPageFieldsNotCanged()
		{
			var isFieldsNotChanged = true;
			if (_editedLoa == null)          // Если сущность добавляется
			{
				if (_newLoaWithSelectedCourier != null)       // Случай, когда новая [Доверенность указанному курьеру]
				{
					isFieldsNotChanged &= EmployeeNamesComboBox.SelectedItem == null;
					isFieldsNotChanged &= _newLoaWithSelectedCourier.Equals(CourierNamesComboBox.SelectedItem);
				}
				else if (_newLoaWithSelectedEmployee != null) // Случай, когда новая [Доверенность указанному сотрудн.]
				{
					isFieldsNotChanged &= CourierNamesComboBox.SelectedItem == null;
					isFieldsNotChanged &= _newLoaWithSelectedEmployee.Equals(EmployeeNamesComboBox.SelectedItem);
				}
				else                                          // Случай обычного добавления 
				{
					isFieldsNotChanged &= CourierNamesComboBox.SelectedItem == null;
					isFieldsNotChanged &= EmployeeNamesComboBox.SelectedItem == null;
				}
				// Текущая дата + количество дней действия по-умолчанию
				var defaultCountDaysValidity = Properties.Settings.Default.DefaultLoaCountDaysValidity;
				isFieldsNotChanged &= DateTime.Today.Equals(ValidityDateStartDatePicker.SelectedDate);
				var initialDateEnd = DateTime.Today.AddDays(defaultCountDaysValidity);
				isFieldsNotChanged &= initialDateEnd.Equals(ValidityDateEndDatePicker.SelectedDate);

				isFieldsNotChanged &= string.IsNullOrWhiteSpace(CompanyTextBox.Text);
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(CargoTextBox.Text);
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(MeasureTextBox.Text);
				isFieldsNotChanged &= CountInventoryDoubleUpDown.Value == null;
				isFieldsNotChanged &= _newShipments == null || _newShipments.Count == 0;
			}
			else                             // Если сущность редактируется
			{
				// Сотрудник
				if (_editedLoa.Courier != null && _editedLoa.Courier.GetType() == typeof(Employee))
				{
					var employeeEditedLoa = (Employee) _editedLoa.Courier;
					isFieldsNotChanged &= employeeEditedLoa.Equals(EmployeeNamesComboBox.SelectedItem);
					isFieldsNotChanged &= CourierNamesComboBox.SelectedItem == null;
				}
				// Курьер
				else if (_editedLoa.Courier != null && _editedLoa.Courier.GetType() == typeof(Courier))
				{
					var courierEditedLoa = _editedLoa.Courier;
					isFieldsNotChanged &= courierEditedLoa.Equals(CourierNamesComboBox.SelectedItem);
					isFieldsNotChanged &= EmployeeNamesComboBox.SelectedItem == null;
				}
				var editedLoaOldCompany = _editedLoa.Company == null ? string.Empty : _editedLoa.Company.Name;
				isFieldsNotChanged &= Equals(CompanyTextBox.Text.Trim(), editedLoaOldCompany);
				isFieldsNotChanged &= Equals(ValidityDateStartDatePicker.SelectedDate, _editedLoa.ValidityDateStart);
				isFieldsNotChanged &= Equals(ValidityDateEndDatePicker.SelectedDate, _editedLoa.ValidityDateEnd);
				
				// Если список ТМЦ в старой доверенности не пустой - SequenceEqual по коллекции
				isFieldsNotChanged &= _editedLoa.Shipments != null && _editedLoa.Shipments.Count != 0
					? _newShipments != null && _editedLoa.Shipments.SequenceEqual(_newShipments)
					: _newShipments == null || _newShipments.Count == 0;
			}
			return isFieldsNotChanged;
		}

		/// <summary>
		/// Указание фокуса ввода на ComboBox [Сотрудников] при обычной загрузке при добавлении 
		/// </summary>
		private void EmployeeNamesComboBoxSetFocus_OnLoaded(object senderIsComboBox, RoutedEventArgs eventArgs)
		{
			var comboBox = senderIsComboBox as ComboBox;
			if (comboBox == null)
			{
				return;
			}
			// Получения TextBox-части из поискового ComboBox
			var textBox = comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
			if (textBox == null || _editedLoa != null)
			{
				return;
			}
			// Если это не случай добавления новой доверенности указанному [сотруднику/курьеру] из страницы печати
			if (_newLoaWithSelectedCourier == null && _newLoaWithSelectedEmployee == null)
			{
				textBox.Focus();
			}
		}
	}
}
