﻿using System;

namespace ManagerByLetterOfAttorney.Entities.Internal
{
	/// <summary>
	/// Единица измерения груза в ТМЦ
	/// </summary>
	/// <inheritdoc />
	[Serializable]
	public class Measure : IComparable<Measure>
	{
		public long Id { get; set; }
		public string Name { get; set; }

		/// <summary>
		/// Сервисное поле, расчитываемое в sql-запросе при выборке списка сущностей, обозначает число ссылок из 
		/// других таблиц БД на эту сущность. Используется для скрытия/отображения кнопки удаления сущности для 
		/// конечного пользователя: кнопка отображается, если число ссылок равно нулю или значение не указано (null).
		/// В случае, если значение не указано (null) удаление всё равно не удастся выполнить (вторичный ключ БД)
		/// </summary>
		public int? ServiceCountUsed { get; set; }

		/// <summary>
		/// Используется для подстановки значения при поиске
		/// </summary>
		public string ServiceSearchDisplayed
		{
			get { return Name; }
		}

		private bool Equals(Measure other)
		{
			const StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
			return Id == other.Id && string.Equals(Name, other.Name, stringComparison);
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
			return obj.GetType() == GetType() && Equals((Measure)obj);
		}

		public override int GetHashCode()
		{
			var stringComparison = StringComparer.OrdinalIgnoreCase;
			unchecked
			{
				return (Id.GetHashCode() * 397) ^ (Name != null ? stringComparison.GetHashCode(Name) : 0);
			}
		}

		public int CompareTo(Measure other)
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
			return nameComparison != 0 ? nameComparison : Id.CompareTo(other.Id);
		}
	}
}