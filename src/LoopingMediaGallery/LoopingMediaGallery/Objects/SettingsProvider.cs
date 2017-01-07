using LoopingMediaGallery.Interfaces;
using System.Collections.Generic;
using System;

namespace LoopingMediaGallery.Objects
{
	public class SettingsProvider : ISettingsProvider
	{
		public SettingsProvider()
		{
			Properties.Settings.Default.SettingChanging += (s, o) => SettingsChanged?.Invoke(s, o);
		}

		public ISet<string> ImageFormats => new HashSet<string> { ".jpg", ".png", ".bmp", ".jpeg", ".tiff" };
		
		public ISet<string> VideoFormats => new HashSet<string> { ".mp4", ".wmv" };

		public int Duration => (int?)Properties.Settings.Default["Duration"] ?? 10;

		public string FolderPath => (string)Properties.Settings.Default["FolderPath"] ?? string.Empty;

		public int RefreshRate => (int?)Properties.Settings.Default["RefreshRate"] ?? 1;

		public bool UseFade => (bool?)Properties.Settings.Default["UseFade"] ?? true;

		public bool ShowPreview => (bool?)Properties.Settings.Default["ShowPreview"] ?? true;

		public event EventHandler SettingsChanged;
	}
}
