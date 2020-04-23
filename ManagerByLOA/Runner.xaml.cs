using System.Windows;
using System.Threading;

using ManagerByLetterOfAttorney.Util;

namespace ManagerByLetterOfAttorney
{
	/// <summary>
	/// Класс запуска приложения
	/// </summary>
	/// <inheritdoc />
	public partial class Runner
	{
		/// <summary>
		/// Маркер, сигнализирующий об захвате нового экземляра Mutex этого приложения
		/// </summary>
		private static bool _isCreatedNew;

		/// <summary>
		/// Примитив синхронизации OC, используемый для проверки запуска копий этого приложения в OC.
		/// NOTE: работа с Mutex базируется на этом решении: https://stackoverflow.com/a/5376828
		/// </summary>
		private static Mutex _mutex = Common.GetApplicationMutex(out _isCreatedNew);

		private Runner()
		{
			// обработка неотловленных исключений
			Dispatcher.UnhandledException += Common.RootExceptionHandler;
		}

		/// <summary>
		/// Служебная инициализация приложения до запуска окон.
		/// </summary>
		/// <inheritdoc />
		protected override void OnStartup(StartupEventArgs eventArgs)
		{
			// Если этот Mutex не первый созданный в текущий момент в операционной системе, сообщение и завершение
			if (!_isCreatedNew) 
			{
				_mutex = null;
				Common.ShowMessageForMutexAlreadyRunning();
				Current.Shutdown();
				return;
			}

			// Установка русско-язычной локали и десятичного разделителя точки
			Common.SetRussianLocaleAndDecimalSeparatorDot();
			
			base.OnStartup(eventArgs);

			// Переключение языка ввода на русский
			Common.SetKeyboardInputFromCurrentLocale();
		}

		/// <summary>
		/// Освобождение Mutex при завершении приложения
		/// </summary>
		/// <inheritdoc />
		protected override void OnExit(ExitEventArgs eventArgs)
		{
			if (_mutex != null) // Освобождение Mutex, если он был захвачен
			{
				_mutex.ReleaseMutex();
			}
			base.OnExit(eventArgs);
		}
	}
}
