using LoopingMediaGallery.Interfaces;
using MediaInfoDotNet;
using System;
using System.IO;

namespace LoopingMediaGallery.Objects
{
	public class LocalVideoObject : IMediaObject
	{
		public LocalVideoObject(string source)
		{
			if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));
			if (!File.Exists(source)) throw new ArgumentException(nameof(source)); 

			Source = new Uri(source);

			var info = new MediaFile(source);
			if (info == null) throw new ArgumentException(nameof(source));

			Duration = TimeSpan.FromMilliseconds(info.General.Duration);
		}

		public TimeSpan Duration { get; }

		public Uri Source { get; }

		public MediaType Type => MediaType.Video;

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
