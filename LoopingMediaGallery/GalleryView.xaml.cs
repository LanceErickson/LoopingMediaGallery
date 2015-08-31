using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LoopingMediaGallery
{
	/// <summary>
	/// Interaction logic for GalleryView.xaml
	/// </summary>
	public partial class GalleryView : Window
    {
        MainWindowViewModel viewModel;

        public GalleryView()
        {
            InitializeComponent();
        }

        public GalleryView(MainWindowViewModel viewModel) : this()
        {
            this.viewModel = viewModel;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            loopingMediaControl.Dispose();
        }

		internal void Present()
		{
			this.WindowStyle = WindowStyle.None;
			this.WindowState = WindowState.Maximized;
		}
	}
}
