using System.Windows.Input;

namespace ManagerByLetterOfAttorney.View.Pages
{
	/// <summary>
	/// Интерфейс страниц главного окна приложения.
	/// </summary>
	public interface IPageable
	{
		/// <summary>
		/// Дополнительная инициализация компонентов страницы.
		/// </summary>
		void AdditionalInitializeComponent();

		/// <summary>
		/// Дополнительная визуальная инициализация страницы (в основном цвета и шрифты) 
		/// </summary>
		void VisualInitializeComponent();

		/// <summary>
		/// Получение строки хоткеев страницы, отображаемые на соответсвующей панели окна
		/// </summary>
		string PageHotkeys();

		/// <summary>
		/// Обработчик KeyDown страницы, на который происходит перенаправление из обработчика KeyDown главного окна.
		/// Этот же обработчик, желательно, привязывать к событию KeyDown самой страницы.
		/// </summary>
		void Page_OnKeyDown(object senderIsPageOrWindow, KeyEventArgs eventArgs);
	}
}
