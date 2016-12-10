using System;
using System.Collections.Generic;

namespace LoopingMediaGallery.Interfaces
{
	public interface IMediaProvider
	{
		ISet<IMediaObject> MediaObjectCollection { get; }

		void ForceUpdate();

		event EventHandler MediaCollectionChanged;

		event EventHandler MediaCollectionPopulated;
	}
}
