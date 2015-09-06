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
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
            
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

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}
	}
}
