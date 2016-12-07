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

	}
}
