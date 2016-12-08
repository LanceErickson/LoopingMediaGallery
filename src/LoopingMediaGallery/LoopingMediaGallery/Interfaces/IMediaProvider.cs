using System;
using System.Collections.Generic;

namespace LoopingMediaGallery.Interfaces
{
	public interface IMediaProvider
	{
		IList<IMediaObject> MediaObjectCollection { get; }

		void ForceUpdate();

		event EventHandler MediaCollectionPopulated;
	}
}
