using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using ManagerByLetterOfAttorney.Util;

namespace ManagerByLetterOfAttorney.View.Windows
{
	/// <summary>
	/// Окно с запросом подтверждения экспорта доверенностей текущего года в архив.	
	/// </summary>
	/// <inheritdoc cref="Window" />
	public partial class ConfirmExportWindow
	{
		private const string ConfirmWord = "подтверждаю"; // слово-подтверждение, для активации кнопки экспорта
		private readonly int _oldYear;
		private readonly int _newYear;

		public ConfirmExportWindow(int oldYear, int newYear)
		{
			_oldYear = oldYear;
			_newYear = newYear;
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Визуальная инициализация окна (цвета и размеры шрифтов контролов)
		/// </summary>
		private void VisualInitializeComponent()
		{
			FontSize = Constants.FontSize;
			Foreground = Constants.ForeColor2_PapayaWhip;
			Background = Constants.BackColor5_WaikawaGray;

			// Визуальное выделение некоторых слов, для акцентирования внимания
			Run[] warningRuns = {
				OperationIsIrrevocableWarningRun, PreviousYear1Run,
				PreviousYear2Run, NextYear1Run, NextYear2Run, ConfirmRun
			};
			foreach (var run in warningRuns)
			{
				run.Foreground = Constants.ForeColor3_Yellow;
			}
		}

		/// <summary>
		/// Указание соответсвующих полей в сообщении: годы и слово-подтверждение активации кнопки
		/// </summary>
		private void AdditionalInitializeComponent()
		{
			PreviousYear1Run.Text = _oldYear.ToString();
			PreviousYear2Run.Text = _oldYear.ToString();

			NextYear1Run.Text = _newYear.ToString();
			NextYear2Run.Text = _newYear.ToString();

			ConfirmRun.Text = ConfirmWord.ToUpperInvariant();
			ConfirmTextBox.Focus();
		}

		/// <summary>
		/// Нажатие кнопки [Выполнить экспорт]
		/// </summary>
		private void ExportButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			DialogResult = true;
			Close();
		}

		/// <summary>
		/// Нажатие кнопки [Закрыть приложение]
		/// </summary>
		private void CloseApplicationButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			Close();
		}

		/// <summary>
		/// Событие изменения текста в поле ввода слова-подтверждения.
		/// </summary>
		private void ConfirmTextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs eventArgs)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			var textBox = senderIsTextBox as TextBox;
			if (textBox == null)
			{
				return;
			}
			var value = textBox.Text.Trim();
			ExportButton.IsEnabled = ConfirmWord.Equals(value, comparisonIgnoreCase);
		}
	}
}
