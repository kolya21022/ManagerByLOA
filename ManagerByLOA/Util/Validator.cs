using System;
using System.Text;
using System.Windows;
using System.Text.RegularExpressions;

using ManagerByLetterOfAttorney.View.Util;

namespace ManagerByLetterOfAttorney.Util
{
	/// <summary>
	/// Утилитарный класс с методами валидации (проверки корректности данных)
	/// </summary>
	public static class Validator
	{
		private const string ErrorLineSizePattern = "Кол-во символов поля [{0}] должно быть не больше [{1}] символов";
		private const string ErrorEmptyOrAbsentPattern = "Значение поля [{0}] не указано/не выбрано";

		/// <summary>
		/// Валидация не пустой строки, с числом символов не больше указанного.
		/// В случае несоответствия уловиям (false), в errorMessages заносится сообщение об ошибке.
		/// </summary>
		public static bool IsLineNotEmptyAndSizeNoMore(string value, string fieldName, int maxLength, 
			StringBuilder errorMessages)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				var messageEmptyValue = string.Format(ErrorEmptyOrAbsentPattern, fieldName);
				errorMessages.AppendLine(messageEmptyValue);
				return false;
			}
			if (maxLength >= value.Trim().Length)
			{
				return true;
			}
			var messageSizeError = string.Format(ErrorLineSizePattern, fieldName, maxLength);
			errorMessages.AppendLine(messageSizeError);
			return false;
		}

		/// <summary>
		/// Валидация строки, которая может быть пустой, но с числом символов не больше указанного.
		/// В случае несоответствия уловиям (false), в errorMessages заносится сообщение об ошибке.
		/// </summary>
		public static bool IsLineMightEmptyAndSizeNoMore(string value, string fieldName, int maxLength, 
			StringBuilder errorMessages)
		{
			if (string.IsNullOrWhiteSpace(value) || maxLength >= value.Trim().Length)
			{
				return true;
			}
			var message = string.Format(ErrorLineSizePattern, fieldName, maxLength);
			errorMessages.AppendLine(message);
			return false;
		}

		/// <summary>
		/// Валидация объекта на null.
		/// В случае если объект null (возвращается false), в errorMessages заносится сообщение об ошибке.
		/// </summary>
		public static bool IsNotNullSelectedObject(object value, string fieldName, StringBuilder errorMessages)
		{
			if (value != null)
			{
				return true;
			}
			var message = string.Format(ErrorEmptyOrAbsentPattern, fieldName);
			errorMessages.AppendLine(message);
			return false;
		}

		/// <summary>
		/// Проверка состоит ли строка только из символов кириллицы и пробелов, и запрос подтверждения продолжения у 
		/// пользователя в противном случае. Для пустой строки или состоящей только из пробелов, возвращается true.
		/// </summary>
		/// <returns>Возвращает true в случае, если поле состоит только из кириллицы или пользователь подтвердил, 
		/// что он и хотел некириллические символы</returns>
		public static bool IsCyrillicWithUserConfirmOtherwise(string value, string fieldName)
		{
			var newLine = Environment.NewLine;
			var confirmPattern = "Поле [{0}] содержит некириллические символы, цифры или иные нестандартные символы." +
			                     newLine + "Вы действительно хотите оставить такое значение?" + newLine + "「{1}」";
			const string cyrillicRegexFormatPattern = "^[\\p{{IsCyrillic}}\\s\\p{{P}}]{{{0},{1}}}$";
			const int maxLength = int.MaxValue;
			const int minLength = 0;

			var cyrillicRegexPattern = string.Format(cyrillicRegexFormatPattern, minLength, maxLength);
			var regex = new Regex(cyrillicRegexPattern);
			var match = regex.Match(value.Trim());
			if (match.Success)
			{
				return true;
			}
			const MessageBoxImage messageType = MessageBoxImage.Asterisk;
			const MessageBoxButton messageButtons = MessageBoxButton.OKCancel;
			var confirmMessage = string.Format(confirmPattern, fieldName, value);
			var result = MessageBox.Show(confirmMessage, PageLiterals.HeaderConfirm, messageButtons, messageType);
			return result == MessageBoxResult.OK;
		}
	}
}
