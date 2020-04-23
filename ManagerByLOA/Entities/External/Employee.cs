using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Entities.External
{
	/// <summary>
	/// Сотрудник организации, перевозящий ТМЦ. Наследник сущности курьер (Courier).
	/// Берётся из БД стороннего приложения АРМ отдела кадров.
	/// </summary>
	/// <inheritdoc />
	public class Employee : Courier
	{
		/// <summary>
		/// Скрытие поля суперкласса
		/// </summary>
		// ReSharper disable once UnusedMember.Global
		public new long? Id { get { return null; } }

		/// <summary>
		/// Табельный номер сотрудника предприятия
		/// </summary>
		public long PersonnelNumber { get; set; }

		private bool Equals(Employee other)
		{
			return base.Equals(other) && PersonnelNumber == other.PersonnelNumber;
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
			return obj.GetType() == GetType() && Equals((Employee)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode() * 397) ^ PersonnelNumber.GetHashCode();
			}
		}
	}
}
