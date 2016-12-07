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
	/// Interaction logic for PresentationView.xaml
	/// </summary>
	public partial class PresentationView : Window
	{
		public PresentationView(MainWindowViewModel viewModel)
		{
			this.DataContext = viewModel;

			InitializeComponent();
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
			=> WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}
	}
}
