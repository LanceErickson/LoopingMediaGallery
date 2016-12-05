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

			MediaObjectCollection = new List<IMediaObject>();

			_fileRefreshTimer = new Timer(ScanFolderPath, new System.Threading.AutoResetEvent(false), (int)TimeSpan.FromMinutes(_settingsProvider.FileRefreshRate).TotalMilliseconds, (int)TimeSpan.FromMinutes(_settingsProvider.FileRefreshRate).TotalMilliseconds);
		}

		private void ScanFolderPath(object state)
		{
			if (string.IsNullOrEmpty(_settingsProvider.FileFolderPath) || string.IsNullOrWhiteSpace(_settingsProvider.FileFolderPath) || !Directory.Exists(_settingsProvider.FileFolderPath))
				return;

			string[] files = Directory.GetFiles(_settingsProvider.FileFolderPath);

			var mediaCollection = new List<IMediaObject>();

			foreach (var file in files)
			{
				var ext = Path.GetExtension(file);

				if (_settingsProvider.ImageFormats.Contains(ext.ToLower()))
					mediaCollection.Add(new LocalImageObject(_settingsProvider, file));

				if (_settingsProvider.VideoFormats.Contains(ext.ToLower()))
					mediaCollection.Add(new LocalVideoObject(file));
			}

			MediaObjectCollection = mediaCollection;			
		}

		public IList<IMediaObject> MediaObjectCollection { get; internal set; }

		public void ForceUpdate() => ScanFolderPath(null);
	}
}
