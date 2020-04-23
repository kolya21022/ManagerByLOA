using System;
using System.Data.SqlClient;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Storages;
using ManagerByLetterOfAttorney.Entities.External;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для классов организаций - [Company] и [CompanyWithUnp]
	/// </summary>
	public static class CompaniesService
	{
		/// <summary>
		/// Получение коллекции [Организаций] (внутренних) 
		/// </summary>
		public static List<Company> GetAllInternal()
		{
			return CompaniesStorage.GetAll();
		}

		/// <summary>
		/// Получение коллекции [Организаций] (внутренних) с указанным полем числа ссылок на запись из других таблиц 
		/// </summary>
		public static List<Company> GetAllInternalWithCountUsing()
		{
			return CompaniesStorage.GetAllWithCountUsing();
		}

		/// <summary>
		/// Получение коллекции [Организаций] (внешних, из АРМ Поставщиков-Потребителей) 
		/// </summary>
		public static List<CompanyWithUnp> GetAllExternal()
		{
			return CompaniesWithUnpStorage.GetAll();
		}

		/// <summary>
		/// Получение коллекции всех [Организаций] (как внутренних, так и внешних из АРМ Поставщиков-Потребителей) 
		/// </summary>
		public static List<Company> GetAllInternalAndExternal()
		{
			var companiesExternal = new List<CompanyWithUnp>(); // внешняя коллекция организаций
			try
			{
				companiesExternal = CompaniesWithUnpStorage.GetAll();
			}
			catch (Exception) { /* Игнорирование исключений, если внешняя БД недоступна */ }

			var companiesInternal = CompaniesStorage.GetAll(); // // внутренняя коллекция организаций
			foreach (var externalCompany in companiesExternal)
			{
				companiesInternal.Add(externalCompany);
			}
			return companiesInternal;
		}

		/// <summary>
		/// Получение коллекции [Организаций] (внутренних) по указанному названию
		/// </summary>
		public static List<Company> GetByNameInternal(string findName)
		{
			return CompaniesStorage.GetByName(findName);
		}

		/// <summary>
		/// Добавление новой [Организации] (внутренней)
		/// </summary>
		public static Company InsertInternal(string name)
		{
			return CompaniesStorage.Insert(name);
		}

		/// <summary>
		/// Добавление новой [Организации] (внутренней)
		/// </summary>
		public static Company InsertInternal(string name, SqlConnection connection, SqlTransaction transaction)
		{
			return CompaniesStorage.Insert(name, connection, transaction);
		}

		/// <summary>
		/// Обновление [Организации] (внутренней) с указанным [Id]
		/// </summary>
		public static Company UpdateInternal(long id, string name)
		{
			return CompaniesStorage.Update(id, name);
		}

		/// <summary>
		/// Удаление [Организации] (внутренней)
		/// </summary>
		public static void Delete(Company company)
		{
			CompaniesStorage.DeleteById(company.Id);
		}
	}
}
