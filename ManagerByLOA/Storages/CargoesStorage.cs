using System;
using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для таблицы перевозимых в ТМЦ грузов [Cargo]
	/// </summary>
	public static class CargoesStorage
	{
		/// <summary>
		/// Получение коллеции [перевозимых в ТМЦ грузов], с указанными ID [единиц измерения] 
		/// </summary>
		public static List<Cargo> GetAll()
		{
			const string queryGetAllWithMeasureId = "SELECT [id], [name], [measuresId] FROM [Cargoes] ORDER BY [name]";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;
			
			var cargoes = new List<Cargo>();
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryGetAllWithMeasureId, connection))
					{
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								var cargo = new Cargo
								{
									Id = reader.GetInt64(0),
									Name = reader.GetString(1),
									ServiceMappedMeasureId = reader.IsDBNull(2) ? (long?)null : reader.GetInt64(2)
								};
								cargoes.Add(cargo);
							}
						}
					}

				}
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
			return cargoes;
		}

		/// <summary>
		/// Получение коллекции [перевозимых в ТМЦ грузов], с указанными ID [единиц измерения], 
		/// с указанным полем числа ссылок на запись из других таблиц
		/// </summary>
		public static List<Cargo> GetAllWithCountUsing()
		{
			const string query = "SELECT [Cargoes].[id], [Cargoes].[name], [Cargoes].[measuresId], " +
			                     "COUNT([Shipments].[cargoesId]) AS serviceCountUsed " +
			                     "FROM [Cargoes] " +
			                     "LEFT JOIN [Shipments] ON [Shipments].[cargoesId] = [Cargoes].[id] " +
			                     "GROUP BY [Cargoes].[id], [Cargoes].[name], [Cargoes].[measuresId] ";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var cargoes = new List<Cargo>();
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(query, connection))
					{
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								var cargo = new Cargo
								{
									Id = reader.GetInt64(0),
									Name = reader.GetString(1),
									ServiceMappedMeasureId = reader.IsDBNull(2) ? (long?)null : reader.GetInt64(2),
									ServiceCountUsed = reader.GetInt32(3)
								};
								cargoes.Add(cargo);
							}
						}
					}
				}
				return cargoes;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение коллекции [перевозимых в ТМЦ грузов], с указанными ID [единиц измерения], 
		/// по указанному названию груза и указанной единицы измерения
		/// </summary>
		public static List<Cargo> GetByNameAndMeasureId(string name, long? measureId,
			SqlConnection connection, SqlTransaction transaction)
		{
			var nullableMeasureIdCriteria = measureId == null ? "IS NULL" : "= " + (long)measureId;
			var queryGetByNameAndMeasureId = "SELECT [id], [name], [measuresId] FROM [Cargoes] " +
			                                 "WHERE [name] = @name AND [measuresId] " + nullableMeasureIdCriteria;

			var cargoes = new List<Cargo>();
			using (var sqlCommand = new SqlCommand(queryGetByNameAndMeasureId, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@name", name);
				using (var reader = sqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						var cargo = new Cargo
						{
							Id = reader.GetInt64(0),
							Name = reader.GetString(1),
							ServiceMappedMeasureId = reader.IsDBNull(2) ? (long?)null : reader.GetInt64(2)
						};
						cargoes.Add(cargo);
					}
				}
			}
			return cargoes;
		}

		/// <summary>
		/// Добавление нового [перевозимого в ТМЦ груза]
		/// </summary>
		public static Cargo Insert(string name, Measure measure, SqlConnection connection, SqlTransaction transaction)
		{
			const string queryInsert = "INSERT INTO [Cargoes] ([name], [measuresId]) OUTPUT INSERTED.ID " +
			                           "VALUES (@name, @measuresId)";
			using (var sqlCommand = new SqlCommand(queryInsert, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@name", name);
				sqlCommand.Parameters.AddWithValue("@measuresId", 
					measure != null ? (object)measure.Id : DBNull.Value);
				var id = (long)sqlCommand.ExecuteScalar();
				return new Cargo
				{
					Id = id,
					Name = name,
					Measure = measure
				};
			}
		}

		/// <summary>
		/// Обновление [перевозимого в ТМЦ груза] с указанным [Id]
		/// </summary>
		public static Cargo Update(long id, string name, Measure measure, SqlConnection connection, 
			SqlTransaction transaction)
		{
			const string updateQuery = "UPDATE [Cargoes] " +
			                           "SET [name] = @name, [measuresId] = @measuresId WHERE [id] = @id";
			try
			{
				using (var sqlCommand = new SqlCommand(updateQuery, connection, transaction))
				{
					sqlCommand.Parameters.AddWithValue("@name", name);
					sqlCommand.Parameters.AddWithValue("@measuresId", 
						measure != null ? (object)measure.Id : DBNull.Value);
					sqlCommand.Parameters.AddWithValue("@id", id);
					sqlCommand.ExecuteNonQuery();
					return new Cargo
					{
						Id = id,
						Name = name,
						Measure = measure
					};
				}
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Удаление [перевозимого в ТМЦ груза] с указанным [Id]
		/// </summary>
		public static void DeleteById(long id)
		{
			const string queryDelete = "DELETE FROM [Cargoes] WHERE [id] = @id";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryDelete, connection))
					{
						sqlCommand.Parameters.AddWithValue("@id", id);
						sqlCommand.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}
	}
}
