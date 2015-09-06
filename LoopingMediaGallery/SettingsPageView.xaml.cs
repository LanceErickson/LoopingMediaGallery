using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
			var dlg = new CommonOpenFileDialog();
			dlg.Title = "Select a path to monitor";
			dlg.IsFolderPicker = true;

			dlg.AddToMostRecentlyUsedList = false;
			dlg.AllowNonFileSystemItems = false;
			dlg.EnsureFileExists = true;
			dlg.EnsurePathExists = true;
			dlg.EnsureReadOnly = false;
			dlg.EnsureValidNames = true;
			dlg.Multiselect = false;
			dlg.ShowPlacesList = true;

			if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
			{
				var folder = dlg.FileName;

				((MainWindowViewModel)DataContext).FolderPath = folder;
			}
			this.Focus();
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
