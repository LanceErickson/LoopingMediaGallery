using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

			//this.loopPreviewPane.IsMuted = true;

			galleryView = new GalleryView();
			galleryView.Show();

			((MainWindowViewModel)DataContext).AddGallery(galleryView);

			//((MainWindowViewModel)DataContext).PreviewPane = this.loopPreviewPane;
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

			System.Windows.Forms.Screen screen;
			if (screens.Count() > 1)
				screen = screens[1];
			else
				screen = screens[0];
						
				System.Drawing.Rectangle r2 = screen.WorkingArea;
				galleryView.Top = r2.Top;
				galleryView.Left = r2.Left;
				galleryView.Show();

				galleryView.Present();
			
		}

		private void btnMute_Click(object sender, RoutedEventArgs e)
		{
			if ((string)((Button)sender).Content == "Mute")
				((Button)sender).Content = "Unmute";
			else
				((Button)sender).Content = "Mute";
		}
	}
}
