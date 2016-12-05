using System.Windows;

namespace LoopingMediaGallery
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow(MainWindowViewModel viewModel)
		{
			//TODO: DI CONTAINER
			this.DataContext = viewModel;

			InitializeComponent();
		}
	}
}
