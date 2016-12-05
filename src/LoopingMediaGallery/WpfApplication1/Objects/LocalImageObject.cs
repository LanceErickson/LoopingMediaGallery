using LoopingMediaGallery.Interfaces;
using System;
using System.IO;

namespace LoopingMediaGallery.Objects
{
	class LocalImageObject : IMediaObject
	{
		private ISettingsProvider _settingsProvider;

		public LocalImageObject(ISettingsProvider settingsProvider, string source)
		{
			if (settingsProvider == null) throw new ArgumentNullException(nameof(settingsProvider));
			if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));
			if (!File.Exists(source)) throw new ArgumentException(nameof(source));

			Source = new Uri(source);
 		}

		public TimeSpan Duration => TimeSpan.FromSeconds(_settingsProvider.Duration);

		public Uri Source { get; }

		public MediaType Type => MediaType.Image;
	}
}
