using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.View.Util;

namespace ManagerByLetterOfAttorney.View.Windows
{
	/// <summary>
	/// Окно пользовательских настроек.
	/// </summary>
	/// <inheritdoc cref="Window" />
	public partial class UserConfigWindow 
	{
		public UserConfigWindow()
		{
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
			Background = Constants.BackColor5_WaikawaGray;
			Foreground = Constants.ForeColor2_PapayaWhip;

			// Цвета Labels и TextBlocks
			var mainLabels = FieldsWrapperGrid.Children.OfType<Label>();
			foreach (var label in mainLabels)
			{
				label.Foreground = Constants.ForeColor2_PapayaWhip;
			}
			DefaultLoaCountDaysValidityLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			TimeoutServerConnectInSecondLabel.Foreground = Constants.ForeColor2_PapayaWhip;

			var mainTextBlocks = new[] 
			{
				IsRunFullscreenTextBlock,
				IsShowDismissalEmployeesInTableTextBlock
			};
			foreach (var textBlock in mainTextBlocks)
			{
				textBlock.Foreground = Constants.ForeColor2_PapayaWhip;
			}

			// Фоны
			BackgroundRectangle.Fill = Constants.BackColor3_SanJuan;
			HotkeysStackPanel.Background = Constants.BackColor4_BlueBayoux;

			// Панель хоткеев
			var helpLabels = HotkeysStackPanel.Children.OfType<Label>();
			foreach (var helpLabel in helpLabels)
			{
				helpLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}
		}

		/// <summary>
		/// Получение и отображение значений пользовательских параметров в нужные поля ввода и надписей хоткеев
		/// </summary>
		private void AdditionalInitializeComponent()
		{
			const string closeWindowHotkey = PageLiterals.HotkeyLabelCloseWindow;
			HotkeysTextBlock.Text = closeWindowHotkey;

			var serverLetterOfAttoney = Properties.Settings.Default.ServerLetterOfAttorney;
			var dbLetterOfAttoney = Properties.Settings.Default.DbLetterOfAttorney;

			var serverEmployees = Properties.Settings.Default.ServerEmployees;
			var dbEmployees = Properties.Settings.Default.DbEmployees;

			var serverConsumers = Properties.Settings.Default.ServerConsumers;
			var dbConsumers = Properties.Settings.Default.DbConsumers;

			var isRunInFullscreen = Properties.Settings.Default.IsRunInFullscreen;
			var defaultLoaCountDaysValidity = Properties.Settings.Default.DefaultLoaCountDaysValidity;
			var isShowDismissalEmployeesInTable = Properties.Settings.Default.IsShowDismissalEmployeesInTable;
			var timeoutServerConnectInSecond = Properties.Settings.Default.TimeoutServerConnectInSecond;

			ServerLetterOfAttoneyTextBox.Text = serverLetterOfAttoney;
			DbLetterOfAttoneyTextBox.Text = dbLetterOfAttoney;

			ServerEmployeesTextBox.Text = serverEmployees;
			DbEmployeesTextBox.Text = dbEmployees;

			ServerConsumersTextBox.Text = serverConsumers;
			DbConsumersTextBox.Text = dbConsumers;

			IsRunFullscreenCheckBox.IsChecked = isRunInFullscreen;
			IsShowDismissalEmployeesInTableCheckBox.IsChecked = isShowDismissalEmployeesInTable;

			DefaultLoaCountDaysValidityIntegerUpDown.Minimum = 1;
			DefaultLoaCountDaysValidityIntegerUpDown.Maximum = int.MaxValue;
			DefaultLoaCountDaysValidityIntegerUpDown.Value = defaultLoaCountDaysValidity;

			TimeoutServerConnectInSecondIntegerUpDown.Minimum = 1;
			TimeoutServerConnectInSecondIntegerUpDown.Maximum = int.MaxValue;
			TimeoutServerConnectInSecondIntegerUpDown.Value = timeoutServerConnectInSecond;
		}

		/// <summary>
		/// Нажатие кнопки [Сохранить] - валидация, сохранение и закрытие окна
		/// </summary>
		private void SaveButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			// Получение названий полей и значений

			var serverLetterOfAttoneyLabel = ServerLetterOfAttoneyLabel.Content.ToString();
			var serverLetterOfAttoney = ServerLetterOfAttoneyTextBox.Text.Trim();

			var dbLetterOfAttoneyLabel = DbLetterOfAttoneyLabel.Content.ToString();
			var dbLetterOfAttoney = DbLetterOfAttoneyTextBox.Text.Trim();

			var serverEmployeesLabel = ServerEmployeesLabel.Content.ToString();
			var serverEmployees = ServerEmployeesTextBox.Text.Trim();

			var dbEmployeesLabel = DbEmployeesLabel.Content.ToString();
			var dbEmployees = DbEmployeesTextBox.Text.Trim();

			var serverConsumersLabel = ServerConsumersLabel.Content.ToString();
			var serverConsumers = ServerConsumersTextBox.Text.Trim();

			var dbConsumersLabel = DbConsumersLabel.Content.ToString();
			var dbConsumers = DbConsumersTextBox.Text.Trim();

			var isRunInFullscreen = IsRunFullscreenCheckBox.IsChecked ?? false;
			var isShowDismissalEmployeesInTable = IsShowDismissalEmployeesInTableCheckBox.IsChecked ?? false;

			var defaultLoaCountDaysValidityLabel = DefaultLoaCountDaysValidityLabel.Content.ToString();
			var nullableDefaultLoaCountDaysValidity = DefaultLoaCountDaysValidityIntegerUpDown.Value;
			
			var timeoutServerConnectInSecondLabel = TimeoutServerConnectInSecondLabel.Content.ToString();
			var nullableTimeoutServerConnectInSecond = TimeoutServerConnectInSecondIntegerUpDown.Value;

			// Валидация на пустоту

			var isValid = true;
			var errorMessage = string.Empty;
			var messagePattern = "Поле [{0}] пустое / не указано" + Environment.NewLine;

			isValid &= !string.IsNullOrWhiteSpace(serverLetterOfAttoney);
			errorMessage += string.IsNullOrWhiteSpace(serverLetterOfAttoney)
				? string.Format(messagePattern, serverLetterOfAttoneyLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(dbLetterOfAttoney);
			errorMessage += string.IsNullOrWhiteSpace(dbLetterOfAttoney)
				? string.Format(messagePattern, dbLetterOfAttoneyLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(serverEmployees);
			errorMessage += string.IsNullOrWhiteSpace(serverEmployees)
				? string.Format(messagePattern, serverEmployeesLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(dbEmployees);
			errorMessage += string.IsNullOrWhiteSpace(dbEmployees)
				? string.Format(messagePattern, dbEmployeesLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(serverConsumers);
			errorMessage += string.IsNullOrWhiteSpace(serverConsumers) 
				? string.Format(messagePattern, serverConsumersLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(dbConsumers);
			errorMessage += string.IsNullOrWhiteSpace(dbConsumers)
				? string.Format(messagePattern, dbConsumersLabel)
				: string.Empty;

			isValid &= nullableDefaultLoaCountDaysValidity != null;
			errorMessage += nullableDefaultLoaCountDaysValidity == null
				? string.Format(messagePattern, defaultLoaCountDaysValidityLabel)
				: string.Empty;

			isValid &= nullableTimeoutServerConnectInSecond != null;
			errorMessage += nullableTimeoutServerConnectInSecond == null
				? string.Format(messagePattern, timeoutServerConnectInSecondLabel)
				: string.Empty;

			if (!isValid) // Если какое-то из полей не указано
			{
				const MessageBoxImage messageBoxType = MessageBoxImage.Error;
				const MessageBoxButton messageBoxButtons = MessageBoxButton.OK;
				MessageBox.Show(errorMessage, PageLiterals.HeaderValidation, messageBoxButtons, messageBoxType);
				return;
			}

			// Сохранение параметров в пользовательский config-файл этой версии приложения и закрытие окна
			// Ориентировочный путь: [ c:\Users\Username\AppData\Local\OJSC_GZSU\ManagerByLetterOfAttorney_Url... ]

			var defaultLoaCountDaysValidity = (int) nullableDefaultLoaCountDaysValidity;
			var timeoutServerConnectInSecond = (int) nullableTimeoutServerConnectInSecond;

			Properties.Settings.Default.ServerLetterOfAttorney = serverLetterOfAttoney;
			Properties.Settings.Default.DbLetterOfAttorney = dbLetterOfAttoney;
			Properties.Settings.Default.ServerEmployees = serverEmployees;
			Properties.Settings.Default.DbEmployees = dbEmployees;
			Properties.Settings.Default.ServerConsumers = serverConsumers;
			Properties.Settings.Default.DbConsumers = dbConsumers;

			Properties.Settings.Default.IsRunInFullscreen = isRunInFullscreen;
			Properties.Settings.Default.DefaultLoaCountDaysValidity = defaultLoaCountDaysValidity;
			Properties.Settings.Default.IsShowDismissalEmployeesInTable = isShowDismissalEmployeesInTable;
			Properties.Settings.Default.TimeoutServerConnectInSecond = timeoutServerConnectInSecond;

			Properties.Settings.Default.Save();
			Close();
		}

		/// <summary>
		/// Нажатие кнопки [Отмена (Закрыть окно)]
		/// </summary>
		private void CloseButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			Close();
		}

		/// <summary>
		/// Обработка нажатия клавиш в окне - [Esc] для закрытия
		/// </summary>
		private void Window_OnPreviewEscapeKeyDownCloseWindow(object senderIsWindow, KeyEventArgs eventArgs)
		{
			if (eventArgs.Key != Key.Escape)
			{
				return;
			}
			eventArgs.Handled = true;
			Close();
		}
	}
}
