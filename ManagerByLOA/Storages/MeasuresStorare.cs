using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для таблицы единиц измерения [Measures]
	/// </summary>
	public static class MeasuresStorare
	{
		/// <summary>
		/// Получение коллекции [Единиц измерения]
		/// </summary>
		public static List<Measure> GetAll()
		{
			const string querySelectAll = "SELECT [id], [name] FROM [Measures] ORDER BY [name]";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var measures = new List<Measure>();
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(querySelectAll, connection))
					{
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								var measure = new Measure
								{
									Id = reader.GetInt64(0),
									Name = reader.GetString(1)
								};
								measures.Add(measure);
							}
						}
					}
				}
				return measures;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение коллекции [Единиц измерения]
		/// </summary>
		public static List<Measure> GetAll(SqlConnection connection, SqlTransaction transaction)
		{
			const string querySelectAll = "SELECT [id], [name] FROM [Measures] ORDER BY [name]";
			var measures = new List<Measure>();
			using (var sqlCommand = new SqlCommand(querySelectAll, connection, transaction))
			{
				using (var reader = sqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						var measure = new Measure
						{
							Id = reader.GetInt64(0),
							Name = reader.GetString(1)
						};
						measures.Add(measure);
					}
				}
			}
			return measures;
		}

		/// <summary>
		/// Получение коллекции [Единиц измерения] с указанным полем числа ссылок на запись из других таблиц
		/// </summary>
		public static List<Measure> GetAllWithCountUsing()
		{
			const string querySelectAllWithCountUsing = "SELECT [Measures].[id], [Measures].[name], " +
			    "COUNT([Cargoes].[measuresId]) AS [serviceCountUsed] " +
			    "FROM [Measures] LEFT JOIN [Cargoes] ON [Cargoes].[measuresId] = [Measures].[id] " +
			    "GROUP BY [Measures].[id], [Measures].[name] ";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var measures = new List<Measure>();
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(querySelectAllWithCountUsing, connection))
					{
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								var measure = new Measure
								{
									Id = reader.GetInt64(0),
									Name = reader.GetString(1),
									ServiceCountUsed = reader.GetInt32(2)
								};
								measures.Add(measure);
							}
						}
					}
				}
				return measures;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение коллекции [Единиц измерения] по указанному названию
		/// </summary>
		public static List<Measure> GetByName(string findedName)
		{
			const string queryGetByName = "SELECT [id], [name] FROM [Measures] WHERE [name] = @findedName";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var measures = new List<Measure>();
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryGetByName, connection))
					{
						sqlCommand.Parameters.AddWithValue("@findedName", findedName);
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								var measure = new Measure
								{
									Id = reader.GetInt64(0),
									Name = reader.GetString(1)
								};
								measures.Add(measure);
							}
						}
					}
				}
				return measures;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение коллекции [Единиц измерения] по указанному названию
		/// </summary>
		public static List<Measure> GetByName(string findedName, SqlConnection connection, SqlTransaction transaction)
		{
			const string queryGetByName = "SELECT [id], [name] FROM [Measures] WHERE [name] = @findedName";
			var measures = new List<Measure>();
			using (var sqlCommand = new SqlCommand(queryGetByName, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@findedName", findedName);
				using (var reader = sqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						var measure = new Measure
						{
							Id = reader.GetInt64(0),
							Name = reader.GetString(1)
						};
						measures.Add(measure);
					}
				}
			}
			return measures;
		}

		/// <summary>
		/// Добавление новой [Единицы измерения]
		/// </summary>
		public static Measure Insert(string name)
		{
			const string queryInsert = "INSERT INTO [Measures] ([name]) OUTPUT INSERTED.ID VALUES (@name)";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryInsert, connection))
					{
						sqlCommand.Parameters.AddWithValue("@name", name);
						var id = (long)sqlCommand.ExecuteScalar();
						return new Measure
						{
							Id = id,
							Name = name
						};
					}
				}
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Добавление новой [Единицы измерения]
		/// </summary>
		public static Measure Insert(string name, SqlConnection connection, SqlTransaction transaction)
		{
			const string queryInsert = "INSERT INTO [Measures] ([name]) OUTPUT INSERTED.ID VALUES (@name)";
			using (var sqlCommand = new SqlCommand(queryInsert, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@name", name);
				var id = (long)sqlCommand.ExecuteScalar();
				return new Measure
				{
					Id = id,
					Name = name
				};
			}
		}
		
		/// <summary>
		/// Обновление [Единицы измерения] с указанным [Id]
		/// </summary>
		public static Measure Update(long id, string name)
		{
			const string queryUpdate = "UPDATE [Measures] SET [name] = @name WHERE [id] = @id";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryUpdate, connection))
					{
						sqlCommand.Parameters.AddWithValue("@name", name);
						sqlCommand.Parameters.AddWithValue("@id", id);
						sqlCommand.ExecuteNonQuery();
						return new Measure
						{
							Id = id,
							Name = name
						};
					}
				}
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Удаление [Единицы измерения] с указанным [Id]
		/// </summary>
		public static void DeleteById(long id)
		{
			const string queryDelete = "DELETE FROM [Measures] WHERE [id] = @id";
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
