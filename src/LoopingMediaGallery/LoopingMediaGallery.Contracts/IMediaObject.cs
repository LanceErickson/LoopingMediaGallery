using System;

namespace LoopingMediaGallery.Interfaces
{
	public enum MediaType
	{
		Image,
		Video
	}

	public interface IMediaObject : IComparable<IMediaObject>, IEquatable<IMediaObject> 
	{
		Uri Source { get; }

		MediaType Type { get; }

		TimeSpan Duration { get; }
	}
}
