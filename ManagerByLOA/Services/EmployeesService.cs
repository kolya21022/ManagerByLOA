using System.Collections.Generic;

using ManagerByLetterOfAttorney.Storages;
using ManagerByLetterOfAttorney.Entities.External;

namespace ManagerByLetterOfAttorney.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для класса сотрудник [Employee].
	/// Методы редактирования данных отсутсвуют, так как это сущность из внешней системы (АРМ отдела кадров).
	/// </summary>
	public static class EmployeesService
	{
		/// <summary>
		/// Получение коллекции [Курьеров] (внешняя) с указанием выбирать ли уволенных сотрудников
		/// </summary>
		public static List<Employee> GetAll(bool withDismissed)
		{
			return EmployeesStorage.GetAll(withDismissed);
		}

		/// <summary>
		/// Загрузка порядкового номера рабочего месяца базы данных отдела кадров
		/// </summary>
		public static int WorkMount()
		{
			return EmployeesStorage.WorkMount();
		}
	}
}
