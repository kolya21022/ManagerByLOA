using System;
using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Entities.Enums;
using ManagerByLetterOfAttorney.Entities.External;

namespace ManagerByLetterOfAttorney.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для внешней таблицы сотрудников [kadryN].
	/// Используемая база данных - стороннего приложения АРМ Отдела кадров, редактирование запрещено.
	/// </summary>
	public static class EmployeesStorage
	{
		/// <summary>
		/// Рабочий месяц базы отдела кадров (в зависимости от его значения, берётся одна из 12 таблиц сотрудников)
		/// </summary>
		private static int _cacheWorkMount = int.MinValue;

		/// <summary>
		/// Загрузка текущего рабочего месяца из таблицы 'a_ok' базы данных отдела кадров
		/// </summary>
		public static int WorkMount()
		{
			if (int.MinValue != _cacheWorkMount) // если значение уже загружено в буферное поле класса, вернуть его
			{
				return _cacheWorkMount;
			}
			const string errorMessage = "Не удаётся получить рабочий месяц из базы данных";
			const string selectWorkMountQuery = "SELECT TOP 1 [rabochijmes] FROM [a_ok]";

			var server = Properties.Settings.Default.ServerEmployees;
			var db = Properties.Settings.Default.DbEmployees;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(selectWorkMountQuery, connection))
					{
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								_cacheWorkMount = int.Parse(reader.GetString(0));
								return _cacheWorkMount;
							}
						}
					}
				}
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
			throw new StorageException(errorMessage);
		}

		/// <summary>
		/// Получение коллекции сотрудников (внешняя)
		/// </summary>
		/// <param name="withDismissed">Выбирать уволенных сотрудников</param>
		public static List<Employee> GetAll(bool withDismissed)
		{
			var workMount = WorkMount();
			const string employeesTablePattern = "kadry{0:D2}";
			var employeesTable = string.Format(employeesTablePattern, workMount);
			var queryPattern = "SELECT [paspor02].[tab] AS [personnelNumber], [paspor02].[famil] AS [lastName], " +
			    "[paspor02].[imya] AS [firstName], [paspor02].[otchestvo] AS [middleName], " +
				"[paspor02].[npasport] AS [passportSeriesAndNumber], " +
			    "[paspor02].[kemvid] AS [passportIssuedByOrganization], [paspor02].[datvid] AS [passportIssueDate], " +
			    "[fsprof].[name] AS [profession], [{0}].[pol] AS [sex] " +
			    "FROM [paspor02] " +
				"INNER JOIN [{0}] ON [paspor02].[tab] = [{0}].[tab] " +
			    "LEFT JOIN [fsprof] ON [{0}].[prof] = [fsprof].[prof] ";
			queryPattern += withDismissed ? string.Empty : " WHERE [{0}].[datauv] IS NULL ";
			var querySelectAll = string.Format(queryPattern, employeesTable);

			var server = Properties.Settings.Default.ServerEmployees;
			var db = Properties.Settings.Default.DbEmployees;

			var employees = new List<Employee>();
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
								var employee = new Employee
								{
									PersonnelNumber = (long)reader.GetDouble(0),
									LastName = reader.IsDBNull(1) ? null : reader.GetString(1),
									FirstName = reader.IsDBNull(2) ? null : reader.GetString(2),
									MiddleName = reader.IsDBNull(3) ? null : reader.GetString(3),
									PassportSeriesAndNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
									PassportIssuedByOrganization = reader.IsDBNull(5) ? null : reader.GetString(5),
									PassportIssueDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
									Profession = reader.IsDBNull(7) ? null : reader.GetString(7),
									Sex = SexUtil.GetFromString(reader.GetString(8))
								};
								employees.Add(employee);
							}
						}
					}
				}
				return employees;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Получение сотрудника по указанному табельному номеру
		/// </summary>
		public static Employee GetByPersonelNumber(long personnelNumber)
		{
			var workMount = WorkMount();
			const string employeesTablePattern = "kadry{0:D2}";
			var employeesTable = string.Format(employeesTablePattern, workMount);
			const string queryPattern = "SELECT [paspor02].[tab] AS [personnelNumber], " +
			    "[paspor02].[famil] AS [lastName], [paspor02].[imya] AS [firstName], " +
			    "[paspor02].[otchestvo] AS [middleName], " +
			    "[paspor02].[npasport] AS [passportSeriesAndNumber], " +
			    "[paspor02].[kemvid] AS [passportIssuedByOrganization], [paspor02].[datvid] AS [passportIssueDate], " +
			    "[fsprof].[name] AS [profession], [{0}].[pol] AS [sex] " +
			    "FROM [paspor02] " +
			    "INNER JOIN [{0}] ON [paspor02].[tab] = [{0}].[tab] " +
			    "LEFT JOIN [fsprof] ON [{0}].[prof] = [fsprof].[prof] " +
			    "WHERE [paspor02].[tab] = @personnelNumber";
			var querySelectByPersonnelNumber = string.Format(queryPattern, employeesTable);

			var server = Properties.Settings.Default.ServerEmployees;
			var db = Properties.Settings.Default.DbEmployees;
			try
			{
				using (var connection = DbControl.GetConnection(server, db))
				{
					connection.TryConnectOpen();
					using (var sqlCommand = new SqlCommand(querySelectByPersonnelNumber, connection))
					{
						sqlCommand.Parameters.AddWithValue("@personnelNumber", personnelNumber);
						using (var reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								return new Employee
								{
									PersonnelNumber = (long)reader.GetDouble(0),
									LastName = reader.IsDBNull(1) ? null : reader.GetString(1),
									FirstName = reader.IsDBNull(2) ? null : reader.GetString(2),
									MiddleName = reader.IsDBNull(3) ? null : reader.GetString(3),
									PassportSeriesAndNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
									PassportIssuedByOrganization = reader.IsDBNull(5) ? null : reader.GetString(5),
									PassportIssueDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
									Profession = reader.IsDBNull(7) ? null : reader.GetString(7),
									Sex = SexUtil.GetFromString(reader.GetString(8))
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
	}
}
