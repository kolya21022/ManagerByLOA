using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Entities.External;

namespace ManagerByLetterOfAttorney.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для внешней таблицы организаций с указанным УНП/Городом [CompanyWithUnp].
	/// Используемая база данных - стороннего приложения АРМ Поставщиков-Потребителей, редактирование запрещено.
	/// </summary>
	public static class CompaniesWithUnpStorage
	{
		/// <summary>
		/// Получение коллекции [Организаций с указанным УНП/Городом] (внешняя)
		/// </summary>
		public static List<CompanyWithUnp> GetAll()
		{
			const string queryGetAll = "SELECT [Consumers].[name] AS [company], [CityPhoneCodes].[locality], " +
			                     "[taxpayerCode] FROM [Consumers], [CityPhoneCodes] " +
			                     "WHERE [Consumers].[cityPhoneCodesId] = [CityPhoneCodes].[id]";
			var server = Properties.Settings.Default.ServerConsumers;
			var db = Properties.Settings.Default.DbConsumers;

			var companiesWithUnp = new List<CompanyWithUnp>();
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
								var company = reader.IsDBNull(0) ? null : reader.GetString(0);
								var city = reader.IsDBNull(1) ? null : reader.GetString(1);
								var unp = reader.IsDBNull(2) ? null : reader.GetString(2);
								if (string.IsNullOrWhiteSpace(company))
								{
									continue;
								}
								var companyWithUnp = new CompanyWithUnp
								{
									Name = company,
									City = city,
									Unp = unp
								};
								companiesWithUnp.Add(companyWithUnp);
							}
						}
					}
				}
				return companiesWithUnp;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}
	}
}
