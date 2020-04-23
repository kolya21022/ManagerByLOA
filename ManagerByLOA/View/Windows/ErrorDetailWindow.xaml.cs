using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.View.Util;

namespace ManagerByLetterOfAttorney.View.Windows
{
	/// <summary>
	/// Окно отображение детальной информации со стектрейсом о выброшеном исключении. 
	/// Для StorageException в отдельном поле отображается 'возможная причина'.
	/// </summary>
	/// <inheritdoc cref="Window" />
	public partial class ErrorDetailWindow
	{
		public ErrorDetailWindow(Exception ex)
		{
			InitializeComponent();
			AdditionalInitializeComponent(ex);
			VisualInitializeComponent();
		}

		/// <summary>
		/// Визуальная инициализация окна (цвета и размеры шрифтов контролов)
		/// </summary>
		private void VisualInitializeComponent()
		{
			Background = Constants.BackColor2_Botticelli;
			Foreground = Constants.ForeColor2_PapayaWhip;
			FontSize = Constants.FontSize;

			// Панель хоткеев
			HotkeysDockPanel.Background = Constants.BackColor4_BlueBayoux;
			var helpLabels = HotkeysDockPanel.Children.OfType<Label>();
			foreach (var helpLabel in helpLabels)
			{
				helpLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}
		}

		/// <summary>
		/// Получение и отображение сообщений/возможных причин/стектрейса в нужные поля ввода и надписей хоткеев
		/// </summary>
		private void AdditionalInitializeComponent(Exception ex)
		{
			const string labelCloseWindow = PageLiterals.HotkeyLabelCloseWindow;
			HotkeysTextBlock.Text = labelCloseWindow;

			var messages = ExceptionMessages(ex);        // Получение и отображение сообщений/возможных причин
			ShowMessages(messages);
			StackTraceTextBox.AppendText(ex.ToString()); // Показ стектрейса

			CloseButton.Focus();
		}

		/// <summary>
		/// Получение списка сообщений/возможных причин возникновения исключения, включая все вложенные исключения
		/// </summary>
		private static List<string> ExceptionMessages(Exception ex)
		{
			var messages = new List<string>();
			var currentEx = ex;
			do
			{
				var currentExStorage = currentEx as StorageException;
				if (currentExStorage != null && !string.IsNullOrWhiteSpace(currentExStorage.ProbableCause))
				{
					var probableCause = currentExStorage.ProbableCause;
					if (!messages.Contains(probableCause))
					{
						messages.Add(probableCause);
					}
				}
				if (!string.IsNullOrWhiteSpace(currentEx.Message))
				{
					var message = currentEx.Message;
					if (!messages.Contains(message))
					{
						messages.Add(message);
					}
				}
				currentEx = currentEx.InnerException; // следующее вложенное исключение
			} while (currentEx != null);
			return messages;
		}

		/// <summary>
		/// Отображение сообщений/возможных причин возникновения исключения в ранее скрытых поля вывода
		/// </summary>
		private void ShowMessages(IList<string> messages)
		{
			var textBoxes = FieldsWrapperGrid.Children.OfType<TextBox>().ToList(); // все TextBox'ы окна
			for (var i = 0; i < textBoxes.Count; i++)
			{
				var textBox = textBoxes[i];
				if (messages.Count > i)
				{
					textBox.Visibility = Visibility.Visible;
					textBox.Text = messages[i];
				}
				else
				{
					break;
				}
			}
		}

		/// <summary>
		/// Нажатие кнопки [Закрыть окно]
		/// </summary>
		private void CloseButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			Close();
		}

		/// <summary>
		/// Обработка нажатия клавиш в окне - [Esc] для закрытия
		/// </summary>
		private void Window_OnPreviewKeyDown(object senderIsWindow, KeyEventArgs eventArgs)
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
