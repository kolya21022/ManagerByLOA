using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;

using Microsoft.Reporting.WinForms;

namespace LoaPrintOfflineApp
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void PrintButton_Click(object sender, RoutedEventArgs eventArgs)
		{
			// Получение введённых значений
			var name = NameTextBox.Text.Trim();
			var profession = ProfessionTextBox.Text.Trim();
			var passportSeries = PassportSeriesTextBox.Text.Trim();
			var passportNumber = PassportNumberTextBox.Text.Trim();
			var issued = IssuedTextBox.Text.Trim();
			var issuedDate = IssuedDateTextBox.Text.Trim();
			var company = CompanyTextBox.Text.Trim();
			var ordinal = OrdinalTextBox.Text.Trim();
			var dateStart = DateStartTextBox.Text.Trim();
			var dateEnd = DateEndTextBox.Text.Trim();

			// Первый обязательный ТМЦ (груз)
			var shipment1 = Shipment1TextBox.Text.Trim();
			var shipment1Measure = Shipment1MeasureTextBox.Text.Trim();
			var shipment1Count = Shipment1CountTextBox.Text.Trim();

			// Проверка заполнености обязательных полей
			if (string.Empty.Equals(name) || string.Empty.Equals(profession) || string.Empty.Equals(passportSeries) ||
				string.Empty.Equals(passportNumber) || string.Empty.Equals(issued) ||
				string.Empty.Equals(issuedDate) || string.Empty.Equals(company) || string.Empty.Equals(ordinal) ||
				string.Empty.Equals(dateStart) || string.Empty.Equals(dateEnd) ||
				string.Empty.Equals(shipment1))
			{
				const string header = "Сообщение проверки корректности";
				const string message = "Введены не все необходимые поля";
				MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Формирование одиночных строковых параметров отчёта
			IEnumerable<ReportParameter> reportParameters = new[]
			{
				new ReportParameter("Company", company),
				new ReportParameter("OrdinalNumber", ordinal),
				new ReportParameter("Profession", profession),
				new ReportParameter("ValidityDateEnd", dateEnd),
				new ReportParameter("PassportSeries", passportSeries),
				new ReportParameter("PassportNumber", passportNumber),
				new ReportParameter("ValidityDateStart", dateStart),
				new ReportParameter("EmployeeFullName", name),
				new ReportParameter("PassportIssuedByOrganization", issued),
				new ReportParameter("PassportIssueDate", issuedDate)
			};

			var listItemOfLoaReports = new List<ListItemOfLoaReport>
			{
				new ListItemOfLoaReport
				{
					OrdinalNumber = 1M, Name = shipment1, Measure = shipment1Measure, Count = shipment1Count
				}
			};

			// Второй необязательный ТМЦ (груз)
			var shipment2 = string.IsNullOrWhiteSpace(Shipment2TextBox.Text) ? null : Shipment2TextBox.Text.Trim();
			var shipment2Measure = Shipment2MeasureTextBox.Text.Trim();
			var shipment2Count = Shipment2CountTextBox.Text.Trim();
			if (shipment2 != null)
			{
				listItemOfLoaReports.Add(new ListItemOfLoaReport
				{
					OrdinalNumber = listItemOfLoaReports.Count + 1M,
					Name = shipment2,
					Measure = shipment2Measure,
					Count = shipment2Count
				});
			}

			// Третий необязательный ТМЦ (груз)
			var shipment3 = string.IsNullOrWhiteSpace(Shipment3TextBox.Text) ? null : Shipment3TextBox.Text.Trim();
			var shipment3Measure = Shipment3MeasureTextBox.Text.Trim();
			var shipment3Count = Shipment3CountTextBox.Text.Trim();
			if (shipment3 != null)
			{
				listItemOfLoaReports.Add(new ListItemOfLoaReport
				{
					OrdinalNumber = listItemOfLoaReports.Count + 1M,
					Name = shipment3,
					Measure = shipment3Measure,
					Count = shipment3Count
				});
			}

			// Формирование DataSource (коллекция перевозимых ТМЦ)
			const string dataSourceName = "LOADataSet";
			var reportDataSource = new ReportDataSource(dataSourceName, listItemOfLoaReports);

			// Полный путь к файлу отчёта
			var reportFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoaReport.rdlc");

			// Инициализация и вывод окна отчёта
			var reportWindow = new ReportWindow(reportFile, reportParameters, reportDataSource)
			{
				Owner = this
			};
			reportWindow.ShowDialog();
		}

		/// <summary>
		/// Первозимый ТМЦ доверенности (используется для отображения в отчёте)
		/// </summary>
		/// <inheritdoc />
		public class ListItemOfLoaReport : IComparable<ListItemOfLoaReport>
		{
			// ReSharper disable MemberCanBePrivate.Global

			public decimal OrdinalNumber { get; set; }
			public string Name { get; set; }
			public string Measure { get; set; }
			public string Count { get; set; }

			// ReSharper restore MemberCanBePrivate.Global

			private bool Equals(ListItemOfLoaReport other)
			{
				const StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
				return OrdinalNumber == other.OrdinalNumber
					   && string.Equals(Name, other.Name, stringComparison)
					   && string.Equals(Measure, other.Measure, stringComparison)
					   && string.Equals(Count, other.Count, stringComparison);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
				{
					return false;
				}
				if (ReferenceEquals(this, obj))
				{
					return true;
				}
				return obj.GetType() == GetType() && Equals((ListItemOfLoaReport)obj);
			}

			public int CompareTo(ListItemOfLoaReport other)
			{
				const StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
				if (ReferenceEquals(this, other))
				{
					return 0;
				}
				if (ReferenceEquals(null, other))
				{
					return 1;
				}
				var ordinalNumberComparison = OrdinalNumber.CompareTo(other.OrdinalNumber);
				if (ordinalNumberComparison != 0)
				{
					return ordinalNumberComparison;
				}
				var nameComparison = string.Compare(Name, other.Name, stringComparison);
				if (nameComparison != 0)
				{
					return nameComparison;
				}
				var measureComparison = string.Compare(Measure, other.Measure, stringComparison);
				return measureComparison != 0 ? measureComparison : string.Compare(Count, other.Count, stringComparison);
			}

			public override int GetHashCode()
			{
				var stringComparer = StringComparer.OrdinalIgnoreCase;
				unchecked
				{
					// ReSharper disable NonReadonlyMemberInGetHashCode

					var hashCode = OrdinalNumber.GetHashCode();
					hashCode = (hashCode * 397) ^ (Name != null ? stringComparer.GetHashCode(Name) : 0);
					hashCode = (hashCode * 397) ^ (Measure != null ? stringComparer.GetHashCode(Measure) : 0);
					hashCode = (hashCode * 397) ^ (Count != null ? stringComparer.GetHashCode(Count) : 0);
					return hashCode;

					// ReSharper restore NonReadonlyMemberInGetHashCode
				}
			}
		}
	}
}
