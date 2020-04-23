using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Storages;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для класса перевозимых в ТМЦ грузов [Cargo]
	/// </summary>
	public static class CargoesService
	{
		/// <summary>
		/// Получение коллеции [перевозимых в ТМЦ грузов]
		/// </summary>
		public static List<Cargo> GetAll()
		{
			var measures = MeasuresStorare.GetAll();          // коллекция единиц измерения
			var cargoes = CargoesStorage.GetAll();            // коллекция грузов
			foreach (var cargo in cargoes)
			{
				var measureId = cargo.ServiceMappedMeasureId; // ID единицы измерения
				foreach (var measure in measures)
				{
					if (measure.Id != measureId)
					{
						continue;
					}
					cargo.Measure = measure;
					break;
				}
			}
			return cargoes;
		}

		/// <summary>
		/// Получение коллеции [перевозимых в ТМЦ грузов] с указанным полем числа ссылок на записи из других таблиц
		/// </summary>
		public static List<Cargo> GetCargoesWithCountUsing()
		{
			var measures = MeasuresService.GetAll();
			var cargoes = CargoesStorage.GetAllWithCountUsing();
			foreach (var cargo in cargoes)
			{
				var measureId = cargo.ServiceMappedMeasureId;
				foreach (var measure in measures)
				{
					if (measure.Id != measureId)
					{
						continue;
					}
					cargo.Measure = measure;
					break;
				}
			}
			return cargoes;
		}

		/// <summary>
		/// Получение коллекции [Перевозимых в ТМЦ грузов] по указанному названию и ID ед.измерения
		/// </summary>
		public static List<Cargo> GetCargoesByNameAndMeasuresId(string name, long? measuresId,
			SqlConnection connection, SqlTransaction transaction)
		{
			var measures = MeasuresService.GetAll(connection, transaction);
			var cargoes = CargoesStorage.GetByNameAndMeasureId(name, measuresId, connection, transaction);
			foreach (var cargo in cargoes)
			{
				var measureId = cargo.ServiceMappedMeasureId;
				foreach (var measure in measures)
				{
					if (measure.Id != measureId)
					{
						continue;
					}
					cargo.Measure = measure;
					break;
				}
			}
			return cargoes;
		}

		/// <summary>
		/// Добавление нового [перевозимого в ТМЦ груза]
		/// </summary>
		public static Cargo Insert(string name, string measureName)
		{
			var findedMeasures = new List<Measure>();
			if (!string.IsNullOrWhiteSpace(measureName)) // если ед.измерения указана - поиск такой ед.измерения в БД
			{
				findedMeasures = MeasuresService.GetByName(measureName);
			}
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var transaction = connection.BeginTransaction("Insert cargo"))
					{
						Measure measure = null;
						if (!string.IsNullOrWhiteSpace(measureName)) // если ед.измерения указана
						{
							// Если не найдено единиц измерения с таким именем - добавляем в транзакции
							measure = findedMeasures.Count > 0
								? findedMeasures[0]
								: MeasuresStorare.Insert(measureName, connection, transaction);
						}
						var cargo = CargoesStorage.Insert(name, measure, connection, transaction);
						transaction.Commit();
						return cargo;
					}
				}
			}
			catch (DbException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Добавление нового [перевозимого в ТМЦ груза]
		/// </summary>
		public static Cargo Insert(string cargoName, Measure measure, SqlConnection connection, 
			SqlTransaction transaction)
		{
			return CargoesStorage.Insert(cargoName, measure, connection, transaction);
		}

		/// <summary>
		/// Обновление [перевозимого в ТМЦ груза] с указанным [Id]
		/// </summary>
		public static Cargo Update(long id, string name, string measureName)
		{
			var findedMeasures = new List<Measure>();
			if (!string.IsNullOrWhiteSpace(measureName)) // если ед.измерения указана - поиск такой ед.измерения в БД
			{
				findedMeasures = MeasuresService.GetByName(measureName);
			}
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var transaction = connection.BeginTransaction("Insert cargo"))
					{
						Measure measure = null;
						if (!string.IsNullOrWhiteSpace(measureName)) // если ед.измерения указана
						{
							// Если не найдено единиц измерения с таким именем - добавляем в транзакции
							measure = findedMeasures.Count > 0
								? findedMeasures[0]
								: MeasuresStorare.Insert(measureName, connection, transaction);
						}
						var cargo = CargoesStorage.Update(id, name, measure, connection, transaction);
						transaction.Commit();
						return cargo;
					}
				}
			}
			catch (DbException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Удаление [перевозимого в ТМЦ груза] с указанным [Id]
		/// </summary>
		public static void Delete(Cargo cargo)
		{
			CargoesStorage.DeleteById(cargo.Id);
		}
	}
}
