using LoopingMediaGallery.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

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
						App.Current.Dispatcher.BeginInvoke(new Action(() => ScanFolderPath()));
						InitializeTimer();
						break;
					case nameof(_settingsProvider.RefreshRate):
						_fileRefreshTimer.Dispose();
						InitializeTimer();
						break;
				}
			};

			MediaObjectCollection = new HashSet<IMediaObject>();

			ScanFolderPath();

			InitializeTimer();
		}

		private void InitializeTimer()
		{
			if (_fileRefreshTimer != null)
				_fileRefreshTimer.Dispose();
			_fileRefreshTimer = new Timer(TimeSpan.FromMinutes(_settingsProvider.RefreshRate).TotalMilliseconds);
			_fileRefreshTimer.Elapsed += (s, o) => ScanFolderPath();
			_fileRefreshTimer.Enabled = true;
		}

		private void ScanFolderPath()
		{
			var mediaCollection = new HashSet<IMediaObject>();

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

				if (MediaObjectCollection.Count == 0 && mediaCollection.Count > 0)
					MediaCollectionPopulated?.Invoke(this, new EventArgs());

				if (!MediaObjectCollection.SetEquals(mediaCollection))
					MediaCollectionChanged?.Invoke(this, new EventArgs());

				MediaObjectCollection = mediaCollection;
			}
		}

		public ISet<IMediaObject> MediaObjectCollection { get; internal set; }

		public void ForceUpdate() => ScanFolderPath();

		public event EventHandler MediaCollectionPopulated;

		public event EventHandler MediaCollectionChanged;
	}
}
