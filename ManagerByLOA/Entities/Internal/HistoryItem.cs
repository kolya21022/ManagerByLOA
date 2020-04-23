using System;
using ManagerByLetterOfAttorney.Util;

namespace ManagerByLetterOfAttorney.Entities.Internal
{
	/// <summary>
	/// Архивные перевозимые ТМЦ прошлых лет. 
	/// Используется только для вывода и фильтрации в соответствующей таблице
	/// </summary>
	/// <inheritdoc />
	public class HistoryItem : IComparable<HistoryItem>
	{
		public long Id { private get; set; }
		public long Year { get; set; }
		public long OrdinalNumber { get; set; }
		public DateTime ValidityDateStart { get; set; }
		public DateTime ValidityDateEnd { get; set; }
		public string Company { get; set; }
		public long? EmployeePersonnelNumber { get; set; }
		public string CourierOrEmployeeFullName { get; set; }
		public string CourierOrEmployeeShortName
		{
			get { return NamesConverter.GetShortName(CourierOrEmployeeFullName); }
		}
		public string CourierOrEmployeeProfession { get; set; }
		public string PassportSeriesAndNumber { get; set; }
		public string PassportIssuedByOrganization { get; set; }
		public DateTime? PassportIssueDate { get; set; }
		public string ShipmentName { get; set; }
		public double? ShipmentCount { get; set; }
		public string ShipmentMeasure { get; set; }

		private bool Equals(HistoryItem other)
		{
			const StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
			return Id == other.Id 
			       && Year == other.Year 
			       && OrdinalNumber == other.OrdinalNumber 
			       && ValidityDateStart.Equals(other.ValidityDateStart) 
			       && ValidityDateEnd.Equals(other.ValidityDateEnd) 
			       && string.Equals(Company, other.Company, stringComparison) 
			       && EmployeePersonnelNumber == other.EmployeePersonnelNumber 
			       && string.Equals(CourierOrEmployeeFullName, other.CourierOrEmployeeFullName, stringComparison) 
			       && string.Equals(CourierOrEmployeeProfession, other.CourierOrEmployeeProfession, stringComparison)
			       && string.Equals(PassportSeriesAndNumber, other.PassportSeriesAndNumber, stringComparison) 
			       && string.Equals(PassportIssuedByOrganization, other.PassportIssuedByOrganization, stringComparison)
			       && PassportIssueDate.Equals(other.PassportIssueDate) 
			       && string.Equals(ShipmentName, other.ShipmentName, stringComparison) 
			       && ShipmentCount.Equals(other.ShipmentCount) 
			       && string.Equals(ShipmentMeasure, other.ShipmentMeasure, stringComparison);
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
			return obj.GetType() == GetType() && Equals((HistoryItem) obj);
		}

		public override int GetHashCode()
		{
			var stringComparer = StringComparer.OrdinalIgnoreCase;
			unchecked
			{
				var hashCode = Id.GetHashCode();
				hashCode = (hashCode * 397) ^ Year.GetHashCode();
				hashCode = (hashCode * 397) ^ OrdinalNumber.GetHashCode();
				hashCode = (hashCode * 397) ^ ValidityDateStart.GetHashCode();
				hashCode = (hashCode * 397) ^ ValidityDateEnd.GetHashCode();
				hashCode = (hashCode * 397) ^ (Company != null ? stringComparer.GetHashCode(Company) : 0);
				hashCode = (hashCode * 397) ^ EmployeePersonnelNumber.GetHashCode();
				hashCode = (hashCode * 397) ^ (CourierOrEmployeeFullName != null 
					           ? stringComparer.GetHashCode(CourierOrEmployeeFullName) : 0);
				hashCode = (hashCode * 397) ^ (CourierOrEmployeeProfession != null 
					           ? stringComparer.GetHashCode(CourierOrEmployeeProfession) : 0);
				hashCode = (hashCode * 397) ^ (PassportSeriesAndNumber != null 
					           ? stringComparer.GetHashCode(PassportSeriesAndNumber) : 0);
				hashCode = (hashCode * 397) ^ (PassportIssuedByOrganization != null 
					           ? stringComparer.GetHashCode(PassportIssuedByOrganization) : 0);
				hashCode = (hashCode * 397) ^ PassportIssueDate.GetHashCode();
				hashCode = (hashCode * 397) ^ (ShipmentName != null ? stringComparer.GetHashCode(ShipmentName) : 0);
				hashCode = (hashCode * 397) ^ ShipmentCount.GetHashCode();
				hashCode = (hashCode * 397) ^ (ShipmentMeasure != null 
					           ? stringComparer.GetHashCode(ShipmentMeasure) : 0);
				return hashCode;
			}
		}

		public int CompareTo(HistoryItem other)
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
			var yearComparison = Year.CompareTo(other.Year);
			if (yearComparison != 0)
			{
				return -yearComparison;          // порядок по убыванию
			}
			var ordinalNumberComparison = OrdinalNumber.CompareTo(other.OrdinalNumber);
			if (ordinalNumberComparison != 0)
			{
				return -ordinalNumberComparison; // порядок по убыванию
			}
			var idComparison = Id.CompareTo(other.Id);
			if (idComparison != 0)
			{
				return idComparison;
			}
			var validityDateStartComparison = ValidityDateStart.CompareTo(other.ValidityDateStart);
			if (validityDateStartComparison != 0)
			{
				return validityDateStartComparison;
			}
			var validityDateEndComparison = ValidityDateEnd.CompareTo(other.ValidityDateEnd);
			if (validityDateEndComparison != 0)
			{
				return validityDateEndComparison;
			}
			var companyComparison = string.Compare(Company, other.Company, stringComparison);
			if (companyComparison != 0)
			{
				return companyComparison;
			}
			var employeePersNumberCompare = Nullable.Compare(EmployeePersonnelNumber, other.EmployeePersonnelNumber);
			if (employeePersNumberCompare != 0)
			{
				return employeePersNumberCompare;
			}
			var fullNameComparison = string.Compare(CourierOrEmployeeFullName, 
				other.CourierOrEmployeeFullName, stringComparison);
			if (fullNameComparison != 0)
			{
				return fullNameComparison;
			}
			var professionComparison = string.Compare(CourierOrEmployeeProfession, 
				other.CourierOrEmployeeProfession, stringComparison);
			if (professionComparison != 0)
			{
				return professionComparison;
			}
			var passportSeriesComparison = string.Compare(PassportSeriesAndNumber,
				other.PassportSeriesAndNumber, stringComparison);
			if (passportSeriesComparison != 0)
			{
				return passportSeriesComparison;
			}
			var passportIssuedByOrganizationComparison = string.Compare(PassportIssuedByOrganization, 
				other.PassportIssuedByOrganization, stringComparison);
			if (passportIssuedByOrganizationComparison != 0)
			{
				return passportIssuedByOrganizationComparison;
			}
			var passportIssueDateComparison = Nullable.Compare(PassportIssueDate, other.PassportIssueDate);
			if (passportIssueDateComparison != 0)
			{
				return passportIssueDateComparison;
			}
			var shipmentNameComparison = string.Compare(ShipmentName, other.ShipmentName, stringComparison);
			if (shipmentNameComparison != 0)
			{
				return shipmentNameComparison;
			}
			var shipmentCountComparison = Nullable.Compare(ShipmentCount, other.ShipmentCount);
			return shipmentCountComparison != 0
				? shipmentCountComparison
				: string.Compare(ShipmentMeasure, other.ShipmentMeasure, stringComparison);
		}
	}
}
