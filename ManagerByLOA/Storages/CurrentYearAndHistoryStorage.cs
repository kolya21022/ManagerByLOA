using System;
using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.Entities.External;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для таблиц истории предыдущих лет [PreviousYearHistories] и 
	/// таблицы текущего года выдачи доверенностей [CurrentDbYear]
	/// </summary>
	public static class CurrentYearAndHistoryStorage
	{
		/// <summary>
		/// Получение текущего года заполнения доверенностей из БД
		/// </summary>
		public static int CurrentYearFromDb()
		{
			const string querySelectCurrentYear = "SELECT TOP 1 [year] FROM [CurrentDbYear]";
			const string failedGetYearMessage = "Не удалось получить текущий год из базы данных";

			var db = Properties.Settings.Default.DbLetterOfAttorney;
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var command = new SqlCommand(querySelectCurrentYear, connection))
					{
						using (var reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								return unchecked((int)reader.GetInt64(0));
							}
						}
					}
				}
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
			throw new StorageException(failedGetYearMessage);
		}

		/// <summary>
		/// Обновление текущего года выдачи доверенностей в базе данных
		/// </summary>
		public static void UpdateCurrentYearInDb(SqlConnection connection, SqlTransaction transaction, int year)
		{
			const string queryUpdateCurrentYear = "UPDATE [CurrentDbYear] " +
			                                      "SET [year] = @year, [dateTransitionToYear] = GETDATE()";
			using (var sqlCommand = new SqlCommand(queryUpdateCurrentYear, connection, transaction))
			{
				sqlCommand.Parameters.AddWithValue("@year", year);
				sqlCommand.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Вставка всех доверенностей предыдущего года в таблицу [Истории доверенностей предыдущих лет]
		/// </summary>
		public static void InsertHistoryLetterOfAttorneys(IEnumerable<LetterOfAttorney> letterOfAttorneys, 
			int archivalYear, SqlConnection connection, SqlTransaction transaction)
		{
			const string fullNamePattern = Constants.EmployeeFullNamePattern;
			const string queryInsertHistoryItem = "INSERT INTO [PreviousYearHistories] ([year], [ordinalNumber], " +
				"[validityDateStart], [validityDateEnd], [company], [employeePersonnelNumber], " +
			    "[courierOrEmployeeFullName], [courierOrEmployeeProfession], [passportSeriesAndNumber], " +
				"[passportIssuedByOrganization], [passportIssueDate], [shipmentName], [shipmentCount], " +
				"[shipmentMeasure]) " +
			    "VALUES (@year, @ordinalNumber, @validityDateStart, @validityDateEnd, @company, " +
			    "@employeePersonnelNumber, @courierOrEmployeeFullName, @courierOrEmployeeProfession, " +
			    "@passportSeriesAndNumber, @passportIssuedByOrganization, @passportIssueDate, @shipmentName, " +
			    "@shipmentCount, @shipmentMeasure)";
			using (var command = new SqlCommand(queryInsertHistoryItem, connection, transaction))
			{
				foreach (var letterOfAttorney in letterOfAttorneys)
				{
					// Подготовка параметров вставки 
					// (null-значения полей сотрудника возможны, если сотрудник удалён из базы данных отдела кадров)

					var isPersonExist = letterOfAttorney.Courier != null;
					var employeePersonnelNumber = isPersonExist && letterOfAttorney.Courier is Employee 
						? ((Employee)letterOfAttorney.Courier).PersonnelNumber 
						: (object)DBNull.Value;
					var firstName = isPersonExist ? letterOfAttorney.Courier.FirstName : (object)DBNull.Value;
					var lastName = isPersonExist ? letterOfAttorney.Courier.LastName : (object)DBNull.Value;
					var middleName = isPersonExist ? letterOfAttorney.Courier.MiddleName : (object)DBNull.Value;
					var courierOrEmployeeFullName = lastName != null || firstName != null || middleName != null 
						? string.Format(fullNamePattern, lastName, firstName, middleName) : (object)DBNull.Value;
					var passportNumber = isPersonExist && letterOfAttorney.Courier.PassportSeriesAndNumber != null 
						? letterOfAttorney.Courier.PassportSeriesAndNumber 
						: (object)DBNull.Value;
					var passportIssued = isPersonExist && letterOfAttorney.Courier.PassportIssuedByOrganization != null
						? letterOfAttorney.Courier.PassportIssuedByOrganization 
						: (object)DBNull.Value;
					var courierOrEmployeeProfession = isPersonExist && letterOfAttorney.Courier.Profession != null 
						? letterOfAttorney.Courier.Profession 
						: (object)DBNull.Value;
					var passportIssueDate = isPersonExist && letterOfAttorney.Courier.PassportIssueDate != null 
						? letterOfAttorney.Courier.PassportIssueDate 
						: (object)DBNull.Value;
					var ordinalNumber = letterOfAttorney.OrdinalNumber;
					var validityDateStart = letterOfAttorney.ValidityDateStart;
					var validityDateEnd = letterOfAttorney.ValidityDateEnd;
					var company = letterOfAttorney.Company.Name;

					foreach (var shipment in letterOfAttorney.Shipments)
					{
						var shipmentName = shipment.Cargo.Name;
						var shipmentMeasure = (object)DBNull.Value;
						var shipmentCount = shipment.Count ?? (object)DBNull.Value;
						if (shipment.Cargo.Measure != null && shipment.Cargo.Measure.Name != null)
						{
							shipmentMeasure = shipment.Cargo.Measure.Name;
						}

						command.Parameters.Clear();
						command.Parameters.AddWithValue("@year", archivalYear);
						command.Parameters.AddWithValue("@ordinalNumber", ordinalNumber);
						command.Parameters.AddWithValue("@validityDateStart", validityDateStart);
						command.Parameters.AddWithValue("@validityDateEnd", validityDateEnd);
						command.Parameters.AddWithValue("@company", company);
						command.Parameters.AddWithValue("@employeePersonnelNumber", employeePersonnelNumber);
						command.Parameters.AddWithValue("@courierOrEmployeeFullName", courierOrEmployeeFullName);
						command.Parameters.AddWithValue("@courierOrEmployeeProfession", courierOrEmployeeProfession);
						command.Parameters.AddWithValue("@passportSeriesAndNumber", passportNumber);
						command.Parameters.AddWithValue("@passportIssuedByOrganization", passportIssued);
						command.Parameters.AddWithValue("@passportIssueDate", passportIssueDate);
						command.Parameters.AddWithValue("@shipmentName", shipmentName);
						command.Parameters.AddWithValue("@shipmentCount", shipmentCount);
						command.Parameters.AddWithValue("@shipmentMeasure", shipmentMeasure);
						command.ExecuteNonQuery();
					}
				}
			}
		}

		/// <summary>
		/// Получение коллекции архива доверенностей (перевозимых сущностей доверенностей) предыдущих лет
		/// </summary>
		public static List<HistoryItem> GetHistories()
		{
			const string query = "SELECT [id], [year], [ordinalNumber], [validityDateStart], [validityDateEnd], " +
			    "[company], [employeePersonnelNumber], [courierOrEmployeeFullName], [courierOrEmployeeProfession], " +
			    "[passportSeriesAndNumber], [passportIssuedByOrganization], [passportIssueDate], [shipmentName], " +
			    "[shipmentCount], [shipmentMeasure] " +
				"FROM [PreviousYearHistories]";
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;
			var historyItems = new List<HistoryItem>();
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var command = new SqlCommand(query, connection))
					{
						using (var reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								var id = reader.GetInt64(0);
								var year = reader.GetInt64(1);
								var ordinalNumber = reader.GetInt64(2);
								var validityDateStart = reader.GetDateTime(3);
								var validityDateEnd = reader.GetDateTime(4);
								var company = reader.GetString(5);
								var employeePersonnelNumber = reader.IsDBNull(6) ? (long?)null : reader.GetInt64(6);
								var courierOrEmployeeFullName = reader.IsDBNull(7) ? null : reader.GetString(7);
								var courierOrEmployeeProfession = reader.IsDBNull(8) ? null : reader.GetString(8);
								var passportSeriesAndNumber = reader.IsDBNull(9) ? null : reader.GetString(9);
								var passportIssuedByOrganization = reader.IsDBNull(10) ? null : reader.GetString(10);
								var passportIssueDate = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11);
								var shipmentName = reader.GetString(12);
								var shipmentCount = reader.IsDBNull(13) ? (double?)null : reader.GetDouble(13);
								var shipmentMeasure = reader.IsDBNull(14) ? null : reader.GetString(14);
								var historyItem = new HistoryItem
								{
									Id = id,
									Year = year,
									OrdinalNumber = ordinalNumber,
									ValidityDateStart = validityDateStart,
									ValidityDateEnd = validityDateEnd,
									Company = company,
									EmployeePersonnelNumber = employeePersonnelNumber,
									CourierOrEmployeeFullName = courierOrEmployeeFullName,
									CourierOrEmployeeProfession = courierOrEmployeeProfession,
									PassportSeriesAndNumber = passportSeriesAndNumber,
									PassportIssuedByOrganization = passportIssuedByOrganization,
									PassportIssueDate = passportIssueDate,
									ShipmentName = shipmentName,
									ShipmentCount = shipmentCount,
									ShipmentMeasure = shipmentMeasure
								};
								historyItems.Add(historyItem);
							}
						}
					}
				}
				return historyItems;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}
	}
}
