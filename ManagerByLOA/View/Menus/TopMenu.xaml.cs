using System.Windows;
using System.Windows.Controls;

using ManagerByLetterOfAttorney.Util;
using ManagerByLetterOfAttorney.View.Windows;

namespace ManagerByLetterOfAttorney.View.Menus
{
	/// <summary>
	/// Верхнее меню главного окна приложения.
	/// </summary>
	/// <inheritdoc cref="UserControl" />
	public partial class TopMenu
	{
		public TopMenu()
		{
			InitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Визуальная инициализация меню (цвета и размеры шрифтов контролов)
		/// </summary>
		private void VisualInitializeComponent()
		{
			WindowMenu.FontSize = Constants.FontSize;
			WindowMenu.Background = Constants.BackColor3_SanJuan;
			foreach (var menuItem in WindowMenu.Items)
			{
				var menuItemControl = menuItem as MenuItem;
				if (menuItemControl == null)
				{
					continue;
				}
				menuItemControl.Background = Constants.BackColor4_BlueBayoux;
				menuItemControl.Foreground = Constants.ForeColor2_PapayaWhip;
			}
		}

		/// <summary>
		/// Нажатие пункта меню [Пользовательские настройки] - инициализация и отображение модального окна настроек
		/// </summary>
		private void ConfigMenuItem_OnClick(object senderIsMenuItem, RoutedEventArgs eventArgs)
		{
			var userConfigWindow = new UserConfigWindow
			{
				Owner = Window.GetWindow(this)
			};
			userConfigWindow.ShowDialog();
		}

		/// <summary>
		/// Нажатие пункта меню [Выход]
		/// </summary>
		private void ExitMenuItem_OnClick(object senderIsMenuItem, RoutedEventArgs eventArgs)
		{
			Application.Current.Shutdown();
		}
	}
}
