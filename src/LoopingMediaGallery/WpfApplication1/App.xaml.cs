using LoopingMediaGallery.Interfaces;
using LoopingMediaGallery.Objects;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LoopingMediaGallery
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			IUnityContainer container = new UnityContainer();
			container.RegisterType<IServeMedia, MediaServer>();
			container.RegisterType<IMediaProvider, MediaProvider>(new ContainerControlledLifetimeManager());
			container.RegisterType<ISettingsProvider, Objects.SettingsProvider>();

			MainWindow mainWindow = container.Resolve<MainWindow>();
			mainWindow.Show();
		}
	}
}
