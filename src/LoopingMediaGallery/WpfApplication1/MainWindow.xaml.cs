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

			var presentationView = new PresentationView(viewModel);

			viewModel.AddPresentationView(presentationView);

			presentationView.Show();

			InitializeComponent();
		}
	}
}
