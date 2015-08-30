using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LoopingMediaGallery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

			var galleryView = new GalleryView();
			galleryView.Show();

			((MainWindowViewModel)DataContext).AddGallery(galleryView);

			((MainWindowViewModel)DataContext).PreviewPane = this.loopPreviewPane;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            ((MainWindowViewModel)DataContext).Dispose();
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if ((string)((Button)sender).Content == "Start")
                ((Button)sender).Content = "Stop";
            else
                ((Button)sender).Content = "Start";
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
		}
	}
}
