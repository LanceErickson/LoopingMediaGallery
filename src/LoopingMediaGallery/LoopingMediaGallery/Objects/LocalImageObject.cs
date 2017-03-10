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
            if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));
			if (!File.Exists(source)) throw new ArgumentException(nameof(source));

			_settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));

			Source = new Uri(source);
 		}

		public TimeSpan Duration => TimeSpan.FromSeconds(_settingsProvider.Duration);

		public Uri Source { get; }

		public MediaType Type => MediaType.Image;

		public int CompareTo(IMediaObject obj)
		{
			if (Type != obj.Type) return 0;
			if (!Source.AbsolutePath.Equals(obj.Source.AbsolutePath)) return 0;
			if (!Duration.Equals(obj.Duration)) return 0;
			return 1;
		}

		public bool Equals(IMediaObject other)
			=> CompareTo(other) == 1;

		public override bool Equals(object obj)
			=> obj is IMediaObject ? CompareTo((IMediaObject)obj) == 1 : false;

		public override int GetHashCode()
			=> Source.GetHashCode();
	}
}
