using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для таблицы организаций [Companies] (внутренней)
	/// </summary>
	public static class CompaniesStorage
	{
		/// <summary>
		/// Получение коллекции [Организаций]
		/// </summary>
		public static List<Company> GetAll()
		{
			const string queryGetAll = "SELECT [id], [name] FROM [Companies] ORDER BY [name]";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var companies = new List<Company>();
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();

					using (var sqlCommand = new SqlCommand(queryGetAll, connection))
					{
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								var company = new Company
								{
									Id = reader.GetInt64(0),
									Name = reader.GetString(1)
								};
								companies.Add(company);
							}
						}
					}
				}
				return companies;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение коллекции [Организаций] с указанным полем числа ссылок на запись из других таблиц
		/// </summary>
		public static List<Company> GetAllWithCountUsing()
		{
			const string querySelectAllWithCountUsing = "SELECT [Companies].[id], [Companies].[name], " +
			    "COUNT([LetterOfAttorneys].[companiesId]) AS serviceCountUsed " +
			    "FROM [Companies] " +
			    "LEFT JOIN [LetterOfAttorneys] ON [LetterOfAttorneys].[companiesId] = [Companies].[id] " +
			    "GROUP BY [Companies].[id], [Companies].[name] ";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var companies = new List<Company>();
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
								var company = new Company
								{
									Id = reader.GetInt64(0),
									Name = reader.GetString(1),
									ServiceCountUsed = reader.GetInt32(2)
								};
								companies.Add(company);
							}
						}
					}
				}
				return companies;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение [Организации] по указанному [Id]
		/// </summary>
		public static Company GetById(long findId)
		{
			const string queryGetById = "SELECT TOP 1 [id], [name] FROM [Companies] WHERE [id] = @finded";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			Company company = null;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryGetById, connection))
					{
						sqlCommand.Parameters.AddWithValue("@finded", findId);
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								company = new Company
								{
									Id = reader.GetInt64(0),
									Name = reader.GetString(1)
								};
								break;
							}
						}
					}
				}
				return company;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение коллекции [Организаций] по указанному названию
		/// </summary>
		public static List<Company> GetByName(string findedName)
		{
			const string queryGetByName = "SELECT [id], [name] FROM [Companies] WHERE [name] = @findedName";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var companies = new List<Company>();
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
								var company = new Company
								{
									Id = reader.GetInt64(0),
									Name = reader.GetString(1)
								};
								companies.Add(company);
							}
						}
					}
				}
				return companies;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Добавление новой [Организации]
		/// </summary>
		public static Company Insert(string name)
		{
			const string queryInsert = "INSERT INTO [Companies] ([name]) OUTPUT INSERTED.ID VALUES (@name)";
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
						return new Company
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
		/// Добавление новой [Организации]
		/// </summary>
		public static Company Insert(string name, SqlConnection connection, SqlTransaction transaction)
		{
			const string queryInsert = "INSERT INTO [Companies] ([name]) OUTPUT INSERTED.ID VALUES (@name)";
			using (var sqlCommand = new SqlCommand(queryInsert, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@name", name);
				var id = (long)sqlCommand.ExecuteScalar();
				return new Company
				{
					Id = id,
					Name = name
				};
			}
		}

		/// <summary>
		/// Обновление [Организации] с указанным [Id]
		/// </summary>
		public static Company Update(long id, string name)
		{
			const string queryUpdate = "UPDATE [Companies] SET [name] = @name WHERE [id] = @id";
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
						return new Company
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
		/// Удаление [Организации] с указанным [Id]
		/// </summary>
		public static void DeleteById(long id)
		{
			const string queryDelete = "DELETE FROM [Companies] WHERE [id] = @id";
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
