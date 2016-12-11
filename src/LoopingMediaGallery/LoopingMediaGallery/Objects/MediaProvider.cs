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
	
		public MediaProvider(ISettingsProvider settingsProvider, IIntervalTimer intervalTimer)
		{
			if (settingsProvider == null) throw new ArgumentNullException(nameof(settingsProvider));
			if (intervalTimer == null) throw new ArgumentNullException(nameof(intervalTimer));

			_settingsProvider = settingsProvider;
			_intervalTimer = intervalTimer;

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
			Console.WriteLine("MediaProvider - Starting to scan folder path.");

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

				if (MediaObjectCollection.Count == 0 && mediaCollection.Count > 0)
					MediaCollectionPopulated?.Invoke(this, new EventArgs());

				if (!MediaObjectCollection.SetEquals(mediaCollection))
					MediaCollectionChanged?.Invoke(this, new EventArgs());

				MediaObjectCollection = mediaCollection;

				Console.WriteLine("MediaProvider - Finished scanning folder path.");
			}
		}

		public ISet<IMediaObject> MediaObjectCollection { get; internal set; }

		public void ForceUpdate() => ScanFolderPath();

		public event EventHandler MediaCollectionPopulated;

		public event EventHandler MediaCollectionChanged;
	}
}
