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

		public void MediaHasEnded()
		{
			_mediaServer.NextMedia();
			SendPropertyChanged(nameof(CurrentMedia));
		}
	}
}
