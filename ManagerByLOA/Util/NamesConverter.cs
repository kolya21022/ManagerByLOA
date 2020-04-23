using System;
using ManagerByLetterOfAttorney.Entities.Enums;

namespace ManagerByLetterOfAttorney.Util
{
	/// <summary>
	/// Утилитарный класс преобразования ФИО (дательный падеж, формирование фамилии с инициалами из полного ФИО)
	/// </summary>
	public static class NamesConverter
	{
		/// <summary>
		/// Получение ФИО пользователя в дательном падеже из именительного.
		/// В случае ошибок преобразования, в out параметр warnings записывается текст предупреждения, 
		/// а ФИО остаётся в именительном падеже.
		/// </summary>
		public static string FullnameInDativeCase(string lastName, string firstName, string middleName, 
			Sex? nullablesex, out string warnings)
		{
			const string separator = " ";
			const string isEmptyMessage = "Фамилия, имя или отчество не указаны, или состоят из одних пробелов, " +
			    "или состоят из 1 символа, что недопустимо для алгоритма преобразования в дательный падеж, " +
			    "и остаются без изменений";
			const string isSexNotSpecified = "Пол сотрудника/курьера не указан, что недопустимо для алгоритма " +
			    "преобразования в дательный падеж, и ФИО остаются без изменений";
			
			var isEmptyLastName = string.IsNullOrWhiteSpace(lastName) || lastName.Length <= 1;
			var isEmptyFirstName = string.IsNullOrWhiteSpace(firstName) || firstName.Length <= 1;
			var isEmptyMiddleName = string.IsNullOrWhiteSpace(middleName) || middleName.Length <= 1;
			
			// Какое-то из значений ФИО пустое или состоит из 1 символа - ФИО не изменяется и пишется предупреждение
			if (isEmptyLastName || isEmptyFirstName || isEmptyMiddleName)
			{
				warnings = isEmptyMessage;
				return (isEmptyLastName ? string.Empty : lastName.Trim() + separator) +
					   (isEmptyFirstName ? string.Empty : firstName.Trim() + separator) +
					   (isEmptyMiddleName ? string.Empty : middleName.Trim() + separator).Trim();
			}
			// Пол не указан - ФИО не изменяется и пишется предупреждение
			if (nullablesex == null)
			{
				warnings = isSexNotSpecified;
				return lastName.Trim() + separator + firstName.Trim() + separator 
				       + (middleName.Trim() + separator).Trim();
			}
			
			var sex = (Sex)nullablesex;
			var dativeLastName = LastNameInDative(lastName, sex);       // Фамилия в дательном падеже
			var dativeFirstName = FirstNameInDative(firstName, sex);    // Имя в дательном падеже
			var dativeMiddleName = MiddleNameInDative(middleName, sex); // Отчество в дательном падеже

			warnings = string.Empty;
			return string.Format(Constants.EmployeeFullNamePattern, dativeLastName, dativeFirstName, dativeMiddleName);
		}

		/// <summary>
		/// Фамилия в дательном падеже.
		/// </summary>
		private static string LastNameInDative(string original, Sex sex)
		{
			const char markerOfCompoundName = '-';
			original = original.Trim();

			// составные фамилии разделяются в массив и рекурсивным вызовом преобразуется каждое слово отдельно
			if (original.Contains(markerOfCompoundName.ToString()))
			{
				var parts = original.Split(markerOfCompoundName);
				var resultCompaund = string.Empty;
				for (var i = 0; i < parts.Length; i++)
				{
					resultCompaund += LastNameInDative(parts[i], sex);
					if (i != parts.Length - 1)
					{
						resultCompaund += markerOfCompoundName;
					}
				}
				return resultCompaund;
			}
			switch (sex)
			{
				case Sex.Male:
					if (IsWordEndOn(original, "о") || IsWordEndOn(original, "у"))
					{
						return original;
					}
					else if (IsWordEndOn(original, "а") || IsWordEndOn(original, "я"))
					{
						return ReplaceEnd(original, "е");
					}
					else if (IsWordEndOn(original, "ый") || IsWordEndOn(original, "ий") || IsWordEndOn(original, "ой"))
					{
						return ReplaceEnd(original, "ому", 2);
					}
					else if (IsWordEndOn(original, "бей"))
					{
						return ReplaceEnd(original, "бью");
					}
					else if (IsWordEndOn(original, "й"))
					{
						return ReplaceEnd(original, "ю");
					}
					else if (IsWordEndOn(original, "ок"))
					{
						return ReplaceEnd(original, "ку");
					}
					else if (IsWordEndOn(original, "лец"))
					{
						return ReplaceEnd(original, "льцу", 3);
					}
					else if (IsWordEndOn(original, "нец"))
					{
						return ReplaceEnd(original, "нецу", 3);
					}
					else if (IsWordEndOn(original, "ец"))
					{
						return ReplaceEnd(original, "цу");
					}
					else
					{
						return ReplaceEnd(original, "у", 0);
					}
				case Sex.Female:
					if (IsWordEndOn(original, "ва"))
					{
						return ReplaceEnd(original, "вой", 2);
					}
					else if (IsWordEndOn(original, "на"))
					{
						return ReplaceEnd(original, "ной", 2);
					}
					else if (IsWordEndOn(original, "ая"))
					{
						return ReplaceEnd(original, "ой");
					}
					else
					{
						return original;
					}
				default:
					throw new ApplicationException(sex.ToString());
			}
		}

		/// <summary>
		/// Имя в дательном падеже
		/// </summary>
		private static string FirstNameInDative(string original, Sex sex)
		{
			const char markerOfCompoundName = '-';
			original = original.Trim();

			// составные имена разделяются в массив и рекурсивным вызовом преобразуется каждое слово отдельно
			if (original.Contains(markerOfCompoundName.ToString()))
			{
				var parts = original.Split(markerOfCompoundName);
				var resultCompaund = string.Empty;
				for (var i = 0; i < parts.Length; i++)
				{
					resultCompaund += FirstNameInDative(parts[i], sex);
					if (i != parts.Length - 1)
					{
						resultCompaund += markerOfCompoundName;
					}
				}
				return resultCompaund;
			}
			switch (sex)
			{
				case Sex.Male:
					if (IsWordEndOn(original, "й"))
					{
						return ReplaceEnd(original, "ю");
					}
					else if (IsWordEndOn(original, "ел"))
					{
						return ReplaceEnd(original, "лу");
					}
					else if (IsWordEndOn(original, "ь"))
					{
						return ReplaceEnd(original, "ю");
					}
					else if (IsWordEndOn(original, "я"))
					{
						return ReplaceEnd(original, "е");
					}
					else
					{
						return ReplaceEnd(original, "у", 0);
					}
				case Sex.Female:
					if (IsWordEndOn(original, "ь"))
					{
						return ReplaceEnd(original, "и");
					}
					else
					{
						return ReplaceEnd(original, "е");
					}
				default:
					throw new ApplicationException(sex.ToString());
			}
		}

		/// <summary>
		/// Отчество в дательном падеже
		/// </summary>
		private static string MiddleNameInDative(string original, Sex sex)
		{
			original = original.Trim();
			switch (sex)
			{
				case Sex.Male:
					return ReplaceEnd(original, "у", 0);
				case Sex.Female:
					return ReplaceEnd(original, "е");
				default:
					throw new ApplicationException(sex.ToString());
			}
		}

		/// <summary>
		/// Проверка оканчивается ли слово на указанное окончание
		/// </summary>
		private static bool IsWordEndOn(string word, string ended)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			var countEnded = ended.Length;
			var startEndingWordIndex = word.Length - countEnded;
			return word.Substring(startEndingWordIndex).Equals(ended, comparisonIgnoreCase);
		}

		/// <summary>
		/// Замена окнончания слова на указанное окончание.
		/// Регистр (строчный или прописной) окончания зависит от регистра последнего символа исходного слова.
		/// </summary>
		private static string ReplaceEnd(string word, string ended)
		{
			return ReplaceEnd(word, ended, ended.Length);
		}

		/// <summary>
		/// Замена окнончания слова на указанное окончание, с указанием числа удаляемых символов старого слова.
		/// Регистр (строчный или прописной) окончания зависит от регистра последнего символа исходного слова.
		/// </summary>
		private static string ReplaceEnd(string word, string ended, int deletedSymbols)
		{
			var lastCharIndex = word.Length - 1;
			var isWorldIsUpperCase = char.IsUpper(word[lastCharIndex]);
			var caseSensitiveEnding = isWorldIsUpperCase ? ended.ToUpper() : ended.ToLower();
			if (deletedSymbols == 0)
			{
				return word + caseSensitiveEnding;
			}
			return word.Remove(word.Length - deletedSymbols, deletedSymbols) + caseSensitiveEnding;
		}

		/// <summary>
		/// Получение сокращённой формы ФИО из полного (Фамилия и инициалы с точками)
		/// </summary>
		public static string GetShortName(string fullName)
		{
			const string parsingErrorResult = "<Ошибка преобразования>";
			const string formatPattern = Constants.EmployeeShortNamePattern;
			if (string.IsNullOrWhiteSpace(fullName))
			{
				return parsingErrorResult;
			}
			var splitted = fullName.Split(null);
			return splitted.Length == 3
				? string.Format(formatPattern, splitted[0], splitted[1][0], splitted[2][0])
				: parsingErrorResult;
		}
	}
}
