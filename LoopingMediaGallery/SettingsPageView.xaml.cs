using System.Windows;
using System.Windows.Forms;

namespace LoopingMediaGallery
{
	/// <summary>
	/// Interaction logic for SettingsPageView.xaml
	/// </summary>
	public partial class SettingsPageView : Window
	{
		public SettingsPageView()
		{
			InitializeComponent();
		}

		public SettingsPageView(MainWindowViewModel viewModel) : this()
		{
			this.DataContext = viewModel;
		}

		private void btnOpen_Click(object sender, RoutedEventArgs e)
		{
			using(var folderDialog = new FolderBrowserDialog()) 
			{
				DialogResult result = folderDialog.ShowDialog();

				if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
				{
					((MainWindowViewModel)DataContext).FolderPath = folderDialog.SelectedPath;
				}
			}
			this.Focus();
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
