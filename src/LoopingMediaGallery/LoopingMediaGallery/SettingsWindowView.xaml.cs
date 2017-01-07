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
	/// Interaction logic for SettingsWindowView.xaml
	/// </summary>
	public partial class SettingsWindowView : Window
	{
		public SettingsWindowView(SettingsWindowViewModel viewModel)
		{
			if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

			viewModel.SendNeedsClose += (s, o) =>
			{
				this.Close();
			};

			this.DataContext = viewModel;

			InitializeComponent();
		}
	}
}
