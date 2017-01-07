using System;
using System.Collections.Generic;

namespace LoopingMediaGallery.Interfaces
{
	public interface ISettingsProvider
	{
		ISet<string> ImageFormats { get; }
		ISet<string> VideoFormats {get; }
		int Duration { get; }
		int RefreshRate { get; }
		string FolderPath { get; }
		bool UseFade { get; }
		bool ShowPreview { get; }
		event EventHandler SettingsChanged;
	}
}
