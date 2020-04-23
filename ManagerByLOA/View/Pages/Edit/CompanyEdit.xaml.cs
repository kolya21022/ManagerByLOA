using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.Services;
using ManagerByLetterOfAttorney.View.Util;
using ManagerByLetterOfAttorney.Entities.Internal;
using ManagerByLetterOfAttorney.View.Pages.TableView;

namespace ManagerByLetterOfAttorney.View.Pages.Edit
{
	/// <summary>
	/// Страница редактирования/добавления [Организации]
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class CompanyEdit : IPageable
	{
		/// <summary>
		/// Текущая организация в режиме редактирования
		/// </summary>
		private readonly Company _editedCompany;

		/// <summary>
		/// Конструктор режима добавления
		/// </summary>
		/// <inheritdoc />
		public CompanyEdit()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Конструктор режима редактирования
		/// </summary>
		/// <inheritdoc />
		public CompanyEdit(Company editedCompany)
		{
			_editedCompany = editedCompany;
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Указание значений полей при редактировании и указание соответсвующих режиму надписей.
		/// </summary>
		/// <inheritdoc />
		public void AdditionalInitializeComponent()
		{
			if (_editedCompany == null)
			{
				EditingTypeLabel.Content = PageLiterals.EditPageTypeAdd;
			}
			else
			{
				EditingTypeLabel.Content = PageLiterals.EditPageTypeEdit;
				NameTextBox.Text = _editedCompany.Name;
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
			ConfirmExitIfDataHasBeenChanged();
		}

		/// <summary>
		/// Перевод клавиатурного фокуса к следующему полю ввода при нажатии Enter.
		/// </summary>
		private void JumpToNextWhenPressEnter_OnKeyDown(object senderIsTextBox, KeyEventArgs eventArgs)
		{
			PageUtil.JumpToNextWhenPressEnter_OnKeyDown(eventArgs);
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
			// Получение значений полей
			var name = NameTextBox.Text.Trim();

			// Валидация полей
			var isValidFields = IsValidFieldsWithShowMessageOtherwise();
			if (!isValidFields)
			{
				return;
			}
			try
			{
				// Если режим добавления - вставка в БД, если редактирования - обновление в БД
				var company = _editedCompany == null 
					? CompaniesService.InsertInternal(name) 
					: CompaniesService.UpdateInternal(_editedCompany.Id, name);
				PageSwitcher.Switch(new CompaniesTable(company));
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

			var isValid = true;
			var errorMessages = new StringBuilder();
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(name, fieldName, 150, errorMessages);
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
			if (_editedCompany == null)     // Если сущность добавляется
			{
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(NameTextBox.Text);
			}
			else                            // Если сущность редактируется
			{
				isFieldsNotChanged &= Equals(NameTextBox.Text.Trim(), _editedCompany.Name);
			}
			// Если введённые поля изменились - запрос у пользователя подтверждения намерение выхода к списку сущностей
			if (!isFieldsNotChanged && !PageUtil.ConfirmBackToListWhenFieldChanged())
			{
				return;
			}
			PageSwitcher.Switch(new CompaniesTable(_editedCompany));
		}
	}
}
