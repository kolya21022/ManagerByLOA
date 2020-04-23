using System;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.Entities.Enums;

namespace ManagerByLetterOfAttorney.Entities.Internal
{
	/// <summary>
	/// Курьер (человек перевозящий ТМЦ, но не являющийся сотрудником этой организации).
	/// Суперкласс для сотрудника организации (Employee).
	/// </summary>
	/// <inheritdoc />
	public class Courier : IComparable<Courier>
	{
		public long? Id { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public Sex? Sex { get; set; }
		public string PassportSeriesAndNumber { get; set; }
		public string PassportIssuedByOrganization { get; set; }
		public DateTime? PassportIssueDate { get; set; }
		public string Profession { get; set; }

		/// <summary>
		/// Сервисное поле, расчитываемое в sql-запросе при выборке списка сущностей, обозначает число ссылок из 
		/// других таблиц БД на эту сущность. Используется для скрытия/отображения кнопки удаления сущности для 
		/// конечного пользователя: кнопка отображается, если число ссылок равно нулю или значение не указано (null).
		/// В случае, если значение не указано (null) удаление всё равно не удастся выполнить (вторичный ключ БД)
		/// </summary>
		public int? ServiceCountUsed { get; set; }
		
		// ReSharper disable once UnusedMember.Global  /* Используется в XAML-выводе */ 
		public string FullName
		{
			get { return string.Format(Constants.EmployeeFullNamePattern, LastName, FirstName, MiddleName); }
		}

		protected bool Equals(Courier other)
		{
			const StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
			return Id == other.Id
			       && string.Equals(LastName, other.LastName, stringComparison)
				   && string.Equals(FirstName, other.FirstName, stringComparison)
				   && string.Equals(MiddleName, other.MiddleName, stringComparison)
				   && EqualityComparer<Sex?>.Default.Equals(Sex, other.Sex)
				   && string.Equals(PassportSeriesAndNumber, other.PassportSeriesAndNumber, stringComparison)
				   && string.Equals(PassportIssuedByOrganization, other.PassportIssuedByOrganization, stringComparison)
				   && Nullable.Compare(PassportIssueDate, other.PassportIssueDate) == 0
				   && string.Equals(Profession, other.Profession, stringComparison);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return obj.GetType() == GetType() && Equals((Courier)obj);
		}

		public override int GetHashCode()
		{
			var stringComparer = StringComparer.OrdinalIgnoreCase;
			unchecked
			{
				var hashCode = Id.GetHashCode();
				hashCode = (hashCode * 397) ^ (LastName != null ? stringComparer.GetHashCode(LastName) : 0);
				hashCode = (hashCode * 397) ^ (FirstName != null ? stringComparer.GetHashCode(FirstName) : 0);
				hashCode = (hashCode * 397) ^ (MiddleName != null ? stringComparer.GetHashCode(MiddleName) : 0);
				hashCode = (hashCode * 397) ^ (Sex != null ? (int)Sex : 0);
				hashCode = (hashCode * 397) ^ (PassportSeriesAndNumber != null 
					           ? stringComparer.GetHashCode(PassportSeriesAndNumber) : 0);
				hashCode = (hashCode * 397) ^ (PassportIssuedByOrganization != null 
					           ? stringComparer.GetHashCode(PassportIssuedByOrganization) : 0);
				hashCode = (hashCode * 397) ^ (PassportIssueDate != null ? ((DateTime)PassportIssueDate).GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Profession != null ? stringComparer.GetHashCode(Profession) : 0);
				return hashCode;
			}
		}

		public int CompareTo(Courier other)
		{
			const StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
			if (ReferenceEquals(this, other))
			{
				return 0;
			}
			if (ReferenceEquals(null, other))
			{
				return 1;
			}
			var lastNameComparison = string.Compare(LastName, other.LastName, stringComparison);
			if (lastNameComparison != 0)
			{
				return lastNameComparison;
			}
			var firstNameComparison = string.Compare(FirstName, other.FirstName, stringComparison);
			if (firstNameComparison != 0)
			{
				return firstNameComparison;
			}
			var middleNameComparison = string.Compare(MiddleName, other.MiddleName, stringComparison);
			if (middleNameComparison != 0)
			{
				return middleNameComparison;
			}
			var idComparison = Nullable.Compare(Id, other.Id);
			if (idComparison != 0)
			{
				return idComparison;
			}
			var sexComparison = Nullable.Compare(Sex, other.Sex);
			if (sexComparison != 0)
			{
				return sexComparison;
			}
			var passportSeriesAndNumberComparison = string.Compare(PassportSeriesAndNumber,
				other.PassportSeriesAndNumber, stringComparison);
			if (passportSeriesAndNumberComparison != 0)
			{
				return passportSeriesAndNumberComparison;
			}
			var passportIssuedByOrganizationComparison = string.Compare(PassportIssuedByOrganization,
				other.PassportIssuedByOrganization, stringComparison);
			if (passportIssuedByOrganizationComparison != 0)
			{
				return passportIssuedByOrganizationComparison;
			}
			var passportIssueDateComparison = Nullable.Compare(PassportIssueDate, other.PassportIssueDate);
			return passportIssueDateComparison != 0 
				? passportIssueDateComparison
				: string.Compare(Profession, other.Profession, stringComparison);
		}
	}
}
