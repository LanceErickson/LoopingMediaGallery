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

		GalleryView galleryView;
        public MainWindow()
        {
            InitializeComponent();

			galleryView = new GalleryView();
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

		private void btnSettings_Click(object sender, RoutedEventArgs e)
		{
			var settingsView = new SettingsPageView(((MainWindowViewModel)DataContext));

			settingsView.ShowDialog();
		}

		private void btnPresent_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;

			if (screens.Count() > 1)
			{
				var secondScreen = screens[1];

				System.Drawing.Rectangle r2 = secondScreen.WorkingArea;
				galleryView.Top = r2.Top;
				galleryView.Left = r2.Left;
				galleryView.Show();

				galleryView.Present();
			}
		}
	}
}
