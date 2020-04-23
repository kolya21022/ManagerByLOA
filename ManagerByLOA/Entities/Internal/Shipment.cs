using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using ManagerByLetterOfAttorney.Util;

namespace ManagerByLetterOfAttorney.Entities.Internal
{
	/// <summary>
	/// Перевозимая товарно-материальная ценность (ТМЦ)
	/// </summary>
	/// <inheritdoc cref="ICloneable" />
	[Serializable]
	public class Shipment : ICloneable, IComparable<Shipment>
	{
		public long Id { private get; set; }
		public Cargo Cargo { get; set; }
		public double? Count { get; set; }

		/// <summary>
		/// Поле связи ID вложенного объекта (Cargo) для связи объектов между собой в сервисном слое.
		/// </summary>
		public long ServiceMappedCargoId { get; set; }

		/// <summary>
		/// Поле связи ID вложенного объекта (LetterOfAttorney) для связи объектов между собой в сервисном слое.
		/// </summary>
		public long ServiceMappedLetterOfAttorneyId { get; set; }
		
		private bool Equals(Shipment other)
		{
			return Id == other.Id && Count.Equals(other.Count) && Equals(Cargo, other.Cargo);
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
			return obj.GetType() == GetType() && Equals((Shipment)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Id.GetHashCode();
				hashCode = (hashCode * 397) ^ Count.GetHashCode();
				hashCode = (hashCode * 397) ^ (Cargo != null ? Cargo.GetHashCode() : 0);
				return hashCode;
			}
		}

		public object Clone()
		{
			using (var stream = new MemoryStream())
			{
				if (!GetType().IsSerializable)
				{
					throw new SerializationException(Constants.SerializeError);
				}
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, this);
				stream.Position = 0;
				return formatter.Deserialize(stream);
			}
		}

		public int CompareTo(Shipment other)
		{
			if (ReferenceEquals(this, other))
			{
				return 0;
			}
			if (ReferenceEquals(null, other))
			{
				return 1;
			}
			var cargoComparison = Comparer<Cargo>.Default.Compare(Cargo, other.Cargo);
			if (cargoComparison != 0)
			{
				return cargoComparison;
			}
			var countComparison = Nullable.Compare(Count, other.Count);
			return countComparison != 0 ? countComparison : Id.CompareTo(other.Id);
		}
	}
}
