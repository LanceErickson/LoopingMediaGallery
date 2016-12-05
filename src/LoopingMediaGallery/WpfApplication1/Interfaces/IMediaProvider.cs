using System.Collections.Generic;

namespace LoopingMediaGallery.Interfaces
{
	public interface IMediaProvider
	{
		IEnumerable<IMediaObject> MediaObjectCollection { get; }

		void ForceUpdate();
	}
}
