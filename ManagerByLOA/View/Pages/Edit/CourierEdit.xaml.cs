using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.Services;
using ManagerByLetterOfAttorney.View.Util;
using ManagerByLetterOfAttorney.Entities.Enums;
using ManagerByLetterOfAttorney.Entities.Internal;
using ManagerByLetterOfAttorney.View.Pages.TableView;

namespace ManagerByLetterOfAttorney.View.Pages.Edit
{
	/// <summary>
	/// Страница редактирования/добавления [Курьера]
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class CourierEdit : IPageable
	{
		/// <summary>
		/// Action-делегат закрытия дополнительного окна добавления/редактирования, когда редактирование не в главном
		/// </summary>
		private readonly Action<Courier> _actionDelegateCloseExtraWindow; 

		/// <summary>
		/// Текущий курьера в режиме редактирования
		/// </summary>
		private readonly Courier _editedCourier;

		/// <summary>
		/// Конструктор режима добавления 
		/// </summary>
		/// <param name="actionDelegateCloseExtraWindow">Action-делегат закрытия дополнительного окна</param>
		/// <inheritdoc />
		public CourierEdit(Action<Courier> actionDelegateCloseExtraWindow)
		{
			_actionDelegateCloseExtraWindow = actionDelegateCloseExtraWindow;
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Конструктор режима редактирования
		/// </summary>
		/// <param name="actionDelegateCloseExtraWindow">Action-делегат закрытия дополнительного окна</param>
		/// <param name="editedCourier">Курьер для редактирования</param>
		/// <inheritdoc />
		public CourierEdit(Action<Courier> actionDelegateCloseExtraWindow, Courier editedCourier)
		{
			_actionDelegateCloseExtraWindow = actionDelegateCloseExtraWindow;
			_editedCourier = editedCourier;
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
			try
			{
				// В зависимости от типа места размещения страницы (главное или дополнительное окно), загружается 
				// нужная иконка кнопки закрытия из ресурсного словаря приложения и соответсвующая подпись
				const string backIcon = "BackSvgIcon";
				const string closeIcon = "CloseSvgIcon";
				const string backMessage = "(Выйти к списку)";
				const string closeMessage = "(Закрыть окно)";
				var isExtraWindow = _actionDelegateCloseExtraWindow != null;
				AnnotateCloseButtonTextBlock.Text = isExtraWindow ? closeMessage : backMessage;
				var closeButtonIcon = (Visual) (isExtraWindow ? FindResource(closeIcon) : FindResource(backIcon));
				CloseButtonRectangle.Fill = new VisualBrush {Visual = closeButtonIcon, Stretch = Stretch.Uniform};
			}
			catch (Exception)
			{
				 /* ignored resource load exceptions */
			}

			if (_editedCourier == null)
			{
				// Режим добавления
				EditingTypeLabel.Content = PageLiterals.EditPageTypeAdd;
			}
			else
			{
				// Режим редактирования
				EditingTypeLabel.Content = PageLiterals.EditPageTypeEdit;
				LastNameTextBox.Text = _editedCourier.LastName;
				FirstNameTextBox.Text = _editedCourier.FirstName;
				MiddleNameTextBox.Text = _editedCourier.MiddleName;
				if (_editedCourier.Sex == Sex.Male)
				{
					MaleRadioButton.IsChecked = true;
				}
				else
				{
					FemaleRadioButton.IsChecked = true;
				}
				PassportSeriesAndNumberTextBox.Text = _editedCourier.PassportSeriesAndNumber;
				PassportIssuedByOrganizationTextBox.Text = _editedCourier.PassportIssuedByOrganization;
				PassportIssueDateDatePicker.SelectedDate = _editedCourier.PassportIssueDate;
				ProfessionTextBox.Text = _editedCourier.Profession;
			}
			LastNameTextBox.Focus();
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
			var closePageOrWindow = _actionDelegateCloseExtraWindow == null 
				? PageLiterals.HotkeyLabelClosePageBackToList 
				: PageLiterals.HotkeyLabelCloseWindow;
			const string separator = PageLiterals.HotkeyLabelsSeparator;
			var displayed = jumpNext + separator + closePageOrWindow;
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
		/// Нажатие кнопки [Сохранить]
		/// </summary>
		private void SaveButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			SaveAndExit();
		}

		/// <summary>
		/// Нажатие кнопки [Отмена] - выход к списку или закрытия дополнительного окна (зависимосит от Action-делегата)
		/// </summary>
		private void CancelButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			ConfirmExitIfDataHasBeenChanged(); // проверка изменений полей и запрос подтверждения выхода, если изменены
		}

		/// <summary>
		/// Валидация, сохранение и выход к списку сущностей (или закрытия дополнительного окна)
		/// </summary>
		private void SaveAndExit()
		{
			// Получение значений и подписей полей
			var fieldLastName = LastNameLabel.Content.ToString();
			var fieldFirstName = FirstNameLabel.Content.ToString();
			var fieldMiddleName = MiddleNameLabel.Content.ToString();
			var fieldPassportIssuedByOrg = PassportIssuedByOrganizationLabel.Content.ToString();
			var fieldProfession = ProfessionLabel.Content.ToString();

			var lastName = LastNameTextBox.Text.Trim();
			var firstName = FirstNameTextBox.Text.Trim();
			var middleName = MiddleNameTextBox.Text.Trim();
			var nullableSex = MaleRadioButton.IsChecked == true ? Sex.Male
				: (FemaleRadioButton.IsChecked == true ? Sex.Female : (Sex?)null);
			var passportSeriesAndNumber = PassportSeriesAndNumberTextBox.Text.Trim();
			var passportIssuedByOrg = PassportIssuedByOrganizationTextBox.Text.Trim();
			var nullablePassportIssueDate = PassportIssueDateDatePicker.SelectedDate;
			var profession = ProfessionTextBox.Text.Trim();
			var nullableEditedId = _editedCourier == null || _editedCourier.Id == null ? null : _editedCourier.Id;

			// Валидация полей
			var isValidFields = IsValidFieldsWithShowMessageOtherwise();
			if (!isValidFields)
			{
				return;
			}

			// Если фамилия, имя, отчество, должность или орган выдавший паспорт содержат 
			// некиррилические символы - получение подтверждений пользователя
			if (!Validator.IsCyrillicWithUserConfirmOtherwise(lastName, fieldLastName) || 
			    !Validator.IsCyrillicWithUserConfirmOtherwise(firstName, fieldFirstName) || 
				!Validator.IsCyrillicWithUserConfirmOtherwise(middleName, fieldMiddleName) || 
			    !Validator.IsCyrillicWithUserConfirmOtherwise(passportIssuedByOrg, fieldPassportIssuedByOrg) || 
			    !Validator.IsCyrillicWithUserConfirmOtherwise(profession, fieldProfession))
			{
				return;
			}
			
			// Проверка nullable DateTime и Sex - после валидации не нужно, но статический анализатор IDE ругается
			if (nullablePassportIssueDate == null || nullableSex == null)
			{
				const string message = "Дата или пол не должны быть NULL после валидации";
				throw new ApplicationException(string.Format(PageLiterals.LogicErrorPattern, message));
			}
			var passportIssueDate = (DateTime)nullablePassportIssueDate;
			var sex = (Sex) nullableSex;

			try
			{
				Courier courier;
				if (_editedCourier == null)
				{
					// Режим добавления - вставка записи в БД
					courier = CouriersService.Insert(lastName, firstName, middleName, sex, 
						passportSeriesAndNumber, passportIssuedByOrg, passportIssueDate, profession);
				}
				else
				{
					// Проверка nullable long - по идее не нужна, но статический анализатор IDE ругается
					if (nullableEditedId == null)
					{
						const string message = "ID курьера при редактировании не должен быть NULL";
						throw new ApplicationException(string.Format(PageLiterals.LogicErrorPattern, message));
					}
					var editedCourierId = (long)nullableEditedId;

					// Режим редактирования - обновления записи в БД
					courier = CouriersService.Update(editedCourierId, lastName, firstName, 
						middleName, sex, passportSeriesAndNumber, passportIssuedByOrg, 
						passportIssueDate, profession);
				}

				// Если Action-делегат закрытия дополнительного окна существует - вызывается он, иначе возврат к списку
				if (_actionDelegateCloseExtraWindow != null)
				{
					_actionDelegateCloseExtraWindow(courier);
				}
				else
				{
					PageSwitcher.Switch(new CouriersTable(courier));
				}
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
			var lastName = LastNameTextBox.Text.Trim();                                     // Фамилия 
			var fieldLastName = LastNameLabel.Content.ToString();
			var firstName = FirstNameTextBox.Text.Trim();                                   // Имя 
			var fieldFirstName = FirstNameLabel.Content.ToString();
			var middleName = MiddleNameTextBox.Text.Trim();                                 // Отчество 
			var fieldMiddleName = MiddleNameLabel.Content.ToString();
			var sex = MaleRadioButton.IsChecked == true ? Sex.Male                          // Пол 
				: (FemaleRadioButton.IsChecked == true ? Sex.Female : (Sex?)null);
			var fieldSex = SexRadioButtonLabel.Content.ToString();
			var passportSeries = PassportSeriesAndNumberTextBox.Text.Trim();                // Серия и номер паспорта 
			var fieldPassportSeries = PassportSeriesAndNumberLabel.Content.ToString();
			var passportIssued = PassportIssuedByOrganizationTextBox.Text.Trim();           // Ограниз. выд. паспорт 
			var fieldPassportIssued = PassportIssuedByOrganizationLabel.Content.ToString();
			var nullablePassportDate = PassportIssueDateDatePicker.SelectedDate;            // Дата выдачи паспорта 
			var fieldPassportDate = PassportIssueDateLabel.Content.ToString();
			var fieldProfession = ProfessionLabel.Content.ToString();                       // Профессия (должность) 
			var profession = ProfessionTextBox.Text.Trim();

			var isValid = true;
			var errorMessages = new StringBuilder();
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(lastName, fieldLastName, 150, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(firstName, fieldFirstName, 150, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(middleName, fieldMiddleName, 150, errorMessages);
			isValid &= Validator.IsNotNullSelectedObject(sex, fieldSex, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(passportSeries, fieldPassportSeries, 150, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(passportIssued, fieldPassportIssued, 150, errorMessages);
			isValid &= Validator.IsNotNullSelectedObject(nullablePassportDate, fieldPassportDate, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(profession, fieldProfession, 150, errorMessages);

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
		/// Проверка, изменились ли поля ввода, и запрос подтверждения, если изменились. 
		/// Далее выход к списку сущностей (или закрытия дополнительного окна)
		/// </summary>
		private void ConfirmExitIfDataHasBeenChanged()
		{
			var isFieldsNotChanged = true;
			if (_editedCourier == null)     // Если сущность добавляется
			{
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(LastNameTextBox.Text);
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(FirstNameTextBox.Text);
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(MiddleNameTextBox.Text);
				isFieldsNotChanged &= MaleRadioButton.IsChecked == null || MaleRadioButton.IsChecked == false;
				isFieldsNotChanged &= FemaleRadioButton.IsChecked == null || FemaleRadioButton.IsChecked == false;
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(PassportSeriesAndNumberTextBox.Text);
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(PassportIssuedByOrganizationTextBox.Text);
				isFieldsNotChanged &= PassportIssueDateDatePicker.SelectedDate == null;
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(ProfessionTextBox.Text);
			}
			else                            // Если сущность редактируется
			{
				isFieldsNotChanged &= Equals(LastNameTextBox.Text.Trim(), _editedCourier.LastName);
				isFieldsNotChanged &= Equals(FirstNameTextBox.Text.Trim(), _editedCourier.FirstName);
				isFieldsNotChanged &= Equals(MiddleNameTextBox.Text.Trim(), _editedCourier.MiddleName);
				if (_editedCourier.Sex == Sex.Male)
				{
					isFieldsNotChanged &= MaleRadioButton.IsChecked == true;
				}
				else
				{
					isFieldsNotChanged &= FemaleRadioButton.IsChecked == true;
				}
				isFieldsNotChanged &= Equals(PassportSeriesAndNumberTextBox.Text.Trim(), 
					_editedCourier.PassportSeriesAndNumber);
				isFieldsNotChanged &= Equals(PassportIssuedByOrganizationTextBox.Text.Trim(), 
					_editedCourier.PassportIssuedByOrganization);
				isFieldsNotChanged &= Equals(PassportIssueDateDatePicker.SelectedDate, 
					_editedCourier.PassportIssueDate);
				isFieldsNotChanged &= Equals(ProfessionTextBox.Text.Trim(), _editedCourier.Profession);
			}
			// Если введённые поля изменились - запрос у пользователя подтверждения намерение выхода к списку сущностей
			if (!isFieldsNotChanged && !PageUtil.ConfirmBackToListWhenFieldChanged())
			{
				return;
			}
			// Если Action-делегат закрытия дополнительного окна существует - вызывается он, иначе возврат к списку
			if (_actionDelegateCloseExtraWindow != null)
			{
				_actionDelegateCloseExtraWindow(_editedCourier);
			}
			else
			{
				PageSwitcher.Switch(new CouriersTable(_editedCourier));
			}
		}

		/// <summary>
		/// Обработка нажатия [Enter] или [Down] в фокусе ввода RadioButton (Пол курьера)
		/// </summary>
		private void RadioButton_OnKeyDown(object senderIsRadioButton, KeyEventArgs eventArgs)
		{
			var radioButton = senderIsRadioButton as RadioButton;
			if (radioButton == null)
			{
				return;
			}
			if (eventArgs.Key != Key.Enter && eventArgs.Key != Key.Down)
			{
				return;
			}
			eventArgs.Handled = true;
			if (eventArgs.Key == Key.Enter)           // Если нажат [Enter] - выбор текущего RadioButton
			{
				radioButton.IsChecked = true;
			}
			PageUtil.JumpToDown_OnKeyDown(eventArgs); // Перевод клавиатурного фокуса ввода на нижележащее поле
		}
	}
}
