using System;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Reporting.WinForms;

namespace LoaPrintOfflineApp
{
	/// <summary>
	/// Окно предпросмотра и печати отчёта
	/// </summary>
	/// <inheritdoc cref="System.Windows.Window" />
	public partial class ReportWindow
	{
		private readonly string _reportFile;                             // Абсолютный путь к файлу отчёта
		private readonly IEnumerable<ReportParameter> _reportParameters; // Одиночные строковые параметры отчёта
		private readonly ReportDataSource _reportDataSource;             // Источник данных печатаемого списка ТЦМ

		public ReportWindow(string reportFile, IEnumerable<ReportParameter> reportParameters, 
			ReportDataSource reportDataSource)
		{
			_reportFile = reportFile;
			_reportParameters = reportParameters;
			_reportDataSource = reportDataSource;
			InitializeComponent();
			ReportViewer.Load += ReportViewer_Load;
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
			report.ProcessingMode = ProcessingMode.Local;
			report.LocalReport.ReportPath = _reportFile;            // Путь к файлу отчёта
			report.SetDisplayMode(DisplayMode.PrintLayout);         // Режим предпросмотра "Разметка страницы"
			report.LocalReport.DataSources.Clear();
			report.ZoomMode = ZoomMode.PageWidth;                   // Режим масштабирования "По ширине страницы"
			report.Visible = true;

			report.LocalReport.SetParameters(_reportParameters);    // Одиночные строковые параметры
			report.LocalReport.DataSources.Add(_reportDataSource);  // Выводимый список ТМЦ доверенности
			report.RefreshReport();
		}

		protected override void OnClosing(CancelEventArgs eventArgs)
		{
			base.OnClosing(eventArgs);
			if (ReportViewer != null)
			{
				ReportViewer.Dispose();
			}
		}
	}
}
