using LoopingMediaGallery.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace LoopingMediaGallery
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private readonly ISettingsProvider _settingsProvider;
		private readonly IServeMedia _mediaServer;
		private readonly IMediaProvider _mediaProvider;
		private readonly ISaveSettings _settingsSaver;
		private readonly IIntervalTimer _timer;

		public event PropertyChangedEventHandler PropertyChanged;
		public void SendPropertyChanged(string propertyName)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public IMediaObject CurrentMedia => _mediaServer?.CurrentMedia;

		public IList<IMediaObject> MediaCollection => new List<IMediaObject>(_mediaProvider.MediaObjectCollection);

		private bool _play = false;
		public bool Play
		{
			get { return _play; }
			set
			{
				if (CurrentMedia == null)
					return;

				if (_play == value)
					return;

				if (value)
				{
					SendPropertyChanged(nameof(CurrentMedia));
					_timer.Initialize(CurrentMedia.Duration, () => MediaHasEnded());
					_timer.Start();
				}
				else
				{
					_timer.Stop();
				}

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

		public bool UseFade => _settingsProvider.UseFade;

		public bool ShowPreview => _settingsProvider.ShowPreview;

		private PresentationView _presentationView;

		public MainWindowViewModel(ISettingsProvider settingsProvider, IServeMedia mediaServer, IMediaProvider mediaProvider, ISaveSettings settingsSaver, IIntervalTimer timer)
		{
			if (settingsProvider == null) throw new ArgumentNullException(nameof(settingsProvider));
			if (mediaServer == null) throw new ArgumentNullException(nameof(mediaServer));
			if (mediaProvider == null) throw new ArgumentNullException(nameof(mediaProvider));
			if (settingsSaver == null) throw new ArgumentNullException(nameof(settingsSaver));
			if (timer == null) throw new ArgumentNullException(nameof(timer));

			_settingsProvider = settingsProvider;
			_mediaServer = mediaServer;
			_mediaProvider = mediaProvider;
			_settingsSaver = settingsSaver;
			_timer = timer;

			_settingsProvider.SettingsChanged += (s, o) =>
			{
				var settingName = (o as System.Configuration.SettingChangingEventArgs).SettingName;
				switch (settingName)
				{
					case nameof(_settingsProvider.UseFade):
						App.Current.Dispatcher.BeginInvoke(new Action(() => SendPropertyChanged(nameof(UseFade))));
						break;
				}
			};

			_mediaProvider.MediaCollectionPopulated += (s, o) =>
				App.Current.Dispatcher.BeginInvoke(new Action(() => SendPropertyChanged(nameof(CurrentMedia))));

			_mediaProvider.MediaCollectionChanged += (s, o) =>
				App.Current.Dispatcher.BeginInvoke(new Action(() => SendPropertyChanged(nameof(MediaCollection))));

			_mediaProvider.ForceUpdate();
			SendPropertyChanged(nameof(_settingsProvider.ShowPreview));
		}

		public void MediaHasEnded()
		{
			SendPropertyChanged(nameof(_settingsProvider.ShowPreview));
			NextHandler();
		}

		public void ResetHandler()
		{
			_mediaServer.Reset();
			SendPropertyChanged(nameof(CurrentMedia));
			_timer.Initialize(CurrentMedia.Duration, () => MediaHasEnded());
			if (Play)
				_timer.Start();
		}

		public void NextHandler()
		{
			_mediaServer.NextMedia();
			SendPropertyChanged(nameof(CurrentMedia));
			_timer.Initialize(CurrentMedia.Duration, () => MediaHasEnded());
			if (Play)
				_timer.Start();
		}

		public void PreviousHandler()
		{
			_mediaServer.PreviousMedia();
			SendPropertyChanged(nameof(CurrentMedia));
			_timer.Initialize(CurrentMedia.Duration, () => MediaHasEnded());
			if(Play)
				_timer.Start();
		}

		public void BlankHandler() => Blank = !Blank;

		public void SelectMedia(IMediaObject media)
		{
			int index = MediaCollection.IndexOf(media);
			_mediaServer.ServeSpecific(index);
			SendPropertyChanged(nameof(CurrentMedia));
			_timer.Initialize(CurrentMedia.Duration, () => MediaHasEnded());
			if (Play)
				_timer.Start();
		}

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
