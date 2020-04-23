using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Storages;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для класса перевозимых товарно-материальных ценностей (ТМЦ) - [Shipment]
	/// </summary>
	public static class ShipmentsService
	{
		/// <summary>
		/// Получение коллеции [Перевозимых ТМЦ] с указанными ID [доверенности]
		/// </summary>
		public static List<Shipment> GetAll()
		{
			var shipments = ShipmentsStorage.GetAll();
			var cargoes = CargoesService.GetAll();
			
			foreach (var shipment in shipments)
			{
				var cargoesId = shipment.ServiceMappedCargoId;   // ID груза
				foreach (var cargo in cargoes)
				{
					if (cargo.Id != cargoesId)
					{
						continue;
					}
					shipment.Cargo = cargo;
					break;
				}
			}
			return shipments;
		}

		/// <summary>
		/// Получение коллеции [Перевозимых ТМЦ] по указанному ID [доверенности]
		/// </summary>
		public static List<Shipment> GetByLetterOfAttorneyId(long letterOfAttorneyId)
		{
			var shipments = ShipmentsStorage.GetAllByLoaId(letterOfAttorneyId);
			var cargoes = CargoesService.GetAll();
			
			foreach (var shipment in shipments)
			{
				var cargoesId = shipment.ServiceMappedCargoId;            // ID груза
				foreach (var cargo in cargoes)
				{
					if (cargo.Id != cargoesId)
					{
						continue;
					}
					shipment.Cargo = cargo;
					break;
				}
			}
			return shipments;
		}

		/// <summary>
		/// Добавление новой [Перевозимой ТМЦ]
		/// </summary>
		public static Shipment Insert(long letterOfAttorneyId, string cargoName, string measureName, 
			double? count, SqlConnection connect, SqlTransaction transaction)
		{
			Measure measure = null;
			if (!string.IsNullOrWhiteSpace(measureName)) // если единица измерения указана
			{
				// Поиск этой единицы измерения в БД, добавление в БД если она не найдена
				var findedMeasures = MeasuresService.GetByName(measureName, connect, transaction);
				measure = findedMeasures == null || findedMeasures.Count == 0
					? MeasuresService.Insert(measureName, connect, transaction)
					: findedMeasures[0];
			}

			// Поиск указанного груза с этой указанной единицей измерения, если не найден добавление груза в БД 
			var measureId = measure == null ? (long?) null : measure.Id;
			var findCargoes = CargoesService.GetCargoesByNameAndMeasuresId(cargoName, measureId, connect, transaction);
			var cargo = findCargoes == null || findCargoes.Count == 0
				? CargoesService.Insert(cargoName, measure, connect, transaction)
				: findCargoes[0];
			
			// Добавление перевозимой ТМЦ по указанному ID доверенности
			return ShipmentsStorage.Insert(letterOfAttorneyId, cargo, count, connect, transaction);
		}

		/// <summary>
		/// Удаление [перевозимых ТМЦ], по указанному ID [доверенности]
		/// </summary>
		public static void DeleteByLetterOfAttoneyId(long loaId, SqlConnection connection, SqlTransaction transaction)
		{
			ShipmentsStorage.DeleteByLetterOfAttoneyId(loaId, connection, transaction);
		}
	}
}
