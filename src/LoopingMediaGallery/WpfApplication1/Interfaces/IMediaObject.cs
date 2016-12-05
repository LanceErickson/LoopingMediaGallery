using System;

namespace LoopingMediaGallery.Interfaces
{
	public enum MediaType
	{
		Image,
		Video
	}

	public interface IMediaObject
	{
		Uri Source { get; }

		MediaType Type { get; }

		TimeSpan Duration { get; }
	}
}
