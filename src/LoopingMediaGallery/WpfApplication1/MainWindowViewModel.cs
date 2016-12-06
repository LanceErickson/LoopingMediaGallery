using LoopingMediaGallery.Interfaces;
using System;
using System.ComponentModel;

namespace LoopingMediaGallery
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private ISettingsProvider _settingsProvider;
		private IServeMedia _mediaServer;
		private IMediaProvider _mediaProvider;

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

		public MainWindowViewModel(ISettingsProvider settingsProvider, IServeMedia mediaServer, IMediaProvider mediaProvider)
		{
			if (settingsProvider == null) throw new ArgumentNullException(nameof(settingsProvider));
			if (mediaServer == null) throw new ArgumentNullException(nameof(mediaServer));
			if (mediaProvider == null) throw new ArgumentNullException(nameof(mediaProvider));

			_settingsProvider = settingsProvider;
			_mediaServer = mediaServer;
			_mediaProvider = mediaProvider;

			_mediaProvider.ForceUpdate();

			_settingsProvider.FileFolderPath = "C:\\Users\\Lance\\Dropbox\\Apps\\Looping Media Gallery";
			_settingsProvider.Duration = 10;
			_settingsProvider.FileRefreshRate = 1;
		}

		public void MediaHasEnded() => NextHandler();

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

		}

		public void SettingsHandler()
		{

		}
	}
}
