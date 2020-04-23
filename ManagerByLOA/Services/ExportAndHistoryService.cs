using System;
using System.Windows;
using System.Data.Common;
using System.Collections.Generic;

using ManagerByLetterOfAttorney.Db;
using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.Storages;
using ManagerByLetterOfAttorney.View.Util;
using ManagerByLetterOfAttorney.View.Windows;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.Services
{
	/// <summary>
	/// Обработчик сервисного слоя экпорта доверенностей в архив, получение архивных записей и текущего года выдачи
	/// </summary>
	public static class ExportAndHistoryService
	{
		/// <summary>
		/// Получение текущего года заполнения доверенностей из БД
		/// </summary>
		public static int GetCurrentYearFromDb()
		{
			return CurrentYearAndHistoryStorage.CurrentYearFromDb();
		}

		/// <summary>
		/// Получение коллекции архива доверенностей (перевозимых сущностей доверенностей) предыдущих лет
		/// </summary>
		public static List<HistoryItem> HistoryPreviousYears()
		{
			return CurrentYearAndHistoryStorage.GetHistories();
		}

		/// <summary>
		/// Проверка необходимости экспортирования в архив, запрос подтверждения и экспорт в историю
		/// </summary>
		public static void VerifyAndExport() 
		{
			try
			{
				// Проверка года выдачи в БД и текущего года системного времени на ПК
				var currentYearInDb = CurrentYearAndHistoryStorage.CurrentYearFromDb();
				if (currentYearInDb == DateTime.Today.Year)
				{
					return;
				}

				// Год выдачи в БД и год системного времени на ПК не совпадают - запрос подтверждения у пользователя
				if (!IsExportConfirm(currentYearInDb, DateTime.Today.Year))
				{
					Application.Current.Shutdown();
					return;
				}
				
				// Экспорт, информирование о необходимости повторного запуска и завершение приложения
				Export(currentYearInDb);

				const MessageBoxButton buttons = MessageBoxButton.OK;
				const MessageBoxImage messageType = MessageBoxImage.Exclamation;
				const string beliefRestartApp = "Для корректной последующей работы запустите приложение заново";
				MessageBox.Show(beliefRestartApp, PageLiterals.HeaderInformationOrWarning, buttons, messageType);

				Application.Current.Shutdown();
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
			}
		}

		/// <summary>
		/// Запрос подтверждения экспорта у пользователя в отдельном модальном окне
		/// </summary>
		private static bool IsExportConfirm(int previewYear, int nextYear)
		{
			var confirmExportWindow = new ConfirmExportWindow(previewYear, nextYear)
			{
				Owner = Application.Current.MainWindow
			};
			confirmExportWindow.ShowDialog();
			return confirmExportWindow.DialogResult.HasValue && confirmExportWindow.DialogResult.Value;
		}

		/// <summary>
		/// Экспорт доверенностей текущего года в архив
		/// </summary>
		/// <param name="archivalYear">Год указываемый архивным</param>
		private static void Export(int archivalYear)
		{
			var currentYear = DateTime.Today.Year;
			var letterOfAttorneys = LetterOfAttorneysService.GetAll();
			var server = Properties.Settings.Default.ServerLetterOfAttorney;
			var db = Properties.Settings.Default.DbLetterOfAttorney;

			using (var connection = DbControl.GetConnection(server, db))
			{
				connection.TryConnectOpen();
				try
				{
					using (var transaction = connection.BeginTransaction("Export"))
					{
						// Вставка доверенностей в таблицу истории
						CurrentYearAndHistoryStorage.InsertHistoryLetterOfAttorneys(letterOfAttorneys, archivalYear,
							connection, transaction);

						// Удаление всех перевозимых ТМЦ
						ShipmentsStorage.DeleteAll(connection, transaction);

						// Удаление всех доверенностей
						LetterOfAttorneysStorage.DeleteAll(connection, transaction);

						// Обновление года выдачи доверенностей
						CurrentYearAndHistoryStorage.UpdateCurrentYearInDb(connection, transaction, currentYear);

						// Подтверждение транзакции
						transaction.Commit();
					}
				} 
				catch (DbException ex)
				{
					throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
				}
			}
		}
	}
}
