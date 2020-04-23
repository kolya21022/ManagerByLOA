using System;
using System.Collections.Generic;

namespace ManagerByLetterOfAttorney.Entities.Internal
{
	/// <summary>
	/// Перевозимый в ТМЦ груз.
	/// </summary>
	/// <inheritdoc />
	[Serializable]
	public class Cargo : IComparable<Cargo>
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public Measure Measure { get; set; }

		/// <summary>
		/// Поле связи ID вложенного объекта (Measure) для связи объектов между собой в сервисном слое.
		/// </summary>
		public long? ServiceMappedMeasureId { get; set; }

		/// <summary>
		/// Сервисное поле, расчитываемое в sql-запросе при выборке списка сущностей, обозначает число ссылок из 
		/// других таблиц БД на эту сущность. Используется для скрытия/отображения кнопки удаления сущности для 
		/// конечного пользователя: кнопка отображается, если число ссылок равно нулю или значение не указано (null).
		/// В случае, если значение не указано (null) удаление всё равно не удастся выполнить (вторичный ключ БД)
		/// </summary>
		public int? ServiceCountUsed { get; set; }

		/// <summary>
		/// Используется для подстановки строкового значения при поиске
		/// </summary>
		public string ServiceSearchResultDisplayed
		{
			get { return Name; }
		}

		private bool Equals(Cargo other)
		{
			const StringComparison strComparison = StringComparison.OrdinalIgnoreCase;
			return Id == other.Id && string.Equals(Name, other.Name, strComparison) && Equals(Measure, other.Measure);
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
			return obj.GetType() == GetType() && Equals((Cargo)obj);
		}

		public override int GetHashCode()
		{
			var stringComparer = StringComparer.OrdinalIgnoreCase;
			unchecked
			{
				var hashCode = Id.GetHashCode();
				hashCode = (hashCode * 397) ^ (Name != null ? stringComparer.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ (Measure != null ? Measure.GetHashCode() : 0);
				return hashCode;
			}
		}

		public int CompareTo(Cargo other)
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
			var nameComparison = string.Compare(Name, other.Name, stringComparison);
			if (nameComparison != 0)
			{
				return nameComparison;
			}
			var measureComparison = Comparer<Measure>.Default.Compare(Measure, other.Measure);
			return measureComparison != 0 ? measureComparison : Id.CompareTo(other.Id);
		}
	}
}
