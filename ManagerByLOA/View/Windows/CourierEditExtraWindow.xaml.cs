using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;

using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.View.Pages;
using ManagerByLetterOfAttorney.View.Pages.Edit;
using ManagerByLetterOfAttorney.Entities.Internal;

namespace ManagerByLetterOfAttorney.View.Windows
{
	/// <summary>
	/// Дополнительное окно добавления/редактирования паспорта курьера.
	/// </summary>
	/// <inheritdoc cref="System.Windows.Window" />
	public partial class CourierEditExtraWindow
	{
		/// <summary>
		/// Результирующий курьер, в случае если курьер успешно добавлен/изменён
		/// </summary>
		private Courier _resultCourier;

		/// <inheritdoc />
		/// <summary>
		/// Конструктор добавления курьера
		/// </summary>
		public CourierEditExtraWindow()
		{
			InitializeComponent();
			// Заполнение фрейма страницей редактирования курьера, с указанием делегата Action (указатель на функцию), 
			// вызываемого в результате успешного сохранения курьера или отмены ввода (кнопка отмены) пользователем.
			WindowFrame.Content = new CourierEdit(CloseWindow);
			VisualInitializeComponent();
		}

		/// <inheritdoc />
		/// <summary>
		/// Конструктор редактирования
		/// </summary>
		public CourierEditExtraWindow(Courier editedCourier)
		{
			InitializeComponent();
			// Заполнение фрейма страницей редактирования курьера, с указанием делегата Action (указатель на функцию), 
			// вызываемого в результате успешного сохранения курьера или отмены ввода (кнопка отмены) пользователем.
			WindowFrame.Content = new CourierEdit(CloseWindow, editedCourier);
			VisualInitializeComponent();
		}

		/// <summary>
		/// Делегат Action (указатель на функцию), вызываемый из страницы загруженной страницы редактирования в 
		/// результате успешного сохранения курьера или отмены ввода (кнопка отмены) пользователем.
		/// </summary>
		/// <param name="courier">Сохранённый в базе данных результат добавления/редактирования</param>
		private void CloseWindow(Courier courier)
		{
			if (courier != null)
			{
				_resultCourier = courier;
				DialogResult = true;
			}
			Close();
		}

		/// <summary>
		/// Визуальная инициализация окна (цвета и размеры шрифтов контролов)
		/// </summary>
		private void VisualInitializeComponent()
		{
			FontSize = Constants.FontSize;
			Background = Constants.BackColor2_Botticelli;
			Foreground = Constants.ForeColor2_PapayaWhip;
			
			// Панель хоткеев
			HotkeysDockPanel.Background = Constants.BackColor4_BlueBayoux;
			var helpLabels = HotkeysDockPanel.Children.OfType<Label>();
			foreach (var helpLabel in helpLabels)
			{
				helpLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}
		}

		/// <summary>
		/// Получение сохранённого в базе данных курьера, полученого в результате сохранения или редактирвоания.
		/// </summary>
		public Courier GetSavedCourier()
		{
			return _resultCourier;
		}

		/// <summary>
		/// Событие загрузки фрейма - указание хоткеев страницы на панели хоткеев
		/// </summary>
		private void Frame_OnLoadCompleted(object senderIsFrame, NavigationEventArgs eventArgs)
		{
			var frame = senderIsFrame as Frame;
			if (frame == null)
			{
				return;
			}
			var currentPage = frame.Content as IPageable;
			if (currentPage == null)
			{
				return;
			}
			HotkeysTextBlock.Text = currentPage.PageHotkeys();
		}
	}
}
