using LoopingMediaGallery.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LoopingMediaGallery
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private readonly ISettingsProvider _settingsProvider;
		private readonly IServeMedia _mediaServer;
		private readonly IMediaProvider _mediaProvider;
		private readonly ISaveSettings _settingsSaver;

		public event PropertyChangedEventHandler PropertyChanged;
		public void SendPropertyChanged(string propertyName)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public IMediaObject CurrentMedia => _mediaServer?.CurrentMedia;

		private bool _play = false;
		public bool Play
		{
			get { return _play; }
			set
			{
				if (_play == value)
					return;
				_play = value;
				SendPropertyChanged(nameof(Play));
			}
		}

		private bool _mute = true;
		public bool Mute
		{
			get { return _mute; }
			set
			{
				if (_mute == value)
					return;
				_mute = value;
				SendPropertyChanged(nameof(Mute));
			}
		}

		private bool _blank = false;
		public bool Blank
		{
			get { return _blank; }
			set
			{
				if (_blank == value)
					return;
				_blank = value;
				SendPropertyChanged(nameof(Blank));
			}
		}

		private bool _useFade = false;
		public bool UseFade
		{
			get { return _useFade; }
			set
			{
				if (_useFade == value)
					return;
				_useFade = value;
				SendPropertyChanged(nameof(UseFade));
			}
		}

		private bool _showPreview = false;
		public bool ShowPreview
		{
			get { return _showPreview; }
			set
			{
				if (_showPreview == value)
					return;
				_showPreview = value;
				SendPropertyChanged(nameof(ShowPreview));
			}
		}

		private PresentationView _presentationView;

		public MainWindowViewModel(ISettingsProvider settingsProvider, IServeMedia mediaServer, IMediaProvider mediaProvider, ISaveSettings settingsSaver)
		{
			if (settingsProvider == null) throw new ArgumentNullException(nameof(settingsProvider));
			if (mediaServer == null) throw new ArgumentNullException(nameof(mediaServer));
			if (mediaProvider == null) throw new ArgumentNullException(nameof(mediaProvider));
			if (settingsSaver == null) throw new ArgumentNullException(nameof(settingsSaver));

			_settingsProvider = settingsProvider;
			_mediaServer = mediaServer;
			_mediaProvider = mediaProvider;
			_settingsSaver = settingsSaver;

			_settingsProvider.SettingsChanged += (s, o) =>
			{
				if ((o as System.Configuration.SettingChangingEventArgs).SettingName == nameof(_settingsProvider.UseFade))
					UseFade = (bool)(o as System.Configuration.SettingChangingEventArgs).NewValue;
			};

			_mediaProvider.ForceUpdate();
			UseFade = _settingsProvider.UseFade;
			ShowPreview = _settingsProvider.ShowPreview;
		}

		public void MediaHasEnded()
		{
			ShowPreview = _settingsProvider.ShowPreview;
			NextHandler();
		}

		public void ResetHandler()
		{
			_mediaServer.Reset();
			SendPropertyChanged(nameof(CurrentMedia));
		}

		public void NextHandler()
		{
			_mediaServer.NextMedia();
			SendPropertyChanged(nameof(CurrentMedia));
		}

		public void PreviousHandler()
		{
			_mediaServer.PreviousMedia();
			SendPropertyChanged(nameof(CurrentMedia));
		}

		public void BlankHandler() => Blank = !Blank;

		public void PresentHandler()
		{
			if (_presentationView == null) return;

			var screens = new List<System.Windows.Forms.Screen>(System.Windows.Forms.Screen.AllScreens);

			System.Windows.Forms.Screen screen;
			if (screens.Count > 1)
				screen = screens[1];
			else
				screen = screens[0];

			System.Drawing.Rectangle r2 = screen.WorkingArea;
			_presentationView.Top = r2.Top;
			_presentationView.Left = r2.Left;

			_presentationView.WindowStyle = System.Windows.WindowStyle.None;
			_presentationView.WindowState = System.Windows.WindowState.Maximized;
		}

		internal void AddPresentationView(PresentationView presentationView)
		{
			_presentationView = presentationView;
		}

		internal void ClosePresentationView()
		{
			if (_presentationView != null)
				_presentationView.Close();
		}

		public void SettingsHandler()
		{
			var settingsWindow = new SettingsWindowView(new SettingsWindowViewModel(_settingsProvider, _settingsSaver));
			settingsWindow.ShowDialog();
		}
	}
}
