using System;
using System.Linq;
using System.Data.Common;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Storages;
using ManagerByLetterOfAttorney.Entities.External;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для класса доверенность - [LetterOfAttorney]
	/// </summary>
	public static class LetterOfAttorneysService
	{
		/// <summary>
		/// Получение коллеции [Доверенностей]
		/// </summary>
		public static List<LetterOfAttorney> GetAll()
		{
			// Коллекция доверенностей с ID компании, флагом сотрудника/курьера и ID-курьера/таб.сотрудника
			var letterOfAttorneys = LetterOfAttorneysStorage.GetAll();
			
			const bool withDismissed = true;                              // выбрать уволенных сотрудников
			var employees = EmployeesService.GetAll(withDismissed);       // сотрудники
			var couriers = CouriersService.GetAll();                      // курьеры
			var companies = CompaniesService.GetAllInternal();            // организации (внутренние)
			var shipments = ShipmentsService.GetAll();                    // перевозимые ТМЦ

			foreach (var letterOfAttorney in letterOfAttorneys)
			{
				var isEmployeeCompany = letterOfAttorney.ServiceMappedIsEmployee; // Принадлежность к сотрудникам
				var courierIdOrEmplPersonel = letterOfAttorney.ServiceMappedCourierIdOrEmployeePersonnel; 
				var companyId = letterOfAttorney.ServiceMappedCompanyId;          // ID-организации

				// Получение перевозящего сотрудника/курьера
				if (isEmployeeCompany)
				{
					foreach (var employee in employees)
					{
						if (employee.PersonnelNumber != courierIdOrEmplPersonel)
						{
							continue;
						}
						letterOfAttorney.Courier = employee;
						break;
					}
				}
				else
				{
					foreach (var courier in couriers)
					{
						if (courier.Id != courierIdOrEmplPersonel)
						{
							continue;
						}
						letterOfAttorney.Courier = courier;
						break;
					}
				}

				// Получение организации
				foreach (var company in companies)
				{
					if (company.Id != companyId)
					{
						continue;
					}
					letterOfAttorney.Company = company;
					break;
				}

				// Получение коллекции перевозимых ТМЦ этой доверенности
				var currentShipments = new List<Shipment>();
				foreach (var shipment in shipments)
				{
					var letterOfAttorneyId = shipment.ServiceMappedLetterOfAttorneyId;
					if (letterOfAttorney.Id == letterOfAttorneyId)
					{
						currentShipments.Add(shipment);
					}
				}
				letterOfAttorney.Shipments = currentShipments;
			}
			return letterOfAttorneys;
		}

		/// <summary>
		/// Получение текущего максимального порядкового номера доверенностей текущего года
		/// </summary>
		public static long? GetMaxOrdinalNumber()
		{
			return LetterOfAttorneysStorage.GetMaxOrdinalNumber();
		}

		/// <summary>
		/// Получение следующего свободного порядкового номера доверенности
		/// </summary>
		public static long NextFreeOrdinalNumber()
		{
			const long firstOrdinalOfYear = 1L;
			var currentMaxOrdinal = LetterOfAttorneysStorage.GetMaxOrdinalNumber();
			return currentMaxOrdinal + 1L ?? firstOrdinalOfYear;
		}

		/// <summary>
		/// Получение [Доверенности] по порядковому номеру в текущем году
		/// </summary>
		public static LetterOfAttorney GetByOrdinalNumber(long ordinalNumber)
		{
			var letterOfAttorney = LetterOfAttorneysStorage.GetByOrdinalNumber(ordinalNumber);
			if (letterOfAttorney == null)
			{
				return null;
			}
			var isEmployeeCompany = letterOfAttorney.ServiceMappedIsEmployee;  // Принадлежность к сотрудникам
			var courierIdOrEployeePersonel = letterOfAttorney.ServiceMappedCourierIdOrEmployeePersonnel; 
			var companyId = letterOfAttorney.ServiceMappedCompanyId;           // ID-организации
			letterOfAttorney.Courier = isEmployeeCompany
				? EmployeesStorage.GetByPersonelNumber(courierIdOrEployeePersonel)
				: CouriersStorage.GetById(courierIdOrEployeePersonel);
			letterOfAttorney.Company = CompaniesStorage.GetById(companyId);
			letterOfAttorney.Shipments = ShipmentsService.GetByLetterOfAttorneyId(letterOfAttorney.Id);
			return letterOfAttorney;
		}

		/// <summary>
		/// Добавление новой [доверенности]
		/// </summary>
		public static LetterOfAttorney Insert(long ordinalNumber, DateTime validityDateStart, 
			DateTime validityDateEnd, Courier courierOrEmployee, string companyName, IEnumerable<Shipment> shipments)
		{
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			// Поиск уже существующей организации с указанным названием в базе данных
			var findedCompanies = CompaniesService.GetByNameInternal(companyName);
			var findCompany = findedCompanies != null && findedCompanies.Any() ? findedCompanies[0] : null;
			try
			{
				using (var connect = DbControl.GetConnection(server, db))
				{
					connect.TryConnectOpen();
					using (var transaction = connect.BeginTransaction("Insert LOA"))
					{
						// Если компания с таким названием не существует - добавление в бд
						var company = findCompany ?? CompaniesService.InsertInternal(companyName, connect,
							              transaction);

						// В зависимости от типа перевозящего груз (курьер или сотрудник), вызывается соответсвующий
						// метод добавление доверенности в БД
						LetterOfAttorney letterOfAttorney;
						if (courierOrEmployee.GetType() == typeof(Employee))
						{
							var employee = (Employee)courierOrEmployee;
							letterOfAttorney = LetterOfAttorneysStorage.Insert(ordinalNumber, validityDateStart,
								validityDateEnd, company, employee, connect, transaction);
						}
						else
						{
							var courier = courierOrEmployee;
							letterOfAttorney = LetterOfAttorneysStorage.Insert(ordinalNumber, validityDateStart,
								validityDateEnd, company, courier, connect, transaction);
						}

						// Добавление перевозимых по этой доверенности ТМЦ в базу данных
						foreach (var shipment in shipments)
						{
							var cargoName = shipment.Cargo.Name;
							var measureName = shipment.Cargo.Measure != null ? shipment.Cargo.Measure.Name : null;
							var newShipment = ShipmentsService.Insert(letterOfAttorney.Id, cargoName, measureName,
								shipment.Count, connect, transaction);
							letterOfAttorney.Shipments.Add(newShipment);
						}

						transaction.Commit();
						return letterOfAttorney;
					}
				}
			}
			catch (DbException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}

		/// <summary>
		/// Обновление [доверенности] с указанным [Id]
		/// </summary>
		public static LetterOfAttorney Update(long id, long ordinalNumber, DateTime validityDateStart,
			DateTime validityDateEnd, Courier courierOrEmployee, string companyName, IEnumerable<Shipment> shipments)
		{
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			// Поиск уже существующей организации с указанным названием в базе данных
			var findedCompanies = CompaniesService.GetByNameInternal(companyName);
			var findCompany = findedCompanies != null && findedCompanies.Any() ? findedCompanies[0] : null;
			try
			{
				using (var connect = DbControl.GetConnection(server, db))
				{
					connect.TryConnectOpen();
					using (var transaction = connect.BeginTransaction("Update LOA"))
					{
						// Если компания с таким названием не существует - добавление в бд
						var company = findCompany ?? CompaniesService.InsertInternal(companyName, connect, 
							              transaction);

						// В зависимости от типа перевозящего груз (курьер или сотрудник), вызывается соответсвующий
						// метод обновления доверенности в БД
						LetterOfAttorney letterOfAttorney;
						if (courierOrEmployee.GetType() == typeof(Employee))
						{
							var employee = (Employee)courierOrEmployee;
							letterOfAttorney = LetterOfAttorneysStorage.Update(id, ordinalNumber, validityDateStart,
								validityDateEnd, company, employee, connect, transaction);
						}
						else
						{
							var courier = courierOrEmployee;
							letterOfAttorney = LetterOfAttorneysStorage.Update(id, ordinalNumber,
								validityDateStart, validityDateEnd, company, courier, connect, transaction);
						}

						// Удаление всех перевозимых по этой доверенности ТМЦ в базе данных
						ShipmentsService.DeleteByLetterOfAttoneyId(letterOfAttorney.Id, connect, transaction);

						// Добавление новых перевозимых по этой доверенности ТМЦ в базу данных
						foreach (var shipment in shipments)
						{
							var cargoName = shipment.Cargo.Name;
							var measureName = shipment.Cargo.Measure != null ? shipment.Cargo.Measure.Name : null;
							var newShipment = ShipmentsService.Insert(letterOfAttorney.Id, cargoName, measureName,
								shipment.Count, connect, transaction);
							letterOfAttorney.Shipments.Add(newShipment);
						}

						transaction.Commit();
						return letterOfAttorney;
					}
				}
			}
			catch (DbException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}
	}
}
