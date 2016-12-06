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
		private IUnityContainer _container;

		protected override void OnStartup(StartupEventArgs e)
		{
			_container = new UnityContainer();
			_container.RegisterType<IServeMedia, MediaServer>();
			_container.RegisterType<IMediaProvider, MediaProvider>(new ContainerControlledLifetimeManager());
			_container.RegisterType<ISettingsProvider, Objects.SettingsProvider>();
			_container.RegisterType<ISaveSettings, SettingsSaver>();

			MainWindow mainWindow = _container.Resolve<MainWindow>();
			mainWindow.Show();
		}
	}
}
