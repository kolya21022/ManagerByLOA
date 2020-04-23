using System;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Entities.External
{
	/// <summary>
	/// Организация с указанными УНП/городом, которая берётся из БД стороннего приложения АРМ Поставщиков-Потребителей.
	/// Используется для поиска и постановки значений при вводе доверенности.
	/// </summary>
	/// <inheritdoc cref="Company" />
	public class CompanyWithUnp : Company, IComparable<CompanyWithUnp>
	{
		/// <summary>
		/// Скрытие поля суперкласса
		/// </summary>
		// ReSharper disable once UnusedMember.Global
		public new long? Id { get { return null; } }

		/// <summary>
		/// Учетный номера налогоплательщика 
		/// (в разных странах и временных периодах разные названия: УНП, УНН, ИНН и т.д.)
		/// </summary>
		public string Unp { get; set; }
		public string City { get; set; }

		/// <summary>
		/// Сервисное поле для подстановки строкового значения при поиске, в формате [Name г.City]
		/// </summary>
		public new string ServiceSearchResultDisplayed
		{
			get { return string.Format("{0}{1}", Name, City == null ? string.Empty : " г." + City); }
		}
		
		private bool Equals(CompanyWithUnp other)
		{
			const StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
			return base.Equals(other) 
			       && string.Equals(Unp, other.Unp, stringComparison) 
			       && string.Equals(City, other.City, stringComparison);
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
			return obj.GetType() == GetType() && Equals((CompanyWithUnp)obj);
		}

		public override int GetHashCode()
		{
			var stringComparer = StringComparer.OrdinalIgnoreCase;
			unchecked
			{
				var hashCode = base.GetHashCode();
				hashCode = (hashCode * 397) ^ (Unp != null ? stringComparer.GetHashCode(Unp) : 0);
				hashCode = (hashCode * 397) ^ (City != null ? stringComparer.GetHashCode(City) : 0);
				return hashCode;
			}
		}

		public int CompareTo(CompanyWithUnp other)
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
			var companyComparison = base.CompareTo(other);
			if (companyComparison != 0)
			{
				return companyComparison;
			}
			var cityComparison = string.Compare(City, other.City, stringComparison);
			return cityComparison != 0 ? cityComparison : string.Compare(Unp, other.Unp, stringComparison);
		}
	}
}
