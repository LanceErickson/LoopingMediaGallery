using System.Windows;

namespace LoopingMediaGallery
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			//TODO: DI CONTAINER
			this.DataContext = new MainWindowViewModel(null, null, null);

			InitializeComponent();
		}
	}
}
