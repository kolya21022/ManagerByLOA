using System;
using System.Linq;
using System.Collections.Generic;

namespace ManagerByLetterOfAttorney.Entities.Internal
{
	/// <summary>
	/// Доверенность
	/// </summary>
	/// <inheritdoc />
	public class LetterOfAttorney : IComparable<LetterOfAttorney>
	{
		public long Id { get; set; }
		public Company Company { get; set; }

		/// <summary>
		/// Порядковый номер в текущем году
		/// </summary>
		public long OrdinalNumber { get; set; }
		public DateTime ValidityDateStart { get; set; }
		public DateTime ValidityDateEnd { get; set; }
		public Courier Courier { get; set; }
		public List<Shipment> Shipments { get; set; }

		/// <summary>
		/// Поле связи ID вложенного объекта (Company) для связи объектов между собой в сервисном слое.
		/// </summary>
		public long ServiceMappedCompanyId { get; set; }

		/// <summary>
		/// Сервисное поле маркер для указания ID курьера или табельный соотрудника используется в поле связи.
		/// </summary>
		public bool ServiceMappedIsEmployee { get; set; }

		/// <summary>
		/// Поле связи ID или табельного вложенного (Courier или Employee) для связи объектов в сервисном слое.
		/// </summary>
		public long ServiceMappedCourierIdOrEmployeePersonnel { get; set; }

		private bool Equals(LetterOfAttorney other)
		{
			return Id == other.Id
			       && Equals(Company, other.Company)
			       && OrdinalNumber == other.OrdinalNumber
			       && ValidityDateStart.Equals(other.ValidityDateStart)
			       && ValidityDateEnd.Equals(other.ValidityDateEnd)
			       && Equals(Courier, other.Courier)
			       && (Shipments == null && other.Shipments == null
			           || Shipments != null && other.Shipments != null
			                                && Shipments.SequenceEqual(other.Shipments));
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
			return obj.GetType() == GetType() && Equals((LetterOfAttorney)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Id.GetHashCode();
				hashCode = (hashCode * 397) ^ (Company != null ? Company.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ OrdinalNumber.GetHashCode();
				hashCode = (hashCode * 397) ^ ValidityDateStart.GetHashCode();
				hashCode = (hashCode * 397) ^ ValidityDateEnd.GetHashCode();
				hashCode = (hashCode * 397) ^ (Courier != null ? Courier.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Shipments != null ? Shipments.GetHashCode() : 0);
				return hashCode;
			}
		}

		public int CompareTo(LetterOfAttorney other)
		{
			if (ReferenceEquals(this, other))
			{
				return 0;
			}
			if (ReferenceEquals(null, other))
			{
				return 1;
			}
			var ordinalNumberComparison = OrdinalNumber.CompareTo(other.OrdinalNumber);
			if (ordinalNumberComparison != 0)
			{
				return ordinalNumberComparison;
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
			return validityDateEndComparison != 0
				? validityDateEndComparison
				: Comparer<Courier>.Default.Compare(Courier, other.Courier);
		}
	}
}
