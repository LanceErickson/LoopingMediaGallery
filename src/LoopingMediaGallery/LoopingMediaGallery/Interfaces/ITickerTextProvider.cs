using System;
using System.Collections.Generic;

namespace LoopingMediaGallery.Interfaces
{
	public interface ITickerTextProvider
	{
		IReadOnlyList<string> TextCollection { get; }
		void AddText(string value);
		void Remove(int index);
		event EventHandler TextCollectionChanged;
	}
}
