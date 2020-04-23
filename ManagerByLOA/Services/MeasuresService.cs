using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Storages;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для класса единиц измерения - [Measure]
	/// </summary>
	public static class MeasuresService
	{
		/// <summary>
		/// Получение коллекции [Единиц измерения]
		/// </summary>
		public static List<Measure> GetAll()
		{
			return MeasuresStorare.GetAll();
		}

		/// <summary>
		/// Получение коллекции [Единиц измерения] с указанным полем числа ссылок на запись из других таблиц
		/// </summary>
		public static List<Measure> GetAllWithCountUsing()
		{
			return MeasuresStorare.GetAllWithCountUsing();
		}

		/// <summary>
		/// Получение коллекции [Единиц измерения]
		/// </summary>
		public static List<Measure> GetAll(SqlConnection connection, SqlTransaction transaction)
		{
			return MeasuresStorare.GetAll(connection, transaction);
		}

		/// <summary>
		/// Получение коллекции [Единиц измерения] по указанному названию
		/// </summary>
		public static List<Measure> GetByName(string name)
		{
			return MeasuresStorare.GetByName(name);
		}

		/// <summary>
		/// Получение коллекции [Единиц измерения] по указанному названию
		/// </summary>
		public static List<Measure> GetByName(string name, SqlConnection connection, SqlTransaction transaction)
		{
			return MeasuresStorare.GetByName(name, connection, transaction);
		}

		/// <summary>
		/// Обновление [Единицы измерения] с указанным [Id]
		/// </summary>
		public static Measure Update(long id, string name)
		{
			return MeasuresStorare.Update(id, name);
		}

		/// <summary>
		/// Добавление новой [Единицы измерения]
		/// </summary>
		public static Measure Insert(string newName)
		{
			return MeasuresStorare.Insert(newName);
		}

		/// <summary>
		/// Добавление новой [Единицы измерения]
		/// </summary>
		public static Measure Insert(string name, SqlConnection connection, SqlTransaction transaction)
		{
			return MeasuresStorare.Insert(name, connection, transaction);
		}

		/// <summary>
		/// Удаление [Единицы измерения]
		/// </summary>
		public static void Delete(Measure measure)
		{
			MeasuresStorare.DeleteById(measure.Id);
		}
	}
}
