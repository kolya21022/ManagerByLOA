using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Reporting.WinForms;

using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.View.Util;
using ManagerByLetterOfAttorney.View.Pages.Edit;
using ManagerByLetterOfAttorney.Entities.Report;
using ManagerByLetterOfAttorney.Entities.External;
using ManagerByLetterOfAttorney.Entities.Internal;
using ManagerByLetterOfAttorney.View.Pages.TableView;

namespace ManagerByLetterOfAttorney.View.Pages.Reports
{
	/// <summary>
	/// Страница предпросмотра печати (и печати) доверенности
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class LoaReportPage : IPageable
	{
		private const string ReportFileName = "LoaReport.rdlc";

		/// <summary>
		/// Доверенность для предпросмотра печати (и печати)
		/// </summary>
		private readonly LetterOfAttorney _printedLoa;

		private string _reportFile;								// Абсолютный путь к файлу отчёта
		private ReportDataSource _reportDataSource;				// Источник данных печатаемого списка ТЦМ
		private IEnumerable<ReportParameter> _reportParameters;	// Одиночные строковые параметры отчёта

		public LoaReportPage(LetterOfAttorney printedLetterOfAttorney)
		{
			InitializeComponent();
			VisualInitializeComponent();
			if (printedLetterOfAttorney == null)
			{
				const MessageBoxImage messageType = MessageBoxImage.Error;
				const MessageBoxButton messageButtons = MessageBoxButton.OK;
				const string appErrorHeader = PageLiterals.HeaderLogicError;
				const string absentReportMessage = "Доверенность для печати отсутствует";
				MessageBox.Show(absentReportMessage, appErrorHeader, messageButtons, messageType);
				return;
			}
			_printedLoa = printedLetterOfAttorney;
			AdditionalInitializeComponent();
			ReportViewer.Load += ReportViewer_Load;     // Подписка на метод загрузки и отображения отчёта
		}

		/// <summary>
		/// Получение/формирование параметров отчёта, DataSource (список сущностей для таблицы), пути файла и заголовка
		/// </summary>
		/// <inheritdoc />
		public void AdditionalInitializeComponent()
		{
			SetPageTitle(_printedLoa.OrdinalNumber);	            // Заголовок страницы
			_reportFile = Common.GetReportFilePath(ReportFileName);	// Путь к файлу отчёта

			// Получение строковых параметров отчёта
			var courierPassportSeries = GetCourierPassportSeries();
			var courierPassportNumber = GetCourierPassportNumber();
			var courierFullnameInDativeCase = GetCourierFullnameInDativeCase();
			var courierProfession = _printedLoa.Courier != null && _printedLoa.Courier.Profession != null
				? _printedLoa.Courier.Profession
				: string.Empty;
			var loaValidityDateStart = _printedLoa.ValidityDateStart.ToShortDateString();
			var loaValidityDateEnd = _printedLoa.ValidityDateEnd.ToShortDateString();
			var loaOrdinal = _printedLoa.OrdinalNumber.ToString();
			var loaCompany = _printedLoa.Company != null && _printedLoa.Company.Name != null 
				? _printedLoa.Company.Name 
				: string.Empty;
			var courierPassportIssuedByOrganization = _printedLoa.Courier != null 
			                                       && _printedLoa.Courier.PassportIssuedByOrganization != null
				? _printedLoa.Courier.PassportIssuedByOrganization
				: string.Empty;
			var courierPassportIssueDate = _printedLoa.Courier != null && _printedLoa.Courier.PassportIssueDate != null
				? ((DateTime) _printedLoa.Courier.PassportIssueDate).ToShortDateString()
				: string.Empty;

			// Формирование одиночных строковых параметров отчёта
			_reportParameters = new[]
			{
				new ReportParameter("Company", loaCompany),
				new ReportParameter("OrdinalNumber", loaOrdinal),
				new ReportParameter("Profession", courierProfession),
				new ReportParameter("ValidityDateEnd", loaValidityDateEnd),
				new ReportParameter("PassportSeries", courierPassportSeries),
				new ReportParameter("PassportNumber", courierPassportNumber),
				new ReportParameter("ValidityDateStart", loaValidityDateStart),
				new ReportParameter("EmployeeFullName", courierFullnameInDativeCase),
				new ReportParameter("PassportIssuedByOrganization", courierPassportIssuedByOrganization),
				new ReportParameter("PassportIssueDate", courierPassportIssueDate)
			};

			// Получение и формирование DataSource отчёта (список сущностей для таблицы)
			const string dataSourceName = "LOADataSet";
			var listItemsOfReport = PrepareListItemsOfReport(_printedLoa.Shipments);
			_reportDataSource = new ReportDataSource(dataSourceName, listItemsOfReport);
		}

		/// <summary>
		/// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
		/// </summary>
		/// <inheritdoc />
		public void VisualInitializeComponent()
		{
			FontSize = Constants.FontSize;

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
			const string closePageBackToListHotkey = PageLiterals.HotkeyLabelClosePageBackToList;
			return closePageBackToListHotkey;
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
			// Если нажат [Esc] - выходим к списку доверенностей
			eventArgs.Handled = true;
			PageSwitcher.Switch(new LetterOfAttorneysTable(_printedLoa));
		}

		/// <summary>
		/// Указание заголовка страницы
		/// </summary>
		private void SetPageTitle(long? ordinalNumber)
		{
			const string titlePattern = PageLiterals.PatternReportPageTitle;
			TitleLabel.Content = string.Format(titlePattern, ordinalNumber);
		}

		/// <summary>
		/// Инициализация и отображение отчёта
		/// </summary>
		private void ReportViewer_Load(object senderIsReportViewer, EventArgs eventArgs)
		{
			var report = senderIsReportViewer as ReportViewer;
			if (report == null)
			{
				return;
			}
			report.SetDisplayMode(DisplayMode.PrintLayout);			// Режим предпросмотра "Разметка страницы"
			report.LocalReport.ReportPath = _reportFile;			// Путь к файлу отчёта
			report.LocalReport.DataSources.Clear();
			report.ZoomMode = ZoomMode.PageWidth;					// Режим масштабирования "По ширине страницы"
			report.Visible = true;

			report.LocalReport.SetParameters(_reportParameters);    // Одиночные строковые параметры
			report.LocalReport.DataSources.Add(_reportDataSource);	// Выводимый список ТМЦ доверенности
			report.RefreshReport();
		}

		/// <summary>
		/// Получение серии паспорта курьера/сотрудника (первые два символа из полного номера паспорта)
		/// </summary>
		private string GetCourierPassportSeries()
		{
			var courier = _printedLoa != null ? _printedLoa.Courier : null;
			return courier != null && !string.IsNullOrWhiteSpace(courier.PassportSeriesAndNumber)
				? courier.PassportSeriesAndNumber.Trim().Substring(0, 2)
				: string.Empty;
		}

		/// <summary>
		/// Получение номера паспорта курьера/сотрудника (числовой номер паспорта с 3-го символа)
		/// </summary>
		private string GetCourierPassportNumber()
		{
			var courier = _printedLoa != null ? _printedLoa.Courier : null;
			return courier != null && !string.IsNullOrWhiteSpace(courier.PassportSeriesAndNumber)
				? courier.PassportSeriesAndNumber.Trim().Substring(2)
				: string.Empty;
		}

		/// <summary>
		/// Получение полного имени курьера/сотрудника в дательном падеже
		/// </summary>
		private string GetCourierFullnameInDativeCase()
		{
			var courier = _printedLoa != null ? _printedLoa.Courier : null;
			if (courier == null)
			{
				return string.Empty;
			}
			var sex = courier.Sex;
			var lastName = courier.LastName;
			var firstName = courier.FirstName;
			var middleName = courier.MiddleName;

			string warningMessages;
			var fullnameInDativeCase = NamesConverter.FullnameInDativeCase(lastName,
				firstName, middleName, sex, out warningMessages);

			// Если в процессе получения ФИО в дательном падеже возникли предупреждения - отображаем их пользователю
			if (!string.IsNullOrWhiteSpace(warningMessages)) 
			{
				const MessageBoxButton messageButtons = MessageBoxButton.OK;
				const MessageBoxImage messageType = MessageBoxImage.Information;
				const string warningHeader = PageLiterals.HeaderInformationOrWarning;
				MessageBox.Show(warningMessages, warningHeader, messageButtons, messageType);
			}
			return fullnameInDativeCase;
		}

		/// <summary>
		/// Формирование выводимого списка ТМЦ доверенности
		/// </summary>
		private static IEnumerable<ListItemOfLoaReport> PrepareListItemsOfReport(List<Shipment> shipments)
		{
			var letterOfAttorneyReports = new List<ListItemOfLoaReport>(shipments.Capacity);
			var ordinal = 0;
			foreach (var shipment in shipments)
			{
				var countInWords = string.Empty;
				if (shipment.Count != null && !shipment.Count.Equals(0D))
				{
					var number = (double) shipment.Count;

					// Получение количетсва ТМЦ прописью
					var precision = NumbersInWordsUtil.GetPrecision(number);
					countInWords = NumbersInWordsUtil.GetNumberString(number, precision);
				}
				var letterOfAttorneyReport = new ListItemOfLoaReport
				{
					Count = countInWords,
					Measure = shipment.Cargo != null && shipment.Cargo.Measure != null 
						? shipment.Cargo.Measure.Name 
						: string.Empty,
					Name = shipment.Cargo != null ? shipment.Cargo.Name : string.Empty,
					OrdinalNumber = ++ordinal // Порядковый номер ТМЦ этой доверенности
				};
				letterOfAttorneyReports.Add(letterOfAttorneyReport);
			}
			return letterOfAttorneyReports;
		}

		/// <summary>
		/// Нажатие кнопки [К списку] - Переход к таблице доверенностей
		/// </summary>
		private void BackToListButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			eventArgs.Handled = true;
			PageSwitcher.Switch(new LetterOfAttorneysTable(_printedLoa));
		}

		/// <summary>
		/// Нажатие кнопки [Правка] - Изменение этой доверенности на соответствующей странице
		/// </summary>
		private void EditLoaButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			eventArgs.Handled = true;
			if (_printedLoa == null)
			{
				return;
			}
			PageSwitcher.Switch(new LetterOfAttorneyEdit(_printedLoa));
		}

		/// <summary>
		/// Нажатие кнопки [Новая доверенность текущему сотруднику/курьеру]
		/// </summary>
		private void AddNewLoaThisCourierButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			eventArgs.Handled = true;
			if (_printedLoa == null)
			{
				return;
			}
			eventArgs.Handled = true;
			// Вызывается нужный конструктор страницы добавления доверенности
			var courierOrEmployee = _printedLoa.Courier;
			if (courierOrEmployee.GetType() == typeof(Courier))			// Если тип перевозчика - курьер
			{
				var courier = courierOrEmployee;
				PageSwitcher.Switch(new LetterOfAttorneyEdit(courier));
			}
			else if (courierOrEmployee.GetType() == typeof(Employee))	// Если тип перевозчика - сотрудник предприятия
			{
				var employee = (Employee)courierOrEmployee;
				PageSwitcher.Switch(new LetterOfAttorneyEdit(employee));
			}
		}
	}
}
