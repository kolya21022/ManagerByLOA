using System;
using System.Text;
using System.Collections.Generic;

namespace ManagerByLetterOfAttorney.Util
{
	/// <summary>
	/// Утилитарный класс преобразование числа в строку. Базируется на этом решении: 
	/// https://notesatprograming.blogspot.com.by/2011/10/public-enum-textcase-nominative.html 
	/// </summary>
	public static class NumbersInWordsUtil
	{
		private const string Space = " ";
		private const string Zero = "ноль";
		private const string FirstMale = "один";
		private const string FirstFemale = "одна";
		private const string FirstFemaleAccusative = "одну";
		private const string FirstMaleGenetive = "одно";
		private const string SecondMale = "два";
		private const string SecondFemale = "две";
		private const string SecondMaleGenetive = "двух";
		private const string SecondFemaleGenetive = "двух";

		private static readonly string[] From3Till19 = { "", 
			"три", "четыре", "пять", "шесть", 
			"семь", "восемь", "девять", "десять", "одиннадцать", 
			"двенадцать", "тринадцать", "четырнадцать", "пятнадцать", 
			"шестнадцать", "семнадцать", "восемнадцать", "девятнадцать"
		};

		private static readonly string[] From3Till19Genetive = { "", 
			"трех", "четырех", "пяти", "шести", 
			"семи", "восеми", "девяти", "десяти", "одиннадцати", 
			"двенадцати", "тринадцати", "четырнадцати", "пятнадцати", 
			"шестнадцати", "семнадцати", "восемнадцати", "девятнадцати"
		};

		private static readonly string[] Tens = { "", 
			"двадцать", "тридцать", "сорок", "пятьдесят", 
			"шестьдесят", "семьдесят", "восемьдесят", "девяносто"
		};

		private static readonly string[] TensGenetive = { "", 
			"двадцати", "тридцати", "сорока", "пятидесяти", 
			"шестидесяти", "семидесяти", "восьмидесяти", "девяноста"
		};

		private static readonly string[] Hundreds = { "", 
			"сто", "двести", "триста", "четыреста", 
			"пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот"
		};

		private static readonly string[] HundredsGenetive = { "", 
			"ста", "двухсот", "трехсот", "четырехсот", 
			"пятисот", "шестисот", "семисот", "восемисот", "девятисот"
		};

		private static readonly string[] Thousands = { "", "тысяча", "тысячи", "тысяч" };

		private static readonly string[] ThousandsAccusative = { "", "тысячу", "тысячи", "тысяч" };

		private static readonly string[] Millions = { "", "миллион", "миллиона", "миллионов" };

		private static readonly string[] Billions = { "", "миллиард", "миллиарда", "миллиардов" };

		private static readonly string[] Trillions = { "", "трилион", "трилиона", "триллионов" };
		
		public static string GetNumberString(double number, int precision)
		{
			return precision != 0
				? NumeralsDoubleToTxt(number, precision, TextCase.Accusative, true)
				: NumeralsToTxt((long)number, TextCase.Accusative, true, true);
		}

		public static int GetPrecision(double number)
		{
			var precision = 0;
			while (Math.Abs(number * Math.Pow(10, precision) -
			                Math.Round(number * Math.Pow(10, precision))) > double.Epsilon)
			{
				precision++;
			}
			return precision;
		}

		private static string NumeralsDoubleToTxt(double sourceNumber, int _decimal, TextCase _case, bool firstCapital)
		{
			const string doublePattern = " {0} целых {1} сотых";
			var decNum = (long)Math.Round(sourceNumber * Math.Pow(10, _decimal)) % (long)Math.Pow(10, _decimal);
			var result = string.Format(doublePattern, NumeralsToTxt((long)sourceNumber, _case, true, firstCapital),
				NumeralsToTxt(decNum, _case, true, false));
			return result.Trim();
		}

		private static bool IsPluralGenitive(int digits)
		{
			return digits >= 5 || digits == 0;
		}

		private static bool IsSingularGenitive(int digits)
		{
			return digits >= 2 && digits <= 4;
		}

		private static int LastDigit(long amount)
		{
			if (amount >= 100)
			{
				amount = amount % 100;
			}
			if (amount >= 20)
			{
				amount = amount % 10;
			}
			return (int)amount;
		}

		private static string MakeText(int digits, IList<string> hundreds, IList<string> tens, 
			IList<string> from3Till19, string second, string first, IList<string> power)
		{
			var stringBuilder = new StringBuilder();
			var bufferDigits = digits;

			if (bufferDigits >= 100)
			{
				stringBuilder.Append(hundreds[bufferDigits / 100]).Append(Space);
				bufferDigits = bufferDigits % 100;
			}
			if (bufferDigits >= 20)
			{
				stringBuilder.Append(tens[bufferDigits / 10 - 1]).Append(Space);
				bufferDigits = bufferDigits % 10;
			}
			if (bufferDigits >= 3)
			{
				stringBuilder.Append(from3Till19[bufferDigits - 2]).Append(Space);
			}
			else if (bufferDigits == 2)
			{
				stringBuilder.Append(second).Append(Space);
			}
			else if (bufferDigits == 1)
			{
				stringBuilder.Append(first).Append(Space);
			}
			if (digits == 0 || power.Count <= 0)
			{
				return stringBuilder.ToString();
			}
			bufferDigits = LastDigit(digits);
			if (IsPluralGenitive(bufferDigits))
			{
				stringBuilder.Append(power[3]).Append(Space);
			}
			else if (IsSingularGenitive(bufferDigits))
			{
				stringBuilder.Append(power[2]).Append(Space);
			}
			else
			{
				stringBuilder.Append(power[1]).Append(Space);
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Реализовано для падежей: именительный (nominative), родительный (Genitive),  винительный (accusative)
		/// </summary>
		private static string NumeralsToTxt(long sourceNumber, TextCase _case, bool isMale, bool firstCapital)
		{
			var resultLine = string.Empty;
			var number = sourceNumber;
			var power = 0;
			if (number >= (long)Math.Pow(10, 15) || number < 0)
			{
				return string.Empty;
			}
			while (number > 0)
			{
				var remainder = (int)(number % 1000);
				number = number / 1000;
				switch (power)
				{
					case 12:
						resultLine = MakeText(remainder, Hundreds, Tens, From3Till19, 
							             SecondMale, FirstMale, Trillions) + resultLine;
						break;
					case 9:
						resultLine = MakeText(remainder, Hundreds, Tens, From3Till19, 
							             SecondMale, FirstMale, Billions) + resultLine;
						break;
					case 6:
						resultLine = MakeText(remainder, Hundreds, Tens, From3Till19, 
							             SecondMale, FirstMale, Millions) + resultLine;
						break;
					case 3:
						switch (_case)
						{
							case TextCase.Accusative:
								resultLine = MakeText(remainder, Hundreds, Tens, From3Till19, 
									SecondFemale, FirstFemaleAccusative, ThousandsAccusative) + resultLine;
								break;
							default:
								resultLine = MakeText(remainder, Hundreds, Tens, From3Till19, 
									SecondFemale, FirstFemale, Thousands) + resultLine;
								break;
						}
						break;
					default:
						string[] powerArray = { };
						switch (_case)
						{
							case TextCase.Genitive:
								resultLine = MakeText(remainder, HundredsGenetive, TensGenetive, From3Till19Genetive, 
									             isMale ? SecondMaleGenetive : SecondFemaleGenetive, 
									             isMale ? FirstMaleGenetive : FirstFemale, powerArray) + resultLine;
								break;
							case TextCase.Accusative:
								resultLine = MakeText(remainder, Hundreds, Tens, From3Till19, 
									             isMale ? SecondMale : SecondFemale, 
									             isMale ? FirstMale : FirstFemaleAccusative, powerArray) + resultLine;
								break;
							default:
								resultLine = MakeText(remainder, Hundreds, Tens, From3Till19, 
									             isMale ? SecondMale : SecondFemale, 
									             isMale ? FirstMale : FirstFemale, powerArray) + resultLine;
								break;
						}
						break;
				}
				power += 3;
			}
			if (sourceNumber == 0)
			{
				resultLine = Zero + Space;
			}
			if (resultLine != string.Empty && firstCapital)
			{
				resultLine = resultLine.Substring(0, 1).ToUpper() + resultLine.Substring(1);
			}
			return resultLine.Trim();
		}
	}

	internal enum TextCase
	{
		/// <summary>
		/// Именительный: Кто? Что?
		/// </summary>
		Nominative,

		/// <summary>
		/// Родительный: Кого? Чего?
		/// </summary>
		Genitive,

		/// <summary>
		/// Дательный: Кому? Чему?
		/// </summary>
		Dative,

		/// <summary>
		/// Винительный: Кого? Что?
		/// </summary>
		Accusative,

		/// <summary>
		/// Творительный: Кем? Чем?
		/// </summary>
		Instrumental,

		/// <summary>
		/// Предложный: О ком? О чём?
		/// </summary>
		Prepositional
	}
}
