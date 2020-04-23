using System;
using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для таблицы перевозимых товарно-материальных ценностей (ТМЦ) [Shipments]
	/// </summary>
	public static class ShipmentsStorage
	{
		/// <summary>
		/// Получение коллеции [Перевозимых ТМЦ], с указанными ID [доверенности] и ID [перевозимого груза]
		/// </summary>
		public static List<Shipment> GetAll()
		{
			const string queryGetAllWithLoaIdAndCargoId = "SELECT [id], [count], [letterOfAttorneysId], [cargoesId] " +
			                                              "FROM [Shipments]";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var shipments = new List<Shipment>();
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryGetAllWithLoaIdAndCargoId, connection))
					{
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								var shipment = new Shipment
								{
									Id = reader.GetInt64(0),
									Count = reader.IsDBNull(1) ? (double?)null : reader.GetDouble(1),
									ServiceMappedLetterOfAttorneyId = reader.GetInt64(2),
									ServiceMappedCargoId = reader.GetInt64(3)
								};
								shipments.Add(shipment);
							}
						}
					}
				}
				return shipments;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение коллеции [Перевозимых ТМЦ] по указанному ID [доверенности], 
		/// с указанными ID [доверенности] и ID [перевозимого груза]
		/// </summary>
		public static List<Shipment> GetAllByLoaId(long letterOfAttorneysId)
		{
			const string queryGetByLoaIdWithCargoId = "SELECT [id], [count], [letterOfAttorneysId], [cargoesId] " +
			                                          "FROM [Shipments] " +
			                                          "WHERE [letterOfAttorneysId] = @letterOfAttorneysId";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var shipments = new List<Shipment>();
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryGetByLoaIdWithCargoId, connection))
					{
						sqlCommand.Parameters.AddWithValue("@letterOfAttorneysId", letterOfAttorneysId);
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								var shipment = new Shipment
								{
									Id = reader.GetInt64(0),
									Count = reader.IsDBNull(1) ? (double?)null : reader.GetDouble(1),
									ServiceMappedLetterOfAttorneyId = reader.GetInt64(2),
									ServiceMappedCargoId = reader.GetInt64(3)
								};
								shipments.Add(shipment);
							}
						}
					}
				}
				return shipments;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Добавление новой [Перевозимой ТМЦ]
		/// </summary>
		public static Shipment Insert(long letterOfAttorneyId, Cargo cargo, double? count, SqlConnection connection, 
			SqlTransaction transaction)
		{
			const string queryInsert = "INSERT INTO [Shipments] ([letterOfAttorneysId], [cargoesId], [count]) " +
			                           "OUTPUT INSERTED.ID " +
			                           "VALUES (@letterOfAttorneysId, @cargoesId, @count)";
			using (var sqlCommand = new SqlCommand(queryInsert, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@letterOfAttorneysId", letterOfAttorneyId);
				sqlCommand.Parameters.AddWithValue("@cargoesId", cargo.Id);
				sqlCommand.Parameters.AddWithValue("@count", count != null ? (object)count : DBNull.Value);
				var id = (long)sqlCommand.ExecuteScalar();
				return new Shipment
				{
					Id = id,
					Cargo = cargo,
					Count = count
				};
			}
		}

		/// <summary>
		/// Удаление [перевозимых ТМЦ], по указанному ID [доверенности]
		/// </summary>
		public static void DeleteByLetterOfAttoneyId(long letterOfAttoneyId, SqlConnection connection, 
			SqlTransaction transaction)
		{
			const string queryDeleteByLetterOfAttoneyId = "DELETE FROM [Shipments] " +
			                                              "WHERE [letterOfAttorneysId] = @letterOfAttorneysId";
			using (var sqlCommand = new SqlCommand(queryDeleteByLetterOfAttoneyId, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@letterOfAttorneysId", letterOfAttoneyId);
				sqlCommand.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Удаление всех [перевозимых ТМЦ]
		/// </summary>
		public static void DeleteAll(SqlConnection connection, SqlTransaction transaction)
		{
			const string queryDeleteAll = "DELETE FROM [Shipments]";
			using (var sqlCommand = new SqlCommand(queryDeleteAll, connection, transaction))
			{
				sqlCommand.ExecuteNonQuery();
			}
		}
	}
}
