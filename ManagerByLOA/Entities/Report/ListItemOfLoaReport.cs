using System;

namespace ManagerByLetterOfAttorney.Entities.Report
{
	/// <summary>
	/// Элемент списка ТМЦ, используемый при выдаче доверенности. Используется только для отображения в отчёте.
	/// </summary>
	/// <inheritdoc />
	public class ListItemOfLoaReport : IComparable<ListItemOfLoaReport>
	{
		// NOTE: Подавления предупреждений статического анализатора о приватных геттерах выставлены не просто так.
		// Эти геттеры используются в отчёте при печати (вероятно через рефлексию), и вызываются только с public!
		// Весьма коварно, что никаких ошибок или предупреждений при этом не вываливается - список просто пустой.
		
		// ReSharper disable MemberCanBePrivate.Global
		public long OrdinalNumber { get; set; }
		public string Name { get; set; }
		public string Measure { get; set; }
		public string Count { get; set; }
		// ReSharper restore MemberCanBePrivate.Global

		private bool Equals(ListItemOfLoaReport other)
		{
			const StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
			return OrdinalNumber == other.OrdinalNumber
				   && string.Equals(Name, other.Name, stringComparison)
				   && string.Equals(Measure, other.Measure, stringComparison)
				   && string.Equals(Count, other.Count, stringComparison);
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
			return obj.GetType() == GetType() && Equals((ListItemOfLoaReport)obj);
		}
		
		public int CompareTo(ListItemOfLoaReport other)
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
			var ordinalNumberComparison = OrdinalNumber.CompareTo(other.OrdinalNumber);
			if (ordinalNumberComparison != 0)
			{
				return ordinalNumberComparison;
			}
			var nameComparison = string.Compare(Name, other.Name, stringComparison);
			if (nameComparison != 0)
			{
				return nameComparison;
			}
			var measureComparison = string.Compare(Measure, other.Measure, stringComparison);
			return measureComparison != 0 ? measureComparison : string.Compare(Count, other.Count, stringComparison);
		}

		public override int GetHashCode()
		{
			var stringComparer = StringComparer.OrdinalIgnoreCase;
			unchecked
			{
				var hashCode = OrdinalNumber.GetHashCode();
				hashCode = (hashCode * 397) ^ (Name != null ? stringComparer.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ (Measure != null ? stringComparer.GetHashCode(Measure) : 0);
				hashCode = (hashCode * 397) ^ (Count != null ? stringComparer.GetHashCode(Count) : 0);
				return hashCode;
			}
		}
	}
}
