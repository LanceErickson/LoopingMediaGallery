using LoopingMediaGallery.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace LoopingMediaGallery.Objects
{
	public class MediaProvider : IMediaProvider
	{
		private ISettingsProvider _settingsProvider;
		private Timer _fileRefreshTimer;

		public MediaProvider(ISettingsProvider settingsProvider)
		{
			if (settingsProvider == null) throw new ArgumentNullException(nameof(settingsProvider));

			_settingsProvider = settingsProvider;
			_settingsProvider.SettingsChanged += (s, o) =>
			{
				var settingName = (o as System.Configuration.SettingChangingEventArgs).SettingName;
				switch (settingName)
				{
					case nameof(_settingsProvider.FolderPath):
						_fileRefreshTimer.Dispose();
						App.Current.Dispatcher.BeginInvoke(new Action(() => ScanFolderPath(null)));
						InitializeTimer();
						break;
					case nameof(_settingsProvider.RefreshRate):
						_fileRefreshTimer.Dispose();
						InitializeTimer();
						break;
				}
			};

			MediaObjectCollection = new List<IMediaObject>();

			InitializeTimer();
		}

		private void InitializeTimer()
		{
			_fileRefreshTimer = new Timer(ScanFolderPath, new AutoResetEvent(false), (int)TimeSpan.FromMinutes(_settingsProvider.RefreshRate).TotalMilliseconds, (int)TimeSpan.FromMinutes(_settingsProvider.RefreshRate).TotalMilliseconds);
		}

		private void ScanFolderPath(object state)
		{
			var mediaCollection = new List<IMediaObject>();

			if (!(string.IsNullOrEmpty(_settingsProvider.FolderPath) || string.IsNullOrWhiteSpace(_settingsProvider.FolderPath) || !Directory.Exists(_settingsProvider.FolderPath)))
			{
				string[] files = Directory.GetFiles(_settingsProvider.FolderPath);
				
				foreach (var file in files)
				{
					var ext = Path.GetExtension(file);

					if (_settingsProvider.ImageFormats.Contains(ext.ToLower()))
						mediaCollection.Add(new LocalImageObject(_settingsProvider, file));

					if (_settingsProvider.VideoFormats.Contains(ext.ToLower()))
						mediaCollection.Add(new LocalVideoObject(file));
				}
			}

			if (MediaObjectCollection.Count == 0 && mediaCollection.Count > 0)
				MediaCollectionPopulated?.Invoke(this, new EventArgs());

			MediaObjectCollection = mediaCollection;			
		}

		public IList<IMediaObject> MediaObjectCollection { get; internal set; }

		public void ForceUpdate() => ScanFolderPath(null);

		public event EventHandler MediaCollectionPopulated;
	}
}
