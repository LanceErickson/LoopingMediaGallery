using System.Windows;
using System.Windows.Input;

namespace LoopingMediaGallery
{
	/// <summary>
	/// Interaction logic for PresentationView.xaml
	/// </summary>
	public partial class PresentationView : Window
	{
		public PresentationView(MainWindowViewModel viewModel)
		{
			this.DataContext = viewModel;

			InitializeComponent();
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
			=> WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}
	}
}
