using System;
using System.Collections.Generic;

namespace LoopingMediaGallery.Interfaces
{
	public interface ISettingsProvider
	{
		ISet<string> ImageFormats { get; }
		ISet<string> VideoFormats {get; }
		int Duration { get; }
		int FileRefreshRate { get; }
		string FileFolderPath { get; }
		event EventHandler SettingsChanged;
	}
}
