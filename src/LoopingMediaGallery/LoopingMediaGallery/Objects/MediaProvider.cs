using LoopingMediaGallery.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace LoopingMediaGallery.Objects
{
	public class MediaProvider : IMediaProvider
	{
		private readonly ISettingsProvider _settingsProvider;
		private readonly IIntervalTimer _intervalTimer;
		private readonly ILogger _logger;
	
		public MediaProvider(ISettingsProvider settingsProvider, IIntervalTimer intervalTimer, ILogger logger)
		{
			if (settingsProvider == null) throw new ArgumentNullException(nameof(settingsProvider));
			if (intervalTimer == null) throw new ArgumentNullException(nameof(intervalTimer));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			_settingsProvider = settingsProvider;
			_intervalTimer = intervalTimer;
			_logger = logger;

			_settingsProvider.SettingsChanged += (s, o) =>
			{
				var settingName = (o as System.Configuration.SettingChangingEventArgs).SettingName;
				switch (settingName)
				{
					case nameof(_settingsProvider.FolderPath):
						App.Current.Dispatcher.BeginInvoke(new Action(() => ScanFolderPath()));
						InitializeTimer();
						break;
					case nameof(_settingsProvider.RefreshRate):
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
			if (_settingsProvider.RefreshRate <= 0) return;
			_intervalTimer.Initialize(TimeSpan.FromMinutes(_settingsProvider.RefreshRate), ScanFolderPath);
			_intervalTimer.Start();
		}

		private void ScanFolderPath()
		{
			_logger.Write(string.Format("{0} - Starting to scan folder path.", this.GetType().Name));

			var currentCollectionDictionary = new Dictionary<string, IMediaObject>();
			foreach (var item in MediaObjectCollection)
			{
				currentCollectionDictionary.Add(item.Source.AbsolutePath, item);
			}
			var mediaCollection = new HashSet<IMediaObject>();

			if (!(string.IsNullOrEmpty(_settingsProvider.FolderPath) || string.IsNullOrWhiteSpace(_settingsProvider.FolderPath) || !Directory.Exists(_settingsProvider.FolderPath)))
			{
				string[] files = Directory.GetFiles(_settingsProvider.FolderPath);

				foreach (var file in files)
				{
					var ext = Path.GetExtension(file);

					if (_settingsProvider.ImageFormats.Contains(ext.ToLower()))
					{
						if (currentCollectionDictionary.ContainsKey(file))
							mediaCollection.Add(currentCollectionDictionary[file]);
						else
							mediaCollection.Add(new LocalImageObject(_settingsProvider, file));
					}

					if (_settingsProvider.VideoFormats.Contains(ext.ToLower()))
					{
						if (currentCollectionDictionary.ContainsKey(file))
							mediaCollection.Add(currentCollectionDictionary[file]);
						else
							mediaCollection.Add(new LocalVideoObject(file));
					}
				}

				bool mediaCollectionPopulated = MediaObjectCollection.Count == 0 && mediaCollection.Count > 0;
				bool mediaCollectionChanged = !MediaObjectCollection.SetEquals(mediaCollection);
					
				MediaObjectCollection = mediaCollection;

				if (mediaCollectionPopulated)
					MediaCollectionPopulated?.Invoke(this, new EventArgs());

				if (mediaCollectionChanged)
					MediaCollectionChanged?.Invoke(this, new EventArgs());

				_logger.Write(string.Format("{0} - Finished scanning folder path.", this.GetType().Name));
			}
		}

		public ISet<IMediaObject> MediaObjectCollection { get; internal set; }

		public void ForceUpdate() => ScanFolderPath();

		public event EventHandler MediaCollectionPopulated;

		public event EventHandler MediaCollectionChanged;
	}
}
