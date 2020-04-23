using System;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace ManagerByLetterOfAttorney.Db
{
	/// <summary>
	/// Класс управление соединением с БД (MS SQL сервер)
	/// </summary>
	public static class DbControl
	{
		/// <summary>
		/// Получение соединения с базой данных на MSSQL-сервере
		/// </summary>
		public static SqlConnection GetConnection(string server, string database)
		{
			var connectRow = SqlServerConnectRow(server, database);
			return new SqlConnection(connectRow);
		}

		/// <summary>
		/// Попытка открытия соединения с базой данных на сервере
		/// </summary>
		public static void TryConnectOpen(this SqlConnection connect)
		{
			const string errorUncertain = "Непредвиденная ошибка соединения с базой данных [{0}] на сервере [{1}]";
			const string errorCauseDatabase = "База данных [{0}] не доступна";
			const string errorCauseServer = "Сервер [{0}] не доступен";

			const int errorNumberCauseDatabase = 4060;
			const int errorNumberCauseServer = 53;

			var server = connect.DataSource;
			var database = connect.Database;
			try
			{
				connect.Open();
			}
			catch (Exception ex)
			{
				string probableCause; // вероятная причина
				var sqlException = ex as SqlException;
				if (sqlException != null)
				{
					var exceptionNumber = sqlException.Number;
					switch (exceptionNumber)
					{
						case errorNumberCauseServer:      // недоступен сервер
							probableCause = string.Format(errorCauseServer, server);
							break;
						case errorNumberCauseDatabase:    // недоступна база
							probableCause = string.Format(errorCauseDatabase, database);
							break;
						default:                          // неопределённая ошибка открытия соединения
							probableCause = string.Format(errorUncertain, database, server);
							break;
					}
				}
				else // неопределённая ошибка открытия соединения
				{
					probableCause = string.Format(errorUncertain, database, server);
				}
				throw new StorageException(ex.Message, probableCause, ex);
			}
		}

		/// <summary>
		/// Получение строки соединения с сервером mssql
		/// </summary>
		private static string SqlServerConnectRow(string server, string database)
		{
			var connectionTimeoutInSecond = Properties.Settings.Default.TimeoutServerConnectInSecond;
			const string connectionPattern = "Data Source={0};Initial Catalog={1};Connection Timeout={2};" +
			                                 "Persist Security Info=True;Integrated Security=SSPI";
			return string.Format(connectionPattern, server, database, connectionTimeoutInSecond);
		}

		/// <summary>
		/// Обработка типовых исключений работы с БД MSSQL и FoxPro, с формированием сообщения о возможной причине.
		/// В случае, если исходное исключение не SqlException, не StorageException или 
		/// не OleDbException - пробрасывается исходное.
		/// </summary>
		public static Exception HandleKnownDbFoxProAndMssqlServerExceptions(DbException ex)
		{
			const int mssqlDublicateRowErrorCode = 2627;
			const string mssqlDublicateMessage = "Одно или несколько полей добавляемой/редактируемой сущности " +
				"помечены признаком уникальности и уже есть в базе данных. Добавление ещё одного экземляра " +
				"сущности запрещено ключом уникальности.";
			const int mssqlDeleteReferencedRowErrorCode = 547;
			const string mssqlDeleteReferencedRowMessage = "Удаление указанной записи запрещено, так как на " +
				"неё есть как минимум одна действующая ссылка из других таблиц базы данных.";
			const int foxproWrongQueryErrorCode = -2147467259;
			const string foxProWrongQueryMessage = "Ошибка в SQL-запросе к базе данных FoxPro: несовпадение типов, " +
				"неверное имя столбца, отсутсвие столбца или что-то иное.";
			const int foxproFileAccessErrorCode = -2147217865;
			const string foxproFileAccessMessage = "Нет доступа к файлу (или отсутвует файл) базы " +
				"данных FoxPro для выполнения запроса.";

			var isDbException = ex is SqlException || ex is StorageException || ex is OleDbException;
			if (!isDbException)
			{
				return ex; // Исключение является не SqlException, не StorageException, не OleDbException
			}
			var probableCause = string.Empty; // предполагаемая ошибка
			var currentInnerException = ex;
			do
			{
				var sqlException = currentInnerException as SqlException;
				var oleDbException = currentInnerException as OleDbException;
				if (sqlException != null)
				{
					var sqlErrorCode = sqlException.Errors[0].Number;
					if (sqlErrorCode == mssqlDublicateRowErrorCode)        // Причина - дубликат по ключу уникальности 
					{
						probableCause = mssqlDublicateMessage;
						break;
					}
					if (sqlErrorCode == mssqlDeleteReferencedRowErrorCode) // Причина - удаление связанной дочерней
					{
						probableCause = mssqlDeleteReferencedRowMessage;
						break;
					}
				}
				else if (oleDbException != null)
				{
					var oleDbErrorCode = oleDbException.ErrorCode;
					if (oleDbErrorCode == foxproWrongQueryErrorCode)      // Причина - ошибочный запрос к FoxPro БД
					{
						probableCause = foxProWrongQueryMessage;
						break;
					}
					if (oleDbErrorCode == foxproFileAccessErrorCode)      // Причина - недоступен файл FoxPro БД
					{
						probableCause = foxproFileAccessMessage;
						break;
					}
				}
				// следующее вложенное исключение
				currentInnerException = currentInnerException.InnerException as DbException;
			} while (currentInnerException != null);

			// Если неизвестная причина SqlException или OleDbException
			if (string.Empty.Equals(probableCause))
			{
				return new StorageException(ex.Message, ex);
			}

			// Если SqlException или OleDbException с "возможной причиной"
			throw new StorageException(ex.Message, probableCause, ex);
		}
	}
}
