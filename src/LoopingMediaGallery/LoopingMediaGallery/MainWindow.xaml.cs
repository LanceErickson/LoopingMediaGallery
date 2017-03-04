using System.ComponentModel;
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
			this.DataContext = viewModel;

			var presentationView = new PresentationView(viewModel);

			viewModel.AddPresentationView(presentationView);

			presentationView.Show();

			InitializeComponent();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			((MainWindowViewModel)DataContext).ClosePresentationView();
		}
	}
}
