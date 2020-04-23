using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Entities.Enums;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для таблицы курьеров [Couriers]
	/// </summary>
	public static class CouriersStorage
	{
		/// <summary>
		/// Получение коллекции [Курьеров]
		/// </summary>
		public static List<Courier> GetAll()
		{
			const string querySelectAll = "SELECT [id], [lastName], [firstName], [middleName], [sex], " +
			    "[passportSeriesAndNumber], [passportIssuedByOrganization], [passportIssueDate], [profession] " +
			    "FROM [Couriers]";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var couriers = new List<Courier>();
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
								var courier = new Courier
								{
									Id = reader.GetInt64(0),
									LastName = reader.IsDBNull(1) ? null : reader.GetString(1),
									FirstName = reader.IsDBNull(2) ? null : reader.GetString(2),
									MiddleName = reader.IsDBNull(3) ? null : reader.GetString(3),
									Sex = SexUtil.GetFromString(reader.GetString(4)),
									PassportSeriesAndNumber = reader.IsDBNull(5) ? null : reader.GetString(5),
									PassportIssuedByOrganization = reader.IsDBNull(6) ? null : reader.GetString(6),
									PassportIssueDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
									Profession = reader.IsDBNull(8) ? null : reader.GetString(8)
								};
								couriers.Add(courier);
							}
						}
					}
				}
				return couriers;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}
		
		/// <summary>
		/// Получение курьера по указанному ID
		/// </summary>
		public static Courier GetById(long courierId)
		{
			const string querySelectById = "SELECT [id], [lastName], [firstName], [middleName], [sex], " +
			    "[passportSeriesAndNumber], [passportIssuedByOrganization], [passportIssueDate], [profession] " +
			    "FROM [Couriers] WHERE [id] = @id";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(querySelectById, connection))
					{
						sqlCommand.Parameters.AddWithValue("@id", courierId);
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								return new Courier
								{
									Id = reader.GetInt64(0),
									LastName = reader.IsDBNull(1) ? null : reader.GetString(1),
									FirstName = reader.IsDBNull(2) ? null : reader.GetString(2),
									MiddleName = reader.IsDBNull(3) ? null : reader.GetString(3),
									Sex = SexUtil.GetFromString(reader.GetString(4)),
									PassportSeriesAndNumber = reader.IsDBNull(5) ? null : reader.GetString(5),
									PassportIssuedByOrganization = reader.IsDBNull(6) ? null : reader.GetString(6),
									PassportIssueDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
									Profession = reader.IsDBNull(8) ? null : reader.GetString(8)
								};
							}
						}
					}
				}
				return null;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение коллекции [Курьеров] с указанным полем числа ссылок на запись из других таблиц
		/// </summary>
		public static List<Courier> GetAllWithCountUsing()
		{
			const string querySelectAllWithCountUsing = "SELECT [Couriers].[id], [Couriers].[lastName], " +
			    "[Couriers].[firstName], [Couriers].[middleName], [Couriers].[sex], " +
			    "[Couriers].[passportSeriesAndNumber], [Couriers].[passportIssuedByOrganization], " +
			    "[Couriers].[passportIssueDate], [Couriers].[profession], " +
			    "COUNT([LetterOfAttorneys].[CouriersId]) AS serviceCountUsed " +
			    "FROM [Couriers] " +
			    "LEFT JOIN [LetterOfAttorneys] ON [LetterOfAttorneys].[CouriersId] = [Couriers].[id] " +
			    "GROUP BY [Couriers].[id], [Couriers].[lastName], [Couriers].[firstName], [Couriers].[middleName], " +
			    "[Couriers].[sex], [Couriers].[passportSeriesAndNumber], [Couriers].[passportIssuedByOrganization], " +
			    "[Couriers].[passportIssueDate], [Couriers].[profession] ";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var couriers = new List<Courier>();
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
								var courier = new Courier
								{
									Id = reader.GetInt64(0),
									LastName = reader.IsDBNull(1) ? null : reader.GetString(1),
									FirstName = reader.IsDBNull(2) ? null : reader.GetString(2),
									MiddleName = reader.IsDBNull(3) ? null : reader.GetString(3),
									Sex = SexUtil.GetFromString(reader.GetString(4)),
									PassportSeriesAndNumber = reader.IsDBNull(5) ? null : reader.GetString(5),
									PassportIssuedByOrganization = reader.IsDBNull(6) ? null : reader.GetString(6),
									PassportIssueDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
									Profession = reader.IsDBNull(8) ? null : reader.GetString(8),
									ServiceCountUsed = reader.GetInt32(9)
								};
								couriers.Add(courier);
							}
						}
					}
				}
				return couriers;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Добавление нового [Курьера]
		/// </summary>
		public static Courier Insert(string lastName, string firstName, string middleName, Sex sex, 
			string passportSeriesAndNumber, string passportIssuedByOrganization, DateTime passportIssueDate, 
			string profession)
		{
			const string queryInsert = "INSERT INTO [Couriers] ([lastName], [firstName], [middleName], [sex], " +
			    "[passportSeriesAndNumber], [passportIssuedByOrganization], [passportIssueDate], [profession]) " +
				"OUTPUT INSERTED.ID " +
				"VALUES (@lastName, @firstName, @middleName, @sex, @passportSeriesAndNumber, " +
			    "@passportIssuedByOrganization, @passportIssueDate, @profession)";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryInsert, connection))
					{
						sqlCommand.Parameters.AddWithValue("@lastName", lastName);
						sqlCommand.Parameters.AddWithValue("@firstName", firstName);
						sqlCommand.Parameters.AddWithValue("@middleName", middleName);
						sqlCommand.Parameters.AddWithValue("@sex", sex.ToString());
						sqlCommand.Parameters.AddWithValue("@passportSeriesAndNumber", passportSeriesAndNumber);
						sqlCommand.Parameters.AddWithValue("@passportIssuedByOrganization", passportIssuedByOrganization);
						sqlCommand.Parameters.AddWithValue("@passportIssueDate", passportIssueDate);
						sqlCommand.Parameters.AddWithValue("@profession", profession);
						var id = (long)sqlCommand.ExecuteScalar();
						return new Courier
						{
							Id = id,
							LastName = lastName,
							FirstName = firstName,
							MiddleName = middleName,
							Sex = sex,
							PassportSeriesAndNumber = passportSeriesAndNumber,
							PassportIssuedByOrganization = passportIssuedByOrganization,
							PassportIssueDate = passportIssueDate,
							Profession = profession
						};
					}
				}
			}
			catch (DbException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Обновление [Курьера] с указанным [Id]
		/// </summary>
		public static Courier Update(long id, string lastName, string firstName, string middleName, Sex sex, 
			string passportSeriesAndNumber, string passportIssuedByOrganization, DateTime passportIssueDate, 
			string profession)
		{
			const string queryUpdate = "UPDATE Couriers SET lastName = @lastName, firstName = @firstName, " +
			    "middleName = @middleName, sex = @sex, passportSeriesAndNumber = @passportSeriesAndNumber, " +
				"passportIssuedByOrganization = @passportIssuedByOrg, passportIssueDate = @passportIssueDate, " +
			    "profession = @profession WHERE id = @id";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryUpdate, connection))
					{
						sqlCommand.Parameters.AddWithValue("@lastName", lastName);
						sqlCommand.Parameters.AddWithValue("@firstName", firstName);
						sqlCommand.Parameters.AddWithValue("@middleName", middleName);
						sqlCommand.Parameters.AddWithValue("@sex", sex.ToString());
						sqlCommand.Parameters.AddWithValue("@passportSeriesAndNumber", passportSeriesAndNumber);
						sqlCommand.Parameters.AddWithValue("@passportIssuedByOrg", passportIssuedByOrganization);
						sqlCommand.Parameters.AddWithValue("@passportIssueDate", passportIssueDate);
						sqlCommand.Parameters.AddWithValue("@profession", profession);
						sqlCommand.Parameters.AddWithValue("@id", id);
						sqlCommand.ExecuteNonQuery();
						return new Courier
						{
							Id = id,
							LastName = lastName,
							FirstName = firstName,
							MiddleName = middleName,
							Sex = sex,
							PassportSeriesAndNumber = passportSeriesAndNumber,
							PassportIssuedByOrganization = passportIssuedByOrganization,
							PassportIssueDate = passportIssueDate,
							Profession = profession
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
		/// Удаление [Курьера] с указанным [Id]
		/// </summary>
		public static void DeleteById(long id)
		{
			const string queryDelete = "DELETE FROM [Couriers] WHERE [id] = @id";
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
