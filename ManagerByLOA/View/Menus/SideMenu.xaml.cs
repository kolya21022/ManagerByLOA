using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.Services;
using ManagerByLetterOfAttorney.View.Util;
using ManagerByLetterOfAttorney.View.Pages.Edit;
using ManagerByLetterOfAttorney.Entities.Internal;
using ManagerByLetterOfAttorney.View.Pages.Reports;
using ManagerByLetterOfAttorney.View.Pages.TableView;

namespace ManagerByLetterOfAttorney.View.Menus
{
	/// <summary>
	/// Боковое меню главного окна приложения
	/// </summary>
	/// <inheritdoc cref="UserControl" />
	public partial class SideMenu
	{
		// Экземляр этого класса, для обновления порядкового номера последней доверенности для печати.
		private static SideMenu _instance;

		public SideMenu()
		{
			InitializeComponent();
			VisualInitializeComponent();
			AdditionalInitializeComponent();
		}

		/// <summary>
		/// Загрузка и отображение порядкового номера последней доверенности для печати и текущего года выдачи.
		/// </summary>
		private void AdditionalInitializeComponent()
		{
			_instance = this;
			UpdatePrintedLoaOrdinalNumber();
			LoadAndDisplayCurrentYear();
		}

		/// <summary>
		/// Визуальная инициализация меню (цвета и размеры шрифтов контролов)
		/// </summary>
		private void VisualInitializeComponent()
		{
			// Экспандеры, вложенные в них StackPanel и вложенные в них Buttons
			var expanders = WrapperStackPanel.Children.OfType<Expander>();
			foreach (var expander in expanders)
			{
				expander.Background = Constants.BackColor4_BlueBayoux;
				expander.BorderBrush = Constants.LineBorderColor2_Nepal;
				expander.Foreground = Constants.ForeColor2_PapayaWhip;
				expander.FontSize = Constants.FontSize;

				var stackPanel = expander.Content as StackPanel;
				if (stackPanel == null)
				{
					continue;
				}
				stackPanel.Background = Constants.BackColor4_BlueBayoux;

				var buttons = new List<Button>();
				buttons.AddRange(stackPanel.Children.OfType<Button>());
				buttons.Add(ViewReportButton);
				foreach (var button in buttons)
				{
					button.Foreground = Constants.ForeColor1_BigStone;
				}
			}
			// Панель текущего года выдачи доверенностей
			CurrentYearTextBlock.Background = Constants.BackColor3_SanJuan;
			CurrentYearTextBlock.Foreground = Constants.ForeColor2_PapayaWhip;
		}

		/// <summary>
		/// Нажатие кнопки [Новая доверенность]
		/// </summary>
		private void AddLetterOfAttorneyButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			PageSwitcher.Switch(new LetterOfAttorneyEdit());
		}

		/// <summary>
		/// Нажатие кнопки [Список доверенностей]
		/// </summary>
		private void LetterOfAttorneysTableButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			 PageSwitcher.Switch(new LetterOfAttorneysTable());
		}

		/// <summary>
		/// Нажатие кнопки [Печать доверенности №]
		/// </summary>
		private void ReportButton_Click(object senderIsButton, RoutedEventArgs eventArgs)
		{
			const string noSpecifiedOrdinalMessage = "Не указан номер доверенности для печати";
			const string specifiedLoaDontExistPattern = "Доверенности с порядковым номером [{0}] нет в базе данных";

			// Проверка указанного порядкового номера
			var nullableOrdinalNumber = OrdinalLongUpDown.Value;
			if (nullableOrdinalNumber == null)
			{
				MessageBox.Show(noSpecifiedOrdinalMessage, PageLiterals.HeaderLogicError,
					MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
				return;
			}
			var ordinalNumber = (long)nullableOrdinalNumber;

			// Получение доверенности с указанным порядковым номером из базы
			LetterOfAttorney letterOfAttorney;
			try
			{
				letterOfAttorney = LetterOfAttorneysService.GetByOrdinalNumber(ordinalNumber);
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
				return;
			}
			if (letterOfAttorney == null)
			{
				var message = string.Format(specifiedLoaDontExistPattern, ordinalNumber);
				MessageBox.Show(message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Переход на страницу печати
			PageSwitcher.Switch(new LoaReportPage(letterOfAttorney));
		}

		/// <summary>
		/// Нажатие кнопки [Паспорта курьеров]
		/// </summary>
		private void CouriersTableButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			PageSwitcher.Switch(new CouriersTable());
		}

		/// <summary>
		/// Нажатие кнопки [Организации (внутренние)]
		/// </summary>
		private void CompaniesTableButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			PageSwitcher.Switch(new CompaniesTable());
		}

		/// <summary>
		/// Нажатие кнопки [Товарно-материальные ценности]
		/// </summary>
		private void CargoesTableButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			PageSwitcher.Switch(new CargoesTable());
		}

		/// <summary>
		/// Нажатие кнопки [Единицы измерения]
		/// </summary>
		private void MeasuresTableButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			PageSwitcher.Switch(new MeasuresTable());
		}

		/// <summary>
		/// Нажатие кнопки [Паспорта сотрудников]
		/// </summary>
		private void EmployeesTableButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			 PageSwitcher.Switch(new EmployeesTable());
		}

		/// <summary>
		/// Нажатие кнопки [Организации (внешние)]
		/// </summary>
		private void CompaniesWithUnpTableButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			PageSwitcher.Switch(new CompaniesWithUnpTable());
		}

		/// <summary>
		/// Нажатие кнопки [Довереренности предыдущих лет]
		/// </summary>
		private void PreviousYearsHistoryTableButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			PageSwitcher.Switch(new PrevYearsHistoryTable());
		}

		/// <summary>
		/// Загрузка из базы данных и отображение текущего года выдачи доверенностей.
		/// </summary>
		private void LoadAndDisplayCurrentYear()
		{
			try
			{
				CurentYearRun.Text = ExportAndHistoryService.GetCurrentYearFromDb().ToString();
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
			}
		}

		/// <summary>
		/// Обновление порядкового номера последней доверенности года для печати.
		/// </summary>
		public static void UpdatePrintedLoaOrdinalNumber()
		{
			if (_instance == null)
			{
				const string cause = "Ошибка инициализации бокового меню приложения";
				var message = string.Format(PageLiterals.LogicErrorPattern, cause);
				throw new ApplicationException(message);
			}
			long? maxOrdinalNumber;
			try
			{
				// Получение из базы текущего максимального порядкового номера доверенности
				maxOrdinalNumber = LetterOfAttorneysService.GetMaxOrdinalNumber();
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
				return;
			}
			if (maxOrdinalNumber == null)
			{
				_instance.OrdinalLongUpDown.Value = null;
				_instance.OrdinalLongUpDown.IsEnabled = false;
			}
			else
			{
				_instance.OrdinalLongUpDown.Minimum = 1;
				_instance.OrdinalLongUpDown.Maximum = maxOrdinalNumber;
				_instance.OrdinalLongUpDown.Value = maxOrdinalNumber;
			}
		}
	}
}
