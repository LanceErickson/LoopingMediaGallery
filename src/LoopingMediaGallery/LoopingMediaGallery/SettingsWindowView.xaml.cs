using System;
using System.Windows;

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
