using System;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.Storages;
using ManagerByLetterOfAttorney.Entities.Enums;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для класса курьер [Courier]
	/// </summary>
	public static class CouriersService
	{
		/// <summary>
		/// Получение коллекции [Курьеров]
		/// </summary>
		public static List<Courier> GetAll()
		{
			return CouriersStorage.GetAll();
		}

		/// <summary>
		/// Получение коллекции [Курьеров] с указанным полем числа ссылок на запись из других таблиц
		/// </summary>
		public static List<Courier> GetAllWithCountUsing()
		{
			return CouriersStorage.GetAllWithCountUsing();
		}

		/// <summary>
		/// Добавление нового [Курьера]
		/// </summary>
		public static Courier Insert(string lastName, string firstName, string middleName, Sex sex,
			string passportSeriesAndNumber, string passportIssuedByOrganization, DateTime passportIssueDate, 
			string profession)
		{
			return CouriersStorage.Insert(lastName, firstName, middleName, sex, passportSeriesAndNumber,
				passportIssuedByOrganization, passportIssueDate, profession);
		}

		/// <summary>
		/// Обновление [Курьера] с указанным [Id]
		/// </summary>
		public static Courier Update(long id, string lastName, string firstName, string middleName, Sex sex,
			string passportSeriesAndNumber, string passportIssuedByOrganization,
			DateTime passportIssueDate, string profession)
		{
			return CouriersStorage.Update(id, lastName, firstName, middleName, sex, passportSeriesAndNumber,
				passportIssuedByOrganization, passportIssueDate, profession);
		}

		/// <summary>
		/// Удаление [Курьера] 
		/// </summary>
		public static void Delete(Courier courier)
		{
			const string errorMessageAbsentOrNull = "У удаляемого курьера отсутствует ID, или сущность не курьер";
			if (courier.Id == null)
			{
				var message = string.Format(Constants.LogicErrorPattern, errorMessageAbsentOrNull);
				throw new ApplicationException(message);
			}
			CouriersStorage.DeleteById((long)courier.Id);
		}
	}
}
