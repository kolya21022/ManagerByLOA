using System;
using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Entities.External;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для таблицы доверенностей [LetterOfAttorneys]
	/// </summary>
	public static class LetterOfAttorneysStorage
	{
		/// <summary>
		/// Получение текущего максимального порядкового номера доверенностей текущего года
		/// </summary>
		public static long? GetMaxOrdinalNumber()
		{
			const string queryMaxOrdinal = "SELECT TOP 1 MAX([ordinalNumber]) FROM [LetterOfAttorneys]";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			long? ordinalNumber = null;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryMaxOrdinal, connection))
					{
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								ordinalNumber = reader.IsDBNull(0) ? (long?)null : reader.GetInt64(0);
								break;
							}
						}
					}
				}
				return ordinalNumber;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение коллеции [доверенностей], с указанными ID [организации], 
		/// флагом принадлежности человека перевозящего груз к сотрудникам организации, ID курьера или 
		/// табельным номером сотрудника (в зависимости от значения флага) 
		/// </summary>
		public static List<LetterOfAttorney> GetAll()
		{
			const string querySelectAll = "SELECT [id], [ordinalNumber], [validityDateStart], [validityDateEnd], " +
				"[isEmployeeCompany], [employeePersonnelNumber], [couriersId], [companiesId] " +
				"FROM [LetterOfAttorneys] ORDER BY [ordinalNumber]";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			var letterOfAttorneys = new List<LetterOfAttorney>();
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
								var letterOfAttorney = new LetterOfAttorney
								{
									Id = reader.GetInt64(0),
									OrdinalNumber = reader.GetInt64(1),
									ValidityDateStart = reader.GetDateTime(2),
									ValidityDateEnd = reader.GetDateTime(3),
									ServiceMappedCompanyId = reader.GetInt64(7)
								};
								var isEmployee = reader.GetBoolean(4);
								letterOfAttorney.ServiceMappedIsEmployee = isEmployee;
								letterOfAttorney.ServiceMappedCourierIdOrEmployeePersonnel
									= reader.GetInt64(isEmployee ? 5 : 6);
								letterOfAttorneys.Add(letterOfAttorney);
							}
						}
					}
				}
				return letterOfAttorneys;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение [Доверенности] по указанному порядковому номеру, с указанным ID [организации], 
		/// флагом принадлежности человека перевозящего груз к сотрудникам организации, ID курьера или 
		/// табельным номером сотрудника (в зависимости от значения флага) 
		/// </summary>
		public static LetterOfAttorney GetByOrdinalNumber(long findedOrdinal)
		{
			const string queryByOrdinal = "SELECT [id], [ordinalNumber], [validityDateStart], [validityDateEnd], " +
			    "[isEmployeeCompany], [employeePersonnelNumber], [couriersId], [companiesId] " +
			    "FROM [LetterOfAttorneys] WHERE [ordinalNumber] = @ordinalNumber";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(queryByOrdinal, connection))
					{
						sqlCommand.Parameters.AddWithValue("@ordinalNumber", findedOrdinal);
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								var letterOfAttorney = new LetterOfAttorney
								{
									Id = reader.GetInt64(0),
									OrdinalNumber = reader.GetInt64(1),
									ValidityDateStart = reader.GetDateTime(2),
									ValidityDateEnd = reader.GetDateTime(3),
									ServiceMappedCompanyId = reader.GetInt64(7)
								};
								var isEmployee = reader.GetBoolean(4);
								letterOfAttorney.ServiceMappedIsEmployee = isEmployee;
								letterOfAttorney.ServiceMappedCourierIdOrEmployeePersonnel
									= reader.GetInt64(isEmployee ? 5 : 6);
								return letterOfAttorney;
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
		/// Добавление новой [доверенности] с типом перевозящего груз [Курьер]
		/// </summary>
		public static LetterOfAttorney Insert(long ordinalNumber, DateTime validityDateStart, 
			DateTime validityDateEnd, Company company, Courier courier, SqlConnection connection, 
			SqlTransaction transaction)
		{
			const string queryInsertWithCourier = "INSERT INTO [LetterOfAttorneys] ([ordinalNumber], [companiesId], " +
			    "[isEmployeeCompany], [employeePersonnelNumber], [couriersId], [validityDateStart], " +
			    "[validityDateEnd]) OUTPUT INSERTED.ID " +
			    "VALUES (@ordinalNumber, @companiesId, 0, null, @couriersId, @validityDateStart, @validityDateEnd)";
			using (var sqlCommand = new SqlCommand(queryInsertWithCourier, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@ordinalNumber", ordinalNumber);
				sqlCommand.Parameters.AddWithValue("@companiesId", company.Id);
				sqlCommand.Parameters.AddWithValue("@couriersId", courier.Id);
				sqlCommand.Parameters.AddWithValue("@validityDateStart", validityDateStart);
				sqlCommand.Parameters.AddWithValue("@validityDateEnd", validityDateEnd);
				var id = (long)sqlCommand.ExecuteScalar();
				return new LetterOfAttorney
				{
					Id = id,
					OrdinalNumber = ordinalNumber,
					Courier = courier,
					Company = company,
					ValidityDateStart = validityDateStart,
					ValidityDateEnd = validityDateEnd,
					Shipments = new List<Shipment>()
				};
			}
		}

		/// <summary>
		/// Добавление новой [доверенности] с типом перевозящего груз [Сотрудник]
		/// </summary>
		public static LetterOfAttorney Insert(long ordinalNumber, DateTime validityDateStart, 
			DateTime validityDateEnd, Company company, Employee employee, SqlConnection connection, 
			SqlTransaction transaction)
		{
			const string queryInsertWithEmployee = "INSERT INTO [LetterOfAttorneys] ([ordinalNumber], " +
			    "[companiesId], [isEmployeeCompany], [employeePersonnelNumber], [couriersId], [validityDateStart], " +
			    "[validityDateEnd]) OUTPUT INSERTED.ID " +
				"VALUES (@ordinalNumber, @companiesId, 1, @employeePersonnelNumber, null, @validityDateStart, " +
			    "@validityDateEnd)";
			using (var sqlCommand = new SqlCommand(queryInsertWithEmployee, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@ordinalNumber", ordinalNumber);
				sqlCommand.Parameters.AddWithValue("@companiesId", company.Id);
				sqlCommand.Parameters.AddWithValue("@employeePersonnelNumber", employee.PersonnelNumber);
				sqlCommand.Parameters.AddWithValue("@validityDateStart", validityDateStart);
				sqlCommand.Parameters.AddWithValue("@validityDateEnd", validityDateEnd);
				var id = (long)sqlCommand.ExecuteScalar();
				return new LetterOfAttorney
				{
					Id = id,
					OrdinalNumber = ordinalNumber,
					Courier = employee,
					Company = company,
					ValidityDateStart = validityDateStart,
					ValidityDateEnd = validityDateEnd,
					Shipments = new List<Shipment>()
				};
			}
		}

		/// <summary>
		/// Обновление [доверенности] с указанным [Id] и типом перевозящего груз [Курьер]
		/// </summary>
		public static LetterOfAttorney Update(long id, long ordinalNumber, DateTime validityDateStart, 
			DateTime validityDateEnd, Company company, Courier courier, SqlConnection connection, 
			SqlTransaction transaction)
		{
			const string queryUpdateWithCourier = "UPDATE [LetterOfAttorneys] " +
			    "SET [ordinalNumber] = @ordinalNumber, [companiesId] = @companiesId, [isEmployeeCompany] = 0, " +
			    "[employeePersonnelNumber] = NULL, [couriersId] = @couriersId, " +
			    "[validityDateStart] = @validityDateStart, [validityDateEnd] = @validityDateEnd " +
			    "WHERE [id] = @id";
			using (var sqlCommand = new SqlCommand(queryUpdateWithCourier, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@ordinalNumber", ordinalNumber);
				sqlCommand.Parameters.AddWithValue("@companiesId", company.Id);
				sqlCommand.Parameters.AddWithValue("@couriersId", courier.Id);
				sqlCommand.Parameters.AddWithValue("@validityDateStart", validityDateStart);
				sqlCommand.Parameters.AddWithValue("@validityDateEnd", validityDateEnd);
				sqlCommand.Parameters.AddWithValue("@id", id);
				sqlCommand.ExecuteScalar();
				return new LetterOfAttorney
				{
					Id = id,
					OrdinalNumber = ordinalNumber,
					Courier = courier,
					Company = company,
					ValidityDateStart = validityDateStart,
					ValidityDateEnd = validityDateEnd,
					Shipments = new List<Shipment>()
				};
			}
		}

		/// <summary>
		/// Обновление [доверенности] с указанным [Id] и типом перевозящего груз [Сотрудник]
		/// </summary>
		public static LetterOfAttorney Update(long id, long ordinalNumber, DateTime validityDateStart, 
			DateTime validityDateEnd, Company company, Employee employee, SqlConnection connection, 
			SqlTransaction transaction)
		{
			const string queryUpdateWithemployee = "UPDATE [LetterOfAttorneys] " +
			    "SET [ordinalNumber] = @ordinalNumber, [companiesId] = @companiesId, [isEmployeeCompany] = 1, " +
				"[employeePersonnelNumber] = @employeePersonnelNumber, [couriersId] = NULL, " +
				"[validityDateStart] = @validityDateStart, [validityDateEnd] = @validityDateEnd " +
			    "WHERE [id] = @id";
			using (var sqlCommand = new SqlCommand(queryUpdateWithemployee, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@ordinalNumber", ordinalNumber);
				sqlCommand.Parameters.AddWithValue("@companiesId", company.Id);
				sqlCommand.Parameters.AddWithValue("@employeePersonnelNumber", employee.PersonnelNumber);
				sqlCommand.Parameters.AddWithValue("@validityDateStart", validityDateStart);
				sqlCommand.Parameters.AddWithValue("@validityDateEnd", validityDateEnd);
				sqlCommand.Parameters.AddWithValue("@id", id);
				sqlCommand.ExecuteScalar();
				return new LetterOfAttorney
				{
					Id = id,
					OrdinalNumber = ordinalNumber,
					Courier = employee,
					Company = company,
					ValidityDateStart = validityDateStart,
					ValidityDateEnd = validityDateEnd,
					Shipments = new List<Shipment>()
				};
			}
		}

		/// <summary>
		/// Удаление всех [доверенностей]
		/// </summary>
		public static void DeleteAll(SqlConnection connection, SqlTransaction transaction)
		{
			const string query = "DELETE FROM [LetterOfAttorneys]";
			using (var sqlCommand = new SqlCommand(query, connection, transaction))
			{
				sqlCommand.ExecuteNonQuery();
			}
		}
	}
}
